<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountForgotPassword.aspx.cs" Inherits="Payout.AccountForgotPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="Scripts/jquery-2.2.1.min.js"></script>    
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="Content/Style.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="backgroundDiv">
            <div id="LoginWrapper" class="form-group">
                        
                <h1>Forgot your Password?</h1>

                <p>Enter your email. We'll email instructions on how to reset your password</p>

                <label for="txtEmail">Your Email:</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>

                <br /><br />
                <asp:Button ID="BtnSubmit" runat="server" OnClick="BtnSubmit_Click" CssClass="form-control" Text="Submit" />
                
                <asp:Label ID="valMessage" runat="server" CssClass="ValidationMessage"></asp:Label>
                <asp:Label ID="SubmitedMessage" runat="server" CssClass="SubmitedMessage"></asp:Label>

                <br />
                <asp:LinkButton ID="lnkRedirectLogin" runat="server" OnClick="lnkRedirectLogin_Click" >&lt;&lt; Back to login form</asp:LinkButton>
            </div>
        </div>
    </form>
</body>
</html>
