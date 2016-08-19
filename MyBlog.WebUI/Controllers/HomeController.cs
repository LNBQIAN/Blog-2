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
using System.Text;

namespace MyBlog.WebUI.Controllers
{
    [CookieAutoLogin]
    public class HomeController : Controller
    {
        private IArticleInfoService ArticleInfoService = BLLContainer.Container.Resolve<IArticleInfoService>();
        private IUserInfoService UserInfoService = BLLContainer.Container.Resolve<IUserInfoService>();
        private IArticleTypeService ArticleTypeService = BLLContainer.Container.Resolve<IArticleTypeService>();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        #region 获取文章数据
        public ActionResult GetArticleList(int pageIndex)
        {
            //每页显示条数
            int pageSize = 7;
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
            //获取管理员用户
            ViewBag.UserInfo = UserInfoService.GetModels(p => p.UName == "admin").FirstOrDefault();
            //获取文章分类
            var zNodes = ArticleTypeService.GetModels(p => true).Select(p => new
            {
                id = p.Id,
                pId = p.ParentId,
                name = p.Title,
                open = true
            });
            ViewBag.zNodes = JsonConvert.SerializeObject(zNodes);
            return PartialView();
        }
        #endregion

        #region 搜索文章
        /// <summary>
        /// 搜索文章
        /// </summary>
        /// <param name="searchStr">搜索内容</param>
        /// <param name="type">搜索类型</param>
        /// <returns></returns>
        public ActionResult SearchArticle(string searchStr, string type, int pageIndex)
        {
            List<ArticleInfo> articleInfoList = null;
            //搜索条件是空，则显示首页
            if (string.IsNullOrEmpty(searchStr))
            {
                return View("Index");
            }
            //如果搜索类型是文章标题
            if (type.ToLower() == "articletitle")
            {
                articleInfoList = ArticleInfoService.GetModels(p => p.ArticleTitle.Contains(searchStr)).OrderByDescending(p => p.PubTime).ToList();
            }
            //如果搜索类型是文章分类
            if (type.ToLower() == "articletype")
            {
                int id = Convert.ToInt32(searchStr);
                var articleType = ArticleTypeService.GetModels(p => p.Id == id).FirstOrDefault();
                articleInfoList = articleType.ArticleInfo.ToList();
            }

            #region 分页参数范围确定
            //每页显示条数
            int pageSize = 7;
            //总记录数
            int totalCount = articleInfoList.Count;
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
            #endregion

            //分页取数据
            var list = articleInfoList.OrderByDescending(p => p.PubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize);

            #region 搜索分页字符串,非ajax,因为不是通用的。不封装在common里
            StringBuilder sb = new StringBuilder();
            //首页  已经是第一页,不能点击
            if (pageIndex == 1)
            {
                sb.AppendFormat("<li  class='am-disabled'><a  href='javascript:void(0);'>首页</a></li>");
            }
            else
            {
                sb.AppendFormat("<li><a href='?searchStr={0}&type={1}&pageIndex={2}'>首页</a></li>", searchStr, type, 1);
            }

            //后退一页
            //当前页<=1 不加超链接 不能点击
            if (pageIndex <= 1)
            {
                sb.AppendFormat("<li  class='am-disabled'><a  href='javascript:void(0);'>«</a></li>");
            }
            else
            {
                sb.AppendFormat("<li><a href='?searchStr={0}&type={1}&pageIndex={2}'>«</a></li>", searchStr, type, pageIndex - 1);
            }



            if (pageIndex + 4 <= pageCount)
            {
                int start = pageIndex - 2;
                int end = pageIndex + 2;
                if (start <= 1)
                {
                    start = 1;
                }
                if (pageIndex == 1)
                {
                    end = pageIndex + 4;
                }
                if (pageIndex == 2)
                {
                    end = pageIndex + 3;
                }
                for (int i = start; i <= end; i++)
                {
                    //i=当前页
                    if (i == pageIndex)
                    {
                        //当前页不加超链接 不能点击
                        sb.AppendFormat("<li class='am-active'><a  href='javascript:void(0);'>{0}</a></li>", i);
                    }
                    else
                    {
                        sb.AppendFormat("<li><a href='?searchStr={0}&type={1}&pageIndex={2}'>{2}</a></li>", searchStr,type,i);
                    }
                }
            }
            else
            {
                int start = pageCount - 4;
                int end = pageCount;
                if (start <= 1)
                {
                    start = 1;
                }
                for (int i = start; i <= end; i++)
                {
                    //i=当前页
                    if (i == pageIndex)
                    {
                        //当前页不加超链接 不能点击
                        sb.AppendFormat("<li class='am-active'><a  href='javascript:void(0);'>{0}</a></li>", i);
                    }
                    else
                    {
                        sb.AppendFormat("<li><a href='?searchStr={0}&type={1}&pageIndex={2}'>{2}</a></li>",searchStr,type ,i);
                    }
                }
            }

            //前进一页
            //当前页>=总页数 不加超链接 不能点击
            if (pageIndex >= pageCount)
            {
                sb.AppendFormat("<li class='am-disabled'><a  href='javascript:void(0);'>»</a></li>");
            }
            else
            {
                sb.AppendFormat("<li><a href='?searchStr={0}&type={1}&pageIndex={2}'>»</a></li>", searchStr,type,pageIndex + 1);
            }

            //尾页 已经是最后一页 不加超链接 不能点击
            if (pageIndex == pageCount)
            {
                sb.AppendFormat("<li class='am-disabled'><a  href='javascript:void(0);'>尾页</a></li>");
            }
            else
            {
                sb.AppendFormat("<li><a href='?searchStr={0}&type={1}&pageIndex={2}'>尾页</a></li>",searchStr,type, pageCount);
            }
            string pageBar = sb.ToString();
            #endregion

            ViewBag.pageBar = pageBar;
            return View(list.ToList());
        }
        #endregion

        #region 关于我
        public ActionResult AboutMe()
        {
            return View();
        }
        #endregion
    }
}



