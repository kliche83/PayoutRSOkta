using System;
using System.Web;
using System.Linq;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.ComponentModel;
using System.Web.UI;
using System.Net.Mail;
using System.Net;
using System.Globalization;
using System.Collections;

namespace WebApplication3
{
    public static class Common
    {
        public static void TotalizeFooter(ref GridView Grv, string ColumnName, bool ApplyCurrency = false)
        {
            int IndexColumn = GetColumnIndexByName(Grv, ColumnName);
            if (IndexColumn != -1)
            {
                decimal total = 0;
                string columnDataType = ((DataTable)Grv.DataSource).Columns[IndexColumn].DataType.Name.ToString().ToLower();

                switch (columnDataType)
                {
                    case "double":
                        total = ((DataTable)Grv.DataSource).AsEnumerable().Sum(row => row.Field<double?>(ColumnName) == null ? 0 : (decimal)row.Field<double>(ColumnName));
                        break;
                    case "decimal":
                        total = ((DataTable)Grv.DataSource).AsEnumerable().Sum(row => row.Field<decimal?>(ColumnName) == null ? 0 : row.Field<decimal>(ColumnName));
                        break;
                    case "int32":
                        total = ((DataTable)Grv.DataSource).AsEnumerable().Sum(row => row.Field<int?>(ColumnName) == null ? 0 : row.Field<int>(ColumnName));
                        break;

                }

                if (columnDataType == "int32")
                {
                    Grv.FooterRow.Cells[IndexColumn].Text = Convert.ToInt32(total).ToString();
                }
                else
                {
                    if (ApplyCurrency)
                        Grv.FooterRow.Cells[IndexColumn].Text = total.ToString("C");
                    else
                        Grv.FooterRow.Cells[IndexColumn].Text = total.ToString("N2");
                }
                

                Grv.FooterRow.Cells[IndexColumn].HorizontalAlign = HorizontalAlign.Right;
            }
        }

        public static void RightAlignCurrencyFormat(ref GridView Grv)
        {
            foreach (DataControlField dc in Grv.Columns)
            {


            }


        }

        public static int GetColumnIndexByName(GridView grid, string name)
        {
            for (int i = 0; i < grid.HeaderRow.Cells.Count; i++)
            {
                if (grid.HeaderRow.Cells[i].Text.ToLower().Trim() == name.ToLower().Trim())
                {
                    return i;
                }
            }
            return -1;
        }

