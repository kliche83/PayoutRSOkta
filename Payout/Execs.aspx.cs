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
    public partial class Execs : System.Web.UI.Page
    {
        string user;
        string execPersonString;
        string execProgramString;

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

            SqlDataSource1.SelectCommand = "SELECT * FROM [PAYOUTexecs] JOIN [PAYOUTpeople] ON [Person] = [PAYOUTpeople].[Id] WHERE ([Firstname] LIKE '%" + nameTXT.Text + "%' OR [Lastname] LIKE '%" + nameTXT.Text + "%') AND [ExecOf] LIKE '" + ((progDDLs.SelectedValue == "All") ? "%" : progDDLs.SelectedValue) + "%' ORDER BY [Firstname], [Lastname]";
        }

        protected void backBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("ExecsBulk.aspx", true);
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

        protected void DropChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            GridViewRow gvr = (GridViewRow)ddl.NamingContainer;
            GridView gv = (GridView)gvr.NamingContainer;
            int rowindex = gvr.RowIndex;
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string update = "UPDATE [PAYOUTexecs] SET [ExecOf] = '" + ddl.SelectedValue + "' WHERE [Id] = '" + gv.Rows[rowindex].Cells[0].Text + "'";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(update, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }
        }

        protected void delLink_Click(object sender, EventArgs e)
        {
            LinkButton del = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)del.NamingContainer;
            GridView gv = (GridView)gvr.NamingContainer;
            int delRow = gvr.RowIndex;

            string delete = "DELETE FROM [PAYOUTexecs] WHERE [Id] = '" + gv.Rows[delRow].Cells[0].Text + "'";
            
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

        protected void addBulkExec_Click(object sender, EventArgs e)
        {
            Response.Redirect("ExecsBulk.aspx", true);
        }

        protected void addExec_Click(object sender, EventArgs e)
        {
            execPersonString = Request.Form["execPerson"];
            execProgramString = Request.Form["execProgram"];

            string insert = string.Empty;

            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                if (execProgramString == "All")
                {
                    insert = "INSERT INTO [PAYOUTexecs] SELECT '" + execPersonString + "', Program FROM PAYOUTschedule WHERE Program != '' AND Program IS NOT NULL GROUP BY Program";
                }
                else
                {
                    insert = "INSERT INTO [PAYOUTexecs] VALUES('" + execPersonString + "', '" + execProgramString + "')";
                }

                con.Open();
                using (SqlCommand cmd = new SqlCommand(insert, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();

                string cleanUp = ";WITH x AS ( SELECT [Person], [ExecOf], rn = ROW_NUMBER() OVER (PARTITION BY [Person], [ExecOf] ORDER BY [Person], [ExecOf]) FROM [PAYOUTexecs] ) DELETE x WHERE rn > 1;";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(cleanUp, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }

            GridView1.DataBind();
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            GridView2.DataBind();
            string attachment = "attachment; filename=\"Payouts Executives List.xls\"";
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