using System;
using System.Data;
using System.Threading.Tasks;
using BOL;
using Utility;

public partial class CCManageAddress : System.Web.UI.UserControl
{
	public event EventHandler btnPagePostback;

	public void LoadAddreessDetail(int? AddressId, bool LoadMap)
	{
		chkCountry.Checked = chkState.Checked = chkCity.Checked = chkArea.Checked = false;

		lblMapType.Text = LoadMap ? ((int)eLoadMap.Yes).ToString() : ((int)eLoadMap.No).ToString();

		divMapData.Visible = LoadMap;
		divAreaDetalNotMap.Visible = !LoadMap;

		LoadArea();

		if (AddressId.HasValue && AddressId != 0)
		{
			var objAddress = new Address() { AddressId = AddressId }.SelectList<Address>()[0];

			ddlArea.SelectedValue = objAddress.AreaId.ToString();
			ddlCity.SelectedValue = objAddress.CityId.ToString();
			ddlState.SelectedValue = objAddress.StateId.ToString();
			ddlCountry.SelectedValue = objAddress.CountryId.ToString();

			txtPincode.Text = txtPincodeMap.Text = objAddress.Pincode.ToString();
			txtAddress1.Text = objAddress.Address1;
			txtAddress2.Text = objAddress.Address2;

			txtLatitude.Text = objAddress.Latitude;
			txtLongitude.Text = objAddress.Longitude;

			if (LoadMap)
			{
				txtAreaMap.Text = objAddress.AreaId > 0 ? (new Area() { AreaId = objAddress.AreaId }.SelectList<Area>()[0].AreaName.ToString()) : string.Empty;
				txtCityMap.Text = objAddress.CityId > 0 ? (new City() { CityId = objAddress.CityId }.SelectList<City>()[0].CityName.ToString()) : string.Empty;
				txtStateMap.Text = objAddress.StateId > 0 ? (new State() { StateId = objAddress.StateId }.SelectList<State>()[0].StateName.ToString()) : string.Empty;
				txtCountryMap.Text = objAddress.CountryId > 0 ? (new Country() { CountryId = objAddress.CountryId }.SelectList<Country>()[0].CountryName.ToString()) : string.Empty;

				System.Web.UI.ScriptManager.RegisterStartupScript(Page, typeof(System.Web.UI.Page), "SetMapData", "SetMapData('true');", true);
			}
		}
		else
		{
			txtSearchAddress.Text = txtLatitude.Text = txtLongitude.Text = txtPincodeMap.Text = txtAreaMap.Text = txtCityMap.Text = txtStateMap.Text = txtCountryMap.Text = string.Empty;
			txtAddress1.Text = txtAddress2.Text = txtPincode.Text = txtOtherCountry.Text = txtOtherState.Text = txtOtherCity.Text = txtOtherArea.Text = string.Empty;
		}
	}

	private bool IsLoadMap()
	{
		return (lblMapType.zToInt() == (int)eLoadMap.Yes);
	}


	private void LoadArea()
	{
		var dtArea = new Query() { eStatus = (int)eStatus.Active, }.Select(eSP.qry_Area);
		if (dtArea.Rows.Count == 0)
		{
			SetControlStatus(eControlEvent.AreaAdd);
			lnkAreaCancle.Enabled = false;
		}
		else
		{
			SetControlStatus(eControlEvent.AreaSelect);
			lnkAreaCancle.Enabled = true;
		}

		var drArea = dtArea.NewRow();
		drArea[CS.AreaId] = "0";
		drArea[CS.FullAreaName] = "-- Select Area --";
		dtArea.Rows.InsertAt(drArea, 0);

		ddlArea.DataSource = dtArea;
		ddlArea.DataValueField = CS.AreaId;
		ddlArea.DataTextField = CS.FullAreaName;
		ddlArea.DataBind();

		LoadCity(new DataView(dtArea).ToTable(true, CS.CityId, CS.FullCityName));
		LoadState(new DataView(dtArea).ToTable(true, CS.StateId, CS.FullStateName));
		LoadCounrty(new DataView(dtArea).ToTable(true, CS.CountryId, CS.CountryName));
	}

	private void LoadCity(DataTable dtCity)
	{
		if (dtCity.Rows.Count == 0)
		{
			SetControlStatus(eControlEvent.CityAdd);
			lnkCityCancle.Enabled = false;
		}
		else if (chkArea.Checked)
		{
			SetControlStatus(eControlEvent.CitySelect);
			lnkCityCancle.Enabled = true;
		}

		var drCity = dtCity.NewRow();
		drCity[CS.CityId] = "0";
		drCity[CS.FullCityName] = "-- Select City --";
		dtCity.Rows.InsertAt(drCity, 0);

		ddlCity.DataSource = dtCity;
		ddlCity.DataValueField = CS.CityId;
		ddlCity.DataTextField = CS.FullCityName;
		ddlCity.DataBind();

		if (ddlCity.Items.Count == 2)
			ddlCity.SelectedIndex = 1;
	}

