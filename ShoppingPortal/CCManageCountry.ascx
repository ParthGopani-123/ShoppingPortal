<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CCManageCountry.ascx.cs"
	Inherits="CCManageCountry" %>
<asp:UpdatePanel ID="UpdatePanel2" runat="server">
	<ContentTemplate>
		<asp:Label ID="lblCountryId" runat="server" Visible="false" Text=""></asp:Label>
		<asp:Panel ID="pnlCountry" DefaultButton="btnSaveCountry" runat="server">
			<div class="modal-dialog">
				<div class="modal-content darkmodel">
					<div class="modal-header bg-black">
						<button type="button" class="ClosePopup close">
							×</button>
						<h4 class="modal-title">
							<asp:Label ID="lblPopupTitle" runat="server">Country</asp:Label></h4>
					</div>
					<div class="modal-body divloadercountry checkvalidCountryDetail">
						<div class="form-horizontal">
							<div class="form-group">
								<label class="col-lg-3 col-md-3 col-sm-4 control-label">
									Name<span class="text-danger">*</span>
								</label>
								<div class="col-lg-8 col-md-8 col-sm-7">
									<asp:TextBox ID="txtCountryName" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=Country Name"
										runat="server" MaxLength="30" placeholder="Enter Country"></asp:TextBox>
								</div>
							</div>
							<div class="form-group">
								<label class="col-lg-3 col-md-3 col-sm-4 control-label">
									Description
								</label>
								<div class="col-lg-8 col-md-8 col-sm-7 htmlareah100">
									<asp:TextBox ID="txtDescription" CssClass="txtDescriptionCountry form-control" runat="server"
										TextMode="MultiLine" placeholder="Enter Description"></asp:TextBox>
								</div>
							</div>
						</div>
					</div>
					<div class="modal-footer">
						<button type="button" class="ClosePopup btn btn-raised btn-default">
							Cancel</button>
						<asp:Button ID="btnSaveCountry" OnClick="btnSaveCountry_OnClick" runat="server" CssClass="btnSaveCountry btn btn-raised btn-black"
							Text="Save" />
						<asp:Button ID="btnSaveAndNewCountry" OnClick="btnSaveAndNewCountry_OnClick" runat="server"
							CssClass="btnSaveAndNewCountry btn btn-raised btn-black" Text="Save & New" />
					</div>
				</div>
			</div>
		</asp:Panel>
	</ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
	Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckpostbackCountry);
	function CheckpostbackCountry() {
		AdjustTextaria("txtDescriptionCountry", "txtDescriptionCountry");
		$(".txtDescriptionCountry").keyup(function () {
			AdjustTextaria("txtDescriptionCountry", "txtDescriptionCountry");
		});

		$(".btnSaveCountry").click(function () {
			if (CheckValidation("checkvalidCountryDetail")) {
				addLoader('btnSaveCountry');
				return true;
			}
			else {
				return false;
			}
		});

		$(".btnSaveAndNewCountry").click(function () {
			if (CheckValidation("checkvalidCountryDetail")) {
				addLoader('btnSaveAndNewCountry');
				return true;
			}
			else {
				return false;
			}
		});
	}

</script>

