using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Payout
{
    public partial class GridRow : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void sendBtn_Click(object sender, EventArgs e)
        {
            GridView grid = (GridView)FindControl("GridView1");
            int gRow;

            int i;

            for (i = 0; i < 3; i++)
            {
                for (gRow = 0; gRow < grid.Rows.Count; gRow++)
                {
                    string what = string.Empty;

                    switch (i)
                    {
                        case 0:
                            what = "Kroger - Atlanta";
                            break;
                        case 1:
                            what = "Kroger - Louisville";
                            break;
                        case 2:
                            what = "Sams";
                            break;
                    }

                    if (grid.Rows[gRow].Cells[3].Text != what)
                    {
                        grid.Rows[gRow].Visible = false;
                    }
                }

                for (gRow = 0; gRow < grid.Rows.Count; gRow++)
                {
                    grid.Rows[gRow].Visible = true;
                }
            }
        }
    }
}