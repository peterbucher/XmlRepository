<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="XmlRepository.UI.Web._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title></title>
    </head>

    <body>
        <form runat="server">
            <asp:Button ID="btnClearContent" Text="Alle Daten löschen" runat="server" /><br />
            <asp:HyperLink ID="hlCreateNewTodo" Text="Neues Todo erstellen" NavigateUrl="~/Default.aspx" runat="server" /><br />
            <asp:Label ID="lblTitle" AssociatedControlID="txtTitle" Text="Titel" runat="server" />
            <asp:TextBox ID="txtTitle" runat="server" /><br />
            <asp:Label ID="lblText" AssociatedControlID="txtText" Text="Text" runat="server" />
            <asp:TextBox ID="txtText" TextMode="MultiLine" Height="120" runat="server" /><br />
            <asp:Button ID="btnSubmit" Text="Hinzufügen" runat="server" />
            <hr />
            <asp:Repeater ID="rptTodos" runat="server">
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <ItemTemplate>
                    <li>
                        <a href='?id=<%#Eval("Id") %>'><%#Eval("Title") %></a>
                        <span>- <%#Eval("DateCreated") %></span>
                        <p>
                            <%#Eval("Text") %>
                        </p>
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
        </form>
    </body>
</html>