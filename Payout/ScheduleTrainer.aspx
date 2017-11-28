<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="ScheduleTrainer.aspx.cs" Inherits="WebApplication3.ScheduleTrainer" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | Schedule</title>
       <link href="Content/CalendarStyle.css" rel="stylesheet" type="text/css" />
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />

    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js" type="text/javascript"></script>
    <script src="//code.jquery.com/ui/1.11.2/jquery-ui.js" type="text/javascript"></script>
    <script src="Scripts/scripts.js" type="text/javascript"></script>
    <style>
        #Logout {
            float: right;            
        }
    </style>

     <script type="text/javascript">
         
         ScheduleTrainerScripts();
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
           
            
            <asp:LinkButton ID="ManagerBtn" runat="server" Text="Managers" PostBackUrl="~/TrainerManager.aspx" ></asp:LinkButton>
            <asp:LinkButton ID="TrainerBtn" runat="server" Text="Trainers" PostBackUrl="~/Trainer.aspx" ></asp:LinkButton>
            <asp:LinkButton ID="SetupCommissionBtn" runat="server" Text="Commission Setup" PostBackUrl="~/SetupRoleCommission.aspx"></asp:LinkButton>
            <asp:LinkButton ID="SetupOverrideBtn" runat="server" Text="Override Setup" PostBackUrl="~/SetupRoleOverride.aspx"></asp:LinkButton>                        
            <asp:LinkButton ID="CumulativeSummary" runat="server" Text="Cumulative PM Summary" PostBackUrl="~/PMCumulativeSummary.aspx" ></asp:LinkButton>
            <asp:LinkButton ID="GridRoleCommission" runat="server" Text="Commission Summary" PostBackUrl="~/GridRoleCommission.aspx" style="display:none"  ></asp:LinkButton>
            <asp:LinkButton ID="GridRoleOverride" runat="server" Text="Override Summary" PostBackUrl="~/GridRoleOverride.aspx" style="display:none"></asp:LinkButton>
            <%--<asp:LinkButton ID="GridRoleCommission" runat="server" Text="Commission Summary" PostBackUrl="~/GridRoleCommission.aspx"  ></asp:LinkButton>
            <asp:LinkButton ID="GridRoleOverride" runat="server" Text="Override Summary" PostBackUrl="~/GridRoleOverride.aspx"></asp:LinkButton>--%>
            <asp:LinkButton ID="ListOutBtn" runat="server" Text="Sales Reports" PostBackUrl="~/GridTrainer.aspx" style="display:none" ></asp:LinkButton>
            <asp:LinkButton ID="exportBtn" runat="server" Text="Export to Excel" OnClick="exportBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="Logout" runat="server" Text="Logout" OnClick="Btn_Logout"></asp:LinkButton>
            
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

        <input id="PostbackButton" type="submit" style="display:none" />
                
             <div id="addDivClosed" class="addDivCls">            
                <asp:TextBox ID="HiddenIdTextBox" runat="server"></asp:TextBox>
                <table class="neoGrid">
                    <thead>
                        <tr>
                            <th>Program</th>
                            <th>Start Date</th>
                            <th>End Date</th>
                            <th>Store Name</th>
                            <th>Store Number</th>
                            <th>City</th>
                            <th>State</th>
                            <th>Owner</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><asp:TextBox ID="txtProgram" runat="server" Enabled="false" ></asp:TextBox></td>
                            <td><asp:TextBox ID="txtStartDate" runat="server" Enabled="false" ></asp:TextBox></td>
                            <td><asp:TextBox ID="txtEndDate" runat="server" Enabled="false" ></asp:TextBox></td>
                            <td><asp:TextBox ID="txtStoreName" runat="server" Enabled="false" ></asp:TextBox></td>
                            <td><asp:TextBox ID="txtStoreNumber" runat="server" Enabled="false" ></asp:TextBox></td>
                            <td><asp:TextBox ID="txtCity" runat="server" Enabled="false" ></asp:TextBox></td>
                            <td><asp:TextBox ID="txtState" runat="server" Enabled="false" ></asp:TextBox></td>
                            <td><asp:TextBox ID="txtOwner" runat="server" Enabled="false" ></asp:TextBox></td>
                        </tr>
                    </tbody>
                </table>
                <br />

                <div>
                    <b>Train Start Date:</b>
                    <asp:TextBox ID="TrainStartDate_Ins" runat="server" CssClass="Date" Enabled="false" ></asp:TextBox>
                    <asp:FilteredTextBoxExtender ID="TrainStartDate_Filter" TargetControlID="TrainStartDate_Ins" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>
                    &nbsp; 
                    <b>Train End Date:</b> 
                    <asp:TextBox ID="TrainEndDate_Ins" runat="server" CssClass="Date" Enabled="false" ></asp:TextBox>
                    <asp:FilteredTextBoxExtender ID="TrainEndDate_Filter" TargetControlID="TrainEndDate_Ins" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>
                    &nbsp; 
                    <b>Trainer:</b> 
                    <asp:DropDownList ID="trainerDDL_Ins" runat="server" Enabled="false"></asp:DropDownList>
                </div>
                    <%--<input id="InsertTrainerBtn" type="button" value="Insert Record" disabled="disabled"/>--%>
                    <asp:Button ID="InsertTrainerBtn" runat="server" Text="Insert Record" OnClick="BtnInsert_Click"/>
                    <input id="ClosePanelBtn" type="button" value="Close Panel"/>
            </div>
       

        <div id="notAdd">        
            Owner: 
            <asp:TextBox ID="ownerTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 

            Trainer: 
            <asp:DropDownList ID="DDTrainer" runat="server"></asp:DropDownList>
           <%--<asp:DropDownList ID="DDTrainer" runat="server" DataSourceID="TrainerSQL"    DataValueField="id" DataTextField="Trainer"  AppendDataBoundItems="false" ></asp:DropDownList>--%>

            Store: 
            <asp:DropDownList ID="sstoreDDL" runat="server">
                <asp:ListItem Value="%" Text="All" Selected="True"></asp:ListItem>
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
                   
            Store Number: 
            <asp:TextBox ID="StoreNumberTXT" runat="server" style="width: 100px;"></asp:TextBox> &nbsp; 
            <asp:FilteredTextBoxExtender ID="StoreNumberTXT_Filter" TargetControlID="StoreNumberTXT" runat="server" FilterMode="ValidChars" ValidChars="1234567890"></asp:FilteredTextBoxExtender> &nbsp; 

            Program: 
            <%--<asp:DropDownList ID="programDDL" runat="server" DataSourceID="sSQLprogram" DataValueField="Program" DataTextField="Program" AppendDataBoundItems="false">
            </asp:DropDownList> &nbsp; --%>
            <asp:DropDownList ID="programDDL" runat="server">
            </asp:DropDownList> &nbsp; 

            Date: from 
            <asp:TextBox ID="dateFrom" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
            <asp:FilteredTextBoxExtender ID="dateFrom_Filter" TargetControlID="dateFrom" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>
            to 
            <asp:TextBox ID="dateTo" runat="server" CssClass="Date" style="width: 100px;"></asp:TextBox> &nbsp; 
            <asp:FilteredTextBoxExtender ID="dateTo_Filter" TargetControlID="dateTo" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender> &nbsp; 

            <asp:Button ID="searchBTN" runat="server" OnClick="searchBTN_Click" Text="Search" />            
            <%--<asp:button id="addBtn" text="Add" postbackurl="~/ScheduleTrainerSetup.aspx" runat="Server"></asp:button>--%>
        
        <br /><br />
        
