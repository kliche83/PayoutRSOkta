<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountResetPassword.aspx.cs" Inherits="Payout.AccountResetPassword" %>

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
                        
                    <h1>Reset your Password</h1>

                    <label for="txtPassword1">Password:</label>
                    <asp:TextBox ID="txtPassword1" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>

                    <label for="txtConfirmPassword1">Confirm Password:</label>
                    <asp:TextBox ID="txtConfirmPassword1" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>

                    <br /><br />
                    <asp:Button ID="BtnReset" runat="server" OnClick="BtnReset_Click" CssClass="form-control" Text="Reset" />
                
                    <asp:Label ID="valMessage" runat="server" CssClass="ValidationMessage"></asp:Label>
                    <asp:Label ID="SubmitedMessage" runat="server" CssClass="SubmitedMessage"></asp:Label>
                    <br />
                    <asp:LinkButton ID="lnkRedirectLogin" runat="server" OnClick="lnkRedirectLogin_Click" >&lt;&lt; Back to login form</asp:LinkButton>
                </div>
            </div>
        </form>
</body>
</html>
