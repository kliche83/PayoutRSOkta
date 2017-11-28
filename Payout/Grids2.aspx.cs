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
    public partial class Grids2 : System.Web.UI.Page
    {
        string user = string.Empty;
        string userFullname = string.Empty;
        string userType = string.Empty;
        string StoreName = string.Empty;
        string StoreNumber = string.Empty;
        string Program = string.Empty;
        string StartDate = string.Empty;
        string Duration = string.Empty;
        string ReportType = string.Empty;
        string Owner = string.Empty;
        string Hub = string.Empty;
        string Location = string.Empty;
        string infoString = string.Empty;
        string Psid = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                user = Session["user"].ToString();
                userFullname = Session["userFullname"].ToString();
                userType = Session["userType"].ToString();

                StoreName = Session["g2StoreName"].ToString();
                StoreNumber = Session["g2StoreNumber"].ToString();
                Program = Session["g2Program"].ToString();
                StartDate = Session["g2StartDate"].ToString();
                Duration = Session["g2Duration"].ToString();
                ReportType = Session["ReportType"].ToString();
                Owner = Session["g2Owner"].ToString();
                Hub = Session["g2Hub"].ToString();
                Location = Session["g2Location"].ToString();
                Psid = Session["g2PSID"].ToString();
                infoString += "Store: " + StoreName;
                infoString += " / Progrm: " + Program;
                infoString += " / Location: #" + StoreNumber + " " + Location;
                infoString += " / Owner: " + Owner;
                infoString += " /  PS ID #: " + Psid;
               
                infoString += " / Hub: " + Hub;

                info.Text = infoString;
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }

            if (!IsPostBack)
            {
                SQLs1.SelectParameters.Add("webStartDate", StartDate);
                SQLs1.SelectParameters.Add("webDuration", Duration);
                SQLs1.SelectParameters.Add("webProgram", Program);
                SQLs1.SelectParameters.Add("webStoreNumber", StoreNumber);
                SQLs1.SelectParameters.Add("webStoreName", StoreName);
                SQLs1.SelectParameters.Add("webLocation", Location);
                SQLs1.SelectParameters.Add("webOwner", Owner);
                SQLs1.SelectParameters.Add("UserType", userType);

                SQLs2.SelectParameters.Add("webStartDate", StartDate);
                SQLs2.SelectParameters.Add("webDuration", Duration);
                SQLs2.SelectParameters.Add("webProgram", Program);
                SQLs2.SelectParameters.Add("webStoreNumber", StoreNumber);
                SQLs2.SelectParameters.Add("webStoreName", StoreName);
                SQLs2.SelectParameters.Add("webLocation", Location);
                SQLs2.SelectParameters.Add("webOwner", Owner);
                SQLs2.SelectParameters.Add("UserType", userType);
                if (Program == "Chipio - Chip Repair")
                {
                    SQLs2.SelectParameters.Add("gross", "Chipio - Chip Repair");
                }
                else
                {
                    SQLs2.SelectParameters.Add("gross", "All");
                }
            }

            SQLs1.SelectCommand = "spx_PAYOUTQty";
            SQLs2.SelectCommand = "spx_PAYOUTRev";

            GetDatatables(SQLs1, SQLs2);

            GVs1.DataBind();
            GVs2.DataBind();
        }

        //test Method
        private void GetDatatables(SqlDataSource SQLs1, SqlDataSource SQLs2)
        {
            //Test datatables
            DataSourceSelectArguments args = new DataSourceSelectArguments();
            DataView view = (DataView)SQLs1.Select(args);
            DataTable dt1 = view.ToTable();

            DataSourceSelectArguments args1 = new DataSourceSelectArguments();
            DataView view1 = (DataView)SQLs2.Select(args1);
            DataTable dt2 = view1.ToTable();
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

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                j = 2;
                k = e.Row.Cells.Count;

                for (int i = j; i < k; i++)
                {
                    e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                }
            }

            //if (ReportType == "sales" && e.Row.Cells[1].Text == "Total Payout")
            //{
            //    e.Row.Visible = false;
            //}
        }

        void exportFile()
        {
            ExcelPackage pck = new ExcelPackage();
            string filename = string.Empty;

            filename = StoreName + " #" + StoreNumber + " - Week Ending " + Convert.ToDateTime(StartDate).AddDays(13).ToShortDateString().Replace("/", "-");

            using (pck)
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(filename);

                SqlDataSource SQL1 = new SqlDataSource();
                SQL1.ID = "SqlDataSource1";
                SQL1.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;
                SQL1.SelectParameters.Add("webStartDate", StartDate);
                SQL1.SelectParameters.Add("webDuration", "14");
                SQL1.SelectParameters.Add("webProgram", Program);
                SQL1.SelectParameters.Add("webStoreNumber", StoreNumber);
                SQL1.SelectParameters.Add("webStoreName", StoreName);
                SQL1.SelectParameters.Add("webLocation", Location);
                SQL1.SelectParameters.Add("webOwner", Owner);
                SQL1.SelectParameters.Add("UserType", userType);
                SQL1.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
                SQL1.Selecting += SQLSelecting;
                SQL1.SelectCommand = "spx_PAYOUTQty";

                SqlDataSource SQL2 = new SqlDataSource();
                SQL2.ID = "SqlDataSource2";
                SQL2.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;
                SQL2.SelectParameters.Add("webStartDate", StartDate);
                SQL2.SelectParameters.Add("webDuration", "14");
                SQL2.SelectParameters.Add("webProgram", Program);
                SQL2.SelectParameters.Add("webStoreNumber", StoreNumber);
                SQL2.SelectParameters.Add("webStoreName", StoreName);
                SQL2.SelectParameters.Add("webLocation", Location);
                SQL2.SelectParameters.Add("webOwner", Owner);
                SQL2.SelectParameters.Add("UserType", userType);
                if (Program == "Chipio - Chip Repair")
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
                ws.Cells["B1"].Value = StoreName;

                ws.Cells["A2"].Value = "Program:";
                ws.Cells["A2"].Style.Font.Bold = true;
                ws.Cells["B2"].Value = Program;

                ws.Cells["A3"].Value = "Owner:";
                ws.Cells["A3"].Style.Font.Bold = true;
                ws.Cells["B3"].Value = Owner;

                ws.Cells["A4"].Value = "Hub:";
                ws.Cells["A4"].Style.Font.Bold = true;
                ws.Cells["B4"].Value = Hub;

                ws.Cells["A5"].Value = "Whse #:";
                ws.Cells["A5"].Style.Font.Bold = true;
                ws.Cells["B5"].Value = StoreNumber;

                ws.Cells["A6"].Value = "Location:";
                ws.Cells["A6"].Style.Font.Bold = true;
                ws.Cells["B6"].Value = Location;

                ws.Cells["A9"].Value = "Quantity";
                ws.Cells["A9"].Style.Font.Bold = true;

                ws.Cells["A10"].LoadFromDataTable(DT1, true);

                ws.Cells["A" + j1.ToString()].Value = "Revenue";
                ws.Cells["A" + j1.ToString()].Style.Font.Bold = true;

                ws.Cells["A" + j2.ToString()].LoadFromDataTable(DT2, true);
                
                ws.Cells["A1:Z2000"].AutoFitColumns();

                Page.Controls.Remove(SQL1);
                Page.Controls.Remove(SQL2);

                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.Flush();
                Response.End();
            }
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            exportFile();

            //GVs1.DataBind();
            //string attachment = "attachment; filename=\"" + StoreName + " " + StoreNumber + " Daily Report " + Convert.ToDateTime(StartDate).AddDays(13).ToShortDateString().Replace("/", "-") + ".xls\"";
            //Response.ClearContent();
            //Response.AddHeader("content-disposition", attachment);
            //Response.ContentType = "application/ms-excel";
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(sw);
            //htw.Write("<b>Store:</b> " + StoreName);
            //htw.Write("<br>");
            //htw.Write("<b>Location:</b> #" + StoreNumber + " " + Location);
            //htw.Write("<br>");
            //htw.Write("<b>Program:</b> " + Program);
            //htw.Write("<br>");
            //htw.Write("<b>Owner:</b> " + Owner);
            //htw.Write("<br>");
            //htw.Write("<b>Hub:</b> " + Hub);
            //htw.Write("<br><br>");
            //htw.Write("<b>Quantity Sold</b><br>");
            //GVs1.RenderControl(htw);
            //htw.Write("<br>");
            //htw.Write("<b>Commissionable Revenue</b><br>");
            //GVs2.RenderControl(htw);

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
    }
}