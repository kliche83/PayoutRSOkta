<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="GridRoleCommissionDaily.aspx.cs" Inherits="WebApplication3.GridRoleCommissionDaily" EnableEventValidation="false" %>

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
                //debugger;

                if ($(this)[0].id == "TabOverrideBtn")
                {                    
                    $(location).attr('href', '/GridRoleOverride.aspx')
                }
               // switchTab($(this)[0].id);
            });
            
            function firstload() {
                $('#TabCommissionBtn').addClass('Active').removeClass('Inactive');
                $('#TabOverrideBtn').addClass('Inactive').removeClass('Active');
            }
        });

        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="menu">            
            <%--<asp:LinkButton ID="BackBtn" runat="server" Text="Back" PostBackUrl="~/GridRoleCommission.aspx" ></asp:LinkButton>--%>
            <asp:LinkButton ID="BackBtn" runat="server" Text="Back" OnClick="BackBtn_Click" ></asp:LinkButton>   
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>                        
        </div>
        

       <%-- <asp:GridView CssClass="neoGrid" ID="TopTable" runat="server" AutoGenerateColumns="true" 
                HeaderStyle-Wrap="false" RowStyle-Wrap="false">
        </asp:GridView>--%>

                            
        <br /><br />
        <div id="External Panel">

            <asp:Panel ID="notAdd" runat="server">
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

                    </asp:Panel>
                </div>

            </asp:Panel>
        </div>

    </div>
    </form>
</body>
</html>