using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication3;

namespace Payout
{
    public partial class AccountManager : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            bindGrids();
        }

        private void bindGrids()
        {
            string SQLstring = @" SELECT 0 [Id], [FirstName],[LastName],[Email],[IsOkta], u.[IsDisabled], r.[Name] [Role]
                                  FROM AspNetUsers u
                                  LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                                  LEFT JOIN AspNetRoles r ON r.Id = ur.RoleId
                                    ORDER BY [FirstName]";

            GRVUserList.DataSource = Queries.GetResultsFromQueryString(SQLstring);
            GRVUserList.DataBind();


            SQLstring = "SELECT 0 [Id], [Name], [IsDisabled] FROM AspNetRoles";
            GRVRoleList.DataSource = Queries.GetResultsFromQueryString(SQLstring);
            GRVRoleList.DataBind();
        }

        private string getRowId(GridViewRow gvr)
        {
            return ((Label)gvr.Cells[0].Controls[1]).Text;
        }


        public override void VerifyRenderingInServerForm(Control control)
        {
            //
        }

        protected void GRVUserList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView drv = e.Row.DataItem as DataRowView;
                DropDownList ddlRole = e.Row.FindControl("DropDownRole") as DropDownList;

                ddlRole.DataSource = Queries.GetResultsFromQueryString("SELECT '- Select Role -' Role UNION ALL SELECT [NAME] Role FROM AspNetRoles");
                ddlRole.DataTextField = "Role";
                ddlRole.DataValueField = "Role";
                ddlRole.DataBind();

                if (ddlRole != null)
                {
                    //Get the data from DB and bind the dropdownlist
                    ddlRole.SelectedValue = drv["Role"].ToString();
                }
            }

        }


        [WebMethod()]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string JSONRequestApiUrlFromClient()
        {
            return ConfigurationManager.AppSettings["ServerAPIURL"];
        }
    }
}