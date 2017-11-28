using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Data.OleDb;
using System.Text;
using System.Web.Configuration;
using System.Net;

namespace WebApplication3
{
    public partial class Grids : System.Web.UI.Page
    {
        string user = string.Empty;
        string userFullname = string.Empty;
        string userType = string.Empty;
        string StoreName = string.Empty;
        string StoreNumber = string.Empty;
        string Program = string.Empty;
        string StartDate = string.Empty;
        string StartDateDDL = string.Empty;
        string Duration = string.Empty;
        string ReportType = string.Empty;
        string Owner = string.Empty;
        string Location = string.Empty;     
        string ExludedPrograms = string.Empty;
        string SQLtable = string.Empty;
        string where = string.Empty;
        bool ControlSheet;
        bool isFastWax;
        bool isweeklyshow;
        bool isFWBoundle;
        bool isdailysales;
        bool isreturn;


        ArrayList storeList = new ArrayList();
        string[] listData;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                user = Session["user"].ToString();
                userFullname = Session["userFullname"].ToString();
                userType = Session["userType"].ToString().ToLower();
                userType = userType == "payoutitadmin" ? "admin" : userType;
                userType = userType == "payoutowners" ? "owner" : userType;
                Session["ReportType"] = "payout";
                ReportType = "payout";
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }

            try
            {
                string error = Session["Error"].ToString();

                errorMsg.Text = error;
                ClientScript.RegisterStartupScript(this.GetType(), "showError", "<script language='javascript'>$('#loadDiv').addClass('keep'); $('#loadDiv').show(); $('#errorSpan').addClass('keep'); $('#errorSpan').show();</script>");

                Session.Remove("Error");
            }
            catch { }

            ctrlSheet.Visible = false;
            fastWax.Visible = false;
            fastWaxTotals.Visible = false;
            changesBtn.Visible = false;
            exportAvg.Visible = false;
            email.Visible = false;
            lblLoc.Visible = true;
            lblOwner.Visible = true;
            lblprgm.Visible = true;
            lblStore.Visible = true;
            lblStorNum.Visible = true;
            lblWeekending.Visible = true;
            webStartDate.Visible = true;
            webOwner.Visible = true;
            webStoreName.Visible = true;
            webProgram.Visible = true;
            webLocation.Visible = true;
            webStoreNumber.Visible = true;
           
        
            if (!Request.Url.AbsoluteUri.ToLower().Contains("localhost") &&
                !Request.Url.AbsoluteUri.ToLower().Contains("podev"))
            {
                krogerPeriods.Visible = false;
            }

            if (userType == "admin" || userType == "sc" || userType == "rsm")
            {
                ctrlSheet.Visible = true;
                fastWax.Visible = true;
                fastWaxTotals.Visible = true;

                if (user == "Admin" || user == "andjelka@thesmartcircle.com")
                {
                    changesBtn.Visible = true;
                    exportAvg.Visible = true;
                }

                SQLtable = "PAYOUTsummary";
                where = "WHERE [PAYOUTwe].[Internal] = 1";
            }
            else
            {

               
                DailySales.Visible = false;
                fastWax.Visible = false;
                fastWaxTotals.Visible = false;
                WeeklyShow.Visible = false;
                FWBoundle.Visible = false;
                Returns.Visible = false;
                krogerPeriods.Visible = false;

           
                
                SQLtable = "PAYOUTsummaryPosted";
                where = "WHERE [PAYOUTwe].[" + userType + "] = 1";
            }

            //weSQL.SelectCommand = "SELECT 'Select WeekEnding' WeekEnding, '' StartDate,'' weDate UNION ALL SELECT DISTINCT CONVERT(NVARCHAR, CONVERT(DATE, [Week Ending]), 101) AS [WeekEnding], CONVERT(NVARCHAR, DATEADD(DAY, -13, CONVERT(DATE, [Week Ending]))) AS [StartDate], CONVERT(DATE, [WeekEnding]) AS [weDate] FROM [" + SQLtable + "] JOIN [PAYOUTwe] ON CONVERT(DATE, [PAYOUTwe].[WeekEnding]) = CONVERT(DATE, [" + SQLtable + "].[Week Ending]) " + where + " ORDER BY [weDate] DESC";


            weSQL.SelectCommand = @"SELECT 0 No, 'Select WeekEnding' WeekEnding, '' StartDate, '' weDate 

                                    UNION ALL

                                    SELECT ROW_NUMBER() OVER (ORDER BY weDate1 DESC) AS No, WeekEnding, StartDate, weDate1 weDate
                                    FROM
                                    (
                                    SELECT DISTINCT CONVERT(NVARCHAR, CONVERT(DATE, [Week Ending]), 101) AS [WeekEnding], 
                                    CONVERT(NVARCHAR, DATEADD(DAY, -13, CONVERT(DATE, [Week Ending]))) AS [StartDate], 
                                    CONVERT(DATE, [WeekEnding]) AS [weDate1] 
                                    FROM [PAYOUTsummary] 
                                    JOIN [PAYOUTwe] ON CONVERT(DATE, [PAYOUTwe].[WeekEnding]) = CONVERT(DATE, [PAYOUTsummary].[Week Ending]) 
                                    WHERE [PAYOUTwe].[Internal] = 1 
                                    ) tab
                                    ORDER BY No";


            if (!IsPostBack)
            {
               webStartDate.DataBind();
                webStartDate.SelectedIndex = 0;

                if (ReportType == "payout")
                {
                    try
                    {
                        StoreNumber = Session["StoreNumber"].ToString();
                        StoreName = Session["StoreName"].ToString();
                        Program = Session["Program"].ToString();
                        StartDate = Session["StartDate"].ToString();
                        Owner = Session["Owner"].ToString();
                        Location = Session["Location"].ToString();
                      
                    }
                    catch
                    {
                        StoreNumber = "All";
                        StoreName = "All";
                        Program = "All";
                        Location = "All";
                        StartDate = webStartDate.SelectedValue == "Select WeekEnding" ? "" : webStartDate.SelectedValue;
                    }

                    try
                    {
                        if (StoreNumber == "All")
                        {
                            webStoreNumber.Text = "";
                        }
                        else
                        {
                            webStoreNumber.Text = StoreNumber;
                        }
                        webStoreName.SelectedValue = StoreName;
                        webProgram.SelectedValue = Program;
                        webStartDate.SelectedValue = StartDate;
                        webOwner.SelectedValue = Owner;
                        webLocation.SelectedValue = Location;
                    
                    }
                    catch { }

                    if (userType == "owner")
                    {
                        SQLs1.SelectParameters.Add("webOwner", userFullname);
                        Session["Owner"] = userFullname;
                    }
                    else
                    {
                        try
                        {
                            Owner = Session["Owner"].ToString();
                        }
                        catch
                        {
                            Owner = "All";
                            Session["Owner"] = "All";
                        }

                        SQLs1.SelectParameters.Add("webOwner", Owner);
                    }

                    Session["StoreName"] = StoreName;
                    Session["Program"] = Program;
                    Session["StartDate"] = StartDate;
                    Session["Location"] = Location;
              

                    SQLs1.SelectParameters.Add("webStartDate", StartDate);
                    SQLs1.SelectParameters.Add("webDuration", "14");
                    SQLs1.SelectParameters.Add("webProgram", Program);
                    SQLs1.SelectParameters.Add("webStoreName", StoreName);
                    SQLs1.SelectParameters.Add("webStoreNumber", StoreNumber);
                    SQLs1.SelectParameters.Add("UserType", userType);
                    SQLs1.SelectParameters.Add("userFullname", userFullname);
                    SQLs1.SelectParameters.Add("webLocation", Location);

                    if (Program == "Chipio - Chip Repair")
                    {
                        SQLs1.SelectParameters.Add("cols", "Chipio");
                    }
                    else
                    {
                        SQLs1.SelectParameters.Add("cols", "All");
                    }
                }
            }

