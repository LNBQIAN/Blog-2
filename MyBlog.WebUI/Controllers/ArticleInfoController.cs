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
using System.Collections;
using System.IO;

namespace MyBlog.WebUI.Controllers
{

    public class ArticleInfoController : Controller
    {
        private IArticleInfoService ArticleInfoService = BLLContainer.Container.Resolve<IArticleInfoService>();
        private IArticleTypeService ArticleTypeService = BLLContainer.Container.Resolve<IArticleTypeService>();
        private IUserInfoService UserInfoService = BLLContainer.Container.Resolve<IUserInfoService>();
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
            var articleList = ArticleInfoService.GetModelsByPage(pageSize, pageIndex, false, p => p.Id, p => true).ToList().Select(p => new
            {
                Id = p.Id,
                ArticleTypeName = p.ArticleType.Title,
                Title = p.ArticleTitle,
                UserInfo = p.UserInfo,
                ReadCount = p.ReadCount,
                PubTime = p.PubTime.ToString()
            });
            string pageBar = PageBar.GetNumberPageBarWithFirstIndexAndLastIndex(pageCount, pageIndex);

            return Content(JsonConvert.SerializeObject(new { articleList = articleList, pageBar = pageBar }));
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
                return Json(new { status = "ok", msg = "删除成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "no", msg = "删除失败" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 添加（管理员访问）
        [IsAdminActionFilter]
        public ActionResult Add()
        {
            return View();
        }

        [IsAdminActionFilter]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(ArticleInfo articleInfo)
        {
            //获取当前登录用户id
            if (Session["UserInfo"] == null)
            {
                return Json(new { status = "no", msg = "未登录" }, JsonRequestBehavior.AllowGet);
            }
            articleInfo.UserId = (Session["UserInfo"] as UserInfo).Id;
            articleInfo.PubTime = DateTime.Now;
            articleInfo.FacePhoto = string.IsNullOrEmpty(articleInfo.FacePhoto) == true ? "/UserFile/defaultFace.jpg" : articleInfo.FacePhoto;

            //默认阅读次数为0
            articleInfo.ReadCount = 0;
            if (ArticleInfoService.Add(articleInfo))
            {
                return Json(new { status = "ok", msg = "添加成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "no", msg = "添加失败" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 修改(管理员访问)
        [IsAdminActionFilter]
        public ActionResult Edit(int id)
        {
            var articleInfo = ArticleInfoService.GetModels(p => p.Id == id).FirstOrDefault();
            ViewBag.articleTypeInfo = ArticleTypeService.GetModels(p => p.Id == articleInfo.ArticleTypeId).FirstOrDefault();
            ViewBag.userInfo = UserInfoService.GetModels(p => p.Id == articleInfo.UserId).FirstOrDefault();
            return View(articleInfo);
        }

        [IsAdminActionFilter]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ArticleInfo articleInfo)
        {
            articleInfo.PubTime = DateTime.Now;
            if (ArticleInfoService.Update(articleInfo))
            {
                return Json(new { status = "ok", msg = "修改成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "no", msg = "修改失败" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 上传文件(管理员访问)
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="imgFile"></param>
        /// <param name="dir">上传文件类型:image、flash、media、file</param>
        /// <returns></returns>
        public ActionResult Upload(HttpPostedFileBase imgFile, string dir)
        {
            //定义允许上传的文件扩展名
            Hashtable extTable = new Hashtable();
            extTable.Add("image", "gif,jpg,jpeg,png,bmp");
            extTable.Add("flash", "swf,flv");
            extTable.Add("media", "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb");
            extTable.Add("file", "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2");
            if (String.IsNullOrEmpty(dir))
            {
                dir = "image";
            }
            if (!extTable.ContainsKey(dir))
            {
                return Content(JsonConvert.SerializeObject(new {error = 1, message = "文件格式不正确"}));
                // return Json(new { error = 1, message = "文件格式不正确" }, JsonRequestBehavior.AllowGet);
            }

            if (imgFile == null)
            {
                return Content(JsonConvert.SerializeObject(new { error = 1, message = "上传文件大小超过限制" }));
                //return Json(new { error = 1, message = "上传文件大小超过限制" }, JsonRequestBehavior.AllowGet);
            }
            string fileName = imgFile.FileName;
            string fileExt = Path.GetExtension(fileName).ToLower();
            if (String.IsNullOrEmpty(fileExt) || Array.IndexOf(((String)extTable[dir]).Split(','), fileExt.Substring(1).ToLower()) == -1)
            {
                 return Content(JsonConvert.SerializeObject(new { error = 1, message = "上传文件扩展名是不允许的扩展名。\n只允许" + ((String)extTable[dir]) + "格式" }));
                //return Json(new { error = 1, message = "上传文件扩展名是不允许的扩展名。\n只允许" + ((String)extTable[dir]) + "格式" }, JsonRequestBehavior.AllowGet);
            }
            //创建文件夹
            //获取当前登录用户id
            if (Session["UserInfo"] == null)
            {
                return Content(JsonConvert.SerializeObject(new { error = 1, message = "未登录" }));
                //return Json(new { error = 1, message = "未登录" }, JsonRequestBehavior.AllowGet);
            }
            string dirPath = "/UserFile/用户编号" + (Session["UserInfo"] as UserInfo).Id + "/" + dir + "/";
            if (!Directory.Exists(Request.MapPath(dirPath)))
            {
                //不存在就创建
                Directory.CreateDirectory(Request.MapPath(dirPath));
            }
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff") + fileExt;
            imgFile.SaveAs(Request.MapPath(dirPath + newFileName));
            //判断保存的文件是否存在
            if (System.IO.File.Exists(Request.MapPath(dirPath + newFileName)))
            {
                return Content(JsonConvert.SerializeObject(new { error = 0, url = dirPath + newFileName }));
                //return Json(new { error = 0, url = dirPath + newFileName }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Content(JsonConvert.SerializeObject(new { error = 1, message = "上传文件失败" }));
               // return Json(new { error = 1, message = "上传文件失败" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 显示文章详细信息
        public ActionResult Detial(int id)
        {
            ArticleInfo articleInfo = ArticleInfoService.GetModels(p => p.Id == id).FirstOrDefault();
            if (articleInfo==null)
            {
                return Redirect(Url.Action("Index","Home"));
            }
            //每次访问阅读量+1
            articleInfo.ReadCount = articleInfo.ReadCount + 1;
            ArticleInfoService.Update(articleInfo);
            return View(articleInfo);
        }
        #endregion
    }
}