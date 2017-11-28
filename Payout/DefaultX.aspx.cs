using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Payout
{
    public partial class DefaultX : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Session.Remove("user");
            }
            catch { }
        }

        protected void loginBtn_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string selectCreds = "SELECT Password, Firstname, Lastname FROM PAYOUTlogin WHERE Username = '" + username.Text + "'";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(selectCreds, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader[0].ToString() == password.Text)
                        {
                            Session["user"] = reader[1].ToString() + " " + reader[2].ToString();
                            kontent.InnerHtml = "";
                        }
                    }
                }
                con.Close();
            }

            note.Text = "Invalid username or password";
            note.ForeColor = System.Drawing.Color.Red;
        }
    }
}