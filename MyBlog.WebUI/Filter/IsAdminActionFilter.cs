using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyBlog.Model;
using MyBlog.IBLL;

namespace MyBlog.WebUI.Filter
{
    /// <summary>
    /// 登陆验证，管理员验证
    /// </summary>
    public class IsAdminActionFilter : ActionFilterAttribute
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
                UserInfo u= UserInfoService.GetModels(p => p.UName == uName).FirstOrDefault();
                if (u != null)
                {
                    //密码正确
                    if (uPwd.Equals(u.UPwd))
                    {
                        filterContext.HttpContext.Session["UserInfo"] = u;
                    }
                }
            }
            
            //没有登陆
            if (filterContext.HttpContext.Session["UserInfo"] == null)
            {
                filterContext.Result = new RedirectResult(Url.Action("Login", "UserInfo"));
            }
            //登陆了
            UserInfo userInfo = filterContext.HttpContext.Session["UserInfo"] as UserInfo;
            if (userInfo != null)
            {
                //判断是否拥有管理员权限
                if (userInfo.ULevel != (int)Model.Enum.UserInfoLevel.Admin)
                {
                    filterContext.Result = new RedirectResult(Url.Action("Index", "Home"));
                }
            }
            else
            {
                filterContext.Result = new RedirectResult(Url.Action("Login", "UserInfo"));
            }

        }
    }
}