using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;

public partial class ManageArea : CompressorPage
{
	string CountryName, StateName, CityName, AreaName, Pincode;
	int CountryNameColumn = 0, StateNameColumn = 1, CityNameColumn = 2, AreaNameColumn = 3, PincodeColumn = 3;

	bool? IsAddEdit, IsAddEditCity, IsAddEditState, IsAddEditCountry;


	private int PageIndex
	{
		get
		{
			if (ViewState["PageIndex"] != null)
				return Convert.ToInt32(ViewState["PageIndex"]);
			else
				return 0;
		}
		set { ViewState["PageIndex"] = value; }
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		LoginUtilities.CheckSession();

		if (!IsPostBack)
		{
			CU.LoadDisplayPerPage(ref ddlRecordPerPage);

			LoadSearchCountry();
			LoadAreaGrid(ePageIndex.Custom);
			CheckVisibleButton();
		}

		Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
		Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
		Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

		popupManageArea.btnSave_OnClick += new EventHandler(btnSave_OnClick);
		popupManageArea.btnSaveAndNew_OnClick += new EventHandler(btnSaveAndNew_OnClick);

		popupManageCity.btnSave_OnClick += new EventHandler(btnSaveCity_OnClick);
		poupManageState.btnSave_OnClick += new EventHandler(btnSaveState_OnClick);
		popupManageCountry.btnSave_OnClick += new EventHandler(btnSaveCountry_OnClick);

		try { grdArea.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}


	private void LoadSearchCountry()
	{
		int? CountryId = ddlSearchCountry.zToInt();

		CU.FillDropdown(ref ddlSearchCountry, new Country() { eStatus = (int)eStatus.Active }.Select(), "-- All Country --", CS.CountryId, CS.CountryName);

		try { ddlSearchCountry.SelectedValue = CountryId.ToString(); }
		catch { }

		int SetStateId = 0;
		if (!IsPostBack && !string.IsNullOrEmpty(Request.QueryString[CS.CityId.Encrypt()]))
		{
			string CityId = Request.QueryString[CS.CityId.Encrypt()].ToString().Decrypt();
			if (CityId.zIsInteger(false))
			{
				var dtCity = new Query() { CityId = CityId.zToInt() }.Select(eSP.qry_City);
				if (dtCity.Rows.Count > 0)
				{
					ddlSearchCountry.SelectedValue = dtCity.Rows[0][CS.CountryId].ToString();
					SetStateId = dtCity.Rows[0][CS.StateId].zToInt().Value;
				}
			}
		}

		LoadSearchState(SetStateId);
	}

	private void LoadSearchState(int? SetStateId)
	{
		int? StateId = ddlSearchState.zToInt();

		CU.FillDropdown(ref ddlSearchState, new State()
		{
			CountryId = ddlSearchCountry.zToInt(),
			eStatus = (int)eStatus.Active
		}.Select(), "-- All State --", CS.StateId, CS.StateName);

		try { ddlSearchState.SelectedValue = StateId.ToString(); }
		catch { }

		if (!IsPostBack)
			ddlSearchState.SelectedValue = "0";

		if (SetStateId.HasValue)
		{
			try { ddlSearchState.SelectedValue = SetStateId.ToString(); }
			catch { }
		}

		LoadSearchCity();
	}

	private void LoadSearchCity()
	{
		int? CityId = ddlSearchState.zToInt();

		CU.FillDropdown(ref ddlSearchCity, new City()
		{
			StateId = ddlSearchState.zToInt(),
			eStatus = (int)eStatus.Active
		}.Select(new City() { CityId = 0, CityName = "" }), "-- All City --", CS.CityId, CS.CityName);

		try { ddlSearchCity.SelectedValue = CityId.ToString(); }
		catch { }

		if (!IsPostBack)
			ddlSearchCity.SelectedValue = "0";

		if (!IsPostBack && !string.IsNullOrEmpty(Request.QueryString[CS.CityId.Encrypt()]))
		{
			try
			{
				ddlSearchCity.SelectedValue = Request.QueryString[CS.CityId.Encrypt()].ToString().Decrypt();
			}
			catch { }
		}
	}

	private DataTable GetAreaDt(ePageIndex ePageIndex)
	{
		int? Status = null;
		if (chkActive.Checked && !chkDeactive.Checked)
			Status = (int)eStatus.Active;
		else if (!chkActive.Checked && chkDeactive.Checked)
			Status = (int)eStatus.Deactive;

		var objQuery = new Query()
		{
			MasterSearch = txtSearch.Text,
			CountryId = ddlSearchCountry.zIsSelect() ? ddlSearchCountry.zToInt() : (int?)null,
			StateId = ddlSearchState.zIsSelect() ? ddlSearchState.zToInt() : (int?)null,
			CityId = ddlSearchCity.zIsSelect() ? ddlSearchCity.zToInt() : (int?)null,
			eStatus = Status,
			eStatusNot = (int)eStatus.Delete,
		};

		#region Page Index

		int RecordPerPage = ddlRecordPerPage.zToInt().Value;
		int PageIndexTemp = PageIndex;

		CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref objQuery, ref txtGotoPageNo, ref lblCount);
		PageIndex = PageIndexTemp;

		#endregion Page Index

		return objQuery.Select(eSP.qry_Area);
	}

