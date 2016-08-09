using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyBlog.IBLL;
using MyBlog.Model;
using Newtonsoft.Json;

namespace MyBlog.WebUI.Controllers
{
    public class ArticleTypeController : Controller
    {
        private IArticleTypeService ArticleTypeService = BLLContainer.Container.Resolve<IArticleTypeService>();
        // GET: ArticleType
        public ActionResult Index()
        {
            var zNodes = ArticleTypeService.GetModels(p => true).Select(p=>new
            {
                id=p.Id,
                pId=p.ParentId,
                name =p.Title,
                open=true
            });
            ViewBag.zNodes = JsonConvert.SerializeObject(zNodes);
                //  { id: 1, pId: 0, name: "[core] 基本功能 演示", open: true},
            return View();
        }
    }
}