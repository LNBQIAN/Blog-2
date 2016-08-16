using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyBlog.Model;
using MyBlog.WebUI.Filter;

namespace MyBlog.WebUI.Controllers
{
    [CookieAutoLogin]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            if (Session["UserInfo"]!=null)
            {
                UserInfo userInfo=Session["UserInfo"]as UserInfo;
                ViewBag.UserInfo = userInfo;
            }
            return View();
        }
        #region 获取博客右边显示的内容(因为需要多次使用,所以是部分视图)
        public ActionResult GetRightView()
        {
            return PartialView();
        }
        #endregion
    }
}