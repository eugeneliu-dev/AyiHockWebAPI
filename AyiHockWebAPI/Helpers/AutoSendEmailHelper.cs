using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AyiHockWebAPI.Helpers
{
    public class AutoSendEmailHelper
    {
        private readonly IConfiguration _configuration;
        public AutoSendEmailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool SendAuthEmail(string userMail, string authLink)
        {
            try
            {
                string account = _configuration.GetValue<string>("Email:Account");
                string pwd = _configuration.GetValue<string>("Email:Pwd");
                string mailBody = "";

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(account, "阿奕の腿庫飯-會員驗證信");
                mail.To.Add(userMail);

                mail.Priority = MailPriority.Normal;
                mail.Subject = "阿奕の腿庫飯-會員驗證信";

                mailBody += "<h5>請依以下訊息進行驗證</h5>";
                mailBody += "<h5>" + authLink + "</h5><hr>";
                mailBody += "<h5>此為系統主動發送信函，請勿直接回覆此封信件。</h5>";
                mail.Body = mailBody;
                mail.IsBodyHtml = true;

                SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);
                MySmtp.Credentials = new System.Net.NetworkCredential(account, pwd);
                MySmtp.EnableSsl = true;
                MySmtp.Send(mail);

                MySmtp = null;
                mail.Dispose();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
