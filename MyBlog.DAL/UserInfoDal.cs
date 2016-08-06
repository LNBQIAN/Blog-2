using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MyBlog.IDAL;
using MyBlog.Model;


namespace MyBlog.DAL
{
    public partial class UserInfoDal : BaseDal<UserInfo>, IUserInfoDal
    {
      
    }
}
