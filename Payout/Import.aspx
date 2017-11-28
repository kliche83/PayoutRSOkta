<%@ Page Language="C#" AutoEventWireup="true" Inherits="Import" Codebehind="Import.aspx.cs" enableEventValidation="false" %>    
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Payout | Import Data</title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css"/>
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script src="//code.jquery.com/ui/1.11.2/jquery-ui.js"></script>

    <script type="text/javascript">

        $(document).ready(function () {

           $('#saveImportBtn').click(function () {
                
                if ($('#HFIsSchedule')[0].value == "true") {
                    var clickButton = document.getElementById("<%= btnsaveSchedule.ClientID %>");
                    clickButton.click();
                }
                else {
                    $dialog.dialog("open");
                }
            });

            
            var $dialog = $("#dialog-form").dialog({
                autoOpen: false,
                height: 200,
                width: 350,
                modal: true,
                open: function (type, data) {
                    $(this).parent().appendTo($("form:first"));
                },
                buttons: {
                    "Submit": SubmitServerButton,                    
                    Cancel: function () {
                        $dialog.dialog("close");
                    }
                }
            });

            function SubmitServerButton()
            {
            var clickButton = document.getElementById("<%= btnUploadOriginal.ClientID %>");
                clickButton.click();
            }


            function ServerPost(CheckDate) {
                var IsSchedule = false;

                if ($('#TableDDL :selected').text() == "Schedule")
                {
                    IsSchedule = true;
                }

                
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "Import.aspx/JSONRequestFromClient",
                    dataType: "json",
                    data: "{'Path': '" + FileUploadOriginalFile.value.replace(/\\/g, '?') + "', 'IsSchedule': " + IsSchedule + "}",
                    success: function (data) {
                        alert(data.d);
                        window.location = "Import.aspx";
                    },
                    error: function (request, status, error) {
                        alert(request.responseText);
                    }
                });
            }
        });
        


        //function undo() {
        //    var undo_confirm_value = document.createElement("INPUT");
        //    undo_confirm_value.type = "hidden";
        //    undo_confirm_value.name = "undo_confirm_value";
        //    if (confirm("Do you want to undo your import? This can not be undone.")) {
        //        undo_confirm_value.value = "Yes";
        //    } else {
        //        undo_confirm_value.value = "No";
        //        window.location = 'Import.aspx';
        //    }
        //    document.forms[0].appendChild(undo_confirm_value);
        //}

        //function save() {
        //    var save_confirm_value = document.createElement("INPUT");
        //    save_confirm_value.type = "hidden";
        //    save_confirm_value.name = "save_confirm_value";
        //    if (confirm("Do you want to save your import? This can not be undone.")) {
        //        save_confirm_value.value = "Yes";
        //    } else {
        //        save_confirm_value.value = "No";
        //        window.location = 'Import.aspx';
        //    }
        //    document.forms[0].appendChild(save_confirm_value);
        //}
    </script>
