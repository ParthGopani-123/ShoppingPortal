using System;
using BOL;

public partial class MyProfile : CompressorPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		LoginUtilities.CheckSession();
		if (!IsPostBack)
		{
			LoadProfileDetail();
		}

		ConfirmPopupRemoveProfile.btnConfirm_OnClick += new EventHandler(btnConfirmRemoveProfile_OnClick);
	}


	protected void LoadProfileDetail()
	{
		var objUser = new Users() { UsersId = CU.GetUsersId() }.SelectList<Users>()[0];
		txtName.Text = lblUserName.Text = objUser.Name;
		txtEmail.Text = objUser.Email;


		lblMobileNo.Text = "Mo." + objUser.MobileNo;

		//lblOldProfileImage.Text = imgUserProfile.ImageUrl = CU.GetFilePath(true, ePhotoSize.W240xH200, eFolder.UserProfile, objUser.UsersId.ToString(), true);
		//lnkRemoveProfile.Visible = CU.CheckFileExist(true, ePhotoSize.W240xH200, eFolder.UserProfile, objUser.UsersId.ToString());

        lblOldProfileImage.Text = imgUserProfile.ImageUrl = CU.GetFilePath(true, ePhotoSize.Original, eFolder.UserProfile, objUser.UsersId.ToString(), true);
        lnkRemoveProfile.Visible = CU.CheckFileExist(true, ePhotoSize.Original, eFolder.UserProfile, objUser.UsersId.ToString());
    }

	private bool IsProfileValidate()
	{
		if (txtName.zIsNullOrEmpty())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Name");
			txtName.Focus();
			return false;
		}

		return true;
	}

	protected void btnChangeProfile_OnClick(object sender, EventArgs e)
	{
		if (!IsProfileValidate())
			return;

		new Users()
		{
			UsersId = CU.GetUsersId(),
			Name = txtName.Text,
			Email = txtEmail.Text,
		}.UpdateAsync();

		CU.ZMessage(eMsgType.Success, string.Empty, "Profile Change Successfully.");
	}

	protected void lnkSaveProfilePic_OnClick(object sender, EventArgs e)
	{
		if (fuProfile.HasFile && !CU.IsImage(fuProfile.FileName))
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Valid Image File");
			fuProfile.Focus();
			return;
		}

		if (fuProfile.HasFile)
		{
			CU.UploadFile(fuProfile, new System.Collections.Generic.List<UploadPhoto>(), eFolder.UserProfile, CU.GetUsersId().ToString(), false);
			Response.Redirect(Request.RawUrl);
		}
	}


	protected void lnkRemoveProfile_OnClick(object sender, EventArgs e)
	{
		ConfirmPopupRemoveProfile.SetPopupType("Confirm", "Are you sure to Remove this Profile Picture?", false);
		popupConfirmRemoveProfile.Show();
	}

	protected void btnConfirmRemoveProfile_OnClick(object sender, EventArgs e)
	{
		CU.DeleteImage(eFolder.UserProfile, CU.GetUsersId().ToString());

		CU.ZMessage(eMsgType.Success, string.Empty, "Profile Removed Successfully.", true);
		Response.Redirect(Request.RawUrl);
	}


	private bool IsPasswordValidate()
	{
		if (txtOldPasswordMaster.zIsNullOrEmpty())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Old Password.");
			txtOldPasswordMaster.Focus();
			return false;
		}

		if (txtNewPasswordMaster.zIsNullOrEmpty())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter New Password.");
			txtNewPasswordMaster.Focus();
			return false;
		}

		if (txtConfirmPasswordMaster.zIsNullOrEmpty())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Confirm Password.");
			txtConfirmPasswordMaster.Focus();
			return false;
		}

		if (txtNewPasswordMaster.Text != txtConfirmPasswordMaster.Text)
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Confirm Password.");
			txtConfirmPasswordMaster.Focus();
			return false;
		}

		if (!LoginUtilities.IsValidPassword(CU.GetUsersId(), txtOldPasswordMaster.Text))
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Old Password.");
			txtOldPasswordMaster.Focus();
			return false;
		}

		return true;
	}

	protected void btnChangePassword_OnClick(object sender, EventArgs e)
	{
		if (!IsPasswordValidate())
			return;

		LoginUtilities.ChangePassword(CU.GetUsersId(), txtNewPasswordMaster.Text);
		CU.ZMessage(eMsgType.Success, string.Empty, "Password Change Successfully");
	}


	protected void btnCancel_OnClick(object sender, EventArgs e)
	{
		Response.Redirect(Request.RawUrl);
	}

    
}
