using System;
using BOL;
using Utility;

public partial class CCManageCity : System.Web.UI.UserControl
{
	public event EventHandler btnSave_OnClick;
	public event EventHandler btnSaveAndNew_OnClick;

	public string SetCityId
	{
		get { return lblCityId.Text; }
		set { lblCityId.Text = value; }
	}


	private void LoadCountry()
	{
		int? CountryId = ddlCountry.zToInt();

		CU.FillDropdown(ref ddlCountry, new Country() { eStatus = (int)eStatus.Active }.Select(), "-- Select Country --", CS.CountryId, CS.CountryName);

		try { ddlCountry.SelectedValue = CountryId.ToString(); }
		catch { }
	}

	private void LoadState()
	{
		int? SateId = ddlState.zToInt();

		CU.FillDropdown(ref ddlState, new State()
		{
			CountryId = ddlCountry.zToInt(),
			eStatus = (int)eStatus.Active
		}.Select(), "-- Select State --", CS.StateId, CS.StateName);

		try { ddlState.SelectedValue = SateId.ToString(); }
		catch { }
	}

	public void LoadCityDetail(bool IsOnlyEdit)
	{
		btnSaveAndNewCity.Visible = !IsOnlyEdit;
		LoadCountry();
		ddlCountry.Focus();

		if (IsEditMode())
		{
			lblPopupTitle.Text = "Edit City";
			var dtCity = new Query() { CityId = lblCityId.zToInt() }.Select(eSP.qry_City);

			var objCity = new City() { }.SelectList<City>(dtCity.Select())[0];

			ddlCountry.SelectedValue = dtCity.Rows[0][CS.CountryId].ToString();
			LoadState();
			ddlState.SelectedValue = objCity.StateId.ToString();

			txtCityName.Text = objCity.CityName;
		}
		else
		{
			lblPopupTitle.Text = "New City";
			LoadState();
			txtCityName.Text = string.Empty;
		}
	}

	private bool IsEditMode()
	{
		return !lblCityId.zIsNullOrEmpty();
	}

	private bool IsValidate()
	{
		if (!ddlCountry.zIsSelect())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Country.");
			ddlCountry.Focus();
			return false;
		}

		if (!ddlState.zIsSelect())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Select State.");
			ddlState.Focus();
			return false;
		}

		if (txtCityName.zIsNullOrEmpty())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter City Name.");
			txtCityName.Focus();
			return false;
		}

		var dtCity = new Query()
		{
			eStatusNot = (int)eStatus.Delete,
			CountryId = ddlCountry.zToInt(),
			StateId = ddlState.zToInt(),
			CityName = txtCityName.Text.Trim(),
		}.Select(eSP.qry_City);

		if (dtCity.Rows.Count > 0 && dtCity.Rows[0][CS.CityId].ToString() != lblCityId.Text)
		{
			string Status = dtCity.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
			CU.ZMessage(eMsgType.Error, string.Empty, "This City is already exist" + Status + ".");
			txtCityName.Focus();
			return false;
		}

		return true;
	}

	private bool SaveData()
	{
		if (!IsValidate())
			return false;

		string Message = string.Empty;

		var objCity = new City()
		{
			StateId = ddlState.zToInt(),
			CityName = txtCityName.Text.Trim().zFirstCharToUpper(),
		};

		if (IsEditMode())
		{
			objCity.CityId = lblCityId.zToInt();
			objCity.Update();

			Message = "City Detail Change Sucessfully.";
		}
		else
		{
			objCity.eStatus = (int)eStatus.Active;
			objCity.Insert();

			Message = "New City Added Sucessfully.";
		}

		CU.ZMessage(eMsgType.Success, string.Empty, Message);

		return true;
	}

	protected void btnSaveCity_OnClick(object sender, EventArgs e)
	{
		if (SaveData())
		{
			try { btnSave_OnClick(null, null); }
			catch { }
		}
	}

	protected void btnSaveAndNewCity_OnClick(object sender, EventArgs e)
	{
		if (SaveData())
		{
			try { btnSaveAndNew_OnClick(null, null); }
			catch { }
		}
	}


	protected void ddlCountry_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		LoadState();
		ddlState.Focus();
	}

	protected void lnkCountry_OnClick(object sender, EventArgs e)
	{
		LoadCountry();
		ddlCountry.Focus();
	}

	protected void lnkState_OnClick(object sender, EventArgs e)
	{
		LoadState();
		ddlState.Focus();
	}
}
