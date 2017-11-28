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
    public partial class Avg : System.Web.UI.Page
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

            //using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            //{
            //    string getEff = "SELECT CONVERT(NVARCHAR, MAX(StartDate), 101) FROM [PAYOUTavgs]";
            //    con.Open();
            //    using (SqlCommand cmd = new SqlCommand(getEff, con))
            //    {
            //        SqlDataReader reader = cmd.ExecuteReader();
            //        while (reader.Read())
            //        {
            //            LatestEffDate.Value = reader[0].ToString();
            //        }
            //    }
            //    con.Close();
            //}

            string whereProgram = "";
            string whereDate = "";

            if (progDDLs.SelectedValue != "All")
            {
                if (progDDLs.SelectedValue.Contains("Pillows"))
                {
                    whereProgram = "AND Program = 'Pillows'";
                }
                else
                {
                    whereProgram = "AND Program LIKE '" + progDDLs.SelectedValue + "%'";
                }
            }

            if (dateFrom.Text != "" && dateTo.Text == "")
            {
                whereDate = "AND StartDate >= '" + dateFrom.Text + "'";
            }
            if (dateFrom.Text == "" && dateTo.Text != "")
            {
                whereDate = "AND EndDate <= '" + dateTo.Text + "'";
            }
            if (dateFrom.Text != "" && dateTo.Text != "")
            {
                whereDate = "AND StartDate >= '" + dateFrom.Text + "' AND EndDate <= '" + dateTo.Text + "'";
            }

            if (!IsPostBack)
            {
                // show latest for all
                SqlDataSource1.SelectCommand = "SELECT DISTINCT Id, StoreName, Program, Limit, MAX(StartDate) AS StartDate, MAX(EndDate) AS EndDate FROM [PAYOUTavgs] GROUP BY Id, StoreName, Program, Limit ORDER BY StoreName, Program";
            }
            else
            {
                SqlDataSource1.SelectCommand = "SELECT * FROM [PAYOUTavgs] WHERE StoreName LIKE '" + storeDDLs.SelectedValue + "%' " + whereProgram + " " + whereDate;
            }
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
                string update = "UPDATE [PAYOUTavgs] SET " + txt.ID + " = '" + txt.Text + "' WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "'";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(update, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }

            GridView1.DataBind();
        }

        protected void addBtn_Click(object sender, EventArgs e)
        {
            //string StoreName = storeDDL.SelectedValue;
            //string Program = progDDL.SelectedValue;
            //string EffDate = EFFD.Text;

            //string Standard = string.Empty;
            //string Special = string.Empty;
            //string RTExtra = string.Empty;
            //string RTExtraC = string.Empty;

            //if (STCMM.Text != "")
            //{
            //    Standard = (Convert.ToInt32(STCMM.Text) / 100).ToString("0.00");
            //}
            //else
            //{
            //    Standard = "0";
            //}
            //if (SPCMM.Text != "")
            //{
            //    Special = (Convert.ToInt32(SPCMM.Text) / 100).ToString("0.00");
            //}
            //else
            //{
            //    Special = "0";
            //}
            //if (RTES.Text != "")
            //{
            //    RTExtra = (Convert.ToInt32(RTES.Text) / 100).ToString("0.00");
            //}
            //else
            //{
            //    RTExtra = "0";
            //}
            //if (RTESC.Text != "")
            //{
            //    RTExtraC = (Convert.ToInt32(RTESC.Text) / 100).ToString("0.00");
            //}
            //else
            //{
            //    RTExtraC = "0";
            //}

            //using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            //{
            //    string insert = "INSERT INTO [PAYOUTcommissions] VALUES ('" + StoreName + "', '" + Program + "', '" + Standard + "', '" + Special + "', '" + RTExtra + "', '" + RTExtraC + "', '" + EffDate + "')";
            //    con.Open();
            //    using (SqlCommand cmd = new SqlCommand(insert, con))
            //    {
            //        SqlDataReader reader = cmd.ExecuteReader();
            //    }
            //    con.Close();

            //    string update = "UPDATE [PAYOUTcommissions] SET [StandardComm] = NULL WHERE [StandardComm] = 0 " +
            //                    "UPDATE [PAYOUTcommissions] SET [SpecialComm] = NULL WHERE [SpecialComm] = 0 " +
            //                    "UPDATE [PAYOUTcommissions] SET [RTExtra] = NULL WHERE [RTExtra] = 0 " +
            //                    "UPDATE [PAYOUTcommissions] SET [RTExtraCali] = NULL WHERE [RTExtraCali] = 0";
            //    con.Open();
            //    using (SqlCommand cmd = new SqlCommand(update, con))
            //    {
            //        SqlDataReader reader = cmd.ExecuteReader();
            //    }
            //    con.Close();
            //}

            //Response.Redirect("Avg.aspx", true);
        }

        protected void delLink_Click(object sender, EventArgs e)
        {
            LinkButton del = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)del.NamingContainer;
            GridView gv = (GridView)gvr.NamingContainer;
            int delRow = gvr.RowIndex;

            string delete = "DELETE FROM [PAYOUTavgs] WHERE [Id] = '" + gv.Rows[delRow].Cells[0].Text + "'";

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

        protected void weekBtn_Click(object sender, EventArgs e)
        {
            string closeWeek_confirmValue = Request.Form["closeWeek_confirm_value"];

            List<string> Ids = new List<string>();

            if (closeWeek_confirmValue == "Yes")
            {

                using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
                {
                    string getList = "INSERT INTO [PAYOUTavgs] SELECT DISTINCT StoreName, Program, Limit, DATEADD(WEEK, 1, MAX(StartDate)) AS StartDate, DATEADD(WEEK, 1, MAX(EndDate)) AS EndDate FROM [PAYOUTavgs] GROUP BY StoreName, Program, Limit ORDER BY StoreName, Program";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(getList, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                    con.Close();

                    // use if above fails
                    //string getList = "SELECT DISTINCT Id, StoreName, Program, MAX(StartDate) AS StartDate, MAX(EndDate) AS EndDate FROM [PAYOUTavgs] GROUP BY Id, StoreName, Program ORDER BY StoreName, Program";
                    //con.Open();
                    //using (SqlCommand cmd = new SqlCommand(getList, con))
                    //{
                    //    SqlDataReader reader = cmd.ExecuteReader();
                    //    while (reader.Read())
                    //    {
                    //        Ids.Add(reader["Id"].ToString());
                    //    }
                    //}
                    //con.Close();

                    //foreach (string Id in Ids)
                    //{
                    //    string close = "INSERT INTO [PAYOUTavgs] SELECT StoreName, Program, Limit, dateadd(week, 1, max(StartDate)) AS StartDate, dateadd(week, 1, max(EndDate)) AS EndDate FROM [PAYOUTavgs] WHERE Id = '" + Id + "' GROUP BY StoreName, Program, Limit, StartDate, EndDate";
                    //    con.Open();
                    //    using (SqlCommand cmd = new SqlCommand(close, con))
                    //    {
                    //        SqlDataReader reader = cmd.ExecuteReader();
                    //    }
                    //    con.Close();
                    //}
                }
            }

            Response.Redirect("Avg.aspx", true);
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            GridView2.DataBind();
            string attachment = "attachment; filename=\"RoadShow Commission Chart.xls\"";
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