            try
            {
                ControlSheet = (bool)Session["ControlSheet"];
            }
            catch
            {
                ControlSheet = false;
            }

            try
            {
                isFastWax = (bool)Session["fastWax"];
            }
            catch
            {
                isFastWax = false;
            }

            try
            {
                isreturn = (bool)Session["return"];
            }
            catch
            {
                isreturn = false;
            }


            try
            {
                isweeklyshow = (bool)Session["WeeklyShow"];
            }
            catch
            {
                isweeklyshow = false;
            }

            try
            {
                isFWBoundle = (bool)Session["FWBoundle"];
            }
            catch
            {
                isFWBoundle = false;
            }

              try
            {
                isdailysales = (bool)Session["DailySales"];
            }
            catch
            {
                isdailysales = false;
            }
            

            if (userType == "admin" || userType == "sc" || userType == "rsm")
            {
                if (ControlSheet)
                {
                    SQLs1.SelectCommand = "spx_PAYOUTsummaryDaily";
                    exportAll.Visible = false;
                    fastWax.Visible = false;
                    fastWaxTotals.Visible = false;
                    Returns.Visible = false;
                    fwSpan.Visible = true;
                    changesBtn.Visible = false;
                    exportSummary.Text = "Export Control Sheet to Excel";
                    ctrlSheet.Text = "Back";
                }
                else if (isdailysales)
                {
               
                    SQLs1.SelectCommand = "spx_PAYOUTSalesDaily";
                    exportSummary.Text = "Export Sales Daily to Excel";
                    DailySales.Text = "Back";
                    fastWax.Visible = false;
                    fastWaxTotals.Visible = false;
                    krogerPeriods.Visible = false;
                    WeeklyShow.Visible = false;
                    exportAll.Visible = false;
                    changesBtn.Visible = false;
                    exportAvg.Visible = false;
                    ctrlSheet.Visible = false;
                    Returns.Visible = false;
                }

                else  if (isweeklyshow)
                {
                    SQLs1.SelectCommand = "spx_PAYOUTDaily";
                    exportSummary.Text = "Export weekly show to Excel";
                    ctrlSheet.Text = "Back";
                    fastWax.Visible = false;
                    fastWaxTotals.Visible = false;
                   krogerPeriods.Visible = false;
                    WeeklyShow.Visible = false;
                    exportAll.Visible = false;
                   changesBtn.Visible = false;
                    exportAvg.Visible = false;
                    DailySales.Visible = false;
                    Returns.Visible = false;
                   
                }

                else if (isreturn)
                {
                    SQLs1.SelectCommand = "spx_PAYOUTReturnProgram";
                    exportSummary.Text = "Export weekly show to Excel";
                    ctrlSheet.Text = "Back";
                    fastWax.Visible = false;
                    fastWaxTotals.Visible = false;
                    krogerPeriods.Visible = false;
                    WeeklyShow.Visible = false;
                    exportAll.Visible = false;
                    changesBtn.Visible = false;
                    exportAvg.Visible = false;
                    DailySales.Visible = false;
                    Returns.Visible = false;

                }
                else if (isFWBoundle)
                {
                    SQLs1.SelectCommand = "spx_FWBoundleReport";
                    exportSummary.Text = "Export FWBundle Report to Excel";
                    ctrlSheet.Text = "Back";
                    fastWax.Visible = false;
                    fastWaxTotals.Visible = false;
                    krogerPeriods.Visible = false;
                    WeeklyShow.Visible = false;
                    FWBoundle.Visible = false;
                    exportAll.Visible = false;
                    changesBtn.Visible = false;
                    exportAvg.Visible = false;
                    DailySales.Visible = false;
                    Returns.Visible = false;

                }
                else 
                {
                    if (isFastWax)
                    {
                        SQLs1.SelectCommand = "spx_PAYOUTfastWax";
                        exportAll.Visible = false;
                        ctrlSheet.Visible = false;
                        fwSpan.Visible = false;
                        changesBtn.Visible = false;
                        Returns.Visible = false;
                        DailySales.Visible = false;
                        exportSummary.Text = "Export FW to Excel";
                        ctrlSheet.Text = "Back";
                    }
                    else
                    {
                        SQLs1.SelectCommand = "spx_PAYOUTsummary";
                        exportAll.Visible = true;
                        
                      
                        
                        fwSpan.Visible = true;
                        if (user == "Admin" || user == "andjelka@thesmartcircle.com")
                        {
                            changesBtn.Visible = true;
                            exportAvg.Visible = true;
                            DailySales.Visible = true;
                            fastWax.Visible = true;
                            fastWaxTotals.Visible = true;
                            WeeklyShow.Visible = true;
                            FWBoundle.Visible = true;
                            Returns.Visible = true;
                        }
                        else
                        {
                            changesBtn.Visible = false;
                            exportAvg.Visible = false;
                        }
                        exportSummary.Text = "Export Summary to Excel";
                        fastWax.Text = "FW Report";
                        ctrlSheet.Text = "Control Sheet";
                    }
                }
            }
            else
            {
                SQLs1.SelectCommand = "spx_PAYOUTsummary";
            }



            //if (IsPostBack)
            //{
            //    GVs1.DataBind();
            //}
            //else
            //{
            //    SQLs1.SelectCommand = "";
            //}

            GVs1.DataBind();
            getDDL();
            getList();

            //else
            //{
            //    GVs1L.Text = "Qty Sold";

            //    SQLs1.SelectParameters.Add("webStartDate", StartDate);
            //    SQLs1.SelectParameters.Add("webDuration", Duration);
            //    SQLs1.SelectParameters.Add("webProgram", Program);
            //    SQLs1.SelectParameters.Add("webStoreName", StoreName);
            //    SQLs1.SelectParameters.Add("webOwner", Owner);
            //    SQLs1.SelectCommand = "spx_PAYOUTQtySummary";

            //    GVs1.DataBind();

            //    GVs2L.Text = "Commissionable Revenue";

            //    SQLs2.SelectParameters.Add("webStartDate", StartDate);
            //    SQLs2.SelectParameters.Add("webDuration", Duration);
            //    SQLs2.SelectParameters.Add("webProgram", Program);
            //    SQLs2.SelectParameters.Add("webStoreName", StoreName);
            //    SQLs2.SelectParameters.Add("webOwner", Owner);
            //    SQLs2.SelectCommand = "spx_PAYOUTRevSummary";

            //    GVs2.DataBind();
            //}