	private void LoadState(DataTable dtState)
	{
		if (dtState.Rows.Count == 0)
		{
			SetControlStatus(eControlEvent.StateAdd);
			lnkStateCancle.Enabled = false;
		}
		else if (chkCity.Checked)
		{
			SetControlStatus(eControlEvent.StateSelect);
			lnkStateCancle.Enabled = true;
		}

		var drState = dtState.NewRow();
		drState[CS.StateId] = "0";
		drState[CS.FullStateName] = "-- Select State --";
		dtState.Rows.InsertAt(drState, 0);

		ddlState.DataSource = dtState;
		ddlState.DataValueField = CS.StateId;
		ddlState.DataTextField = CS.FullStateName;
		ddlState.DataBind();

		if (ddlState.Items.Count == 2)
			ddlState.SelectedIndex = 1;

	}

	private void LoadCounrty(DataTable dtCountry)
	{
		if (dtCountry.Rows.Count == 0)
		{
			SetControlStatus(eControlEvent.CountryAdd);
			lnkCountryCancle.Enabled = false;
		}
		else if (chkState.Checked)
		{
			SetControlStatus(eControlEvent.CountrySelect);
			lnkCountryCancle.Enabled = true;
		}

		var drCountry = dtCountry.NewRow();
		drCountry[CS.CountryId] = "0";
		drCountry[CS.CountryName] = "-- Select Country --";
		dtCountry.Rows.InsertAt(drCountry, 0);

		ddlCountry.DataSource = dtCountry;
		ddlCountry.DataValueField = CS.CountryId;
		ddlCountry.DataTextField = CS.CountryName;
		ddlCountry.DataBind();

		if (ddlCountry.Items.Count == 2)
			ddlCountry.SelectedIndex = 1;
	}


	public bool IsValidateAddress()
	{
		bool LoadMap = IsLoadMap();

		if (LoadMap)
		{
			if (!txtLatitude.zIsNullOrEmpty() && !txtLatitude.zIsFloat(true))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Latitude.");
				txtLatitude.Focus();
				return false;
			}

			if (!txtLongitude.zIsNullOrEmpty() && !txtLongitude.zIsFloat(true))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Longitude.");
				txtLongitude.Focus();
				return false;
			}

