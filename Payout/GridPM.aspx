<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridPM.aspx.cs" Inherits="Payout.GridPM" enableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Reports</title>
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

            $("#WeeklyShow").click(function () {
                $("#tipSpan").hide();
               
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
            <asp:LinkButton ID="exportSummary" OnClick="exportSummary_Click" runat="server" Text="Export Summary to Excel"></asp:LinkButton>
            <asp:LinkButton ID="exportAll" OnClick="exportAll_Click" runat="server" Text="Export All to Excel"></asp:LinkButton>       
            <a id="email" runat="server">Send via Email</a>            
        </div>

        <asp:SqlDataSource ID="weSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>

        <asp:UpdatePanel ID="filterPanel" runat="server" UpdateMode="Always">
        <ContentTemplate>

            <div id="storeProg">
                 
                 <asp:Label ID="lblWeekending" runat="server" Text="Week Ending: "></asp:Label>
                <asp:DropDownList ID="webStartDate" runat="server" OnSelectedIndexChanged="DropChanged" AutoPostBack="true" style="width: 110px;" DataSourceID="weSQL" DataValueField="StartDate" DataTextField="WeekEnding" AppendDataBoundItems="false">
                </asp:DropDownList>
                <span id="ownerSpan" runat="server">
                     &nbsp; / &nbsp;
                   <asp:Label ID="lblOwner" runat="server" Text="Owner: "></asp:Label>
                    <asp:SqlDataSource ID="ownerSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting" EnableCaching="true" CacheDuration="300"
                        SelectCommand="">
                    </asp:SqlDataSource>
                    <asp:DropDownList ID="webOwner" runat="server" AutoPostBack="true" DataSourceID="ownerSQL" DataValueField="OwnerName" DataTextField="OwnerName" AppendDataBoundItems="false" OnSelectedIndexChanged="DropChanged" style="width: 200px;">
                    </asp:DropDownList>
                </span>

                &nbsp; / &nbsp;
                  <asp:Label ID="LblTrainer" runat="server" Text="Trainer: "></asp:Label>
                       <asp:DropDownList ID="webTrainer" runat="server" DataSourceID="TrainerSQL"   DataValueField="id" DataTextField="Trainer"  AppendDataBoundItems="true" onselectedindexchanged="DropChanged" AutoPostBack="true" ></asp:DropDownList> 
                       <asp:SqlDataSource ID="TrainerSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"   SelectCommand="select 0 As ID ,'All'  as Trainer  Union  all SELECT [ID],([Firstname] + ' ' +[Lastname]) as Trainer FROM [dbo].[PAYOUTtrainer]   ">  </asp:SqlDataSource>                         

                  &nbsp;  &nbsp;

                <span id="fwSpan" runat="server">
                    &nbsp;  &nbsp;
                  
                      <asp:Label ID="lblStore" runat="server" Text="/   Store: "></asp:Label>
                    <asp:SqlDataSource ID="storeSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting" EnableCaching="true" CacheDuration="300"
                        SelectCommand="">
                    </asp:SqlDataSource>
                    <asp:DropDownList ID="webStoreName" runat="server" AutoPostBack="true" DataSourceID="storeSQL" DataValueField="StoreName" DataTextField="StoreName" AppendDataBoundItems="false" OnSelectedIndexChanged="DropChanged" style="width: 110px;">
                    </asp:DropDownList>
                    &nbsp;  &nbsp;
                    <%-- Program:  --%>
                    <asp:SqlDataSource ID="progSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting" EnableCaching="true" CacheDuration="300"
                        SelectCommand="">
                    </asp:SqlDataSource>
                    <asp:Label ID="lblprgm" runat="server" Text="/ Program: "></asp:Label>
                    <asp:DropDownList ID="webProgram" runat="server" AutoPostBack="true" DataSourceID="progSQL" DataValueField="Program" DataTextField="Program" AppendDataBoundItems="false" OnSelectedIndexChanged="DropChanged" style="width: 200px;">
                    </asp:DropDownList>
                    &nbsp;  &nbsp;
                    
                      <asp:Label ID="lblLoc" runat="server" Text="/ Location: "></asp:Label>
                    <asp:SqlDataSource ID="locSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting" EnableCaching="true" CacheDuration="300"
                        SelectCommand="">
                      
                    </asp:SqlDataSource>
                    <asp:DropDownList ID="webLocation" runat="server" AutoPostBack="true" DataSourceID="locSQL" DataValueField="Location" DataTextField="Location" AppendDataBoundItems="false" OnSelectedIndexChanged="DropChanged" style="width: 200px;">
                    </asp:DropDownList>
                      
                    &nbsp;  &nbsp;
                       <asp:Label ID="lblStorNum" runat="server" Text="/ Store #:  "></asp:Label> 
                    
                    <asp:TextBox ID="webStoreNumber" runat="server" AutoPostBack="true" OnTextChanged="TextChanged" style="width: 50px;"></asp:TextBox>
                    <asp:FilteredTextBoxExtender ID="webStoreNumber_Filter" TargetControlID="webStoreNumber" runat="server" FilterType="Numbers"></asp:FilteredTextBoxExtender>
                </span>
            
                <%--Location: 
                <asp:SqlDataSource ID="locationSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting" EnableCaching="true" CacheDuration="300"
                    SelectCommand="SELECT DISTINCT Location FROM PAYOUTsummary GROUP BY Location ORDER BY Location">
                </asp:SqlDataSource>
                <asp:DropDownList ID="locationDDL" runat="server" AutoPostBack="true" DataSourceID="locationSQL" DataValueField="Location" DataTextField="Location" AppendDataBoundItems="true" OnSelectedIndexChanged="DropChanged">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList> 
            
                Hub: 
                <asp:SqlDataSource ID="hubSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting" EnableCaching="true" CacheDuration="300"
                    SelectCommand="SELECT DISTINCT Hub FROM PAYOUTsummary GROUP BY Hub ORDER BY Hub">
                </asp:SqlDataSource>
                <asp:DropDownList ID="hubDDL" runat="server" AutoPostBack="true" DataSourceID="hubSQL" DataValueField="Hub" DataTextField="Hub" AppendDataBoundItems="true" OnSelectedIndexChanged="DropChanged">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList> 
            
                <asp:Label ID="StoreNameL" runat="server"></asp:Label>
                &nbsp;/&nbsp;
                <asp:Label ID="progL" runat="server"></asp:Label>--%>
            </div>
        
            <div id="gridContainer" class="panes" runat="server">
                <br />
                <b>Summary</b> <span id="tipSpan" runat="server">[ Double-click a row for more details ] </span>
                <br />

                <asp:SqlDataSource ID="SQLs1" runat="server" SelectCommandType="StoredProcedure" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting" EnableCaching="true" CacheDuration="300"></asp:SqlDataSource>
                <asp:SqlDataSource ID="SQLs2" runat="server" SelectCommandType="StoredProcedure" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting" EnableCaching="true" CacheDuration="300"></asp:SqlDataSource>
            
                <asp:Label ID="GVs1L" runat="server"></asp:Label>
                <br />
                <asp:GridView CssClass="neoGrid" ID="GVs1" runat="server" OnRowDataBound="GridDataBound" DataSourceID="SQLs1" HeaderStyle-Wrap="false" RowStyle-Wrap="false" AutoGenerateColumns="true" OnSelectedIndexChanged="GridIndexChanged">
                    <EmptyDataTemplate>
                        No results found.
                    </EmptyDataTemplate>
                </asp:GridView>
            
                <%--<asp:Label ID="GVs2L" runat="server"></asp:Label>
                <br />
                <asp:GridView CssClass="neoGrid" ID="GVs2" runat="server" OnRowDataBound="GridDataBound" DataSourceID="SQLs2" HeaderStyle-Wrap="false" RowStyle-Wrap="false" AutoGenerateColumns="true">
                </asp:GridView>--%>
            
                <%--<asp:SqlDataSource ID="SQLm" runat="server" SelectCommandType="StoredProcedure" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting" EnableCaching="true" CacheDuration="300"></asp:SqlDataSource>
                <asp:Label ID="GVmL" runat="server"></asp:Label>
                <br />
                <asp:GridView CssClass="neoGrid" ID="GVm" runat="server" OnRowDataBound="GridDataBound" DataSourceID="SQLm" HeaderStyle-Wrap="false" RowStyle-Wrap="false" AutoGenerateColumns="true">
                </asp:GridView>--%>
            </div>

        </ContentTemplate>
        </asp:UpdatePanel>

    </div>
    </form>
</body>
</html>

