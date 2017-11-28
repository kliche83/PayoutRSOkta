    <%@ Page Language="C#" AutoEventWireup="True" CodeBehind="AuditSales.aspx.cs" Inherits="WebApplication3.AuditSales" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%--<%@ Register Src="~/UserControls/ServerGridPagingUC.ascx" TagPrefix="uc1" TagName="ServerGridPagingUC" %>--%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Audit Sales</title>
    <%--<link href="Content/CalendarStyle.css" rel="stylesheet" type="text/css" />--%>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script src="//code.jquery.com/ui/1.11.2/jquery-ui.js"></script>

    <script type="text/javascript">

        $(document).ready(function () {

            $("#loadDiv").not(".keep").hide();
            $("#errorSpan").not(".keep").hide();
            $("#loadGif").hide();
            setCheckBoxes();

            var prm = Sys.WebForms.PageRequestManager.getInstance();
            var $dialog;

            prm.add_beginRequest(function (source, args) {
                $("#loadDiv").show();
                $("#loadGif").show();
            });

            prm.add_endRequest(function (source, args) {                
                $("#loadDiv").not(".keep").hide();
                $("#errorSpan").not(".keep").hide();
                $("#loadGif").hide();

                setCheckBoxes();
                /**********************************************************************************************************/
                $("#btnSnapshot").click(function () {

                    SetDialogMessageOkCancel("<div> Unlocked transactions will be posted and locked.Are you sure to continue?</div>");
                    $dialog.dialog("open");
                })

                function LoadImageShow() {
                    $("#loadDiv").show();
                    $("#loadGif").show();
                }


                function LoadImageStop() {
                    $("#loadDiv").not(".keep").hide();
                    $("#errorSpan").not(".keep").hide();
                    $("#loadGif").hide();
                }


                function ConfirmPost() {
                    LoadImageShow();

                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: "AuditSales.aspx/JSONRequestFromClient",
                        dataType: "json",
                        success: function (data) {
                            if (data.d == "0") {
                                SetDialogMessageClose("<div>No sales were posted</div>")
                            }
                            else {
                                SetDialogMessageClose("<div>" + data.d + " sales were posted successfully</div>")
                            }
                            LoadImageStop();
                        },
                        error: function (request, status, error) {
                            alert(request.responseText);
                        }
                    });
                }

                function SetDialogMessageClose(htmlMessage) {
                    $(htmlMessage).dialog({
                        modal: true,
                        buttons: {
                            Ok: function () {
                                LoadImageShow();
                                $(this).dialog("close");
                                window.location = "AuditSales.aspx";
                            }
                        }
                    });
                }

                function SetDialogMessageOkCancel(htmlMessage) {                    
                    $dialog = $(htmlMessage).dialog({
                        autoOpen: false,
                        resizable: false,
                        height: "auto",
                        width: 400,
                        modal: true,
                        buttons: {
                            Ok: function () {
                                ConfirmPost();
                                $(this).dialog("close");
                            },
                            Cancel: function () {
                                $(this).dialog("close");
                            }
                        }
                    });
                }  

                function setCheckBoxes() {      
                    if ($("#rbl_ReportType").find(":checked").val() == "Reconciliation") {
                        //$("#btnSnapshot").show();
                        $("#ChkUserChanges").next().hide();
                        $("#ChkUserChanges").hide();
                        $(".reconciliationHeader").show();
                        
                        if ($("#cbl_SubReconciliation").find('input:checkbox')[4].checked) {
                            $("#cbl_SubReconciliation").find('input:checkbox')[0].disabled = true;
                            $("#cbl_SubReconciliation").find('input:checkbox')[1].disabled = true;
                            $("#cbl_SubReconciliation").find('input:checkbox')[2].disabled = true;
                            $("#cbl_SubReconciliation").find('input:checkbox')[3].disabled = true;
                            //$("#btnSnapshot")[0].disabled = true;
                        }
                        else {
                            $("#cbl_SubReconciliation").find('input:checkbox')[0].disabled = false;
                            $("#cbl_SubReconciliation").find('input:checkbox')[1].disabled = false;
                            $("#cbl_SubReconciliation").find('input:checkbox')[2].disabled = false;
                            $("#cbl_SubReconciliation").find('input:checkbox')[3].disabled = false;
                            //$("#btnSnapshot")[0].disabled = false;
                        }
                    }
                    else
                    {
                        //$("#btnSnapshot").hide();
                        $("#ChkUserChanges").next().show();
                        $("#ChkUserChanges").show();
                        $(".reconciliationHeader").hide();
                    }
                }

                $("#cbl_SubReconciliation input[type='checkbox']").change(function () {
                    setCheckBoxes();
                });
                /**********************************************************************************************************/

            });

            

            $("#cbl_SubReconciliation input[type='checkbox']").change(function () {
                setCheckBoxes();
            });


            function setCheckBoxes()
            {
                if ($("#rbl_ReportType").find(":checked").val() == "Reconciliation") {
                    //$("#btnSnapshot").show();
                    $("#ChkUserChanges").next().hide();
                    $("#ChkUserChanges").hide();
                    if ($("#cbl_SubReconciliation").find('input:checkbox')[4].checked) {
                        $("#cbl_SubReconciliation").find('input:checkbox')[0].disabled = true;
                        $("#cbl_SubReconciliation").find('input:checkbox')[1].disabled = true;
                        $("#cbl_SubReconciliation").find('input:checkbox')[2].disabled = true;
                        $("#cbl_SubReconciliation").find('input:checkbox')[3].disabled = true;
                       // $("#btnSnapshot")[0].disabled = true;
                    }
                    else {
                        $("#cbl_SubReconciliation").find('input:checkbox')[0].disabled = false;
                        $("#cbl_SubReconciliation").find('input:checkbox')[1].disabled = false;
                        $("#cbl_SubReconciliation").find('input:checkbox')[2].disabled = false;
                        $("#cbl_SubReconciliation").find('input:checkbox')[3].disabled = false;
                        //$("#btnSnapshot")[0].disabled = false;
                    }
                }
                else
                {
                    //$("#btnSnapshot").hide();
                    $("#ChkUserChanges").next().show();
                    $("#ChkUserChanges").show();
                }
            }


            //$("#btnSnapshot").click(function () {

            //    SetDialogMessageOkCancel("<div> Unlocked transactions will be posted and locked.Are you sure to continue?</div>");
            //    $dialog.dialog("open");
            //})

            function LoadImageShow()
            {
                $("#loadDiv").show();
                $("#loadGif").show();                
            }


            function LoadImageStop() {
                $("#loadDiv").not(".keep").hide();
                $("#errorSpan").not(".keep").hide();
                $("#loadGif").hide();
            }


            //function ConfirmPost()
            //{
            //    LoadImageShow();

            //    $.ajax({
            //        type: "POST",
            //        contentType: "application/json; charset=utf-8",
            //        url: "AuditSales.aspx/JSONRequestFromClient",
            //        dataType: "json",                    
            //        success: function (data) {
            //            if (data.d == "0") {
            //                SetDialogMessageClose("<div>No sales were posted</div>")
            //            }
            //            else
            //            {
            //                SetDialogMessageClose("<div>" + data.d + " sales were posted successfully</div>")                            
            //            }
            //            LoadImageStop();
            //        },
            //        error: function (request, status, error) {
            //            alert(request.responseText);
            //        }
            //    });                
            //}

            //function SetDialogMessageClose(htmlMessage) {
            //    $(htmlMessage).dialog({
            //        modal: true,
            //        buttons: {
            //            Ok: function () {
            //                LoadImageShow();
            //                $(this).dialog("close");
            //                window.location = "AuditSales.aspx";
            //            }
            //        }
            //    });
            //}

            //function SetDialogMessageOkCancel(htmlMessage)
            //{
            //    $dialog = $(htmlMessage).dialog({
            //        autoOpen: false,
            //        resizable: false,
            //        height: "auto",
            //        width: 400,
            //        modal: true,
            //        buttons: {
            //            Ok: function () {
            //                ConfirmPost();
            //                $(this).dialog("close");
            //            },
            //            Cancel: function () {
            //                $(this).dialog("close");
            //            }
            //        }
            //    });
            //}            

        });

    </script>