			if (!txtPincodeMap.zIsNullOrEmpty() && !txtPincodeMap.zIsNumber())
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid  Pincode.");
				txtPincodeMap.Focus();
				return false;
			}

			if (txtAreaMap.zIsNullOrEmpty())
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Area.");
				txtAreaMap.Focus();
				return false;
			}

			if (txtCityMap.zIsNullOrEmpty())
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter City.");
				txtCityMap.Focus();
				return false;
			}

			if (txtStateMap.zIsNullOrEmpty())
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter State.");
				txtStateMap.Focus();
				return false;
			}

			if (txtCountryMap.zIsNullOrEmpty())
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Country.");
				txtCountryMap.Focus();
				return false;
			}
		}
		else
		{
			if (!txtPincode.zIsNullOrEmpty() && !txtPincode.zIsNumber())
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid  Pincode.");
				txtPincode.Focus();
				return false;
			}

			if (chkArea.Checked && txtOtherArea.zIsNullOrEmpty())
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter  Area.");
				txtOtherArea.Focus();
				return false;
			}

			if (chkCity.Checked && txtOtherCity.zIsNullOrEmpty())
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter  City.");
				txtOtherCity.Focus();
				return false;
			}

			if (chkState.Checked && txtOtherState.zIsNullOrEmpty())
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter  State.");
				txtOtherState.Focus();
				return false;
			}

			if (chkCountry.Checked && txtOtherCountry.zIsNullOrEmpty())
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter  Country.");
				txtOtherCountry.Focus();
				return false;
			}
		}
		return true;
	}

	public int SaveAddress(int? AddressId)
	{
		bool LoadMap = IsLoadMap();

		var objAddress = new Address()
		{
			Address1 = RemoveLastComma(txtAddress1.Text).zFirstCharToUpper(),
			Address2 = RemoveLastComma(txtAddress2.Text).zFirstCharToUpper(),
			Pincode = LoadMap ? txtPincodeMap.Text : txtPincode.Text,

			Latitude = txtLatitude.Text,
			Longitude = txtLongitude.Text,
		};

		if (LoadMap)
		{
			objAddress.CountryId = CU.GetCountryId(txtCountryMap.Text);
			objAddress.StateId = CU.GetStateId(objAddress.CountryId, txtStateMap.Text);
			objAddress.CityId = CU.GetCityId(objAddress.StateId, txtCityMap.Text);
			objAddress.AreaId = CU.GetAreaId(objAddress.CityId, txtAreaMap.Text, objAddress.Pincode);
		}
		else
		{
			#region Set Country Value

			if (chkCountry.Checked)
				objAddress.CountryId = CU.GetCountryId(txtOtherCountry.Text);
			else
				objAddress.CountryId = ddlCountry.zToInt();

			#endregion Set Country Value

			#region Set State Value

			if (chkState.Checked)
				objAddress.StateId = CU.GetStateId(objAddress.CountryId, txtOtherState.Text);
			else
				objAddress.StateId = ddlState.zToInt();

			#endregion Set State Value

			#region Set City Value

			if (chkCity.Checked)
				objAddress.CityId = CU.GetCityId(objAddress.StateId, txtOtherCity.Text);
			else
				objAddress.CityId = ddlCity.zToInt();

			#endregion Set City Value

			#region Set Area Value

			if (chkArea.Checked)
				objAddress.AreaId = CU.GetAreaId(objAddress.CityId, txtOtherArea.Text, txtPincode.Text);
			else
				objAddress.AreaId = ddlArea.zToInt();

			#endregion Set Area Value
		}

		if (AddressId.HasValue && AddressId != 0)
		{
			objAddress.AddressId = AddressId;
			objAddress.UpdateAsync();
		}
		else
		{
			AddressId = objAddress.Insert();
		}

		return AddressId.Value;
	}

	private string RemoveLastComma(string Input)
	{
		Input = Input.Trim();
		if (Input.EndsWith(","))
			Input = Input.Substring(0, Input.LastIndexOf(","));

		return Input;
	}


	protected void ddlArea_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var dtArea = new Query() { AreaId = ddlArea.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_Area);
		if (dtArea.Rows.Count > 0)
		{
			var drArea = dtArea.Rows[0];
			if (txtPincode.zIsNullOrEmpty())
				txtPincode.Text = drArea[CS.Pincode].ToString();
			ddlCity.SelectedValue = drArea[CS.CityId].ToString();
			ddlState.SelectedValue = drArea[CS.StateId].ToString();
			ddlCountry.SelectedValue = drArea[CS.CountryId].ToString();
		}
		else
		{
			txtPincode.Text = string.Empty;
			ddlCity.SelectedValue = ddlState.SelectedValue = ddlCountry.SelectedValue = "0";
		}

		try { btnPagePostback(null, null); }
		catch { }
	}

	protected void ddlCity_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var dtCity = new Query() { CityId = ddlCity.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_City);
		if (dtCity.Rows.Count > 0)
		{
			var drCity = dtCity.Rows[0];
			ddlState.SelectedValue = drCity[CS.StateId].ToString();
			ddlCountry.SelectedValue = drCity[CS.CountryId].ToString();
		}
		else
		{
			ddlState.SelectedValue = ddlCountry.SelectedValue = "0";
		}

		try { btnPagePostback(null, null); }
		catch { }
	}

	protected void ddlState_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var dtState = new Query() { StateId = ddlState.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_State);
		if (dtState.Rows.Count > 0)
		{
			ddlCountry.SelectedValue = dtState.Rows[0][CS.CountryId].ToString();
		}
		else
		{
			ddlCountry.SelectedValue = "0";
		}

		try { btnPagePostback(null, null); }
		catch { }
	}

	protected void ddlCountry_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		try { btnPagePostback(null, null); }
		catch { }
	}


	protected void lnkAreaAdd_OnClick(object sender, EventArgs e)
	{
		SetControlStatus(eControlEvent.AreaAdd);

		try { btnPagePostback(null, null); }
		catch { }
	}

	protected void lnkCityAdd_OnClick(object sender, EventArgs e)
	{
		SetControlStatus(eControlEvent.CityAdd);

		try { btnPagePostback(null, null); }
		catch { }
	}

	protected void lnkStateAdd_OnClick(object sender, EventArgs e)
	{
		SetControlStatus(eControlEvent.StateAdd);

		try { btnPagePostback(null, null); }
		catch { }
	}

	protected void lnkCountryAdd_OnClick(object sender, EventArgs e)
	{
		SetControlStatus(eControlEvent.CountryAdd);

		try { btnPagePostback(null, null); }
		catch { }
	}

	protected void lnkAreaCancle_OnClick(object sender, EventArgs e)
	{
		SetControlStatus(eControlEvent.AreaSelect);

		try { btnPagePostback(null, null); }
		catch { }
	}

	protected void lnkCityCancle_OnClick(object sender, EventArgs e)
	{
		SetControlStatus(eControlEvent.CitySelect);

		try { btnPagePostback(null, null); }
		catch { }
	}

	protected void lnkStateCancle_OnClick(object sender, EventArgs e)
	{
		SetControlStatus(eControlEvent.StateSelect);

		try { btnPagePostback(null, null); }
		catch { }
	}

	protected void lnkCountryCancle_OnClick(object sender, EventArgs e)
	{
		SetControlStatus(eControlEvent.CountrySelect);

		try { btnPagePostback(null, null); }
		catch { }
	}


	private void SetControlStatus(eControlEvent ControlEvent)
	{
		switch (ControlEvent)
		{
			case eControlEvent.AreaSelect:
				#region Area Select

				divArea.Visible = true;
				divOtherArea.Visible = false;
				chkArea.Checked = chkCity.Checked = chkState.Checked = chkCountry.Checked = false;

				divCity.Visible = divState.Visible = divCountry.Visible = false;
				divOtherCity.Visible = divOtherState.Visible = divOtherCountry.Visible = false;

				#endregion Area Select
				break;
			case eControlEvent.AreaAdd:
				#region Area Add

				divArea.Visible = false;
				divOtherArea.Visible = true;
				chkArea.Checked = true;

				//ddlArea.SelectedValue = ddlCity.SelectedValue = "0";

				txtOtherArea.Text = string.Empty;
				txtOtherArea.Focus();

				divCity.Visible = true;
				divState.Visible = divCountry.Visible = false;

				divOtherCity.Visible = divOtherState.Visible = divOtherCountry.Visible = false;

				#endregion Area Add
				break;
			case eControlEvent.CitySelect:
				#region CitySelect

				divCity.Visible = true;
				divOtherCity.Visible = false;
				chkCity.Checked = chkState.Checked = chkCountry.Checked = false;

				divState.Visible = divCountry.Visible = false;
				divOtherState.Visible = divOtherCountry.Visible = false;

				#endregion CitySelect
				break;
			case eControlEvent.CityAdd:
				#region CityAdd

				divCity.Visible = false;
				divOtherCity.Visible = true;
				chkCity.Checked = true;

				//ddlCity.SelectedValue = ddlState.SelectedValue = "0";

				txtOtherCity.Text = string.Empty;
				txtOtherCity.Focus();

				divState.Visible = true;
				divCountry.Visible = false;

				divOtherState.Visible = divOtherCountry.Visible = false;

				#endregion CityAdd
				break;
			case eControlEvent.StateSelect:
				#region StateSelect

				divState.Visible = true;
				divOtherState.Visible = false;
				chkState.Checked = chkCountry.Checked = false;

				divCountry.Visible = false;
				divOtherCountry.Visible = false;

				#endregion StateSelect
				break;
			case eControlEvent.StateAdd:
				#region StateAdd

				divState.Visible = false;
				divOtherState.Visible = true;
				chkState.Checked = true;

				//ddlState.SelectedValue = ddlCountry.SelectedValue = "0";

				txtOtherState.Text = string.Empty;
				txtOtherState.Focus();

				divCountry.Visible = true;
				divOtherCountry.Visible = false;

				#endregion StateAdd
				break;
			case eControlEvent.CountrySelect:
				#region CountrySelect

				divCountry.Visible = true;
				divOtherCountry.Visible = false;
				chkCountry.Checked = false;

				#endregion CountrySelect
				break;
			case eControlEvent.CountryAdd:
				#region CountryAdd

				divCountry.Visible = false;
				divOtherCountry.Visible = true;
				chkCountry.Checked = true;

				//ddlCountry.SelectedValue = "0";

				txtOtherCountry.Text = string.Empty;
				txtOtherCountry.Focus();

				#endregion CountryAdd
				break;
		}
	}

	public enum eControlEvent
	{
		CountrySelect = 1,
		CountryAdd = 2,

		StateSelect = 3,
		StateAdd = 4,

		CitySelect = 5,
		CityAdd = 6,

		AreaSelect = 7,
		AreaAdd = 8,
	}

	public enum eLoadMap
	{
		Yes = 1,
		No = 2,
	}

}
