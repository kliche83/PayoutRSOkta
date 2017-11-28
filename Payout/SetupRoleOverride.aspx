<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="SetupRoleOverride.aspx.cs" Inherits="WebApplication3.SetupRoleOverride" EnableEventValidation="false" %>

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
                        
            $("table tbody").find("tr").click(function (e) {
                $(this).toggleClass("menuActive");
                
                $('#HiddenIdTextBox').val(e.currentTarget.cells['0'].innerHTML);
                $('#trainerAddDDL option').filter(function (index) { return $(this).text() === e.currentTarget.cells['1'].innerHTML; }).prop('selected', true);
                $('#sstoreAddDDL option[value="' + e.currentTarget.cells['2'].innerHTML + '"]').attr('selected', 'selected');
                $('#programAddDDL option[value="' + e.currentTarget.cells['3'].innerHTML + '"]').attr('selected', 'selected');

                var WeeklyComp = e.currentTarget.cells['4'].innerHTML;
                if (WeeklyComp == "&nbsp;") {
                    WeeklyComp = "";
                }
                $('#WeeklyCompensationTxt').val(WeeklyComp);
                
                $('#OverrideTxt').val(e.currentTarget.cells['5'].innerHTML);

                //var EffDate = e.currentTarget.cells['6'].innerHTML;
                //if (EffDate == "" || EffDate == "&nbsp;") {
                //    var d = new Date();
                //    var curr_date = d.getDate();
                //    var curr_month = d.getMonth() + 1;
                //    var curr_year = d.getFullYear();

                //    EffDate = curr_month + "/" + curr_date + "/" + curr_year;
                //}
                //$('#EffectiveDate').val(EffDate);

                $('#EffectiveDate').css('visibility', 'hidden');
                $('#effdate_label').css('visibility', 'hidden');                

                OpenFieldsDiv();
            });

            $("#addBTN").click(function (event) {
                event.preventDefault();
                
                $('#HiddenIdTextBox').val("");
                $('#trainerAddDDL option').filter(function (index) { return $(this).text() === "All" }).prop('selected', true);
                $('#sstoreAddDDL option[value="All"]').attr('selected', 'selected');
                $('#programAddDDL option[value="All"]').attr('selected', 'selected');                
                $('#WeeklyCompensationTxt').val("");
                $('#OverrideTxt').val("");
                $('#EffectiveDate').val("");
                $('#EffectiveDate').css('visibility', 'visible');
                $('#effdate_label').css('visibility', 'visible');

                OpenFieldsDiv();
            });

            $("#ClosePanelBtn").click(function (event) {
                event.preventDefault();

                $("input[type=text]").css("background-color", "");
                $(".ValidationMessage").empty();

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

                    $("#ClosePanelBtn").click(function (event) {
                        event.preventDefault();
                        
                        $("input[type=text]").css("background-color", "");
                        $(".ValidationMessage").empty();

                        $("#addDiv").hide();
                        $("#notAdd").animate({ top: 50 }, 300);
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

            //$("#Apply").click(function () {
            //    if ($.isNumeric("-10"))
            //    {

            //    }
            //});


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
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>

    <div>
        <asp:UpdatePanel ID="UpdatePanelTopWrapper" runat="server" UpdateMode="Conditional">
            <ContentTemplate>

        <div id="menu">            
            <asp:LinkButton ID="BackBtn" runat="server" Text="Back" PostBackUrl="~/ScheduleTrainer.aspx" ></asp:LinkButton>
        </div>
        
        

        <div id="addDiv" class="addDivMaintenance">
         
            <asp:TextBox ID="HiddenIdTextBox" runat="server"></asp:TextBox>

            <asp:UpdatePanel ID="UpdatePanelUpperBox" UpdateMode="Conditional" runat="server">
                <ContentTemplate>

                    <ul>
                        <li>
                            <div>
                                <b>Trainer:</b> 
                                <asp:DropDownList ID="trainerAddDDL" runat="server" AppendDataBoundItems="true" Width="180px"></asp:DropDownList>
                            </div>                    
                        </li>
                        <li>
                            <div>
                                <b>Store:</b> 
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
                                <b>Weekly Compensation:</b> 
                                <asp:TextBox ID="WeeklyCompensationTxt" runat="server" style="width: 170px;"></asp:TextBox>
                                <br />
                                <label id="ValidateCompensation" runat="server" class="ValidationMessage"></label>
                            </div>                    
                        </li>
                        <li>
                            <div>
                                <b>Override:</b> 
                                <asp:TextBox ID="OverrideTxt" runat="server" style="width: 170px;"></asp:TextBox>
                                <br />
                                <label id="ValidateOverride" runat="server" class="ValidationMessage"></label>
                            </div>                    
                        </li>
                        <li>
                            <div>
                                <b id="effdate_label">Effective Date:</b>                         
                                <asp:TextBox ID="EffectiveDate" runat="server" CssClass="Date" style="width: 170px;"></asp:TextBox> &nbsp; 
                                <br />
                                <label id="ValidateEffectiveDate" runat="server" class="ValidationMessage"></label>
                                <asp:FilteredTextBoxExtender ID="EffectiveDate_Filter" TargetControlID="EffectiveDate" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>
                            </div>                    
                        </li>
                        <li>
                            <div>
                                <asp:Button ID="Apply" runat="server" OnClick="Apply_Click" Text="Apply Changes" />                                
                                <asp:Button ID="ClosePanelBtn" runat="server" Text="Close Panel" />
                            </div>                    
                        </li>
                    </ul>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>


        <div id="notAdd">
            
            Trainer: 
            <asp:DropDownList ID="trainerDDL" runat="server" AppendDataBoundItems="false" OnSelectedIndexChanged="DropDownChanged" AutoPostBack="true">            
            </asp:DropDownList> &nbsp;                              
            Store: 
            <asp:DropDownList ID="sstoreDDL" runat="server" AppendDataBoundItems="false" OnSelectedIndexChanged="DropDownChanged" AutoPostBack="true">            
            </asp:DropDownList> &nbsp; 
            Program:
            <asp:DropDownList ID="programDDL" runat="server" AppendDataBoundItems="false" OnSelectedIndexChanged="DropDownChanged" AutoPostBack="true">
            </asp:DropDownList> &nbsp;

            <input type="submit" id="addBTN" value="Add Record" />
                    
            <br /><br />
        
            
            <%--<asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Always">--%>
            <asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server" HeaderStyle-Wrap="false" RowStyle-Wrap="false" AutoGenerateColumns="true"
                    OnRowDataBound="updateGrid_RowDataBound">
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

                </ContentTemplate>
            </asp:UpdatePanel>

    </div>
    </form>
</body>
</html>