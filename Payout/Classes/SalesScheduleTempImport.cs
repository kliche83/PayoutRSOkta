using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Configuration;

namespace WebApplication3
{
    public class SalesScheduleTempImport
    {
        private static void ImportDataFromExcel(string Path, string StoreName, string User)
        {
            StringBuilder sb = new StringBuilder();

            if (StoreName != "Schedule")
            {
                DataTable dt = GetTableFromExcelRecords(Path, StoreName, User);
                bool flagFirstRecord = true;

                sb.Append("INSERT INTO [PAYOUTsalesTemp] (SalesDate, StoreName, StoreNumber, ItemNumber, ItemName, Qty, UnitCost, ExtendedCost) ");
                foreach (DataRow dr in dt.Rows)
                {
                    if (flagFirstRecord)
                        flagFirstRecord = false;
                    else
                        sb.Append(" UNION ALL ");

                    sb.Append(" SELECT '");
                    sb.Append(dr["SalesDate"]);
                    sb.Append("' SalesDate, '");

                    sb.Append(dr["StoreName"]);
                    sb.Append("' StoreName, '");

                    sb.Append(dr["StoreNumber"]);
                    sb.Append("' StoreNumber, '");

                    sb.Append(dr["ItemNumber"]);
                    sb.Append("' ItemNumber, '");

                    sb.Append(dr["ItemName"].ToString().Replace("'", "''"));
                    sb.Append("' ItemName, ISNULL(");

                    sb.Append(dr["Qty"]);
                    sb.Append(", 0) Qty, ISNULL(");

                    sb.Append(dr["UnitCost"]);
                    sb.Append(", 0) UnitCost, ISNULL(");

                    sb.Append(dr["ExtendedCost"]);
                    sb.Append(", 0) ExtendedCost");
                }
            }

            string test = sb.ToString();
            //Queries.ExecuteFromQueryString(sb.ToString());


        }

