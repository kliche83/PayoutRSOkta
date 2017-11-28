<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="ReportMaker.aspx.cs" Inherits="WebApplication3.ReportMaker" %>
    
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
            $("#progress").hide();
            //$("#<%= viewBtn.ClientID %>").hide();

            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function (source, args) {
                $("#<%= viewBtn.ClientID %>").click(function () {
                    $("#<%= viewBtn.ClientID %>").hide();
                    $("#progress").fadeIn();
                });

                $("#<%= typeDDL.ClientID %>").change(function () {
                    if ($("#<%= typeDDL.ClientID %>").val() != "") {
                        //$("#<%= viewBtn.ClientID %>").fadeIn("");
                    }
                    else {
                        //$("#<%= viewBtn.ClientID %>").fadeOut("");
                    }
                });
            });

            $("#<%= viewBtn.ClientID %>").click(function () {
                $("#<%= viewBtn.ClientID %>").hide();
                $("#progress").fadeIn();
            });

            $("#<%= typeDDL.ClientID %>").change(function () {
                if ($("#<%= typeDDL.ClientID %>").val() != "") {
                    //$("#<%= viewBtn.ClientID %>").fadeIn("");
                }
                else {
                    //$("#<%= viewBtn.ClientID %>").fadeOut("");
                }
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">

        <div id="menu">
            
        </div>
        
        <br /><br />
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>

        <asp:UpdatePanel ID="cyclePanel" runat="server" UpdateMode="Always">
            <ContentTemplate>
        
                <asp:SqlDataSource ID="storeSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
                    SelectCommand="">
                </asp:SqlDataSource>

                <span runat="server" id="typeSpan">
                    Type: 
                    <asp:DropDownList ID="typeDDL" runat="server" required =" true"> 
                        <asp:ListItem></asp:ListItem>
                        <%--<asp:ListItem Value="sales">Sales Report</asp:ListItem>--%>
                        <asp:ListItem Value="payout" Selected="True">Payout Report</asp:ListItem>
                    </asp:DropDownList>

                    <br />
                </span>

                Week Ending: 
                <asp:DropDownList ID="weDDL" runat="server" OnSelectedIndexChanged="weDDL_SelectedIndexChanged" AutoPostBack="true" required ="true">
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem Value="20150419">04/19/2015</asp:ListItem>
                    <%--<asp:ListItem Value="20150405">04/05/2015</asp:ListItem>--%>
                </asp:DropDownList>

                <span runat="server" id="owDDLspan">
                    <br />

                    <asp:SqlDataSource ID="owSQL" runat="server" SelectCommand="" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"></asp:SqlDataSource>
                    Owner: 
                    <asp:DropDownList ID="owDDL" runat="server" OnSelectedIndexChanged="owDDL_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="false" DataTextField="OwnerName" DataValueField="OwnerName" DataSourceID="owSQL" required ="true">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </span>

                <br />

                Store: 
                <asp:DropDownList ID="storeDDL" runat="server" OnSelectedIndexChanged="storeDDL_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="false" DataSourceID="storeSQL" DataValueField="StoreName" DataTextField="StoreName" required ="true">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>

                <br />

                <asp:SqlDataSource ID="progSQL" runat="server" SelectCommand="" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"></asp:SqlDataSource>
                Program: 
                <asp:DropDownList ID="progDDL" runat="server" AutoPostBack="True" AppendDataBoundItems="false" DataTextField="Program" DataValueField="Program" DataSourceID="progSQL" required  ="true">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>

                <br /><br />

                <asp:Button ID="viewBtn" runat="server" Text="Generate" OnClick="viewBtn_Click" CssClass="uploadBtn" />

            </ContentTemplate>
        </asp:UpdatePanel>
        
        <img id="progress" src="Content/load.gif" style="margin-left: 240px;" /> 

    </form>
</body>
</html>
