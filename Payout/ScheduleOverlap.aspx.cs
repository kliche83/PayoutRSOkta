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
using WebApplication3;

namespace Payout
{
    public partial class ScheduleOverlap : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            

            GridView1.DataSource = getResults();
            GridView1.DataBind();
        }

        protected void exportBtn_Click(object sender, EventArgs e)
        {
            List<DataTable> lstResults = new List<DataTable>() { getResults() };
            Common.ExportToExcel_List(lstResults.Take(1000000).ToList(), "RoadShow Schedules Overlap");
        }

        private DataTable getResults()
        {
            Dictionary<string, string> Params = new Dictionary<string, string>();
            Params.Add("Option", "ScheduleOverlap");
            DataTable dt = Queries.GetResultsFromStoreProcedure("spx_PAYOUTschedule", ref Params).Tables[0];
            dt.Columns.Remove("Id");

            return dt;
        }
    }
}