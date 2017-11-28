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
    public partial class GridRoleOverride : System.Web.UI.Page
    {
        string ParamStartDate;
        string ParamEndDate;
        //string ParamTrainer;
        string userType = string.Empty;
        string user = string.Empty;
        string userFullname = string.Empty;
        string Owner = string.Empty;
        string StoreName = string.Empty;
        string StoreNumber = string.Empty;
        string Program = string.Empty;
        string QueryString = string.Empty;
        Dictionary<string, string> SQLParameters;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    Session["dateFrom"] = string.Empty;
                    Session["dateTo"] = string.Empty;
                    Session["ResultTable"] = new DataTable();
                    Session["ExportTable"] = new DataTable();
                    user = Session["user"].ToString();
                    userType = Session["userType"].ToString();
                    userFullname = Session["userFullname"].ToString();

                    BindGridData();
                    Session["ExportTable"] = (DataTable)Session["ResultTable"];
                }
                catch(Exception ex)
                {
                    Response.Write("<script>window.top.location = '../';</script>");
                }                
            }
        }


        private void FillFromDatabase()
        {
            DataTable dt = new DataTable();

            //DateFilters(ref ParamStartDate, ref ParamEndDate);


            string SQLString = string.Format("SELECT [Id] FROM [PAYOUTPMPayroll] WHERE [CheckDate] = '{0}'", (string)Session["ParamEndDate"]);

            List<int> dtPayrollIds = Queries.GetResultsFromQueryString(SQLString).AsEnumerable().Select(i => i.Field<int>("Id")).ToList();

            if (dtPayrollIds.Count > 0)
            {
                SQLString = string.Format(@"SELECT  TrainerName [Trainer], Retailer [StoreName], [Program], [WeeklyCompensation], 
                                                    OverridePercentage [Override], TotalSales [Total Sales], [NCR], [Overrides], OverridesDue [Overrides Due]
                                            FROM    PAYOUTPMOverrideSummary
                                            WHERE   PMPayroll_Id IN ({0})
                                                    AND [TrainerName] = '{1}'", 
                                                    string.Join(", ", dtPayrollIds.ToArray()), 
                                                    (string)Session["Trainer"]);

                dt = Queries.GetResultsFromQueryString(SQLString);
            }
            else
            {
                ParamStartDate = (string)Session["ParamStartDate"];
                ParamEndDate = (string)Session["ParamEndDate"];
                

                dateFrom.Text = DatePickerFormat(ParamStartDate);
                dateTo.Text = DatePickerFormat(ParamEndDate);

                SQLParameters.Add("@webStartDate", ParamStartDate);
                SQLParameters.Add("@webEndDate", ParamEndDate);
                SQLParameters.Add("@webTrainer", (string)Session["Trainer"]);
                SQLParameters.Add("@Action", "SELECT");

                dt = Queries.GetResultsFromStoreProcedure("spx_PAYOUTsummaryOverrides", ref SQLParameters).Tables[0];
            }                                    

            DataView dv = dt.DefaultView;
            //dt = dv.Table.Rows.OfType<DataRow>().Where(f => f.Field<string>("Trainer") == ParamTrainer).CopyToDataTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                BindDDL(dt, "Program", programDDL);
                BindDDL(dt, "StoreName", StoreNameDDL);
                BindDDL(dt, "Trainer", TrainerDDL);
            }            

            Session["ResultTable"] = dt;            
            Session["dateFrom"] = ParamStartDate;
            Session["dateTo"] = ParamEndDate;
        }

        private void BindGridData()
        {
            DataTable dt = new DataTable();            
            SQLParameters = new Dictionary<string, string>();
            
            ParamStartDate = dateFrom.Text;
            ParamEndDate = dateTo.Text;


            if (Session["dateFrom"].ToString() != ParamStartDate || Session["dateTo"].ToString() != ParamEndDate)
            {
                FillFromDatabase();
            }
            else
            {
                if (ParamStartDate == string.Empty && ParamEndDate == string.Empty)
                {
                    FillFromDatabase();
                }
                else
                {                    
                    Session["ExportTable"] = dt;
                }
            }

            dt = (DataTable)Session["ResultTable"];

            if (dt != null && dt.Rows.Count > 0)
            {
                if (programDDL.SelectedValue.ToLower() != "all")
                    filterDatatable(ref dt, "Program", programDDL.SelectedValue);

                if (StoreNameDDL.SelectedValue.ToLower() != "all")
                    filterDatatable(ref dt, "StoreName", StoreNameDDL.SelectedValue);

                if (TrainerDDL.SelectedValue.ToLower() != "all")
                    filterDatatable(ref dt, "Trainer", TrainerDDL.SelectedValue);
            }                

            GridView1.DataSource = dt;
            GridView1.DataBind();

            if (dt != null && dt.Rows.Count > 0)
                FooterListTotalization(ref GridView1);
        }


        private void filterDatatable(ref DataTable dt, string FieldName, string Value)
        {
            try
            {
                dt = dt.AsEnumerable()
                        .Where(row => row.Field<String>(FieldName).ToLower().Contains(Value.ToLower().Trim())).CopyToDataTable();
            }
            catch{}            
        }
        

        private void BindDDL(DataTable dt, string FieldName, DropDownList ControlDDL)
        {
            List<string> lstFilteredDT = new List<string>();

            lstFilteredDT = dt.AsEnumerable()
                .Where(t => !string.IsNullOrWhiteSpace(t.Field<string>(FieldName)))
                .Select(t => t.Field<string>(FieldName)).ToList();
            lstFilteredDT = lstFilteredDT.Distinct().ToList();
            lstFilteredDT.Sort();
            lstFilteredDT.Insert(0, "All");

            ControlDDL.DataSource = lstFilteredDT;
            ControlDDL.DataBind();
            if (!string.IsNullOrWhiteSpace(Convert.ToString(Session[FieldName + "DDL"])))
            {
                if (lstFilteredDT.Where(s => s.IndexOf(Session[FieldName + "DDL"].ToString()) == 0).Count() > 0)
                {
                    try
                    {
                        ControlDDL.SelectedValue = Session[FieldName + "DDL"].ToString();
                    }
                    catch { }
                }
            }
        }


        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType != DataControlRowType.Pager)
            //{
            //    e.Row.Cells[0].Visible = false;
            //}

            if (e.Row.RowType != DataControlRowType.Header)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if ((e.Row.Cells[i]).Text != "&nbsp;" && (e.Row.Cells[i]).Text != (e.Row.Cells[0]).Text)
                    {
                        List<string> DecimalCells = new List<string> { "Override", "Total Sales", "NCR", "Overrides", "Overrides Due" };

                        if(DecimalCells.Contains(((BoundField)((DataControlFieldCell)((e.Row.Cells[i]))).ContainingField).DataField))
                        {
                            decimal result = 0;
                            decimal.TryParse((e.Row.Cells[i]).Text, out result);
                            (e.Row.Cells[i]).Text = result.ToString("C");
                            e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                        }
                        else
                        {
                            e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Center;
                        }
                    }
                }
            }
        }        

        protected void searchBTN_Click(object sender, EventArgs e)
        {
            Session["StoreName"] = StoreNameDDL.SelectedValue;
            Session["Program"] = programDDL.SelectedValue;
            Session["Trainer"] = TrainerDDL.SelectedValue;

            BindGridData();
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            GridView2.AllowPaging = false;
            GridView2.DataSource = Session["ExportTable"];
            GridView2.DataBind();
            

            string attachment = string.Format("attachment; filename=\"RoadShow Override Summary {0}.xls\"", ApplyDateFormat(DateTime.Now));
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

        private void DateFilters(ref string StartDate, ref string EndDate)
        {
            if (string.IsNullOrWhiteSpace(StartDate))
            {                
                DateTime dateFirstDayYear = new DateTime(DateTime.Now.Year, 1, 1);
                StartDate = Common.ApplyDateFormat(dateFirstDayYear.ToString());
            }


            if (string.IsNullOrWhiteSpace(EndDate))
            {
                //EndDate = ApplyDateFormat(DateTime.Now.ToString());
                EndDate = Common.ApplyDateFormat("2017-01-15");
            }
                
        }

        //private string ApplyDateFormat(string dateToFormat)
        //{
        //    return string.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(dateToFormat));
        //}

        private string ApplyDateFormat(DateTime dateToFormat)
        {
            return string.Format("{0:yyyy-MM-dd}", dateToFormat);
        }

        protected void SQLSelecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            e.Command.CommandTimeout = 3600;
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            BindGridData();            
        }

        private string DatePickerFormat(string date)
        {
            return string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(date));
        }

        private void FooterListTotalization(ref GridView GridView1)
        {            
            Common.TotalizeFooter(ref GridView1, "Overrides Due", true);
            int IndexColumn = Common.GetColumnIndexByName(GridView1, "Overrides Due") - 1;
            GridView1.FooterRow.Cells[IndexColumn].Text = "Grand Total";
            GridView1.FooterRow.Cells[IndexColumn].HorizontalAlign = HorizontalAlign.Right;
        }

    }
}