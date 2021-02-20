<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CCManageCity.ascx.cs"
	Inherits="CCManageCity" %>
<asp:UpdatePanel ID="UpdatePanel2" runat="server">
	<ContentTemplate>
		<asp:Label ID="lblCityId" runat="server" Visible="false" Text=""></asp:Label>
		<asp:Panel ID="pnlCity" DefaultButton="btnSaveCity" runat="server">
			<div class="modal-dialog">
				<div class="modal-content darkmodel">
					<div class="modal-header bg-black">
						<button type="button" class="ClosePopup close">
							×</button>
						<h4 class="modal-title">
							<asp:Label ID="lblPopupTitle" runat="server">City</asp:Label></h4>
					</div>
					<div class="modal-body divloaderCity div-check-validation-City">
						<div class="form-horizontal">
							<div class="form-group">
								<label class="col-lg-3 col-md-3 col-sm-4 control-label">
									Country<span class="text-danger">*</span>
								</label>
								<div class="col-lg-8 col-md-8 col-sm-7">
									<div class="input-group">
										<asp:DropDownList ID="ddlCountry" AutoPostBack="true" OnSelectedIndexChanged="ddlCountry_OnSelectedIndexChanged"
											onchange="addRegionLoader('divloaderCity')" CssClass="form-control" ZValidation="e=change|v=IsSelect|m=Country"
											runat="server">
										</asp:DropDownList>
										<asp:LinkButton ID="lnkCountry" OnClick="lnkCountry_OnClick" CssClass="input-group-addon btnspinner tooltips"
											ToolTip="Refresh" data-toggle="tooltip" runat="server"><i class="fa fa-refresh"></i></asp:LinkButton>
									</div>
								</div>
							</div>
							<div class="form-group">
								<label class="col-lg-3 col-md-3 col-sm-4 control-label">
									State<span class="text-danger">*</span>
								</label>
								<div class="col-lg-8 col-md-8 col-sm-7">
									<div class="input-group">
										<asp:DropDownList ID="ddlState" CssClass="form-control" ZValidation="e=change|v=IsSelect|m=State" runat="server">
										</asp:DropDownList>
										<asp:LinkButton ID="lnkState" OnClick="lnkState_OnClick" CssClass="input-group-addon btnspinner tooltips"
											ToolTip="Refresh" data-toggle="tooltip" runat="server"><i class="fa fa-refresh"></i></asp:LinkButton>
									</div>
								</div>
							</div>
							<div class="form-group">
								<label class="col-lg-3 col-md-3 col-sm-4 control-label">
									Name<span class="text-danger">*</span>
								</label>
								<div class="col-lg-8 col-md-8 col-sm-7">
									<asp:TextBox ID="txtCityName" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=City Name"
										runat="server" MaxLength="30" placeholder="Enter City"></asp:TextBox>
								</div>
							</div>
						</div>
					</div>
					<div class="modal-footer">
						<button type="button" class="ClosePopup btn btn-raised btn-default">
							Cancel</button>
						<asp:Button ID="btnSaveCity" OnClick="btnSaveCity_OnClick" runat="server"
							CssClass="btnSaveCity btn btn-raised btn-black" Text="Save" />
						<asp:Button ID="btnSaveAndNewCity" OnClick="btnSaveAndNewCity_OnClick"
							runat="server" CssClass="btnSaveAndNewCity btn btn-raised btn-black"
							Text="Save & New" />
					</div>
				</div>
			</div>
		</asp:Panel>
	</ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
	Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckpostbackCity);
	function CheckpostbackCity() {
		
		$(".btnSaveCity").click(function () {
			if (CheckValidation("div-check-validation-City")) {
				addLoader('btnSaveCity');
				return true;
			}
			else {
				return false;
			}
		});

		$(".btnSaveAndNewCity").click(function () {
			if (CheckValidation("div-check-validation-City")) {
				addLoader('btnSaveAndNewCity');
				return true;
			}
			else {
				return false;
			}
		});
	}
</script>

