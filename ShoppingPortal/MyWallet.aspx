<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="MyWallet.aspx.cs" Inherits="MyWallet" Title="My Wallet" EnableEventValidation="false" %>

<%@ Register Src="~/CCConfirmationPopup.ascx" TagName="ConfirmationPopup" TagPrefix="CP" %>
<%@ Register Src="~/CCExcelExport.ascx" TagName="ExcelExportPopup" TagPrefix="EE" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .TransactionDescription span {
            display: block;
            font-size: 11px;
            color: #b5b5b5;
        }

        .icoTransaction {
            font-size: 15px !important;
            margin-right: 5px;
        }

        .color-red {
            color: Red;
        }

        .color-green {
            color: Green;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Label ID="lbltabHeight1" runat="server" CssClass="lbltabHeight1 hidden" Text="151"></asp:Label>
            <asp:Label ID="lbltabHeightSmall1" runat="server" CssClass="lbltabHeightSmall1 hidden"
                Text="190"></asp:Label>
            <asp:Label ID="lblOrganizationId" runat="server" Visible="false"></asp:Label>
            <asp:Label ID="lblFirmId" runat="server" Visible="false"></asp:Label>
            <asp:Label ID="lblWalletId" runat="server" Visible="false" Text=""></asp:Label>
            <div class="row">
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
                        <div class="btn-group pull-right mr-5">
                            <asp:LinkButton ID="lnkExcelExport" ToolTip="Excel Export" runat="server" OnClick="lnkExcelExport_OnClick"
                                CssClass="lnkExcelExport btn btngroup btn-export tooltips" data-toggle="tooltip">
                                <i class="fa fa-file-excel-o"></i>
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
                            <div id="divSearchUser" runat="server" class="padbm" note-colspan="note-colspan-class">
                                <asp:DropDownList ID="ddlSearchUser" CssClass="form-control" runat="server" note-colspan="note-colspan-class">
                                </asp:DropDownList>
                            </div>
                            <div class="padbm" note-colspan="note-colspan-class">
                                <div class="input-group">
                                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control datepickerpast"
                                        note-colspan="note-colspan-class"></asp:TextBox>
                                    <span class="input-group-addon" note-colspan="note-colspan-class">to</span>
                                    <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control datepickerpast"
                                        note-colspan="note-colspan-class"></asp:TextBox>
                                </div>
                            </div>
                            <div id="divSearchActive" runat="server" class="col-md-6 col-sm-6 col-xs-6 col-sm-6 searchchk"
                                note-colspan="note-colspan-class">
                                <div class="checkbox-custom" note-colspan="note-colspan-class">
                                    <asp:CheckBox ID="chkActive" runat="server" note-colspan="note-colspan-class" CssClass="chk"
                                        Checked="true" Text="Active" />
                                </div>
                            </div>
                            <div id="divSearchDeactive" runat="server" class="col-md-6 col-sm-6 col-xs-6 col-sm-6 searchchk"
                                note-colspan="note-colspan-class">
                                <div class="checkbox-custom" note-colspan="note-colspan-class">
                                    <asp:CheckBox ID="chkDeactive" runat="server" note-colspan="note-colspan-class" CssClass="chk"
                                        Text="Deactive" />
                                </div>
                            </div>
                            <div class="padbm text-right" note-colspan="note-colspan-class">
                                <asp:LinkButton ID="lnkSearch" note-colspan="note-colspan-class" OnUserClick="addRegionLoader('divloader')"
                                    OnClick="Control_CheckedChanged" class="btn btn-warning btnsearch" runat="server"><i class="fa fa-filter"></i> Filter</asp:LinkButton>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
                <div class="page-content col-md-12 divtable divloader">
                    <div class="table-responsive tabHeight1 fixheight2">
                        <asp:GridView ID="grdWallet" AllowPaging="false" runat="server" OnRowDataBound="grdWallet_OnRowDataBound"
                            OnSelectedIndexChanged="grdWallet_OnSelectedIndexChanged" AutoGenerateColumns="False"
                            class="table table-bordered table-hover nomargin selectonrowclick rowloader fixheader">
                            <Columns>
                                <asp:TemplateField HeaderStyle-CssClass="hide" ItemStyle-CssClass="hide">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelect" CssClass="" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="eStatus" HeaderText="eStatus" HeaderStyle-CssClass="hide"
                                    ItemStyle-CssClass="hide" />
                                <asp:BoundField DataField="WalletId" HeaderText="WalletId" HeaderStyle-CssClass="hide"
                                    ItemStyle-CssClass="hide" />
                                <asp:BoundField DataField="TransactionTime" HeaderText="Date" DataFormatString="{0:dd MMM yy}"
                                    HeaderStyle-CssClass="text-center" ItemStyle-CssClass="valigntop text-center" ItemStyle-Width="100px" HeaderStyle-Width="100px" />
                                <asp:TemplateField HeaderText="Transaction Id" HeaderStyle-CssClass="text-center"
                                    ItemStyle-CssClass="valigntop text-center">
                                    <ItemTemplate>
                                        <asp:Literal ID="ltrTransactionId" runat="server"></asp:Literal>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Narration" HeaderText="Description" HeaderStyle-CssClass=""
                                    ItemStyle-CssClass="valigntop" />
                                <asp:TemplateField HeaderText="Debit" ItemStyle-CssClass="valigntop text-right" HeaderStyle-CssClass="text-right" ItemStyle-Width="120px" HeaderStyle-Width="120px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTransactionAmountDebit" runat="server" CssClass="text-danger" Text=""></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Credit" ItemStyle-CssClass="valigntop text-right" HeaderStyle-CssClass="text-right" ItemStyle-Width="120px" HeaderStyle-Width="120px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTransactionAmountCredit" runat="server" CssClass="text-success" Text=""></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Balance" ItemStyle-CssClass="valigntop text-right" HeaderStyle-CssClass="text-right" ItemStyle-Width="120px" HeaderStyle-Width="120px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblBalance" runat="server" Text=""></asp:Label>
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
                        <asp:LinkButton ID="lnkFirst" OnClick="lnkFirst_Click" OnUserClick="addRegionLoader('divloader')"
                            runat="server" ToolTip="First Page" CssClass="fa fa-fast-backward btn-paging tooltips"></asp:LinkButton>
                        <asp:LinkButton ID="lnkPrev" OnClick="lnkPrev_Click" OnUserClick="addRegionLoader('divloader')"
                            runat="server" ToolTip="Previous Page" CssClass="fa fa-backward btn-paging tooltips"></asp:LinkButton>
                        <asp:TextBox ID="txtGotoPageNo" Text="1" runat="server" CssClass="txt-paging" OnTextChanged="txtGotoPageNo_OnTextChange"
                            OnChange="addRegionLoader('divloader')" AutoPostBack="true"></asp:TextBox>
                        <asp:LinkButton ID="lnkNext" OnClick="lnkNext_Click" OnUserClick="addRegionLoader('divloader')"
                            runat="server" ToolTip="Next Page" CssClass="fa fa-forward btn-paging tooltips"></asp:LinkButton>
                        <asp:LinkButton ID="lnkLast" OnClick="lnkLast_Click" OnUserClick="addRegionLoader('divloader')"
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
            <asp:LinkButton ID="lnkFackWallet" runat="server"> </asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupWallet" runat="server" DropShadow="false" BehaviorID="PopupBehaviorID2"
                PopupControlID="pnlpopupWallet" TargetControlID="lnkFackWallet" BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlpopupWallet" runat="server" DefaultButton="btnSave" CssClass="modelpopup col-lg-4 col-md-4 col-sm-8 col-xs-12 p0"
                Style="display: none">
                <div class="modal-dialog">
                    <div class="modal-content darkmodel">
                        <div class="modal-header bg-header">
                            <button type="button" class="ClosePopup close">
                                ×</button>
                            <h4 class="modal-title">
                                <asp:Label ID="lblPopupTitle" runat="server"></asp:Label></h4>
                        </div>
                        <div class="modal-body divloaderwallet divcheckvalidation">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                        User<span class="text-danger">*</span>
                                    </label>
                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                        <asp:DropDownList ID="ddlUser" CssClass="form-control" ZValidation="e=change|v=IsSelect|m=Account"
                                            runat="server">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                        Date<span class="text-danger">*</span>
                                    </label>
                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                        <asp:TextBox ID="txtTransactionTime" CssClass="form-control datetimepicker24past"
                                            ZValidation="e=change|v=IsRequired|m=Transaction Date" runat="server" MaxLength="10"
                                            placeholder="Enter Transaction Date"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                    </label>
                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                        <div class="radio-custom">
                                            <asp:RadioButton ID="rdoCredit" Text="Credit" GroupName="TransactionType" runat="server" />
                                        </div>
                                        <div class="radio-custom ml-15">
                                            <asp:RadioButton ID="rdoDebit" Text="Debit" GroupName="TransactionType" runat="server" />
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                        Amount<span class="text-danger">*</span>
                                    </label>
                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                        <asp:TextBox ID="txtAmount" CssClass="form-control flotnumber2" ZValidation="e=blur|v=IsNumber|m=Amount"
                                            runat="server" MaxLength="10" placeholder="Enter Amount"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                        Narration
                                    </label>
                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                        <asp:TextBox ID="txtNarration" CssClass="form-control" runat="server" TextMode="MultiLine"
                                            placeholder="Enter Narration"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="ClosePopup btn btn-raised btn-default">
                                Cancel</button>
                            <asp:Button ID="btnSave" OnClick="btnSave_OnClick" runat="server" CssClass="btnSave btn btn-raised btn-save"
                                Text="Save" />
                            <asp:Button ID="btnSaveAndNew" OnClick="btnSaveAndNew_OnClick" runat="server" CssClass="btnSaveAndNew btn btn-raised btn-saveandnew"
                                Text="Save & New" />
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

    <script type="text/javascript">
        function AddControl() {
            if ($(".lnkAdd").attr("class") != undefined) {
			    <%= Page.ClientScript.GetPostBackEventReference(lnkAdd, String.Empty) %>;
                ShowPopupAndLoader(2, "divloaderdestination");
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
                    ShowPopupAndLoader(2, "divloaderdestination");
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
                return AddControl();
            });

            $(".lnkEdit").click(function () {
                return EditControl();
            });

            $(".lnkExcelExport").click(function () {
                ShowPopupAndLoader(3, "divloaderexport");
                return true;
            });

            $(".btnSave").click(function () {
                if (CheckValidation("divcheckvalidation")) {
                    addLoader('btnSave');
                    return true;
                }
                else {
                    return false;
                }
            });

            $(".btnSaveAndNew").click(function () {
                if (CheckValidation("divcheckvalidation")) {
                    addLoader('btnSaveAndNew');
                    return true;
                }
                else {
                    return false;
                }
            });
        }
    </script>

</asp:Content>
