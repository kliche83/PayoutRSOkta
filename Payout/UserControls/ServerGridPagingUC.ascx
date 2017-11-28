<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServerGridPagingUC.ascx.cs" Inherits="WebApplication3.ServerGridPagingUC" %>



<div>
    PageSize:
    <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="PageSize_Changed">
        <asp:ListItem Text="10" Value="10" />
        <asp:ListItem Text="25" Value="25" />
        <asp:ListItem Text="50" Value="50" />
    </asp:DropDownList>
    <hr />


    <div style="height:auto;width:100%; padding:0; margin-top: 10px;" >
        <asp:GridView CssClass="neoGrid" id="GRVHeader" runat="server" HeaderStyle-Wrap="false" 
            OnRowDataBound="GRVHeader_RowDataBound" ShowHeaderWhenEmpty="true">            
        </asp:GridView>
    </div>
    

    <div style ="height:98%; width:100%; overflow:auto;">
        <asp:GridView CssClass="neoGrid" ID="GRV" runat="server" OnRowDataBound="GRV_RowDataBound"
            AllowSorting="True" HeaderStyle-Wrap="false" ItemStyle-Wrap="False" RowStyle-Wrap="false" >
            <%--<Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        Country:
                        <asp:DropDownList ID="ddlCountry" runat="server"                         
                        AppendDataBoundItems = "true">
                        <asp:ListItem Text = "ALL" Value = "ALL"></asp:ListItem>
                        <asp:ListItem Text = "Top 10" Value = "10"></asp:ListItem>
                        </asp:DropDownList>
                    </HeaderTemplate>
                </asp:TemplateField>
            </Columns>--%>

            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>

    <div style ="width:100%; overflow:auto; display:none">
        <asp:GridView CssClass="TotalizationRow" ID="GRVTotals" runat="server" AutoGenerateColumns="false"
            HeaderStyle-Wrap="false" RowStyle-Wrap="false" ShowHeader = "false">
        </asp:GridView>
    </div>
    
    <br />
    <asp:Repeater ID="rptPager" runat="server">
        <ItemTemplate>
            <asp:LinkButton ID="lnkPage" runat="server" Text = '<%#Eval("Text") %>' CommandArgument = '<%# Eval("Value") %>' Enabled = '<%# Eval("Enabled") %>' OnClick = "Page_Changed"></asp:LinkButton>
        </ItemTemplate>
    </asp:Repeater>
</div>