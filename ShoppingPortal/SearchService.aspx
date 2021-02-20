<%@ Page Language="C#" AutoEventWireup="true" Title="Search Pincode" CodeFile="SearchService.aspx.cs" Inherits="SearchService" EnableEventValidation="false" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link rel='shortcut icon' type='image/x-icon' href="<%= string.Format("{0}SystemImages/icon.ico", CU.StaticFilePath) %>" />
    <link rel='apple-touch-icon' href="<%= string.Format("{0}SystemImages/icon.ico", CU.StaticFilePath) %>" />

    <style type="text/css">
        body {
            font-family: 'Helvetica', sans-serif;
        }

        .flexsearch {
            margin-bottom: 5px;
        }

        .flexsearch--wrapper {
            height: auto;
            width: 100%;
            max-width: 100%;
            overflow: hidden;
            background: transparent;
            margin: 0;
            position: static;
            text-align: center;
        }

        .flexsearch--form {
            overflow: hidden;
            position: relative;
        }

        .flexsearch--input-wrapper {
            padding: 0 66px 0 0; /* Right padding for submit button width */
            overflow: hidden;
        }

        .flexsearch--input {
            width: 100%;
        }

        .flexsearch--input {
            -webkit-box-sizing: content-box;
            -moz-box-sizing: content-box;
            box-sizing: content-box;
            height: 60px;
            padding: 0 26px 0 30px;
            border-color: #888;
            border-radius: 35px;
            border-style: solid;
            border-width: 5px;
            margin-top: 15px;
            color: #333;
            font-family: 'Helvetica', sans-serif;
            font-size: 26px;
            -webkit-appearance: none;
            -moz-appearance: none;
            text-align: center;
        }

        .flexsearch--submit {
            position: absolute;
            right: 0;
            top: 0;
            display: block;
            width: 60px;
            height: 60px;
            padding: 0;
            border: none;
            margin-top: 20px; /* margin-top + border-width */
            margin-right: 5px; /* border-width */
            background: transparent;
            color: #888;
            font-family: 'Helvetica', sans-serif;
            font-size: 40px;
            line-height: 60px;
            outline: none;
        }

        .flexsearch--input:focus {
            outline: none;
            border-color: #333;
        }

            .flexsearch--input:focus.flexsearch--submit {
                color: #333;
            }

        .flexsearch--submit:hover {
            color: #333;
            cursor: pointer;
        }

        ::-webkit-input-placeholder {
            color: #ccc;
        }

        input:-moz-placeholder {
            color: #ccc
        }

        .h1 {
            margin: 25px 25px 5px 25px;
            color: #333;
            font-size: 45px;
            font-weight: bold;
            line-height: 45px;
            text-align: center;
        }

        .searchingresult {
            width: 100%;
            text-align: center;
        }

        .error {
            padding: 10px;
            background-color: #dc2828;
            color: #fff;
            font-size: 22px;
            display: none;
        }

        .close {
            color: #fff;
            text-decoration: none;
            font-size: 12px;
            float: right;
            margin-top: -7px;
            margin-right: -5px;
        }

        .lblPincode {
            display: block;
            font-size: 40px;
            color: #4e4d4d;
            font-weight: bold;
        }

        .lblPincodeDetail {
            display: block;
            font-size: 30px;
            color: #a2a2a2;
        }

        .divwidget {
            width: 20%;
            color: #fff;
            display: inline-block;
            padding: 10px;
            font-size: 30px;
            margin-top: 5px;
        }

        .bg-success {
            background-color: green;
        }

        .bg-purple {
            background-color: purple;
        }

        .bg-red {
            background-color: #d60505;
        }

        .Status {
            font-size: 15px;
            display: block;
            color: #dadada;
        }

        @media only screen and (max-width: 500px) {
            .divwidget {
                width: 40%;
                font-size: 20px;
            }
        }

        .imgloader {
            display: none;
            height: 50px;
            width: 50px;
            margin-right: 18px;
            top: 5px;
        }

        .lblCourierName {
            display: block;
            font-size: 18px;
            font-weight: bold;
            color: #a2a2a2;
        }

        .divViewAllMatchProduct {
            text-align: right;
            margin: 6px 3px;
        }

        .bg-success {
            background-color: #1abe9e;
            color: #FFF;
            border-radius: 10px;
        }

        .col-sm-3 {
            width: 25%;
            margin: 0 2px;
            text-align: center;
        }

        .TopRow {
            display: flex;
        }

        .media-body {
            padding: 1px 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="frmService" method="post">
        <cc1:ToolkitScriptManager ID="scriptManager1" runat="server" AsyncPostBackTimeout="9999" ScriptMode="Release"
            EnablePageMethods="true">
        </cc1:ToolkitScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Label ID="lblOrganizationId" runat="server" Visible="false"></asp:Label>

                <asp:Label ID="lblErrorMessage" runat="server" CssClass="lblErrorMessage" Style="display: none;" Text=""></asp:Label>
                <%--<h1 class="h1">Search Pincode</h1>--%>
                <div class="TopRow">
                    <div class="col-sm-3">
                        <a class="aLink" href="Home.aspx">
                            <div class="widget no-border p-15 bg-success media">
                                <div class="media-body">
                                    <h4 class="m-0">Home</h4>
                                </div>
                            </div>
                        </a>
                    </div>
                    <div class="col-sm-3">
                        <a class="aLink" href="ManageOrder.aspx">
                            <div class="widget no-border p-15 bg-success media">
                                <div class="media-body">
                                    <h4 class="m-0">Order</h4>
                                </div>
                            </div>
                        </a>
                    </div>
                    <div class="col-sm-3">
                        <a class="aLink" href="OrderView.aspx">
                            <div class="widget no-border p-15 bg-success media">
                                <div class="media-body">
                                    <h4 class="m-0">My Order</h4>
                                </div>
                            </div>
                        </a>
                    </div>

                    <div class="col-sm-3">
                        <a id="aSearchProduct" runat="server" target="_blank" class="aLink">
                            <div class="widget no-border p-15 bg-success media">
                                <div class="media-body">
                                    <h4 class="m-0">Product</h4>
                                </div>
                            </div>
                        </a>
                    </div>
                </div>
                <div class="content">
                    <div class="flexsearch">
                        <div class="flexsearch--wrapper">
                            <div class="flexsearch--form">
                                <div class="flexsearch--input-wrapper">
                                    <asp:TextBox ID="txtPincode" runat="server" CssClass="txtPincode flexsearch--input" autocomplete="off" placeholder="Enter Pincode"></asp:TextBox>
                                </div>
                                <asp:Button ID="btnSearchPincode" OnClick="btnSearchPincode_OnClick" CssClass="btnSearchPincode flexsearch--submit" runat="server" Text="&#10140;" />
                                <img src="<%= CU.StaticFilePath %>SystemImages/loading.gif" class="imgloader flexsearch--submit" />
                            </div>
                        </div>
                    </div>
                    <div class="divViewAllMatchProduct">
                        <a class="ml-10" href="Login.aspx">Login</a>
                    </div>
                    <div class="searchingresult">
                        <div class="error">
                            <sapn id="ErrorMessage"></sapn>
                            <a href="javascript:;" class="close">&times;</a>
                        </div>
                        <asp:Repeater ID="rptServiceAvailability" runat="server" OnItemDataBound="rptServiceAvailability_OnItemDataBound">
                            <ItemTemplate>
                                <div class="result">
                                    <asp:Label ID="lblCourierName" runat="server" CssClass="lblCourierName"></asp:Label>
                                    <div id="divPrepaid" runat="server" class="divwidget">
                                        <span class="divTitle">Prepaid</span>
                                        <asp:Label ID="lblPrepaidStatus" runat="server" CssClass="Status" Text="Unvailable"></asp:Label>
                                    </div>
                                    <div id="divCOD" runat="server" class="divwidget">
                                        <span class="divTitle">COD</span>
                                        <asp:Label ID="lblCODStatus" runat="server" CssClass="Status" Text="Unvailable"></asp:Label>
                                    </div>
                                    <div id="divPickup" runat="server" class="divwidget">
                                        <span class="divTitle">Pickup</span>
                                        <asp:Label ID="lblPickupStatus" runat="server" CssClass="Status" Text="Unvailable"></asp:Label>
                                    </div>
                                    <div id="divReversePickup" runat="server" class="divwidget">
                                        <span class="divTitle">R Pickup</span>
                                        <asp:Label ID="lblReversePickupStatus" runat="server" CssClass="Status" Text="Unvailable"></asp:Label>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Label ID="lblPincode" CssClass="lblPincode" runat="server" Text="395004"></asp:Label>
                        <asp:Label ID="lblPincodeDetail" CssClass="lblPincodeDetail" runat="server" Text="Surat, Gujrat, India"></asp:Label>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>

<script src="<%= CU.StaticFilePath %>plugins/js/jquery.min.js" type="text/javascript"></script>

<script type="text/javascript">
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(AfterUpdate);
    jQuery(function () {
        AfterUpdate();
    });

    function AfterUpdate() {
        if ($(".lblErrorMessage").text() != "") {
            SetErrorMsg($(".lblErrorMessage").text());
        }

        $('.preloader').fadeOut('slow');

        $("input:text").focus(function () { $(this).select(); });

        $(".txtPincode").focus();

        $('.btnSearchPincode').click(function () {
            if (CheckValidate()) {
                $(".imgloader").show();
                return true;
            }
            else
                return false;
        });

        function CheckValidate() {
            var strPincode = $(".txtPincode").val().replace(/\D/g, '');
            var Pincode = parseInt(strPincode);

            if (Pincode == "") {
                SetErrorMsg("Please Enter Pincode");
                $(".txtPincode").focus();
                return false;
            }
            else if (Pincode.toString() != strPincode) {
                SetErrorMsg("Please Enter Valid Pincode");
                $(".txtPincode").focus();
                return false;
            }
            else if (isNaN(Pincode)) {
                SetErrorMsg("Please Enter Valid Pincode");
                $(".txtPincode").focus();
                return false;
            }
            else if (Pincode.toString().length != 6) {
                SetErrorMsg("Please Enter Valid Pincode");
                $(".txtPincode").focus();
                return false;
            }
            else {
                return true;
            }
        }

        $(".txtPincode").keydown(function (e) {
            // Allow: backspace, delete, tab, escape, enter and .
            if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                // Allow: Ctrl+A, Command+A
                (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
                // Allow: home, end, left, right, down, up
                (e.keyCode >= 35 && e.keyCode <= 40) ||
                (e.ctrlKey && e.keyCode == 86)) {
                // let it happen, don't do anything
                return;
            }
            // Ensure that it is a number and stop the keypress
            if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                e.preventDefault();
            }

            $(".error").hide();
        });

        $(".txtPincode").on('keyup contextmenu input', function (e) {
            //$(".txtPincode").on('keyup',function (e) {
            var Pincode = $(".txtPincode").val().replace(/\D/g, '');
            $(".txtPincode").val(Pincode);

            if (Pincode.length >= 6) {
                SerachValue();
            }
        });

        $(".close").click(function () {
            $(this).parent().hide("slow");
        });

        function SerachValue() {
            if (CheckValidate()) {
                $(".imgloader").show();
				<%= Page.ClientScript.GetPostBackEventReference(btnSearchPincode, String.Empty) %>;
                return true;
            }
            else {
                return false;
            }
        }

        function SetErrorMsg(ErrorMessage) {
            $("#ErrorMessage").text(ErrorMessage);
            $(".error").show();
        }
    }
</script>
</html>
