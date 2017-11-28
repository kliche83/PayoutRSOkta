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
    public partial class CC : System.Web.UI.Page
    {
        string user;
        string userType;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                user = Session["user"].ToString();
                userType = Session["userType"].ToString();
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }

            if (user != "Admin")
            {
                if (user == "andjelka@thesmartcircle.com")
                {
                    typeDDL.Items.Remove("Sales");
                    typeDDL.Items.Remove("FWreports");
                }
                else if (user == "cband@thesmartcircle.com" || user == "thourtovenko@thesmartcircle.com")
                {
                    typeDDL.Items.Remove("Payouts");
                    typeDDL.Items.Remove("Overrides");
                    typeDDL.Items.Remove("FWreports");
                }
                else
                {
                    typeDDL.Items.Remove("Sales");
                    typeDDL.Items.Remove("Payouts");
                    typeDDL.Items.Remove("Overrides");
                }
            }

            if (IsPostBack)
            {
                SqlDataSource1.SelectCommand = "SELECT * FROM ( SELECT *, (select Firstname + ' ' + Lastname from PAYOUTpeople where Id = CC1) AS [CC1Name], (select Firstname + ' ' + Lastname from PAYOUTpeople where Id = CC2) AS [CC2Name], (select Firstname + ' ' + Lastname from PAYOUTpeople where Id = CC3) AS [CC3Name], (select Firstname + ' ' + Lastname from PAYOUTpeople where Id = CC4) AS [CC4Name], (select Firstname + ' ' + Lastname from PAYOUTpeople where Id = CC5) AS [CC5Name], (select Firstname + ' ' + Lastname from PAYOUTpeople where Id = CC6) AS [CC6Name] FROM [PAYOUTmailList] JOIN [PAYOUTpeople] ON [To] = [Id] WHERE [Type] = '" + typeDDL.SelectedValue + "' AND [Firstname] + ' ' + [Lastname] LIKE '%" + nameTXT.Text + "%' )x ORDER BY [Firstname], [Lastname]";
                SqlDataSource1.UpdateCommand = "UPDATE [PAYOUTmailList] SET CC1=@CC1, CC2=@CC2, CC3=@CC3, CC4=@CC4, CC5=@CC5, CC6=@CC6 WHERE [Type] = '" + typeDDL.SelectedValue + "' AND [To] = @To";
            }
        }

        protected void searchBTN_Click(object sender, EventArgs e)
        {
            GridView1.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.Pager)
                {
                    e.Row.Cells[3].Visible = false;
                }
            }
            catch { }
        }

        //protected void DropChanged(object sender, EventArgs e)
        //{
        //    DropDownList ddl = (DropDownList)sender;
        //    GridViewRow gvr = (GridViewRow)ddl.NamingContainer;
        //    int rowindex = gvr.RowIndex;
        //    using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        //    {
        //        string update = "UPDATE [PAYOUTmailList] SET " + ddl.ID + " = '" + ddl.SelectedValue + "' WHERE [Type] = '" + typeDDL.SelectedValue + "' AND [To] = '" + GridView1.Rows[rowindex].Cells[3].Text + "'";
        //        con.Open();
        //        using (SqlCommand cmd = new SqlCommand(update, con))
        //        {
        //            SqlDataReader reader = cmd.ExecuteReader();
        //        }
        //        con.Close();
        //    }
        //}

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            GridView2.DataBind();
            string attachment = "attachment; filename=\"Payouts Email List.xls\"";
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