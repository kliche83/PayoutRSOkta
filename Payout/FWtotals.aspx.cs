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
    public partial class FWtotals : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string user = Session["user"].ToString();
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }
        }

        protected void backBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("Grids.aspx", true);
        }

        protected void searchBtn_Click(object sender, EventArgs e)
        {
            string StartDate = dateFrom.Text;
            string EndDate = dateTo.Text;

            //FastWax
            sqlFastWax.SelectCommand = @"SELECT [Attribute], [Week Ending], NULLIF([Retail], 0) AS [Retail], NULLIF([BCRF], 0) AS [BCRF], NULLIF([Total Sales], 0) AS [Total Sales], NULLIF([Available Stock], 0) AS [Available Stock], NULLIF([Rate], 0) AS [Rate], NULLIF([Dollars], 0) AS [Dollars], NULLIF([Weeks Left], 0) AS [Weeks Left] 
                                         FROM [PAYOUTfwTotals] WHERE [Attribute] = 'FastWax' AND [Week Ending] BETWEEN '" + StartDate + "' AND '" + EndDate + "' ORDER BY [Week Ending]";
            gvFastWax.DataBind();

            //DetailKits
            sqlDetailKits.SelectCommand = @"SELECT [Attribute], [Week Ending], NULLIF([Retail], 0) AS [Retail], NULLIF([BCRF], 0) AS [BCRF], NULLIF([Total Sales], 0) AS [Total Sales], NULLIF([Available Stock], 0) AS [Available Stock], NULLIF([Rate], 0) AS [Rate], NULLIF([Dollars], 0) AS [Dollars], NULLIF([Weeks Left], 0) AS [Weeks Left] 
                                            FROM [PAYOUTfwTotals] WHERE [Attribute] = 'DetailKits' AND [Week Ending] BETWEEN '" + StartDate + "' AND '" + EndDate + "' ORDER BY [Week Ending]";
            gvDetailKits.DataBind();

            //ActionPacks
            sqlActionPacks.SelectCommand = @"SELECT [Attribute], [Week Ending], NULLIF([Retail], 0) AS [Retail], NULLIF([BCRF], 0) AS [BCRF], NULLIF([Total Sales], 0) AS [Total Sales], NULLIF([Available Stock], 0) AS [Available Stock], NULLIF([Rate], 0) AS [Rate], NULLIF([Dollars], 0) AS [Dollars], NULLIF([Weeks Left], 0) AS [Weeks Left] 
                                             FROM [PAYOUTfwTotals] WHERE [Attribute] = 'ActionPacks' AND [Week Ending] BETWEEN '" + StartDate + "' AND '" + EndDate + "' ORDER BY [Week Ending]";
            gvActionPacks.DataBind();

            //CarShammy
            sqlCarShammy.SelectCommand = @"SELECT [Attribute], [Week Ending], NULLIF([Retail], 0) AS [Retail], NULLIF([BCRF], 0) AS [BCRF], NULLIF([Total Sales], 0) AS [Total Sales], NULLIF([Available Stock], 0) AS [Available Stock], NULLIF([Rate], 0) AS [Rate], NULLIF([Dollars], 0) AS [Dollars], NULLIF([Weeks Left], 0) AS [Weeks Left] 
                                           FROM [PAYOUTfwTotals] WHERE [Attribute] = 'CarShammy' AND [Week Ending] BETWEEN '" + StartDate + "' AND '" + EndDate + "' ORDER BY [Week Ending]";
            gvCarShammy.DataBind();

            //ShineRewind
            sqlShineRewind.SelectCommand = @"SELECT [Attribute], [Week Ending], NULLIF([Retail], 0) AS [Retail], NULLIF([BCRF], 0) AS [BCRF], NULLIF([Total Sales], 0) AS [Total Sales], NULLIF([Available Stock], 0) AS [Available Stock], NULLIF([Rate], 0) AS [Rate], NULLIF([Dollars], 0) AS [Dollars], NULLIF([Weeks Left], 0) AS [Weeks Left] 
                                             FROM [PAYOUTfwTotals] WHERE [Attribute] = 'ShineRewind' AND [Week Ending] BETWEEN '" + StartDate + "' AND '" + EndDate + "' ORDER BY [Week Ending]";
            gvShineRewind.DataBind();
        }

        protected void SQLSelecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            e.Command.CommandTimeout = 3600;
        }

        protected void GridDataBound(object sender, GridViewRowEventArgs e)
        {
            //GridView grv = (GridView)sender;
            //e.Row.Cells[0].Visible = false;

            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //}
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            //gvFastWax.DataBind();
            //gvDetailKits.DataBind();
            //gvActionPacks.DataBind();
            //gvCarShammy.DataBind();
            //gvShineRewind.DataBind();

            string attachment = "attachment; filename=\"FW Totals by Week " + DateTime.Now.Date.ToShortDateString().Replace("/", "-") + ".xls\"";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            gvFastWax.RenderControl(htw);
            sw.Write("<br>");
            gvDetailKits.RenderControl(htw);
            sw.Write("<br>");
            gvActionPacks.RenderControl(htw);
            sw.Write("<br>");
            gvCarShammy.RenderControl(htw);
            sw.Write("<br>");
            gvShineRewind.RenderControl(htw);
            
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