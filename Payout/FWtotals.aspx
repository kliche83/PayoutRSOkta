<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FWtotals.aspx.cs" Inherits="Payout.FWtotals" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | FW Totals</title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jqueryui/1.9.1/jquery-ui.min.js"></script>
    <%--<script type="text/javascript" src="/Scripts/gridviewScroll.min.js"></script>--%>
    <script type="text/javascript">
        $(document).ready(function () {
            //gridviewScroll();
            $("#loadDiv").not(".keep").hide();
            $("#errorSpan").not(".keep").hide();
            $("#loadGif").hide();

            $(function () {
                $(".Date").datepicker({
                    beforeShowDay: function (date) {
                        // Allow only Saturdays
                        return [date.getDay() == 6];
                    }
                });
            });

            var prm = Sys.WebForms.PageRequestManager.getInstance();

            prm.add_beginRequest(function (source, args) {
                $("#loadDiv").show();
                $("#loadGif").show();
            });

            prm.add_endRequest(function (source, args) {
                $(".Date").datepicker({
                    beforeShowDay: function (date) {
                        // Allow only Saturdays
                        return [date.getDay() == 6];
                    }
                });
                //gridviewScroll();
                $("#loadDiv").not(".keep").hide();
                $("#errorSpan").not(".keep").hide();
                $("#loadGif").hide();

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
            });

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

            $("#dailyPayout").click(function () {
                location.href = "DailyGrid.aspx";
            });

            $("#gotIt").click(function () {
                $("#loadDiv").removeClass("keep");
                $("#loadDiv").hide();
                $("#errorSpan").removeClass("keep");
                $("#errorSpan").hide();
            });
        });

        //window.onresize = function (event) {
        //    gridviewScroll();
        //};

        <%--function gridviewScroll() {
            $('#<%= GVs1.ClientID %>').gridviewScroll({
                width: window.innerWidth - 50,
                height: window.innerHeight - 205,
                freezesize: 0,
                arrowsize: 30,
                varrowtopimg: "Content/arrowvt.png",
                varrowbottomimg: "Content/arrowvb.png",
                harrowleftimg: "Content/arrowhl.png",
                harrowrightimg: "Content/arrowhr.png"
            });
        }--%>
    </script>
    <style type="text/css">
        th, td {
            width: 11.11%!important;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>

        <div style="position: absolute; top: 0; left: 0; z-index: 99999; background-color: ActiveCaption; display: none;">
            <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server">
            </asp:GridView>
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

        <div id="menu">
            <asp:LinkButton ID="backBtn" OnClick="backBtn_Click" runat="server" Text="Back"></asp:LinkButton>
            <asp:LinkButton ID="exportBtn" OnClick="exportBtn_Click" runat="server" Text="Export to Excel"></asp:LinkButton>
            <%--<a id="maint" onclick="alert('Not available yet!');">Maintain</a>--%>

            
        </div>

        <asp:SqlDataSource ID="weSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>

        <asp:UpdatePanel ID="filterPanel" runat="server" UpdateMode="Always">
        <ContentTemplate>

            <div id="storeProg">
                From: <asp:TextBox ID="dateFrom" runat="server" CssClass="Date" style="width: 100px;" required></asp:TextBox> &nbsp;&nbsp;
                To: <asp:TextBox ID="dateTo" runat="server" CssClass="Date" style="width: 100px;" required></asp:TextBox> &nbsp;&nbsp;
                <asp:Button ID="searchBtn" runat="server" OnClick="searchBtn_Click" Text="Search" />
            </div>
        
            <div id="gridContainer" class="panes" runat="server">
                <%--<br />
                <b>FastWax Weekly Totals</b>--%>
                <br />
                
                <asp:SqlDataSource ID="sqlFastWax" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting"></asp:SqlDataSource>
                <asp:SqlDataSource ID="sqlDetailKits" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting"></asp:SqlDataSource>
                <asp:SqlDataSource ID="sqlActionPacks" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting"></asp:SqlDataSource>
                <asp:SqlDataSource ID="sqlCarShammy" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting"></asp:SqlDataSource>
                <asp:SqlDataSource ID="sqlShineRewind" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting"></asp:SqlDataSource>
            
                <%--<asp:Label ID="gvFastWaxL" runat="server" Text="FastWax" Font-Bold="true"></asp:Label>
                <br />--%>
                <asp:GridView CssClass="neoGrid" ID="gvFastWax" runat="server" OnRowDataBound="GridDataBound" DataSourceID="sqlFastWax" HeaderStyle-Wrap="false" RowStyle-Wrap="false" AutoGenerateColumns="false">
                    <EmptyDataTemplate>
                        No results found.
                    </EmptyDataTemplate>

                    <Columns>
                        <asp:BoundField DataField="Attribute" HeaderText="Attribute" SortExpression="Attribute" />
                        <asp:BoundField DataField="Week Ending" HeaderText="Week Ending" SortExpression="Week Ending" DataFormatString="{0:MM/dd/yyyy}" htmlencode="false" />
                        <asp:BoundField DataField="Retail" HeaderText="Retail" SortExpression="Retail" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="BCRF" HeaderText="BCRF" SortExpression="BCRF" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Total Sales" HeaderText="Total Sales" SortExpression="Total Sales" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Available Stock" HeaderText="Available Stock" SortExpression="Available Stock" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Rate" HeaderText="Rate" SortExpression="Rate" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Dollars" HeaderText="Dollars" SortExpression="Dollars" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Weeks Left" HeaderText="Weeks Left" SortExpression="Weeks Left" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />
                    </Columns>
                </asp:GridView>

                <br />

                <asp:GridView CssClass="neoGrid" ID="gvDetailKits" runat="server" OnRowDataBound="GridDataBound" DataSourceID="sqlDetailKits" HeaderStyle-Wrap="false" RowStyle-Wrap="false" AutoGenerateColumns="false">
                    <EmptyDataTemplate>
                        No results found.
                    </EmptyDataTemplate>

                    <Columns>
                        <asp:BoundField DataField="Attribute" HeaderText="Attribute" SortExpression="Attribute" />
                        <asp:BoundField DataField="Week Ending" HeaderText="Week Ending" SortExpression="Week Ending" DataFormatString="{0:MM/dd/yyyy}" htmlencode="false" />
                        <asp:BoundField DataField="Retail" HeaderText="Retail" SortExpression="Retail" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="BCRF" HeaderText="BCRF" SortExpression="BCRF" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Total Sales" HeaderText="Total Sales" SortExpression="Total Sales" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Available Stock" HeaderText="Available Stock" SortExpression="Available Stock" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Rate" HeaderText="Rate" SortExpression="Rate" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Dollars" HeaderText="Dollars" SortExpression="Dollars" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Weeks Left" HeaderText="Weeks Left" SortExpression="Weeks Left" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />
                    </Columns>
                </asp:GridView>

                <br />

                <asp:GridView CssClass="neoGrid" ID="gvActionPacks" runat="server" OnRowDataBound="GridDataBound" DataSourceID="sqlActionPacks" HeaderStyle-Wrap="false" RowStyle-Wrap="false" AutoGenerateColumns="false">
                    <EmptyDataTemplate>
                        No results found.
                    </EmptyDataTemplate>

                    <Columns>
                        <asp:BoundField DataField="Attribute" HeaderText="Attribute" SortExpression="Attribute" />
                        <asp:BoundField DataField="Week Ending" HeaderText="Week Ending" SortExpression="Week Ending" DataFormatString="{0:MM/dd/yyyy}" htmlencode="false" />
                        <asp:BoundField DataField="Retail" HeaderText="Retail" SortExpression="Retail" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="BCRF" HeaderText="BCRF" SortExpression="BCRF" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Total Sales" HeaderText="Total Sales" SortExpression="Total Sales" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Available Stock" HeaderText="Available Stock" SortExpression="Available Stock" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Rate" HeaderText="Rate" SortExpression="Rate" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Dollars" HeaderText="Dollars" SortExpression="Dollars" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Weeks Left" HeaderText="Weeks Left" SortExpression="Weeks Left" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />
                    </Columns>
                </asp:GridView>

                <br />

                <asp:GridView CssClass="neoGrid" ID="gvCarShammy" runat="server" OnRowDataBound="GridDataBound" DataSourceID="sqlCarShammy" HeaderStyle-Wrap="false" RowStyle-Wrap="false" AutoGenerateColumns="false">
                    <EmptyDataTemplate>
                        No results found.
                    </EmptyDataTemplate>

                    <Columns>
                        <asp:BoundField DataField="Attribute" HeaderText="Attribute" SortExpression="Attribute" />
                        <asp:BoundField DataField="Week Ending" HeaderText="Week Ending" SortExpression="Week Ending" DataFormatString="{0:MM/dd/yyyy}" htmlencode="false" />
                        <asp:BoundField DataField="Retail" HeaderText="Retail" SortExpression="Retail" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="BCRF" HeaderText="BCRF" SortExpression="BCRF" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Total Sales" HeaderText="Total Sales" SortExpression="Total Sales" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Available Stock" HeaderText="Available Stock" SortExpression="Available Stock" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Rate" HeaderText="Rate" SortExpression="Rate" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Dollars" HeaderText="Dollars" SortExpression="Dollars" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Weeks Left" HeaderText="Weeks Left" SortExpression="Weeks Left" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />
                    </Columns>
                </asp:GridView>

                <br />

                <asp:GridView CssClass="neoGrid" ID="gvShineRewind" runat="server" OnRowDataBound="GridDataBound" DataSourceID="sqlShineRewind" HeaderStyle-Wrap="false" RowStyle-Wrap="false" AutoGenerateColumns="false">
                    <EmptyDataTemplate>
                        No results found.
                    </EmptyDataTemplate>

                    <Columns>
                        <asp:BoundField DataField="Attribute" HeaderText="Attribute" SortExpression="Attribute" />
                        <asp:BoundField DataField="Week Ending" HeaderText="Week Ending" SortExpression="Week Ending" DataFormatString="{0:MM/dd/yyyy}" htmlencode="false" />
                        <asp:BoundField DataField="Retail" HeaderText="Retail" SortExpression="Retail" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="BCRF" HeaderText="BCRF" SortExpression="BCRF" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Total Sales" HeaderText="Total Sales" SortExpression="Total Sales" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Available Stock" HeaderText="Available Stock" SortExpression="Available Stock" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Rate" HeaderText="Rate" SortExpression="Rate" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Dollars" HeaderText="Dollars" SortExpression="Dollars" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Weeks Left" HeaderText="Weeks Left" SortExpression="Weeks Left" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />
                    </Columns>
                </asp:GridView>


            </div>

        </ContentTemplate>
        </asp:UpdatePanel>

    </div>
    </form>
</body>
</html>
