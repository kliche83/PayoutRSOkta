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
    public partial class AccountForgotPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["OkMessage"] != null)
            {
                SubmitedMessage.Text = "we have sent an email, you should get it shortly. If you do not receive a message, check your spam folder.";
            }
        }

        protected void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (txtEmail.Text.Trim() == "")
            {
                valMessage.Text = "This field cannot be empty";
            }
            else
            {
                string EncodedEmail = WebUtility.UrlEncode(txtEmail.Text);

                Response.Redirect(string.Format("{0}/api/account/SendEmailResetPassword?Email={1}",
                            ConfigurationManager.AppSettings["ServerAPIURL"],
                            EncodedEmail
                            )
                        );
            }            
        }

        protected void lnkRedirectLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("~");
        }
    }
}