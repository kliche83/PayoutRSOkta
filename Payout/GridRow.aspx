<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridRow.aspx.cs" Inherits="Payout.GridRow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        
        <asp:Button ID="sendBtn" runat="server" OnClick="sendBtn_Click" Text="Send" />

        <br /><br /><br /><br /><br /><br />
    
        <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" />
                <asp:BoundField DataField="Program" HeaderText="Program" SortExpression="Program" />
                <asp:BoundField DataField="SalesDate" HeaderText="SalesDate" SortExpression="SalesDate" />
                <asp:BoundField DataField="StoreName" HeaderText="StoreName" SortExpression="StoreName" />
                <asp:BoundField DataField="StoreNumber" HeaderText="StoreNumber" SortExpression="StoreNumber" />
                <asp:BoundField DataField="ItemNumber" HeaderText="ItemNumber" SortExpression="ItemNumber" />
                <asp:BoundField DataField="ItemName" HeaderText="ItemName" SortExpression="ItemName" />
                <asp:BoundField DataField="Qty" HeaderText="Qty" SortExpression="Qty" />
                <asp:BoundField DataField="UnitCost" HeaderText="UnitCost" SortExpression="UnitCost" />
                <asp:BoundField DataField="ExtendedCost" HeaderText="ExtendedCost" SortExpression="ExtendedCost" />
                <asp:BoundField DataField="OwnerFirstname" HeaderText="OwnerFirstname" SortExpression="OwnerFirstname" />
                <asp:BoundField DataField="OwnerLastname" HeaderText="OwnerLastname" SortExpression="OwnerLastname" />
                <asp:BoundField DataField="StartDate" HeaderText="StartDate" SortExpression="StartDate" />
                <asp:BoundField DataField="EndDate" HeaderText="EndDate" SortExpression="EndDate" />
                <asp:BoundField DataField="City" HeaderText="City" SortExpression="City" />
                <asp:BoundField DataField="State" HeaderText="State" SortExpression="State" />
                <asp:BoundField DataField="HubFirstname" HeaderText="HubFirstname" SortExpression="HubFirstname" />
                <asp:BoundField DataField="HubLastname" HeaderText="HubLastname" SortExpression="HubLastname" />
                <asp:BoundField DataField="ImportedBy" HeaderText="ImportedBy" SortExpression="ImportedBy" />
                <asp:BoundField DataField="ImportedOn" HeaderText="ImportedOn" SortExpression="ImportedOn" />
                <asp:BoundField DataField="Exception" HeaderText="Exception" SortExpression="Exception" />
                <asp:BoundField DataField="Lock" HeaderText="Lock" SortExpression="Lock" />
            </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="SELECT TOP 100 * FROM [PAYOUTsales]"></asp:SqlDataSource>
    
    </div>
    </form>
</body>
</html>
