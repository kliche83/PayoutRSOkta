<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="SalesFile.aspx.cs" Inherits="WebApplication3.SalesFile" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Sales File</title>
       <link href="Content/CalendarStyle.css" rel="stylesheet" type="text/css" />
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />

    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js" type="text/javascript"></script>
    <script src="//code.jquery.com/ui/1.11.2/jquery-ui.js" type="text/javascript"></script>
    

    <script type="text/javascript">

        $(document).ready(function () {

            $("#loadDiv").not(".keep").hide();
            $("#errorSpan").not(".keep").hide();
            $("#loadGif").hide();

            var prm = Sys.WebForms.PageRequestManager.getInstance();

            prm.add_beginRequest(function (source, args) {
                $("#loadDiv").show();
                $("#loadGif").show();
            });

            prm.add_endRequest(function (source, args) {                
                $("#loadDiv").not(".keep").hide();
                $("#errorSpan").not(".keep").hide();
                $("#loadGif").hide();

                $(function () {
                    $(".Date").datepicker();
                });
            });

            $(function () {
                $(".Date").datepicker();
            });
            
            if ($('input:text:first').val() == "")
            {
                $('input:text:first').focus();
            }

        });

        var specialKeys = new Array();
        specialKeys.push(8); //Backspace
        function IsNumeric(e) {
            var keyCode = e.which ? e.which : e.keyCode
            var ret = ((keyCode >= 48 && keyCode <= 57) || specialKeys.indexOf(keyCode) != -1);
            document.getElementById("error").style.display = ret ? "none" : "inline";
            return ret;
        }

    </script>

</head>
<body>
    <form id="form1" runat="server">
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>

        <div id="loadDiv" runat="server" style="position: absolute; top: 0; bottom: 0; left: 0; right: 0; background-color: rgbA(255, 255, 255, 0.8); z-index: 9999; overflow: hidden; text-align: center; padding: 120px 10px 10px 10px;">
            <span id="errorSpan" runat="server">
                <asp:Label ID="errorMsg" runat="server" ForeColor="Black" Font-Bold="true" Font-Size="Medium"></asp:Label>
                <br /><br />
                <input id="gotIt" runat="server" value="OK" style="padding: 10px; border: 0; background-color: #2764AB; color: #fff; cursor: pointer; width: 50px!important;" />
                <br />
            </span>

            <img id="loadGif" runat="server" src="../Content/load.gif" />
        </div>

        <asp:UpdatePanel ID="upd1" runat="server" UpdateMode="Always">
            <ContentTemplate>  

                <div>                    
                    Date From:        
                    <asp:TextBox ID="DateFromTXT" runat="server" CssClass="Date" style="width: 170px;" AutoPostBack="true" OnTextChanged="TXT_TextChanged"></asp:TextBox> &nbsp;         
                    <asp:FilteredTextBoxExtender ID="DateFromTXT_Filter" TargetControlID="DateFromTXT" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>                
                </div>
                &nbsp;

                <div>
                    Date To: 
                    <asp:TextBox ID="DateToTXT" runat="server" CssClass="Date" style="width: 170px;" AutoPostBack="true" OnTextChanged="TXT_TextChanged"></asp:TextBox> &nbsp;         
                    <asp:FilteredTextBoxExtender ID="DateToTXT_Filter" TargetControlID="DateToTXT" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>                        
                </div>
                &nbsp; 
                
                <div>
                    Retailer: 
                    <asp:DropDownList ID="RetailerDDL" runat="server" style="width: 110px;" AppendDataBoundItems="false" AutoPostBack="true" OnSelectedIndexChanged="DDL_SelectedIndexChanged">
                    </asp:DropDownList> 
                </div>
                &nbsp; 

                <div>
                    Program: 
                    <asp:DropDownList ID="ProgramDDL" runat="server" style="width: 110px;" AppendDataBoundItems="false" AutoPostBack="true" OnSelectedIndexChanged="DDL_SelectedIndexChanged">
                    </asp:DropDownList>  
                </div>
                &nbsp;
            
                <div>
                    Store Number: 
                    <asp:TextBox ID="StoreNumberTXT" runat="server" style="width: 170px;" AutoPostBack="true" OnTextChanged="TXT_TextChanged" onkeypress="return IsNumeric(event);" ondrop="return false;" onpaste="return false;"></asp:TextBox> &nbsp;
                    <span id="error" style="color: Red; display: none">* Input digits (0 - 9)</span>
                </div>
                &nbsp;

                <div>
                    Item Number: 
                    <asp:TextBox ID="ItemNumberTXT" runat="server" style="width: 170px;" AutoPostBack="true" OnTextChanged="TXT_TextChanged"></asp:TextBox>
                </div>
                &nbsp;

                <div>
                    <asp:Label ID="CountRows" runat="server" Font-Bold="true"></asp:Label>
                </div>
                &nbsp;

                <div>
                    <asp:Label ID="ValMessage" runat="server" Font-Bold="true" CssClass="ValidationMessage"></asp:Label>
                </div>
                &nbsp;

            </ContentTemplate>
        </asp:UpdatePanel> 

        <div>
            <asp:Button ID="ResetFilters" Text="Reset Filters" runat="server" OnClick="ResetFilters_Click" />            
            <asp:Button ID="ExportBtn" Text="Export File" runat="server" OnClick="ExportBtn_Click" OnClientClick="this.disabled=true;" UseSubmitBehavior="false"/>
        </div>
        &nbsp;

    </form>
</body>
</html>