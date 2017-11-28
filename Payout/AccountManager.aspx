<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountManager.aspx.cs" Inherits="Payout.AccountManager" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css" />
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script src="//code.jquery.com/ui/1.11.2/jquery-ui.js"></script>

    <%--<script src="Scripts/jquery-2.2.1.min.js"></script>
    <script src="h_ttp://code.jquery.com/ui/1.7.3/jquery-ui.js"></script>--%>


    <script src="Scripts/Controllers/AccountManager.js"></script>
    <script src="Scripts/Scripts.js"></script>

    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="Content/Style.css" rel="stylesheet" />
    
</head>
<body>
    <form id="form1" runat="server">      
        <div id="UserRoles">
            <div id="tabsAccManager">
                <ul>
                    <li class="tab1">User & Roles</li>
                    <li class="tab2">Roles & Modules</li>
                    <li class="tab3">Modules & Access Levels</li>
                </ul>

                <div id="AssignUserRoles" class="Content-Wrapper tab1">
                
                    <div id="AddUser">
                        <h1>Add User</h1>
                        <br />

                        <div><span>First Name: </span><input id="fname" type="text" value="Carlos" /></div>
                        <div><span>Last Name: </span><input id="lname" type="text" value="Yahoo" /></div>
                        <div><span>Email: </span><input id="email" type="text" value="andresin83@yahoo.es" /></div>
                        <div><span>Is Okta?: </span><input id="isokta" type="checkbox" /></div>
                
                        <input id="SubmitUser" type="submit" value="submit" />
                    </div>

                    <div id="AddRole">
                        <br />
                        <div><span>Role Name: </span><input id="rname" type="text" /></div>

                        <input id="SubmitRole" type="submit" value="submit" />
                    </div>
                    
                    <h2>User List</h2>
                    <asp:GridView ID="GRVUserList" runat="server" OnRowDataBound="GRVUserList_RowDataBound" AutoGenerateColumns="False">
                        <EmptyDataTemplate>
                            <h4>No results found.</h4>
                        </EmptyDataTemplate>

                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Label ID="Id" CssClass="clsId HideCtrl" runat="server" Text='<%# Bind("Email") %>'></asp:Label>                                
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <a class="delUserLink" style="color:red; cursor:pointer; text-decoration:none" >X</a>                                
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="First Name" SortExpression="FirstName">
                                <ItemTemplate>                        
                                    <asp:TextBox ID="FirstName" runat="server" Text='<%# Bind("FirstName") %>'></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Last Name" SortExpression="LastName">
                                <ItemTemplate>
                                    <asp:TextBox ID="LastName" runat="server" Text='<%# Bind("LastName") %>'></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Email" SortExpression="Email">
                                <ItemTemplate>
                                    <asp:TextBox ID="Email" runat="server" Text='<%# Bind("Email") %>'></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Is Okta?" SortExpression="IsOkta">
                                <ItemTemplate>
                                    <asp:CheckBox ID="IsOkta" runat="server" Checked='<%# Eval("IsOkta") %>'></asp:CheckBox>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Disabled" SortExpression="IsDisabled">
                                <ItemTemplate>
                                    <asp:CheckBox ID="uIsDisabled" runat="server" Checked='<%# Eval("IsDisabled") %>'></asp:CheckBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Role" SortExpression="Role">
                                <ItemTemplate>                                    
                                    <asp:DropDownList ID="DropDownRole" runat="server"></asp:DropDownList>                                    
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>




                    <h2>Role List</h2>
                    <asp:GridView ID="GRVRoleList" runat="server" AutoGenerateColumns="False">
                        <EmptyDataTemplate>
                            <h4>No results found.</h4>
                        </EmptyDataTemplate>

                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Label ID="Id" CssClass="clsId HideCtrl" runat="server" Text='<%# Bind("Name") %>'></asp:Label>                                
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <a class="delRoleLink" style="color:red; cursor:pointer; text-decoration:none" >X</a>                                
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                <ItemTemplate>                        
                                    <%--<asp:TextBox ID="Name" runat="server" Text='<%# Bind("Name") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:TextBox>--%>
                                    <asp:TextBox ID="Name" runat="server" Text='<%# Bind("Name") %>'></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Disabled" SortExpression="IsDisabled">
                                <ItemTemplate>                        
                                    <%--<asp:CheckBox ID="rIsDisabled" runat="server" Checked='<%# Eval("IsDisabled") %>' AutoPostBack="true" OnTextChanged="FieldChanged"></asp:CheckBox>--%>
                                    <asp:CheckBox ID="rIsDisabled" runat="server" Checked='<%# Eval("IsDisabled") %>'></asp:CheckBox>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>

                    </asp:GridView>
                
                </div>


                <div id="AssignRoleModuleLevel" class="Content-Wrapper tab2">
                    

                    <div id="AddModule">
                        <h1>Add Module</h1>
                        <br />

                        <div><span>Module Name: </span><input id="modname" type="text" /></div>                        
                        <br />
                        <input id="SubmitModule" type="submit" value="submit" />
                    </div>
                                
                </div>
            

                <div id="AssignModuleAccessLevel" class="Content-Wrapper tab3">
                    <p>Add module: </p>                
                    <input id="Add Module" type="text" />
                    <input type="button" value="Add"/>
                </div>
            </div>
        </div>    
    </form>

</body>
</html>
