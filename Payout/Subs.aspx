<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Subs.aspx.cs" Inherits="Payout.Subs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Substitutions</title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script src="//code.jquery.com/ui/1.11.2/jquery-ui.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#addDiv").hide();

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
                    $(".Date").datepicker();
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

            $(function () {
                $(".Date").datepicker();
            });

            $("input[type=text]").click(function (e) {
                $(this).select();
            });

            $("input[type=text]").focus(function (e) {
                $(this).select();
            });

            $("#add").click(function () {
                $(this).toggleClass("menuActive");
                if ($("#notAdd").offset().top == 50) {
                    $("#notAdd").animate({ top: 280 }, 300, function () {
                        $("#addDiv").fadeIn();
                    });
                }
                else {
                    $("#addDiv").hide();
                    $("#notAdd").animate({ top: 50 }, 300);
                }
            });

            $("#cancelAdd").click(function () {
                $("#add").removeClass("menuActive");
                $("#addDiv").hide();
                $("#notAdd").animate({ top: 50 }, 300);
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="menu">
            <a id="add">Add Substitution</a>
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>

            
        </div>

        <div id="addDiv">
            <div>
                <b>Store Name:</b> 
                <asp:DropDownList ID="storeDDL" runat="server" Width="162px">
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem Value="BJs">BJ's</asp:ListItem>
                    <asp:ListItem Value="CanadianTire">Canadian Tire</asp:ListItem>
                    <asp:ListItem Value="Costco">Costco</asp:ListItem>
                    <asp:ListItem Value="HEB">HEB</asp:ListItem>
                    <asp:ListItem Value="Kroger">Kroger</asp:ListItem>
                    <asp:ListItem Value="Meijer">Meijer</asp:ListItem>
                    <asp:ListItem Value="Sams">Sam's</asp:ListItem>
                    <asp:ListItem Value="WalMart">WalMart</asp:ListItem>
                    <asp:ListItem Value="WinnDixie">WinnDixie</asp:ListItem>
                </asp:DropDownList>
                <br />
                <b>Store Number:</b> <asp:TextBox ID="SNO" runat="server" width="150px"></asp:TextBox>
                <br />
                <b>Item Number:</b> <asp:TextBox ID="INO" runat="server" width="150px"></asp:TextBox>
                <br />
                <b>Sub Number:</b> <asp:TextBox ID="SubNO" runat="server" width="150px"></asp:TextBox>
                <br />
                <b>Sub Cost:</b> <asp:TextBox ID="SubUC" runat="server" width="150px"></asp:TextBox>
                <br />
            </div>
            <div>
                <b>Start Date:</b> <asp:TextBox ID="SSD" runat="server" width="150px" CssClass="Date"></asp:TextBox>
                <br />
                <b>End Date:</b> <asp:TextBox ID="SED" runat="server" width="150px" CssClass="Date"></asp:TextBox>
                <br />
                <b><asp:CheckBox ID="wasTest" runat="server" Text="Test Product" /></b>
                <br />
                <br />
                <input type="button" id="cancelAdd" value="Cancel" />
                &nbsp;&nbsp;
                <asp:Button ID="addBtn" runat="server" OnClick="addBtn_Click" Text="Add" />
            </div>
        </div>
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>
        
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>
        
        <div id="notAdd">

        Store: 
        <asp:DropDownList ID="storeDDLs" runat="server">
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
        Store Number: 
        <asp:TextBox ID="StoreNumberTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="StoreNumberTXT_Filter" TargetControlID="StoreNumberTXT" runat="server" FilterMode="ValidChars" ValidChars="1234567890"></asp:FilteredTextBoxExtender> &nbsp; 
        Item Number: 
        <asp:TextBox ID="ItemNumberTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="ItemNumberTXT_Filter" TargetControlID="ItemNumberTXT" runat="server" FilterMode="ValidChars" ValidChars="1234567890"></asp:FilteredTextBoxExtender> &nbsp; 
        Sales Date: from 
        <asp:TextBox ID="dateFrom" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="dateFrom_Filter" TargetControlID="dateFrom" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender> &nbsp; 
        to 
        <asp:TextBox ID="dateTo" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="dateTo_Filter" TargetControlID="dateTo" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender> &nbsp; 
        Test Products: 
        <asp:DropDownList ID="TestDDL" runat="server" style="width: 100px;">
            <asp:ListItem Value="All" Text="All" Selected="True"></asp:ListItem>
            <asp:ListItem Value="1" Text="Was Test"></asp:ListItem>
            <asp:ListItem Value="0" Text="Wasn't Test"></asp:ListItem>
        </asp:DropDownList> &nbsp; 

        <asp:Button ID="searchBTN" runat="server" OnClick="searchBTN_Click" Text="Search" />
        
        <br /><br />

<asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Always">
<ContentTemplate>
        <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server" AutoGenerateColumns="false" DataSourceID="SqlDataSource1" AllowPaging="True" PageSize="15" 
            AllowSorting="False" HeaderStyle-Wrap="false" RowStyle-Wrap="false" OnRowDataBound="GridView1_RowDataBound">
            <PagerStyle CssClass="neoPager" />
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="Id" />
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton ID="delLink0" runat="server" Text="X" ForeColor="Red" OnClick="delLink_Click" Font-Underline="false"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Store Name" SortExpression="StoreName">
                    <ItemTemplate>
                        <asp:DropDownList ID="StoreName" runat="server" SelectedValue='<%# Bind("[StoreName]") %>' AutoPostBack="true" OnSelectedIndexChanged="DropChanged" Width="150px" style="border: 0;">
                            <asp:ListItem></asp:ListItem>
                            <asp:ListItem Value="BJs">BJ's</asp:ListItem>
                            <asp:ListItem Value="CanadianTire">Canadian Tire</asp:ListItem>
                            <asp:ListItem Value="Costco">Costco</asp:ListItem>
                            <asp:ListItem Value="HEB">HEB</asp:ListItem>
                            <asp:ListItem Value="Kroger">Kroger</asp:ListItem>
                            <asp:ListItem Value="Meijer">Meijer</asp:ListItem>
                            <asp:ListItem Value="Sams">Sam's</asp:ListItem>
                            <asp:ListItem Value="WalMart">WalMart</asp:ListItem>
                            <asp:ListItem Value="WinnDixie">WinnDixie</asp:ListItem>
                        </asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Store Number" SortExpression="StoreNumber">
                    <ItemTemplate>
                        <asp:TextBox ID="StoreNumber" runat="server" Text='<%# Bind("[StoreNumber]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Item Number" SortExpression="ItemNumber">
                    <ItemTemplate>
                        <asp:TextBox ID="ItemNumber" runat="server" Text='<%# Bind("[ItemNumber]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Substituted Item" SortExpression="SubItem">
                    <ItemTemplate>
                        <asp:TextBox ID="SubItem" runat="server" Text='<%# Bind("[SubItem]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Substituted Cost" SortExpression="SubCost">
                    <ItemTemplate>
                        <asp:TextBox ID="SubCost" runat="server" Text='<%# Bind("[SubCost]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Start Date" SortExpression="StartDate">
                    <ItemTemplate>
                        <asp:TextBox ID="StartDate" runat="server" Text='<%# Bind("[StartDate]", "{0:dd/MM/yyyy}") %>' AutoPostBack="true" OnTextChanged="FieldChanged" CssClass="Date"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="End Date" SortExpression="EndDate">
                    <ItemTemplate>
                        <asp:TextBox ID="EndDate" runat="server" Text='<%# Bind("[EndDate]", "{0:dd/MM/yyyy}") %>' AutoPostBack="true" OnTextChanged="FieldChanged" CssClass="Date"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Test Product" SortExpression="Test">
                    <ItemTemplate>
                        <asp:CheckBox ID="Test" runat="server" Checked='<%# Bind("[Test]") %>' AutoPostBack="true" OnCheckedChanged="CheckChanged" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    
        <asp:GridView CssClass="neoGrid" ID="GridView2" runat="server" AutoGenerateColumns="false" DataSourceID="SqlDataSource1" AllowPaging="false" PageSize="15" 
            AllowSorting="False" HeaderStyle-Wrap="false" RowStyle-Wrap="false" style="display: none;">
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="StoreName" HeaderText="Store Name" SortExpression="StoreName" ReadOnly="true" />
                <%--<asp:BoundField DataField="Program" HeaderText="Program" SortExpression="Program" ReadOnly="true" />--%>
                <asp:BoundField DataField="StoreNumber" HeaderText="Store Number" SortExpression="StoreNumber" ReadOnly="true" />
                <asp:BoundField DataField="ItemNumber" HeaderText="Item Number" SortExpression="ItemNumber" ReadOnly="true" />
                <asp:BoundField DataField="SubItem" HeaderText="Substituted Item" SortExpression="SubItem" ReadOnly="true" />
                <asp:BoundField DataField="SubCost" HeaderText="Substituted Cost" SortExpression="SubCost" ReadOnly="true" />
                <asp:BoundField DataField="StartDate" HeaderText="Start Date" SortExpression="StartDate" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="EndDate" HeaderText="End Date" SortExpression="EndDate" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:TemplateField HeaderText="Test Item" SortExpression="Test">
                    <ItemTemplate><%# (Boolean.Parse(Eval("Test").ToString())) ? "Yes" : "No" %></ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

 </ContentTemplate>
</asp:UpdatePanel>

            </div>

    </div>
    </form>
</body>
</html>