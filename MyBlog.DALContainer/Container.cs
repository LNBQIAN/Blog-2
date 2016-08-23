using Autofac;
using MyBlog.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBlog.IDAL;

namespace MyBlog.DALContainer
{
    public class Container
    {
        /// <summary>
        /// IOC 容器
        /// </summary>
        public static IContainer container = null;
        /// <summary>
        /// 获取 IDal 的实例化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            try
            {
                if (container == null)
                {
                    Initialise();
                }
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("IOC实例化出错!" + ex.Message);
            }

            return container.Resolve<T>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialise()
        {
            var builder = new ContainerBuilder();
            //格式：builder.RegisterType<xxxx>().As<Ixxxx>().InstancePerLifetimeScope();
          
            builder.RegisterType<UserInfoDal>().As<IUserInfoDal>().InstancePerLifetimeScope();
            builder.RegisterType<ArticleTypeDal>().As<IArticleTypeDal>().InstancePerLifetimeScope();
            builder.RegisterType<ArticleInfoDal>().As<IArticleInfoDal>().InstancePerLifetimeScope();
            container = builder.Build();
        }
    }
}
