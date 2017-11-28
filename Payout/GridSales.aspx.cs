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
    public partial class GridSales : System.Web.UI.Page
    {
        string FirstDayOfYear = "2017-01-01";
        string ParamStartDate;
        string ParamEndDate;
        string userType = string.Empty;
        string user = string.Empty;
        string userFullname = string.Empty;
        string Owner = string.Empty;
        string StoreName = string.Empty;
        string StoreNumber = string.Empty;
        string Program = string.Empty;
        string QueryString = string.Empty;
        Dictionary<string, string> SQLParameters;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                user = Session["user"].ToString();

                if (!IsPostBack)
                {
                    Session["dateFrom"] = string.Empty;
                    Session["dateTo"] = string.Empty;
                    Session["ResultTable"] = new DataTable();
                    Session["ExportTable"] = new DataTable();

                    BindGridData();
                    Session["ExportTable"] = (DataTable)Session["ResultTable"];
                }
            }            
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }
        }


        private void FillFromDatabase()
        {
            DataTable dt = new DataTable();
            
            DateFilters(ref ParamStartDate, ref ParamEndDate);
            dateFrom.Text = DatePickerFormat(ParamStartDate);
            dateTo.Text = DatePickerFormat(ParamEndDate);

            SQLParameters.Add("@webStartDate", ParamStartDate);
            SQLParameters.Add("@webEndDate", ParamEndDate);
            SQLParameters.Add("@UserType", userType);

            dt = Queries.GetResultsFromStoreProcedure("spx_PAYOUTsales", ref SQLParameters).Tables[0];

            BindDDL(dt, "Program");
            BindDDL(dt, "StoreName");
            StoreNumberTXT.Text = string.Empty;
            ownerTXT.Text = string.Empty;
            

            Session["ResultTable"] = dt;            
            
            Session["dateFrom"] = ParamStartDate;
            Session["dateTo"] = ParamEndDate;
        }

        private void BindGridData()
        {
            DataTable dt = new DataTable();            
            SQLParameters = new Dictionary<string, string>();
            
            ParamStartDate = dateFrom.Text;
            ParamEndDate = dateTo.Text;


            if (Session["dateFrom"].ToString() != ParamStartDate || Session["dateTo"].ToString() != ParamEndDate)
            {
                FillFromDatabase();
            }
            else
            {
                if (ParamStartDate == string.Empty && ParamEndDate == string.Empty)
                {
                    FillFromDatabase();
                }
                else
                {                    
                    Session["ExportTable"] = dt;
                    StoreNumberTXT.Text = Session["StoreNumber"].ToString();
                    ownerTXT.Text = Session["Owner"].ToString();
                }
            }

            dt = (DataTable)Session["ResultTable"];

            if (programDDL.SelectedValue.ToLower() != "all")
                filterDatatable(ref dt, "Program", programDDL.SelectedValue);

            if (sstoreDDL.SelectedValue.ToLower() != "all")
                filterDatatable(ref dt, "StoreName", sstoreDDL.SelectedValue);

            if (!string.IsNullOrWhiteSpace(StoreNumberTXT.Text))
                filterDatatable(ref dt, "StoreNumber", StoreNumberTXT.Text);

            if (!string.IsNullOrWhiteSpace(ownerTXT.Text))
                filterDatatable(ref dt, "Owner", ownerTXT.Text);

            GridView1.DataSource = dt;
            GridView1.DataBind();
        }


        private void filterDatatable(ref DataTable dt, string FieldName, string Value)
        {
            try
            {
                dt = dt.AsEnumerable()
                        .Where(row => row.Field<String>(FieldName).ToLower().Contains(Value.ToLower().Trim())).CopyToDataTable();
            }
            catch{}            
        }


        private void BindDDL(DataTable dt, string Field)
        {
            List<string> lstFilteredDT = new List<string>();

            lstFilteredDT = dt.AsEnumerable()
                .Select(t => t.Field<string>(Field)).ToList();
            lstFilteredDT = lstFilteredDT.Distinct().ToList();
            lstFilteredDT.Sort();
            lstFilteredDT.Insert(0, "All");
            

            switch (Field)
            {
                case "Program":
                    programDDL.DataSource = lstFilteredDT;
                    programDDL.DataBind();
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(Session[Field])))
                    {                        
                        if (lstFilteredDT.Where(s => s.IndexOf(Session[Field].ToString()) == 0).Count() > 0)
                        {
                            try
                            {
                                programDDL.SelectedValue = Session[Field].ToString();
                            }
                            catch{}
                        }
                    }                    
                    break;

                case "StoreName":
                    sstoreDDL.DataSource = lstFilteredDT;
                    sstoreDDL.DataBind();
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(Session[Field])))
                    {                        
                        if (lstFilteredDT.Where(s => s.IndexOf(Session[Field].ToString()) == 0).Count() > 0)
                        {
                            try
                            {
                                sstoreDDL.SelectedValue = Session[Field].ToString();
                            }
                            catch { }
                        }
                    }                    
                    break;
            }
        }
        

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Pager)
            {
                e.Row.Cells[0].Visible = false;
            }
        }        

        protected void searchBTN_Click(object sender, EventArgs e)
        {
            Session["StoreName"] = sstoreDDL.SelectedValue;
            Session["Program"] = programDDL.SelectedValue;
            Session["Owner"] = ownerTXT.Text;
            Session["StoreNumber"] = StoreNumberTXT.Text;
            BindGridData();
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            GridView2.AllowPaging = false;
            GridView2.DataSource = Session["ExportTable"];
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

        private void DateFilters(ref string StartDate, ref string EndDate)
        {
            if (string.IsNullOrWhiteSpace(StartDate))
                StartDate = FirstDayOfYear;

            if (string.IsNullOrWhiteSpace(EndDate))
                EndDate = DateTime.Now.ToString("yyyy-MM-dd");
        }

        protected void SQLSelecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            e.Command.CommandTimeout = 3600;
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            BindGridData();            
        }

        private string DatePickerFormat(string date)
        {
            return string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(date));
        }
        
    }
}