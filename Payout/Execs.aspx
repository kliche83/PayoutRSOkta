<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="Execs.aspx.cs" Inherits="WebApplication3.Execs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Executives</title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#addDiv").hide();

            $("#add").click(function () {
                $(this).toggleClass("menuActive");
                if ($("#notAdd").offset().top == 50) {
                    $("#notAdd").animate({ top: 180 }, 300, function () {
                        $("#addDiv").fadeIn();
                    });
                }
                else {
                    $("#addDiv").hide();
                    $("#notAdd").animate({ top: 50 }, 300);
                }
            });

            $("#cancelAdd").click(function () {
                $("#add").removeClass("menuActive");
                $("#addDiv").hide();
                $("#notAdd").animate({ top: 50 }, 300);
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="menu">
            <%--<a id="add">Add Executive</a>--%>
            <asp:LinkButton ID="backBtn" OnClick="backBtn_Click" runat="server" Text="Back"></asp:LinkButton>
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>
        </div>

        <div id="addDiv">
            <br />
            <asp:DropDownList ID="execPerson" name="execPerson" runat="server" DataSourceID="SQLpeople" AppendDataBoundItems="true" DataTextField="Name" DataValueField="Id">
                <asp:ListItem></asp:ListItem>
            </asp:DropDownList> 
            &nbsp;
            is an executive of
            &nbsp
            <asp:DropDownList ID="execProgram" name="execProgram" runat="server" DataSourceID="SQLprograms" AppendDataBoundItems="true" DataTextField="Program" DataValueField="Program">
                <asp:ListItem></asp:ListItem>
            </asp:DropDownList> 
            <br />
            <br />
            <input type="button" id="cancelAdd" value="Cancel" />
            &nbsp;&nbsp;
            <asp:Button ID="addExec" runat="server" OnClick="addExec_Click" Text="Add" />
            &nbsp;&nbsp;
            <asp:Button ID="addBulkExec" runat="server" OnClick="addBulkExec_Click" Text="Bulk Add" style="background-color: #2764AB; color: #fff;" />
        </div>
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>
        
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="SELECT * FROM [PAYOUTexecs] JOIN [PAYOUTpeople] ON [Person] = [PAYOUTpeople].[Id] ORDER BY [Firstname], [Lastname]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SQLprogram" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="SELECT * FROM (SELECT TOP 1000 Program FROM PAYOUTschedule WHERE Program != '' AND Program IS NOT NULL GROUP BY Program ORDER BY Program)x">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SQLprograms" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="SELECT 'All' AS Program 
            UNION ALL 
            SELECT * 
            FROM 
            (
                SELECT DisplayProgram Program FROM PAYOUTOwnerPrograms UNION
                SELECT TOP 1000 Program FROM PAYOUTschedule WHERE Program != '' AND Program IS NOT NULL GROUP BY Program ORDER BY Program
            )x">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SQLpeople" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="SELECT [Id], ([Firstname] + ' ' + [Lastname]) AS [Name] FROM [PAYOUTpeople] GROUP BY [Id], [Firstname], [Lastname] ORDER BY [Firstname], [Lastname]">
        </asp:SqlDataSource>

        <br /><br /><br />

        <div id="notAdd">

        Name: 
        <asp:TextBox ID="nameTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        Program: 
        <asp:DropDownList ID="progDDLs" runat="server" DataSourceID="SQLprograms" DataValueField="Program" DataTextField="Program" AppendDataBoundItems="false">
        </asp:DropDownList> &nbsp; 

        <asp:Button ID="searchBTN" runat="server" OnClick="searchBTN_Click" Text="Search" />
        
        <br /><br />

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
<ContentTemplate>

        <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" AllowPaging="True" PageSize="15" 
            AllowSorting="False" HeaderStyle-Wrap="false" RowStyle-Wrap="false" DataKeyNames="Id" OnRowDataBound="GridView1_RowDataBound">
            <PagerStyle CssClass="neoPager" />
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="Id" />
                <%--<asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton ID="delLink" runat="server" Text="X" ForeColor="Red" OnClick="delLink_Click" Font-Underline="false"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:TemplateField HeaderText="Firstname" SortExpression="Firstname">
                    <ItemTemplate>
                        <asp:Label ID="Firstname" runat="server" Text='<%# Bind("[Firstname]") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Lastname" SortExpression="Lastname">
                    <ItemTemplate>
                        <asp:Label ID="Lastname" runat="server" Text='<%# Bind("[Lastname]") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="ExecOF" HeaderText="Executive of" SortExpression="ExecOF" ReadOnly="True" />
                <%--<asp:TemplateField HeaderText="Assign New Program" SortExpression="Program">
                    <ItemTemplate>
                        <asp:DropDownList ID="Program" runat="server" DataSourceID="SQLprogram" AppendDataBoundItems="true" DataTextField="Program" DataValueField="Program" OnSelectedIndexChanged="DropChanged" AutoPostBack="true" Width="200px" style="border: 0;">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>--%>
            </Columns>
        </asp:GridView>
    
        <asp:GridView CssClass="neoGrid" ID="GridView2" runat="server" AutoGenerateColumns="false" DataSourceID="SqlDataSource1" AllowPaging="false" PageSize="15" 
            AllowSorting="False" HeaderStyle-Wrap="false" RowStyle-Wrap="false" style="display: none;">
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="Firstname" HeaderText="Firstname" SortExpression="Firstname" ReadOnly="true" />
                <asp:BoundField DataField="Lastname" HeaderText="Lastname" SortExpression="Lastname" ReadOnly="true" />
                <asp:BoundField DataField="ExecOF" HeaderText="Executive of" SortExpression="ExecOF" ReadOnly="true" />
            </Columns>
        </asp:GridView>

 </ContentTemplate>
</asp:UpdatePanel>

            </div>

    </div>
    </form>
</body>
</html>
