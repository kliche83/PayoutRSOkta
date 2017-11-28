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
    public partial class GridSummaryYTD : System.Web.UI.Page
    {
        string ParamStartDate;
        string ParamEndDate;
        string userType = string.Empty;
        //string user = string.Empty;
        string userFullname = string.Empty;
        //string Owner = string.Empty;
        //string StoreName = string.Empty;
        //string StoreNumber = string.Empty;
        //string Program = string.Empty;
        string QueryString = string.Empty;
        Dictionary<string, string> SQLParameters;

        protected void Page_Load(object sender, EventArgs e)
        {
            string user = string.Empty;

            try
            {
                user = Session["user"].ToString();

                if (!IsPostBack)
                {
                    FillControls();
                }
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }
        }

        private void FillControls()
        {
            DataTable dt;
            userType = Session["userType"].ToString();
            userFullname = Session["userFullname"].ToString();
            Session["YTDdateFrom"] = string.Empty;
            Session["YTDdateTo"] = string.Empty;
            Session["YTDowner"] = string.Empty;
            Session["YTDstoreName"] = string.Empty;
            Session["YTDstoreNumber"] = string.Empty;
            Session["YTDprogram"] = string.Empty;
            Session["YTDlocation"] = string.Empty;
            Session["YTDUserFullname"] = string.Empty;
            Session["ResultTable"] = null;
            Session["ExportTable"] = new DataTable();

            BindGridData();

            dt = (DataTable)Session["ResultTable"];
            Session["YTDExportTable"] = (DataTable)Session["ResultTable"];

            Common.BindDDLFromTableResults(dt, "Program", ref programDDL, Session["YTDprogram"].ToString());
            Common.BindDDLFromTableResults(dt, "StoreName", ref sstoreDDL, Session["YTDStoreName"].ToString());
            Common.BindDDLFromTableResults(dt, "Owner", ref ownerDDL, Session["YTDOwner"].ToString());
            Common.BindDDLFromTableResults(dt, "location", ref locationDDL, Session["YTDlocation"].ToString());
        }


        private void FillFromDatabase()
        {
            DataTable dt = new DataTable();

            //DateFilters(ref ParamStartDate, ref ParamEndDate);
            Common.StartEndDateFilters(ref ParamStartDate, ref ParamEndDate);

            dateFrom.Text = DatePickerFormat(ParamStartDate);
            dateTo.Text = DatePickerFormat(ParamEndDate);

            SQLParameters.Add("@webStartDate", ApplyDateFormat(ParamStartDate));
            SQLParameters.Add("@webEndDate", ApplyDateFormat(ParamEndDate));

            //SQLParameters.Add("@webProgram", Session["YTDprogram"].ToString());
            //SQLParameters.Add("@webStoreName", Session["YTDstoreName"].ToString());
            //SQLParameters.Add("@webStoreNumber", Session["YTDstoreNumber"].ToString());            
            //SQLParameters.Add("@webOwner", Session["YTDowner"].ToString());
            //SQLParameters.Add("@webLocation", Session["YTDlocation"].ToString());
            SQLParameters.Add("@UserFullname", Session["YTDUserFullname"].ToString());
            SQLParameters.Add("@UserType", userType);

            dt = Queries.GetResultsFromStoreProcedure("spx_PAYOUTsummaryYTD", ref SQLParameters).Tables[0];

            StoreNumberTXT.Text = string.Empty;
            //ownerTXT.Text = string.Empty;            

            Session["ResultTable"] = dt;
            //Session["ResultTable"] = sendingResultTableByValue(dt);

            //Session["YTDdateFrom"] = ParamStartDate;
            //Session["YTDdateTo"] = ParamEndDate;
        }


        private void BindGridData()
        {

            SQLParameters = new Dictionary<string, string>();

            ParamStartDate = dateFrom.Text;
            ParamEndDate = dateTo.Text;

            FillFromDatabase();
            DataTable dt1 = new DataTable();
            dt1 = ((DataTable)Session["ResultTable"]).Copy(); //Copy() clone datatable including records by VALUE instead REFERENCE

            if (Session["YTDdateFrom"].ToString() != ApplyDateFormat(ParamStartDate) || Session["YTDdateTo"].ToString() != ApplyDateFormat(ParamEndDate))
            {
                Common.BindDDLFromTableResults(dt1, "Program", ref programDDL, Session["YTDprogram"].ToString());
                Common.BindDDLFromTableResults(dt1, "StoreName", ref sstoreDDL, Session["YTDStoreName"].ToString());
                Common.BindDDLFromTableResults(dt1, "Owner", ref ownerDDL, Session["YTDOwner"].ToString());
                Common.BindDDLFromTableResults(dt1, "location", ref locationDDL, Session["YTDlocation"].ToString());
            }
            else
            {
                if (ParamStartDate == string.Empty && ParamEndDate == string.Empty)
                {
                    Common.BindDDLFromTableResults(dt1, "Program", ref programDDL, Session["YTDprogram"].ToString());
                    Common.BindDDLFromTableResults(dt1, "StoreName", ref sstoreDDL, Session["YTDStoreName"].ToString());
                    Common.BindDDLFromTableResults(dt1, "Owner", ref ownerDDL, Session["YTDOwner"].ToString());
                    Common.BindDDLFromTableResults(dt1, "location", ref locationDDL, Session["YTDlocation"].ToString());
                }
                else
                {
                    Session["YTDExportTable"] = dt1;
                    StoreNumberTXT.Text = Session["StoreNumber"] != null ? Session["StoreNumber"].ToString() : string.Empty;
                }
            }
            Session["YTDdateFrom"] = ApplyDateFormat(ParamStartDate);
            Session["YTDdateTo"] = ApplyDateFormat(ParamEndDate);

            if (ownerDDL.SelectedValue.ToLower() != "all")
                Common.ApplyRowsfilterDatatable(ref dt1, "Owner", ownerDDL.SelectedValue);

            if (programDDL.SelectedValue.ToLower() != "all")
                Common.ApplyRowsfilterDatatable(ref dt1, "Program", programDDL.SelectedValue);

            if (sstoreDDL.SelectedValue.ToLower() != "all")
                Common.ApplyRowsfilterDatatable(ref dt1, "StoreName", sstoreDDL.SelectedValue);

            if (!string.IsNullOrWhiteSpace(StoreNumberTXT.Text))
                Common.ApplyRowsfilterDatatable(ref dt1, "StoreNumber", StoreNumberTXT.Text);

            if (locationDDL.SelectedValue.ToLower() != "all")
                Common.ApplyRowsfilterDatatable(ref dt1, "Location", locationDDL.SelectedValue);

            ApplyProfilesToDatatable(ref dt1, userType);
            RemoveDataTableColumns(ref dt1);
            dt1 = RenameDataTableColsName(dt1);

            GridView1.DataSource = dt1;
            GridView1.DataBind();


            if (dt1 != null && dt1.Columns.Count > 0)
                FooterListTotalization(ref GridView1);

            Session["YTDExportTable"] = GridView1.DataSource;
        }

        private void FooterListTotalization(ref GridView GridView1)
        {
            GridView1.FooterRow.Cells[2].Text = "Total";
            GridView1.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
            Common.TotalizeFooter(ref GridView1, "Retail Sales");
            Common.TotalizeFooter(ref GridView1, "NCR");
            Common.TotalizeFooter(ref GridView1, "Gross Payout");
            Common.TotalizeFooter(ref GridView1, "Total RT");
            Common.TotalizeFooter(ref GridView1, "Total Extra Comm#");
            Common.TotalizeFooter(ref GridView1, "Subsidy");
            Common.TotalizeFooter(ref GridView1, "Net Payout");
            Common.TotalizeFooter(ref GridView1, "Qty Ins");
            Common.TotalizeFooter(ref GridView1, "Insur Chip Payout");
            Common.TotalizeFooter(ref GridView1, "Cash Chip");
            Common.TotalizeFooter(ref GridView1, "Cash Chip Payout");
            Common.TotalizeFooter(ref GridView1, "SC ($6)");
        }
        
        private void RemoveRowsfilterDatatable(ref DataTable dt, string FieldName, List<string> lstValues)
        {
            dt = (from a in dt.AsEnumerable()
                  from b in lstValues
                  where !a.Field<string>(FieldName).ToUpper().Contains(b.ToUpper())
                  select a).CopyToDataTable();
        }


        private void BindDDLFromTableResults(DataTable dt, string FieldName, DropDownList ControlDDL)
        {
            List<string> lstFilteredDT = new List<string>();

            lstFilteredDT = dt.AsEnumerable()
                .Select(t => t.Field<string>(FieldName)).ToList();
            lstFilteredDT = lstFilteredDT.Distinct().ToList();
            lstFilteredDT.Sort();
            lstFilteredDT.Insert(0, "All");

            ControlDDL.DataSource = lstFilteredDT;
            ControlDDL.DataBind();
            if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["YTD" + FieldName])))
            {
                if (lstFilteredDT.Where(s => s.IndexOf(Session["YTD" + FieldName].ToString()) == 0).Count() > 0)
                {
                    try
                    {
                        ControlDDL.SelectedValue = Session["YTD" + FieldName].ToString();
                    }
                    catch { }
                }
            }
        }


        //protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType != DataControlRowType.Pager)
        //    {
        //        e.Row.Cells[0].Visible = false;
        //    }
        //}        

        protected void searchBTN_Click(object sender, EventArgs e)
        {
            Session["StoreName"] = sstoreDDL.SelectedValue;
            Session["Program"] = programDDL.SelectedValue;
            //Session["Owner"] = ownerTXT.Text;
            Session["StoreNumber"] = StoreNumberTXT.Text;
            BindGridData();
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)Session["YTDExportTable"];

            if (dt.Rows.Count > 0)
            {
                GridView2.AllowPaging = false;
                GridView2.DataSource = dt;
                GridView2.DataBind();

                string attachment = "attachment; filename=\"RoadShow Schedule.xls\"";
                Response.ClearContent();
                Response.AddHeader("content-disposition", attachment);
                Response.ContentType = "application/ms-excel";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                GridView2.FooterRow.Visible = true;
                FooterListTotalization(ref GridView2);

                GridView2.RenderControl(htw);
                Response.Write(sw.ToString());
                Response.End();
                GridView2.AllowPaging = true;
            }            
        }


        public override void VerifyRenderingInServerForm(Control control)
        {
            //
        }

        //private void DateFilters(ref string StartDate, ref string EndDate)
        //{
        //    if (string.IsNullOrWhiteSpace(StartDate))
        //    {
        //        DateTime dt = new DateTime(DateTime.Now.Year, 1, 1);
        //        StartDate = ApplyDateFormat(dt.ToString());
        //    }

        //    if (string.IsNullOrWhiteSpace(EndDate))
        //    {
        //        EndDate = ApplyDateFormat(DateTime.Now.ToString());
        //    }               

        //}

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            BindGridData();
        }

        private string DatePickerFormat(string date)
        {
            return string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(date));
        }


        private string ApplyDateFormat(string dateToFormat)
        {
            return string.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(dateToFormat));
        }

        private void ApplyProfilesToDatatable(ref DataTable dt, string userType)
        {

            switch (userType.ToLower())
            {
                case "owner":
                    ProgramFilter1(ref dt);
                    break;
                case "hub":
                    ProgramFilter1(ref dt);
                    break;
                case "nc":
                    ProgramFilter1(ref dt);
                    break;
                case "rsm":
                    ProgramFilter1(ref dt);
                    break;
                case "rc":
                    ProgramFilter1(ref dt);
                    break;
            }
        }

        private void ProgramFilter1(ref DataTable dt)
        {
            List<string> lstValues = new List<string>();
            lstValues.Add("Chipio - Chip Repair");
            if (Convert.ToDateTime(dateFrom) < new DateTime(2015, 04, 20)) //for FastWax programs
            {
                lstValues.Add("Fast Wax");
                lstValues.Add("Fast Wax Inside");
            }
            RemoveRowsfilterDatatable(ref dt, "Program", lstValues);
        }

        private int GetColumnIndexByName(GridView grid, string name)
        {
            for (int i = 0; i < grid.HeaderRow.Cells.Count; i++)
            {
                if (grid.HeaderRow.Cells[i].Text.ToLower().Trim() == name.ToLower().Trim())
                {
                    return i;
                }
            }
            return -1;
        }

        protected void GridView1_PageIndexChanging1(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            BindGridData();
        }

        private void RemoveDataTableColumns(ref DataTable dt)
        {
            if (dt.Columns.Count > 0)
            {
                if (Session["YTDprogram"].ToString() == "Chipio - Chip Repair")
                {
                    dt.Columns.Remove("AVG");
                    dt.Columns.Remove("Extra Comm#");
                    dt.Columns.Remove("Gross Payout");
                    dt.Columns.Remove("NCR");
                    dt.Columns.Remove("Net Payout");
                    dt.Columns.Remove("RT");
                    dt.Columns.Remove("Subsidy");
                    dt.Columns.Remove("Total Extra Comm#");
                    dt.Columns.Remove("Total RT");
                    dt.Columns.Remove("Limit");
                }
                else
                {
                    dt.Columns.Remove("Cash Chip Payout");
                    dt.Columns.Remove("Cash Chip");
                    dt.Columns.Remove("Insur Chip Payout");
                    dt.Columns.Remove("Qty Ins");
                    dt.Columns.Remove("SC ($6)");

                    if (userFullname != "Payout Admin" && userFullname != "Andjelka Sarac")
                    {
                        dt.Columns.Remove("Limit");
                    }
                }
            }            
        }

        private DataTable RenameDataTableColsName(DataTable dt2)
        {
            if (dt2.Columns.Contains("% RT Extra Subsidy (CALIF)"))
                dt2.Columns["% RT Extra Subsidy (CALIF)"].ColumnName = "% RT Extra (CA)";
            if (dt2.Columns.Contains("% RT Extra Subsidy"))
                dt2.Columns["% RT Extra Subsidy"].ColumnName = "% RT Extra";
            if (dt2.Columns.Contains("Cash Chip Payout"))
                dt2.Columns["Cash Chip Payout"].ColumnName = "$ Cash Chips";
            if (dt2.Columns.Contains("Cash Chip"))
                dt2.Columns["Cash Chip"].ColumnName = "Qty Cash";
            if (dt2.Columns.Contains("Extra Comm#"))
                dt2.Columns["Extra Comm#"].ColumnName = "% Extra Comm.";
            if (dt2.Columns.Contains("Insur Chip Payout"))
                dt2.Columns["Insur Chip Payout"].ColumnName = "$ Ins Chips";
            if (dt2.Columns.Contains("qty"))
                dt2.Columns["qty"].ColumnName = "Pcs.Sold";
            if (dt2.Columns.Contains("RT"))
                dt2.Columns["RT"].ColumnName = "% RT";
            if (dt2.Columns.Contains("StoreName"))
                dt2.Columns["StoreName"].ColumnName = "Store Name";
            if (dt2.Columns.Contains("StoreNumber"))
                dt2.Columns["StoreNumber"].ColumnName = "Club #";
            if (dt2.Columns.Contains("Total Extra Comm#"))
                dt2.Columns["Total Extra Comm#"].ColumnName = "$ Extra Comm.";
            if (dt2.Columns.Contains("Total RT"))
                dt2.Columns["Total RT"].ColumnName = "$ RT";

            return dt2;
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridView grv = (GridView)sender;

            if (StoreNumberTXT.Text == "")
            {
                Session["StoreNumber"] = "All";
            }
            else
            {
                Session["StoreNumber"] = StoreNumberTXT.Text;
            }

            string SQLstring = string.Format("SELECT TOP 1 StartDate FROM PAYOUTwe WHERE WeekEnding = '{0}'", grv.SelectedRow.Cells[0].Text);
            DataTable dt = Queries.GetResultsFromQueryString(SQLstring);

            Session["g2StartDate"] = dt.Rows[0][0].ToString();
            Session["g2Duration"] = "14";

            for (int i = 0; i < grv.SelectedRow.Cells.Count; i++)
            {
                string columnName = ((BoundField)((DataControlFieldCell)(grv.SelectedRow.Cells[i])).ContainingField).DataField;

                switch (columnName.ToLower().Trim())
                {
                    case "owner":
                        Session["g2Owner"] = grv.SelectedRow.Cells[i].Text;
                        break;
                    case "hub":
                        Session["g2Hub"] = grv.SelectedRow.Cells[i].Text;
                        break;
                    case "store name":
                        Session["g2StoreName"] = grv.SelectedRow.Cells[i].Text.Replace("amp;", "");
                        break;
                    case "program":
                        Session["g2Program"] = grv.SelectedRow.Cells[i].Text;
                        break;
                    case "club #":
                        Session["g2StoreNumber"] = grv.SelectedRow.Cells[i].Text;
                        break;
                    case "location":
                        Session["g2Location"] = grv.SelectedRow.Cells[i].Text;
                        break;
                    case "ps id":
                        Session["g2PSID"] = grv.SelectedRow.Cells[i].Text;
                        break;
                }
            }

            Response.Redirect("Grids2.aspx", true);
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView grv = (GridView)sender;
            int j = 0;
            int k = 0;
            string gridID = grv.ID;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (gridID == "GridView1")
                {
                    j = 8;
                    k = e.Row.Cells.Count - 3;

                    e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                    e.Row.Cells[k + 1].HorizontalAlign = HorizontalAlign.Right;
                    e.Row.Cells[k + 2].HorizontalAlign = HorizontalAlign.Right;
                    {
                        e.Row.Attributes["ondblclick"] = ClientScript.GetPostBackClientHyperlink(grv, "Select$" + e.Row.RowIndex);
                    }

                }

                for (int i = j; i < k; i++)
                {
                    e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                }
            }
        }
    }
}