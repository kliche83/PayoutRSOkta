using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using WebApplication3;

namespace Payout
{
    public class FullSalesAuditCls
    {
        public static void SchedBuildCommentSalesAuditFull(string SchedId, string FieldChanged, string user)
        {
            if (ConfigurationManager.AppSettings.Get("UseAuditFullTable") == "true")
            {
                //Get Ids from scheduled changed and get schedules associated to that Id
                string SQLString = string.Format(@" SELECT s.Id SalesId, MAX(Overlaps.Id) SchedId,
                                                (SELECT CONVERT(NVARCHAR(15), StartDate) + ' - ' + CONVERT(NVARCHAR(15), EndDate) FROM PAYOUTschedule WHERE Id = MAX(Overlaps.Id)) SchedRange
                                                FROM PAYOUTschedule sc
                                                Left Join PAYOUTsales s ON 
                                                sc.StoreName = s.StoreName ANd
                                                sc.StoreNumber = s.StoreNumber and
                                                sc.Program = s.Program and
                                                s.SalesDate BETWEEN sc.StartDate AND sc.EndDate
                                                Left Join PAYOUTschedule Overlaps ON 
                                                Overlaps.StoreName = s.StoreName ANd
                                                Overlaps.StoreNumber = s.StoreNumber and
                                                Overlaps.Program = s.Program and
                                                s.SalesDate BETWEEN Overlaps.StartDate AND Overlaps.EndDate
                                                AND Overlaps.Id <> {0}
                                                WHERE sc.Id = {0}
                                                AND s.Id IS NOT NULL
                                                GROUP BY s.Id, s.storename, s.program, s.storenumber, s.salesdate", SchedId);

                DataTable dt = Queries.GetResultsFromQueryString(SQLString);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        AuditSalesIdWithComment(dr[0].ToString(), user,
                            string.Format("Module: Schedule, Method: FieldChanged, Action: {0}",
                                            "Field Changed: " + FieldChanged + " - Schedule Changed: " + SchedId + " - Overlaps: " + dr[1].ToString() + " - Range: " + dr[2].ToString()
                                        )
                            );
                    }
                }
            }            
        }


        public static void AuditExceptions(DataTable dt, string user, string ExceptionType)
        {

            if (ExceptionType == "Pending" && dt.Rows.Count > 0 && ConfigurationManager.AppSettings.Get("UseAuditFullTable") == "true")
            {
                List<string> lstId = dt.AsEnumerable().Select(f => f.Field<int>("Id").ToString()).ToList();

                foreach (string Item in lstId)
                {
                    string strFullAudit = string.Format(@"SELECT 'Module: Exceptions, Method: setSelect, Action:' + 
                                                                CASE WHEN sc.Id IS NULL 
                                                                THEN 'No Schedule Matches' 
                                                                ELSE 'SchedId: ' + CONVERT(NVARCHAR(12), sc.Id) + ' - SchedOwner: ' + sc.OwnerFirstname + ' ' + sc.OwnerLastname + 
                                                                     ' - SchedImportedOn: ' + CONVERT(NVARCHAR(12), sc.ImportedOn) + ' - SchedImportedBy: ' + sc.ImportedBy END
                                                FROM PAYOUTsales s
                                                LEFT JOIN PAYOUTschedule sc ON 
                                                sc.StoreName = s.StoreName AND
                                                sc.Program = s.Program AND
                                                sc.StoreNumber = s.StoreNumber AND
                                                s.SalesDate BETWEEN sc.StartDate AND sc.EndDate
                                                WHERE s.Program IS NOT NULL AND s.Program <> '' AND s.Id = {0}", Item);

                    strFullAudit = Queries.GetResultsFromQueryString(strFullAudit).Rows[0][0].ToString();
                    AuditSalesIdWithComment(Item, user, strFullAudit);
                }
            }
        }


        public static void AuditSalesIdWithComment(string Id, string UserName, string Comment)
        {
            if (Queries.GetResultsFromQueryString("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'PAYOUTsalesAuditFull'").Rows[0][0].ToString() == "1")
            {
                string SQLString = string.Format(@"INSERT INTO PAYOUTsalesAuditFull 
                                                ([SalesId],[Program],[SalesDate],[StoreName],[StoreNumber],[ItemNumber],[ItemName],[Qty],[UnitCost]
                                                ,[ExtendedCost],[OwnerFirstname],[OwnerLastname],[StartDate],[EndDate],[City],[State],[HubFirstname]
                                                ,[HubLastname],[ImportedBy],[ImportedOn],[Archive],[TimeStamp],[UserName],[Comments])

                                                SELECT [Id],[Program],[SalesDate],[StoreName],[StoreNumber],[ItemNumber],[ItemName],[Qty],[UnitCost]
                                                ,[ExtendedCost],[OwnerFirstname],[OwnerLastname],[StartDate],[EndDate],[City],[State],[HubFirstname]
                                                ,[HubLastname],[ImportedBy],[ImportedOn],[Archive], GETDATE(), '{0}', '{1}'
                                                FROM PAYOUTsales
                                                WHERE Id = {2}",
                                                UserName, Comment, Id);

                Queries.ExecuteFromQueryString(SQLString);
            }
        }
    }
}