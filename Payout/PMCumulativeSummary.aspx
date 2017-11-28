<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PMCumulativeSummary.aspx.cs" EnableEventValidation="false" Inherits="WebApplication3.PMCumulativeSummary" %>

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

        var TabChosen = "TabYearButton";

        $(document).ready(function () {

            firstload();

            $(".TopButtons").find(".TabButtons").click(function () {
                switchTab($(this)[0].id);
            });


            function firstload(){
                switchTab(TabChosen);
            }

            function switchTab(TabName)
            {
                $('#ExternalPanelYear').hide();
                $('#ExternalPanelBiWeek').hide();
                //$('#ExternalPanelCheckPeriod').hide();
                $('#TabYearButton').addClass('Inactive').removeClass('Active');
                $('#TabBiWeekButton').addClass('Inactive').removeClass('Active');
                //$('#TabCheckPeriodButton').addClass('Inactive').removeClass('Active');

                switch (TabName) {
                    case "TabYearButton":
                        $('#ExternalPanelYear').show();
                        $('#TabYearButton').addClass('Active').removeClass('Inactive');
                        break;
                    case "TabBiWeekButton":
                        $('#ExternalPanelBiWeek').show();
                        $('#TabBiWeekButton').addClass('Active').removeClass('Inactive');
                        break;
                    //case "TabCheckPeriodButton":
                    //    $('#ExternalPanelCheckPeriod').show();
                    //    $('#TabCheckPeriodButton').addClass('Active').removeClass('Inactive');
                    //    break;
                }
            }
            
        });

        
    </script>
