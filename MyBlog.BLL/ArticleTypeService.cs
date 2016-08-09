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
        public override void GetDal()
        {
            Dal = ArticleTypeDal;
        }
    }
}
