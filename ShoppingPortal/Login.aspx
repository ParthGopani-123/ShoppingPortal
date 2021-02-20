<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<html lang="en" style="height: 100%">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel='shortcut icon' type='image/x-icon' href="<%= string.Format("{0}SystemImages/OCTFISIcon.ico", CU.StaticFilePath) %>" />
    <link rel='apple-touch-icon' href="<%= string.Format("{0}SystemImages/OCTFISIcon.ico", CU.StaticFilePath) %>" />
    <title>Login - Shopping Portal</title>
    <link rel="stylesheet" type="text/css" href="<%= CU.StaticFilePath %>plugins/bootstrap/dist/css/bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="<%= CU.StaticFilePath %>plugins/themify-icons/themify-icons.css" />
    <link rel="stylesheet" type="text/css" href="<%= CU.StaticFilePath %>plugins/build/css/first-layout.css" />
    <link rel="stylesheet" type="text/css" href="<%= CU.StaticFilePath %>plugins/OwnPlugin/css/loader/zloder.css" />
    <link href="<%= CU.StaticFilePath %>plugins/js/jquery-ui.css" rel="stylesheet" type="text/css" />
    <style>
        *::-webkit-input-placeholder {
            /* WebKit, Blink, Edge */
            color: #b5b5b5 !important;
        }

        *:-moz-placeholder {
            /* Mozilla Firefox 4 to 18 */
            color: #b5b5b5 !important;
            opacity: 1;
        }

        *::-moz-placeholder {
            /* Mozilla Firefox 19+ */
            color: #b5b5b5 !important;
            opacity: 1;
        }

        *:-ms-input-placeholder {
            /* Internet Explorer 10-11 */
            color: #b5b5b5 !important;
        }

        .errormessage {
            color: Red;
        }

        .Userimage {
            height: 100px;
            width: 100px;
            border-radius: 50%;
            border: 2px solid #e6e6e6;
            padding: 2px;
        }

        .body-bg-full .logo {
            margin-bottom: 15px;
        }

        .imglogo {
            height: 100px;
            width: 100px;
        }

        .pnlSlide {
            display: none;
        }

        .minheight {
            min-height: 282px;
        }
    </style>
