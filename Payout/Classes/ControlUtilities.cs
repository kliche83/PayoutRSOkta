using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;
using System.Data;
using System.Linq;
using System.Configuration;

namespace WebApplication3
{
    public static class ControlUtilities
    {
        private static List<int> ColumnsIdToRemove;
        private static Dictionary<string, int> OriginalColumnOrder;
        private static string ServerPath, RootURL;
        /// <summary>
        /// Find the first ancestor of the selected control in the control tree
        /// </summary>
        /// <typeparam name="TControl">Type of the ancestor to look for</typeparam>
        /// <param name="control">The control to look for its ancestors</param>
        /// <returns>The first ancestor of the specified type, or null if no ancestor is found.</returns>
        public static TControl FindAncestor<TControl>(this Control control) where TControl : Control
        {
            if (control == null) throw new ArgumentNullException("control");

            Control parent = control;
            do
            {
                parent = parent.Parent;
                var candidate = parent as TControl;
                if (candidate != null)
                {
                    return candidate;
                }
            } while (parent != null);
            return null;
        }

        /// <summary>
        /// Finds all descendants of a certain type of the specified control.
        /// </summary>
        /// <typeparam name="TControl">The type of descendant controls to look for.</typeparam>
        /// <param name="parent">The parent control where to look into.</param>
        /// <returns>All corresponding descendants</returns>
        public static IEnumerable<TControl> FindDescendants<TControl>(this Control parent) where TControl : Control
        {
            if (parent == null) throw new ArgumentNullException("control");

            if (parent.HasControls())
            {
                foreach (Control childControl in parent.Controls)
                {
                    var candidate = childControl as TControl;
                    if (candidate != null) yield return candidate;

                    foreach (var nextLevel in FindDescendants<TControl>(childControl))
                    {
                        yield return nextLevel;
                    }
                }
            }
        }

        public class GridViewTemplateControls : ITemplate
        {
            private DataControlRowType _templateType;
            public string _columnName;
            private string _controlType;
            private bool _IsAutoPostBack;
            private List<string> _headerDdlList;

            public GridViewTemplateControls(DataControlRowType type, string colname, string ctlType, bool IsAutoPostBack, List<string> headDdlList = null)
            {
                _templateType = type;
                _columnName = colname;
                _controlType = ctlType;
                _headerDdlList = headDdlList;
                _IsAutoPostBack = IsAutoPostBack;
            }

            public static void dynamicDDL_SelectedIndexChanged(object sender, EventArgs e)
            {
                DropDownList ddl = (DropDownList)sender;
                SetSessionVariableSelectedValue(ddl.ID, ddl.SelectedValue);
            }

            public static void dynamicTXT_TextChanged(object sender, EventArgs e)
            {
                TextBox txt = (TextBox)sender;

                if (txt.ID.Contains("DPK"))
                {
                    DateTime OutResult;
                    DateTime.TryParse(txt.Text, out OutResult);
                    
                    if (OutResult != DateTime.MinValue)
                        txt.Text = Common.ApplyDateFormat(txt.Text);
                    else
                        txt.Text = "";

                }
                    
                SetSessionVariableSelectedValue(txt.ID, txt.Text);
            }

            public static void dynamicCHK_CheckedChanged(object sender, EventArgs e)
            {
                CheckBox chk = (CheckBox)sender;
                SetSessionVariableSelectedValue(chk.ID, Convert.ToInt32(chk.Checked).ToString());
            }

            private static void SetSessionVariableSelectedValue(string fieldName, string fieldValue)
            {
                Dictionary<string, string> ControlSelection = new Dictionary<string, string>();
                if (HttpContext.Current.Session["DDLSelection"] != null)
                {
                    ControlSelection = (Dictionary<string, string>)HttpContext.Current.Session["DDLSelection"];

                    if (ControlSelection.ContainsKey(fieldName))
                        ControlSelection[fieldName] = fieldValue.Trim() == "" ? "" : fieldValue;
                    else
                        ControlSelection.Add(fieldName, fieldValue.Trim() == "" ? "" : fieldValue);
                }
                else
                {
                    ControlSelection.Add(fieldName, fieldValue.Trim() == "" ? "" : fieldValue);
                }
                HttpContext.Current.Session["DDLSelection"] = ControlSelection;
            }

