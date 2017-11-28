<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="TrainerManager.aspx.cs" Inherits="WebApplication3.TrainerManager" EnableEventValidation="false" %>

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
            });

            $("#gotIt").click(function () {
                $("#loadDiv").removeClass("keep");
                $("#loadDiv").hide();
                $("#errorSpan").removeClass("keep");
                $("#errorSpan").hide();
            });


            jQuery(function ($) {
                var focusedElementId = "";
                var prm = Sys.WebForms.PageRequestManager.getInstance();
                prm.add_beginRequest(function (source, args) {
                    var fe = document.activeElement;
                    if (fe != null)
                    {
                        focusedElementId = fe.id;
                    }

                    else

                    {
                        focusedElementId = "";
                    }
                });
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
              <asp:LinkButton ID="BackBtn" runat="server" Text="Back" PostBackUrl="~/ScheduleTrainer.aspx" ></asp:LinkButton>   
              <a id="add">Add</a>
        </div>
        
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


        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>
      

        <div id="addDiv">
            <div>
                <b>First Name:</b> 
                 <asp:TextBox ID="TxtFName" runat="server" ></asp:TextBox>
                <br />

                <b>Last Name:</b> 
                 <asp:TextBox ID="TxtLname" runat="server" ></asp:TextBox>
                <br />
                                                
                 <b>Is Manager Active?</b> 
                 <asp:checkbox ID="CHKActive" runat="server"  ></asp:checkbox>
                <br />
                <input type="button" id="cancelAdd" value="Cancel" />
                &nbsp;&nbsp;
                <asp:Button ID="addBtn" runat="server" OnClick="addBtn_Click" Text="Add" />

                 &nbsp;&nbsp;
               
            </div>
        </div>

        <div id="notAdd">
                                         
            <br /><br />
        
            <asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Always">
            <ContentTemplate>

                    <asp:GridView CssClass="neoGrid"  ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" 
                        AllowSorting="True" HeaderStyle-Wrap="false" RowStyle-Wrap="false" DataKeyNames="Id" OnRowDataBound="GridView1_RowDataBound">
                        <PagerStyle CssClass="neoPager" />
                        <EmptyDataTemplate>
                            <h4>No results found.</h4>
                        </EmptyDataTemplate>
                        <Columns>                

                            <asp:BoundField DataField="Id" />

                            <asp:TemplateField HeaderText="" ItemStyle-Width="10%">
                                <ItemTemplate>
                                    <asp:LinkButton ID="delLink" runat="server" Text="X" ForeColor="Red" OnClick="delLink_Click" Font-Underline="false"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="FirstName" HeaderText="FirstName" SortExpression="FirstName" ReadOnly="True"  ItemStyle-Width="40%"  />
                            <asp:BoundField DataField="Lastname" HeaderText="Lastname" SortExpression="Lastname" ReadOnly="True"  ItemStyle-Width="40%" />
                
                             <asp:TemplateField HeaderText="Manager Activation" SortExpression="Trainer"  ItemStyle-Width="10%"  >
                                <ItemTemplate>                       
                                    <asp:DropDownList ID="DDLActiveManager" runat="server" DataSourceID="ManagerSQL"  DataValueField="Active" AppendDataBoundItems="true" onselectedindexchanged="DropDownChanged" AutoPostBack="true"     >                                               
                                   </asp:DropDownList> 
                                  <asp:SqlDataSource ID="ManagerSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"   SelectCommand="SELECT 'True' Active UNION ALL SELECT 'False' Active">  </asp:SqlDataSource>
                          
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

             </ContentTemplate>
            </asp:UpdatePanel>
            
        </div>

    </div>
    </form>
</body>
</html>