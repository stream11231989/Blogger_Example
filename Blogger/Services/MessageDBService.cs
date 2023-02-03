using Blogger.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace Blogger.Services
{
    public class MessageDBService
    {
        //建立與資料庫的連線自船
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        //建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 查詢留言陣列資料
        //根據分頁及搜尋來取得資料陣列的方法
        public List<Message> GetDataList(ForPaging paging, int A_Id)
        {
            List<Message> DataList = new List<Message>();
            SetMaxPaging(paging, A_Id);
            DataList = GetAllDataList(paging, A_Id);
            return DataList;
        }

        #endregion

        #region 設定頁數
        public void SetMaxPaging(ForPaging Paging,int A_Id)
        {
            //計算頁數
            int Row = 0;
            //Sql語法
            string sql = $@" SELECT * FROM Message WHERE A_Id = {A_Id};";
            try
            {
                conn.Open();
                //執行SQL指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())//獲得下一筆，直到沒資料
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
            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            //重新設定正確的頁數，避免有不正確直傳入
            Paging.SetRightPage();
        }

        #endregion

        #region 取得Message資料
        public List<Message> GetAllDataList(ForPaging paging,int A_Id)
        {
            //宣告要傳回的搜尋資料為資料庫中的Message
            List<Message> DataList = new List<Message>();
            //sql語法
            string sql = $@" SELECT m.*, d.Name FROM(SELECT row_number() OVER (ORDER BY M_Id) AS sort,* FROM Message WHERE A_Id ={A_Id}) m 
                        INNER JOIN Members d ON m.Account = d.Account 
                        WHERE m.sort BETWEEN {(paging.NowPage - 1) * paging.ItemNum + 1} AND {paging.NowPage * paging.ItemNum};";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Message Data = new Message();
                    Data.M_Id = Convert.ToInt32(dr["M_Id"]);
                    Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                    Data.Account = dr["Account"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    Data.Member.Name = dr["Name"].ToString();
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

        #endregion

        #region 新增文章留言
        public void InsertMessage(Message newData)
        {
            //取得新一筆的_Id
            newData.M_Id = LastMessageFinder(newData.A_Id);
            //sql新增語法
            //設定新增時間為現在
            string sql = $@"INSERT INTO Message (A_Id,M_Id,Account,Content,CreateTime) VALUES ('{newData.A_Id}','{newData.M_Id}','{newData.Account}','{newData.Content}','{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}'); ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion

        #region 計算目前留言最新一筆的M_Id
        public int LastMessageFinder(int A_Id)
        {
            //宣告要回傳的值
            int Id;
            //sql查詢語法
            string sql = $@" SELECT TOP 1 * FROM Message WHERE A_Id = {A_Id} ORDER BY M_Id DESC";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Id = Convert.ToInt32(dr["M_Id"]);
            }
            catch (Exception e)
            {
                //若沒資料時，Id為0
                Id = 0;
            }
            finally
            {
                conn.Close();
            }
            return Id + 1;
        }

        #endregion

        #region 修改留言
        public void UpdateMessage(Message UpdaeData)
        {
            string sql = $@"UPDATE Message SET Content = '{UpdaeData.Content}' WHERE A_Id = {UpdaeData.A_Id} AND M_Id = {UpdaeData.M_Id};";
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

        #region 刪除留言
        public void DeleteMessage(int A_Id, int M_Id)
        {
            string sql = $@"DELETE FROM Message WHERE A_Id = {A_Id} AND M_Id = {M_Id};";
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