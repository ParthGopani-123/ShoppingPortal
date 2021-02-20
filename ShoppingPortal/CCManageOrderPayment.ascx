<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CCManageOrderPayment.ascx.cs"
    Inherits="CCManageOrderPayment" %>
<asp:UpdatePanel ID="UpdatePanel2" runat="server">
    <ContentTemplate>
        <asp:Label ID="lblOrdersId" runat="server" Visible="false"></asp:Label>
        <asp:Label ID="lblOrderPaymentId" runat="server" Visible="false"></asp:Label>
        <asp:Panel ID="pnlOrderPayment" DefaultButton="btnSaveOrderPayment" runat="server">
            <div class="modal-dialog">
                <div class="modal-content darkmodel">
                    <div class="modal-header bg-black">
                        <button type="button" class="ClosePopup close">
                            ×</button>
                        <h4 class="modal-title">
                            <asp:Label ID="lblPopupTitle" runat="server">Order Payment</asp:Label></h4>
                    </div>
                    <div class="modal-body divloaderOrderPayment checkvalidOrderPaymentDetail">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                    Bank<span class="text-danger">*</span>
                                </label>
                                <div class="col-lg-8 col-md-8 col-sm-7">
                                    <asp:DropDownList ID="ddlBankAccount" CssClass="form-control" ZValidation="e=change|v=IsSelect|m=Bank Account"
                                        runat="server" MaxLength="8" placeholder="Enter Payment Amount">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                    Date<span class="text-danger">*</span>
                                </label>
                                <div class="col-lg-8 col-md-8 col-sm-7">
                                    <asp:TextBox ID="txtPaymentDate" CssClass="form-control datepicker" ZValidation="e=blur|v=IsDate|m=Payment Date"
                                        runat="server" placeholder="Enter Payment Date"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                    Amount<span class="text-danger">*</span>
                                </label>
                                <div class="col-lg-8 col-md-8 col-sm-7">
                                    <asp:TextBox ID="txtAmount" CssClass="form-control flotnumber" ZValidation="e=blur|v=IsNumber|m=Payment Amount"
                                        runat="server" MaxLength="8" placeholder="Enter Payment Amount"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                    TransactionId
                                </label>
                                <div class="col-lg-8 col-md-8 col-sm-7">
                                    <asp:TextBox ID="txtTransactionId" CssClass="form-control" runat="server" placeholder="Enter TransactionId"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                    Note
                                </label>
                                <div class="col-lg-8 col-md-8 col-sm-7 htmlareah100">
                                    <asp:TextBox ID="txtPaymentNote" CssClass="txtPaymentNote form-control" runat="server"
                                        TextMode="MultiLine" placeholder="Enter Note"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="ClosePopup btn btn-raised btn-default">Cancel</button>
                        <asp:Button ID="btnSaveOrderPayment" OnClick="btnSaveOrderPayment_OnClick" runat="server" CssClass="btnSaveOrderPayment btn btn-raised btn-black"
                            Text="Save" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">

    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckpostbackOrderPayment);
    function CheckpostbackOrderPayment() {
        AdjustTextaria("txtPaymentNote", "txtPaymentNote");
        $(".txtPaymentNote").keyup(function () {
            AdjustTextaria("txtPaymentNote", "txtPaymentNote");
        });

        $(".btnSaveOrderPayment").click(function () {
            if (CheckValidation("checkvalidOrderPaymentDetail")) {
                addLoader('btnSaveOrderPayment');
                return true;
            }
            else {
                return false;
            }
        });
    }

</script>

