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

namespace Payout
{
    public partial class Subsidy : System.Web.UI.Page
    {
        string user;
        string userType;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                user = Session["user"].ToString();
                userType = Session["userType"].ToString();
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }

            string where = string.Empty;

            if (userType == "Admin" || userType == "SC" || userType == "PMMerch")
            {
                where = "WHERE [Internal] = 1";
            }
            else
            {
                where = "WHERE [" + userType + "] = 1";
            }

            weSQL.SelectCommand = "SELECT 'SELECT' AS [WeekEnding], 'SELECT' AS [StartDate] UNION ALL SELECT DISTINCT CONVERT(NVARCHAR, CONVERT(DATE, [Week Ending]), 101) AS [WeekEnding], CONVERT(NVARCHAR, DATEADD(DAY, -13, CONVERT(DATE, [Week Ending]))) AS [StartDate] FROM [PAYOUTsummary] JOIN [PAYOUTwe] ON CONVERT(DATE, [PAYOUTwe].[WeekEnding]) = CONVERT(DATE, [PAYOUTsummary].[Week Ending]) " + where + " ORDER BY [WeekEnding] DESC";

            if(!IsPostBack)
            {
                SSD.DataBind();
                SSD.SelectedIndex = 0;
                hidSpan.Visible = false;
            }