            //MIX
            //if (ReportType != "payout")
            //{
            //    GVmL.Text = "Mix";
            //
            //    SQLm.SelectParameters.Add("webStartDate", StartDate);
            //    SQLm.SelectParameters.Add("webDuration", Duration);
            //    SQLm.SelectParameters.Add("webProgram", Program);
            //    SQLm.SelectParameters.Add("webStoreName", StoreName);
            //    SQLm.SelectParameters.Add("webOwner", Owner);
            //
            //    GVm.DataBind();
            //}
        }

        void getDDL()
        {
            string DateDDL = string.Empty;
            string OwnerDDL = string.Empty;
            string StoreNameDDL = string.Empty;
            string ProgramDDL = string.Empty;
            string LocationDDL = string.Empty;
 
            string selectedOwner = string.Empty;
            string selectedStoreName = string.Empty;
            string selectedProgram = string.Empty;
            string selectedLocation = string.Empty;
       

            DateDDL = webStartDate.SelectedValue;
            OwnerDDL = webOwner.SelectedValue;
            StoreNameDDL = webStoreName.SelectedValue;
            ProgramDDL = webProgram.SelectedValue;
            LocationDDL = webLocation.SelectedValue;
      

            if (OwnerDDL == "") { OwnerDDL = "All"; }
            if (StoreNameDDL == "") { OwnerDDL = "All"; }
            if (ProgramDDL == "") { OwnerDDL = "All"; }
            if (LocationDDL == "") { LocationDDL = "All"; }
           

            if (OwnerDDL == "All") { selectedOwner = ""; } else { selectedOwner = OwnerDDL; }
            if (StoreNameDDL == "All") { selectedStoreName = ""; } else { selectedStoreName = StoreNameDDL; }
            if (ProgramDDL == "All") { selectedProgram = ""; } else { selectedProgram = ProgramDDL; }
            if (LocationDDL == "All") { selectedLocation = ""; } else { selectedLocation = LocationDDL; }
           

            //if (Convert.ToDateTime(DateDDL) < Convert.ToDateTime("2015-04-20"))
            //{
            //    ExludedPrograms = "'Chipio - Chip Repair', 'Fast Wax', 'Fast Wax Inside'";
            //}
            //else
            //{
            //    ExludedPrograms = "'Chipio - Chip Repair'";
            //}
            ExludedPrograms = "''";

            if (userType == "owner")
            {
                ownerSpan.Visible = false;
                storeSQL.SelectCommand = "SELECT 'All' AS [StoreName] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [StoreName] FROM ( SELECT CASE WHEN [Store Name] LIKE 'Kroger %' THEN 'Kroger' ELSE [Store Name] END AS StoreName FROM [" + SQLtable + "] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') AND [Payout Owner Name] = '" + userFullname + "' AND Program NOT IN (" + ExludedPrograms + ") )x Group BY [StoreName] Order BY [StoreName] )ddl";
                progSQL.SelectCommand = "SELECT 'All' AS [Program] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [Program] FROM [" + SQLtable + "] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') AND [Payout Owner Name] = '" + userFullname + "' AND Program NOT IN (" + ExludedPrograms + ") Group BY [Program] Order BY [Program] )ddl";
                locSQL.SelectCommand = "SELECT 'All' AS [Location] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [Location] FROM [" + SQLtable + "] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') AND [Payout Owner Name] = '" + userFullname + "' AND Program NOT IN (" + ExludedPrograms + ") AND [Program] LIKE '%" + selectedProgram + "%' Group BY [Location] Order BY [Location] )ddl";
                
            }
            else if (userType == "hub")
            {
                ownerSQL.SelectCommand = "SELECT 'All' AS [OwnerName] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [Payout Owner Name] AS OwnerName FROM [" + SQLtable + "] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') AND [" + SQLtable + "].[Hub] = '" + userFullname + "' AND Program NOT IN (" + ExludedPrograms + ") Group BY [Payout Owner Name] Order BY [Payout Owner Name] )ddl";
                storeSQL.SelectCommand = "SELECT 'All' AS [StoreName] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [StoreName] FROM ( SELECT CASE WHEN [Store Name] LIKE 'Kroger %' THEN 'Kroger' ELSE [Store Name] END AS StoreName FROM [" + SQLtable + "] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') AND [" + SQLtable + "].[Hub] = '" + userFullname + "' AND Program NOT IN (" + ExludedPrograms + ") AND [Payout Owner Name] LIKE '%" + selectedOwner + "%' )x Group BY [StoreName] Order BY [StoreName] )ddl";
                progSQL.SelectCommand = "SELECT 'All' AS [Program] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [Program] FROM [" + SQLtable + "] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') AND [" + SQLtable + "].[Hub] = '" + userFullname + "' AND Program NOT IN (" + ExludedPrograms + ") AND [Payout Owner Name] LIKE '%" + selectedOwner + "%' Group BY [Program] Order BY [Program] )ddl";
                locSQL.SelectCommand = "SELECT 'All' AS [Location] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [Location] FROM [" + SQLtable + "] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') AND [" + SQLtable + "].[Hub] = '" + userFullname + "' AND Program NOT IN (" + ExludedPrograms + ") AND [Payout Owner Name] LIKE '%" + selectedOwner + "%' AND [Program] LIKE '%" + selectedProgram + "%' Group BY [Location] Order BY [Location] )ddl";
                 
            }
            else if (userType == "nc")
            {
                ownerSQL.SelectCommand = "SELECT 'All' AS [OwnerName] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [Payout Owner Name] AS OwnerName FROM [" + SQLtable + "] JOIN PAYOUTmapNC ON Owner = [Payout Owner Name] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') AND NC = '" + userFullname + "' AND Program NOT IN (" + ExludedPrograms + ") Group BY [Payout Owner Name] Order BY [Payout Owner Name] )ddl";
                storeSQL.SelectCommand = "SELECT 'All' AS [StoreName] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [StoreName] FROM ( SELECT CASE WHEN [Store Name] LIKE 'Kroger %' THEN 'Kroger' ELSE [Store Name] END AS StoreName FROM [" + SQLtable + "] JOIN PAYOUTmapNC ON Owner = [Payout Owner Name] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') AND NC = '" + userFullname + "' AND Program NOT IN (" + ExludedPrograms + ") AND [Payout Owner Name] LIKE '%" + selectedOwner + "%' )x Group BY [StoreName] Order BY [StoreName] )ddl";
                progSQL.SelectCommand = "SELECT 'All' AS [Program] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [Program] FROM [" + SQLtable + "] JOIN PAYOUTmapNC ON Owner = [Payout Owner Name] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') AND NC = '" + userFullname + "' AND Program NOT IN (" + ExludedPrograms + ") AND [Payout Owner Name] LIKE '%" + selectedOwner + "%' Group BY [Program] Order BY [Program] )ddl";
                locSQL.SelectCommand = "SELECT 'All' AS [Location] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [Location] FROM [" + SQLtable + "] JOIN PAYOUTmapNC ON Owner = [Payout Owner Name] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') AND NC = '" + userFullname + "' AND Program NOT IN (" + ExludedPrograms + ") AND [Payout Owner Name] LIKE '%" + selectedOwner + "%' AND [Program] LIKE '%" + selectedProgram + "%' Group BY [Location] Order BY [Location] )ddl";
                
            }
            else if (userType == "rc")
            {
                ownerSQL.SelectCommand = "SELECT 'All' AS [OwnerName] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [Payout Owner Name] AS OwnerName FROM [" + SQLtable + "] JOIN PAYOUTmapRC ON Owner = [Payout Owner Name] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') AND RC = '" + userFullname + "' AND Program NOT IN (" + ExludedPrograms + ") Group BY [Payout Owner Name] Order BY [Payout Owner Name] )ddl";
                storeSQL.SelectCommand = "SELECT 'All' AS [StoreName] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [StoreName] FROM ( SELECT CASE WHEN [Store Name] LIKE 'Kroger %' THEN 'Kroger' ELSE [Store Name] END AS StoreName FROM [" + SQLtable + "] JOIN PAYOUTmapRC ON Owner = [Payout Owner Name] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') AND RC = '" + userFullname + "' AND Program NOT IN (" + ExludedPrograms + ") AND [Payout Owner Name] LIKE '%" + selectedOwner + "%' )x Group BY [StoreName] Order BY [StoreName] )ddl";
                progSQL.SelectCommand = "SELECT 'All' AS [Program] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [Program] FROM [" + SQLtable + "] JOIN PAYOUTmapRC ON Owner = [Payout Owner Name] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') AND RC = '" + userFullname + "' AND Program NOT IN (" + ExludedPrograms + ") AND [Payout Owner Name] LIKE '%" + selectedOwner + "%' Group BY [Program] Order BY [Program] )ddl";
                locSQL.SelectCommand = "SELECT 'All' AS [Location] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [Location] FROM [" + SQLtable + "] JOIN PAYOUTmapRC ON Owner = [Payout Owner Name] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') AND RC = '" + userFullname + "' AND Program NOT IN (" + ExludedPrograms + ") AND [Payout Owner Name] LIKE '%" + selectedOwner + "%' AND [Program] LIKE '%" + selectedProgram + "%' Group BY [Location] Order BY [Location] )ddl";
               
            }
            //else if (userType == "RSM")
            //{
            //    ownerSQL.SelectCommand = "SELECT 'All' AS [OwnerName] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [Payout Owner Name] AS OwnerName FROM [" + SQLtable + "] WHERE [Week Ending] = DATEADD(DAY, -13, '" + DateDDL + "') AND Program NOT IN (" + ExludedPrograms + ") Group BY [Payout Owner Name] Order BY [Payout Owner Name] )ddl";
            //    storeSQL.SelectCommand = "SELECT 'All' AS [StoreName] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [StoreName] FROM ( SELECT CASE WHEN [Store Name] LIKE 'Kroger %' THEN 'Kroger' ELSE [Store Name] END AS StoreName FROM [" + SQLtable + "] WHERE [Week Ending] = DATEADD(DAY, -13, '" + DateDDL + "') AND Program NOT IN (" + ExludedPrograms + ") AND [Payout Owner Name] LIKE '%" + selectedOwner + "%' )x Group BY [StoreName] Order BY [StoreName] )ddl";
            //    progSQL.SelectCommand = "SELECT 'All' AS [Program] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [Program] FROM [" + SQLtable + "] WHERE [Week Ending] = DATEADD(DAY, -13, '" + DateDDL + "') AND Program NOT IN (" + ExludedPrograms + ") AND [Payout Owner Name] LIKE '%" + selectedOwner + "%' Group BY [Program] Order BY [Program] )ddl";
            //    locSQL.SelectCommand = "SELECT 'All' AS [Location] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [Location] FROM [" + SQLtable + "] WHERE [Week Ending] = DATEADD(DAY, -13, '" + DateDDL + "') AND Program NOT IN (" + ExludedPrograms + ") AND [Payout Owner Name] LIKE '%" + selectedOwner + "%' AND [Program] LIKE '%" + selectedProgram + "%' Group BY [Location] Order BY [Location] )ddl";
            //}
            else if (userType == "admin" || userType == "sc" || userType == "rsm")
            {
                ownerSQL.SelectCommand = "SELECT 'All' AS [OwnerName] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [Payout Owner Name] AS OwnerName FROM [" + SQLtable + "] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') Group BY [Payout Owner Name] Order BY [Payout Owner Name] )ddl";
                storeSQL.SelectCommand = "SELECT 'All' AS [StoreName] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [StoreName] FROM ( SELECT CASE WHEN [Store Name] LIKE 'Kroger %' THEN 'Kroger' ELSE [Store Name] END AS StoreName FROM [" + SQLtable + "] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') AND [Payout Owner Name] LIKE '%" + selectedOwner + "%' )x Group BY [StoreName] Order BY [StoreName] )dll";
                progSQL.SelectCommand = "SELECT 'All' AS [Program] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [Program] FROM [" + SQLtable + "] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') AND [Payout Owner Name] LIKE '%" + selectedOwner + "%' AND [Store Name] LIKE '%" + selectedStoreName + "%' Group BY [Program] Order BY [Program] )ddl";
                locSQL.SelectCommand = "SELECT 'All' AS [Location] UNION ALL SELECT * FROM ( SELECT TOP 100000000 [Location] FROM [" + SQLtable + "] WHERE [Week Ending] = DATEADD(DAY, 13, '" + DateDDL + "') AND [Payout Owner Name] LIKE '%" + selectedOwner + "%' AND [Store Name] LIKE '%" + selectedStoreName + "%' AND [Program] LIKE '%" + selectedProgram + "%' Group BY [Location] Order BY [Location] )ddl";
                

            }

            ownerSQL.DataBind();
            storeSQL.DataBind();
            progSQL.DataBind();
            locSQL.DataBind();
          

            if (userType != "owner")
            {
                try
                {
                    webOwner.DataBind();
                }
                catch { }
            }
            try
            {
                webStoreName.DataBind();
            }
            catch { }
            try
            {
                webProgram.DataBind();
            }
            catch { }
            try
            {
                webLocation.DataBind();
            }
            catch { }

           
            try { webOwner.SelectedValue = OwnerDDL; }
            catch { Session["Error"] = "Selected owner was not found. Showing all results."; Response.Redirect("~/Grids.aspx", true); }
            try { webStoreName.SelectedValue = StoreNameDDL; }
            catch { Session["Error"] = "Selected store was not found. Showing all results."; Response.Redirect("~/Grids.aspx", true); }
            try { webProgram.SelectedValue = ProgramDDL; }
            catch { Session["Error"] = "Selected program was not found. Showing all results."; Response.Redirect("~/Grids.aspx", true); }
            try { webLocation.SelectedValue = LocationDDL; }
            catch { Session["Error"] = "Selected location was not found. Showing all results."; Response.Redirect("~/Grids.aspx", true); }
         
        }

        void getList()
        {
            storeList.Clear();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                //Stores List

                string qStore = string.Empty;
                string qProgram = string.Empty;
                string qOwner = string.Empty;
                string qLocation = string.Empty;
                


                if (webStoreName.SelectedValue == "All") { qStore = ""; } else { qStore = webStoreName.SelectedValue; }
                if (webProgram.SelectedValue == "All") { qProgram = ""; } else { qProgram = webProgram.SelectedValue; }
                if (webOwner.SelectedValue == "All") { qOwner = ""; } else { qOwner = webOwner.SelectedValue; }
                if (webLocation.SelectedValue == "All") { qLocation = ""; } else { qLocation = webLocation.SelectedValue; }

                string selectStores = "SELECT [Club #], [Store Name], [Location], [Payout Owner Name], [Hub], [Program] FROM [" + SQLtable + "] WHERE [Week Ending] = CONVERT(NVARCHAR, DATEADD(DAY, 13, '" + webStartDate.SelectedValue + "'), 101) AND [Store Name] LIKE '%" + qStore + "%' AND [Program] LIKE '%" + qProgram + "%' AND [Payout Owner Name] LIKE '%" + qOwner + "%' AND [Location] LIKE '%" + qLocation + "%' AND [Payout Owner Name] != 'Chad Powers' GROUP BY [Program], [Payout Owner Name], [Store Name], [Club #], [Location], [Hub] ORDER BY [Club #], [Program]";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(selectStores, con))
                {
                    cmd.CommandTimeout = 3600;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string tbStoreNumber = reader["Club #"].ToString();
                        string tbStoreName = reader["Store Name"].ToString();
                        string tbLocation = reader["Location"].ToString();
                        string tbOwner = reader["Payout Owner Name"].ToString();
                        //string tbOwner = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(reader["Payout Owner Name"].ToString().ToLower());
                        string tbHub = reader["Hub"].ToString();
                        string tbProgram = reader["Program"].ToString();

                        listData = new string[6] { tbStoreNumber, tbStoreName, tbLocation, tbOwner, tbHub, tbProgram };
                        storeList.Add(listData);
                    }
                }
                con.Close();
            }
        }

        void exportFile(string what)
        {
            ExcelPackage pck = new ExcelPackage();
            string shStartDate = webStartDate.SelectedValue;
            string exStartDate = webStartDate.SelectedItem.Text.Replace("/", "-");
            string filename = string.Empty;
            string MainSheet = string.Empty;

            if (ControlSheet)
            {
                MainSheet = "Control Sheet";
                what = "Control Sheet";
            }
            else if (isdailysales)
            {
                MainSheet = "DailySales";
                what = "DailySales";
            }

            else if (isreturn)
            {
                MainSheet = "Return";
                what = "Return";
            }
            else if (isweeklyshow)
            {
                MainSheet = "Weeklyshow";
                what = "Weeklyshow";
            }
            else if (isFWBoundle)
            {
                MainSheet = "FWBundle Report";
                what = "FWBundle Report";
            }
           
           else
            {
                if (isFastWax)
                {
                    MainSheet = "Fast Wax";
                    what = "Fast Wax";
                }
                else
                {
                     MainSheet = "Summary";
                    
                }
            } 
            
            filename = "RS Week Ending " + exStartDate + " - " + what;

            using (pck)
            {
                if (ReportType == "payout")
                {
                    string selectedOwner = string.Empty;

                    if (userType == "owner")
                    {
                        selectedOwner = userFullname;
                    }
                    else
                    {
                        selectedOwner = webOwner.SelectedValue;
                    }

                    ExcelWorksheet wss = pck.Workbook.Worksheets.Add(MainSheet);
                    SqlDataSource SQLs = new SqlDataSource();
                    SQLs.ID = "SqlDataSource-SummarydashDivPre";
                    SQLs.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;
                    SQLs.SelectParameters.Add("webStartDate", shStartDate);
                    SQLs.SelectParameters.Add("webDuration", "14");
                    SQLs.SelectParameters.Add("webProgram", webProgram.SelectedValue);
                    SQLs.SelectParameters.Add("webStoreName", webStoreName.SelectedValue);
                    SQLs.SelectParameters.Add("webStoreNumber", "All");
                    SQLs.SelectParameters.Add("webOwner", selectedOwner);
                    SQLs.SelectParameters.Add("UserType", userType);
                    SQLs.SelectParameters.Add("userFullname", userFullname);
                    SQLs.SelectParameters.Add("webLocation", webLocation.SelectedValue);
                    if (webProgram.SelectedValue == "Chipio - Chip Repair")
                    {
                        SQLs.SelectParameters.Add("cols", "Chipio");
                    }
                    else
                    {
                        SQLs.SelectParameters.Add("cols", "All");
                    }
                    SQLs.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
                    SQLs.Selecting += SQLSelecting;
                    if (ControlSheet)
                    {
                        SQLs.SelectCommand = "spx_PAYOUTsummaryDaily";
                    }
                    else if (isweeklyshow) 
                    {
                        SQLs.SelectCommand = "spx_PAYOUTDaily";
                        
                    }
                    else if (isreturn)
                    {
                        SQLs.SelectCommand = "spx_PAYOUTReturnProgram";

                    }
                    else if (isFWBoundle)
                    {
                        SQLs.SelectCommand = "spx_FWBoundleReport";

                    }
                    else if (isdailysales)
                    {
                      
                        SQLs.SelectCommand = "spx_PAYOUTSalesDaily";

                    }
                                        
                    else
                    {
                        if (isFastWax)
                        {
                            SQLs.SelectCommand = "spx_PAYOUTfastWax";
                        }
                        else
                        {
                            SQLs.SelectCommand = "spx_PAYOUTsummary";
                        }
                    }
                    DataSourceSelectArguments argss = new DataSourceSelectArguments();
                    DataView DVs = (DataView)SQLs.Select(argss);
                    DataTable DTs = DVs.ToTable();
                    wss.Cells["A1"].LoadFromDataTable(DTs, true);
                    wss.Cells["A1:Z2000"].AutoFitColumns();
                    //wss.Column("A").Hidden = true;
                }
                //else
                //{
                //    ExcelWorksheet wss = pck.Workbook.Worksheets.Add("Summary");
                //    SqlDataSource SQL1s = new SqlDataSource();
                //    SQL1s.ID = "SqlDataSource1-Summary";
                //    SQL1s.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;
                //    SQL1s.SelectParameters.Add("webStartDate", shStartDate);
                //    SQL1s.SelectParameters.Add("webDuration", "14");
                //    SQL1s.SelectParameters.Add("webProgram", "All");
                //    SQL1s.SelectParameters.Add("webStoreName", "All");
                //    SQL1s.SelectParameters.Add("webOwner", Owner);
                //    SQL1s.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
                //    SQL1s.Selecting += SQLSelecting;
                //    SQL1s.SelectCommand = "spx_PAYOUTQtySummary";

                //    SqlDataSource SQL2s = new SqlDataSource();
                //    SQL2s.ID = "SqlDataSource2-Summary";
                //    SQL2s.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;
                //    SQL2s.SelectParameters.Add("webStartDate", shStartDate);
                //    SQL2s.SelectParameters.Add("webDuration", "14");
                //    SQL2s.SelectParameters.Add("webProgram", "All");
                //    SQL2s.SelectParameters.Add("webStoreName", "All");
                //    SQL2s.SelectParameters.Add("webOwner", Owner);
                //    SQL2s.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
                //    SQL2s.Selecting += SQLSelecting;
                //    SQL2s.SelectCommand = "spx_PAYOUTRevSummary";

                //    DataSourceSelectArguments args1s = new DataSourceSelectArguments();
                //    DataView DV1s = (DataView)SQL1s.Select(args1s);
                //    DataTable DT1s = DV1s.ToTable();
                //    DataSourceSelectArguments args2s = new DataSourceSelectArguments();
                //    DataView DV2s = (DataView)SQL2s.Select(args2s);
                //    DataTable DT2s = DV2s.ToTable();
                //    int j1s = DV1s.Table.Rows.Count + 6;
                //    int j2s = j1s + 1;
                //    wss.Cells["A1"].Value = "Program:";
                //    wss.Cells["A1"].Style.Font.Bold = true;
                //    wss.Cells["B1"].Value = Program;
                //    wss.Cells["A3"].Value = "Quantity Sold";
                //    wss.Cells["A3"].Style.Font.Bold = true;
                //    wss.Cells["A4"].LoadFromDataTable(DT1s, true);
                //    wss.Cells["A" + j1s.ToString()].Value = "Commissionable Revenue";
                //    wss.Cells["A" + j1s.ToString()].Style.Font.Bold = true;
                //    wss.Cells["A" + j2s.ToString()].LoadFromDataTable(DT2s, true);
                //    wss.Cells["A1:Z2000"].AutoFitColumns();

                //    //MIX
                //    ExcelWorksheet wsm = pck.Workbook.Worksheets.Add("Mix");
                //    SqlDataSource SQLm = new SqlDataSource();
                //    SQLm.ID = "SqlDataSource-Mix";
                //    SQLm.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;
                //    SQLm.SelectParameters.Add("webStartDate", shStartDate);
                //    SQLm.SelectParameters.Add("webDuration", "14");
                //    SQLm.SelectParameters.Add("webProgram", Program);
                //    SQLm.SelectParameters.Add("webStoreName", StoreName);
                //    SQLm.SelectParameters.Add("webOwner", Owner);
                //    SQLm.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
                //    SQLm.Selecting += SQLSelecting;
                //    SQLm.SelectCommand = "spx_PAYOUTMix";
                //    DataSourceSelectArguments argsm = new DataSourceSelectArguments();
                //    DataView DVm = (DataView)SQLm.Select(argsm);
                //    DataTable DTm = DVm.ToTable();
                //    wsm.Cells["A1"].Value = "Program:";
                //    wsm.Cells["A1"].Style.Font.Bold = true;
                //    wsm.Cells["B1"].Value = Program;
                //    wsm.Cells["A3"].LoadFromDataTable(DTm, true);
                //    wsm.Cells["A1:Z2000"].AutoFitColumns();
                //}

                if (what == "All")
                {
                    foreach (string[] i in storeList)
                    {
                        //storeList = tbStoreNumber, tbStoreName, tbLocation, tbOwner, tbHub, tbProgram
                        string SheetName = "#" + i[0] + " " + i[1] + " " + i[5];
                        string shStoreNumber = i[0];
                        string shStoreName = i[1];
                        string shLocation = i[2];
                        string shOwner = i[3];
                        string shHub = i[4];
                        string shProgram = i[5];

                        ExcelWorksheet ws;
                        try
                        {
                            ws = pck.Workbook.Worksheets.Add(SheetName);
                        }
                        catch
                        {
                            string n = new Random().Next(1, 9).ToString();
                            ws = pck.Workbook.Worksheets.Add(SheetName + " " + n);
                        }

                        SqlDataSource SQL1 = new SqlDataSource();
                        SQL1.ID = "SqlDataSource1";
                        SQL1.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;
                        SQL1.SelectParameters.Add("webStartDate", shStartDate);
                        SQL1.SelectParameters.Add("webDuration", "14");
                        SQL1.SelectParameters.Add("webProgram", shProgram);
                        SQL1.SelectParameters.Add("webStoreNumber", shStoreNumber);
                        SQL1.SelectParameters.Add("webStoreName", shStoreName);
                        SQL1.SelectParameters.Add("webLocation", shLocation);
                        SQL1.SelectParameters.Add("webOwner", shOwner);
                        SQL1.SelectParameters.Add("UserType", userType);
                        SQL1.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
                        SQL1.Selecting += SQLSelecting;
                        SQL1.SelectCommand = "spx_PAYOUTQty";

                        SqlDataSource SQL2 = new SqlDataSource();
                        SQL2.ID = "SqlDataSource2";
                        SQL2.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;
                        SQL2.SelectParameters.Add("webStartDate", shStartDate);
                        SQL2.SelectParameters.Add("webDuration", "14");
                        SQL2.SelectParameters.Add("webProgram", shProgram);
                        SQL2.SelectParameters.Add("webStoreNumber", shStoreNumber);
                        SQL2.SelectParameters.Add("webStoreName", shStoreName);
                        SQL2.SelectParameters.Add("webLocation", shLocation);
                        SQL2.SelectParameters.Add("webOwner", shOwner);
                        SQL2.SelectParameters.Add("UserType", userType);
                        if (shProgram == "Chipio - Chip Repair")
                        {
                            SQL2.SelectParameters.Add("gross", "Chipio - Chip Repair");
                        }
                        else
                        {
                            SQL2.SelectParameters.Add("gross", "All");
                        }
                        SQL2.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
                        SQL2.Selecting += SQLSelecting;
                        SQL2.SelectCommand = "spx_PAYOUTRev";

                        DataSourceSelectArguments args1 = new DataSourceSelectArguments();
                        DataView DV1 = (DataView)SQL1.Select(args1);
                        DataTable DT1 = DV1.ToTable();
                        DataSourceSelectArguments args2 = new DataSourceSelectArguments();
                        DataView DV2 = (DataView)SQL2.Select(args2);
                        DataTable DT2 = DV2.ToTable();

                        int j1 = DV1.Table.Rows.Count + 12;
                        int j2 = j1 + 1;

                        ws.Cells["A1"].Value = "Store:";
                        ws.Cells["A1"].Style.Font.Bold = true;
                        ws.Cells["B1"].Value = shStoreName;

                        ws.Cells["A2"].Value = "Program:";
                        ws.Cells["A2"].Style.Font.Bold = true;
                        ws.Cells["B2"].Value = shProgram;

                        ws.Cells["A3"].Value = "Owner:";
                        ws.Cells["A3"].Style.Font.Bold = true;
                        ws.Cells["B3"].Value = shOwner;

                        ws.Cells["A4"].Value = "Hub:";
                        ws.Cells["A4"].Style.Font.Bold = true;
                        ws.Cells["B4"].Value = shHub;

                        ws.Cells["A5"].Value = "Whse #:";
                        ws.Cells["A5"].Style.Font.Bold = true;
                        ws.Cells["B5"].Value = shStoreNumber;

                        ws.Cells["A6"].Value = "Location:";
                        ws.Cells["A6"].Style.Font.Bold = true;
                        ws.Cells["B6"].Value = shLocation;

                        ws.Cells["A9"].Value = "Quantity";
                        ws.Cells["A9"].Style.Font.Bold = true;

                        ws.Cells["A10"].LoadFromDataTable(DT1, true);

                        ws.Cells["A" + j1.ToString()].Value = "Revenue";
                        ws.Cells["A" + j1.ToString()].Style.Font.Bold = true;

                        ws.Cells["A" + j2.ToString()].LoadFromDataTable(DT2, true);

                        ws.Cells["A1:Z2000"].AutoFitColumns();

                        Page.Controls.Remove(SQL1);
                        Page.Controls.Remove(SQL2);
                    }
                }
                    
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.Flush();
                Response.End();

                //Response.Write("<script language='javascript'>$('#exportAll').text('Export All to Excel');</script>");
                //Response.AddHeader("script", "$('#exportAll').text('Export All to Excel');</script>");
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "Javascript", "<script language='javascript'>$('#exportAll').text('Export All to Excel');</script>");
            }
        }

        protected void krogerPeriods_Click(object sender, EventArgs e)
        {
            Response.Redirect("periods.aspx", true);
        }

        protected void exportAll_Click(object sender, EventArgs e)
        {
            exportFile("All");

            //Session["expStoreList"] = storeList;
            //Session["expStartDate"] = webStartDate.SelectedValue;
            //Session["expProgram"] = webProgram.SelectedValue;
            //Session["expStoreName"] = webStoreName.SelectedValue;
            //Session["expOwner"] = webOwner.SelectedValue;

            //ScriptManager.RegisterStartupScript(Page, typeof(Page), "exportAll", "window.open('ExportAll.aspx');", true);
        }

        protected void SQLSelecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            e.Command.CommandTimeout = 3600;
        }

        protected void GridDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView grv = (GridView)sender;
            int j = 0;
            int k = 0;
            string gridID = grv.ID;

            if (user != "andjelka@thesmartcircle.com" && user != "carlos@innovage.net" && user != "ben@innovage.net" && user != "Admin" && e.Row.Cells.Count > 1)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if ((((DataControlFieldCell)((e.Row.Cells[i]))).ContainingField).HeaderText == "DCB")
                    {
                        e.Row.Cells[i].Visible = false;
                    }
                }
            }

            //if ((e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header) &&
            //    ReportType == "payout" && gridID == "GVs1")
            //{
            //    if (webProgram.SelectedValue == "Chipio - Chip Repair")
            //    {
            //        e.Row.Cells[9].Visible = true;
            //        e.Row.Cells[10].Visible = true;
            //        e.Row.Cells[11].Visible = true;
            //        e.Row.Cells[12].Visible = true;

            //        e.Row.Cells[13].Visible = false;
            //        e.Row.Cells[14].Visible = false;
            //        e.Row.Cells[16].Visible = false;
            //        e.Row.Cells[17].Visible = false;
            //        e.Row.Cells[18].Visible = false;
            //        e.Row.Cells[19].Visible = false;
            //        e.Row.Cells[20].Visible = false;
            //        e.Row.Cells[21].Visible = false;
            //        e.Row.Cells[22].Visible = false;
            //    }
            //    else
            //    {
            //        e.Row.Cells[9].Visible = false;
            //        e.Row.Cells[10].Visible = false;
            //        e.Row.Cells[11].Visible = false;
            //        e.Row.Cells[12].Visible = false;

            //        e.Row.Cells[13].Visible = true;
            //        e.Row.Cells[14].Visible = true;
            //        e.Row.Cells[16].Visible = true;
            //        e.Row.Cells[17].Visible = true;
            //        e.Row.Cells[18].Visible = true;
            //        e.Row.Cells[19].Visible = true;
            //        e.Row.Cells[20].Visible = true;
            //        e.Row.Cells[21].Visible = true;
            //        e.Row.Cells[22].Visible = true;
            //    }
            //}

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (ReportType == "payout")
                {

                   if (gridID == "GVs1")
                     {
                        j = 8;
                        k = e.Row.Cells.Count - 3;

                        e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                        e.Row.Cells[k + 1].HorizontalAlign = HorizontalAlign.Right;
                        e.Row.Cells[k + 2].HorizontalAlign = HorizontalAlign.Right;                       
                        if (e.Row.Cells[3].Text != "Total")
                        {
                            e.Row.Attributes["ondblclick"] = ClientScript.GetPostBackClientHyperlink(grv, "Select$" + e.Row.RowIndex);
                        }
                       
                     }
                   

                    for (int i = j; i < k; i++)
                    {
                        e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                    }
                }                
            }
        }

        protected void GridIndexChanged(object sender, EventArgs e)
        {
            GridView grv = (GridView)sender;

            if (webStoreNumber.Text == "")
            {
                Session["StoreNumber"] = "All";
            }
            else
            {
                Session["StoreNumber"] = webStoreNumber.Text;
            }
            Session["StoreName"] = webStoreName.SelectedValue;
            Session["Program"] = webProgram.SelectedValue;
            Session["StartDate"] = webStartDate.SelectedValue;
            Session["Owner"] = webOwner.SelectedValue;
            Session["Location"] = webLocation.SelectedValue;
            Session["g2StartDate"] = webStartDate.SelectedValue;
            Session["g2Duration"] = "14";
            Session["g2Owner"] = grv.SelectedRow.Cells[3].Text;
            Session["g2Hub"] = grv.SelectedRow.Cells[grv.SelectedRow.Cells.Count - 3].Text;
            Session["g2StoreName"] = grv.SelectedRow.Cells[4].Text.Replace("amp;", "");
            Session["g2Program"] = grv.SelectedRow.Cells[5].Text;
            Session["g2StoreNumber"] = grv.SelectedRow.Cells[6].Text;
            Session["g2Location"] = grv.SelectedRow.Cells[7].Text;
            Session["g2PSID"] = grv.SelectedRow.Cells[1].Text;
            Response.Redirect("Grids2.aspx", true);
        }

        protected void exportSummary_Click(object sender, EventArgs e)
        {
            exportFile("Summary");

            //SQLs1.SelectCommand = "spx_PAYOUTsummary";
            //GVs1.DataBind();
            //string attachment = "attachment; filename=\"Payout Summary " + Convert.ToDateTime(webStartDate.SelectedValue).AddDays(13).ToShortDateString().Replace("/", "-") + ".xls\"";
            //Response.ClearContent();
            //Response.AddHeader("content-disposition", attachment);
            //Response.ContentType = "application/ms-excel";
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(sw);
            ////sw.Write("LT Report <b>" + WE.Replace("/", "-") + "</b>");
            ////sw.Write("<br><br>");
            //GVs1.RenderControl(htw);
            //Response.Write(sw.ToString());
            //Response.End();
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

        // This Method is used to render gridview control
        public string GetGridviewData(GridView gv)
        {
            StringBuilder strBuilder = new StringBuilder();
            StringWriter strWriter = new StringWriter(strBuilder);
            HtmlTextWriter htw = new HtmlTextWriter(strWriter);
            gv.RenderControl(htw);
            return strBuilder.ToString();
        }

        protected void DropChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;

            SQLs1.SelectParameters.Remove(SQLs1.SelectParameters[ddl.ID]);
            SQLs1.SelectParameters.Add(ddl.ID, ddl.SelectedValue);

            if (isweeklyshow)
            {
                SQLs1.SelectCommand = "spx_PAYOUTDaily";
                
            }
            else  if (ControlSheet)
            {
                SQLs1.SelectCommand = "spx_PAYOUTsummaryDaily";
            }
            else if (isreturn)
            {
                SQLs1.SelectCommand = "spx_PAYOUTReturnProgram";
            }
            else if (isdailysales)
            {
               
                SQLs1.SelectCommand = "spx_PAYOUTSalesDaily";
            }
            else if (isFWBoundle)
            {
                SQLs1.SelectCommand = "spx_FWBoundleReport";
            }
            else
            {
                if (isFastWax)
                {
                    SQLs1.SelectCommand = "spx_PAYOUTfastWax";
                }
                else
                {
                    SQLs1.SelectCommand = "spx_PAYOUTsummary";

                    SQLs1.SelectParameters.Remove(SQLs1.SelectParameters["cols"]);
                    if (webProgram.SelectedValue == "Chipio - Chip Repair")
                    {
                        SQLs1.SelectParameters.Add("cols", "Chipio");
                    }
                    else
                    {
                        SQLs1.SelectParameters.Add("cols", "All");
                    }
                }
            }

           
            GVs1.DataBind();
            getList();
            getDDL();
        }

        protected void TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;

            SQLs1.SelectParameters.Remove(SQLs1.SelectParameters[txt.ID]);
            if (txt.Text == "")
            {
                SQLs1.SelectParameters.Add(txt.ID, "All");
            }
            else
            {
                SQLs1.SelectParameters.Add(txt.ID, txt.Text);
            }

            if (isweeklyshow)
            {
                SQLs1.SelectCommand = "spx_PAYOUTDaily";

            }
            else  if (ControlSheet)
            {
                SQLs1.SelectCommand = "spx_PAYOUTsummaryDaily";
            }

            else if (isreturn)
            {
                SQLs1.SelectCommand = "spx_PAYOUTReturnProgram";
            }
            else if (isdailysales)
            {
              
                SQLs1.SelectCommand = "spx_PAYOUTSalesDaily";
            }
            else if (isFWBoundle)
            {
                SQLs1.SelectCommand = "spx_FWBoundleReport";
            }
            else
            {
                if (isFastWax)
                {
                    SQLs1.SelectCommand = "spx_PAYOUTfastWax";
                }
                else
                {
                    SQLs1.SelectCommand = "spx_PAYOUTsummary";
                }
            }

            GVs1.DataBind();
        }

        protected void ctrlSheet_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            if (btn.Text == "Control Sheet")
            {
                SQLs1.SelectCommand = "spx_PAYOUTsummaryDaily";
                btn.Text = "Back";
                exportSummary.Text = "Export Control Sheet to Excel";
                exportAll.Visible = false;
                fastWax.Visible = false;
                fastWaxTotals.Visible = false;
                changesBtn.Visible = false;
                exportAvg.Visible = false;
                WeeklyShow.Visible = false;
                DailySales.Visible = false;
                Session["ControlSheet"] = true;
                
            }
            else
            {
                SQLs1.SelectCommand = "spx_PAYOUTsummary";
                btn.Text = "Control Sheet";
                exportSummary.Text = "Export Summary to Excel";
               
                fastWax.Text = "FW Report";
                exportAll.Visible = true;
              
                if (user == "Admin" || user == "andjelka@thesmartcircle.com")
                {
                    changesBtn.Visible = true;
                    exportAvg.Visible = true;
                    DailySales.Visible = true;
                   
                    fastWax.Visible = true;
                    fastWaxTotals.Visible = true;
                    FWBoundle.Visible = true;
                    WeeklyShow.Visible = true;
              
                }
                try { Session.Remove("ControlSheet"); } catch { }
            }

            GVs1.DataBind();
        }

        protected void fastWax_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            if (btn.Text == "FW Report")
            {
                SQLs1.SelectCommand = "spx_PAYOUTfastWax";
                btn.Text = "Back";
                exportSummary.Text = "Export FW to Excel";
                exportAll.Visible = false;
                ctrlSheet.Visible = false;
                fwSpan.Visible = false;
                changesBtn.Visible = false;
                DailySales.Visible = false;
                exportAvg.Visible = false;
                FWBoundle.Visible = false;
                WeeklyShow.Visible = false;
                Session["fastWax"] = true;
            }
            else
            {
                SQLs1.SelectCommand = "spx_PAYOUTsummary";
                btn.Text = "FW Report";
                exportSummary.Text = "Export Summary to Excel";
                exportAll.Visible = true;
               
               
                WeeklyShow.Visible = true;
                if (user == "Admin" || user == "andjelka@thesmartcircle.com")
                {
                    changesBtn.Visible = true;
                    exportAvg.Visible = true;
                    DailySales.Visible = true;
                    ctrlSheet.Visible = true;
                    fwSpan.Visible = true;
                }
                ctrlSheet.Text = "Control Sheet";
                try { Session.Remove("fastWax"); } catch { }
            }

            GVs1.DataBind();
        }

        
        protected void Return_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            if (btn.Text == "Returns")
            {
                SQLs1.SelectCommand = "spx_PAYOUTReturnProgram";
                btn.Text = "Back";
                exportSummary.Text = "Export Control Sheet to Excel";
                lblLoc.Visible = false;
                lblOwner.Visible = false;
                lblprgm.Visible = false;
                lblStore.Visible = false;
                lblStorNum.Visible = false;
                lblWeekending.Visible = false;
                webStartDate.Visible = false;
                webOwner.Visible = false;
                webStoreName.Visible = false;
                webProgram.Visible = false;
                webLocation.Visible = false;
                webStoreNumber.Visible = false;
            
                exportAll.Visible = false;
                fastWax.Visible = false;
                fastWaxTotals.Visible = false;
                changesBtn.Visible = false;
                exportAvg.Visible = false;
                WeeklyShow.Visible = false;
                DailySales.Visible = false;
                ctrlSheet.Visible = false;
                krogerPeriods.Visible = false;
                FWBoundle.Visible = false;
                Session["return"] = true;

            }
            else
            {
                SQLs1.SelectCommand = "spx_PAYOUTsummary";
                btn.Text = "Returns";
                exportSummary.Text = "Export Summary to Excel";
                exportAll.Visible = true;
                
                fastWax.Text = "FW Report";
               
                
                WeeklyShow.Visible = true;
                Returns.Visible = true;
                lblLoc.Visible = true;
                lblOwner.Visible = true;
                lblprgm.Visible = true;
                lblStore.Visible = true;
                lblStorNum.Visible = true;
                lblWeekending.Visible = true;
                webStartDate.Visible = true;
                webOwner.Visible = true;
                webStoreName.Visible = true;
                webProgram.Visible = true;
                webLocation.Visible = true;
                webStoreNumber.Visible = true;

                if (user == "Admin" || user == "andjelka@thesmartcircle.com")
                {
                    changesBtn.Visible = true;
                    exportAvg.Visible = true;
                    DailySales.Visible = true;
                    fastWaxTotals.Visible = true;
                    FWBoundle.Visible = true;
                    fastWax.Visible = true;
                }
                try { Session.Remove("return"); }
                catch { }
            }

            GVs1.DataBind();
        }

     
        protected void weeklyshow_Click(object sender, EventArgs e)
        { 
            LinkButton btn = (LinkButton)sender;

            if (btn.Text == "Weekly Show Total")
            {

                SQLs1.SelectCommand = "spx_PAYOUTDaily";
                btn.Text = "Back";
                exportSummary.Text = "Export weekly Show to Excel";
                tipSpan.Visible = false;
                krogerPeriods.Visible = false;
                DailySales.Visible = false;
                ctrlSheet.Visible = false;
                exportAll.Visible = false;
                fastWax.Visible = false;
                fastWaxTotals.Visible = false;
                changesBtn.Visible = false;
                FWBoundle.Visible = false;
                exportAvg.Visible = false;
                Session["WeeklyShow"] = true;
              
            }
            else
            {
                SQLs1.SelectCommand = "spx_PAYOUTsummary";
                btn.Text = "Weekly Show Total";
                exportSummary.Text = "Export Summary to Excel";
                exportAll.Visible = true;
               
                
                FWBoundle.Visible = true;
                krogerPeriods.Text = "Kroger Periods";
                fastWax.Text = "FW Report";
                ctrlSheet.Text = "Control Sheet";
                if (user == "Admin" || user == "andjelka@thesmartcircle.com")
                {
                    changesBtn.Visible = true;
                    exportAvg.Visible = true;
                    DailySales.Visible = true;
                    fastWax.Visible = true;
                    ctrlSheet.Visible = true;
                    WeeklyShow.Visible = true;
                    krogerPeriods.Visible = true;
                    fastWaxTotals.Visible = true;
                }
                try { Session.Remove("WeeklyShow"); }
                catch { }
            }
    
            GVs1.DataBind();
        }


        protected void FWBoundle_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            if (btn.Text == "FW Bundle Report")
            {

                SQLs1.SelectCommand = "spx_FWBoundleReport";
                btn.Text = "Back";
                exportSummary.Text = "FW Bundle Report to Excel";
                webProgram.Visible = false;
                DailySales.Visible = false;
                lblprgm.Visible = false;
                tipSpan.Visible = false;
                krogerPeriods.Visible = false;
                ctrlSheet.Visible = false;
                exportAll.Visible = false;
                fastWax.Visible = false;
                fastWaxTotals.Visible = false;
                WeeklyShow.Visible = false;
                changesBtn.Visible = false;
                exportAvg.Visible = false;
                Session["FWBoundle"] = true;

               

            }
            else
            {
                SQLs1.SelectCommand = "spx_PAYOUTsummary";
                btn.Text = "FW Bundle Report";
                exportSummary.Text = "Export Summary to Excel";
                webProgram.Visible = true;
                lblprgm.Visible = true;
                exportAll.Visible = true;
               
                krogerPeriods.Text = "Kroger Periods";
                fastWax.Text = "FW Report";
                ctrlSheet.Text = "FW Bundle Report";
                if (user == "Admin" || user == "andjelka@thesmartcircle.com")
                {
                    changesBtn.Visible = true;
                    exportAvg.Visible = true;
                    DailySales.Visible = true;
                    fastWax.Visible = true;
                    FWBoundle.Visible = true;
                    ctrlSheet.Visible = true;
                    WeeklyShow.Visible = true;
                    krogerPeriods.Visible = true;
                    fastWaxTotals.Visible = true;
                    
                }
                try { Session.Remove("FWBoundle"); }
                catch { }
            }

            GVs1.DataBind();
        }


        protected void DailySales_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            if (btn.Text == "Daily Sales")
            {
                
                SQLs1.SelectCommand = "spx_PAYOUTSalesDaily";
                btn.Text = "Back";
                exportSummary.Text = "Export Daily Sales to Excel";
                exportAll.Visible = false;
                fastWax.Visible = false;
                fastWaxTotals.Visible = false;
                changesBtn.Visible = false;
                exportAvg.Visible = false;
                WeeklyShow.Visible = false;
                FWBoundle.Visible = false;
                krogerPeriods.Visible = false;
                ctrlSheet.Visible = false;
              

                Session["DailySales"] = true;

            }
            else
            {
                SQLs1.SelectCommand = "spx_PAYOUTsummary";
                btn.Text = "Daily Sales";
                exportSummary.Text = "Export Summary to Excel";
                exportAll.Visible = true;
                fastWax.Visible = true;
                DailySales.Visible = true;
                fastWax.Text = "FW Report";
                fastWaxTotals.Visible = true;
                FWBoundle.Visible = true;
                WeeklyShow.Visible = true;
               
                if (user == "Admin" || user == "andjelka@thesmartcircle.com")
                {
                    changesBtn.Visible = true;
                    exportAvg.Visible = true;
                }
                try { Session.Remove("DailySales"); }
                catch { }
            }

            GVs1.DataBind();
        }


        protected void fastWaxTotals_Click(object sender, EventArgs e)
        {
            Response.Redirect("FWtotals.aspx", true);
        }

        protected void changesBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("Changes.aspx", true);
        }

        protected void exportAvg_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in GVs1.Rows)
            {
                decimal avg = Convert.ToDecimal(((row.Cells[22].Text != "&nbsp;") ? row.Cells[22].Text : "0"));
                decimal avgP = Convert.ToDecimal(((row.Cells[23].Text != "&nbsp;") ? row.Cells[23].Text : "0"));

                if (avg < avgP || avgP == 0)
                {
                    row.Visible = false;
                }
            }

            //SQLs1.SelectCommand = "spx_PAYOUTsummary";
            //GVs1.DataBind();
            string attachment = "attachment; filename=\"Payout Averages " + Convert.ToDateTime(webStartDate.SelectedValue).AddDays(13).ToShortDateString().Replace("/", "-") + ".xls\"";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            GVs1.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();

            foreach (GridViewRow row in GVs1.Rows)
            {
                row.Visible = true;
            }
        }
    }
}