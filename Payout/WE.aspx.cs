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
using OfficeOpenXml;

namespace Payout
{
    public partial class WE : System.Web.UI.Page
    {
        string user;
        string userFullname;
        string userType;
        List<string> UsersRW;

        protected void Page_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> Params = new Dictionary<string, string>();
            try
            {
                user = Session["user"].ToString();
                userFullname = Session["userFullname"].ToString();
                userType = Session["userType"].ToString();


                UsersRW = new List<string>();                
                UsersRW.Add("andjelka@thesmartcircle.com");
                UsersRW.Add("carlos@innovage.net");
                UsersRW.Add("ben@innovage.net");
                UsersRW.Add("Admin");

            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }
           
            if (!IsPostBack)
            {
                using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
                {


                    //string insert = @"INSERT INTO [PAYOUTwe] 

                    //                  SELECT DISTINCT DATEADD(DAY, 14, CONVERT(DATE, [Week Ending])), DATEADD(DAY, 1, CONVERT(DATE, [Week Ending])), 1, 0, 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0, 0
                    //                  FROM [PAYOUTsummary]
                    //                  WHERE NOT EXISTS (
                    //                 SELECT [WeekEnding]
                    //                 FROM [PAYOUTwe]
                    //                 WHERE [WeekEnding] = DATEADD(DAY, 14, CONVERT(DATE, [Week Ending]))
                    //                  )
                    //                 ";



                    ////Validates Week ending to adjust properly the first week ending of year
                    //string insert = @"INSERT INTO [PAYOUTwe] 
                    //                  SELECT DISTINCT

                    //                    CASE WHEN CONVERT(DATE, [Week Ending]) < CONVERT(DATE, '2017-01-01')  AND
                    //                    DATEADD(DAY, 14, CONVERT(DATE, [Week Ending])) > CONVERT(DATE, '2017-01-01') 
                    //                    THEN CONVERT(DATE, '2017-01-01')
                    //                    ELSE DATEADD(DAY, 14, CONVERT(DATE, [Week Ending])) END,

                    //                    DATEADD(DAY, 1, CONVERT(DATE, [Week Ending])),1, 0, 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0, 0
                    //                    FROM [PAYOUTsummary]
                    //                    WHERE NOT EXISTS (
	                   //                     SELECT [WeekEnding]
	                   //                     FROM [PAYOUTwe]	                                        
	                   //                     WHERE [WeekEnding] = CASE WHEN CONVERT(DATE, [Week Ending]) < CONVERT(DATE, '2017-01-01')  AND
							             //                           DATEADD(DAY, 14, CONVERT(DATE, [Week Ending])) > CONVERT(DATE, '2017-01-01') 
							             //                           THEN CONVERT(DATE, '2017-01-01')
							             //                           ELSE DATEADD(DAY, 14, CONVERT(DATE, [Week Ending])) END
                    //                    )
                    //                 ";

                    //con.Open();
                    //using (SqlCommand cmd = new SqlCommand(insert, con))
                    //{
                    //    SqlDataReader reader = cmd.ExecuteReader();
                    //}
                    //con.Close();


                    Params = new Dictionary<string, string>();
                    Params.Add("Action", "INSERT");
                    WebApplication3.Queries.ExecuteFromStoreProcedure("spx_PAYOUTwe", Params);
                }
            }

            //Params = new Dictionary<string, string>();
            //Params.Add("Action", "SELECT");            
            //GridView1.DataSource = WebApplication3.Queries.GetResultsFromStoreProcedure("spx_PAYOUTwe", ref Params);
            //GridView1.DataBind();

