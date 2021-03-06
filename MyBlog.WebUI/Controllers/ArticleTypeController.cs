﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyBlog.IBLL;
using MyBlog.Model;
using MyBlog.Model.Enum;
using MyBlog.WebUI.Filter;
using Newtonsoft.Json;

namespace MyBlog.WebUI.Controllers
{
    [IsAdminActionFilter]//管理员才有权限对文章分类进行操作
    public class ArticleTypeController : Controller
    {
        private IArticleTypeService ArticleTypeService = BLLContainer.Container.Resolve<IArticleTypeService>();
        // GET: ArticleType

        public ActionResult Index()
        {
            //  { id: 1, pId: 0, name: "[core] 基本功能 演示", open: true},
            var zNodes = ArticleTypeService.GetModels(p => true).Select(p => new
            {
                id = p.Id,
                pId = p.ParentId,
                name = p.Title,
                open = true
            });
            ViewBag.zNodes = JsonConvert.SerializeObject(zNodes);
            return View();
        }

        #region 添加文章分类
        public ActionResult Add(ArticleType articleType)
        {
            if (ArticleTypeService.Add(articleType))
            {
                return Json(new { status = "ok", msg = "添加成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "no", msg = "添加失败" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 更新文章分类
        public ActionResult Update(int id)
        {
            ArticleType articleType = ArticleTypeService.GetModels(p => p.Id == id).FirstOrDefault();
            return Json(articleType, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Update(ArticleType articleType)
        {
            if (ArticleTypeService.Update(articleType))
            {
                return Json(new { status = "ok", msg = "修改成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "no", msg = "修改失败" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 删除文章分类
        public ActionResult Delete(int id)
        {
            if (ArticleTypeService.DeleteArticleType(id))
            {
                return Json(new { status = "ok", msg = "删除成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "no", msg = "删除失败" }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region 获取分类信息
        public ActionResult GetArticleType(int parentId)
        {
            var articleTypeList = ArticleTypeService.GetModels(p => p.ParentId == parentId).ToList();
            if (articleTypeList.Count <= 0)
            {
                return Content(JsonConvert.SerializeObject(new { status = "no", msg = "没有分类信息" }));
            }
            return Content(JsonConvert.SerializeObject(new { status = "ok", msg = "获取成功", articleTypeList = articleTypeList }));
        }
        #endregion
    }
}
