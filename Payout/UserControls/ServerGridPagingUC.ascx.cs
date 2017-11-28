using System;
using System.Data;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

namespace WebApplication3
{
    public partial class ServerGridPagingUC : UserControl
    {
        bool IsPageChanged;
        
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPageChanged)
                LoadGridData(1);

            IsPageChanged = false;
        }

        protected void Page_Changed(object sender, EventArgs e)
        {
            IsPageChanged = true;
            int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
            LoadGridData(pageIndex);
        }

        protected void PageSize_Changed(object sender, EventArgs e)
        {
            Session["UC_GridPagesize_BtnHit"] = true;
            LoadGridData(1);
        }

        protected void GRV_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            FormatColumns(ref e);
        }

        protected void GRVHeader_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            FormatColumns(ref e);
        }
        
        private void FormatColumns(ref GridViewRowEventArgs e)
        {
            if (e.Row.Cells.Count > 1)
            {
                string FieldName = string.Empty;

                List<ToolClasses.UCGridColumnDetails> lstUCGridColumnDetails = (List<ToolClasses.UCGridColumnDetails>)Session["lstUCGridColumnDetails"];
                ToolClasses.UCGridColumnDetails objUCGridColumnDetails = new ToolClasses.UCGridColumnDetails();

                for (int j = 0; j < e.Row.Cells.Count; j++)
                {
                    FieldName = ((BoundField)((DataControlFieldCell)(e.Row.Cells[j])).ContainingField).DataField.ToLower();
                    objUCGridColumnDetails = lstUCGridColumnDetails.Where(l => l.columnName.ToLower() == FieldName.ToLower()).FirstOrDefault();

                    if (objUCGridColumnDetails == null)
                    {
                        e.Row.Cells[j].CssClass = "HideCellVisibility";
                    }
                    else
                    {
                        e.Row.Cells[j].Width = new Unit(objUCGridColumnDetails.columnWidth);
                        

                        if (e.Row.RowType == DataControlRowType.Header)
                        {
                            e.Row.Cells[j].Text = objUCGridColumnDetails.columnDisplayName;                            
                        }
                    }
                }
            }            
        }
        
        private void LoadGridData(int pageIndex)
        {
            if (Session["SP_Parameters"] != null && Session["SP_Name"] != null)
            {
                int recordCount;
                Dictionary<string, string> Params =
                    new Dictionary<string, string>((Dictionary<string, string>)Session["SP_Parameters"]);

                Params.Add("PageIndex", pageIndex.ToString());
                Params.Add("PageSize", ddlPageSize.SelectedValue);

                DataTable dt = Queries.GetResultsFromStoreProcedure((string)Session["SP_Name"], ref Params).Tables[0];

                if (Session["AddDelOption"] != null && (bool)Session["AddDelOption"])
                {
                    dt.Columns.Add("Action");
                    dt.Columns["Action"].SetOrdinal(1);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["Action"] = "x";
                    }
                }

                if (dt.Rows.Count > 0)
                    recordCount = int.Parse(dt.Rows[0]["CountResults"].ToString());
                else
                    recordCount = 0;
                
                dt.Columns.Remove("CountResults");

                //if(Session["HeaderFilterList"] != null)
                //{
                //    AddHeaderTemplates(dt);
                //}
                                
                GRV.DataSource = dt;

                if (Session["ShowFixedHeader"] != null && !(bool)Session["ShowFixedHeader"])
                    GRV.ShowHeader = true;
                else
                    GRV.ShowHeader = false;

                GRV.DataBind();

                setHeaders(dt.Clone()); //Lightweight datatable

                PopulatePager(recordCount, pageIndex, 10);
            }                     
        }

        private void PopulatePager(int recordCount, int currentPage, int PagesToDisplay)
        {
            double dblPageCount = (double)(recordCount / decimal.Parse(ddlPageSize.SelectedValue));
            int pageCount = (int)Math.Ceiling(dblPageCount);
            List<ListItem> pages = new List<ListItem>();
            if (pageCount > 0)
            {
                pages.Add(new ListItem("First", "1", currentPage > 1));
                //for (int i = 1; i <= pageCount; i++)
                //{
                //    pages.Add(new ListItem(i.ToString(), i.ToString(), i != currentPage));
                //}

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

        private void setHeaders(DataTable dt)
        {
            if (!(Session["ShowFixedHeader"] != null && !(bool)Session["ShowFixedHeader"]))
            {
                GRVHeader.DataSource = dt;
                GRVHeader.DataBind();
            }
                
        }
    }
}