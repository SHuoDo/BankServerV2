<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Registration_Form.aspx.vb" Inherits="BankFormVB.Registration_Form" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Bank Registration Form - ASP.Net - VB</title>
    <style type="text/css">
        #form1 {
            height: 420px;
            width: 296px;
        }
    </style>
</head> 
<body>
    <form id="form1" runat="server">
        <asp:Label ID="Label1" runat="server" Font-Size="X-Large" Text="Registration Form"></asp:Label>
        <br />
        <br />
        <asp:Label ID="Label2" runat="server" Font-Size="X-Large" Text="Name"></asp:Label>
        <br />
        <asp:TextBox ID="TextBoxName" runat="server" Font-Size="X-Large"></asp:TextBox>
        <br />
        <br />
        <asp:Label ID="Label3" runat="server" Font-Size="X-Large" Text="Username"></asp:Label>
        <br />
        <asp:TextBox ID="TextBoxUser" runat="server" Font-Size="X-Large"></asp:TextBox>
        <br />
        <br />
        <asp:Label ID="Label4" runat="server" Font-Size="X-Large" Text="Password"></asp:Label>
        <br />
        <asp:TextBox ID="TextBoxPassword" runat="server" Font-Size="X-Large"></asp:TextBox>
        <br />
        <br />
        <asp:Label ID="Label5" runat="server" Font-Size="X-Large" Text="Amount"></asp:Label>
        <br />
        <asp:TextBox ID="TextBoxAmount" runat="server" Font-Size="X-Large"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="ButtonSubmit" runat="server" Font-Size="X-Large" Text="Submit" />
    </form>
</body>
</html>
