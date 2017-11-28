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
    public partial class Trainer : System.Web.UI.Page
    {
        string user = string.Empty;
        int rowindex;
      

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                user = Session["user"].ToString();

                SqlDataSource1.SelectCommand =
                @"SELECT t.[ID], t.[Firstname], t.[Lastname], t.[EmailAddress], t.[Role], t.[Active], t.[Salary], m.FirstName + ' ' + m.LastName Manager
                FROM [PAYOUTtrainer] t
                LEFT JOIN [PAYOUTtrainerManager] tm ON t.ID = tm.TrainerId
                LEFT JOIN [PAYOUTtrainManager] m ON tm.TrainManagerId = m.Id
                WHERE t.Firstname  LIKE '%" + Firstname.Text + "%' AND t.Lastname LIKE '%" + Lastname.Text + "%' ";
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Pager)
            {
                e.Row.Cells[0].Visible = false;
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView drv = e.Row.DataItem as DataRowView;
                DropDownList ddlTrainer= e.Row.FindControl("DropDownTrainer") as DropDownList;
                if (ddlTrainer != null)
                {
                    //Get the data from DB and bind the dropdownlist
                    ddlTrainer.SelectedValue = drv["active"].ToString();
                }

                DropDownList ddlRole = e.Row.FindControl("DropDownRole") as DropDownList;
                if (ddlRole != null)
                {
                    //Get the data from DB and bind the dropdownlist
                    ddlRole.SelectedValue = drv["role"].ToString();
                }

                DropDownList ddlManager = e.Row.FindControl("DropDownManager") as DropDownList;
                if (ddlManager != null)
                {
                    //Get the data from DB and bind the dropdownlist
                    ddlManager.SelectedValue = drv["Manager"].ToString();
                }
            }
        }


        


        protected void FieldChanged(Object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow gvr = (GridViewRow)txt.NamingContainer;
            rowindex = gvr.RowIndex;
            using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                if (txt.ID == "Salary")
                {
                    decimal OUTresult = 0;
                    if (!decimal.TryParse(txt.Text, out OUTresult))
                        txt.Text = OUTresult.ToString("F2");
                }
                string update = "UPDATE [PAYOUTTrainer] SET " + txt.ID + " = '" + txt.Text + "' WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "' " /*+ updateR*/;
                con.Open();
                using (SqlCommand cmd = new SqlCommand(update, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                }
                con.Close();
            }
        }

        protected void DropDownChanged(Object sender, EventArgs e)
        {
            DropDownList DDL = (DropDownList)sender;
            GridViewRow gvr = (GridViewRow)DDL.NamingContainer;
            string DDLValue = string.Empty;

            if (DDL.DataValueField.ToLower() == "role")
                DDLValue = "'" + DDL.SelectedValue + "'";

            if (DDL.DataValueField.ToLower() == "manager")
                DDLValue = "'" + DDL.SelectedValue + "'";

            if (DDL.DataValueField.ToLower() == "active")
                DDLValue = DDL.SelectedValue.ToLower().Replace("true", "1").Replace("false", "0");


            
            rowindex = gvr.RowIndex;
            string SQLstring = string.Empty;

            if (DDL.DataValueField.ToLower() == "manager")
            {
                SQLstring = string.Format(@"SELECT COUNT(*) FROM[PAYOUTtrainerManager] WHERE TrainerId = {0}", GridView1.Rows[rowindex].Cells[0].Text);
                DataTable dt = Queries.GetResultsFromQueryString(SQLstring);

                if (dt.Rows[0][0].ToString() == "0")
                {
                    SQLstring = string.Format(@"INSERT INTO [PAYOUTtrainerManager] ([TrainerId], [TrainManagerId]) VALUES ({0}, (SELECT TOP 1 Id 
                                                                                                                                  FROM [PAYOUTtrainManager] 
                                                                                                                                  WHERE FirstName + ' ' + LastName = {1}))",
                                                GridView1.Rows[rowindex].Cells[0].Text,
                                                DDLValue);
                }
                else
                {
                    if (DDLValue == "'Unassigned'")
                    {
                        SQLstring = string.Format(@"DELETE FROM [PAYOUTtrainerManager] WHERE TrainerId = {0}", GridView1.Rows[rowindex].Cells[0].Text);
                    }
                    else
                    {
                        SQLstring = string.Format(@"UPDATE [PAYOUTtrainerManager] SET TrainManagerId = (SELECT TOP 1 Id 
                                                                                                  FROM [PAYOUTtrainManager] 
                                                                                                  WHERE FirstName + ' ' + LastName = {0}) WHERE [TrainerId] = '{1}'",
                                                DDLValue,
                                                GridView1.Rows[rowindex].Cells[0].Text);
                    }                    
                }
            }
            else
            {
                SQLstring = string.Format("UPDATE [PAYOUTTrainer] SET {0} = {1} WHERE [Id] = '{2}'",
                DDL.DataValueField.ToLower(),
                DDLValue,
                GridView1.Rows[rowindex].Cells[0].Text);
            }

            Queries.ExecuteFromQueryString(SQLstring);
            
        }

        protected void addBtn_Click(object sender, EventArgs e)
        {
            string Firstname = Request.Form["TxtFName"].ToString();
            string lastname = Request.Form["TxtLname"].ToString();
            string Email = Request.Form["Txtemail"].ToString();
            string Role = Request.Form["DDLRole"].ToString();
            string Salary = Request.Form["TxtSalary"].ToString();
            string Manager = Request.Form["DDLManager"].ToString();


            bool Active = true;
              if (CHKActive.Checked)

                  Active = true;
              else
                  Active = false;


            DataTable dt = Queries.GetResultsFromQueryString("select MAX(ID) + 1 from [PAYOUTtrainer]");

            decimal OUTresult = 0;
            if (!decimal.TryParse(Salary, out OUTresult))
                Salary = OUTresult.ToString("F2");

            string SQLstring = string.Format(@"INSERT INTO [PAYOUTtrainer] (ID, Firstname, Lastname, EmailAddress, Active, Role, Salary) 
                                VALUES ({0}, '{1}', '{2}', '{3}', {4}, '{5}', {6})", dt.Rows[0][0].ToString(), Firstname, lastname, Email, Convert.ToInt32(Active), Role, Salary);

            Queries.ExecuteFromQueryString(SQLstring);

            SQLstring = string.Format(@"INSERT INTO [PAYOUTtrainerManager] ([TrainerId], [TrainManagerId]) VALUES ({0}, (SELECT TOP 1 Id 
                                                                                                                        FROM [PAYOUTtrainManager] 
                                                                                                                        WHERE FirstName + ' ' + LastName = '{1}'))", 
                                      dt.Rows[0][0].ToString(), 
                                      Manager);

            Queries.ExecuteFromQueryString(SQLstring);

            Response.Redirect("Trainer.aspx", true);
        }
        

       

        protected void searchBTN_Click(object sender, EventArgs e)
        {
            GridView1.DataBind();
        }

     /*   protected void exportBtn_Click(object sender, EventArgs e)
        {
            GridView2.AllowPaging = false;
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
        */
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
            int delRow = gvr.RowIndex;

            string SQLstring = string.Empty;

            SQLstring = string.Format(@"SELECT COUNT(*) FROM[PAYOUTtrainerManager] WHERE TrainerId = {0}", GridView1.Rows[rowindex].Cells[0].Text);
            DataTable dt = Queries.GetResultsFromQueryString(SQLstring);

            if (dt.Rows[0][0].ToString() == "0")
            {
                SQLstring = "DELETE FROM [PAYOUTtrainerManager] WHERE [TrainerId] = " + GridView1.Rows[delRow].Cells[0].Text;
                Queries.ExecuteFromQueryString(SQLstring);
            }

            SQLstring = "DELETE FROM [PAYOUTtrainer] WHERE [Id] = " + GridView1.Rows[delRow].Cells[0].Text;
            Queries.ExecuteFromQueryString(SQLstring);
            

            GridView1.DataBind();
        }
    }
}