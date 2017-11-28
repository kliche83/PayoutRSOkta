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
    public partial class Schedule : System.Web.UI.Page
    {
        List<string> UsersRW;
        string user = string.Empty;
        int rowindex;
        string updateR = @"UPDATE PAYOUTsales SET [ItemNumber] = I.[ItemNumber], [ItemName] = I.[ItemName] 
                           FROM PAYOUTsales S 
                           JOIN PAYOUTchanges C ON C.SubItem = S.ItemNumber AND C.StoreNumber = S.StoreNumber AND C.StoreName LIKE S.StoreName + '%' AND S.SalesDate BETWEEN C.StartDate AND C.EndDate 
                           JOIN PAYOUTitemMaster I ON I.ItemNumber = C.ItemNumber 
                           

                           UPDATE PAYOUTsales SET [Program] = P.[Program] 
                           FROM PAYOUTsales S 
                           JOIN PAYOUTschedule P ON P.StoreName = S.StoreName AND P.StoreNumber = S.StoreNumber AND S.SalesDate BETWEEN P.StartDate AND P.EndDate 
                           JOIN PAYOUTitemMaster I ON I.ItemNumber = S.ItemNumber AND I.StoreName LIKE S.StoreName + '%' AND I.Program = S.Program 
                           

                           UPDATE PAYOUTsales SET [Program] = I.[Program] 
                           FROM PAYOUTsales S 
                           JOIN PAYOUTitemMaster I ON I.ItemNumber = S.ItemNumber AND I.StoreName LIKE S.StoreName + '%' AND I.Program = S.Program 
                           WHERE S.Program IS NULL
                          ";

        protected void Page_Load(object sender, EventArgs e)
        {
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

            string dateQuery = string.Empty;

            if (dateFrom.Text == "" && dateTo.Text == "")
            {
                //dateQuery = "StartDate >= '2014-12-29' /*OR EndDate >= '2014-12-29'*/ ";
                dateQuery = "StartDate > = '" + Common.ApplyDateFormat(DateTime.Now.AddMonths(-2)) + "'";
                dateFrom.Text = Common.ApplyDateFormat(DateTime.Now.AddMonths(-2));
            }
            if (dateFrom.Text != "" && dateTo.Text != "")
            {
                dateQuery = "StartDate >= '" + dateFrom.Text + "' AND '" + dateTo.Text + "' >= EndDate ";
            }
            if (dateFrom.Text != "" && dateTo.Text == "")
            {
                dateQuery = "StartDate >= '" + dateFrom.Text + "' ";
            }
            if (dateFrom.Text == "" && dateTo.Text != "")
            {
                dateQuery = "EndDate <= '" + dateTo.Text + "' ";
            }

            SqlDataSource1.SelectCommand =
                "SELECT Id, Program, StartDate, EndDate, StoreName, StoreNumber, City, State, OwnerFirstname, OwnerLastname, HubFirstname, HubLastname, ImportedBy, CONVERT(NVARCHAR, ImportedOn, 101) AS ImportedOn, EventId " +
                "FROM [PAYOUTSchedule] A " +
                "WHERE [Archive] = 0 AND " +
                dateQuery +
                "AND StoreName LIKE '" + sstoreDDL.SelectedValue + "%' AND Program LIKE '" + ((programDDL.SelectedValue == "All") ? "%" : programDDL.SelectedValue) + "%' AND StoreNumber LIKE '%" + StoreNumberTXT.Text + "%' AND (OwnerFirstname LIKE '%" + ownerTXT.Text + "%' OR OwnerLastname LIKE '%" + ownerTXT.Text + "%') " +
                "GROUP BY Id, Program, StartDate, EndDate, StoreName, StoreNumber, City, State, OwnerFirstname, OwnerLastname, HubFirstname, HubLastname, ImportedBy, ImportedOn, EventId " +
                "ORDER BY StartDate DESC, Program, StoreName, StoreNumber, ImportedBy, ImportedOn";
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


            if (txt.ID == "StartDate" || txt.ID == "EndDate")
                Payout.FullSalesAuditCls.SchedBuildCommentSalesAuditFull(GridView1.Rows[rowindex].Cells[0].Text, txt.ID, user);
            
            GridView1.DataBind();
        }
                
        protected void addBtn_Click(object sender, EventArgs e)
        {
            string start = Request.Form["StartDateT"].ToString();
            string end = Request.Form["EndDateT"].ToString();
            string program = progDDL.SelectedValue;
            string city = Request.Form["CityT"].ToString();
            string state = Request.Form["StateT"].ToString();
            //string country = Request.Form["CountryT"].ToString();
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
                                hbf.Trim() =="" ? "NULL" : "'" + hbf + "'",
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