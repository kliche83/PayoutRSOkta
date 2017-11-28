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
    public partial class ShowsCount : Page
    {
        string user = string.Empty;
        List<string> ColumnsList;
              

        protected void Page_Load(object sender, EventArgs e)
        {
            ColumnsList = new List<string>() { "Shows Scheduled", "Open Shows", "Unopened Shows" };
            if (!IsPostBack)
            {
                FillDDls();                
                GridView1.DataSource = getResults();
                GridView1.DataBind();
            }
            ValidationText.Text = "";
        }

        protected void searchBTN_Click(object sender, EventArgs e)
        {
            if (DDLShowCount.SelectedValue == "" || txtSalesDate.Text == "")
            {
                ValidationText.Text = "*All the fields are required";
            }
            else
            {                
                GridView1.DataSource = getResults();
                GridView1.DataBind();

                if (((DataTable)GridView1.DataSource).Rows.Count > 0)
                {
                    Common.TotalizeFooter(ref GridView1, "Shows Scheduled");
                    Common.TotalizeFooter(ref GridView1, "Open Shows");
                    Common.TotalizeFooter(ref GridView1, "Unopened Shows");
                }                
            }
        }

        private DataTable getResults()
        {
            Dictionary<string, string> Params = new Dictionary<string, string>();

            if (DDLShowCount.SelectedValue == "")
                Params.Add("WebStoreName", "NULL");
            else
                Params.Add("WebStoreName", DDLShowCount.SelectedValue);


            DateTime OutdateResult = new DateTime();
            DateTime.TryParse(txtSalesDate.Text.Trim(), out OutdateResult);
            if (OutdateResult == DateTime.MinValue)
                Params.Add("WebSalesDate", "NULL");
            else
                Params.Add("WebSalesDate", txtSalesDate.Text);

            DataTable dt = Queries.GetResultsFromStoreProcedure("spx_PAYOUTShowsCount", ref Params).Tables[0];

            return dt;
        }

        private void FillDDls()
        {
            string sqlString = "SELECT DISTINCT CASE WHEN StoreName LIKE 'Kroger%' THEN 'Kroger' ELSE StoreName END StoreName FROM PAYOUTsales";
            DDLShowCount.DataSource = Queries.GetResultsFromQueryString(sqlString);
            DDLShowCount.DataValueField = "StoreName";
            DDLShowCount.DataBind();
        }


        //This allow Gridview to be downloaded as .xls, .csv, etc
        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Verifies that the control is rendered */
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {               
            string attachment = "attachment; filename=\"Shows Count.xls\"";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            GridView1.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();     
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.Cells.Count > 1)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (ColumnsList.Contains((((DataControlFieldCell)((e.Row.Cells[i]))).ContainingField).HeaderText))
                        e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                }
            }            
        }
    }
}