<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="SetupRoleCommission.aspx.cs" Inherits="WebApplication3.SetupRoleCommission" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/ServerGridPagingUC.ascx" TagPrefix="uc1" TagName="ServerGridPagingUC" %>

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
            
            $("table tbody").find("tr").click(function (e) {
                $(this).toggleClass("menuActive");
                
                $('#HiddenId').val(e.currentTarget.cells['0'].innerHTML);
                $('#sstoreAddDDL option[value="' + e.currentTarget.cells['1'].innerHTML + '"]').attr('selected', 'selected');
                $('#programAddDDL option[value="' + e.currentTarget.cells['2'].innerHTML + '"]').attr('selected', 'selected');
                $('#roleAddDDL option').filter(function (index) { return $(this).text() === e.currentTarget.cells['3'].innerHTML; }).prop('selected', true);
                $('#CommissionTxt').val(e.currentTarget.cells['4'].innerHTML);

                //var EffDate = e.currentTarget.cells['5'].innerHTML;
                //if (EffDate == "" || EffDate == "&nbsp;") {
                //    var d = new Date();
                //    var curr_date = d.getDate();
                //    var curr_month = d.getMonth() + 1;
                //    var curr_year = d.getFullYear();

                //    EffDate = curr_month + "/" + curr_date + "/" + curr_year;
                //}
                //$('#EffectiveDate').val(EffDate);
                $('#lblEffectiveDate').css('visibility', 'hidden');
                $('#effdate_label').css('visibility', 'hidden');


                OpenFieldsDiv();
            });

            $("#addBTN").click(function (event) {
                event.preventDefault();

                $('#HiddenId').val("");
                $('#sstoreAddDDL option[value="All"]').attr('selected', 'selected');
                $('#programAddDDL option[value="All"]').attr('selected', 'selected');
                $('#roleAddDDL option').filter(function (index) { return $(this).text() === "All" }).prop('selected', true);
                $('#CommissionTxt').val("");
                //$('#EffectiveDate').val("");
                $('#lblEffectiveDate').css('visibility', 'visible');
                $('#effdate_label').css('visibility', 'visible');

                OpenFieldsDiv();
            });

            $("#ClosePanelBtn").click(function (event) {
                event.preventDefault();

                $("#addDiv").hide();
                $("#notAdd").animate({ top: 50 }, 300);
            });

            

            function OpenFieldsDiv()
            {
                if ($("#notAdd").offset().top == 50) {
                    $("#notAdd").animate({ top: 350 }, 300, function () {
                        $("#addDiv").fadeIn();
                    });
                }
                else {
                    $("#addDiv").hide();
                    $("#notAdd").animate({ top: 50 }, 300);
                }
            }
            

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

            <%--function gridviewScroll() {
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
            }--%>

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
            <asp:LinkButton ID="BackBtn" runat="server" Text="Back" PostBackUrl="~/ScheduleTrainer.aspx" ></asp:LinkButton>
        </div>
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>

        <div id="addDiv" class="addDivMaintenance">


            <%--<span id="HiddenId" style="display:none"></span>--%>            
            <asp:TextBox ID="HiddenIdTextBox" runat="server"></asp:TextBox>

            <ul>
                <li>
                    <div>
                        <b>StoreName:</b> 
                        <asp:DropDownList ID="sstoreAddDDL" runat="server" AppendDataBoundItems="true" Width="180px"></asp:DropDownList>
                    </div>                    
                </li>
                <li>
                    <div>
                        <b>Program:</b> 
                        <asp:DropDownList ID="programAddDDL" runat="server" AppendDataBoundItems="true" Width="180px"></asp:DropDownList>
                    </div>                    
                </li>
                <li>
                    <div>
                        <b>Role:</b> 
                        <asp:DropDownList ID="roleAddDDL" runat="server" AppendDataBoundItems="true" Width="180px"></asp:DropDownList>
                    </div>                    
                </li>
                <li>
                    <div>
                        <b>Standard Commission:</b> 
                        <asp:TextBox ID="CommissionTxt" runat="server" style="width: 170px;"></asp:TextBox>
                    </div>                    
                </li>

                <li>
                    <div>
                        <b id="effdate_label">Effective Date:</b>
                        <asp:TextBox ID="lblEffectiveDate" runat="server" CssClass="Date" style="width: 170px;"></asp:TextBox> &nbsp; 
                        <asp:FilteredTextBoxExtender ID="lblEffectiveDate_Filter" TargetControlID="lblEffectiveDate" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>
                    </div>                    
                </li>
                

                <li>
                    <div>
                        <asp:Button ID="Apply" runat="server" OnClick="Apply_Click" Text="Apply Changes" />
                        <asp:Button ID="ClosePanelBtn" runat="server" Text="Close Panel" />
                    </div>                    
                </li>
            </ul>            
            
        </div>


        <div id="notAdd">
                             
            Store: 
            <asp:DropDownList ID="storeDDL" runat="server" CssClass="FilterControl" AppendDataBoundItems="false" OnSelectedIndexChanged="DropDownChanged" AutoPostBack="true">            
            </asp:DropDownList> &nbsp; 
            Program:
            <asp:DropDownList ID="programDDL" runat="server" CssClass="FilterControl" AppendDataBoundItems="false" OnSelectedIndexChanged="DropDownChanged" AutoPostBack="true">
            </asp:DropDownList> &nbsp; 
            Role:        
            <asp:DropDownList ID="roleDDL" runat="server" CssClass="FilterControl" AppendDataBoundItems="false" OnSelectedIndexChanged="DropDownChanged" AutoPostBack="true">
            </asp:DropDownList> &nbsp; 

            <input type="submit" id="addBTN" value="Add Record" />
                    
            <br /><br />
        
            <span class="DescriptionMessage">* Please click on a record to perform change on it</span>
            <uc1:ServerGridPagingUC runat="server" id="ServerGridPagingUC1" />


            <%--<asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Always">                

            <ContentTemplate>
                <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server" HeaderStyle-Wrap="false" RowStyle-Wrap="false" AutoGenerateColumns="true"
                    OnRowDataBound="GridView1_RowDataBound">
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


            </asp:UpdatePanel>--%>
            
        </div>

    </div>
    </form>
</body>
</html>