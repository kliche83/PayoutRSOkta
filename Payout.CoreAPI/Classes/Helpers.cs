using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Payout.CoreAPI.Models;
using System.Net;
using System.Net.Mail;

namespace Payout.CoreAPI
{
    public class Helpers
    {
        private readonly AppSettingsCls.MailCredentials _optionMailCredentials;

        public Helpers(IOptions<AppSettingsCls.MailCredentials> optionMailCredentials)
        {
            _optionMailCredentials = optionMailCredentials.Value;
        }

        public static void SendEmail(AppSettingsCls.MailCredentials _optionMailCredentials, string UserEmail, string Subject, string Body)
        {
            SmtpClient smtpClient = new SmtpClient();            
            smtpClient.Host = _optionMailCredentials.Host;
            smtpClient.Port = _optionMailCredentials.Port;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = false;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(_optionMailCredentials.User, _optionMailCredentials.Password);

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_optionMailCredentials.SenderEmail, _optionMailCredentials.SenderName);
            mail.To.Add(new MailAddress(UserEmail));
            mail.Subject = Subject;
            mail.IsBodyHtml = true;
            mail.Body = Body;

            smtpClient.Send(mail);
        }
    }
}
