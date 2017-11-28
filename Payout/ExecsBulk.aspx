<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExecsBulk.aspx.cs" Inherits="Payout.ExecsBulk" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Execs Bulk Add</title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jqueryui/1.9.1/jquery-ui.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            //
        });
    </script>
    <style type="text/css">
        th:first-child, td:first-child {
            width: 50px!important;
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

        <div id="menu">
            <asp:LinkButton ID="overviewBtn" runat="server" Text="Overview" OnClick="overviewBtn_Click"></asp:LinkButton>
            <%--<asp:LinkButton ID="backBtn" OnClick="backBtn_Click" runat="server" Text="Back"></asp:LinkButton>--%>
        </div>
        
        <asp:SqlDataSource ID="SQLprogram" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting"
            SelectCommand=" SELECT 'All' AS Program UNION ALL
                            SELECT 'CanadianTire' AS Program UNION ALL
                            SELECT Program
                            FROM
                            (
	                            SELECT DisplayProgram Program FROM PAYOUTOwnerPrograms UNION
	                            SELECT TOP 1000 Program FROM PAYOUTschedule WHERE Program != '' AND Program IS NOT NULL GROUP BY Program ORDER BY Program
                            ) tab">
        </asp:SqlDataSource>

        <asp:UpdatePanel ID="filterPanel" runat="server" UpdateMode="Always">
        <ContentTemplate>

            <div id="storeProg">
                Program: 
                <asp:DropDownList ID="progDDL" runat="server" style="width: 200px;"
                    DataSourceID="SQLprogram" DataValueField="Program" DataTextField="Program" AppendDataBoundItems="false">
                </asp:DropDownList> &nbsp;&nbsp;

                <asp:TextBox ID="FilterPersonTXT" runat="server"></asp:TextBox>
                <asp:Button ID="searchBtn" runat="server" OnClick="searchBtn_Click" Text="Search" />
            </div>
        
            <div id="gridContainer" class="panes" runat="server">
                <br />
            
                <asp:GridView CssClass="neoGrid" ID="gvOwners" runat="server" OnRowDataBound="GridDataBound" HeaderStyle-Wrap="false" RowStyle-Wrap="false" AutoGenerateColumns="false">
                    <EmptyDataTemplate>
                        No results found.
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:BoundField DataField="PersonId" />
                        <asp:BoundField DataField="Exec" />
                        <asp:TemplateField HeaderText="">
                            <ItemTemplate>
                                <asp:CheckBox ID="isExec" runat="server" OnCheckedChanged="CheckedChanged" AutoPostBack="true"></asp:CheckBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                    </Columns>
                </asp:GridView>

            </div>

        </ContentTemplate>
        </asp:UpdatePanel>

    </div>
    </form>
</body>
</html>