            void ITemplate.InstantiateIn(Control container)
            {
                Label lbl = new Label();

                switch (_templateType)
                {
                    case DataControlRowType.Header:
                        lbl = new Label();
                        lbl.Text = _columnName;
                        container.Controls.Add(lbl);

                        Dictionary<string, string> ControlSelection = (Dictionary<string, string>)HttpContext.Current.Session["DDLSelection"];

                        Panel pnl = new Panel();
                                                
                        switch (_controlType)
                        {
                            case "ddl":
                                DropDownList DDL = new DropDownList();
                                DDL.ID = "Web" + _columnName + "DDL";
                                DDL.DataSource = _headerDdlList;
                                DDL.AutoPostBack = _IsAutoPostBack;
                                DDL.SelectedIndexChanged += dynamicDDL_SelectedIndexChanged;                                

                                if (HttpContext.Current.Session["DDLSelection"] != null)
                                {
                                    if (ControlSelection.ContainsKey(DDL.ID)/* && _headerDdlList.Contains(ControlSelection[DDL.ID])*/)
                                        if(_headerDdlList.Contains(ControlSelection[DDL.ID]))
                                            DDL.SelectedValue = ControlSelection[DDL.ID];
                                        else
                                            DDL.SelectedValue = _headerDdlList[0];
                                    //else
                                    //    DDL.SelectedValue = _headerDdlList[0];
                                }                                

                                pnl.Controls.Add(DDL);
                                pnl.CssClass = "FilterDiv";
                                container.Controls.Add(pnl);
                                break;

                            case "txt":
                                TextBox TXT = new TextBox();
                                TXT.ID = "Web" + _columnName + "TXT";
                                TXT.AutoPostBack = _IsAutoPostBack;
                                TXT.TextChanged += dynamicTXT_TextChanged;

                                if (HttpContext.Current.Session["DDLSelection"] != null)
                                {
                                    if (ControlSelection.ContainsKey(TXT.ID))
                                        TXT.Text = ControlSelection[TXT.ID];
                                }

                                pnl.Controls.Add(TXT);
                                pnl.CssClass = "FilterDiv";
                                container.Controls.Add(pnl);
                                break;

                            case "chk":
                                CheckBox CHK = new CheckBox();
                                CHK.ID = "Web" + _columnName + "CHK";
                                CHK.AutoPostBack = _IsAutoPostBack;
                                CHK.CheckedChanged += dynamicCHK_CheckedChanged;

                                if (HttpContext.Current.Session["DDLSelection"] != null)
                                {
                                    if (ControlSelection.ContainsKey(CHK.ID))
                                        CHK.Checked = Convert.ToBoolean(int.Parse(ControlSelection[CHK.ID]));
                                }
                                pnl.Controls.Add(CHK);
                                container.Controls.Add(pnl);
                                break;
                            case "dpk":
                                //DatePicker Control
                                TextBox TXTDPK = new TextBox();
                                TXTDPK.ID = "Web" + _columnName + "DPK";
                                TXTDPK.AutoPostBack = _IsAutoPostBack;
                                TXTDPK.TextChanged += dynamicTXT_TextChanged;

                                if (HttpContext.Current.Session["DDLSelection"] != null)
                                {
                                    if (ControlSelection.ContainsKey(TXTDPK.ID))
                                        TXTDPK.Text = ControlSelection[TXTDPK.ID];
                                }

                                AjaxControlToolkit.CalendarExtender calendarExtension = new AjaxControlToolkit.CalendarExtender();
                                calendarExtension.TargetControlID = TXTDPK.ID;

                                pnl.Controls.Add(TXTDPK);
                                pnl.Controls.Add(calendarExtension);
                                pnl.CssClass = "FilterDiv";
                                container.Controls.Add(pnl);

                                break;
                            case "dpkR":
                                //DatePicker Range Controls
                                TextBox TXTDPKFROM = new TextBox();
                                TXTDPKFROM.ID = "Web" + _columnName + "FromDPK";
                                TXTDPKFROM.AutoPostBack = _IsAutoPostBack;
                                TXTDPKFROM.TextChanged += dynamicTXT_TextChanged;

                                if (HttpContext.Current.Session["DDLSelection"] != null)
                                {
                                    if (ControlSelection.ContainsKey(TXTDPKFROM.ID))
                                        TXTDPKFROM.Text = ControlSelection[TXTDPKFROM.ID];
                                }

                                AjaxControlToolkit.CalendarExtender calendarExtensionFrom = new AjaxControlToolkit.CalendarExtender();
                                calendarExtensionFrom.TargetControlID = TXTDPKFROM.ID;
                                TXTDPKFROM.Attributes.Add("placeholder", "From");

                                /*--------------------------------------------------------------------*/

                                TextBox TXTDPKTO = new TextBox();
                                TXTDPKTO.ID = "Web" + _columnName + "ToDPK";
                                TXTDPKTO.AutoPostBack = _IsAutoPostBack;
                                TXTDPKTO.TextChanged += dynamicTXT_TextChanged;

                                if (HttpContext.Current.Session["DDLSelection"] != null)
                                {
                                    if (ControlSelection.ContainsKey(TXTDPKTO.ID))
                                        TXTDPKTO.Text = ControlSelection[TXTDPKTO.ID];
                                }

                                AjaxControlToolkit.CalendarExtender calendarExtensionTo = new AjaxControlToolkit.CalendarExtender();
                                calendarExtensionTo.TargetControlID = TXTDPKTO.ID;
                                TXTDPKTO.Attributes.Add("placeholder", "To");

                                Panel MainPanel = new Panel();

                                pnl = new Panel();
                                //lbl = new Label();
                                //lbl.Text = "From: ";                                                                
                                //pnl.Controls.Add(lbl);
                                pnl.Controls.Add(TXTDPKFROM);
                                pnl.Controls.Add(calendarExtensionFrom);
                                pnl.CssClass = "DateRangeGridHeaderLeft";
                                MainPanel.Controls.Add(pnl);

                                pnl = new Panel();
                                //lbl = new Label();
                                //lbl.Text = "To: ";
                                //pnl.Controls.Add(lbl);
                                pnl.Controls.Add(TXTDPKTO);
                                pnl.Controls.Add(calendarExtensionTo);
                                MainPanel.Controls.Add(pnl);

                                MainPanel.CssClass = "DateRangeGridHeader";


                                container.Controls.Add(MainPanel);

                                break;
                        }

                        break;
                    case DataControlRowType.DataRow:

                        switch (_controlType)
                        {
                            case "lbl":
                                //GridViewDataItemTemplateContainer Container = (container as GridViewDataItemTemplateContainer);
                                //LiteralControl lit = new LiteralControl("<div id='hr' style='height:100%; font-size:x-large;'>"
                                //    + DataBinder.Eval(Container.DataItem, _columnName) + "</div>");


                                Label lblItem = new Label();
                                lblItem.DataBinding += new EventHandler(lblItem_DataBinding);
                                lblItem.ID = "lbl" + _columnName;
                                lblItem.ClientIDMode = ClientIDMode.Predictable;
                                container.Controls.Add(lblItem);
                                break;
                            case "ddl":
                                DropDownList ddlItem = new DropDownList();

                                ddlItem.DataBinding += new EventHandler(ddl_DataBinding);
                                ddlItem.ID = "ddl" + _columnName;
                                ddlItem.Visible = false;
                                //ddl.DataSource = new string[] { "item1", "item2" };
                                container.Controls.Add(ddlItem);
                                break;
                            case "chk":
                                CheckBox chkItem = new CheckBox();
                                chkItem.DataBinding += new EventHandler(chkItem_DataBinding);
                                chkItem.ID = "chk" + _columnName;
                                chkItem.Enabled = false;
                                container.Controls.Add(chkItem);
                                break;
                        }
                        break;
                }
            }

