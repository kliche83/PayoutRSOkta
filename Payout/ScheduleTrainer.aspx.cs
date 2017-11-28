using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication3
{
    public partial class ScheduleTrainer : System.Web.UI.Page
    {
        string user = string.Empty;
        string userType = string.Empty;
        string userFullname = string.Empty;
        string SQLtable = string.Empty;
        string where = string.Empty;        
        Dictionary<string, string> SQLParameters;
        List<string> ControlColumns = new List<string>();
        DataTable dtTrainerResults = new DataTable();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            
            try
            {
                user = Session["user"].ToString();
                userType = Session["userType"].ToString();
                userFullname = Session["userFullname"].ToString();
                Session["SelectedTrainerId"] = trainerDDL_Ins.SelectedValue;

                if (!IsPostBack)
                {
                    Session["trainerName"] = string.Empty;
                    Session["trainerStartDate"] = string.Empty;
                    Session["trainerEndDate"] = string.Empty;

                    ControlColumns.Add("trainer");
                    ControlColumns.Add("start train");
                    ControlColumns.Add("end train");
                    ControlColumns.Add("action");
                }
                
                TrainerTableResults();

                if (userType != "Trainer")
                {
                    Logout.Visible = false;
                }                    
                else
                {
                    ManagerBtn.Visible = false;
                    SetupCommissionBtn.Visible = false;
                    SetupOverrideBtn.Visible = false;
                }



                if ((string)Session["trainerStartDate"] != string.Empty)
                {
                    dateFrom.Text = (string)Session["trainerStartDate"];
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(dateFrom.Text))
                        dateFrom.Text = Common.ApplyDateFormat(DateTime.Now.AddMonths(-2), 1);
                }

                if ((string)Session["trainerEndDate"] != string.Empty)
                {
                    dateTo.Text = (string)Session["trainerEndDate"];
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(dateTo.Text))
                        dateTo.Text = Common.ApplyDateFormat(DateTime.Now, 1);
                }

                BindGridData();

            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }
        }


        private void BindGridData()
        {            
            SQLParameters = new Dictionary<string, string>();
            SQLParameters.Add("@webStartDate", dateFrom.Text);
            SQLParameters.Add("@webEndDate", dateTo.Text);
            SQLParameters.Add("@webProgram", programDDL.SelectedValue);
            SQLParameters.Add("@webStoreName", sstoreDDL.SelectedValue);
            SQLParameters.Add("@webStoreNumber", StoreNumberTXT.Text);
            SQLParameters.Add("@webOwner", ownerTXT.Text);
            SQLParameters.Add("@Option", "ScheduleTrainer");
            SQLParameters.Add("@webTrainerId", DDTrainer.SelectedValue == "0" ? "" : DDTrainer.SelectedValue);

            DataTable dt = Queries.GetResultsFromStoreProcedure("[spx_PAYOUTschedule]", ref SQLParameters).Tables[0];

            ProfileScreenSettings(ref dt);

            GridView1.DataSource = dt;
            GridView1.DataBind();

            GridView2.DataSource = dt;
            GridView2.DataBind();

            BindDDLs();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType != DataControlRowType.Pager)
            //{
            //    e.Row.Cells[0].Visible = false;
            //}
                        
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView drv = e.Row.DataItem as DataRowView;
                DropDownList ddlTrainer= e.Row.FindControl("DropDownTrainer") as DropDownList;

                ddlTrainer.DataSource = dtTrainerResults;
                ddlTrainer.DataTextField = "trainer";
                ddlTrainer.DataValueField = "ID";
                ddlTrainer.DataBind();

                if (ddlTrainer != null)
                {
                    //Get the data from DB and bind the dropdownlist
                    ddlTrainer.SelectedValue = drv["trainerid"].ToString();                    
                }
            }

            string FieldName = string.Empty;

            if (e.Row.Cells.Count > 1)
            {
                for (int j = 0; j < e.Row.Cells.Count; j++)
                {
                    if (!ControlColumns.Contains(((DataControlFieldCell)(e.Row.Cells[j])).ContainingField.HeaderText.ToLower()))
                    {

                        FieldName = (((DataControlFieldCell)(e.Row.Cells[j])).ContainingField).HeaderText.ToLower();

                        //FieldName = ((BoundField)((DataControlFieldCell)(e.Row.Cells[j])).ContainingField).DataField.ToLower();

                        if (FieldName.Contains("scheduleTrainerId"))
                            e.Row.Cells[j].CssClass = "HideCellVisibility";

                        if (FieldName.Contains("ScheduleId"))
                            e.Row.Cells[j].CssClass = "HideCellVisibility";

                        if (FieldName.Contains("start show") || FieldName.Contains("end show"))
                        {
                            if (e.Row.RowType == DataControlRowType.DataRow)
                            {
                                e.Row.Cells[j].Text = Common.ApplyDateFormat(e.Row.Cells[j].Text);
                            }
                        }
                    }                    
                }
            }
        }


        protected void BtnValidate_Click(object sender, EventArgs e)
        {
            GridViewRow gvr = (GridViewRow)((Control)sender).Parent.Parent;
            
            DateTime? ScheduleStartDate = null, ScheduleEndDate = null, TrainStartDate = null, TrainEndDate = null;
            string trainerId = null;
            
            for (int i = 0; i < gvr.Cells.Count; i++)
            {
                if (gvr.Cells[i].Controls.Count > 0)
                {
                    switch (gvr.Cells[i].Controls[1].GetType().Name)
                    {
                        case "TextBox":
                            switch ((gvr.Cells[i].Controls[1]).ID)
                            {
                                case "TrainStartDate":
                                    if (!string.IsNullOrWhiteSpace(((TextBox)gvr.Cells[i].Controls[1]).Text))
                                        TrainStartDate = Convert.ToDateTime(((TextBox)gvr.Cells[i].Controls[1]).Text);
                                    break;

                                case "TrainEndDate":
                                    if(!string.IsNullOrWhiteSpace(((TextBox)gvr.Cells[i].Controls[1]).Text))
                                        TrainEndDate = Convert.ToDateTime(((TextBox)gvr.Cells[i].Controls[1]).Text);
                                    break;
                            }
                            break;
                        case "DropDownList":
                            if ((gvr.Cells[i].Controls[1]).ID == "DropDownTrainer")
                            {
                                trainerId = ((DropDownList)gvr.Cells[i].Controls[1]).SelectedValue;
                            }
                            break;
                    }
                }
                else
                {
                    switch (((BoundField)(((DataControlFieldCell)(gvr.Cells[i])).ContainingField)).DataField)
                    {
                        case "StartDate":
                            //ScheduleStartDate = Convert.ToDateTime(((TextBox)gvr.Cells[i].Controls[1]).Text);
                            ScheduleStartDate = Convert.ToDateTime((gvr.Cells[i]).Text);
                            break;


                        case "EndDate":
                            ScheduleEndDate = Convert.ToDateTime((gvr.Cells[i]).Text);
                            break;                            
                    }
                }
            }

            EvaluateDateEntry(ref TrainStartDate, ref TrainEndDate, ref ScheduleStartDate, ref ScheduleEndDate);

            bool TrainerExistInDateRange = false;
            if (TrainStartDate != null)
                TrainerExistInDateRange = ValidateSameDaySchedule(TrainStartDate.GetValueOrDefault(), TrainEndDate.GetValueOrDefault(), trainerId, gvr.Cells[0].Text);

            if (TrainerExistInDateRange)
            {
                if (TrainStartDate == null || TrainEndDate == null)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(),
                      "err_msg",
                      "alert('Please select a valid range of dates');",
                      true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(),
                      "err_msg",
                      "alert('Trainer exists in the same schedule');",
                      true);
                }
                
            }
            else
            {
                if (TrainStartDate == null || TrainEndDate == null)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(),
                      "err_msg",
                      "alert('Please select a valid range of dates');",
                      true);
                }
                else
                {
                    int TransactionResult = 0;
                    if (gvr.Cells[0].Text == "&nbsp;") //Cells[0] = ScheduleTrainerId                    
                    {
                        //Run Insert method no matters if we are inside update method (BtnValidate_Click)
                        TransactionResult = InsertRecord(gvr.Cells[1].Text, trainerId, TrainStartDate.ToString(), TrainEndDate.ToString());
                    }
                    else
                    {
                        TransactionResult = UpdateScheduleRecord(TrainStartDate, TrainEndDate, ScheduleStartDate, ScheduleEndDate, trainerId, gvr.Cells[0].Text);
                    }
                    ValidationMessages(TransactionResult);
                }
                

                BindGridData();
                //Response.Redirect("~/ScheduleTrainer.aspx", true);
            }
        }
        
        private void EvaluateDateEntry(ref DateTime? TrainStartDate, ref DateTime? TrainEndDate, ref DateTime? StartDate, ref DateTime? EndDate)
        {
            if (TrainStartDate != null)
            {
                if (TrainStartDate < StartDate || TrainStartDate > EndDate)
                {
                    TrainStartDate = null;
                }
            }
            
            if (TrainEndDate != null)
            {
                if (TrainEndDate < StartDate || TrainEndDate > EndDate)
                {
                    TrainEndDate = null;
                }
            }

            if (TrainStartDate != null && TrainEndDate != null)
            {
                if (TrainStartDate > TrainEndDate)
                {
                    TrainStartDate = null;
                    TrainEndDate = null;
                }
            }


                     
        }

        private bool ValidateSameDaySchedule(DateTime TrainStartDate, DateTime TrainEndDate, string trainerId, string scheduleTrainerId)
        {
            if (scheduleTrainerId == "&nbsp;")
                return false;

            string TrainerInScheduleStatus = string.Empty;
            string SQLstring = string.Format(@" SELECT COUNT(*)
                                                FROM [PAYOUTschedule] s INNER JOIN
                                                [PAYOUTScheduleTrainer] st ON s.Id = st.ScheduleId
                                                WHERE s.[TrainerID] = '{0}' 
                                                AND (
	                                                ('{1}' BETWEEN s.[TrainStartDate] AND s.[TrainEndDate] AND '{2}' BETWEEN s.[TrainStartDate] AND s.[TrainEndDate])
	                                                OR
	                                                (s.[TrainStartDate] BETWEEN '{1}' AND '{2}')
	                                                OR
	                                                (s.[TrainEndDate] BETWEEN '{1}' AND '{2}')
	                                                )
                                                AND st.Id <> '{3}'", trainerId, Common.ApplyDateFormat(TrainStartDate), Common.ApplyDateFormat(TrainEndDate), scheduleTrainerId);

            DataTable dt = Queries.GetResultsFromQueryString(SQLstring);
            
            if (dt.Rows[0][0].ToString() != "0")
                return true;

            return false;
        }        

        private int UpdateScheduleRecord(DateTime? TrainStartDate, DateTime? TrainEndDate, DateTime? ScheduleStartDate, DateTime? ScheduleEndDate, string trainerId, string scheduleTrainerId)
        {
            if (trainerId != "0)")
            {
                string SQLstring = string.Empty;
                SQLstring = string.Format(@" SELECT COUNT(*)
                                            FROM [PAYOUTScheduleTrainer]
                                            WHERE ([TrainStartDate] BETWEEN '{0}' AND '{1}'
                                            OR [TrainEndDate] BETWEEN '{0}' AND '{1}'
                                            OR ([TrainStartDate] <= '{0}' AND [TrainEndDate] >= '{1}')) 
                                            AND [TrainerId] = {2}", TrainStartDate, TrainEndDate, trainerId);

                try
                {
                    if (Queries.GetResultsFromQueryString(SQLstring).Rows[0][0].ToString() != "0")
                    {
                        SQLstring = "UPDATE [PAYOUTScheduleTrainer] SET";

                        if (TrainStartDate != null && TrainEndDate != null && trainerId != null && trainerId != "0")
                        {
                            SQLstring += string.Format(" {0} = {1},", "TrainStartDate", "'" + TrainStartDate + "'");
                            SQLstring += string.Format(" {0} = {1},", "TrainEndDate", "'" + TrainEndDate + "'");
                            SQLstring += string.Format(" {0} = {1}", "TrainerID", "'" + trainerId + "'");
                        }
                        else
                        {
                            SQLstring += string.Format(" {0} = {1},", "TrainStartDate", "NULL");
                            SQLstring += string.Format(" {0} = {1},", "TrainEndDate", "NULL");
                            SQLstring += string.Format(" {0} = {1}", "TrainerID", "NULL");
                        }

                        SQLstring += " WHERE [Id] = '" + scheduleTrainerId + "' ";
                        Queries.ExecuteFromQueryString(SQLstring);
                        return 1;
                    }
                }
                catch
                {
                    return -1;
                }
            }
            return 0;
        }


        private void BindDDLs()
        {            
            trainerDDL_Ins.DataSource = dtTrainerResults.Copy();
            trainerDDL_Ins.DataTextField = "trainer";
            trainerDDL_Ins.DataValueField = "ID";
            trainerDDL_Ins.DataBind();


            DDTrainer.DataSource = dtTrainerResults.Copy();
            DDTrainer.DataTextField = "trainer";
            DDTrainer.DataValueField = "ID";
            DDTrainer.DataBind();

            string SQLstring = @"SELECT 'All' AS Program 
                                UNION ALL 
                                SELECT * FROM 
                                (
                                    SELECT TOP 1000 Program 
                                    FROM PAYOUTschedule 
                                    WHERE Program != '' AND Program IS NOT NULL
                                    GROUP BY Program 
                                    ORDER BY Program
                                )x";                        

            programDDL.DataSource = Queries.GetResultsFromQueryString(SQLstring);
            programDDL.DataTextField = "Program";
            programDDL.DataValueField = "Program";
            programDDL.DataBind();

            
        }


        private void TrainerTableResults()
        {
            string SQLstring = @"SELECT 0 [ID], 'All' [trainer] UNION ALL SELECT [ID],[Firstname] + ' ' + [Lastname] [trainer] FROM [Payout].[dbo].[PAYOUTtrainer] WHERE [Active] = 1";

            if (userType == "Trainer")
                SQLstring = string.Format("{0} AND EmailAddress = '{1}'", SQLstring, user);

            dtTrainerResults = Queries.GetResultsFromQueryString(SQLstring);
        }

        //private void GetSelectedRowFields(GridViewRow gvr, ref DateTime? TrainStartDate, ref DateTime? TrainEndDate, ref DateTime? ScheduleStartDate, ref DateTime? ScheduleEndDate, ref string trainerId)
        //{
        //    //GridView gv = (GridView)gvr.NamingContainer;
        //    //rowindex = gvr.RowIndex;

        //    //for (int i = 0; i < gv.SelectedRow.Cells.Count; i++)
        //    for (int i = 0; i < gvr.Cells.Count; i++)
        //    {
        //        //string columnName = ((BoundField)((DataControlFieldCell)(gvr.Cells[i])).ContainingField).DataField;
        //        string columnName = ((DataControlFieldCell)(gvr.Cells[i])).ContainingField.HeaderText;


        //        switch (columnName.ToLower().Trim())
        //        {
        //            case "start train date":
        //                if (!string.IsNullOrWhiteSpace(((TextBox)gvr.Cells[i].Controls[1]).Text))
        //                {                            
        //                    TrainStartDate = Convert.ToDateTime(((TextBox)gvr.Cells[i].Controls[1]).Text);
        //                }
        //                //TrainStartDate = Convert.ToDateTime(GridView1.Rows[rowindex].Cells[i].Text);
        //                break;
        //            case "end train date":
        //                if (!string.IsNullOrWhiteSpace(((TextBox)gvr.Cells[i].Controls[1]).Text))
        //                {
        //                    TrainEndDate = Convert.ToDateTime(((TextBox)gvr.Cells[i].Controls[1]).Text);
        //                }
        //                //TrainEndDate = Convert.ToDateTime(GridView1.Rows[rowindex].Cells[i].Text);
        //                break;
        //            case "start date":
        //                if (!string.IsNullOrWhiteSpace(gvr.Cells[i].Text))
        //                {
        //                    ScheduleStartDate = Convert.ToDateTime(gvr.Cells[i].Text);
        //                }
        //                break;
        //            case "end date":
        //                if (!string.IsNullOrWhiteSpace(gvr.Cells[i].Text))
        //                {
        //                    //ScheduleEndDate = Convert.ToDateTime(GridView1.Rows[rowindex].Cells[i].Text);
        //                    ScheduleEndDate = Convert.ToDateTime(gvr.Cells[i].Text);
        //                }                            
        //                break;
        //            case "trainer":
        //                if (!string.IsNullOrWhiteSpace(((DropDownList)gvr.Cells[i].Controls[1]).SelectedValue))
        //                {
        //                    trainerId = ((DropDownList)gvr.Cells[i].Controls[1]).SelectedValue;                            
        //                }
        //                //trainer = GridView1.Rows[rowindex].Cells[i].Text;
        //            break;
        //        }                
        //    }
        //}



        //protected void DropDownChanged(Object sender, EventArgs e)
        //{
        //    DropDownList DDL = (DropDownList)sender;
        //    GridViewRow gvr = (GridViewRow)DDL.NamingContainer;

        //    rowindex = gvr.RowIndex;
        //    using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        //    {
        //        string update = "UPDATE [PAYOUTschedule] SET TrainerID =" + DDL.SelectedValue + " WHERE [Id] = '" + GridView1.Rows[rowindex].Cells[0].Text + "' " /*+ updateR*/;
        //        con.Open();
        //        using (SqlCommand cmd = new SqlCommand(update, con))
        //        {
        //            SqlDataReader reader = cmd.ExecuteReader();
        //        }
        //        con.Close();
        //    }
        //}







        protected void searchBTN_Click(object sender, EventArgs e)
        {
            Session["trainerStartDate"] = dateFrom.Text;
            Session["trainerEndDate"] = dateTo.Text;

            GridView1.DataBind();
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            GridView2.AllowPaging = false;
            GridView2.DataBind();
            string attachment = "attachment; filename=\"RoadShow ScheduleTrainer.xls\"";
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

        protected void Btn_Logout(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("~/", true);
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

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            BindGridData();
        }

        //[WebMethod()]
        //public static int InsertTrainerBtn_Click(int ScheduleId, int TrainerId, string TrainStartDate, string TrainEndDate)
        //{
        //    try
        //    {
        //        if (TrainerId != 0)
        //        {
        //            string SQLString = string.Format(@" SELECT COUNT(*)
        //                                        FROM [PAYOUTScheduleTrainer]
        //                                        WHERE ([TrainStartDate] BETWEEN '{0}' AND '{1}'
        //                                        OR [TrainEndDate] BETWEEN '{0}' AND '{1}'
        //                                        OR ([TrainStartDate] <= '{0}' AND [TrainEndDate] >= '{1}')) 
        //                                        AND [TrainerId] = {2}", TrainStartDate, TrainEndDate, TrainerId);

        //            if (Queries.GetResultsFromQueryString(SQLString).Rows[0][0].ToString() == "0")
        //            {
        //                SQLString = string.Format(@"INSERT INTO PAYOUTScheduleTrainer
        //                                    (ScheduleId, TrainerId, TrainStartDate, TrainEndDate)
        //                                    VALUES
        //                                    ({0},{1},'{2}','{3}')", ScheduleId, TrainerId, TrainStartDate, TrainEndDate);

        //                Queries.ExecuteFromQueryString(SQLString);

        //                return 1;
        //            }                    
        //        }
        //        return 0;
        //    }
        //    catch
        //    {
        //        return -1;
        //    }
        //}



        //[WebMethod()]
        ////public static int DeleteRecord(int ScheduleId, int TrainerId, string TrainStartDate, string TrainEndDate)
        //public static int DeleteRecord(string ScheduleTrainerId)        
        //{
        //    try
        //    {
        //        string SQLString = string.Format(@" DELETE
        //                                        FROM [PAYOUTScheduleTrainer]
        //                                        WHERE [Id] = {0}", ScheduleTrainerId);

        //        Queries.ExecuteFromQueryString(SQLString);
        //        return 1;
        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //}
        
        protected void BtnInsert_Click(object sender, EventArgs e)
        {
            int TransactionResult = InsertRecord(HiddenIdTextBox.Text, (string)Session["SelectedTrainerId"], TrainStartDate_Ins.Text, TrainEndDate_Ins.Text);
            BindGridData();
            ValidationMessages(TransactionResult);
        }

        private int InsertRecord(string ScheduleId, string TrainerId, string TrainStartDate, string TrainEndDate)
        {
            try
            {
                if (TrainerId != "0")
                {
                    string SQLString = string.Format(@" SELECT COUNT(*)
                                                FROM [PAYOUTScheduleTrainer]
                                                WHERE ([TrainStartDate] BETWEEN '{0}' AND '{1}'
                                                OR [TrainEndDate] BETWEEN '{0}' AND '{1}'
                                                OR ([TrainStartDate] <= '{0}' AND [TrainEndDate] >= '{1}')) 
                                                AND [TrainerId] = {2}", TrainStartDate, TrainEndDate, TrainerId);

                    if (Queries.GetResultsFromQueryString(SQLString).Rows[0][0].ToString() == "0")
                    {
                        SQLString = string.Format(@"INSERT INTO PAYOUTScheduleTrainer
                                            (ScheduleId, TrainerId, TrainStartDate, TrainEndDate)
                                            VALUES
                                            ({0},{1},'{2}','{3}')", ScheduleId, TrainerId, TrainStartDate, TrainEndDate);

                        Queries.ExecuteFromQueryString(SQLString);
                        return 1;
                    }
                }
            }
            catch
            {
                return -1;
            }
            return 0;
        }


        private void ValidationMessages(int TransactionResult)
        {
            switch (TransactionResult)
            {
                case 1:
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(),
                          "err_msg",
                          "alert('Record created successfully');",
                          true);
                    break;
                case 0:
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(),
                          "err_msg",
                          "alert('Please select valid values');",
                          true);
                    break;
                case -1:
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(),
                          "err_msg",
                          "alert('The insertion caused an error');",
                          true);
                    break;
            }
        }


        protected void BtnDelete_Click(object sender, EventArgs e)
        {
            string ScheduleTrainerId = string.Empty;
            GridViewRow gvr = (GridViewRow)((Control)sender).Parent.Parent;

            for (int i = 0; i < gvr.Cells.Count; i++)
            {
                if (gvr.Cells[i].Controls.Count == 0)
                {
                    switch (((DataControlFieldCell)(gvr.Cells[i])).ContainingField.HeaderText)
                    {
                        case "scheduleTrainerId":                            
                            ScheduleTrainerId = (gvr.Cells[i]).Text;
                            break;
                    }
                }
            }

            if (ScheduleTrainerId != "&nbsp;")
            {
                try
                {
                    string SQLString = string.Format(@" DELETE
                                                FROM [PAYOUTScheduleTrainer]
                                                WHERE [Id] = {0}", ScheduleTrainerId);

                    Queries.ExecuteFromQueryString(SQLString);
                    BindGridData();
                }
                catch{}
            }

            
        }





        private void ProfileScreenSettings(ref DataTable dt)
        {
            if (userType == "Trainer")
            {
                //SetupCommissionBtn.Visible = false;
                //SetupOverrideBtn.Visible = false;
                TrainerBtn.Visible = false;
 
                try
                {
                    string SQLstring = string.Format(@" SELECT [ID]
                                                    FROM[Payout].[dbo].[PAYOUTtrainer]
                                                    WHERE[EmailAddress] = '{0}'", user);
                    string trainerId = Queries.GetResultsFromQueryString(SQLstring).Rows[0][0].ToString();


                    if (dt.Rows.Count > 0)
                        dt = dt.AsEnumerable().Where(t => t.Field<int?>("TrainerId") == int.Parse(trainerId) ||
                                                          t.Field<int?>("TrainerId") == null).CopyToDataTable();
                }
                catch{ }
                


                //foreach (DataRow row in dt.Rows) //Clean Ids from datatable where is not existing
                //{
                //    if (row["TrainerId"].ToString() != trainerId)
                //        row["Id"] = null;                    
                //}
            }
        }
    }
}