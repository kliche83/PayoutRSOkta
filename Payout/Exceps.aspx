<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Exceps.aspx.cs" Inherits="Payout.Execps" EnableEventValidation="false" Debug="true" %>
    
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Payouts | Exceptions Report</title>

    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />

    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script src="//code.jquery.com/ui/1.11.2/jquery-ui.js"></script>
    <script type="text/javascript" src="/Scripts/gridviewScroll.min.js"></script>
    
    <script type="text/javascript">
        $(document).ready(function () {
            $("#addDiv").hide();
            $("#loadDiv").hide();
            $("#loadGif").hide();
            $("#colDDL").hide();
            $("#newVal").hide();
            gridviewScroll();

            var prm = Sys.WebForms.PageRequestManager.getInstance();

            prm.add_beginRequest(function (source, args) {
                $("#loadDiv").show();
                $("#loadGif").show();
            });

            prm.add_endRequest(function (source, args) {
                $("#loadDiv").hide();
                $("#loadGif").hide();
                gridviewScroll();
                $(".Date").datepicker();
            });

            $("#BulkActionsBtn").click(function () {
                $(this).toggleClass("menuActive");
                if ($("#notAdd").offset().top == 50) {
                    $("#notAdd").animate({ top: 230 }, 300, function () {
                        $("#addDiv").fadeIn();
                    });
                }
                else {
                    $("#addDiv").hide();
                    $("#notAdd").animate({ top: 50 }, 300);
                }
            });

            $("#cancelAdd").click(function () {
                $("#BulkActionsBtn").removeClass("menuActive");
                $("#addDiv").hide();
                $("#notAdd").animate({ top: 50 }, 300);
            });

            $("#bulkDDL").change(function () {
                if ($("#bulkDDL").val() == "update") {
                    $("#colDDL").show();
                    $("#newVal").show();
                }
                else {
                    $("#colDDL").hide();
                    $("#newVal").hide();
                }
            });
        });

        window.onresize = function (event) {
            gridviewScroll();
        };

        function gridviewScroll() {
            $('#<%= salesGrid.ClientID%>').gridviewScroll({
                width: window.innerWidth - 50,
                height: window.innerHeight - 180,
                freezesize: 0,
                arrowsize: 30,
                varrowtopimg: "Content/arrowvt.png",
                varrowbottomimg: "Content/arrowvb.png",
                harrowleftimg: "Content/arrowhl.png",
                harrowrightimg: "Content/arrowhr.png"
            });
        }

        $(function () {
            $(".Date").datepicker();
        });
    </script>
    <style type="text/css">
        body {margin: 40px 20px 20px 20px; }
        #addDiv { margin-top: -10px; }
        #updatePanel { padding: 0; margin: 0; }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <div id="menu">
            <asp:LinkButton ID="BulkActionsBtn" runat="server" Text="Bulk Actions" />

            <%--<a id="sales" class="tab menuActive">Sales</a>
            <a id="schedule" class="tab">Schedule</a>--%>
            
            <asp:LinkButton ID="exportSales" runat="server" onclick="exportSales_Click" Text="Export to Excel" />
            <%--<asp:LinkButton ID="exportSchedule" runat="server" onclick="exportSchedule_Click" Text="Export Schedule to Excel" />--%>

            
        </div>

        <div id="loadDiv" runat="server" style="position: absolute; top: 0; bottom: 0; left: 0; right: 0; background-color: rgbA(255, 255, 255, 0.8); z-index: 9999; overflow: hidden; text-align: center; padding: 120px 10px 10px 10px;">
            <img id="loadGif" runat="server" src="../Content/load.gif" />
        </div>
        
        <br /><br />

        <asp:SqlDataSource ID="SQLprogram" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>
        
        <div id="addDiv">

            Action: 
            <asp:DropDownList ID="bulkDDL" runat="server">
                <asp:ListItem></asp:ListItem>
                <asp:ListItem Value="update" Text="Update a column"></asp:ListItem>
                <asp:ListItem Value="pending" Text="Mark as pending"></asp:ListItem>
                <asp:ListItem Value="misc" Text="Mark as misc sales"></asp:ListItem>
                <asp:ListItem Value="return" Text="Mark as return"></asp:ListItem>
                <asp:ListItem Value="archive" Text="Archive"></asp:ListItem>
            </asp:DropDownList> 
            <asp:DropDownList ID="colDDL" runat="server">                
                <asp:ListItem Value="ItemNumber" Text="Item Number"></asp:ListItem>
                <asp:ListItem Value="ItemName" Text="Item Name"></asp:ListItem>
                <asp:ListItem Value="Qty" Text="Qty"></asp:ListItem>                
            </asp:DropDownList> 
            <asp:TextBox ID="newVal" runat="server" placeholder="new value"></asp:TextBox>
            <br /><br />
            <div style="text-align: right; width: 250px; line-height: 20px;">
                <i>Bulk actions apply to selected filters, excluding archives.</i>
                <br /><br />
                <input type="button" id="cancelAdd" value="Cancel" /> <asp:Button ID="bulkApply" runat="server" Text="Apply" OnClick="bulkApply_Click" />
            </div>

        </div>
        
        <div id="notAdd">

    <%--<asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Always">
        <ContentTemplate>--%>

        Week Ending: 
        <asp:DropDownList ID="webStartDate" runat="server" style="width: 110px;" AppendDataBoundItems="false">
        </asp:DropDownList> &nbsp; 
        Sales Date: 
        <asp:TextBox ID="saleDate" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="saleDate_Filter" TargetControlID="saleDate" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>
        View: 
        <asp:DropDownList ID="viewDDL" runat="server" style="width: 100px!important;">
            <asp:ListItem Value="Pending">Pending</asp:ListItem>
            <asp:ListItem Value="Misc">Misc Sales</asp:ListItem>
            <asp:ListItem Value="Return">Returns</asp:ListItem>
            <asp:ListItem Value="Archive">Archive</asp:ListItem>
        </asp:DropDownList> &nbsp; 
        Store: 
        <asp:DropDownList ID="storeDDL" runat="server" style="width: 100px!important;">
            <asp:ListItem Value="%" Text="All" Selected="True"></asp:ListItem>
            <asp:ListItem Value="BJs">BJ's</asp:ListItem>
            <asp:ListItem Value="CanadianTire">Canadian Tire</asp:ListItem>
            <asp:ListItem Value="Costco">Costco</asp:ListItem>
            <asp:ListItem Value="HEB">HEB</asp:ListItem>
            <asp:ListItem Value="Kroger">Kroger</asp:ListItem>
            <asp:ListItem Value="Meijer">Meijer</asp:ListItem>
            <asp:ListItem Value="Sams">Sam's</asp:ListItem>
            <asp:ListItem Value="WalMart">WalMart</asp:ListItem>
            <asp:ListItem Value="WinnDixie">WinnDixie</asp:ListItem>
        </asp:DropDownList> &nbsp; 
        <%--Program: 
        <asp:DropDownList ID="programDDL" runat="server" DataSourceID="SQLprogram" DataValueField="Program" DataTextField="Program" AppendDataBoundItems="false">
        </asp:DropDownList> &nbsp; --%>
        Store Number: 
        <asp:TextBox ID="StoreNumberTXT" runat="server" style="width: 70px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="StoreNumberTXT_Filter" TargetControlID="StoreNumberTXT" runat="server" FilterMode="ValidChars" ValidChars="1234567890"></asp:FilteredTextBoxExtender>
        Qty: 
        <asp:DropDownList ID="opDDL" runat="server" style="width: 60px;">
            <asp:ListItem Value="All" Text="All" Selected="True"></asp:ListItem>
            <asp:ListItem Value="=" Text="="></asp:ListItem>
            <asp:ListItem Value=">=" Text=">="></asp:ListItem>
            <asp:ListItem Value=">" Text=">"></asp:ListItem>
            <asp:ListItem Value="<=" Text="<="></asp:ListItem>
            <asp:ListItem Value="<" Text="<"></asp:ListItem>
        </asp:DropDownList> &nbsp; 
        <asp:TextBox ID="opTXT" runat="server" style="width: 60px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="opTXT_Filter" TargetControlID="opTXT" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>

        <asp:Button ID="searchBTN" runat="server" OnClick="searchBTN_Click" Text="Search" />
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>

            <br />
            <br />
                
        <%--<div id="salesDiv" class="div">--%>
            <b>Sales data not matching to an entry on the schedule:</b>
            <br />
            <br />
            <asp:GridView CssClass="neoGrid" ID="salesGrid" runat="server" AllowSorting="false" AllowPaging="true" PageSize="20" AutoGenerateColumns="false" 
                DataKeyNames="Id" HeaderStyle-Wrap="false" RowStyle-Wrap="false" OnRowDataBound="Grid_RowDataBound">
                <PagerStyle CssClass="neoPager" />
                <EmptyDataTemplate>
                    <h4>No results found.</h4>
                </EmptyDataTemplate>
                <Columns>
                    <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" ReadOnly="true" />
                    <asp:BoundField DataField="StoreName" HeaderText="Store Name" SortExpression="StoreName" ReadOnly="true" />
                    <asp:TemplateField HeaderText="Store Number" SortExpression="StoreNumber">
                        <ItemTemplate>
                            <asp:TextBox ID="StoreNumber" runat="server" Text='<%# Bind("[StoreNumber]") %>' AutoPostBack="true" OnTextChanged="TextChanged" Enabled="false"></asp:TextBox>
                            <asp:FilteredTextBoxExtender ID="StoreNumber_Filter" TargetControlID="StoreNumber" runat="server" FilterMode="ValidChars" ValidChars="1234567890"></asp:FilteredTextBoxExtender>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Program" HeaderText="Program" SortExpression="Program" ReadOnly="true" />
                    <asp:TemplateField HeaderText="Assign New Program" SortExpression="Program">
                        <ItemTemplate>
                            <asp:DropDownList ID="Program" runat="server" DataSourceID="SQLprogram" DataValueField="Program" DataTextField="Program" AppendDataBoundItems="false" AutoPostBack="true" OnSelectedIndexChanged="DropChanged" style="width: 100%; min-width: 100px; border: 0;">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Sales Date" SortExpression="SalesDate">
                        <ItemTemplate>
                            <asp:TextBox ID="SalesDate" runat="server" Text='<%# Bind("[SalesDate]", "{0:MM/dd/yyyy}") %>' CssClass="Date" AutoPostBack="true" OnTextChanged="TextChanged" Enabled="false"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Item Number" SortExpression="ItemNumber">
                        <ItemTemplate>
                            <asp:TextBox ID="ItemNumber" runat="server" Text='<%# Bind("[ItemNumber]") %>' AutoPostBack="true" OnTextChanged="TextChanged"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Item Name" SortExpression="ItemName">
                        <ItemTemplate>
                            <asp:TextBox ID="ItemName" runat="server" Text='<%# Bind("[ItemName]") %>' AutoPostBack="true" OnTextChanged="TextChanged"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Qty" SortExpression="Qty">
                        <ItemTemplate>
                            <asp:TextBox ID="Qty" runat="server" Text='<%# Bind("[Qty]") %>' AutoPostBack="true" OnTextChanged="TextChanged"></asp:TextBox>
                            <asp:FilteredTextBoxExtender ID="Qty_Filter" TargetControlID="Qty" runat="server" FilterMode="ValidChars" ValidChars="1234567890-."></asp:FilteredTextBoxExtender>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Extended Cost" SortExpression="ExtendedCost">
                        <ItemTemplate>
                            <asp:TextBox ID="ExtendedCost" runat="server" Text='<%# Bind("[ExtendedCost]") %>' AutoPostBack="true" OnTextChanged="TextChanged" Enabled="false"></asp:TextBox>
                            <asp:FilteredTextBoxExtender ID="ExtendedCost_Filter" TargetControlID="ExtendedCost" runat="server" FilterMode="ValidChars" ValidChars="1234567890-."></asp:FilteredTextBoxExtender>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="ImportedBy" HeaderText="Imported By" SortExpression="ImportedBy" ReadOnly="true" />
                    <asp:BoundField DataField="ImportedOn" HeaderText="Imported On" SortExpression="ImportedOn" ReadOnly="true" DataFormatString="{0:MM\/dd\/yyyy}" />
                    <asp:TemplateField HeaderText="Archive" SortExpression="Archive">
                        <ItemTemplate>
                            <asp:CheckBox ID="Archive" runat="server" Checked='<%# Bind("Archive") %>' Enabled="true" AutoPostBack="true" OnCheckedChanged="CheckChanged" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        <%--</div>--%>
        
        <%--<div id="scheduleDiv" class="div">
            <b>Schedule entries not matching to any sales data:</b>
            <br />
            <br />
            <asp:GridView CssClass="neoGrid" ID="scheduleGrid" runat="server" AllowSorting="false" AllowPaging="true" PageSize="20" AutoGenerateColumns="false" DataSourceID="scheduleSQL" OnRowDataBound="Grid_RowDataBound">
                <EmptyDataTemplate>
                    <h4>No results found.</h4>
                </EmptyDataTemplate>
                <Columns>
                    <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" ReadOnly="true" />
                    <asp:BoundField DataField="Hub" HeaderText="Hub" SortExpression="Hub" ReadOnly="true" />
                    <asp:BoundField DataField="Owner" HeaderText="Owner" SortExpression="Owner" ReadOnly="true" />
                    <asp:BoundField DataField="Program" HeaderText="Program" SortExpression="Program" ReadOnly="true" />
                    <asp:BoundField DataField="StartDate" HeaderText="Start Date" SortExpression="StartDate" ReadOnly="true" DataFormatString="{0:MM\/dd\/yyyy}" />
                    <asp:BoundField DataField="EndDate" HeaderText="End Date" SortExpression="EndDate" ReadOnly="true" DataFormatString="{0:MM\/dd\/yyyy}" />
                    <asp:BoundField DataField="StoreName" HeaderText="Store Name" SortExpression="StoreName" ReadOnly="true" />
                    <asp:BoundField DataField="StoreNumber" HeaderText="Store #" SortExpression="StoreNumber" ReadOnly="true" />
                    <asp:BoundField DataField="City" HeaderText="City" SortExpression="City" ReadOnly="true" />
                    <asp:BoundField DataField="State" HeaderText="State" SortExpression="State" ReadOnly="true" />
                    <asp:BoundField DataField="ImportedBy" HeaderText="Imported By" SortExpression="ImportedBy" ReadOnly="true" />
                    <asp:BoundField DataField="ImportedOn" HeaderText="Imported On" SortExpression="ImportedOn" ReadOnly="true" DataFormatString="{0:MM\/dd\/yyyy}" />
                    <asp:TemplateField HeaderText="Exception" SortExpression="Exception">
                        <ItemTemplate>
                            <asp:CheckBox ID="Exception" runat="server" Checked='<%# Bind("Exception") %>' Enabled="true" AutoPostBack="true" OnCheckedChanged="CheckChanged" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>--%>
                
        <%--</ContentTemplate>
    </asp:UpdatePanel>--%>
        
        </div>

        <%--<asp:SqlDataSource ID="salesSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
            SelectCommand="">
        </asp:SqlDataSource>--%>

        <%--<asp:SqlDataSource ID="scheduleSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
            SelectCommand="">
        </asp:SqlDataSource>--%>

    </form>
</body>
</html>
