using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using WebApplication3;

namespace Payout
{
    public partial class Edit : System.Web.UI.Page
    {
        string user = string.Empty;
        string userType = string.Empty;
        string Query = "";
        string assignOwner = string.Empty;
        int rowindex;
        List<string> UsersRW;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                user = Session["user"].ToString();
                userType = Session["userType"].ToString();

                UsersRW = new List<string>();
                UsersRW.Add("cband@thesmartcircle.com");
                UsersRW.Add("thourtovenko@thesmartcircle.com");
                UsersRW.Add("carlos@innovage.net");
                UsersRW.Add("ben@innovage.net");
                UsersRW.Add("Admin");
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }

            exportRawBtn.Visible = false;

            if (user == "Admin" || user == "andjelka@thesmartcircle.com")
            {
                exportRawBtn.Visible = true;
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

            //weSQL.SelectCommand = "SELECT DISTINCT CONVERT(NVARCHAR, CONVERT(DATE, [Week Ending]), 101) AS [WeekEnding], CONVERT(NVARCHAR, [PAYOUTwe].StartDate) AS [StartDate], CONVERT(DATE, [WeekEnding]) AS [weDate] FROM [PAYOUTsummary] JOIN [PAYOUTwe] ON CONVERT(DATE, [PAYOUTwe].[WeekEnding]) = CONVERT(DATE, [PAYOUTsummary].[Week Ending]) " + where + " ORDER BY [weDate] DESC";

            //if (!IsPostBack)
            //{
            //    dateDDL.DataBind();
            //    dateDDL.SelectedIndex = 0;
            //}

            //Query = "SELECT * FROM [PAYOUTsales] WHERE [StoreName] LIKE '" + storeDDL.SelectedValue + "%' " + ((programDDL.SelectedValue == "All") ? "" : "AND [Program] = '" + programDDL.SelectedValue + "'") + " " + ((StoreNumberTXT.Text == "") ? "" : "AND [StoreNumber] = '" + StoreNumberTXT.Text + "'") + " " + ((saleDate.Text == "") ? "" : "AND [SalesDate] = '" + saleDate.Text + "'") + " AND [SalesDate] BETWEEN '" + dateDDL.SelectedValue + "' AND DATEADD(DAY, 13, '" + dateDDL.SelectedValue + "') ORDER BY [SalesDate] DESC";

            //Query = @"  SELECT * 
            //            FROM [PAYOUTsales] 
            //            WHERE [StoreName] 
            //                LIKE '" + storeDDL.SelectedValue + "%' " + ((programDDL.SelectedValue == "All") ? "" : @"
            //                AND [Program] = '" + programDDL.SelectedValue + "'") + " " + ((StoreNumberTXT.Text == "") ? "" : @"
            //                AND [StoreNumber] = '" + StoreNumberTXT.Text + "'") + " " + ((saleDate.Text == "") ? "" : @"
            //                AND [SalesDate] = '" + saleDate.Text + "'") + @" 
            //                AND [SalesDate] BETWEEN '" + dateDDL.SelectedValue + "' AND (SELECT TOP 1 WeekEnding FROM PAYOUTwe WHERE StartDate = '" + dateDDL.SelectedValue + @"') 
            //            ORDER BY [SalesDate] DESC";


            //string[] dateRanges = GetDateInputs();


            exportRawBtn.Enabled = true;
            exportBtn.Enabled = true;
            delBtn.Enabled = true;

            List<string> ValidatedDates = validateSalesDates();
            salesFromTXT.Text = ValidatedDates[0];
            salesToTXT.Text = ValidatedDates[1];
            
            Query = string.Format(@"
                        SELECT * 
                        FROM [PAYOUTsales] 
                        WHERE [StoreName] LIKE '{0}%' 
                                {1} {2} 
                                AND [SalesDate] >= '{3}'
                                AND [SalesDate] <= '{4}'

                        ORDER BY [SalesDate] DESC",

                        storeDDL.SelectedValue,
                        programDDL.SelectedValue == "All" ? "" : string.Format(" AND [Program] = '{0}'", programDDL.SelectedValue),
                        StoreNumberTXT.Text.Trim() == "" ? "" : string.Format(" AND [StoreNumber] = '{0}'", StoreNumberTXT.Text),
                        salesFromTXT.Text,
                        salesToTXT.Text
                        //dateRanges[0] == "" ? "" : string.Format(" AND [SalesDate] >= '{0}'", dateRanges[0]),
                        //dateRanges[1] == "" ? "" : string.Format(" AND [SalesDate] <= '{0}'", dateRanges[0])

            );


            SqlDataSource1.SelectCommand = Query;

            assignOwner = @"
                                UPDATE PAYOUTsales SET [OwnerFirstname] = LTRIM(RTRIM(P.[OwnerFirstname])), [OwnerLastname] = LTRIM(RTRIM(P.[OwnerLastname])), [StartDate] = P.[StartDate], [EndDate] = P.[EndDate], [City] = P.[City], [State] = P.[State], [HubFirstname] = LTRIM(RTRIM(P.[HubFirstname])), [HubLastname] = LTRIM(RTRIM(P.[HubLastname]))
                                FROM PAYOUTsales S
                                JOIN PAYOUTschedule P ON P.StoreName = S.StoreName AND P.StoreNumber = S.StoreNumber AND P.Program = S.Program AND S.SalesDate BETWEEN P.StartDate AND P.EndDate
                                /*WHERE S.Program IS NOT NULL AND S.Program != '' AND S.OwnerFirstname IS NULL*/
                                WHERE S.Id = {0}
                           ";
        }

