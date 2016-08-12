using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBlog.Model;

namespace MyBlog.IBLL
{
    public partial interface IArticleInfoService : IBaseService<ArticleInfo>
    {
        bool DeleteList(List<int> idList);
    }
}
