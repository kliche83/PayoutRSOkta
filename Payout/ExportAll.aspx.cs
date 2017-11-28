using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml;

namespace Payout
{
    public partial class ExportAll : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string user = string.Empty;
            string userFullname = string.Empty;
            string userType = string.Empty;
            string ReportType = string.Empty;
            string StartDate = string.Empty;
            string StoreName = string.Empty;
            string Program = string.Empty;
            string Owner = string.Empty;
            string filename = string.Empty;
            string what = string.Empty;

            ExcelPackage pck = new ExcelPackage();
            ArrayList storeList = new ArrayList();

            try
            {
                user = Session["user"].ToString();
                userFullname = Session["userFullname"].ToString();
                userType = Session["userType"].ToString();
                ReportType = "payout";

                storeList = (ArrayList)Session["expStoreList"];
                StartDate = Session["expStartDate"].ToString();
                Program = Session["expProgram"].ToString();
                StoreName = Session["expStoreName"].ToString();
                Owner = Session["expOwner"].ToString();
            }
            catch
            {
                //Response.Write("<script>window.top.location = '../';</script>");
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "expired", "alert('<script>document.body.innerHTML = 'There was an error.';</script>');", true);
            }

            what = "All";
            filename = "RS Week Ending " + StartDate.Replace("/", "-") + " - " + what;

            using (pck)
            {
                if (ReportType == "payout")
                {
                    ExcelWorksheet wss = pck.Workbook.Worksheets.Add("Summary");
                    SqlDataSource SQLs = new SqlDataSource();
                    SQLs.ID = "SqlDataSource-Summary";
                    SQLs.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;
                    SQLs.SelectParameters.Add("webStartDate", StartDate);
                    SQLs.SelectParameters.Add("webDuration", "14");
                    SQLs.SelectParameters.Add("webProgram", Program);
                    SQLs.SelectParameters.Add("webStoreName", StoreName);
                    SQLs.SelectParameters.Add("webStoreNumber", "All");
                    SQLs.SelectParameters.Add("webOwner", Owner);
                    SQLs.SelectParameters.Add("UserType", userType);
                    SQLs.SelectParameters.Add("userFullname", userFullname);
                    SQLs.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
                    SQLs.Selecting += SQLSelecting;
                    SQLs.SelectCommand = "spx_PAYOUTsummary";
                    DataSourceSelectArguments argss = new DataSourceSelectArguments();
                    DataView DVs = (DataView)SQLs.Select(argss);
                    DataTable DTs = DVs.ToTable();
                    wss.Cells["A1"].LoadFromDataTable(DTs, true);
                }
                //else
                //{
                //    ExcelWorksheet wss = pck.Workbook.Worksheets.Add("Summary");
                //    SqlDataSource SQL1s = new SqlDataSource();
                //    SQL1s.ID = "SqlDataSource1-Summary";
                //    SQL1s.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;
                //    SQL1s.SelectParameters.Add("webStartDate", StartDate);
                //    SQL1s.SelectParameters.Add("webDuration", "14");
                //    SQL1s.SelectParameters.Add("Program", "All");
                //    SQL1s.SelectParameters.Add("StoreName", "All");
                //    SQL1s.SelectParameters.Add("webOwner", Owner);
                //    SQL1s.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
                //    SQL1s.Selecting += SQLSelecting;
                //    SQL1s.SelectCommand = "spx_PAYOUTQtySummary";

                //    SqlDataSource SQL2s = new SqlDataSource();
                //    SQL2s.ID = "SqlDataSource2-Summary";
                //    SQL2s.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;
                //    SQL2s.SelectParameters.Add("webStartDate", StartDate);
                //    SQL2s.SelectParameters.Add("webDuration", "14");
                //    SQL2s.SelectParameters.Add("Program", "All");
                //    SQL2s.SelectParameters.Add("StoreName", "All");
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

                //    //MIX
                //    ExcelWorksheet wsm = pck.Workbook.Worksheets.Add("Mix");
                //    SqlDataSource SQLm = new SqlDataSource();
                //    SQLm.ID = "SqlDataSource-Mix";
                //    SQLm.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;
                //    SQLm.SelectParameters.Add("webStartDate", StartDate);
                //    SQLm.SelectParameters.Add("webDuration", "14");
                //    SQLm.SelectParameters.Add("Program", Program);
                //    SQLm.SelectParameters.Add("StoreName", StoreName);
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
                        SQL1.SelectParameters.Add("webStartDate", StartDate);
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
                        SQL2.SelectParameters.Add("webStartDate", StartDate);
                        SQL2.SelectParameters.Add("webDuration", "14");
                        SQL2.SelectParameters.Add("webProgram", shProgram);
                        SQL2.SelectParameters.Add("webStoreNumber", shStoreNumber);
                        SQL2.SelectParameters.Add("webStoreName", shStoreName);
                        SQL2.SelectParameters.Add("webLocation", shLocation);
                        SQL2.SelectParameters.Add("webOwner", shOwner);
                        SQL2.SelectParameters.Add("UserType", userType);
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

                        Page.Controls.Remove(SQL1);
                        Page.Controls.Remove(SQL2);
                    }
                }

                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.Flush();
                Response.End();

                //ScriptManager.RegisterStartupScript(Page, typeof(Page), "doneAlert", "alert('Your export is ready.');", true);
            }
        }

        protected void SQLSelecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            e.Command.CommandTimeout = 3600;
        }
    }
}