<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
	CodeFile="MyProfile.aspx.cs" Inherits="MyProfile" EnableEventValidation="false"
	Title="My Profile" %>

<%@ Register Src="~/CCConfirmPopup.ascx" TagName="ConfirmPopup" TagPrefix="CPP" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
	<style>
		.overlay-container {
			position: relative;
			overflow: hidden;
		}

			.overlay-container .overlay-bg {
				position: absolute;
				top: 0;
				left: 0;
				right: 0;
				bottom: 0;
				margin: auto;
				z-index: -1;
			}

		.div-profile {
			display: block;
			border-radius: 50%;
			padding: 3px;
			background-color: #fff;
		}

		.img-bg {
			width: 100%;
			height: 255px;
		}
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">
	<asp:UpdatePanel ID="up1" runat="server">
		<Triggers>
			<asp:PostBackTrigger ControlID="lnkSaveProfilePic" />
		</Triggers>
		<ContentTemplate>
			<asp:Label ID="lbltabHeight1" runat="server" CssClass="lbltabHeight1 hidden" Text="126"></asp:Label>
			<div class="row">
				<div class="page-header clearfix">
					<ol class="breadcrumb mb-0">
						<li><a href="Home.aspx">Home</a></li>
						<li class="active"><a>Profile</a></li>
					</ol>
				</div>
				<div class="page-contan tabHeight1">
					<div class="col-lg-12">
						<div class="overlay-container divprofileloader text-white">
							<img src="<%= CU.StaticFilePath %>SystemImages/Backgrounds-Profile.jpg" alt="" class="overlay-bg img-bg img-responsive">
							<div style="padding: 120px 30px 30px 30px" class="overlay-content clearfix">
								<div class="pull-left media">
									<div class="media-left">
										<div class="div-profile">
											<asp:Image runat="server" ID="imgUserProfile" alt="" class="img-circle media-object hw100px imgUserProfile" />
										</div>
										<asp:FileUpload ID="fuProfile" CssClass="fuProfile hidden form-control" runat="server" />
										<asp:Label ID="lblOldProfileImage" runat="server" CssClass="lblOldProfileImage hidden"
											Text=""></asp:Label>
										<asp:LinkButton ID="lnkRemoveProfile" CssClass="btnchangeprofilepic btnremoveprofile"
											OnClientClick="addRegionLoader('divprofileloader')" OnClick="lnkRemoveProfile_OnClick"
											runat="server"><i class="fa fa-trash-o"></i></asp:LinkButton>
										<a class="btnchangePic btnchangeprofilepic btnchangeprofile"><i class="fa fa-pencil"></i></a>
										<asp:LinkButton ID="lnkSaveProfilePic" CssClass="hidden" OnClick="lnkSaveProfilePic_OnClick"
											runat="server"></asp:LinkButton>
									</div>
									<div style="width: auto" class="media-body media-middle">
										<h2 class="media-heading">
											<asp:Label ID="lblUserName" runat="server" CssClass="WordWrap" Text=""></asp:Label></h2>
										<div class="fs-20">
											<asp:Label ID="lblMobileNo" runat="server" CssClass="WordWrap" Text=""></asp:Label>
										</div>
									</div>
								</div>
							</div>
						</div>
					</div>
					<div class="col-sm-6 mt-5">
						<asp:Panel ID="pnlProfile" runat="server" DefaultButton="btnChangeProfile">
							<div class="widget mb-0">
								<div class="widget-heading">
									<h3 class="widget-title">My Profile</h3>
								</div>
								<div class="widget-body pb-0 div-validation-Change-Profile">
									<div class="form-horizontal">
										<div class="form-group">
											<label class="col-lg-4 col-md-4 col-sm-5 control-label">
												Name&nbsp;<span class="text-danger">*</span>
											</label>
											<div class="col-lg-8 col-md-8 col-sm-7">
												<asp:TextBox ID="txtName" CssClass="form-control" runat="server" MaxLength="100"
													ZValidation="e=blur|v=IsRequired|m=Name" placeholder="Enter Name"></asp:TextBox>
											</div>
										</div>
										<div class="form-group">
											<label class="col-lg-4 col-md-4 col-sm-5 control-label">
												Email
											</label>
											<div class="col-lg-8 col-md-8 col-sm-7">
												<asp:TextBox ID="txtEmail" CssClass="form-control" runat="server" MaxLength="100"
													ZValidation="e=blur|v=IsNullEmail|m=Email" placeholder="Enter Email"></asp:TextBox>
											</div>
										</div>
									</div>
								</div>
								<div class="panel-footer text-right">
									<asp:Button ID="btnCancel" OnClick="btnCancel_OnClick" runat="server" CssClass="btn btn-raised btn-default clickloader"
										Text="Cancel" />
									<asp:Button ID="btnChangeProfile" OnClick="btnChangeProfile_OnClick" runat="server"
										CssClass="btnChangeProfile btn btn-raised btn-black" Text="Save" />
								</div>
							</div>
						</asp:Panel>
					</div>
					<div class="col-sm-6 mt-5">
						<asp:Panel ID="Panel1" runat="server" DefaultButton="btnChangePassword">
							<div class="widget mb-0">
								<div class="widget-heading">
									<h3 class="widget-title">Change Password</h3>
								</div>
								<div class="widget-body div-validation-Change-Password">
									<div class="form-horizontal">
										<div class="form-group">
											<label class="col-lg-3 col-md-3 col-sm-4 control-label">
												Old&nbsp;<span class="text-danger">*</span>
											</label>
											<div class="col-lg-8 col-md-8 col-sm-7">
												<asp:TextBox ID="txtOldPasswordMaster" TextMode="Password" CssClass="txtOldPasswordMaster form-control"
													runat="server" MaxLength="50" ZValidation="e=blur|v=IsRequired|m=Old Password"
													placeholder="Enter Old Password"></asp:TextBox>
											</div>
										</div>
										<div class="form-group">
											<label class="col-lg-3 col-md-3 col-sm-4 control-label">
												New&nbsp;<span class="text-danger">*</span>
											</label>
											<div class="col-lg-8 col-md-8 col-sm-7">
												<asp:TextBox ID="txtNewPasswordMaster" TextMode="Password" CssClass="txtNewPasswordMaster form-control"
													runat="server" MaxLength="50" ZValidation="e=blur|v=IsRequired|m=New Password"
													placeholder="Enter New Password"></asp:TextBox>
											</div>
										</div>
										<div class="form-group mb-39">
											<label class="col-lg-3 col-md-3 col-sm-4 control-label">
												Confirm&nbsp;<span class="text-danger">*</span>
											</label>
											<div class="col-lg-8 col-md-8 col-sm-7">
												<asp:TextBox ID="txtConfirmPasswordMaster" TextMode="Password" CssClass="txtConfirmPasswordMaster form-control"
													runat="server" MaxLength="50" ZValidation="e=blur|v=IsRequired|m=Confirm Password"
													placeholder="Enter Confirm Password"></asp:TextBox>
											</div>
										</div>
									</div>
								</div>
								<div class="panel-footer text-right">
									<asp:Button ID="btnCancelPassword" OnClick="btnCancel_OnClick" runat="server" CssClass="btn btn-raised btn-default clickloader"
										Text="Cancel" />
									<asp:Button ID="btnChangePassword" OnClick="btnChangePassword_OnClick" runat="server"
										CssClass="btnChangePassword btn btn-raised btn-black" Text="Change" />
								</div>
							</div>
						</asp:Panel>
					</div>

                    
				</div>
			</div>
		</ContentTemplate>
	</asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="popup" runat="Server">
	<asp:UpdatePanel ID="UpdatePanel1" runat="server">
		<ContentTemplate>
			<cc1:ModalPopupExtender ID="popupConfirmRemoveProfile" runat="server" DropShadow="false"
				PopupControlID="pnlConfirmRemoveProfile" BehaviorID="PopupBehaviorID22" TargetControlID="lnkFackConfirmRemoveProfile"
				BackgroundCssClass="modalBackground">
			</cc1:ModalPopupExtender>
			<asp:LinkButton ID="lnkFackConfirmRemoveProfile" runat="server"></asp:LinkButton>
			<asp:Panel ID="pnlConfirmRemoveProfile" CssClass="modal-content zoomIn modal-confirmation col-xs-12 col-sm-12 col-md-12 p0"
				Style="display: none" runat="server">
				<CPP:ConfirmPopup ID="ConfirmPopupRemoveProfile" runat="server" />
			</asp:Panel>
		</ContentTemplate>
	</asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="script" runat="Server">

	<script type="text/javascript">
		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckPostback);
		jQuery(function () {
			CheckPostback();
		});
		function CheckPostback() {

			$(".btnchangePic").click(function () {
				$(".fuProfile").trigger('click');
			});
			$(".fuProfile").change(function (event) {
				var ValidFilesTypes = ["png", "jpg", "jpeg"];
				var File = document.getElementById("<%=fuProfile.ClientID%>");
				var Path = File.value;
				var ExtFile = Path.substring(Path.lastIndexOf(".") + 1, Path.length).toLowerCase();
				var isValidFile = false;

				for (var i = 0; i < ValidFilesTypes.length; i++) {
					if (ExtFile == ValidFilesTypes[i]) {
						isValidFile = true;;
						break;
					}
				}
               
				if (Path != "" && !isValidFile) {
					SetErrorMessage("Please Select Valid Image.(jpg,jpeg,png)");
				}
				else {
					if (event.target.files[0] == "" || typeof event.target.files[0] === "undefined") {
						$(".imgUserProfile").fadeIn("fast").attr('src', $(".lblOldProfileImage").text().replace("~/", ""));
					}
					else {
						addRegionLoader('divprofileloader');
						$(".imgUserProfile").fadeIn("fast").attr('src', URL.createObjectURL(event.target.files[0]));
                        <%= Page.ClientScript.GetPostBackEventReference(lnkSaveProfilePic, String.Empty) %>;
					}
				}
			});

			$(".btnChangePassword").click(function () {
				if (CheckValidation("div-validation-Change-Password") && IsMatch(".txtNewPasswordMaster", "", "Confirm Password", ".txtConfirmPasswordMaster")) {
					addLoader('btnChangePassword');
					return true;
				}
				else {
					return false;
				}
			});

			$(".btnChangeProfile").click(function () {
				if (CheckValidation("div-validation-Change-Profile")) {
					addLoader('btnChangeProfile');
					return true;
				}
				else {
					return false;
				}
			});
		}
    </script>

</asp:Content>
