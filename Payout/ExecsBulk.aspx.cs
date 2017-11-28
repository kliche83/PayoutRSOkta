using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Payout
{
    public partial class ExecsBulk : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string user = string.Empty;

            try
            {
                user = Session["user"].ToString();
                FilterPersonTXT.Attributes.Add("placeholder", "Executive Name");
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }

            if (!IsPostBack)
            {
                progDDL.SelectedIndex = 0;
            }
        }

        protected void backBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("Grids.aspx", true);
        }

        protected void searchBtn_Click(object sender, EventArgs e)
        {
            DataTable dt;
            Dictionary<string, string> Params = new Dictionary<string, string>();
            Params.Add("WebPersonFilter", FilterPersonTXT.Text);
            Params.Add("WebExecOf", progDDL.SelectedValue == "All" ? "" : progDDL.SelectedValue);

            if (progDDL.SelectedValue != "All")
            {
                Params.Add("Action", "SELECT");
                dt = WebApplication3.Queries.GetResultsFromStoreProcedure("spx_PAYOUTExecs", ref Params).Tables[0];
            }
            else
            {
                Params.Add("Action", "SELECTALL");
                dt = WebApplication3.Queries.GetResultsFromStoreProcedure("spx_PAYOUTExecs", ref Params).Tables[0];
            }

            gvOwners.DataSource = dt;
            gvOwners.DataBind();
        }

        protected void SQLSelecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            e.Command.CommandTimeout = 3600;
        }

        protected void GridDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.Pager)
                {
                    e.Row.Cells[0].Visible = false;
                    e.Row.Cells[1].Visible = false;
                }

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chb = (CheckBox)e.Row.Cells[3].FindControl("isExec");

                    if (e.Row.Cells[1].Text == "1" || e.Row.Cells[1].Text == "True")
                    {
                        chb.Checked = true;
                        e.Row.Cells[3].Attributes.Add("style", "font-weight: bold;");
                    }
                }
            }
            catch { }
        }

        protected void CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chb = (CheckBox)sender;
            GridViewRow gvr = (GridViewRow)chb.NamingContainer;
            int rowindex = gvr.RowIndex;
            string program = progDDL.SelectedValue;
            Dictionary<string, string> Params = new Dictionary<string, string>();
            Params.Add("WebPersonId", gvOwners.Rows[rowindex].Cells[0].Text);
            Params.Add("WebExecOf", program);

            if (chb.Checked)
            {
                if (program == "All")
                {
                    Params.Add("Action", "CHECKALL");
                    WebApplication3.Queries.ExecuteFromStoreProcedure("spx_PAYOUTExecs", Params);
                }
                else
                {
                    Params.Add("Action", "CHECKNONALL");
                    WebApplication3.Queries.ExecuteFromStoreProcedure("spx_PAYOUTExecs", Params);
                }
            }
            else
            {

                if (program == "All")
                {
                    Params.Add("Action", "UNCHECKALL");
                    WebApplication3.Queries.ExecuteFromStoreProcedure("spx_PAYOUTExecs", Params);
                }
                else
                {
                    Params.Add("Action", "UNCHECKNONALL");
                    WebApplication3.Queries.ExecuteFromStoreProcedure("spx_PAYOUTExecs", Params);
                }
            }

            Params["Action"] = "CLEANUP";
            WebApplication3.Queries.ExecuteFromStoreProcedure("spx_PAYOUTExecs", Params);
        }

        protected void overviewBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("Execs.aspx", true);
        }

    }
}