<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Error.aspx.cs" Inherits="ErrorPages_Error" %>

<html lang="en" style="height: 100%">
<head>
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel='shortcut icon' type='image/x-icon' href="<%= CU.StaticFilePath %>SystemImages/icon.ico" />
    <link rel='apple-touch-icon' href="<%= CU.StaticFilePath %>SystemImages/icon.ico" />
    <title>Error - OCTFIS</title>

    <!--Link Bootstrap -->
    <link rel="stylesheet" type="text/css" href="<%= CU.StaticFilePath %>plugins/bootstrap/dist/css/bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="<%= CU.StaticFilePath %>plugins/themify-icons/themify-icons.css" />
    <link rel="stylesheet" type="text/css" href="<%= CU.StaticFilePath %>plugins/build/css/first-layout.css" />
    <link href="<%= CU.StaticFilePath %>plugins/OwnPlugin/css/loader/zloder.css" rel="stylesheet"
        type="text/css" />
</head>
<body style="background-image: url(<%= CU.StaticFilePath %>SystemImages/ErrorBackground.jpg)"
    class="body-bg-full">
    <div class="container page-container">
        <div class="page-content">
            <div class="logo">
                <img src="<%= CU.StaticFilePath %>SystemImages/logo-gif.gif" alt="" width="80" /></div>
            <h1 style="font-size: 130px" class="m-0 text-muted fw-300">
                <asp:Literal ID="ltrlErrorCode" runat="server"></asp:Literal>
            </h1>
            <h4 class="fs-16 text-white dblViewMessage fw-300">
                <asp:Literal ID="ltrlErrorMessage" runat="server"></asp:Literal></h4>
            <p class="text-muted mb-15">
                <asp:Literal ID="ltrlErrorDescription" runat="server"></asp:Literal></p>
            <a href="../Default.aspx" role="button" style="width: 130px" class="btn btn-primary btn-rounded">
                Return to home</a>
            <p class="text-muted erredescription hidden mb-15 text-left">
                <asp:Literal ID="ltrlErrorDetail" runat="server"></asp:Literal></p>
        </div>
    </div>
</body>

<script src="<%= CU.StaticFilePath %>plugins/js/jquery.min.js" type="text/javascript"></script>

<script type="text/javascript">
    jQuery(function() {
        $(".dblViewMessage").dblclick(function() {
            $(".erredescription").toggleClass("hidden");
        });
    });
</script>

</html>