        //private string[] GetDateInputs()
        //{
        //    string[] dateRanges = new string[2];
        //    DateTime OutValue;
        //    DateTime.TryParse(salesFromTXT.Text.Trim(), out OutValue);
        //    if (OutValue != DateTime.MinValue)
        //        dateRanges[0] = salesFromTXT.Text.Trim();

        //    DateTime.TryParse(salesToTXT.Text.Trim(), out OutValue);
        //    if (OutValue != DateTime.MinValue)
        //        dateRanges[1] = salesToTXT.Text.Trim();

        //    return dateRanges;
        //}

        protected void searchBTN_Click(object sender, EventArgs e)
        {
            GridView1.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Pager)
            {
                e.Row.Cells[0].Visible = false;

                if (!UsersRW.Contains(user) && e.Row.Cells.Count > 1)
                {
                    e.Row.Cells[1].Visible = false;
                }
            }

            if (!UsersRW.Contains(user) && e.Row.Cells.Count > 1)
                Common.TurnOffControlsFromGrid(e);
        }

        protected void DropChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            GridViewRow gvr = (GridViewRow)ddl.NamingContainer;
            rowindex = gvr.RowIndex;

            UpdateSales(ddl.ID, ddl.SelectedValue);

            if (ddl.ID == "Program")
            {
                assignOwner = string.Format(assignOwner, GridView1.Rows[rowindex].Cells[0].Text);
                Queries.ExecuteFromQueryString(assignOwner);
            }



            //using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            //{
            //    //string update = "UPDATE [PAYOUTsales] SET " + ddl.ID + " = '" + ddl.SelectedValue + "' WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "' " +
            //    //                "UPDATE [PAYOUTsales] SET [ItemName] = I.[ItemName] FROM [PAYOUTsales] S JOIN [PAYOUTitemMaster] I ON I.Program = S.Program AND I.ItemNumber = S.ItemNumber AND S.StoreName LIKE I.StoreName + '%' WHERE S.Program IS NOT NULL";

            //    string update = "UPDATE [PAYOUTsales] SET " + ddl.ID + " = '" + ddl.SelectedValue + "' WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "'";
            //    con.Open();
            //    using (SqlCommand cmd = new SqlCommand(update, con))
            //    {
            //        SqlDataReader reader = cmd.ExecuteReader();
            //    }
            //    con.Close();

            //    if (ddl.ID == "Program")
            //    {
            //        assignOwner = string.Format(assignOwner, GridView1.Rows[rowindex].Cells[0].Text);
            //        Queries.ExecuteFromQueryString(assignOwner);
            //    }
            //}
        }

        protected void FieldChanged(Object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow gvr = (GridViewRow)txt.NamingContainer;
            rowindex = gvr.RowIndex;

            UpdateSales(txt.ID, txt.Text);
        }

        private void UpdateSales(string FieldId, string FieldText)
        {
            Dictionary<string, string> Params = new Dictionary<string, string>();
            Params.Add("Action", "UPDATE");
            Params.Add("WebSalesId", GridView1.Rows[rowindex].Cells[0].Text);
            if (FieldId == "Program")
                Params.Add("WebProgramSet", FieldText);
            if (FieldId == "ItemName")
                Params.Add("WebItemName", FieldText);
            if (FieldId == "Qty")
                Params.Add("WebQtySet", FieldText);
            if (FieldId == "UnitCost")
                Params.Add("WebUnitcost", FieldText);

            Queries.ExecuteFromStoreProcedure("spx_PAYOUTsales", Params);
        }

        protected void delLink_Click(object sender, EventArgs e)
        {
            LinkButton del = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)del.NamingContainer;
            int delRow = gvr.RowIndex;

            Dictionary<string, string> Params = new Dictionary<string, string>();
            Params.Add("Action", "DELETE");
            Params.Add("WebSalesId", GridView1.Rows[delRow].Cells[0].Text);
            Queries.GetResultsFromStoreProcedure("spx_PAYOUTsales", ref Params);

            GridView1.DataBind();
        }

        //BULK DELETE
        protected void delBtn_Click(object sender, EventArgs e)
        {            
            string deleteView_confirm_value = Request.Form["deleteView_confirm_value"];

            if (deleteView_confirm_value == "Yes")
            {
                //string[] dateRanges = GetDateInputs();

                Dictionary<string, string> Params = new Dictionary<string, string>();
                Params.Add("Action", "BULKDELETE");
                Params.Add("WebStoreName", storeDDL.SelectedValue == "%" ? "NULL" : storeDDL.SelectedValue.Replace("'", "''"));
                Params.Add("WebStoreNumber", StoreNumberTXT.Text == "" ? "NULL" : StoreNumberTXT.Text.Replace("'", "''"));
                Params.Add("WebProgramWhere", programDDL.SelectedValue == "All" ? "NULL" : " = '" + programDDL.SelectedValue.Replace("'", "''") + "'");
                //Params.Add("WebSalesDate", saleDate.Text == "" ? "NULL" : saleDate.Text);
                //Params.Add("WebStartDate", dateDDL.SelectedValue);

                Params.Add("WebStartSalesRange", salesFromTXT.Text);
                Params.Add("WebEndSalesRange", salesToTXT.Text);

                //Params.Add("WebStartSalesRange", dateRanges[0]);
                //Params.Add("WebEndSalesRange", dateRanges[1]);

                Queries.GetResultsFromStoreProcedure("spx_PAYOUTsales", ref Params);

                //using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
                //{
                //    string delete = Query.Replace("SELECT *", "DELETE").Replace(" ORDER BY [SalesDate] DESC", "");
                //    con.Open();
                //    using (SqlCommand cmd = new SqlCommand(delete, con))
                //    {
                //        SqlDataReader reader = cmd.ExecuteReader();
                //    }
                //    con.Close();
                //}

                GridView1.DataBind();
            }
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            GridView2.DataBind();
            string attachment = "attachment; filename=\"RoadShow Sales.xls\"";
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

        protected void exportRawBtn_Click(object sender, EventArgs e)
        {
            List<string> ValidatedDates = validateSalesDates();

            SqlDataSource rawSQL = new SqlDataSource();
            rawSQL.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            rawSQL.Selecting += SQLSelecting;
            //rawSQL.SelectCommand = string.Format(@"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED 
            //                    SELECT [SalesDate], [Program], [StoreName], [StoreNumber], [ItemNumber], [ItemName], [Qty], 
            //                    [UnitCost], [ExtendedCost], [OwnerFirstname], [OwnerLastname], [StartDate], [EndDate], [City], 
            //                    [State], [HubFirstname], [HubLastname] 
            //                    FROM PAYOUTsales 

            //                    WHERE ARCHIVE = 0 AND Program NOT LIKE 'Return%' AND Program NOT IN ('', 'Misc')
            //                    AND [SalesDate] BETWEEN '{0}' AND '{1}'                                
            //                    ORDER BY SalesDate DESC, StoreName, Program, StoreNumber, ItemNumber",
            //                    ValidatedDates[0],
            //                    ValidatedDates[1]
            //                    );


            rawSQL.SelectCommand = string.Format(@"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED 
                                SELECT [SalesDate], [Program], [StoreName], [StoreNumber], [ItemNumber], [ItemName], [Qty], 
                                [UnitCost], [ExtendedCost], [OwnerFirstname], [OwnerLastname], [StartDate], [EndDate], [City], 
                                [State], [HubFirstname], [HubLastname] 
                                FROM PAYOUTsales
                                WHERE 
                                ARCHIVE = 0 AND [SalesDate] BETWEEN '{0}' AND '{1}'
                                {2} {3}
                                ORDER BY SalesDate DESC, StoreName, Program, StoreNumber, ItemNumber",
                                salesFromTXT.Text,
                                salesToTXT.Text,
                                storeDDL.SelectedValue == "%" ? "" : string.Format(" AND StoreName = '{0}'", storeDDL.SelectedValue.Replace("'", "''")),
                                StoreNumberTXT.Text == "%" || StoreNumberTXT.Text.Trim() == "" ? "" : string.Format(" AND StoreNumber = '{0}'", StoreNumberTXT.Text),
                                programDDL.SelectedValue == "%" ? "" : string.Format(" AND Program = '{0}'", programDDL.SelectedValue.Replace("'", "''"))
                                );


            DataSourceSelectArguments args = new DataSourceSelectArguments();
            DataView view = (DataView)rawSQL.Select(args);
            DataTable dt = view.ToTable();

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet wss = pck.Workbook.Worksheets.Add("Raw Sales");
            wss.Cells["A1"].LoadFromDataTable(dt, true);

            wss.Column(1).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            wss.Column(12).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            wss.Column(13).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;

            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;  filename=Raw Sales " + DateTime.Now.AddDays(-28).ToShortDateString().Replace("/", "-") + " - " + DateTime.Now.ToShortDateString().Replace("/", "-") + ".xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.Flush();
            Response.End();
        }

        private List<string> validateSalesDates()
        {
            List<DateTime> results = new List<DateTime>();

            DateTime DateParseResult;
            DateTime.TryParse(salesFromTXT.Text, out DateParseResult);
            results.Add(DateParseResult);
            DateTime.TryParse(salesToTXT.Text, out DateParseResult);
            results.Add(DateParseResult);

            var some = results.Where(item => item.Date != DateTime.MinValue).Count();

            if (results.Where(item => item.Date != DateTime.MinValue).Count() == 0)
            {
                return new List<string>()
                {
                    Common.ApplyDateFormat(DateTime.Now.AddDays(-15)),
                    Common.ApplyDateFormat(DateTime.Now)
                };
            }
            else
            {
                return new List<string>()
                {
                    results[0] == DateTime.MinValue ? Common.ApplyDateFormat(results[1].AddDays(-15)) 
                                                    : results[0] > results[1] ? Common.ApplyDateFormat(results[1].AddDays(-15)) : Common.ApplyDateFormat(results[0]),
                    
                    results[1] != DateTime.MinValue ? Common.ApplyDateFormat(results[1]) : Common.ApplyDateFormat(results[0].AddDays(15))
                };
            }            
        }



        protected void SQLSelecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            e.Command.CommandTimeout = 3600;
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