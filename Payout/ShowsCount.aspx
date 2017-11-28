<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="ShowsCount.aspx.cs" Inherits="WebApplication3.ShowsCount" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Shows Count</title>
    <link href="Content/CalendarStyle.css" rel="stylesheet" type="text/css" />
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />

    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js" type="text/javascript"></script>
    <script src="//code.jquery.com/ui/1.11.2/jquery-ui.js" type="text/javascript"></script>
    <script type="text/javascript">

        $(document).ready(function () {

            $("#loadDiv").not(".keep").hide();
            $("#errorSpan").not(".keep").hide();
            $("#loadGif").hide();

            var prm = Sys.WebForms.PageRequestManager.getInstance();

            prm.add_beginRequest(function (source, args) {
                $("#loadDiv").show();
                $("#loadGif").show();
            });

            prm.add_endRequest(function (source, args) {
                $("#loadDiv").not(".keep").hide();
                $("#errorSpan").not(".keep").hide();
                $("#loadGif").hide();

                $(function () {
                    $(".Date").datepicker();
                });
            });

            $(function () {
                $(".Date").datepicker();
            });

            if ($('input:text:first').val() == "") {
                $('input:text:first').focus();
            }

        });

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="menu">        
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>
        </div>         
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>

        <div id="loadDiv" runat="server" style="position: absolute; top: 0; bottom: 0; left: 0; right: 0; background-color: rgbA(255, 255, 255, 0.8); z-index: 9999; overflow: hidden; text-align: center; padding: 120px 10px 10px 10px;">
            <span id="errorSpan" runat="server">
                <asp:Label ID="errorMsg" runat="server" ForeColor="Black" Font-Bold="true" Font-Size="Medium"></asp:Label>
                <br /><br />
                <input id="gotIt" runat="server" value="OK" style="padding: 10px; border: 0; background-color: #2764AB; color: #fff; cursor: pointer; width: 50px!important;" />
                <br />
            </span>

            <img id="loadGif" runat="server" src="../Content/load.gif" />
        </div>

        <br />

        Retailer: 
        <asp:DropDownList ID="DDLShowCount" runat="server"></asp:DropDownList>
                
        Sales Date:             
        <asp:TextBox ID="txtSalesDate" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="txtSalesDate_Filter" TargetControlID="txtSalesDate" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>                                                
        <asp:Button ID="searchBTN" runat="server" OnClick="searchBTN_Click" Text="Search" />
        <asp:Label ID="ValidationText" CssClass="ValidationMessage" runat="server"></asp:Label>        
        <br /><br />

        <asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Always">
            <ContentTemplate>
                
                <asp:GridView   ID="GridView1" CssClass="neoGrid AutoWidth" 
                    runat="server" HeaderStyle-Wrap="false" RowStyle-Wrap="false" AutoGenerateColumns="true" ShowFooter="true" OnRowDataBound="GridView1_RowDataBound">
                    <PagerStyle CssClass="neoPager" />
                    <EmptyDataTemplate>
                        <h4>No results found.</h4>
                    </EmptyDataTemplate>
                </asp:GridView>

            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>