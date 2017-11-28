using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace WebApplication3
{
    public partial class PMCumulativeSummary : Page
    {
        string userType = string.Empty;
        string user = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                user = Session["user"].ToString();
                userType = Session["userType"].ToString();

                if (!IsPostBack)
                {
                    FillControls();
                }
            }
            catch(Exception ex)
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }            
        }

        private void FillControls()
        {
            Session["Trainer"] = null;
            Session["ddlYear"] = null;
            Session["StartDate"] = null;
            Session["EndDate"] = null;
            Session["BiweekTable"] = null;
            Session["YearTable"] = null;
            Session["lstResultTables"] = new List<DataTable>();

            string paramStartDate = "", paramEndDate = "";
            //List<weekending> lstWE = GetWeekendingList(ref paramStartDate, ref paramEndDate);
            List<DataTable> LstDT = new List<DataTable>();

            Session["BiweekTable"] = PMCommon.GetPMPayrollresults(ref paramStartDate, ref paramEndDate);
            Session["StartDate"] = paramStartDate;
            Session["EndDate"] = paramEndDate;

            BiweeklyGridResults();
            LoadDDL();
            YearlyGridResults();
        }


        private void BiweeklyGridResults(Dictionary<string, string> filters = null)
        {
            string SQLstring, ParamStartDate = string.Empty, ParamEndDate = string.Empty, ParamTrainer = string.Empty;
            DataTable dtUnion = new DataTable();
            List<DataTable> LstDT = new List<DataTable>();
            
            LstDT = Common.CloneList<DataTable>((List<DataTable>)Session["BiweekTable"]);
                        
            SQLstring = string.Format("SELECT MIN(WeekEnding) FROM PAYOUTPMCheckDates WHERE WeekEnding >=  '{0}'", Session["StartDate"]);
            
            if (filters != null)
            {
                string yearDate = null;
                filters.TryGetValue("CheckDate", out yearDate);
                if (!string.IsNullOrWhiteSpace(yearDate))
                {
                    GetWEYearDates(ref ParamStartDate, ref ParamEndDate, int.Parse(yearDate));
                }
            }
            else
            {
                ParamStartDate = Queries.GetResultsFromQueryString(SQLstring).Rows[0][0].ToString();
                ParamEndDate = Session["EndDate"].ToString();
                ParamTrainer = (string)Session["Trainer"] ?? "";
            }

            
            List<DataTable> DTsToExclude = new List<DataTable>();
            foreach (DataTable dt in LstDT)
            {
                if (Convert.ToDateTime(dt.Rows[0]["CheckDate"].ToString()) < Convert.ToDateTime(ParamStartDate) || 
                    Convert.ToDateTime(dt.Rows[0]["CheckDate"].ToString()) > Convert.ToDateTime(ParamEndDate)) 
                    DTsToExclude.Add(dt);                
            }
            
            foreach (DataTable dt in DTsToExclude)
            {
                LstDT.Remove(dt);
            }


            //Remove Trainers different from chosen
            if (ParamTrainer != "" && ParamTrainer != "All")
            {
                LstDT = LstDT.Select(f1 => f1.AsEnumerable().Where(f2 => f2.Field<string>("TrainerName") == ParamTrainer).CopyToDataTable()).ToList();                
            }
            

            if (LstDT.Count > 0)
            {
                dtUnion = Common.UnionAllDatatables(LstDT);

                dtUnion.Columns["TrainerName"].ColumnName = "Trainer";

                if (filters != null)
                {
                    foreach (KeyValuePair<string, string> item in filters)
                    {
                        if (item.Key != "CheckDate")
                            dtUnion = dtUnion.AsEnumerable().Where(f => f.Field<string>(item.Key) == item.Value).CopyToDataTable();
                    }
                }
            }
            
            GRVBiWeekly.DataSource = dtUnion;
            GRVBiWeekly.DataBind();

            GRVBiWeeklyTotals.DataSource = totalizeBiWeeklyResults(dtUnion);
            GRVBiWeeklyTotals.DataBind();

            Common.RightAlignCurrencyFormat(ref GRVBiWeekly);       
        }

        private void YearlyGridResults(string selectedYear = "All")
        {
            //GridView grv;
            DataTable dtUnion = new DataTable();
            List<DataTable> LstDT = new List<DataTable>();

            string SQLstring = @"SELECT YearBalance
                                FROM PAYOUTPMCheckDates
                                GROUP BY YearBalance";

            DataTable SQL_DTResult = Queries.GetResultsFromQueryString(SQLstring);

            if (Session["YearTable"] == null)
            {
                for (int i = SQL_DTResult.Rows.Count - 1; i >= 0; i--)
                    LstDT.Add(SummaryYearly((int)SQL_DTResult.Rows[i][0]));
                Session["YearTable"] = Common.UnionAllDatatables(LstDT);
            }

            dtUnion = ((DataTable)Session["YearTable"]).Copy();

            if (dtUnion.Columns.Contains("TrainerName"))
                dtUnion.Columns["TrainerName"].ColumnName = "Trainer";

            if (selectedYear != "All")
                dtUnion = dtUnion.AsEnumerable().Where(f => f.Field<string>("CheckDate") == selectedYear).CopyToDataTable();

            GRVYearly.DataSource = dtUnion;
            GRVYearly.DataBind();

            GRVYearlyTotals.DataSource = totalizeYearlyResults(dtUnion);
            GRVYearlyTotals.DataBind();

            Common.RightAlignCurrencyFormat(ref GRVYearly);
        }

        private DataTable totalizeBiWeeklyResults(DataTable dt)
        {

            //Validate if Columns type are decimal, if not, will be switched
            PMCommon.FormatColumnTypes(dt);

            dt.Columns.Add("Id", typeof(string));

            dt = dt.AsEnumerable()
            .Where(x => x.Field<string>("Trainer") != "") //Remove white rows
            .GroupBy(x => x.Field<string>("Id"))
            .Select
            (
                n => new
                {
                    CheckDate = "",
                    Trainer = "Total",
                    Manager = "",
                    Role = "",
                    Salary = n.Sum(z => z.Field<decimal?>("Salary") == null ? 0 : z.Field<decimal?>("Salary")),
                    Override = n.Sum(z => z.Field<decimal?>("Override") == null ? 0 : z.Field<decimal?>("Override")),
                    Commission = n.Sum(z => z.Field<decimal?>("Commission") == null ? 0 : z.Field<decimal?>("Commission")),
                    OverPay = n.Sum(z => z.Field<decimal?>("Over Pay") == null ? 0 : z.Field<decimal?>("Over Pay")),
                    UnderPay = n.Sum(z => z.Field<decimal?>("Under Pay") == null ? 0 : z.Field<decimal?>("Under Pay"))
                }
            )
            .ToList().ToDataTable();

            return dt;
        }        


        private DataTable totalizeYearlyResults(DataTable dt)
        {
            dt.Columns.Add("Id", typeof(string));

            dt = dt.AsEnumerable()
            .Where(x => x.Field<string>("Trainer") != "") //Remove white rows
            .GroupBy(x => x.Field<string>("Id"))
            .Select
            (
                n => new
                {
                    CheckDate = "",
                    Trainer = "Total",
                    Manager = "",
                    Role = "",
                    YearEndBalance = n.Sum(z => z.Field<double?>("Year-End Balance") == null ? 0 : z.Field<double?>("Year-End Balance")),
                    QuarterlyTrueUpBonus = n.Sum(z => z.Field<double?>("Quarterly True-Up Bonus") == null ? 0 : z.Field<double?>("Quarterly True-Up Bonus")),
                    RemainingBalance = n.Sum(z => z.Field<double?>("Remaining Balance") == null ? 0 : z.Field<double?>("Remaining Balance"))
                }
            )
            .ToList().ToDataTable();

            return dt;
        }                
        
        protected void GRVBiWeekly_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridView grv = (GridView)sender;
            
            Session["ParamStartDate"] = Common.ApplyDateFormat(Convert.ToDateTime(grv.SelectedRow.Cells[0].Text).AddDays(-13));
            Session["ParamEndDate"] = Common.ApplyDateFormat(grv.SelectedRow.Cells[0].Text);
            Session["Trainer"] = grv.SelectedRow.Cells[1].Text;

            Response.Redirect("GridRoleCommissionDaily.aspx", true);
        }

        protected void Apply_Click(object sender, EventArgs e)
        {
            if (Convert.ToDateTime(webEndDate.SelectedItem.Text) < Convert.ToDateTime(webStartDate.SelectedItem.Text))
            {
                webStartDate.SelectedValue = webStartDate.Items.FindByText(Common.ApplyDateFormat(Session["StartDate"].ToString(), 1)).Value;
                webEndDate.SelectedValue = webEndDate.Items.FindByText(Common.ApplyDateFormat(Session["EndDate"].ToString(), 1)).Value;

                LabelValidation.InnerText = "Date selection is not allowed";
            }
            else
            {
                Session["StartDate"] = webStartDate.SelectedItem.Text;
                Session["EndDate"] = webEndDate.SelectedItem.Text;
                Session["Trainer"] = webTrainer.SelectedItem.Text;
                Session["ddlYear"] = ddlYear.SelectedItem.Text;

                LabelValidation.InnerText = "";
            }

            BiweeklyGridResults();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "TabChosen = \"TabBiWeekButton\";", true);
        }
        

        private DataTable SummaryYearly(int yearDate)
        {
            string ParamStartDate = null;
            string ParamEndDate = null;

            GetWEYearDates(ref ParamStartDate, ref ParamEndDate, yearDate);

            DataTable dt = PMCommon.SummaryBiweekly(ParamStartDate, ParamEndDate);


            dt.Columns.Add("Year-End Balance", typeof(double));
            dt.Columns.Add("Quarterly True-Up Bonus", typeof(double));
            dt.Columns.Add("Remaining Balance", typeof(double));

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i < dt.Rows.Count - 1)
                {
                    dt.Rows[i][0] = yearDate;

                    dt.Rows[i]["Year-End Balance"] =
                    (dt.Rows[i]["Over Pay"].ToString() == "" ? 0 : double.Parse(dt.Rows[i]["Over Pay"].ToString())) +
                    (dt.Rows[i]["Under Pay"].ToString() == "" ? 0 : double.Parse(dt.Rows[i]["Under Pay"].ToString()));

                    
                    if ((double)dt.Rows[i]["Year-End Balance"] >= 0)
                        dt.Rows[i]["Quarterly True-Up Bonus"] = dt.Rows[i]["Year-End Balance"];
                    else
                        dt.Rows[i]["Remaining Balance"] = dt.Rows[i]["Year-End Balance"];
                }
            }

            dt.Columns.Remove("Salary");
            dt.Columns.Remove("Override");
            dt.Columns.Remove("Commission");
            dt.Columns.Remove("Over Pay");
            dt.Columns.Remove("Under Pay");

            return dt;
        }

        private void GetWEYearDates(ref string ParamStartDate, ref string ParamEndDate, int yearDate)
        {
            string SQLstring = string.Format(@" SELECT COALESCE(
                                                (
	                                                SELECT DATEADD(DAY, 1, MAX(WeekEnding))
	                                                FROM PAYOUTPMCheckDates
	                                                WHERE YearBalance = {0} - 1
                                                ),
                                                (
	                                                SELECT DATEADD(DAY, -14, MIN(WeekEnding))
	                                                FROM PAYOUTPMCheckDates
	                                                WHERE YearBalance = {0}
                                                ))", yearDate);
            ParamStartDate = Queries.GetResultsFromQueryString(SQLstring).Rows[0][0].ToString();

            SQLstring = string.Format(@"SELECT MAX(WeekEnding)
                                        FROM PAYOUTPMCheckDates
                                        WHERE YearBalance = {0}", yearDate);
            ParamEndDate = Queries.GetResultsFromQueryString(SQLstring).Rows[0][0].ToString();
        }
        
        private void LoadDDL()
        {
            string DateFormatFixed;
                        
            string SQLString = @"SELECT Id, CONVERT(NVARCHAR, CONVERT(DATE, WeekEnding), 101) [Week Ending] FROM PAYOUTPMCheckDates ORDER BY WeekEnding DESC";

            DataTable dt = Queries.GetResultsFromQueryString(SQLString);

            webStartDate.DataSource = dt;
            webStartDate.DataTextField = "Week Ending";
            webStartDate.DataValueField = "Id";
            webStartDate.DataBind();
            DateFormatFixed = Common.ApplyDateFormat(Session["StartDate"].ToString(), 2);
            webStartDate.SelectedValue = webStartDate.Items.FindByText(DateFormatFixed).Value;

            webEndDate.DataSource = dt;
            webEndDate.DataTextField = "Week Ending";
            webEndDate.DataValueField = "Id";
            webEndDate.DataBind();
            DateFormatFixed = Common.ApplyDateFormat(Session["EndDate"].ToString(), 2);
            webEndDate.SelectedValue = webEndDate.Items.FindByText(DateFormatFixed).Value;


            SQLString = string.Format(
                @"SELECT 0 ID, 'All' Trainer 
                UNION ALL 
                SELECT Id, [Firstname] + ' ' + [Lastname] [Trainer] 
                FROM [PAYOUTtrainer] 
                WHERE [Active] = 1 
                {0}
                ORDER BY [Trainer]", userType == "Trainer" ? string.Format("AND EmailAddress = '{0}'", SQLString, user) : "");            

            dt = Queries.GetResultsFromQueryString(SQLString);

            webTrainer.DataSource = dt;
            webTrainer.DataTextField = "Trainer";
            webTrainer.DataValueField = "Id";
            webTrainer.DataBind();
            webTrainer.SelectedValue = "0";


            SQLString = "SELECT 'All' [YearBalance] UNION ALL SELECT CONVERT(NVARCHAR(4), [YearBalance]) [YearBalance] FROM [PAYOUTPMCheckDates] GROUP BY [YearBalance] ORDER BY [YearBalance] DESC";
            dt = Queries.GetResultsFromQueryString(SQLString);

            ddlYear.DataSource = dt;
            ddlYear.DataTextField = "YearBalance";
            ddlYear.DataValueField = "YearBalance";
            ddlYear.DataBind();
            ddlYear.SelectedValue = dt.Rows[0][0].ToString();
        }

        protected void GRVBiWeekly_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            List<string> DecimalCells = new List<string> { "Salary", "Override", "Commission", "Over Pay", "Under Pay" };
            CellFormating(ref sender, ref e, DecimalCells);

            GridView grv = (GridView)sender;
            e.Row.Attributes["ondblclick"] = ClientScript.GetPostBackClientHyperlink(grv, "Select$" + e.Row.RowIndex);
        }

        protected void GridViewYear_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            List<string> DecimalCells = new List<string> { "Year-End Balance", "Quarterly True-Up Bonus", "Remaining Balance" };
            CellFormating(ref sender, ref e, DecimalCells);
        }

        protected void GRVBiWeeklyTotals_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            List<string> DecimalCells = new List<string> { "Salary", "Override", "Commission", "OverPay", "UnderPay" };
            CellFormating(ref sender, ref e, DecimalCells, false);
        }

        protected void GRVYearlyTotals_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            List<string> DecimalCells = new List<string> { "YearEndBalance", "QuarterlyTrueUpBonus", "RemainingBalance" };
            CellFormating(ref sender, ref e, DecimalCells, false);
        }

        private void CellFormating(ref object sender, ref GridViewRowEventArgs e, List<string> DecimalCells, bool ColumnCategory = true)
        {
            if(e.Row.Cells.Count > 1)
            {
                CellFormat(ref e);

                if (ColumnCategory && e.Row.Cells[1].Text != "&nbsp;")
                    e.Row.Cells[1].CssClass = "ColumnCategory";

                if (e.Row.RowType != DataControlRowType.Header)
                {
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if ((e.Row.Cells[i]).Text != "&nbsp;")
                        {
                            if (DecimalCells.Contains(((BoundField)((DataControlFieldCell)((e.Row.Cells[i]))).ContainingField).DataField))
                            {
                                decimal result = 0;
                                decimal.TryParse((e.Row.Cells[i]).Text, out result);
                                (e.Row.Cells[i]).Text = result.ToString("C");

                                if (result < 0)
                                    (e.Row.Cells[i]).Style.Add("color", "Red");

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
        }
        

        protected void Weekending_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            BiweeklyGridResults();            
            Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "TabChosen = \"TabCheckPeriodButton\";", true);
        }
        
        private void CellFormat(ref GridViewRowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (((DataControlFieldCell)e.Row.Cells[i]).ContainingField.HeaderText == "Trainer")
                    if (e.Row.Cells[i].Text == "&nbsp;")
                        e.Row.CssClass = "EmptyRow";                
            }
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {

            YearlyGridResults(((ListControl)sender).SelectedValue);
        }


        protected void exportBtn_Click(object sender, EventArgs e)
        {
            List<string> ColumnsToRemove = new List<string>();

            ColumnsToRemove = new List<string>() { "TrainerId", "OTPremium", "VacationPay", "TravelTime", "ClawBack", "Adjustment", "AdjustmentComment", "PayCheck"};
            List<DataTable> lstDTBiWeek = new List<DataTable>();
            lstDTBiWeek.Add(Common.UnionAllDatatables(((List<DataTable>)Session["BiweekTable"])));
            RemoveColumnsToExportTable(ref lstDTBiWeek, ColumnsToRemove);

            ColumnsToRemove = new List<string>() { "TrainerId", "OTPremium", "VacationPay", "TravelTime", "ClawBack", "Adjustment", "AdjustmentComment", "PayCheck", "Id" };
            List<DataTable> lstDTYear = new List<DataTable>() { ((DataTable)Session["YearTable"]).Copy() };
            RemoveColumnsToExportTable(ref lstDTYear, ColumnsToRemove);


            Common.ExportToExcel_List(lstDTBiWeek, "RoadShow PM Cummulative");
        }

        private void RemoveColumnsToExportTable(ref List<DataTable> LstDT, List<string> ColumnsToRemove)
        {
            foreach (string col in ColumnsToRemove)
            {
                if(LstDT[0].Columns.Contains(col))
                    LstDT[0].Columns.Remove(col);
            }            
        }
        
        

        private void ShowValidation()
        {
            LabelValidation.InnerText = "Date selection is not allowed";         
        }

    }

    

    public class weekending
    {
        public int id { get; set; }
        public string WeekEnding { get; set; }
    }
}