<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CCManageState.ascx.cs"
	Inherits="CCManageState" %>
<asp:UpdatePanel ID="UpdatePanel2" runat="server">
	<ContentTemplate>
		<asp:Label ID="lblStateId" runat="server" Visible="false" Text=""></asp:Label>
		<asp:Panel ID="pnlState" DefaultButton="btnSaveState" runat="server">
			<div class="modal-dialog">
				<div class="modal-content darkmodel">
					<div class="modal-header bg-black">
						<button type="button" class="ClosePopup close">
							×</button>
						<h4 class="modal-title">
							<asp:Label ID="lblPopupTitle" runat="server">State</asp:Label></h4>
					</div>
					<div class="modal-body divloaderstate div-check-validation-State">
						<div class="form-horizontal">
							<div class="form-group">
								<label class="col-lg-3 col-md-3 col-sm-4 control-label">
									Country<span class="text-danger">*</span>
								</label>
								<div class="col-lg-8 col-md-8 col-sm-7">
									<div class="input-group">
										<asp:DropDownList ID="ddlCountry" CssClass="form-control" ZValidation="e=change|v=IsSelect|m=Country"
											runat="server">
										</asp:DropDownList>
										<asp:LinkButton ID="lnkCountry" OnClick="lnkCountry_OnClick" CssClass="input-group-addon btnspinner tooltips"
											ToolTip="Refresh" data-toggle="tooltip" runat="server"><i class="fa fa-refresh"></i></asp:LinkButton>
									</div>
								</div>
							</div>
							<div class="form-group">
								<label class="col-lg-3 col-md-3 col-sm-4 control-label">
									Name<span class="text-danger">*</span>
								</label>
								<div class="col-lg-8 col-md-8 col-sm-7">
									<asp:TextBox ID="txtStateName" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=State Name"
										runat="server" MaxLength="30" placeholder="Enter State"></asp:TextBox>
								</div>
							</div>
							<div class="form-group">
								<label class="col-lg-3 col-md-3 col-sm-4 control-label">
									Description
								</label>
								<div class="col-lg-8 col-md-8 col-sm-7">
									<asp:TextBox ID="txtDescription" CssClass="txtDescriptionState form-control" runat="server"
										TextMode="MultiLine" placeholder="Enter Description"></asp:TextBox>
								</div>
							</div>
						</div>
					</div>
					<div class="modal-footer">
						<button type="button" class="ClosePopup btn btn-raised btn-default">
							Cancel</button>
						<asp:Button ID="btnSaveState" OnClick="btnSaveState_OnClick" runat="server" CssClass="btnSaveState btn btn-raised btn-black"
							Text="Save" />
						<asp:Button ID="btnSaveAndNewState" OnClick="btnSaveAndNewState_OnClick" runat="server"
							CssClass="btnSaveAndNewState btn btn-raised btn-black" Text="Save & New" />
					</div>
				</div>
			</div>
		</asp:Panel>
	</ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
	Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckpostbackState);
	function CheckpostbackState() {
		AdjustTextaria("txtDescriptionState", "txtDescriptionState");
		$(".txtDescriptionState").keyup(function () {
			AdjustTextaria("txtDescriptionState", "txtDescriptionState");
		});

		$(".btnSaveState").click(function () {
			if (CheckValidation("div-check-validation-State")) {
				addLoader('btnSaveState');
				return true;
			}
			else {
				return false;
			}
		});

		$(".btnSaveAndNewState").click(function () {
			if (CheckValidation("div-check-validation-State")) {
				addLoader('btnSaveAndNewState');
				return true;
			}
			else {
				return false;
			}
		});
	}
</script>

