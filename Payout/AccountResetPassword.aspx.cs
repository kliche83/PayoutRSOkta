using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Payout
{
    public partial class AccountResetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["OkMessage"] != null)
            {
                if(Request.QueryString["OkMessage"] == "True")
                    SubmitedMessage.Text = "Password reseted successfully. Please go back to the login form to access with your account";
                else
                    valMessage.Text = "Request expired, please perform a new 'Forgot Password' request";
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["UserId"] != null)
                {
                    Session["UserId"] = Request.QueryString["UserId"];
                    Session["Code"] = Request.QueryString["Code"];

                    txtPassword1.Text = "";
                    txtConfirmPassword1.Text = "";
                }
            }
        }

        protected void BtnReset_Click(object sender, EventArgs e)
        {
            if (txtPassword1.Text.Trim() == "" || txtConfirmPassword1.Text.Trim() == "")
            {
                valMessage.Text = "Fields cannot be empty";
            }
            else
            {
                if (txtPassword1.Text != txtConfirmPassword1.Text)
                {
                    valMessage.Text = "Passwords are different";
                }
                else
                {
                    string EncodedPassword = txtPassword1.Text;
                    EncodedPassword = WebUtility.UrlEncode(EncodedPassword);

                    Response.Redirect(string.Format("{0}/api/account/ResetPassword?UserId={1}&Token={2}&NewPassword={3}",
                            ConfigurationManager.AppSettings["ServerAPIURL"],
                            Session["UserId"].ToString(),
                            HttpUtility.UrlEncode(Session["Code"].ToString()),
                            EncodedPassword
                            )
                        );
                }                    
            }
            txtPassword1.Text = "";
            txtConfirmPassword1.Text = "";
        }

        protected void lnkRedirectLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("~");
        }
    }
}