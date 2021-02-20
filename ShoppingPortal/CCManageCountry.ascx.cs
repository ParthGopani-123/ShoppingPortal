using System;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;

public partial class CCManageCountry : System.Web.UI.UserControl
{
	public event EventHandler btnSave_OnClick;
	public event EventHandler btnSaveAndNew_OnClick;

	public string SetCountryId
	{
		get { return lblCountryId.Text; }
		set { lblCountryId.Text = value; }
	}

	public void LoadCountryDetail(bool OnlyEdit)
	{
		txtCountryName.Focus();
		btnSaveAndNewCountry.Visible = !OnlyEdit;

		if (IsEditMode())
		{
			lblPopupTitle.Text = "Edit Country";
			var objCountry = new Country() { CountryId = lblCountryId.zToInt(), }.SelectList<Country>()[0];

			txtCountryName.Text = objCountry.CountryName;
			txtDescription.Text = objCountry.Description;
		}
		else
		{
			lblPopupTitle.Text = "New Country";

			txtCountryName.Text = txtDescription.Text = string.Empty;
		}
	}


	private bool IsEditMode()
	{
		return !lblCountryId.zIsNullOrEmpty();
	}

	private bool IsValidate()
	{
		if (txtCountryName.zIsNullOrEmpty())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Country Name.");
			txtCountryName.Focus();
			return false;
		}

		var dtCountry = new Query()
		{
			eStatusNot = (int)eStatus.Delete,
			CountryName = txtCountryName.Text.Trim(),
		}.Select(eSP.qry_Country);

		if (dtCountry.Rows.Count > 0 && dtCountry.Rows[0][CS.CountryId].ToString() != lblCountryId.Text)
		{
			string Status = dtCountry.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
			CU.ZMessage(eMsgType.Error, string.Empty, "This Country is already exist" + Status + ".");
			txtCountryName.Focus();
			return false;
		}

		return true;
	}

	private bool SaveData()
	{
		if (!IsValidate())
			return false;

		string Message = string.Empty;

		var objCountry = new Country()
		{
			CountryName = txtCountryName.Text.Trim().zFirstCharToUpper(),
			Description = txtDescription.Text,
		};

		if (IsEditMode())
		{
			objCountry.CountryId = lblCountryId.zToInt();
			objCountry.Update();

			Message = "Country Detail Change Sucessfully.";
		}
		else
		{
			objCountry.eStatus = (int)eStatus.Active;
			objCountry.CountryId = objCountry.Insert();

			Message = "New Country Added Sucessfully.";
		}

		CU.ZMessage(eMsgType.Success, string.Empty, Message);

		return true;
	}


	protected void btnSaveCountry_OnClick(object sender, EventArgs e)
	{
		if (SaveData())
		{
			try { btnSave_OnClick(null, null); }
			catch { }
		}
	}

	protected void btnSaveAndNewCountry_OnClick(object sender, EventArgs e)
	{
		if (SaveData())
		{
			try { btnSaveAndNew_OnClick(null, null); }
			catch { }
		}
	}

}
