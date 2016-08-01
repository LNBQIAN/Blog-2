using MyBlog.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.DAL
{
    public partial class BaseDal<T> where T : class, new()
    {
        DbContext dbContext = new MyBlogEntities();
        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Add(T t)
        {
            dbContext.Set<T>().Add(t);
            return dbContext.SaveChanges() > 0;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Delete(T t)
        {
            dbContext.Set<T>().Remove(t);
            return dbContext.SaveChanges() > 0;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Update(T t)
        {
            dbContext.Set<T>().AddOrUpdate(t);
            return dbContext.SaveChanges() > 0;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetModels(Expression<Func<T, bool>> WhereLambda)
        {
            return dbContext.Set<T>().Where(WhereLambda);
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="type"></typeparam>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="isAsc">是否升序</param>
        /// <param name="OrderByLambda"></param>
        /// <param name="WhereLambda"></param>
        /// <returns></returns>
        public IQueryable<T> GetModelsByPage<type>(int pageSize, int pageIndex, bool isAsc, Expression<Func<T, type>> OrderByLambda, Expression<Func<T, bool>> WhereLambda)
        {
            if (isAsc)
            {
                return dbContext.Set<T>().Where(WhereLambda).OrderBy(OrderByLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            else
            {
                return dbContext.Set<T>().Where(WhereLambda).OrderByDescending(OrderByLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
        }
    }
}
