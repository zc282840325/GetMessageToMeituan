using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetMessageToMeituan
{
   public class Comment
    {
        public Comment(int Id,string userName, string comment, DateTime commentTime)
        {
            this.Id = Id;
            this.userName = userName;
            this.comment = comment;
            this.commentTime = commentTime;
        }
        [Description("编号")]
        public int Id{ get; set; }
        [Description("用户名")]
        public string userName { get; set; }
        [Description("评价内容")]
        public string comment { get; set; }
        [Description("评价时间")]
        public DateTime commentTime  { get; set; }
    }
}
