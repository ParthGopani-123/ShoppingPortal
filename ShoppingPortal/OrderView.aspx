<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="OrderView.aspx.cs" Inherits="OrderView" Title="Order" EnableEventValidation="false" %>

<%@ Register Src="~/CCManageOrderPayment.ascx" TagName="ManageOrderPaymentPopup" TagPrefix="MOP" %>
<%@ Register Src="~/CCExcelExport.ascx" TagName="ExcelExportPopup" TagPrefix="EE" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="<%= CU.StaticFilePath %>plugins/jqueryTypeahead-Autocompletion/css/typeahead.tagging.css" rel="stylesheet" />

    <style type="text/css">
        .divProintOrderSlipBody {
            max-height: 406px;
            overflow: auto;
        }

        .lblGrid {
            display: block;
        }

        .grdOrder tbody tr td {
            font-size: 12px;
        }

        .tdDate {
            width: 112px !important;
        }

        .tdCourier {
            width: 140px !important;
        }

        .tdCustomer {
            width: 180px !important;
        }

        .text-muted {
            color: #969696;
            font-size: 11px;
        }

        .lblOrderStatus {
            display: inline-block;
            width: 98px;
            padding: 4px 0px 2px 0px;
            margin-bottom: 2px;
            color: #fff;
            text-align: center;
            font-size: 11px;
            line-height: 1;
            border-radius: 3px;
        }

        .ddlOrderStatus {
            padding: 0px 3px !important;
            height: 26px;
        }

        .txtAWBNo {
            padding: 3px 3px;
            height: 26px;
            text-align: center;
        }

        .text-desc {
            font-size: 11px;
        }

        .lblOCOrderDetail {
            display: block;
        }

        .tdOCOrderDetail {
            min-width: 120px;
        }

        .lblPaymentStatus {
            display: inline-block;
            width: 98px;
            padding: 4px 0px 2px 0px;
            margin-bottom: 2px;
            color: #fff;
            text-align: center;
            font-size: 11px;
            line-height: 1;
            border-radius: 3px;
        }

        .stsPayNotPaid {
            background-color: #da0000;
        }

        .stsPayParPaid {
            background-color: #e6f14c;
            color: black;
        }

        .stsPayPaid {
            background-color: #1e5403;
        }

        .lblStatusColumnName {
            font-size: 12px;
            color: #8a8a8a;
        }

        .tblOrderComplain thead th {
            background: #767676;
            color: #ffffff;
        }

        .lblTrakStatus {
            display: block;
            font-size: 13px;
        }

        .lblTrakStatusLocation {
            display: block;
            font-size: 11px;
            font-style: italic
        }
    </style>
    <style type="text/css">
        .stsDraft {
            background-color: #e8dee4;
            color: black;
        }

        .stsConfirm {
            background-color: #a6ff79;
            color: black;
        }

        .stsPrinted {
            background-color: #4fe006;
            color: black;
        }

        .stsDispatch {
            background-color: #3dad05;
        }

        .stsInTranst {
            background-color: #2e7f05;
        }

        .stsDelivered {
            background-color: #1e5403;
        }

        .stsUnDelivered {
            background-color: #E10585;
        }

        .stsAvailabaleRTO {
            background-color: #ff6868;
        }

        .stsRTO {
            background-color: #ff6868;
        }

        .stsRTODelivered {
            background-color: #de0000;
        }

        .stsRPickup {
            background-color: #626df7;
        }

        .stsRPickupDelivered {
            background-color: #000c9c;
        }

        .stsUnDefined {
            background-color: #bd2a2a;
        }

        .stsCancel {
            background-color: #da0000;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="lnkTrackCurior" />
        </Triggers>
        <ContentTemplate>
            <asp:Label ID="lbltabHeight1" runat="server" CssClass="lbltabHeight1 hidden" Text="151"></asp:Label>
            <asp:Label ID="lbltabHeightSmall1" runat="server" CssClass="lbltabHeightSmall1 hidden"
                Text="190"></asp:Label>
            <asp:Label ID="lblFirmId" runat="server" Visible="false"></asp:Label>
            <asp:Label ID="lblOrganizationId" runat="server" Visible="false"></asp:Label>
            <asp:Label ID="lbleOrganization" runat="server" Visible="false"></asp:Label>
            <asp:Label ID="lblUsersId" runat="server" Visible="false"></asp:Label>
            <asp:Label ID="lblOrdersId" runat="server" Visible="false"></asp:Label>
            <asp:Label ID="lblOrderStatusMode" runat="server" Visible="false"></asp:Label>
            <div class="row">
                <div class="page-header clearfix">
                    <div class="col-lg-9 col-md-9 col-sm-9 col-xs-12 p0">
                        <div class="btn-group">
                            <a id="lnkAdd" tooltip="Add (alt+n)" runat="server" href="ManageOrder.aspx" target="_blank" class="lnkAdd btn btngroup btn-add tooltips" data-toggle="tooltip">
                                <i class="fa fa-plus"></i>
                            </a>
                            <asp:LinkButton ID="lnkEdit" ToolTip="Edit (alt+u)" allowon="1" runat="server" OnClick="lnkEdit_OnClick"
                                CssClass="lnkEdit btn btngroup btn-edit tooltips clickloader" data-toggle="tooltip">
                            <i class="fa fa-edit"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkRefresh" ToolTip="Refresh" runat="server" OnClick="lnkRefresh_OnClick"
                                CssClass="btn btngroup btn-refresh tooltips clickloader" data-toggle="tooltip">
                            <i class="fa fa-refresh"></i>
                            </asp:LinkButton>
                        </div>
                        <div class="btn-group">
                            <asp:LinkButton ID="lnkSetOrderStatus" ToolTip="Set Order Status" runat="server" OnClick="lnkSetOrderStatus_OnClick"
                                CssClass="btn btngroup btn-edit tooltips clickloader" data-toggle="tooltip">
                            <i class="fa fa-wpexplorer"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkPrintOrderSlip" ToolTip="Print Order Slip" runat="server" OnClick="lnkPrintOrderSlip_OnClick"
                                CssClass="btn btngroup btn-delete tooltips clickloader" data-toggle="tooltip">
                            <i class="fa fa-print"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkPrintOrderProduct" ToolTip="Print Order Product" runat="server" OnClick="lnkPrintOrderProduct_OnClick"
                                CssClass="btn btngroup btn-extra5 tooltips clickloader" data-toggle="tooltip">
                            <i class="fa fa-cart-arrow-down"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkOrderComplain" ToolTip="Order Complain" runat="server" OnClick="lnkOrderComplain_OnClick"
                                CssClass="btn btngroup btn-deactive tooltips clickloader" data-toggle="tooltip">
                            <i class="fa fa-envelope-o"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkOrderPayment" ToolTip="Order Payment" allowon="1" runat="server" OnClick="lnkOrderPayment_OnClick"
                                CssClass="btn btngroup btn-add tooltips clickloader" data-toggle="tooltip">
                            <i class="fa fa-inr"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkExcelPayment" ToolTip="Excel Order Payment" runat="server" OnClick="lnkExcelPayment_OnClick"
                                CssClass="lnkExcelImport btn btngroup clickloader btn-add tooltips" data-toggle="tooltip">
                            <i class="fa fa-money"></i>
                            </asp:LinkButton>
                        </div>
                        <div class="btn-group">
                            <asp:LinkButton ID="lnkTrackCurior" ToolTip="Order Traking" runat="server" OnClick="lnkTrackCurior_OnClick"
                                CssClass="btn btngroup btn-import tooltips clickloader" data-toggle="tooltip">
                            <i class="fa fa-truck"></i>
                            </asp:LinkButton>
                        </div>

                        <div class="btn-group pull-right mr-5">
                            <asp:LinkButton ID="lnkExcelChangeStatus" ToolTip="Update Status" runat="server" OnClick="lnkExcelChangeStatus_OnClick"
                                CssClass="lnkExcelImport btn btngroup btn-import tooltips" data-toggle="tooltip">
                            <i class="fa fa-eye"></i>
                            </asp:LinkButton>
                        </div>
                        <div class="btn-group pull-right mr-5">
                            <asp:LinkButton ID="lnkExcelExport" ToolTip="Excel Export" runat="server" OnClick="lnkExcelExport_Click"
                                CssClass="lnkExcelExport btn btngroup btn-export tooltips" data-toggle="tooltip">
                            <i class="fa fa-file-excel-o"></i>
                            </asp:LinkButton>
                        </div>
                    </div>
                    <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12 search div-master-search div-master-search-xs">
                        <asp:Label ID="lblCount" ToolTip="Count" runat="server" Text="10" CssClass="pull-left mr-5 btn btn-icon btn-total tooltips"
                            data-toggle="tooltip">
                        </asp:Label>
                        <asp:Panel ID="pnlSearchText" runat="server" DefaultButton="lnkSearch" class="input-group">
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control master-search" placeholder="Search..."></asp:TextBox>
                            <a note-colspan="note-colspan-class" class="tooltips input-group-addon aShowSearch btn btn-master-search ml5"
                                data-original-title="Search" data-toggle="tooltip"><i note-colspan="note-colspan-class"
                                    class="fa fa-chevron-down"></i></a>
                        </asp:Panel>
                    </div>
                    <div note-colspan="note-colspan-class" class="search-tools divShowSearch note-colspan-class">
                        <asp:Panel ID="pnlSearch" runat="server" DefaultButton="lnkSearch" class="divsearchloader"
                            note-colspan="note-colspan-class">
                            <div id="divFirm" runat="server" class="padbm" note-colspan="note-colspan-class">
                                <asp:DropDownList ID="ddlFirm" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFirm_OnSelectedIndexChanged"
                                    onchange="addRegionLoader('divsearchloader')" note-colspan="note-colspan-class">
                                </asp:DropDownList>
                            </div>
                            <div id="divUser" runat="server" class="padbm" note-colspan="note-colspan-class">
                                <asp:DropDownList ID="ddlUser" CssClass="form-control" runat="server" note-colspan="note-colspan-class">
                                </asp:DropDownList>
                            </div>
                            <%--<div class="padbm" note-colspan="note-colspan-class">
                                <asp:DropDownList ID="ddlCustomer" CssClass="form-control" runat="server" note-colspan="note-colspan-class">
                                </asp:DropDownList>
                            </div>--%>
                            <div class="padbm" note-colspan="note-colspan-class">
                                <asp:DropDownList ID="ddlOrderSource" CssClass="form-control" runat="server" note-colspan="note-colspan-class">
                                </asp:DropDownList>
                            </div>
                            <div class="padbm" note-colspan="note-colspan-class">
                                <asp:DropDownList ID="ddlCourier" CssClass="form-control" runat="server" note-colspan="note-colspan-class">
                                </asp:DropDownList>
                            </div>
                            <div class="padbm mt-5" note-colspan="note-colspan-class">
                                <span>Status Type</span>
                                <asp:ListBox ID="lstStatusTypeIn" SelectionMode="Multiple" CssClass="form-control" runat="server" note-colspan="note-colspan-class"></asp:ListBox>
                            </div>
                            <div class="padbm mt-5" note-colspan="note-colspan-class">
                                <span>Status Type Not</span>
                                <asp:ListBox ID="lstStatusTypeNotIn" SelectionMode="Multiple" CssClass="form-control" runat="server" note-colspan="note-colspan-class"></asp:ListBox>
                            </div>
                            <div class="padbm" note-colspan="note-colspan-class">
                                <div class="input-group">
                                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control datepicker"
                                        note-colspan="note-colspan-class"></asp:TextBox>
                                    <span class="input-group-addon" note-colspan="note-colspan-class">to</span>
                                    <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control datepicker"
                                        note-colspan="note-colspan-class"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-6 col-sm-6 col-xs-6 col-sm-6 searchchk" note-colspan="note-colspan-class">
                                <div class="checkbox-custom" note-colspan="note-colspan-class">
                                    <asp:CheckBox ID="chkPrepaid" runat="server" note-colspan="note-colspan-class" CssClass="chk"
                                        Checked="true" Text="Prepaid" />
                                </div>
                            </div>
                            <div class="col-md-6 col-sm-6 col-xs-6 col-sm-6 searchchk" note-colspan="note-colspan-class">
                                <div class="checkbox-custom" note-colspan="note-colspan-class">
                                    <asp:CheckBox ID="chkCOD" runat="server" note-colspan="note-colspan-class" CssClass="chk"
                                        Checked="true"
                                        Text="COD" />
                                </div>
                            </div>
                            <div class="padbm text-right">
                                <asp:LinkButton ID="lnkSearch" OnClientClick="addRegionLoader('divloader')"
                                    OnClick="lnkRefresh_OnClick" class="btn btn-warning btnsearch" runat="server"><i class="fa fa-filter"></i> Filter</asp:LinkButton>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
                <div class="page-content col-md-12 divtable divloader">
                    <div class="table-responsive tabHeight1 fixheight2">
                        <asp:GridView ID="grdOrder" AllowPaging="false" runat="server" OnRowDataBound="grdOrder_OnRowDataBound"
                            AutoGenerateColumns="False"
                            class="grdOrder table table-bordered table-hover nomargin selectonrowclick fixheader">
                            <Columns>
                                <asp:TemplateField HeaderStyle-CssClass="hide" ItemStyle-CssClass="hide">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelect" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="OrdersId" HeaderText="OrdersId" HeaderStyle-CssClass="hide"
                                    ItemStyle-CssClass="hide" />
                                <asp:TemplateField HeaderText="Order" ItemStyle-CssClass="valigntop tdDate">
                                    <ItemTemplate>
                                        <a id="aEditOrder" class="lnkEditOrder" runat="server" target="_blank"></a>
                                        <asp:Literal ID="ltrOrder" runat="server"></asp:Literal>
                                        <asp:Label ID="lblOrderDate" runat="server" CssClass="lblGrid"></asp:Label>
                                        <asp:Label ID="lblTotalAmount" runat="server" CssClass="lblGrid"></asp:Label>
                                        <asp:Label ID="lblUserName" runat="server" CssClass="lblGrid text-muted"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Courier" ItemStyle-CssClass="valigntop tdCourier text-center">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRowOrderId" runat="server" class="lblRowOrderId hide"></asp:Label>
                                        <asp:DropDownList ID="ddlOrderStatus" CssClass="ddlOrderStatus ddlNotSearch form-control" runat="server"></asp:DropDownList>
                                        <asp:Label ID="lblOrderStatus" runat="server" class="lblGrid lblOrderStatus"></asp:Label>
                                        <asp:Label ID="lblCourierType" runat="server" CssClass="lblGrid"></asp:Label>
                                        <asp:TextBox ID="txtAWBNo" CssClass="form-control txtAWBNo" runat="server"></asp:TextBox>
                                        <asp:Label ID="lblAWBNo" runat="server" CssClass="lblGrid"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Customer" ItemStyle-CssClass="valigntop tdCustomer">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCustomerName" runat="server" CssClass="lblGrid"></asp:Label>
                                        <asp:Label ID="lblCustomerCity" runat="server" CssClass="lblGrid"></asp:Label>
                                        <asp:Label ID="lblCustomerMobile" runat="server" CssClass="lblGrid"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Product" ItemStyle-CssClass="valigntop tdCustomer">
                                    <ItemTemplate>
                                        <asp:Label ID="lblProductList" runat="server" CssClass="lblGrid"></asp:Label>
                                        <asp:Label ID="lblTotalPaidAmount" runat="server" CssClass="lblGrid"></asp:Label>
                                        <asp:Label ID="lblPaymentStatus" runat="server" class="lblGrid lblPaymentStatus"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Traking" ItemStyle-CssClass="valigntop">
                                    <ItemTemplate>
                                        <asp:Label ID="lblgrdOrdersId" runat="server" Visible="false"></asp:Label>
                                        <asp:Label ID="lblCarrierCode" runat="server" Visible="false"></asp:Label>
                                        <asp:Label ID="lblTraking" runat="server" Visible="false"></asp:Label>
                                        <a id="aTrakingInfo" runat="server" target="_blank" class="lblTrakStatus"></a>
                                        <asp:Label ID="lblLastTrakingInfo" runat="server" CssClass="lblTrakStatusLocation"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="text-center text-danger">
                                    <br />
                                    <i class="fa fa-4x fa-smile-o"></i>
                                    <h3>Sorry, No Data Found.</h3>
                                </div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                    <div id="divPaging" runat="server" class="col-md-12 col-sm-12 col-xs-12 div-paging">
                        <asp:LinkButton ID="lnkFirst" OnClick="lnkFirst_Click" OnClientClick="addRegionLoader('divloader')"
                            runat="server" ToolTip="First Page" CssClass="fa fa-fast-backward btn-paging tooltips"></asp:LinkButton>
                        <asp:LinkButton ID="lnkPrev" OnClick="lnkPrev_Click" OnClientClick="addRegionLoader('divloader')"
                            runat="server" ToolTip="Previous Page" CssClass="fa fa-backward btn-paging tooltips"></asp:LinkButton>
                        <asp:TextBox ID="txtGotoPageNo" Text="1" runat="server" CssClass="txt-paging" OnTextChanged="txtGotoPageNo_OnTextChange"
                            OnChange="addRegionLoader('divloader')" AutoPostBack="true"></asp:TextBox>
                        <asp:LinkButton ID="lnkNext" OnClick="lnkNext_Click" OnClientClick="addRegionLoader('divloader')"
                            runat="server" ToolTip="Next Page" CssClass="fa fa-forward btn-paging tooltips"></asp:LinkButton>
                        <asp:LinkButton ID="lnkLast" OnClick="lnkLast_Click" OnClientClick="addRegionLoader('divloader')"
                            runat="server" ToolTip="Last Page" CssClass="fa fa-fast-forward btn-paging tooltips"></asp:LinkButton>
                        <asp:DropDownList ID="ddlRecordPerPage" runat="server" CssClass="ddlNotSearch ml-5 ddl-paging"
                            AutoPostBack="True" OnChange="addRegionLoader('divloader')" OnSelectedIndexChanged="ddlRecordPerPage_LoadMember">
                        </asp:DropDownList>
                        <span class="lbl-paging">Records / Page</span>
                        <label class="pull-right mt2 lbl-paging hidden-xs">
                            <asp:Literal ID="ltrTotalContent" runat="server"></asp:Literal>
                        </label>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="popup" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnChangeStatus" />
            <asp:PostBackTrigger ControlID="btnSendOrderComplain" />
        </Triggers>
        <ContentTemplate>
            <asp:LinkButton ID="lnkFackPrintOrderSlip" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupPrintOrderSlip" runat="server" DropShadow="false" BehaviorID="PopupBehaviorID1"
                PopupControlID="pnlpopupPrintOrderSlip" TargetControlID="lnkFackPrintOrderSlip" BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlpopupPrintOrderSlip" runat="server" DefaultButton="btnPrint" CssClass="modelpopup col-lg-8 col-md-8 col-sm-8 col-xs-12 p0"
                Style="display: none">
                <div class="modal-dialog">
                    <div class="modal-content darkmodel">
                        <div class="modal-header bg-black">
                            <button type="button" class="ClosePopup close">
                                ×</button>
                            <h4 class="modal-title">
                                <span>Print Order Slip</span></h4>
                        </div>
                        <div class="modal-body divloaderorderslip">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <asp:Panel ID="pnlNoofPrint" DefaultButton="lnkNoofPrint" runat="server" class="col-md-3">
                                        <label class="">
                                            No of Print
                                        </label>
                                        <div class="input-group">
                                            <asp:TextBox ID="txtNoofPrint" runat="server" CssClass="form-control"></asp:TextBox>
                                            <asp:LinkButton ID="lnkNoofPrint" runat="server" OnClick="lnkNoofPrint_OnClick" OnClientClick="addRegionLoader('divloaderorderslip')" CssClass="input-group-addon"><i class="fa fa-play"></i></asp:LinkButton>
                                        </div>
                                    </asp:Panel>
                                    <div class="col-md-9">
                                        <div class="divProintOrderSlipBody divPrintOrderSlip">
                                            <style type="text/css">
                                                /* Order Slip */
                                                .divOrderSlip {
                                                    width: 565px;
                                                    margin: 0 auto;
                                                }

                                                .tblOrderSlip {
                                                    width: 100%;
                                                    border: 1px solid #000;
                                                    margin-top: 5px;
                                                    border-spacing: 0;
                                                    border-collapse: collapse;
                                                }

                                                    .tblOrderSlip tr td {
                                                        font-size: 14px;
                                                        vertical-align: top;
                                                    }

                                                .tdCustDetail {
                                                    padding-left: 5px;
                                                    text-transform: uppercase;
                                                }

                                                .tdCustTitle {
                                                    width: 90px;
                                                }

                                                .tdOrderDetail {
                                                    width: 210px;
                                                    vertical-align: top;
                                                    border-left: 1px solid #000;
                                                    padding-top: 5px;
                                                }

                                                .lblOrderDetail {
                                                    display: block;
                                                    text-align: -webkit-right;
                                                    margin-bottom: 0px;
                                                    padding-right: 5px;
                                                }

                                                .lblOrderAmountTitle {
                                                    display: block;
                                                    text-align: center;
                                                    border-top: 1px solid #000;
                                                    border-bottom: 1px solid #000;
                                                    padding: 2px;
                                                    font-weight: bold;
                                                }

                                                .lblOrderAmount {
                                                    display: block;
                                                    text-align: center;
                                                    border-bottom: 1px solid #000;
                                                    padding: 5px;
                                                    font-size: 18px;
                                                    background-color: black;
                                                    color: white;
                                                    font-weight: 900;
                                                }

                                                .lblOrderCouriarType {
                                                    display: block;
                                                    text-align: center;
                                                    border-bottom: 1px solid #000;
                                                    padding: 8px;
                                                    font-size: 15px;
                                                    text-decoration: underline;
                                                    font-weight: bold;
                                                    background-color: #dcdcdc;
                                                }

                                                .lblOrderCouriarName {
                                                    display: block;
                                                    text-align: center;
                                                    border-bottom: 1px solid #000;
                                                    padding: 2px;
                                                    font-size: 15px;
                                                }

                                                .lblOrderFirm {
                                                    display: block;
                                                    text-align: center;
                                                    margin-top: 50px;
                                                }

                                                .tdOrderCustAddress {
                                                    height: 80px;
                                                }

                                                .tdOrderFirmName {
                                                    border-top: 1px solid #000;
                                                }

                                                .tdItemTitle {
                                                    border-top: 1px solid #000;
                                                    border-bottom: 1px solid #000;
                                                    padding: 2px 5px;
                                                    font-weight: bold;
                                                }

                                                .tdItemDetail {
                                                    padding: 0px;
                                                }

                                                .tdOrderTotalQuantity {
                                                    text-align: right;
                                                    font-weight: bold;
                                                    vertical-align: middle !important;
                                                    font-size: 18px;
                                                    padding-right: 5px;
                                                    border-left: 1px solid #000;
                                                    background-color: black;
                                                    color: white;
                                                }

                                                .divItemName {
                                                    /*width: 269px;*/
                                                    display: inline-block;
                                                    padding: 5px;
                                                    border-left: 1px solid #000;
                                                    border-bottom: 1px solid #000;
                                                    margin-left: -1px;
                                                    margin-bottom: -1px;
                                                }

                                                .divPageSaparator {
                                                    border-bottom: 1px dotted;
                                                    margin: 10px 0;
                                                }

                                                @media print {
                                                    .divPageBreak {
                                                        page-break-after: always;
                                                    }
                                                }
                                            </style>
                                            <div class="divOrderSlip">
                                                <asp:Repeater ID="rptPrintOrderSlip" runat="server" OnItemDataBound="rptPrintOrderSlip_OnItemDataBound">
                                                    <ItemTemplate>
                                                        <div style="min-height: 200px">
                                                            <table class="tblOrderSlip">
                                                                <tr>
                                                                    <td colspan="2" class="tdCustDetail pt-5"><b>DELIVER TO :</b></td>
                                                                    <td rowspan="9" class="tdOrderDetail">
                                                                        <asp:Label ID="lblOrderDate" runat="server" CssClass="lblOrderDetail"></asp:Label>
                                                                        <asp:Label ID="lblOrderNo" runat="server" CssClass="lblOrderDetail"></asp:Label>
                                                                        <asp:Label ID="lblOrderAmountTitle" runat="server" CssClass="lblOrderAmountTitle"></asp:Label>
                                                                        <asp:Label ID="lblOrderAmount" runat="server" CssClass="lblOrderAmount"></asp:Label>
                                                                        <asp:Label ID="lblOrderCouriarType" runat="server" CssClass="lblOrderCouriarType"></asp:Label>
                                                                        <asp:Label ID="lblOrderCouriarName" runat="server" CssClass="lblOrderCouriarName"></asp:Label>
                                                                        <asp:Label ID="lblOrderFirm" runat="server" CssClass="lblOrderFirm"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="2" class="tdCustDetail">
                                                                        <asp:Label ID="lblCustomerName" runat="server" CssClass="font-bold"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="2" class="tdCustDetail tdOrderCustAddress">
                                                                        <asp:Label ID="lblCustomerAddress" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="tdCustDetail tdCustTitle"><b>Mobile :</b></td>
                                                                    <td class="tdCustDetail">
                                                                        <asp:Label ID="lblCustomerMobile" runat="server" CssClass="font-bold"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="tdCustDetail tdCustTitle"><b>Pincode :</b></td>
                                                                    <td class="tdCustDetail">
                                                                        <asp:Label ID="lblCustomerPincode" runat="server" CssClass=""></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="tdCustDetail tdCustTitle"><b>City :</b></td>
                                                                    <td class="tdCustDetail">
                                                                        <asp:Label ID="lblCustomerCity" runat="server" CssClass=""></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="2" class="tdCustDetail tdOrderFirmName">
                                                                        <asp:Label ID="lblOrderFirmName" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="2" class="tdCustDetail">
                                                                        <asp:Label ID="lblOrderFirmAddress" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="tdCustDetail tdCustTitle">Contact :</td>
                                                                    <td class="tdCustDetail">
                                                                        <asp:Label ID="lblOrderFirmMobile" runat="server"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="2" class="tdItemTitle">Item</td>
                                                                    <td class="tdOrderTotalQuantity">
                                                                        <asp:Label ID="lblOrderTotalQuantity" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="3" class="tdItemDetail">
                                                                        <asp:Repeater ID="rptPrintOrderProduct" runat="server" OnItemDataBound="rptPrintOrderProduct_OnItemDataBound">
                                                                            <ItemTemplate>
                                                                                <div class="divItemName">
                                                                                    <asp:Label ID="lblItemName" runat="server"></asp:Label>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <div class="divPageSaparator"></div>
                                                            <div runat="server" id="divPageBreak" class="divPageBreak"></div>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="ClosePopup btn btn-raised btn-default">
                                Cancel</button>
                            <asp:Button ID="btnPrint" runat="server" CssClass="btnPrint btn btn-raised btn-black"
                                Text="Print" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <asp:LinkButton ID="lnkFackOrderComplain" runat="server"> </asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupOrderComplain" runat="server" DropShadow="false" BehaviorID="PopupBehaviorID2"
                PopupControlID="pnlpopupOrderComplain" TargetControlID="lnkFackOrderComplain" BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlpopupOrderComplain" runat="server" DefaultButton="btnSendOrderComplain" CssClass="modelpopup col-lg-11 col-md-10 col-sm-12 col-xs-12 p0"
                Style="display: none">
                <div class="modal-dialog">
                    <div class="modal-content darkmodel">
                        <div class="modal-header bg-black">
                            <button type="button" class="ClosePopup close">
                                ×</button>
                            <h4 class="modal-title">
                                <asp:Label ID="lblPopupTitle" runat="server">Order Complain</asp:Label></h4>
                        </div>
                        <div class="modal-body divloaderOrderComplain divValidOrderComplain">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="col-md-3">
                                        <label class="">
                                            Email <span class="text-desc">(Multiplae Aeparated by comma[,])</span>
                                        </label>
                                        <asp:TextBox ID="txtOrderComplainEmail" runat="server" CssClass="txtOrderComplainEmail form-control" ZValidation="e=blur|v=IsRequired|m=Email"></asp:TextBox>
                                        <asp:Label ID="lblOrderComplainEmailList" CssClass="lblOrderComplainEmailList hide" runat="server"></asp:Label>
                                    </div>
                                    <div class="col-md-3">
                                        <label class="">
                                            CC Email
                                        </label>
                                        <asp:TextBox ID="txtOrderComplainCCEmail" runat="server" CssClass="txtOrderComplainCCEmail form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-md-3">
                                        <label class="">
                                            Subject<span class="text-danger">*</span>
                                        </label>
                                        <asp:TextBox ID="txtOrderComplainSubject" runat="server" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=Subject"></asp:TextBox>
                                    </div>
                                    <div class="col-md-3">
                                        <label class="">
                                            Date
                                        </label>
                                        <asp:TextBox ID="txtOrderComplainDate" runat="server" AutoPostBack="true" OnTextChanged="txtOrderComplainDate_OnTextChanged" onchange="addRegionLoader('divloaderOrderComplain')" CssClass="form-control datepicker"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <table class="tblOrderComplain table table-bordered table-hover nomargin">
                                        <thead>
                                            <tr>
                                                <th>AWB No</th>
                                                <th>Order</th>
                                                <th>Name</th>
                                                <th>Address</th>
                                                <th class="text-center">Total Complain</th>
                                                <th>Action Taken</th>
                                                <th>Customer Reply</th>
                                                <th>Action To Be Taken</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:Repeater ID="rptOrderComplain" runat="server" OnItemDataBound="rptOrderComplain_OnItemDataBound">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="text-center">
                                                            <asp:Label ID="lblComplainOrderId" runat="server" Visible="false"></asp:Label>
                                                            <asp:Label ID="lblOrderNo" runat="server" CssClass="lblOCOrderDetail"></asp:Label>
                                                            <asp:Label ID="lblAWBNo" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="text-center tdOCOrderDetail">
                                                            <asp:Label ID="lblOrderDate" runat="server" CssClass="lblOCOrderDetail"></asp:Label>
                                                            <asp:Label ID="lblOrderStatus" runat="server" CssClass="lblOCOrderDetail"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="lblCustomerName" runat="server"></asp:Label>
                                                            <br />
                                                            <asp:Label ID="lblCustomerMobile" runat="server"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="lblCustomerAddress" runat="server"></asp:Label>
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:Label ID="lblComplainCount" runat="server"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtActionTaken" runat="server" list="dlActionTaken" CssClass="txtOrderComplain form-control"></asp:TextBox>
                                                            <datalist id="dlActionTaken">
                                                                <asp:Repeater ID="rptActionTaken" runat="server" OnItemDataBound="rptAutoFill_OnItemDataBound">
                                                                    <ItemTemplate>
                                                                        <asp:Literal ID="ltrActionTaken" runat="server"></asp:Literal>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </datalist>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtCustomerReply" runat="server" CssClass="txtOrderComplain form-control"></asp:TextBox>
                                                            <datalist id="dlCustomerReply">
                                                                <asp:Repeater ID="rptCustomerReply" runat="server" OnItemDataBound="rptAutoFill_OnItemDataBound">
                                                                    <ItemTemplate>
                                                                        <asp:Literal ID="ltrCustomerReply" runat="server"></asp:Literal>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </datalist>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtActionToBeTaken" runat="server" list="dlActionToBeTaken" CssClass="txtOrderComplain form-control"></asp:TextBox>
                                                            <datalist id="dlActionToBeTaken">
                                                                <asp:Repeater ID="rptActionToBeTaken" runat="server" OnItemDataBound="rptAutoFill_OnItemDataBound">
                                                                    <ItemTemplate>
                                                                        <asp:Literal ID="ltrActionToBeTaken" runat="server"></asp:Literal>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </datalist>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="ClosePopup btn btn-raised btn-default">
                                Cancel</button>
                            <asp:Button ID="btnSendOrderComplain" OnClick="btnSendOrderComplain_OnClick" runat="server" CssClass="btnSendOrderComplain btn btn-raised btn-black"
                                Text="Send" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <asp:LinkButton ID="lnkFackExcelChangeStatus" runat="server"> </asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupExcelChangeStatus" runat="server" DropShadow="false" BehaviorID="PopupBehaviorID3"
                PopupControlID="pnlpopupExcelChangeStatus" TargetControlID="lnkFackExcelChangeStatus" BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlpopupExcelChangeStatus" runat="server" DefaultButton="btnChangeStatus" CssClass="modelpopup col-lg-4 col-md-4 col-sm-8 col-xs-12 p0"
                Style="display: none">
                <div class="modal-dialog">
                    <div class="modal-content darkmodel">
                        <div class="modal-header bg-black">
                            <button type="button" class="ClosePopup close">
                                ×</button>
                            <h4 class="modal-title">Change Status</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal form-manual">
                                <div class="form-group mb-0">
                                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                        File<span class="text-danger">*</span>
                                    </label>
                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                        <asp:FileUpload ID="fuImportChangeStatus" CssClass="form-control" runat="server" />
                                        <span class="lblStatusColumnName">docket_no, status, payment_mode</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <a href="Download/Courier Status.xls" data-toggle="tooltip" title="Download Sample File"
                                class="pull-left btn btn-raised btn-black tooltips"><i class="fa fa-download"></i></a>
                            <button type="button" class="ClosePopup btn btn-raised btn-default">
                                Cancel</button>
                            <asp:Button ID="btnChangeStatus" OnClick="btnChangeStatus_OnClick" runat="server" CssClass="btn btn-raised btn-black clickloader"
                                Text="Save" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <asp:LinkButton ID="lnkFackPrintOrderProduct" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupPrintOrderProduct" runat="server" DropShadow="false" BehaviorID="PopupBehaviorID4"
                PopupControlID="pnlpopupPrintOrderProduct" TargetControlID="lnkFackPrintOrderProduct" BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlpopupPrintOrderProduct" runat="server" DefaultButton="btnPrint" CssClass="modelpopup col-lg-8 col-md-8 col-sm-8 col-xs-12 p0"
                Style="display: none">
                <div class="modal-dialog">
                    <div class="modal-content darkmodel">
                        <div class="modal-header bg-black">
                            <button type="button" class="ClosePopup close">×</button>
                            <h4 class="modal-title">
                                <span>Print Order Product</span></h4>
                        </div>
                        <div class="modal-body divProintOrderProductBody">
                            <div class="form-horizontal">
                                <div class="form-group divPrintOrderProduct">
                                    <style type="text/css">
                                        /* Order Product */
                                        .divOrderProduct {
                                            width: 100%;
                                        }

                                        .tblOrderProduct {
                                            width: 100%;
                                            border-spacing: 0;
                                            border-collapse: collapse;
                                        }

                                            .tblOrderProduct tr td, .tblOrderProduct tr th {
                                                border: 1px solid #000;
                                                padding: 7px 10px;
                                                background-color: #fff;
                                            }

                                            .tblOrderProduct tr th {
                                                font-weight: bold;
                                                text-align: center;
                                            }

                                        .tdOrderVendorName {
                                            font-weight: bold;
                                            text-align: center;
                                        }

                                        .tdOrderProductImage {
                                            width: 91px;
                                        }

                                        .tdOrderProductImage {
                                            width: 70px !important;
                                            padding: 0px !important;
                                        }

                                        .imgOrderProductImage {
                                            height: 70px;
                                            width: 70px;
                                        }

                                        .tdOrderRequiredQty {
                                            text-align: center;
                                        }

                                        .lblOrderProductName, .lblOrderProductPrice {
                                            display: block;
                                        }
                                    </style>
                                    <div class="divOrderProduct">
                                        <table class="tblOrderProduct">
                                            <thead>
                                                <tr>
                                                    <th colspan="2">Product Info</th>
                                                    <th>Required Qty</th>
                                                    <th>Requested Qty</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:Repeater ID="rptPrintOrderVendor" runat="server" OnItemDataBound="rptPrintOrderVendor_OnItemDataBound">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td class="tdOrderVendorName" colspan="4">
                                                                <asp:Literal ID="ltrOrderVendorName" runat="server"></asp:Literal>
                                                            </td>
                                                        </tr>
                                                        <asp:Repeater ID="rptOrderProduct" runat="server" OnItemDataBound="rptOrderProduct_OnItemDataBound">
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <td class="tdOrderProductImage">
                                                                        <asp:Image ID="imgOrderProductImage" runat="server" CssClass="imgOrderProductImage" />
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="lblOrderProductName" runat="server" CssClass="lblOrderProductName"></asp:Label>
                                                                        <asp:Label ID="lblOrderProductPrice" runat="server" CssClass="lblOrderProductPrice"></asp:Label>
                                                                    </td>
                                                                    <td class="tdOrderRequiredQty">
                                                                        <asp:Literal ID="ltrOrderRequiredQty" runat="server"></asp:Literal>
                                                                    </td>
                                                                    <td></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                        <tr>
                                                            <td colspan="4"></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>

                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="ClosePopup btn btn-raised btn-default">
                                Cancel</button>
                            <asp:Button ID="btnPrintOrderProduct" runat="server" CssClass="btnPrintOrderProduct btn btn-raised btn-black"
                                Text="Print" />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <asp:LinkButton ID="lnkFakeExcelPayment" runat="server"> </asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupExcelPayment" runat="server" DropShadow="false" BehaviorID="PopupBehaviorID5"
                PopupControlID="pnlpopupExcelPayment" TargetControlID="lnkFakeExcelPayment" BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlpopupExcelPayment" runat="server" DefaultButton="btnPay" CssClass="modelpopup col-lg-4 col-md-4 col-sm-8 col-xs-12 p0"
                Style="display: none">
                <div class="modal-dialog">
                    <div class="modal-content darkmodel">
                        <div class="modal-header bg-black">
                            <button type="button" class="ClosePopup close">
                                ×</button>
                            <h4 class="modal-title">Payment</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal form-manual">
                                <div class="form-group mb-0">
                                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                        File<span class="text-danger">*</span>
                                    </label>
                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                        <asp:FileUpload ID="fuImportPayment" CssClass="form-control" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <a href="Download/OrderPayment Sample.xls" data-toggle="tooltip" title="Download Sample File"
                                class="pull-left btn btn-raised btn-black tooltips"><i class="fa fa-download"></i></a>
                            <button type="button" class="ClosePopup btn btn-raised btn-default">
                                Cancel</button>
                            <asp:Button ID="btnPay" OnClick="btnPay_OnClick" runat="server" CssClass="btn btn-raised btn-black clickloader"
                                Text="Save" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:LinkButton ID="lnkFackOrderPayment" runat="server"> </asp:LinkButton>
    <cc1:ModalPopupExtender ID="popupOrderPayment" runat="server" DropShadow="false" BehaviorID="PopupBehaviorID6"
        PopupControlID="pnlpopupOrderPayment" TargetControlID="lnkFackOrderPayment" BackgroundCssClass="modalBackground">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlpopupOrderPayment" runat="server" CssClass="modelpopup col-lg-6 col-md-6 col-sm-8 col-xs-12 p0"
        Style="display: none">
        <MOP:ManageOrderPaymentPopup ID="popupManageOrderPayment" runat="server" />
    </asp:Panel>
    <asp:LinkButton ID="lnkFakeExcelExport" runat="server"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="popupExcelExport" runat="server" DropShadow="false" PopupControlID="pblExcelExport"
        BehaviorID="PopupBehaviorID8" TargetControlID="lnkFakeExcelExport" BackgroundCssClass="modalBackground">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pblExcelExport" CssClass="modelpopup col-lg-3 col-md-3 col-sm-6 col-xs-12 p0"
        Style="display: none" runat="server">
        <EE:ExcelExportPopup ID="ExcelExport" runat="server" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="script" runat="Server">

    <script src="<%= CU.StaticFilePath %>plugins/jqueryTypeahead-Autocompletion/js/typeahead.bundle.min.js"></script>
    <script src="<%= CU.StaticFilePath %>plugins/jqueryTypeahead-Autocompletion/js/typeahead.tagging.js"></script>

    <script type="text/javascript">

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Checkpostback);
        jQuery(function () {
            Checkpostback();
        });
        function Checkpostback() {
            $(".ddlOrderStatus").change(function () {
                var OrdersId = $(this).parent().find(".lblRowOrderId").text();
                var txtAWBNo = $(this).parent().find(".txtAWBNo");
                $.ajax({
                    type: "POST",
                    url: "OrderView.aspx/SetOrderStatus",
                    data: "{ OrdersId:" + OrdersId + ",OrderStatusId:" + $(this).val() + " }",
                    contentType: "application/json; charset=utf-8",
                    success: function (response) {
                        txtAWBNo.val(response["d"]);
                    }
                });
            });

            $(".txtAWBNo").change(function () {
                var OrdersId = $(this).parent().find(".lblRowOrderId").text();

                $.ajax({
                    type: "POST",
                    url: "OrderView.aspx/SetAWBNo",
                    data: "{ OrdersId:" + OrdersId + ",AWBNo:'" + $(this).val() + "' }",
                    contentType: "application/json; charset=utf-8",
                    success: function (response) {
                        //alert(response);
                    }
                });
            });

            $(".btnPrint").click(function () {
                var htmlToPrint = $(".divPrintOrderSlip").html();
                newWin = window.open("");
                newWin.document.write(htmlToPrint);
                newWin.print();
                //newWin.close();

                return false;
            });

            $(".btnPrintOrderProduct").click(function () {
                var htmlToPrint = $(".divPrintOrderProduct").html();
                newWin = window.open("");
                newWin.document.write(htmlToPrint);
                newWin.print();
                //newWin.close();

                return false;
            });

            $(".btnSendOrderComplain").click(function () {
                if (CheckValidation("divValidOrderComplain")) {
                    addLoader('btnSendOrderComplain');
                    return true;
                }
                else {
                    return false;
                }
            });

            var tagsource = [];
            var arrOrderComplainEmailList = $('.lblOrderComplainEmailList').text().split(',');
            $.each(arrOrderComplainEmailList, function (index, value) {
                if (value != "") {
                    tagsource.push(value);
                }
            });

            $('.txtOrderComplainEmail').tagging(tagsource);
            $('.txtOrderComplainCCEmail').tagging(tagsource);
        }

    </script>

</asp:Content>
