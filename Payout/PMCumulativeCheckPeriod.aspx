<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PMCumulativeCheckPeriod.aspx.cs" Inherits="WebApplication3.PMCumulativeCheckPeriod" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css"/>
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script src="//code.jquery.com/ui/1.11.2/jquery-ui.js"></script>

    <script type="text/javascript">

        var txtCheckDate = $("#txtCheckDate"),
            allFields = $([]).add(txtCheckDate)

        $(document).ready(function () {
   
            var $dialog = $("#dialog-form").dialog({
                autoOpen: false,
                height: 200,
                width: 350,
                modal: true,
                buttons: {
                    "Set CheckDate": SetCheckDate,
                    Cancel: function () {
                        $dialog.dialog("close");
                    }
                },
                close: function () {
                    allFields.removeClass("ui-state-error");
                }
            });

            var $form = $dialog.find("form").on("submit", function (event) {
                event.preventDefault();
                SetCheckDate();
            });

            var date1 = new Date($("#NextCheckDate")[0].value);
            var date2 = new Date(date1);


            firstload();

            function firstload() {                    
                
                if ($("#ExistOpenCheckDate")[0].value == "True")
                {                    
                    date1.setDate(date1.getDate() + 1);
                    date2.setDate(date2.getDate() + 15);

                    var SelDateFormated = date2.getFullYear() + "-" + ("0" + (date2.getMonth() + 1)).slice(-2) + "-" + date2.getDate()

                    $("#txtCheckDate").val(SelDateFormated);
                    $dialog.dialog("open");
                }
            }

            function SetCheckDate() {                
                allFields.removeClass("ui-state-error");
                ServerPost($("#txtCheckDate")[0].value);
                //Build Ajax to send selected date
            }
            
            
            var __picker = $.fn.datepicker;

            $.fn.datepicker = function (options) {
                __picker.apply(this, [options]);
                var $self = this;

                if (options && options.trigger) {
                    $(options.trigger).bind("click", function () {
                        $self.datepicker("show");
                    });
                }
            }
                            

            $("#txtCheckDate").datepicker({
                
                dateFormat: 'yy-mm-dd',
                beforeShowDay: function (date) {
                    
                    if (date < date1) {                        
                        return [false, '', 'unAvailable'];
                    }
                    else
                    {
                        if (date <= date2){
                            return [true, 'ui-state-error', '15 Days'];
                        }                            
                        else {
                            return [true, '', 'Available'];
                        }
                        
                    }                    
                }
            });            


            function ServerPost(CheckDate)
            {
                var CheckDateCast = new Date(CheckDate);
                if (CheckDateCast > date1)
                {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: "PMCumulativeCheckPeriod.aspx/JSONRequestFromClient",
                        dataType: "json",
                        data: "{'CheckDate': '" + CheckDate + "'}",
                        success: function (data) {
                            window.location = 'PMCumulativeCheckPeriod.aspx';
                        },
                        error: function (request, status, error) {
                            alert(request.responseText);
                        }
                    });
                }                
            }
        });

    </script>
</head>
<body>

    

    <form id="form1" runat="server">

        <asp:HiddenField ID="ExistOpenCheckDate" runat="server" />
        <asp:HiddenField ID="NextCheckDate" runat="server" />

        <div id="dialog-form" title="Create new Check Date">
            <p class="validateTips">A new Check Date is Required.</p>
 
            Next Check Date:
            <input id="txtCheckDate" type="text" style="width: 100px;" />
            <button id="button" style="display:none"></button>        
            <input type="submit" tabindex="-1" style="position:absolute; top:-1000px"/>

        </div>    

        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>

        <div id="menu">            
            <asp:LinkButton ID="BackBtn" runat="server" Text="Back" PostBackUrl="~/PMCumulativeSummary.aspx" ></asp:LinkButton>
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>
        </div> 

        <div id="notAdd">

            <span>To</span>
            <asp:DropDownList ID="ddlCheckPeriod" runat="server" style="width: 110px;" AutoPostBack="true" OnSelectedIndexChanged="ddlCheckPeriod_OnSelectedIndexChanged">
            </asp:DropDownList>

            <label id="MessageAlert" runat="server"></label>

            <asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Always">
                <ContentTemplate>
                    <asp:GridView CssClass="neoGrid DynamicTable scrollableContent" ID="GRVCheckPeriod" runat="server" AutoGenerateColumns="false"
                        AllowSorting="True" HeaderStyle-Wrap="false" RowStyle-Wrap="false" OnRowDataBound="GRVCheckPeriod_RowDataBound">

                        <EmptyDataTemplate>
                            <h4>No results found.</h4>
                        </EmptyDataTemplate>
                        <Columns>
                    
                            <asp:BoundField DataField="CheckDate" HeaderText="Check Date" SortExpression="CheckDate" ReadOnly="True" dataformatstring="{0:MM/dd/yyyy}" />
                            <asp:BoundField DataField="TrainerName" HeaderText="Trainer" ItemStyle-CssClass="ColumnCategory" SortExpression="TrainerName" ReadOnly="True" />
                            <asp:BoundField DataField="Manager" HeaderText="Manager" SortExpression="Manager" ReadOnly="True" />
                            <asp:BoundField DataField="Role" HeaderText="Role" SortExpression="Role" ReadOnly="True" />
                            <asp:BoundField DataField="Salary" HeaderText="Salary" SortExpression="Salary" ReadOnly="True" />
                            <asp:BoundField DataField="Override" HeaderText="Override" SortExpression="Override" ReadOnly="True" />
                            <asp:BoundField DataField="Commission" HeaderText="Commission" SortExpression="Commission" ReadOnly="True" />
                            <asp:BoundField DataField="OverPay" HeaderText="Over Pay" SortExpression="OverPay" ReadOnly="True" />
                            <asp:BoundField DataField="UnderPay" HeaderText="Under Pay" SortExpression="UnderPay" ReadOnly="True" />

                            <asp:TemplateField HeaderText="OTPremium" SortExpression="OTPremium">
                                <ItemTemplate>
                                    <asp:TextBox ID="OTPremium" runat="server" Text='<%# Eval("OTPremium") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="VacationPay" SortExpression="VacationPay">
                                <ItemTemplate>
                                    <asp:TextBox ID="VacationPay" runat="server" Text='<%# Eval("VacationPay") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="TravelTime" SortExpression="Traveltime">
                                <ItemTemplate>
                                    <asp:TextBox ID="Traveltime" runat="server" Text='<%# Eval("TravelTime") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="ClawBack" SortExpression="ClawBack">
                                <ItemTemplate>
                                    <asp:TextBox ID="ClawBack" runat="server" Text='<%# Eval("ClawBack") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Adjustment" SortExpression="Adjustment">
                                <ItemTemplate>
                                    <asp:TextBox ID="Adjustment" runat="server" Text='<%# Eval("Adjustment") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="AdjustmentComment" SortExpression="AdjustmentComment">
                                <ItemTemplate>
                                    <asp:TextBox ID="AdjustmentComment" runat="server" Text='<%# Eval("AdjustmentComment") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>                                                
                        
                            <asp:BoundField DataField="PayCheck" HeaderText="Pay Check" SortExpression="PayCheck" ReadOnly="True" />
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:Button ID="SubmitPayroll" runat="server" OnClick="SubmitPayroll_Click" Text="Submit Rotation" />

        </div>
    </form>
</body>
</html>
