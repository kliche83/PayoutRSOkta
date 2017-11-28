using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using WebApplication3;
using System.Text.RegularExpressions;

namespace Payout
{
    public partial class Execps : System.Web.UI.Page
    {
        string user = string.Empty;
        string userFullname = string.Empty;
        string userType = string.Empty;
        string WeekEnding = string.Empty;
        string assignOwner = string.Empty;
        List<string> UsersRW;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                user = Session["user"].ToString();
                userFullname = Session["userFullname"].ToString();
                userType = Session["userType"].ToString();

                BulkActionsBtn.Attributes.Add("onClick", "return false;");
                UsersRW = new List<string>();
                UsersRW.Add("Admin");
                UsersRW.Add("cband@thesmartcircle.com");
                UsersRW.Add("thourtovenko@thesmartcircle.com");
                UsersRW.Add("carlos@innovage.net");
                UsersRW.Add("ben@innovage.net");

                if (!UsersRW.Contains(user))
                {
                    BulkActionsBtn.Visible = false;
                }

                if (!IsPostBack)
                {
                    Dictionary<string, string> Params = new Dictionary<string, string>();
                    Params.Add("userType", userType);
                    Params.Add("Action", "Select");

                    DataTable dt = Queries.GetResultsFromStoreProcedure("spx_PAYOUTExceptions", ref Params).Tables[0];
                    webStartDate.DataSource = dt;
                    webStartDate.DataValueField = "StartDate";
                    webStartDate.DataTextField = "WeekEnding";
                    webStartDate.DataBind();

                    setSelect();
                    webStartDate.SelectedIndex = 0;
                }