</head>
<body>
    <form id="form1" runat="server">
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>

        <div id="menu">
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>
        </div>  
              
        <br />
        <div id="loadDiv" runat="server" style="position: absolute; top: 0; bottom: 0; left: 0; right: 0; background-color: rgbA(255, 255, 255, 0.8); z-index: 9999; overflow: hidden; text-align: center; padding: 120px 10px 10px 10px;">
            <span id="errorSpan" runat="server">
                <asp:Label ID="errorMsg" runat="server" ForeColor="Black" Font-Bold="true" Font-Size="Medium"></asp:Label>
                <br /><br />
                <input id="gotIt" runat="server" value="OK" style="padding: 10px; border: 0; background-color: #2764AB; color: #fff; cursor: pointer; width: 50px!important;" />
                <br />
            </span>

            <img id="loadGif" runat="server" src="../Content/load.gif" />
        </div>

        <%--<asp:Button ID="btnResetFilters" runat="server" Text="Reset Filters" OnClick="btnResetFilters_Click" />--%>

        <asp:UpdatePanel ID="UpdatePanelUpperBox" UpdateMode="Conditional" runat="server">
            <ContentTemplate>
                
                <div id="rblContainer">
                    <div id="header">
                        <p>Select Report</p>
                        <p class="reconciliationHeader">Select Filter</p>
                        <p class="reconciliationHeader">Select Snapshots to display</p>
                    </div>
                    <div>
                        <asp:RadioButtonList ID="rbl_ReportType" CssClass="ctrlAudit" runat="server" AutoPostBack="true">
                            <asp:ListItem>Rpt</asp:ListItem>
                            <asp:ListItem>Reconciliation</asp:ListItem>
                        </asp:RadioButtonList>

                        <asp:CheckBoxList ID="cbl_SubReconciliation" CssClass="ctrlAudit" runat="server">
                            <asp:ListItem Text="Program" Value="1" ></asp:ListItem>
                            <asp:ListItem Text="Qty" Value="2" ></asp:ListItem>
                            <asp:ListItem Text="Retailer Cost" Value="3" ></asp:ListItem>
                            <asp:ListItem Text="Extended Cost" Value="4" ></asp:ListItem>
                            <asp:ListItem Text="Deleted" Value="5" ></asp:ListItem>
                        </asp:CheckBoxList>                    
                    
                        <asp:RadioButtonList id="rbtn_LastLocked" CssClass="ctrlAudit" runat="server" >
                            <asp:ListItem Text="All"></asp:ListItem>
                            <asp:ListItem Text="Latest"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>


                <div id="btnPanel">
                    <asp:Button ID="btnResetFilters" runat="server" Text="Reset Filters" OnClick="btnResetFilters_Click" />
                    PageSize:
                    <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true">
                        <asp:ListItem Text="10" Value="10" />
                        <asp:ListItem Text="25" Value="25" />
                        <asp:ListItem Text="50" Value="50" />
                    </asp:DropDownList>
                
                    <asp:CheckBox ID="ChkUserChanges" runat="server" Text="User Changes" AutoPostBack="true"/>
                
                    <%--<asp:Button ID="btnSnapshot" runat="server" Text="Snapshot" />--%>

                    <input id="btnRefresh" type="submit" value="Refresh"/>
                </div>
                
                                
                <hr />

                <div style="overflow: auto; overflow-y: auto; max-height: 63vh;" runat="server">
                    <asp:gridview id="AuditGridView" runat="server" CssClass="neoGrid"
                        HeaderStyle-Wrap="false" ItemStyle-Wrap="False" RowStyle-Wrap="false" ShowHeaderWhenEmpty="true"> 
                        <EmptyDataTemplate>
                            No results found.
                        </EmptyDataTemplate>
                    </asp:gridview>
                </div>

                <br />
                <asp:Repeater ID="rptPager" runat="server">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkPage" runat="server" Text = '<%#Eval("Text") %>' CommandArgument = '<%# Eval("Value") %>' Enabled = '<%# Eval("Enabled") %>' OnClick = "Page_Changed"></asp:LinkButton>
                    </ItemTemplate>
                </asp:Repeater>
                <br />
                <asp:Label ID="RecordCount" runat="server"></asp:Label>
            </ContentTemplate>
        </asp:UpdatePanel>
        
    </form>
</body>
</html>