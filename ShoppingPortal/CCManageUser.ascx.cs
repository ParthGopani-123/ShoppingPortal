using System;
using BOL;
using Utility;
using System.Web.UI;

public partial class CCManageUser : System.Web.UI.UserControl
{
	public event EventHandler btnSaveUser_OnClick;
	public event EventHandler btnSaveAndNewUser_OnClick;

	public string SetUsersId
	{
		get { return lblUsersId.Text; }
		set { lblUsersId.Text = value; }
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			var dtDesignation = new Query() { eDesignationNot = (int)eDesignation.SystemAdmin, eStatus = (int)eStatus.Active }.Select(eSP.qry_Designation);
			CU.FillDropdown(ref ddlDesignation, dtDesignation, "-- Select Designation --", CS.DesignationId, CS.DesignationName);
		}
	}


	private void LoadOrganization()
	{
		var dtOrganization = new Query() { eStatus = (int)eStatus.Active }.Select(eSP.qry_Organization);
		CU.FillDropdown(ref ddlOrganization, dtOrganization, "-- Select Organization --", CS.OrganizationId, CS.OrganizationName);
	}

	private void LoadFirm()
	{
		var dtFirm = new Query() { OrganizationId = ddlOrganization.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_Firm);
		CU.FillDropdown(ref ddlFirm, dtFirm, "-- Select Firm --", CS.FirmId, CS.FirmName);
	}

	private void LoadParentUser()
	{
		var dtUser = new Query() { FirmId = ddlFirm.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_User);
		CU.FillDropdown(ref ddlParentUser, dtUser, "-- Parent User --", CS.UsersId, CS.Name);
	}

	private void LoadPriceList()
	{
		var dtPriceList = new Query() { FirmId = ddlFirm.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_PriceList);
		CU.FillDropdown(ref ddlPriceList, dtPriceList, "-- Price List --", CS.PriceListId, CS.PriceListName);
	}

	public void LoadUserDetail()
	{
		txtName.Focus();
		int? UsersId = null, AddressId = null;
		txtUserName.Enabled = false;

		divOldPassword.Visible = IsEditMode();
		txtOldPassword.Text = string.Empty;

		lblUserName.Attributes.Add("title", "");
		txtPassword.Attributes.Add("ZValidation", "e=blur|v=IsNullRequired|m=Password");
		txtConfirmPassword.Attributes.Add("ZValidation", "e=blur|v=IsNullRequired|m=Confirm Password");

		int FirmId = 0, OrganizationId = 0, ParentUsersId = 0, PriceListId = 0;

		if (IsEditMode())
		{
			lblpopupUserTitle.Text = "Edit User";
			var dtUser = new Query() { UsersId = lblUsersId.zToInt() }.Select(eSP.qry_User);

			var objUser = new Users() { }.SelectList<Users>(dtUser.Select())[0];

			UsersId = objUser.UsersId;

			ddlDesignation.SelectedValue = objUser.DesignationId.ToString();
			OrganizationId = dtUser.Rows[0][CS.OrganizationId].zToInt().Value;
			FirmId = objUser.FirmId.Value;
			ParentUsersId = objUser.ParentUsersId.Value;
			PriceListId = objUser.PriceListId.Value;
			txtDescription.Text = objUser.Description;

			txtName.Text = objUser.Name;
			txtMobileNo.Text = objUser.MobileNo;
			txtEmail.Text = objUser.Email;
			AddressId = objUser.AddressId;
			lblAddressId.Text = objUser.AddressId.ToString();

			var lstLogins = new Logins() { UsersId = objUser.UsersId }.SelectList<Logins>();
			if (lstLogins.Count > 0)
			{
				txtUserName.Text = lstLogins[0].Username;
				lblUserName.Attributes.Add("title", LoginUtilities.GetDBPassword(lstLogins[0].Password, lstLogins[0].PwdSalt));
			}
		}
		else
		{
			lblpopupUserTitle.Text = "New User";
			txtName.Text = txtMobileNo.Text = txtEmail.Text = txtDescription.Text = lblAddressId.Text = txtUserName.Text = string.Empty;
			txtUserName.Enabled = true;
			//ddlDesignation.SelectedValue = ((int)eDesignation.User).ToString();

			txtPassword.Text = txtConfirmPassword.Text = string.Empty;
			txtPassword.Attributes.Add("ZValidation", "e=blur|v=IsRequired|m=Password");
			txtConfirmPassword.Attributes.Add("ZValidation", "e=blur|v=IsRequired|m=Confirm Password");
		}

		eDesignation Designation = CU.GeteDesignationId(CU.GetUsersId());
		divOrganization.Visible = divFirm.Visible = Designation == eDesignation.SystemAdmin;
		if (OrganizationId == 0 && Designation != eDesignation.SystemAdmin)
		{
			CU.GetFirmOrganizationId(ref FirmId, ref OrganizationId);
		}

		LoadOrganization();
		ddlOrganization.SelectedValue = OrganizationId.ToString();
		LoadFirm();
		ddlFirm.SelectedValue = FirmId.ToString();
		LoadParentUser();
		ddlParentUser.SelectedValue = ParentUsersId.ToString();
		LoadPriceList();
		ddlPriceList.SelectedValue = PriceListId.ToString();

		SetControl(eControl.UserDetail);

		ManageContact.LoadContactDetail(UsersId, (int)eParentType.User);
		ManageAddress.LoadAddreessDetail(AddressId, false);
	}

	private bool IsEditMode()
	{
		return !lblUsersId.zIsNullOrEmpty();
	}

	private bool IsValidate()
	{
		if (!ddlDesignation.zIsSelect())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Designation.");
			ddlDesignation.Focus();
			SetControl(eControl.UserDetail);
			return false;
		}

		if (txtName.zIsNullOrEmpty())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Name");
			txtName.Focus();
			SetControl(eControl.UserDetail);
			return false;
		}

		if (!txtMobileNo.zIsMobile())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Mobile no.");
			txtMobileNo.Focus();
			SetControl(eControl.UserDetail);
			return false;
		}

		if (!txtEmail.zIsNullEmail())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Email.");
			txtEmail.Focus();
			SetControl(eControl.UserDetail);
			return false;
		}

		var dtUser = new Query()
		{
			eStatusNot = (int)eStatus.Delete,
			MobileNo = txtMobileNo.Text.Trim(),
		}.Select(eSP.qry_User);

		if (dtUser.Rows.Count > 0 && dtUser.Rows[0][CS.UsersId].ToString() != lblUsersId.Text)
		{
			string Status = dtUser.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
			CU.ZMessage(eMsgType.Error, string.Empty, "This mobile no is already exist" + Status + ".");
			txtMobileNo.Focus();
			SetControl(eControl.UserDetail);
			return false;
		}

		if (!ManageAddress.IsValidateAddress())
		{
			SetControl(eControl.ContactDetail);
			return false;
		}

		if (!ManageContact.IsValidateContactDetail())
		{
			SetControl(eControl.ContactDetail);
			return false;
		}

		if (!IsEditMode())
		{
			if (txtUserName.zIsNullOrEmpty())
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid User Name.");
				txtUserName.Focus();
				SetControl(eControl.Authentication);
				return false;
			}

			if (new Logins() { Username = txtUserName.Text.ToLower().Trim(), }.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Username is Already Exist.");
				txtUserName.Focus();
				SetControl(eControl.Authentication);
				return false;
			}
		}

		if (IsEditMode() && (!txtPassword.zIsNullOrEmpty() || !txtConfirmPassword.zIsNullOrEmpty() || !txtOldPassword.zIsNullOrEmpty()))
		{
			if (txtOldPassword.zIsNullOrEmpty())
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Old Password.");
				txtOldPassword.Focus();
				SetControl(eControl.Authentication);
				return false;
			}

			if (!LoginUtilities.IsValidPassword(lblUsersId.zToInt().Value, txtOldPassword.Text))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Old Password.");
				txtOldPassword.Focus();
				SetControl(eControl.Authentication);
				return false;
			}
		}

		if (!IsEditMode() || !txtPassword.zIsNullOrEmpty() || !txtConfirmPassword.zIsNullOrEmpty() || !txtOldPassword.zIsNullOrEmpty())
		{
			if (txtPassword.zIsNullOrEmpty())
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Password.");
				txtPassword.Focus();
				SetControl(eControl.Authentication);
				return false;
			}

			if (txtConfirmPassword.zIsNullOrEmpty())
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Confirm Password.");
				txtConfirmPassword.Focus();
				SetControl(eControl.Authentication);
				return false;
			}

			if (txtPassword.Text != txtConfirmPassword.Text)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Confirm Password Not Match.");
				txtConfirmPassword.Focus();
				SetControl(eControl.Authentication);
				return false;
			}
		}
		return true;
	}

	private bool SaveData()
	{
		if (!IsValidate())
			return false;

		lblAddressId.Text = ManageAddress.SaveAddress(lblAddressId.zToInt()).ToString();

		string Message = string.Empty;
		int? UsersId = null;
		var objUser = new Users()
		{
			FirmId = ddlFirm.zToInt(),
			Name = txtName.Text,
			AddressId = lblAddressId.zToInt(),
			DesignationId = ddlDesignation.zToInt(),
			MobileNo = txtMobileNo.Text.Trim(),
			ParentUsersId = ddlParentUser.zToInt(),
			PriceListId = ddlPriceList.zToInt(), //??
			Email = txtEmail.Text,
			Description = txtDescription.Text,
		};

		if (IsEditMode())
		{
			UsersId = lblUsersId.zToInt();
			objUser.UsersId = UsersId;
			objUser.Update();

			Message = "User Detail Change Sucessfully";

			if (!txtPassword.zIsNullOrEmpty())
				LoginUtilities.ChangePassword(UsersId.Value, txtPassword.Text);
		}
		else
		{
			objUser.eStatus = (int)eStatus.Active;
			UsersId = objUser.Insert();

			LoginUtilities.CreateLogin(UsersId.Value, txtUserName.Text.ToLower(), txtPassword.Text);

			CU.SetDefaultAuthority(UsersId.Value, objUser.DesignationId.Value);

			Message = "User Added Sucessfully";
		}

		ManageContact.SaveContactDetail(UsersId.Value, (int)eParentType.User);

		lblUsersId.Text = UsersId.ToString();

		CU.ZMessage(eMsgType.Success, string.Empty, Message);

		SetUsersId = string.Empty;
		return true;
	}


	protected void btnSave_OnClick(object sender, EventArgs e)
	{
		if (SaveData())
			btnSaveUser_OnClick(null, null);
	}

	protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
	{
		if (SaveData())
		{
			lblUsersId.Text = string.Empty;
			LoadUserDetail();
			btnSaveAndNewUser_OnClick(null, null);
		}
	}


	protected void ddlOrganization_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		LoadFirm();
		LoadParentUser();
		LoadPriceList();
		ddlFirm.Focus();
	}

	protected void ddlFirm_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		LoadParentUser();
		LoadPriceList();
		ddlDesignation.Focus();
	}


	private void SetControl(eControl Control)
	{
		switch (Control)
		{
			case eControl.UserDetail:
				ScriptManager.RegisterStartupScript(Page, typeof(Page), "ManageTabUser", "ManageTabUser('liTabUserDetail', 'pnlUserDetail');", true);
				break;
			case eControl.ContactDetail:
				ScriptManager.RegisterStartupScript(Page, typeof(Page), "ManageTabUser", "ManageTabUser('liTabContactDetail', 'pnlContactDetail');", true);
				break;
			case eControl.Authentication:
				ScriptManager.RegisterStartupScript(Page, typeof(Page), "ManageTabUser", "ManageTabUser('liTabAuthentication', 'pnlAuthentication');", true);
				break;
			default:
				break;
		}
	}

	private enum eControl
	{
		UserDetail = 1,
		ContactDetail = 2,
		Authentication = 3,
	}
}
