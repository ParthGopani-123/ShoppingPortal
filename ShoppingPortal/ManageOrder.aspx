<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="ManageOrder.aspx.cs" Inherits="ManageOrder" Title="Order" EnableEventValidation="false" %>

<%@ Register Src="~/CCManageCustomer.ascx" TagName="ManageCustomerCC" TagPrefix="MC" %>
<%@ Register Src="~/CCManageOrderPayment.ascx" TagName="ManageOrderPaymentPopup" TagPrefix="MOP" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        ::placeholder { /* Chrome, Firefox, Opera, Safari 10.1+ */
            color: #d0d0d0 !important;
            opacity: 1; /* Firefox */
        }

        :-ms-input-placeholder { /* Internet Explorer 10-11 */
            color: #d0d0d0 !important;
        }

        ::-ms-input-placeholder { /* Microsoft Edge */
            color: #d0d0d0 !important;
        }

        .divCustomerDetail span {
            display: block;
        }

        .divCustomerDetail {
            margin-top: 5px;
        }

        .lblCustomerName {
            font-weight: bold;
        }

        .divddlnoborder select {
            border: none;
        }

        .divddlnoborder .select2-container--default .select2-selection--single {
            border: none;
            border-bottom: 1px solid #e6e6e6 !important;
            border-radius: 0px;
        }

        .divddlnoborder .select2-container--default.select2-container--open.select2-container--above .select2-selection--single, .select2-container--default.select2-container--open.select2-container--above .select2-selection--multiple, .select2-container--open .select2-selection--single {
            border-bottom: 1px solid #e6e6e6 !important;
        }

        .divddlnoborder .select2-selection:focus, .select2-container--open .select2-selection--single {
            box-shadow: none;
        }

        .tblProduct {
            margin-bottom: -1px;
            border-bottom: none !important;
        }

        .tdProduct {
            padding: 0px !important;
            vertical-align: top !important;
        }

        .txtProductDescription {
            width: 100%;
            border: none;
            border-top: 1px solid #e6e6e6 !important;
            border-bottom: 1px solid #e6e6e6 !important;
            padding: 5px 7px;
            margin-bottom: -7px;
            margin-top: -1px;
        }

        .thQuantity, .tdQuantity {
            width: 120px;
            text-align: right;
        }

        .tdQuantity {
            padding: 0px !important;
            vertical-align: top !important;
        }

        .txtQuantity {
            width: 45px;
            padding: 5px 10px;
            border: none;
            height: 34px;
            text-align: center;
        }

        .thPrice {
            text-align: center;
        }

        .tdPrice {
            padding: 0px !important;
            vertical-align: top !important;
            width: 175px;
        }

        .txtProductSalePrice {
            width: 60px;
            padding: 5px 10px;
            border: none;
            height: 34px;
            text-align: center;
        }

        .divProductPrice {
            border-bottom: 1px solid #e6e6e6;
            margin-top: -1px;
        }

        .addonSpacification {
            padding-top: 13px;
        }

        .addonicon {
            border-top: none;
            padding-left: 0px;
            padding-right: 0px;
            border-color: #e6e6e6;
            border: none;
        }

        .lblProductViewUserPrice {
            float: right;
            margin-right: 4px;
            margin-top: 3px;
        }

        .lblProductWeight {
            float: left;
            font-size: 9px;
            margin-left: 4px;
            margin-top: 5px;
        }

        .lblProductAmount {
            padding: 6px 5px;
            border-bottom: 1px solid #e6e6e6;
            width: 51px !important;
            display: block;
            text-align: right;
        }

        .tdAction {
            width: 30px;
            text-align: center;
            vertical-align: top !important;
        }

        .tdGrandTotal {
            font-size: 17px;
            font-weight: bold;
        }

        .txtCustomerShipCharge, .txtAdjustment {
            text-align: right;
            padding-right: 6px;
        }

        .trCommission {
            color: green;
        }

        .ddlProductItem {
            width: 325px;
            margin-top: -1px;
            margin-bottom: -1px;
            border-radius: 0px;
            border-left: none;
            display: inline;
            padding: 0px 4px !important;
        }

        .divProductImage {
            padding: 5px 0px 0px 5px;
            width: 55px;
            display: inline-block;
            text-align: center;
        }

        .imgProductImage {
            height: 50px;
            width: 50px;
        }

        .lblOrderStatus {
            font-weight: bold;
            font-size: 16px;
            padding: 5px;
            animation: blinkingText 0.8s infinite;
        }

        @keyframes blinkingText {
            0% {
                color: #ff0202;
            }

            49% {
                color: #fc3232;
            }

            50% {
                color: #ff4f4f;
            }

            99% {
                color: #f47c7c;
            }

            100% {
                color: #ffa0a0;
            }
        }

        .divCourierDetail {
            margin-right: -1px;
            padding: 5px;
            min-height: 148px;
        }

        .divTotalDetail {
            margin-top: -1px;
            padding: 5px;
        }

        .tblGTotal {
            float: right;
        }

            .tblGTotal tr td {
                padding: 5px;
            }

        .tdGTotalTit {
            text-align: right;
        }

        .tdGTotalVal {
            text-align: right;
            padding-right: 6px;
            width: 110px;
        }
    </style>
    <style type="text/css">
        .divwidget {
            width: 23%;
            color: #fff;
            display: inline-block;
            padding: 10px;
            font-size: 11px;
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
            font-size: 9px;
            display: block;
            color: #dadada;
        }

        .txtFirmShipCharge {
            width: 80px !important;
        }

        .divCustomer .select2-selection {
            border-top-right-radius: 0;
            border-bottom-right-radius: 0;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Label ID="lbltabHeight1" runat="server" CssClass="lbltabHeight1 hidden" Text="139"></asp:Label>
            <asp:Label ID="lblOrganizationId" runat="server" Visible="false"></asp:Label>
            <asp:Label ID="lblFirmId" runat="server" Visible="false"></asp:Label>
            <asp:Label ID="lblOrdersId" runat="server" Visible="false"></asp:Label>
            <asp:Label ID="lblCustomerId" runat="server" Visible="false"></asp:Label>
            <asp:Label ID="lblCourierIdMenual" runat="server" Visible="false"></asp:Label>
            <div class="row">
                <asp:Panel ID="pnlOrder" runat="server" DefaultButton="lnkSave" class="page-content col-md-12 divtable divloader">
                    <div class="tabHeight1 checkvalidOrderDetail mt-10">
                        <div class="form-horizontal">
                            <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                <div class="form-group">
                                    <div id="divUser" runat="server" class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                        User<span class="text-danger">*</span>
                                        <asp:DropDownList ID="ddlUser" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUser_OnSelectedIndexChanged" onchange="addRegionLoader('divloader')" ZValidation="e=change|v=IsSelect|m=User"></asp:DropDownList>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                        Customer<span class="text-danger">*</span>
                                        <div class="input-group divCustomer">
                                            <asp:DropDownList ID="ddlCustomer" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_OnSelectedIndexChanged" onchange="addRegionLoader('divloader')"></asp:DropDownList>
                                            <span class="input-group-addon">
                                                <asp:LinkButton ID="lnkRefreshCustomer" runat="server" OnClick="lnkRefreshCustomer_Click" OnClientClick="addRegionLoader('divloader')">
                                                    <i class="fa fa-refresh"></i>
                                                </asp:LinkButton>
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                        Whatsapp No<span class="text-danger">*</span>
                                        <asp:TextBox ID="txtWhatsappNo" CssClass="form-control intnumber" ZValidation="e=blur|v=IsMobileNumber|m=Whatsapp No"
                                            runat="server" MaxLength="10" placeholder="Enter Whatsapp No"
                                            AutoPostBack="true" OnTextChanged="txtWhatsappNo_OnTextChanged" onchange="addRegionLoader('divloader')"></asp:TextBox>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                        Alternet No<span class="text-danger">*</span>
                                        <asp:TextBox ID="txtMobileNo" CssClass="form-control intnumber" ZValidation="e=blur|v=IsMobileNumber|m=Mobile No"
                                            runat="server" MaxLength="10" placeholder="Enter Mobile No"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        Name<span class="text-danger">*</span>
                                        <asp:TextBox ID="txtName" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=Name"
                                            runat="server" MaxLength="100" placeholder="Enter Name"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        Address<span class="text-danger">*</span>
                                        <asp:TextBox ID="txtAddress" TextMode="MultiLine" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=Address"
                                            runat="server" placeholder="Enter Address"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                        Pincode<span class="text-danger">*</span>
                                        <asp:TextBox ID="txtPincode" AutoPostBack="true" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=Pincode"
                                            OnTextChanged="txtPincode_OnTextChanged" onchange="addRegionLoader('divloader')" runat="server" MaxLength="10" placeholder="Enter Pincode"></asp:TextBox>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                        City<span class="text-danger">*</span>
                                        <asp:TextBox ID="txtCity" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=City"
                                            runat="server" MaxLength="100" placeholder="Enter City"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                        State<span class="text-danger">*</span>
                                        <asp:TextBox ID="txtState" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=State"
                                            AutoPostBack="true" OnTextChanged="txtState_OnTextChanged" onchange="addRegionLoader('divloader')" runat="server" MaxLength="100" placeholder="Enter State"></asp:TextBox>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                        Country<span class="text-danger">*</span>
                                        <asp:TextBox ID="txtCountry" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=Country"
                                            runat="server" MaxLength="100" placeholder="Enter Country"></asp:TextBox>
                                    </div>
                                </div>
                                <asp:Panel ID="pnlService" runat="server" class="form-group text-center">
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
                                </asp:Panel>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                <div class="form-group">
                                    <div class="col-lg-3 col-md-3 col-sm-4 control-label">
                                    </div>
                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                        <div class="radio-custom">
                                            <asp:RadioButton ID="rdoPrepaid" runat="server" GroupName="CourierType" Text="Prepaid" AutoPostBack="true" OnCheckedChanged="CourierType_OnCheckedChanged" onchange="addRegionLoader('divloader')" />
                                        </div>
                                        <div class="radio-custom ml-20">
                                            <asp:RadioButton ID="rdoCOD" runat="server" GroupName="CourierType" Text="COD" AutoPostBack="true" OnCheckedChanged="CourierType_OnCheckedChanged" onchange="addRegionLoader('divloader')" />
                                        </div>
                                    </div>
                                </div>
                                <div id="divCourier" runat="server" class="form-group">
                                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                        Courier
                                    </label>
                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                        <asp:DropDownList ID="ddlCourier" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCourier_OnSelectedIndexChanged" onchange="addRegionLoader('divloader')"></asp:DropDownList>
                                    </div>
                                </div>
                                <div id="divZone" runat="server" class="form-group">
                                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                        Zone
                                    </label>
                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                        <asp:DropDownList ID="ddlZone" CssClass="form-control" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-12 col-md-12 col-sm-6 col-xs-6 p-0">
                                    <div class="form-group">
                                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                            Date<span class="text-danger">*</span>
                                        </label>
                                        <div class="col-lg-8 col-md-8 col-sm-7">
                                            <asp:TextBox ID="txtDate" CssClass="form-control datepicker" ZValidation="e=blur|v=IsDate|m=Date"
                                                runat="server" placeholder="Enter Date"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-12 col-md-12 col-sm-6 col-xs-6 p-0">
                                    <div class="form-group">
                                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                            Order Source
                                        </label>
                                        <div class="col-lg-8 col-md-8 col-sm-7">
                                            <asp:DropDownList ID="ddlOrderSource" CssClass="form-control" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <table class="table table-bordered nomargin tblProduct">
                            <thead>
                                <tr>
                                    <th>Product</th>
                                    <th class="thPrice">Qty * Price = Total</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptOrderProduct" runat="server" OnItemDataBound="rptOrderProduct_OnItemDataBound">
                                    <ItemTemplate>
                                        <tr>
                                            <td colspan="3" class="p-0"></td>
                                        </tr>
                                        <tr class="trOrderProduct">
                                            <td class="tdProduct">
                                                <asp:Label ID="lblPK" runat="server" Visible="false"></asp:Label>
                                                <div class="divddlnoborder">
                                                    <asp:DropDownList ID="ddlProduct" CssClass="form-control ddlProduct" runat="server" AutoPostBack="true" onchange="addRegionLoader('divloader')" OnSelectedIndexChanged="ddlProduct_OnSelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                                <asp:DropDownList ID="ddlProductItem" CssClass="form-control txtNotFocus ddlNotSearch ddlProductItem" runat="server"></asp:DropDownList>
                                                <asp:Label ID="lbleStockStatus" runat="server" Visible="false"></asp:Label>
                                                <asp:Label ID="lblStockNote" runat="server" Visible="false"></asp:Label>
                                                <asp:Label ID="lblOrderStatus" runat="server" CssClass="lblOrderStatus"></asp:Label>
                                            </td>
                                            <td class="tdPrice">
                                                <div class="input-group divProductPrice">
                                                    <asp:TextBox ID="txtQuantity" CssClass="txtQuantity intnumber" runat="server" AutoPostBack="true" onchange="addRegionLoader('divloader')" OnTextChanged="txtQuantity_OnTextChanged"></asp:TextBox>
                                                    <span class="input-group-addon addonicon addonSpacification">*</span>
                                                    <asp:TextBox ID="txtProductSalePrice" CssClass="txtProductSalePrice intnumber" runat="server"></asp:TextBox>
                                                    <span class="input-group-addon addonicon">=</span>
                                                    <asp:Label ID="lblProductAmount" runat="server" CssClass="lblProductAmount" Text="0"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblProductPurchasePrice" runat="server" Visible="false"></asp:Label>
                                                <asp:Label ID="lblProductUserPrice" runat="server" CssClass="lblProductUserPrice hide"></asp:Label>
                                                <asp:Label ID="lblProductWeight" runat="server" CssClass="lblProductWeight"></asp:Label>
                                                <asp:Label ID="lblProductViewUserPrice" runat="server" CssClass="lblProductViewUserPrice"></asp:Label>
                                            </td>
                                            <td class="tdAction">
                                                <asp:LinkButton ID="lnkDeleteProduct" OnClick="lnkDeleteProduct_OnClick" OnClientClick="addRegionLoader('divloader')" runat="server"><i class="fa fa-times"></i></asp:LinkButton>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdProduct" colspan="3">
                                                <asp:TextBox ID="txtProductDescription" CssClass="txtProductDescription" TextMode="MultiLine" placeholder="Size - Color" runat="server"></asp:TextBox>
                                                <asp:Repeater ID="rptProductImage" runat="server" OnItemDataBound="rptProductImage_OnItemDataBound">
                                                    <ItemTemplate>
                                                        <div class="divProductImage">
                                                            <label class="lblProductImage">
                                                                <asp:Label ID="lblProductImageId" runat="server" Visible="false"></asp:Label>
                                                                <asp:Image ID="imgProductImage" runat="server" class="imgProductImage" />
                                                                <div class="radio-custom">
                                                                    <asp:RadioButton ID="rdoProductImage" runat="server" CssClass="rdoProductImage" />
                                                                </div>
                                                            </label>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <tr>
                                    <td colspan="3" class="p-0">
                                        <div class="col-md-7 divCourierDetail">
                                            <asp:LinkButton ID="lnkAddNewProduct" OnClick="lnkAddNewProduct_OnClick" OnClientClick="addRegionLoader('divloader')" runat="server"><i class="fa fa-plus"></i> Add New</asp:LinkButton>
                                            <div class="form-horizontal mt-15">
                                                <div id="divAWBNo" runat="server" class="form-group">
                                                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                                        AWB No
                                                    </label>
                                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                                        <asp:TextBox ID="txtAWBNo" CssClass="form-control" runat="server" placeholder="Enter AWB No"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div id="divReturnAWBNo" runat="server" class="form-group">
                                                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                                        Return AWB No
                                                    </label>
                                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                                        <asp:TextBox ID="txtReturnAWBNo" CssClass="form-control" runat="server" placeholder="Enter Return AWB No"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                                        Note
                                                    </label>
                                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                                        <asp:TextBox ID="txtDescription" TextMode="MultiLine" CssClass="form-control" runat="server" placeholder="Enter Order Note"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-5 divTotalDetail">
                                            <table class="tblGTotal">
                                                <tr>
                                                    <td class="tdGTotalTit">Sub Total :</td>
                                                    <td class="tdGTotalVal">
                                                        <asp:Label ID="lblSubTotal" runat="server" CssClass="lblSubTotal" Text="0"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="tdGTotalTit">
                                                        <div class="input-group">
                                                            <span class="input-group-addon">Ship Charge </span>
                                                            <asp:TextBox ID="txtFirmShipCharge" runat="server" CssClass="txtFirmShipCharge form-control intnumber" ZValidation="e=blur|v=IsNumber|m=Ship Charge"></asp:TextBox>
                                                        </div>
                                                    </td>
                                                    <td class="tdGTotalVal pr-0">
                                                        <asp:Label ID="lblWeight" runat="server" Visible="false"></asp:Label>
                                                        <asp:Label ID="lblShipCharge" runat="server" Visible="false"></asp:Label>
                                                        <asp:TextBox ID="txtCustomerShipCharge" runat="server" CssClass="txtCustomerShipCharge form-control intnumber" ZValidation="e=blur|v=IsNumber|m=Ship Charge"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="tdGTotalTit">Adjustment :</td>
                                                    <td class="tdGTotalVal pr-0">
                                                        <asp:TextBox ID="txtAdjustment" runat="server" CssClass="txtAdjustment form-control flotnumber SetPointZero" placeholder="Adjustment"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="tdGTotalTit tdGrandTotal">Total :</td>
                                                    <td class="tdGrandTotal tdGTotalVal">
                                                        <asp:Label ID="lblSalePrice" runat="server" CssClass="lblSalePrice" Text="0"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="tdGTotalTit">Commission Adjustment :</td>
                                                    <td class="tdGTotalVal pr-0">
                                                        <asp:TextBox ID="txtCommissionAdjustment" runat="server" CssClass="txtCommissionAdjustment form-control flotnumber SetPointZero" placeholder="Commission Adjustment"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr class="trCommission">
                                                    <td class="tdGTotalTit">Commission :</td>
                                                    <td class="tdGTotalVal">
                                                        <asp:Label ID="lblCommission" runat="server" CssClass="lblCommission" Text="0"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div class="modal-footer footer">
                        <asp:LinkButton ID="lnkCancel" OnClick="lnkCancel_OnClick" runat="server" CssClass="btn btn-raised btn-default clickloader">Cancel</asp:LinkButton>
                        <asp:LinkButton ID="lnkSave" OnClick="lnkSave_OnClick" runat="server" CssClass="lnkSave btn btn-raised btn-black">Save</asp:LinkButton>
                        <asp:LinkButton ID="lnkSaveAndNew" OnClick="lnkSaveAndNew_OnClick" runat="server" CssClass="lnkSaveAndNew btn btn-raised btn-black">Save & New</asp:LinkButton>
                        <asp:LinkButton ID="lnkSaveAndPay" OnClick="lnkSaveAndPay_OnClick" runat="server" CssClass="lnkSaveAndPay btn btn-raised btn-black">Save & Pay</asp:LinkButton>
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="popup" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:LinkButton ID="lnkFackCustomer" runat="server"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="popupCustomer" runat="server" DropShadow="false" BehaviorID="PopupBehaviorID2"
        PopupControlID="pnlCustomer" TargetControlID="lnkFackCustomer" BackgroundCssClass="modalBackground">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlCustomer" runat="server" CssClass="modelpopup col-lg-6 col-md-6 col-sm-8 col-xs-12 p0"
        Style="display: none">
    </asp:Panel>

    <asp:LinkButton ID="lnkFackOrderPayment" runat="server"> </asp:LinkButton>
    <cc1:ModalPopupExtender ID="popupOrderPayment" runat="server" DropShadow="false" BehaviorID="PopupBehaviorID3"
        PopupControlID="pnlpopupOrderPayment" TargetControlID="lnkFackOrderPayment" BackgroundCssClass="modalBackground">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlpopupOrderPayment" runat="server" CssClass="modelpopup col-lg-6 col-md-6 col-sm-8 col-xs-12 p0"
        Style="display: none">
        <MOP:ManageOrderPaymentPopup ID="popupManageOrderPayment" runat="server" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="script" runat="Server">

    <script type="text/javascript">

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Checkpostback);
        jQuery(function () {
            Checkpostback();
        });
        function Checkpostback() {

            $('input').focus(function () {
                $(this).select();
            })

            $(".lnkSave").click(function () {
                if (CheckValidation("checkvalidOrderDetail")) {
                    addLoader('lnkSave');
                    return true;
                }
                else {
                    return false;
                }
            });

            $(".lnkSaveAndNew").click(function () {
                if (CheckValidation("checkvalidOrderDetail")) {
                    addLoader('lnkSaveAndNew');
                    return true;
                }
                else {
                    return false;
                }
            });

            $(".lnkSaveAndPay").click(function () {
                if (CheckValidation("checkvalidOrderDetail")) {
                    addLoader('lnkSaveAndPay');
                    return true;
                }
                else {
                    return false;
                }
            });

            $(".txtQuantity").keyup(function () {
                CountTotal();
            });

            $(".txtProductSalePrice").keyup(function () {
                CountTotal();
            });

            $(".txtFirmShipCharge").keyup(function () {
                CountTotal();
            });

            $(".txtCustomerShipCharge").keyup(function () {
                CountTotal();
            });

            $(".txtAdjustment").keyup(function () {
                CountTotal();
            });

            $(".txtCommissionAdjustment").keyup(function () {
                CountTotal();
            });

            CountTotal();
            function CountTotal() {

                var TotalProductAmount = 0;
                var TotalFirmProductAmount = 0;

                $(".trOrderProduct").each(function () {
                    var Quantity = parseInt($(this).find(".txtQuantity").val());
                    var ProductSalePrice = parseFloat($(this).find(".txtProductSalePrice").val());
                    var ProductUserPrice = parseFloat($(this).find(".lblProductUserPrice").text());

                    if (isNaN(Quantity))
                        Quantity = 0;

                    if (isNaN(ProductSalePrice))
                        ProductSalePrice = 0;

                    if (isNaN(ProductUserPrice))
                        ProductUserPrice = 0;


                    var ProductAmount = Quantity * ProductSalePrice;

                    TotalProductAmount += ProductAmount;
                    TotalFirmProductAmount += (Quantity * ProductUserPrice);

                    $(this).find(".lblProductAmount").text(ProductAmount);
                });

                $(".lblSubTotal").text(TotalProductAmount);

                var ShipCharge = parseFloat($(".txtCustomerShipCharge").val());
                var FirmShipCharge = parseFloat($(".txtFirmShipCharge").val());
                var Adjustment = parseFloat($(".txtAdjustment").val());

                if (isNaN(ShipCharge))
                    ShipCharge = 0;

                if (isNaN(FirmShipCharge))
                    FirmShipCharge = 0;

                if (isNaN(Adjustment))
                    Adjustment = 0;

                var SalePrice = TotalProductAmount + ShipCharge + Adjustment;

                $(".lblSalePrice").text(SalePrice);
                var CommissionAdjustment = parseFloat($(".txtCommissionAdjustment").val());
                if (isNaN(CommissionAdjustment))
                    CommissionAdjustment = 0;

                $(".lblCommission").text(SalePrice - (TotalFirmProductAmount + FirmShipCharge) + CommissionAdjustment);
            }

            $(".lblProductImage").click(function () {
                $(this).parent().parent().each(function () {
                    $(this).find('input:radio').prop("checked", false);
                });

                $(this).find('input:radio').eq(0).prop("checked", true);
            });
        }

    </script>

</asp:Content>
