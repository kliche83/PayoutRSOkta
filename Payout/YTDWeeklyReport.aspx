<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="YTDWeeklyReport.aspx.cs" Inherits="WebApplication3.YTDWeeklyReport" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/ServerGridPagingUC.ascx" TagPrefix="uc1" TagName="ServerGridPagingUC" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Audit Sales</title>
    <link href="Content/CalendarStyle.css" rel="stylesheet" type="text/css" />
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script src="//code.jquery.com/ui/1.11.2/jquery-ui.js"></script>
    
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
            });
        });

    </script>

</head>
<body>
    <form id="frmYTDWeeklyReport" runat="server">
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>
                
        <div id="menu">
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>
        </div>

        <div id="loadDiv" runat="server" style="position: absolute; top: 0; bottom: 0; left: 0; right: 0; background-color: rgbA(255, 255, 255, 0.8); z-index: 9999; overflow: hidden; text-align: center; padding: 120px 10px 10px 10px;">
            <span id="errorSpan" runat="server">
                <asp:Label ID="errorMsg" runat="server" ForeColor="Black" Font-Bold="true" Font-Size="Medium"></asp:Label>
                <br /><br />
                <input id="gotIt" runat="server" value="OK" style="padding: 10px; border: 0; background-color: #2764AB; color: #fff; cursor: pointer; width: 50px!important;" />
                <br />
            </span>

            <img id="loadGif" runat="server" src="../Content/load.gif" />
        </div>

        <asp:UpdatePanel ID="UpdatePanelUpperBox" UpdateMode="Conditional" runat="server">
            <ContentTemplate>

                <div id="notAdd">

                    <b>Select Year:</b>
                        <asp:DropDownList ID="YearDDL" runat="server"  AppendDataBoundItems="false" AutoPostBack="true"></asp:DropDownList> &nbsp; 
                    <b>Retailer:</b>
                        <asp:DropDownList ID="WebStoreNameDDL" runat="server"  AppendDataBoundItems="false" AutoPostBack="true"></asp:DropDownList> &nbsp; 
                    <b>Program:</b>
                        <asp:DropDownList ID="WebProgramDDL" runat="server"  AppendDataBoundItems="false" AutoPostBack="true"></asp:DropDownList> &nbsp; 
                    <b>Rotation:</b>
                        <asp:DropDownList ID="WebRotationDDL" runat="server" AppendDataBoundItems="false" AutoPostBack="true"></asp:DropDownList> &nbsp; 
                    <b>Owner:</b>
                        <asp:DropDownList ID="WebOwnerDDL" runat="server" AppendDataBoundItems="false" AutoPostBack="true"></asp:DropDownList> &nbsp; 
                    
                    <br /><br />
                                    
                    <asp:UpdatePanel ID="UpdatePanelGrid" UpdateMode="Conditional" runat="server">
                        <ContentTemplate>

                            <b>Summary Sales:</b>
                            <div>
                                <asp:GridView ID="GRVSummary" CssClass="neoGrid AutoWidth" runat="server" OnRowDataBound="GRV_RowDataBound"
                                    AllowSorting="True" HeaderStyle-Wrap="false" ItemStyle-Wrap="False" RowStyle-Wrap="false" AutoGenerateColumns="true" >
                                    <EmptyDataTemplate>
                                        <h4>No results found.</h4>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>

                            <br />

                            <b>Quantity Sold:</b>
                            <div class="YTDWeeklyGrids">
                                <asp:GridView ID="GRVQty" CssClass="neoGrid" runat="server" OnRowDataBound="GRV_RowDataBound"
                                    AllowSorting="True" HeaderStyle-Wrap="false" ItemStyle-Wrap="False" RowStyle-Wrap="false" AutoGenerateColumns="true">
                                    <EmptyDataTemplate>
                                        <h4>No results found.</h4>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>

                            <br />

                            <b>Commissionable Revenue:</b>
                            <div class="YTDWeeklyGrids">
                                <asp:GridView ID="GRVRev" CssClass="neoGrid" runat="server" OnRowDataBound="GRV_RowDataBound"
                                    AllowSorting="True" HeaderStyle-Wrap="false" ItemStyle-Wrap="False" RowStyle-Wrap="false" AutoGenerateColumns="true">
                                    <EmptyDataTemplate>
                                        <h4>No results found.</h4>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                        
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>