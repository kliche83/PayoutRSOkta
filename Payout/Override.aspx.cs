using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Payout
{
    public partial class Override : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string user = string.Empty;
            string userType = string.Empty;

            try
            {
                user = Session["user"].ToString();
                userType = Session["userType"].ToString();
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }

            adminSpan.Visible = false;

            if (userType == "Admin")
            {
                adminSpan.Visible = true;
            }
        }

        protected void searchBtn_Click(object sender, EventArgs e)
        {
            string ovType = ovTypeDDL.SelectedValue;
            string fDate = dateFrom.Text;
            string tDate = dateTo.Text;
            string ownerid = promoDDL.SelectedValue;
            string cur = curDDL.SelectedValue;
            string edglc = (glTXT.Text != "") ? glTXT.Text:"*" ;

            SqlDataSource1.SelectCommand = "spx_PAYOUToverrides";
            SqlDataSource1.SelectParameters["ovType"].DefaultValue = ovType;
            SqlDataSource1.SelectParameters["fDate"].DefaultValue = fDate;
            SqlDataSource1.SelectParameters["tDate"].DefaultValue = tDate;
            SqlDataSource1.SelectParameters["ownerid"].DefaultValue = ownerid;
            SqlDataSource1.SelectParameters["edglc"].DefaultValue = edglc;
            SqlDataSource1.SelectParameters["cur"].DefaultValue = cur;

            GridView1.DataBind();
        }

        protected void SQLSelecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            e.Command.CommandTimeout = 3600;
        }

        double RunTotflt;
        protected void GridDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (ovTypeDDL.SelectedValue == "DTV")
                {
                    if (e.Row.Cells[3].Text == "Total")
                    {
                        e.Row.Attributes.Add("class", "selectedRow");

                        RunTotflt = 0;
                    }
                    else
                    {
                        RunTotflt += Convert.ToDouble(e.Row.Cells[10].Text);
                        e.Row.Cells[11].Text = RunTotflt.ToString("#,0.00");
                    }

                    for (int i = 0; i < 25; i++)
                    {
                        if (i == 4 && e.Row.Cells[i].Text != "&nbsp;")
                        {
                            e.Row.Cells[i].Text = Convert.ToDateTime(e.Row.Cells[i].Text).ToString("MM/dd/yy");
                        }

                        if (8 <= i && i <= 11 && i != 9 && e.Row.Cells[i].Text != "&nbsp;")
                        {
                            e.Row.Cells[i].Text = Convert.ToDouble(e.Row.Cells[i].Text).ToString("#,0.00");
                        }

                        if (7 <= i && i <= 11)
                        {
                            e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                        }
                    }
                }
                else
                {
                    if (e.Row.Cells[3].Text == "Total")
                    {
                        e.Row.Attributes.Add("class", "selectedRow");

                        RunTotflt = 0;
                    }
                    else
                    {
                        RunTotflt += Convert.ToDouble(e.Row.Cells[16].Text);
                        e.Row.Cells[17].Text = RunTotflt.ToString("#,0.00");
                    }

                    for (int i = 0; i < 25; i++)
                    {
                        if (i == 6 && e.Row.Cells[i].Text != "&nbsp;")
                        {
                            e.Row.Cells[i].Text = Convert.ToDateTime(e.Row.Cells[i].Text).ToString("MM/dd/yy");
                        }

                        if (11 <= i && i <= 18 && i != 15 && e.Row.Cells[i].Text != "&nbsp;")
                        {
                            e.Row.Cells[i].Text = Convert.ToDouble(e.Row.Cells[i].Text).ToString("#,0.00");
                        }

                        if (10 <= i && i <= 18)
                        {
                            e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                        }
                    }
                }
            }
        }

        protected void email_Click(object sender, EventArgs e)
        {
            string emailClick_confirm_value = Request.Form["emailClick_confirm_value"];

            if (emailClick_confirm_value == "Yes")
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
                {
                    string addMissing = @"
                                            INSERT INTO [PAYOUTpeople]
                                            SELECT SUBSTRING([Person], 1, CHARINDEX(' ', [Person]) ) AS [Firstname], SUBSTRING([Person], CHARINDEX(' ', [Person]) + 1, LEN([Person]) ) AS [Lastname], [Email], NULL, NULL, 0 
                                            FROM (
	                                            SELECT DISTINCT o.[Promo] AS [Person], p.[Email]
	                                            FROM [PAYOUToverride] o
	                                            LEFT JOIN [PAYOUTpeople] p on p.Firstname + ' ' + p.Lastname = o.[Promo]
                                            )x
                                            WHERE x.Person != '' AND (x.Email IS NULL OR x.Email NOT LIKE '%@%') 
                                            AND x.Person NOT IN (
	                                            SELECT DISTINCT [Name] FROM [PAYOUToverrideExceps]
                                            ) 
                                            AND NOT EXISTS (
	                                            SELECT *
	                                            FROM [PAYOUTpeople] p
	                                            WHERE p.Firstname + ' ' + p.Lastname = x.Person
                                            ) 
                                           ";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(addMissing, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                    }
                    con.Close();

                    string checkMissing = @"
                                            SELECT SUBSTRING([Person], 1, CHARINDEX(' ', [Person]) ) AS [Firstname], SUBSTRING([Person], CHARINDEX(' ', [Person]) + 1, LEN([Person]) ) AS [Lastname], [Email] 
                                            FROM (
	                                            SELECT DISTINCT o.[Promo] AS [Person], p.[Email]
	                                            FROM [PAYOUToverride] o
	                                            LEFT JOIN [PAYOUTpeople] p on p.Firstname + ' ' + p.Lastname = o.[Promo]
                                            )x
                                            WHERE x.Person != '' AND (x.Email IS NULL OR x.Email NOT LIKE '%@%') 
                                            AND x.Person NOT IN (
	                                            SELECT DISTINCT [Name] FROM [PAYOUToverrideExceps]
                                            ) 
                                           ";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(checkMissing, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            Response.Redirect("People.aspx", true);
                        }
                    }
                    con.Close();
                }

                //If everyone has an email assigned, then:

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = "mail.innovageusa.com";
                smtpClient.Port = 587; //or 25
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = false;
                smtpClient.Credentials = new NetworkCredential("salesreport@innovageusa.com", "SalesReport#13");
                smtpClient.UseDefaultCredentials = false;

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
                {
                    string selectPromos = @"
                                            SELECT DISTINCT o.[Promo], p.[Email] AS [PromoEmail]
	                                            , (SELECT [Email] FROM [PAYOUTpeople] WHERE [Id] = m.[CC1]) AS [CC1]
	                                            , (SELECT [Email] FROM [PAYOUTpeople] WHERE [Id] = m.[CC2]) AS [CC2]
	                                            , (SELECT [Email] FROM [PAYOUTpeople] WHERE [Id] = m.[CC3]) AS [CC3]
	                                            , (SELECT [Email] FROM [PAYOUTpeople] WHERE [Id] = m.[CC4]) AS [CC4]
	                                            , (SELECT [Email] FROM [PAYOUTpeople] WHERE [Id] = m.[CC5]) AS [CC5]
	                                            , (SELECT [Email] FROM [PAYOUTpeople] WHERE [Id] = m.[CC6]) AS [CC6]
                                            FROM [PAYOUToverride] o
                                            JOIN [PAYOUTpeople] p ON p.[Firstname] + ' ' + p.[Lastname] = o.[Promo]
                                            LEFT JOIN [PAYOUTmailList] m ON m.[To] = p.[Id]
                                            WHERE m.[Type] = 'Overrides' AND o.[Promo] NOT IN (
												SELECT DISTINCT [Name] FROM [PAYOUToverrideExceps]
											)
                                            ORDER BY o.[Promo]";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(selectPromos, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            MailMessage mail = new MailMessage();

                            //string sendToEmail = "amir@innovage.net";
                            string sendToEmail = reader["PromoEmail"].ToString();
                            string sendToName = reader["Promo"].ToString();

                            mail.From = new MailAddress("andjelka@thesmartcircle.com", "Andjelka Sarac");

                            mail.To.Add(new MailAddress(sendToEmail));

                            for (int i = 2; i <= 7; i++)
                            {
                                if (reader[i] != DBNull.Value && reader[i].ToString().Contains("@"))
                                {
                                    mail.CC.Add(new MailAddress(reader[i].ToString()));
                                }
                            }

                            //mail.Bcc.Add(new MailAddress("amir@innovage.net"));
                            mail.Bcc.Add(new MailAddress("andjelka@thesmartcircle.com"));

                            mail.Subject = sendToName + " Overrides - " + dateFrom.Text + " to " + dateTo.Text + "";

                            mail.IsBodyHtml = true;

                            mail.Body += "<style> " +
                                         "span { font-family: 'Segoe UI'; font-weight: normal; } " +
                                         "table { border-collapse:collapse; font-family: 'Segoe UI'; font-size: 10pt; font-weight: normal; } " +
                                         "th { border: 1px solid #000; text-align: left; background-color: #328EFE; color: #fff; padding: 0 5px; font-family: 'Segoe UI'; font-weight: normal; white-space: nowrap; } " +
                                         "td { border: 1px solid #000; text-align: left; padding: 0 5px; font-family: 'Segoe UI'; font-weight: normal; white-space: nowrap; } " +
                                         ".hideCol { display: none; padding: 0; border: 0; } " +
                                         ".right { text-align: right; } " +
                                         "</style>";

                            mail.Body += "<span>Hi " + sendToName + ", here are your overrides for the period " + dateFrom.Text + " to " + dateTo.Text + ":</span><br /><br />";

                            mail.Body += "<table>";

                            mail.Body += "<tr>";

                            if (ovTypeDDL.SelectedValue == "DTV")
                            {
                                mail.Body += "<th>ID</th>";
                                mail.Body += "<th>Promo</th>";
                                mail.Body += "<th>DC</th>";
                                mail.Body += "<th>CC</th>";
                                mail.Body += "<th>Date</th>";
                                mail.Body += "<th>Ship</th>";
                                mail.Body += "<th>Name</th>";
                                mail.Body += "<th>Qty</th>";
                                mail.Body += "<th>Amount</th>";
                                mail.Body += "<th>%</th>";
                                mail.Body += "<th>OR</th>";
                                mail.Body += "<th>Cum.OR</th>";
                                mail.Body += "<th>GL</th>";
                                mail.Body += "<th>Cur</th>";
                                mail.Body += "<th>Club</th>";
                            }
                            else
                            {
                                mail.Body += "<th>ID</th>";
                                mail.Body += "<th>Promo</th>";
                                mail.Body += "<th>DC</th>";
                                mail.Body += "<th>CC</th>";
                                mail.Body += "<th>TY</th>";
                                mail.Body += "<th>SC1</th>";
                                mail.Body += "<th>Date</th>";
                                mail.Body += "<th>Ship</th>";
                                mail.Body += "<th>Name</th>";
                                mail.Body += "<th>O#</th>";
                                mail.Body += "<th>Qty</th>";
                                mail.Body += "<th>Amount</th>";
                                mail.Body += "<th>TA</th>";
                                mail.Body += "<th>%</th>";
                                mail.Body += "<th>OR</th>";
                                mail.Body += "<th>Cum.OR</th>";
                                mail.Body += "<th>Cur</th>";
                            }

                            mail.Body += "</tr>";

                            foreach (GridViewRow row in GridView1.Rows)
                            {
                                if (row.Cells[1].Text.ToString().Equals(sendToName))
                                {
                                    mail.Body += "<tr>";

                                    int colStart = 0;
                                    int colEnd = 0;
                                    int rightStart = 0;
                                    int rightEnd = 0;

                                    if (ovTypeDDL.SelectedValue == "DTV")
                                    {
                                        colStart = 0;
                                        colEnd = 15;
                                        rightStart = 7;
                                        rightEnd = 11;
                                    }
                                    else
                                    {
                                        colStart = 0;
                                        colEnd = 21;
                                        rightStart = 10;
                                        rightEnd = 18;
                                    }

                                    for (int i = colStart; i < colEnd; i++)
                                    {
                                        string css = "";

                                        if (rightStart <= i && i <= rightEnd)
                                        {
                                            css = " class='right'";
                                        }

                                        if (ovTypeDDL.SelectedValue == "DTV")
                                        {
                                            mail.Body += "<td" + css + ">";
                                            mail.Body += row.Cells[i].Text.ToString();
                                            mail.Body += "</td>";
                                        }

                                        if (ovTypeDDL.SelectedValue != "DTV" && i != 12 && i != 14 && i != 18 && i != 20)
                                        {
                                            mail.Body += "<td" + css + ">";
                                            mail.Body += row.Cells[i].Text.ToString();
                                            mail.Body += "</td>";
                                        }
                                    }

                                    mail.Body += "</tr>";
                                }
                            }

                            mail.Body += "</table>";

                            //Attachment attFile = new Attachment(Server.MapPath("~/Exports/file.xlsx"));
                            //mail.Attachments.Add(attFile);
                            try
                            {
                                smtpClient.Send(mail);
                            }
                            catch { }
                            System.Threading.Thread.Sleep(700);
                        }
                    }
                }
            }
        }

        // This Method is used to render gridview control
        public string GetGridviewData(Control gv)
        {
            StringBuilder strBuilder = new StringBuilder();
            StringWriter strWriter = new StringWriter(strBuilder);
            HtmlTextWriter htw = new HtmlTextWriter(strWriter);
            gv.RenderControl(htw);
            return strBuilder.ToString();
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            string attachment = "attachment; filename=\"Payout Override " + dateFrom.Text.Replace("/", "-") + " to " + dateTo.Text.Replace("/", "-") + ".xls\"";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            GridView1.RenderControl(htw);

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

        protected void exclBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("OverrideExceps.aspx", true);
        }
    }
}