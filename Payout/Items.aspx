<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Items.aspx.cs" Inherits="Payout.Items" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Items</title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script> 
    <script type="text/javascript">
        $(document).ready(function () {
            $("#addDiv").hide();

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

            $("input[type=text]").click(function (e) {
                $(this).select();
            });

            $("input[type=text]").focus(function (e) {
                $(this).select();
            });

            $("#add").click(function () {
                $(this).toggleClass("menuActive");
                if ($("#notAdd").offset().top == 50)
                {
                    $("#notAdd").animate({ top: 240 }, 300, function ()
                    { $("#addDiv").fadeIn(); });
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
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="menu">
            <a id="add">Add Item</a>
            <asp:LinkButton ID="exportSales" runat="server" onclick="exportSales_Click" Text="Export to Excel" />

            
        </div>

        <asp:SqlDataSource ID="SQLprogram" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="SELECT * FROM (SELECT [Program] FROM [PAYOUTitemMaster] UNION All SELECT [Program] FROM [PAYOUTschedule])X WHERE Program != '' AND Program IS NOT NULL group by [Program] ORDER BY [Program]">
        </asp:SqlDataSource>

        <div id="addDiv">
            <div>
                <b>Store Name:</b> 
                <asp:DropDownList ID="storeDDL" runat="server" Width="162px">
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem Value="BJs">BJ's</asp:ListItem>
                    <asp:ListItem Value="CanadianTire">Canadian Tire</asp:ListItem>
                    <asp:ListItem Value="Costco">Costco</asp:ListItem>
                    <asp:ListItem Value="HEB">HEB</asp:ListItem>
                    <asp:ListItem Value="Kroger">Kroger</asp:ListItem>
                    <asp:ListItem Value="Meijer">Meijer</asp:ListItem>
                    <asp:ListItem Value="Sams">Sam's</asp:ListItem>
                    <asp:ListItem Value="WalMart">WalMart</asp:ListItem>
                    <asp:ListItem Value="WinnDixie">WinnDixie</asp:ListItem>
                    
                </asp:DropDownList>
                <br />
                <b>Item Number:</b> <asp:TextBox ID="INO" runat="server" width="150px"></asp:TextBox>
                <br />
                <b>JDE Number:</b> <asp:TextBox ID="JNO" runat="server" width="150px"></asp:TextBox>
                <br />
                <b>Item Name:</b> <asp:TextBox ID="INA" runat="server" width="150px"></asp:TextBox>&nbsp;
            </div>
            <div>
                <b>Unit Cost:</b> <asp:TextBox ID="IUC" runat="server" width="150px"></asp:TextBox>
                <br />
                <b>Program:</b> 
                <asp:DropDownList ID="progDDL" runat="server" DataSourceID="SQLprogram" AppendDataBoundItems="true" DataTextField="Program" DataValueField="Program" Width="162px">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <br />
                <input type="button" id="cancelAdd" value="Cancel" />
                &nbsp;&nbsp;
                <asp:Button ID="addBtn" runat="server" OnClick="addBtn_Click" Text="Add" />
            </div>
        </div>
        
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>
        
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            SelectCommand="">
        </asp:SqlDataSource>
        
        <div id="notAdd">
            
        Store:
        <asp:DropDownList ID="StoreNameDDL" runat="server" Width="150px">
            <asp:ListItem></asp:ListItem>
            <asp:ListItem Value="BJs">BJ's</asp:ListItem>
            <asp:ListItem Value="CanadianTire">Canadian Tire</asp:ListItem>
            <asp:ListItem Value="Costco">Costco</asp:ListItem>
            <asp:ListItem Value="HEB">HEB</asp:ListItem>
            <asp:ListItem Value="Kroger">Kroger</asp:ListItem>
            <asp:ListItem Value="Meijer">Meijer</asp:ListItem>
            <asp:ListItem Value="Sams">Sam's</asp:ListItem>
            <asp:ListItem Value="WalMart">WalMart</asp:ListItem>
            <asp:ListItem Value="WinnDixie">WinnDixie</asp:ListItem>
        
        </asp:DropDownList> &nbsp; 
        Program: 
        <asp:DropDownList ID="ProgramDDL" runat="server" DataSourceID="SQLprogram" AppendDataBoundItems="true" DataTextField="Program" DataValueField="Program" Width="150px">
            <asp:ListItem></asp:ListItem>
        </asp:DropDownList> &nbsp; 

        Item Number: 
        <asp:TextBox ID="ItemNumberTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
        <asp:FilteredTextBoxExtender ID="ItemNumberTXT_Filter" TargetControlID="ItemNumberTXT" runat="server" FilterMode="ValidChars" ValidChars="1234567890"></asp:FilteredTextBoxExtender> &nbsp; 
        Item Name: 
        <asp:TextBox ID="ItemNameTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
       

       JDE Number:
            <asp:TextBox ID="ItemJDENumberTXT" runat="server" style="width: 100px;"></asp:TextBox>
            <asp:FilteredTextBoxExtender ID="ItemJdeTXT_Filter" TargetControlID="ItemJDENumberTXT" runat="server" FilterMode="ValidChars" ValidChars="1234567890"></asp:FilteredTextBoxExtender> &nbsp; 

       Status:
         <asp:DropDownList ID="DDLStatus" runat="server"  Width="150px">
             <asp:ListItem></asp:ListItem>
            <asp:ListItem>Active</asp:ListItem>
              <asp:ListItem>Obsolete</asp:ListItem>
        </asp:DropDownList> &nbsp; 

        <asp:Button ID="searchBTN" runat="server" OnClick="searchBTN_Click" Text="Search" />
        
        <br /><br />

<asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Always">
<ContentTemplate>
    <asp:GridView CssClass="neoGrid" ID="GridView1" runat="server" AutoGenerateColumns="false" DataSourceID="SqlDataSource1" AllowPaging="True" PageSize="15" HeaderStyle-Wrap="false" RowStyle-Wrap="false" OnRowDataBound="GridView1_RowDataBound">
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
                <asp:TemplateField HeaderText="Store Name" SortExpression="StoreName">
                    <ItemTemplate>
                        <asp:DropDownList ID="StoreName" runat="server" SelectedValue='<%# Bind("[StoreName]") %>' AutoPostBack="true" OnSelectedIndexChanged="DropChanged" Width="200px" style="border: 0;">
                            <asp:ListItem></asp:ListItem>
                            <asp:ListItem Value="BJs">BJ's</asp:ListItem>
                            <asp:ListItem Value="CanadianTire">Canadian Tire</asp:ListItem>
                            <asp:ListItem Value="Costco">Costco</asp:ListItem>
                            <asp:ListItem Value="HEB">HEB</asp:ListItem>
                            <asp:ListItem Value="Kroger">Kroger</asp:ListItem>
                            <asp:ListItem Value="Meijer">Meijer</asp:ListItem>
                            <asp:ListItem Value="Sams">Sam's</asp:ListItem>
                            <asp:ListItem Value="WalMart">WalMart</asp:ListItem>
                            <asp:ListItem Value="WinnDixie">WinnDixie</asp:ListItem>
                            
                        </asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Vendor Item Number" SortExpression="ItemNumber">
                    <ItemTemplate>
                        <asp:TextBox ID="ItemNumber" runat="server" Text='<%# Bind("[ItemNumber]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="JDE Number" SortExpression="JDENumber">
                    <ItemTemplate>
                        <asp:TextBox ID="JDENumber" runat="server" Text='<%# Bind("[JDENumber]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Item Name" SortExpression="ItemName">
                    <ItemTemplate>
                        <asp:TextBox ID="ItemName" runat="server" Text='<%# Bind("[ItemName]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Retail Price" SortExpression="UnitCost">
                    <ItemTemplate>
                        <asp:TextBox ID="UnitCost" runat="server" Text='<%# Bind("[UnitCost]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                 <asp:TemplateField HeaderText="Status" SortExpression="Status">
                    <ItemTemplate>
                         <asp:DropDownList ID="Status" runat="server" SelectedValue='<%# Bind("[Status]") %>' AutoPostBack="true" OnSelectedIndexChanged="DropChanged" >
                            <asp:ListItem></asp:ListItem>
                            <asp:ListItem Value="Active">Active</asp:ListItem>
                            <asp:ListItem Value="Obsolete">Obsolete</asp:ListItem>
                          </asp:DropDownList>
                     </ItemTemplate>
                </asp:TemplateField>
                 <asp:TemplateField HeaderText="Vendor Cost" SortExpression="Status">
                    <ItemTemplate>
                        <asp:TextBox ID="VendorCost" runat="server" Text='<%# Bind("[VendorCost]") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Effective Date" SortExpression="Status">
                    <ItemTemplate>
                        <asp:TextBox ID="EffectiveDate" runat="server" Text='<%# Bind("[EffectiveDate]","{0:dd/MM/yyyy}") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>
                        <asp:calendarextender id="CalendarExtender1" runat="server" targetcontrolid="EffectiveDate"  Format="dd/MM/yyyy"/>
                     </ItemTemplate>
                </asp:TemplateField>

                    <asp:BoundField DataField="Username" HeaderText="UpdatedByUser"  ReadOnly="True" />

                <asp:BoundField DataField="Program" HeaderText="Program" SortExpression="Program" ReadOnly="True" />

                <asp:TemplateField HeaderText="Assign New Program" SortExpression="Program">
                    <ItemTemplate>
                        <asp:DropDownList ID="Program" runat="server" DataSourceID="SQLprogram" AppendDataBoundItems="true" DataTextField="Program" DataValueField="Program" OnSelectedIndexChanged="DropChanged" AutoPostBack="true" Width="200px" style="border: 0;">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
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