        public static DataTable GetTableFromExcelRecords(string ExcelFilePath, string StoreName, string UserName)
        {
            DataTable OriginalFile = ImportExceltoDatatable(ExcelFilePath);
            string SQLString = "SELECT TOP 0 [PAYOUTsalesTemp].* FROM [PAYOUTsalesTemp] WHERE 1 = 2;";
            DataTable salesTempColumnOrderedFile = Queries.GetResultsFromQueryString(SQLString);

            SQLString = "SELECT TOP 0 [PAYOUTscheduleTemp].* FROM [PAYOUTscheduleTemp] WHERE 1 = 2;";
            DataTable scheduleTempColumnOrderedFile = Queries.GetResultsFromQueryString(SQLString);

            DataTable dt;

            foreach (DataColumn dc in OriginalFile.Columns)
            {
                dc.ColumnName = dc.ColumnName.Trim();
            }
            //DataColumn dc = new DataColumn()
            //ColumnOrderedFile.Columns.Add

            switch (StoreName)
            {
                case "CanadianTire":

                    foreach (DataRow dr in OriginalFile.Rows)
                    {
                        DataRow drOrdered = salesTempColumnOrderedFile.NewRow();
                        drOrdered["SalesDate"] = dr["Date"];
                        drOrdered["StoreName"] = StoreName;
                        drOrdered["StoreNumber"] = dr["Store"];
                        drOrdered["ItemNumber"] = dr["Item"];
                        drOrdered["ItemName"] = dr["Product"];
                        drOrdered["Qty"] = dr["Qty"];
                        drOrdered["UnitCost"] = dr["Unit PP"];
                        drOrdered["ExtendedCost"] = dr["Total Sales"];

                        salesTempColumnOrderedFile.Rows.Add(drOrdered);
                    }
                    break;
                case "WinnDixie":
                    foreach (DataRow dr in OriginalFile.Rows)
                    {
                        DataRow drOrdered = salesTempColumnOrderedFile.NewRow();
                        drOrdered["SalesDate"] = dr["Division"];
                        drOrdered["StoreName"] = StoreName;
                        drOrdered["StoreNumber"] = dr["Store Number"];
                        drOrdered["ItemNumber"] = dr["Item Number"];
                        drOrdered["ItemName"] = dr["Description"];
                        drOrdered["Qty"] = dr["QS - Gross Sales"];
                        drOrdered["UnitCost"] = dr["Cost"];
                        drOrdered["ExtendedCost"] = dr["Extended Cost"];

                        salesTempColumnOrderedFile.Rows.Add(drOrdered);
                    }
                    break;
                case "Meijer":
                    foreach (DataRow dr in OriginalFile.Rows)
                    {
                        DataRow drOrdered = salesTempColumnOrderedFile.NewRow();
                        drOrdered["SalesDate"] = dr["Date"];
                        drOrdered["StoreName"] = StoreName;
                        drOrdered["StoreNumber"] = dr["Store"];
                        drOrdered["ItemNumber"] = dr["Item"];
                        drOrdered["ItemName"] = dr["Product"];
                        drOrdered["Qty"] = dr["Qty"];
                        drOrdered["UnitCost"] = dr["Total Sales"];
                        drOrdered["ExtendedCost"] = dr["Unit PP"];

                        salesTempColumnOrderedFile.Rows.Add(drOrdered);
                    }
                    break;
                case "SamCreditCard":
                    foreach (DataRow dr in OriginalFile.Rows)
                    {
                        DataRow drOrdered = salesTempColumnOrderedFile.NewRow();
                        drOrdered["SalesDate"] = dr["Date"];
                        drOrdered["StoreName"] = StoreName;
                        drOrdered["StoreNumber"] = dr["Store"];
                        drOrdered["ItemNumber"] = dr["Item"];
                        drOrdered["ItemName"] = dr["Product"];
                        drOrdered["Qty"] = dr["Qty"];
                        drOrdered["UnitCost"] = dr["Total Sales"];
                        drOrdered["ExtendedCost"] = dr["Unit PP"];

                        salesTempColumnOrderedFile.Rows.Add(drOrdered);
                    }
                    break;
                case "SamCypressCreek":
                    foreach (DataRow dr in OriginalFile.Rows)
                    {
                        DataRow drOrdered = salesTempColumnOrderedFile.NewRow();
                        drOrdered["SalesDate"] = dr["Date"];
                        drOrdered["StoreName"] = StoreName;
                        drOrdered["StoreNumber"] = dr["Store"];
                        drOrdered["ItemNumber"] = dr["Item"];
                        drOrdered["ItemName"] = dr["Product"];
                        drOrdered["Qty"] = dr["Qty"];
                        drOrdered["UnitCost"] = dr["Total Sales"];
                        drOrdered["ExtendedCost"] = dr["Unit PP"];

                        salesTempColumnOrderedFile.Rows.Add(drOrdered);
                    }
                    break;
                case "SamZeroEnergy":
                    foreach (DataRow dr in OriginalFile.Rows)
                    {
                        DataRow drOrdered = salesTempColumnOrderedFile.NewRow();
                        drOrdered["SalesDate"] = dr["Date"];
                        drOrdered["StoreName"] = StoreName;
                        drOrdered["StoreNumber"] = dr["Store"];
                        drOrdered["ItemNumber"] = dr["Item"];
                        drOrdered["ItemName"] = dr["Product"];
                        drOrdered["Qty"] = dr["Qty"];
                        drOrdered["UnitCost"] = dr["Total Sales"];
                        drOrdered["ExtendedCost"] = dr["Unit PP"];

                        salesTempColumnOrderedFile.Rows.Add(drOrdered);
                    }
                    break;
                case "HEB":
                    foreach (DataRow dr in OriginalFile.Rows)
                    {
                        DataRow drOrdered = salesTempColumnOrderedFile.NewRow();
                        drOrdered["SalesDate"] = dr["Receipt date"];
                        drOrdered["StoreName"] = StoreName;
                        drOrdered["StoreNumber"] = dr["Location ID"];
                        drOrdered["ItemNumber"] = dr["UPC"];
                        drOrdered["ItemName"] = dr["Description"];
                        drOrdered["Qty"] = dr["Final quantity paid"];
                        drOrdered["UnitCost"] = dr["HEB cost"];
                        drOrdered["ExtendedCost"] = dr["Final extended"];

                        salesTempColumnOrderedFile.Rows.Add(drOrdered);
                    }
                    break;
                case "Costco":
                    foreach (DataRow dr in OriginalFile.Rows)
                    {
                        DataRow drOrdered = salesTempColumnOrderedFile.NewRow();
                        drOrdered["SalesDate"] = dr["sales-date"];
                        drOrdered["StoreName"] = StoreName;
                        drOrdered["StoreNumber"] = dr["warehouse"];
                        drOrdered["ItemNumber"] = dr["item"];
                        drOrdered["ItemName"] = dr["item-description"];
                        drOrdered["Qty"] = dr["quanitity"];
                        drOrdered["UnitCost"] = dr["unit-cost"];
                        drOrdered["ExtendedCost"] = dr["extended-cost"];

                        salesTempColumnOrderedFile.Rows.Add(drOrdered);
                    }
                    break;
                case "Kroger":
                    dt = Queries.GetResultsFromQueryString("SELECT RegionId, Region FROM PAYOUTkrogerRegions");

                    foreach (DataRow dr in OriginalFile.Rows)
                    {
                        //Get kroger Region Id
                        string result = Regex.Match(dr["RPT_SHORT_DESC"].ToString(), @"\d+").ToString();

                        //Find region from table krogerRegions by Id
                        result = dt.AsEnumerable()
                                    .Where(f => f.Field<string>("RegionId") == result)
                                    .Select(f => f.Field<string>("Region")).FirstOrDefault();

                        DataRow drOrdered = salesTempColumnOrderedFile.NewRow();
                        drOrdered["SalesDate"] = dr["DATE_KEY"];
                        drOrdered["StoreName"] = string.Format("{0} - {1}", StoreName, result);
                        drOrdered["StoreNumber"] = dr["RE_STO_NUM"];
                        drOrdered["ItemNumber"] = dr["ITM_SCN_CD"];
                        drOrdered["ItemName"] = dr["ITEM_DESCRIPTION"];
                        drOrdered["Qty"] = dr["SCAN_UNITS"];
                        drOrdered["UnitCost"] = dr["AVE_UNIT_PRC"];
                        drOrdered["ExtendedCost"] = dr["SCAN_DOLLARS"];

                        salesTempColumnOrderedFile.Rows.Add(drOrdered);
                    }
                    break;
                case "KrogerC":
                    dt = Queries.GetResultsFromQueryString("SELECT RegionId, Region FROM PAYOUTkrogerRegions");

                    foreach (DataRow dr in OriginalFile.Rows)
                    {
                        //Find region from table krogerRegions by Id
                        string result = dt.AsEnumerable()
                                        .Where(f => f.Field<string>("RegionId") == dr["Div"].ToString())
                                        .Select(f => f.Field<string>("Region")).FirstOrDefault();

                        DataRow drOrdered = salesTempColumnOrderedFile.NewRow();
                        drOrdered["SalesDate"] = dr["Date"];
                        drOrdered["StoreName"] = string.Format("{0} - {1}", StoreName, result);
                        drOrdered["StoreNumber"] = dr["Store"];
                        drOrdered["ItemNumber"] = dr["UPC"];
                        drOrdered["ItemName"] = dr["UPC Sell Unit Desc"];
                        drOrdered["Qty"] = dr["Qty Sold"];
                        drOrdered["UnitCost"] = "0";
                        drOrdered["ExtendedCost"] = dr["Sales"];

                        salesTempColumnOrderedFile.Rows.Add(drOrdered);
                    }
                    break;
                case "Schedule":

                    foreach (DataRow dr in OriginalFile.Rows)
                    {
                        DataRow drOrdered = salesTempColumnOrderedFile.NewRow();
                        drOrdered["Program"] = dr["Programs"];
                        drOrdered["StartDate"] = dr["Start Date"];
                        drOrdered["EndDate"] = dr["End Date"];
                        drOrdered["StoreName"] = dr["Retailer"];
                        drOrdered["StoreNumber"] = dr["Club #"];
                        drOrdered["City"] = dr["City"];
                        drOrdered["State"] = dr["State"];
                        drOrdered["OwnerFirstname"] = dr["Payout Owner First Name"];
                        drOrdered["OwnerLastname"] = dr["Payout Owner Last Name"];
                        drOrdered["HubFirstname"] = dr["Payout HUB First Name"];
                        drOrdered["HubLastname"] = dr["Payout HUB Last Name"];
                        drOrdered["eventID"] = dr["eventID"];
                        drOrdered["ImportedOn"] = DateTime.Now.ToString();
                        drOrdered["Archive"] = "0";

                        scheduleTempColumnOrderedFile.Rows.Add(drOrdered);
                    }

                    for (int i = 0; i < scheduleTempColumnOrderedFile.Rows.Count; i++)
                    {
                        scheduleTempColumnOrderedFile.Rows[i]["ImportedBy"] = UserName;
                    }

                    break;
                case "BJs":
                    foreach (DataRow dr in OriginalFile.Rows)
                    {
                        DataRow drOrdered = salesTempColumnOrderedFile.NewRow();
                        drOrdered["SalesDate"] = dr["Date"];
                        drOrdered["StoreName"] = StoreName;
                        drOrdered["StoreNumber"] = dr["Club"];
                        drOrdered["ItemNumber"] = dr["Article"];
                        drOrdered["ItemName"] = dr["Article Description"];
                        drOrdered["Qty"] = dr["Sales Qty"];
                        drOrdered["UnitCost"] = "0";
                        drOrdered["ExtendedCost"] = dr["Sales $"];

                        salesTempColumnOrderedFile.Rows.Add(drOrdered);
                    }
                    break;
                case "Sams":
                    List<saleSamsWalmart> lstSalesS = SamsWalmartUnpivotSales(OriginalFile, true);
                    foreach (saleSamsWalmart objSales in lstSalesS)
                    {
                        DataRow drOrdered = salesTempColumnOrderedFile.NewRow();

                        drOrdered["SalesDate"] = objSales.salesDate;
                        drOrdered["StoreName"] = StoreName;
                        drOrdered["StoreNumber"] = objSales.warehouse;
                        drOrdered["ItemNumber"] = objSales.item;
                        drOrdered["ItemName"] = objSales.itemDescription;
                        drOrdered["Qty"] = objSales.quantity;
                        drOrdered["UnitCost"] = "0";
                        drOrdered["ExtendedCost"] = objSales.extendedCost;

                        salesTempColumnOrderedFile.Rows.Add(drOrdered);
                    }
                    break;
                case "WalMart":
                    List<saleSamsWalmart> lstSalesW = SamsWalmartUnpivotSales(OriginalFile, true);
                    foreach (saleSamsWalmart objSales in lstSalesW)
                    {
                        DataRow drOrdered = salesTempColumnOrderedFile.NewRow();

                        drOrdered["SalesDate"] = objSales.salesDate;
                        drOrdered["StoreName"] = StoreName;
                        drOrdered["StoreNumber"] = objSales.warehouse;
                        drOrdered["ItemNumber"] = objSales.item;
                        drOrdered["ItemName"] = objSales.itemDescription;
                        drOrdered["Qty"] = objSales.quantity;
                        drOrdered["UnitCost"] = "0";
                        drOrdered["ExtendedCost"] = objSales.extendedCost;

                        salesTempColumnOrderedFile.Rows.Add(drOrdered);
                    }
                    break;
            }

            for (int i = 0; i < salesTempColumnOrderedFile.Rows.Count; i++)
            {
                salesTempColumnOrderedFile.Rows[i]["ImportedBy"] = UserName;
            }

            return salesTempColumnOrderedFile;
        }

