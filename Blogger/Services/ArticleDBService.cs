using Blogger.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Blogger.Services
{
    public class ArticleDBService
    {
        //建立與資料庫的連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        //建立與資料庫連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 查詢一筆資料
        //藉由編號取得單筆資料的方法
        public Article GetArticleDataById(int A_Id)
        {
            Article Data = new Article();
            //sql語法

            string sql = $@" SELECT m.*, d.Name, d.Image FROM Article m INNER JOIN Members d ON m.Account = d.Account WHERE m.A_Id = {A_Id}";

            //確保城市部會因為執行錯誤而中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                Data.Account = dr["Account"].ToString();
                Data.Title = dr["Title"].ToString();
                Data.Content = dr["Content"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                Data.Watch = Convert.ToInt32(dr["Watch"]);
                Data.Member.Name = dr["Name"].ToString();
                Data.Member.Image = dr["Image"].ToString();
            }
            catch (Exception e)
            {
                //沒有資料回傳null
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return Data;
        }


        #endregion

        #region 查詢陣列資料
        //根據搜尋來取得陣列的方法
        public List<Article> GetDataList(ForPaging Paging, string Search,string Account)
        {
            //宣告要接受全部搜尋資料的物件
            List<Article> DataList = new List<Article>();
            //sql語法
            if (!string.IsNullOrWhiteSpace(Search))
            {
                //有搜尋到條件時
                SetMaxPaging(Paging, Search,Account);
                DataList = GetAllDataList(Paging, Search,Account);
            }
            else
            {
                //無收尋條件時
                SetMaxPaging(Paging,Account);
                DataList = GetAllDataList(Paging,Account);
            }
            return DataList;
        }

        //無搜尋值得搜尋資料方法
        public List<Article> GetAllDataList(ForPaging paging, string Account)
        {
            //宣告要回傳的搜尋資料為資料庫中的Article 資料表
            List<Article> DataList = new List<Article>();
            //sql語法
            string sql = $@" SELECT m.*, d.Name FROM (SELECT row_number() OVER (ORDER BY A_Id) AS sort,* FROM Article 
                            WHERE Account = '{Account}') 
                            m INNER JOIN Members d ON m.Account = d.Account WHERE m.sort BETWEEN {(paging.NowPage - 1) * paging.ItemNum + 1 } AND {paging.NowPage * paging.ItemNum}; ";
            //確保城市部會因執行錯誤而整個中斷
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())// 獲得下一筆資料直到沒有資料
                {
                    Article Data = new Article();
                    Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                    Data.Title = dr["Title"].ToString();
                    Data.Account = dr["Account"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    Data.Member.Name = dr["Name"].ToString();
                    Data.Watch = Convert.ToInt32(dr["Watch"]);
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return DataList;

        }

        //有搜尋值的搜尋資料方法
        public List<Article> GetAllDataList(ForPaging paging, string Search,string Account)
        {
            //宣告要回傳的搜尋資料為資料庫中的Article資料表
            List<Article> DataList = new List<Article>();
            //sql語法
            string sql = $@"SELECT m.* ,d.Name FROM (SELECT row_number() OVER (ORDER BY A_Id) AS sort,* FROM Article WHERE (Title LIKE '%{Search}%'
                          OR Content LIKE '%{Search}%') AND Account ='{Account}' ) 
                            m INNER JOIN Members d ON m.Account = d.Account WHERE m.sort BETWEEN {(paging.NowPage - 1) * paging.ItemNum + 1} AND {paging.NowPage * paging.ItemNum}; ";
            //確保城市部會因為執行錯誤而終段
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Article Data = new Article();
                    Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                    Data.Title = dr["Title"].ToString();
                    Data.Account = dr["Account"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    Data.Member.Name = dr["Name"].ToString();
                    Data.Watch = Convert.ToInt32(dr["Watch"]);
                    DataList.Add(Data);
                }               
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();

            }
            return DataList;

        }

        #region 設定最大頁數方法
        //無搜尋值的設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging,string Account)
        {
            //計算列數
            int Row = 0;
            string sql = $@"SELECT * FROM Article WHERE Account = '{Account}'; ";
            //確保城市部會因執行錯誤而中斷
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Row++;
                }
            }
            catch (Exception e )
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            //重心設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();
        }

        //有搜尋值的設定最大頁數方式
        public void SetMaxPaging (ForPaging Paging, string Search,string Account)
        {
            int Row = 0;
            string sql = $@" SELECT * FROM Article WHERE ( Title LIKE '%{Search}%' OR Content LIKE '%{Search}%' ) AND Account = '{Account}';";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Row++;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            Paging.SetRightPage();
        }
        #endregion

        #endregion

        #region 新增文章
        //新增資料方法
        public void InsertArticle(Article newData)
        {
            //取得最新一筆A_Id
            newData.A_Id = LastArticleFinder();
            //sql新增語法
            //設定新增時間為現在
            string sql = $@" INSERT INTO Article (A_Id,Title,Content,Account,CreateTime,Watch)
                         VALUES ( {newData.A_Id},'{newData.Title}','{newData.Content}','{newData.Account}','{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}',0 )";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                //用法：ExecuteNonQuery用來執行INSERT、UPDATE、DELETE和其他沒有返回值得SQL命令
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion

        #region 計算目前文章最新一筆A_Id
        public int LastArticleFinder()
        {
            //宣告要回傳的值
            int Id;
            string sql = $@" SELECT TOP 1 * FROM article ORDER BY A_Id DESC; ";
            try
            {

                conn.Open();
                //執行SQL指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                //取得SQL資料(先執行找到並排序後，在取得該資料)
                //用法：ExecuteReader 方法存在的目的只有一個：盡可能快地對資料庫進行查詢並得到結果
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Id = Convert.ToInt32(dr["A_Id"]);
                

            }
            catch (Exception e)
            {
                //沒資料時Id 為0
                Id = 0;
            }
            finally
            {
                conn.Close();
            }
            return Id+1;

        }

        #endregion

        #region 修改文章
        public void UpdateArticle(Article UpdateData)
        {
            //sql修改語法
            string sql = $@" UPDATE Article SET Content = '{UpdateData.Content}' WHERE A_Id = {UpdateData.A_Id} ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion

        #region  刪除文章
        //刪除文章方法
        public void DeleteArticle(int A_Id)
        {
            //必須先將該文章的留言刪除
            string DeleteMessage = $@" DELETE FROM Message WHERE A_Id = {A_Id}; ";
            //再根據文章Id取得要刪除的文章
            string DeleteArticle = $@" DELETE FROM Article WHERE A_Id = {A_Id}; ";
            //將兩段Sql語法一起放入SQL執行，能避免一值開啟資料庫連線，降低資料庫負擔
            string CombineSql = DeleteMessage + DeleteArticle;
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(CombineSql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

        }

        #endregion

        #region 修改文章檢查方法
        public bool CheckUpdate(int A_Id)
        {
            //根據Id取得要修改的資料
            Article Data = GetArticleDataById(A_Id);
            //抓取文章內的留言
            //留言比數
            int MessageCount = 0;
            string sql = $@" SELECT * FROM Message WHERE A_Id = {A_Id}; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                //抓取文章內的留言，若有文章，記數加一，最後結果回傳False(代表有回覆，無法修改文章)
                while (dr.Read())
                {
                    MessageCount++;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            //判斷並回傳(判斷是否有資料及是否有回復)
            return (Data != null && MessageCount == 0);
        }

        #endregion

        #region 人氣查詢
        public List<Article> GetPopularList(string Account)
        {
            List<Article> popularList = new List<Article>();
            //查詢Top5 watch
            string sql = $@" SELECT TOP 5 * FROM Article m INNER JOIN Members d ON m.Account = d.Account WHERE m.Account = '{Account}' ORDER BY watch DESC; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Article Data = new Article();
                    Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                    Data.Account = dr["Account"].ToString();
                    Data.Title = dr["Title"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    Data.Watch = Convert.ToInt32(dr["Watch"]);
                    Data.Member.Name = dr["Name"].ToString();
                    popularList.Add(Data);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return popularList;
        }
        #endregion

        #region 增加觀看人數
        public void AddWatch(int A_Id)
        {
            string sql = $@" UPDATE Article SET Watch = Watch + 1 WHERE A_Id = '{A_Id}'; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }



        #endregion
    }
}