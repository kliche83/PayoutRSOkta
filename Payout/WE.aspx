<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WE.aspx.cs" Inherits="Payout.WE" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | WE Management</title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
</head>
<body style="margin: 0; padding: 0;">
    <form id="form1" runat="server">
    <div>
        <div id="menu" style="display: none;">
            <asp:LinkButton ID="backBtn" runat="server" OnClick="backBtn_Click" Text="Back"></asp:LinkButton>
                 </div>

        <%--<br /><br />--%>
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>

        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>

<asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Always">
<ContentTemplate>
        <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server" AutoGenerateColumns="false" DataSourceID="SqlDataSource1" AllowPaging="True" PageSize="15" 
            AllowSorting="False" HeaderStyle-Wrap="false" RowStyle-Wrap="false" OnRowDataBound="GridView1_RowDataBound">
            <PagerStyle CssClass="neoPager" />
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="Id" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                      
                        <asp:LinkButton ID="refreshBtn" runat="server" Text="Refresh Data" OnClick="Trigger_Click" CssClass="actionLink"></asp:LinkButton>
                          &nbsp; &nbsp;
                        <asp:LinkButton ID="postBtn" runat="server" Text="Post Data" OnClick="Trigger_Click" CssClass="actionLink"></asp:LinkButton>
                        &nbsp; &nbsp;
                        <asp:LinkButton ID="transferBtn" runat="server" Text="RS to Live" OnClick="Trigger_Click" CssClass="actionLink"></asp:LinkButton>
                        
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="WeekEnding" HeaderText="Week Ending" SortExpression="WeekEnding" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="StartDate" HeaderText="Start Date" SortExpression="StartDate" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:TemplateField HeaderText="Internal" SortExpression="Internal">
                    <ItemTemplate>
                        <asp:CheckBox ID="Internal" runat="server" Checked='<%# Bind("Internal") %>' OnCheckedChanged="CheckChanged" AutoPostBack="true" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Owner" SortExpression="Owner">
                    <ItemTemplate>
                        <asp:CheckBox ID="Owner" runat="server" Checked='<%# Bind("Owner") %>' OnCheckedChanged="CheckChanged" AutoPostBack="true" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Hub" SortExpression="Hub">
                    <ItemTemplate>
                        <asp:CheckBox ID="Hub" runat="server" Checked='<%# Bind("Hub") %>' OnCheckedChanged="CheckChanged" AutoPostBack="true" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="NC" SortExpression="NC">
                    <ItemTemplate>
                        <asp:CheckBox ID="NC" runat="server" Checked='<%# Bind("NC") %>' OnCheckedChanged="CheckChanged" AutoPostBack="true" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="RC" SortExpression="RC">
                    <ItemTemplate>
                        <asp:CheckBox ID="RC" runat="server" Checked='<%# Bind("RC") %>' OnCheckedChanged="CheckChanged" AutoPostBack="true" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="RSM" SortExpression="RSM">
                    <ItemTemplate>
                        <asp:CheckBox ID="RSM" runat="server" Checked='<%# Bind("RSM") %>' OnCheckedChanged="CheckChanged" AutoPostBack="true" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="LastRefreshOn" HeaderText="Last Refresh On" SortExpression="LastRefreshOn" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy @ hh:mm tt}" HtmlEncode="false" />
                <asp:BoundField DataField="LastRefreshBy" HeaderText="Last Refresh By" SortExpression="LastRefreshBy" ReadOnly="True" />
                <asp:BoundField DataField="LastPostOn" HeaderText="Last Post On" SortExpression="LastPostOn" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy @ hh:mm tt}" HtmlEncode="false" />
                <asp:BoundField DataField="LastPostBy" HeaderText="Last Post By" SortExpression="LastPostBy" ReadOnly="True" />
            </Columns>
        </asp:GridView>

 </ContentTemplate>
</asp:UpdatePanel>

            </div>

  
    </form>
</body>
</html>