        public static DataTable ImportExceltoDatatable(string ExcelFilePath)
        {
            DataSet ds = new DataSet();
            string constring = ConfigurationManager.ConnectionStrings["conXLSX"].ConnectionString;
            constring = string.Format(constring, ExcelFilePath, "Yes");

            DataTable dt = new DataTable();
            using (OleDbConnection con = new OleDbConnection(constring))
            {
                con.Open();
                string sqlquery = string.Format("SELECT * FROM [{0}]", con.GetSchema("Tables").Rows[0]["TABLE_NAME"].ToString());
                OleDbDataAdapter da = new OleDbDataAdapter(sqlquery, con);
                da.Fill(ds);
                dt = ds.Tables[0];
            }

            return dt;
        }


        private static void InsertToTempTable()
        {
            //using (var bulkCopy = new SqlBulkCopy(_connection.ConnectionString, SqlBulkCopyOptions.KeepIdentity))
            //{
            //    // my DataTable column names match my SQL Column names, so I simply made this loop. However if your column names don't match, just pass in which datatable name matches the SQL column name in Column Mappings
            //    foreach (DataColumn col in table.Columns)
            //    {
            //        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
            //    }

            //    bulkCopy.BulkCopyTimeout = 600;
            //    bulkCopy.DestinationTableName = destinationTableName;
            //    bulkCopy.WriteToServer(table);
            //}
        }


