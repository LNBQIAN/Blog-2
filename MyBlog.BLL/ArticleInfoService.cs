using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBlog.IBLL;
using MyBlog.IDAL;
using MyBlog.Model;

namespace MyBlog.BLL
{
    public partial class ArticleInfoService : BaseService<ArticleInfo>, IArticleInfoService
    {
        public IArticleInfoDal ArticleInfoDal = DALContainer.Container.Resolve<IArticleInfoDal>();
        public override void GetDal()
        {
            Dal = ArticleInfoDal;
        }

        public bool DeleteList(List<int> idList )
        {
            List<ArticleInfo> articleInfoList = ArticleInfoDal.GetModels(p => idList.Contains(p.Id)).ToList();
            if (articleInfoList.Count()>0)
            {
                foreach (var article in articleInfoList)
                {
                    ArticleInfoDal.Delete(article);   
                }
                return DbContext.SaveChanges();
            }
            else
            {
                return false;
            }
        }

    }
}
