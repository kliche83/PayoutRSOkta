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
    public partial class GridRoleCommissionDaily : System.Web.UI.Page
    {
        string ParamStartDate;
        string ParamEndDate;
        string Trainer;
        int NumberOfWeeks;        

        protected void Page_Load(object sender, EventArgs e)
        {
            string user = string.Empty;

            try
            {
                user = Session["user"].ToString();

                if (!IsPostBack)
                {
                    Fillcontrols();
                }
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }
        }



        private void Fillcontrols()
        {
            string SQLstring = string.Format(@" SELECT StartDate, WeekEnding
                                      FROM PAYOUTwe 
                                      WHERE WeekEnding 
                                      BETWEEN CONVERT(DATE, '{0}') AND CONVERT(DATE, '{1}')",
                                      (string)Session["ParamStartDate"],
                                      (string)Session["ParamEndDate"]);

            DataTable dtWE = Queries.GetResultsFromQueryString(SQLstring);
            ParamStartDate = Convert.ToDateTime(dtWE.Rows[0][0].ToString()).AddDays(-2).ToString("yyyy-MM-dd");
            ParamEndDate = Convert.ToDateTime(dtWE.Rows[0][1].ToString()).AddDays(-2).ToString("yyyy-MM-dd");

            //ParamStartDate = (string)Session["ParamStartDate"];
            //ParamEndDate = (string)Session["ParamEndDate"];
            Trainer = (string)Session["Trainer"];

            Session["ResultTable"] = new List<DataTable>();
            Session["ExportTable"] = new List<DataTable>();


            NumberOfWeeks =
            ((((decimal)(Convert.ToDateTime(ParamEndDate) - Convert.ToDateTime(ParamStartDate)).TotalDays + 1) / 7) % 1) != 0 ?
            (((int)(Convert.ToDateTime(ParamEndDate) - Convert.ToDateTime(ParamStartDate)).TotalDays + 1) / 7) + 1 :
            ((int)(Convert.ToDateTime(ParamEndDate) - Convert.ToDateTime(ParamStartDate)).TotalDays + 1) / 7;

            FillFromDatabase();


            List<DataTable> lstResultTables = new List<DataTable>();
            lstResultTables = (List<DataTable>)Session["lstResultTables"];


            CommissionPanel.Controls.Add(BindGridData(topGrid(), 1));

            int incremental = 1;
            foreach (DataTable dt in lstResultTables)
            {
                Panel pnl = new Panel() { CssClass = "TitleBox" };
                pnl.Controls.Add(new Label() { Text = "Week #" + incremental.ToString() });
                incremental++;

                CommissionPanel.Controls.Add(pnl);
                CommissionPanel.Controls.Add(BindGridData(dt, incremental, true));
            }
        }

        private void FillFromDatabase()
        {
            DataTable dt = new DataTable();
            string SQLString = string.Format("SELECT [Id] FROM [PAYOUTPMPayroll] WHERE [CheckDate] = '{0}'", (string)Session["ParamEndDate"]);

            List<int> dtPayrollIds = Queries.GetResultsFromQueryString(SQLString).AsEnumerable().Select(i => i.Field<int>("Id")).ToList();

            
            if (dtPayrollIds.Count > 0)
            {
                SQLString = string.Format(@"SELECT  CONVERT(NVARCHAR(20), [Date]) [Date], TrainerName, Program, Retailer, StoreNumber [Club #], 
                                                    [City] + ' ' + [State] [City/State], NCR, RoleDescription, Rate
                                            FROM    PAYOUTPMCommissionSummary
                                            WHERE   PMPayroll_Id IN ({0})
                                                    AND [TrainerName] = '{1}'", 
                                                    string.Join(", ", dtPayrollIds.ToArray()), 
                                                    Trainer);

                dt = Queries.GetResultsFromQueryString(SQLString);
            }
            else
            {
                Dictionary<string, string> SQLParameters = new Dictionary<string, string>();
                SQLParameters.Add("@webStartDate", (string)Session["ParamStartDate"]);
                SQLParameters.Add("@webEndDate", (string)Session["ParamEndDate"]);
                SQLParameters.Add("@webTrainer", Trainer);
                SQLParameters.Add("@Action", "SELECT");

                dt = Queries.GetResultsFromStoreProcedure("spx_PAYOUTsummaryCommissionDaily", ref SQLParameters).Tables[0];
            }

            

            //for (int i = 0; i < dt.Rows.Count; i++)            
            //    dt.Rows[i]["Date"] = dt.Rows[i]["Date"].ToString() == "" ? "" : Common.ApplyDateFormat(dt.Rows[i]["Date"].ToString());

            List<DataTable> lstResultTable = new List<DataTable>();

            if (dt != null && dt.Rows.Count > 0)                
                lstResultTable = ReportDatatable(dt);            

            Session["lstResultTables"] = lstResultTable;
        }

        private List<DataTable> ReportDatatable(DataTable dt)
        {
            DataTable dtResults = new DataTable();
            DataTable dtPivot = new DataTable();
            List<DataTable> lstdtResults = new List<DataTable>();

            DateTime StartDate = Convert.ToDateTime(ParamStartDate);
            DateTime EndDate = Convert.ToDateTime(ParamEndDate);
            List<DataTable> lstDatatable;
            
            List<DataTable> TableList = new List<DataTable>();

            dt.Columns.Remove("TrainerName");

            ////Generate the column with name of day
            DataColumn Col = dt.Columns.Add("Day", Type.GetType("System.String"));
            //Calculate Check Commission Due
            DataColumn Col1 = dt.Columns.Add("CheckCommissionDue", Type.GetType("System.Decimal"));

            Col.SetOrdinal(1);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //dt.Rows[i][1] = Convert.ToDateTime(dt.Rows[i][0]).DayOfWeek; //Column Day
                dt.Rows[i][dt.Columns.Count - 1] = decimal.Parse(dt.Rows[i][6].ToString()) * decimal.Parse(dt.Rows[i][8].ToString()) / 100; //Column Check Commission Due
            }

            for (int i = 0; i < NumberOfWeeks; i++)
            {
                lstDatatable = new List<DataTable>();
                dtResults = dt.Clone();
                
                var RecordsByWeek = dt.AsEnumerable().Where(row => Convert.ToDateTime(row.Field<string>("Date").ToLower()) >= StartDate.AddDays(i * 7) &&
                                        Convert.ToDateTime(row.Field<string>("Date").ToLower()) <= StartDate.AddDays(((i + 1) * 7) - 1))
                                        .OrderBy(row => Convert.ToDateTime(row.Field<string>("Date").ToLower()));

                if(RecordsByWeek.Count() > 0)
                    dtResults = RecordsByWeek.CopyToDataTable();


                DataTable dt1Col = new DataTable();
                DataRow dr;

                DataColumn dc = new DataColumn();
                dc.ColumnName = "Id";
                dt1Col.Columns.Add(dc);

                dc = new DataColumn();
                dc.ColumnName = "Date";
                dt1Col.Columns.Add(dc);
                
                for (int l = 1; l < dtResults.Columns.Count; l++)
                {
                    dr = dt1Col.NewRow();
                    dr[0] = l;
                    dr[1] = dtResults.Columns[l].ColumnName;
                    dt1Col.Rows.Add(dr);
                }

                lstDatatable.Add(dt1Col);
                
                
                int dtResultsCounter = 0;
                for (int j = 0; j < 7; j++)
                {
                    dt1Col = new DataTable();
                    dc = new DataColumn();
                    dc.ColumnName = "Id";
                    dt1Col.Columns.Add(dc);

                    dc = new DataColumn();
                                        
                    if (dtResults.Rows.Count > 0 && (StartDate.AddDays((i * 7) + j) - Convert.ToDateTime(dtResults.Rows[dtResultsCounter]["Date"].ToString())).TotalDays == 0)
                    {
                        dc.ColumnName = dtResults.Rows[dtResultsCounter]["Date"].ToString();
                        dt1Col.Columns.Add(dc);

                        for (int k = 1; k < dtResults.Columns.Count; k++)
                        {
                            dr = dt1Col.NewRow();
                            dr[0] = k;

                            if (k == 1)
                                dr[1] = StartDate.AddDays((i * 7) + j).DayOfWeek;
                            else
                                dr[1] = dtResults.Rows[dtResultsCounter][k].ToString();
                            
                            dt1Col.Rows.Add(dr);
                        }
                        if(dtResultsCounter + 1 < dtResults.Rows.Count)
                            dtResultsCounter++;
                    }
                    else
                    {
                        dc.ColumnName = Common.ApplyDateFormat(StartDate.AddDays((i * 7) + j));
                        dt1Col.Columns.Add(dc);

                        for (int k = 1; k < dtResults.Columns.Count; k++)
                        {
                            dr = dt1Col.NewRow();
                            dr[0] = k;

                            if (k == 1)
                                dr[1] = StartDate.AddDays((i * 7) + j).DayOfWeek;
                            else
                                dr[1] = "";

                            dt1Col.Rows.Add(dr);
                        }
                    }
                    lstDatatable.Add(dt1Col);
                }

                dtPivot = lstDatatable.MergeAll("Id");
                dtPivot.Columns.Remove("Id");

                lstdtResults.Add(dtPivot);
            }

            return lstdtResults;
        }
        
        private GridView BindGridData(DataTable dt, int GridId, bool AddDataBound = false)
        {
            GridView grv = new GridView();
            
            grv.DataSource = dt;
            grv.ID = "GV_" + GridId;
            if (AddDataBound)
                grv.RowDataBound += GridView_RowDataBound;            
            grv.DataBind();

            grv.CssClass = "neoGrid DynamicTable";


            return grv;
        }                
        
        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            e.Row.Cells[0].CssClass = "ColumnCategory";

            if((e.Row.Cells[0]).Text == "CheckCommissionDue")
                e.Row.Cells[0].Text = "Check Commission Due";

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if ((e.Row.Cells[i]).Text != "&nbsp;" && (e.Row.Cells[i]).Text != (e.Row.Cells[0]).Text)
                {
                    e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Center;
                    switch ((e.Row.Cells[0]).Text)
                    {
                        case "NCR":
                            e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;                            
                            (e.Row.Cells[i]).Text = decimal.Parse((e.Row.Cells[i]).Text).ToString("C");
                            break;
                        case "Rate":
                            (e.Row.Cells[i]).Text = (int)decimal.Parse((e.Row.Cells[i]).Text) + "%";                            
                            break;
                        case "Check Commission Due":
                            e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                            (e.Row.Cells[i]).Text = decimal.Parse((e.Row.Cells[i]).Text).ToString("C");
                            break;
                    }
                }
            }            
        }


        private DataTable topGrid()
        {
            DataTable dt = new DataTable();
            DateTime StartDate = Convert.ToDateTime(ParamStartDate);
            DateTime EndDate = Convert.ToDateTime(ParamEndDate);
            List<string> lstWeekDates = new List<string>();

            string dateRange = string.Empty;

            for (int i = 0; i < NumberOfWeeks; i++)
            {
                dateRange = Common.ApplyDateFormat(StartDate.AddDays(7 * i).ToString()) + " - " +
                            Common.ApplyDateFormat(StartDate.AddDays((7 * i) + 6).ToString());


                lstWeekDates.Add(dateRange);
            }


            
            DataColumn dc = new DataColumn("Name");
            dt.Columns.Add(dc);

            for (int i = 0; i < lstWeekDates.Count; i++)
            {
                dc = new DataColumn("Week " + (i + 1)  + " Dates");
                dt.Columns.Add(dc);
            }

            DataRow dr = dt.NewRow();

            dr[0] = Trainer;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                dr[i] = lstWeekDates[i-1];
            }

            dt.Rows.Add(dr);

            return dt;
        }


        protected void exportBtn_Click(object sender, EventArgs e)
        {
            Common.ExportToExcel_List((List<DataTable>)Session["lstResultTables"], "RoadShow Trainer Daily Commission Summary");
        }

        protected void BackBtn_Click(object sender, EventArgs e)
        {
            Session["HitBackButton"] = true;
            //Response.Redirect("GridRoleCommission.aspx", true);
            Response.Redirect("PMCumulativeSummary.aspx", true);
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

        private string ApplyDateFormat(DateTime dateToFormat)
        {
            return string.Format("{0:yyyy-MM-dd}", dateToFormat);
        }
        
        
        private void FooterListTotalization(ref GridView GridView1)
        {            
            Common.TotalizeFooter(ref GridView1, "Commissions");
            int IndexColumn = Common.GetColumnIndexByName(GridView1, "Program Wk1");
            GridView1.FooterRow.Cells[IndexColumn].Text = "Grand Total";
            GridView1.FooterRow.Cells[IndexColumn].HorizontalAlign = HorizontalAlign.Right;
        }
        
        
    }
}