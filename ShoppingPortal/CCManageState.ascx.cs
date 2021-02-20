using System;
using BOL;
using Utility;

public partial class CCManageState : System.Web.UI.UserControl
{
	public event EventHandler btnSave_OnClick;
	public event EventHandler btnSaveAndNew_OnClick;

	public string SetStateId
	{
		get { return lblStateId.Text; }
		set { lblStateId.Text = value; }
	}

	private void LoadCountry()
	{
		int? CountryId = ddlCountry.zToInt();

		CU.FillDropdown(ref ddlCountry, new Country() { eStatus = (int)eStatus.Active }.Select(), "-- Select Country --", CS.CountryId, CS.CountryName);

		try { ddlCountry.SelectedValue = CountryId.ToString(); }
		catch { }

	}

	public void LoadStateDetail(bool IsOnlyEdit)
	{
		btnSaveAndNewState.Visible = !IsOnlyEdit;

		LoadCountry();
		ddlCountry.Focus();

		if (IsEditMode())
		{
			lblPopupTitle.Text = "Edit State";
			var objState = new State() { StateId = lblStateId.zToInt(), }.SelectList<State>()[0];

			ddlCountry.SelectedValue = objState.CountryId.ToString();
			txtStateName.Text = objState.StateName;
			txtDescription.Text = objState.Description;
		}
		else
		{
			lblPopupTitle.Text = "New State";
			txtStateName.Text = txtDescription.Text = string.Empty;
		}
	}

	private bool IsEditMode()
	{
		return !lblStateId.zIsNullOrEmpty();
	}

	private bool IsValidate()
	{
		if (!ddlCountry.zIsSelect())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Country.");
			ddlCountry.Focus();
			return false;
		}

		if (txtStateName.zIsNullOrEmpty())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter State Name.");
			txtStateName.Focus();
			return false;
		}

		var dtState = new Query()
		{
			eStatusNot = (int)eStatus.Delete,
			CountryId = ddlCountry.zToInt(),
			StateName = txtStateName.Text.Trim(),
		}.Select(eSP.qry_State);

		if (dtState.Rows.Count > 0 && dtState.Rows[0][CS.StateId].ToString() != lblStateId.Text)
		{
			string Status = dtState.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
			CU.ZMessage(eMsgType.Error, string.Empty, "This State is already exist" + Status + ".");
			txtStateName.Focus();
			return false;
		}

		return true;
	}

	private bool SaveData()
	{
		if (!IsValidate())
			return false;

		string Message = string.Empty;

		var objState = new State()
		{
			CountryId = ddlCountry.zToInt(),
			StateName = txtStateName.Text.Trim().zFirstCharToUpper(),
			Description = txtDescription.Text,
		};

		if (IsEditMode())
		{
			objState.StateId = lblStateId.zToInt();
			objState.Update();

			Message = "State Detail Change Sucessfully.";
		}
		else
		{
			objState.eStatus = (int)eStatus.Active;
			objState.Insert();

			Message = "New State Added Sucessfully.";
		}

		CU.ZMessage(eMsgType.Success, string.Empty, Message);

		return true;
	}

	protected void btnSaveState_OnClick(object sender, EventArgs e)
	{
		if (SaveData())
		{
			try { btnSave_OnClick(null, null); }
			catch { }
		}
	}

	protected void btnSaveAndNewState_OnClick(object sender, EventArgs e)
	{
		if (SaveData())
		{
			try { btnSaveAndNew_OnClick(null, null); }
			catch { }
		}
	}


	protected void lnkCountry_OnClick(object sender, EventArgs e)
	{
		LoadCountry();
		ddlCountry.Focus();
	}
}
