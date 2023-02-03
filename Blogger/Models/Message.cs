using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Blogger.Models
{
    public class Message
    {
        //文章編號
        public int A_Id { get; set; }
        //留言編號
        public int M_Id { get; set; }
        [DisplayName("留言帳號:")]
        public string Account { get; set; }
        [DisplayName("留言內容:")]
        public string Content { get; set; }
        //留言時間
        [DisplayName("留言時間:")]
        public DateTime CreateTime { get; set; }
        //Members資料表(外接鍵)
        //預設時就將Members物件建立好
        public Members Member { get; set; } = new Members();

    }
}