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

    public  partial class UserInfoService:BaseService<UserInfo>,IUserInfoService
    {
        public override void GetDal()
        {
            Dal = DALContainer.Container.Resolve<IUserInfoDal>();
        }
    }
}
