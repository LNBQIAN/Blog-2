using MyBlog.IBLL;
using MyBlog.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyBlog.WebUI.Filter
{
    /// <summary>
    /// 不需要验证权限，不需要登陆,但有cookie的话则自动登陆
    /// </summary>
    public class CookieAutoLogin: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var Url = new UrlHelper(filterContext.RequestContext);
            //判断有没有cookie，有的话，验证正确后可以登录
            if (filterContext.HttpContext.Request.Cookies["UName"] != null && filterContext.HttpContext.Request.Cookies["UPwd"] != null)
            {
                string uName = filterContext.HttpContext.Request.Cookies["UName"].Value;
                string uPwd = filterContext.HttpContext.Request.Cookies["UPwd"].Value;//存的时候已经是 MD5加密过后的
                IUserInfoService UserInfoService = BLLContainer.Container.Resolve<IUserInfoService>();
                //判断用户名和密码
                UserInfo u = UserInfoService.GetModels(p => p.UName == uName).FirstOrDefault();
                if (u != null)
                {
                    //密码正确
                    if (uPwd.Equals(u.UPwd))
                    {
                        filterContext.HttpContext.Session["UserInfo"] = u;
                    }
                }
            }

        }
    }
}