            string WE = string.Empty;
            if (weDate.Text != "") { WE = " AND [StartDate] >= '" + weDate.Text + "'"; } else { WE = ""; }
            SqlDataSource1.SelectCommand = "SELECT * FROM [PAYOUTsubsidy] WHERE [Owner] LIKE '%" + ownerTXT.Text + "%' AND [StoreName] LIKE '" + storeDDLs.SelectedValue + "%' AND [StoreNumber] LIKE '%" + StoreNumberTXT.Text + "%' AND [Type] LIKE '%" + typeDDLs.SelectedValue + "%'" + WE + " ORDER BY [StartDate] DESC, [Owner], [StoreNumber]";
            storeSQL.SelectCommand = "SELECT StoreName FROM PAYOUTschedule WHERE StoreName != '' OR StoreName IS NOT NULL GROUP BY StoreName ORDER BY StoreName";
            programSQL.SelectCommand = "SELECT [Program] FROM [PAYOUTschedule] GROUP BY [Program] ORDER BY [Program]";
        }

        protected void searchBTN_Click(object sender, EventArgs e)
        {
            GridView1.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Pager && e.Row.RowType != DataControlRowType.EmptyDataRow)
            {
                e.Row.Cells[0].Visible = false;
            }
        }

        protected void FieldChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow gvr = (GridViewRow)txt.NamingContainer;
            int rowindex = gvr.RowIndex;
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string update = "UPDATE [PAYOUTsubsidy] SET " + txt.ID + " = '" + txt.Text + "' WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "'";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(update, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }
        }

        protected void DropChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            GridViewRow gvr = (GridViewRow)ddl.NamingContainer;
            int rowindex = gvr.RowIndex;
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string update = "UPDATE [PAYOUTsubsidy] SET " + ddl.ID + " = '" + ddl.SelectedValue + "' WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "'";
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
            string Owner = owDDL.SelectedValue;
            string StoreName = storeDDL.SelectedValue;
            string StoreNumber = SNO.Text;
            string Prog = progDDL.SelectedValue;
            string EarnType = typeDDL.SelectedValue;
            string Amount = AMN.Text;
            string Comments = CMN.Text;
            string StartDate = Convert.ToDateTime(STD.Text).ToString("yyyy-MM-dd");
            string EndDate = Convert.ToDateTime(END.Text).ToString("yyyy-MM-dd");
            string AppBy = APB.Text;
            string AppOn = Convert.ToDateTime(APO.Text).ToString("yyyy-MM-dd");

            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string insert = "INSERT INTO [PAYOUTsubsidy] VALUES ('" + Owner + "', '" + StoreName + "', '" + StoreNumber + "', '" + Prog + "', '" + EarnType + "', '" + Amount + "', '" + Comments + "', '" + StartDate + "', '" + EndDate + "', '" + AppOn + "', '" + AppBy + "', 0)";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(insert, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }

            Response.Redirect("Subsidy.aspx", true);
        }

        protected void delLink_Click(object sender, EventArgs e)
        {
            LinkButton del = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)del.NamingContainer;
            GridView gv = (GridView)gvr.NamingContainer;
            int delRow = gvr.RowIndex;

            string delete = "DELETE FROM [PAYOUTsubsidy] WHERE [Id] = '" + gv.Rows[delRow].Cells[0].Text + "'";

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

        protected void addLink_Click(object sender, EventArgs e)
        {
            LinkButton del = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)del.NamingContainer;
            GridView gv = (GridView)gvr.NamingContainer;
            int delRow = gvr.RowIndex;

            string add = @"
                            INSERT INTO [PAYOUTsubsidy] 
                            SELECT TOP 1 
	                               [Owner]
                                  ,[StoreName]
                                  ,[StoreNumber]
                                  ,[Program]
                                  ,'n/a' AS [Type]
                                  ,NULL AS [Amount]
                                  ,NULL AS [Comments]
                                  ,[StartDate]
                                  ,[EndDate]
                                  ,NULL AS [ApprovedOn]
                                  ,NULL AS [ApprovedBy]
                                  ,0 AS [Lock]
                            FROM [PAYOUTsubsidy]
                            WHERE [Id] = '" + gv.Rows[delRow].Cells[0].Text + @"'
                         ";

            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(add, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }

            gv.DataBind();
        }

        protected void DateChanged(object sender, EventArgs e)
        {
            if (SSD.SelectedValue != "SELECT")
            {
                //owSQL.SelectCommand = "SELECT [OwnerFirstname] + ' ' + [OwnerLastname] AS [OwnerName] FROM [PAYOUTschedule] WHERE [StartDate] >= '" + SSD.Text + "' AND [EndDate] <= '" + SED.Text + "' GROUP BY [OwnerFirstname], [OwnerLastname] ORDER BY [OwnerFirstname], [OwnerLastname]";
                owSQL.SelectCommand = "SELECT [OwnerFirstname] + ' ' + [OwnerLastname] AS [OwnerName] FROM [PAYOUTschedule] WHERE '" + SSD.SelectedValue + "' BETWEEN [StartDate] AND [EndDate] GROUP BY [OwnerFirstname], [OwnerLastname] ORDER BY [OwnerFirstname], [OwnerLastname]";
                owDDL.DataBind();
                hidSpan.Visible = true;
            }
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            GridView2.AllowPaging = false;
            GridView2.DataBind();
            string attachment = "attachment; filename=\"Payouts Extra Earnings.xls\"";
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
            GridView2.AllowPaging = true;
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

        protected void popBtn_Click(object sender, EventArgs e)
        {
            //update list
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string insert = @"
                                    INSERT INTO [PAYOUTsubsidy]
                                    SELECT [OwnerFirstname] + ' ' + [OwnerLastname] AS [Owner], [StoreName], [StoreNumber], [Program], 'n/a' AS [Type], NULL AS [Amount], NULL AS [Comments], [StartDate], [EndDate], NULL AS [ApprovedOn], NULL AS [ApprovedBy], 0 AS [Lock] 
                                    FROM [PAYOUTschedule] A 
                                    WHERE StoreName IS NOT NULL AND StoreNumber IS NOT NULL 
                                    AND NOT EXISTS (
	                                    SELECT [Owner], [StoreName], [StoreNumber], [Program], [Type], [Amount], [Comments], [StartDate], [EndDate], [ApprovedOn], [ApprovedBy] 
	                                    FROM [PAYOUTsubsidy] 
	                                    WHERE [Owner] = A.[OwnerFirstname] + ' ' +  A.[OwnerLastname] AND [StoreName] = A.[StoreName] AND [StoreNumber] = A.[StoreNumber] AND [StartDate] = A.[StartDate] AND [EndDate] = A.[EndDate] AND [Program] = A.[Program]
                                    )

                                    DELETE FROM [PAYOUTsubsidy] WHERE Id IN (
	                                    SELECT S.[Id] FROM [PAYOUTsubsidy] S
	                                    WHERE NOT EXISTS (
		                                    SELECT *
		                                    FROM [PAYOUTschedule] A
		                                    WHERE S.[Owner] = A.[OwnerFirstname] + ' ' +  A.[OwnerLastname] AND S.[StoreName] = A.[StoreName] AND S.[StoreNumber] = A.[StoreNumber] AND S.[StartDate] = A.[StartDate] AND S.[EndDate] = A.[EndDate] AND S.[Program] = A.[Program]
	                                    )
                                    )
                                ";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(insert, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }
        }
    }
}