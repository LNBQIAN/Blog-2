using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyBlog.Common;
using MyBlog.IBLL;
using MyBlog.Model;
using MyBlog.WebUI.Filter;
using Newtonsoft.Json;

namespace MyBlog.WebUI.Controllers
{

    public class ArticleInfoController : Controller
    {
        private IArticleInfoService ArticleInfoService = BLLContainer.Container.Resolve<IArticleInfoService>();
        // GET: ArticleInfo
        #region 后台管理文章首页（管理员访问）
        [IsAdminActionFilter]
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region 获取文章数据（管理员访问）
        [IsAdminActionFilter]
        public ActionResult GetArticleList(int pageIndex)
        {
            //每页显示条数
            int pageSize = 10;
            //总记录数
            int totalCount = ArticleInfoService.GetRecord("ArticleInfo");
            if (totalCount <= 0)
            {
                totalCount = 1;
            }
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
            var articleList = ArticleInfoService.GetModelsByPage(pageSize, pageIndex, false, p => p.Id, p => true).Select(p=>new
            {
                Id=p.Id,
                ArticleTypeName=p.ArticleType.Title,
                Title=p.ArticleTitle,
                UserInfo = p.UserInfo,
                ReadCount=p.ReadCount,
                PubTime=p.PubTime
            });
            string pageBar = PageBar.GetNumberPageBarWithFirstIndexAndLastIndex(pageCount, pageIndex);

            return Content(JsonConvert.SerializeObject(new {articleList = articleList, pageBar = pageBar}));
        }
        #endregion

        #region 删除(管理员访问)
        [IsAdminActionFilter]
        public ActionResult Delete(string idStr)
        {
            //idStr: 1,2,3,4,5
            string[] temp = idStr.Split(',');
            List<int> idList = temp.Select(p => Convert.ToInt32(p)).ToList();
            if (ArticleInfoService.DeleteList(idList))
            {
                return Json(new { status ="ok",msg="删除成功"}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "no", msg = "删除失败" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 添加（管理员访问）
        public ActionResult Add()
        {
            return View();
        }
        #endregion
    }
}