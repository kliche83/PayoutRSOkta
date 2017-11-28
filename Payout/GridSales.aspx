<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="GridSales.aspx.cs" Inherits="WebApplication3.GridSales" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Schedule</title>
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
                    $("#notAdd").animate({ top: 350 }, 300, function () {
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
    <style>
        tr:not(:last-child) td:nth-child(5), 
        tr:not(:last-child) td:nth-child(7) {
            width: 150px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="menu">            
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>                        
        </div>
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>

        <div id="addDiv">
            <div>
                <b>Program:</b> 
                <asp:DropDownList ID="progDDL" runat="server" AppendDataBoundItems="true" Width="180px">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <br />
                <b>Start Date:</b> <asp:TextBox ID="StartDateT" runat="server" CssClass="Date" placeholder="mm-dd-yyyy"></asp:TextBox>
                <br />
                <b>End Date:</b> <asp:TextBox ID="EndDateT" runat="server" CssClass="Date" placeholder="mm-dd-yyyy"></asp:TextBox>
                <br />
                <b>Store Name:</b> 
                <asp:DropDownList ID="storeDDL" runat="server" AppendDataBoundItems="true" Width="180px">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <br />
                <b>Store Number:</b> <asp:TextBox ID="ClubT" runat="server"></asp:TextBox>
                <br />
            </div>
            <%--<div>
                <b>City:</b> <asp:TextBox ID="CityT" runat="server"></asp:TextBox>
                <br />
                <b>State:</b> <asp:TextBox ID="StateT" runat="server"></asp:TextBox>
                <br />
                <b>Owner Firstname:</b> <asp:TextBox ID="OFN" runat="server"></asp:TextBox>
                <br />
                <b>Owner Lastname:</b> <asp:TextBox ID="OLN" runat="server"></asp:TextBox>
                <br />
                <b>Hub Firstname:</b> <asp:TextBox ID="HFN" runat="server"></asp:TextBox>
                <br />
                <b>Hub Lastname:</b> <asp:TextBox ID="HLN" runat="server"></asp:TextBox>
                <br />
                <input type="button" id="cancelAdd" value="Cancel" />
                &nbsp;&nbsp;
                <asp:Button ID="addBtn" runat="server" OnClick="addBtn_Click" Text="Add" />
            </div>--%>
        </div>

        <div id="notAdd">

        
        Owner: 
        <asp:TextBox ID="ownerTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        Store: 
        <asp:DropDownList ID="sstoreDDL" runat="server" AppendDataBoundItems="false">            
        </asp:DropDownList> &nbsp; 
        Program:
        <asp:DropDownList ID="programDDL" runat="server" AppendDataBoundItems="false">
        </asp:DropDownList> &nbsp; 
        Store Number: 
        <asp:TextBox ID="StoreNumberTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="StoreNumberTXT_Filter" TargetControlID="StoreNumberTXT" runat="server" FilterMode="ValidChars" ValidChars="1234567890"></asp:FilteredTextBoxExtender> &nbsp; 
        Date: from 
        <asp:TextBox ID="dateFrom" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="dateFrom_Filter" TargetControlID="dateFrom" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>
        to 
        <asp:TextBox ID="dateTo" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="dateTo_Filter" TargetControlID="dateTo" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender> &nbsp; 

        <asp:Button ID="searchBTN" runat="server" OnClick="searchBTN_Click" Text="Search" />
        
        <br /><br />
        
<asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Always">
<ContentTemplate>
            <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server" AutoGenerateColumns="False" AllowPaging="True" PageSize="15" 
                    AllowSorting="True" HeaderStyle-Wrap="false" RowStyle-Wrap="false" OnPageIndexChanging="GridView1_PageIndexChanging">
            <PagerStyle CssClass="neoPager" />
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="Program" HeaderText="Program" SortExpression="Program" ReadOnly="True" />                
                <asp:BoundField DataField="SalesDate" HeaderText="Sales Date" SortExpression="SalesDate" ReadOnly="True" dataformatstring="{0:MM/dd/yyyy}" htmlencode="false" />
                <asp:BoundField DataField="StoreName" HeaderText="Store Name" SortExpression="StoreName" ReadOnly="True" />
                <asp:BoundField DataField="StoreNumber" HeaderText="Store Number" SortExpression="StoreNumber" ReadOnly="True" />
                <asp:BoundField DataField="ItemNumber" HeaderText="Item Number" SortExpression="ItemNumber" ReadOnly="True" />
                <asp:BoundField DataField="ItemName" HeaderText="Item Name" SortExpression="ItemName" ReadOnly="True" />
                <asp:BoundField DataField="Qty" HeaderText="Qty" SortExpression="Qty" ReadOnly="True" />
                <asp:BoundField DataField="UnitCost" HeaderText="Unit Cost" SortExpression="UnitCost" ReadOnly="True" />
                <asp:BoundField DataField="ExtendedCost" HeaderText="Extended Cost" SortExpression="ExtendedCost" ReadOnly="True" />
                <asp:BoundField DataField="Owner" HeaderText="Owner" SortExpression="Owner" ReadOnly="True" />
                <asp:BoundField DataField="StartDate" HeaderText="Start Date" SortExpression="StartDate" ReadOnly="True" dataformatstring="{0:MM/dd/yyyy}" htmlencode="false" />
                <asp:BoundField DataField="EndDate" HeaderText="End Date" SortExpression="EndDate" ReadOnly="True" dataformatstring="{0:MM/dd/yyyy}" htmlencode="false" />
                <asp:BoundField DataField="City" HeaderText="City" SortExpression="City" ReadOnly="True" />
                <asp:BoundField DataField="State" HeaderText="State" SortExpression="State" ReadOnly="True" />
                <asp:BoundField DataField="Hub" HeaderText="Hub" SortExpression="Hub" ReadOnly="True" />
                <asp:BoundField DataField="ImportedBy" HeaderText="Imported By" SortExpression="ImportedBy" ReadOnly="True" />
                <asp:BoundField DataField="ImportedOn" HeaderText="Imported On" SortExpression="ImportedOn" ReadOnly="True" dataformatstring="{0:MM/dd/yyyy}" htmlencode="false" />
            </Columns>
        </asp:GridView>
    


        <asp:GridView CssClass="neoGrid" ID="GridView2" runat="server" AutoGenerateColumns="false" AllowPaging="true" PageSize="1" 
            AllowSorting="False" HeaderStyle-Wrap="false" RowStyle-Wrap="false" style="display: none;">            
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>                
                <asp:BoundField DataField="Program" HeaderText="Program" SortExpression="Program" ReadOnly="True" />                
                <asp:BoundField DataField="SalesDate" HeaderText="Sales Date" SortExpression="SalesDate" ReadOnly="True" dataformatstring="{0:MM/dd/yyyy}" htmlencode="false" />
                <asp:BoundField DataField="StoreName" HeaderText="Store Name" SortExpression="StoreName" ReadOnly="True" />
                <asp:BoundField DataField="StoreNumber" HeaderText="Store Number" SortExpression="StoreNumber" ReadOnly="True" />
                <asp:BoundField DataField="ItemNumber" HeaderText="Item Number" SortExpression="ItemNumber" ReadOnly="True" />
                <asp:BoundField DataField="ItemName" HeaderText="Item Name" SortExpression="ItemName" ReadOnly="True" />
                <asp:BoundField DataField="Qty" HeaderText="Qty" SortExpression="Qty" ReadOnly="True" />
                <asp:BoundField DataField="UnitCost" HeaderText="Unit Cost" SortExpression="UnitCost" ReadOnly="True" />
                <asp:BoundField DataField="ExtendedCost" HeaderText="Extended Cost" SortExpression="ExtendedCost" ReadOnly="True" />
                <asp:BoundField DataField="Owner" HeaderText="Owner" SortExpression="Owner" ReadOnly="True" />
                <asp:BoundField DataField="StartDate" HeaderText="Start Date" SortExpression="StartDate" ReadOnly="True" dataformatstring="{0:MM/dd/yyyy}" htmlencode="false" />
                <asp:BoundField DataField="EndDate" HeaderText="End Date" SortExpression="EndDate" ReadOnly="True" dataformatstring="{0:MM/dd/yyyy}" htmlencode="false" />
                <asp:BoundField DataField="City" HeaderText="City" SortExpression="City" ReadOnly="True" />
                <asp:BoundField DataField="State" HeaderText="State" SortExpression="State" ReadOnly="True" />
                <asp:BoundField DataField="Hub" HeaderText="Hub" SortExpression="Hub" ReadOnly="True" />
                <asp:BoundField DataField="ImportedBy" HeaderText="Imported By" SortExpression="ImportedBy" ReadOnly="True" />
                <asp:BoundField DataField="ImportedOn" HeaderText="Imported On" SortExpression="ImportedOn" ReadOnly="True" dataformatstring="{0:MM/dd/yyyy}" htmlencode="false" />
            </Columns>
        </asp:GridView>

 </ContentTemplate>
</asp:UpdatePanel>
            
        </div>

    </div>
    </form>
</body>
</html>