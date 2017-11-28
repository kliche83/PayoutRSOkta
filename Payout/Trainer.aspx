<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="Trainer.aspx.cs" Inherits="WebApplication3.Trainer" EnableEventValidation="false" %>

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

        $(document).ajaxStart(function () {
            // show loader on start
            $("#loadGif").css("display", "block");
        }).ajaxSuccess(function () {
            // hide loader on success
            $("#loadGif").css("display", "none");
        });

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
              <%--<asp:LinkButton ID="TrainerManager" runat="server" Text="Manager" PostBackUrl="~/TrainerManager.aspx" ></asp:LinkButton>   --%>
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

        <asp:SqlDataSource ID="TrainerManagerSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="SELECT 'Unassigned' Manager UNION ALL SELECT [FirstName] + ' ' + [LastName] Manager FROM [Payout].[dbo].[PAYOUTtrainManager] WHERE [Active] = 1">
        </asp:SqlDataSource>
      
        <asp:SqlDataSource ID="TrainerRoleSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"   
            SelectCommand="SELECT RoleDescription Role FROM [PAYOUTtrainerRoles]">  
        </asp:SqlDataSource>


        <div id="addDiv" class="addDivMaintenance">
            <ul>
                <li>
                    <div>
                        <b>First Name:</b> 
                        <asp:TextBox ID="TxtFName" runat="server" ></asp:TextBox>
                    </div>
                </li>
                <li>
                    <div>
                        <b>Last Name:</b> 
                        <asp:TextBox ID="TxtLname" runat="server" ></asp:TextBox>
                    </div>
                </li>
                <li>
                    <div>
                        <b>Email Address:</b> 
                        <asp:TextBox ID="Txtemail" runat="server" ></asp:TextBox>
                    </div>
                </li>
                <li>
                    <div>
                        <b>Salary:</b> 
                        <asp:TextBox ID="TxtSalary" runat="server" ></asp:TextBox>
                    </div>
                </li>
                <li>
                    <div>
                        <b>Role:</b> 
                        <asp:DropDownList ID="DDLRole" runat="server" DataSourceID="TrainerRoleSQL"  DataValueField="Role" AppendDataBoundItems="true"></asp:DropDownList>
                    </div>
                </li>
                <li>
                    <div>
                        <b>Manager:</b> 
                        <asp:DropDownList ID="DDLManager" runat="server" DataSourceID="TrainerManagerSQL"  DataValueField="Manager" AppendDataBoundItems="true"></asp:DropDownList>
                    </div>
                </li>
                <li>
                    <div>
                        <b>Is Trainer Active?</b> 
                        <asp:checkbox ID="CHKActive" runat="server"  ></asp:checkbox>
                    </div>
                </li>
                <li>
                    <div>
                        <input type="button" id="cancelAdd" value="Cancel" />
                        &nbsp;&nbsp;
                        <asp:Button ID="addBtn" runat="server" OnClick="addBtn_Click" Text="Add" />
                    </div>
                </li>
            </ul>

            <%--<div>
                <b>First Name:</b> 
                 <asp:TextBox ID="TxtFName" runat="server" ></asp:TextBox>
                <br />

                <b>Last Name:</b> 
                 <asp:TextBox ID="TxtLname" runat="server" ></asp:TextBox>
                <br />

                 <b>Email Address:</b> 
                 <asp:TextBox ID="Txtemail" runat="server" ></asp:TextBox>
                <br />

                <b>Salary:</b> 
                 <asp:TextBox ID="TxtSalary" runat="server" ></asp:TextBox>
                <br />

                <b>Role:</b> 
                 <asp:DropDownList ID="DDLRole" runat="server" DataSourceID="TrainerRoleSQL"  DataValueField="Role" AppendDataBoundItems="true"></asp:DropDownList>
                <br />

                <b>Manager:</b> 
                 <asp:DropDownList ID="DDLManager" runat="server" DataSourceID="TrainerManagerSQL"  DataValueField="Manager" AppendDataBoundItems="true"></asp:DropDownList>
                <br />                
                                                
                 <b>Is Trainer Active?</b> 
                 <asp:checkbox ID="CHKActive" runat="server"  ></asp:checkbox>
                <br />
                <input type="button" id="cancelAdd" value="Cancel" />
                &nbsp;&nbsp;
                <asp:Button ID="addBtn" runat="server" OnClick="addBtn_Click" Text="Add" />

                 &nbsp;&nbsp;
               
            </div>--%>
        </div>

        <div id="notAdd">

        
      
     
       First Name: 
        <asp:TextBox ID="Firstname" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
       
       Last Name: 
        <asp:TextBox ID="Lastname" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
      

        <asp:Button ID="searchBTN" runat="server" OnClick="searchBTN_Click" Text="Search" />
        
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

                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton ID="delLink" runat="server" Text="X" ForeColor="Red" OnClick="delLink_Click" Font-Underline="false"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="FirstName" HeaderText="FirstName" SortExpression="FirstName" ReadOnly="True"  ItemStyle-Width="15%"  />
                <asp:BoundField DataField="Lastname" HeaderText="Lastname" SortExpression="Lastname" ReadOnly="True"  ItemStyle-Width="15%" />

             

                 <asp:TemplateField HeaderText="Email Address" SortExpression="Lastname"   ItemStyle-Width="20%" >
                    <ItemTemplate>
                        <asp:TextBox ID="EmailAddress" runat="server" Text='<%# Bind("[EmailAddress]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                 
                <asp:TemplateField HeaderText="Salary" SortExpression="Salary" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Right" >
                    <ItemTemplate>
                        <asp:TextBox ID="Salary" runat="server" Text='<%# Bind("[Salary]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Role" SortExpression="Role"  ItemStyle-Width="15%"  >
                    <ItemTemplate>                       
                        <asp:DropDownList ID="DropDownRole" runat="server" DataSourceID="TrainerRoleSQL"  DataValueField="Role" AppendDataBoundItems="true" onselectedindexchanged="DropDownChanged" AutoPostBack="true">
                       </asp:DropDownList>                       
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Manager" SortExpression="Role"  ItemStyle-Width="15%"  >
                    <ItemTemplate>
                        <asp:DropDownList ID="DropDownManager" runat="server" DataSourceID="TrainerManagerSQL"  DataValueField="Manager" AppendDataBoundItems="true" onselectedindexchanged="DropDownChanged" AutoPostBack="true">
                       </asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>

                 <asp:TemplateField HeaderText="Trainer Activation" SortExpression="Trainer"  ItemStyle-Width="15%"  >
                    <ItemTemplate>
                       <%--<asp:DropDownList ID="DropDownTrainer" runat="server" DataSourceID="TrainerSQL" SelectedValue='<%# Eval("Active") %>'   AppendDataBoundItems="true"  DataValueField="Active"    onselectedindexchanged="DropDownChanged" AutoPostBack="true"     > --%>
                        <asp:DropDownList ID="DropDownTrainer" runat="server" DataSourceID="TrainerSQL"  DataValueField="Active" AppendDataBoundItems="true" onselectedindexchanged="DropDownChanged" AutoPostBack="true"     > 
                                              
                       </asp:DropDownList> 
                      <asp:SqlDataSource ID="TrainerSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"   SelectCommand="SELECT 'True' Active UNION ALL SELECT 'False' Active /*select distinct active from [PAYOUTtrainer] */">  </asp:SqlDataSource>
                          
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