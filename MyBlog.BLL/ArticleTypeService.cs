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
    public partial class ArticleTypeService : BaseService<ArticleType>, IArticleTypeService
    {
        public IArticleTypeDal ArticleTypeDal = DALContainer.Container.Resolve<IArticleTypeDal>();
        public IArticleInfoDal ArticleInfoDal = DALContainer.Container.Resolve<IArticleInfoDal>();

        public override void GetDal()
        {
            Dal = ArticleTypeDal;
        }
        public bool DeleteArticleType(int id)
        {
            var articleType = Dal.GetModels(p => p.Id == id).FirstOrDefault();
            //先删除该分类下所有的文章
            foreach (var article in articleType.ArticleInfo.ToList())
            {
                ArticleInfoDal.Delete(article);
            }
            //articleType.ArticleInfo.Clear();
            ArticleTypeDal.Delete(articleType);
            return Dal.SaveChanges();
        }

    }
}
