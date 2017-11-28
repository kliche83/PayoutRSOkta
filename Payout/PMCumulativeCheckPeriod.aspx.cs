using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication3
{
    public partial class PMCumulativeCheckPeriod : System.Web.UI.Page
    {
        Dictionary<string, string> DFilters = null;
        string user = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            string user = string.Empty;

            try
            {
                user = Session["user"].ToString();
                FillControls();
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }
        }

        private void FillControls()
        {
            MessageAlert.Visible = false;
            if (!IsPostBack)
            {
                Session["DDLShow"] = "Open";
                Session["DateExistInPayroll"] = false;


                //Session is only empty at this point if this page is reloaded and is not bringing values from previous page PMCumulativeSummary
                //Or new CheckDate was created and is attemping to refresh data
                if (Session["BiweekTable"] == null)
                {
                    string paramStartDate = "", paramEndDate = "";

                    List<DataTable> lstResults = PMCommon.GetPMPayrollresults(ref paramStartDate, ref paramEndDate);

                    if (lstResults != null && lstResults.Count > 0)
                    {
                        Session["BiweekTable"] = lstResults;
                    }
                }


                if (Session["BiweekTable"] != null)
                {
                    FirstLoadPaycheckList(UnionAllDatatables((List<DataTable>)Session["BiweekTable"]));
                    LoadDDL();
                    SetSession_PeriodIsOpenOrClosed();
                    CheckPeriodResults(ddlCheckPeriod.SelectedItem.Text);

                    FindOpenCheckDates();
                }
                else
                {
                    Response.Redirect("/");
                }
            }
        }

        private void FindOpenCheckDates()
        {
            string SQLstring = @"SELECT 
                                COUNT(*) OpenCheckDates,
                                (SELECT MAX(WeekEnding)  FROM [Payout].[dbo].[PAYOUTPMCheckDates]) LastClosed
                                FROM [Payout].[dbo].[PAYOUTPMCheckDates] WHERE [ClosedPeriodDate] IS NULL";

            DataTable dt = Queries.GetResultsFromQueryString(SQLstring);
            if (dt.Rows[0][0].ToString() == "0")
            {
                ExistOpenCheckDate.Value = "True";
                NextCheckDate.Value = dt.Rows[0][1].ToString();
            }
            else
            {
                ExistOpenCheckDate.Value = "False";
                NextCheckDate.Value = "";                
            }            
        }

        protected void GRVCheckPeriod_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if(e.Row.Cells.Count > 1) //Row with only 1 column means message "Empty results"
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (((DataControlFieldCell)e.Row.Cells[i]).ContainingField.HeaderText == "Trainer")
                    {
                        if (e.Row.Cells[i].Text == "&nbsp;")
                        {
                            e.Row.CssClass = "EmptyRow";
                        }
                    }


                    string ColumnName = (((DataControlFieldCell)(e.Row.Cells[i])).ContainingField).HeaderText;

                    if (ColumnName == "OTPremium" ||
                        ColumnName == "VacationPay" ||
                        ColumnName == "TravelTime" ||
                        ColumnName == "ClawBack" ||
                        ColumnName == "Adjustment" ||
                        ColumnName == "AdjustmentComment")
                    {
                        //if (Session["userType"] == null || Session["userType"].ToString() != "Admin")
                        //{
                            if ((Session["DDLShow"] == null || (string)Session["DDLShow"] == "Close"))
                            {
                                //This means if we are not in an empty line
                                if (e.Row.Cells[i].Text != "&nbsp;")
                                    e.Row.Cells[i].Enabled = false;
                            }
                            else
                            {
                                e.Row.Cells[i].Enabled = true;
                            }
                        //}
                        //else
                        //{
                        //    e.Row.Cells[i].Enabled = true;
                        //}                        
                    }
                }
            }                
        }

        protected void FieldChanged(Object sender, EventArgs e)
        {            
            if (ddlCheckPeriod.SelectedValue == "0" || ddlCheckPeriod.SelectedValue == "")
            {
                Session["DDLShow"] = "Close";
                CheckPeriodResults();
            }
            else
            {
                DFilters = new Dictionary<string, string>();


                int CurrentRowIndex = ((GridViewRow)((Control)sender).Parent.BindingContainer).DataItemIndex;

                //Get current CheckDate and Trainer From Row
                for (int i = 0; i < 2; i++)
                {
                    DFilters.Add(
                                    ((BoundField)((DataControlFieldCell)(GRVCheckPeriod.Rows[CurrentRowIndex].Cells[i])).ContainingField).DataField,
                                    GRVCheckPeriod.Rows[CurrentRowIndex].Cells[i].Text
                                  );
                }

                decimal result;

                string HeaderText = ((DataControlFieldCell)((Control)sender).Parent).ContainingField.HeaderText;

                if (HeaderText == "OTPremium" ||
                    HeaderText == "VacationPay" ||
                    HeaderText == "TravelTime" ||
                    HeaderText == "ClawBack" ||
                    HeaderText == "Adjustment")
                {
                    if (decimal.TryParse(((TextBox)sender).Text, out result))
                        DFilters.Add(((DataControlFieldCell)((Control)sender).Parent).ContainingField.HeaderText, ((TextBox)sender).Text);
                    else
                        DFilters.Add(((DataControlFieldCell)((Control)sender).Parent).ContainingField.HeaderText, "0");
                }
                else
                {
                    DFilters.Add(((DataControlFieldCell)((Control)sender).Parent).ContainingField.HeaderText, ((TextBox)sender).Text);
                }

                //if (decimal.TryParse(((TextBox)sender).Text, out result))
                //    DFilters.Add(((DataControlFieldCell)((Control)sender).Parent).ContainingField.HeaderText, ((TextBox)sender).Text);
                //else
                //    DFilters.Add(((DataControlFieldCell)((Control)sender).Parent).ContainingField.HeaderText, "0");


                SetSession_PeriodIsOpenOrClosed();

                CheckPeriodResults(ddlCheckPeriod.SelectedItem.Text);
            }            
        }

        private void SetSession_PeriodIsOpenOrClosed()
        {
            string SQLstring = string.Format(@"SELECT ClosedPeriodDate
                                                FROM [PAYOUTPMCheckDates]
                                                WHERE WeekEnding = '{0}'", ddlCheckPeriod.SelectedItem.Text);

            string result = Queries.GetResultsFromQueryString(SQLstring).Rows[0][0].ToString();


            if (result != "")
            {
                Session["DDLShow"] = "Close";
                SubmitPayroll.Visible = false;
            }
            else
            {
                Session["DDLShow"] = "Open";
                SubmitPayroll.Visible = true;
                //if (Convert.ToDateTime(result) >= DateTime.Now)
                //    //if (Convert.ToDateTime(result) >= Convert.ToDateTime("2017-04-07"))
                //    Session["DDLShow"] = "Open";
                //else
                //    Session["DDLShow"] = "Close";
            }


            //string result = Queries.GetResultsFromQueryString(SQLstring).Rows[0][0].ToString();

            //if (result == "")
            //{
            //    Session["DDLShow"] = "Close";
            //}
            //else
            //{
            //    if (Convert.ToDateTime(result) >= DateTime.Now)
            //    //if (Convert.ToDateTime(result) >= Convert.ToDateTime("2017-04-07"))
            //        Session["DDLShow"] = "Open";
            //    else
            //        Session["DDLShow"] = "Close";
            //}            
        }

        private void CheckPeriodResults(string filter = null)
        {
            List<PMPaycheck> lstPMPaychecks = (List<PMPaycheck>)Session["CheckPeriodTable"];

            if (DFilters != null)
            {
                string FieldName = DFilters.Where(d => d.Key != "CheckDate" && d.Key != "TrainerName").Select(d => d.Key).FirstOrDefault().ToString();
                string FieldValue = DFilters.Where(d => d.Key != "CheckDate" && d.Key != "TrainerName").Select(d => d.Value).FirstOrDefault().ToString();


                lstPMPaychecks = lstPMPaychecks.Select(row =>
                {
                    if (row.CheckDate == DFilters["CheckDate"] && row.TrainerName == DFilters["TrainerName"])
                    {
                        switch (FieldName)
                        {
                            case "OTPremium":
                                row.OTPremium = decimal.Parse(FieldValue);
                                break;
                            case "VacationPay":
                                row.VacationPay = decimal.Parse(FieldValue);
                                break;
                            case "TravelTime":
                                row.TravelTime = decimal.Parse(FieldValue);
                                break;
                            case "ClawBack":
                                row.ClawBack = decimal.Parse(FieldValue);
                                break;
                            case "Adjustment":
                                row.Adjustment = decimal.Parse(FieldValue);
                                break;
                            case "AdjustmentComment":
                                row.AdjustmentComment = FieldValue;
                                break;
                        }

                        row.Paycheck = row.Salary > 0 ? row.Salary :
                        row.Override + row.Commission + row.OverPay + row.UnderPay + (row.OTPremium ?? 0) + (row.VacationPay ?? 0) + (row.TravelTime ?? 0) + (row.ClawBack ?? 0) + (row.Adjustment ?? 0);                                                
                    }
                    return row;
                }).ToList();                
                
            }


            if (!string.IsNullOrWhiteSpace(filter))
            {
                lstPMPaychecks = new List<PMPaycheck>(lstPMPaychecks.Where(l => l.CheckDate == Common.ApplyDateFormat(filter)).ToList());
            }

            GRVCheckPeriod.DataSource = lstPMPaychecks;
            GRVCheckPeriod.CssClass = "neoGrid DynamicTable scrollableContent";
            GRVCheckPeriod.DataBind();            
        }

        protected void SubmitPayroll_Click(object sender, EventArgs e)
        {
            string SQLstring;
            string SQLstringDates = string.Format(@"SELECT COUNT(*)
                                              FROM[PAYOUTPMPayroll]
                                              WHERE[CheckDate] = '{0}'", GRVCheckPeriod.Rows[0].Cells[0].Text);

            List<PMPaycheck> CheckPeriodToInsertUpdate = ((List<PMPaycheck>)Session["CheckPeriodTable"]).Where(f => f.CheckDate == GRVCheckPeriod.Rows[0].Cells[0].Text).ToList();


            StringBuilder sb = new StringBuilder();
            foreach (PMPaycheck item in CheckPeriodToInsertUpdate)
            {
                if (sb.Length > 0)
                    sb.Append(" UNION ALL");                

                SQLstring = string.Format(@"(SELECT {0} CheckDate, {1} TrainerId, {2} TrainerName, {3} Manager, {4} [Role], {5} Salary, 
                                                        {6} Override, {7} Commission, {8} OverPay, {9} UnderPay,{10}  OTPremium, 
                                                        {11} VacationPay, {12} TravelTime, {13} ClawBack, {14} Adjustment, 
                                                        {15} AdjustmentComment, {16} PayCheck, GETDATE() CreatedOn)",
                                        "'" + item.CheckDate + "'",
                                        item.TrainerId,
                                        "'" + item.TrainerName + "'",
                                        item.Manager == null ? "null" : "'" + item.Manager + "'",
                                        item.Role == null ? "null" : "'" + item.Role + "'",
                                        item.Salary == null ? "null" : item.Salary.Value.ToString("F2"),
                                        item.Override == null ? "null" : item.Override.Value.ToString("F2"),
                                        item.Commission == null ? "null" : item.Commission.Value.ToString("F2"),
                                        item.OverPay == null ? "null" : item.OverPay.Value.ToString("F2"),
                                        item.UnderPay == null ? "null" : item.UnderPay.Value.ToString("F2"),
                                        item.OTPremium == null ? "null" : item.OTPremium.Value.ToString("F2"),
                                        item.VacationPay == null ? "null" : item.VacationPay.Value.ToString("F2"),
                                        item.TravelTime == null ? "null" : item.TravelTime.Value.ToString("F2"),
                                        item.ClawBack == null ? "null" : item.ClawBack.Value.ToString("F2"),
                                        item.Adjustment == null ? "null" : "'" + item.Adjustment + "'",
                                        item.AdjustmentComment == null ? "null" : "'" + item.AdjustmentComment + "'",
                                        item.Paycheck == null ? "null" : item.Paycheck.Value.ToString("F2")
                                        );

                sb.Append(SQLstring);
            }

            //Here we have the table we are going to insert or update
            SQLstring = sb.ToString();

            if (Queries.GetResultsFromQueryString(SQLstringDates).Rows[0][0].ToString() == "0")
            {
                //Insert Values
                SQLstring = @"INSERT INTO PAYOUTPMPayroll
                         (CheckDate, TrainerId, TrainerName, Manager, Role, Salary, Override, Commission, OverPay, 
                        UnderPay, OTPremium, VacationPay, TravelTime, ClawBack, Adjustment, AdjustmentComment, PayCheck, CreatedOn)" + SQLstring;
                                                
                try
                {
                    Queries.ExecuteFromQueryString(SQLstring);
                    SubmitCommissionOverrideSummaries(Convert.ToDateTime(GRVCheckPeriod.Rows[0].Cells[0].Text));
                    
                    SQLstring = string.Format(@"UPDATE PAYOUTPMCheckDates 
                                                SET ClosedPeriodDate = GETDATE(), ClosedBy = {0}
                                                WHERE WeekEnding = '{1}'",
                                                user == null ? "NULL" : "'" + user + "'",
                                                GRVCheckPeriod.Rows[0].Cells[0].Text);
                    Queries.ExecuteFromQueryString(SQLstring);
                    FindOpenCheckDates();
                    Session["BiweekTable"] = null;

                    MessageAlert.InnerText = "Records Inserted successfully";
                    MessageAlert.Attributes["style"] = "color:green; font-weight:bold";
                }
                catch(Exception ex)
                {
                    MessageAlert.InnerText = "Error Inserting Values";
                    MessageAlert.Attributes["style"] = "color:red; font-weight:bold";
                }                
            }
            else
            {
                SQLstringDates = "SELECT COUNT(*) FROM [Payout].[dbo].[PAYOUTPMCheckDates] WHERE [WeekEnding] >= GETDATE()";
                if (Queries.GetResultsFromQueryString(SQLstringDates).Rows[0][0].ToString() == "1")
                {
                    try
                    {
                        SQLstring = string.Format(@"UPDATE p
                                                    SET p.OTPremium = v.OTPremium, 
                                                    p.VacationPay = v.VacationPay, 
                                                    p.TravelTime = v.TravelTime, 
                                                    p.ClawBack = v.ClawBack, 
                                                    p.Adjustment = v.Adjustment, 
                                                    p.AdjustmentComment = v.AdjustmentComment,
                                                    p.PayCheck = v.PayCheck
                                                    FROM PAYOUTPMPayroll p INNER JOIN
                                                    ({0}) v ON p.CheckDate = v.CheckDate AND p.TrainerId = v.TrainerId", SQLstring);
                        Queries.ExecuteFromQueryString(SQLstring);

                        MessageAlert.InnerText = "Records Updated successfully";
                        MessageAlert.Attributes["style"] = "color:green; font-weight:bold";
                    }
                    catch(Exception ex)
                    {
                        MessageAlert.InnerText = "Error Updating Values";
                        MessageAlert.Attributes["style"] = "color:red; font-weight:bold";
                    }
                }
                else
                {
                    MessageAlert.InnerText = "Operation Denied";
                    MessageAlert.Attributes["style"] = "color:red; font-weight:bold";
                }                
            }

            if (ddlCheckPeriod.SelectedValue == "0" || ddlCheckPeriod.SelectedValue == "")
            {
                Session["DDLShow"] = "Close";
                CheckPeriodResults();
            }
            else
            {
                SetSession_PeriodIsOpenOrClosed();
                CheckPeriodResults(ddlCheckPeriod.SelectedItem.Text);
            }

            MessageAlert.Visible = true;
        }


        private void SubmitCommissionOverrideSummaries(DateTime CheckDate)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string SQLstring = string.Format(@";WITH StartDateTable AS
                                                (
                                                SELECT MAX(DATEADD(DAY, 1, [WeekEnding])) [StartDate]
                                                FROM [PAYOUTPMCheckDates]
                                                WHERE [WeekEnding] < '{0}'
                                                )

                                                SELECT CASE WHEN (SELECT [StartDate] FROM StartDateTable) IS NULL 
                                                THEN DATEADD(DAY, -14, (SELECT MIN([WeekEnding]) FROM [PAYOUTPMCheckDates]))
                                                ELSE (SELECT [StartDate] FROM StartDateTable) 
                                                END", CheckDate.ToString());

            string startDate = Common.ApplyDateFormat(Queries.GetResultsFromQueryString(SQLstring).Rows[0][0].ToString());
            string endDate = Common.ApplyDateFormat(CheckDate);
            
            parameters.Add("@webStartDate", startDate);
            parameters.Add("@webEndDate", endDate);
            parameters.Add("@webTrainer", "");
            parameters.Add("@Action", "INSERT");

            Queries.ExecuteFromStoreProcedure("spx_PAYOUTsummaryOverrides", parameters);            
            Queries.ExecuteFromStoreProcedure("spx_PAYOUTsummaryCommissionDaily", parameters);

        }

        protected void ddlCheckPeriod_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ExistOpenCheckDate.Value = "False";
            NextCheckDate.Value = "";

            if (ddlCheckPeriod.SelectedValue == "0" || ddlCheckPeriod.SelectedValue == "")
            {
                Session["DDLShow"] = "Close";
                CheckPeriodResults();
                SubmitPayroll.Visible = false;
            }
            else
            {
                //string SQLstring = string.Format(@"SELECT Id
                //                                FROM PAYOUTPMPayroll
                //                                WHERE [CheckDate] = '{0}'", Weekending.SelectedItem.Text);

                //if (Queries.GetResultsFromQueryString(SQLstring).Rows.Count > 0)
                //    Session["DateExistInPayroll"] = true;
                //else
                //    Session["DateExistInPayroll"] = false;
                                
                SetSession_PeriodIsOpenOrClosed();
                CheckPeriodResults(ddlCheckPeriod.SelectedItem.Text);
            }
        }
        
        private void LoadDDL()
        {
            string SQLString = @"SELECT Id, CONVERT(NVARCHAR, CONVERT(DATE, WeekEnding), 101) [Week Ending] FROM PAYOUTPMCheckDates ORDER BY WeekEnding DESC";

            DataTable dt = Queries.GetResultsFromQueryString(SQLString);
            
            DataRow dr = dt.NewRow();
            dr[0] = 0;
            dr[1] = "All";
            dt.Rows.InsertAt(dr, 0);
            ddlCheckPeriod.DataSource = dt;
            ddlCheckPeriod.DataTextField = "Week Ending";
            ddlCheckPeriod.DataValueField = "Id";
            ddlCheckPeriod.DataBind();
            ddlCheckPeriod.SelectedIndex = 1;
        }

        private void FirstLoadPaycheckList(DataTable DTBiweeklyResults)
        {
            List<List<PMPaycheck>> lstByBiweek = new List<List<PMPaycheck>>();
            List<PMPaycheck> lstPaycheck = new List<PMPaycheck>();
            List<PMPaycheck> lstPaycheckResult = new List<PMPaycheck>();

            //DTBiweeklyResults.Columns["TrainerName"].ColumnName = "Trainer";


            for (int i = 0; i < DTBiweeklyResults.Rows.Count; i++)
            {
                if (DTBiweeklyResults.Rows[i]["TrainerName"].ToString() == "") //If trainer field is empty it means is an empty row
                {
                    lstByBiweek.Add(lstPaycheck);

                    //if (DTBiweeklyResults.Rows[i]["TrainerName"].ToString() == "")
                    //{
                    //    lstByBiweek.Add(lstPaycheck);
                    //}
                    
                    if (i < DTBiweeklyResults.Rows.Count - 1)
                        lstPaycheck = new List<PMPaycheck>();
                }
                else
                {
                    if (i == 0)
                    {
                        lstPaycheck = new List<PMPaycheck>();
                    }

                    PMPaycheck objPMPaycheck = new PMPaycheck()
                    {
                        CheckDate = DTBiweeklyResults.Rows[i]["CheckDate"].ToString(),
                        TrainerId = int.Parse(DTBiweeklyResults.Rows[i]["TrainerId"].ToString()),
                        TrainerName = DTBiweeklyResults.Rows[i]["TrainerName"].ToString(),
                        Manager = DTBiweeklyResults.Rows[i]["Manager"].ToString(),
                        Role = DTBiweeklyResults.Rows[i]["Role"].ToString(),
                        Salary = DTBiweeklyResults.Rows[i]["Salary"].ToString() == "" ? 0 : decimal.Parse(DTBiweeklyResults.Rows[i]["Salary"].ToString()),
                        Override = DTBiweeklyResults.Rows[i]["Override"].ToString() == "" ? 0 : decimal.Parse(DTBiweeklyResults.Rows[i]["Override"].ToString()),
                        Commission = DTBiweeklyResults.Rows[i]["Commission"].ToString() == "" ? 0 : decimal.Parse(DTBiweeklyResults.Rows[i]["Commission"].ToString()),
                        OverPay = DTBiweeklyResults.Rows[i]["Over Pay"].ToString() == "" ? 0 : decimal.Parse(DTBiweeklyResults.Rows[i]["Over Pay"].ToString()),
                        UnderPay = DTBiweeklyResults.Rows[i]["Under Pay"].ToString() == "" ? 0 : decimal.Parse(DTBiweeklyResults.Rows[i]["Under Pay"].ToString()),
                        OTPremium = DTBiweeklyResults.Rows[i]["OTPremium"].ToString() == "" ? 0 : decimal.Parse(DTBiweeklyResults.Rows[i]["OTPremium"].ToString()),
                        VacationPay = DTBiweeklyResults.Rows[i]["VacationPay"].ToString() == "" ? 0 : decimal.Parse(DTBiweeklyResults.Rows[i]["VacationPay"].ToString()),
                        TravelTime = DTBiweeklyResults.Rows[i]["TravelTime"].ToString() == "" ? 0 : decimal.Parse(DTBiweeklyResults.Rows[i]["TravelTime"].ToString()),
                        ClawBack = DTBiweeklyResults.Rows[i]["ClawBack"].ToString() == "" ? 0 : decimal.Parse(DTBiweeklyResults.Rows[i]["ClawBack"].ToString()),
                        Adjustment = DTBiweeklyResults.Rows[i]["Adjustment"].ToString() == "" ? 0 : decimal.Parse(DTBiweeklyResults.Rows[i]["Adjustment"].ToString()),
                        AdjustmentComment = DTBiweeklyResults.Rows[i]["AdjustmentComment"].ToString(),
                        Paycheck = DTBiweeklyResults.Rows[i]["Paycheck"].ToString() == "" ? 0 : decimal.Parse(DTBiweeklyResults.Rows[i]["Paycheck"].ToString())
                    };

                    if(objPMPaycheck.Paycheck == 0)
                        objPMPaycheck.Paycheck = objPMPaycheck.Salary > 0 ? objPMPaycheck.Salary :
                            objPMPaycheck.Override + objPMPaycheck.Commission + objPMPaycheck.OverPay + objPMPaycheck.UnderPay;

                    lstPaycheck.Add(objPMPaycheck);
                }
            }

            //string SQLstring = @"SELECT DISTINCT [CheckDate] FROM [PAYOUTPMPayroll] ORDER BY [CheckDate] DESC";
            //DataTable dt = Queries.GetResultsFromQueryString(SQLstring);

            //foreach (DataRow dr in dt.Rows)
            //{
            //    //Remove existing checkdates which were inserted
            //    lstByBiweek = lstByBiweek.Where(Sublist => Sublist.Any(item => item.CheckDate != Common.ApplyDateFormat(dr[0].ToString(), 1))).ToList();
            //}

            ////GEt Payroll records from Database
            //List<List<PMPaycheck>> DBresults = GetResultsFromDB();
            //foreach (List<PMPaycheck> SubListDBResuts in DBresults)
            //{
            //    lstByBiweek.Add(SubListDBResuts);
            //}
            
            ////Sort Results
            //lstByBiweek = lstByBiweek.OrderByDescending(f => f.Select(sub => Convert.ToDateTime(sub.CheckDate)).FirstOrDefault()).ToList();

            foreach (List<PMPaycheck> ItemlstPaycheck in lstByBiweek)
            {
                ItemlstPaycheck.Add(new PMPaycheck());

                if (lstPaycheckResult.Count == 0)
                {
                    lstPaycheckResult = new List<PMPaycheck>(ItemlstPaycheck);
                }
                else
                {
                    lstPaycheckResult = lstPaycheckResult.Union(ItemlstPaycheck).ToList();
                }
            }

            Session["CheckPeriodTable"] = lstPaycheckResult;
        }

        private List<List<PMPaycheck>> GetResultsFromDB()
        {
            List<List<PMPaycheck>> lstByBiweek = new List<List<PMPaycheck>>();
            List<PMPaycheck> lstPaycheck = new List<PMPaycheck>();

            string SQLstring = @"SELECT CheckDate, TrainerId, TrainerName, Manager, [Role], Salary, [Override], Commission, OverPay, UnderPay, 
                                        OTPremium, VacationPay, TravelTime, ClawBack, Adjustment, AdjustmentComment, PayCheck
                                FROM PAYOUTPMPayroll
                                ORDER BY CheckDate DESC";

            DataTable dtPayroll = Queries.GetResultsFromQueryString(SQLstring);

            SQLstring = @"  SELECT WeekEnding
                            FROM [PAYOUTPMCheckDates]
                            WHERE WeekEnding IN (SELECT DISTINCT CheckDate FROM PAYOUTPMPayroll)
                            ORDER BY WeekEnding DESC";

            DataTable dtWeekEndings = Queries.GetResultsFromQueryString(SQLstring);

            foreach (DataRow dr in dtWeekEndings.Rows)
            {
                DataView dv = dtPayroll.AsEnumerable().Where(f => f.Field<DateTime>("CheckDate") == Convert.ToDateTime(dr[0].ToString())).CopyToDataTable().DefaultView;

                var dv1 = dv[0];      
                foreach (DataRowView item in dv)
                {                    
                    lstPaycheck.Add(new PMPaycheck() {
                        CheckDate = Common.ApplyDateFormat(item["CheckDate"].ToString(), 1),
                        TrainerId = int.Parse(item["TrainerId"].ToString()),
                        TrainerName = item["TrainerName"].ToString(),
                        Manager = item["Manager"].ToString() == "" ? null : item[2].ToString(),
                        Role = item["Role"].ToString() == "" ? null : item[3].ToString(),
                        Salary = item["Salary"].ToString() == "" ? 0 : decimal.Parse(item[4].ToString()),
                        Override = item["Override"].ToString() == "" ? 0 : decimal.Parse(item[5].ToString()),
                        Commission = item["Commission"].ToString() == "" ? 0 : decimal.Parse(item[6].ToString()),
                        OverPay = item["OverPay"].ToString() == "" ? 0 : decimal.Parse(item[7].ToString()),
                        UnderPay = item["UnderPay"].ToString() == "" ? 0 : decimal.Parse(item[8].ToString()),
                        OTPremium = item["OTPremium"].ToString() == "" ? 0 : decimal.Parse(item[9].ToString()),
                        VacationPay = item["VacationPay"].ToString() == "" ? 0 : decimal.Parse(item[10].ToString()),
                        TravelTime = item["TravelTime"].ToString() == "" ? 0 : decimal.Parse(item[11].ToString()),
                        ClawBack = item["ClawBack"].ToString() == "" ? 0 : decimal.Parse(item[12].ToString()),
                        Adjustment = item["Adjustment"].ToString() == "" ? 0 : decimal.Parse(item[13].ToString()),
                        AdjustmentComment = item["AdjustmentComment"].ToString() == "" ? null : item[14].ToString(),
                        Paycheck = item["Paycheck"].ToString() == "" ? 0 : decimal.Parse(item[15].ToString())
                    });                                        
                }
                lstPaycheck.Add(new PMPaycheck());
                lstByBiweek.Add(lstPaycheck);                
            }
            return lstByBiweek;
        }


        protected void exportBtn_Click(object sender, EventArgs e)
        {
            DataTable dt = Common.ToDataTable((List<PMPaycheck>)Session["CheckPeriodTable"]);
            List<DataTable> lstDT = new List<DataTable>();
            lstDT.Add(dt);
            Common.ExportToExcel_List(lstDT, "RoadShow PM Cummulative Payroll");
        }


        private DataTable UnionAllDatatables(List<DataTable> LstDT)
        {
            return LstDT
                    .Select(d => d.Select().AsEnumerable())
                    .Aggregate((current, next) => current.Union(next))
                    .CopyToDataTable<DataRow>();
        }


        [WebMethod()]        
        public static void JSONRequestFromClient(string CheckDate)
        {
            DateTime date = new DateTime();
            DateTime.TryParse(CheckDate, out date);

            if (date.Year > 0001)
            {
                string SQLString = string.Format(@"INSERT INTO PAYOUTPMCheckDates (WeekEnding, YearBalance) VALUES ('{0}', {1})", CheckDate, date.Year);

                Queries.ExecuteFromQueryString(SQLString);
            }
                
            // I want it here!
        }
    }
}