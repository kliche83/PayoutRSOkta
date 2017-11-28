using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication3
{
    public partial class People : System.Web.UI.Page
    {
        string user;

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

            missing.Visible = false;

            //"WHO'S MISSING?"
            //try
            //{
                using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
                {
                    string selectMissing = "insert into [PAYOUTpeople] select [OwnerFirstname], [OwnerLastname], NULL, NULL,NULL, 0 from [PAYOUTschedule] where not exists ( select [Firstname], [Lastname] from [PAYOUTpeople] where ltrim(rtrim([OwnerFirstname])) = ltrim(rtrim([Firstname])) and ltrim(rtrim([OwnerLastname])) = ltrim(rtrim([Lastname])) ) AND ([OwnerFirstname] != '' OR [OwnerLastname] != '') group by [OwnerFirstname], [OwnerLastname] order by [OwnerFirstname], [OwnerLastname]";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(selectMissing, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                    con.Close();

                    string selectMissingCC1 = "insert into [PAYOUTmailList] select [Id], NULL, NULL, NULL, NULL, NULL, NULL, 'Sales' from [PAYOUTpeople] where not exists ( select [TO] from [PAYOUTmailList] where [Type] = 'Sales' and [Id] = [To] ) group by [Id]";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(selectMissingCC1, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                    con.Close();

                    string selectMissingCC2 = "insert into [PAYOUTmailList] select [Id], NULL, NULL, NULL, NULL, NULL, NULL, 'Payouts' from [PAYOUTpeople] where not exists ( select [TO] from [PAYOUTmailList] where [Type] = 'Payouts' and [Id] = [To] ) group by [Id]";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(selectMissingCC2, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                    con.Close();

                    string selectMissingCC3 = "insert into [PAYOUTmailList] select [Id], NULL, NULL, NULL, NULL, NULL, NULL, 'Overrides' from [PAYOUTpeople] where not exists ( select [TO] from [PAYOUTmailList] where [Type] = 'Overrides' and [Id] = [To] ) group by [Id]";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(selectMissingCC3, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                    con.Close();

                    string PSID = "update PAYOUTpeople set PSID = cast(cast(cast(ol.OfficeDistributionCode as float) as int) as nvarchar(50)), PSID2 = cast(cast(cast(ol.SecurityLedger as float) as int) as nvarchar(50)) from PAYOUTpeople join Herbjoy.dbo.Owner ow on ow.OwnerFirstName = Firstname and ow.OwnerLastName = Lastname join Herbjoy.dbo.OwnerMapping owm on owm.OwnerId = ow.OwnerId join Herbjoy.dbo.OfficeMapping ofm on ofm.CorporationID = owm.CorporationId join Herbjoy.dbo.OfficeLocation ol on ol.OfficeID = ofm.OfficeID where PSID is null or PSID = ''";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(PSID, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                    con.Close();

                    string PSID2 = "update PAYOUTpeople set PSID2 = NULL where PSID2 = PSID";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(PSID2, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                    con.Close();

                    string cleanup = @"                    
	                                    ;WITH x AS 
	                                    (
	                                      SELECT [Firstname], [Lastname], [Email], [PSID], [PSID2], [ExecOfAll], rn = ROW_NUMBER() OVER 
		                                      (PARTITION BY [Firstname], [Lastname], [Email], [PSID], [PSID2], [ExecOfAll] ORDER BY [Firstname], [Lastname], [Email], [PSID], [PSID2], [ExecOfAll])
	                                      FROM [PAYOUTpeople]
	                                    )
	                                    DELETE x WHERE rn > 1;
                                    ";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(cleanup, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                    con.Close();
                }
            //}
            //catch { }

                SqlDataSource1.SelectCommand = "SELECT * FROM [PAYOUTpeople] WHERE [Firstname] LIKE '%" + nameTXT.Text + "%' OR [Lastname] LIKE '%" + nameTXT.Text + "%' ORDER BY CASE WHEN [Email] IS NULL THEN [Email] WHEN [Email] = '' THEN [Email] ELSE [Firstname] END, [Firstname], [Lastname]";
        }

        protected void searchBTN_Click(object sender, EventArgs e)
        {
            GridView1.DataBind();
        }

        int i = 0;

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Pager)
            {
                e.Row.Cells[0].Visible = false;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (((TextBox)e.Row.FindControl("Email")).Text == "")
                    {
                        i++;
                        e.Row.CssClass = "noEmail";
                        missingL.Text = i.ToString() + " owner(s) missing from the email list (highlighted in red).<br />" +
                                        "Please fill in their email address before sending reports.<br /><br /><br />" +
                                        "<a id='gotIt'>Got It</a>";
                        missing.Visible = true;
                    }
                }
            }
        }

        protected void FieldChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow gvr = (GridViewRow)txt.NamingContainer;
            int rowindex = gvr.RowIndex;
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string update = "UPDATE [PAYOUTpeople] SET " + txt.ID + " = '" + txt.Text + "' WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "'";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(update, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }
        }

        protected void addBtn_Click(object sender, EventArgs e)
        {
            string first = Request.Form["EFN"].ToString();
            string last = Request.Form["ELN"].ToString();
            string email = Request.Form["EML"].ToString();

            string Id = string.Empty;

            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string insert1 = "INSERT INTO [PAYOUTpeople] VALUES ('" + first + "', '" + last + "', '" + email + "', NULL, NULL, 0)";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(insert1, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();

                string select = "SELECT MAX(Id) FROM [PAYOUTpeople]";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(select, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Id = reader[0].ToString();
                    }
                }
                con.Close();

                string insert2 = "INSERT INTO [PAYOUTmailList] VALUES ('" + Id + "', NULL, NULL, NULL, NULL, NULL, NULL, 'Sales') " +
                                 "INSERT INTO [PAYOUTmailList] VALUES ('" + Id + "', NULL, NULL, NULL, NULL, NULL, NULL, 'Payouts') " +
                                 "INSERT INTO [PAYOUTmailList] VALUES ('" + Id + "', NULL, NULL, NULL, NULL, NULL, NULL, 'Overrides')";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(insert2, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }

            Response.Redirect("People.aspx", true);
        }

        protected void renameBtn_Click(object sender, EventArgs e)
        {
            string first = Request.Form["renF"].ToString();
            string last = Request.Form["renL"].ToString();
            string who = Request.Form["whoDDL"].ToString();

            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string renameMe = "UPDATE [PAYOUTpeople] SET [Firstname] = '" + first + "', [Lastname] = '" + last + "' WHERE [Firstname] + ' ' + [Lastname] = '" + who + "'";
                
                renameMe += " UPDATE [PAYOUTlogin] SET [Firstname] = '" + first + "', [Lastname] = '" + last + "' WHERE [Firstname] + ' ' + [Lastname] = '" + who + "'";

                renameMe += " UPDATE [PAYOUTsummary] SET [Payout Owner Name] = '" + first + " " + last + "' WHERE [Payout Owner Name] = '" + who + "'";
                renameMe += " UPDATE [PAYOUTsummaryPosted] SET [Payout Owner Name] = '" + first + " " + last + "' WHERE [Payout Owner Name] = '" + who + "'";
                
                renameMe += " UPDATE [PAYOUTdaily] SET [Owner] = '" + first + " " + last + "' WHERE [Owner] = '" + who + "'";
                renameMe += " UPDATE [PAYOUTdailyPosted] SET [Owner] = '" + first + " " + last + "' WHERE [Owner] = '" + who + "'";

                renameMe += " UPDATE [PAYOUTsales] SET [OwnerFirstname] = '" + first + "', [OwnerLastname] = '" + last + "' WHERE [OwnerFirstname] + ' ' + [OwnerLastname] = '" + who + "'";
                renameMe += " UPDATE [PAYOUTschedule] SET [OwnerFirstname] = '" + first + "', [OwnerLastname] = '" + last + "' WHERE [OwnerFirstname] + ' ' + [OwnerLastname] = '" + who + "'";

                renameMe += " UPDATE [PAYOUTmapNC] SET [Owner] = '" + first + " " + last + "' WHERE [Owner] = '" + who + "'";
                renameMe += " UPDATE [PAYOUTmapRC] SET [Owner] = '" + first + " " + last + "' WHERE [Owner] = '" + who + "'";

                renameMe += " UPDATE [PAYOUTsubsidy] SET [Owner] = '" + first + " " + last + "' WHERE [Owner] = '" + who + "'";

                con.Open();
                using (SqlCommand cmd = new SqlCommand(renameMe, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }

            Response.Redirect("People.aspx", true);
        }

        protected void delLink_Click(object sender, EventArgs e)
        {
            LinkButton del = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)del.NamingContainer;
            GridView gv = (GridView)gvr.NamingContainer;
            int delRow = gvr.RowIndex;

            string delete = "DELETE FROM [PAYOUTpeople] WHERE [Id] = '" + gv.Rows[delRow].Cells[0].Text + "' " +
                            "DELETE FROM [PAYOUTmailList] WHERE [To] = '" + gv.Rows[delRow].Cells[0].Text + "' " +
                            "DELETE FROM [PAYOUTexecs] WHERE [Person] = '" + gv.Rows[delRow].Cells[0].Text + "' " +
                            "UPDATE [PAYOUTmailList] SET [CC1] = NULL WHERE [CC1] = '" + gv.Rows[delRow].Cells[0].Text + "' " +
                            "UPDATE [PAYOUTmailList] SET [CC2] = NULL WHERE [CC2] = '" + gv.Rows[delRow].Cells[0].Text + "' " +
                            "UPDATE [PAYOUTmailList] SET [CC3] = NULL WHERE [CC3] = '" + gv.Rows[delRow].Cells[0].Text + "' " +
                            "UPDATE [PAYOUTmailList] SET [CC4] = NULL WHERE [CC4] = '" + gv.Rows[delRow].Cells[0].Text + "' " +
                            "UPDATE [PAYOUTmailList] SET [CC5] = NULL WHERE [CC5] = '" + gv.Rows[delRow].Cells[0].Text + "' " +
                            "UPDATE [PAYOUTmailList] SET [CC6] = NULL WHERE [CC6] = '" + gv.Rows[delRow].Cells[0].Text + "'";
            
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(delete, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }

            gv.DataBind();
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            GridView2.DataBind();
            string attachment = "attachment; filename=\"Payouts People.xls\"";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            //htw.WriteLine("<style>.number {mso-number-format:\"" + @"\@" + "\"" + "; } </style>");
            //htw.WriteLine(@"<style>.number9 {mso-number-format:000000000\; } </style>");
            GridView2.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }

        protected void UploadDataTableToExcel(DataTable dtEmp, string filename)
        {
            string attachment = "attachment; filename=" + filename;
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";
            string tab = string.Empty;
            foreach (DataColumn dtcol in dtEmp.Columns)
            {
                Response.Write(tab + dtcol.ColumnName);
                tab = "\t";
            }
            Response.Write("\n");
            foreach (DataRow dr in dtEmp.Rows)
            {
                tab = "";
                for (int j = 0; j < dtEmp.Columns.Count; j++)
                {
                    Response.Write(tab + Convert.ToString(dr[j]));
                    tab = "\t";
                }
                Response.Write("\n");
            }
            Response.End();
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            //
        }
    }
}