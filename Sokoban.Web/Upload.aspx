<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="Sokoban.Web.Upload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <table>
            <tbody>
                <tr>
                    <td style="width: 169px; height: 21px">
                        ASPX</td>
                    <td style="height: 21px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 169px">
                        <asp:Label ID="lblAspxResult" runat="server"></asp:Label></td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td style="width: 169px; height: 21px">
                        <asp:Button ID="btnDeleteAspx" runat="server" Text="Delete all Aspx files" OnClick="btnDeleteAspx_Click" /></td>
                    <td style="height: 21px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 169px; height: 21px">
                        <asp:Button ID="btnUploadAspx" runat="server" OnClick="btnUploadAspx_Click" Text="Upload Aspx Files" /></td>
                    <td style="height: 21px">
                        <input id="FindFile" type="file" runat="Server" />
                        <asp:Button ID="btnAdd" runat="server" OnClick="btnAdd_Click" Text="Add" /></td>
                </tr>
                <tr>
                    <td colspan="2" style="height: 21px">
                        <asp:ListBox ID="lstAspxFiles" runat="server" Width="385px"></asp:ListBox></td>
                </tr>
                <tr>
                    <td colspan="2" style="height: 25px">
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="height: 21px">
                        Bin Folder</td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblBinResult" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="2" style="height: 21px">
                        <asp:Button ID="btnDeleteBinFiles" runat="server" Text="Delete all Bin files" OnClick="btnDeleteBinFiles_Click" /></td>
                </tr>
                <tr>
                    <td style="height: 21px">
                        <asp:Button ID="btnUploadBinFiles" runat="server" OnClick="btnUploadBinFiles_Click"
                            Text="Upload Bin Files" /></td>
                    <td>
                        <asp:FileUpload ID="FileUpload1" runat="server" />
                        <asp:Button ID="btnAddBin" runat="server" OnClick="btnAddBin_Click" Text="Add" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:ListBox ID="lstBinFiles" runat="server" Width="385px"></asp:ListBox></td>
                </tr>
                <tr>
                    <td colspan="2">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2">
                        SQL Statement</td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblSqlResult" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="btnExecuteSql" runat="server" OnClick="btnExecuteSql_Click"
                            Text="Exceute SQL" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:TextBox ID="txtSqlStatement" runat="server" Height="112px" 
                            TextMode="MultiLine" Width="385px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:PlaceHolder ID="phSqlResult" runat="server"></asp:PlaceHolder>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="btnStopServerr" runat="server" OnClick="btnStopServerr_Click"
                            Text="Stop Server" />
                    </td>
                </tr>
            </tbody>
        </table>
    </form>
</body>
</html>
