<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Sokoban.Web.Index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Sokoban</title>
    <script src="Scripts/Silverlight.js" type="text/javascript"></script>
    <script src="Scripts/SilverlightOnError.js" type="text/javascript"></script>
    <link href="Styles/Style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server" style="height: 100%;">
        <div id="silverlightControlHost" >
            <object data="data:application/x-silverlight-2," type="application/x-silverlight-2"
                width="100%" height="100%">
                <param name="source" value="ClientBin/Sokoban.xap?id=17" />
                <param name="onError" value="onSilverlightError" />
                <param name="background" value="white" />
                <param name="minRuntimeVersion" value="3.0.40818.0" />
                <param name="autoUpgrade" value="true" />
                 <asp:Literal ID="ParamInitParams" runat="server"></asp:Literal>

                <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=3.0.40818.0" style="text-decoration: none">
                    <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight"
                        style="border-style: none" />
                </a>
            </object>
            <iframe id="_sl_historyFrame" style="visibility: hidden; height: 0px; width: 0px;
                border: 0px"></iframe>
        </div>
    </form>
</body>
</html>
