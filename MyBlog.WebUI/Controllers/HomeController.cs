using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyBlog.IBLL;
using MyBlog.Model;
using MyBlog.WebUI.Filter;
using Newtonsoft.Json;
using MyBlog.Common;

namespace MyBlog.WebUI.Controllers
{
    [CookieAutoLogin]
    public class HomeController : Controller
    {
        private IArticleInfoService ArticleInfoService = BLLContainer.Container.Resolve<IArticleInfoService>();

        // GET: Home
        public ActionResult Index()
        {
            if (Session["UserInfo"] != null)
            {
                UserInfo userInfo = Session["UserInfo"] as UserInfo;
                ViewBag.UserInfo = userInfo;
            }
            return View();
        }

        #region 获取文章数据
        public ActionResult GetArticleList(int pageIndex)
        {
            //每页显示条数
            int pageSize = 2;
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
            var articleList = ArticleInfoService.GetModelsByPage(pageSize, pageIndex, false, p => p.PubTime, p => true).ToList().Select(p => new
            {
                Id = p.Id,
                ArticleTypeName = p.ArticleType.Title,
                Title = p.ArticleTitle,
                UserInfo = p.UserInfo,
                ReadCount = p.ReadCount,
                PubTime = p.PubTime.ToString(),
                ArticleContent = Common.MyHtml.StripHTML(p.ArticleContent),
                FacePhoto = p.FacePhoto
            });
            string pageBar = PageBar.GetNumberPageBarWithFirstIndexAndLastIndex(pageCount, pageIndex);

            return Content(JsonConvert.SerializeObject(new { articleList = articleList, pageBar = pageBar }));
        }
        #endregion

        #region 获取博客右边显示的内容(因为需要多次使用,所以是部分视图)
        public ActionResult GetRightView()
        {
            return PartialView();
        }
        #endregion

        #region 搜索文章
        public ActionResult SearchArticle(string searchStr)
        {
            //搜索条件是空，则显示首页
            if (string.IsNullOrEmpty(searchStr))
            {
                return View("Index");
            }
            //搜索条件不是空,则显示所有符合条件的文章数据
            else
            {
                List<ArticleInfo> articleInfoList = ArticleInfoService.GetModels(p => p.ArticleTitle.Contains(searchStr)).OrderByDescending(p=>p.PubTime).ToList();
                return View(articleInfoList);
            }
        }
        #endregion
    }
}