	private void LoadAreaGrid(ePageIndex ePageIndex)
	{
		DataTable dtArea = GetAreaDt(ePageIndex);

		if (dtArea.Rows.Count > 0)
			lblCount.Text = dtArea.Rows[0][CS.TotalRecord].ToString();
		else
			lblCount.Text = "0";

		divPaging.Visible = (dtArea.Rows.Count > 0);

		txtGotoPageNo.Text = PageIndex.ToString();

		ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);
		grdArea.DataSource = dtArea;
		grdArea.DataBind();

		try { grdArea.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}

	private void CheckVisibleButton()
	{
		var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageArea);

		lnkAdd.Visible = lnkEdit.Visible = lnkExcelImport.Visible = objAuthority.IsAddEdit;
		lnkDelete.Visible = objAuthority.IsDelete;

		lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
		lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));

		if (!objAuthority.IsAddEdit)
			grdArea.Attributes.Add("class", grdArea.Attributes["class"].ToString().Replace("rowloader", ""));
	}


	protected void btnSave_OnClick(object sender, EventArgs e)
	{
		LoadAreaGrid(ePageIndex.Custom);
		popupArea.Hide();
	}

	protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
	{
		LoadAreaGrid(ePageIndex.Custom);
		lnkAdd_OnClick(null, null);
	}


	protected void btnSaveCity_OnClick(object sender, EventArgs e)
	{
		LoadAreaGrid(ePageIndex.Custom);
		popupCity.Hide();
	}

	protected void btnSaveState_OnClick(object sender, EventArgs e)
	{
		LoadAreaGrid(ePageIndex.Custom);
		popupState.Hide();
	}

	protected void btnSaveCountry_OnClick(object sender, EventArgs e)
	{
		LoadAreaGrid(ePageIndex.Custom);
		popupCountry.Hide();
	}


	protected void lnkAdd_OnClick(object sender, EventArgs e)
	{
		lblAreaId.Text = string.Empty;
		popupManageArea.SetAreaId = lblAreaId.Text;
		popupManageArea.LoadAreaDetail(false);
		popupArea.Show();
	}

	protected void lnkEdit_OnClick(object sender, EventArgs e)
	{
		if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageArea).IsAddEdit && (sender == null || grdArea.zIsValidSelection(lblAreaId, "chkSelect", CS.AreaId)))
		{
			popupManageArea.SetAreaId = lblAreaId.Text;
			popupManageArea.LoadAreaDetail(false);
			popupArea.Show();
		}
	}


	protected void lnkEditArea_OnClick(object sender, EventArgs e)
	{
		lblAreaId.Text = ((LinkButton)sender).CommandArgument.ToString();
		lnkEdit_OnClick(null, null);
	}

	protected void lnkEditCity_OnClick(object sender, EventArgs e)
	{
		popupManageCity.SetCityId = ((LinkButton)sender).CommandArgument.ToString();
		popupManageCity.LoadCityDetail(true);
		popupCity.Show();
	}

	protected void lnkEditState_OnClick(object sender, EventArgs e)
	{
		poupManageState.SetStateId = ((LinkButton)sender).CommandArgument.ToString();
		poupManageState.LoadStateDetail(true);
		popupState.Show();
	}

	protected void lnkEditCountry_OnClick(object sender, EventArgs e)
	{
		popupManageCountry.SetCountryId = ((LinkButton)sender).CommandArgument.ToString();
		popupManageCountry.LoadCountryDetail(true);
		popupCountry.Show();
	}


	protected void lnkRefresh_OnClick(object sender, EventArgs e)
	{
		LoadAreaGrid(ePageIndex.Custom);
	}

	protected void lnkActive_OnClick(object sender, EventArgs e)
	{
		if (grdArea.zIsValidSelection(lblAreaId, "chkSelect", CS.AreaId))
		{
			if (new Area()
			{
				AreaId = lblAreaId.zToInt(),
				eStatus = (int)eStatus.Active
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This Area is already Active.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Active, "Active Area", "Are You Sure To Active Area?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDeactive_OnClick(object sender, EventArgs e)
	{
		if (grdArea.zIsValidSelection(lblAreaId, "chkSelect", CS.AreaId))
		{
			if (new Area()
			{
				AreaId = lblAreaId.zToInt(),
				eStatus = (int)eStatus.Deactive
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This Area is already Deactive.");
				return;
			}

			string Message = string.Empty;
			if (CU.IsAreaUsed(lblAreaId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Deactive.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive Area", "Are You Sure To Deactive Area?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDelete_OnClick(object sender, EventArgs e)
	{
		if (grdArea.zIsValidSelection(lblAreaId, "chkSelect", CS.AreaId))
		{
			string Message = string.Empty;
			if (CU.IsAreaUsed(lblAreaId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Delete.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Area", "Are You Sure To Delete Area?");
			popupConfirmation.Show();
		}
	}


	private void ManageAreaStatus(eStatus Status)
	{
		new Area()
		{
			AreaId = lblAreaId.zToInt(),
			eStatus = (int)Status
		}.Update();

		LoadAreaGrid(ePageIndex.Custom);
	}

	protected void btnActive_OnClick(object sender, EventArgs e)
	{
		ManageAreaStatus(eStatus.Active);
		CU.ZMessage(eMsgType.Success, string.Empty, "Area Activated Successfully.");
	}

	protected void btnDeactive_OnClick(object sender, EventArgs e)
	{
		ManageAreaStatus(eStatus.Deactive);
		CU.ZMessage(eMsgType.Success, string.Empty, "Area Deactive Successfully.");
	}

	protected void btnDelete_OnClick(object sender, EventArgs e)
	{
		ManageAreaStatus(eStatus.Delete);
		CU.ZMessage(eMsgType.Success, string.Empty, "Area Delete Successfully.");
	}



	protected void Control_CheckedChanged(object sender, EventArgs e)
	{
		LoadAreaGrid(ePageIndex.Custom);
		CheckVisibleButton();
	}

	protected void lnkRefreshCountry_OnClick(object sender, EventArgs e)
	{
		LoadSearchCountry();
	}

	protected void lnkRefreshState_OnClick(object sender, EventArgs e)
	{
		LoadSearchState(null);
	}

	protected void lnkRefreshCity_OnClick(object sender, EventArgs e)
	{
		LoadSearchCity();
	}



	protected void ddlSearchCountry_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		LoadSearchState(null);
	}

	protected void ddlSearchState_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		LoadSearchCity();
	}


	protected void grdArea_OnRowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			if (!IsAddEdit.HasValue)
				IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageArea).IsAddEdit;

			if (!IsAddEditCity.HasValue)
				IsAddEditCity = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCity).IsAddEdit;

			if (!IsAddEditState.HasValue)
				IsAddEditState = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageState).IsAddEdit;

			if (!IsAddEditCountry.HasValue)
				IsAddEditCountry = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCountry).IsAddEdit;

			if (IsAddEdit.Value)
				e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdArea, "Select$" + e.Row.RowIndex);

			if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdArea, CS.eStatus)].Text) != (int)eStatus.Active)
				e.Row.Attributes["class"] = "GridDesableRow ";

			#region Country

			DataRowView dataItem = (DataRowView)e.Row.DataItem;
			var lnkEditCountry = e.Row.FindControl("lnkEditCountry") as LinkButton;
			var ltrCountry = e.Row.FindControl("ltrCountry") as Literal;

			lnkEditCountry.Visible = IsAddEditCountry.Value;
			ltrCountry.Visible = !IsAddEditCountry.Value;

			lnkEditCountry.Text = ltrCountry.Text = dataItem[CS.CountryName].ToString();
			lnkEditCountry.CommandArgument = dataItem[CS.CountryId].ToString();

			#endregion

			#region State

			var lnkEditState = e.Row.FindControl("lnkEditState") as LinkButton;
			var ltrState = e.Row.FindControl("ltrState") as Literal;

			lnkEditState.Visible = IsAddEditState.Value;
			ltrState.Visible = !IsAddEditState.Value;

			lnkEditState.Text = ltrState.Text = dataItem[CS.StateName].ToString();
			lnkEditState.CommandArgument = dataItem[CS.StateId].ToString();

			#endregion

			#region City

			var lnkEditCity = e.Row.FindControl("lnkEditCity") as LinkButton;
			var ltrCity = e.Row.FindControl("ltrCity") as Literal;

			lnkEditCity.Visible = IsAddEditCity.Value;
			ltrCity.Visible = !IsAddEditCity.Value;

			lnkEditCity.Text = ltrCity.Text = dataItem[CS.CityName].ToString();
			lnkEditCity.CommandArgument = dataItem[CS.CityId].ToString();

			#endregion

			#region Area

			var lnkEditArea = e.Row.FindControl("lnkEditArea") as LinkButton;
			var ltrArea = e.Row.FindControl("ltrArea") as Literal;

			lnkEditArea.Visible = IsAddEdit.Value;
			ltrArea.Visible = !IsAddEdit.Value;

			lnkEditArea.Text = ltrArea.Text = dataItem[CS.AreaName].ToString();
			lnkEditArea.CommandArgument = dataItem[CS.AreaId].ToString();

			#endregion
		}
	}

	protected void grdArea_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		lblAreaId.Text = grdArea.Rows[grdArea.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdArea, CS.AreaId)].Text;
		lnkEdit_OnClick(null, null);
	}

	#region Excel Import / Export

	protected void lnkExcelExport_OnClick(object sender, EventArgs e)
	{
		var dtArea = GetAreaDt(ePageIndex.AllPage);
		var lstColumns = new System.Collections.Generic.List<string>();
		lstColumns.Add("CountryName");
		lstColumns.Add("StateName");
		lstColumns.Add("CityName");
		lstColumns.Add("AreaName");
		lstColumns.Add("Pincode");

		ExcelExport.SetExportData(dtArea, lstColumns, lstColumns, "Area");
		popupExcelExport.Show();
	}


	protected void lnkExcelImport_OnClick(object sender, EventArgs e)
	{
		chkReplace.Checked = false;
		popupExcelImport.Show();
	}

	protected void btnUpload_OnClick(object sender, EventArgs e)
	{
		var dt = new DataTable();
		if (!CU.IsValidExcelFile(fuImportExcel, ref dt, 5, "Area"))
		{
			popupExcelImport.Show();
			return;
		}

		if (CheckData(dt))
			InsertData(dt);
		else
			popupExcelImport.Show();
	}

	private bool CheckData(DataTable dt)
	{
		int TotalCount = 0, SuccessCount = 0, FailCount = 0;
		string Message = string.Empty;

		try
		{
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				TotalCount++;
				bool IsValid = true;
				int CountryId = 0, StateId = 0, CityId = 0;

				string Connecter = " in Record-" + TotalCount.ToString() + ".<br />";

				#region Value Initialization

				CountryName = dt.Rows[i][CountryNameColumn].ToString().Trim();
				StateName = dt.Rows[i][StateNameColumn].ToString().Trim();
				CityName = dt.Rows[i][CityNameColumn].ToString().Trim();
				AreaName = dt.Rows[i][AreaNameColumn].ToString().Trim();

				#endregion

				#region Check Country Name

				if (IsValid)
				{
					if (CountryName.zIsNullOrEmpty())
					{
						Message += CS.Arrow + "Country Name Is Empty" + Connecter;
						IsValid = false;
					}
				}

				if (IsValid)
				{
					var lstCountry = new Country() { CountryName = CountryName.ToLower(), eStatus = (int)eStatus.Active, }.SelectList<Country>();
					if (lstCountry.Count == 0)
					{
						Message += CS.Arrow + "Country is Invalid" + Connecter;
						IsValid = false;
					}
					else
						CountryId = lstCountry[0].CountryId.Value;
				}

				#endregion

				#region Check State Name

				if (IsValid)
				{
					if (StateName.zIsNullOrEmpty())
					{
						Message += CS.Arrow + "State Name Is Empty" + Connecter;
						IsValid = false;
					}
				}

				if (IsValid)
				{
					var lstState = new State() { StateName = StateName.ToLower(), CountryId = CountryId, eStatus = (int)eStatus.Active }.SelectList<State>();
					if (lstState.Count == 0)
					{
						Message += CS.Arrow + "State is Invalid" + Connecter;
						IsValid = false;
					}
					else
						StateId = lstState[0].StateId.Value;
				}

				#endregion

				#region Check City Name

				if (IsValid)
				{
					if (CityName.zIsNullOrEmpty())
					{
						Message += CS.Arrow + "City Name Is Empty" + Connecter;
						IsValid = false;
					}
				}

				if (IsValid)
				{
					var lstCity = new City() { CityName = CityName.ToLower(), StateId = StateId, eStatus = (int)eStatus.Active }.SelectList<City>();
					if (lstCity.Count == 0)
					{
						Message += CS.Arrow + "City is Invalid" + Connecter;
						IsValid = false;
					}
					else
						CityId = lstCity[0].StateId.Value;
				}

				#endregion

				#region Check Area Name

				if (IsValid)
				{
					if (AreaName.zIsNullOrEmpty())
					{
						Message += CS.Arrow + "Area Name Is Empty" + Connecter;
						IsValid = false;
					}
				}

				if (IsValid)
				{
					string RepeateColumn = string.Empty;
					if (CU.IsRepeateExcelRow(dt, i, AreaName, AreaNameColumn, CityName, CityNameColumn, StateName, StateNameColumn, ref RepeateColumn))
					{
						Message += CS.Arrow + "Area " + AreaName + " is Repeating in Record-" + RepeateColumn;
						IsValid = false;
					}
				}

				if (IsValid)
				{
					DataTable dtAreaName = new Query()
					{
						CountryId = CountryId,
						StateId = StateId,
						CityId = CityId,
						AreaName = AreaName,
						eStatusNot = (int)eStatus.Delete
					}.Select(eSP.qry_Area);

					if (dtAreaName.Rows.Count > 0 && !chkReplace.Checked)
					{
						string Status = dtAreaName.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
						Message += CS.Arrow + AreaName + " Area is already exist" + Status + "." + Connecter;
						IsValid = false;
					}
				}

				#endregion

				#region Check Pincode

				if (!Pincode.zIsNullOrEmpty())
				{
					if (IsValid)
					{
						string RepeateColumn = string.Empty;
						if (CU.IsRepeateExcelRow(dt, i, Pincode, PincodeColumn, string.Empty, null, string.Empty, null, ref RepeateColumn))
						{
							Message += CS.Arrow + "Pincode " + Pincode + " is Repeating in Record-" + RepeateColumn;
							IsValid = false;
						}
					}

					if (IsValid)
					{
						DataTable dtAreaPincode = new Query()
						{
							Pincode = Pincode,
							eStatusNot = (int)eStatus.Delete
						}.Select(eSP.qry_Area);

						if (dtAreaPincode.Rows.Count > 0 && !chkReplace.Checked)
						{
							string Status = dtAreaPincode.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
							Message += CS.Arrow + Pincode + " Pincode is already exist" + Status + "." + Connecter;
							IsValid = false;
						}
					}
				}

				#endregion

				if (IsValid)
					SuccessCount++;
				else
				{
					FailCount++;
					if (FailCount >= 10)
						break;
				}
			}
		}
		catch (Exception ex)
		{
			CU.ZMessage(eMsgType.Error, string.Empty, ex.Message, 0);
			return false;
		}

		if (FailCount == 0)
			return true;
		else
		{
			CU.SetErrorExcelMessage(Message, SuccessCount, FailCount);
			return false;
		}
	}

	private void InsertData(DataTable dt)
	{
		int UpdateCount = 0, InsertCount = 0;

		try
		{
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				#region Value Initialization

				CountryName = dt.Rows[i][CountryNameColumn].ToString().Trim();
				StateName = dt.Rows[i][StateNameColumn].ToString().Trim();
				CityName = dt.Rows[i][CityNameColumn].ToString().Trim();
				AreaName = dt.Rows[i][AreaNameColumn].ToString().Trim();
				Pincode = dt.Rows[i][PincodeColumn].ToString().Trim();

				#endregion

				int CountryId = new Country() { eStatus = (int)eStatus.Active, CountryName = CountryName.ToLower() }.SelectList<Country>()[0].CountryId.Value;
				int StateId = new State() { CountryId = CountryId, StateName = StateName, eStatus = (int)eStatus.Active }.SelectList<State>()[0].StateId.Value;
				int CityId = new City() { StateId = StateId, CityName = CityName, eStatus = (int)eStatus.Active }.SelectList<City>()[0].CityId.Value;

				DataTable dtArea = new Query()
				{
					StateId = StateId,
					AreaName = AreaName.zFirstCharToUpper(),
					eStatusNot = (int)eStatus.Delete
				}.Select(eSP.qry_Area);

				var objArea = new Area()
				{
					AreaId = dtArea.Rows.Count > 0 ? dtArea.Rows[0][CS.AreaId].zToInt() : (int?)null,
					CityId = CityId,
					AreaName = AreaName.zFirstCharToUpper(),
					Pincode = Pincode,
				};

				if (objArea.AreaId.HasValue)
				{
					objArea.Update();
					UpdateCount++;
				}
				else
				{
					objArea.eStatus = (int)eStatus.Active;
					objArea.Insert();
					InsertCount++;
				}
			}

			CU.SetSuccessExcelMessage(InsertCount, UpdateCount, "Area");
		}
		catch (Exception ex)
		{
			CU.ZMessage(eMsgType.Error, string.Empty, ex.Message, 0);
		}

		LoadAreaGrid(ePageIndex.Custom);
	}

	#endregion


	#region Pagging

	protected void lnkPrev_Click(object sender, EventArgs e)
	{
		LoadAreaGrid(ePageIndex.Prev);
	}

	protected void lnkNext_Click(object sender, EventArgs e)
	{
		LoadAreaGrid(ePageIndex.Next);
	}

	protected void lnkFirst_Click(object sender, EventArgs e)
	{
		LoadAreaGrid(ePageIndex.First);
	}

	protected void lnkLast_Click(object sender, EventArgs e)
	{
		LoadAreaGrid(ePageIndex.Last);
	}

	protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
	{
		if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
		{
			txtGotoPageNo.Text = "1";
			txtGotoPageNo.Focus();
		}
		LoadAreaGrid(ePageIndex.Custom);
	}

	protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
	{
		txtGotoPageNo.Text = "1";
		LoadAreaGrid(ePageIndex.Custom);
		Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
	}

	#endregion
}
