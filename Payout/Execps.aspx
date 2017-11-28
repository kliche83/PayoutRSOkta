<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Execps.aspx.cs" Inherits="Payout.Execps" EnableEventValidation="false" %>
    
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Payouts | Exception Report</title>

    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />

    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script src="//code.jquery.com/ui/1.11.2/jquery-ui.js"></script>
    
    <script type="text/javascript">
        $(document).ready(function () {
            $(".tab").click(function () {
                $(".tab").removeClass("menuActive");
                $(this).addClass("menuActive");
                $(".div").hide();
                $("#" + $(this).attr("id") + "Div").fadeIn();
            });

            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function (source, args) {
                $("#<%= dateFrom.ClientID %>").datepicker();
                $("#<%= dateTo.ClientID %>").datepicker();
            });
        });

        $(function () {
            $("#<%= dateFrom.ClientID %>").datepicker();
        });

        $(function () {
            $("#<%= dateTo.ClientID %>").datepicker();
        });
    </script>
    <style type="text/css">
        .div {
            display: block;
            position: absolute;
            top: 110px;
            bottom: 10px;
            left: 10px;
            right: 10px;
            overflow: auto;
        }

        .grid { width: 100%; height: 100%; }

        #scheduleDiv { display: none; }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <div id="menu">
            <a id="sales" class="tab menuActive">Sales</a>
            <a id="schedule" class="tab">Schedule</a>
            
            <asp:LinkButton ID="exportSales" runat="server" onclick="exportSales_Click" Text="Export Sales to Excel" />
            <asp:LinkButton ID="exportSchedule" runat="server" onclick="exportSchedule_Click" Text="Export Schedule to Excel" />

            <a id="refresh" onclick="javascript: location.reload();" class="right">Refresh</a>
            <a id="back" href="Reports.aspx" class="right">Back</a>
        </div>

        <asp:SqlDataSource ID="SQLprogram" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="SELECT 'All' AS Program UNION ALL SELECT * FROM (SELECT TOP 1000 Program FROM PAYOUTschedule WHERE Program != '' AND Program IS NOT NULL GROUP BY Program ORDER BY Program)x">
        </asp:SqlDataSource>

        Store: 
        <asp:DropDownList ID="storeDDL" runat="server">
            <asp:ListItem Value="%" Text="All" Selected="True"></asp:ListItem>
            <asp:ListItem Value="costco" Text="Costco"></asp:ListItem>
            <asp:ListItem Value="kroger" Text="Kroger"></asp:ListItem>
            <asp:ListItem Value="sams" Text="Sam's"></asp:ListItem>
        </asp:DropDownList> &nbsp; 
        Program: 
        <asp:DropDownList ID="programDDL" runat="server" DataSourceID="SQLprogram" DataValueField="Program" DataTextField="Program" AppendDataBoundItems="false">
        </asp:DropDownList> &nbsp; 
        Store Number: #
        <asp:TextBox ID="StoreNumberTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="StoreNumberTXT_Filter" TargetControlID="StoreNumberTXT" runat="server" FilterMode="ValidChars" ValidChars="1234567890"></asp:FilteredTextBoxExtender> &nbsp; 
        Date: from 
        <asp:TextBox ID="dateFrom" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="dateFrom_Filter" TargetControlID="dateFrom" runat="server" FilterMode="ValidChars" ValidChars="1234567890-"></asp:FilteredTextBoxExtender>
        to 
        <asp:TextBox ID="dateTo" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="dateTo_Filter" TargetControlID="dateTo" runat="server" FilterMode="ValidChars" ValidChars="1234567890-"></asp:FilteredTextBoxExtender> &nbsp; 
        Qty: 
        <asp:DropDownList ID="opDDL" runat="server" style="width: 60px;">
            <asp:ListItem Value="All" Text="All" Selected="True"></asp:ListItem>
            <asp:ListItem Value="=" Text="="></asp:ListItem>
            <asp:ListItem Value=">=" Text=">="></asp:ListItem>
            <asp:ListItem Value=">" Text=">"></asp:ListItem>
            <asp:ListItem Value="<=" Text="<="></asp:ListItem>
            <asp:ListItem Value="<" Text="<"></asp:ListItem>
        </asp:DropDownList> &nbsp; 
        <asp:TextBox ID="opTXT" runat="server" style="width: 60px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="opTXT_Filter" TargetControlID="opTXT" runat="server" FilterMode="ValidChars" ValidChars="1234567890-"></asp:FilteredTextBoxExtender>

        <asp:Button ID="searchBTN" runat="server" OnClick="searchBTN_Click" Text="Search" />
        
        <br /><br />
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>

    <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Always">
        <ContentTemplate>
        
        <div id="salesDiv" class="div">
            <b>Sales data not matching to an entry on the schedule:</b>
            <br />
            <br />
            <asp:GridView ID="salesGrid" CssClass="grid" runat="server" AllowSorting="false" AllowPaging="true" PageSize="20" AutoGenerateColumns="true" DataSourceID="salesSQL"></asp:GridView>
        </div>
        
        <div id="scheduleDiv" class="div">
            <b>Schedule entries not matching to any sales data:</b>
            <br />
            <br />
            <asp:GridView ID="scheduleGrid" CssClass="grid" runat="server" AllowSorting="false" AllowPaging="true" PageSize="20" AutoGenerateColumns="true" DataSourceID="scheduleSQL"></asp:GridView>
        </div>
                
        </ContentTemplate>
    </asp:UpdatePanel>
        
        <asp:SqlDataSource ID="salesSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
            SelectCommand="">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="scheduleSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
            SelectCommand="">
        </asp:SqlDataSource>

    </form>
</body>
</html>
