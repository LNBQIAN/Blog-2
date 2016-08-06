using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Common
{
    public class PageBar
    {
        /// <summary>
        /// 获取分页字符串(样式属于妹子UI)
        /// </summary>
        /// <param name="pageCount">总页数</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public static string GetNumberPageBar(int pageCount, int pageIndex)
        {
            //« 1 2 3 4 5 »   li
            StringBuilder sb = new StringBuilder();

            //后退一页
            //当前页<=1 不加超链接 不能点击
            if (pageIndex <= 1)
            {
                sb.AppendFormat("<li  class='am-disabled'><a  href='javascript:void(0);'>«</a></li>");
            }
            else
            {
                sb.AppendFormat("<li><a pageIndex='{0}' href='javascript:void(0);'>«</a></li>", pageIndex - 1);
            }
            //总页数<=5
            if (pageCount <= 5)
            {
                for (int i = 1; i <= pageCount; i++)
                {
                    //i=当前页
                    if (i == pageIndex)
                    {
                        //当前页不加超链接 不能点击
                        sb.AppendFormat("<li class='am-active'><a  href='javascript:void(0);'>{0}</a></li>", i);
                    }
                    else
                    {
                        sb.AppendFormat("<li><a  pageIndex='{0}'  href='javascript:void(0);'>{0}</a></li>",  i);
                    }
                }
            }
            //To Do: 大于5 或者其他..... 
            //前进一页
            //当前页>=总页数 不加超链接 不能点击
            if (pageIndex >= pageCount)
            {
                sb.AppendFormat("<li class='am-disabled'><a  href='javascript:void(0);'>»</a></li>");
            }
            else
            {
                sb.AppendFormat("<li><a pageIndex='{0}' href='javascript:void(0);'>»</a></li>",pageIndex+1);
            }

            return sb.ToString();
        }
    }
}
