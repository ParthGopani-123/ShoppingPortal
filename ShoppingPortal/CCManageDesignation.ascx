<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CCManageDesignation.ascx.cs"
    Inherits="CCManageDesignation" %>
<asp:UpdatePanel ID="UpdatePanel2" runat="server">
    <ContentTemplate>
        <asp:Label ID="lblDesignationId" runat="server" Visible="false" Text=""></asp:Label>
			<asp:Label ID="lblOrganizationId" runat="server" Visible="false"></asp:Label>
        <asp:Panel ID="pnlDesignation" DefaultButton="btnSaveDesignation" runat="server">
            <div class="modal-dialog">
                <div class="modal-content darkmodel">
                    <div class="modal-header bg-black">
                        <button type="button" class="ClosePopup close">
                            ×</button>
                        <h4 class="modal-title">
                            <asp:Label ID="lblPopupTitle" runat="server">Designation</asp:Label></h4>
                    </div>
                    <div class="modal-body divloaderdesignation div-check-validation-Designation">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                    Designation<span class="text-danger">*</span>
                                </label>
                                <div class="col-lg-8 col-md-8 col-sm-7">
                                    <asp:DropDownList ID="ddlDesignation" CssClass="form-control" ZValidation="e=change|v=IsSelect|m=Designation"
                                        runat="server">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                    Name<span class="text-danger">*</span>
                                </label>
                                <div class="col-lg-8 col-md-8 col-sm-7">
                                    <asp:TextBox ID="txtDesignationName" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=Designation Name"
                                        runat="server" MaxLength="30" placeholder="Enter Designation"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                    Serial No<span class="text-danger">*</span>
                                </label>
                                <div class="col-lg-8 col-md-8 col-sm-7">
                                    <asp:TextBox ID="txtSerialNo" CssClass="form-control intnumber" ZValidation="e=blur|v=IsNumber|m=Serial No"
                                        runat="server" MaxLength="3" placeholder="Enter Serial No"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="ClosePopup btn btn-raised btn-default">
                            Cancel</button>
                        <asp:Button ID="btnSaveDesignation" OnClick="btnSaveDesignation_OnClick" runat="server"
                            CssClass="btnSaveDesignation btn btn-raised btn-black" Text="Save" />
                        <asp:Button ID="btnSaveAndNewDesignation" OnClick="btnSaveAndNewDesignation_OnClick"
                            runat="server" CssClass="btnSaveAndNewDesignation btn btn-raised btn-black"
                            Text="Save & New" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckpostbackDesignation);
    function CheckpostbackDesignation() {
        $(".btnSaveDesignation").click(function () {
            if (CheckValidation("div-check-validation-Designation")) {
                addLoader('btnSaveDesignation');
                return true;
            }
            else {
                return false;
            }
        });

        $(".btnSaveAndNewDesignation").click(function () {
            if (CheckValidation("div-check-validation-Designation")) {
                addLoader('btnSaveAndNewDesignation');
                return true;
            }
            else {
                return false;
            }
        });
    }
</script>

