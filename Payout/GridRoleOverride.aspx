<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="GridRoleOverride.aspx.cs" Inherits="WebApplication3.GridRoleOverride" EnableEventValidation="false" %>

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

            firstload();

            $(".TopButtons").find(".TabButtons").click(function () {
                if ($(this)[0].id == "TabCommissionBtn") {
                    $(location).attr('href', '/GridRoleCommissionDaily.aspx')
                }
            });

            function firstload() {
                $('#TabCommissionBtn').addClass('Inactive').removeClass('Active');
                $('#TabOverrideBtn').addClass('Active').removeClass('Inactive');
            }


            $("#addDiv").hide();

            //$("#BackBtn").click(function () {
            //    $("#loadDiv").show();
            //    $("#loadGif").show();
            //});

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
            <asp:LinkButton ID="BackBtn" runat="server" Text="Back" PostBackUrl="~/PMCumulativeSummary.aspx" ></asp:LinkButton>
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>                        
        </div>
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>

        <%--<div id="addDiv">
            <div>                
                <b>Start Date:</b> <asp:TextBox ID="StartDateT" runat="server" CssClass="Date" placeholder="mm-dd-yyyy"></asp:TextBox>
                <br />
                <b>End Date:</b> <asp:TextBox ID="EndDateT" runat="server" CssClass="Date" placeholder="mm-dd-yyyy"></asp:TextBox>
                <br />
                <b>Store Name:</b> 
                <asp:DropDownList ID="storeDDL" runat="server" AppendDataBoundItems="true" Width="180px">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <br />
                <b>Program:</b> 
                <asp:DropDownList ID="progDDL" runat="server" AppendDataBoundItems="true" Width="180px">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <br />
                <b>Trainer:</b> 
                <asp:DropDownList ID="TrainDDL" runat="server" AppendDataBoundItems="true" Width="180px">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <br />
                <br />
            </div>
        </div>--%>

        <div id="notAdd">
        
            <ul class="TopButtons">
                <li>
                    <div id="containerCommission" class="WrapperDivs">
                        <div id="TabCommissionBtn" class="TabButtons">Detailed Commission</div>
                        <div class="whiteBox"></div>
                    </div>
                    
                </li>
                <li>
                    <div id="containerOverride" class="WrapperDivs">
                        <div id="TabOverrideBtn" class="TabButtons">Detailed Override</div>
                        <div class="whiteBox"></div>         
                    </div>
                </li>
            </ul>
            
            <div id="TableWrapper">
                <asp:Panel ID="CommissionPanel" CssClass="tablesWrapper" runat="server">

                    Date: from 
                    <asp:TextBox ID="dateFrom" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
                    <asp:FilteredTextBoxExtender ID="dateFrom_Filter" TargetControlID="dateFrom" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>
                    to 
                    <asp:TextBox ID="dateTo" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
                    <asp:FilteredTextBoxExtender ID="dateTo_Filter" TargetControlID="dateTo" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender> &nbsp;             
                    Program:
                    <asp:DropDownList ID="programDDL" runat="server" AppendDataBoundItems="false">
                    </asp:DropDownList> &nbsp; 
                    Store Name:
                    <asp:DropDownList ID="StoreNameDDL" runat="server" AppendDataBoundItems="false">
                    </asp:DropDownList> &nbsp; 
                    Trainer:
                    <asp:DropDownList ID="TrainerDDL" runat="server" AppendDataBoundItems="false">
                    </asp:DropDownList> &nbsp; 


                    <asp:Button ID="searchBTN" runat="server" OnClick="searchBTN_Click" Text="Search" />
        
                    <br /><br />
        
                    <asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                                <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server" AutoGenerateColumns="true" 
                                        HeaderStyle-Wrap="false" RowStyle-Wrap="false" OnPageIndexChanging="GridView1_PageIndexChanging" ShowFooter="true" OnRowDataBound="GridView1_RowDataBound">
                                    <PagerStyle CssClass="neoPager" />
                                    <EmptyDataTemplate>
                                        <h4>No results found.</h4>
                                    </EmptyDataTemplate>
                                </asp:GridView>
    
                                <asp:GridView CssClass="neoGrid" ID="GridView2" runat="server" AutoGenerateColumns="true"
                                    HeaderStyle-Wrap="false" RowStyle-Wrap="false" style="display: none;" ShowFooter="true">            
                                    <EmptyDataTemplate>
                                        <h4>No results found.</h4>
                                    </EmptyDataTemplate>
                                </asp:GridView>

                         </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
            </div>
        </div>

    </div>
    </form>
</body>
</html>