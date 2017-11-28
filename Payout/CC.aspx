<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="CC.aspx.cs" Inherits="WebApplication3.CC" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Carbon Copy</title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            jQuery(function ($) {
                var focusedElementId = "";
                var prm = Sys.WebForms.PageRequestManager.getInstance();
                prm.add_beginRequest(function (source, args) {
                    var fe = document.activeElement;
                    if (fe != null) {
                        focusedElementId = fe.id;
                    } else {
                        focusedElementId = "";
                    }
                });
                prm.add_endRequest(function (source, args) {
                    bindKeyDown();
                    if (focusedElementId != "") {
                        $("#" + focusedElementId).focus();
                    }
                });
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="menu">
            
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>
        </div>
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>
        
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand=""
            UpdateCommand="">

            <UpdateParameters>
                <asp:Parameter Name="To" Type="String" />
                <asp:Parameter Name="CC1" Type="String" />
                <asp:Parameter Name="CC2" Type="String" />
                <asp:Parameter Name="CC3" Type="String" />
                <asp:Parameter Name="CC4" Type="String" />
                <asp:Parameter Name="CC5" Type="String" />
                <asp:Parameter Name="CC6" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="SQLpeople" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="SELECT Id, (Firstname + ' ' + Lastname) AS Name FROM PAYOUTpeople GROUP BY Id, Firstname, Lastname ORDER BY Firstname, Lastname">
        </asp:SqlDataSource>

        <div id="notAdd">

        Report Type: 
            <asp:DropDownList ID="typeDDL" runat="server" AutoPostBack="false">
                <asp:ListItem Value="Sales">Sales</asp:ListItem>
                <asp:ListItem Value="Overrides">Overrides</asp:ListItem>
                <asp:ListItem Value="Payouts">Payouts</asp:ListItem>
                <asp:ListItem Value="FWreports">FWreports</asp:ListItem>
            </asp:DropDownList>

        Name: 
        <asp:TextBox ID="nameTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 

        <asp:Button ID="searchBTN" runat="server" OnClick="searchBTN_Click" Text="Search" />
        
        <br /><br />

<%--<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
<ContentTemplate>--%>
        <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" AllowPaging="True" PageSize="15" 
            HeaderStyle-Wrap="false" RowStyle-Wrap="false" DataKeyNames="To" OnRowDataBound="GridView1_RowDataBound">

<HeaderStyle Wrap="False"></HeaderStyle>

            <PagerStyle CssClass="neoPager" />
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:TemplateField>
                    <EditItemTemplate>
                        <asp:LinkButton runat="server" ID="CancelButton" CommandName="CANCEL" Text="Cancel"></asp:LinkButton>
                        <asp:LinkButton runat="server" ID="UpdateButton" CommandName="UPDATE" Text="Update"></asp:LinkButton>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="EditButton" CommandName="EDIT" Text="Edit"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Firstname" SortExpression="Firstname">
                    <ItemTemplate>
                        <asp:Label ID="Firstname" runat="server" Text='<%# Bind("Firstname") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Lastname" SortExpression="Lastname">
                    <ItemTemplate>
                        <asp:Label ID="Lastname" runat="server" Text='<%# Bind("Lastname") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="To" SortExpression="To">
                    <ItemTemplate>
                        <asp:Label ID="To" runat="server" Text='<%# Bind("To") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="CC 1" SortExpression="CC1">
                    <ItemTemplate>
                        <asp:Label ID="LCC1" runat="server" Text='<%# Bind("CC1Name") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="CC1" runat="server" DataSourceID="SQLpeople" SelectedValue='<%# Bind("CC1") %>' AppendDataBoundItems="true" DataTextField="Name" DataValueField="Id" style="border: 0;">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="CC 2" SortExpression="CC2">
                    <ItemTemplate>
                        <asp:Label ID="LCC2" runat="server" Text='<%# Bind("CC2Name") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="CC2" runat="server" DataSourceID="SQLpeople" SelectedValue='<%# Bind("CC2") %>' AppendDataBoundItems="true" DataTextField="Name" DataValueField="Id" style="border: 0;">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="CC 3" SortExpression="CC3">
                    <ItemTemplate>
                        <asp:Label ID="LCC3" runat="server" Text='<%# Bind("CC3Name") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="CC3" runat="server" DataSourceID="SQLpeople" SelectedValue='<%# Bind("CC3") %>' AppendDataBoundItems="true" DataTextField="Name" DataValueField="Id" style="border: 0;">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="CC 4" SortExpression="CC4">
                    <ItemTemplate>
                        <asp:Label ID="LCC4" runat="server" Text='<%# Bind("CC4Name") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="CC4" runat="server" DataSourceID="SQLpeople" SelectedValue='<%# Bind("CC4") %>' AppendDataBoundItems="true" DataTextField="Name" DataValueField="Id" style="border: 0;">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="CC 5" SortExpression="CC5">
                    <ItemTemplate>
                        <asp:Label ID="LCC5" runat="server" Text='<%# Bind("CC5Name") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="CC5" runat="server" DataSourceID="SQLpeople" SelectedValue='<%# Bind("CC5") %>' AppendDataBoundItems="true" DataTextField="Name" DataValueField="Id" style="border: 0;">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="CC 6" SortExpression="CC6">
                    <ItemTemplate>
                        <asp:Label ID="LCC6" runat="server" Text='<%# Bind("CC6Name") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="CC6" runat="server" DataSourceID="SQLpeople" SelectedValue='<%# Bind("CC6") %>' AppendDataBoundItems="true" DataTextField="Name" DataValueField="Id" style="border: 0;">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>
            </Columns>

<RowStyle Wrap="False"></RowStyle>
        </asp:GridView>
    
        <asp:GridView CssClass="neoGrid" ID="GridView2" runat="server" AutoGenerateColumns="false" DataSourceID="SqlDataSource1" AllowPaging="false" PageSize="15" 
            AllowSorting="False" HeaderStyle-Wrap="false" RowStyle-Wrap="false" style="display: none;">
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="Firstname" HeaderText="Firstname" SortExpression="Firstname" ReadOnly="true" />
                <asp:BoundField DataField="Lastname" HeaderText="Lastname" SortExpression="Lastname" ReadOnly="true" />
                <asp:BoundField DataField="CC1Name" HeaderText="CC 1" SortExpression="CC1Name" ReadOnly="true" />
                <asp:BoundField DataField="CC2Name" HeaderText="CC 2" SortExpression="CC2Name" ReadOnly="true" />
                <asp:BoundField DataField="CC3Name" HeaderText="CC 3" SortExpression="CC3Name" ReadOnly="true" />
                <asp:BoundField DataField="CC4Name" HeaderText="CC 4" SortExpression="CC4Name" ReadOnly="true" />
                <asp:BoundField DataField="CC5Name" HeaderText="CC 5" SortExpression="CC5Name" ReadOnly="true" />
                <asp:BoundField DataField="CC6Name" HeaderText="CC 6" SortExpression="CC6Name" ReadOnly="true" />
            </Columns>
        </asp:GridView>

 <%--</ContentTemplate>
</asp:UpdatePanel>--%>

            </div>

    </div>
    </form>
</body>
</html>
