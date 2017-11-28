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
    public partial class Comm : System.Web.UI.Page
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

            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string getEff = "SELECT CONVERT(NVARCHAR, MAX([EffectiveDate]), 101) FROM [PAYOUTcommissions]";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(getEff, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LatestEffDate.Value = reader[0].ToString();
                    }
                }
                con.Close();
            }

            string qDate;
            if (dateEff.Text == "") { qDate = ""; } else { qDate = "[EffectiveDate] = '" + dateEff.Text + "' AND "; }
            //SqlDataSource1.SelectCommand = "SELECT Id, StoreName, Program, CONVERT(NVARCHAR, StandardComm*100) + ' %' AS StandardComm, CONVERT(NVARCHAR, SpecialComm*100) + ' %' AS SpecialComm, CONVERT(NVARCHAR, RTExtra*100) + ' %' AS RTExtra, CONVERT(NVARCHAR, RTExtraCali*100) + ' %' AS RTExtraCali ,DollerPerDay,DollerPerPiece, EffectiveDate  FROM [PAYOUTcommissions] WHERE " + qDate + "StoreName LIKE '" + storeDDLs.SelectedValue + "%' AND Program LIKE '" + ((progDDLs.SelectedValue == "All") ? "%" : progDDLs.SelectedValue) + "%' ORDER BY [EffectiveDate] DESC, [StoreName], [Program]";
            SqlDataSource1.SelectCommand = "SELECT Id, StoreName, Program, CONVERT(NVARCHAR, StandardComm*100) + ' %' AS StandardComm, CONVERT(NVARCHAR, SpecialComm*100) + ' %' AS SpecialComm, CONVERT(NVARCHAR, RTExtra*100) + ' %' AS RTExtra, CONVERT(NVARCHAR, RTExtraCali*100) + ' %' AS RTExtraCali , EffectiveDate  FROM [PAYOUTcommissions] WHERE " + qDate + "StoreName LIKE '" + storeDDLs.SelectedValue + "%' AND Program LIKE '" + ((progDDLs.SelectedValue == "All") ? "%" : progDDLs.SelectedValue) + "%' ORDER BY [EffectiveDate] DESC, [StoreName], [Program]";
        }

        protected void searchBTN_Click(object sender, EventArgs e)
        {
            GridView1.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Pager && e.Row.RowType != DataControlRowType.EmptyDataRow)
            {
                e.Row.Cells[0].Visible = false;
            }
        }

        protected void FieldChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow gvr = (GridViewRow)txt.NamingContainer;
            int rowindex = gvr.RowIndex;
            string val = string.Empty;
           // if (txt.ID != "EffectiveDate" && txt.ID != "DollerPerPiece" && txt.ID != "DollerPerDay")

            if (txt.ID != "EffectiveDate" )
            {
                if (txt.Text == "" || txt.Text.Replace("%", "").Replace(" ", "") == "0")
                {
                    val = "NULL";
                }
                else
                {
                    val = txt.Text.Replace("%", "").Replace("000", "0").Replace(" ", "");
                    if (val == "0")
                    {
                        val = "NULL";
                    }
                    else
                    {
                        val = "'" + (Convert.ToDecimal(txt.Text.Replace("%", "").Replace(" ", "")) / 100).ToString() + "'";
                    }
                }
            }
            else
            {
                    val = "'" + txt.Text + "'";
            }

            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string update = "UPDATE [PAYOUTcommissions] SET " + txt.ID + " = " + val + " WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "'";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(update, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }

            GridView1.DataBind();
        }

        protected void DropChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            GridViewRow gvr = (GridViewRow)ddl.NamingContainer;
            int rowindex = gvr.RowIndex;
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                string update = "UPDATE [PAYOUTcommissions] SET " + ddl.ID + " = '" + ddl.SelectedValue + "' WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "'";
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
            string Program = progDDL.SelectedValue;
            string EffDate = EFFD.Text;

            string Standard = string.Empty;
            string Special = string.Empty;
            string RTExtra = string.Empty;
            string RTExtraC = string.Empty;
            
            if (STCMM.Text != "")
            {
                Standard = (Convert.ToInt32(STCMM.Text) / 100).ToString("0.00");
            }
            else
            {
                Standard = "0";
            }
            if (SPCMM.Text != "")
            {
                Special = (Convert.ToInt32(SPCMM.Text) / 100).ToString("0.00");
            }
            else
            {
                Special = "0";
            }
            if (RTES.Text != "")
            {
                RTExtra = (Convert.ToInt32(RTES.Text) / 100).ToString("0.00");
            }
            else
            {
                RTExtra = "0";
            }
            if (RTESC.Text != "")
            {
                RTExtraC = (Convert.ToInt32(RTESC.Text) / 100).ToString("0.00");
            }
            else
            {
                RTExtraC = "0";
            }

            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
               // string insert = "INSERT INTO [PAYOUTcommissions] VALUES ('" + StoreName + "', '" + Program + "', '" + Standard + "', '" + Special + "', '" + RTExtra + "', '" + RTExtraC + "', '" + EffDate + "', '" + TxtPerDay + "', '" + TxtPerPiece + "')";
                /*  <asp:TemplateField HeaderText="Doller Per Day"  SortExpression="DollerPerDay" >
                    <ItemTemplate>
                        <asp:TextBox ID="DollerPerDay" runat="server" Text='<%# Bind("[DollerPerDay]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>

                    <asp:TemplateField HeaderText="Doller Per Piece"  SortExpression="DollerPerPiece" >
                    <ItemTemplate>
                        <asp:TextBox ID="DollerPerPiece" runat="server" Text='<%# Bind("[DollerPerPiece]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>*/

                /*
                 
                   <asp:BoundField DataField="DollerPerDay" HeaderText="Doller Per Day" SortExpression="DollerPerDay" ReadOnly="true" />
                 <asp:BoundField DataField="DollerPerPiece" HeaderText="Doller Pe rPiece" SortExpression="DollerPerPiece" ReadOnly="true" />
                 */
                string insert = "INSERT INTO [PAYOUTcommissions] VALUES ('" + StoreName + "', '" + Program + "', '" + Standard + "', '" + Special + "', '" + RTExtra + "', '" + RTExtraC + "', '" + EffDate +  "')";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(insert, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();

                string update = "UPDATE [PAYOUTcommissions] SET [StandardComm] = NULL WHERE [StandardComm] = 0 " +
                                "UPDATE [PAYOUTcommissions] SET [SpecialComm] = NULL WHERE [SpecialComm] = 0 " +
                                "UPDATE [PAYOUTcommissions] SET [RTExtra] = NULL WHERE [RTExtra] = 0 " +
                                "UPDATE [PAYOUTcommissions] SET [RTExtraCali] = NULL WHERE [RTExtraCali] = 0";
                con.Open();
                using (SqlCommand cmd = new SqlCommand(update, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }

            Response.Redirect("Comm.aspx", true);
        }

        protected void delLink_Click(object sender, EventArgs e)
        {
            LinkButton del = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)del.NamingContainer;
            GridView gv = (GridView)gvr.NamingContainer;
            int delRow = gvr.RowIndex;

            string delete = "DELETE FROM [PAYOUTcommissions] WHERE [Id] = '" + gv.Rows[delRow].Cells[0].Text + "'";

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

        protected void weekBtn_Click(object sender, EventArgs e)
        {
            string closeWeek_confirmValue = Request.Form["closeWeek_confirm_value"];
            if (closeWeek_confirmValue == "Yes")
            {
                string close = "INSERT INTO [PAYOUTcommissions] SELECT StoreName, Program, StandardComm, SpecialComm, RTExtra, RTExtraCali, dateadd(week, 2, max([EffectiveDate])) AS EffectiveDate  FROM [PAYOUTcommissions] WHERE [EffectiveDate] = (select max([EffectiveDate]) from [PAYOUTcommissions]) GROUP BY StoreName, Program, StandardComm, SpecialComm, RTExtra, RTExtraCali";
              //  string close = "INSERT INTO [PAYOUTcommissions] SELECT StoreName, Program, StandardComm, SpecialComm, RTExtra, RTExtraCali, dateadd(week, 2, max([EffectiveDate])) AS EffectiveDate ,DollerPerDay,DollerPerDay FROM [PAYOUTcommissions] WHERE [EffectiveDate] = (select max([EffectiveDate]) from [PAYOUTcommissions]) GROUP BY StoreName, Program, StandardComm, SpecialComm, RTExtra, RTExtraCali,DollerPerDay,DollerPerPiece";
                
                using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(close, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                    con.Close();
                }
            }

            Response.Redirect("Comm.aspx", true);
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            GridView2.DataBind();
            string attachment = "attachment; filename=\"RoadShow Commission Chart.xls\"";
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