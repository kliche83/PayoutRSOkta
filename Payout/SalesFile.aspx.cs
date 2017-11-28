using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace WebApplication3
{
    public partial class SalesFile : Page
    {
        string user, userFullname;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                user = Session["user"].ToString();
                userFullname = Session["userFullname"].ToString();

                ExportBtn.Enabled = true;

                if (!IsPostBack)
                {
                    Session["RowCount"] = null;
                    GetDDL();
                }
                ValMessage.Text = "";
            }
            catch(Exception ex)
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }
        }

        protected void ExportBtn_Click(object sender, EventArgs e)        
        {
            if (int.Parse(Session["RowCount"].ToString()) <= 1000000)
            {
                Dictionary<string, string> Params = fillParams();
                Params.Add("Action", "BULKSELECTTABLE");

                string FolderPath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["FolderPath"] + @"\DownloadFilefolder\");
                Common.ExportCSVFile(FolderPath, "SalesFile", userFullname, Params, "spx_PAYOUTsales");
            }
            else
            {
                ValMessage.Text = "No more than 1000,000 Rows allowed to generate file";
            }


            //if (int.Parse(Session["RowCount"].ToString()) <= 1000000)
            //{
            //    Dictionary<string, string> Params = fillParams();

            //    Params.Add("Action", "BULKSELECTTABLE");


            //    string FolderPath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["FolderPath"] + @"\SalesFiles\");
            //    string FileName = "SalesFile_" + userFullname + "_" + Common.ApplyDateFormat(DateTime.Now) + ".csv";
                

            //    if (!Directory.Exists(FolderPath))
            //    {
            //        try
            //        {
            //            Directory.CreateDirectory(FolderPath);
                        
            //        }
            //        catch(Exception ex)
            //        {
            //            FolderPath = null;
            //        }
            //    }

            //    if (File.Exists(FolderPath + FileName))
            //    {
            //        File.Delete(FolderPath + FileName);
            //    }
            //    if (FolderPath != null)
            //    {
            //        Queries.CSVFileDownloadFromSP("spx_PAYOUTsales", ref Params, ",", FolderPath + FileName);
            //        DownloadFileFromServer(FolderPath, FileName);
            //    }
            //}
            //else
            //{
            //    ValMessage.Text = "No more than 1000,000 Rows allowed to generate file";
            //}
        }


        private void DownloadFileFromServer(string serverFolderPath, string fileName)
        {

            string attachment = "attachment; filename=\"" + fileName + "\"";

            HttpResponse response = HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ClearHeaders();
            response.ContentType = "text/plain";
            response.AddHeader("Content-Disposition", attachment);

            response.WriteFile(serverFolderPath + fileName);                        
            response.Flush();
            File.Delete(serverFolderPath + fileName);            
            response.End();

            
        }
        
        protected void ResetFilters_Click(object sender, EventArgs e)
        {
            DateFromTXT.Text = "";
            DateToTXT.Text = "";
            ProgramDDL.SelectedValue = "All";
            RetailerDDL.SelectedValue = "All";
            StoreNumberTXT.Text = "";

            GetDDL();
        }

        protected void DDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetDDL();
        }

        protected void TXT_TextChanged(object sender, EventArgs e)
        {
            GetDDL();
        }

        private void GetDDL()
        {
            Dictionary<string, string> SelectedValues = new Dictionary<string, string>();
            Dictionary<string, string> Params = fillParams(SelectedValues);
            
            Params.Add("Action", "BULKSELECTDDL");
            DataTable dt = Queries.GetResultsFromStoreProcedure("spx_PAYOUTsales", ref Params).Tables[0];

            List<string> lstObjects = new List<string>();

            lstObjects = dt.AsEnumerable()
                .OrderBy(f => f.Field<string>("Program"))
                .Select(f => f.Field<string>("Program")).Distinct().ToList();
            lstObjects.Insert(0, "All");

            if (ProgramDDL == null)
                ProgramDDL = new DropDownList();
            ProgramDDL.DataSource = lstObjects;
            if (!(SelectedValues.ContainsKey("SelectedProgram") && lstObjects.Contains(SelectedValues["SelectedProgram"])))
                SelectedValues.Remove("SelectedProgram");


            lstObjects = dt.AsEnumerable()
                .OrderBy(f => f.Field<string>("StoreName"))
                .Select(f => f.Field<string>("StoreName")).Distinct().ToList();

            if (lstObjects.Where(x => x.Contains("Kroger")).Count() > 0)
            {
                lstObjects.Add("Kroger All");
                lstObjects.Sort();
            }

            lstObjects.Insert(0, "All");

            if (RetailerDDL == null)
                RetailerDDL = new DropDownList();
            RetailerDDL.DataSource = lstObjects;
            if (!(SelectedValues.ContainsKey("SelectedRetailer") && lstObjects.Contains(SelectedValues["SelectedRetailer"])))
                SelectedValues.Remove("SelectedRetailer");

            ProgramDDL.DataBind();
            RetailerDDL.DataBind();

            ProgramDDL.SelectedValue = SelectedValues.ContainsKey("SelectedProgram") ? SelectedValues["SelectedProgram"] : "All";
            RetailerDDL.SelectedValue = SelectedValues.ContainsKey("SelectedRetailer") ? SelectedValues["SelectedRetailer"] : "All";

            int RowCount = dt.AsEnumerable().Select(f => f.Field<int>("TotalRows")).Take(1).FirstOrDefault();
            CountRows.Text = "Rows count: " + string.Format("{0:n0}", RowCount);
            Session["RowCount"] = RowCount;
        }

        private DataTable GetTable(ref Dictionary<string, string> Params)
        {
            Params["Action"] = "BULKSELECTTABLE";            
            return Queries.GetResultsFromStoreProcedure("spx_PAYOUTsales", ref Params).Tables[0];
        }

        private Dictionary<string, string> fillParams(Dictionary<string, string> SelectedValues = null)
        {
            if(SelectedValues == null)
                SelectedValues = new Dictionary<string, string>();

            DateTime DatetimeVarCast = new DateTime();
            Dictionary<string, string> Params = new Dictionary<string, string>();

            if (DateFromTXT != null && DateFromTXT.Text != "")
            {
                DateTime.TryParse(DateFromTXT.Text, out DatetimeVarCast);
                if (DateTime.MinValue != DatetimeVarCast)
                    Params.Add("WebStartSalesRange", DateFromTXT.Text);
                else
                    DateFromTXT.Text = "";
            }
            if (DateToTXT != null && DateToTXT.Text != "")
            {
                DateTime.TryParse(DateToTXT.Text, out DatetimeVarCast);
                if (DateTime.MinValue != DatetimeVarCast)
                    Params.Add("WebEndSalesRange", DateToTXT.Text);
                else
                    DateToTXT.Text = "";
            }

            if (ProgramDDL != null && ProgramDDL.SelectedValue != "" && ProgramDDL.SelectedValue != "All")
            {
                SelectedValues.Add("SelectedProgram", ProgramDDL.SelectedValue);
                Params.Add("WebProgramWhere", ProgramDDL.SelectedValue == "All" ? "NULL" : " = '" + ProgramDDL.SelectedValue.Replace("'", "''") + "'");
            }
            if (RetailerDDL != null && RetailerDDL.SelectedValue != "" && RetailerDDL.SelectedValue != "All")
            {
                SelectedValues.Add("SelectedRetailer", RetailerDDL.SelectedValue);
                Params.Add("WebStoreName", RetailerDDL.SelectedValue == "Kroger All" ? "Kroger" : RetailerDDL.SelectedValue);
            }                
            if (StoreNumberTXT != null && StoreNumberTXT.Text != "")
            {
                Params.Add("WebStoreNumber", StoreNumberTXT.Text);
            }
            if (ItemNumberTXT != null && ItemNumberTXT.Text != "")
            {
                Params.Add("WebItemNumber", ItemNumberTXT.Text);
            }


            return Params;
        }        
    }
}
