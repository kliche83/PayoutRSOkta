using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Payout
{
    public partial class Changes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string user = string.Empty;

            try
            {
                user = Session["user"].ToString();
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }

            weSQL.SelectCommand = "SELECT DISTINCT CONVERT(NVARCHAR, CONVERT(DATE, [Week Ending]), 101) AS [WeekEnding] FROM [PAYOUTsummary] JOIN [PAYOUTwe] ON CONVERT(DATE, [PAYOUTwe].[WeekEnding]) = CONVERT(DATE, [PAYOUTsummary].[Week Ending]) ORDER BY [WeekEnding] DESC";
            
            if (!IsPostBack)
            {
                weDDL.SelectedIndex = 0;
                gvChanges.DataBind();
            }
        }

        protected void backBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("Grids.aspx", true);
        }

        protected void searchBtn_Click(object sender, EventArgs e)
        {
            string WE = weDDL.SelectedValue;

            sqlChanges.SelectCommand = "spx_PAYOUTchanges";
            sqlChanges.SelectParameters["WE"].DefaultValue = WE;
            gvChanges.DataBind();
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
            string attachment = "attachment; filename=\"Payout Changes " + weDDL.SelectedValue.Replace("/", "-") + ".xls\"";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            gvChanges.RenderControl(htw);

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