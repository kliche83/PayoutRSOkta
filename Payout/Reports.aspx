<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="Reports.aspx.cs" Inherits="WebApplication3.Reports" %>
    
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Payouts | Reports</title>

    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />

    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script src="//code.jquery.com/ui/1.11.2/jquery-ui.js"></script>
    
    <script type="text/javascript">
        $(document).ready(function () {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function (source, args) {
                $("#<%= StartDate.ClientID %>").datepicker();
                $("#<%= viewBtn.ClientID %>").click(function () {
                    $("#<%= viewBtn.ClientID %>").hide();
                    $("#progress").fadeIn();
                });
            });

            $(function() {
                $("#<%= StartDate.ClientID %>").datepicker();
            });

            $("#progress").hide();

            $("#<%= viewBtn.ClientID %>").click(function () {
                $("#<%= viewBtn.ClientID %>").hide();
                $("#progress").fadeIn();
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">

        <div id="menu">
            <a id="excep" href="Execps.aspx">Exception Report</a>

            <a id="refresh" onclick="javascript: location.reload();" class="right">Refresh</a>
        </div>
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>

        <asp:UpdatePanel ID="cyclePanel" runat="server" UpdateMode="Always">
            <ContentTemplate>
        
                <%--<asp:SqlDataSource ID="storeSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
                    SelectCommand="SELECT StoreName FROM PAYOUTschedule GROUP BY StoreName ORDER BY StoreName">
                </asp:SqlDataSource>--%>
                Store: 
                <asp:DropDownList ID="storeDDL" runat="server" OnSelectedIndexChanged="storeDDL_SelectedIndexChanged" AutoPostBack="true"> <%--AppendDataBoundItems="true" DataSourceID="storeSQL" DataValueField="StoreName" DataTextField="StoreName"--%>
                    <asp:ListItem Selected="True"></asp:ListItem>
                    <asp:ListItem Value="Costco">Costco</asp:ListItem>
                    <asp:ListItem Value="Kroger">Kroger</asp:ListItem>
                    <asp:ListItem Value="Sams">Sam's</asp:ListItem>
                </asp:DropDownList>

                <br />

                <asp:SqlDataSource ID="progSQL" runat="server" SelectCommand="" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"></asp:SqlDataSource>
                Program: 
                <asp:DropDownList ID="progDDL" runat="server" AutoPostBack="True" AppendDataBoundItems="false" DataTextField="Program" DataValueField="Program" DataSourceID="progSQL">
                    <%--<asp:ListItem></asp:ListItem>--%>
                </asp:DropDownList>

                <br />

                Start Date:
                <asp:TextBox ID="StartDate" runat="server" OnTextChanged="FieldChanged" AutoPostBack="true" placeholder="mm/dd/yyyy"></asp:TextBox>

                <br />

                Duration: 
                <asp:TextBox ID="Duration" runat="server" Width="30px" OnTextChanged="FieldChanged" AutoPostBack="true"></asp:TextBox>
                days

                <br /><br />

                <asp:Button ID="viewBtn" runat="server" Text="Generate" OnClick="viewBtn_Click" CssClass="uploadBtn" />

            </ContentTemplate>
        </asp:UpdatePanel>
        
        <img id="progress" src="Content/load.gif" style="margin-left: 240px;" /> 

    </form>
</body>
</html>
