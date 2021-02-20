<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="ManageCourier.aspx.cs" Inherits="ManageCourier" Title="Courier" EnableEventValidation="false" %>

<%@ Register Src="~/CCConfirmationPopup.ascx" TagName="ConfirmationPopup" TagPrefix="CP" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .tblShippingCharge thead tr th {
            text-align: center;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Label ID="lbltabHeight1" runat="server" CssClass="lbltabHeight1 hidden" Text="151"></asp:Label>
            <asp:Label ID="lbltabHeightSmall1" runat="server" CssClass="lbltabHeightSmall1 hidden"
                Text="190"></asp:Label>
            <asp:Label ID="lbltabHeight2" runat="server" CssClass="lbltabHeight2 hidden" Text="129"></asp:Label>
            <asp:Label ID="lblOrganizationId" runat="server" Visible="false"></asp:Label>
            <asp:Label ID="lblFirmId" runat="server" Visible="false"></asp:Label>

            <asp:Label ID="lblCourierId" runat="server" Visible="false"></asp:Label>
            <asp:Panel ID="pnlCourier" runat="server" class="row">
                <div class="page-header clearfix">
                    <div class="col-lg-9 col-md-9 col-sm-9 col-xs-12 p0">
                        <div class="btn-group">
                            <asp:LinkButton ID="lnkAdd" ToolTip="Add (alt+n)" runat="server" OnClick="lnkAdd_OnClick"
                                CssClass="lnkAdd btn btngroup btn-add tooltips" data-toggle="tooltip">
                            <i class="fa fa-plus"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkEdit" ToolTip="Edit (alt+u)" allowon="1" runat="server" OnClick="lnkEdit_OnClick"
                                CssClass="lnkEdit btn btngroup btn-edit tooltips" data-toggle="tooltip">
                            <i class="fa fa-edit"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkActive" ToolTip="Active (alt+a)" allowon="1" runat="server"
                                OnClick="lnkActive_OnClick" CssClass="lnkActive btn btngroup btn-active tooltips clickloader"
                                data-toggle="tooltip">
                            <i class="fa fa-check"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkDeactive" ToolTip="Deactive (alt+r)" allowon="1" runat="server"
                                OnClick="lnkDeactive_OnClick" CssClass="lnkDeactive btn btngroup btn-deactive tooltips clickloader"
                                data-toggle="tooltip">
                            <i class="fa fa-ban"></i>
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
                        <div class="btn-group">
                            <asp:LinkButton ID="lnkShippingCharge" ToolTip="Shipping Charge" allowon="1" runat="server" OnClick="lnkShippingCharge_OnClick"
                                CssClass="btn btngroup btn-extra1 tooltips clickloader" data-toggle="tooltip">
                            <i class="fa fa-inr"></i>
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
                        <asp:GridView ID="grdCourier" AllowPaging="false" runat="server" OnRowDataBound="grdCourier_OnRowDataBound"
                            OnSelectedIndexChanged="grdCourier_OnSelectedIndexChanged" AutoGenerateColumns="False"
                            class="table table-bordered table-hover nomargin selectonrowclick rowloader fixheader">
                            <Columns>
                                <asp:TemplateField HeaderStyle-CssClass="hide" ItemStyle-CssClass="hide">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelect" CssClass="" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="CourierId" HeaderText="CourierId" HeaderStyle-CssClass="hide"
                                    ItemStyle-CssClass="hide" />
                                <asp:BoundField DataField="eStatus" HeaderText="eStatus" HeaderStyle-CssClass="hide"
                                    ItemStyle-CssClass="hide" />
                                <asp:BoundField DataField="SerialNo" HeaderText="Serial No." />
                                <asp:TemplateField HeaderText="Courier">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEditCourier" CssClass="lnkEditCourier" OnClick="lnkEditCourier_OnClick"
                                            runat="server"></asp:LinkButton>
                                        <asp:Literal ID="ltrCourier" runat="server"></asp:Literal>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="CarrierCode" HeaderText="Carrier Code" />
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
            <asp:Panel ID="pnlShippingCharge" runat="server" class="row">
                <div class="page-content col-md-12 divtable divloader">
                    <div class="table-responsive tabHeight2 divValidateShippingCharge">
                        <table class="tblShippingCharge table table-bordered table-hover nomargin">
                            <thead>
                                <tr>
                                    <th>*</th>
                                    <asp:Repeater ID="rptZoneHead" runat="server" OnItemDataBound="rptZoneHead_OnItemDataBound">
                                        <ItemTemplate>
                                            <th id="tdZone" runat="server">
                                                <asp:Literal ID="ltrZone" runat="server"></asp:Literal>
                                            </th>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <th>*</th>
                                </tr>
                                <tr>
                                    <th>*</th>
                                    <asp:Repeater ID="rptZoneHead2" runat="server" OnItemDataBound="rptZoneHead2_OnItemDataBound">
                                        <ItemTemplate>
                                            <asp:Repeater ID="rptCourierTypeHead" runat="server" OnItemDataBound="rptCourierTypeHead_OnItemDataBound">
                                                <ItemTemplate>
                                                    <th colspan="2">
                                                        <asp:Literal ID="ltrCourierTypeHead" runat="server"></asp:Literal>
                                                    </th>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <th>*</th>
                                </tr>
                                <tr>
                                    <th>Weight</th>
                                    <asp:Repeater ID="rptZoneHead3" runat="server" OnItemDataBound="rptZoneHead2_OnItemDataBound">
                                        <ItemTemplate>
                                            <asp:Repeater ID="rptCourierTypeHead" runat="server">
                                                <ItemTemplate>
                                                    <th>Charge</th>
                                                    <th>Firm Charge</th>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <th>*</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptShippingCharge" runat="server" OnItemDataBound="rptShippingCharge_OnItemDataBound">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="tdWeight">
                                                <asp:Label ID="lblPK" runat="server" Visible="false"></asp:Label>
                                                <asp:TextBox ID="txtWeight" runat="server" CssClass="form-control flotnumber2" MaxLength="6"></asp:TextBox>
                                            </td>
                                            <asp:Repeater ID="rptZone" runat="server" OnItemDataBound="rptZone_OnItemDataBound">
                                                <ItemTemplate>
                                                    <asp:Repeater ID="rptCourierType" runat="server" OnItemDataBound="rptCourierType_OnItemDataBound">
                                                        <ItemTemplate>
                                                            <td>
                                                                <asp:Label ID="lblCourierTypeId" runat="server" Visible="false"></asp:Label>
                                                                <asp:Label ID="lblZoneId" runat="server" Visible="false"></asp:Label>
                                                                <asp:TextBox ID="txtShipCharge" runat="server" CssClass="form-control intnumber" MaxLength="6"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtFirmShipCharge" runat="server" CssClass="form-control intnumber" MaxLength="6"></asp:TextBox>
                                                            </td>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <td>
                                                <asp:LinkButton ID="lnkDeleteShippingCharge" OnClick="lnkDeleteShippingCharge_OnClick" OnClientClick="addRegionLoader('divloader')" runat="server"><i class="fa fa-times"></i></asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12 p-0 pt-15">
                            <asp:LinkButton ID="lnkAddNewShippingCharge" OnClientClick="addRegionLoader('divloader')" OnClick="lnkAddNewShippingCharge_OnClick" runat="server"><i class="fa fa-plus"></i> Add New</asp:LinkButton>
                        </div>
                    </div>
                    <div class="modal-footer footer">
                        <asp:LinkButton ID="lnkCancelShippingCharge" OnClick="lnkCancelShippingCharge_OnClick" runat="server" CssClass="btn btn-raised btn-default clickloader">Cancel</asp:LinkButton>
                        <asp:LinkButton ID="lnkSaveShippingCharge" OnClick="lnkSaveShippingCharge_OnClick" runat="server" CssClass="lnkSaveShippingCharge btn btn-raised btn-black">Save</asp:LinkButton>
                    </div>
                </div>
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
            <asp:LinkButton ID="lnkFackCourier" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupCourier" runat="server" DropShadow="false" BehaviorID="PopupBehaviorID2"
                PopupControlID="pnlpopupCourier" TargetControlID="lnkFackCourier" BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlpopupCourier" runat="server" DefaultButton="btnSave" CssClass="modelpopup col-lg-6 col-md-6 col-sm-8 col-xs-12 p0"
                Style="display: none">
                <div class="modal-dialog">
                    <div class="modal-content darkmodel">
                        <div class="modal-header bg-black">
                            <button type="button" class="ClosePopup close">
                                ×</button>
                            <h4 class="modal-title">
                                <asp:Label ID="lblPopupTitle" runat="server">Courier</asp:Label></h4>
                        </div>
                        <div class="modal-body divloaderCourier checkvalidCourierDetail">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="row">
                                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                            Name<span class="text-danger">*</span>
                                        </label>
                                        <div class="col-lg-8 col-md-8 col-sm-7">
                                            <asp:TextBox ID="txtCourierName" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=Courier Name"
                                                runat="server" MaxLength="50" placeholder="Enter Courier"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="row">
                                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                            Serial No.<span class="text-danger">*</span>
                                        </label>
                                        <div class="col-lg-8 col-md-8 col-sm-7">
                                            <asp:TextBox ID="txtSerialNo" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=Serial No"
                                                runat="server" placeholder="Enter Serial No."></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="row">
                                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                            Carrier Code
                                        </label>
                                        <div class="col-lg-8 col-md-8 col-sm-7">
                                            <asp:TextBox ID="txtCarrierCode" CssClass="form-control" runat="server" placeholder="Enter Carrier Code"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="row">
                                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                            COD Tracking URL
                                        </label>
                                        <div class="col-lg-8 col-md-8 col-sm-7">
                                            <asp:TextBox ID="txtCODTrackingURL" CssClass="form-control" ZValidation="e=blur|v=IsNullURL|m=COD Tracking URL"
                                                runat="server" MaxLength="500" placeholder="Enter COD Tracking URL"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="row">
                                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                            Prepaid Tracking URL
                                        </label>
                                        <div class="col-lg-8 col-md-8 col-sm-7">
                                            <asp:TextBox ID="txtPrepaidTrackingURL" CssClass="form-control" ZValidation="e=blur|v=IsNullURL|m=Prepaid Tracking URL"
                                                runat="server" MaxLength="500" placeholder="Enter Prepaid Tracking URL"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="row">
                                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                        </label>
                                        <div class="col-lg-8 col-md-8 col-sm-7">
                                            <div class="checkbox-custom">
                                                <asp:CheckBox ID="chkIsPost" runat="server" CssClass="chk" Text="Post" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="ClosePopup btn btn-raised btn-default">
                                Cancel</button>
                            <asp:Button ID="btnSave" OnClick="btnSave_OnClick" runat="server" CssClass="btnSave btn btn-raised btn-black"
                                Text="Save" />
                            <asp:Button ID="btnSaveAndNew" OnClick="btnSaveAndNew_OnClick" runat="server"
                                CssClass="btnSaveAndNew btn btn-raised btn-black" Text="Save & New" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="script" runat="Server">

    <script type="text/javascript">
        function AddControl() {
            if ($(".lnkAdd").attr("class") != undefined) {
			    <%= Page.ClientScript.GetPostBackEventReference(lnkAdd, String.Empty) %>;
                ShowPopupAndLoader(2, "divloaderCourier");
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
                    ShowPopupAndLoader(2, "divloaderCourier");
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
                ShowPopupAndLoader(2, "divloaderCourier");
                return true;
            });

            $(".lnkEdit").click(function () {
                ShowPopupAndLoader(2, "divloaderCourier");
                return true;
            });

            $(".lnkEditCourier").click(function () {
                ShowPopupAndLoader(2, "divloaderCourier");
                return true;
            });

            $(".btnSave").click(function () {
                if (CheckValidation("checkvalidCourierDetail")) {
                    addLoader('btnSave');
                    return true;
                }
                else {
                    return false;
                }
            });

            $(".btnSaveAndNew").click(function () {
                if (CheckValidation("checkvalidCourierDetail")) {
                    addLoader('btnSaveAndNew');
                    return true;
                }
                else {
                    return false;
                }
            });

            $(".lnkSaveShippingCharge").click(function () {
                if (CheckValidation("divValidateShippingCharge")) {
                    addLoader('lnkSaveShippingCharge');
                    return true;
                }
                else {
                    return false;
                }
            });
        }

    </script>

</asp:Content>