<asp:UpdatePanel ID="updateGrid" runat="server" UpdateMode="Conditional">
<ContentTemplate>
        
    <script type="text/javascript">
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function () {
            
            //Scripts have to be re-called when Ggidviews are bind
            ScheduleTrainerScripts();
        });
     </script>


        <%--<asp:GridView CssClass="neoGrid TrainerGrid" ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" AllowPaging="True" PageSize="15" 
            AllowSorting="True" HeaderStyle-Wrap="false" RowStyle-Wrap="false" DataKeyNames="Id" OnRowDataBound="GridView1_RowDataBound">--%>

        <asp:GridView CssClass="neoGrid TrainerGrid" ID="GridView1" runat="server" AutoGenerateColumns="False" AllowPaging="True" PageSize="15" 
                AllowSorting="True" HeaderStyle-Wrap="false" RowStyle-Wrap="false" DataKeyNames="ScheduleId" OnRowDataBound="GridView1_RowDataBound" OnPageIndexChanging="GridView1_PageIndexChanging">
            <PagerStyle CssClass="neoPager" />
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="scheduleTrainerId" HeaderText="scheduleTrainerId" ReadOnly="true" ItemStyle-CssClass="HideControl" HeaderStyle-CssClass="HideControl" FooterStyle-CssClass="HideControl" />
                <asp:BoundField DataField="ScheduleId" HeaderText="ScheduleId" ReadOnly="true" ItemStyle-CssClass="HideControl" HeaderStyle-CssClass="HideControl" FooterStyle-CssClass="HideControl"/>

                 <asp:TemplateField ItemStyle-CssClass="ColumnWidthTrainer" HeaderText="Trainer" SortExpression="Trainer">
                    <ItemTemplate>                       
                        <asp:DropDownList ID="DropDownTrainer" runat="server"></asp:DropDownList> 
                        <%--<asp:DropDownList ID="DropDownTrainer" runat="server" DataSourceID="TrainerSQL"   DataValueField="id" DataTextField="Trainer"  AppendDataBoundItems="true"></asp:DropDownList> --%>
                       <%--<asp:SqlDataSource ID="TrainerSQL" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"   SelectCommand="select 0 As ID ,' '  as Trainer  Union  all SELECT [ID],([Firstname] + ' ' +[Lastname]) as Trainer FROM [dbo].[PAYOUTtrainer] where [Active] = 1">  </asp:SqlDataSource>--%>
                    </ItemTemplate>
                </asp:TemplateField>

                   <asp:TemplateField HeaderText="Start Train" SortExpression="StartDate">
                    <ItemTemplate>                        
                        <asp:TextBox ID="TrainStartDate" CssClass="Date" runat="server" Text='<%#(Eval("TrainStartDate") == null ? "0" : Eval("TrainStartDate" , "{0:MM/dd/yyyy}")) %>'></asp:TextBox>
                           <asp:FilteredTextBoxExtender ID="dateStart_Filter" TargetControlID="TrainStartDate" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>                           
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="End Train" SortExpression="EndDate">
                    <ItemTemplate>                        
                        <asp:TextBox ID="TrainEndDate"  CssClass="Date" runat="server" Text='<%#(Eval("TrainEndDate") == null ? "0" : Eval("TrainEndDate","{0:MM/dd/yyyy}")) %>'></asp:TextBox>
                         <asp:FilteredTextBoxExtender ID="dateEnd_Filter" TargetControlID="TrainEndDate" runat="server" FilterMode="ValidChars" ValidChars="1234567890-/"></asp:FilteredTextBoxExtender>
                   
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:Button ID="BtnValidate" runat="server" Text="Upd" OnClick="BtnValidate_Click"/>
                        <input id="BtnAdd" type="button" value="Add"/>
                        <%--<input id="BtnDel" type="button" value="Del"/>--%>
                        <asp:Button ID="BtnDel" runat="server" Text="Del" OnClick="BtnDelete_Click"/>
                    </ItemTemplate>
                </asp:TemplateField>                


                <asp:BoundField DataField="Program" HeaderText="Program" SortExpression="Program" ReadOnly="True" />
                 <asp:BoundField DataField="StartDate" HeaderText="Start Show" SortExpression="StartDate" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy}"  />
                 <asp:BoundField DataField="EndDate" HeaderText="End Show" SortExpression="EndDate" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy}"  />
                 <asp:BoundField DataField="StoreName" HeaderText="Store Name" SortExpression="StoreName" ReadOnly="True"   ItemStyle-Width="10px" />
                 <asp:BoundField DataField="StoreNumber" HeaderText="Store #" SortExpression="StoreNumber" ReadOnly="True"  ItemStyle-Width="10px"  />
                 <asp:BoundField DataField="City" HeaderText="City" SortExpression="City" ReadOnly="True"   />
                 <asp:BoundField DataField="State" HeaderText="State" SortExpression="State" ReadOnly="True"   />
                 <%--<asp:BoundField DataField="OwnerFirstname" HeaderText="Owner Firstname" SortExpression="OwnerFirstname" ReadOnly="True"   />
                 <asp:BoundField DataField="OwnerLastname" HeaderText="Owner Lastname" SortExpression="OwnerLastname" ReadOnly="True"   />--%>
                <asp:BoundField DataField="Owner" HeaderText="Owner" SortExpression="Owner" ReadOnly="True"   />
                
               
            </Columns>
        </asp:GridView>
    
        <%--<asp:GridView CssClass="neoGrid" ID="GridView2" runat="server" AutoGenerateColumns="false" DataSourceID="SqlDataSource1" AllowPaging="true" PageSize="1" 
            AllowSorting="False" HeaderStyle-Wrap="false" RowStyle-Wrap="false" style="display: none;">--%>
            <asp:GridView CssClass="neoGrid" ID="GridView2" runat="server" AutoGenerateColumns="false" AllowPaging="true" PageSize="1" 
                AllowSorting="False" HeaderStyle-Wrap="false" RowStyle-Wrap="false" style="display: none;">
            <EmptyDataTemplate>
                <h4>No results found.</h4>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="TrainerId" HeaderText="TrainerId" SortExpression="TrainerId" ReadOnly="true" />
                <asp:BoundField DataField="TrainStartDate" HeaderText="Train Start Date" SortExpression="TrainStartDate" ReadOnly="true" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="TrainEndDate" HeaderText="Train End Date" SortExpression="TrainEndDate" ReadOnly="true" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="Program" HeaderText="Program" SortExpression="Program" ReadOnly="true" />
                <asp:BoundField DataField="StartDate" HeaderText="Start Date" SortExpression="StartDate" ReadOnly="true" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="EndDate" HeaderText="End Date" SortExpression="EndDate" ReadOnly="true" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="StoreName" HeaderText="Store Name" SortExpression="StoreName" ReadOnly="true" />
                <asp:BoundField DataField="StoreNumber" HeaderText="Store Number" SortExpression="StoreNumber" ReadOnly="true" />
                <asp:BoundField DataField="City" HeaderText="City" SortExpression="City" ReadOnly="true" />
                <asp:BoundField DataField="State" HeaderText="State" SortExpression="State" ReadOnly="true" />
                <%--<asp:BoundField DataField="OwnerFirstname" HeaderText="Owner Firstname" SortExpression="OwnerFirstname" ReadOnly="true" />
                <asp:BoundField DataField="OwnerLastname" HeaderText="Owner Lastname" SortExpression="OwnerLastname" ReadOnly="true" />
                <asp:BoundField DataField="HubFirstname" HeaderText="Hub Firstname" SortExpression="HubFirstname" ReadOnly="true" />
                <asp:BoundField DataField="HubLastname" HeaderText="Hub Lastname" SortExpression="HubLastname" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />--%>
                <asp:BoundField DataField="Owner" HeaderText="Owner Firstname" SortExpression="Owner" ReadOnly="true" />                
                <asp:BoundField DataField="Hub" HeaderText="Hub Firstname" SortExpression="Hub" ReadOnly="true" />
                

                <asp:BoundField DataField="ImportedBy" HeaderText="ImportedBy" SortExpression="ImportedBy" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="ImportedOn" HeaderText="Imported On" SortExpression="ImportedOn" ReadOnly="True" DataFormatString="{0:MM/dd/yyyy}" HtmlEncode="false" />
            </Columns>
        </asp:GridView>

 </ContentTemplate>
</asp:UpdatePanel>
            
        </div>

    </div>
    </form>
</body>
</html>