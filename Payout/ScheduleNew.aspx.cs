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
    public partial class ScheduleNew : System.Web.UI.Page
    {
        List<string> UsersRW;
        string user = string.Empty;
        int rowindex;
        private static readonly int ColumnsCount = 3;
        bool hasdaysBool = false;
        int pageIndexvar;

        protected void Page_Load(object sender, EventArgs e)
        {
            pageIndexvar = 1;
            UsersRW = new List<string>();
            UsersRW.Add("cband@thesmartcircle.com");
            UsersRW.Add("thourtovenko@thesmartcircle.com");
            UsersRW.Add("carlos@innovage.net");
            UsersRW.Add("ben@innovage.net");
            UsersRW.Add("Admin");

            try
            {
                user = Session["user"].ToString();

                OverlapsReport.Visible = false;
                if (!UsersRW.Contains(user))
                {
                    add.Visible = false;
                }
                else
                {
                    OverlapsReport.Visible = true;
                }
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }




            string SQLString = "";

            if (SetWeekEndingColumns())
            {
                hasdaysBool = true;
                //PIVOT SCRIPT
                SQLString = @"DECLARE @cols NVARCHAR (MAX)
                                DECLARE @query NVARCHAR(MAX)

                                /*Get List of Schedule in the date range of Rotation WeekEndings Table*/
	                            SELECT	
	                            s.Id, Program,
	                            s.StartDate, EndDate, StoreName, StoreNumber, City, [State], 
			                            OwnerFirstname, OwnerLastname, HubFirstname, HubLastname, ImportedBy, 
			                            CONVERT(NVARCHAR, ImportedOn, 101) AS ImportedOn, EventId, w.WeekEnding,
			                            CASE WHEN s.StartDate < w.StartDate THEN DATEDIFF(DAY, s.EndDate, w.WeekEnding)
				                                ELSE DATEDIFF(DAY, s.StartDate, w.WeekEnding) + 1 END DayCount
	                            INTO #TempTable
	                            FROM PAYOUTSchedule s
	                            INNER JOIN PAYOUTwe w ON 
	                            (s.StartDate BETWEEN w.StartDate AND w.WeekEnding) OR
	                            (s.EndDate BETWEEN w.StartDate AND w.WeekEnding)
	                            WHERE [Archive] = 0 AND
	                            {0}

                                SELECT @cols = COALESCE (@cols + ',[' + CONVERT(NVARCHAR, [WeekEnding], 20) + ']', 
                                               '[' + CONVERT(NVARCHAR, [WeekEnding], 20) + ']')
                                               FROM    (SELECT DISTINCT [WeekEnding] FROM #TempTable) PV  
                                               ORDER BY [WeekEnding]

                                SET @query = '
                                              SELECT Id, Program,StartDate, EndDate, StoreName, StoreNumber, City, [State], 
			                                        OwnerFirstname, OwnerLastname, HubFirstname, HubLastname, ImportedBy, ImportedOn, EventId, {1} days, {2}
                                            FROM 
                                             (
                                                 SELECT * FROM #TempTable
                                             ) x
                                             PIVOT 
                                             (
                                                 SUM(DayCount)
                                                 FOR [WeekEnding] IN (' + @cols + ')
                                            ) p
                                    '
                                EXEC SP_EXECUTESQL @query;
                                DROP TABLE #TempTable";

                List<string> lstDateColumns = DateColumns();
                string SumWeekEndings = "";

                for (int i = 0; i < ColumnsCount; i++)
                {
                    try
                    {
                        lstDateColumns[i] = "[" + lstDateColumns[i] + "] WeekEnding" + (i + 1).ToString();
                    }
                    catch
                    {
                        lstDateColumns.Add("NULL WeekEnding" + (i + 1).ToString());
                    }

                    if (i > 0)
                        SumWeekEndings += "+";

                    SumWeekEndings += "ISNULL(" + lstDateColumns[i].Split(' ')[0] + " , 0)";
                }

                //for (int i = 0; i < lstDateColumns.Count; i++)                
                //    lstDateColumns[i] = "[" + lstDateColumns[i] + "] WeekEnding" + (i + 1).ToString();
                

                SQLString = string.Format(SQLString, 
                    "{0} {1} {2} {3} ",
                    SumWeekEndings,
                    string.Join(",", lstDateColumns),
                    "{3}"
                    );

                //WeekEnding1

                SQLString = SetFilters(SQLString);
            }
            else
            {
                hasdaysBool = false;
                List<string> EmptyColumns = new List<string>();
                for (int i = 0; i < ColumnsCount; i++)
                    EmptyColumns.Add("NULL WeekEnding" + (i+1).ToString());

                SQLString = string.Format(
                    @"SELECT Id, Program, StartDate, EndDate, StoreName, StoreNumber, City, State, OwnerFirstname, OwnerLastname, 
                        HubFirstname, HubLastname, ImportedBy, CONVERT(NVARCHAR, ImportedOn, 101) AS ImportedOn, EventId, NULL Days, {0}
                        FROM [PAYOUTSchedule] s 
                        WHERE [Archive] = 0 AND {1}
                        GROUP BY Id, Program, StartDate, EndDate, StoreName, StoreNumber, City, State, OwnerFirstname, OwnerLastname, HubFirstname, HubLastname, ImportedBy, ImportedOn, EventId                             
                        ORDER BY StartDate DESC, Program, StoreName, StoreNumber, ImportedBy, ImportedOn",
                    
                    string.Join(",", EmptyColumns),
                    "{0} {1} {2} {3} "
                    );
                    
                SQLString = SetFilters(SQLString);
            }          

            SqlDataSource1.SelectCommand = SQLString;

            
        }

        //Send SQL script as CTE
        private string PaginationString(string Query)
        {
            string SQLstring = "ORDER BY StartDate DESC, Program, StoreName, StoreNumber, ImportedBy, ImportedOn";

            return SQLstring;
        }
        

        private bool SetWeekEndingColumns()
        {
            List<string> Columns = DateColumns();

            if (Columns.Count <= ColumnsCount && Columns.Count > 0)
            {
                for (int i = 0; i < ColumnsCount; i++)
                {
                    if (i < Columns.Count)                    
                        GridView1.Columns[14 + i].HeaderText = Columns[i];
                    else
                        GridView1.Columns[14 + i].HeaderText = "WeekEnding" + (i + 1).ToString();
                }
                return true;
            }
            else
            {
                for (int i = 0; i < ColumnsCount; i++)
                    GridView1.Columns[14 + i].HeaderText = "WeekEnding" + (i + 1).ToString();
            }
                
            return false;
        }

        private List<string> DateColumns()
        {
            string SQLString = @"SELECT DISTINCT CONVERT(NVARCHAR, WeekEnding, 20) WeekEnding
                                FROM PAYOUTSchedule s
                                INNER JOIN PAYOUTwe w ON 
                                (s.StartDate BETWEEN w.StartDate AND w.WeekEnding) OR
                                (s.EndDate BETWEEN w.StartDate AND w.WeekEnding)
                                WHERE[Archive] = 0 AND {0} {1} {2} {3}
                                ORDER BY WeekEnding";
            SQLString = SetFilters(SQLString);

            return Queries.GetResultsFromQueryString(SQLString).AsEnumerable().Select(f => f.Field<string>("WeekEnding")).ToList();
        }

        private string SetFilters(string SQLString)
        {
            return string.Format(SQLString,
                DateRangeQuery("s"),
                sstoreDDL.SelectedValue != "All" && sstoreDDL.SelectedValue != "%" ? string.Format("AND StoreName LIKE '{0}%'", sstoreDDL.SelectedValue) : "",
                //programDDL.SelectedValue != "All" && programDDL.SelectedValue != "" ? string.Format("AND Program LIKE '{0}%' ", programDDL.SelectedValue) : "",
                programDDL.SelectedValue != "All" && programDDL.SelectedValue != "" ? string.Format("AND Program = '{0}' ", programDDL.SelectedValue) : "",
                StoreNumberTXT.Text.Trim() != "" ? string.Format("AND StoreNumber LIKE '{0}%'", StoreNumberTXT.Text.Trim()) : "",
                ownerTXT.Text.Trim() != "" ? string.Format("AND OwnerFirstname + ' ' + OwnerLastname LIKE '{0}%'", ownerTXT.Text.Trim()) : ""
                );
        }

        private string DateRangeQuery(string TablePrefix)
        {
            string dateQuery = string.Empty;

            if (dateFrom.Text == "" && dateTo.Text == "")
            {
                dateFrom.Text = Common.ApplyDateFormat(DateTime.Now.AddMonths(-2));
                return TablePrefix + ".StartDate > = '" + Common.ApplyDateFormat(DateTime.Now.AddMonths(-2)) + "'";                
            }
            if (dateFrom.Text != "" && dateTo.Text != "")
            {
                return TablePrefix + ".StartDate >= '" + dateFrom.Text + "' AND '" + dateTo.Text + "' >= " + TablePrefix + ".EndDate ";
            }
            if (dateFrom.Text != "" && dateTo.Text == "")
            {
                return TablePrefix + ".StartDate >= '" + dateFrom.Text + "' ";
            }
            if (dateFrom.Text == "" && dateTo.Text != "")
            {
                return TablePrefix + ".EndDate <= '" + dateTo.Text + "' ";
            }

            return string.Empty;
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Pager)
            {
                e.Row.Cells[0].Visible = false;
                if (!UsersRW.Contains(user))
                {
                    e.Row.Cells[1].Visible = false;
                }
                
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if ((((DataControlFieldCell)((e.Row.Cells[i]))).ContainingField).HeaderText.StartsWith("WeekEnding") ||
                        (!hasdaysBool && (((DataControlFieldCell)((e.Row.Cells[i]))).ContainingField).HeaderText.Contains("Days")))
                    {
                        e.Row.Cells[i].Visible = false;
                    }
                }                
            }

            if (!UsersRW.Contains(user))
                Common.TurnOffControlsFromGrid(e);
        }

        protected void FieldChanged(Object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow gvr = (GridViewRow)txt.NamingContainer;
            rowindex = gvr.RowIndex;
            string update;

            if (txt.ID == "OwnerFirstname" || txt.ID == "OwnerLastname" || txt.ID == "HubFirstname" || txt.ID == "HubLastname")
            {
                update = string.Format("UPDATE [PAYOUTschedule] SET {0} = {1} WHERE [Id] = '{2}'",
                txt.ID,
                txt.Text.Trim() == "" ? "NULL" : "'" + txt.Text + "'",
                GridView1.Rows[rowindex].Cells[0].Text);
            }
            else
            {
                update = string.Format("UPDATE [PAYOUTschedule] SET {0} = '{1}' WHERE [Id] = '{2}'",
                txt.ID,
                txt.Text,
                GridView1.Rows[rowindex].Cells[0].Text);
            }

            if (txt.ID == "EventId")
            {
                if (txt.Text.All(char.IsNumber))
                {
                    Queries.ExecuteFromQueryString(update);
                }
            }
            else
                Queries.ExecuteFromQueryString(update);
            
            GridView1.DataBind();
        }

        protected void addBtn_Click(object sender, EventArgs e)
        {
            string start = Request.Form["StartDateT"].ToString();
            string end = Request.Form["EndDateT"].ToString();
            string program = progDDL.SelectedValue;
            string city = Request.Form["CityT"].ToString();
            string state = Request.Form["StateT"].ToString();
            string club = Request.Form["ClubT"].ToString();
            string owf = Request.Form["OFN"].ToString();
            string owl = Request.Form["OLN"].ToString();
            string hbf = Request.Form["HFN"].ToString();
            string hbl = Request.Form["HLN"].ToString();
            string eventid = Request.Form["EventIdT"] == null ? "NULL" : Request.Form["EventIdT"].ToString();

            int IdConvert = 0;
            int.TryParse(eventid, out IdConvert);
            eventid = IdConvert == 0 ? "NULL" : IdConvert.ToString();

            string retailer = storeDDL.SelectedValue;
            string Id = string.Empty;
                        
            string update = string.Format(@"declare @MaxId as int = (SELECT MAX(Id) + 1 FROM [PAYOUTschedule]);

                                          INSERT INTO [PAYOUTschedule] 
                                            (Id, Program, StartDate, EndDate, StoreName, StoreNumber, City, State, OwnerFirstname, OwnerLastname, 
                                            HubFirstname, HubLastname, ImportedBy, EventId, ImportedOn, Archive)
                                            VALUES (@MaxId, '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', {7}, {8}, {9}, {10}, '{11}', {12}, CURRENT_TIMESTAMP, 0);
		
                                            /*And then RESEED Temp table*/
                                            DBCC CHECKIDENT ('[Payout].[dbo].[PAYOUTscheduleTemp]', RESEED, @MaxId);",
                                program, start, end, retailer, club, city, state,
                                owf.Trim() == "" ? "NULL" : "'" + owf + "'",
                                owl.Trim() == "" ? "NULL" : "'" + owl + "'",
                                hbf.Trim() == "" ? "NULL" : "'" + hbf + "'",
                                hbl.Trim() == "" ? "NULL" : "'" + hbl + "'",
                                user, eventid
                                );

            Queries.ExecuteFromQueryString(update);
            
            Response.Redirect("Schedule.aspx", true);
        }

        protected void delLink_Click(object sender, EventArgs e)
        {
            LinkButton del = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)del.NamingContainer;
            int delRow = gvr.RowIndex;


            string delete = string.Format(@"DELETE FROM [PAYOUTschedule] WHERE [Id] = '{0}';"
                                            , GridView1.Rows[delRow].Cells[0].Text);
            Queries.ExecuteFromQueryString(delete);
            
            GridView1.DataBind();
        }

        protected void searchBTN_Click(object sender, EventArgs e)
        {
            GridView1.DataBind();
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            GridView2.AllowPaging = false;
            GridView2.DataBind();
            string attachment = "attachment; filename=\"RoadShow Schedule.xls\"";
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
    }
}