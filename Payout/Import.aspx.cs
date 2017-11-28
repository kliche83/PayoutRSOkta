using System;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Web;
using WebApplication3;

public partial class Import : System.Web.UI.Page
{
    private static string user = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["OriginalImport"] = null;
            Session["Uploadedfile"] = null;
        }
        else
        {
            lblLoadingMessage.Text = "";
        }

        importOptions.Visible = false;

        try
        {

            user = Session["user"].ToString();
        }
        catch
        {
            Response.Write("<script>window.top.location = '../';</script>");
        }
    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        Session["Uploadedfile"] = FileUpload1.FileName;

        //FileUploadToServer(ref FileUpload1, ConfigurationManager.AppSettings["FolderPath"] + @"\Uploads\");

        //FileUploadToServer(ref FileUpload1, Server.MapPath(ConfigurationManager.AppSettings["FolderPath"] + @"\Uploads\"));


        if (FileUpload1.HasFile)
        {
            string FileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
            Session["Uploadedfile"] = FileName;
            string Extension = Path.GetExtension(FileUpload1.PostedFile.FileName);
            string FolderPath = ConfigurationManager.AppSettings["FolderPath"] + @"\Uploads\";
            string FilePath = Server.MapPath(FolderPath + FileName);
            FileUpload1.SaveAs(FilePath);
            GetExcelSheets(FilePath, Extension, "Yes");
        }
    }

    public static string MapPathReverse(string fullServerPath)
    {

        return @"~\" + fullServerPath.Replace(HttpContext.Current.Request.PhysicalApplicationPath, string.Empty);
    }

    public static string VirtualPath(string physicalPath)
    {

        return @"~\" + physicalPath.Replace(HttpContext.Current.Request.PhysicalApplicationPath, string.Empty);
    }

    private bool FileUploadToServer(ref FileUpload objUpload, string FolderPath)
    {
        if (objUpload.HasFile)
        {
            try
            {
                string FileName = Path.GetFileName(objUpload.PostedFile.FileName);
                //Session["Uploadedfile"] = FileName;
                string Extension = Path.GetExtension(objUpload.PostedFile.FileName);
                //string FilePath = Server.MapPath(FolderPath);

                //string FilePath = Server.MapPath(FolderPath + FileName);

                objUpload.SaveAs(FolderPath + objUpload.FileName);
                GetExcelSheets(FolderPath, Extension, "Yes");

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        return false;
    }

    protected void btnSync_Click(object sender, EventArgs e)

    {
        using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            string generate = "exec SPX_PayoutGetGranton";

            SqlCommand cmd = new SqlCommand(generate, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

    }

    private void GetExcelSheets(string FilePath, string Extension, string isHDR)
    {
        string conStr = "";
        switch (Extension)
        {
            case ".xls": //Excel 97-03
                         //conStr = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                lblMessage.Text = "Excel 97-03 (.xls) is not supported at this time.";
                lblMessage.ForeColor = System.Drawing.Color.Red;

                break;

            case ".xlsx": //Excel 07
                conStr = ConfigurationManager.ConnectionStrings["conXLSX"].ConnectionString;

                //REMOVE BELOW AFTER SUPPORT FOR XLS
                conStr = String.Format(conStr, FilePath, isHDR);
                OleDbConnection connExcel = new OleDbConnection(conStr);
                OleDbCommand cmdExcel = new OleDbCommand();
                OleDbDataAdapter oda = new OleDbDataAdapter();
                cmdExcel.Connection = connExcel;
                connExcel.Open();
                ddlSheets.Items.Clear();
                //ddlSheets.Items.Add(new ListItem("--Select Sheet--", ""));
                ddlSheets.DataSource = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                ddlSheets.DataTextField = "TABLE_NAME";
                ddlSheets.DataValueField = "TABLE_NAME";
                ddlSheets.DataBind();
                connExcel.Close();
                lblFileName.Text = Path.GetFileName(FilePath);
                Panel2.Visible = true;
                Panel1.Visible = false;

                break;
            default:
                lblMessage.Text = "Excel 97-03 (.xls) is not supported at this time.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                break;
        }

        //ENABLE AFTER SUPPORT FOR XLS:

        ////Get the Sheets in Excel WorkBoo
        //conStr = String.Format(conStr, FilePath, isHDR);
        //OleDbConnection connExcel = new OleDbConnection(conStr);
        //OleDbCommand cmdExcel = new OleDbCommand();
        //OleDbDataAdapter oda = new OleDbDataAdapter();
        //cmdExcel.Connection = connExcel;
        //connExcel.Open();

        ////Bind the Sheets to DropDownList
        //ddlSheets.Items.Clear();
        ////ddlSheets.Items.Add(new ListItem("--Select Sheet--", ""));
        //ddlSheets.DataSource = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        //ddlSheets.DataTextField = "TABLE_NAME";
        //ddlSheets.DataValueField = "TABLE_NAME";
        //ddlSheets.DataBind();
        //connExcel.Close();
        //lblFileName.Text = Path.GetFileName(FilePath);
        //Panel2.Visible = true;
        //Panel1.Visible = false;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (TableDDL.SelectedValue == "Schedule")
            HFIsSchedule.Value = "true";

        string FileName = lblFileName.Text;
        string Extension = Path.GetExtension(FileName);
        string FolderPath = Server.MapPath(ConfigurationManager.AppSettings["FolderPath"] + @"\Uploads\");
        string CommandText = "";
        Dictionary<string, string> Params;

        switch (Extension)
        {
            case ".xls": //Excel 97-03
                         //CommandText = "spx_ImportFromExcel03";
                lblMessage.Text = "Excel 97-03 (.xls) is not supported at this time. Save the Excel file as XLSX.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                break;
            case ".xlsx": //Excel 07
                          //CommandText = "spx_ImportFromExcel07";
                CommandText = "spx_PAYOUTImportXLSX";

                //REMOVE BELOW AFTER SUPPORT FOR XLS

                Params = new Dictionary<string, string>();
                Params.Add("SheetName", ddlSheets.SelectedItem.Text);
                Params.Add("FileName", FileName);
                Params.Add("HDR", rbHDR.SelectedItem.Text);
                Params.Add("StoreName", TableDDL.SelectedValue);
                Params.Add("UploadPath", ConfigurationManager.AppSettings.Get("ServerPath") + @"\Uploads\");
                Params.Add("Username", user);
                Params.Add("Action", "OriginalLoad");

                //string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                //SqlConnection con = new SqlConnection(strConnString);
                //SqlCommand cmd = new SqlCommand();
                //cmd.CommandTimeout = 3600;
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.CommandText = CommandText;
                //cmd.Parameters.Add("@SheetName", SqlDbType.VarChar).Value = ddlSheets.SelectedItem.Text;
                //cmd.Parameters.Add("@FileName", SqlDbType.VarChar).Value = FileName;
                //cmd.Parameters.Add("@HDR", SqlDbType.VarChar).Value = rbHDR.SelectedItem.Text;
                //cmd.Parameters.Add("@StoreName", SqlDbType.VarChar).Value = TableDDL.SelectedValue;
                //cmd.Parameters.Add("@UploadPath", SqlDbType.VarChar).Value = ConfigurationManager.AppSettings.Get("ServerPath") + @"\Uploads\";
                ////cmd.Parameters.Add("@Program", SqlDbType.VarChar).Value = progDDL.SelectedValue;
                //cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = user;
                //cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = "OriginalLoad";
                //cmd.Connection = con;
                try
                {
                    Session["StoreName"] = TableDDL.SelectedValue;
                    importOptions.Visible = true;
                    Queries.ExecuteFromStoreProcedure("spx_PAYOUTImportXLSX", Params);

                    string SQLstring = @"SELECT * FROM PAYOUTsalesTemp";
                    DataTable dt = Queries.GetResultsFromQueryString(SQLstring);
                    Session["OriginalImport"] = dt;

                    Params = new Dictionary<string, string>();
                    Params.Add("Action", "UpdateTemp");
                    Params.Add("Username", user);
                    Queries.ExecuteFromStoreProcedure("spx_PAYOUTImportXLSX", Params);





                    //con.Open();
                    //object count = cmd.ExecuteNonQuery();

                    if (TableDDL.SelectedValue != "Schedule")
                    {
                        SqlDataSource1.SelectCommand = "SELECT [OriginalId], [Program], convert(nvarchar, [SalesDate], 101) AS [SalesDate], [StoreName], [StoreNumber], [ItemNumber], [ItemName], [Qty], [UnitCost], [ExtendedCost], [ImportedBy], convert(nvarchar, [ImportedOn], 101) AS [ImportedOn] FROM [PAYOUTsalesTemp] WHERE [ImportedBy] = '" + user + "' AND [TYPE] = 'U' ORDER BY [SalesDate] DESC";
                    }
                    else
                    {
                        SqlDataSource1.SelectCommand = "SELECT [Id], [Program], convert(nvarchar, [StartDate], 101) AS [StartDate], convert(nvarchar, [EndDate], 101) AS [EndDate], [StoreName], [StoreNumber], [City], [State], [OwnerFirstname], [OwnerLastname], [HubFirstname], [ImportedBy], convert(nvarchar, [ImportedOn], 101) AS [ImportedOn] FROM [PAYOUTscheduleTemp] WHERE [ImportedBy] = '" + user + "' ORDER BY [StartDate] DESC";
                    }
                    GridView1.DataBind();
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                    lblMessage.Text = GridView1.Rows.Count.ToString() + " records inserted.";
                }
                catch (Exception ex)
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = ex.Message + "<br /> <span style='color: black!important;'>Mayday! Get <a href='mailto:amir@innovage.net?Subject=Payout Error&Body=Error Message: %0D%0A" + ex.Message.Replace("'", "?") + " %0D%0A%0D%0AImport Type: %0D%0A" + TableDDL.SelectedValue + " %0D%0A%0D%0AImport File: %0D%0A" + FolderPath + FileName + "'>help</a> with this error.</span>";
                }
                finally
                {
                    //con.Close();
                    //con.Dispose();
                    Panel1.Visible = true;
                    Panel2.Visible = false;
                }

                break;
        }

        ////Clean up Duplicates
        //   string dupDel = @"
        //	;WITH x AS 
        //	(
        //		SELECT [Program],[StartDate],[EndDate],[StoreName],[StoreNumber],[City],[State],[OwnerFirstname],[OwnerLastname],[HubFirstname],[HubLastname], 
        //		rn = ROW_NUMBER() OVER (
        //			PARTITION BY [Program],[StartDate],[EndDate],[StoreName],[StoreNumber],[City],[State],[OwnerFirstname],[OwnerLastname],[HubFirstname],[HubLastname] 
        //			ORDER BY [Program],[StartDate],[EndDate],[StoreName],[StoreNumber],[City],[State],[OwnerFirstname],[OwnerLastname],[HubFirstname],[HubLastname] 
        //		)
        //		FROM PAYOUTschedule
        //	)
        //	DELETE x WHERE rn > 1;

        //	;WITH x AS 
        //	(
        //		SELECT [Program],[SalesDate],[StoreName],[StoreNumber],[ItemNumber],[ItemName],[Qty],[UnitCost],[ExtendedCost],[OwnerFirstname],[OwnerLastname],[StartDate],[EndDate],[City],[State],[HubFirstname],[HubLastname], 
        //		rn = ROW_NUMBER() OVER (
        //			PARTITION BY [Program],[SalesDate],[StoreName],[StoreNumber],[ItemNumber],[ItemName],[Qty],[UnitCost],[ExtendedCost],[OwnerFirstname],[OwnerLastname],[StartDate],[EndDate],[City],[State],[HubFirstname],[HubLastname] 
        //			ORDER BY [Program],[SalesDate],[StoreName],[StoreNumber],[ItemNumber],[ItemName],[Qty],[UnitCost],[ExtendedCost],[OwnerFirstname],[OwnerLastname],[StartDate],[EndDate],[City],[State],[HubFirstname],[HubLastname] 
        //		)
        //		FROM PAYOUTsalesTemp
        //	)
        //	DELETE x WHERE rn > 1;

        //	;WITH x AS 
        //	(
        //		SELECT [Program],[SalesDate],[StoreName],[StoreNumber],[ItemNumber],[ItemName],[Qty],[UnitCost],[ExtendedCost],[OwnerFirstname],[OwnerLastname],[StartDate],[EndDate],[City],[State],[HubFirstname],[HubLastname], 
        //		rn = ROW_NUMBER() OVER (
        //			PARTITION BY [Program],[SalesDate],[StoreName],[StoreNumber],[ItemNumber],[ItemName],[Qty],[UnitCost],[ExtendedCost],[OwnerFirstname],[OwnerLastname],[StartDate],[EndDate],[City],[State],[HubFirstname],[HubLastname] 
        //			ORDER BY [Program],[SalesDate],[StoreName],[StoreNumber],[ItemNumber],[ItemName],[Qty],[UnitCost],[ExtendedCost],[OwnerFirstname],[OwnerLastname],[StartDate],[EndDate],[City],[State],[HubFirstname],[HubLastname] 
        //		)
        //		FROM PAYOUTsales
        //	)
        //	DELETE x WHERE rn > 1;
        //";

        //using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        //{
        //    con.Open();
        //    using (SqlCommand cmd = new SqlCommand(dupDel, con))
        //    {
        //        SqlDataReader reader = cmd.ExecuteReader();
        //    }
        //    con.Close();
        //}

        Params = new Dictionary<string, string>();
        Params.Add("user", user);
        WebApplication3.Queries.ExecuteFromStoreProcedure("spx_PAYOUTImportClean", Params);


        //ENABLE AFTER SUPPORT FOR XLS:

        ////Read Excel Sheet using Stored Procedure
        ////And import the data into Database Table
        //String strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        //SqlConnection con = new SqlConnection(strConnString);
        //SqlCommand cmd = new SqlCommand();
        //cmd.CommandType = CommandType.StoredProcedure;
        //cmd.CommandText = CommandText;
        //cmd.Parameters.Add("@SheetName", SqlDbType.VarChar).Value = ddlSheets.SelectedItem.Text;
        //cmd.Parameters.Add("@FilePath", SqlDbType.VarChar).Value = FolderPath + @"\" + FileName;
        //cmd.Parameters.Add("@HDR", SqlDbType.VarChar).Value = rbHDR.SelectedItem.Text;
        //cmd.Parameters.Add("@TableName", SqlDbType.VarChar).Value = TableDDL.SelectedValue;
        //cmd.Parameters.Add("@Program", SqlDbType.VarChar).Value = progDDL.SelectedValue;
        //cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = user;
        //cmd.Connection = con;

        //try
        //{
        //    con.Open();
        //    object count = cmd.ExecuteNonQuery();
        //    SqlDataSource1.SelectCommand = "SELECT * FROM [" + TableDDL.SelectedValue + "]";
        //    GridView1.DataBind();
        //    lblMessage.ForeColor = System.Drawing.Color.Green;
        //    lblMessage.Text = GridView1.Rows.Count.ToString() + " records inserted.";
        //    if (TableDDL.SelectedValue != "PAYOUTowners")
        //    {
        //        nextBtn.Visible = true;
        //    }
        //}
        //catch (Exception ex)
        //{
        //    lblMessage.ForeColor = System.Drawing.Color.Red;
        //    lblMessage.Text = ex.Message;
        //}
        //finally
        //{
        //    con.Close();
        //    con.Dispose();
        //    Panel1.Visible = true;
        //    Panel2.Visible = false;
        //}
    }

    protected void btnsaveSchedule_Click(object sender, EventArgs e)
    {
        string serverpath = ConfigurationManager.AppSettings["ServerPath"].Replace(@"\c$\", @"\c:\");
        serverpath = serverpath.Substring(serverpath.IndexOf(@"c:\"));

        string UploadedPathNewFolder = GetTodaysFolder(serverpath + @"\Uploads\");

        if (UploadedPathNewFolder != null)
        {
            string UploadedFile = serverpath + @"\Uploads\" + (string)HttpContext.Current.Session["Uploadedfile"];
            UploadedPathNewFolder = UploadedPathNewFolder + "\\" + (string)HttpContext.Current.Session["Uploadedfile"];

            File.Copy(UploadedFile, UploadedPathNewFolder, true);
            File.Delete(UploadedFile);

            InsertSales(true);

            Response.Redirect("~/Import.aspx", true);
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Panel1.Visible = true;
        Panel2.Visible = false;
    }


    protected void undoImportBtn_Click(object sender, EventArgs e)
    {
        Session["OriginalImport"] = null;
        string undo = string.Empty;

        if (TableDDL.SelectedValue != "Schedule")
        {
            undo = string.Format(@" /*DELETE Aud
                                    FROM [Payout].[dbo].[PAYOUTsalesAudit] aud
                                    INNER JOIN [Payout].[dbo].[PAYOUTsalesTemp] temp ON Aud.Id = temp.Id  
                                    WHERE temp.ImportedBy = '{0}';
  */
                                    DELETE FROM PAYOUTsalesTemp WHERE ImportedBy = '{0}';", user);
        }
        else
        {
            undo = "DELETE FROM PAYOUTscheduleTemp WHERE ImportedBy = '" + user + "'";
        }

        Queries.ExecuteFromQueryString(undo);

        //      using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        //{
        //	string undo = string.Empty;

        //	if (TableDDL.SelectedValue != "Schedule")
        //	{
        //		undo = string.Format(@" DELETE Aud
        //                                      FROM [Payout].[dbo].[PAYOUTsalesAudit] aud
        //                                      INNER JOIN [Payout].[dbo].[PAYOUTsalesTemp] temp ON Aud.Id = temp.Id  
        //                                      WHERE temp.ImportedBy = '{0}';

        //                                      DELETE FROM PAYOUTsalesTemp WHERE ImportedBy = '{0}';", user);
        //	}
        //	else
        //	{
        //		undo = "DELETE FROM PAYOUTscheduleTemp WHERE ImportedBy = '" + user + "'";
        //	}

        //	con.Open();
        //	using (SqlCommand cmd = new SqlCommand(undo, con))
        //	{
        //		SqlDataReader reader = cmd.ExecuteReader();
        //	}
        //	con.Close();
        //}

        lblMessage.Text = "Import reverted by user.";
        GridView1.DataBind();
        importOptions.Visible = false;

    }

    //protected void clearSales_Click(object sender, EventArgs e)
    //{
    //       string delSales = "DELETE FROM PAYOUTsales DELETE FROM PAYOUTsalesTemp";
    //       WebApplication3.Queries.ExecuteFromQueryString(delSales);


    // //      using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
    //	//{
    //	//	string delSales = "DELETE FROM PAYOUTsales DELETE FROM PAYOUTsalesTemp";

    //	//	con.Open();
    //	//	using (SqlCommand cmd = new SqlCommand(delSales, con))
    //	//	{
    //	//		SqlDataReader reader = cmd.ExecuteReader();
    //	//	}
    //	//	con.Close();
    //	//}
    //}

    protected void clearSchedule_Click(object sender, EventArgs e)
    {
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            string delSchedule = "DELETE FROM PAYOUTschedule DELETE FROM PAYOUTscheduleTemp";

            con.Open();
            using (SqlCommand cmd = new SqlCommand(delSchedule, con))
            {
                SqlDataReader reader = cmd.ExecuteReader();
            }
            con.Close();
        }
    }

    protected void SqlDataSource1_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
    {
        e.Command.CommandTimeout = 3600;
    }

    protected void btnUploadOriginal_Click(object sender, EventArgs e)
    {
        string serverpath = ConfigurationManager.AppSettings["ServerPath"].Replace(@"\c$\", @"\c:\");
        serverpath = serverpath.Substring(serverpath.IndexOf(@"c:\"));

        string OriginalPathNewFolder = GetTodaysFolder(serverpath + @"\Original\");
        string UploadedPathNewFolder = GetTodaysFolder(serverpath + @"\Uploads\");

        FileUploadToServer(ref FileUploadOriginalFile, serverpath + @"\Original\");

        if (OriginalPathNewFolder != null)
        {
            string OriginalPathUploaded = serverpath + @"\Original\" + FileUploadOriginalFile.FileName;
            string UploadedFile = serverpath + @"\Uploads\" + (string)HttpContext.Current.Session["Uploadedfile"];

            OriginalPathNewFolder = OriginalPathNewFolder + "\\" + FileUploadOriginalFile.FileName;
            UploadedPathNewFolder = UploadedPathNewFolder + "\\" + (string)HttpContext.Current.Session["Uploadedfile"];

            File.Copy(UploadedFile, UploadedPathNewFolder, true);
            File.Delete(UploadedFile);
            File.Copy(OriginalPathUploaded, OriginalPathNewFolder, true);
            File.Delete(OriginalPathUploaded);

            if (TableDDL.SelectedValue == "Schedule")
                InsertSales(true);
            else
                InsertSales(false, OriginalPathNewFolder, UploadedPathNewFolder);

            Response.Redirect("~/Import.aspx", true);
        }
    }


    private static bool InsertSales(bool IsSchedule, string OriginalPath = "", string ReformattedPath = "")
    {
        if (!IsSchedule)
        {
            string serverpath = ConfigurationManager.AppSettings["ServerPath"].Replace(@"c$\inetpub\wwwroot\", "");
            OriginalPath = serverpath + OriginalPath.Substring(OriginalPath.IndexOf("Original") - 1).Replace(@"Original\\", @"Original\");
            ReformattedPath = serverpath + ReformattedPath.Substring(ReformattedPath.IndexOf("Uploads") - 1).Replace(@"Uploads\\", @"Uploads\");

            Dictionary<string, string> Params = new Dictionary<string, string>();
            Params.Add("user", user);
            Params.Add("IsSchedule", "False");

            OriginalPath = OriginalPath.Replace("'", "''").Replace(@"c$\inetpub\wwwroot\", "");
            ReformattedPath = ReformattedPath.Replace("'", "''").Replace(@"c$\inetpub\wwwroot\", "");

            Params.Add("OriginalFilePath", OriginalPath);
            Params.Add("ReformattedFilePath", ReformattedPath);

            ////Params.Add("OriginalFilePath", OriginalPath.Replace("'", "''").Replace(@"c$\inetpub\wwwroot\", ""));
            ////Params.Add("ReformattedFilePath", ReformattedPath.Replace("'", "''").Replace(@"c$\inetpub\wwwroot\", ""));


            //DataTable dt = (DataTable)HttpContext.Current.Session["OriginalImport"];
            //foreach (DataRow dr in dt.Rows)
            //{
            //    string SQLstring = string.Format(
            //                                @"INSERT INTO PAYOUTsalesAudit
            //                                (Id, SalesDate, StoreName, StoreNumber, ItemNumber, ItemName, Qty, UnitCost, ExtendedCost,
            //                                ImportedBy, ImportedOn, Archive, TimeStamp, TYPE, OriginalFilePath, ReformattedFilePath)
            //                                VALUES
            //                                ({0},'{1}',{2},{3},{4},{5},{6},{7},{8},
            //                                '{9}','{10}',{11},GETDATE(),'{12}','{13}','{14}')",
            //                                dr["Id"], dr["SalesDate"],
            //                                dr["StoreName"].ToString().Trim() == "" ? "NULL" : "'" + dr["StoreName"].ToString().Replace("'", "''") + "'",
            //                                dr["StoreNumber"].ToString().Trim() == "" ? "NULL" : "'" + dr["StoreNumber"] + "'",
            //                                dr["ItemNumber"].ToString().Trim() == "" ? "NULL" : "'" + dr["ItemNumber"].ToString().Replace("'", "''") + "'",
            //                                dr["ItemName"].ToString().Trim() == "" ? "NULL" : "'" + dr["ItemName"].ToString().Replace("'", "''") + "'",
            //                                dr["Qty"].ToString().Trim() == "" ? "NULL" : dr["Qty"],
            //                                dr["UnitCost"].ToString().Trim() == "" ? "NULL" : dr["UnitCost"],
            //                                dr["ExtendedCost"].ToString().Trim() == "" ? "NULL" : dr["ExtendedCost"],
            //                                dr["ImportedBy"], dr["ImportedOn"],
            //                                dr["Archive"].ToString().ToLower() == "true" ? "1" : "0",
            //                                "N", OriginalPath, ReformattedPath
            //                                );
            //    Queries.ExecuteFromQueryString(SQLstring);
            //}

            Queries.ExecuteFromStoreProcedure("spx_PAYOUTImportSave", Params);
        }
        else
        {

            Dictionary<string, string> Params = new Dictionary<string, string>();
            Params.Add("user", user);
            Params.Add("IsSchedule", "True");
            Queries.ExecuteFromStoreProcedure("spx_PAYOUTImportSave", Params);
        }

        return true;
    }


    private static string GetTodaysFolder(string MainPath)
    {
        DateTime currentDate = DateTime.Now;


        List<string> PathHierarchy = new List<string>();
        PathHierarchy.Add(MainPath);
        PathHierarchy.Add(currentDate.ToString("yyyy"));
        PathHierarchy.Add(currentDate.ToString("MMM"));
        PathHierarchy.Add(currentDate.ToString("yyyy-MM-dd"));

        string PathToSearch = string.Join("\\", PathHierarchy);
        if (!Directory.Exists(PathToSearch))
        {
            try
            {
                Directory.CreateDirectory(PathToSearch);
            }
            catch
            {
                PathToSearch = null;
            }
        }

        return PathToSearch;
    }
}