</head>
<body>
    <form id="form1" runat="server">
                
        <asp:HiddenField ID="HFIsSchedule" runat="server" />
        <div id="dialog-form" title="Original file is required">
            <p class="validateTips">Please upload the original file</p>             
            <button id="button" style="display:none">close</button>        
            <input type="submit" tabindex="-1" style="position:absolute; top:-1000px" value="Submit" />            
            <asp:FileUpload ID="FileUploadOriginalFile" runat="server"/>
            <asp:Button ID="btnUploadOriginal" runat="server" Text="Upload" OnClick="btnUploadOriginal_Click" CssClass="HideControl" />
        </div>  


        <div id="menu" style="z-index: 999;">
            
        </div>
        
        <br /><br />

    <asp:Panel ID="Panel1" runat="server">
        <asp:FileUpload ID="FileUpload1" runat="server" class="bar" />

        <div runat="server" id="uploadOptions">
            <span id="fileNameSpan"></span>
            <br /><br />            
            <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" CssClass="uploadBtn" />
            <input type="button" id="cancelBtn" value="Cancel" class="uploadBtn" />
            
        </div>
       <div>            
            <asp:Button ID="SyncShedule" runat="server" Text="Sync Schedule" OnClick="btnSync_Click" CssClass="uploadBtn" />
            <br />
            <asp:label ID="lblLoadingMessage" runat="server" CssClass="ValidationMessage" /> 
        </div>
        <div runat="server" id="importOptions">            
            <input id="saveImportBtn" type="button", value ="Save Import" class="uploadBtn" />
            <asp:Button ID="btnsaveSchedule" runat="server" Text="Save Import" OnClick="btnsaveSchedule_Click" CssClass="HideControl" />
            <asp:Button ID="undolImportBtn" runat="server" Text="Undo Import" OnClick="undoImportBtn_Click" CssClass="uploadBtn" />
        </div>

        <br /><br />
        <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
    </asp:Panel>
    <asp:Panel ID="Panel2" runat="server" Visible = "false">
        <span>            
        <asp:Label ID="Label5" runat="server" Text="File Name: "/>
        <asp:Label ID="lblFileName" runat="server" Text="" Font-Bold="true" style="margin-left: 10px;" />
        <br /><br /><br />
        </span>
        <div style="clear: both;"></div>
        <asp:Label ID="Label2" runat="server" Text="Sheet" />
        <asp:DropDownList ID="ddlSheets" runat="server" AppendDataBoundItems = "true">
        </asp:DropDownList>
        <br /><br />
        <asp:Label ID="Label3" runat="server" Text="Import Type "/>
        <asp:DropDownList ID="TableDDL" runat="server"> <%--AppendDataBoundItems="true" DataSourceID="storeSQL" DataValueField="StoreName" DataTextField="StoreName">--%>
            <asp:ListItem Value="Schedule" Selected="True">Schedule</asp:ListItem>
            <asp:ListItem Value="BJs">BJ's</asp:ListItem>
            <asp:ListItem Value="CanadianTire">Canadian Tire</asp:ListItem>
            <asp:ListItem Value="Costco">Costco</asp:ListItem>
            <asp:ListItem Value="HEB">HEB</asp:ListItem>
            <asp:ListItem Value="Kroger">Kroger</asp:ListItem>
            <asp:ListItem Value="KrogerC">Kroger C</asp:ListItem>
            <asp:ListItem Value="Meijer">Meijer</asp:ListItem>
            <asp:ListItem Value="Sams">Sam's</asp:ListItem>
            <asp:ListItem Value="WalMart">WalMart</asp:ListItem>
            <asp:ListItem Value="WinnDixie">WinnDixie</asp:ListItem>
            <asp:ListItem Value="SamCreditCard">Sam's Credit Card</asp:ListItem>
            <asp:ListItem Value="SamCypressCreek">Sam's Cypress Creek</asp:ListItem>
            
        </asp:DropDownList>
        <%--<br /><br />
        <asp:Label ID="Label4" runat="server" Text="Program "/>
        <asp:DropDownList ID="progDDL" runat="server" AppendDataBoundItems="true" DataSourceID="progSQL" DataValueField="Program" DataTextField="Program">
        </asp:DropDownList>--%>
        <br /><br />
        <span>
        <asp:Label ID="Label1" runat="server" Text="Has Header Row?"></asp:Label>
        <br />
        </span>
        <asp:RadioButtonList ID="rbHDR" runat="server" BorderStyle="none">
            <asp:ListItem Text = "Yes" Value = "Yes" Selected = "True" ></asp:ListItem>
            <asp:ListItem Text = "No" Value = "No"></asp:ListItem>
        </asp:RadioButtonList>
        <br />
        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />    
     </asp:Panel>

        <img id="progress" src="Content/load.gif" style="margin-left: 240px;" /> 

        <br /><br />
        
        <asp:SqlDataSource ID="progSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
            SelectCommand="SELECT Program FROM PAYOUTschedule GROUP BY Program ORDER BY Program">
        </asp:SqlDataSource>
        
        
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="" OnSelecting="SqlDataSource1_Selecting"></asp:SqlDataSource>

        <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server" AutoGenerateColumns="True" DataSourceID="SqlDataSource1">
        </asp:GridView>
        
    <script type="text/javascript">
        $(document).ready(function () {
            $("#progress").hide();
            $("#<%= uploadOptions.ClientID %>").hide();
            <%--$("#<%= progDDL.ClientID %>").hide();
            $("#<%= Label4.ClientID %>").hide();--%>

            if ($("#<%= importOptions.ClientID %>").is(":visible"))
            {
                $('#<%= FileUpload1.ClientID %>').hide();
            }

            $("#<%= FileUpload1.ClientID %>").on('change', function (e) {
                if (e.target.files.length) {
                    $('#<%= FileUpload1.ClientID %>').hide();
                    $('#fileNameSpan').html("Filename: <b>" + $('#<%= FileUpload1.ClientID %>').val().replace(/C:\\fakepath\\/i, '') + "</b>");
                    $("#<%= uploadOptions.ClientID %>").fadeIn();
                }
            });

            $("#cancelBtn").click(function (e) {
                $('#<%= uploadOptions.ClientID %>').hide();
                $("#<%= FileUpload1.ClientID %>").val("");
                $("#<%= FileUpload1.ClientID %>").fadeIn();
            });

            $("#<%= btnSave.ClientID %>").click(function (e) {
                $("#<%= btnSave.ClientID %>").hide();
                $("#<%= btnCancel.ClientID %>").hide();
                $("#progress").fadeIn();
            });

        });
    </script>

    </form>
</body>
</html>
