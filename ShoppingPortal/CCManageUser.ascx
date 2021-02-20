<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CCManageUser.ascx.cs"
    Inherits="CCManageUser" %>
<%@ Register Src="~/CCManageAddress.ascx" TagName="ManageAddress" TagPrefix="MA" %>
<%@ Register Src="~/CCManageContact.ascx" TagName="ManageContact" TagPrefix="MC" %>
<asp:UpdatePanel ID="upnl" runat="server">
    <ContentTemplate>
        <style type="text/css">
            .Userdetminh {
                min-height: 300px;
            }
        </style>
        <asp:Label ID="lblUsersId" runat="server" Visible="false" Text=""></asp:Label>
        <asp:Label ID="lblAddressId" runat="server" Visible="false" Text=""></asp:Label>
        <asp:Panel ID="pnlUser" runat="server" DefaultButton="btnSave">
            <div class="modal-dialog">
                <div class="modal-content darkmodel">
                    <div class="modal-header bg-black">
                        <asp:LinkButton ID="btnClose" runat="server" CssClass="close ClosePopup" Text="×" />
                        <h4 class="modal-title">
                            <asp:Label ID="lblpopupUserTitle" runat="server">New User</asp:Label></h4>
                    </div>
                    <div class="modal-body div-loader-User div-check-validation-User pt-10">
                        <asp:TextBox ID="txtActiveTabUser" CssClass="txtActiveTabUser hidden" runat="server"></asp:TextBox>
                        <ul class="nav nav-tabs mb-15">
                            <li id="liTabUserDetail" onclick="ManageTabUser('liTabUserDetail', 'pnlUserDetail')"
                                runat="server" class="liTabUserDetail"><a>User</a></li>
                            <li id="liTabContactDetail" onclick="ManageTabUser('liTabContactDetail', 'pnlContactDetail')"
                                runat="server" class="liTabContactDetail"><a>Contact</a></li>
                            <li id="liTabAuthentication" onclick="ManageTabUser('liTabAuthentication', 'pnlAuthentication')"
                                runat="server" class="liTabAuthentication"><a>Authentication</a></li>
                        </ul>
                        <div id="pnlUserDetail" class="pnlUserDetail checkvalidUserDetail Userdetminh"
                            runat="server">
                            <div class="form-horizontal">
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                    <ContentTemplate>
                                        <div id="divOrganization" runat="server" class="form-group">
                                            <div class="row">
                                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                                    Organization<span class="text-danger">*</span>
                                                </label>
                                                <div class="col-lg-8 col-md-8 col-sm-7">
                                                    <asp:DropDownList ID="ddlOrganization" AutoPostBack="true" OnSelectedIndexChanged="ddlOrganization_OnSelectedIndexChanged" onchange="addRegionLoader('div-loader-User')"
                                                        CssClass="form-control" runat="server" ZValidation="e=change|v=IsSelect|m=Organization">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                        <div id="divFirm" runat="server" class="form-group">
                                            <div class="row">
                                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                                    Firm<span class="text-danger">*</span>
                                                </label>
                                                <div class="col-lg-8 col-md-8 col-sm-7">
                                                    <asp:DropDownList ID="ddlFirm" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlFirm_OnSelectedIndexChanged" onchange="addRegionLoader('div-loader-User')"
                                                        runat="server" ZValidation="e=change|v=IsSelect|m=Firm">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="row">
                                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                                    Designation<span class="text-danger">*</span>
                                                </label>
                                                <div class="col-lg-8 col-md-8 col-sm-7">
                                                    <asp:DropDownList ID="ddlDesignation" CssClass="form-control" runat="server" ZValidation="e=change|v=IsSelect|m=Designation">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="row">
                                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                                    Name<span class="text-danger">*</span>
                                                </label>
                                                <div class="col-lg-8 col-md-8 col-sm-7">
                                                    <asp:TextBox ID="txtName" CssClass="form-control" MaxLength="500" ZValidation="e=blur|v=IsRequired|m=User Name"
                                                        runat="server" placeholder="Enter User Name"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="row">
                                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                                    Mobile<span class="text-danger">*</span>
                                                </label>
                                                <div class="col-lg-8 col-md-8 col-sm-7">
                                                    <asp:TextBox ID="txtMobileNo" CssClass="form-control intnumber" MaxLength="10" ZValidation="e=blur|v=IsMobileNumber|m=Mobile No"
                                                        runat="server" placeholder="Enter Mobile No"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="row">
                                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                                    Email
                                                </label>
                                                <div class="col-lg-8 col-md-8 col-sm-7">
                                                    <asp:TextBox ID="txtEmail" CssClass="form-control" MaxLength="100" ZValidation="e=blur|v=IsNullEmail|m=Email"
                                                        runat="server" placeholder="Enter Email"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="row">
                                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                                    Parent User
                                                </label>
                                                <div class="col-lg-8 col-md-8 col-sm-7">
                                                    <asp:DropDownList ID="ddlParentUser" CssClass="form-control" runat="server">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="row">
                                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                                    Price List
                                                </label>
                                                <div class="col-lg-8 col-md-8 col-sm-7">
                                                    <asp:DropDownList ID="ddlPriceList" CssClass="form-control" runat="server">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="row">
                                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                                    Description
                                                </label>
                                                <div class="col-lg-8 col-md-8 col-sm-7">
                                                    <asp:TextBox ID="txtDescription" TextMode="MultiLine" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <div id="pnlContactDetail" class="pnlContactDetail checkvalidContactDetail Userdetminh"
                            runat="server">
                            <div class="form-horizontal">
                                <MA:ManageAddress ID="ManageAddress" runat="server" />
                                <MC:ManageContact ID="ManageContact" runat="server" />
                            </div>
                        </div>
                        <div id="pnlAuthentication" class="pnlAuthentication checkvalidAuthentication Userdetminh"
                            runat="server">
                            <div class="form-horizontal">
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                    <ContentTemplate>
                                        <div class="form-group">
                                            <div class="row">
                                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                                    <asp:Label ID="lblUserName" runat="server" Text="Username"></asp:Label><span class="text-danger">*</span>
                                                </label>
                                                <div class="col-lg-8 col-md-8 col-sm-7">
                                                    <asp:TextBox ID="txtUserName" CssClass="form-control text-lowercase" MaxLength="30" ZValidation="e=blur|v=IsRequired|m=User Name"
                                                        runat="server" placeholder="Enter User Name"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div id="divOldPassword" runat="server" class="form-group">
                                            <div class="row">
                                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                                    Old Password<span class="text-danger">*</span>
                                                </label>
                                                <div class="col-lg-8 col-md-8 col-sm-7">
                                                    <asp:TextBox ID="txtOldPassword" CssClass="txtOldPassword form-control" MaxLength="20"
                                                        runat="server" placeholder="Enter Old Password"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="row">
                                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                                    Password<span class="text-danger">*</span>
                                                </label>
                                                <div class="col-lg-8 col-md-8 col-sm-7">
                                                    <asp:TextBox ID="txtPassword" CssClass="txtPassword form-control" MaxLength="20"
                                                        TextMode="Password" runat="server" placeholder="Enter Password"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="row">
                                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                                    Confirm<span class="text-danger">*</span>
                                                </label>
                                                <div class="col-lg-8 col-md-8 col-sm-7">
                                                    <asp:TextBox ID="txtConfirmPassword" CssClass="txtConfirmPassword form-control" MaxLength="20"
                                                        TextMode="Password" runat="server" placeholder="Enter Confirm Password"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-raised btn-default ClosePopup" Text="Cancel" />
                        <asp:Button ID="btnSave" OnClick="btnSave_OnClick" runat="server" CssClass="btnSaveUser btn btn-raised btn-black" Text="Save" />
                        <asp:Button ID="btnSaveAndNew" OnClick="btnSaveAndNew_OnClick" runat="server" CssClass="btnSaveUserAndNew btn btn-raised btn-black" Text="Save & New" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckpostbackUser);

    function CheckpostbackUser() {
        $(".btnSaveUser").click(function () {
            if (CheckUserValidation()) {
                addLoader('btnSaveUser');
                return true;
            }
            else {
                return false;
            }
        });

        $(".btnSaveUserAndNew").click(function () {
            if (CheckUserValidation()) {
                addLoader('btnSaveUserAndNew');
                return true;
            }
            else {
                return false;
            }
        });

        function CheckUserValidation() {
            if (!CheckValidation("checkvalidUserDetail")) {
                ManageTabUser('liTabUserDetail', 'pnlUserDetail');
                return false;
            }
            else if (!CheckValidation("checkvalidContactDetail")) {
                ManageTabUser('liTabContactDetail', 'pnlContactDetail');
                return false;
            }
            else if (!CheckValidation("checkvalidAuthentication")
                || !IsMatch(".txtPassword", "", "Confirm Password", ".txtConfirmPassword")) {
                ManageTabUser('liTabAuthentication', 'pnlAuthentication');
                return false;
            }
            else {
                return true;
            }
        }
        ManageTabUser("", "");
    }

    function ManageTabUser(activetab, activepanel) {
        var arractivetab = ['liTabUserDetail', 'liTabContactDetail', 'liTabAuthentication'];
        var arractivepanel = ['pnlUserDetail', 'pnlContactDetail', 'pnlAuthentication'];
        arractivetab = arractivetab.join(',');
        arractivepanel = arractivepanel.join(',');

        ManageDetailTab(arractivetab, arractivepanel, activetab, activepanel, "txtActiveTabUser");
    }
</script>

