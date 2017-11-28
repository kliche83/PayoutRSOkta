<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="Default.aspx.cs" Inherits="Payout._Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payout</title>
    
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <meta name="viewport" content="width=device-width, user-scalable=no" />

    <link rel="shortcut icon" type="image/x-icon" href="Content/favicon.png" />
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />

    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#panelBtn").click(function () {
                if ($("#panelBtn").text() == "Panel") {
                    
                    $("#panelDiv").animate({
                        top: "30px"
                    }, 300, "swing", function () {
                        $("#panelFrame").fadeIn();
                    });
                    $("#panelBtn").text("Close Panel");
                    $("#panelBtn").addClass("menuActive");
                }
                else {
                    $("#panelFrame").hide();
                    $("#panelDiv").animate({
                        top: "-300px"
                    }, 300, "swing");
                    $("#panelBtn").text("Panel");
                    $("#panelBtn").removeClass("menuActive");
                }
            });
        });




        //$(window).on("resize", function (event) {
        //    if ($(window).width() < 400) {
        //        $('#dashDivPre').attributes("style", "visibility: hidden;");
        //        $('#dashDiv').attributes("style", "top: 45px;");
        //    }
        //    else {
        //        $('#dashDivPre').attributes("style", "visibility: visible;");
        //        $('#dashDiv').attributes("style", "top: 130px;");
        //    }
        //});

        function refreshData(job) {
            var refreshData_confirm_value = document.createElement("INPUT");
            refreshData_confirm_value.type = "hidden";
            refreshData_confirm_value.name = "refreshData_confirm_value";
            var confirmMsg = "Do you want to " + job + " data for the 6/14 week ending on RS?\n\nNOTE: Depending on the amount of data, it may take up to 10 minutes for it to complete. You can use the site during this operation.\n\nNOTE: Click the " + job + " data button again to check status (you will see this confirmation first).";
            if (confirm(confirmMsg)) {
                refreshData_confirm_value.value = "Yes";
            } else {
                refreshData_confirm_value.value = "No";
            }
            document.forms[0].appendChild(refreshData_confirm_value);
        }

        function transferData() {
            var transferData_confirm_value = document.createElement("INPUT");
            transferData_confirm_value.type = "hidden";
            transferData_confirm_value.name = "transferData_confirm_value";
            var confirmMsg = "Do you want to transfer data for the 6/14 week ending to live?\n\nNOTE: This will overwrite existing data in the live site and is irreversible.\n\nNOTE: Depending on the amount of data, it may take a few minutes for it to complete.";
            if (confirm(confirmMsg)) {
                transferData_confirm_value.value = "Yes";
            } else {
                transferData_confirm_value.value = "No";
            }
            document.forms[0].appendChild(transferData_confirm_value);
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <div>

        <div id="menu" class="main" runat="server">
            <span id="notifSpan" runat="server">
                <asp:Image runat="server" ID="alert" ImageUrl="~/Content/alert.png" ImageAlign="Left" />
                <asp:Label runat="server" ID="notif"></asp:Label>
            </span>

            <span id="menuBtns" runat="server">
                <asp:LinkButton runat="server" ID="homeBtn" Text="Payout" OnClick="homeBtn_Click"></asp:LinkButton>
            </span>
            
            <span id="logoutSpan" runat="server" class="right">
                <a id="panelBtn" runat="server">Panel</a>                
                <asp:LinkButton runat="server" ID="logoutBtn" Text="Logout" OnClick="logoutBtn_Click"></asp:LinkButton>
            </span>
        </div>
        
<%--        <div id="loginDiv" runat="server">
            <asp:TextBox ID="username" runat="server" placeholder="Username"></asp:TextBox>            
            <asp:TextBox ID="password" runat="server" placeholder="Password" TextMode="Password"></asp:TextBox>
             <asp:LinkButton ID="loginBtn" runat="server" OnClick="loginBtn_Click" Text="Login" class="btn btn-primary btn-lg"></asp:LinkButton>
        </div>--%>
        
        <div id="panelDiv" class="clspanelDiv" runat="server">
            <iframe id="panelFrame" class="clspanelFrame" runat="server"></iframe>

            <div id="panelContent" class="clspanelContent" style="display: none;">
                <asp:LinkButton ID="panelWE" runat="server" OnClick="panelWE_Click" Text="WE Maint."></asp:LinkButton>                
                <asp:LinkButton runat="server" ID="transferBtn" Text="RS to Live" OnClick="transferBtn_Click" OnClientClick="transferData()"></asp:LinkButton>
                <asp:LinkButton runat="server" ID="postBtn" Text="Post Data" OnClick="refreshBtn_Click" OnClientClick="refreshData('post')"></asp:LinkButton>
                <asp:LinkButton runat="server" ID="refreshBtn" Text="Refresh Data" OnClick="refreshBtn_Click" OnClientClick="refreshData('refresh')"></asp:LinkButton>
                <asp:LinkButton runat="server" ID="refreshDevBtn" Text="RefreshDev Data" OnClick="refreshBtn_Click" OnClientClick="refreshData('refresh')"></asp:LinkButton>
            </div>
        </div>
                
        <ul id="dashDivPre" runat="server">
                <li runat="server" id="AccountManager">Accounts</li>
                <li runat="server" id="Avg">Averages</li>
                <li runat="server" id="CC">Carbon Copy</li>
                <li runat="server" id="Comm">Commissions</li>
                <li runat="server" id="Edit">Edit Import</li>
                <li runat="server" id="Exceps">Exceptions</li>
                <li runat="server" id="ExecsBulk">Executives</li>
                <li runat="server" id="Subsidy">Extra Earnings</li>
                <li runat="server" id="Import">Import Data</li>
                <li runat="server" id="Items">Items</li>
                <li runat="server" id="Override">Overrides</li>
                <li runat="server" id="People">People</li>
                <li runat="server" id="Grids">Reports</li>
                <li runat="server" id="Schedule">Schedule</li>
                <li runat="server" id="Subs">Substitutions</li>
                <li runat="server" id="ScheduleTrainer">PM & Merch</li>
                <li runat="server" id="AuditSales">Sales Audit</li>
                <li runat="server" id="GridPM" style="display:none">Reports PM</li>                
                <li runat="server" id="GridSummaryYTD">Reports YTD</li>
                <li runat="server" id="YTDWeeklyReport">YTD & Weekly</li>
                <li runat="server" id="SalesFile">Sales Export</li>
                <li runat="server" id="ShowsCount">Shows Count</li>

                 <%-- <li runat="server" id="Howto">How-to</li>
                 <li runat="server" id="Codes">Short Codes</li>--%>
        </ul>

        <div id="dashDiv" runat="server">            
            <div id="loading">
                <asp:Image runat="server" ID="lodGif" ImageUrl="~/Content/load.gif" />
            </div>

            <iframe runat="server" id="contentFrame" src="Grids.aspx"></iframe>
        </div>

        <span id="footer">
            &copy; <asp:Label runat="server" ID="yr"></asp:Label> Smart Circle LLC
        </span>

        <script type="text/javascript">
            $(document).ready(function () {
                $('#contentFrame').hide();
                //$('#Grids').addClass('selected');
                selectPage();

                $('#dashDivPre li').click(function () {
                    $('#dashDivPre li').removeClass('selected');
                    $(this).addClass('selected');
                    $('#contentFrame').hide();
                    $('#loading').show();
                    $('#contentFrame').attr('src', $(this).attr("id") + '.aspx');
                });

                $('#contentFrame').load(function () {
                    $('#loading').fadeOut(function () { $('#contentFrame').fadeIn(); });
                });

                function selectPage() {                    
                    if (typeof (contentFrame) === "undefined") {
                        $('#Grids').addClass('selected');
                    }
                    else {
                        var PageName = $('#contentFrame')[0].attributes['1'].value;
                        PageName = PageName.replace(".aspx", "");
                        $('#' + PageName).addClass('selected');
                    }
                }
            });
        </script>

    </div>
    </form>
</body>
</html>
