<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CCConfirmationPopup.ascx.cs"
    Inherits="CCConfirmationPopup" %>
<div class="modal-header">
    <a class="close ClosePopup">×</a>
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
    <button class="ClosePopup btn btn-default">Close</button>
    <asp:Button ID="btnActive" runat="server" CssClass="btn btn-info btnActive clickloader"
        Text="Active" OnClick="btnActive_OnClick" autofocus />
    <asp:Button ID="btnDeactive" runat="server" CssClass="btn btn-warning btnDeactive clickloader"
        Text="Deactive" OnClick="btnDeactive_OnClick" autofocus />
    <asp:Button ID="btnDelete" runat="server" CssClass="btn btn-danger btnDelete clickloader"
        Text="Delete" OnClick="btnDelete_OnClick" autofocus />
</div>