        public static List<saleSamsWalmart> SamsWalmartUnpivotSales(DataTable dt, bool IsSams)
        {
            bool IsQty = true;
            List<saleSamsWalmart> lstSale = new List<saleSamsWalmart>();
            DataTable DatatableToUnpivot = dt;

            foreach (DataRow dr in DatatableToUnpivot.Rows)     //DatatableToUnpivot is the original datatable      
            {
                saleSamsWalmart objSale = new saleSamsWalmart();
                objSale.item = dr["Item Nbr"].ToString();
                objSale.itemDescription = dr["Item Desc 1"].ToString() + " " + dr["Item Desc 1"].ToString();

                if (IsSams)
                    objSale.warehouse = int.Parse(dr["Club Nbr"].ToString());
                else
                    objSale.warehouse = int.Parse(dr["Store Nbr"].ToString());


                foreach (DataColumn dc in DatatableToUnpivot.Columns)
                {
                    if ((IsSams && dc.Ordinal >= 6) || (!IsSams && dc.Ordinal >= 7))//From this id starts columns to unpivot
                    {
                        string numericValuetCast = Regex.Match(dr[dc].ToString(), @"-?\d{1,3}(,\d{3})*(\.\d+)?").Value;
                        if (IsQty)
                        {
                            objSale.quantity = int.Parse(numericValuetCast);
                            objSale.salesDate = SamsWalmartGetDateFromColumn(dc.Caption);
                            IsQty = false;
                        }
                        else
                        {
                            objSale.extendedCost = decimal.Parse(numericValuetCast);
                            IsQty = true;

                            lstSale.Add(new saleSamsWalmart()
                            {
                                item = objSale.item,
                                itemDescription = objSale.itemDescription,
                                warehouse = objSale.warehouse,
                                quantity = objSale.quantity,
                                extendedCost = objSale.extendedCost,
                                salesDate = objSale.salesDate
                            });
                        }
                    }
                }
            }

            return lstSale;
        }

