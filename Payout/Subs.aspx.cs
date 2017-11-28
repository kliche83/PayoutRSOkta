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

namespace Payout
{
    public partial class Subs : System.Web.UI.Page
    {
        string user;
        string Query = "";
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
            try
            {
                user = Session["user"].ToString();
                FillControls();
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }
        }

        private void FillControls()
        {
            string testQuery = "";
            if (TestDDL.SelectedValue == "All") { testQuery = ""; }
            if (TestDDL.SelectedValue == "1") { testQuery = " AND [Test] = 1"; }
            if (TestDDL.SelectedValue == "0") { testQuery = " AND [Test] = 0"; }

            if (dateFrom.Text == "" && dateTo.Text == "")
            {
                Query = "SELECT * FROM [PAYOUTchanges] WHERE [StoreName] LIKE '" + storeDDLs.SelectedValue + "%' AND [StoreNumber] LIKE '%" + StoreNumberTXT.Text + "%' AND ([ItemNumber] LIKE '%" + ItemNumberTXT.Text + "%' OR [SubItem] LIKE '%" + ItemNumberTXT.Text + "%')" + testQuery + " ORDER BY [StartDate] DESC";
            }
            if (dateFrom.Text != "" && dateTo.Text != "")
            {
                Query = "SELECT * FROM [PAYOUTchanges] WHERE [StoreName] LIKE '" + storeDDLs.SelectedValue + "%' AND [StoreNumber] LIKE '%" + StoreNumberTXT.Text + "%' AND ([ItemNumber] LIKE '%" + ItemNumberTXT.Text + "%' OR [SubItem] LIKE '%" + ItemNumberTXT.Text + "%')" + testQuery + " AND [StartDate] >= '" + dateFrom.Text + "' AND [EndDate] <= '" + dateTo.Text + "' ORDER BY [StartDate] DESC";
            }
            if (dateFrom.Text != "" && dateTo.Text == "")
            {
                Query = "SELECT * FROM [PAYOUTchanges] WHERE [StoreName] LIKE '" + storeDDLs.SelectedValue + "%' AND [StoreNumber] LIKE '%" + StoreNumberTXT.Text + "%' AND ([ItemNumber] LIKE '%" + ItemNumberTXT.Text + "%' OR [SubItem] LIKE '%" + ItemNumberTXT.Text + "%')" + testQuery + " AND [StartDate] >= '" + dateFrom.Text + "' ORDER BY [StartDate] DESC";
            }
            if (dateFrom.Text == "" && dateTo.Text != "")
            {
                Query = "SELECT * FROM [PAYOUTchanges] WHERE [StoreName] LIKE '" + storeDDLs.SelectedValue + "%' AND [StoreNumber] LIKE '%" + StoreNumberTXT.Text + "%' AND ([ItemNumber] LIKE '%" + ItemNumberTXT.Text + "%' OR [SubItem] LIKE '%" + ItemNumberTXT.Text + "%')" + testQuery + " AND [EndDate] <= '" + dateTo.Text + "' ORDER BY [StartDate] DESC";
            }
            SqlDataSource1.SelectCommand = Query;
        }

        protected void searchBTN_Click(object sender, EventArgs e)
        {
            GridView1.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Pager)
            {
                e.Row.Cells[0].Visible = false;
            }
        }

        protected void FieldChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow gvr = (GridViewRow)txt.NamingContainer;
            int rowindex = gvr.RowIndex;
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string update = "UPDATE [PAYOUTchanges] SET " + txt.ID + " = '" + txt.Text + "', ModifiedBy = '" + user + "', ModifiedOn = CURRENT_TIMESTAMP WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "' " /*+ updateR*/;
                con.Open();
                using (SqlCommand cmd = new SqlCommand(update, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }
        }

        protected void DropChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            GridViewRow gvr = (GridViewRow)ddl.NamingContainer;
            int rowindex = gvr.RowIndex;
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string update = "UPDATE [PAYOUTchanges] SET " + ddl.ID + " = '" + ddl.SelectedValue + "' WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "' " /*+ updateR*/;
                con.Open();
                using (SqlCommand cmd = new SqlCommand(update, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }
        }

        protected void CheckChanged(object sender, EventArgs e)
        {
            CheckBox chb = (CheckBox)sender;
            GridViewRow gvr = (GridViewRow)chb.NamingContainer;
            int rowindex = gvr.RowIndex;
            string isChecked = (chb.Checked) ? "1" : "0";
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string update = "UPDATE [PAYOUTchanges] SET " + chb.ID + " = '" + isChecked + "', ModifiedBy = '" + user + "', ModifiedOn = CURRENT_TIMESTAMP WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "'";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(update, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }
        }

        protected void addBtn_Click(object sender, EventArgs e)
        {
            string StoreName = storeDDL.SelectedValue;
            string StoreNumber = SNO.Text;
            string ItemNumber = INO.Text;
            string SubItem = SubNO.Text;
            string SubCost = SubUC.Text;
            string StartDate = SSD.Text;
            string EndDate = SED.Text;
            string Test = (wasTest.Checked) ? "1" : "0";

            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string insert = "INSERT INTO [PAYOUTchanges] VALUES ('" + StoreName + "', '" + StoreNumber + "', '" + ItemNumber + "', '" + SubItem + "', '" + SubCost + "', '" + StartDate + "', '" + EndDate + "', '" + Test + "', '" + user + "', CURRENT_TIMESTAMP, NULL, NULL) " /*+ updateR*/;
                con.Open();
                using (SqlCommand cmd = new SqlCommand(insert, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }

            Response.Redirect("Subs.aspx", true);
        }

        protected void delLink_Click(object sender, EventArgs e)
        {
            LinkButton del = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)del.NamingContainer;
            GridView gv = (GridView)gvr.NamingContainer;
            int delRow = gvr.RowIndex;

            string delete = "DELETE FROM [PAYOUTchanges] WHERE [Id] = '" + gv.Rows[delRow].Cells[0].Text + "'";

            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(delete, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }

            gv.DataBind();
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            GridView2.DataBind();
            string attachment = "attachment; filename=\"RoadShow Item Substitutions.xls\"";
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