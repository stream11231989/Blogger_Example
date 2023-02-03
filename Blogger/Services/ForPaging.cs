using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blogger.Services
{
    //分業內容類別
    public class ForPaging
    {
        //當前頁數
        public int NowPage { get; set; }
        //最大頁數
        public int MaxPage { get; set; }
        //分頁項目個數，在此設為唯讀
        //以後要修改個數，只需修改此處
        public int ItemNum
        {
            get
            {
                return 5;
            }
        }
        //此類別建構式
        public ForPaging()
        {
            //預設當前頁數為1
            this.NowPage = 1;
        }
        //此類別建構式，含傳入頁數
        public ForPaging(int Page)
        {
            this.NowPage = Page;
        }
        //設定正確頁樹的方法，以免傳入部正確值
        public void SetRightPage()
        {
            //判斷當前頁數是否小於1
            if (this.NowPage < 1)
            {
                this.NowPage = 1;
            }
            //判斷當前頁數是否大於總頁數
            else if (this.NowPage > this.MaxPage)
            {
                //設定當前頁數為總頁數
                this.NowPage = this.MaxPage;
            }
            //將無資料時的當前頁數，重心返回1
            if (this.MaxPage.Equals(0))
            {
                this.NowPage = 1;
            }
        }
    }
}