using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Payout
{
    public partial class Execps : System.Web.UI.Page
    {
        string user = string.Empty;

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

            string salesQuery = string.Empty;
            string scheduleQuery = string.Empty;

            if (dateFrom.Text == "" && dateTo.Text == "")
            {
                salesQuery = "SalesDate >= '2014-12-29' ";
                scheduleQuery = "StartDate >= '2014-12-29' /*OR EndDate >= '2014-12-29'*/ ";
            }
            if (dateFrom.Text != "" && dateTo.Text != "")
            {
                salesQuery = "SalesDate BETWEEN '" + dateFrom.Text + "' AND '" + dateTo.Text + "' ";
                scheduleQuery = "StartDate >= '" + dateFrom.Text + "' AND '" + dateTo.Text + "' >= EndDate ";
            }
            if (dateFrom.Text != "" && dateTo.Text == "")
            {
                salesQuery = "SalesDate >= '" + dateFrom.Text + "' ";
                scheduleQuery = "StartDate >= '" + dateFrom.Text + "' ";
            }
            if (dateFrom.Text == "" && dateTo.Text != "")
            {
                salesQuery = "SalesDate <= '" + dateTo.Text + "' ";
                scheduleQuery = "EndDate <= '" + dateTo.Text + "' ";
            }

            salesSQL.SelectCommand =
                "SELECT StoreName, StoreNumber, Program, CONVERT(NVARCHAR, SalesDate, 101) AS SalesDate, sum(Qty) Qty, sum(ExtendedCost) ExtendedCost, ImportedBy, CONVERT(NVARCHAR, ImportedOn, 101) AS ImportedOn " +
                "FROM [PAYOUThistory] A " +
                "WHERE " + 
                salesQuery +
                "AND StoreName LIKE '" + storeDDL.SelectedValue + "%' AND Program LIKE '" + ((programDDL.SelectedValue == "All") ? "%" : programDDL.SelectedValue) + "%' AND StoreNumber LIKE '%" + StoreNumberTXT.Text + "%' " + ((opDDL.SelectedValue != "All" && opTXT.Text != "") ? "AND Qty " + opDDL.SelectedValue + " '" + opTXT.Text + "' " : "") +
                "AND NOT EXISTS ( " +
	                "select * from [Herbjoy].[dbo].[PAYOUTschedule] " +
	                "where Program = A.Program and A.SalesDate between StartDate and EndDate and StoreNumber = A.StoreNumber and StoreName = A.StoreName " +
                ") " +
                "GROUP BY StoreName, StoreNumber, Program, SalesDate, ImportedBy, ImportedOn " +
                "ORDER BY StoreName, StoreNumber, Program, SalesDate, ImportedBy, ImportedOn";

            scheduleSQL.SelectCommand =
                "SELECT StoreName, StoreNumber, Program, CONVERT(NVARCHAR, StartDate, 101) AS StartDate, CONVERT(NVARCHAR, EndDate, 101) AS EndDate, City, State, OwnerFirstname + ' ' + OwnerLastname AS Owner, HubFirstname + ' ' + HubLastname AS Hub, ImportedBy, CONVERT(NVARCHAR, ImportedOn, 101) AS ImportedOn " +
                "FROM [PAYOUTschedule] A " +
                "WHERE " +
                scheduleQuery +
                "AND StoreName LIKE '" + storeDDL.SelectedValue + "%' AND Program LIKE '" + ((programDDL.SelectedValue == "All") ? "%" : programDDL.SelectedValue) + "%' AND StoreNumber LIKE '%" + StoreNumberTXT.Text + "%' /*AND Qty " + opDDL.SelectedValue + " '" + opTXT.Text + "'*/ " +
                "AND NOT EXISTS ( " +
	                "select * from [Herbjoy].[dbo].[PAYOUThistory] " +
	                "where Program = A.Program and SalesDate between A.StartDate and A.EndDate and StoreNumber = A.StoreNumber and StoreName = A.StoreName " +
                ") " +
                "GROUP BY StoreName, StoreNumber, Program, StartDate, EndDate, City, State, OwnerFirstname, OwnerLastname, HubFirstname, HubLastname, ImportedBy, ImportedOn " +
                "ORDER BY StoreName, StoreNumber, Program, StartDate, EndDate, City, State, OwnerFirstname, OwnerLastname, HubFirstname, HubLastname, ImportedBy, ImportedOn";
        }

        protected void searchBTN_Click(object sender, EventArgs e)
        {
            salesGrid.DataBind();
            scheduleGrid.DataBind();
        }

        protected void exportSales_Click(object sender, EventArgs e)
        {
            salesGrid.DataBind();
            string attachment = "attachment; filename=Payout Exceptions - Sales.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            salesGrid.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }

        protected void exportSchedule_Click(object sender, EventArgs e)
        {
            scheduleGrid.DataBind();
            string attachment = "attachment; filename=Payout Exceptions - Schedule.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            scheduleGrid.RenderControl(htw);
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