using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyBlog.IBLL;
using MyBlog.Model;
using MyBlog.WebUI.Filter;

namespace MyBlog.WebUI.Controllers
{
    public class UserInfoController : Controller
    {
        private IUserInfoService UserInfoService = BLLContainer.Container.Resolve<IUserInfoService>();

        /// <summary>
        ///   后台用户管理页面，(登陆后,管理员等级才能访问)
        /// </summary>
        /// <returns></returns>
        [IsAdminActionFilter]
        public ActionResult Index()
        {
            return View();
        }

        #region 注册用户信息
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(UserInfo userInfo, string validateCode)
        {
            #region 判断验证码
            ViewBag.Msg = "";
            //验证码过期
            if (Session["validateCode"] == null)
            {
                ViewBag.Msg = "验证码已过期";
                return View();
            }
            //验证码不相等
            if (!Session["validateCode"].ToString().Equals(validateCode))
            {
                ViewBag.Msg = "验证码不正确";
                Session["validateCode"] = null;
                return View();
            }
            #endregion

            #region 验证码正确,数据加入数据库
            Session["validateCode"] = null;
            userInfo.UPwd = Common.MD5Helper.GetMD5String(userInfo.UPwd);// 密码MD5加密
            userInfo.Active = (int)Model.Enum.UserInfoActive.NoActive;//默认未激活状态
            userInfo.ULevel = (int)Model.Enum.UserInfoLevel.OrdinaryUser;//默认是普通用户
            userInfo.UHeadPhoto = "/UserPhoto/defaultPhoto.jpg";//默认头像
            userInfo.URegTime = DateTime.Now;
            string emailVCode = Guid.NewGuid().ToString();//验证用户是否激活需要的验证码
            userInfo.validateCode = emailVCode;
            //判断用户名，昵称，邮箱是否已经存在
            int uNameCount = UserInfoService.GetModels(p => p.UName == userInfo.UName).Count();
            if (uNameCount > 0)
            {
                ViewBag.Msg = "用户名已存在";
                return View();
            }
            int uNickNameCount = UserInfoService.GetModels(p => p.UNickName == userInfo.UNickName).Count();
            if (uNickNameCount > 0)
            {
                ViewBag.Msg = "昵称已存在";
                return View();
            }
            int emailCount = UserInfoService.GetModels(p => p.Email == userInfo.Email).Count();
            if (emailCount > 0)
            {
                ViewBag.Msg = "邮箱已存在";
                return View();
            }
            //加入数据库
            if (UserInfoService.Add(userInfo))
            {
                //注册成功
                Session["UserInfo"] = userInfo;
                return View("SendEmail");
            }
            else
            {
                ViewBag.Msg = "注册失败";
                return View();
            }
            #endregion
        }

        /// <summary>
        /// 发送邮件页面,（登陆后可以访问）
        /// </summary>
        /// <returns></returns>
        [IsLoginActionFilter]
        public ActionResult SendEmail()
        {
            return View();
        }


