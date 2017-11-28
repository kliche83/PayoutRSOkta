<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Override.aspx.cs" Inherits="Payout.Override" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Schedule</title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jqueryui/1.9.1/jquery-ui.min.js"></script>
    <script type="text/javascript" src="/Scripts/gridviewScroll.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            gridviewScroll();
            $("#loadDiv").hide();
            $("#loadGif").hide();

            jQuery(function ($) {
                var prm = Sys.WebForms.PageRequestManager.getInstance();
                prm.add_beginRequest(function (source, args) {
                    $("#loadDiv").show();
                    $("#loadGif").show();
                });
                prm.add_endRequest(function (source, args) {
                    gridviewScroll();
                    $(".Date").datepicker();
                    $("#loadDiv").hide();
                    $("#loadGif").hide();

                //    $("tr").not("tr:first-child").click(function (e) {
                //        if (e.ctrlKey) {
                //            if ($(this).hasClass("selectedRow")) {
                //                $(this).removeClass("selectedRow");
                //            }
                //            else {
                //                $(this).addClass("selectedRow");
                //            }
                //        }
                //        else {
                //            if ($(this).hasClass("selectedRow")) {
                //                $("tr").removeClass("selectedRow");
                //            }
                //            else {
                //                $("tr").removeClass("selectedRow");
                //                $(this).addClass("selectedRow");
                //            }
                //        }
                //    }).children().children().click(function (e) {
                //        //if ($(this).hasClass("selectedRow")) {
                //        //return false;
                //        //}
                //        //else {
                //        //    $("tr").removeClass("selectedRow");
                //        //    $(this).addClass("selectedRow");
                //        //}
                //    });
                });
            });

            //$("tr").not("tr:first-child").click(function (e) {
            //    if (e.ctrlKey) {
            //        if ($(this).hasClass("selectedRow")) {
            //            $(this).removeClass("selectedRow");
            //        }
            //        else {
            //            $(this).addClass("selectedRow");
            //        }
            //    }
            //    else {
            //        if ($(this).hasClass("selectedRow")) {
            //            $("tr").removeClass("selectedRow");
            //        }
            //        else {
            //            $("tr").removeClass("selectedRow");
            //            $(this).addClass("selectedRow");
            //        }
            //    }
            //}).children().children().click(function (e) {
            //    //if ($(this).hasClass("selectedRow")) {
            //    //return false;
            //    //}
            //    //else {
            //    //    $("tr").removeClass("selectedRow");
            //    //    $(this).addClass("selectedRow");
            //    //}
            //});

            $(function () {
                $(".Date").datepicker();
            });
        });

        window.onresize = function (event) {
            gridviewScroll();
        };

        function gridviewScroll() {
            $('#<%= GridView1.ClientID %>').gridviewScroll({
                width: window.innerWidth - 50,
                height: window.innerHeight - 150,
                freezesize: 0,
                arrowsize: 30,
                varrowtopimg: "Content/arrowvt.png",
                varrowbottomimg: "Content/arrowvb.png",
                harrowleftimg: "Content/arrowhl.png",
                harrowrightimg: "Content/arrowhr.png"
            });
        }

        function emailClick() {
            $("#loadDiv").fadeIn();
            $("#loadGif").fadeIn();
            var emailClick_confirm_value = document.createElement("INPUT");
            emailClick_confirm_value.type = "hidden";
            emailClick_confirm_value.name = "emailClick_confirm_value";
            if (confirm("Are you sure you want to send out overrides?")) {
                $("#waitMsg").html("Sending overrides... This may take up to 5 minutes. Please stay on this page.");
                emailClick_confirm_value.value = "Yes";
            } else {
                emailClick_confirm_value.value = "No";
            }
            document.forms[0].appendChild(emailClick_confirm_value);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="menu">
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="exclBtn" runat="server" Text="Exclusions" OnClick="exclBtn_Click"></asp:LinkButton>  
            <asp:LinkButton ID="email" runat="server" Text="Send via Email" OnClick="email_Click" OnClientClick="emailClick()"></asp:LinkButton>            
        </div>

        <div id="loadDiv" runat="server" style="position: absolute; top: 0; bottom: 0; left: 0; right: 0; background-color: rgbA(255, 255, 255, 0.8); z-index: 9999; overflow: hidden; text-align: center; padding: 120px 10px 10px 10px;">
            <span id="errorSpan" runat="server">
                <asp:Label ID="waitMsg" runat="server" ForeColor="Black" Font-Bold="true" Font-Size="Medium"></asp:Label>
                <br /><br />
                <%--<input id="gotIt" runat="server" value="OK" style="padding: 10px; border: 0; background-color: #2764AB; color: #fff; cursor: pointer; width: 50px!important;" />
                <br />--%>
            </span>

            <img id="loadGif" runat="server" src="../Content/load.gif" />
        </div>
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>

        <asp:SqlDataSource ID="SqlDataSource1" runat="server" SelectCommandType="StoredProcedure" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting"
            SelectCommand="">
            <SelectParameters>
                <asp:Parameter Name="ovType" Type="String" />
                <asp:Parameter Name="fDate" Type="String" />
                <asp:Parameter Name="tDate" Type="String" />
                <asp:Parameter Name="ownerid" Type="String" />
                <asp:Parameter Name="edglc" Type="String" />
                <asp:Parameter Name="cur" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="SqlPromo" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" OnSelecting="SQLSelecting"
            SelectCommand="SELECT [OwnerName] AS [Promo] FROM [Herbjoy].[dbo].[POwnerMapping] GROUP BY [OwnerName] ORDER BY [OwnerName]">
        </asp:SqlDataSource>

        <div id="notAdd">
            
        <asp:DropDownList ID="ovTypeDDL" runat="server" style="width: 80px;">
            <asp:ListItem Value="DTV" Text="DTV" Selected="True"></asp:ListItem>
            <asp:ListItem Value="B2B" Text="B2B"></asp:ListItem>
        </asp:DropDownList>
        Date: from 
        <asp:TextBox ID="dateFrom" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="dateFrom_Filter" TargetControlID="dateFrom" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>
        to 
        <asp:TextBox ID="dateTo" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="dateTo_Filter" TargetControlID="dateTo" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender> 
        
        <span id="adminSpan" runat="server">
            &nbsp;
            Promo: 
            <asp:DropDownList ID="promoDDL" runat="server" DataSourceID="SqlPromo" AppendDataBoundItems="true" DataTextField="Promo" DataValueField="Promo">
                <asp:ListItem Value="*" Text="All"></asp:ListItem>
            </asp:DropDownList>
            &nbsp;
            Type: 
            <asp:TextBox ID="glTXT" runat="server" style="width: 80px;"></asp:TextBox>
            <%--<asp:DropDownList ID="glDDL" runat="server" AutoPostBack="true" DataSourceID="SqlGL" AppendDataBoundItems="false" DataTextField="gl" DataValueField="gl">
            </asp:DropDownList>--%>
            &nbsp;
            Currency: 
            <asp:DropDownList ID="curDDL" runat="server" style="width: 80px;">
                <asp:ListItem Value="*" Text="All"></asp:ListItem>
                <asp:ListItem Value="USD" Text="USD"></asp:ListItem>
                <asp:ListItem Value="CAD" Text="CAD"></asp:ListItem>
            </asp:DropDownList>
        </span>

        &nbsp;&nbsp;
        <asp:Button ID="searchBtn" runat="server" OnClick="searchBtn_Click" Text="Search" />
        
        <br /><br />
        
<asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Always">
<ContentTemplate>

        <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server" AutoGenerateColumns="true" DataSourceID="SqlDataSource1" AllowPaging="false" PageSize="15" 
            AllowSorting="false" HeaderStyle-Wrap="false" RowStyle-Wrap="false" OnRowDataBound="GridDataBound">
            <PagerStyle CssClass="neoPager" />
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <%--<Columns>
                <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" ReadOnly="true" />
                <asp:BoundField DataField="Promo" HeaderText="Promo" SortExpression="Promo" ReadOnly="true" />
                <asp:BoundField DataField="DC" HeaderText="DC" SortExpression="DC" ReadOnly="true" />
                <asp:BoundField DataField="CC" HeaderText="CC" SortExpression="AC11" ReadOnly="true" />
                <asp:BoundField DataField="Date" HeaderText="Date" SortExpression="Date" ReadOnly="True" DataFormatString="{0:MM/dd/yy}" HtmlEncode="false" />
                <asp:BoundField DataField="Ship" HeaderText="Ship" SortExpression="Ship" ReadOnly="true" />
                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ReadOnly="true" />
                <asp:BoundField DataField="Qty" HeaderText="Qty" SortExpression="Qty" ReadOnly="true" ItemStyle-HorizontalAlign="Right" />
                <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="Amount" ReadOnly="true" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:N}" />
                <asp:BoundField DataField="%" HeaderText="%" SortExpression="%" ReadOnly="true" ItemStyle-HorizontalAlign="Right" />
                <asp:BoundField DataField="OR" HeaderText="OR" SortExpression="OR" ReadOnly="true" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:N}" />
                <asp:BoundField DataField="Cum.OR" HeaderText="Cum.OR" SortExpression="Cum.OR" ReadOnly="true" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:N}" />
                <asp:BoundField DataField="GL" HeaderText="GL" SortExpression="GL" ReadOnly="true" />
                <asp:BoundField DataField="Cur" HeaderText="Cur" SortExpression="Cur" ReadOnly="true" />
                <asp:BoundField DataField="Club" HeaderText="Club" SortExpression="Club" ReadOnly="true" />
                <asp:BoundField DataField="Hub Name" HeaderText="Hub Name" SortExpression="Hub Name" ReadOnly="true" />
                <asp:BoundField DataField="NC Name" HeaderText="NC Name" SortExpression="NC Name" ReadOnly="true" />
                <asp:BoundField DataField="ST" HeaderText="ST" SortExpression="ST" ReadOnly="true" />
            </Columns>--%>
        </asp:GridView>

 </ContentTemplate>
</asp:UpdatePanel>
            
        </div>

    </div>
    </form>
</body>
</html>