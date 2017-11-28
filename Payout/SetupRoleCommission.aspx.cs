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
    public partial class SetupRoleCommission : System.Web.UI.Page
    {
        string QueryString = string.Empty;
        string user = string.Empty;
        string userType = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            string user = string.Empty;

            try
            {
                user = Session["user"].ToString();
                userType = Session["userType"].ToString();

                if (!IsPostBack)
                {
                    DataTable dt;
                    Session["storeName"] = string.Empty;
                    Session["program"] = string.Empty;
                    Session["role"] = string.Empty;
                    Session["ResultTable"] = null;
                    Session["ExportTable"] = new DataTable();

                    dt = (DataTable)Session["ResultTable"];
                    Session["ExportTable"] = (DataTable)Session["ResultTable"];
                }

                Dictionary<string, string> SQLParameters = new Dictionary<string, string>();
                fillUCSessionVariables(ref SQLParameters);
                BindDDL(SQLParameters);
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }
        }
        
        
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Session["SP_Parameters"] != null &&
                Session["UC_GridPagesize_BtnHit"] != null &&
                (bool)Session["UC_GridPagesize_BtnHit"])
            {
                Session["UC_GridPagesize_BtnHit"] = false;
                Dictionary<string, string> currentFilters = new Dictionary<string, string>((Dictionary<string, string>)Session["SP_Parameters"]);

                storeDDL.SelectedValue = currentFilters["storeDDL"] != "NULL" ? currentFilters["storeDDL"] : "ALL";
                programDDL.SelectedValue = currentFilters["programDDL"] != "NULL" ? currentFilters["programDDL"] : "ALL"; ;
                roleDDL.SelectedValue = currentFilters["roleDDL"] != "NULL" ? currentFilters["roleDDL"] : "ALL"; ;
            }
        }



        private void BindDDL(Dictionary<string, string> SQLParameters)
        {                        
            /* Add DropDownLists*/
            string SQLString = @"SELECT StoreName FROM [PAYOUTcommissions] WHERE StoreName <> '' GROUP BY StoreName ORDER BY StoreName";
            DataTable dtAdd = Queries.GetResultsFromQueryString(SQLString);
            sstoreAddDDL.DataSource = dtAdd;
            sstoreAddDDL.DataTextField = "StoreName";
            sstoreAddDDL.DataBind();

            SQLString = @"SELECT Program FROM [PAYOUTcommissions] WHERE Program <> '' GROUP BY Program ORDER BY Program";
            dtAdd = Queries.GetResultsFromQueryString(SQLString);
            programAddDDL.DataSource = dtAdd;
            programAddDDL.DataTextField = "Program";
            programAddDDL.DataBind();

            if(userType != "Trainer")
                SQLString = @"SELECT [ID] ,[RoleDescription] FROM [Payout].[dbo].[PAYOUTtrainerRoles]";
            else
                SQLString = string.Format(@"  SELECT r.[ID] ,r.[RoleDescription]
                                              FROM [Payout].[dbo].[PAYOUTtrainerRoles] r
                                              INNER JOIN PAYOUTtrainer t ON r.RoleDescription = t.[Role]
                                              WHERE EmailAddress = '{0}'", user);

            dtAdd = Queries.GetResultsFromQueryString(SQLString);
            roleAddDDL.DataSource = dtAdd;
            roleAddDDL.DataTextField = "RoleDescription";
            roleAddDDL.DataValueField = "ID";
            roleAddDDL.DataBind();


            /* Filtered DropDownLists*/

            SQLParameters["Action"] = "SELECTDDL";
            SQLParameters["PageIndex"] = "NULL";
            SQLParameters["PageSize"] = "NULL";            
            DataTable dt = Queries.GetResultsFromStoreProcedure("spx_PAYOUTSetupCommission", ref SQLParameters).Tables[0];

            Common.BindDDLFromTableResults(dt, "Program", ref programDDL, null);
            Common.BindDDLFromTableResults(dt, "StoreName", ref storeDDL, null);
            Common.BindDDLFromTableResults(dt, "Role", ref roleDDL, null);


            /* -----------------------------------------------------------------------------  */

            //DataRow row = dt.NewRow();
            //row["RoleId"] = 0;
            //dt.Rows.Add(row);

            //var lstFilteredDT = dt.AsEnumerable()
            //    .Select(t =>
            //    new
            //    {
            //        Id = t.Field<string>("RoleId"),
            //        DisplayName = t.Field<string>("RoleDescription")
            //    }
            //    ).Distinct()                
            //    .ToList();

            //roleDDL.DataSource = lstFilteredDT;
            //roleDDL.DataTextField = "RoleDescription";
            //roleDDL.DataTextField = "RoleId";
            //roleDDL.DataBind();


            /* -----------------------------------------------------------------------------  */





            //List<string> lstFilteredDT = new List<string>();
            //lstFilteredDT = dt.AsEnumerable()
            //    .Select(t => t.Field<string>("Role")).ToList();
            //lstFilteredDT = lstFilteredDT.Distinct().ToList();

            //if (userType != "Trainer")
            //    SQLString = string.Format(@"SELECT [ID] ,[RoleDescription] FROM [Payout].[dbo].[PAYOUTtrainerRoles] WHERE [RoleDescription] IN ({0}) ORDER BY [RoleDescription]"
            //                                    , "'" + string.Join("','", lstFilteredDT) + "'");
            //else
            //    SQLString = string.Format(@"  SELECT r.[ID] ,r.[RoleDescription]
            //                                  FROM [Payout].[dbo].[PAYOUTtrainerRoles] r
            //                                  INNER JOIN PAYOUTtrainer t ON r.RoleDescription = t.[Role]
            //                                  WHERE EmailAddress = '{0}'", user);

            //DataTable dt1 = Queries.GetResultsFromQueryString(SQLString);      

            //DataRow dr = dt1.NewRow();
            //dr["RoleDescription"] = "All";
            //dt1.Rows.InsertAt(dr, 0);

            //roleDDL.DataSource = dt1;
            //roleDDL.DataTextField = "RoleDescription";
            //roleDDL.DataValueField = "ID";
            //roleDDL.DataBind();

        }

        //private void BindGridData()
        //{
        //    bool isFirstFilter = true;
        //    string SqlString;
        //    if (userType != "Trainer")
        //        SqlString = @"SELECT TOP 10 rc.[Id],rc.[StoreName], rc.[Program], r.[RoleDescription] [Role], rc.[StandardComm], rc.[EffectiveDate] 
        //                        FROM [PAYOUTRoleCommission] rc
        //                        INNER JOIN PAYOUTtrainerRoles r ON rc.RoleId = r.ID";
        //    else
        //        SqlString = string.Format( @"SELECT rc.[Id],rc.[StoreName], rc.[Program], r.[RoleDescription] [Role], rc.[StandardComm], rc.[EffectiveDate]
        //                                    FROM[PAYOUTRoleCommission] rc
        //                                    INNER JOIN PAYOUTtrainerRoles r ON rc.RoleId = r.ID
        //                                    INNER JOIN PAYOUTtrainer t ON t.[Role] = r.RoleDescription
        //                                    WHERE EmailAddress = '{0}'", user);



        //    SqlString += AddFilterQuery("rc.StoreName", storeDDL, ref isFirstFilter);
        //    SqlString += AddFilterQuery("rc.Program", programDDL, ref isFirstFilter);
        //    SqlString += AddFilterQuery("r.Id", roleDDL, ref isFirstFilter);

        //    DataTable dt = Queries.GetResultsFromQueryString(SqlString);
        //    Session["ResultTable"] = dt;            

        //    //GridView1.DataSource = dt;                        
        //    //GridView1.DataBind();

        //}

        

        private void fillUCSessionVariables(ref Dictionary<string, string> SQLParameters)
        {   
            Common.filtersFromPageDDLs(ref SQLParameters, Page);
            SQLParameters.Add("Action", "SELECTGRID");

            if (userType != "Trainer")
                SQLParameters.Add("webEmailAddress", "NULL");
            else
                SQLParameters.Add("webEmailAddress", user);
                        
            Session["SP_Parameters"] = new Dictionary<string, string>(SQLParameters);
            Session["SP_Name"] = "spx_PAYOUTSetupCommission";
            
            List<ToolClasses.UCGridColumnDetails> lstUCGridColumnDetails = new List<ToolClasses.UCGridColumnDetails>();
            lstUCGridColumnDetails.Add(
                new ToolClasses.UCGridColumnDetails(){ columnName = "StoreName", columnDisplayName = "Store Name", columnWidth = "10%" });
            lstUCGridColumnDetails.Add(
                new ToolClasses.UCGridColumnDetails() { columnName = "Program", columnDisplayName = "Program", columnWidth = "10%" });
            lstUCGridColumnDetails.Add(
                new ToolClasses.UCGridColumnDetails() { columnName = "Role", columnDisplayName = "Role", columnWidth = "10%" });
            lstUCGridColumnDetails.Add(
                new ToolClasses.UCGridColumnDetails() { columnName = "StandardComm", columnDisplayName = "Standard Commission", columnWidth = "10%" });
            lstUCGridColumnDetails.Add(
                new ToolClasses.UCGridColumnDetails() { columnName = "EffectiveDate", columnDisplayName = "Effective Date", columnWidth = "10%" });
                        
            Session["lstUCGridColumnDetails"] = lstUCGridColumnDetails;
            Session["ShowFixedHeader"] = true;
        }


        private string AddFilterQuery(string FieldName, DropDownList DDL, ref bool isFirstFilter)
        {
            string SqlString = string.Empty;

            if (DDL.SelectedValue.ToLower() != "all" && DDL.SelectedValue.ToLower() != "")
            {
                if (isFirstFilter)
                {
                    SqlString += " WHERE ";
                    isFirstFilter = false;
                }
                else
                    SqlString += " AND ";

                SqlString += string.Format(FieldName + " = '{0}'", DDL.SelectedValue.ToLower());
            }

            return SqlString;
        }
        
        //protected void searchBTN_Click(object sender, EventArgs e)
        //{
        //    Session["storeName"] = storeDDL.SelectedValue;
        //    Session["program"] = programDDL.SelectedValue;
        //    Session["role"] = roleDDL.SelectedValue;            
        //    BindGridData();
        //}
        
        //protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    string FieldName = string.Empty;

        //    for (int j = 0; j < e.Row.Cells.Count; j++)
        //    {
        //        FieldName = ((BoundField)((DataControlFieldCell)(e.Row.Cells[j])).ContainingField).DataField.ToLower();

        //        if (FieldName.Contains("id"))
        //        {
        //            e.Row.Cells[j].CssClass = "HideCellVisibility";                    
        //        }
        //    }            
        //}

        protected void DropDownChanged(object sender, EventArgs e)
        {
            //BindGridData();

            if (Session["SP_Parameters"] != null)
            {
                Dictionary<string, string> currentFilters = new Dictionary<string, string>((Dictionary<string, string>)Session["SP_Parameters"]);

                storeDDL.SelectedValue = currentFilters["storeDDL"];
                programDDL.SelectedValue = currentFilters["programDDL"];
                roleDDL.SelectedValue = currentFilters["roleDDL"];
            }

        }

        protected void Apply_Click(object sender, EventArgs e)
        {
            string SQLstring;
            if (HiddenIdTextBox.Text != "")
            {
                SQLstring = string.Format(@"UPDATE PAYOUTRoleCommission
                                            SET StoreName = '{0}', Program = '{1}', RoleId = {2}, StandardComm = {3}
                                            WHERE Id = {4}",
                                            sstoreAddDDL.SelectedValue,
                                            programAddDDL.SelectedValue,
                                            roleAddDDL.SelectedValue,
                                            CommissionTxt.Text == "" ? "NULL" :CommissionTxt.Text,
                                            HiddenIdTextBox.Text);
            }
            else
            {
                SQLstring = string.Format(@"INSERT INTO PAYOUTRoleCommission (StoreName, Program, RoleId, StandardComm, EffectiveDate) 
                                            VALUES ('{0}','{1}',{2},{3},'{4}')",
                                            sstoreAddDDL.SelectedValue,
                                            programAddDDL.SelectedValue,
                                            roleAddDDL.SelectedValue,
                                            CommissionTxt.Text == "" ? "NULL" : CommissionTxt.Text,
                                            lblEffectiveDate.Text == "" ? Common.ApplyDateFormat(DateTime.Now) : lblEffectiveDate.Text);

            }
            

            Queries.ExecuteFromQueryString(SQLstring);
            //BindGridData();
        }
    }
}
