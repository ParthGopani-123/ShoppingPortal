using System;

using BOL;

public partial class ForgotPassword : CompressorPage
{
    int OTPResendInterval = 60;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["OTPUsersId"] != null)
            {
                txtOtp.Focus();
                SetActiveIndex(1, false);
            }
            else
            {
                txtUserName.Focus();
                SetActiveIndex(0, false);
                lblResendOTPAfter.Text = "0";
            }
        }

        try { lblResendOTPAfter.Text = (OTPResendInterval - (IndianDateTime.Now - Convert.ToDateTime(Session["OTPSentTime"])).TotalSeconds).ToString(); }
        catch { lblResendOTPAfter.Text = "0"; }

        SetAttribute(txtUserName, true);
        SetAttribute(txtOtp, true);
        SetAttribute(txtNewPassword, true);
        SetAttribute(txtConfirmPassword, true);
        lblChangePasswordMSG.Text = lblErrorUserName.Text = lblOTPMsg.Text = string.Empty;
    }


    private void SentOTP()
    {
        if (Session["OTPUsersId"] == null)
        {
            SetActiveIndex(0, true);
            txtUserName.Focus();
        }
        else
        {
            int? UsersId = Convert.ToInt32(Session["OTPUsersId"]);
            var objUser = new Users() { UsersId = UsersId }.SelectList<Users>()[0];

            string SMSText = CU.GetNameValue(eNameValue.ForgotPasswordSMSText);
            SMSText = SMSText.Replace("#OTP#", GetOTP());

            if (CU.SendSMS(UsersId.Value, SMSText, objUser.MobileNo, false, 0, 0) != (int)eSMSStatus.Sent)
            {
                lblOTPMsg.Text = "Message Send Fail Please Try Again.";
                SetAttribute(txtOtp, false);
                txtOtp.Focus();
            }
            else
            {
                Session["OTPSentTime"] = IndianDateTime.Now;
                lblResendOTPAfter.Text = OTPResendInterval.ToString();
            }
        }
    }

    private string GetOTP()
    {
        string OTP = string.Empty;
        var objLogin = new Logins() { UsersId = GetUsersId() }.SelectList<Logins>()[0];
        if (!string.IsNullOrEmpty(objLogin.OTP) && objLogin.OTPGenerateTime.HasValue && objLogin.OTPGenerateTime.Value.AddMinutes(Convert.ToInt32(CU.GetNameValue(eNameValue.OTPValidMinute))) > IndianDateTime.Now)
        {
            OTP = objLogin.OTP;
            objLogin.OTPGenerateTime = IndianDateTime.Now;
            objLogin.UpdateAsync();
        }
        else
        {
            OTP = new Random().Next(1000, 100000000).ToString().Substring(0, 6);
            objLogin.OTP = OTP;
            objLogin.OTPGenerateTime = IndianDateTime.Now;
            objLogin.UpdateAsync();
        }
        return OTP;
    }

    private int? GetUsersId()
    {
        if (Session["OTPUsersId"] == null)
        {
            var lstLogin = new Logins() { Username = txtUserName.Text.Trim() }.SelectList<Logins>();
            if (lstLogin.Count != 0)
            {
                var UserCount = new Users()
                {
                    UsersId = lstLogin[0].UsersId,
                    eStatus = (int)eStatus.Active,
                }.SelectCount();

                if (UserCount > 0)
                    return lstLogin[0].UsersId;
            }
            else
            {
                var lstUser = new Users()
                {
                    MobileNo = txtUserName.Text.Trim(),
                    eStatus = (int)eStatus.Active,
                }.SelectList<Users>();

                if (lstUser.Count != 0)
                {
                    return lstUser[0].UsersId;
                }
            }
        }
        else
        {
            return Convert.ToInt32(Session["OTPUsersId"]);
        }

        txtUserName.Focus();
        SetActiveIndex(0, true);
        return null;
    }


    protected void btnSend_Click(object sender, EventArgs e)
    {
        int? UsersId = GetUsersId();
        if (!UsersId.HasValue)
        {
            lblErrorUserName.Text = "Sorry, We don't recognize " + txtUserName.Text + ".";
            SetAttribute(txtUserName, false);
            txtUserName.Focus();
        }
        else
        {
            Session["OTPUsersId"] = UsersId;
            SentOTP();
            txtOtp.Focus();
            SetActiveIndex(1, true);
        }
    }

    protected void lbtnResendOtp_Click(object sender, EventArgs e)
    {
        SentOTP();
    }

    protected void btnOtpSubmit_Click(object sender, EventArgs e)
    {
        var objLogin = new Logins() { UsersId = GetUsersId() }.SelectList<Logins>()[0];
        if (string.IsNullOrEmpty(txtOtp.Text.Trim()) || txtOtp.Text.Trim() != objLogin.OTP)
        {
            lblOTPMsg.Text = "Please Enter Valid OTP.";
            SetAttribute(txtOtp, false);
            txtOtp.Focus();
        }
        else
        {
            txtNewPassword.Focus();
            //bool IsDefault = false;
            //ProfilrPic.Src = CU.GetPersonPhoto(true, objLogin.PersonId.Value, ref IsDefault);
            SetActiveIndex(2, true);
        }
    }

    private bool IsValidatePassword()
    {
        if (string.IsNullOrEmpty(txtNewPassword.Text.Trim()))
        {
            lblChangePasswordMSG.Text = "Please Enter Valid New Password";
            SetAttribute(txtNewPassword, true);
            return false;
        }

        if (string.IsNullOrEmpty(txtConfirmPassword.Text.Trim()))
        {
            SetAttribute(txtConfirmPassword, true);
            lblChangePasswordMSG.Text = "Please Enter Valid Confirm Password";
            return false;
        }

        if (txtNewPassword.Text.Trim() != txtConfirmPassword.Text.Trim())
        {
            SetAttribute(txtConfirmPassword, true);
            lblChangePasswordMSG.Text = "Confirm Password Not Match.";
            return false;
        }
        return true;
    }

    protected void btnResetPassword_Click(object sender, EventArgs e)
    {
        if (!IsValidatePassword())
        {
            txtNewPassword.Focus();
            return;
        }

        LoginUtilities.ChangePassword(GetUsersId().Value, txtNewPassword.Text);
        Response.Redirect("Login.aspx");
    }


    protected void lnkBackToLogin_Click(object sender, EventArgs e)
    {
        Session["OTPUsersId"] = null;
        Response.Redirect("Login.aspx");
    }


    private void SetActiveIndex(int Index, bool SetSlider)
    {
        MultiView.ActiveViewIndex = Index;
        lbtnResendOtp.Visible = false;
        if (Index == 0)
        { }
        else if (Index == 1)
        {
            lbtnResendOtp.Visible = true;
        }
        else if (Index == 2)
        { }

        txtAllowSlide.Text = SetSlider ? "1" : "0";
    }


    private void SetAttribute(System.Web.UI.WebControls.TextBox txtControl, bool IsValid)
    {
        if (IsValid)
            txtControl.Attributes.Add("style", "border-color:transparent transparent #e6e6e6");
        else
            txtControl.Attributes.Add("style", "border-color:transparent transparent #ff0b0b");
    }
}
