using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Blogger.Services
{
    public class MailService
    {
        private string gmail_account = "testforasp123@gmail.com";
        private string gmail_password = "fpjmpplbokvnolia";
        private string gmail_mail = "testforasp123@gmail.com";

        #region  寄會員驗證信
        //產生驗證碼方法
        public string GetValidateCode()
        {
            //設定驗證碼字元的陣列
            string[] Code = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                              "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
                              "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"};
            //宣告初始為空的驗證碼自串
            string ValidateCode = string.Empty;
            //宣告可產出隨機述職的物件
            Random rd = new Random();
            //使用迴圈產出驗證碼
            for (int i = 0; i < 10; i++)
            {
                //Count 可以省略數字，舉例如下，可省略編寫變數count
                //var count = 0;
                //for (int i = 0; i < x.Length; i++)
                //{
                //    count++;
                //}
                ValidateCode += Code[rd.Next(Code.Count())];
            }
            return ValidateCode;
        }
        //將使用者資料填入驗證信範本中
        public string GetRegisterMailBody(string TempString, string UsrName, string ValidateUrl)
        {
            //將使用者資料填入
            TempString = TempString.Replace("{{UserName}}", UsrName);
            TempString = TempString.Replace("{{ValidateUrl}}", ValidateUrl);
            //回傳最後結果
            return TempString;
        }
        public void SendRegisterMail(string MailBody, string ToEmail)
        {
            //建立寄信用Smtp物建，這裡已Gamil為例
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            //設定使用的Port，這裡設定Gmail所使用的
            SmtpServer.Port = 587;
            //建立使用者憑據，這裡要設定你的GMail帳戶
            SmtpServer.Credentials = new System.Net.NetworkCredential(gmail_account, gmail_password);
            //開啟SSL
            SmtpServer.EnableSsl = true;
            //宣告信件內容物件
            MailMessage mail = new MailMessage();
            //設定來源信箱
            mail.From = new MailAddress(gmail_mail);
            //設定收信者信箱
            mail.To.Add(ToEmail);
            //設定信件主旨
            mail.Subject = "會員註冊確認信";
            //設定信件內容
            mail.Body = MailBody;
            //設定信件為html格式
            mail.IsBodyHtml = true;
            //送出信件
            SmtpServer.Send(mail);

        }
        #endregion

    }
}