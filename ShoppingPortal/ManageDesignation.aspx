<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="ManageDesignation.aspx.cs" Inherits="ManageDesignation" Title="Designation"
    EnableEventValidation="false" %>

<%@ Register Src="~/CCManageAuthority.ascx" TagName="ManageAuthority" TagPrefix="MA" %>
<%@ Register Src="~/CCConfirmationPopup.ascx" TagName="ConfirmationPopup" TagPrefix="CP" %>
<%@ Register Src="~/CCManageDesignation.ascx" TagName="ManageDesignationPopup" TagPrefix="MC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Label ID="lbltabHeight1" runat="server" CssClass="lbltabHeight1 hidden" Text="151"></asp:Label>
            <asp:Label ID="lbltabHeightSmall1" runat="server" CssClass="lbltabHeightSmall1 hidden"
                Text="190"></asp:Label>
            <asp:Label ID="lblDesignationId" runat="server" Visible="false" Text=""></asp:Label>
			<asp:Label ID="lblOrganizationId" runat="server" Visible="false"></asp:Label>
            <asp:Panel ID="pnlDesignation" runat="server" class="row">
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
                            <asp:LinkButton ID="lnkActive" ToolTip="Active (alt+a)" allowon="1" runat="server" OnClick="lnkActive_OnClick"
                                CssClass="lnkActive btn btngroup btn-active tooltips clickloader" data-toggle="tooltip">
                            <i class="fa fa-check"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkDeactive" ToolTip="Deactive (alt+r)" allowon="1" runat="server" OnClick="lnkDeactive_OnClick"
                                CssClass="lnkDeactive btn btngroup btn-deactive tooltips clickloader" data-toggle="tooltip">
                            <i class="fa fa-ban"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkDelete" ToolTip="Delete (alt+x)" allowon="1" runat="server" OnClick="lnkDelete_OnClick"
                                CssClass="lnkDelete btn btngroup btn-delete tooltips clickloader" data-toggle="tooltip">
                            <i class="fa fa-trash-o"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkRefresh" ToolTip="Refresh" runat="server" OnClick="lnkRefresh_OnClick"
                                CssClass="btn btngroup btn-refresh tooltips clickloader" data-toggle="tooltip">
                            <i class="fa fa-refresh"></i>
                            </asp:LinkButton>
                        </div>
                        <div class="btn-group">
                            <asp:LinkButton ID="lnkSetAuthority" ToolTip="Set Authority" allowon="1" runat="server" OnClick="lnkSetAuthority_OnClick"
                                CssClass="btn btngroup btn-extra1 clickloader" data-toggle="tooltip">
                            <i class="fa fa-key"></i>
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
                        <asp:GridView ID="grdDesignation" AllowPaging="false" runat="server" OnRowDataBound="grdDesignation_OnRowDataBound"
                            OnSelectedIndexChanged="grdDesignation_OnSelectedIndexChanged" AutoGenerateColumns="False"
                            class="table table-bordered table-hover nomargin selectonrowclick rowloader fixheader">
                            <Columns>
                                <asp:TemplateField HeaderStyle-CssClass="hide" ItemStyle-CssClass="hide">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelect" CssClass="" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="DesignationId" HeaderText="DesignationId" HeaderStyle-CssClass="hide"
                                    ItemStyle-CssClass="hide" />
                                <asp:BoundField DataField="eStatus" HeaderText="eStatus" HeaderStyle-CssClass="hide"
                                    ItemStyle-CssClass="hide" />
                                <asp:TemplateField HeaderText="Designation">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEditDesignation" CssClass="lnkEditDesignation" OnClick="lnkEditDesignation_OnClick"
                                            runat="server"></asp:LinkButton>
                                        <asp:Literal ID="ltrDesignation" runat="server"></asp:Literal>
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
            </asp:Panel>
            <asp:Panel ID="pnlDesignationAuthority" runat="server" class="row">
                <MA:ManageAuthority ID="ManageAuthority" runat="server" />
            </asp:Panel>
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
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:LinkButton ID="lnkFackDesignation" runat="server"> </asp:LinkButton>
    <cc1:ModalPopupExtender ID="popupDesignation" runat="server" DropShadow="false" BehaviorID="PopupBehaviorID3"
        PopupControlID="pnlpopupDesignation" TargetControlID="lnkFackDesignation" BackgroundCssClass="modalBackground">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlpopupDesignation" runat="server" CssClass="modelpopup col-lg-4 col-md-4 col-sm-8 col-xs-12 p0"
        Style="display: none">
        <MC:ManageDesignationPopup ID="popupManageDesignation" runat="server" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="script" runat="Server">

    <script type="text/javascript">
        function AddControl() {
            if ($(".lnkAdd").attr("class") != undefined) {
			    <%= Page.ClientScript.GetPostBackEventReference(lnkAdd, String.Empty) %>;
                ShowPopupAndLoader(3, "divloaderdesignation");
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
                    ShowPopupAndLoader(3, "divloaderdesignation");
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

            $(".lnkEditDesignation").click(function () {
                ShowPopupAndLoader(3, "divloaderdesignation");
                return true;
            });
        }

    </script>

    <script type="text/javascript">

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckpostbackAuthority);
        function CheckpostbackAuthority() {

            CheckAllSelect();
            function CheckAllSelect() {
                SelectAllCheck('chkAllView', 'chkView');
                SelectAllCheck('chkAllAddEdit', 'chkAddEdit');
                SelectAllCheck('chkAllDelete', 'chkDelete');

                if ($(".chkAddEdit").length == 0) {
                    $('.chkAllAddEdit input[type="checkbox"]').prop("checked", false);
                }

                if ($(".chkDelete").length == 0) {
                    $('.chkAllDelete input[type="checkbox"]').prop("checked", false);
                }
            }

            checkSelectAllRow();

            $(".chkAllRow").change(function () {

                var AuthorityId = $(this).attr("AuthorityId");
                var Ischecked = $('.chkAllRow' + AuthorityId + ' input[type="checkbox"]').prop('checked');

                $('.chkView' + AuthorityId + ' input[type="checkbox"]').prop("checked", Ischecked);
                $('.chkAddEdit' + AuthorityId + ' input[type="checkbox"]').prop("checked", Ischecked);
                $('.chkDelete' + AuthorityId + ' input[type="checkbox"]').prop("checked", Ischecked);

                CheckAllSelect();
            });

            function checkSelectAllRow() {
                $(".chkAllRow").each(function () {
                    checkSelectRow($(this).attr("AuthorityId"));
                });
            }

            function checkSelectRow(AuthorityId) {
                var allcheck = ($('.chkView' + AuthorityId + ' input[type="checkbox"]').prop('checked')
                    && ($(".chkAddEdit" + AuthorityId).length == 0 || $('.chkAddEdit' + AuthorityId + ' input[type="checkbox"]').prop('checked'))
                    && ($(".chkDelete" + AuthorityId).length == 0 || $('.chkDelete' + AuthorityId + ' input[type="checkbox"]').prop('checked')));

                $('.chkAllRow' + AuthorityId + ' input[type="checkbox"]').prop("checked", allcheck);
            }

            $(".chkView").change(function () {
                var AuthorityId = $(this).attr("AuthorityId");
                if ($('.chkAddEdit' + AuthorityId + ' input[type="checkbox"]').prop('checked')
                    || $('.chkDelete' + AuthorityId + ' input[type="checkbox"]').prop('checked')) {
                    $('.chkView' + AuthorityId + ' input[type="checkbox"]').prop("checked", true);
                    SelectAllCheck('chkAllView', 'chkView');
                }

                checkSelectRow(AuthorityId);
            });
            $(".chkAllView").change(function () {
                $(".chkView").each(function () {
                    var AuthorityId = $(this).attr("AuthorityId");
                    if ($('.chkAddEdit' + AuthorityId + ' input[type="checkbox"]').prop('checked')
                        || $('.chkDelete' + AuthorityId + ' input[type="checkbox"]').prop('checked')) {
                        $('.chkView' + AuthorityId + ' input[type="checkbox"]').prop("checked", true);
                    }
                });

                SelectAllCheck('chkAllView', 'chkView');
                checkSelectAllRow();
            });


            $(".chkAddEdit").change(function () {
                var AuthorityId = $(this).attr("AuthorityId");
                if ($('.chkAddEdit' + AuthorityId + ' input[type="checkbox"]').prop('checked')) {
                    $('.chkView' + AuthorityId + ' input[type="checkbox"]').prop("checked", true);
                    SelectAllCheck('chkAllView', 'chkView');
                }

                checkSelectRow(AuthorityId);
            });
            $(".chkAllAddEdit").change(function () {
                $(".chkAddEdit").each(function () {
                    var AuthorityId = $(this).attr("AuthorityId");
                    if ($('.chkAddEdit' + AuthorityId + ' input[type="checkbox"]').prop('checked')) {
                        $('.chkView' + AuthorityId + ' input[type="checkbox"]').prop("checked", true);
                    }
                });

                SelectAllCheck('chkAllView', 'chkView');
                checkSelectAllRow();
            });


            $(".chkDelete").change(function () {
                var AuthorityId = $(this).attr("AuthorityId");
                if ($('.chkDelete' + AuthorityId + ' input[type="checkbox"]').prop('checked')) {
                    $('.chkView' + AuthorityId + ' input[type="checkbox"]').prop("checked", true);
                    SelectAllCheck('chkAllView', 'chkView');
                }

                checkSelectRow(AuthorityId);
            });
            $(".chkAllDelete").change(function () {
                $(".chkDelete").each(function () {
                    var AuthorityId = $(this).attr("AuthorityId");
                    if ($('.chkDelete' + AuthorityId + ' input[type="checkbox"]').prop('checked')) {
                        $('.chkView' + AuthorityId + ' input[type="checkbox"]').prop("checked", true);
                    }
                });

                SelectAllCheck('chkAllView', 'chkView');
                checkSelectAllRow();
            });
        }

    </script>

</asp:Content>
