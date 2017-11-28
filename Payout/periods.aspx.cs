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
    public partial class periods : System.Web.UI.Page
    {
        string user = string.Empty;
        string userFullname = string.Empty;
        string userType = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                user = Session["user"].ToString();
                userFullname = Session["userFullname"].ToString();
                userType = Session["userType"].ToString();
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }


            if (!IsPostBack)
            {
                webYear.Text = DateTime.Now.Year.ToString();
                webDivision.SelectedValue = "All";
                webProgram.SelectedValue = "All";

                //perSQL.SelectCommand = "SELECT [PeriodId], convert(nvarchar, [PeriodId]) + ' -- ' + convert(nvarchar, [StartDate], 101) + ' to ' + convert(nvarchar, [EndDate], 101) AS [DateRange] FROM [PAYOUTperiods] WHERE [Year] = '" + webYear.Text + "'";
                divSQL.SelectCommand = "SELECT 'All' AS [Division] UNION ALL SELECT [Division] FROM [PAYOUTperiodsKroger] WHERE [Year] = '" + webYear.Text + "' GROUP BY [Division] ORDER BY [Division]";
                progSQL.SelectCommand = "SELECT 'All' AS [Program] UNION ALL SELECT [Program] FROM [PAYOUTperiodsKroger] WHERE [Year] = '" + webYear.Text + "' GROUP BY [Program] ORDER BY [Program]";

                webDivision.DataBind();
                webProgram.DataBind();
            }

            gridSQL.SelectCommand = "SELECT * FROM [PAYOUTperiodsKroger] WHERE [Year] = '" + ((webYear.Text == "") ? DateTime.Now.Year.ToString() : webYear.Text) + "'" + ((webDivision.SelectedValue == "All") ? "" : " AND [Division] = '" + webDivision.SelectedValue + "'") + ((webProgram.SelectedValue == "All") ? "" : " AND [Program] = '" + webProgram.SelectedValue + "'") + " ORDER BY [Division], [Program]";
            
            perGrid.DataBind();
        }

        protected void GridDataBound(object sender, GridViewRowEventArgs e)
        {
            // do stuff
        }

        protected void SQLSelecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            e.Command.CommandTimeout = 3600;
        }

        protected void backBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Grids");
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            perGrid.DataBind();
            string attachment = "attachment; filename=\"Kroger Periods.xls\"";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            //sw.Write("LT Report <b>" + WE.Replace("/", "-") + "</b>");
            //sw.Write("<br><br>");
            perGrid.RenderControl(htw);
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

        protected void dataBtn_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string generate = "exec spx_PAYOUTperiodsKroger @Year=" + webYear.Text;

                SqlCommand cmd = new SqlCommand(generate, con);
                cmd.CommandTimeout = 3600;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            Response.Redirect("periods.aspx");
        }
    }
}