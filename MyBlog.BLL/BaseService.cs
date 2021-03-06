﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyBlog.IDAL;

namespace MyBlog.BLL
{
    public abstract partial class BaseService<T> where T : class, new()
    {
        public BaseService()
        {
            GetDal();
        }
        /// <summary>
        /// 具体执行数据操作的实例
        /// </summary>
        public IBaseDal<T> Dal { get; set; }

        /// <summary>
        /// 由子类重写该方法,为Dal赋值
        /// </summary>
        public abstract void GetDal();

       
        
        #region 增删改查
        public bool Add(T t)
        {
            Dal.Add(t);
            return Dal.SaveChanges();
        }
        public bool Delete(T t)
        {
            Dal.Delete(t);
            return Dal.SaveChanges();
        }
        public bool Update(T t)
        {
            Dal.Update(t);
            return Dal.SaveChanges();
        }
        public IQueryable<T> GetModels(Expression<Func<T, bool>> WhereLambda)
        {
            return Dal.GetModels(WhereLambda);
        }
        public IQueryable<T> GetModelsByPage<type>(int pageSize, int pageIndex, bool isAsc, Expression<Func<T, type>> OrderByLambda, Expression<Func<T, bool>> WhereLambda)
        {
            return Dal.GetModelsByPage(pageSize, pageIndex, isAsc, OrderByLambda, WhereLambda);
        }

        public int GetRecord(string tableName)
        {
            return Dal.GetRecord(tableName);
        }
        #endregion
    }
}
