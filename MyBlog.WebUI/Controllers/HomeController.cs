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
    }
}