</head>
<body style="background-image: url(<%= CU.StaticFilePath %>SystemImages/Backgrounds.jpg)"
    class="body-bg-full v2">
    <div class="container page-container">
        <div class="page-content">
            <div class="v2 divloader pb-1 minheight">
                <form id="form2" runat="server" class="form-horizontal mb-0">
                    <asp:ScriptManager ID="scriptManager1" runat="server" ScriptMode="Release" EnablePageMethods="true">
                    </asp:ScriptManager>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <asp:TextBox ID="txtAllowSlide" Text="0" CssClass="txtAllowSlide hide" runat="server"></asp:TextBox>
                            <asp:MultiView ID="MultiView" runat="server" ActiveViewIndex="0">
                                <asp:View ID="Step1" runat="server">
                                    <asp:Panel ID="pnlStep1" DefaultButton="btnCheckUserName" runat="server">
                                        <div class="pnlSlide" direction="left">
                                            <div class="logo p-20">
                                                <img src="<%= CU.StaticFilePath %>SystemImages/logo-gif.gif" alt="" class="imglogo">
                                            </div>
                                            <asp:Label ID="lblErrorUserName" CssClass="errormessage lblErrorUserName" runat="server"
                                                Text=""></asp:Label>
                                            <div class="form-group">
                                                <div class="col-xs-12">
                                                    <asp:TextBox ID="txtUsername" autocomplete="off" runat="server" placeholder="Enter Your Username"
                                                        MaxLength="50" CssClass="txtUsername txtFocus form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <asp:Button ID="btnCheckUserName" OnClick="btnCheckUserName_OnClick" runat="server"
                                                CssClass="btnCheckUserName btn-lg btn btn-primary btn-rounded btn-block" Text="Next" />
                                            <div class="form-group mt-10">
                                                <div class="col-xs-12">
                                                    <div class="pull-right">
                                                        <a href="ForgotPassword.aspx" class="inline-block form-control-static">Forgot a Password?</a>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </asp:View>
                                <asp:View ID="Step2" runat="server">
                                    <asp:Panel ID="pnlStep2" DefaultButton="btnCheckPassword" runat="server">
                                        <div class="pnlSlide" direction="right">
                                            <div class="logo">
                                                <asp:Image ID="imgUserImage" runat="server" alt="" CssClass="Userimage" />
                                            </div>
                                            <h4 class="fs-16 fw- fw-700 mb-0 mt-0">
                                                <asp:Label ID="lblUserName" runat="server" Text=""></asp:Label>
                                                <br />
                                                <asp:LinkButton Style="font-weight: normal; font-size: 11px;" ID="lnkDefferentAccount"
                                                    OnClientClick="AddLoader()" OnClick="lnkDefferentAccount_OnClick" runat="server"
                                                    class="inline-block form-control-static">Not You?</asp:LinkButton></h4>
                                            <asp:Label ID="lblErrorPassword" CssClass="errormessage lblErrorPassword" runat="server"
                                                Text=""></asp:Label>
                                            <div class="form-group">
                                                <div class="col-xs-12">
                                                    <asp:TextBox ID="txtPassword" TextMode="Password" runat="server" MaxLength="50" placeholder="Enter Your Password"
                                                        CssClass="form-control txtFocus txtPassword"></asp:TextBox>
                                                </div>
                                            </div>
                                            <asp:Button ID="btnCheckPassword" OnClick="btnCheckPassword_OnClick" runat="server"
                                                CssClass="btnCheckPassword btn-lg btn btn-primary btn-rounded btn-block" Text="Login" />
                                            <div class="form-group pt-10">
                                                <div class="col-xs-12">
                                                    <div class="checkbox-inline checkbox-custom pull-left">
                                                        <asp:CheckBox ID="chkRememberme" runat="server" CssClass="checkbox-muted text-muted"
                                                            Checked="true" Text="Remember me" />
                                                    </div>
                                                    <div class="pull-right">
                                                        <a href="ForgotPassword.aspx" class="inline-block form-control-static">Forgot a Password?</a>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </asp:View>
                            </asp:MultiView>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </form>
            </div>
        </div>
    </div>
    <!-- jQuery-->

    <script src="<%= CU.StaticFilePath %>plugins/js/jquery.min.js" type="text/javascript"></script>

    <!-- Bootstrap JavaScript-->

    <script type="text/javascript" src="<%= CU.StaticFilePath %>plugins/bootstrap/dist/js/bootstrap.min.js"></script>

    <!-- Custom JS-->

    <script type="text/javascript" src="<%= CU.StaticFilePath %>plugins/build/js/first-layout/extra-demo.js"></script>

    <script src="<%= CU.StaticFilePath %>plugins/js/jquery-ui.js" type="text/javascript"></script>

    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Checkpostback);
        jQuery(function () {
            Checkpostback();
        });
        function Checkpostback() {
            var txtAllowSlide = $(".txtAllowSlide");
            if (parseInt(txtAllowSlide.val()) == 0) {
                $('.pnlSlide').show();
                $('.txtFocus').focus();
            }
            else {
                setTimeout(function () {
                    $('.pnlSlide').show('slide', { direction: $('.pnlSlide').attr('direction') }, 200);
                    $('.txtFocus').focus();
                    txtAllowSlide.val("0");
                }, 100);
            }

            $(".divloader").removeClass("csspinner traditional");

            $(".btnCheckUserName").click(function () {
                if ($(".txtUsername").val() == "") {
                    $(".lblErrorUserName").text("Please Enter Username.");
                    $(".txtUsername").css("border-color", "transparent transparent #ff0b0b");
                    $(".txtUsername").focus();
                    return false;
                }
                else {
                    $(".lblErrorUserName").text("");
                    $(".txtUsername").css("border-color", "transparent transparent #e6e6e6");
                    AddLoader();
                    return true;
                }
            });

            $(".btnCheckPassword").click(function () {
                if ($(".txtPassword").val() == "") {
                    $(".lblErrorPassword").text("Please Enter Password.");
                    $(".txtPassword").css("border-color", "transparent transparent #ff0b0b");
                    $(".txtPassword").focus();
                    return false;
                }
                else {
                    $(".lblErrorPassword").text("");
                    $(".txtPassword").css("border-color", "transparent transparent #e6e6e6");
                    AddLoader();
                    return true;
                }
            });
        }

        function AddLoader() {
            $(".divloader").addClass("csspinner traditional");
        }

        //Error : Notify Null In Chrome-----------------------------------------------
        Sys.Browser.WebKit = {};
        if (navigator.userAgent.indexOf('WebKit/') > -1) {
            Sys.Browser.agent = Sys.Browser.WebKit;
            Sys.Browser.version = parseFloat(navigator.userAgent.match(/WebKit\/(\d+(\.\d+)?)/)[1]);
            Sys.Browser.name = 'WebKit';
        }

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
        function EndRequestHandler(sender, args) {
            if (args.get_error() != undefined) {
                args.set_errorHandled(true);
            }
        }
    </script>

</body>
</html>
