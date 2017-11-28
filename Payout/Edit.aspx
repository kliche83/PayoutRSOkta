<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Payout.Edit" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Edit Import</title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script src="//code.jquery.com/ui/1.11.2/jquery-ui.js"></script> 
    <script type="text/javascript">
        $(document).ready(function () {
            jQuery(function ($) {
                var focusedElementId = "";
                var prm = Sys.WebForms.PageRequestManager.getInstance();
                prm.add_beginRequest(function (source, args) {
                    var fe = document.activeElement;
                    if (fe != null) {
                        focusedElementId = fe.id;
                    } else {
                        focusedElementId = "";
                    }
                });
                prm.add_endRequest(function (source, args) {
                    $("#<%= salesFromTXT.ClientID %>").datepicker();
                    $("#<%= salesToTXT.ClientID %>").datepicker();
                    bindKeyDown();
                    if (focusedElementId != "") {
                        $("#" + focusedElementId).focus();
                    }

                    $("input[type=text]").click(function (e) {
                        $(this).select();
                    });

                    $("input[type=text]").focus(function (e) {
                        $(this).select();
                    });
                });
            });

            $(".ControlChangeCls").change(function () {
                $("#exportRawBtn").attr("disabled", "disable");
                $("#exportBtn").attr("disabled", "disable");
                $("#delBtn").attr("disabled", "disable");

                $("#exportRawBtn").css("background-color", "#2764AB").css("color", "Gray");
                $("#exportBtn").css("background-color", "#2764AB").css("color", "Gray");
                $("#delBtn").css("background-color", "#2764AB").css("color", "Gray");

                $("#exportRawBtn").css("cursor", "not-allowed");
                $("#exportBtn").css("cursor", "not-allowed");
                $("#delBtn").css("cursor", "not-allowed");

                $("#searchBTN").css("border", "solid 2px red"); 
            });

            $("input[type=text]").click(function (e) {
                $(this).select();
            });

            $("input[type=text]").focus(function (e) {
                $(this).select();
            });

            $(function () {
                $("#<%= salesFromTXT.ClientID %>").datepicker();
                $("#<%= salesToTXT.ClientID %>").datepicker();
            });

            function bindKeyDown() {
                $('input').keydown(function (e) {
                    //            if(e.which==39)
                    //            $(this).closest('td').next().find('input').focus();
                    //            else if(e.which==37)
                    //            $(this).closest('td').prev().find('input').focus();
                    if (e.which == 13)
                        $(this).closest('tr').next().find('td:eq(' + $(this).closest('td').index() + ')').find('input').focus();
                    else if (e.which == 40)
                        $(this).closest('tr').next().find('td:eq(' + $(this).closest('td').index() + ')').find('input').focus();
                    else if (e.which == 38)
                        $(this).closest('tr').prev().find('td:eq(' + $(this).closest('td').index() + ')').find('input').focus();
                });
            }
            bindKeyDown();
        });

        function deleteView() {
            var deleteView_confirm_value = document.createElement("INPUT");
            deleteView_confirm_value.type = "hidden";
            deleteView_confirm_value.name = "deleteView_confirm_value";
            var confirmMsg = "Do you want to delete the selected data?\n\nNOTE: This operation is irreversible and only applies to the selected filters.";
            if (confirm(confirmMsg)) {
                deleteView_confirm_value.value = "Yes";
            } else {
                deleteView_confirm_value.value = "No";
            }
            document.forms[0].appendChild(deleteView_confirm_value);
        }
    </script>
    <style>
        th a { color: #000; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="EditContainer">
        <div id="menu">
            <asp:LinkButton ID="exportRawBtn" runat="server" Text="Export Raw" OnClick="exportRawBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="delBtn" runat="server" Text="Bulk Delete" OnClick="delBtn_Click" OnClientClick="deleteView()"></asp:LinkButton>
        </div>
        
        <br />
       
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>

        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SQLprogram" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="SELECT 'All' AS Program UNION ALL SELECT * FROM (SELECT TOP 1000 Program FROM PAYOUTschedule WHERE Program != '' AND Program IS NOT NULL GROUP BY Program ORDER BY Program)x">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SQLprogram2" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="SELECT '' AS Program UNION ALL SELECT 'Misc' AS Program UNION ALL SELECT 'Return' AS Program UNION ALL SELECT * FROM (SELECT TOP 1000 Program FROM PAYOUTschedule WHERE Program != '' AND Program IS NOT NULL GROUP BY Program ORDER BY Program)x">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="weSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>

        <%--Week Ending: 
        <asp:DropDownList ID="dateDDL" runat="server" style="width: 110px;" DataSourceID="weSQL" DataValueField="StartDate" DataTextField="WeekEnding" AppendDataBoundItems="false">
        </asp:DropDownList> &nbsp; 
        Sales Date: 
        <asp:TextBox ID="saleDate" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="saleDate_Filter" TargetControlID="saleDate" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>--%>

        Sales From:
        <asp:TextBox ID="salesFromTXT" CssClass="ControlChangeCls" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="salesFromTXT_Filter" TargetControlID="salesFromTXT" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>
        To:
        <asp:TextBox ID="salesToTXT" CssClass="ControlChangeCls" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="salesToTXT_Filter" TargetControlID="salesToTXT" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>

        Store: 
        <asp:DropDownList ID="storeDDL" CssClass="ControlChangeCls" runat="server" style="width: 110px;">
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
        Program: 
        <asp:DropDownList ID="programDDL" CssClass="ControlChangeCls" runat="server" DataSourceID="SQLprogram" DataValueField="Program" DataTextField="Program" AppendDataBoundItems="false">
        </asp:DropDownList> &nbsp; 
        Store Number: 
        <asp:TextBox ID="StoreNumberTXT" CssClass="ControlChangeCls" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="StoreNumberTXT_Filter" TargetControlID="StoreNumberTXT" runat="server" FilterMode="ValidChars" ValidChars="1234567890"></asp:FilteredTextBoxExtender> &nbsp; 

        <asp:Button ID="searchBTN" runat="server" OnClick="searchBTN_Click" Text="Search" />
        
        <br /><br />
        
<%--<asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Always">
<ContentTemplate>--%>

        <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server" AutoGenerateColumns="false" DataSourceID="SqlDataSource1" AllowPaging="True" PageSize="15" 
            AllowSorting="True" HeaderStyle-Wrap="false" RowStyle-Wrap="false" DataKeyNames="Id" OnRowDataBound="GridView1_RowDataBound">
            <PagerStyle CssClass="neoPager" />
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="Id" />
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton ID="delLink" runat="server" Text="X" ForeColor="Red" OnClick="delLink_Click" Font-Underline="false"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SalesDate" HeaderText="Sales Date" SortExpression="SalesDate" ReadOnly="True" dataformatstring="{0:MM/dd/yyyy}" htmlencode="false" />
                <%--<asp:TemplateField HeaderText="Sales Date (yyyy-mm-dd)" SortExpression="SalesDate">
                    <ItemTemplate>
                        <asp:TextBox ID="SalesDate" runat="server" Text='<%# Bind("[SalesDate]", "{0:MM/dd/yyyy}") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:BoundField DataField="StoreName" HeaderText="Store Name" SortExpression="StoreName" ReadOnly="True" />
                <asp:BoundField DataField="Program" HeaderText="Program" SortExpression="Program" ReadOnly="True" />
                <asp:TemplateField HeaderText="Assign New Program" SortExpression="Program">
                    <ItemTemplate>
                        <asp:DropDownList ID="Program" runat="server" DataSourceID="SQLprogram2" DataValueField="Program" DataTextField="Program" AppendDataBoundItems="false" AutoPostBack="true" OnSelectedIndexChanged="DropChanged" style="width: 100%; min-width: 100px; border: 0;">
                        </asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="StoreNumber" HeaderText="Store Number" SortExpression="StoreNumber" ReadOnly="True" />
                <asp:BoundField DataField="ItemNumber" HeaderText="Item Number" SortExpression="ItemNumber" ReadOnly="True" />
                <%--<asp:BoundField DataField="ItemName" HeaderText="Item Name" SortExpression="ItemName" ReadOnly="True" />--%>

                <asp:TemplateField HeaderText="Item Name" SortExpression="ItemName">
                    <ItemTemplate>                        
                        <asp:TextBox ID="ItemName" runat="server" Text='<%# Bind("[ItemName]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>

                <%--<asp:TemplateField HeaderText="Store Number" SortExpression="StoreNumber">
                    <ItemTemplate>
                        <asp:TextBox ID="StoreNumber" runat="server" Text='<%# Bind("[StoreNumber]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Item Number" SortExpression="ItemNumber">
                    <ItemTemplate>
                        <asp:TextBox ID="ItemNumber" runat="server" Text='<%# Bind("[ItemNumber]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Item Name" SortExpression="ItemName">
                    <ItemTemplate>
                        <asp:TextBox ID="ItemName" runat="server" Text='<%# Bind("[ItemName]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:TemplateField HeaderText="Qty" SortExpression="Qty">
                    <ItemTemplate>
                        <%--<asp:TextBox ID="Qty" runat="server" Text='<%# Bind("[Qty]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>--%>
                        <asp:TextBox ID="Qty" runat="server" Text='<%# Bind("[Qty]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Unit Cost" SortExpression="UnitCost">
                    <ItemTemplate>
                        <asp:TextBox ID="UnitCost" runat="server" Text='<%# Bind("[UnitCost]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:TemplateField HeaderText="Extended Cost" SortExpression="ExtendedCost">
                    <ItemTemplate>
                        <asp:TextBox ID="ExtendedCost" runat="server" Text='<%# Bind("[ExtendedCost]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:BoundField DataField="ExtendedCost" HeaderText="Extended Cost" SortExpression="ExtendedCost" ReadOnly="True" />
                <asp:BoundField DataField="ImportedBy" HeaderText="Imported By" SortExpression="ImportedBy" ReadOnly="True" />
                <asp:BoundField DataField="ImportedOn" HeaderText="Imported On" SortExpression="ImportedOn" ReadOnly="True" dataformatstring="{0:MM/dd/yyyy}" htmlencode="false" />
            </Columns>
        </asp:GridView>
    
        <asp:GridView CssClass="neoGrid" ID="GridView2" runat="server" AutoGenerateColumns="false" DataSourceID="SqlDataSource1" AllowPaging="false" PageSize="15" 
            AllowSorting="False" HeaderStyle-Wrap="false" RowStyle-Wrap="false" style="display: none;">
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="SalesDate" HeaderText="Sales Date" SortExpression="SalesDate" ReadOnly="True" dataformatstring="{0:MM/dd/yyyy}" htmlencode="false" />
                <asp:BoundField DataField="StoreName" HeaderText="Store Name" SortExpression="StoreName" ReadOnly="True" />
                <asp:BoundField DataField="Program" HeaderText="Program" SortExpression="Program" ReadOnly="True" />
                <asp:BoundField DataField="StoreNumber" HeaderText="Store Number" SortExpression="StoreNumber" ReadOnly="True" />
                <asp:BoundField DataField="ItemNumber" HeaderText="Item Number" SortExpression="ItemNumber" ReadOnly="True" />
                <asp:BoundField DataField="ItemName" HeaderText="Item Name" SortExpression="ItemName" ReadOnly="True" />
                <asp:BoundField DataField="Qty" HeaderText="Qty" SortExpression="Qty" ReadOnly="true" />
                <asp:BoundField DataField="UnitCost" HeaderText="Unit Cost" SortExpression="UnitCost" ReadOnly="true" />
                <asp:BoundField DataField="ExtendedCost" HeaderText="Extended Cost" SortExpression="ExtendedCost" ReadOnly="True" />
                <asp:BoundField DataField="ImportedBy" HeaderText="Imported By" SortExpression="ImportedBy" ReadOnly="True" />
                <asp:BoundField DataField="ImportedOn" HeaderText="Imported On" SortExpression="ImportedOn" ReadOnly="True" dataformatstring="{0:MM/dd/yyyy}" htmlencode="false" />
            </Columns>
        </asp:GridView>

 <%--</ContentTemplate>
</asp:UpdatePanel>--%>

    </div>
    </form>
</body>
</html>