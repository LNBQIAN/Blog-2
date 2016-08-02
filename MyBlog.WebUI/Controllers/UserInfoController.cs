using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyBlog.WebUI.Controllers
{
    public class UserInfoController : Controller
    {
        // GET: UserInfo
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Add()
        {
            return View();
        }

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