        /// <summary>
        /// 发送邮件（登陆后可以访问）
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [IsLoginActionFilter]
        public ActionResult SendEmail(string email)
        {
            //从数据库查找该邮箱，看看是否存在
            if (Session["UserInfo"] == null)
            {
                return Json(new { status = "no", msg = "还没有登录" }, JsonRequestBehavior.AllowGet);
            }
            UserInfo userInfo = Session["UserInfo"] as UserInfo;
            //判断当前用户是否已经激活
            if (userInfo.Active == (int)Model.Enum.UserInfoActive.Active)
            {
                return Json(new { status = "no", msg = "当前用户已激活" }, JsonRequestBehavior.AllowGet);
            }
            if (userInfo.Email == email)
            {
                //网站的域名
                string domainName = ConfigurationManager.AppSettings["domainName"];
                //验证邮箱的验证码
                string validateCode = userInfo.validateCode;
                System.Net.Mail.MailAddress from = new System.Net.Mail.MailAddress("656872652@qq.com", "凛凛蝶大人"); //填写电子邮件地址，和显示名称
                System.Net.Mail.MailAddress to = new System.Net.Mail.MailAddress(email, userInfo.UNickName); //填写邮件的收件人地址和名称
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.From = from;
                mail.To.Add(to);
                mail.Subject = "Please verify your email address.";
                System.Text.StringBuilder strBody = new System.Text.StringBuilder();
                strBody.Append("Hi " + userInfo.UNickName + "</br>");
                strBody.AppendFormat("Help me  verifying your email address address({0}).</br>", userInfo.Email);
                string url = domainName + Url.Action("Active", "UserInfo") + "?userId=" + userInfo.Id + "&validateCode=" + validateCode;
                strBody.AppendFormat("<a href='{0}'>Please click here!</a></br>", url);
                strBody.Append("Button not working? Paste the following link into your browser:</br>");
                strBody.AppendFormat("<a href='{0}'>{1}</a>", url, url);
                mail.Body = strBody.ToString();
                mail.IsBodyHtml = true;//设置显示htmls
                                       //设置好发送邮件服务地址
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                client.EnableSsl = true;
                client.Host = "smtp.qq.com";
                //填写服务器地址相关的用户名和密码信息
                client.Credentials = new System.Net.NetworkCredential("656872652@qq.com", "ipmxqpkycllnbcbd");
                //发送邮件
                client.Send(mail);
                return Json(new { status = "ok", msg = "邮件已发送" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "no", msg = "与注册时填写的邮箱不匹配" }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="validateCode"></param>
        /// <returns></returns>
        public ActionResult Active(int userId, string validateCode)
        {
            //验证码是否有效分为两种情况：1.验证是否在规定时间内激活链接；2.验证码和数据库中保存的是否相同
            var userInfo = UserInfoService.GetModels(p => p.Id == userId).FirstOrDefault();
            if (userInfo != null)
            {
                if (userInfo.Active == (int)Model.Enum.UserInfoActive.Active)
                {
                    return Json(new { status = "no", msg = "用户已激活" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //超过了 规定时间
                    if (DateTime.Now.Day - userInfo.URegTime.Day > 1)
                    {
                        UserInfoService.Delete(userInfo);
                        return Json(new { status = "no", msg = "未在规定时间内激活,请重新注册" }, JsonRequestBehavior.AllowGet);
                    }
                    //判断验证码是否相同
                    if (validateCode.Equals(userInfo.validateCode))
                    {
                        userInfo.Active = (int)Model.Enum.UserInfoActive.Active;
                        if (UserInfoService.Update(userInfo))
                        {
                            //激活成功
                            Session["UserInfo"] = null;
                            Session["UserInfo"] = userInfo;
                            return Redirect(Url.Action("Index", "Home"));
                        }
                        else
                        {
                            return Json(new { status = "no", msg = "激活失败" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { status = "no", msg = "激活失败" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                return Json(new { status = "no", msg = "当前用户不存在" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        
        #region 登陆
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string uName, string uPwd, string rememberMe, string validateCode)
        {
            //判断验证码
            if (Session["validateCode"] == null)
            {
                return Json(new { status = "no", msg = "验证码已过期" }, JsonRequestBehavior.AllowGet);
            }
            //验证码不相等
            if (!validateCode.Equals(Session["validateCode"].ToString()))
            {
                Session["validateCode"] = null;
                return Json(new { status = "no", msg = "验证码错误" }, JsonRequestBehavior.AllowGet);
            }
            Session["validateCode"] = null;
            //根据用户名判断用户是否存在
            UserInfo userInfo = UserInfoService.GetModels(p => p.UName == uName).FirstOrDefault();
            if (userInfo == null)
            {
                return Json(new { status = "no", msg = "用户名不存在" }, JsonRequestBehavior.AllowGet);
            }
            //用户名存在,判断密码
            uPwd = Common.MD5Helper.GetMD5String(uPwd);
            //密码不相等
            if (!uPwd.Equals(userInfo.UPwd))
            {
                return Json(new { status = "no", msg = "用户名或密码错误" }, JsonRequestBehavior.AllowGet);
            }
            //登陆成功
            Session["UserInfo"] = userInfo;
            //记住我
            if (!string.IsNullOrEmpty(rememberMe))
            {
                Response.Cookies["UName"].Value = userInfo.UName;
                Response.Cookies["UPwd"].Value = userInfo.UPwd;//已经是md5加密
                Response.Cookies["UName"].Expires = DateTime.Now.AddDays(3);
                Response.Cookies["UPwd"].Expires = DateTime.Now.AddDays(3);
            }
            return Json(new { status = "ok", msg = "登陆成功" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 退出
        public ActionResult Exit()
        {
            //清除session cookie
            Session["UserInfo"] = null;
            Response.Cookies["UName"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["UPwd"].Expires = DateTime.Now.AddDays(-1);
            return Redirect(Url.Action("Login","UserInfo"));
        }
        #endregion

        #region 获取验证码
        public ActionResult GetVCode()
        {
            string validateCode = Common.ValidateCode.CreateValidateCode(5);
            Session["validateCode"] = validateCode;
            return File(Common.ValidateCode.CreateValidateGraphic(validateCode), "image/jpeg");
        }
        #endregion
    }
}