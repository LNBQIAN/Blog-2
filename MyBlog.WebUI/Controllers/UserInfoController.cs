using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
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
            //验证码过期
            if (Session["validateCode"] == null)
            {
                return Json(new { status = "no", msg = "验证码已过期" }, JsonRequestBehavior.AllowGet);
            }
            //验证码不相等
            if (!Session["validateCode"].ToString().Equals(validateCode))
            {
                Session["validateCode"] = null;
                return Json(new { status = "no", msg = "验证码错误" }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region 验证码正确，验证参数
            //判断用户名
            if (!Regex.IsMatch(userInfo.UName, @"^[a-zA-Z][a-zA-Z0-9]{3,11}$"))
            {
                return Json(new { status = "no", msg = "用户名格式不正确" }, JsonRequestBehavior.AllowGet);
            }
            //判断密码
            if (!Regex.IsMatch(userInfo.UPwd, @"^[a-zA-Z]\w{5,15}$"))
            {
                return Json(new { status = "no", msg = "密码格式不正确" }, JsonRequestBehavior.AllowGet);
            }
            //判断昵称
            if (!Regex.IsMatch(userInfo.UNickName, @"^[a-zA-Z\u4e00-\u9fa5][a-zA-Z0-9_\u4E00-\u9FA5]{1,11}$"))
            {
                return Json(new { status = "no", msg = "昵称格式不正确" }, JsonRequestBehavior.AllowGet);
            }
            //判断邮箱
            if (!Regex.IsMatch(userInfo.Email, @"[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?"))
            {
                return Json(new { status = "no", msg = "邮箱格式不正确" }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region 数据加入数据库
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
                return Json(new { status = "no", msg = "用户名已存在" }, JsonRequestBehavior.AllowGet);
            }
            int uNickNameCount = UserInfoService.GetModels(p => p.UNickName == userInfo.UNickName).Count();
            if (uNickNameCount > 0)
            {
                return Json(new { status = "no", msg = "昵称已存在" }, JsonRequestBehavior.AllowGet);
            }
            int emailCount = UserInfoService.GetModels(p => p.Email == userInfo.Email).Count();
            if (emailCount > 0)
            {
                return Json(new { status = "no", msg = "邮箱已存在" }, JsonRequestBehavior.AllowGet);
            }
            //加入数据库
            if (UserInfoService.Add(userInfo))
            {
                //注册成功
                Session["UserInfo"] = userInfo;
                return Json(new { status = "ok", msg = "注册成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "no", msg = "注册失败" }, JsonRequestBehavior.AllowGet);
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
                //更新一下注册时间,因为注册的时候 会判断时间,如果用户不是注册后马上验证邮箱。可能是过了一天或者两天。那么就无法激活成功，按逻辑来说
                // 激活时间应该按照 验证发出去的时间 来判断
                userInfo.URegTime = DateTime.Now;
                UserInfoService.Update(userInfo);
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
                    if ((DateTime.Now - userInfo.URegTime).TotalDays >= 1)
                    {
                        //删除
                        UserInfoService.Delete(userInfo);
                        //如果已经登陆的话,清除session
                        if (Session["UserInfo"] != null)
                        {
                            //判断当前登录的用户，就是要激活的用户
                            if ((Session["UserInfo"] as UserInfo).Id == userInfo.Id)
                            {
                                //清除
                                Session["UserInfo"] = null;
                            }
                        }
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
            return Redirect(Url.Action("Login", "UserInfo"));
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

        #region 分页获取数据
        /// <summary>
        /// 获取用户数据 ，(登陆后,管理员等级才能访问)
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpPost]
        [IsAdminActionFilter]
        public ActionResult GetUserInfoList(int pageIndex)
        {
            //每页显示条数
            int pageSize = 1;
            //总记录数
            int totalCount = UserInfoService.GetRecord("UserInfo");
            //总页数
            int pageCount = Convert.ToInt32(Math.Ceiling(totalCount * 1.0 / pageSize));
            //确定pageIndex范围
            if (pageIndex <= 1)
            {
                pageIndex = 1;
            }
            if (pageIndex >= pageCount)
            {
                pageIndex = pageCount;
            }
            //查询当前页数据
            var userInfoList = UserInfoService.GetModelsByPage(pageSize, pageIndex, true, p => p.Id, p => true).Select(p => new
            {
                Id = p.Id,
                UName = p.UName,
                UNickName = p.UNickName,
                ULevel = p.ULevel == (int)Model.Enum.UserInfoLevel.Admin ? "管理员" : "普通用户",
                Active = p.Active == (int)Model.Enum.UserInfoActive.Active ? "已激活" : "未激活",
                URegTime = p.URegTime
            });
            //分页字符串
            string pageBar=Common.PageBar.GetNumberPageBar(pageCount,pageIndex);

            return Json(new { userInfoList=userInfoList,pageBar=pageBar },JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}