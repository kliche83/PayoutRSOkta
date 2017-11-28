<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Codes.aspx.cs" Inherits="Payout.Codes" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Short Codes</title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#addDiv").hide();

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

                    $("input[type=text]").click(function (e) {
                        $(this).select();
                    });

                    $("input[type=text]").focus(function (e) {
                        $(this).select();
                    });
                });
            });

            $("#<%= missing.ClientID %>").click(function (e) {
                $(this).fadeOut();
            });

            $("input[type=text]").click(function (e) {
                $(this).select();
            });

            $("input[type=text]").focus(function (e) {
                $(this).select();
            });
            
            $("#add").click(function () {
                $(this).toggleClass("menuActive");
                if ($("#notAdd").offset().top == 50) {
                    $("#notAdd").animate({ top: 240 }, 300, function () {
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

            function bindKeyDown() {
                $('input').keydown(function (e) {
                    //            if(e.which==39)
                    //            $(this).closest('td').next().find('input').focus();
                    //            else if(e.which==37)
                    //            $(this).closest('td').prev().find('input').focus();
                    if (e.which == 13)
                        $(this).closest('tr').next().find('td:eq(' + $(this).closest('td').index() + ')').find('input').focus();
                    else if (e.which == 40)
                        $(this).closest('tr').next().find('td:eq(' + $(this).closest('td').index() + ')').find('input').focus();
                    else if (e.which == 38)
                        $(this).closest('tr').prev().find('td:eq(' + $(this).closest('td').index() + ')').find('input').focus();
                });
            }
            bindKeyDown();
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="menu">
            <%--<a id="add">Add Code</a>--%>
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>

            
        </div>

        <asp:SqlDataSource ID="SQLprogram" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="SELECT * FROM (SELECT [Program] FROM [PAYOUTitemMaster] UNION All SELECT [Program] FROM [PAYOUTschedule])X WHERE Program != '' AND Program IS NOT NULL group by [Program] ORDER BY [Program]">
        </asp:SqlDataSource>

        <%--<div id="addDiv">
            <div>
                <b>Store Name:</b> 
                <asp:DropDownList ID="storeDDL" runat="server" Width="162px">
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem Value="BJs">BJ's</asp:ListItem>
                    <asp:ListItem Value="CanadianTire">Canadian Tire</asp:ListItem>
                    <asp:ListItem Value="Costco">Costco</asp:ListItem>
                    <asp:ListItem Value="HEB">HEB</asp:ListItem>
                    <asp:ListItem Value="Kroger">Kroger</asp:ListItem>
                    <asp:ListItem Value="Meijer">Meijer</asp:ListItem>
                    <asp:ListItem Value="Sams">Sam's</asp:ListItem>
                    <asp:ListItem Value="WalMart">WalMart</asp:ListItem>
                    <asp:ListItem Value="WinnDixie">WinnDixie</asp:ListItem>
                </asp:DropDownList>
                <br />
                <b>Item Number:</b> <asp:TextBox ID="INO" runat="server" width="150px"></asp:TextBox>
                <br />
                <b>Item Name:</b> <asp:TextBox ID="INA" runat="server" width="150px"></asp:TextBox>
                <br />
                <b>Item Name 2:</b> <asp:TextBox ID="INA2" runat="server" width="150px"></asp:TextBox>&nbsp;
            </div>
            <div>
                <b>UPC:</b> <asp:TextBox ID="UPC" runat="server" width="150px"></asp:TextBox>
                <br />
                <b>Unit Cost:</b> <asp:TextBox ID="IUC" runat="server" width="150px"></asp:TextBox>
                <br />
                <b>Program:</b> 
                <asp:DropDownList ID="progDDL" runat="server" DataSourceID="SQLprogram" AppendDataBoundItems="true" DataTextField="Program" DataValueField="Program" Width="162px">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <br />
                <input type="button" id="cancelAdd" value="Cancel" />
                &nbsp;&nbsp;
                <asp:Button ID="addBtn" runat="server" OnClick="addBtn_Click" Text="Add" />
            </div>
        </div>--%>

        <div id="missing" runat="server">
            <asp:Label runat="server" ID="missingL" Text="Missing Owners from Email List"></asp:Label>
        </div>
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>
        
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>
        
        <div id="notAdd">

        Program Name: 
        <asp:TextBox ID="ProgNameTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp;  
        Program Code: 
        <asp:TextBox ID="ProgCodeTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 

        <asp:Button ID="searchBTN" runat="server" OnClick="searchBTN_Click" Text="Search" />
        
        <br /><br />

<asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Always">
<ContentTemplate>
        <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server" AutoGenerateColumns="false" DataSourceID="SqlDataSource1" AllowPaging="True" PageSize="20" 
            AllowSorting="False" HeaderStyle-Wrap="false" RowStyle-Wrap="false" OnRowDataBound="GridView1_RowDataBound">
            <PagerStyle CssClass="neoPager" />
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="Id" />
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton ID="delLink0" runat="server" Text="X" ForeColor="Red" OnClick="delLink_Click" Font-Underline="false"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Name" HeaderText="Program Name" SortExpression="Name" ReadOnly="True" />
                <asp:TemplateField HeaderText="Short Code" SortExpression="Code">
                    <ItemTemplate>
                        <asp:TextBox ID="Code" runat="server" Text='<%# Bind("[Code]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    
        <asp:GridView CssClass="neoGrid" ID="GridView2" runat="server" AutoGenerateColumns="false" DataSourceID="SqlDataSource1" AllowPaging="False" PageSize="15" 
            AllowSorting="False" HeaderStyle-Wrap="false" RowStyle-Wrap="false" style="display: none;">
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Program Name" SortExpression="Name" ReadOnly="True" />
                <asp:BoundField DataField="Code" HeaderText="Short Code" SortExpression="Code" ReadOnly="True" />
            </Columns>
        </asp:GridView>

 </ContentTemplate>
</asp:UpdatePanel>

            </div>

    </div>
    </form>
</body>
</html>
