using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication3
{
    public partial class ReportMaker : System.Web.UI.Page
    {
        string user = string.Empty;
        string userType = string.Empty;
        string userFUllname = string.Empty;
        string StartDate = string.Empty;
        string OwnerName = string.Empty;
        string StoreName = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                user = Session["user"].ToString();
                userType = Session["userType"].ToString();
                userFUllname = Session["userFullname"].ToString();
                //Response.Write("<script>window.top.location = '../" + userFUllname + "';</script>");

                if (userType != "Owner" && userType != "Hub")
                {
                    userFUllname = "";
                }
            }
            catch
            {
                Response.Write("<script>window.top.location = '../';</script>");
            }

            if (userType == "Owner")
            {
                typeSpan.Visible = false;
                owDDLspan.Visible = false;
            }

            if (userType == "Hub" || userType == "NC")
            {
                typeSpan.Visible = false;
            }
        }

        protected void weDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            StartDate = weDDL.SelectedValue;

            if (StartDate == "20150405")
            {
                StartDate = "2015-04-05";
            }
            else
            {
                StartDate = "2015-04-19";
            }

            if (userType == "Owner")
            {
                owSQL.SelectCommand = "SELECT '" + userFUllname + "' AS OwnerName";
                owDDL.DataBind();
                owDDL.SelectedValue = userFUllname;
                owDDL_SelectedIndexChanged(owDDL, e);
            }
            else
            {
                if (userType == "Hub")
                {
                    owSQL.SelectCommand = "SELECT '' AS OwnerName UNION ALL SELECT OwnerFirstname + ' ' + OwnerLastname AS OwnerName FROM PAYOUTsales WHERE SalesDate BETWEEN DATEADD(DAY, -13, '" + StartDate + "') AND DATEADD(DAY, 1, '" + StartDate + "') AND OwnerFirstname IS NOT NULL AND OwnerFirstname != '' AND HubFirstName + ' ' + HubLastname = '" + userFUllname + "' AND Program != 'Chipio - Chip Repair' Group BY OwnerFirstname, OwnerLastname";
                    owDDL.DataBind();
                }
                else
                {
                    owSQL.SelectCommand = "SELECT '' AS OwnerName UNION ALL SELECT 'All' AS OwnerName UNION ALL SELECT OwnerFirstname + ' ' + OwnerLastname AS OwnerName FROM PAYOUTsales WHERE SalesDate BETWEEN DATEADD(DAY, -13, '" + StartDate + "') AND DATEADD(DAY, 1, '" + StartDate + "') AND OwnerFirstname IS NOT NULL AND OwnerFirstname != '' Group BY OwnerFirstname, OwnerLastname";
                    owDDL.DataBind();
                }
            }
        }

        protected void owDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            StartDate = weDDL.SelectedValue;

            if (StartDate == "20150405")
            {
                StartDate = "2015-04-05";
            }
            else
            {
                StartDate = "2015-04-19";
            }

            OwnerName = owDDL.SelectedValue;

            if (OwnerName == "All")
            {
                OwnerName = "";
            }

            if (userType == "Owner" || userType == "Hub" || userType == "NC")
            {
                storeSQL.SelectCommand = "SELECT '' AS StoreName UNION ALL SELECT 'All' AS StoreName UNION ALL SELECT * FROM ( SELECT CASE WHEN StoreName LIKE 'Kroger%' THEN 'Kroger' ELSE StoreName END AS StoreName FROM PAYOUTsales WHERE SalesDate BETWEEN DATEADD(DAY, -13, '" + StartDate + "') AND DATEADD(DAY, 1, '" + StartDate + "') AND OwnerFirstname + ' ' + OwnerLastname LIKE '%" + OwnerName + "%' AND Program != 'Chipio - Chip Repair' )x Group BY StoreName ORDER BY StoreName";
            }
            else
            {
                storeSQL.SelectCommand = "SELECT '' AS StoreName UNION ALL SELECT 'All' AS StoreName UNION ALL SELECT * FROM ( SELECT CASE WHEN StoreName LIKE 'Kroger%' THEN 'Kroger' ELSE StoreName END AS StoreName FROM PAYOUTsales WHERE SalesDate BETWEEN DATEADD(DAY, -13, '" + StartDate + "') AND DATEADD(DAY, 1, '" + StartDate + "') AND OwnerFirstname + ' ' + OwnerLastname LIKE '%" + OwnerName + "%' )x Group BY StoreName ORDER BY StoreName";
            }
            storeDDL.DataBind();
        }

        protected void storeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            StartDate = weDDL.SelectedValue;

            if (StartDate == "20150405")
            {
                StartDate = "2015-04-05";
            }
            else
            {
                StartDate = "2015-04-19";
            }

            OwnerName = owDDL.SelectedValue;

            if (OwnerName == "All")
            {
                OwnerName = "";
            }

            StoreName = storeDDL.SelectedValue;

            if (StoreName == "All")
            {
                StoreName = "";
            }

            if (userType == "Owner" || userType == "Hub" || userType == "NC")
            {
                progSQL.SelectCommand = "SELECT x.Program FROM ( SELECT '' AS Program UNION ALL SELECT 'All' AS Program UNION ALL SELECT Program FROM PAYOUTsales WHERE Program != 'Chipio - Chip Repair' AND SalesDate BETWEEN DATEADD(DAY, -13, '" + StartDate + "') AND DATEADD(DAY, 1, '" + StartDate + "') AND OwnerFirstname + ' ' + OwnerLastname LIKE '%" + OwnerName + "%' AND StoreName LIKE '%" + StoreName + "%' Group BY Program ) x GROUP BY x.Program ORDER BY x.Program";
            }
            else
            {
                progSQL.SelectCommand = "SELECT x.Program FROM ( SELECT '' AS Program UNION ALL SELECT 'All' AS Program UNION ALL SELECT Program FROM PAYOUTsales WHERE SalesDate BETWEEN DATEADD(DAY, -13, '" + StartDate + "') AND DATEADD(DAY, 1, '" + StartDate + "') AND OwnerFirstname + ' ' + OwnerLastname LIKE '%" + OwnerName + "%' AND StoreName LIKE '%" + StoreName + "%' Group BY Program ) x GROUP BY x.Program ORDER BY x.Program";
            }
            progDDL.DataBind();
        }

        protected void viewBtn_Click(object sender, EventArgs e)
        {
            StartDate = weDDL.SelectedValue;

            if (StartDate == "20150405")
            {
                StartDate = "2015-04-05";
            }
            else
            {
                StartDate = "2015-04-19";
            }

            Session["StoreName"] = storeDDL.SelectedValue;
            Session["Program"] = progDDL.SelectedValue;
            Session["StartDate"] = Convert.ToDateTime(StartDate).AddDays(-13).ToString("yyyy-MM-dd");
            Session["Duration"] = "14";

            if (userType == "Owner")
            {
                Session["ReportType"] = "payout";
                Session["Owner"] = userFUllname;
            }
            else
            {
                if (userType == "Hub" || userType == "NC")
                {
                    Session["ReportType"] = "payout";
                    Session["Owner"] = owDDL.SelectedValue;
                }
                else
                {
                    Session["ReportType"] = typeDDL.SelectedValue;
                    Session["Owner"] = owDDL.SelectedValue;
                }
            }

            Response.Redirect("Grids.aspx", true);
        }
    }
}