            SqlDataSource1.SelectCommand = string.Format(@"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED 
		                                    SELECT TOP {0} [Id], [WeekEnding], [StartDate], [Internal], [Owner], [Hub], [NC], [RC], [RSM], 
		                                    [LastRefreshOn], [LastRefreshBy], [LastPostOn], [LastPostBy], [Refresh], [Post] 
		                                    FROM [PAYOUTwe] 
		                                    WHERE EXISTS
		                                    (
			                                    SELECT 1
			                                    FROM PAYOUTsummary s
			                                    WHERE CONVERT(DATE, s.[Week Ending]) = [PAYOUTwe].[WeekEnding]
		                                    )
                                            OR
                                            (SELECT MAX(SalesDate) FROM PAYOUTsales) BETWEEN StartDate AND WeekEnding
		                                    ORDER BY [WeekEnding] DESC", UsersRW.Contains(user) ? "10" : "5")
                ;
            //"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED SELECT TOP 5 [Id], [WeekEnding], [StartDate], [Internal], [Owner], [Hub], [NC], [RC], [RSM], [LastRefreshOn], [LastRefreshBy], [LastPostOn], [LastPostBy], [Refresh], [Post] FROM [PAYOUTwe] ORDER BY [WeekEnding] DESC";
        }
        
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Pager)
            {
                e.Row.Cells[0].Visible = false;

                if (user != "Admin" && user != "cband@thesmartcircle.com" && user != "thourtovenko@thesmartcircle.com")
                {
                    try
                    {
                        LinkButton tBtn = (LinkButton)e.Row.Cells[1].FindControl("transferBtn");
                        tBtn.Visible = false;
                    }
                    catch { }
                }

                if (user == "cband@thesmartcircle.com" || user == "thourtovenko@thesmartcircle.com")
                {
                    try
                    {
                        LinkButton pBtn = (LinkButton)e.Row.Cells[1].FindControl("postBtn");
                        pBtn.Visible = false;
                    }
                    catch { }

                    e.Row.Cells[4].Visible = false;
                    e.Row.Cells[5].Visible = false;
                    e.Row.Cells[6].Visible = false;
                    e.Row.Cells[7].Visible = false;
                    e.Row.Cells[8].Visible = false;
                    e.Row.Cells[9].Visible = false;
                }

                if (e.Row.RowIndex > 1 && user != "andjelka@thesmartcircle.com")
                {
                    LinkButton tBtn = (LinkButton)e.Row.Cells[1].FindControl("transferBtn");
                    tBtn.Visible = false;

                    LinkButton pBtn = (LinkButton)e.Row.Cells[1].FindControl("postBtn");
                    pBtn.Visible = false;

                   LinkButton rBtn = (LinkButton)e.Row.Cells[1].FindControl("refreshBtn");
                    rBtn.Visible = false;
                }
            }
        }

        protected void CheckChanged(object sender, EventArgs e)
        {
            CheckBox chb = (CheckBox)sender;
            GridViewRow gvr = (GridViewRow)chb.NamingContainer;
            int rowindex = gvr.RowIndex;

            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string update = "UPDATE [PAYOUTwe] SET " + chb.ID + " = '" + chb.Checked + "' WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "'";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(update, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }
        }

        protected void backBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/", true);
        }

        protected void Trigger_Click(object sender, EventArgs e)
        {
            LinkButton trigger = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)trigger.NamingContainer;
            int rowindex = gvr.RowIndex;
            string action = trigger.ID.Replace("Btn", "");

            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {

                if (action != "transfer")
                {
                    string setAction = "UPDATE [PAYOUTwe] SET " + action + " = 1 WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "'";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(setAction, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                    con.Close();

                    string execAction = "exec msdb.dbo.sp_start_job [RS " + action + "]";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(execAction, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                    con.Close();

                    string lastStamp = "UPDATE [PAYOUTwe] SET [Last" + action + "On] = CURRENT_TIMESTAMP, [Last" + action + "By] = '" + userFullname + "' WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "'";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(lastStamp, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                    con.Close();
                }
                else
                {

                    Dictionary<string, string> Parameters = new Dictionary<string, string>();
                    Parameters.Add("@webStartDate", GridView1.Rows[rowindex].Cells[3].Text);
                    Parameters.Add("@webEndDate", GridView1.Rows[rowindex].Cells[2].Text);

                    WebApplication3.Queries.ExecuteFromStoreProcedure("spx_PAYOUTRStoLive", Parameters);                    
                }
            }

            Response.Redirect("~/WE.aspx", true);
        }
    }
}