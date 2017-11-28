<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="periods.aspx.cs" Inherits="Payout.periods" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Periods</title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jqueryui/1.9.1/jquery-ui.min.js"></script>
    <script type="text/javascript" src="/Scripts/gridviewScroll.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            gridviewScroll();
            $("#loadDiv").not(".keep").hide();
            $("#errorSpan").not(".keep").hide();
            $("#loadGif").hide();
            $("#pbar_outerdiv").hide();

            var prm = Sys.WebForms.PageRequestManager.getInstance();

            prm.add_beginRequest(function (source, args) {
                $("#loadDiv").show();
                $("#loadGif").show();
            });

            prm.add_endRequest(function (source, args) {
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

        var timer = 0,
            perc = 0,
            timeTotal = 150,
            timeCount = 1000;

        function updateProgress(percentage) {
            var x = (percentage / timeTotal) * 100,
                y = x.toFixed(0);
            $('#pbar_innerdiv').css("width", x + "%");
            $('#pbar_innertext').text(y + "%");
        }

        function animateUpdate() {
            if (perc < timeTotal) {
                perc++;
                updateProgress(perc);
                timer = setTimeout(animateUpdate, timeCount);
            }
        }

        function generate() {
            perc = 0;
            animateUpdate();
            $("#pbar_outerdiv").fadeIn();
        }

        window.onresize = function (event) {
            gridviewScroll();
        };

        function gridviewScroll() {
            $('#<%= perGrid.ClientID %>').gridviewScroll({
                width: window.innerWidth - 50,
                height: window.innerHeight - 205,
                freezesize: 0,
                arrowsize: 30,
                varrowtopimg: "Content/arrowvt.png",
                varrowbottomimg: "Content/arrowvb.png",
                harrowleftimg: "Content/arrowhl.png",
                harrowrightimg: "Content/arrowhr.png"
            });
        }
    </script>
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
            <asp:LinkButton ID="dataBtn" OnClick="dataBtn_Click" runat="server" Text="Generate" OnClientClick="alert('This may take a minute or two.\n\Please wait on this page till then.\n\Newly generated report will be displayed when done.'); generate();"></asp:LinkButton>
            <asp:LinkButton ID="LinkButton1" OnClick="exportBtn_Click" runat="server" Text="Export to Excel"></asp:LinkButton>

            <%----%>
            <%--<a id="tip" onclick="javascript: alert('Click a row to highlight it.\n\Hold Ctrl and click multiple rows to highlight them.\n\n\Double-click a row for more details.');" class="right">Tip</a>--%>
        </div>

        <asp:UpdatePanel ID="filterPanel" runat="server" UpdateMode="Always">
        <ContentTemplate>

            <div id="storeProg">
                Year: 
                <asp:TextBox ID="webYear" runat="server" AutoPostBack="true" style="width: 50px;"></asp:TextBox>
                <asp:FilteredTextBoxExtender ID="webYear_Filter" TargetControlID="webYear" runat="server" FilterType="Numbers"></asp:FilteredTextBoxExtender>

                &nbsp; / &nbsp;
                
                <%--Period: 
                <asp:SqlDataSource ID="perSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting" EnableCaching="true" CacheDuration="300"
                    SelectCommand="">
                </asp:SqlDataSource>
                <asp:DropDownList ID="webPeriod" runat="server" AutoPostBack="true" DataSourceID="perSQL" DataValueField="PeriodId" DataTextField="DateRange" AppendDataBoundItems="false" style="width: 200px;">
                </asp:DropDownList>

                &nbsp; / &nbsp;--%>
                
                Division: 
                <asp:SqlDataSource ID="divSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting" EnableCaching="true" CacheDuration="300"
                    SelectCommand="">
                </asp:SqlDataSource>
                <asp:DropDownList ID="webDivision" runat="server" AutoPostBack="true" DataSourceID="divSQL" DataValueField="Division" DataTextField="Division" AppendDataBoundItems="false" style="width: 200px;">
                </asp:DropDownList>

                &nbsp; / &nbsp;
                
                Program: 
                <asp:SqlDataSource ID="progSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting" EnableCaching="true" CacheDuration="300"
                    SelectCommand="">
                </asp:SqlDataSource>
                <asp:DropDownList ID="webProgram" runat="server" AutoPostBack="true" DataSourceID="progSQL" DataValueField="Program" DataTextField="Program" AppendDataBoundItems="false" style="width: 200px;">
                </asp:DropDownList>
            </div>
        
            <div id="gridContainer" class="panes" runat="server" style="margin-top: 20px;">
                <%--<br />
                <b>Summary</b> <span id="tipSpan" runat="server">[ Double-click a row for more details ]</span>
                <br />--%>

                <div id="pbar_outerdiv" style="width: 300px; height: 20px; border: 1px solid grey; z-index: 1; position: relative;">
                    <div id="pbar_innerdiv" style="background-color: #ccc; z-index: 2; height: 100%; width: 0%;"></div>
                    <div id="pbar_innertext" style="z-index: 3; position: absolute; top: 0; left: 0; width: 100%; height: 100%; color: black; font-weight: bold; text-align: center;">0%</div>
                </div>

                <br />

                <asp:SqlDataSource ID="gridSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting"></asp:SqlDataSource>
            
                <asp:GridView CssClass="neoGrid" ID="perGrid" runat="server" OnRowDataBound="GridDataBound" DataSourceID="gridSQL" HeaderStyle-Wrap="false" RowStyle-Wrap="false" AutoGenerateColumns="true">
                    <EmptyDataTemplate>
                        No results found.
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>

        </ContentTemplate>
        </asp:UpdatePanel>

    </div>
    </form>
</body>
</html>
