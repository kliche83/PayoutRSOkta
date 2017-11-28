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
    public partial class TrainerManager : System.Web.UI.Page
    {
        string user = string.Empty;
        int rowindex;
      

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                user = Session["user"].ToString();

                SqlDataSource1.SelectCommand =
                "SELECT distinct [ID],[Firstname],[Lastname],[Active] FROM [PAYOUTtrainManager]";
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
                DropDownList ddlTrainer= e.Row.FindControl("DDLActiveManager") as DropDownList;
                if (ddlTrainer != null)
                {
                    //Get the data from DB and bind the dropdownlist
                    ddlTrainer.SelectedValue = drv["active"].ToString();
                }                
            }
        }        
                
        protected void DropDownChanged(Object sender, EventArgs e)
        {
            DropDownList DDL = (DropDownList)sender;
            GridViewRow gvr = (GridViewRow)DDL.NamingContainer;
            string DDLValue = string.Empty;
            
            if (DDL.DataValueField.ToLower() == "active")
                DDLValue = DDL.SelectedValue.ToLower().Replace("true", "1").Replace("false", "0");

            rowindex = gvr.RowIndex;

            string SQLstring = string.Format("UPDATE [PAYOUTtrainManager] SET {0} = {1} WHERE [Id] = '{2}'",
                    DDL.DataValueField.ToLower(),
                    DDLValue,
                    GridView1.Rows[rowindex].Cells[0].Text);

            Queries.ExecuteFromQueryString(SQLstring);
            
        }

        protected void addBtn_Click(object sender, EventArgs e)
        {
            string Firstname = Request.Form["TxtFName"].ToString();
            string lastname = Request.Form["TxtLname"].ToString();

            bool Active = true;
              if (CHKActive.Checked)

                  Active = true;
              else
                  Active = false;

            DataTable dt = Queries.GetResultsFromQueryString("select MAX(ID) + 1 from [PAYOUTtrainManager]");

            string SQLstring = string.Format(@"INSERT INTO [PAYOUTtrainManager] (Firstname, Lastname, Active) 
                                VALUES ('{0}', '{1}', {2})", Firstname, lastname, Convert.ToInt32(Active));
            Queries.ExecuteFromQueryString(SQLstring);

            Response.Redirect("TrainerManager.aspx", true);
        }
        
        protected void delLink_Click(object sender, EventArgs e)
        {
            LinkButton del = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)del.NamingContainer;
            int delRow = gvr.RowIndex;

            //Cascade deleting
            string SQLstring = string.Format("DELETE FROM [PAYOUTtrainerManager] WHERE TrainManagerId = {0}", GridView1.Rows[delRow].Cells[0].Text);
            Queries.ExecuteFromQueryString(SQLstring);

            SQLstring = "DELETE FROM [PAYOUTtrainManager] WHERE [Id] = '" + GridView1.Rows[delRow].Cells[0].Text + "'";
            Queries.ExecuteFromQueryString(SQLstring);

            GridView1.DataBind();
        }
    }
}