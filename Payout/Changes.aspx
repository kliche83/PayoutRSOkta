<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Changes.aspx.cs" Inherits="Payout.Changes" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Changes</title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jqueryui/1.9.1/jquery-ui.min.js"></script>
    <script type="text/javascript" src="/Scripts/gridviewScroll.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            gridviewScroll();
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
                gridviewScroll();
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

        window.onresize = function (event) {
            gridviewScroll();
        };

        function gridviewScroll() {
            $('#<%= gvChanges.ClientID %>').gridviewScroll({
                width: window.innerWidth - 50,
                height: window.innerHeight - 170,
                freezesize: 0,
                arrowsize: 30,
                varrowtopimg: "Content/arrowvt.png",
                varrowbottomimg: "Content/arrowvb.png",
                harrowleftimg: "Content/arrowhl.png",
                harrowrightimg: "Content/arrowhr.png"
            });
        }
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

        <%--<div id="loadDiv" runat="server" style="position: absolute; top: 0; bottom: 0; left: 0; right: 0; background-color: rgbA(255, 255, 255, 0.8); z-index: 9999; overflow: hidden; text-align: center; padding: 120px 10px 10px 10px;">
            <span id="errorSpan" runat="server">
                <asp:Label ID="errorMsg" runat="server" ForeColor="Black" Font-Bold="true" Font-Size="Medium"></asp:Label>
                <br /><br />
                <input id="gotIt" runat="server" value="OK" style="padding: 10px; border: 0; background-color: #2764AB; color: #fff; cursor: pointer; width: 50px!important;" />
                <br />
            </span>

            <img id="loadGif" runat="server" src="../Content/load.gif" />
        </div>--%>

        <div id="menu">
            <asp:LinkButton ID="backBtn" OnClick="backBtn_Click" runat="server" Text="Back"></asp:LinkButton>
            <asp:LinkButton ID="exportBtn" OnClick="exportBtn_Click" runat="server" Text="Export to Excel"></asp:LinkButton>
        </div>

        <asp:SqlDataSource ID="weSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>

        <asp:UpdatePanel ID="filterPanel" runat="server" UpdateMode="Always">
        <ContentTemplate>

            <div id="storeProg">
                Week Ending: 
                <asp:DropDownList ID="weDDL" runat="server" style="width: 110px;" DataSourceID="weSQL" DataValueField="WeekEnding" DataTextField="WeekEnding" AppendDataBoundItems="false">
                </asp:DropDownList> &nbsp;&nbsp;
                <asp:Button ID="searchBtn" runat="server" OnClick="searchBtn_Click" Text="Search" />
            </div>
        
            <div id="gridContainer" class="panes" runat="server">
                <br />
                
                <asp:SqlDataSource ID="sqlChanges" runat="server" SelectCommandType="StoredProcedure" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting">
                    <SelectParameters>
                        <asp:Parameter Name="WE" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
            
                <asp:GridView CssClass="neoGrid" ID="gvChanges" runat="server" OnRowDataBound="GridDataBound" DataSourceID="sqlChanges" HeaderStyle-Wrap="false" RowStyle-Wrap="false" AutoGenerateColumns="true">
                    <EmptyDataTemplate>
                        There were no changes.
                    </EmptyDataTemplate>
                </asp:GridView>

            </div>

        </ContentTemplate>
        </asp:UpdatePanel>

    </div>
    </form>
</body>
</html>
