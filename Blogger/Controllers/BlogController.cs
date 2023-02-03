using Blogger.Services;
using Blogger.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blogger.Controllers
{
    public class BlogController : Controller
    {
        //宣告Members資料表的Service物件
        private readonly MembersDBService membersService = new MembersDBService();
        // GET: Blog
        #region 部落格首頁
        public ActionResult Index(string Account)
        {
            BlogViewModel Data = new BlogViewModel();
            Data.Member = membersService.GetDatabyAccount(Account);
            return View(Data);
        }
        #endregion

    }
}