using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyBlog.IBLL;

namespace MyBlog.WebUI.Controllers
{
    public class UserInfoController : Controller
    {
        private IUserInfoService UserInfoService = BLLContainer.Container.Resolve<IUserInfoService>();
        // GET: UserInfo
        public ActionResult Index()
        {
            ViewBag.UserInfo = UserInfoService.GetModels(p => true).FirstOrDefault();
            return View();
        }
    }
}