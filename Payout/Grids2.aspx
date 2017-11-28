<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Grids2.aspx.cs" Inherits="WebApplication3.Grids2" enableEventValidation="false" %>

<%--<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>--%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Reports</title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <script src="http://cdn.jquerytools.org/1.2.7/full/jquery.tools.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("tr").not("tr:first-child").click(function (e) {
                if (e.ctrlKey) {
                    if ($(this).hasClass("selectedRow")) {
                        $(this).removeClass("selectedRow");
                    }
                    else {
                        $(this).addClass("selectedRow");
                    }
                }
                else {
                    if ($(this).hasClass("selectedRow")) {
                        $("tr").removeClass("selectedRow");
                    }
                    else {
                        $("tr").removeClass("selectedRow");
                        $(this).addClass("selectedRow");
                    }
                }
            }).children().children().click(function (e) {
                //if ($(this).hasClass("selectedRow")) {
                //return false;
                //}
                //else {
                //    $("tr").removeClass("selectedRow");
                //    $(this).addClass("selectedRow");
                //}
            });

            $("#back").click(function () {
                window.history.back();
                //location.href = "ReportMaker.aspx";
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <%--<asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>--%>

        <div style="position: absolute; top: 0; left: 0; z-index: 99999; background-color: ActiveCaption; display: none;">
            <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server">
            </asp:GridView>
        </div>

        <div id="menu">
            <a id="back">Back</a>
            <asp:LinkButton ID="exportBtn" OnClick="exportBtn_Click" runat="server" Text="Export to Excel"></asp:LinkButton>

            
        </div>

        <div id="storeProg">
            <asp:Label ID="info" runat="server"></asp:Label>
        </div>
        
        <div id="gridContainer" class="panes" runat="server">
            <asp:SqlDataSource ID="SQLs1" runat="server" SelectCommandType="StoredProcedure" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting" EnableCaching="true" CacheDuration="300"></asp:SqlDataSource>
            <asp:SqlDataSource ID="SQLs2" runat="server" SelectCommandType="StoredProcedure" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting" EnableCaching="true" CacheDuration="300"></asp:SqlDataSource>
            
            <br />

            <asp:Label ID="GVs1L" runat="server" Text="Quantity Sold:"></asp:Label>
            <br />
            <br />
            <asp:GridView CssClass="neoGrid" ID="GVs1" runat="server" OnRowDataBound="GridDataBound" DataSourceID="SQLs1" HeaderStyle-Wrap="false" RowStyle-Wrap="false" AutoGenerateColumns="true">
            </asp:GridView>
            
            <br />
            <br />
            <br />

            <asp:Label ID="GVs2L" runat="server" Text="Commissionable Revenue:"></asp:Label>
            <br />
            <br />
            <asp:GridView CssClass="neoGrid" ID="GVs2" runat="server" OnRowDataBound="GridDataBound" DataSourceID="SQLs2" HeaderStyle-Wrap="false" RowStyle-Wrap="false" AutoGenerateColumns="true">
            </asp:GridView>
        </div>

    </div>
    </form>
</body>
</html>
