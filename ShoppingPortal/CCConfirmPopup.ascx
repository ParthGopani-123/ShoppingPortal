<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CCConfirmPopup.ascx.cs"
    Inherits="CCConfirmPopup" %>
<div class="modal-header">
    <a id="aClosePopup" runat="server" class="ClosePopup close">×</a>
    <h3 class="modal-title text-center">
        <asp:Literal ID="lblPopupTitle" runat="server" Text=""></asp:Literal></h3>
</div>
<div class="modal-body text-center p0">
    <div class="row">
        <div class="col-md-12">
            <div class="form-group">
                <h4>
                    <asp:Literal ID="lblPopupMessage" runat="server" Text=""></asp:Literal></h4>
            </div>
        </div>
    </div>
</div>
<div class="modal-footer text-center">
    <asp:Button ID="btnCancelPopup" runat="server" CssClass="btn btn-default clickloader"
        Text="Cancel" OnClick="btnCancelPopup_OnClick" />
    <asp:Button ID="btnConfirmPopup" runat="server" CssClass="btn btn-info btnActive clickloader"
        Text="Confirm" OnClick="btnConfirmPopup_OnClick" autofocus />
</div>
