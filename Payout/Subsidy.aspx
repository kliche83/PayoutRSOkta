<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Subsidy.aspx.cs" Inherits="Payout.Subsidy" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Extra Earnings</title>
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
                    $(".Date").datepicker();
                    $("#loadDiv").hide();
                    $("#loadGif").hide();
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

                    $(".showSpan").change(function (e) {
                        if ($("#<%= SSD.ClientID %>").val() != "SELECT") {
                            $(".hidSpan").show();
                        }
                        else {
                            $(".hidSpan").hide();
                        }
                    });
                });
            });

            $(function () {
                $(".Date").datepicker();
            });

            $(".showSpan").change(function (e) {
                if ($("#<%= SSD.ClientID %>").val() != "SELECT") {
                    $(".hidSpan").show();
                }
                else {
                    $(".hidSpan").hide();
                }
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
                    $("#notAdd").animate({ top: 400 }, 300, function () {
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
    <style type="text/css">
        .hidSpan { display: none; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="menu">
            <%--<a id="add">Add Earning</a>--%>
            <asp:LinkButton ID="popBtn" runat="server" Text="Update List" OnClick="popBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>

            
        </div>

        <div id="loadDiv" runat="server" style="position: absolute; top: 0; bottom: 0; left: 0; right: 0; background-color: rgbA(255, 255, 255, 0.8); z-index: 9999; overflow: hidden; text-align: center; padding: 120px 10px 10px 10px;">
            <img id="loadGif" runat="server" src="../Content/load.gif" />
        </div>
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>
        
        <asp:SqlDataSource ID="owSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="storeSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="programSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="weSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>

        <div id="addDiv">

            <div>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
 <ContentTemplate>
                <%--<b>Week Ending:</b> <asp:TextBox ID="SSD" runat="server" width="150px" CssClass="Date showSpan" OnTextChanged="DateChanged" AutoPostBack="true"></asp:TextBox>--%>
                <b>Week Ending:</b> 
                <asp:DropDownList ID="SSD" runat="server" style="width: 150px;" CssClass="showSpan" OnDropChanged="DateChanged" AutoPostBack="true" DataSourceID="weSQL" DataValueField="StartDate" DataTextField="WeekEnding" AppendDataBoundItems="false">
                </asp:DropDownList>
                <br />
                <%--<b>End Date:</b> <asp:TextBox ID="SED" runat="server" width="150px" CssClass="Date showSpan" OnTextChanged="DateChanged" AutoPostBack="true"></asp:TextBox>
                <br />--%>
                <span id="hidSpan" runat="server">
                    <b>Owner:</b> 
                    <asp:DropDownList ID="owDDL" runat="server" Width="162px" DataSourceID="owSQL" DataValueField="OwnerName" DataTextField="OwnerName">
                    </asp:DropDownList>&nbsp;
                </span>
 </ContentTemplate>
</asp:UpdatePanel>
                <span class="hidSpan">
                    <br />
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
                    <b>Store Number:</b> <asp:TextBox ID="SNO" runat="server" width="150px"></asp:TextBox>
                    <br />
                    <b>Program:</b> 
                    <asp:DropDownList ID="progDDL" runat="server" DataSourceID="programSQL" DataValueField="Program" DataTextField="Program" AppendDataBoundItems="true" Width="162px">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>&nbsp;
                </span>
            </div>
            <div>
                <span class="hidSpan">
                    <b>Earning Type:</b> 
                    <asp:DropDownList ID="typeDDL" runat="server" Width="162px">
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem Value="ExtraCommission">Extra Commission</asp:ListItem>
                        <asp:ListItem Value="RoadShow">Road Trip</asp:ListItem>
                        <asp:ListItem Value="Subsidy">Subsidy</asp:ListItem>
                      
                    </asp:DropDownList>
                    <br />
                    <b>Amount:</b> <asp:TextBox ID="AMN" runat="server" width="150px"></asp:TextBox>
                    <br />
                    <b>Comments:</b> <asp:TextBox ID="CMN" runat="server" width="150px"></asp:TextBox>
                    <br />
                    <b>Start Date:</b> <asp:TextBox ID="STD" runat="server" CssClass="Date" width="150px"></asp:TextBox>
                    <br />
                    <b>End Date:</b> <asp:TextBox ID="END" runat="server" CssClass="Date" width="150px"></asp:TextBox>
                    <br />
                    <b>Approved By:</b> <asp:TextBox ID="APB" runat="server" width="150px"></asp:TextBox>
                    <br />
                    <b>Approved On:</b> <asp:TextBox ID="APO" runat="server" CssClass="Date" width="150px"></asp:TextBox>
                    <br />
                    <asp:Button ID="addBtn" runat="server" OnClick="addBtn_Click" Text="Add" />
                    &nbsp;&nbsp;
                </span>
                    <input type="button" id="cancelAdd" value="Cancel" />
            </div>

        </div>
        
        <div id="notAdd">

<asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Always">
<ContentTemplate>
            
        Owner: 
        <asp:TextBox ID="ownerTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        Store: 
        <asp:DropDownList ID="storeDDLs" runat="server">
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
        Store Number: 
        <asp:TextBox ID="StoreNumberTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="StoreNumberTXT_Filter" TargetControlID="StoreNumberTXT" runat="server" FilterMode="ValidChars" ValidChars="1234567890"></asp:FilteredTextBoxExtender> &nbsp; 
        Type: 
        <asp:DropDownList ID="typeDDLs" runat="server" Width="100px">
            <asp:ListItem></asp:ListItem>
            <asp:ListItem Value="ExtraCommission">Extra Commission</asp:ListItem>
            <asp:ListItem Value="RoadShow">Road Trip</asp:ListItem>
            <asp:ListItem Value="Subsidy">Subsidy</asp:ListItem>
             
        </asp:DropDownList>&nbsp; 
        Start Date: 
        <asp:TextBox ID="weDate" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="weDate_Filter" TargetControlID="weDate" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender> &nbsp; 

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
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton ID="addLink" runat="server" Text="Add Another Type" OnClick="addLink_Click" Font-Underline="false"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Owner" HeaderText="Owner" SortExpression="Owner" ReadOnly="True" />
                <asp:BoundField DataField="StoreName" HeaderText="Store Name" SortExpression="StoreName" ReadOnly="True" />
                <asp:BoundField DataField="StoreNumber" HeaderText="Store Number" SortExpression="StoreNumber" ReadOnly="True" />
                <asp:BoundField DataField="Program" HeaderText="Program" SortExpression="Program" ReadOnly="True" />
                <%--<asp:TemplateField HeaderText="Store Name" SortExpression="StoreName">
                    <ItemTemplate>
                        <asp:DropDownList ID="StoreName" runat="server" SelectedValue='<%# Bind("[StoreName]") %>' DataSourceID="storeSQL" DataValueField="StoreName" DataTextField="StoreName" AutoPostBack="true" OnSelectedIndexChanged="DropChanged" Width="150px" style="border: 0;">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Store Number" SortExpression="StoreNumber">
                    <ItemTemplate>
                        <asp:TextBox ID="StoreNumber" runat="server" Text='<%# Bind("[StoreNumber]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Program" SortExpression="Program">
                    <ItemTemplate>
                        <asp:DropDownList ID="Program" runat="server" SelectedValue='<%# Bind("[Program]") %>' DataSourceID="programSQL" DataValueField="Program" DataTextField="Program" AutoPostBack="true" OnSelectedIndexChanged="DropChanged" Width="150px" style="border: 0;">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:TemplateField HeaderText="Type" SortExpression="Type" ItemStyle-Width="150px">
                    <ItemTemplate>
                        <asp:DropDownList ID="Type" runat="server" SelectedValue='<%# Bind("[Type]") %>' AutoPostBack="true" OnSelectedIndexChanged="DropChanged" Width="150px" style="border: 0;">
                            <asp:ListItem Value="n/a" Text=""></asp:ListItem>
                            <asp:ListItem Value="ExtraCommission">Extra Commission</asp:ListItem>
                            <asp:ListItem Value="RoadShow">Road Trip</asp:ListItem>
                            <asp:ListItem Value="Subsidy">Subsidy</asp:ListItem>
                             
                        </asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Amount" SortExpression="Amount">
                    <ItemTemplate>
                        <asp:TextBox ID="Amount" runat="server" Text='<%# Bind("[Amount]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Comments" SortExpression="Comments">
                    <ItemTemplate>
                        <asp:TextBox ID="Comments" runat="server" Text='<%# Bind("[Comments]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="StartDate" HeaderText="Start Date" SortExpression="StartDate" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="EndDate" HeaderText="End Date" SortExpression="EndDate" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <%--<asp:TemplateField HeaderText="Start Date" SortExpression="StartDate">
                    <ItemTemplate>
                        <asp:TextBox ID="StartDate" runat="server" Text='<%# Bind("[StartDate]", "{0:MM/dd/yyyy}") %>' AutoPostBack="true" OnTextChanged="FieldChanged" CssClass="Date"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="End Date" SortExpression="EndDate">
                    <ItemTemplate>
                        <asp:TextBox ID="EndDate" runat="server" Text='<%# Bind("[EndDate]", "{0:MM/dd/yyyy}") %>' AutoPostBack="true" OnTextChanged="FieldChanged" CssClass="Date"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:TemplateField HeaderText="Approved On" SortExpression="ApprovedOn">
                    <ItemTemplate>
                        <asp:TextBox ID="ApprovedOn" runat="server" Text='<%# Bind("[ApprovedOn]", "{0:MM/dd/yyyy}") %>' AutoPostBack="true" OnTextChanged="FieldChanged" CssClass="Date"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Approved By" SortExpression="ApprovedBy">
                    <ItemTemplate>
                        <asp:TextBox ID="ApprovedBy" runat="server" Text='<%# Bind("[ApprovedBy]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    
        <asp:GridView CssClass="neoGrid" ID="GridView2" runat="server" AutoGenerateColumns="false" DataSourceID="SqlDataSource1" AllowPaging="true" PageSize="1" 
            AllowSorting="False" HeaderStyle-Wrap="false" RowStyle-Wrap="false" style="display: none;">
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="Owner" HeaderText="Owner" SortExpression="Owner" ReadOnly="true" />
                <asp:BoundField DataField="StoreName" HeaderText="Store Name" SortExpression="StoreName" ReadOnly="true" />
                <asp:BoundField DataField="StoreNumber" HeaderText="Store Number" SortExpression="StoreNumber" ReadOnly="true" />
                <asp:BoundField DataField="Program" HeaderText="Program" SortExpression="Program" ReadOnly="true" />
                <asp:TemplateField HeaderText="Type" SortExpression="Type">
                    <ItemTemplate><%# (Eval("Type").ToString() == "n/a") ? "" : Eval("Type").ToString() %></ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" ReadOnly="true" />
                <asp:BoundField DataField="Comments" HeaderText="Comments" SortExpression="Comments" ReadOnly="true" />
                <asp:BoundField DataField="StartDate" HeaderText="Start Date" SortExpression="StartDate" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="EndDate" HeaderText="End Date" SortExpression="EndDate" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="ApprovedOn" HeaderText="Approved On" SortExpression="ApprovedOn" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="ApprovedBy" HeaderText="Approved By" SortExpression="ApprovedBy" ReadOnly="true" />
            </Columns>
        </asp:GridView>

 </ContentTemplate>
</asp:UpdatePanel>

        </div>

    </div>
    </form>
</body>
</html>