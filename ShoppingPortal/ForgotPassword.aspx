<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ForgotPassword.aspx.cs" Inherits="ForgotPassword" %>

<html lang="en" style="height: 100%">
<head>
	<meta charset="utf-8" />
	<meta http-equiv="X-UA-Compatible" content="IE=edge" />
	<meta name="viewport" content="width=device-width, initial-scale=1" />
	<link rel='shortcut icon' type='image/x-icon' href="<%= CU.StaticFilePath %>SystemImages/OCTFISIcon.ico" />
	<link rel='apple-touch-icon' href="<%= CU.StaticFilePath %>SystemImages/OCTFISIcon.ico" />
	<title>Forgot Password - Shopping Portal</title>
	<link rel="stylesheet" type="text/css" href="<%= CU.StaticFilePath %>plugins/bootstrap/dist/css/bootstrap.css" />
	<link rel="stylesheet" type="text/css" href="<%= CU.StaticFilePath %>plugins/themify-icons/themify-icons.css" />
	<link rel="stylesheet" type="text/css" href="<%= CU.StaticFilePath %>plugins/build/css/first-layout.css" />
	<link href="<%= CU.StaticFilePath %>plugins/OwnPlugin/css/loader/zloder.css" rel="stylesheet"
		type="text/css" />
	<link href="<%= CU.StaticFilePath %>plugins/js/jquery-ui.css" rel="stylesheet" type="text/css" />
	<style>
		*::-webkit-input-placeholder {
			/* WebKit, Blink, Edge */
			color: #b5b5b5 !important;
		}

		*:-moz-placeholder {
			/* Mozilla Firefox 4 to 18 */
			color: #b5b5b5 !important;
			opacity: 1;
		}

		*::-moz-placeholder {
			/* Mozilla Firefox 19+ */
			color: #b5b5b5 !important;
			opacity: 1;
		}

		*:-ms-input-placeholder {
			/* Internet Explorer 10-11 */
			color: #b5b5b5 !important;
		}

		.errormessage {
			color: Red;
		}

		.Userimage {
			height: 100px;
			width: 100px;
			border-radius: 50%;
			border: 2px solid #e6e6e6;
			padding: 2px;
		}

		.body-bg-full .logo {
			margin-bottom: 15px;
		}

		.imglogo {
			height: 100px;
			width: 100px;
		}

		.pnlSlide {
			display: none;
		}

		.minheight {
			min-height: 325px;
		}
	</style>
