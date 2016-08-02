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
                    UserInfoDal.Delete(u);
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
