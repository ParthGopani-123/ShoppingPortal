<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="ManageAdjustment.aspx.cs" Inherits="ManageAdjustment" Title="Adjustment" EnableEventValidation="false" %>

<%@ Register Src="~/CCConfirmationPopup.ascx" TagName="ConfirmationPopup" TagPrefix="CP" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .thPrice {
            text-align: center;
            width: 175px;
        }

        .tdProduct {
            padding: 0px !important;
        }

        .tdPrice {
            text-align: center;
            width: 175px;
            padding: 0px !important;
        }

        .txtQuantity, .txtProductSalePrice {
            border: none !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Label ID="lbltabHeight1" runat="server" CssClass="lbltabHeight1 hidden" Text="151"></asp:Label>
            <asp:Label ID="lbltabHeight2" runat="server" CssClass="lbltabHeight2 hidden" Text="139"></asp:Label>
            <asp:Label ID="lbltabHeightSmall1" runat="server" CssClass="lbltabHeightSmall1 hidden"
                Text="190"></asp:Label>
            <asp:Label ID="lblOrganizationId" runat="server" Visible="false"></asp:Label>
            <asp:Label ID="lblItemAdjustmentId" runat="server" Visible="false"></asp:Label>
            <asp:Panel ID="pnlItemAdjustment" runat="server" class="row">
                <div class="page-header clearfix">
                    <div class="col-lg-9 col-md-9 col-sm-9 col-xs-12 p0">
                        <div class="btn-group">
                            <asp:LinkButton ID="lnkAdd" ToolTip="Add (alt+n)" runat="server" OnClick="lnkAdd_OnClick"
                                CssClass="lnkAdd btn btngroup btn-add clickloader" data-toggle="tooltip">
                            <i class="fa fa-plus"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkEdit" ToolTip="Edit (alt+u)" allowon="1" runat="server" OnClick="lnkEdit_OnClick"
                                CssClass="lnkEdit btn btngroup btn-edit tooltips clickloader" data-toggle="tooltip">
                            <i class="fa fa-edit"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkDelete" ToolTip="Delete (alt+x)" allowon="1" runat="server"
                                OnClick="lnkDelete_OnClick" CssClass="lnkDelete btn btngroup btn-delete tooltips clickloader"
                                data-toggle="tooltip">
                            <i class="fa fa-trash-o"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkRefresh" ToolTip="Refresh" runat="server" OnClick="lnkRefresh_OnClick"
                                CssClass="btn btngroup btn-refresh tooltips clickloader" data-toggle="tooltip">
                            <i class="fa fa-refresh"></i>
                            </asp:LinkButton>
                        </div>
                    </div>
                    <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12 search div-master-search div-master-search-xs">
                        <asp:Label ID="lblCount" ToolTip="Count" runat="server" Text="10" CssClass="pull-left mr-5 btn btn-icon btn-total tooltips"
                            data-toggle="tooltip">
                        </asp:Label>
                        <div class="input-group">
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control master-search"
                                OnChange="addRegionLoader('divloader')" AutoPostBack="true" OnTextChanged="Control_CheckedChanged"
                                placeholder="Search..."></asp:TextBox>
                            <a note-colspan="note-colspan-class" class="tooltips input-group-addon aShowSearch btn btn-master-search ml5"
                                data-original-title="Search" data-toggle="tooltip"><i note-colspan="note-colspan-class"
                                    class="fa fa-chevron-down"></i></a>
                        </div>
                    </div>
                    <div note-colspan="note-colspan-class" class="search-tools divShowSearch note-colspan-class">
                        <asp:Panel ID="pnlSearch" runat="server" DefaultButton="lnkSearch" class="divsearchloader"
                            note-colspan="note-colspan-class">
                            <div class="padbm" note-colspan="note-colspan-class">
                                <div class="input-group">
                                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control datepicker"
                                        note-colspan="note-colspan-class"></asp:TextBox>
                                    <span class="input-group-addon" note-colspan="note-colspan-class">to</span>
                                    <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control datepicker"
                                        note-colspan="note-colspan-class"></asp:TextBox>
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
                        <asp:GridView ID="grdItemAdjustment" AllowPaging="false" runat="server" OnRowDataBound="grdItemAdjustment_OnRowDataBound"
                            OnSelectedIndexChanged="grdItemAdjustment_OnSelectedIndexChanged" AutoGenerateColumns="False"
                            class="table table-bordered table-hover nomargin selectonrowclick rowloader fixheader">
                            <Columns>
                                <asp:TemplateField HeaderStyle-CssClass="hide" ItemStyle-CssClass="hide">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelect" CssClass="" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="ItemAdjustmentId" HeaderText="ItemAdjustmentId" HeaderStyle-CssClass="hide"
                                    ItemStyle-CssClass="hide" />
                                <asp:TemplateField HeaderText="ReferenceNo">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEditItemAdjustment" CssClass="lnkEditItemAdjustment" OnClick="lnkEditItemAdjustment_OnClick"
                                            runat="server"></asp:LinkButton>
                                        <asp:Literal ID="ltrItemAdjustment" runat="server"></asp:Literal>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="AdjustmentDate" HeaderText="Date" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-center" DataFormatString="{0:dd-MM-yy}" />
                                <asp:BoundField DataField="Note" HeaderText="Note" />
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
            </asp:Panel>
            <asp:Panel ID="pnlItemAdjustmentDetail" runat="server" class="row">
                <asp:Panel ID="pnlOrder" runat="server" DefaultButton="lnkSave" class="page-content col-md-12 divtable divloader">
                    <div class="tabHeight2 checkvalidItemAdjustmentDetail mt-10">
                        <div class="form-horizontal">
                            <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                <div class="form-group">
                                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                        Reference No<span class="text-danger">*</span>
                                        <asp:TextBox ID="txtReferenceNo" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=ReferenceNo"
                                            runat="server" placeholder="Enter ReferenceNo"></asp:TextBox>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                        Date<span class="text-danger">*</span>
                                        <asp:TextBox ID="txtAdjustmentDate" CssClass="form-control datepicker" ZValidation="e=change|v=IsDate|m=Adjustment Date"
                                            runat="server" placeholder="Enter Adjustment Date"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                <div class="form-group">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        Note
                                        <asp:TextBox ID="txtNote" CssClass="form-control" runat="server" placeholder="Enter Note"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <table class="table table-bordered nomargin tblProduct">
                            <thead>
                                <tr>
                                    <th>Product</th>
                                    <th class="thPrice">Qty</th>
                                    <th class="thPrice">Rate</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptAdjustmentDetail" runat="server" OnItemDataBound="rptAdjustmentDetail_OnItemDataBound">
                                    <ItemTemplate>
                                        <tr>
                                            <td colspan="3" class="p-0"></td>
                                        </tr>
                                        <tr class="trOrderProduct">
                                            <td class="tdProduct" colspan="3">
                                                <asp:Label ID="lblPK" runat="server" Visible="false"></asp:Label>
                                                <div class="input-group">
                                                    <div class="divddlnoborder">
                                                        <asp:DropDownList ID="ddlProduct" CssClass="form-control ddlProduct" runat="server" AutoPostBack="true" onchange="addRegionLoader('divloader')" OnSelectedIndexChanged="ddlProduct_OnSelectedIndexChanged"></asp:DropDownList>
                                                    </div>
                                                    <asp:LinkButton ID="lnkDeleteProduct" CssClass="input-group-addon" OnClick="lnkDeleteProduct_OnClick" OnClientClick="addRegionLoader('divloader')" runat="server"><i class="fa fa-times"></i></asp:LinkButton>
                                                </div>
                                            </td>
                                        </tr>
                                        <asp:Repeater ID="rptAdjustmentItemDetail" runat="server" OnItemDataBound="rptAdjustmentItemDetail_OnItemDataBound">
                                            <ItemTemplate>
                                                <tr class="trOrderProduct">
                                                    <td>
                                                        <asp:Label ID="lblItemAdjustmentDetailId" runat="server" Visible="false"></asp:Label>
                                                        <asp:Label ID="lblItemId" runat="server" Visible="false"></asp:Label>
                                                        <asp:Label ID="lblItemName" runat="server"></asp:Label>
                                                    </td>
                                                    <td class="tdPrice">
                                                        <asp:TextBox ID="txtQuantity" CssClass="txtQuantity flotnumber form-control" placeholder="Quantity" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td class="tdPrice">
                                                        <asp:TextBox ID="txtRate" CssClass="txtProductSalePrice flotnumber form-control" placeholder="Rate" runat="server"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <tr>
                                    <td colspan="3" class="text-right">
                                        <asp:LinkButton ID="lnkAddNewProduct" OnClick="lnkAddNewProduct_OnClick" OnClientClick="addRegionLoader('divloader')" runat="server"><i class="fa fa-plus"></i> Add New</asp:LinkButton>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div class="modal-footer footer">
                        <asp:LinkButton ID="lnkCancel" OnClick="lnkCancel_OnClick" runat="server" CssClass="btn btn-raised btn-default clickloader">Cancel</asp:LinkButton>
                        <asp:LinkButton ID="lnkSave" OnClick="lnkSave_OnClick" runat="server" CssClass="lnkSave btn btn-raised btn-black">Save</asp:LinkButton>
                        <asp:LinkButton ID="lnkSaveAndNew" OnClick="lnkSaveAndNew_OnClick" runat="server" CssClass="lnkSaveAndNew btn btn-raised btn-black">Save & New</asp:LinkButton>
                    </div>
                </asp:Panel>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="popup" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <asp:LinkButton ID="lnkFakeConfirmation" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupConfirmation" runat="server" DropShadow="false"
                PopupControlID="pnlConfirmation" BehaviorID="PopupBehaviorID1" TargetControlID="lnkFakeConfirmation"
                BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlConfirmation" CssClass="modal-content zoomIn modal-confirmation col-xs-12 col-sm-12 col-md-12 p0"
                Style="display: none" runat="server">
                <CP:ConfirmationPopup ID="Confirmationpopup" runat="server" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="script" runat="Server">

    <script type="text/javascript">
        function AddControl() {
            if ($(".lnkAdd").attr("class") != undefined) {
			    <%= Page.ClientScript.GetPostBackEventReference(lnkAdd, String.Empty) %>;
                return true;
            }
            else {
                return true;
            }
        }

        function EditControl() {
            if ($(".lnkEdit").attr("class") != undefined) {
                if (IsValidRowSelection()) {
			        <%= Page.ClientScript.GetPostBackEventReference(lnkEdit, String.Empty) %>;
                    return true;
                }
                else {
                    return true;
                }
            }
            else {
                return false;
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

            $(".lnkSave").click(function () {
                if (CheckValidation("checkvalidItemAdjustmentDetail")) {
                    addLoader('lnkSave');
                    return true;
                }
                else {
                    return false;
                }
            });

            $(".lnkSaveAndNew").click(function () {
                if (CheckValidation("checkvalidItemAdjustmentDetail")) {
                    addLoader('lnkSaveAndNew');
                    return true;
                }
                else {
                    return false;
                }
            });
        }

    </script>

</asp:Content>
