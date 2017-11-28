<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="People.aspx.cs" Inherits="WebApplication3.People" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | People</title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#addDiv").hide();
            $("#renameDiv").hide();

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

            $("#<%= missing.ClientID %>").click(function (e) {
                $(this).fadeOut();
            });

            $("input[type=text]").click(function (e) {
                $(this).select();
            });

            $("input[type=text]").focus(function (e) {
                $(this).select();
            });

            $("#whoDDL").change(function (e) {
                $("#renF").val($("#whoDDL").val().split(' ').slice(0, -1).join(' '));
                $("#renL").val($("#whoDDL").val().split(' ').slice(-1).join(' '));
            });

            $(".mnLink").click(function () {
                var id = "#" + $(this).attr("id") + "Div";
                $(this).toggleClass("menuActive");
                if ($("#notAdd").offset().top == 50) {
                    $("#notAdd").animate({ top: 240 }, 300, function () {
                        $(id).fadeIn();
                    });
                }
                else {
                    $("#addDiv").hide();
                    $("#renameDiv").hide();
                    $(".mnLink").removeClass("menuActive");
                    $("#notAdd").animate({ top: 50 }, 300);
                }
            });

            $("#cancelAdd").click(function () {
                $("#add").removeClass("menuActive");
                $("#addDiv").hide();
                $("#notAdd").animate({ top: 50 }, 300);
            });

            $("#cancelRename").click(function () {
                $("#rename").removeClass("menuActive");
                $("#renameDiv").hide();
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
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="menu">
            <a class="mnLink" id="add">Add Email</a>
            <a class="mnLink" id="rename">Rename Someone</a>
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>

            
        </div>

        <div id="addDiv">
            <div>
                <b>Firstname:</b> <asp:TextBox ID="EFN" runat="server" width="200px"></asp:TextBox>
                <br />
                <b>Lastname:</b> <asp:TextBox ID="ELN" runat="server" width="200px"></asp:TextBox>
                <br />
                <b>Email:</b> <asp:TextBox ID="EML" runat="server" width="200px"></asp:TextBox>
                <br />
                <input type="button" id="cancelAdd" value="Cancel" />
                &nbsp;&nbsp;
                <asp:Button ID="addBtn" runat="server" OnClick="addBtn_Click" Text="Add" />
            </div>
        </div>

        <asp:SqlDataSource ID="whoSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="SELECT [Firstname] + ' ' + [Lastname] as [Name] FROM [PAYOUTpeople] GROUP BY [Firstname], [Lastname] ORDER BY [Firstname], [Lastname]">
        </asp:SqlDataSource>

        <div id="renameDiv">
            <div>
                <b>Who?</b> 
                <asp:DropDownList ID="whoDDL" runat="server" style="width: 212px;" DataSourceID="whoSQL" DataValueField="Name" DataTextField="Name" AppendDataBoundItems="false">
                </asp:DropDownList>
                <br />
                <b>New Firstname:</b> <asp:TextBox ID="renF" runat="server" width="200px"></asp:TextBox>
                <br />
                <b>New Lastname:</b> <asp:TextBox ID="renL" runat="server" width="200px"></asp:TextBox>
                <br />
                <input type="button" id="cancelRename" value="Cancel" />
                &nbsp;&nbsp;
                <asp:Button ID="renameBtn" runat="server" OnClick="renameBtn_Click" Text="Rename" />
            </div>
        </div>

        <div id="missing" runat="server">
            <asp:Label runat="server" ID="missingL" Text="Missing Owners from Email List"></asp:Label>
        </div>
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>
        
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>
        
        <div id="notAdd">

        Name: 
        <asp:TextBox ID="nameTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        <%--PS ID: 
        <asp:TextBox ID="psTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        Email: 
        <asp:TextBox ID="emailTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp;--%> 
        
        <asp:Button ID="searchBTN" runat="server" OnClick="searchBTN_Click" Text="Search" />
        
        <br /><br />

<asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Always">
<ContentTemplate>
        <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" AllowPaging="True" PageSize="15" 
            AllowSorting="True" HeaderStyle-Wrap="false" RowStyle-Wrap="false" DataKeyNames="Id" OnRowDataBound="GridView1_RowDataBound">
            <PagerStyle CssClass="neoPager" />
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="Id" />
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton ID="delLink0" runat="server" Text="X" ForeColor="Red" OnClick="delLink_Click" Font-Underline="false"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Firstname" HeaderText="Firstname" SortExpression="Firstname" ReadOnly="True" />
                <asp:BoundField DataField="Lastname" HeaderText="Lastname" SortExpression="Lastname" ReadOnly="True" />
                <%--<asp:TemplateField HeaderText="Firstname" SortExpression="Firstname">
                    <ItemTemplate>
                        <asp:TextBox ID="Firstname" runat="server" Text='<%# Bind("[Firstname]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Lastname" SortExpression="Lastname">
                    <ItemTemplate>
                        <asp:TextBox ID="Lastname" runat="server" Text='<%# Bind("[Lastname]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:TemplateField HeaderText="PS ID" SortExpression="PSID">
                    <ItemTemplate>
                        <asp:TextBox ID="PSID" runat="server" Text='<%# Bind("[PSID]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="2nd ID" SortExpression="PSID2">
                    <ItemTemplate>
                        <asp:TextBox ID="PSID2" runat="server" Text='<%# Bind("[PSID2]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Email" SortExpression="Email">
                    <ItemTemplate>
                        <asp:TextBox ID="Email" runat="server" Text='<%# Bind("[Email]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    
        <asp:GridView CssClass="neoGrid" ID="GridView2" runat="server" AutoGenerateColumns="false" DataSourceID="SqlDataSource1" AllowPaging="false" PageSize="15" 
            AllowSorting="False" HeaderStyle-Wrap="false" RowStyle-Wrap="false" style="display: none;">
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="Firstname" HeaderText="Firstname" SortExpression="Firstname" ReadOnly="true" />
                <asp:BoundField DataField="Lastname" HeaderText="Lastname" SortExpression="Lastname" ReadOnly="true" />
                <asp:BoundField DataField="PSID" HeaderText="PS ID" SortExpression="PSID" ReadOnly="True" />
                <asp:BoundField DataField="PSID2" HeaderText="2nd ID" SortExpression="PSID2" ReadOnly="True" />
                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" ReadOnly="true" />
            </Columns>
        </asp:GridView>

 </ContentTemplate>
</asp:UpdatePanel>

            </div>

    </div>
    </form>
</body>
</html>
