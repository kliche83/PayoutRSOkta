<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="Schedule.aspx.cs" Inherits="WebApplication3.Schedule" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Schedule</title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script src="//code.jquery.com/ui/1.11.2/jquery-ui.js"></script>
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
                    $("#notAdd").animate({ top: 350 }, 300, function () {
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
    <style>
        tr:not(:last-child) td:nth-child(5), 
        tr:not(:last-child) td:nth-child(7) {
            width: 150px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="menu">
            <a id="add" runat="server">Add</a>
            <asp:LinkButton ID="OverlapsReport" runat="server" Text="Overlaps Report" PostBackUrl="~/ScheduleOverlap.aspx" ></asp:LinkButton>
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>
            
        </div>
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>

        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SQLprogram" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="SELECT [Program] FROM [PAYOUTschedule] WHERE [Program] != '' GROUP BY [Program] ORDER BY [Program]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sSQLprogram" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="SELECT 'All' AS Program UNION ALL SELECT * FROM (SELECT TOP 1000 Program FROM PAYOUTschedule WHERE Program != '' AND Program IS NOT NULL GROUP BY Program ORDER BY Program)x">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="storeSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
            SelectCommand="SELECT StoreName FROM PAYOUTschedule WHERE StoreName != '' AND StoreName IS NOT NULL GROUP BY StoreName ORDER BY StoreName">
        </asp:SqlDataSource>

        <div id="addDiv">
            <div>
                <b>Program:</b> 
                <asp:DropDownList ID="progDDL" runat="server" DataSourceID="SQLprogram" AppendDataBoundItems="true" DataTextField="Program" DataValueField="Program" Width="180px">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <br />
                <b>Start Date:</b> <asp:TextBox ID="StartDateT" runat="server" CssClass="Date" placeholder="mm-dd-yyyy"></asp:TextBox>
                <br />
                <b>End Date:</b> <asp:TextBox ID="EndDateT" runat="server" CssClass="Date" placeholder="mm-dd-yyyy"></asp:TextBox>
                <br />
                <b>Store Name:</b> 
                <asp:DropDownList ID="storeDDL" runat="server" DataSourceID="storeSQL" AppendDataBoundItems="true" DataTextField="StoreName" DataValueField="StoreName" Width="180px">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <br />
                <b>Store Number:</b> <asp:TextBox ID="ClubT" runat="server"></asp:TextBox>
                <br />
                <b>City:</b> <asp:TextBox ID="CityT" runat="server"></asp:TextBox>
                <br />
                <b>State:</b> <asp:TextBox ID="StateT" runat="server"></asp:TextBox>
                <br />
            </div>
            <div>                
                <%--<b>Country:</b> <asp:TextBox ID="CountryT" runat="server"></asp:TextBox>
                <br />--%>
                <b>Owner Firstname:</b> <asp:TextBox ID="OFN" runat="server"></asp:TextBox>
                <br />
                <b>Owner Lastname:</b> <asp:TextBox ID="OLN" runat="server"></asp:TextBox>
                <br />
                <b>Hub Firstname:</b> <asp:TextBox ID="HFN" runat="server"></asp:TextBox>
                <br />
                <b>Hub Lastname:</b> <asp:TextBox ID="HLN" runat="server"></asp:TextBox>
                <br />
                <b>Event Id:</b> <asp:TextBox ID="EventIdT" runat="server"></asp:TextBox>
                <br />
                <input type="button" id="cancelAdd" value="Cancel" />
                &nbsp;&nbsp;
                <asp:Button ID="addBtn" runat="server" OnClick="addBtn_Click" Text="Add" />
            </div>
        </div>

        <div id="notAdd">

        
        Owner: 
        <asp:TextBox ID="ownerTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        Store: 
        <asp:DropDownList ID="sstoreDDL" runat="server">
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
        <asp:DropDownList ID="programDDL" runat="server" DataSourceID="sSQLprogram" DataValueField="Program" DataTextField="Program" AppendDataBoundItems="false">
        </asp:DropDownList> &nbsp; 
        Store Number: 
        <asp:TextBox ID="StoreNumberTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="StoreNumberTXT_Filter" TargetControlID="StoreNumberTXT" runat="server" FilterMode="ValidChars" ValidChars="1234567890"></asp:FilteredTextBoxExtender> &nbsp; 
        Date: from 
        <asp:TextBox ID="dateFrom" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="dateFrom_Filter" TargetControlID="dateFrom" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>
        to 
        <asp:TextBox ID="dateTo" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="dateTo_Filter" TargetControlID="dateTo" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender> &nbsp; 

        <asp:Button ID="searchBTN" runat="server" OnClick="searchBTN_Click" Text="Search" />
        
        <br /><br />
        
<asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Always">
<ContentTemplate>

        <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" AllowPaging="True" PageSize="15" 
            AllowSorting="True" HeaderStyle-Wrap="false" RowStyle-Wrap="false" DataKeyNames="Id" OnRowDataBound="GridView1_RowDataBound">
            <PagerStyle CssClass="neoPager" />
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="Id" />
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton ID="delLink" runat="server" Text="X" ForeColor="Red" OnClick="delLink_Click" Font-Underline="false"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Program" HeaderText="Program" SortExpression="Program" ReadOnly="True" />
                <asp:TemplateField HeaderText="Start Date" SortExpression="StartDate">
                    <ItemTemplate>
                        <asp:TextBox ID="StartDate" runat="server" Text='<%# Bind("[StartDate]", "{0:MM/dd/yyyy}") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="End Date" SortExpression="EndDate">
                    <ItemTemplate>
                        <asp:TextBox ID="EndDate" runat="server" Text='<%# Bind("[EndDate]", "{0:MM/dd/yyyy}") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Store Name" SortExpression="StoreName">
                    <ItemTemplate>
                        <asp:TextBox ID="StoreName" runat="server" Text='<%# Bind("[StoreName]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Store Number" SortExpression="StoreNumber">
                    <ItemTemplate>
                        <asp:TextBox ID="StoreNumber" runat="server" Text='<%# Bind("[StoreNumber]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="City" SortExpression="City">
                    <ItemTemplate>
                        <asp:TextBox ID="City" runat="server" Text='<%# Bind("[City]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="State" SortExpression="State">
                    <ItemTemplate>
                        <asp:TextBox ID="State" runat="server" Text='<%# Bind("[State]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:TemplateField HeaderText="Country" SortExpression="Country">
                    <ItemTemplate>
                        <asp:TextBox ID="Country" runat="server" Text='<%# Bind("[Country]") %>' AutoPostBack="true" OnTextChanged="FieldChanged" Width="50px"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:TemplateField HeaderText="Owner Firstname" SortExpression="OwnerFirstname">
                    <ItemTemplate>
                        <asp:TextBox ID="OwnerFirstname" runat="server" Text='<%# Bind("[OwnerFirstname]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Owner Lastname" SortExpression="OwnerLastname">
                    <ItemTemplate>
                        <asp:TextBox ID="OwnerLastname" runat="server" Text='<%# Bind("[OwnerLastname]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Hub Firstname" SortExpression="HubFirstname">
                    <ItemTemplate>
                        <asp:TextBox ID="HubFirstname" runat="server" Text='<%# Bind("[HubFirstname]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Hub Lastname" SortExpression="HubLastname">
                    <ItemTemplate>
                        <asp:TextBox ID="HubLastname" runat="server" Text='<%# Bind("[HubLastname]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="ImportedBy" HeaderText="Imported By" SortExpression="ImportedBy" ReadOnly="True" />
                <asp:BoundField DataField="ImportedOn" HeaderText="Imported On" SortExpression="ImportedOn" ReadOnly="True" dataformatstring="{0:MM/dd/yyyy}" htmlencode="false" />
                <asp:BoundField DataField="EventId" HeaderText="Event Id" SortExpression="EventId" ReadOnly="True" />

                <%--<asp:TemplateField HeaderText="Event Id" SortExpression="EventId">
                    <ItemTemplate>
                        <asp:TextBox ID="EventId" runat="server" Text='<%# Bind("[EventId]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>--%>
            </Columns>
        </asp:GridView>
    
        <asp:GridView CssClass="neoGrid" ID="GridView2" runat="server" AutoGenerateColumns="false" DataSourceID="SqlDataSource1" AllowPaging="true" PageSize="1" 
            AllowSorting="False" HeaderStyle-Wrap="false" RowStyle-Wrap="false" style="display: none;">
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="Program" HeaderText="Program" SortExpression="Program" ReadOnly="true" />
                <asp:BoundField DataField="StartDate" HeaderText="Start Date" SortExpression="StartDate" ReadOnly="true" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="EndDate" HeaderText="End Date" SortExpression="EndDate" ReadOnly="true" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="StoreName" HeaderText="Store Name" SortExpression="StoreName" ReadOnly="true" />
                <asp:BoundField DataField="StoreNumber" HeaderText="Store Number" SortExpression="StoreNumber" ReadOnly="true" />
                <asp:BoundField DataField="City" HeaderText="City" SortExpression="City" ReadOnly="true" />
                <asp:BoundField DataField="State" HeaderText="State" SortExpression="State" ReadOnly="true" />
                <asp:BoundField DataField="OwnerFirstname" HeaderText="Owner Firstname" SortExpression="OwnerFirstname" ReadOnly="true" />
                <asp:BoundField DataField="OwnerLastname" HeaderText="Owner Lastname" SortExpression="OwnerLastname" ReadOnly="true" />
                <asp:BoundField DataField="HubFirstname" HeaderText="Hub Firstname" SortExpression="HubFirstname" ReadOnly="true" />
                <asp:BoundField DataField="HubLastname" HeaderText="Hub Lastname" SortExpression="HubLastname" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="ImportedBy" HeaderText="ImportedBy" SortExpression="ImportedBy" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="ImportedOn" HeaderText="Imported On" SortExpression="ImportedOn" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="EventId" HeaderText="Event Id" SortExpression="EventId" ReadOnly="True" />
            </Columns>
        </asp:GridView>

 </ContentTemplate>
</asp:UpdatePanel>
            
        </div>

    </div>
    </form>
</body>
</html>