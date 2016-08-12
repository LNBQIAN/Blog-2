using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBlog.IDAL;
using MyBlog.Model;

namespace MyBlog.DAL
{
    public partial class ArticleInfoDal : BaseDal<ArticleInfo>, IArticleInfoDal
    {
    }
}
