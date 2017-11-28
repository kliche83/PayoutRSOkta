using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication3
{
    public partial class SetupRoleOverride : System.Web.UI.Page
    {
        string QueryString = string.Empty;
        string user = string.Empty;
        string userType = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    DataTable dt;
                    Session["ResultTable"] = null;
                    Session["ExportTable"] = new DataTable();
                    user = Session["user"].ToString();
                    userType = Session["userType"].ToString();

                    BindGridData();

                    dt = (DataTable)Session["ResultTable"];
                    Session["ExportTable"] = (DataTable)Session["ResultTable"];

                    BindDDL(dt);
                }
                catch
                {
                    Response.Write("<script>window.top.location = '../';</script>");
                }                
            }            
        }

        private void BindDDL(DataTable dt)
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
                SQLString = @"SELECT Id, Firstname + ' ' + Lastname [name] FROM PAYOUTtrainer";
            else
                SQLString = string.Format(@"SELECT Id, Firstname + ' ' + Lastname [name] FROM PAYOUTtrainer WHERE EmailAddress = '{0}'", user);
            dtAdd = Queries.GetResultsFromQueryString(SQLString);
            trainerAddDDL.DataSource = dtAdd;
            trainerAddDDL.DataTextField = "name";
            trainerAddDDL.DataValueField = "ID";
            trainerAddDDL.DataBind();


            /* Filtered DropDownLists*/
            Common.BindDDLFromTableResults(dt, "Program", ref programDDL);            
            Common.BindDDLFromTableResults(dt, "StoreName", ref sstoreDDL);            

            List<string> lstFilteredDT = new List<string>();
            lstFilteredDT = dt.AsEnumerable()
                .Select(t => t.Field<string>("Employee")).ToList();
            lstFilteredDT = lstFilteredDT.Distinct().ToList();

            if (userType != "Trainer")
                SQLString = string.Format(@"SELECT Id, Firstname + ' ' + Lastname [name] FROM PAYOUTtrainer WHERE [Firstname] + ' ' + [Lastname] IN ({0}) ORDER BY [Firstname], [Lastname]"
                                                , "'" + string.Join("','", lstFilteredDT) + "'");
            else
                SQLString = string.Format(@"SELECT Id, Firstname + ' ' + Lastname [name] FROM PAYOUTtrainer WHERE EmailAddress = '{0}'", user);


            DataTable dt1 = Queries.GetResultsFromQueryString(SQLString);
            DataRow dr = dt1.NewRow();
            dr["name"] = "All";
            dt1.Rows.InsertAt(dr, 0);


            trainerDDL.DataSource = dt1;
            trainerDDL.DataTextField = "name";
            trainerDDL.DataValueField = "ID";
            trainerDDL.DataBind();         
        }
        
        private void BindGridData()
        {
            //updateGrid.Update();
            //UpdatePanelTopWrapper.Update();
            bool isFirstFilter = true;

            string SqlString;
            if (userType != "Trainer")
                SqlString = @"SELECT o.[Id],t.[Firstname] + ' ' + t.[Lastname] [Employee],o.[StoreName],o.[Program],o.[WeeklyCompensation],o.[Override],convert(varchar(10), o.[EffectiveDate], 23) [EffectiveDate]
                                FROM [PAYOUTOverridesPM] o
                                INNER JOIN PAYOUTtrainer t ON o.TrainerId = t.ID";
            else
                SqlString = string.Format(@"SELECT o.[Id],t.[Firstname] + ' ' + t.[Lastname] [Employee],o.[StoreName],o.[Program],o.[WeeklyCompensation],o.[Override],convert(varchar(10), o.[EffectiveDate], 23) [EffectiveDate]
                                            FROM [PAYOUTOverridesPM] o
                                            INNER JOIN PAYOUTtrainer t ON o.TrainerId = t.ID
                                            WHERE t.EmailAddress = '{0}'", user);

            SqlString += AddFilterQuery("o.StoreName", sstoreDDL, ref isFirstFilter);
            SqlString += AddFilterQuery("o.Program", programDDL, ref isFirstFilter);
            SqlString += AddFilterQuery("t.Id", trainerDDL, ref isFirstFilter);

            DataTable dt = Queries.GetResultsFromQueryString(SqlString);
            Session["ResultTable"] = dt;            

            GridView1.DataSource = dt;                        
            GridView1.DataBind();

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
        
        protected void searchBTN_Click(object sender, EventArgs e)
        {        
            BindGridData();
        }
        

        protected void DropDownChanged(object sender, EventArgs e)
        {
            BindGridData();
        }

        protected void Apply_Click(object sender, EventArgs e)
        {
            decimal weeklycompensation = -1, Override = -1;
            DateTime EffDate = new DateTime();

            decimal.TryParse(WeeklyCompensationTxt.Text, out weeklycompensation);
            decimal.TryParse(OverrideTxt.Text, out Override);
            DateTime.TryParse(EffectiveDate.Text, out EffDate);

            if (weeklycompensation == 0 || Override == 0 || EffDate.Year == 1)
            {
                WeeklyCompensationTxt.BackColor = Color.White;
                OverrideTxt.BackColor = Color.White;
                EffectiveDate.BackColor = Color.White;
                ValidateCompensation.InnerText = "";
                ValidateOverride.InnerText = "";
                ValidateEffectiveDate.InnerText = "";


                UpdatePanelUpperBox.Update();
                if (weeklycompensation == 0)
                {
                    ValidateCompensation.InnerText = "Please enter a valid value";
                    WeeklyCompensationTxt.BackColor = Color.Salmon;
                }

                if (Override == 0)
                {
                    ValidateOverride.InnerText = "Please enter a valid value";
                    OverrideTxt.BackColor = Color.Salmon;
                }

                if (EffDate.Year == 1 )
                {
                    ValidateEffectiveDate.InnerText = "Please enter a valid value";
                    EffectiveDate.BackColor = Color.Salmon;
                }
            }
            else
            {
                UpdatePanelTopWrapper.Update();
                UpdatePanelUpperBox.Update();

                string SQLstring;
                if (HiddenIdTextBox.Text != "")
                {
                    SQLstring = string.Format(@"UPDATE PAYOUTOverridesPM
                                                SET TrainerId = {0}, StoreName = '{1}', Program = '{2}', WeeklyCompensation = {3}, Override = {4}
                                                WHERE Id = {5}",
                                                trainerAddDDL.SelectedValue,
                                                sstoreAddDDL.SelectedValue,
                                                programAddDDL.SelectedValue,
                                                weeklycompensation,
                                                Override,
                                                HiddenIdTextBox.Text);
                }
                else
                {
                    SQLstring = string.Format(@"INSERT INTO PAYOUTOverridesPM (TrainerId, StoreName, Program, WeeklyCompensation, Override, EffectiveDate) 
                                                VALUES ({0},'{1}','{2}',{3},{4},'{5}')",
                                                trainerAddDDL.SelectedValue,
                                                sstoreAddDDL.SelectedValue,
                                                programAddDDL.SelectedValue,
                                                weeklycompensation,
                                                Override,
                                                EffectiveDate.Text);

                }


                Queries.ExecuteFromQueryString(SQLstring);
                Response.Redirect(Request.RawUrl);
                //BindGridData();
            }
        }


        protected void updateGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string FieldName = string.Empty;

            if (e.Row.Cells.Count > 1)
            {
                for (int j = 0; j < e.Row.Cells.Count; j++)
                {
                    FieldName = ((BoundField)((DataControlFieldCell)(e.Row.Cells[j])).ContainingField).DataField.ToLower();

                    if (FieldName.Contains("Id"))
                    {
                        e.Row.Cells[j].CssClass = "HideCellVisibility";
                    }
                }
            }            
        }
    }
}
