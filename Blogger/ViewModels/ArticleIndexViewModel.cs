using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Blogger.Models;
using Blogger.Services;

namespace Blogger.ViewModels
{
    //文章列表用ViewModel
    public class ArticleIndexViewModel
    {
        //搜尋欄位
        [DisplayName("搜尋：")]
        public string Search { get; set; }
        //顯示資料陣列
        public List<Article> DataList { get; set; }
        //分頁內容
        public ForPaging Paging { get; set; }
        //文章列表的帳號
        public string Account { get; set; }
    }

}