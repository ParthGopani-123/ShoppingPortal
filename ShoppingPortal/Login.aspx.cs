using System;
using BOL;
using Utility;
using System.Web.UI.WebControls;

public partial class Login : CompressorPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			LoginUtilities.CheckCookies();
			if (Session[LoginUtilities.SessionId] != null)
				LoginUtilities.Redirect(eLoginType.Cookies, Convert.ToInt32(Session[LoginUtilities.SessionId]));

			CheckCookies();

			if (new Users() { DesignationId = CU.GetDesignationId(eDesignation.SystemAdmin), eStatus = (int)eStatus.Active }.SelectCount() == 0)
				CU.CreateDefaultAdmin();

			if (Request.QueryString.Count > 0 && Request.QueryString["CheckUsersId".Encrypt()] != null)
			{
				Session["CheckUsersId"] = Request.QueryString["CheckUsersId".Encrypt()].Decrypt();
				lnkDefferentAccount.Text = "Enter Password To Confirm Your Presence.";
			}
			else if (Request.Cookies["CheckUsersId".Encrypt()] != null)
				Session["CheckUsersId"] = Request.Cookies["CheckUsersId".Encrypt()].Value.Decrypt();

			SetControl(false);
		}

		lblErrorUserName.Text = lblErrorPassword.Text = string.Empty;
		txtUsername.Attributes.Add("style", "border-color: transparent transparent #e6e6e6");
		txtPassword.Attributes.Add("style", "border-color: transparent transparent #e6e6e6");
	}


	private void CheckCookies()
	{
		if (Request.Cookies["RememberCheckBox"] != null)
		{
			if (Request.Cookies["RememberCheckBox"].Value == "Checked")
				chkRememberme.Checked = true;
		}
	}


	protected void btnCheckUserName_OnClick(object sender, EventArgs e)
	{
		if (txtUsername.zIsNullOrEmpty())
		{
			lblErrorUserName.Text = "Please Enter Username.";
			txtUsername.Attributes.Add("style", "border-color:transparent transparent #ff0b0b");
			txtUsername.Focus();
			return;
		}

		int? UsersId = null;
		if (LoginUtilities.IsValidUserName(ref UsersId, txtUsername.Text))
		{
			Session["CheckUsersId"] = UsersId.ToString();
			SetControl(true);
		}
		else
		{
			lblErrorUserName.Text = "Sorry, We doesn't recognize " + txtUsername.Text + ".";
			txtUsername.Attributes.Add("style", "border-color:transparent transparent #ff0b0b");
			txtUsername.Focus();
		}
	}

	protected void btnCheckPassword_OnClick(object sender, EventArgs e)
	{
		if (txtPassword.zIsNullOrEmpty())
		{
			lblErrorPassword.Text = "Please Enter Password.";
			txtPassword.Attributes.Add("style", "border-color: transparent transparent #ff0b0b");
			txtPassword.Focus();
			return;
		}

		int? CheckUsersId = GetCheckUsersId();
		if (!CheckUsersId.HasValue)
		{
			if (Request.QueryString.Count > 0 && Request.QueryString["CheckUsersId".Encrypt()] != null)
				Session["CheckUsersId"] = Request.QueryString["CheckUsersId".Encrypt()].Decrypt();
			SetControl(true);
			CheckUsersId = GetCheckUsersId();
			if (!CheckUsersId.HasValue)
			{
				SetControl(true);
				return;
			}
		}

		if (!LoginUtilities.IsValidPassword(GetCheckUsersId().Value, txtPassword.Text))
		{
			lblErrorPassword.Text = "Wrong password. Try again.";
			txtPassword.Attributes.Add("style", "border-color: transparent transparent #ff0b0b");
			txtPassword.Focus();
		}
		else
		{
			LoginUtilities.Login(GetCheckUsersId().Value, chkRememberme.Checked);
		}
	}

	protected void lnkDefferentAccount_OnClick(object sender, EventArgs e)
	{
		txtUsername.Text = string.Empty;
		Session["CheckUsersId"] = null;
		SetControl(true);
	}


	private int? GetCheckUsersId()
	{
		if (Session["CheckUsersId"].zIsInteger(false) && Session["CheckUsersId"].zToInt() != 0)
			return Session["CheckUsersId"].zToInt();

		return null;
	}

	private void SetControl(bool SetSlider)
	{
		int? UsersId = GetCheckUsersId();
		if (UsersId.HasValue)
		{
			var drUser = new Users() { UsersId = UsersId }.Select(new Users() { Name = string.Empty }).Rows[0];
			imgUserImage.ImageUrl = CU.GetFilePath(true, ePhotoSize.Original, eFolder.UserProfile, UsersId.ToString(), true);
			lblUserName.Text = drUser[CS.Name].ToString();
			MultiView.ActiveViewIndex = 1;
			txtPassword.Focus();
		}
		else
		{
			MultiView.ActiveViewIndex = 0;
			txtUsername.Focus();
		}

		txtAllowSlide.Text = SetSlider ? "1" : "0";
	}
}
