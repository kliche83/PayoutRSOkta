
namespace Payout.CoreAPI.Models
{
    public class AppSettingsCls
    {
        public class URLKeys
        {
            public string ClientURL { get; set; }
        }

        public class ConfigJwtSecurityToken
        {
            public string Key { get; set; }
            public string Issuer { get; set; }
            public string Audience { get; set; }
        }

        public class OktaKeys
        {
            public string URL { get; set; }
            public string APIKey { get; set; }
            public string ClientId { get; set; }
        }

        public class MailCredentials
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public string User { get; set; }
            public string Password { get; set; }
            public string SenderEmail { get; set; }
            public string SenderName { get; set; }
        }
    }
}
