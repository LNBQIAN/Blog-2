using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyBlog.IBLL;
using MyBlog.Model;

namespace MyBlog.WebUI.Filter
{
    /// <summary>
    /// 登陆验证
    /// </summary>
    public class IsLoginActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var Url = new UrlHelper(filterContext.RequestContext);
            base.OnActionExecuting(filterContext);
            //判断有没有cookie，有的话，验证正确后可以登录
            if (filterContext.HttpContext.Request.Cookies["UName"] != null && filterContext.HttpContext.Request.Cookies["UPwd"] != null)
            {
                string uName = filterContext.HttpContext.Request.Cookies["UName"].Value;
                string uPwd = filterContext.HttpContext.Request.Cookies["UPwd"].Value;//存的时候已经是 MD5加密过后的
                IUserInfoService UserInfoService = BLLContainer.Container.Resolve<IUserInfoService>();
                //判断用户名和密码
                UserInfo userInfo = UserInfoService.GetModels(p => p.UName == uName).FirstOrDefault();
                if (userInfo != null)
                {
                    //密码正确
                    if (uPwd.Equals(userInfo.UPwd))
                    {
                        filterContext.HttpContext.Session["UserInfo"] = userInfo;
                    }
                }
            }

            //没有登陆
            if (filterContext.HttpContext.Session["UserInfo"] == null)
            {
                filterContext.Result = new RedirectResult(Url.Action("Login", "UserInfo"));
            }
        }
    }
}