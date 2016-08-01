using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using MyBlog.IDAL;
using MyBlog.Model;

namespace MyBlog.DAL
{
    /// <summary>
    /// EF上下文对象创建工厂
    /// </summary>
    public class DbContextFactory
    {
        /// <summary>
        /// EF上下文对象,已存在就直接取,不存在就创建,保证线程内是唯一。
        /// </summary>
        public static DbContext CreateDbContext()
        {
            DbContext dbContext = CallContext.GetData("dbContext") as DbContext;
            if (dbContext == null)
            {
                dbContext = new MyBlogEntities();
                CallContext.SetData("dbContext", dbContext);
            }
            return dbContext;
        }
    }
}
