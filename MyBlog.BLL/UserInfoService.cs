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
    public partial class UserInfoService : BaseService<UserInfo>, IUserInfoService
    {
        public IUserInfoDal UserInfoDal = DALContainer.Container.Resolve<IUserInfoDal>();
        public IArticleInfoDal ArticleInfoDal= DALContainer.Container.Resolve<IArticleInfoDal>();
        public override void GetDal()
        {
            Dal = UserInfoDal;
        }

        public bool DeleteList(List<int> idList)
        {
            List<UserInfo> userInfos = UserInfoDal.GetModels(p => idList.Contains(p.Id)).ToList();
            if (userInfos.Count > 0)
            {
                foreach (var u in userInfos)
                {
                    //先删除该用户所有文章
                    foreach (var articleInfo in u.ArticleInfo.ToList())
                    {
                        ArticleInfoDal.Delete(articleInfo);
                    }
                    
                    UserInfoDal.Delete(u);
                }
                return Dal.SaveChanges();
            }
            else
            {
                return false;
            }
        }
    }
}