</head>
<body>
    <form id="form1" runat="server">

    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>

        <div>
            <div id="menu">            
                <asp:LinkButton ID="BackBtn" runat="server" Text="Back" PostBackUrl="~/ScheduleTrainer.aspx" ></asp:LinkButton>
                <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>                        
                <asp:LinkButton ID="CheckPeriod" runat="server" Text="Check Period" PostBackUrl="~/PMCumulativeCheckPeriod.aspx"></asp:LinkButton>                        
            </div>        
                            
            <br /><br />
            <div id="notAdd">
            
                <ul class="TopButtons">
                    <li>
                        <div id="containerYear" class="WrapperDivs">
                            <div id="TabYearButton" runat="server" class="TabButtons">Year Report</div>                        
                        </div>
                    
                    </li>
                    <li>
                        <div id="containerBiWeek" class="WrapperDivs">
                            <div id="TabBiWeekButton" runat="server" class="TabButtons">Bi Weekly Report</div>                        
                        </div>
                    </li>
                </ul>            

            
                <div id="TableWrapper" style="height:87%">

                    <asp:Panel ID="ExternalPanelYear" CssClass="tablesWrapper" runat="server">
                        <asp:UpdatePanel ID="updateGridYearly" runat="server" UpdateMode="Conditional" style="height:100%">
                            <ContentTemplate>

                                <span>Year</span>
                                <asp:DropDownList ID="ddlYear" runat="server" style="width: 110px;" AutoPostBack="true" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged">
                                </asp:DropDownList>

                                <div style="height:auto;width:100%; padding:0; margin-top: 10px;" >
                                    <table id="HeaderYearly" class="neoGrid" style="font-family:Arial;font-size:10pt;width:100%;color:white;
                                        border-collapse:collapse;height:100%;">
                                        <thead>
                                            <tr>
                                                <th style ="width:10%;text-align:center">CheckDate</th>
                                                <th style ="width:22%;text-align:center">Trainer</th>
                                                <th style ="width:22%;text-align:center">Manager</th>
                                                <th style ="width:6%;text-align:center">Role</th>
                                                <th style ="width:13%;text-align:center">Year-End Balance</th>
                                                <th style ="width:13%;text-align:center">Quarterly True-Up Bonus</th>
                                                <th style ="width:13%;text-align:center">Remaining Balance</th>
                                                <th style ="width:1%"></th>
                                            </tr>
                                        </thead>                                        
                                    </table>
                                </div>
                            
                                <div style ="height:98%; width:100%; overflow:auto;">
                                    <asp:GridView CssClass="neoGrid" ID="GRVYearly" runat="server" AutoGenerateColumns="false"
                                        AllowSorting="True" HeaderStyle-Wrap="false" RowStyle-Wrap="false" OnRowDataBound="GridViewYear_RowDataBound" ShowHeader = "false">

                                        <EmptyDataTemplate>
                                            <h4>No results found.</h4>
                                        </EmptyDataTemplate>
                                        <Columns>                    
                                            <asp:BoundField ItemStyle-Width = "10%" DataField="CheckDate" HeaderText="Check Date" SortExpression="CheckDate" ReadOnly="True" dataformatstring="{0:MM/dd/yyyy}" />
                                            <asp:BoundField ItemStyle-Width = "22%" DataField="Trainer" HeaderText="Trainer" ItemStyle-CssClass="ColumnCategory" SortExpression="Trainer" ReadOnly="True" />
                                            <asp:BoundField ItemStyle-Width = "22%" DataField="Manager" HeaderText="Manager" SortExpression="Manager" ReadOnly="True" />
                                            <asp:BoundField ItemStyle-Width = "6%" DataField="Role" HeaderText="Role" SortExpression="Role" ReadOnly="True" />
                                            <asp:BoundField ItemStyle-Width = "13%" DataField="Year-End Balance" HeaderText="Year-End Balance" SortExpression="Year-End Balance" ReadOnly="True" />
                                            <asp:BoundField ItemStyle-Width = "13%" DataField="Quarterly True-Up Bonus" HeaderText="Quarterly True-Up Bonus" SortExpression="Quarterly True-Up Bonus" ReadOnly="True" />
                                            <asp:BoundField ItemStyle-Width = "13%" DataField="Remaining Balance" HeaderText="Remaining Balance" SortExpression="Remaining Balance" ReadOnly="True" />
                                        </Columns>
                                    </asp:GridView>
                                </div>

                                <div style ="width:100%; overflow:auto;">
                                    <asp:GridView CssClass="TotalizationRow" ID="GRVYearlyTotals" runat="server" AutoGenerateColumns="false"
                                        HeaderStyle-Wrap="false" RowStyle-Wrap="false" OnRowDataBound="GRVYearlyTotals_RowDataBound" ShowHeader = "false">
                                        <Columns>                    
                                            <asp:BoundField ItemStyle-Width = "10%" DataField="CheckDate" HeaderText="Check Date" ReadOnly="True" />
                                            <asp:BoundField ItemStyle-Width = "22%" DataField="Trainer" HeaderText="Trainer" ReadOnly="True" />
                                            <asp:BoundField ItemStyle-Width = "22%" DataField="Manager" HeaderText="Manager" ReadOnly="True" />
                                            <asp:BoundField ItemStyle-Width = "6%" DataField="Role" HeaderText="Role" ReadOnly="True" />
                                            <asp:BoundField ItemStyle-Width = "13%" DataField="YearEndBalance" HeaderText="Year-End Balance" ReadOnly="True" />
                                            <asp:BoundField ItemStyle-Width = "13%" DataField="QuarterlyTrueUpBonus" HeaderText="Quarterly True-Up Bonus" ReadOnly="True" />
                                            <asp:BoundField ItemStyle-Width = "13%" DataField="RemainingBalance" HeaderText="Remaining Balance" ReadOnly="True" />
                                            <asp:BoundField ItemStyle-Width = "1%"/>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>    
                    </asp:Panel>
                            
                                


                        
                    <asp:Panel ID="ExternalPanelBiWeek" CssClass="tablesWrapper" runat="server">
                        <asp:UpdatePanel ID="updateGridBiWeekly" runat="server" UpdateMode="Conditional" style="height:100%">
                            <ContentTemplate>

                                <span>From</span>
                                <asp:DropDownList ID="webStartDate" runat="server" style="width: 110px;">
                                </asp:DropDownList>

                                <span>To</span>
                                <asp:DropDownList ID="webEndDate" runat="server" style="width: 110px;">
                                </asp:DropDownList>

                                <span>Trainer</span>
                                <asp:DropDownList ID="webTrainer" runat="server" style="width: 110px;">
                                </asp:DropDownList>

                                <asp:Button ID="Apply" runat="server" OnClick="Apply_Click" Text="Filter" />                    
                                <%--<asp:Button ID="Undo" runat="server" OnClick="Undo_Click" Text="Reset Results" />--%>

                                <label id="LabelValidation" runat="server" class="ValidationMessage"></label>

                                <asp:Panel ID="ExternalPanelSubBiWeek" CssClass="tablesWrapper" runat="server" Height="100%">
                                    <span class="DescriptionMessage">* Please double click on row to get detailed Commission and Override</span>
                                        
                                                                        
                                    <div style="height:auto;width:100%; padding:0; margin-top: 10px;">
                                        <table id="HeaderBiWeekly" class="neoGrid" style="width:100%;color:white;
                                            border-collapse:collapse;height:100%; ">
                                            <thead>
                                                <tr>
                                                    <th style ="width:15%;text-align:center">CheckDate</th>
                                                    <th style ="width:15%;text-align:center">Trainer</th>
                                                    <th style ="width:15%;text-align:center">Manager</th>
                                                    <th style ="width:5%;text-align:center">Role</th>
                                                    <th style ="width:8%;text-align:center">Salary</th>
                                                    <th style ="width:8%;text-align:center">Override</th>
                                                    <th style ="width:8%;text-align:center">Commission</th>
                                                    <th style ="width:8%;text-align:center">Over Pay</th>
                                                    <th style ="width:8%;text-align:center">Under Pay</th>
                                                    <th style ="width:1%"></th>
                                                </tr>
                                            </thead>
                                        
                                        </table>
                                    </div>

                                
                                    <div style ="max-height:90%; width:100%; overflow:auto;">

                                        <asp:GridView CssClass="neoGrid scrollableContent" ID="GRVBiWeekly" runat="server" AutoGenerateColumns="false"
                                            AllowSorting="True" HeaderStyle-Wrap="false" RowStyle-Wrap="false" OnRowDataBound="GRVBiWeekly_RowDataBound" OnSelectedIndexChanged="GRVBiWeekly_SelectedIndexChanged" ShowHeader = "false">

                                            <EmptyDataTemplate>
                                                <h4>No results found.</h4>
                                            </EmptyDataTemplate>
                                            <Columns>                    
                                                <asp:BoundField ItemStyle-Width = "15%" DataField="CheckDate" HeaderText="Check Date" SortExpression="CheckDate" ReadOnly="True" dataformatstring="{0:MM/dd/yyyy}" />
                                                <asp:BoundField ItemStyle-Width = "15%" DataField="Trainer" HeaderText="Trainer" ItemStyle-CssClass="ColumnCategory" SortExpression="Trainer" ReadOnly="True" />
                                                <asp:BoundField ItemStyle-Width = "15%" DataField="Manager" HeaderText="Manager" SortExpression="Manager" ReadOnly="True" />
                                                <asp:BoundField ItemStyle-Width = "5%" DataField="Role" HeaderText="Role" SortExpression="Role" ReadOnly="True" />
                                                <asp:BoundField ItemStyle-Width = "8%" DataField="Salary" HeaderText="Salary" SortExpression="Salary" ReadOnly="True" />
                                                <asp:BoundField ItemStyle-Width = "8%" DataField="Override" HeaderText="Override" SortExpression="Override" ReadOnly="True" />
                                                <asp:BoundField ItemStyle-Width = "8%" DataField="Commission" HeaderText="Commission" SortExpression="Commission" ReadOnly="True" />
                                                <asp:BoundField ItemStyle-Width = "8%" DataField="Over Pay" HeaderText="Over Pay" SortExpression="OverPay" ReadOnly="True" />
                                                <asp:BoundField ItemStyle-Width = "8%" DataField="Under Pay" HeaderText="Under Pay" SortExpression="UnderPay" ReadOnly="True" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                        
                                    <div style ="width:100%; overflow:auto;">

                                        <asp:GridView CssClass="TotalizationRow" ID="GRVBiWeeklyTotals" runat="server" AutoGenerateColumns="false"
                                            HeaderStyle-Wrap="false" RowStyle-Wrap="false" OnRowDataBound="GRVBiWeeklyTotals_RowDataBound" ShowHeader = "false">

                                            <Columns>                    
                                                <asp:BoundField ItemStyle-Width = "15%" DataField="CheckDate" HeaderText="Check Date" ReadOnly="True" />
                                                <asp:BoundField ItemStyle-Width = "15%" DataField="Trainer" HeaderText="Trainer" ReadOnly="True" />
                                                <asp:BoundField ItemStyle-Width = "15%" DataField="Manager" HeaderText="Manager" ReadOnly="True" />
                                                <asp:BoundField ItemStyle-Width = "5%" DataField="Role" HeaderText="Role" ReadOnly="True" />
                                                <asp:BoundField ItemStyle-Width = "8%" DataField="Salary" HeaderText="Salary" ReadOnly="True" />
                                                <asp:BoundField ItemStyle-Width = "8%" DataField="Override" HeaderText="Override" ReadOnly="True" />
                                                <asp:BoundField ItemStyle-Width = "8%" DataField="Commission" HeaderText="Commission" ReadOnly="True" />
                                                <asp:BoundField ItemStyle-Width = "8%" DataField="OverPay" HeaderText="Over Pay" ReadOnly="True" />
                                                <asp:BoundField ItemStyle-Width = "8%" DataField="UnderPay" HeaderText="Under Pay" ReadOnly="True" />
                                                <asp:BoundField ItemStyle-Width = "1%"/>
                                            </Columns>
                                        </asp:GridView>
                                    </div>


                                </asp:Panel>                
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>                            
                </div>                
            </div>
        </div>
    </form>
</body>
</html>