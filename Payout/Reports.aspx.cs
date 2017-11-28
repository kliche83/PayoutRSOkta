using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication3
{
    public partial class Reports : System.Web.UI.Page
    {
        string user = string.Empty;
        string store = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                user = Session["user"].ToString();
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }
        }

        protected void storeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            store = storeDDL.SelectedValue;
            progSQL.SelectCommand = "SELECT x.Program FROM ( SELECT Program FROM PAYOUThistory WHERE StoreName LIKE '" + store + "%' Group BY Program ) x GROUP BY x.Program ORDER BY x.Program";
            progDDL.DataBind();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string selectCycle = "SELECT StartDate, Duration FROM PAYOUTcycle WHERE StoreName = '" + store + "'";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(selectCycle, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        StartDate.Text = Convert.ToDateTime(reader[0].ToString()).ToString("MM/dd/yyyy").Replace("-", "/");
                        Duration.Text = reader[1].ToString();
                    }
                }
                con.Close();
            }

            //Enable for auto roll over
            //if (DateTime.Now.Date > Convert.ToDateTime(StartDate.Text))
            //{
            //    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            //    {
            //        string updateCycle = "UPDATE PAYOUTcycle SET StartDate = DATEADD(DAY, Duration, StartDate) WHERE Store = '" + store + "'";
            //        con.Open();
            //        using (SqlCommand cmd = new SqlCommand(updateCycle, con))
            //        {
            //            SqlDataReader reader = cmd.ExecuteReader();
            //        }
            //        con.Close();

            //        string selectCycle = "SELECT StartDate, Duration FROM PAYOUTcycle WHERE Store = '" + store + "'";
            //        con.Open();
            //        using (SqlCommand cmd = new SqlCommand(selectCycle, con))
            //        {
            //            SqlDataReader reader = cmd.ExecuteReader();
            //            while (reader.Read())
            //            {
            //                StartDate.Text = Convert.ToDateTime(reader[0].ToString()).ToString("MM/dd/yyyy").Replace("-", "/");
            //                Duration.Text = reader[1].ToString();
            //            }
            //        }
            //        con.Close();
            //    }
            //}
        }

        protected void FieldChanged(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string updateCycle = "IF EXISTS ( SELECT * FROM PAYOUTcycle WHERE StoreName = '" + storeDDL.SelectedValue + "' ) " +
                                     "BEGIN " +
                                     "UPDATE PAYOUTcycle SET StartDate = '" + StartDate.Text + "', Duration = '" + Duration.Text + "' WHERE StoreName = '" + storeDDL.SelectedValue + "' " +
                                     "END " +
                                     "ELSE " +
                                     "BEGIN " +
                                     "INSERT INTO PAYOUTcycle VALUES ( '" + storeDDL.SelectedValue + "', '" + StartDate.Text + "', '" + Duration.Text + "' ) " +
                                     "END";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(updateCycle, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }
        }

        protected void viewBtn_Click(object sender, EventArgs e)
        {
            Session["StoreName"] = storeDDL.SelectedValue;
            Session["Program"] = progDDL.SelectedValue;
            Session["StartDate"] = StartDate.Text;
            Session["Duration"] = Duration.Text;

            Response.Redirect("Grids.aspx", true);
        }

        protected void email_Click(object sender, EventArgs e)
        {
            Response.Redirect("Email.aspx", true);
        }
    }
}