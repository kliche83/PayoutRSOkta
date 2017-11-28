using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Payout
{
    public partial class _Default : Page
    {
        string user;
        string userType;
        string userFullname;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                user = Session["user"].ToString();
                RunDefault();
            }
            catch(Exception ex)
            {
                Server.Transfer("Login.aspx", true);
            }
        }

        private void SetEnvironment()
        {
            //Session["user"] = ((System.Security.Claims.ClaimsPrincipal)HttpContext.Current.User).Claims.AsEnumerable().Where(x => x.Type.Contains("identity/claims/emailaddress")).Select(x => x.Value).FirstOrDefault();
            //Session["userType"]  = ((System.Security.Claims.ClaimsPrincipal)HttpContext.Current.User).Claims.AsEnumerable().Where(x => x.Type.Contains("identity/claims/role")).Select(x => x.Value).FirstOrDefault();

            //Session["user"] = username.Text;            
            //Session["userFullname"] = userFullname;
            //Session["userType"] = reader[3].ToString();

            logoutSpan.Visible = true;            
            dashDiv.Visible = true;
            menuBtns.Visible = true;
            notifSpan.Visible = false;
            menu.Attributes.Add("style", "background-color: #2764AB;");

            if ((string)Session["userType"] != "Owner")
            {
                dashDivPre.Visible = true;
            }
        }

        private void RunDefault()
        {
            yr.Text = DateTime.Now.Year.ToString();
            logoutSpan.Visible = false;
            menuBtns.Visible = false;
            notifSpan.Visible = false;

            if (!IsPostBack)
                SetEnvironment();
            
            try
            {
                user = Session["user"].ToString();
                userFullname = Session["userFullname"].ToString();
                homeBtn.Text = Session["userFullname"].ToString();
                userType = Session["userType"].ToString().ToLower();
                userType = userType == "payoutitadmin" ? "admin" : userType;                

                logoutSpan.Visible = true;
                //loginDiv.Visible = false;
                dashDivPre.Visible = true;
                dashDiv.Visible = true;
                menuBtns.Visible = true;
                refreshBtn.Visible = false;
                postBtn.Visible = false;
                transferBtn.Visible = false;
                panelBtn.Visible = false;
                Override.Visible = false;
                GridSummaryYTD.Visible = false;
                Avg.Visible = false;
                ScheduleTrainer.Visible = false;
                AuditSales.Visible = false;
                SalesFile.Visible = false;
                YTDWeeklyReport.Visible = false;
                ShowsCount.Visible = false;
                AccountManager.Visible = false;


                if (user == "carlos@innovage.net" || user == "ben@innovage.net" || user == "Admin")
                {
                    Override.Visible = true;
                    GridSummaryYTD.Visible = true;
                    Avg.Visible = true;
                    ScheduleTrainer.Visible = true;
                    AuditSales.Visible = true;
                    refreshBtn.Visible = true;
                    transferBtn.Visible = true;
                    panelBtn.Visible = true;
                    SalesFile.Visible = true;
                    YTDWeeklyReport.Visible = true;
                    ShowsCount.Visible = true;
                    AccountManager.Visible = true;

                    contentFrame.Src = "Grids.aspx";
                }

                if (user == "cband@thesmartcircle.com" || user == "thourtovenko@thesmartcircle.com")
                {
                    SalesFile.Visible = true;
                    ShowsCount.Visible = true;
                }

                if (user == "tmarshall@thesmartcircle.com")
                {
                    YTDWeeklyReport.Visible = true;
                    GridSummaryYTD.Visible = true;
                }

                if (user == "cwind@thesmartcircle.com" || user == "tjohnson@thesmartcircle.com")
                {
                    AuditSales.Visible = true;
                }


                //if (userType != "Unknown"  )
                //{
                //    //Trainer.Visible = false;
                //    ScheduleTrainer.Visible = false;

                //}

                if (user == "Admin" || user == "admin")
                {
                    ScheduleTrainer.Visible = true;
                }

                if (userType != "admin" && userType != "SC")
                {
                    dashDiv.Attributes.Add("style", "left: 10px;");
                }

                if (userType == "admin" || userType == "SC")
                {
                    refreshBtn.Visible = true;
                    postBtn.Visible = true;
                    panelBtn.Visible = true;
                    //panelYTD.Visible = true;

                    //panelWE.Visible = true;
                    if (user != "andjelka@thesmartcircle.com")
                    {
                        transferBtn.Visible = true;
                    }
                    if (user == "Admin" || user == "andjelka@thesmartcircle.com")
                    {
                        Override.Visible = true;
                        GridSummaryYTD.Visible = true;
                        Avg.Visible = true;
                        //panelYTD.Visible = true;
                    }
                    if (user == "andjelka@thesmartcircle.com")
                    {
                        YTDWeeklyReport.Visible = true;
                        SalesFile.Visible = true;
                    }
                    if (userType == "admin")
                    {
                        GridSummaryYTD.Visible = true;
                    }
                    panelFrame.Src = "WE.aspx";

                }

                if (user == "cband@thesmartcircle.com" || user == "njohnston@thesmartcircle.com")
                {
                    refreshBtn.Visible = true;
                    transferBtn.Visible = true;
                    panelBtn.Visible = true;
                    //panelYTD.Visible = true;
                }

                if (user == "epark@thesmartcircle.com" ||
                    user == "pzand@thesmartcircle.com" ||
                    user == "maryt@thesmartcircle.com")
                {
                    ScheduleTrainer.Visible = true;
                    contentFrame.Src = "ScheduleTrainer.aspx";

                    if (user != "epark@thesmartcircle.com")
                    {
                        AuditSales.Visible = true;
                    }
                }
            }
            catch
            {
                //loginDiv.Visible = true;
                dashDivPre.Visible = false;
                dashDiv.Visible = false;
            }

            string checkLast = @"select name as Job, convert(nvarchar, convert(date, run_requested_date), 101) + ' @ ' + convert(nvarchar, left(convert(time, run_requested_date, 101), 5)) as LastUpdated
                                 from msdb.dbo.SysJobs j 
                                 left join msdb.dbo.SysJobActivity a on a.job_id = j.job_id 
                                 where name in ('RS Refresh', 'RS Post')
                                ";

            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(checkLast, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["Job"].ToString() == "RS Refresh")
                        {
                            refreshBtn.ToolTip = "Last Refreshed: " + reader["LastUpdated"].ToString();
                        }
                        if (reader["Job"].ToString() == "RS Post")
                        {
                            postBtn.ToolTip = "Last Posted: " + reader["LastUpdated"].ToString();
                        }
                    }
                }
                con.Close();
            }

            try
            {
                string IsRunning = Session["job"].ToString();


                if (userType == "admin" || userType == "SC")
                {
                    ScriptManager.RegisterStartupScript(Page, typeof(Page), "alertBox", "alert('" + IsRunning + "');", true);
                }

                Session.Remove("job");
            }
            catch { }
        }

        //protected void loginBtn_Click(object sender, EventArgs e)
        //{
        //    notifSpan.Visible = true;
        //    notif.Text = "Invalid username or password";
        //    notif.ForeColor = System.Drawing.Color.White;
        //    menu.Attributes.Add("style", "background-color: red;");
        //    bool loggedIn = false;

        //    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        //    {
        //        string selectCreds = "SELECT Password, Firstname, Lastname, Type FROM PAYOUTlogin WHERE Username = '" + username.Text + "'";
        //        con.Open();
        //        using (SqlCommand cmd = new SqlCommand(selectCreds, con))
        //        {
        //            SqlDataReader reader = cmd.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                if (reader[0].ToString() == password.Text)
        //                {
        //                    Session["user"] = username.Text;
        //                    string userFullname = reader[1].ToString() + " " + reader[2].ToString();
        //                    homeBtn.Text = userFullname;
        //                    Session["userFullname"] = userFullname;
        //                    Session["userType"] = reader[3].ToString();
        //                    logoutSpan.Visible = true;
        //                    loginDiv.Visible = false;
        //                    dashDiv.Visible = true;
        //                    menuBtns.Visible = true;
        //                    notifSpan.Visible = false;
        //                    menu.Attributes.Add("style", "background-color: #2764AB;");
        //                    loggedIn = true;
        //                    if (reader[3].ToString() != "Owner")
        //                    {
        //                        dashDivPre.Visible = true;
        //                    }
        //                }
        //            }
        //        }
        //        con.Close();

        //        if (loggedIn)
        //        {
        //            string updateLog = "UPDATE PAYOUTlogin SET LastLogin = CURRENT_TIMESTAMP WHERE Username = '" + username.Text + "'";
        //            con.Open();
        //            using (SqlCommand cmd = new SqlCommand(updateLog, con))
        //            {
        //                SqlDataReader reader = cmd.ExecuteReader();
        //            }
        //            con.Close();
        //        }

        //        Response.Redirect("~/", true);
        //    }
        //}

        protected void homeBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/", true);
        }

        /*FIX THIS LOGOUT*/
        protected void logoutBtn_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            //try { Session.Remove("user"); } catch { }
            //try { Session.Remove("userFullname"); } catch { }
            //try { Session.Remove("type"); } catch { }

            //try { Session.Remove("StoreNumber"); } catch { }
            //try { Session.Remove("StoreName"); } catch { }
            //try { Session.Remove("Program"); } catch { }
            //try { Session.Remove("StartDate"); } catch { }
            //try { Session.Remove("Owner"); } catch { }

            //try { Session.Remove("g2StartDate"); } catch { }
            //try { Session.Remove("g2Duration"); } catch { }
            //try { Session.Remove("g2Owner"); } catch { }
            //try { Session.Remove("g2Hub"); } catch { }
            //try { Session.Remove("g2StoreName"); } catch { }
            //try { Session.Remove("g2Program"); } catch { }
            //try { Session.Remove("g2StoreNumber"); } catch { }
            //try { Session.Remove("g2Location"); } catch { }
            
            Response.Redirect(string.Format("{0}/api/account/Logout", ConfigurationManager.AppSettings["ServerAPIURL"]));

            //Response.Redirect("~/", true);
        }

        void execJob(string job)
        {
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string exec = "exec msdb.dbo.sp_start_job [" + job + "]";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(exec, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }
        }

        protected void refreshBtn_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            
            string refreshData_confirm_value = Request.Form["refreshData_confirm_value"];
            string job = "RS " + System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(btn.ID.Replace("Btn", "").ToLower());

            string check = @"select name as Job, convert(date, run_requested_date) as LastUpdatedDate, left(convert(time, run_requested_date, 101), 8) as LastUpdatedTime, last_executed_step_date as LastStep, start_execution_date as StartTime, stop_execution_date as StopTime, start_step_id as StartStep, enabled as Enabled 
                             from msdb.dbo.SysJobs j 
                             left join msdb.dbo.SysJobActivity a on a.job_id = j.job_id 
                             where name in ('" + job + @"')
                            ";

            if (refreshData_confirm_value == "Yes")
            {
                using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(check, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            if (reader["StopTime"] == null || reader["StopTime"] == DBNull.Value)
                            {
                                Session["job"] = "Still " + btn.ID.Replace("Btn", "") + "ing data...";
                            }
                            else
                            {
                                execJob(job);
                            }
                        }
                    }
                    con.Close();

                    string WEmaint = string.Empty;
                    if (job == "RS Refresh")
                    {
                        WEmaint = "UPDATE [PAYOUTwe] SET [LastRefreshOn] = CURRENT_TIMESTAMP, [LastRefreshBy] = '" + userFullname + "' WHERE [WeekEnding] = (select max(WeekEnding) from PAYOUTwe)";
                    }
                    if (job == "RS RefreshDev")
                    {
                        WEmaint = "UPDATE [PAYOUTwe] SET [LastRefreshOn] = CURRENT_TIMESTAMP, [LastRefreshBy] = '" + userFullname + "' WHERE [WeekEnding] = (select max(WeekEnding) from PAYOUTwe)";
                    }
                    else
                    {
                        WEmaint = "UPDATE [PAYOUTwe] SET [LastPostOn] = CURRENT_TIMESTAMP, [LastPostBy] = '" + userFullname + "' WHERE [WeekEnding] = (select max(WeekEnding) from PAYOUTwe)";
                    }
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(WEmaint, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                    con.Close();
                }
            }
        }

        protected void transferBtn_Click(object sender, EventArgs e)
        {
            string transferData_confirm_value = Request.Form["transferData_confirm_value"];

            string check = @"delete from Herbjoy.dbo.PAYOUThistory 
                            where SalesDate between '2015-05-18' and '2015-06-14' 

                            insert into Herbjoy.dbo.PAYOUThistory 
                            select [Id],[Program],[SalesDate],[StoreName],[StoreNumber],[ItemNumber],[ItemName],[Qty],[UnitCost],[ExtendedCost],[ImportedBy],[ImportedOn] 
                            from Payout.dbo.PAYOUTsales 
                            where Archive = 0 and SalesDate between '2015-05-18' and '2015-06-14' 

                            delete from Herbjoy.dbo.PAYOUTschedule 
                            where StartDate >= '2015-05-01' 

                            insert into Herbjoy.dbo.PAYOUTschedule 
                            select [Id],[Program],[StartDate],[EndDate],[StoreName],[StoreNumber],[City],[State],[OwnerFirstname],[OwnerLastname],[HubFirstname],[HubLastname],[ImportedBy],[ImportedOn] 
                            from Payout.dbo.PAYOUTschedule 
                            where StartDate >= '2015-05-01'

                            update Herbjoy.dbo.PAYOUThistory
                            set Program = 'Fast Wax - C'
                            where SalesDate between '2015-05-18' and '2015-06-14'
                            and StoreName in ('Kroger - Loaf & Jug', 'Kroger - Turkey Hill', 'Kroger - Tom Thumb', 'Kroger - Pueblo')

                            update Herbjoy.dbo.PAYOUTschedule
                            set Program = 'Fast Wax - C'
                            where StartDate >= '2015-05-01'
                            and StoreName in ('Kroger - Loaf & Jug', 'Kroger - Turkey Hill', 'Kroger - Tom Thumb', 'Kroger - Pueblo')

                            delete from [Herbjoy].[dbo].[PAYOUTpeople]
                            insert into [Herbjoy].[dbo].[PAYOUTpeople]
                            select [Id], [Firstname], [Lastname], [Email]
                            from [Payout].[dbo].[PAYOUTpeople]

                            delete from [Herbjoy].[dbo].[PAYOUTmailList]
                            insert into [Herbjoy].[dbo].[PAYOUTmailList]
                            select [To], [CC1], [CC2], [CC3], [CC4], [CC5], [CC6]
                            from [Payout].[dbo].[PAYOUTmailList]

                            delete from [Herbjoy].[dbo].[PAYOUTexecs]
                            insert into [Herbjoy].[dbo].[PAYOUTexecs]
                            select [Id], [Person], [ExecOf]
                            from [Payout].[dbo].[PAYOUTexecs]
                            ";

            if (transferData_confirm_value == "Yes")
            {
                using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(check, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                    con.Close();
                }
            }
        }

        protected void panelWE_Click(object sender, EventArgs e)
        {
            Response.Redirect("WE.aspx", true);
        }        
    }
}