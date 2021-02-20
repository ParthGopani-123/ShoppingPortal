<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CCManageArea.ascx.cs"
	Inherits="CCManageArea" %>
<asp:UpdatePanel ID="UpdatePanel2" runat="server">
	<ContentTemplate>
		<asp:Label ID="lblAreaId" runat="server" Visible="false" Text=""></asp:Label>
		<asp:Panel ID="pnlArea" DefaultButton="btnSaveArea" runat="server">
			<div class="modal-dialog">
				<div class="modal-content darkmodel">
					<div class="modal-header bg-black">
						<button type="button" class="ClosePopup close">
							×</button>
						<h4 class="modal-title">
							<asp:Label ID="lblPopupTitle" runat="server">Area</asp:Label></h4>
					</div>
					<div class="modal-body divloaderArea div-check-validation-Area">
						<div class="form-horizontal">
							<div class="form-group">
								<label class="col-lg-3 col-md-3 col-sm-4 control-label">
									Country<span class="text-danger">*</span>
								</label>
								<div class="col-lg-8 col-md-8 col-sm-7">
									<div class="input-group">
										<asp:DropDownList ID="ddlCountry" AutoPostBack="true" OnSelectedIndexChanged="ddlCountry_OnSelectedIndexChanged"
											onchange="addRegionLoader('divloaderArea')" CssClass="form-control" ZValidation="e=change|v=IsSelect|m=Country"
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
										<asp:DropDownList ID="ddlState" AutoPostBack="true" OnSelectedIndexChanged="ddlState_OnSelectedIndexChanged"
											onchange="addRegionLoader('divloaderArea')" CssClass="form-control" ZValidation="e=change|v=IsSelect|m=State" runat="server">
										</asp:DropDownList>
										<asp:LinkButton ID="lnkState" OnClick="lnkState_OnClick" CssClass="input-group-addon btnspinner tooltips"
											ToolTip="Refresh" data-toggle="tooltip" runat="server"><i class="fa fa-refresh"></i></asp:LinkButton>
									</div>
								</div>
							</div>
							<div class="form-group">
								<label class="col-lg-3 col-md-3 col-sm-4 control-label">
									City<span class="text-danger">*</span>
								</label>
								<div class="col-lg-8 col-md-8 col-sm-7">
									<div class="input-group">
										<asp:DropDownList ID="ddlCity" CssClass="form-control" ZValidation="e=change|v=IsSelect|m=City" runat="server">
										</asp:DropDownList>
										<asp:LinkButton ID="lnkCity" OnClick="lnkCity_OnClick" CssClass="input-group-addon btnspinner tooltips"
											ToolTip="Refresh" data-toggle="tooltip" runat="server"><i class="fa fa-refresh"></i></asp:LinkButton>
									</div>
								</div>
							</div>
							<div class="form-group">
								<label class="col-lg-3 col-md-3 col-sm-4 control-label">
									Name<span class="text-danger">*</span>
								</label>
								<div class="col-lg-8 col-md-8 col-sm-7">
									<asp:TextBox ID="txtAreaName" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=Area Name"
										runat="server" MaxLength="30" placeholder="Enter Area"></asp:TextBox>
								</div>
							</div>
							<div class="form-group">
								<label class="col-lg-3 col-md-3 col-sm-4 control-label">
									Pincode
								</label>
								<div class="col-lg-8 col-md-8 col-sm-7">
									<asp:TextBox ID="txtPincode" CssClass="form-control intnumber" ZValidation="e=blur|v=IsNullPincode|m=Pincode"
										runat="server" MaxLength="6" placeholder="Enter Pincode"></asp:TextBox>
								</div>
							</div>
						</div>
					</div>
					<div class="modal-footer">
						<button type="button" class="ClosePopup btn btn-raised btn-default">
							Cancel</button>
						<asp:Button ID="btnSaveArea" OnClick="btnSaveArea_OnClick" runat="server"
							CssClass="btnSaveArea btn btn-raised btn-black" Text="Save" />
						<asp:Button ID="btnSaveAndNewArea" OnClick="btnSaveAndNewArea_OnClick"
							runat="server" CssClass="btnSaveAndNewArea btn btn-raised btn-black"
							Text="Save & New" />
					</div>
				</div>
			</div>
		</asp:Panel>
	</ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
	Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckpostbackArea);
	function CheckpostbackArea() {

		$(".btnSaveArea").click(function () {
			if (CheckValidation("div-check-validation-Area")) {
				addLoader('btnSaveArea');
				return true;
			}
			else {
				return false;
			}
		});

		$(".btnSaveAndNewArea").click(function () {
			if (CheckValidation("div-check-validation-Area")) {
				addLoader('btnSaveAndNewArea');
				return true;
			}
			else {
				return false;
			}
		});
	}
</script>