            void ddl_DataBinding(object sender, EventArgs e)
            {
                DropDownList txtdata = (DropDownList)sender;
                GridViewRow container = (GridViewRow)txtdata.NamingContainer;
                object dataValue = DataBinder.Eval(container.DataItem, _columnName);
                if (dataValue != DBNull.Value)
                {
                    txtdata.Text = dataValue == null ? "" : dataValue.ToString();
                }
            }

            void lblItem_DataBinding(object sender, EventArgs e)
            {
                Label l = (Label)sender;
                GridViewRow row = (GridViewRow)l.NamingContainer;
                l.Text = DataBinder.Eval(row.DataItem, _columnName).ToString();
            }

            void chkItem_DataBinding(object sender, EventArgs e)
            {
                CheckBox c = (CheckBox)sender;
                GridViewRow row = (GridViewRow)c.NamingContainer;
                c.Checked = Convert.ToBoolean(DataBinder.Eval(row.DataItem, _columnName).ToString());
            }
        }

        public static void BindGridWithFilters(DataTable GridDT, DataTable FiltersDT, ref GridView GRV, Dictionary<string, string> DTColumns, bool IsControlPostBack, bool IsLoadComplete = false)
        {
            //HttpContext.Current.Session["TableStructure"] = GridDT.Clone();
            //GRV.DataSource = GridDT;
            int ColumnCount = 0;
            
            foreach (KeyValuePair<string, string> item in DTColumns)
            {
                TemplateField bfield = new TemplateField();

                switch (item.Value)
                {
                    case "ddl":
                        DataTable dtNoPaging = FiltersDT;
                        List<string> lstFromDT = dtNoPaging.AsEnumerable()
                            .Where(f => f.Field<string>(item.Key) != null)
                            .OrderBy(f => f.Field<string>(item.Key))
                            .Select(f => f.Field<string>(item.Key)).Distinct().ToList();
                        lstFromDT.Insert(0, "All");

                        bfield.HeaderTemplate = new GridViewTemplateControls(DataControlRowType.Header, item.Key, "ddl", IsControlPostBack, lstFromDT);
                        break;
                    default:
                        bfield.HeaderTemplate = new GridViewTemplateControls(DataControlRowType.Header, item.Key, item.Value, IsControlPostBack);
                        break;
                }

                bfield.ItemTemplate = new GridViewTemplateControls(DataControlRowType.DataRow, item.Key, item.Value == "chk" ? "chk" : "lbl", IsControlPostBack);

                if (!IsLoadComplete)
                {
                    GRV.Columns.Add(bfield);
                }
                else
                {
                    GRV.Columns.RemoveAt(ColumnCount);
                    GRV.Columns.Insert(ColumnCount, bfield);
                }

                ColumnCount++;
            }

            HttpContext.Current.Session["TableStructure"] = GridDT.Clone();
            GRV.DataSource = GridDT;

            if (HttpContext.Current.Session["PageCompleteFired"] == null)
            {
                GRV.RowCreated += GridWithFilters_RowCreated;
                GRV.RowDataBound += GridWithFilters_RowDataBound;
            }                
            else
                HttpContext.Current.Session["PageCompleteFired"] = null;

            ServerPath = ConfigurationManager.AppSettings.Get("ServerPath").Replace(@"c$\inetpub\wwwroot\", "");
            RootURL = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + VirtualPathUtility.ToAbsolute("~/");
            GRV.DataBind();
        }


        public static void GridWithFilters_RowCreated(object sender, GridViewRowEventArgs e)
        {
            OrderColumnsWithControls(e);
        }

        public static void GridWithFilters_RowDataBound(object sender, GridViewRowEventArgs e)
        {            
            if (HttpContext.Current.Session["HyperlinkColumns"] != null && e.Row.Cells.Count > 1)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (((List<string>)HttpContext.Current.Session["HyperlinkColumns"]).Contains(
                        (((DataControlFieldCell)((e.Row.Cells[i]))).ContainingField).HeaderText)
                        )
                    {
                        if (e.Row.Cells[i].Text.Contains(ServerPath))
                        {
                            HyperLink Hlink = new HyperLink();
                            Hlink.Text = (string)e.Row.Cells[i].Text;
                            Hlink.Text = Hlink.Text.Remove(0, Hlink.Text.LastIndexOf(@"\") + 1);
                            Hlink.NavigateUrl = (string)e.Row.Cells[i].Text;
                            Hlink.NavigateUrl = Hlink.NavigateUrl.Replace(ServerPath, RootURL).Replace(@"\", "/");
                            e.Row.Cells[i].Controls.Add(Hlink);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// After filling a gridview with datasource and column templates, columns will be ordered according the structure
        /// from session variable "TableStructure" and columns with header controls will have priority to be part of the Grid
        /// Removing the other duplicate columns
        /// </summary>
        /// <param name="e"></param>
        private static void OrderColumnsWithControls(GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;
            if (row.Cells.Count > 1)
            {
                List<TableCell> columns = new List<TableCell>();

                List<string> ControlColumns = new List<string>();

                if (e.Row.RowType == DataControlRowType.Header)
                {
                    ColumnsIdToRemove = new List<int>();
                    OriginalColumnOrder = new Dictionary<string, int>();
                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        try
                        {
                            TemplateField tf = (TemplateField)((DataControlFieldCell)e.Row.Cells[i]).ContainingField;
                            ControlColumns.Add(((GridViewTemplateControls)tf.HeaderTemplate)._columnName);
                            OriginalColumnOrder.Add(((GridViewTemplateControls)tf.HeaderTemplate)._columnName, OriginalColumnOrder.Count() + 1);
                        }
                        catch
                        {
                            if (ControlColumns.Contains(((DataControlFieldCell)e.Row.Cells[i]).ContainingField.HeaderText))
                            {
                                ColumnsIdToRemove.Add(i);
                            }
                            else
                            {
                                OriginalColumnOrder.Add(((DataControlFieldCell)e.Row.Cells[i]).ContainingField.HeaderText, OriginalColumnOrder.Count() + 1);
                            }
                        }
                    }

                    ColumnsIdToRemove.Reverse();
                }

                if (e.Row.RowType != DataControlRowType.Pager)
                {
                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        if (((DataControlFieldCell)e.Row.Cells[i]).ContainingField.HeaderText == "CountResults")
                            row.Cells[i].CssClass = "HideControl";
                    }
                }

                if (ColumnsIdToRemove.Count() > 0)
                {
                    foreach (int IdToRemove in ColumnsIdToRemove)
                    {
                        if (row.Cells.Count >= IdToRemove)
                        {
                            TableCell cell = row.Cells[IdToRemove];
                            row.Cells.Remove(cell);
                        }
                    }
                }

                if (HttpContext.Current.Session["TableStructure"] != null)
                {
                    TableRow tabRow = new TableRow();

                    while (row.Cells.Count > 0)
                    {
                        tabRow.Cells.Add(row.Cells[0]);
                    }

                    DataTable dt = (DataTable)HttpContext.Current.Session["TableStructure"];
                    Dictionary<string, int> dict = OriginalColumnOrder.Select((val, index) => new { Index = index, Value = val })
                                   .ToDictionary(inc => inc.Value.Key, inc => inc.Value.Value - 1);

                    foreach (DataColumn dc in dt.Columns)
                    {
                        row.Cells.Add(
                            tabRow.Cells[
                                dict[
                                    dc.ColumnName
                                    ]
                                ]
                            );

                        dict.Remove(dc.ColumnName);
                        dict = dict.Select((val, index) => new { Index = index, Value = val })
                                   .ToDictionary(inc => inc.Value.Key, inc => inc.Index);
                    }
                }
            }
        }
    }
}