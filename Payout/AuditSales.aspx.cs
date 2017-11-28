using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication3
{
    public partial class AuditSales : Page
    {
        Dictionary<string, string> DTColumnsList;
        string user, userFullname;
        int pageIndexvar;
        static string StoredProcedureSelect;

        protected void Page_Load(object sender, EventArgs e)
        {
            pageIndexvar = 1;
            user = string.Empty;
            try
            {
                user = Session["user"].ToString();
                userFullname = Session["userFullname"].ToString();
                Session["TableStructure"] = null;                

                DTColumnsList = new Dictionary<string, string>();
                DTColumnsList.Add("SalesId", "txt");
                DTColumnsList.Add("Program", "ddl");
                DTColumnsList.Add("SalesDate", "dpkR");
                DTColumnsList.Add("Retailer", "ddl");
                DTColumnsList.Add("StoreNumber", "txt");
                DTColumnsList.Add("ItemNumber", "txt");
                DTColumnsList.Add("ItemName", "txt");
                DTColumnsList.Add("Owner", "ddl");                
                DTColumnsList.Add("Type", "ddl");                
                DTColumnsList.Add("TimeStamp", "dpkR");
                //DTColumnsList.Add("LockedTimeStamp", "ddl");
                DTColumnsList.Add("Snapshot", "ddl");
                DTColumnsList.Add("PreviousSnapshot", "ddl");


                cbl_SubReconciliation.Visible = false;

                if (!IsPostBack)
                {
                    if (user == "cwind@thesmartcircle.com")
                    {
                        StoredProcedureSelect = "SELECTGRID";
                        rbl_ReportType.SelectedIndex = 0;                        
                    }
                    else
                    {
                        StoredProcedureSelect = "RECONCILIATION";
                        rbl_ReportType.SelectedIndex = 1;
                        rbtn_LastLocked.SelectedIndex = 0;
                        cbl_SubReconciliation.Visible = true;

                        for (int i = 0; i < cbl_SubReconciliation.Items.Count; i++)
                        {
                            cbl_SubReconciliation.Items[i].Selected = false;
                        }
                    }

                    if (ConfigurationManager.AppSettings.Get("CleanAuditDuplicates") == "true")
                        CleanDuplicates();
                    Dictionary<string, string> DefaultValues = new Dictionary<string, string>();
                    DefaultValues.Add("WebSalesDateFromDPK", Common.ApplyDateFormat(DateTime.Now.AddMonths(-1)));
                    DefaultValues.Add("WebSalesDateToDPK", Common.ApplyDateFormat(DateTime.Now));                    
                    Session["DDLSelection"] = DefaultValues;
                    Session["GridResults"] = null;

                    Dictionary<string, string> Params = new Dictionary<string, string>();
                    Params.Add("WebSalesDateFromDPK", Common.ApplyDateFormat(DateTime.Now.AddMonths(-1)));
                    Params.Add("WebSalesDateToDPK", Common.ApplyDateFormat(DateTime.Now));
                    Params.Add("Action", StoredProcedureSelect);                    
                    Params.Add("PageIndex", "1");
                    Params.Add("PageSize", "20");
                    
                    Session["Params"] = Params;

                }

                if (rbl_ReportType.SelectedIndex == 0)
                {
                    StoredProcedureSelect = "SELECTGRID";
                    rbl_ReportType.SelectedIndex = 0;
                }
                else
                {
                    StoredProcedureSelect = "RECONCILIATION";
                    rbl_ReportType.SelectedIndex = 1;
                    cbl_SubReconciliation.Visible = true;

                    //cbl_SubReconciliation.Items.OfType<ListItem>().Where(l => l.Selected).
                    
                }

                List<string> HyperlinkColumns = new List<string>();
                HyperlinkColumns.Add("OriginalFilePath");
                HyperlinkColumns.Add("ReformattedFilePath");

                Session["PageCompleteFired"] = null;
                Session["HyperlinkColumns"] = HyperlinkColumns;
                DataSet GridDS = ImportDataToGrid(1);

                if (StoredProcedureSelect == "RECONCILIATION" && GridDS != null && GridDS.Tables[1] != null)
                {
                    GridDS.Tables[1].Columns.Remove("Type");
                    if(DTColumnsList.ContainsKey("Type"))
                        DTColumnsList.Remove("Type");
                }                    

                ControlUtilities.BindGridWithFilters(GridDS.Tables[0], GridDS.Tables[1], ref AuditGridView, DTColumnsList, false);
                Session["GridResults"] = GridDS.Tables[0];                
            }
            catch(Exception ex)
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }
        }

        private void CleanParameters()
        {
            Dictionary<string, string> DefaultValues = new Dictionary<string, string>();
            DefaultValues.Add("WebSalesDateFromDPK", Common.ApplyDateFormat(DateTime.Now.AddMonths(-1)));
            DefaultValues.Add("WebSalesDateToDPK", Common.ApplyDateFormat(DateTime.Now));
            Session["DDLSelection"] = DefaultValues;

            Dictionary<string, string> Params = new Dictionary<string, string>();
            Params.Add("WebSalesDateFromDPK", Common.ApplyDateFormat(DateTime.Now.AddMonths(-2)));
            Params.Add("WebSalesDateToDPK", Common.ApplyDateFormat(DateTime.Now));
            Params.Add("Action", StoredProcedureSelect);

            rbtn_LastLocked.SelectedIndex = 0;

            for (int i = 0; i < cbl_SubReconciliation.Items.Count; i++)
            {
                cbl_SubReconciliation.Items[i].Selected = false;
            }            

            Params.Add("PageIndex", "1");
            Params.Add("PageSize", "20");
            HttpContext.Current.Session["Params"] = Params;
            ChkUserChanges.Checked = false;            
        }

        //This method is built in order to clean all duplicates in Audit table which inevitably are being 
        //generated when edit import and exceptions are executed in different browsers
        private void CleanDuplicates()
        {
            Dictionary<string, string> Params = new Dictionary<string, string>();
            Params.Add("Action", "CLEANUP");
            Queries.ExecuteFromStoreProcedure("spx_PAYOUTAuditSales", Params);
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            try {

                user = Session["user"].ToString();

                
                Session["PageCompleteFired"] = true;
                DataSet GridDS = ImportDataToGrid(pageIndexvar);
                ControlUtilities.BindGridWithFilters(GridDS.Tables[0], GridDS.Tables[1], ref AuditGridView, DTColumnsList, false, true);
                
                if (GridDS.Tables[0].Rows.Count > 0)
                {
                    Common.PopulatePager(int.Parse(GridDS.Tables[2].Rows[0][0].ToString()), pageIndexvar, 10, ddlPageSize.SelectedValue, ref rptPager);                    
                    RecordCount.Text = "Row Count: " + int.Parse(GridDS.Tables[2].Rows[0][0].ToString());
                }                    
                else
                {
                    Common.PopulatePager(0,0,0, ddlPageSize.SelectedValue, ref rptPager);                    
                    RecordCount.Text = "Row Count: 0";
                }
                    
            }
            catch
            {

            }
        }

        private DataSet ImportDataToGrid(int pageIndex)
        {
            Dictionary<string, string> Params = GetFilterParameters();

            Params["PageIndex"] = pageIndex.ToString();
            Params["PageSize"] = ddlPageSize.SelectedValue;
            Params["Action"] = StoredProcedureSelect;


            foreach (var item in cbl_SubReconciliation.Items.OfType<ListItem>())
            {
                if (Params.ContainsKey(item.Text.Replace(" ", "") + "Reconciliation"))
                    Params[item.Text.Replace(" ", "") + "Reconciliation"] = item.Selected.ToString();
                else
                    Params.Add(item.Text.Replace(" ", "") + "Reconciliation", item.Selected.ToString());
            }


            if (rbtn_LastLocked.SelectedItem.Text == "Latest")
            {
                if (Params.ContainsKey(rbtn_LastLocked.SelectedItem.Text))
                    Params["ShowLastLocked"] = rbtn_LastLocked.SelectedItem.Text;
                else
                    Params.Add("ShowLastLocked", rbtn_LastLocked.SelectedItem.Text);
            }

            //Params[item.Text.Replace("$ ", "Dollar")] = item.Selected.ToString();


            if (ChkUserChanges.Checked)
                Params["HideOriginals1"] = "1";
            else
                Params["HideOriginals1"] = "0";
            
            DataSet ds = Queries.GetResultsFromStoreProcedure("spx_PAYOUTAuditSales", ref Params);
            
            return ds;
        }

        private static Dictionary<string, string> GetFilterParameters()
        {
            Dictionary<string, string> Params = (Dictionary<string, string>)HttpContext.Current.Session["Params"];

            if (HttpContext.Current.Session["DDLSelection"] != null)
            {
                Dictionary<string, string> SelectedParams = (Dictionary<string, string>)HttpContext.Current.Session["DDLSelection"];

                foreach (KeyValuePair<string, string> SelectedParam in SelectedParams)
                {
                    Params[SelectedParam.Key] = SelectedParam.Value;
                }
            }

            return Params;
        }


        protected void btnResetFilters_Click(object sender, EventArgs e)
        {
            CleanParameters();
        }


        /// <summary>
        /// Add ScriptManagaer to Handle DatePicker Extension
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            Page.Init += delegate (object sender, EventArgs e_Init)
            {
                if (ScriptManager.GetCurrent(Page) == null)
                {
                    ScriptManager sMgr = new ScriptManager();
                    Page.Form.Controls.AddAt(0, sMgr);
                }
            };
            base.OnInit(e);
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> Params = (Dictionary<string, string>)HttpContext.Current.Session["Params"];

            if(StoredProcedureSelect == "SELECTGRID")
                Params["Action"] = "EXPORTFILESEL";
            else
                Params["Action"] = "EXPORTFILEREC";

            string FolderPath = Server.MapPath(ConfigurationManager.AppSettings["FolderPath"] + @"\DownloadFilefolder\");
            Common.ExportCSVFile(FolderPath, "AuditSales", userFullname, Params, "spx_PAYOUTAuditSales");
   
            Params["Action"] = StoredProcedureSelect;

            foreach (var item in cbl_SubReconciliation.Items.OfType<ListItem>())
                Params[item.Text.Replace("$ ", "Dollar")] = item.Selected.ToString();

            //if (FieldToFilter != null)
            //    Params["ColumnToFilter"] = FieldToFilter;
        }

        
        protected void Page_Changed(object sender, EventArgs e)
        {
            pageIndexvar = int.Parse((sender as LinkButton).CommandArgument);            
        }

        [WebMethod()]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string JSONRequestFromClient()
        {
            Dictionary<string, string> Params = GetFilterParameters();
            if (Params.ContainsKey("Action"))
                Params["Action"] = "LOCK_AUDIT_SALES";
            else
                Params.Add("Action", "LOCK_AUDIT_SALES");

            Queries.ExecuteFromStoreProcedure2("spx_PAYOUTAuditSales", Params, "Results");
            //DataSet ds = Queries.GetResultsFromStoreProcedure("spx_PAYOUTAuditSales", ref Params, "Results");


            //string FolderPath = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["FolderPath"] + @"\DownloadFilefolder\");
            //Common.ExportCSVFile(FolderPath, "AuditSales", HttpContext.Current.Session["userFullname"].ToString(), Params, "spx_PAYOUTAuditSales", "Results");

            //if(ds.Tables.Count > 0)
            //    Common.ExportToExcel_List(new List<DataTable> { ds.Tables[0] }, "RoadShow SalesAudit");

            return Params["Results"];
        }
    }
}
