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
    public partial class Codes : System.Web.UI.Page
    {
        string user;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                user = Session["user"].ToString();
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }

            missing.Visible = false;

            //"WHAT'S MISSING?"
            //try
            //{
                using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
                {
                    string insertMissing = "INSERT INTO PAYOUTcodes SELECT 'Program' AS [Type], x.Program AS [Name], '' AS [Code] FROM ( select Program from PAYOUTschedule where Program != '' and Program is not null union all select Program from PAYOUTsales where Program != '' and Program is not null )x WHERE NOT EXISTS ( select Name from PAYOUTcodes c where Type = 'Program' and x.Program like '%' + c.Name + '%' ) GROUP BY Program ORDER BY Program";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(insertMissing, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                    con.Close();
                }
            //}
            //catch { }

                SqlDataSource1.SelectCommand = "SELECT [Id], [Name], [Code] FROM [PAYOUTcodes] WHERE [Type] = 'Program' AND [Name] LIKE '%" + ProgNameTXT.Text + "%' AND [Code] LIKE '%" + ProgCodeTXT.Text + "%' ORDER BY CASE WHEN [Code] = '' THEN [Code] ELSE [Name] END";
        }

        protected void searchBTN_Click(object sender, EventArgs e)
        {
            GridView1.DataBind();
        }

        int i = 0;

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Pager)
            {
                e.Row.Cells[0].Visible = false;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (((TextBox)e.Row.FindControl("Code")).Text == "")
                    {
                        i++;
                        e.Row.CssClass = "noEmail";
                        missingL.Text = i.ToString() + " program(s) are missing a short code (highlighted in red).<br />" +
                                        "Please update them before sending reports.<br /><br /><br />" +
                                        "<a id='gotIt'>Got It</a>";
                        missing.Visible = true;
                    }
                }
            }
        }

        protected void delLink_Click(object sender, EventArgs e)
        {
            LinkButton del = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)del.NamingContainer;
            GridView gv = (GridView)gvr.NamingContainer;
            int delRow = gvr.RowIndex;

            string delete = "DELETE FROM [PAYOUTcodes] WHERE [Id] = '" + gv.Rows[delRow].Cells[0].Text + "'";

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

        protected void FieldChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow gvr = (GridViewRow)txt.NamingContainer;
            int rowindex = gvr.RowIndex;
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string update = "UPDATE [PAYOUTcodes] SET " + txt.ID + " = '" + txt.Text + "' WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "'";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(update, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }
        }

        //protected void addBtn_Click(object sender, EventArgs e)
        //{
        //    string StoreName = storeDDL.SelectedValue;
        //    string UPCs = UPC.Text;
        //    string ItemNumber = INO.Text;
        //    string ItemName = INA.Text;
        //    string ItemName2 = INA2.Text;
        //    string UnitCost = IUC.Text;
        //    string Program = progDDL.SelectedValue;

        //    using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        //    {
        //        string insert = "INSERT INTO [PAYOUTcodes] VALUES ('" + StoreName + "', '" + UPCs + "', '" + ItemNumber + "', '" + ItemName + "', '" + ItemName2 + "', '" + UnitCost + "', '" + Program + "')";
        //        con.Open();
        //        using (SqlCommand cmd = new SqlCommand(insert, con))
        //        {
        //            SqlDataReader reader = cmd.ExecuteReader();
        //        }
        //        con.Close();
        //    }

        //    Response.Redirect("Items.aspx", true);
        //}

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            GridView2.DataBind();
            string attachment = "attachment; filename=\"RoadShow Short Codes.xls\"";
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