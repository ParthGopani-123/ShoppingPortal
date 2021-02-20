<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="ManageProduct.aspx.cs" Inherits="ManageProduct" Title="Product" EnableEventValidation="false" %>

<%@ Register Src="~/CCConfirmationPopup.ascx" TagName="ConfirmationPopup" TagPrefix="CP" %>
<%@ Register Src="~/CCExcelExport.ascx" TagName="ExcelExportPopup" TagPrefix="EE" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" type="text/css" href="<%= CU.StaticFilePath %>plugins/build/css/jquery.ezdz.min.css" />
    <link href="<%= CU.StaticFilePath %>plugins/jqueryTypeahead-Autocompletion/css/typeahead.tagging.css" rel="stylesheet" />

    <style type="text/css">
        .divProductImage {
            min-height: 81px;
            display: block !important;
        }

        .hideimage {
            display: none !important;
        }

        .ezdz-dropzone {
            height: 100px;
        }

            .ezdz-dropzone > div {
                margin-top: -88px;
            }

        .imgProduct {
            height: 100%;
            width: 100%;
            border-radius: 4px;
        }

        .divProductImage:hover .RemoveProductImage {
            display: block !important;
        }

        .divProductImage .RemoveProductImage {
            position: absolute;
            top: 5px;
            right: 6px;
            margin-left: 0px;
            padding: 5px 5px 0px 0px;
            text-align: right;
            float: right;
            color: #fff;
            background: linear-gradient(15deg, transparent 0%, transparent 45%, rgba(0,0,0,0.12) 70%, rgba(0,0,0,0.33) 100%);
            border-radius: 4px;
            height: 40px;
            width: 90%;
            cursor: pointer;
            display: none;
        }

        .imgProductImage {
            height: 110px;
            width: 110px;
        }

        .VariantNote {
            text-align: right;
            font-size: 12px;
            color: #9c9c9c;
        }

        .lblPrice {
            display: block;
        }

        .aSearchProduct {
            font-size: 20px;
        }

        .lblError {
            color: red;
            font-size: 12px;
            font-style: italic;
        }

        .mt--10 {
            margin-top: -10px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Label ID="lbltabHeight1" runat="server" CssClass="lbltabHeight1 hidden" Text="151"></asp:Label>
            <asp:Label ID="lbltabHeightSmall1" runat="server" CssClass="lbltabHeightSmall1 hidden"
                Text="190"></asp:Label>
            <asp:Label ID="lblFirmId" runat="server" Visible="false"></asp:Label>
            <asp:Label ID="lblOrganizationId" runat="server" Visible="false"></asp:Label>
            <asp:Label ID="lbleOrganization" runat="server" Visible="false"></asp:Label>
            <asp:Label ID="lblProductId" runat="server" Visible="false"></asp:Label>
            <div class="row">
                <div class="page-header clearfix">
                    <div class="col-lg-9 col-md-9 col-sm-9 col-xs-12 p0">
                        <div class="btn-group">
                            <asp:LinkButton ID="lnkAdd" ToolTip="Add (alt+n)" runat="server" OnClick="lnkAdd_OnClick"
                                CssClass="lnkAdd btn btngroup btn-add">
                            <i class="fa fa-plus"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkEdit" ToolTip="Edit (alt+u)" allowon="1" runat="server" OnClick="lnkEdit_OnClick"
                                CssClass="lnkEdit btn btngroup btn-edit">
                            <i class="fa fa-edit"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkClone" ToolTip="Clone" runat="server" OnClick="lnkClone_Click" allowon="1"
                                CssClass="lnkClone btn btngroup btn-extra1">
                            <i class="fa fa-clone"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkActive" ToolTip="Active (alt+a)" allowon="1" runat="server"
                                OnClick="lnkActive_OnClick" CssClass="lnkActive btn btngroup btn-active clickloader">
                            <i class="fa fa-check"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkDeactive" ToolTip="Deactive (alt+r)" allowon="1" runat="server"
                                OnClick="lnkDeactive_OnClick" CssClass="lnkDeactive btn btngroup btn-deactive clickloader">
                            <i class="fa fa-ban"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkDelete" ToolTip="Delete (alt+x)" runat="server"
                                OnClick="lnkDelete_OnClick" CssClass="lnkDelete btn btngroup btn-delete clickloader">
                            <i class="fa fa-trash-o"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkRefresh" ToolTip="Refresh" runat="server" OnClick="lnkRefresh_OnClick"
                                CssClass="btn btngroup btn-refresh clickloader">
                            <i class="fa fa-refresh"></i>
                            </asp:LinkButton>
                        </div>
                        <div class="btn-group pull-right mr-5">
                            <asp:LinkButton ID="lnkExcelExport" ToolTip="Excel Export" runat="server" OnClick="lnkExcelExport_OnClick"
                                CssClass="lnkExcelExport btn btngroup btn-export">
                            <i class="fa fa-file-excel-o"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkExcelImport" ToolTip="Excel Import" runat="server" OnClick="lnkExcelImport_OnClick"
                                CssClass="lnkExcelImport btn btngroup btn-import">
                            <i class="fa fa-cloud-upload"></i>
                            </asp:LinkButton>
                        </div>
                    </div>
                    <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12 search div-master-search div-master-search-xs">
                        <asp:Label ID="lblCount" ToolTip="Count" runat="server" Text="10" CssClass="pull-left mr-5 btn btn-icon btn-total">
                        </asp:Label>
                        <div class="input-group">
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control master-search"
                                OnChange="addRegionLoader('divloader')" AutoPostBack="true" OnTextChanged="Control_CheckedChanged"
                                placeholder="Search..."></asp:TextBox>
                            <a note-colspan="note-colspan-class" class="tooltips input-group-addon aShowSearch btn btn-master-search ml5"
                                data-original-title="Search"><i note-colspan="note-colspan-class"
                                    class="fa fa-chevron-down"></i></a>
                        </div>
                    </div>
                    <div note-colspan="note-colspan-class" class="search-tools divShowSearch note-colspan-class">
                        <asp:Panel ID="pnlSearch" runat="server" DefaultButton="lnkSearch" class="divsearchloader"
                            note-colspan="note-colspan-class">
                            <div class="padbm" note-colspan="note-colspan-class">
                                <asp:TextBox ID="txtSearchCode" runat="server" CssClass="form-control" placeholder="Product Code" note-colspan="note-colspan-class"></asp:TextBox>
                            </div>
                            <div class="padbm" note-colspan="note-colspan-class">
                                <asp:TextBox ID="txtSearchVendorName" runat="server" CssClass="form-control" AutoPostBack="true" placeholder="Vendor Name" note-colspan="note-colspan-class"></asp:TextBox>
                            </div>
                            <div class="padbm" note-colspan="note-colspan-class">
                                <div class="input-group" note-colspan="note-colspan-class">
                                    <asp:TextBox ID="txtPriceFrom" runat="server" CssClass="form-control intnumber" placeholder="From Price" note-colspan="note-colspan-class"></asp:TextBox>
                                    <span class="input-group-addon" note-colspan="note-colspan-class">to</span>
                                    <asp:TextBox ID="txtPriceTo" runat="server" CssClass="form-control intnumber" placeholder="Price" note-colspan="note-colspan-class"></asp:TextBox>
                                </div>
                            </div>
                            <div class="padbm" note-colspan="note-colspan-class">
                                <asp:TextBox ID="txtSearchNameTag" runat="server" CssClass="form-control" placeholder="Name Tag" note-colspan="note-colspan-class"></asp:TextBox>
                            </div>
                            <div class="padbm" note-colspan="note-colspan-class">
                                <asp:TextBox ID="txtSearchDescription" runat="server" CssClass="form-control" placeholder="Description" note-colspan="note-colspan-class"></asp:TextBox>
                            </div>
                            <div class="padbm" note-colspan="note-colspan-class">
                                <asp:DropDownList ID="ddlSearchStock" runat="server" CssClass="form-control" note-colspan="note-colspan-class"></asp:DropDownList>
                            </div>
                            <div class="col-md-6 col-sm-6 col-xs-6 col-sm-6 searchchk" note-colspan="note-colspan-class">
                                <div class="checkbox-custom" note-colspan="note-colspan-class">
                                    <asp:CheckBox ID="chkActive" runat="server" note-colspan="note-colspan-class" CssClass="chk"
                                        Checked="true" Text="Active" />
                                </div>
                            </div>
                            <div class="col-md-6 col-sm-6 col-xs-6 col-sm-6 searchchk" note-colspan="note-colspan-class">
                                <div class="checkbox-custom" note-colspan="note-colspan-class">
                                    <asp:CheckBox ID="chkDeactive" runat="server" note-colspan="note-colspan-class" CssClass="chk"
                                        Text="Deactive" />
                                </div>
                            </div>
                            <div class="padbm text-right" note-colspan="note-colspan-class">
                                <asp:LinkButton ID="lnkSearch" note-colspan="note-colspan-class" OnClientClick="addRegionLoader('divloader')"
                                    OnClick="Control_CheckedChanged" class="btn btn-warning btnsearch" runat="server"><i class="fa fa-filter"></i> Filter</asp:LinkButton>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
                <div class="page-content col-md-12 divtable divloader">
                    <div class="table-responsive tabHeight1 fixheight2">
                        <asp:GridView ID="grdProduct" AllowPaging="false" runat="server" OnRowDataBound="grdProduct_OnRowDataBound"
                            OnSelectedIndexChanged="grdProduct_OnSelectedIndexChanged" AutoGenerateColumns="False"
                            class="table table-bordered table-hover nomargin selectonrowclick rowloader fixheader">
                            <Columns>
                                <asp:TemplateField HeaderStyle-CssClass="hide" ItemStyle-CssClass="hide">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelect" CssClass="AllowMultiple" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="ProductId" HeaderText="ProductId" HeaderStyle-CssClass="hide"
                                    ItemStyle-CssClass="hide" />
                                <asp:BoundField DataField="eStatus" HeaderText="eStatus" HeaderStyle-CssClass="hide"
                                    ItemStyle-CssClass="hide" />
                                <asp:TemplateField HeaderText="Product Code" ItemStyle-CssClass="valigntop">
                                    <ItemTemplate>
                                        <asp:Image ID="imgProductImage" CssClass="imgProductImage" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Price" ItemStyle-CssClass="valigntop text-right">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEditProduct" CssClass="pull-left lnkEditProduct" OnClick="lnkEditProduct_OnClick"
                                            runat="server"></asp:LinkButton>
                                        <asp:Literal ID="ltrProduct" runat="server"></asp:Literal>
                                        <a id="aSearchProduct" runat="server" class="aSearchProduct" target="_blank"><i class="fa fa-info-circle pull-right"></i></a>
                                        <br />
                                        <asp:Label ID="lblPrice" runat="server" CssClass="lblPrice"></asp:Label>
                                        <asp:Repeater ID="rptProductPrice" runat="server" OnItemDataBound="rptProductPrice_OnItemDataBound">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPrice" runat="server" CssClass="lblPrice"></asp:Label>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Description" HeaderText="Description" HtmlEncode="false" />
                                <asp:TemplateField HeaderText="Vendor" ItemStyle-CssClass="valigntop">
                                    <ItemTemplate>
                                        <asp:Label ID="lblVendorName" runat="server" CssClass="lblPrice"></asp:Label>
                                        <asp:Label ID="lblUploadDate" runat="server" CssClass="lblPrice"></asp:Label>
                                        <br />
                                        <asp:Label ID="lblNameTag" runat="server" CssClass="lblPrice"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Stock" ItemStyle-CssClass="valigntop">
                                    <ItemTemplate>
                                        <asp:Literal ID="ltrProductStock" runat="server"></asp:Literal>
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
                            runat="server" ToolTip="First Page" CssClass="fa fa-fast-backward btn-paging"></asp:LinkButton>
                        <asp:LinkButton ID="lnkPrev" OnClick="lnkPrev_Click" OnClientClick="addRegionLoader('divloader')"
                            runat="server" ToolTip="Previous Page" CssClass="fa fa-backward btn-paging"></asp:LinkButton>
                        <asp:TextBox ID="txtGotoPageNo" Text="1" runat="server" CssClass="txt-paging" OnTextChanged="txtGotoPageNo_OnTextChange"
                            OnChange="addRegionLoader('divloader')" AutoPostBack="true"></asp:TextBox>
                        <asp:LinkButton ID="lnkNext" OnClick="lnkNext_Click" OnClientClick="addRegionLoader('divloader')"
                            runat="server" ToolTip="Next Page" CssClass="fa fa-forward btn-paging"></asp:LinkButton>
                        <asp:LinkButton ID="lnkLast" OnClick="lnkLast_Click" OnClientClick="addRegionLoader('divloader')"
                            runat="server" ToolTip="Last Page" CssClass="fa fa-fast-forward btn-paging"></asp:LinkButton>
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
            <asp:PostBackTrigger ControlID="btnUpload" />
            <asp:PostBackTrigger ControlID="btnSaveProduct" />
            <asp:PostBackTrigger ControlID="btnSaveAndNewProduct" />
        </Triggers>
        <ContentTemplate>
            <cc1:ModalPopupExtender ID="popupConfirmation" runat="server" DropShadow="false"
                PopupControlID="pnlConfirmation" BehaviorID="PopupBehaviorID1" TargetControlID="lnkFakeConfirmation"
                BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>
            <asp:LinkButton ID="lnkFakeConfirmation" runat="server"></asp:LinkButton>
            <asp:Panel ID="pnlConfirmation" CssClass="modal-content zoomIn modal-confirmation col-xs-12 col-sm-12 col-md-12 p0"
                Style="display: none" runat="server">
                <CP:ConfirmationPopup ID="Confirmationpopup" runat="server" />
            </asp:Panel>
            <asp:LinkButton ID="lnkFackExcelImport" runat="server"> </asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupExcelImport" runat="server" DropShadow="false" BehaviorID="PopupBehaviorID2"
                PopupControlID="pnlpopupExcelImport" TargetControlID="lnkFackExcelImport" BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlpopupExcelImport" runat="server" DefaultButton="btnUpload" CssClass="modelpopup col-lg-4 col-md-4 col-sm-8 col-xs-12 p0"
                Style="display: none">
                <div class="modal-dialog">
                    <div class="modal-content darkmodel">
                        <div class="modal-header bg-black">
                            <button type="button" class="ClosePopup close">
                                ×</button>
                            <h4 class="modal-title">Import Excel</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal form-manual">
                                <div class="form-group mb-0">
                                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                        File<span class="text-danger">*</span>
                                    </label>
                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                        <asp:FileUpload ID="fuImportExcel" CssClass="form-control" runat="server" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-lg-3 col-md-3 col-sm-4 hidden-xs control-label">
                                    </label>
                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                        <div class="checkbox-custom">
                                            <asp:CheckBox ID="chkReplace" Text="Replace Product if Any" runat="server" />
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-lg-3 col-md-3 col-sm-4 hidden-xs control-label">
                                    </label>
                                    <div class="col-lg-9 col-md-9 col-sm-8">
                                        <label class="text-danger">
                                            * You can not change <b>Vendor Code</b> by Excel</label>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <a href="Download/Product Sample.xls" title="Download Sample File"
                                class="pull-left btn btn-raised btn-black"><i class="fa fa-download"></i></a>
                            <button type="button" class="ClosePopup btn btn-raised btn-default">
                                Cancel</button>
                            <asp:Button ID="btnUpload" OnClick="btnUpload_OnClick" runat="server" CssClass="btn btn-raised btn-black clickloader"
                                Text="Save" />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <asp:LinkButton ID="lnkFackProduct" runat="server"> </asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupProduct" runat="server" DropShadow="false" BehaviorID="PopupBehaviorID4"
                PopupControlID="pnlpopupProduct" TargetControlID="lnkFackProduct" BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlpopupProduct" runat="server" DefaultButton="btnSaveProduct" CssClass="modelpopup col-lg-6 col-md-6 col-sm-8 col-xs-12 p0"
                Style="display: none">
                <div class="modal-dialog">
                    <div class="modal-content darkmodel">
                        <div class="modal-header bg-black">
                            <button type="button" class="ClosePopup close">
                                ×</button>
                            <h4 class="modal-title">
                                <asp:Label ID="lblPopupTitle" runat="server">Product</asp:Label></h4>
                        </div>
                        <div class="modal-body pt-5 divloaderProduct checkvalidProductDetail">
                            <asp:TextBox ID="txtActiveTabProduct" CssClass="txtActiveTabProduct hidden" Text="" runat="server"></asp:TextBox>
                            <ul class="nav nav-tabs mb-15">
                                <li id="liTabProductDetail" onclick="ManageTabProduct('liTabProductDetail', 'pnlProductDetail')"
                                    runat="server" class="liTabProductDetail"><a>Product</a></li>
                                <li id="liTabProductImage" onclick="ManageTabProduct('liTabProductImage', 'pnlProductImage')"
                                    runat="server" class="liTabProductImage"><a>Image</a></li>
                                <li id="liTabProductVariant" onclick="ManageTabProduct('liTabProductVariant', 'pnlProductVariant')"
                                    runat="server" class="liTabProductVariant"><a>Variant</a></li>
                                <li id="liTabProductPriceList" onclick="ManageTabProduct('liTabProductPriceList', 'pnlProductPriceList')"
                                    runat="server" class="liTabProductPriceList"><a>Price List</a></li>
                                <li id="liTabPortalProduct" onclick="ManageTabProduct('liTabPortalProduct', 'pnlPortalProduct')"
                                    runat="server" class="liTabPortalProduct"><a>Portal Product</a></li>
                            </ul>
                            <div class="pnlProductDetail Productdetminh">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                            Stock<span class="text-danger">*</span>
                                        </label>
                                        <div class="col-lg-3 col-md-3 col-sm-3">
                                            <asp:DropDownList ID="ddlStock" CssClass="form-control" runat="server">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-lg-5 col-md-5 col-sm-4">
                                            <asp:TextBox ID="txtStockNote" CssClass="txtStockNote form-control"
                                                runat="server" placeholder="Stock Not If Any"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                            Vendor Code<span class="text-danger">*</span>
                                        </label>
                                        <div class="col-lg-8 col-md-8 col-sm-7">
                                            <asp:TextBox ID="txtVendorCode" OnTextChanged="txtVendorCode_OnTextChanged" onchange="addRegionLoader('divloaderProduct')" AutoPostBack="true" CssClass="txtVendorCode form-control" ZValidation="e=blur|v=IsRequired|m=Vendor Code"
                                                runat="server" MaxLength="50" placeholder="Enter Vendor Code"></asp:TextBox>
                                            <asp:Label ID="lblVendorCodeError" runat="server" CssClass="lblError"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                            Product Code<span class="text-danger">*</span>
                                        </label>
                                        <div class="col-lg-8 col-md-8 col-sm-7">
                                            <div class="input-group">
                                                <asp:TextBox ID="txtProductCode" OnTextChanged="txtProductCode_OnTextChanged" onchange="addRegionLoader('divloaderProduct')" AutoPostBack="true" CssClass="txtProductCode form-control" ZValidation="e=blur|v=IsRequired|m=Code"
                                                    runat="server" MaxLength="10" placeholder="Enter Product Code"></asp:TextBox>
                                                <asp:LinkButton ID="lnkGetProductCode" runat="server" OnClick="lnkGetProductCode_OnClick" CssClass="clickloader input-group-addon"><i class="fa fa-exclamation-circle"></i></asp:LinkButton>
                                            </div>
                                            <asp:Label ID="lblProductCodeError" runat="server" CssClass="lblError"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                            Vendor
                                        </label>
                                        <div class="col-lg-8 col-md-8 col-sm-7">
                                            <asp:DropDownList ID="ddlVendor" CssClass="form-control" runat="server">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="form-group hide">
                                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                            Receler Price
                                        </label>
                                        <div class="col-lg-8 col-md-8 col-sm-7">
                                            <asp:TextBox ID="txtRecelerPrice" CssClass="form-control intnumber" ZValidation="e=blur|v=IsNullNumber|m=Receler Price"
                                                runat="server" MaxLength="10" placeholder="Enter Receler Price"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group hide">
                                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                            Product Price<span class="text-danger">*</span>
                                        </label>
                                        <div class="col-lg-8 col-md-8 col-sm-7">
                                            <asp:TextBox ID="txtPrice" CssClass="form-control intnumber" ZValidation="e=blur|v=IsNumber|m=Product Price"
                                                runat="server" MaxLength="10" placeholder="Enter Product Price"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                            Weight
                                        </label>
                                        <div class="col-lg-8 col-md-8 col-sm-7">
                                            <div class="input-group">
                                                <asp:TextBox ID="txtWeight" CssClass="form-control flotnumber" ZValidation="e=blur|v=IsNullNumber|m=Weight"
                                                    runat="server" MaxLength="10" placeholder="Enter Weight"></asp:TextBox>
                                                <span class="input-group-addon">kg</span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                            Name Tag
                                        </label>
                                        <div class="col-lg-8 col-md-8 col-sm-7">
                                            <asp:TextBox ID="txtNameTag" runat="server" CssClass="txtNameTag form-control"></asp:TextBox>
                                            <asp:Label ID="lblNameTagList" CssClass="lblNameTagList form-control hide" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                            Description
                                        </label>
                                        <div class="col-lg-8 col-md-8 col-sm-7 htmlareah100">
                                            <asp:TextBox ID="txtDescription" CssClass="txtDescription htmlarea form-control" runat="server"
                                                TextMode="MultiLine" placeholder="Enter Description"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="pnlProductImage checkvalidProductImage Productdetminh" runat="server">
                                <div class="form-horizontal">
                                    <div class="form-group mb-0">
                                        <div class="col-md-12">
                                            <b>Modeling Image</b>
                                        </div>
                                    </div>
                                    <div class="form-group mb-0">
                                        <div class="col-md-12">
                                            <asp:LinkButton ID="lnkProductImageUpload" OnClick="lnkProductImageUpload_OnClick"
                                                CssClass="hide" runat="server"></asp:LinkButton>
                                            <asp:TextBox ID="txtProductImage" CssClass="txtProductImage hidden" runat="server"></asp:TextBox>
                                            <input id="fuProductImage" type="file" class="form-control dropfile fuProductImage"
                                                multiple="multiple" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div id="ProductImage" class="col-md-12 p-0 pl-10 pr-10 sortableProductImage">
                                            <asp:Repeater ID="rptProductImage" runat="server" OnItemDataBound="rptProductImage_OnItemDataBound">
                                                <ItemTemplate>
                                                    <div id="divProductImage" runat="server" class="divProductImage col-lg-2 col-md-3 col-sm-4 col-xs-6 p-5">
                                                        <asp:TextBox ID="txteStatus" runat="server" CssClass="txteStatus hide"></asp:TextBox>
                                                        <asp:Label ID="lblPK" runat="server" Visible="false" Text=""></asp:Label>
                                                        <asp:Label ID="lblProductImageId" runat="server" Visible="false" Text=""></asp:Label>
                                                        <asp:Label ID="lblImagePath" runat="server" Visible="false" Text=""></asp:Label>
                                                        <asp:Image ID="imgProduct" runat="server" CssClass="imgProduct" alt="" />
                                                        <asp:TextBox ID="txtImageSerialNo" CssClass="txtImageSerialNo hide" runat="server"></asp:TextBox>
                                                        <span class="fa fa-times RemoveProductImage"></span>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>
                                    <div class="form-group mb-0">
                                        <div class="col-md-12">
                                            <b>Original Image</b>
                                        </div>
                                    </div>
                                    <div class="form-group mb-0">
                                        <div class="col-md-12">
                                            <asp:LinkButton ID="lnkProductImageOriginalUpload" OnClick="lnkProductImageOriginalUpload_OnClick"
                                                CssClass="hide" runat="server"></asp:LinkButton>
                                            <asp:TextBox ID="txtProductImageOriginal" CssClass="txtProductImageOriginal hidden" runat="server"></asp:TextBox>
                                            <input id="fuProductImageOriginal" type="file" class="form-control dropfile fuProductImageOriginal"
                                                multiple="multiple" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div id="ProductImageOriginal" class="col-md-12 p-0 pl-10 pr-10 sortableProductImageOriginal">
                                            <asp:Repeater ID="rptProductImageOriginal" runat="server" OnItemDataBound="rptProductImage_OnItemDataBound">
                                                <ItemTemplate>
                                                    <div id="divProductImage" runat="server" class="divProductImage col-lg-2 col-md-3 col-sm-4 col-xs-6 p-5">
                                                        <asp:TextBox ID="txteStatus" runat="server" CssClass="txteStatus hide"></asp:TextBox>
                                                        <asp:Label ID="lblPK" runat="server" Visible="false" Text=""></asp:Label>
                                                        <asp:Label ID="lblProductImageId" runat="server" Visible="false" Text=""></asp:Label>
                                                        <asp:Label ID="lblImagePath" runat="server" Visible="false" Text=""></asp:Label>
                                                        <asp:Image ID="imgProduct" runat="server" CssClass="imgProduct" alt="" />
                                                        <asp:TextBox ID="txtImageSerialNo" CssClass="txtImageSerialNo hide" runat="server"></asp:TextBox>
                                                        <span class="fa fa-times RemoveProductImage"></span>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>
                                    <div class="form-group mb-0">
                                        <div class="col-md-12">
                                            <b>Customer Image</b>
                                        </div>
                                    </div>
                                    <div class="form-group mb-0">
                                        <div class="col-md-12">
                                            <asp:LinkButton ID="lnkProductImageCustomerUpload" OnClick="lnkProductImageCustomerUpload_OnClick"
                                                CssClass="hide" runat="server"></asp:LinkButton>
                                            <asp:TextBox ID="txtProductImageCustomer" CssClass="txtProductImageCustomer hidden" runat="server"></asp:TextBox>
                                            <input id="fuProductImageCustomer" type="file" class="form-control dropfile fuProductImageCustomer"
                                                multiple="multiple" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div id="ProductImageCustomer" class="col-md-12 p-0 pl-10 pr-10 sortableProductImageCustomer">
                                            <asp:Repeater ID="rptProductImageCustomer" runat="server" OnItemDataBound="rptProductImage_OnItemDataBound">
                                                <ItemTemplate>
                                                    <div id="divProductImage" runat="server" class="divProductImage col-lg-2 col-md-3 col-sm-4 col-xs-6 p-5">
                                                        <asp:TextBox ID="txteStatus" runat="server" CssClass="txteStatus hide"></asp:TextBox>
                                                        <asp:Label ID="lblPK" runat="server" Visible="false" Text=""></asp:Label>
                                                        <asp:Label ID="lblProductImageId" runat="server" Visible="false" Text=""></asp:Label>
                                                        <asp:Label ID="lblImagePath" runat="server" Visible="false" Text=""></asp:Label>
                                                        <asp:Image ID="imgProduct" runat="server" CssClass="imgProduct" alt="" />
                                                        <asp:TextBox ID="txtImageSerialNo" CssClass="txtImageSerialNo hide" runat="server"></asp:TextBox>
                                                        <span class="fa fa-times RemoveProductImage"></span>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="pnlProductVariant Productdetminh">
                                <div class="form-horizontal">
                                    <div class="form-group mb-0">
                                        <div class="col-lg-11 col-md-11 col-sm-11 VariantNote">
                                            <span>Multiple Variant Separated by Comma(,)</span>
                                        </div>
                                    </div>
                                    <asp:Repeater ID="rptProductVariant" runat="server" OnItemDataBound="rptProductVariant_OnItemDataBound">
                                        <ItemTemplate>
                                            <div class="form-group">
                                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                                    <asp:Label ID="lblVariantId" runat="server" Visible="false"></asp:Label>
                                                    <asp:Literal ID="ltrVariantName" runat="server"></asp:Literal>
                                                </label>
                                                <div class="col-lg-8 col-md-8 col-sm-7">
                                                    <asp:ListBox ID="lbxVariantValue" SelectionMode="Multiple" CssClass="form-control" runat="server"></asp:ListBox>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                            <div class="pnlProductPriceList Productdetminh">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                            Vendor Price
                                        </label>
                                        <div class="col-lg-8 col-md-8 col-sm-7">
                                            <asp:TextBox ID="txtPurchasePrice" CssClass="form-control intnumber" ZValidation="e=blur|v=IsNullNumber|m=Purchase Price"
                                                runat="server" MaxLength="10" placeholder="Enter Purchase Price"></asp:TextBox>
                                        </div>
                                    </div>
                                    <asp:Repeater ID="rptPriceList" runat="server" OnItemDataBound="rptPriceList_OnItemDataBound">
                                        <ItemTemplate>
                                            <div class="form-group">
                                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                                    <asp:Label ID="lblPriceListName" runat="server"></asp:Label>
                                                </label>
                                                <div class="col-lg-8 col-md-8 col-sm-7">
                                                    <asp:Label ID="lblPriceListId" runat="server" Visible="false"></asp:Label>
                                                    <asp:TextBox ID="txtProductPriceListPrice" CssClass="form-control intnumber"
                                                        runat="server" MaxLength="6" placeholder="Enter Price"></asp:TextBox>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                            <div class="pnlPortalProduct Productdetminh">
                                <div class="form-horizontal">
                                    <asp:Repeater ID="rptPortalProduct" runat="server" OnItemDataBound="rptPortalProduct_OnItemDataBound">
                                        <ItemTemplate>
                                            <div class="form-group">
                                                <div class="col-lg-3 col-md-3 col-sm-3">
                                                    <asp:Label ID="lblPK" runat="server" Visible="false"></asp:Label>
                                                    <asp:Label ID="lblPortalProductId" runat="server" Visible="false"></asp:Label>
                                                    <asp:DropDownList ID="ddlPortal" CssClass="form-control" runat="server">
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-lg-8 col-md-8 col-sm-7">
                                                    <div class="input-group">
                                                        <asp:TextBox ID="txtPortalProductName" CssClass="form-control"
                                                            runat="server" placeholder="Enter Name"></asp:TextBox>
                                                        <asp:LinkButton ID="lnkDeleteProduct" CssClass="input-group-addon" OnClick="lnkDeletePortalProduct_OnClick" OnClientClick="addRegionLoader('divloader')" runat="server"><i class="fa fa-times"></i></asp:LinkButton>
                                                    </div>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <div class="form-group">
                                        <div class="col-lg-11 col-md-11 col-sm-10 text-right mt--10">
                                            <asp:LinkButton ID="lnkAddNewPortalProduct" OnClick="lnkAddNewPortalProduct_Click" OnClientClick="addRegionLoader('divloader')" runat="server"><i class="fa fa-plus"></i> Add New</asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="ClosePopup btn btn-raised btn-default">
                                Cancel</button>
                            <asp:Button ID="btnSaveProduct" OnClick="btnSaveProduct_OnClick" runat="server" CssClass="btnSaveProduct btn btn-raised btn-black"
                                Text="Save" />
                            <asp:Button ID="btnSaveAndNewProduct" OnClick="btnSaveAndNewProduct_OnClick" runat="server"
                                CssClass="btnSaveAndNewProduct btn btn-raised btn-black" Text="Save & New" />
                            <asp:Button ID="btnSaveAndCloneProduct" OnClick="btnSaveAndCloneProduct_OnClick" runat="server"
                                CssClass="btnSaveAndCloneProduct btn btn-raised btn-black" Text="Save & Clone" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:LinkButton ID="lnkFakeExcelExport" runat="server"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="popupExcelExport" runat="server" DropShadow="false" PopupControlID="pblExcelExport"
        BehaviorID="PopupBehaviorID3" TargetControlID="lnkFakeExcelExport" BackgroundCssClass="modalBackground">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pblExcelExport" CssClass="modelpopup col-lg-3 col-md-3 col-sm-6 col-xs-12 p0"
        Style="display: none" runat="server">
        <EE:ExcelExportPopup ID="ExcelExport" runat="server" />
    </asp:Panel>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="script" runat="Server">
    <script type="text/javascript" src="<%= CU.StaticFilePath %>plugins/js/jquery-ui.js"></script>
    <script src="<%= CU.StaticFilePath %>plugins/jqueryTypeahead-Autocompletion/js/typeahead.bundle.min.js"></script>
    <script src="<%= CU.StaticFilePath %>plugins/jqueryTypeahead-Autocompletion/js/typeahead.tagging.js"></script>
    <script type="text/javascript">
        function AddControl() {
            if ($(".lnkAdd").attr("class") != undefined) {
			    <%= Page.ClientScript.GetPostBackEventReference(lnkAdd, String.Empty) %>;
                ShowPopupAndLoader(4, "divloaderProduct");
                return true;
            }
            else {
                return false;
            }
        }

        function EditControl() {
            if ($(".lnkEdit").attr("class") != undefined) {
                if (IsValidRowSelection()) {
			        <%= Page.ClientScript.GetPostBackEventReference(lnkEdit, String.Empty) %>;
                    ShowPopupAndLoader(4, "divloaderProduct");
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }
        }

        function ActiveControl() {
            if ($(".lnkActive").attr("class") != undefined) {
                addLoader('lnkActive');
			    <%= Page.ClientScript.GetPostBackEventReference(lnkActive, String.Empty) %>;
            }
        }

        function DeactiveControl() {
            if ($(".lnkDeactive").attr("class") != undefined) {
                addLoader('lnkDeactive');
			    <%= Page.ClientScript.GetPostBackEventReference(lnkDeactive, String.Empty) %>;
            }
        }

        function DeleteControl() {
            if ($(".lnkDelete").attr("class") != undefined) {
                addLoader('lnkDelete');
			    <%= Page.ClientScript.GetPostBackEventReference(lnkDelete, String.Empty) %>;
            }
        }

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Checkpostback);
        jQuery(function () {
            Checkpostback();
        });
        function Checkpostback() {
            $(".lnkAdd").click(function () {
                ShowPopupAndLoader(4, "divloaderProduct");
                return true;
            });

            $(".lnkEdit").click(function () {
                ShowPopupAndLoader(4, "divloaderProduct");
                return true;
            });

            $(".lnkClone").click(function () {
                ShowPopupAndLoader(4, "divloaderProduct");
                return true;
            });

            $(".lnkEditProduct").click(function () {
                ShowPopupAndLoader(4, "divloaderProduct");
                return true;
            });

            $(".lnkExcelImport").click(function () {
                document.getElementById("<%= chkReplace.ClientID %>").checked = false;
                ShowOnlyPopup("2");
                return false;
            });

            $(".lnkExcelExport").click(function () {
                ShowPopupAndLoader(3, "divloaderexport");
                return true;
            });


            $(".txtVendorCode").change(function () {
                $(".txtProductCode").val($(".txtVendorCode").val());
            });

            AdjustTextaria("txtNameTag", "txtNameTag");
            $(".txtNameTag").keyup(function () {
                AdjustTextaria("txtNameTag", "txtNameTag");
            });

            AdjustTextaria("txtDescription", "txtDescription");
            $(".txtDescription").keyup(function () {
                AdjustTextaria("txtDescription", "txtDescription");
            });


            $(".btnSaveProduct").click(function () {
                if (CheckValidation("checkvalidProductDetail")) {
                    addLoader('btnSaveProduct');
                    return true;
                }
                else {
                    return false;
                }
            });

            $(".btnSaveAndNewProduct").click(function () {
                if (CheckValidation("checkvalidProductDetail")) {
                    addLoader('btnSaveAndNewProduct');
                    return true;
                }
                else {
                    return false;
                }
            });

            $(".btnSaveAndCloneProduct").click(function () {
                if (CheckValidation("checkvalidProductDetail")) {
                    addLoader('btnSaveAndCloneProduct');
                    return true;
                }
                else {
                    return false;
                }
            });

            jQuery.getScript("<%= CU.StaticFilePath %>plugins/build/js/jquery.ezdz.min.js");

            setTimeout(function () {
                try {
                    $('.dropfile').ezdz({
                        text: 'Drop a Image',
                        validators: {},
                        accept: function () { },
                        reject: function (file, errors) { }
                    });
                }
                catch (e) { }
            }, 500);

            //Image

            SetSortable("ProductImage");
            SetSortable("ProductImageOriginal");
            SetSortable("ProductImageCustomer");

            function SetSortable(ImageType) {
                $(".sortable" + ImageType).sortable({
                    revert: false,
                    cursor: "move",
                    scroll: false,
                    dropOnEmpty: false,
                    stop: function (event, ui) {
                        var i = 0;
                        $(".sortable" + ImageType + " .txtImageSerialNo").each(function () {
                            i++;
                            $(this).val(i);
                        });
                    }
                });
            }

            var divRemoveImage = "";
            $(".RemoveProductImage").click(function () {
                $(this).parent().find(".txteStatus").val("2");
                $(this).parent().fadeOut();
                divRemoveImage = $(this).parent();
                setTimeout(function () {
                    divRemoveImage.addClass("hideimage");
                }, 400);
            });

            setProductImageWidth();
            function setProductImageWidth() {
                var ImageWidth = $(".divProductImage").width();

                if (parseInt(ImageWidth) > 50) {
                    $(".imgProduct").width(ImageWidth + "px");
                    $(".imgProduct").height(ImageWidth + "px");
                }

                setTimeout(function () {
                    setProductImageWidth();
                }, 1000);
            }

            $(".fuProductImage").change(function () {
                UploadImage("ProductImage");
            });

            $(".fuProductImageOriginal").change(function () {
                UploadImage("ProductImageOriginal");
            });

            $(".fuProductImageCustomer").change(function () {
                UploadImage("ProductImageCustomer");
            });

            function UploadImage(ImageType) {
                var thisVar = document.getElementById('fu' + ImageType);

                if (thisVar != null && thisVar.files.length > 0) {
                    addRegionLoader('divloaderProduct');
                    var data = new FormData();
                    for (var i = 0; i < thisVar.files.length; i++) {
                        data.append(thisVar.files[i].name, thisVar.files[i]);
                    }

                    $.ajax({
                        url: "FileUploadHandler.ashx",
                        type: "POST",
                        data: data,
                        contentType: false,
                        processData: false,
                        success: function (result) {
                            var Flist = result.split("~");
                            var FLink = "";
                            var ImagePath = "";
                            for (var i = 0; i < Flist.length; i++) {
                                ImagePath = ImagePath + Flist[i] + "~";
                            }

                            $(".txt" + ImageType).val(ImagePath);

                            if (ImageType == "ProductImage")
								<%= Page.ClientScript.GetPostBackEventReference(lnkProductImageUpload, String.Empty) %>;
                            else if (ImageType == "ProductImageOriginal")
								<%= Page.ClientScript.GetPostBackEventReference(lnkProductImageOriginalUpload, String.Empty) %>;
                            else if (ImageType == "ProductImageCustomer")
								<%= Page.ClientScript.GetPostBackEventReference(lnkProductImageCustomerUpload, String.Empty) %>;

                        },
                        error: function (err) {
                            alert(err.statusText)
                        }
                    });
                }
            }

            var tagsource = [];
            var arrOrderComplainEmailList = $('.lblNameTagList').text().split(',');
            $.each(arrOrderComplainEmailList, function (index, value) {
                if (value != "") {
                    tagsource.push(value);
                }
            });

            $('.txtNameTag').tagging(tagsource);

            ManageTabProduct("", "");
        }

        function ManageTabProduct(activetab, activepanel) {
            var arractivetab = ['liTabProductDetail', 'liTabProductImage', 'liTabProductVariant', 'liTabProductPriceList', 'liTabPortalProduct'];
            var arractivepanel = ['pnlProductDetail', 'pnlProductImage', 'pnlProductVariant', 'pnlProductPriceList', 'pnlPortalProduct'];

            arractivetab = arractivetab.join(',');
            arractivepanel = arractivepanel.join(',');

            ManageDetailTab(arractivetab, arractivepanel, activetab, activepanel, "txtActiveTabProduct");
        }

    </script>

</asp:Content>
