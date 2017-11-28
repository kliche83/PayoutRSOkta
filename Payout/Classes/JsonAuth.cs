using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading;
using System.Web;

namespace Payout
{
    public class JsonAuth : IHttpModule
    {
        private static readonly Lazy<string> _securityKey = new Lazy<string>(() => GetSecurityKey());

        private static string _authority = ConfigurationManager.AppSettings["Authority"];

        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += new EventHandler(JsonWebTokenHandler);
        }

        private static void JsonWebTokenHandler(Object source, EventArgs e)
        {
            var context = HttpContext.Current;
            var request = context.Request;
            var authorizationHeader = request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return;
            }
            var token = authorizationHeader.Substring(7);
            try
            {
                ValidateTokenAndSetIdentity(token);
            }
            catch (System.IdentityModel.Tokens.SecurityTokenValidationException ex)
            {
                // log error here
            }
            catch
            {
                // log error here
            }
        }

        public static void ValidateTokenAndSetIdentity(string token)
        {
            if (token != null && token != "undefined" && token != "")
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = GetValidationParameters();
                SecurityToken validToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validToken);
                Thread.CurrentPrincipal = principal;
                HttpContext.Current.User = principal;



                //var some1 = ((System.Security.Claims.ClaimsPrincipal)HttpContext.Current.User).Claims.AsEnumerable().Select(x => x.Value).FirstOrDefault();
                //var some2 = ((System.Security.Claims.ClaimsPrincipal)HttpContext.Current.User).Claims.AsEnumerable().Where(x => x.Type.Contains("identity/claims/emailaddress")).Select(x => x.Value).FirstOrDefault();
            }
        }



        private static TokenValidationParameters GetValidationParameters()
        {
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["TokenKey"]));

            return new TokenValidationParameters
            {
                ValidAudience = ConfigurationManager.AppSettings["Audience"],
                ValidIssuer = ConfigurationManager.AppSettings["Issuer"],
                IssuerSigningToken = new BinarySecretSecurityToken(hmac.Key)
            };
        }


        private static TokenValidationParameters GetValidationParametersTests()
        {


            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["TokenKey"]));

            //var hmac1 = new HMACSHA256(Convert.FromBase64String(ConfigurationManager.AppSettings["TokenKey"]));
            var signingCredentials = new SigningCredentials(new InMemorySymmetricSecurityKey(hmac.Key), SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);


            //var securityKey = Encoding.Unicode.GetBytes(ConfigurationManager.AppSettings["TokenKey"]);
            //var token = new X509SecurityToken(new X509Certificate2(securityKey));

            //var bytes = Convert.FromBase64String(_securityKey.Value);
            //var token = new X509SecurityToken(new X509Certificate2(bytes));

            return new TokenValidationParameters
            {
                ValidAudience = ConfigurationManager.AppSettings["Audience"],
                ValidIssuer = ConfigurationManager.AppSettings["Issuer"],
                //IssuerSigningKeyResolver = (arbitrarily, declaring, these, parameters) => { return token.SecurityKeys.First(); },
                IssuerSigningToken = new BinarySecretSecurityToken(hmac.Key)

                ////IssuerSigningKeyResolver = (arbitrarily, declaring, these, parameters) => { return token.SecurityKeys.First(); },
                //IssuerSigningToken = new BinarySecretSecurityToken(securityKey)//token
            };
        }


        private static TokenValidationParameters GetValidationParametersOLD()
        {
            var bytes = Convert.FromBase64String(_securityKey.Value);
            var token = new X509SecurityToken(new X509Certificate2(bytes));
            return new TokenValidationParameters
            {
                ValidAudience = _authority + "/resources",
                ValidIssuer = _authority
                //,
                //IssuerSigningKeyResolver = (arbitrarily, declaring, these, parameters) => { return token.SecurityKeys.AsEnumerable(); },
                //IssuerSigningToken = token
            };
        }

        private static string GetSecurityKey()
        {
            var webClient = new WebClient();
            var endpoint = _authority + "/.well-known/openid-configuration";
            var json = webClient.DownloadString(endpoint);
            dynamic metadata = JsonConvert.DeserializeObject<dynamic>(json);
            var jwksUri = metadata.jwks_uri.Value;
            json = webClient.DownloadString(jwksUri);
            var key = JsonConvert.DeserializeObject<dynamic>(json).keys[0];
            return (string)key.x5c[0];
        }

        private static string ConvertToSecurityKey()
        {
            var webClient = new WebClient();
            var json = ConfigurationManager.AppSettings["TokenKey"];
            dynamic metadata = JsonConvert.DeserializeObject<dynamic>(json);
            var jwksUri = metadata.jwks_uri.Value;
            json = webClient.DownloadString(jwksUri);
            var key = JsonConvert.DeserializeObject<dynamic>(json).keys[0];
            return (string)key.x5c[0];
        }

        public void Dispose()
        {

        }
    }
}