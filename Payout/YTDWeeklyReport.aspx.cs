using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication3
{
    public partial class YTDWeeklyReport : Page
    {
        static string strStoreNameDDL;
        static string strProgramDDL;
        static string strOwnerDDL;
        static string strRotationDDL;
        static string strYearDDL;
        static string SQLstring;
        static List<string> NonDecimalColumns;
        DataTable dtStoreProgOwnerInSales;

        protected void Page_Load(object sender, EventArgs e)
        {
            string user = string.Empty;

            try
            {
                user = Session["user"].ToString();
                
                if (!IsPostBack)
                {
                    strStoreNameDDL = null;
                    YearDDLPopulate();
                }
                                                
                NonDecimalColumns = new List<string>();
                NonDecimalColumns.Add("Owner");
                NonDecimalColumns.Add("StoreNumber");
                NonDecimalColumns.Add("StoreName");
                NonDecimalColumns.Add("Location");

                BindDDL();
                SetRotations();

                if (strYearDDL != YearDDL.SelectedValue)
                {
                    WebStoreNameDDL.SelectedValue = "--Select--";
                    WebProgramDDL.SelectedValue = "--Select--";
                    WebOwnerDDL.SelectedValue = "All";
                    WebRotationDDL.SelectedValue = "All";
                    strYearDDL = YearDDL.SelectedValue;
                }

                if (IsPostBack)
                    BindGrid();                
            }
            catch(Exception ex)
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }
        }                
        
        private void BindDDL()
        {            
            UpdatePanelGrid.Update();

            FillRetailer();

            if (strStoreNameDDL != WebStoreNameDDL.SelectedValue)
            {
                strStoreNameDDL = WebStoreNameDDL.SelectedValue;


                if (!string.IsNullOrWhiteSpace(strStoreNameDDL))
                {
                    List<string> lstPrograms = new List<string>();
                    if (strStoreNameDDL != "Kroger All")
                    {
                        lstPrograms = dtStoreProgOwnerInSales.AsEnumerable()
                                                .Where(f => f.Field<string>("Program") != "" && f.Field<string>("StoreName") == strStoreNameDDL)
                                                .OrderBy(f => f.Field<string>("Program"))
                                                .Select(f => f.Field<string>("Program"))
                                                .Distinct().ToList();
                    }
                    else
                    {
                        lstPrograms = dtStoreProgOwnerInSales.AsEnumerable()
                                                .Where(f => f.Field<string>("StoreName").Contains("Kroger") && f.Field<string>("Program") != "" && f.Field<string>("Program") != null)
                                                .OrderBy(f => f.Field<string>("Program"))
                                                .Select(f => f.Field<string>("Program"))
                                                .Distinct().ToList();
                    }

                    lstPrograms.Insert(0,"--Select--");
                    WebProgramDDL.DataSource = lstPrograms;
                    WebProgramDDL.DataBind();
                }
            }

            if (strStoreNameDDL != WebStoreNameDDL.SelectedValue || strProgramDDL != WebProgramDDL.SelectedValue || strRotationDDL != WebRotationDDL.SelectedValue)
            {
                FillOwner();
            }
            
            strProgramDDL = WebProgramDDL.SelectedValue;
            strOwnerDDL = WebOwnerDDL.SelectedValue;
        }

        private void FillRetailer()
        {
            Session["ReportsToPrint"] = null;

            SQLstring = string.Format(@"SELECT DISTINCT StoreName, Program, [Owner]
                                        FROM
                                        (
	                                        SELECT 'Kroger All' StoreName, NULL Program, NULL [Owner]

	                                        UNION ALL

	                                        SELECT StoreName, Program, OwnerFirstname + ' ' + OwnerLastname [Owner] 
	                                        FROM PAYOUTsales 
	                                        WHERE DATEPART(YEAR, SalesDate) = {0} AND Qty > 0 GROUP BY StoreName, Program, OwnerFirstname, OwnerLastname

	                                        UNION ALL

	                                        SELECT StoreName, Program, OwnerFirstname + ' ' + OwnerLastname [Owner] 
	                                        FROM PAYOUTsalesArchive 
	                                        WHERE DATEPART(YEAR, SalesDate) = {0} AND Qty > 0 GROUP BY StoreName, Program, OwnerFirstname, OwnerLastname
                                        ) tab",
                            YearDDL.SelectedValue);            

            dtStoreProgOwnerInSales = Queries.GetResultsFromQueryString(SQLstring);

            List<string> lstStoreName = dtStoreProgOwnerInSales.AsEnumerable()
                                                            .OrderBy(f => f.Field<string>("StoreName"))
                                                            .Select(f => f.Field<string>("StoreName"))
                                                            .Distinct().ToList();
            lstStoreName.Sort();
            lstStoreName.Insert(0, "--Select--");

            string selectedValue = WebStoreNameDDL.SelectedValue;
            WebStoreNameDDL.DataSource = lstStoreName;
            WebStoreNameDDL.DataBind();
            if (lstStoreName.Contains(selectedValue))
                WebStoreNameDDL.SelectedValue = selectedValue;
        }

        private void FillOwner()
        {
            List<string> lstOwner = new List<string>();
            if (WebProgramDDL.SelectedValue != "--Select--")
            {
                string tempStoreName = strStoreNameDDL;
                if (strStoreNameDDL == "Kroger All")
                    tempStoreName = "Kroger";

                SQLstring = string.Format(@"SELECT DISTINCT OwnerFirstname + ' ' + OwnerLastname [Owner] 
                                FROM PAYOUTsales
                                WHERE 
                                StoreName LIKE '{0}%'
                                AND Program = '{1}'
                                {2}",
                                tempStoreName,
                                WebProgramDDL.SelectedValue,

                                WebRotationDDL.SelectedValue != "All" ? 
                                string.Format(" AND SalesDate BETWEEN (SELECT StartDate FROM PAYOUTwe WHERE WeekEnding = '{0}') AND '{0}'", WebRotationDDL.SelectedValue) :
                                "");

                DataTable dt = Queries.GetResultsFromQueryString(SQLstring);

                lstOwner = dt.AsEnumerable().Select(f => f.Field<string>("Owner")).ToList();
            }
            lstOwner.Insert(0, "All");
            WebOwnerDDL.DataSource = lstOwner;
            WebOwnerDDL.DataBind();                        
        }

        private void YearDDLPopulate()
        {
            SQLstring = @"SELECT DISTINCT SalesDate
                            FROM
                            (
	                            SELECT DISTINCT DATEPART(YEAR, SalesDate) SalesDate
	                            FROM PAYOUTsales
	                            UNION ALL
	                            SELECT DISTINCT DATEPART(YEAR, SalesDate) SalesDate
	                            FROM PAYOUTsalesArchive	
                            ) tab
                            WHERE SalesDate <> 2014
                            ORDER BY SalesDate DESC";

            YearDDL.DataSource = Queries.GetResultsFromQueryString(SQLstring);
            YearDDL.DataValueField = "SalesDate";
            YearDDL.DataBind();

            strYearDDL = YearDDL.SelectedValue;
        }

        private void SetRotations()
        {
            SQLstring = string.Format(@";WITH CTE AS
                                        (
	                                        SELECT SalesDate
	                                        FROM PAYOUTsales
	                                        WHERE StoreName LIKE '{0}%'
	                                        AND program = '{1}'
	                                        AND DATEPART(YEAR, SalesDate) = {2}

	                                        UNION ALL

	                                        SELECT SalesDate
	                                        FROM PAYOUTsalesArchive
	                                        WHERE StoreName LIKE '{0}%'
	                                        AND program = '{1}'
	                                        AND DATEPART(YEAR, SalesDate) = {2}
                                        ) 

                                        SELECT 'All' WeekEnding 
                                        UNION ALL
                                        SELECT DISTINCT CONVERT(NVARCHAR(20), [WeekEnding]) WeekEnding
                                        FROM PAYOUTwe w
                                        INNER JOIN (SELECT SalesDate FROM CTE) 
                                        SalesFiltered ON SalesFiltered.SalesDate BETWEEN w.StartDate AND w.WeekEnding
                                        ORDER BY WeekEnding DESC
                                    ",
                                    WebStoreNameDDL.SelectedValue.Replace("'", "''").Replace("Kroger All", "Kroger"),
                                    WebProgramDDL.SelectedValue.Replace("'", "''"), 
                                    YearDDL.SelectedValue
                                        );

            string selectedValue = WebRotationDDL.SelectedValue;
            DataTable RotationsResult = Queries.GetResultsFromQueryString(SQLstring);
            WebRotationDDL.DataSource = RotationsResult;
            WebRotationDDL.DataValueField = "WeekEnding";
            WebRotationDDL.DataBind();
            if (RotationsResult.AsEnumerable().Where(f => f.Field<string>("WeekEnding").Contains(selectedValue)).Count() > 0)
                WebRotationDDL.SelectedValue = selectedValue;

            strRotationDDL = WebRotationDDL.SelectedValue;
        }

        private void SetOwners()
        {
            SQLstring = string.Format(@";WITH CTE AS
                                        (
	                                        SELECT SalesDate
	                                        FROM PAYOUTsales
	                                        WHERE StoreName LIKE '{0}%'
	                                        AND program = '{1}'
	                                        AND DATEPART(YEAR, SalesDate) = {2}

	                                        UNION ALL

	                                        SELECT SalesDate
	                                        FROM PAYOUTsalesArchive
	                                        WHERE StoreName LIKE '{0}%'
	                                        AND program = '{1}'
	                                        AND DATEPART(YEAR, SalesDate) = {2}
                                        ) 

                                        SELECT 'All' WeekEnding 
                                        UNION ALL
                                        SELECT DISTINCT CONVERT(NVARCHAR(20), [WeekEnding]) WeekEnding
                                        FROM PAYOUTwe w
                                        INNER JOIN (SELECT SalesDate FROM CTE) 
                                        SalesFiltered ON SalesFiltered.SalesDate BETWEEN w.StartDate AND w.WeekEnding
                                        ORDER BY WeekEnding DESC
                                    ",
                                    WebStoreNameDDL.SelectedValue.Replace("'", "''").Replace("Kroger All", "Kroger"),
                                    WebProgramDDL.SelectedValue.Replace("'", "''"),
                                    YearDDL.SelectedValue
                                        );

            string selectedValue = WebRotationDDL.SelectedValue;
            DataTable RotationsResult = Queries.GetResultsFromQueryString(SQLstring);
            WebRotationDDL.DataSource = RotationsResult;
            WebRotationDDL.DataValueField = "WeekEnding";
            WebRotationDDL.DataBind();
            if (RotationsResult.AsEnumerable().Where(f => f.Field<string>("WeekEnding").Contains(selectedValue)).Count() > 0)
                WebRotationDDL.SelectedValue = selectedValue;

            strRotationDDL = WebRotationDDL.SelectedValue;
        }

        private void BindGrid()
        {
            UpdatePanelGrid.Update();
            if(WebStoreNameDDL.SelectedValue != "--Select--" &&
                WebProgramDDL.SelectedValue != "--Select--")
            {
                List<DataTable> lstdtResults = BuildReport();
                lstdtResults.Add(BuildSummaryReport());

                GRVSummary.DataSource = lstdtResults[2];
                GRVSummary.DataBind();

                if (lstdtResults[0].AsEnumerable().Where(f => f.Field<string>("Owner") != null).ToList().Count() == 0)
                {
                    GRVQty.DataSource = lstdtResults[0];
                    GRVQty.DataBind();

                    GRVRev.DataSource = lstdtResults[1];
                    GRVRev.DataBind();
                }
                else
                {
                    GRVQty.DataSource = SortSalesByTotal(lstdtResults[0]);
                    GRVQty.DataBind();

                    GRVRev.DataSource = SortSalesByTotal(lstdtResults[1]);
                    GRVRev.DataBind();
                }

                Session["ReportsToPrint"] = lstdtResults;
            }
            else
            {                
                GRVSummary.DataSource = null;
                GRVSummary.DataBind();
                GRVQty.DataSource = null;
                GRVQty.DataBind();
                GRVRev.DataSource = null;
                GRVRev.DataBind();
            }
        }
        
        private DataTable SortSalesByTotal(DataTable dt)
        {
            List<DataTable> lstResults = new List<DataTable>();
            DataTable dtToSort = dt;
            dtToSort.Columns.Add("MergeColumn");
            for (int i = 0; i < dt.Rows.Count; i++)
                dtToSort.Rows[i]["MergeColumn"] = i;

            DataView dv1 = dtToSort.AsEnumerable().Where(f => f.Field<string>("Owner") != null).ToList().CopyToDataTable().DefaultView;
            DataView dv2 = dtToSort.AsEnumerable().Where(f => f.Field<string>("Owner") == null).ToList().CopyToDataTable().DefaultView;
            dv1.Sort = "Total desc";
            lstResults.Add(dv1.ToTable());
            lstResults.Add(dv2.ToTable());

            dtToSort = lstResults.MergeAll("MergeColumn");
            dtToSort.Columns.Remove("MergeColumn");

            return dtToSort;
        }
        
        private static List<DataTable> BuildReport()
        {
            SQLstring = string.Format(@"SELECT StartDate, CONVERT(NVARCHAR(20), [WeekEnding]) WeekEnding FROM PAYOUTweList_fnc('{0}', '{1}') ORDER BY WeekEnding ASC", strRotationDDL, strYearDDL);
            List<DataTable> lstdtRev = new List<DataTable>();
            List<DataTable> lstdtQty = new List<DataTable>();
            List<DataTable> lstdtResults = new List<DataTable>();

            DataTable dt = Queries.GetResultsFromQueryString(SQLstring);
            DataTable dtResults, tempTable;

            if (strRotationDDL == "All")
            {
                foreach (DataRow drRotation in dt.Rows)
                {                    
                    dtResults = SalesReport(drRotation["StartDate"].ToString(), drRotation["WeekEnding"].ToString(), 2, strYearDDL);

                    tempTable = dtResults.Copy();
                    tempTable.Columns.Remove("TotalQty");
                    tempTable.Columns["TotalRev"].ColumnName = drRotation["WeekEnding"].ToString();                    
                    lstdtRev.Add(tempTable);

                    tempTable = dtResults.Copy();
                    tempTable.Columns.Remove("TotalRev");
                    tempTable.Columns["TotalQty"].ColumnName = drRotation["WeekEnding"].ToString();
                    lstdtQty.Add(tempTable);
                }
            }
            else
            {
                DateTime WeekEnding = Convert.ToDateTime(dt.Rows[0]["WeekEnding"].ToString());

                int dayDiff = Convert.ToInt32(
                               (WeekEnding - 
                               Convert.ToDateTime(dt.Rows[0]["StartDate"].ToString())
                               ).TotalDays);
                
                for (int i = dayDiff; i >= 0; i--)
                {
                    dtResults = SalesReport(WeekEnding.AddDays(-i).ToString(), WeekEnding.AddDays(-i).ToString(), 2);

                    tempTable = dtResults.Copy();
                    tempTable.Columns.Remove("TotalQty");
                    tempTable.Columns["TotalRev"].ColumnName = Common.ApplyDateFormat(WeekEnding.AddDays(-i).ToString());
                    lstdtRev.Add(tempTable);

                    tempTable = dtResults.Copy();
                    tempTable.Columns.Remove("TotalRev");
                    tempTable.Columns["TotalQty"].ColumnName = Common.ApplyDateFormat(WeekEnding.AddDays(-i).ToString());
                    lstdtQty.Add(tempTable);
                }

            }

            lstdtResults.Add(MergeTables(lstdtQty));
            lstdtResults.Add(MergeTables(lstdtRev));            

            return lstdtResults;
        }
        
        private DataTable BuildSummaryReport()
        {
            string SQLstring, StartDateRotation, WeekendingRotation, LastSalesDate;
            List<DataTable> lstDt = new List<DataTable>();
            DataTable dtResults;

            string JoinedSalesTables = string.Format(@";WITH CTE AS
                                            (
	                                            SELECT MAX(SalesDate) SalesDate
	                                            FROM PAYOUTsales
	                                            WHERE StoreName LIKE '{0}%'
	                                            AND program = '{1}'
	                                            AND DATEPART(YEAR, SalesDate) = {2}

	                                            UNION ALL

	                                            SELECT MAX(SalesDate) SalesDate
	                                            FROM PAYOUTsalesArchive
	                                            WHERE StoreName LIKE '{0}%'
	                                            AND program = '{1}'
	                                            AND DATEPART(YEAR, SalesDate) = {2}
                                            ) ",
                                            WebStoreNameDDL.SelectedValue.Replace("'", "''").Replace("Kroger All", "Kroger"),
                                            WebProgramDDL.SelectedValue,
                                            YearDDL.SelectedValue
                                            );

            //YTD
            DataTable dt = SalesReport(null, null, 1, strYearDDL);
            dt.Columns.Add("Concept");
            dt.Rows[0]["Concept"] = "YTD Net Commissionable Revenue";
            dt.Columns["Concept"].SetOrdinal(0);
            lstDt.Add(dt);

            //Rotation
            if (WebRotationDDL.SelectedValue == "All") //Get last rotation dates      
            {
                SQLstring = string.Format(@"{0}
                                            SELECT CONVERT(NVARCHAR(20), StartDate) StartDate, CONVERT(NVARCHAR(20),WeekEnding) WeekEnding 
                                            FROM PAYOUTwe
                                            WHERE (
	                                            SELECT MAX(SalesDate) SalesDate
	                                            FROM CTE
	                                            WHERE SalesDate IS NOT NULL
                                            ) BETWEEN StartDate AND WeekEnding",
                                                JoinedSalesTables
                                             );
            }
            else
            {
                SQLstring = string.Format("SELECT TOP 1 CONVERT(NVARCHAR(20), MAX(StartDate)) StartDate, CONVERT(NVARCHAR(20), MAX(WeekEnding)) WeekEnding FROM PAYOUTwe WHERE WeekEnding = '{0}'", WebRotationDDL.SelectedValue);
            }

            dt = Queries.GetResultsFromQueryString(SQLstring);
            StartDateRotation = dt.Rows[0]["StartDate"].ToString();
            WeekendingRotation = dt.Rows[0]["WeekEnding"].ToString();
            dt = SalesReport(StartDateRotation, WeekendingRotation, 1, strYearDDL);
            dt.Columns.Add("Concept");
            dt.Rows[0]["Concept"] = "Rotation To Date NCR";
            dt.Columns["Concept"].SetOrdinal(0);
            lstDt.Add(dt);

            //Last day of rotation   
            if (WebRotationDDL.SelectedValue == "All") //Get last rotation dates
            {
                SQLstring = string.Format(@"{0}
                                            SELECT CONVERT(NVARCHAR(20), MAX(SalesDate)) SalesDate
                                            FROM CTE",
                                            JoinedSalesTables);                                
            }
            else
            {
                SQLstring = string.Format(@"SELECT CONVERT(NVARCHAR(20), MAX(SalesDate)) SalesDate 
                                        FROM PAYOUTsales 
                                        WHERE SalesDate BETWEEN '{0}' AND '{1}' 
                                        AND StoreName = '{2}'
                                        AND Program = '{3}'",
                                        StartDateRotation,
                                        WeekendingRotation,
                                        WebStoreNameDDL.SelectedValue.Replace("'", "''").Replace("Kroger All", "Kroger"),
                                        WebProgramDDL.SelectedValue);
            }

            dt = Queries.GetResultsFromQueryString(SQLstring);
            LastSalesDate = dt.Rows[0]["SalesDate"].ToString();
            dt = SalesReport(LastSalesDate, LastSalesDate, 1, strYearDDL);
            dt.Columns.Add("Concept");
            dt.Rows[0]["Concept"] = LastSalesDate;
            dt.Columns["Concept"].SetOrdinal(0);
            lstDt.Add(dt);

            dtResults = Common.UnionAllDatatables(lstDt);
            return dtResults;
        }
        
        private static DataTable MergeTables(List<DataTable> lstToMerge)
        {
            DataTable dtResults = lstToMerge.MergeAll("MergeColumn");

            for (int i = 0; i < NonDecimalColumns.Count; i++)
            {
                dtResults.Columns.Add(NonDecimalColumns[i], typeof(string));
                dtResults.Columns[NonDecimalColumns[i]].SetOrdinal(i);
            }

            dtResults.Columns.Add("Total", typeof(decimal));
            dtResults.Columns.Add("Average", typeof(decimal));

            foreach (DataRow dr in dtResults.Rows)
            {
                List<decimal> RowValues = new List<decimal>();

                string[] splitRecords = dr["MergeColumn"].ToString().Trim().Split('|');

                for (int i = 0; i < dtResults.Columns.Count; i++)
                {
                    if (NonDecimalColumns.Contains(dtResults.Columns[i].ColumnName))
                    {
                        if (splitRecords.Length == NonDecimalColumns.Count)
                            dr[dtResults.Columns[i].ColumnName] = splitRecords[i];
                    }
                    else if (dtResults.Columns[i].ColumnName == "Total")
                    {
                        dr[dtResults.Columns[i].ColumnName] = RowValues.Sum();
                    }
                    else if (dtResults.Columns[i].ColumnName == "Average")
                    {
                        dr[dtResults.Columns[i].ColumnName] =
                            RowValues.Sum() > 0 ? RowValues.Sum() / RowValues.Where(x => x.ToString() != "0" && x.ToString() != "").Count() : 0;
                    }
                    else if (dtResults.Columns[i].ColumnName != "MergeColumn")
                    {
                        decimal decConversion;
                        decimal.TryParse(dr[i].ToString(), out decConversion);
                        RowValues.Add(decConversion);
                    }
                }
            }

            dtResults.Columns.Remove("MergeColumn");

            DataView dv = dtResults.DefaultView;
            dv.Sort = "Owner";
            dtResults = dv.ToTable();

            DataRow drAverage = totalizeColumns(dtResults, "Average");

            dtResults.Rows.Add(totalizeColumns(dtResults, "Total"));
            dtResults.Rows.Add(drAverage);

            return dtResults;
        }

        private static DataRow totalizeColumns(DataTable dtResults, string Expression)
        {
            DataRow drTotals = dtResults.NewRow();
            for (int i = 0; i < dtResults.Columns.Count; i++)
            {
                if (NonDecimalColumns.Contains(dtResults.Columns[i].ColumnName))
                {
                    if (dtResults.Columns[i].ColumnName == "Location")
                        drTotals[i] = Expression;
                }
                else
                {
                    if(Expression == "Total")
                        drTotals[i] = dtResults.Compute(string.Format("Sum([{0}])", dtResults.Columns[i].ColumnName), "");
                    if (Expression == "Average")
                        drTotals[i] = dtResults.Compute(string.Format("Avg([{0}])", dtResults.Columns[i].ColumnName), "");
                }                    
            }

            return drTotals;
        }
        
        protected void GRV_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.Cells.Count > 1)
            {
                List<string> NumericCells = new List<string>();

                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    string HeaderText = ((BoundField)((DataControlFieldCell)((e.Row.Cells[i]))).ContainingField).HeaderText;
                    if (!NonDecimalColumns.Contains(HeaderText))
                        NumericCells.Add(HeaderText);

                }

                if (((Control)sender).ClientID == "GRVRev")
                {
                    Common.CellFormating(ref e, NumericCells, "Currency", "Owner");
                }
                else if (((Control)sender).ClientID == "GRVQty")
                {
                    Common.CellFormating(ref e, NumericCells, "Integer", "Owner");
                }
            }
        }
        
        private static DataTable SalesReport(string StartDate, string EndDate, int reportNumber, string ParamYearDDL = null)
        {            
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            Parameters.Add("@WebStoreNameDDL", strStoreNameDDL);
            Parameters.Add("@WebProgramDDL", strProgramDDL);
            Parameters.Add("@WebOwnerDDL", strOwnerDDL);
            Parameters.Add("@StartDate", StartDate);
            Parameters.Add("@EndDate", EndDate);
            Parameters.Add("@YearDDL", ParamYearDDL != null ? ParamYearDDL : "NULL");
            Parameters.Add("@ReportNumber", reportNumber.ToString());

            return Queries.GetResultsFromStoreProcedure("spx_PAYOUTYtdWeeklyRpt", ref Parameters).Tables[0];
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            UpdatePanelUpperBox.Update();
            List<DataTable> lstdt = (List<DataTable>)Session["ReportsToPrint"];

            Common.ExportToExcel_List(lstdt, string.Format("YTD and Weekly Report - {0}-{1}-{2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
        }
        
    }
}
