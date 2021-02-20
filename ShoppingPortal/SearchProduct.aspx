<%@ Page Language="C#" AutoEventWireup="true" Title="Search Product" CodeFile="SearchProduct.aspx.cs" Inherits="SearchProduct" EnableEventValidation="false" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link rel='shortcut icon' type='image/x-icon' href="<%= string.Format("{0}SystemImages/icon.ico", CU.StaticFilePath) %>" />
    <link rel='apple-touch-icon' href="<%= string.Format("{0}SystemImages/icon.ico", CU.StaticFilePath) %>" />
    <link rel='stylesheet' href="<%= string.Format("{0}plugins/font-awesome/css/font-awesome.min.css", CU.StaticFilePath) %>" />

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
            color: #888;
        }

        input:-moz-placeholder {
            color: #888
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
            margin-bottom: 5px;
            border: 1px solid #fff;
        }

        .close {
            color: #fff;
            text-decoration: none;
            font-size: 12px;
            float: right;
            margin-top: -7px;
            margin-right: -5px;
        }

        .result-notfound {
            font-size: 29px;
            font-weight: bold;
            color: #c10606;
            padding: 20px;
            border: 1px solid #c10606;
        }

        .imgloader {
            display: none;
            height: 50px;
            width: 50px;
            margin-right: 18px;
            top: 5px;
        }

        .tblProduct {
            width: 100%;
            margin-bottom: 10px;
        }

            .tblProduct tbody tr th {
                padding: 5px;
                background-color: #888;
                color: #fff;
                border: 1px solid #ccc;
            }

            .tblProduct tbody tr td {
                padding: 5px;
                border: 1px solid #ccc;
            }

        .text-left {
            text-align: left;
        }

        .pull-right {
            float: right;
        }

        /* Image Gallery */
        div.gallery {
            margin: 5px;
            border: 1px solid #ccc;
            float: left;
            width: 180px;
        }

            div.gallery:hover {
                border: 1px solid #777;
            }

            div.gallery img {
                width: 100%;
                height: auto;
                cursor: pointer;
            }

        div.desc {
            text-align: center;
            display: flex
        }

        .aProductImage {
            text-decoration: none;
            width: 50%;
            height: 40px;
            display: inherit;
            display: block;
            font-size: 30px;
            color: #1abe9e;
            border: 1px solid #1abe9e;
            border-radius: 10px;
            padding: 5px 0px 0px 0px;
            margin: 2px 5px;
        }

        ::-webkit-input-placeholder {
            color: #ccc;
        }

        input:-moz-placeholder {
            color: #ccc;
        }

        .divViewAllMatchProduct {
            text-align: right;
            margin: 6px 3px;
        }

        .outOfStock {
            background-color: #f9c3c3;
        }

        .waitingStock {
            background-color: #c3cff9;
        }

        .tdSearchProduct {
            padding: 0px;
        }

        .aSearchProduct {
            text-decoration: none;
            font-size: 24px;
        }

        .ml-10 {
            margin-left: 10px;
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

        .txtDescription {
            width: 1px;
            height: 1px;
            border: none;
            color: #FFF;
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
                <asp:Label ID="lblErrorMessage" runat="server" CssClass="lblErrorMessage" Style="display: none;" Text=""></asp:Label>
                <asp:Label ID="lblOrganizationId" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="lbleOrganization" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="lblFirmId" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="lbleDesignation" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="lblViewProductVendor" runat="server" Visible="false"></asp:Label>
                
                <asp:Label ID="lblPriceListId" runat="server" Visible="false"></asp:Label>
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
                        <a id="aSearchService" runat="server" target="_blank" class="aLink">
                            <div class="widget no-border p-15 bg-success media">
                                <div class="media-body">
                                    <h4 class="m-0">Pincode</h4>
                                </div>
                            </div>
                        </a>
                    </div>

                </div>
                <%--<h1 class="h1">Search Pincode</h1>--%>
                <div class="content">
                    <div class="flexsearch">
                        <div class="flexsearch--wrapper">
                            <div class="flexsearch--form">
                                <div class="flexsearch--input-wrapper">
                                    <asp:TextBox ID="txtProduct" runat="server" CssClass="txtProduct flexsearch--input" autocomplete="off" placeholder="Enter Product / Vendor Code"></asp:TextBox>
                                </div>
                                <asp:Button ID="btnSearchProduct" OnClick="btnSearchProduct_OnClick" CssClass="btnSearchProduct flexsearch--submit" runat="server" Text="&#10140;" />
                                <img src="<%= CU.StaticFilePath %>SystemImages/loading.gif" class="imgloader flexsearch--submit" />
                            </div>
                        </div>
                    </div>
                    <div class="divViewAllMatchProduct">
                        <asp:CheckBox ID="chkPrice" Text="Price" runat="server" />
                        <asp:CheckBox ID="chkGlobal" Text="Global" runat="server" />
                        <a class="ml-10" href="Login.aspx">Login</a>
                        <a id="aWhatsAppLink" target="_blank" class="ml-10 aWhatsAppLink" runat="server">Share</a>
                    </div>
                    <div class="searchingresult">
                        <div class="error">
                            <sapn id="ErrorMessage"></sapn>
                            <a href="javascript:;" class="close">&times;</a>
                        </div>
                        <div id="divSearchResult" runat="server" class="result">
                            <asp:Repeater ID="rptProduct" runat="server" OnItemDataBound="rptProduct_OnItemDataBound">
                                <ItemTemplate>
                                    <div runat="server" id="divProduct">
                                        <table class="tblProduct">
                                            <thead>
                                                <tr>
                                                    <th>Product Code</th>
                                                    <th id="thStock" runat="server">Stock</th>
                                                    <asp:Repeater ID="rptPriceListHead" runat="server" OnItemDataBound="rptPriceListHead_OnItemDataBound">
                                                        <ItemTemplate>
                                                            <th id="thPriceListHead" runat="server">
                                                                <asp:Literal ID="ltrPriceListName" runat="server"></asp:Literal></th>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                    <th id="thPrice" runat="server">Price</th>
                                                    <th id="thResalerPrice" runat="server">Resaler Price</th>
                                                    <th id="thVendorPrice" runat="server">Vendor Price</th>
                                                    <th id="thVendorName" runat="server">Vendor Name</th>
                                                    <th id="thViewProduct" runat="server">*</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblProductCode" runat="server"></asp:Label>&nbsp
                                                        <a id="aWhatsAppProduct" target="_blank" class="aWhatsAppProduct" runat="server"><i class="fa fa-whatsapp"></i></a>

                                                    </td>
                                                    <td id="tdStock" runat="server">
                                                        <asp:Label ID="lblStock" Font-Bold="true" runat="server"></asp:Label><br />
                                                        <asp:Label ID="lblStockNote" Font-Bold="true" runat="server"></asp:Label>
                                                    </td>
                                                    <asp:Repeater ID="rptPriceList" runat="server" OnItemDataBound="rptPriceList_OnItemDataBound">
                                                        <ItemTemplate>
                                                            <td id="tdPriceList" runat="server">
                                                                <asp:Literal ID="ltrPriceListPrice" runat="server"></asp:Literal></td>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                    <td id="tdPrice" runat="server">
                                                        <asp:Label ID="lblPrice" runat="server"></asp:Label></td>
                                                    <td id="tdResalerPrice" runat="server">
                                                        <asp:Label ID="lblRecelerPrice" runat="server"></asp:Label></td>
                                                    <td id="tdVendorPrice" runat="server">
                                                        <asp:Label ID="lblVendorPrice" runat="server"></asp:Label></td>
                                                    <td id="tdVendorName" runat="server">
                                                        <asp:Label ID="lblVendorName" runat="server"></asp:Label></td>
                                                    <td id="tdViewProduct" runat="server" class="tdSearchProduct">
                                                        <a id="aSearchProduct" runat="server" class="aSearchProduct" target="_blank">&#x21e6;</a>
                                                    </td>
                                                </tr>
                                                <tr id="trDescription" runat="server">
                                                    <td colspan="20" class="text-left">
                                                        <asp:Label ID="lblDescription" runat="server"></asp:Label>
                                                        <asp:TextBox ID="txtDescription" CssClass="txtDescription pull-right" runat="server"></asp:TextBox>
                                                        <br /><br /><button class="btnCopy">Copy text</button>
                                                    </td>
                                                </tr>
                                                <tr id="trImage" runat="server">
                                                    <td colspan="20" class="text-left">
                                                        <asp:Repeater ID="rptProductImage" runat="server" OnItemDataBound="rptProductImage_OnItemDataBound">
                                                            <ItemTemplate>
                                                                <div id="divImage" runat="server" class="gallery">
                                                                    <asp:Image ID="imgProductImage" runat="server" alt="OCTFIS" Width="180px" Height="180px" />
                                                                    <div class="desc">
                                                                        <a id="aProductImage" target="_blank" class="aProductImage" runat="server" download><i class="fa fa-download"></i></a>
                                                                        <a id="aShareWhatsApp" target="_blank" class="aProductImage" runat="server" download><i class="fa fa-whatsapp"></i></a>
                                                                    </div>
                                                                    <%--<asp:Button ID="btnDownloadProduct" OnClick="btnDownloadProduct_OnClick" 
                                                                    CssClass="btnDownloadProduct " runat="server" Text="Download Image" />--%>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <div id="divNoDataFound" runat="server" class="result-notfound">
                            <span>No Data Found</span>
                        </div>
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

        $(".txtProduct").focus();

        $('.btnSearchProduct').click(function () {
            if (CheckValidate()) {
                $(".imgloader").show();
                return true;
            }
            else
                return false;
        });

        //$(".txtProduct").on('keyup contextmenu input', function (e) {
        //    var Product = $(".txtProduct").val();
        //    var length = 5;
        //    if (isNaN(Product))
        //        length = 6;

        //    if (Product.length >= length) {
        //        SerachValue();
        //    }
        //});

        function CheckValidate() {
            if ($(".txtProduct").val().trim() == "") {
                SetErrorMsg("Please Enter Product / Vendor Code");
                $(".txtProduct").val("");
                $(".txtProduct").focus();
                return false;
            }
            else {
                return true;
            }
        }

        $(".close").click(function () {
            $(this).parent().hide("slow");
        });

        function SerachValue() {
            if (CheckValidate()) {
                $(".imgloader").show();
				<%= Page.ClientScript.GetPostBackEventReference(btnSearchProduct, String.Empty) %>;
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

        $(".btnCopy").click(function () {
            var copyText = document.getElementById($(this).parent().find(".txtDescription").attr("id"));
            copyText.select();
            copyText.setSelectionRange(0, 99999)
            document.execCommand("copy");
            return false;
        });
    }
</script>
</html>
