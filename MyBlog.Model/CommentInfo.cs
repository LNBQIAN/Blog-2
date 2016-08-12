//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyBlog.Model
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    
    public partial class CommentInfo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ArticleId { get; set; }
        public string CommentContent { get; set; }
        public System.DateTime CommentTime { get; set; }
        public int ParentId { get; set; }
    
    	[JsonIgnore]
        public virtual ArticleInfo ArticleInfo { get; set; }
    	[JsonIgnore]
        public virtual UserInfo UserInfo { get; set; }
    }
}