        public static void PopulateWeekendingDDL(ref DropDownList weDDL)
        {
            string SQLString = @" SELECT CONVERT(NVARCHAR(20), [WeekEnding]) [WeekEnding]
                                  FROM [PAYOUTwe]
                                  GROUP BY [WeekEnding] 
                                  ORDER BY [WeekEnding] DESC";

            DataTable dt = Queries.GetResultsFromQueryString(SQLString);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i][0] = ApplyDateFormat(dt.Rows[i][0].ToString());
            }
            weDDL.DataSource = dt;
            weDDL.DataValueField = "WeekEnding";
            weDDL.DataTextField = "WeekEnding";
            weDDL.DataBind();
            weDDL.SelectedIndex = 0;
        }

        public static string ApplyDateFormat(string dateToFormat, int FormatType = 0)
        {
            if (FormatType == 0)
                return Convert.ToDateTime(dateToFormat).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            else
                return Convert.ToDateTime(dateToFormat).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
        }

        public static string ApplyDateFormat(DateTime dateToFormat, int FormatType = 0)
        {
            if (FormatType == 0)
                return Convert.ToDateTime(dateToFormat).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            else
                return Convert.ToDateTime(dateToFormat).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
        }

        public static DataTable MergeAll(this IList<DataTable> tables, string primaryKeyColumn)
        {
            if (!tables.Any())
                throw new ArgumentException("Tables must not be empty", "tables");
            if (primaryKeyColumn != null)
                foreach (DataTable t in tables)
                    if (!t.Columns.Contains(primaryKeyColumn))
                        throw new ArgumentException("All tables must have the specified primarykey column " + primaryKeyColumn, "primaryKeyColumn");

            if (tables.Count == 1)
                return tables[0];

            DataTable table = new DataTable("TblUnion");
            table.BeginLoadData(); // Turns off notifications, index maintenance, and constraints while loading data
            foreach (DataTable t in tables)
            {
                table.Merge(t); // same as table.Merge(t, false, MissingSchemaAction.Add);
            }
            table.EndLoadData();

            if (primaryKeyColumn != null)
            {
                // since we might have no real primary keys defined, the rows now might have repeating fields
                // so now we're going to "join" these rows ...
                var pkGroups = table.AsEnumerable()
                    .GroupBy(r => r[primaryKeyColumn]);
                var dupGroups = pkGroups.Where(g => g.Count() > 1);
                foreach (var grpDup in dupGroups)
                {
                    // use first row and modify it
                    DataRow firstRow = grpDup.First();
                    foreach (DataColumn c in table.Columns)
                    {
                        if (firstRow.IsNull(c))
                        {
                            DataRow firstNotNullRow = grpDup.Skip(1).FirstOrDefault(r => !r.IsNull(c));
                            if (firstNotNullRow != null)
                                firstRow[c] = firstNotNullRow[c];
                        }
                    }
                    // remove all but first row
                    var rowsToRemove = grpDup.Skip(1);
                    foreach (DataRow rowToRemove in rowsToRemove)
                        table.Rows.Remove(rowToRemove);
                }
            }

            return table;
        }

        public static void BindDDLFromTableResults(DataTable dt, string FieldName, ref DropDownList ControlDDL, string SelectedValue = "")
        {
            List<string> lstFilteredDT = new List<string>();

            lstFilteredDT = dt.AsEnumerable()
                .Where(t => t.Field<string>(FieldName).Trim() != "")
                .Select(t => t.Field<string>(FieldName)).ToList();
            lstFilteredDT = lstFilteredDT.Distinct().ToList();
            lstFilteredDT.Sort();
            lstFilteredDT.Insert(0, "All");

            ControlDDL.DataSource = lstFilteredDT;

            ControlDDL.DataBind();
            if (!string.IsNullOrWhiteSpace(Convert.ToString(SelectedValue)))
            {
                if (lstFilteredDT.Where(s => s.IndexOf(SelectedValue) == 0).Count() > 0)
                {
                    try
                    {
                        ControlDDL.SelectedValue = SelectedValue;
                    }
                    catch { }
                }
            }
        }

        public static void StartEndDateFilters(ref string StartDate, ref string EndDate)
        {
            if (string.IsNullOrWhiteSpace(StartDate))
            {
                DateTime dt = new DateTime(DateTime.Now.Year, 1, 1);
                StartDate = ApplyDateFormat(dt.ToString());
            }

            if (string.IsNullOrWhiteSpace(EndDate))
            {
                EndDate = ApplyDateFormat(DateTime.Now.ToString());
            }

        }

        public static void ApplyRowsfilterDatatable(ref DataTable dt, string FieldName, string Value)
        {
            try
            {
                int columnId = (dt.Columns[FieldName]).Ordinal;
                string ColumnType = dt.Columns[columnId].DataType.Name.ToLower();

                switch (ColumnType)
                {
                    case "string":
                        dt = dt.AsEnumerable()
                            .Where(row => row.Field<string>(FieldName).ToLower().Contains(Value.ToLower().Trim())).CopyToDataTable();
                        break;
                    case "double":
                        dt = dt.AsEnumerable()
                            .Where(row => row.Field<double>(FieldName).ToString().Contains(Value.ToLower().Trim())).CopyToDataTable();
                        break;
                }
            }
            catch (Exception ex)
            {
                dt = new DataTable();
            }
        }

        public static void ExportToExcel_List(List<DataTable> lstDt, string fileName, bool WithStyle = true)
        {
            string attachment = string.Format("attachment; filename=\"" + fileName + " {0}.xls\"", ApplyDateFormat(DateTime.Now));
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.Buffer = true;
            //HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            HttpContext.Current.Response.ContentType = "text/xml";
            
            //HttpContext.Current.Response.ContentType = "text/plain";

            HttpContext.Current.Response.Write(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">");
            HttpContext.Current.Response.AddHeader("Content-Disposition", attachment);
            HttpContext.Current.Response.Charset = "utf-8";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1250");
            HttpContext.Current.Response.Write(WithStyle ? "<style> .amt{mso-number-format:\"\\#\\#0\\.00\";} </style>" : "");

            foreach (DataTable dt in lstDt)
            {
                int columnscount = dt.Columns.Count;
                
                HttpContext.Current.Response.Write(WithStyle ? @"<Table border='1' bgColor='#ffffff' 
                                               borderColor='#000000' cellSpacing='0' cellPadding='0' 
                                               style='font-size:10.0pt; font-family:Calibri; background:white;'>" : "<Table>");

                HttpContext.Current.Response.Write("<TR>");
                for (int j = 0; j < columnscount; j++)
                {
                    //write in new column
                    if (j == 0)
                        HttpContext.Current.Response.Write(WithStyle ? "<Td style='font-weight:bold' align='left'>" : "<Td>");
                    else
                        HttpContext.Current.Response.Write(WithStyle ? "<Td style='font-weight:bold' align='center'>" : "<Td>");
                    //Get column headers  and make it as bold in excel columns
                    HttpContext.Current.Response.Write("");
                    HttpContext.Current.Response.Write(dt.Columns[j].ColumnName.ToString());
                    HttpContext.Current.Response.Write("");
                    HttpContext.Current.Response.Write("</Td>");
                }
                HttpContext.Current.Response.Write("</TR>");

                foreach (DataRow row in dt.Rows)
                {//write in new row
                    HttpContext.Current.Response.Write("<TR>");
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (i == 0)
                            HttpContext.Current.Response.Write(WithStyle ? "<Td style='font-weight:bold' align='left'>" : "<Td>");
                        else
                            HttpContext.Current.Response.Write(WithStyle ? "<Td align='center'>" : "<Td>");

                        HttpContext.Current.Response.Write(row[i].ToString());
                        HttpContext.Current.Response.Write("</Td>");
                    }
                    HttpContext.Current.Response.Write("</TR>");
                }
                HttpContext.Current.Response.Write("</Table>");

                HttpContext.Current.Response.Write("<BR><BR>");
            }

            //HttpContext.Current.Response.Write("</font>");
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        public static DataTable PivotTable(DataTable SourceDT, List<string> ColumnOrder)
        {
            DataTable ResultTable = new DataTable();
            List<string> ArrayRow;

            for (int i = 0; i < SourceDT.Rows.Count + 1; i++)
            {
                DataColumn dc = new DataColumn();
                dc.ColumnName = i.ToString();
                ResultTable.Columns.Add(dc);
            }


            int countColumnOrderPosition = 0;
            foreach (string ColName in ColumnOrder)
            {
                int Columnindex = SourceDT.Columns[ColName].Ordinal;
                ArrayRow = new List<string>();

                ArrayRow.Add(ColumnOrder[countColumnOrderPosition]);
                countColumnOrderPosition++;

                for (int RowIndex = 0; RowIndex < SourceDT.Rows.Count; RowIndex++)
                {
                    ArrayRow.Add(SourceDT.Rows[RowIndex][Columnindex].ToString());
                }
                ResultTable.Rows.Add(ArrayRow.ToArray());
            }

            return ResultTable;
        }

        /// <summary>
        /// Convert Any kind of list to datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties =
            TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public static List<T> CloneList<T>(List<T> oldList)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, oldList);
            stream.Position = 0;
            return (List<T>)formatter.Deserialize(stream);
        }

        /// <summary>
        /// Get selected values from all filled page dropdownlists
        /// </summary>
        /// <param name="SQLParameters"></param>
        /// <param name="AllPage"></param>
        public static void filtersFromPageDDLs(ref Dictionary<string, string> SQLParameters, Page AllPage)
        {
            //Method comes from custom class ControlUtilities
            var allDropDowns = AllPage.FindDescendants<DropDownList>();
            var lstDropDownWithCustomClass = allDropDowns.Where(
                ddl => ddl.CssClass.Contains("FilterControl")
                ).ToList();

            foreach (DropDownList ddl in lstDropDownWithCustomClass)
            {
                if (ddl != null && ddl.SelectedValue != null && ddl.SelectedValue != "")
                    SQLParameters.Add(ddl.ID, ddl.SelectedValue);
                else
                    SQLParameters.Add(ddl.ID, "NULL");
            }
        }

        public static void filtersFromPageTextboxes(ref Dictionary<string, string> SQLParameters, Page AllPage)
        {
            //Method comes from custom class ControlUtilities
            var allTextboxes = AllPage.FindDescendants<TextBox>();
            var lstTextboxWithCustomClass = allTextboxes.Where(
                txt => txt.CssClass.Contains("FilterControl")
                ).ToList();

            foreach (TextBox txt in lstTextboxWithCustomClass)
            {
                if (txt != null && txt.Text.Trim() != "")
                    SQLParameters.Add(txt.ID, txt.Text);
                else
                    SQLParameters.Add(txt.ID, "NULL");
            }
        }

        public static void filtersFromPageCheckboxes(ref Dictionary<string, string> SQLParameters, Page AllPage)
        {
            //Method comes from custom class ControlUtilities
            var allCheckboxes = AllPage.FindDescendants<CheckBox>();
            var lstCheckBoxWithCustomClass = allCheckboxes.Where(
                chk => chk.CssClass.Contains("FilterControl")
                ).ToList();

            foreach (CheckBox chk in lstCheckBoxWithCustomClass)
            {
                if (chk != null)
                    SQLParameters.Add(chk.ID, chk.Checked.ToString());
                else
                    SQLParameters.Add(chk.ID, "NULL");
            }
        }

        public static void CellFormating(ref GridViewRowEventArgs e, List<string> DecimalCells, string CellFormat, string HighlightedColumn = null)
        {
            if (e.Row.Cells.Count > 1)
            {
                if (e.Row.RowType != DataControlRowType.Header)
                {
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {

                        if (HighlightedColumn != null && ((BoundField)((DataControlFieldCell)((e.Row.Cells[i]))).ContainingField).HeaderText == HighlightedColumn)
                            e.Row.Cells[i].CssClass = "ColumnCategory";

                        if ((e.Row.Cells[i]).Text != "&nbsp;")
                        {
                            if (DecimalCells.Contains(((BoundField)((DataControlFieldCell)((e.Row.Cells[i]))).ContainingField).DataField))
                            {
                                decimal result = 0;
                                decimal.TryParse((e.Row.Cells[i]).Text, out result);

                                if (CellFormat == "Currency")
                                    (e.Row.Cells[i]).Text = result.ToString("C");
                                else if (CellFormat == "Integer")
                                    (e.Row.Cells[i]).Text = Convert.ToInt32(result).ToString();

                                if (result < 0)
                                    (e.Row.Cells[i]).Style.Add("color", "Red");

                                e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                            }
                            else
                            {
                                e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Center;
                            }
                        }
                    }
                }
            }
        }

        public static void TurnOffControlsFromGrid(GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Controls.Count > 0)
                    {
                        Control FoundControl = null;
                        foreach (Control ctrl in e.Row.Cells[i].Controls)
                        {
                            if (ctrl.GetType().Name == "TextBox" || ctrl.GetType().Name == "DropDownList")
                            {
                                FoundControl = ctrl;
                            }
                        }

                        if (FoundControl != null)
                        {
                            Literal NewLt = new Literal();

                            switch (FoundControl.GetType().Name)
                            {
                                case "TextBox":
                                    NewLt.Text = ((TextBox)FoundControl).Text;
                                    break;
                                case "DropDownList":
                                    NewLt.Text = ((DropDownList)FoundControl).SelectedValue;
                                    break;
                            }
                            e.Row.Cells[i].Controls.Remove(FoundControl);
                            e.Row.Cells[i].Controls.Add(NewLt);
                        }
                    }
                }
            }
        }        

        /// <summary>
        /// Perform union all of a list of Datatables with same number of columns
        /// </summary>
        /// <param name="LstDT"></param>
        /// <returns></returns>
        public static DataTable UnionAllDatatables(List<DataTable> LstDT)
        {
            return LstDT
                    .Select(d => d.Select().AsEnumerable())
                    .Aggregate((current, next) => current.Union(next))
                    .CopyToDataTable<DataRow>();
        }

        public static string GetUserSession()
        {
            try
            {
                return HttpContext.Current.Session["user"].ToString();
            }
            catch
            {
                HttpContext.Current.Response.Write("<script>window.top.location = '../';</script>");
            }

            return null;
        }

        public static void SendEmailFromDataSet(DataSet ds, string sendToEmail, string Subject)
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "mail.innovageusa.com";
            smtpClient.Port = 587; //or 25
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = false;
            smtpClient.Credentials = new NetworkCredential("salesreport@innovageusa.com", "SalesReport#13");
            smtpClient.UseDefaultCredentials = false;

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("salesreports@thesmartcircle.com", "Sales Reports");            
            mail.To.Add(new MailAddress(sendToEmail));
            mail.Subject = Subject;
            mail.IsBodyHtml = true;

            mail.Body += "<style> " +
                         "span { font-family: 'Segoe UI'; font-weight: normal; } " +
                         "table { border: 1px solid #000; border-collapse:collapse; font-family: 'Segoe UI'; font-weight: normal; } " +
                         "th { border: 1px solid #000; text-align: left; background-color: #328EFE; color: #fff; padding: 0 5px; font-family: 'Segoe UI'; font-weight: normal; white-space: nowrap; } " +
                         "td { border: 1px solid #000; text-align: left; padding: 0 5px; font-family: 'Segoe UI'; font-weight: normal; white-space: nowrap; } " +
                         ".right { text-align: right; } " +
                         "tr:last-child, tr:nth-last-child(2) { background-color: #BBDAFF; } " +
                         "</style>";

            foreach (DataTable dt in ds.Tables)
            {
                mail.Body += CastDatatableToHTML(dt);
                mail.Body += "<br/><br/>";
            }

            smtpClient.Send(mail);
            System.Threading.Thread.Sleep(700);
        }

        private static string CastDatatableToHTML(DataTable dt)
        {
            DataGrid dg = new DataGrid();
            dg.DataSource = dt;
            dg.DataBind();

            StringWriter sw = new StringWriter();
            HtmlTextWriter w = new HtmlTextWriter(sw);
            dg.RenderControl(w);

            return sw.ToString();
        }
        
        public static void ExportCSVFile(string FolderPath, string ModuleName, string userFullName, Dictionary<string, string> Params, string SPName, string ReturnParam = null)
        {            
            string FileName = ModuleName + "_" + userFullName + "_" + ApplyDateFormat(DateTime.Now) + ".csv";
            
            if (!Directory.Exists(FolderPath))
            {
                try
                {
                    Directory.CreateDirectory(FolderPath);

                }
                catch (Exception ex)
                {
                    FolderPath = null;
                }
            }

            if (File.Exists(FolderPath + FileName))
            {
                File.Delete(FolderPath + FileName);
            }
            if (FolderPath != null)
            {
                if(ReturnParam == null)
                    Queries.CSVFileDownloadFromSP(SPName, ref Params, ",", FolderPath + FileName);
                else
                    Queries.CSVFileDownloadFromSP(SPName, ref Params, ",", FolderPath + FileName, ReturnParam);

                DownloadFileFromServer(FolderPath, FileName);
            }
        }

        public static void DownloadFileFromServer(string serverFolderPath, string fileName)
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

        public static void PopulatePager(int recordCount, int currentPage, int PagesToDisplay, string PageSize, ref Repeater rptPager)
        {
            double dblPageCount = (double)(recordCount / decimal.Parse(PageSize));
            int pageCount = (int)Math.Ceiling(dblPageCount);
            List<ListItem> pages = new List<ListItem>();
            if (pageCount > 0)
            {
                pages.Add(new ListItem("First", "1", currentPage > 1));

                int startPage, endPage;

                if (PagesToDisplay > pageCount)
                {
                    startPage = 1;
                    endPage = PagesToDisplay;

                    for (int i = 1; i <= pageCount; i++)
                    {
                        pages.Add(new ListItem(i.ToString(), i.ToString(), i != currentPage));
                    }
                }
                else
                {
                    if (currentPage + PagesToDisplay - 1 > pageCount)
                    {
                        startPage = pageCount - PagesToDisplay;
                        endPage = pageCount;
                    }
                    else if (currentPage > PagesToDisplay - 2)
                    {
                        startPage = currentPage - 2;
                        endPage = currentPage + PagesToDisplay - 2;
                    }
                    else if (currentPage < PagesToDisplay / 2)
                    {
                        startPage = 1;
                        endPage = PagesToDisplay;
                    }
                    else
                    {
                        startPage = currentPage - 2;
                        endPage = currentPage + PagesToDisplay - 2;
                    }

                    for (int i = startPage; i <= endPage; i++)
                    {
                        pages.Add(new ListItem(i.ToString(), i.ToString(), i != currentPage));
                    }
                }

                pages.Add(new ListItem("Last", pageCount.ToString(), currentPage < pageCount));
            }
            rptPager.DataSource = pages;
            rptPager.DataBind();
        }


        /// <summary>
        /// Parse the invoking object to string to send as URL
        /// </summary>
        /// <param name="request"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ObjectToQueryString(this object request, string separator = ",")
        {
            if (request == null)
                throw new ArgumentNullException("request");

            // Get all properties on the object
            var properties = request.GetType().GetProperties()
                .Where(x => x.CanRead)
                .Where(x => x.GetValue(request, null) != null)
                .ToDictionary(x => x.Name, x => x.GetValue(request, null));

            // Get names for all IEnumerable properties (excl. string)
            var propertyNames = properties
                .Where(x => !(x.Value is string) && x.Value is IEnumerable)
                .Select(x => x.Key)
                .ToList();

            // Concat all IEnumerable properties into a comma separated string
            foreach (var key in propertyNames)
            {
                var valueType = properties[key].GetType();
                var valueElemType = valueType.IsGenericType
                                        ? valueType.GetGenericArguments()[0]
                                        : valueType.GetElementType();
                if (valueElemType.IsPrimitive || valueElemType == typeof(string))
                {
                    var enumerable = properties[key] as IEnumerable;
                    properties[key] = string.Join(separator, enumerable.Cast<object>());
                }
            }

            // Concat all key/value pairs into a string separated by ampersand
            return string.Join("&", properties
                .Select(x => string.Concat(
                    Uri.EscapeDataString(x.Key), "=",
                    Uri.EscapeDataString(x.Value.ToString()))));
        }

    }
}