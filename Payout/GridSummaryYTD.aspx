<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="GridSummaryYTD.aspx.cs" Inherits="WebApplication3.GridSummaryYTD" EnableEventValidation="false" %>

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

            function gridviewScroll() {
            $('#<%= GridView1.ClientID %>').gridviewScroll({
                width: window.innerWidth - 50,
                height: window.innerHeight - 205,
                freezesize: 0,
                arrowsize: 30,
                varrowtopimg: "Content/arrowvt.png",
                varrowbottomimg: "Content/arrowvb.png",
                harrowleftimg: "Content/arrowhl.png",
                harrowrightimg: "Content/arrowhr.png"
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
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>                        
        </div>
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>

        <div id="addDiv">
            <div>
                <b>Program:</b> 
                <asp:DropDownList ID="progDDL" runat="server" AppendDataBoundItems="true" Width="180px">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <br />
                <b>Start Date:</b> <asp:TextBox ID="StartDateT" runat="server" CssClass="Date" placeholder="mm-dd-yyyy"></asp:TextBox>
                <br />
                <b>End Date:</b> <asp:TextBox ID="EndDateT" runat="server" CssClass="Date" placeholder="mm-dd-yyyy"></asp:TextBox>
                <br />
                <b>Store Name:</b> 
                <asp:DropDownList ID="storeDDL" runat="server" AppendDataBoundItems="true" Width="180px">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <br />
                <b>Store Number:</b> <asp:TextBox ID="ClubT" runat="server"></asp:TextBox>
                <br />
            </div>
            <%--<div>
                <b>City:</b> <asp:TextBox ID="CityT" runat="server"></asp:TextBox>
                <br />
                <b>State:</b> <asp:TextBox ID="StateT" runat="server"></asp:TextBox>
                <br />
                <b>Owner Firstname:</b> <asp:TextBox ID="OFN" runat="server"></asp:TextBox>
                <br />
                <b>Owner Lastname:</b> <asp:TextBox ID="OLN" runat="server"></asp:TextBox>
                <br />
                <b>Hub Firstname:</b> <asp:TextBox ID="HFN" runat="server"></asp:TextBox>
                <br />
                <b>Hub Lastname:</b> <asp:TextBox ID="HLN" runat="server"></asp:TextBox>
                <br />
                <input type="button" id="cancelAdd" value="Cancel" />
                &nbsp;&nbsp;
                <asp:Button ID="addBtn" runat="server" OnClick="addBtn_Click" Text="Add" />
            </div>--%>
        </div>

        <div id="notAdd">

        
        Date: from 
        <asp:TextBox ID="dateFrom" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="dateFrom_Filter" TargetControlID="dateFrom" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>
        to 
        <asp:TextBox ID="dateTo" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="dateTo_Filter" TargetControlID="dateTo" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender> &nbsp; 
        Owner: 
        <asp:DropDownList ID="ownerDDL" runat="server" AppendDataBoundItems="false" styles="width: 200px;">
        </asp:DropDownList> &nbsp;         
        Store: 
        <asp:DropDownList ID="sstoreDDL" runat="server" AppendDataBoundItems="false">            
        </asp:DropDownList> &nbsp; 
        Program:
        <asp:DropDownList ID="programDDL" runat="server" AppendDataBoundItems="false">
        </asp:DropDownList> &nbsp; 
        Location:        
        <asp:DropDownList ID="locationDDL" runat="server" AppendDataBoundItems="false" styles="width: 200px;">
        </asp:DropDownList> &nbsp; 
        Store #: 
        <asp:TextBox ID="StoreNumberTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="StoreNumberTXT_Filter" TargetControlID="StoreNumberTXT" runat="server" FilterMode="ValidChars" ValidChars="1234567890"></asp:FilteredTextBoxExtender> &nbsp; 
        

        <asp:Button ID="searchBTN" runat="server" OnClick="searchBTN_Click" Text="Search" />
        
        <br /><br />
        
        <asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server" HeaderStyle-Wrap="false" RowStyle-Wrap="false" AutoGenerateColumns="true"
                AllowPaging="True" PageSize="15" ShowFooter="true" OnPageIndexChanging="GridView1_PageIndexChanging1" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" OnRowDataBound="GridView1_RowDataBound">
                <PagerStyle CssClass="neoPager" />
                <EmptyDataTemplate>
                    No results found.
                </EmptyDataTemplate>
            </asp:GridView>   
            
            <asp:GridView CssClass="neoGrid" ID="GridView2" runat="server" HeaderStyle-Wrap="false" RowStyle-Wrap="false" AutoGenerateColumns="true" 
                AllowPaging="True" PageSize="1" ShowFooter="true">
                <EmptyDataTemplate>
                    No results found.
                </EmptyDataTemplate>
            </asp:GridView> 
         </ContentTemplate>
        </asp:UpdatePanel>
            
        </div>

    </div>
    </form>
</body>
</html>