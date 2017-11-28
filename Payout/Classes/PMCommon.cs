using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace WebApplication3
{
    public class PMCommon
    {
        /// <summary>
        /// Get full list PMPayroll from range in table CheckDates
        /// </summary>
        /// <param name="paramStartDate">Get the lower value from range of existing check dates</param>
        /// <param name="paramEndDate">Get the Higher value from range of existing check dates</param>
        /// <returns></returns>
        public static List<DataTable> GetPMPayrollresults(ref string paramStartDate, ref string paramEndDate)
        {
            List<weekending> lstWE = GetWeekendingList(ref paramStartDate, ref paramEndDate);

            List<DataTable> LstDT = new List<DataTable>();

            string SQLString = "SELECT [CheckDate] FROM [PAYOUTPMPayroll] GROUP BY [CheckDate] ORDER BY [CheckDate] DESC";
            List<DateTime> lstCheckDates = Queries.GetResultsFromQueryString(SQLString).AsEnumerable().Select(f => f.Field<DateTime>("CheckDate")).ToList();


            if (lstCheckDates.Count > 0)
                lstWE = lstWE.Where(w => Convert.ToDateTime(w.WeekEnding) > lstCheckDates.Max()).ToList();

            //Calculate results
            for (int i = 0; i < lstWE.Count; i++)
            {
                if (i < lstWE.Count - 1)
                {
                    LstDT.Add(SummaryBiweekly(
                    Convert.ToDateTime(lstWE[i + 1].WeekEnding).AddDays(1).ToShortDateString(),
                    lstWE[i].WeekEnding
                    ));
                }
                else
                {
                    LstDT.Add(SummaryBiweekly(
                    Convert.ToDateTime(lstWE[i].WeekEnding).AddDays(-14).ToShortDateString(),
                    lstWE[i].WeekEnding
                    ));
                }
            }

            //Get PM Payroll results
            SQLString = @"  SELECT CONVERT(NVARCHAR(20), CheckDate) CheckDate, TrainerId, TrainerName, Manager, Role, Salary, Override, Commission, OverPay [Over Pay], UnderPay [Under Pay],
                                   [OTPremium],[VacationPay],[TravelTime],[ClawBack],[Adjustment],[AdjustmentComment],[PayCheck]
                            FROM PAYOUTPMPayroll";
            DataTable dt = Queries.GetResultsFromQueryString(SQLString);

            foreach (DateTime date in lstCheckDates)
            {
                DataTable dtPayrollByDate = dt.AsEnumerable().Where(f => Convert.ToDateTime(f.Field<string>("CheckDate")) == date).CopyToDataTable();
                dtPayrollByDate.Rows.Add();
                LstDT.Add(dtPayrollByDate);
            }

            return Common.CloneList<DataTable>(LstDT);
        }
        
        public static DataTable SummaryBiweekly(string ParamStartDate, string ParamEndDate)
        {
            Dictionary<string, string> SQLParameters = new Dictionary<string, string>();
            SQLParameters.Add("@webStartDate", ParamStartDate);
            SQLParameters.Add("@webEndDate", ParamEndDate);

            DataTable dt = Queries.GetResultsFromStoreProcedure("spx_PAYOUTsummaryCumulativePM", ref SQLParameters).Tables[0];

            //Adding Row Column Date
            dt.Columns.Add("CheckDate", typeof(string)).SetOrdinal(0);

            dt.Columns.Add("Over Pay", typeof(decimal));
            dt.Columns.Add("Under Pay", typeof(decimal));

            dt.Columns.Add("OTPremium", typeof(decimal));
            dt.Columns.Add("VacationPay", typeof(decimal));
            dt.Columns.Add("TravelTime", typeof(decimal));
            dt.Columns.Add("ClawBack", typeof(decimal));
            dt.Columns.Add("Adjustment", typeof(decimal));
            dt.Columns.Add("AdjustmentComment", typeof(string));
            dt.Columns.Add("PayCheck", typeof(decimal));


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i][0] = Common.ApplyDateFormat(ParamEndDate);

                decimal total =
                    (dt.Rows[i]["Override"].ToString() == "" ? 0 : decimal.Parse(dt.Rows[i]["Override"].ToString())) +
                    (dt.Rows[i]["Commission"].ToString() == "" ? 0 : decimal.Parse(dt.Rows[i]["Commission"].ToString())) -
                    (dt.Rows[i]["Salary"].ToString() == "" ? 0 : decimal.Parse(dt.Rows[i]["Salary"].ToString()));

                if (total < 0)
                {
                    dt.Rows[i]["Over Pay"] = total;//.ToString("#.##");
                }
                else
                {
                    dt.Rows[i]["Under Pay"] = total;//.ToString("#.##");
                }
            }

            dt.Rows.Add();

            FormatColumnTypes(dt);
            return dt;
            //Check how to fix header with multiple nested tables
            //h_ttp://stackoverflow.com/questions/30079298/align-scrollable-table-columns-with-header-in-css
            //h_ttp://jsfiddle.net/gfy4pwrr/6/

            //Scrollable header
            //h_ttp://jsfiddle.net/T9Bhm/7/
            //h_ttp://jsfiddle.net/7UZA4/1385/
            //h_ttps://codepen.io/ajkochanowicz/pen/KHdih
            //h_ttp://jsfiddle.net/emn13/YMvk9/
        }

        public static void FormatColumnTypes(DataTable dt)
        {
            List<string> ColumnsToValidate = new List<string>() { "Salary", "Override", "Commission", "Over Pay", "Under Pay" };
            foreach (string Column in ColumnsToValidate)
            {
                if (dt.Columns[Column].DataType != typeof(decimal))
                {
                    int ColumnIndex = dt.Columns[Column].Ordinal;

                    dt.Columns[Column].ColumnName = Column + "Temp";
                    dt.Columns.Add(Column, typeof(decimal));

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i][Column] = dt.Rows[i][Column + "Temp"];
                    }

                    dt.Columns.Remove(Column + "Temp");

                    dt.Columns[Column].SetOrdinal(ColumnIndex);
                }
            }
        }

        private static List<weekending> GetWeekendingList(ref string paramStartDate, ref string paramEndDate)
        {
            string SQLstring;
            List<weekending> lstWE = new List<weekending>();
            //string StartDate, EndDate;


            SQLstring = @"SELECT Id, WeekEnding 
                        FROM PAYOUTPMCheckDates 
                        ORDER BY WeekEnding DESC";

            DataTable dt = Queries.GetResultsFromQueryString(SQLstring);

            foreach (DataRow dr in dt.Rows)
            {
                lstWE.Add(new weekending()
                {
                    id = int.Parse(dr["Id"].ToString()),
                    WeekEnding = dr["Weekending"].ToString()
                }
                        );
            }

            //Current year range
            SQLstring = @"  SELECT MIN(WeekEnding) MinWE, MAX(WeekEnding) MaxWE
                            FROM PAYOUTPMCheckDates 
                            WHERE YearBalance = (SELECT DATEPART(YEAR,GETDATE()))";

            paramStartDate = Queries.GetResultsFromQueryString(SQLstring).Rows[0][0].ToString();
            paramEndDate = Queries.GetResultsFromQueryString(SQLstring).Rows[0][1].ToString();

            //Session["StartDate"] = StartDate;
            //Session["EndDate"] = EndDate;

            return lstWE;
        }
                
    }
}