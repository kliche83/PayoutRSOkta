<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Payout.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
        <title>Login</title>
        <script src="Scripts/jquery-2.2.1.min.js"></script>
        <%--<script src="h_ttp://code.jquery.com/ui/1.7.3/jquery-ui.js"></script>--%>
        <link href="Content/bootstrap.min.css" rel="stylesheet" />
        <link href="Content/Style.css" rel="stylesheet" />

    </head>

    <body>
        <form id="form1" runat="server">
            <div id="backgroundDiv">
                <div id="LoginWrapper" class="form-group">
                        
                    <div id="UserLogo">👤</div>

                    <asp:Panel ID="pnlUser" runat="server">
                        <label for="txtUserName">User:</label>
                        <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control" ></asp:TextBox>
                    </asp:Panel>                    

                    <asp:Panel ID="pnlPassword" runat="server">
                        <label for="txtPassword">Password:</label>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                    </asp:Panel>
                    

                    <br />
                    <asp:LinkButton ID="lnkForgotPassword" runat="server" OnClick="lnkForgotPassword_Click" >Forgot Password</asp:LinkButton>
                    
                    <br /><br />
                    <asp:Button ID="BtnNextAsp" runat="server" OnClick="BtnNextAsp_Click" CssClass="form-control" Text="Next" />
                
                    <asp:Label ID="valMessage" runat="server" CssClass="ValidationMessage"></asp:Label>
                </div>
            </div>
        </form>
    </body>
</html>