        public static DateTime SamsWalmartGetDateFromColumn(string ColumnString) //Example of ColumnString: 201702 Sat Qty
        {
            int YearPart = int.Parse(ColumnString.Substring(0, 4));
            int weekPart = int.Parse(ColumnString.Substring(4, 2));
            string DayPart = ColumnString.Substring(7, 3).ToLower();
            DateTime endOfMonth = new DateTime(YearPart, 2, 1).AddDays(-1); //Get last day of January from Column Year

            while (endOfMonth.DayOfWeek != DayOfWeek.Saturday)
            {
                endOfMonth = endOfMonth.AddDays(-1); //Get Last Saturday of January
            }

            bool runWhile = true;
            int DayNumber = 0;

            while (runWhile)
            {
                if (endOfMonth.ToString("ddd").ToLower() != DayPart)
                {
                    if (DayNumber >= 7)
                    {
                        endOfMonth = DateTime.MinValue; //DayPart is not well written
                        runWhile = false;
                    }
                    else
                        endOfMonth = endOfMonth.AddDays(1);
                }
                else
                {
                    endOfMonth = endOfMonth.AddDays(weekPart * 7); //Add weeks
                    runWhile = false;
                }

                DayNumber++;
            }

            return endOfMonth;
        }

        public class saleSamsWalmart
        {
            public DateTime salesDate { get; set; }
            public int quantity { get; set; }
            public string itemDescription { get; set; }
            public string item { get; set; }
            public int warehouse { get; set; }
            public decimal extendedCost { get; set; }
        }
    }
}