</head>
<body style="background-image: url(<%= CU.StaticFilePath %>SystemImages/Backgrounds.jpg)"
	class="body-bg-full v2">
	<div class="container page-container">
		<div class="page-content">
			<div class="v2 divloader pb-1 minheight">
				<form id="form2" runat="server" class="form-horizontal">
					<cc1:ToolkitScriptManager ID="scriptManager1" runat="server" ScriptMode="Release"
						CombineScripts="true" EnablePageMethods="true">
					</cc1:ToolkitScriptManager>
					<asp:UpdatePanel ID="UpdatePanel1" runat="server">
						<ContentTemplate>
							<asp:TextBox ID="txtAllowSlide" Text="0" CssClass="txtAllowSlide hide" runat="server"></asp:TextBox>
							<asp:Label ID="lblResendOTPAfter" Text="0" CssClass="lblResendOTPAfter hide" runat="server"></asp:Label>
							<div class="logo">
								<img src="<%= CU.StaticFilePath %>SystemImages/logo-gif.gif" alt="" class="imglogo" />
							</div>
							<asp:MultiView ID="MultiView" runat="server" ActiveViewIndex="0">
								<asp:View ID="Step1" runat="server">
									<asp:Panel ID="pnlStep1" runat="server" DefaultButton="btnSend">
										<div class="pnlSlide" direction="left">
											<h4 class="fs-16 fw-300 mt-0">Forgot Password</h4>
											<p class="text-muted">
												Enter Username or registerd Mobile Number associated with your account to reset
                                            your password
											</p>
											<asp:Label ID="lblErrorUserName" CssClass="errormessage lblErrorUserName" runat="server"></asp:Label>
											<div class="form-group">
												<div class="col-xs-12">
													<asp:TextBox ID="txtUserName" runat="server" autocomplete="off" MaxLength="50" placeholder="Enter User Name / Mobile No"
														CssClass="txtUserName txtFocus form-control"></asp:TextBox>
												</div>
											</div>
											<asp:Button ID="btnSend" OnClick="btnSend_Click" runat="server" CssClass="btnSend btn-lg btn btn-primary btn-rounded btn-block"
												Text="verify" />
										</div>
									</asp:Panel>
								</asp:View>
								<asp:View ID="Step2" runat="server">
									<asp:Panel ID="pnlStep2" runat="server" DefaultButton="btnOtpSubmit">
										<div class="pnlSlide" direction="right">
											<h4 class="fs-16 fw-300 mt-0">Enter your OTP</h4>
											<p class="text-muted">
												Enter the OTP sent to the registerd mobileno on your account
											</p>
											<asp:Label ID="lblOTPMsg" CssClass="errormessage lblOTPMsg" runat="server" Text=""></asp:Label>
											<div class="form-group">
												<div class="col-xs-12">
													<asp:TextBox ID="txtOtp" autocomplete="off" MaxLength="10" runat="server" placeholder="Enter OTP"
														CssClass="form-control txtFocus txtOtp"></asp:TextBox>
												</div>
											</div>
											<asp:Button ID="btnOtpSubmit" OnClick="btnOtpSubmit_Click" runat="server" CssClass="btnOtpSubmit btn-lg btn btn-primary btn-rounded btn-block"
												Text="Submit" />
										</div>
									</asp:Panel>
								</asp:View>
								<asp:View ID="Step3" runat="server">
									<asp:Panel ID="pnlStep3" runat="server" DefaultButton="btnResetPassword">
										<div class="pnlSlide" direction="right">
											<h4 class="fs-16 fw-300 mt-0">Change your password</h4>
											<asp:Label ID="lblChangePasswordMSG" CssClass="errormessage lblChangePasswordMSG"
												runat="server" Text=""></asp:Label>
											<div class="form-group">
												<div class="col-xs-12">
													<asp:TextBox ID="txtNewPassword" runat="server" MaxLength="50" TextMode="Password"
														placeholder="Enter New Password" CssClass="txtNewPassword txtFocus form-control"></asp:TextBox>
												</div>
											</div>
											<div class="form-group">
												<div class="col-xs-12">
													<asp:TextBox ID="txtConfirmPassword" runat="server" MaxLength="50" TextMode="Password"
														placeholder="Enter Comfirm Password" CssClass="txtConfirmPassword form-control"></asp:TextBox>
												</div>
											</div>
											<asp:Button ID="btnResetPassword" OnClick="btnResetPassword_Click" runat="server"
												CssClass="btnResetPassword btn-lg btn btn-primary btn-rounded btn-block" Text="Reset" />
										</div>
									</asp:Panel>
								</asp:View>
							</asp:MultiView>
							<div class="form-group pt-10">
								<div class="col-md-12">
									<asp:LinkButton ID="lbtnResendOtp" runat="server" OnClick="lbtnResendOtp_Click" CssClass="lbtnResendOtp inline-block pull-left">Resend OTP</asp:LinkButton>
									<asp:LinkButton ID="lnkBackToLogin" OnClick="lnkBackToLogin_Click" class="inline-block pull-right lnkBackToLogin"
										runat="server">Back To Login</asp:LinkButton>
								</div>
							</div>
						</ContentTemplate>
					</asp:UpdatePanel>
				</form>
			</div>
		</div>
	</div>

	<script src="<%= CU.StaticFilePath %>plugins/js/jquery.min.js" type="text/javascript"></script>

	<script src="<%= CU.StaticFilePath %>plugins/js/jquery-ui.js" type="text/javascript"></script>

	<%--<script src="<%= CU.StaticFilePath %>plugins/Festival/christmas.js" type="text/javascript"></script>--%>

	<script type="text/javascript">
		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Checkpostback);
		jQuery(function () {
			Checkpostback();
		});
		var interval = null;
		function Checkpostback() {

			var txtAllowSlide = $(".txtAllowSlide");
			if (parseInt(txtAllowSlide.val()) == 0) {
				$('.pnlSlide').show();
				$('.txtFocus').focus();
			}
			else {
				setTimeout(function () {
					$('.pnlSlide').show('slide', { direction: $('.pnlSlide').attr('direction') }, 200);
					$('.txtFocus').focus();
					txtAllowSlide.val("0");
				}, 100);
			}


			clearInterval(interval);
			StartResendCounter();
			$(".divloader").removeClass("csspinner traditional");

			$(".lbtnResendOtp").click(function () {
				if (parseInt($(".lblResendOTPAfter").text()) > 0) { return false; }
				else { AddLoader(); return true; }
			});

			$(".btnSend").click(function () {
				if ($(".txtUserName").val() == "") {
					validatecontrol(true, "lblErrorUserName", "txtUsername", "Please Enter Username.");
					$(".txtUsername").focus();
					return false;
				}
				else {
					validatecontrol(false, "lblErrorUserName", "txtUsername", "");
					AddLoader();
					return true;
				}
			});

			$(".btnOtpSubmit").click(function () {
				if ($(".txtOtp").val() == "") {
					validatecontrol(true, "lblOTPMsg", "txtOtp", "Please Enter OTP.");
					$(".txtOtp").focus();
					return false;
				}
				else {
					validatecontrol(false, "lblOTPMsg", "txtOtp", "");
					AddLoader();
					return true;
				}
			});

			$(".btnResetPassword").click(function () {
				if ($(".txtNewPassword").val() == "") {
					validatecontrol(true, "lblChangePasswordMSG", "txtNewPassword", "Please Enter Password.");
					$(".txtNewPassword").focus();
					return false;
				}
				else {
					validatecontrol(false, "lblChangePasswordMSG", "txtNewPassword", "");
				}

				if ($(".txtConfirmPassword").val() == "") {
					validatecontrol(true, "lblChangePasswordMSG", "txtConfirmPassword", "Please Enter Confirm Password.");
					$(".txtConfirmPassword").focus();
					return false;
				}
				else {
					validatecontrol(false, "lblChangePasswordMSG", "txtConfirmPassword", "");
				}

				if ($(".txtNewPassword").val() != $(".txtConfirmPassword").val()) {
					validatecontrol(true, "lblChangePasswordMSG", "txtConfirmPassword", "Confirm Password Not Match.");
					$(".txtConfirmPassword").focus();
					return false;
				}
				else {
					validatecontrol(false, "lblChangePasswordMSG", "txtConfirmPassword", "");
				}

				AddLoader();
				return true;
			});

			$(".lnkBackToLogin").click(function () {
				AddLoader();
			});
		}

		function StartResendCounter() {
			var counter = parseInt($(".lblResendOTPAfter").text());
			if (counter > 0) {
				$(".lbtnResendOtp").text("Resend (" + counter + ")");
				interval = setInterval(function () {
					$(".lbtnResendOtp").text("Wait (" + counter + ")");
					$(".lbtnResendOtp").css("pointer-events", "none");
					if (counter <= 0) {
						$(".lbtnResendOtp").css("pointer-events", "auto");
						$(".lbtnResendOtp").text("Resend");
						clearInterval(interval);
					}
					else {
						counter = counter - 1;
						$(".lblResendOTPAfter").text(counter);
					}
				}, 1000);
			}
			else {
				$(".lbtnResendOtp").css("pointer-events", "auto");
				$(".lbtnResendOtp").text("Resend");
			}
			return false;
		}

		function validatecontrol(isNotvalid, lblError, txtval, Message) {
			if (isNotvalid) {
				$("." + lblError).text(Message);
				$("." + txtval).css("border-color", "transparent transparent #ff0b0b");
				$("." + txtval).focus();
			}
			else {
				$("." + lblError).text(Message);
				$("." + txtval).css("border-color", "transparent transparent #e6e6e6");
				$("." + txtval).focus();
			}
		}

		function AddLoader() {
			$(".divloader").addClass("csspinner traditional");
		}
		//Error : Notify Null In Chrome-----------------------------------------------
		Sys.Browser.WebKit = {};
		if (navigator.userAgent.indexOf('WebKit/') > -1) {
			Sys.Browser.agent = Sys.Browser.WebKit;
			Sys.Browser.version = parseFloat(navigator.userAgent.match(/WebKit\/(\d+(\.\d+)?)/)[1]);
			Sys.Browser.name = 'WebKit';
		}

		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
		function EndRequestHandler(sender, args) {
			if (args.get_error() != undefined) {
				args.set_errorHandled(true);
			}
		}
	</script>

	<!-- Bootstrap JavaScript-->
</body>
</html>
