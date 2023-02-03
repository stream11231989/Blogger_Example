using Blogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blogger.ViewModels
{
    public class ArticleViewModel
    {

        //文章本體
        public Article article { get; set; }

        //顯示留言
        public List<Message> DataList { get; set; }
    }
}