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
using OfficeOpenXml;

namespace Payout
{
    public partial class Items : System.Web.UI.Page
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

          

            SqlDataSource1.SelectCommand = "SELECT [Id], [StoreName], [ItemNumber] , [JDENumber], [ItemName], [UnitCost] , [Status] , [VendorCost], [Program],[EffectiveDate] ,[Username] FROM [PAYOUTitemMaster] WHERE [ItemName] LIKE '%" + ItemNameTXT.Text + "%'" + ((StoreNameDDL.SelectedValue == "") ? "" : " AND [StoreName] = '" + StoreNameDDL.SelectedValue + "'") + ((ProgramDDL.SelectedValue == "") ? "" : " AND [Program] = '" + ProgramDDL.SelectedValue + "'") + ((ItemNumberTXT.Text == "") ? "" : " AND [ItemNumber] = '" + ItemNumberTXT.Text + "'") + ((ItemJDENumberTXT.Text == "") ? "" : " AND [JDENumber] = '" + ItemJDENumberTXT.Text + "'") + ((DDLStatus.SelectedValue == "") ? "" : " AND [Status] = '" + DDLStatus.SelectedValue + "'") + " ORDER BY [ItemName]";
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

                /*update database when vendorcost is change by a user*/
                if (txt.ID == "VendorCost") {

                    string update = "UPDATE [PAYOUTitemMaster] SET  [Username] = '"+ user + "'," + txt.ID + " = '" + txt.Text + "' WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "'";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(update, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                    con.Close();
                    GridView1.DataBind();
                }

                else
                {/*update database for any changes */
                    string update = "UPDATE [PAYOUTitemMaster] SET " + txt.ID + " = '" + txt.Text + "' WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "'";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(update, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                    con.Close();
                }
            }
        }

        protected void DropChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            GridViewRow gvr = (GridViewRow)ddl.NamingContainer;
            int rowindex = gvr.RowIndex;
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string update = "UPDATE [PAYOUTitemMaster] SET " + ddl.ID + " = '" + ddl.SelectedValue + "' WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "'";
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
            string ItemNumber = INO.Text;
            string JDENumber = JNO.Text;
            string ItemName = INA.Text;
            string UnitCost = IUC.Text;
            string Program = progDDL.SelectedValue;
            decimal VendorCost = 0;
            string Status = "Active";
            DateTime EffectiveDate = DateTime.Now;


            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string insert = "INSERT INTO [PAYOUTitemMaster] VALUES ('" + Program + "', '" + StoreName + "', '" + ItemNumber + "', '" + JDENumber + "', '" + ItemName + "', '" + UnitCost + "', '" + VendorCost + "', '" + Status + "', '" + user + "', '" + EffectiveDate + "')";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(insert, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }

            Response.Redirect("Items.aspx", true);
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

        protected void delLink_Click(object sender, EventArgs e)
        {
            LinkButton del = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)del.NamingContainer;
            GridView gv = (GridView)gvr.NamingContainer;
            int delRow = gvr.RowIndex;

            string delete = "DELETE FROM [PAYOUTitemMaster] WHERE [Id] = '" + gv.Rows[delRow].Cells[0].Text + "'";

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

        protected void exportSales_Click(object sender, EventArgs e)
        {
            ExcelPackage pck = new ExcelPackage();
            string filename = string.Empty;

            filename = "RS Item Master " + DateTime.Now.ToShortDateString().Replace("/", "-");

            using (pck)
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(filename);
                DataSourceSelectArguments args = new DataSourceSelectArguments();
                DataView DV = (DataView)SqlDataSource1.Select(args);
                DataTable DT = DV.ToTable();

                ws.Cells["A1"].LoadFromDataTable(DT, true);
                ws.Cells["A1:Z2000"].AutoFitColumns();

                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.Flush();
                Response.End();
            }
        }
    }
}