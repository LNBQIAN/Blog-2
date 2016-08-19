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
        /// 获取分页字符串(样式属于妹子UI),有首页和尾页
        /// </summary>
        /// <param name="pageCount">总页数</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public static string GetNumberPageBarWithFirstIndexAndLastIndex(int pageCount, int pageIndex)
        {
            //« 1 2 3 4 5 »   
            StringBuilder sb = new StringBuilder();
            
            //首页  已经是第一页,不能点击
            if (pageIndex==1)
            {
                sb.AppendFormat("<li  class='am-disabled'><a  href='javascript:void(0);'>首页</a></li>");
            }
            else
            {
                sb.AppendFormat("<li><a pageIndex='{0}' href='javascript:void(0);'>首页</a></li>", 1);
            }

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


            if (pageIndex + 4 <= pageCount)
            {
                int start = pageIndex - 2;
                int end = pageIndex + 2;
                if (start <= 1)
                {
                    start = 1;
                }
                if (pageIndex == 1)
                {
                    end = pageIndex + 4;
                }
                if (pageIndex == 2)
                {
                    end = pageIndex + 3;
                }
                for (int i = start; i <= end; i++)
                {
                    //i=当前页
                    if (i == pageIndex)
                    {
                        //当前页不加超链接 不能点击
                        sb.AppendFormat("<li class='am-active'><a  href='javascript:void(0);'>{0}</a></li>", i);
                    }
                    else
                    {
                        sb.AppendFormat("<li><a  pageIndex='{0}'  href='javascript:void(0);'>{0}</a></li>", i);
                    }
                }
            }
            else
            {
                int start = pageCount - 4;
                int end = pageCount;
                if (start <= 1)
                {
                    start = 1;
                }
                for (int i = start; i <= end; i++)
                {
                    //i=当前页
                    if (i == pageIndex)
                    {
                        //当前页不加超链接 不能点击
                        sb.AppendFormat("<li class='am-active'><a  href='javascript:void(0);'>{0}</a></li>", i);
                    }
                    else
                    {
                        sb.AppendFormat("<li><a  pageIndex='{0}'  href='javascript:void(0);'>{0}</a></li>", i);
                    }
                }
            }

            //前进一页
            //当前页>=总页数 不加超链接 不能点击
            if (pageIndex >= pageCount)
            {
                sb.AppendFormat("<li class='am-disabled'><a  href='javascript:void(0);'>»</a></li>");
            }
            else
            {
                sb.AppendFormat("<li><a pageIndex='{0}' href='javascript:void(0);'>»</a></li>", pageIndex + 1);
            }

            //尾页 已经是最后一页 不加超链接 不能点击
            if (pageIndex==pageCount)
            {
                sb.AppendFormat("<li class='am-disabled'><a  href='javascript:void(0);'>尾页</a></li>");
            }
            else
            {
                sb.AppendFormat("<li><a pageIndex='{0}' href='javascript:void(0);'>尾页</a></li>", pageCount);
            }
            return sb.ToString();
        }
        /// <summary>
        ///  获取分页字符串(样式属于妹子UI)
        /// </summary>
        /// <param name="pageCount"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public static string GetNumberPageBar(int pageCount, int pageIndex)
        {
            //« 1 2 3 4 5 »   
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


            if (pageIndex + 4 <= pageCount)
            {
                int start = pageIndex - 2;
                int end = pageIndex + 2;
                if (start <= 1)
                {
                    start = 1;
                }
                if (pageIndex == 1)
                {
                    end = pageIndex + 4;
                }
                if (pageIndex == 2)
                {
                    end = pageIndex + 3;
                }
                for (int i = start; i <= end; i++)
                {
                    //i=当前页
                    if (i == pageIndex)
                    {
                        //当前页不加超链接 不能点击
                        sb.AppendFormat("<li class='am-active'><a  href='javascript:void(0);'>{0}</a></li>", i);
                    }
                    else
                    {
                        sb.AppendFormat("<li><a  pageIndex='{0}'  href='javascript:void(0);'>{0}</a></li>", i);
                    }
                }
            }
            else
            {
                int start = pageCount - 4;
                int end = pageCount;
                if (start <= 1)
                {
                    start = 1;
                }
                for (int i = start; i <= end; i++)
                {
                    //i=当前页
                    if (i == pageIndex)
                    {
                        //当前页不加超链接 不能点击
                        sb.AppendFormat("<li class='am-active'><a  href='javascript:void(0);'>{0}</a></li>", i);
                    }
                    else
                    {
                        sb.AppendFormat("<li><a  pageIndex='{0}'  href='javascript:void(0);'>{0}</a></li>", i);
                    }
                }
            }

            //前进一页
            //当前页>=总页数 不加超链接 不能点击
            if (pageIndex >= pageCount)
            {
                sb.AppendFormat("<li class='am-disabled'><a  href='javascript:void(0);'>»</a></li>");
            }
            else
            {
                sb.AppendFormat("<li><a pageIndex='{0}' href='javascript:void(0);'>»</a></li>", pageIndex + 1);
            }

        
            return sb.ToString();
        }


    }
}
