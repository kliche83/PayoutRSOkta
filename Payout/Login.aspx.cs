using System;
using System.Configuration;
using System.Data;
using System.Net;
using System.Web;
using WebApplication3;
using System.Linq;


namespace Payout
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["user"] = null;
                Session["userType"] = null;
                Session["userFullname"] = null;
                
                pnlPassword.Visible = false;                
            }
            valMessage.Text = "";

            if (Request.QueryString["param1"] != null)
            {
                try
                {
                    if (Request.QueryString["param1"] == "NotInGroup")
                    {
                        valMessage.Text = "Okta user not assigned to group. Please contact the administrator";
                    }
                    if (Request.QueryString["param1"] == "IncorrectCredentials")
                    {
                        valMessage.Text = "The username or password you entered is incorrect";
                    }
                    else
                    {
                        JsonAuth.ValidateTokenAndSetIdentity(Request.QueryString["param1"]);

                        Session["user"] = ((System.Security.Claims.ClaimsPrincipal)HttpContext.Current.User).Claims.AsEnumerable().Where(x => x.Type.Contains("identity/claims/emailaddress")).Select(x => x.Value).FirstOrDefault();
                        Session["userType"] = ((System.Security.Claims.ClaimsPrincipal)HttpContext.Current.User).Claims.AsEnumerable().Where(x => x.Type.Contains("identity/claims/role")).Select(x => x.Value).FirstOrDefault();
                        Session["userFullname"] = ((System.Security.Claims.ClaimsPrincipal)HttpContext.Current.User).Claims.AsEnumerable().Where(x => x.Type.Contains("identity/claims/givenname")).Select(x => x.Value).FirstOrDefault();

                        if (Session["user"] != null)
                        {
                            Response.Redirect("~/", true);
                            //Response.Redirect("~/", false);
                            //Context.ApplicationInstance.CompleteRequest();
                        }                        
                        else
                            valMessage.Text = "Error validating user";
                    }
                }
                catch(Exception ex)
                {
                    valMessage.Text = "Authentication error: " +ex.ToString();
                }                
            }
        }

        protected void BtnNextAsp_Click(object sender, EventArgs e)
        {
            string userResult = validateUser();

            if (userResult.Contains("Okta"))
            {
                if (userResult == "IsOkta")
                {
                    Response.Redirect(ConfigurationManager.AppSettings["ServerAPIURL"] + "/api/account/login");
                }
                else
                {
                    if (txtPassword.Text == "")
                    {
                        pnlPassword.Visible = true;
                    }
                    else
                    {
                        string EncodedPassword = txtPassword.Text;
                        EncodedPassword = WebUtility.UrlEncode(EncodedPassword);

                        Response.Redirect(string.Format("{0}/api/account/LoginCredentials?userName={1}&password={2}",
                            ConfigurationManager.AppSettings["ServerAPIURL"],
                            txtUserName.Text,
                            EncodedPassword
                            )
                        );
                    }
                }
            }
            else
            {
                valMessage.Text = userResult;
            }            
        }

        //protected void txtUserName_TextChanged(object sender, EventArgs e)
        //{
        //    //txtPassword.Text = "";
        //    //txtPassword.Visible = true;

        //    //int userResult = validateUser();
        //    //if (userResult == -1)
        //    //{
        //    //    valMessage.Text = "Incorrect user name";
        //    //}
        //    //else
        //    //{
        //    //    if (userResult == 1)
        //    //    {
        //    //        Response.Redirect(ConfigurationManager.AppSettings["ServerAPIURL"] + "/api/account/login");
        //    //    }
        //    //}                
        //}

        private string validateUser()
        {
            string SQLString = string.Format(@" SELECT 
                                                CASE WHEN IsDisabled = 1 THEN 'Disabled'
                                                WHEN IsOkta = 1 THEN 'IsOkta'
                                                ELSE 'NoOkta' END
                                                FROM AspNetUsers
                                                WHERE Email = '{0}'", txtUserName.Text);

            DataTable dt = Queries.GetResultsFromQueryString(SQLString);

            if (dt.Rows.Count == 0)//User not found
            {
                return "User incorrect";
            }                
            else
            {
                string message = "";
                switch (dt.Rows[0][0].ToString())
                {
                    case "Disabled":
                        message = "User disabled, please contact the administrator";
                        break;
                    case "IsOkta":
                        message = "IsOkta";
                        break;
                    case "NoOkta":
                        message = "NoOkta";
                        break;
                }

                return message;

            }
        }

        protected void lnkForgotPassword_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/AccountForgotPassword.aspx");
        }
    }
}