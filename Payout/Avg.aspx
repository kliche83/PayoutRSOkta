<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="Avg.aspx.cs" Inherits="Payout.Avg" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Averages</title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script src="//code.jquery.com/ui/1.11.2/jquery-ui.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#addDiv").hide();
            $("#loadDiv").hide();
            $("#loadGif").hide();

            jQuery(function ($) {
                var focusedElementId = "";
                var prm = Sys.WebForms.PageRequestManager.getInstance();
                prm.add_beginRequest(function (source, args) {
                    $("#loadDiv").show();
                    $("#loadGif").show();
                    var fe = document.activeElement;
                    if (fe != null) {
                        focusedElementId = fe.id;
                    } else {
                        focusedElementId = "";
                    }
                });
                prm.add_endRequest(function (source, args) {
                    $("#loadDiv").hide();
                    $("#loadGif").hide();
                    $(".Date").datepicker();
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

            $(function () {
                $(".Date").datepicker();
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
                    $("#notAdd").animate({ top: 280 }, 300, function () {
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

        function closeWeek() {
            var closeWeek_confirm_value = document.createElement("INPUT");
            closeWeek_confirm_value.type = "hidden";
            closeWeek_confirm_value.name = "closeWeek_confirm_value";
            if (confirm("Do you want to add the next rotation?")) {
                closeWeek_confirm_value.value = "Yes";
            } else {
                closeWeek_confirm_value.value = "No";
            }
            document.forms[0].appendChild(closeWeek_confirm_value);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="menu">
            <%--<a id="add">Add Avg Limit</a>--%>
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>

            <asp:LinkButton ID="weekBtn" runat="server" OnClick="weekBtn_Click" OnClientClick="closeWeek()" Text="Add New Rotation"></asp:LinkButton>

            
        </div>

        <div id="loadDiv" runat="server" style="position: absolute; top: 0; bottom: 0; left: 0; right: 0; background-color: rgbA(255, 255, 255, 0.8); z-index: 9999; overflow: hidden; text-align: center; padding: 120px 10px 10px 10px;">
            <img id="loadGif" runat="server" src="../Content/load.gif" />
        </div>

        <asp:SqlDataSource ID="SQLprogram" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="SELECT [Program] FROM [PAYOUTschedule] WHERE [Program] != '' GROUP BY [Program] ORDER BY [Program]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SQLprograms" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="SELECT 'All' AS Program UNION ALL SELECT * FROM (SELECT TOP 1000 Program FROM PAYOUTschedule WHERE Program != '' AND Program IS NOT NULL GROUP BY Program ORDER BY Program)x">
        </asp:SqlDataSource>

        <div id="addDiv">
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
                <b>Program:</b> 
                <asp:DropDownList ID="progDDL" runat="server" DataSourceID="SQLprogram" AppendDataBoundItems="true" DataTextField="Program" DataValueField="Program" Width="162px">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <br />
            </div>
            <div>
                <b>Limit</b> <asp:TextBox ID="RTES" runat="server" width="150px"></asp:TextBox>
                <br />
                <b>Week Ending:</b> <asp:TextBox ID="EFFD" runat="server" width="150px" CssClass="Date"></asp:TextBox>
                <br />
                <br />
                <input type="button" id="cancelAdd" value="Cancel" />
                &nbsp;&nbsp;
                <asp:Button ID="addBtn" runat="server" OnClick="addBtn_Click" Text="Add" />
            </div>
        </div>
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>
        
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>
       
<asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Always">
<ContentTemplate> 

        <div id="notAdd">

        Store: 
        <asp:DropDownList ID="storeDDLs" runat="server" style="width: 100px;">
            <asp:ListItem Value="%" Text="All" Selected="True"></asp:ListItem>
            <asp:ListItem Value="BJs">BJ's</asp:ListItem>
            <asp:ListItem Value="CanadianTire">Canadian Tire</asp:ListItem>
            <asp:ListItem Value="Costco">Costco</asp:ListItem>
            <asp:ListItem Value="HEB">HEB</asp:ListItem>
            <asp:ListItem Value="Kroger">Kroger</asp:ListItem>
            <asp:ListItem Value="Meijer">Meijer</asp:ListItem>
            <asp:ListItem Value="Sams">Sam's</asp:ListItem>
            <asp:ListItem Value="WalMart">WalMart</asp:ListItem>
            <asp:ListItem Value="WinnDixie">WinnDixie</asp:ListItem>
        </asp:DropDownList> &nbsp; 
        Program: 
        <asp:DropDownList ID="progDDLs" runat="server" DataSourceID="SQLprograms" DataValueField="Program" DataTextField="Program" AppendDataBoundItems="false">
        </asp:DropDownList> &nbsp; 
        Start Date >= 
        <asp:TextBox ID="dateFrom" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="dateFrom_Filter" TargetControlID="dateFrom" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>
        End Date <=
        <asp:TextBox ID="dateTo" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="dateTo_Filter" TargetControlID="dateTo" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>

        <asp:Button ID="searchBTN" runat="server" OnClick="searchBTN_Click" Text="Search" />
        
        <br /><br />

        <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server" AutoGenerateColumns="false" DataSourceID="SqlDataSource1" AllowPaging="True" PageSize="15" 
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
                <asp:BoundField DataField="StoreName" HeaderText="Store Name" SortExpression="StoreName" ReadOnly="True" />
                <asp:BoundField DataField="Program" HeaderText="Program" SortExpression="Program" ReadOnly="True" />
                <asp:TemplateField HeaderText="Average Limit" SortExpression="Limit">
                    <ItemTemplate>
                        <asp:TextBox ID="Limit" runat="server" Text='<%# Bind("[Limit]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Start Date" SortExpression="StartDate">
                    <ItemTemplate>
                        <asp:TextBox ID="StartDate" runat="server" Text='<%# Bind("[StartDate]", "{0:MM/dd/yyyy}") %>' AutoPostBack="true" OnTextChanged="FieldChanged" CssClass="Date"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="End Date" SortExpression="EndDate">
                    <ItemTemplate>
                        <asp:TextBox ID="EndDate" runat="server" Text='<%# Bind("[EndDate]", "{0:MM/dd/yyyy}") %>' AutoPostBack="true" OnTextChanged="FieldChanged" CssClass="Date"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    
        <asp:GridView CssClass="neoGrid" ID="GridView2" runat="server" AutoGenerateColumns="false" DataSourceID="SqlDataSource1" AllowPaging="false" PageSize="15" 
            AllowSorting="False" HeaderStyle-Wrap="false" RowStyle-Wrap="false" style="display: none;">
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="StoreName" HeaderText="Store Name" SortExpression="StoreName" ReadOnly="true" />
                <asp:BoundField DataField="Program" HeaderText="Program" SortExpression="Program" ReadOnly="true" />
                <asp:BoundField DataField="Limit" HeaderText="Average Limit" SortExpression="Limit" ReadOnly="true" />
                <asp:BoundField DataField="StartDate" HeaderText="Start Date" SortExpression="StartDate" ReadOnly="True" dataformatstring="{0:MM/dd/yy}" />
                <asp:BoundField DataField="EndDate" HeaderText="End Date" SortExpression="EndDate" ReadOnly="True" dataformatstring="{0:MM/dd/yy}" />
            </Columns>
        </asp:GridView>

 </ContentTemplate>
</asp:UpdatePanel>

            </div>

    </div>
    </form>
</body>
</html>