                WeekEnding = webStartDate.SelectedValue;
            }
            catch (Exception ex)
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }
        }

        void setSelect()
        {
            SQLprogram.SelectCommand = "SELECT '' AS Program UNION ALL SELECT 'Misc' AS Program UNION ALL SELECT 'Return' AS Program UNION ALL SELECT * FROM (SELECT TOP 10000 Program FROM [PAYOUTschedule] WHERE Program != '' AND Program IS NOT NULL /*AND SalesDate BETWEEN DATEADD(DAY, -13, '" + webStartDate.SelectedValue + "') AND '" + webStartDate.SelectedValue + "'*/ GROUP BY Program ORDER BY Program)x";

            Dictionary<string, string> Params = new Dictionary<string, string>();
            Params.Add("storeDDL", storeDDL.SelectedValue);
            Params.Add("SalesDateTXT", saleDate.Text);
            Params.Add("StoreNumberTXT", StoreNumberTXT.Text);
            Params.Add("QtyOption", opDDL.SelectedValue);
            Params.Add("QtyText", opTXT.Text);
            Params.Add("WeText", webStartDate.SelectedValue);
            Params.Add("viewDDL", viewDDL.SelectedValue);
            Params.Add("Action", "SelectWithFilters");

            DataTable dt = Queries.GetResultsFromStoreProcedure("spx_PAYOUTExceptions", ref Params).Tables[0];

            FullSalesAuditCls.AuditExceptions(dt, viewDDL.SelectedValue, viewDDL.SelectedValue);

            Session["ExceptionsTable"] = dt;
            salesGrid.DataSource = dt;
            salesGrid.DataBind();
        }



        protected void searchBTN_Click(object sender, EventArgs e)
        {
            setSelect();
        }

        protected void DropChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            GridViewRow gvr = (GridViewRow)ddl.NamingContainer;
            GridView gv = (GridView)gvr.NamingContainer;

            int gvRow = gvr.RowIndex;
            string gvID = gv.ID;


            UpdateSales(ddl.ID, ddl.SelectedValue, gv.Rows[gvRow].Cells[0].Text);
            if (ddl.ID == "Program")
            {
                /*This portion of code is to notify this method ran and is related with an schedule*/

                FullSalesAuditCls.AuditSalesIdWithComment(gv.Rows[gvRow].Cells[0].Text, user, "Module: Exceptions, Method: DropChanged, Action: SalesId BEFORE AssignOwner is applied");

                Dictionary<string, string> Params = new Dictionary<string, string>();
                Params.Add("Action", "AssignOwner");
                Queries.ExecuteFromStoreProcedure("spx_PAYOUTExceptions", Params);

                FullSalesAuditCls.AuditSalesIdWithComment(gv.Rows[gvRow].Cells[0].Text, user, "Module: Exceptions, Method: DropChanged, Action: SalesId AFTER AssignOwner is applied");
            }

            gv.DataBind();
            setSelect();
        }

        protected void CheckChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            GridViewRow gvr = (GridViewRow)cb.NamingContainer;
            GridView gv = (GridView)gvr.NamingContainer;
            int gvRow = gvr.RowIndex;
            string gvID = gv.ID;
            string isChecked = (cb.Checked) ? "1" : "0";

            UpdateSales(cb.ID, isChecked, gv.Rows[gvRow].Cells[0].Text);

            gv.DataBind();
            setSelect();
        }

        protected void TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow gvr = (GridViewRow)txt.NamingContainer;
            GridView gv = (GridView)gvr.NamingContainer;
            int gvRow = gvr.RowIndex;
            string gvID = gv.ID;

            UpdateSales(txt.ID, txt.Text, gv.Rows[gvRow].Cells[0].Text);

            gv.DataBind();
            setSelect();
        }


        private void UpdateSales(string FieldId, string FieldText, string SalesId)
        {
            Dictionary<string, string> Params = new Dictionary<string, string>();
            Params.Add("Action", "UPDATE");
            Params.Add("WebSalesId", SalesId);
            if (FieldId == "Program")
                Params.Add("WebProgramSet", FieldText);
            if (FieldId == "ItemNumber")
                Params.Add("WebItemNumber", FieldText);
            if (FieldId == "ItemName")
                Params.Add("WebItemName", FieldText);
            if (FieldId == "Qty")
                Params.Add("WebQtySet", FieldText);
            if (FieldId == "UnitCost")
                Params.Add("WebUnitcost", FieldText);
            if (FieldId == "Archive")
                Params.Add("WebArchiveSet", FieldText);

            Queries.ExecuteFromStoreProcedure("spx_PAYOUTsales", Params);
        }

        protected void SQLSelecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            e.Command.CommandTimeout = 3600;
        }

        void exportFile()
        {
            ExcelPackage pck = new ExcelPackage();
            string filename = string.Empty;

            filename = "Exceptions - Week Ending " + WeekEnding.Replace("/", "-");

            using (pck)
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(filename);
                DataSourceSelectArguments args = new DataSourceSelectArguments();
                DataTable DT = (DataTable)Session["ExceptionsTable"];

                ws.Cells["A1"].LoadFromDataTable(DT, true);
                ws.Cells["A1:Z2000"].AutoFitColumns();

                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.Flush();
                Response.End();
            }
        }

        protected void exportSales_Click(object sender, EventArgs e)
        {
            exportFile();
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

        protected void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Pager && e.Row.RowType != DataControlRowType.EmptyDataRow)
            {
                e.Row.Cells[0].Visible = false;
            }

            if (e.Row.RowType == DataControlRowType.DataRow && !UsersRW.Contains(user))
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Enabled = false;
                }
            }
        }

        protected void bulkApply_Click(object sender, EventArgs e)
        {
            if (UsersRW.Contains(user))
            {
                Dictionary<string, string> Params = new Dictionary<string, string>();
                Params.Add("WebOwnerCondition", "IS NULL");
                Params.Add("WebStoreName", storeDDL.SelectedValue);
                Params.Add("WebArchiveSet", "0");
                Params.Add("WebArchiveWhere", "0");
                Params.Add("WebQtyLogicalOP", opDDL.SelectedValue);
                Params.Add("WebQtyWhere", opTXT.Text);
                Params.Add("WebStartDate", webStartDate.SelectedValue);
                Params.Add("WebStoreNumber", StoreNumberTXT.Text.Trim() == "" ? "NULL" : StoreNumberTXT.Text.Trim());
                Params.Add("WebSalesDate", saleDate.Text.Trim() == "" ? "NULL" : saleDate.Text.Trim());

                Params.Add("Action", "BULKUPDATE");

                switch (bulkDDL.SelectedValue)
                {
                    case "update":
                        if (colDDL.SelectedValue == "Qty")
                            Params.Add("WebQtySet", newVal.Text.Replace("'", ""));
                        else
                            Params["Web" + colDDL.SelectedValue] = newVal.Text.Replace("'", "");
                        break;

                    case "pending":
                        Params.Add("WebProgramSet", "");
                        break;

                    case "misc":
                        Params.Add("WebProgramSet", "Misc");
                        break;

                    case "return":
                        Params.Add("WebProgramSet", "Return");
                        break;

                    case "archive":
                        Params["WebArchiveSet"] = "1";
                        break;
                }

                switch (viewDDL.SelectedValue)
                {
                    case "Pending":
                        Params.Add("WebProgramWhere", " NOT IN ('Return', 'Misc')");
                        break;

                    case "Return":
                        Params.Add("WebProgramWhere", " = 'Return'");
                        break;

                    case "Misc":
                        Params.Add("WebProgramWhere", " = 'Misc'");
                        break;

                    case "Archive":
                        Params["WebArchiveWhere"] = "1";
                        break;
                }

                if (bulkDDL.SelectedValue != "")
                    Queries.ExecuteFromStoreProcedure("spx_PAYOUTsales", Params);

                salesGrid.DataBind();
            }
        }
    }
}