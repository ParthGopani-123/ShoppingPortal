using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;

public partial class ManageCity : CompressorPage
{
	string CountryName, StateName, CityName;
	int CountryNameColumn = 0, StateNameColumn = 1, CityNameColumn = 2;

	bool? IsAddEdit, IsAddEditState, IsAddEditCountry;


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
			LoadCityGrid(ePageIndex.Custom);
			CheckVisibleButton();
		}

		Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
		Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
		Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

		popupManageCity.btnSave_OnClick += new EventHandler(btnSave_OnClick);
		popupManageCity.btnSaveAndNew_OnClick += new EventHandler(btnSaveAndNew_OnClick);

		poupManageState.btnSave_OnClick += new EventHandler(btnSaveState_OnClick);
		popupManageCountry.btnSave_OnClick += new EventHandler(btnSaveCountry_OnClick);

		try { grdCity.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}


	private void LoadSearchCountry()
	{
		int? CountryId = ddlSearchCountry.zToInt();

		CU.FillDropdown(ref ddlSearchCountry, new Country() { eStatus = (int)eStatus.Active }.Select(), "-- All Country --", CS.CountryId, CS.CountryName);

		try { ddlSearchCountry.SelectedValue = CountryId.ToString(); }
		catch { }

		if (!IsPostBack && !string.IsNullOrEmpty(Request.QueryString[CS.StateId.Encrypt()]))
		{
			string StateId = Request.QueryString[CS.StateId.Encrypt()].ToString().Decrypt();
			if (StateId.zIsInteger(false))
			{
				var lstState = new State() { StateId = StateId.zToInt() }.SelectList<State>();
				if (lstState.Count > 0)
				{
					ddlSearchCountry.SelectedValue = lstState[0].CountryId.ToString();
				}
			}
		}

		LoadSearchState();
	}

	private void LoadSearchState()
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

		if (!IsPostBack && !string.IsNullOrEmpty(Request.QueryString[CS.StateId.Encrypt()]))
		{
			try
			{
				ddlSearchState.SelectedValue = Request.QueryString[CS.StateId.Encrypt()].ToString().Decrypt();
			}
			catch { }
		}
	}

	private DataTable GetCityDt(ePageIndex ePageIndex)
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
			eStatus = Status,
			eStatusNot = (int)eStatus.Delete,
		};

		#region Page Index

		int RecordPerPage = ddlRecordPerPage.zToInt().Value;
		int PageIndexTemp = PageIndex;

		CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref objQuery, ref txtGotoPageNo, ref lblCount);
		PageIndex = PageIndexTemp;

		#endregion Page Index

		return objQuery.Select(eSP.qry_City);
	}

	private void LoadCityGrid(ePageIndex ePageIndex)
	{
		DataTable dtCity = GetCityDt(ePageIndex);

		if (dtCity.Rows.Count > 0)
			lblCount.Text = dtCity.Rows[0][CS.TotalRecord].ToString();
		else
			lblCount.Text = "0";

		divPaging.Visible = (dtCity.Rows.Count > 0);

		txtGotoPageNo.Text = PageIndex.ToString();

		ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);
		grdCity.DataSource = dtCity;
		grdCity.DataBind();

		try { grdCity.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}

	private void CheckVisibleButton()
	{
		var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCity);

		lnkAdd.Visible = lnkEdit.Visible = lnkExcelImport.Visible = objAuthority.IsAddEdit;
		lnkDelete.Visible = objAuthority.IsDelete;

		lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
		lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));

		if (!objAuthority.IsAddEdit)
			grdCity.Attributes.Add("class", grdCity.Attributes["class"].ToString().Replace("rowloader", ""));
	}


	protected void btnSave_OnClick(object sender, EventArgs e)
	{
		LoadCityGrid(ePageIndex.Custom);
		popupCity.Hide();
	}

	protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
	{
		LoadCityGrid(ePageIndex.Custom);
		lnkAdd_OnClick(null, null);
	}


	protected void btnSaveState_OnClick(object sender, EventArgs e)
	{
		LoadCityGrid(ePageIndex.Custom);
		popupState.Hide();
	}

	protected void btnSaveCountry_OnClick(object sender, EventArgs e)
	{
		LoadCityGrid(ePageIndex.Custom);
		popupCountry.Hide();
	}


	protected void lnkAdd_OnClick(object sender, EventArgs e)
	{
		lblCityId.Text = string.Empty;
		popupManageCity.SetCityId = lblCityId.Text;
		popupManageCity.LoadCityDetail(false);
		popupCity.Show();
	}

	protected void lnkEdit_OnClick(object sender, EventArgs e)
	{
		if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCity).IsAddEdit && (sender == null || grdCity.zIsValidSelection(lblCityId, "chkSelect", CS.CityId)))
		{
			popupManageCity.SetCityId = lblCityId.Text;
			popupManageCity.LoadCityDetail(false);
			popupCity.Show();
		}
	}


	protected void lnkEditCity_OnClick(object sender, EventArgs e)
	{
		lblCityId.Text = ((LinkButton)sender).CommandArgument.ToString();
		lnkEdit_OnClick(null, null);
	}

	protected void lnkEditState_OnClick(object sender, EventArgs e)
	{
		if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageState).IsAddEdit)
		{
			poupManageState.SetStateId = ((LinkButton)sender).CommandArgument.ToString();
			poupManageState.LoadStateDetail(true);
			popupState.Show();
		}
	}

	protected void lnkEditCountry_OnClick(object sender, EventArgs e)
	{
		if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCountry).IsAddEdit)
		{
			popupManageCountry.SetCountryId = ((LinkButton)sender).CommandArgument.ToString();
			popupManageCountry.LoadCountryDetail(true);
			popupCountry.Show();
		}
	}


	protected void lnkRefresh_OnClick(object sender, EventArgs e)
	{
		LoadCityGrid(ePageIndex.Custom);
	}

	protected void lnkActive_OnClick(object sender, EventArgs e)
	{
		if (grdCity.zIsValidSelection(lblCityId, "chkSelect", CS.CityId))
		{
			if (new City()
			{
				CityId = lblCityId.zToInt(),
				eStatus = (int)eStatus.Active
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This City is already Active.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Active, "Active City", "Are You Sure To Active City?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDeactive_OnClick(object sender, EventArgs e)
	{
		if (grdCity.zIsValidSelection(lblCityId, "chkSelect", CS.CityId))
		{
			if (new City()
			{
				CityId = lblCityId.zToInt(),
				eStatus = (int)eStatus.Deactive
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This City is already Deactive.");
				return;
			}

			string Message = string.Empty;
			if (CU.IsCityUsed(lblCityId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Deactive.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive City", "Are You Sure To Deactive City?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDelete_OnClick(object sender, EventArgs e)
	{
		if (grdCity.zIsValidSelection(lblCityId, "chkSelect", CS.CityId))
		{
			string Message = string.Empty;
			if (CU.IsCityUsed(lblCityId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Delete.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete City", "Are You Sure To Delete City?");
			popupConfirmation.Show();
		}
	}


	private void ManageCityStatus(eStatus Status)
	{
		new City()
		{
			CityId = lblCityId.zToInt(),
			eStatus = (int)Status
		}.Update();

		LoadCityGrid(ePageIndex.Custom);
	}

	protected void btnActive_OnClick(object sender, EventArgs e)
	{
		ManageCityStatus(eStatus.Active);
		CU.ZMessage(eMsgType.Success, string.Empty, "City Activated Successfully.");
	}

	protected void btnDeactive_OnClick(object sender, EventArgs e)
	{
		ManageCityStatus(eStatus.Deactive);
		CU.ZMessage(eMsgType.Success, string.Empty, "City Deactive Successfully.");
	}

	protected void btnDelete_OnClick(object sender, EventArgs e)
	{
		ManageCityStatus(eStatus.Delete);
		CU.ZMessage(eMsgType.Success, string.Empty, "City Delete Successfully.");
	}



	protected void Control_CheckedChanged(object sender, EventArgs e)
	{
		LoadCityGrid(ePageIndex.Custom);
		CheckVisibleButton();
	}

	protected void lnkRefreshCountry_OnClick(object sender, EventArgs e)
	{
		LoadSearchCountry();
	}

	protected void lnkRefreshState_OnClick(object sender, EventArgs e)
	{
		LoadSearchState();
	}



	protected void ddlSearchCountry_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		LoadSearchState();
	}


	protected void grdCity_OnRowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			if (!IsAddEdit.HasValue)
				IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCity).IsAddEdit;

			if (!IsAddEditState.HasValue)
				IsAddEditState = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageState).IsAddEdit;

			if (!IsAddEditCountry.HasValue)
				IsAddEditCountry = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCountry).IsAddEdit;

			if (IsAddEdit.Value)
				e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdCity, "Select$" + e.Row.RowIndex);

			if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdCity, CS.eStatus)].Text) != (int)eStatus.Active)
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

			lnkEditCity.Visible = IsAddEdit.Value;
			ltrCity.Visible = !IsAddEdit.Value;

			lnkEditCity.Text = ltrCity.Text = dataItem[CS.CityName].ToString();
			lnkEditCity.CommandArgument = dataItem[CS.CityId].ToString();

			#endregion
		}
	}

	protected void grdCity_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		lblCityId.Text = grdCity.Rows[grdCity.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdCity, CS.CityId)].Text;
		if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageArea).IsView)
			Response.Redirect("ManageArea.aspx?" + CS.CityId.Encrypt() + "=" + lblCityId.Text.Encrypt());
		else
			lnkEdit_OnClick(null, null);
	}

	#region Excel Import / Export

	protected void lnkExcelExport_OnClick(object sender, EventArgs e)
	{
		var dtCity = GetCityDt(ePageIndex.AllPage);
		var lstColumns = new System.Collections.Generic.List<string>();
		lstColumns.Add("CountryName");
		lstColumns.Add("StateName");
		lstColumns.Add("CityName");

		ExcelExport.SetExportData(dtCity, lstColumns, lstColumns, "City");
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
		if (!CU.IsValidExcelFile(fuImportExcel, ref dt, 3, "City"))
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
				int CountryId = 0, StateId = 0;

				string Connecter = " in Record-" + TotalCount.ToString() + ".<br />";

				#region Value Initialization

				CountryName = dt.Rows[i][CountryNameColumn].ToString().Trim();
				StateName = dt.Rows[i][StateNameColumn].ToString().Trim();
				CityName = dt.Rows[i][CityNameColumn].ToString().Trim();

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
					string RepeateColumn = string.Empty;
					if (CU.IsRepeateExcelRow(dt, i, CityName, CityNameColumn, CountryName, CountryNameColumn, StateName, StateNameColumn, ref RepeateColumn))
					{
						Message += CS.Arrow + "City " + CityName + " is Repeating in Record-" + RepeateColumn;
						IsValid = false;
					}
				}

				if (IsValid)
				{
					DataTable dtCity = new Query()
					{
						CountryId = CountryId,
						StateId = StateId,
						CityName = CityName,
						eStatusNot = (int)eStatus.Delete
					}.Select(eSP.qry_City);

					if (dtCity.Rows.Count > 0 && !chkReplace.Checked)
					{
						string Status = dtCity.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
						Message += CS.Arrow + CityName + " City is already exist" + Status + "." + Connecter;
						IsValid = false;
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

				#endregion

				int CountryId = new Country() { eStatus = (int)eStatus.Active, CountryName = CountryName.ToLower() }.SelectList<Country>()[0].CountryId.Value;

				int StateId = 0;
				if (!StateName.zIsNullOrEmpty())
					StateId = new State() { CountryId = CountryId, StateName = StateName, eStatus = (int)eStatus.Active }.SelectList<State>()[0].StateId.Value;

				DataTable dtCity = new Query()
				{
					CountryId = CountryId,
					StateId = StateId,
					CityName = CityName.zFirstCharToUpper(),
					eStatusNot = (int)eStatus.Delete
				}.Select(eSP.qry_City);

				var objCity = new City()
				{
					CityId = dtCity.Rows.Count > 0 ? dtCity.Rows[0][CS.CityId].zToInt() : (int?)null,
					StateId = StateId,
					CityName = CityName,
				};

				if (objCity.CityId.HasValue)
				{
					objCity.Update();
					UpdateCount++;
				}
				else
				{
					objCity.eStatus = (int)eStatus.Active;
					objCity.Insert();
					InsertCount++;
				}
			}

			CU.SetSuccessExcelMessage(InsertCount, UpdateCount, "City");
		}
		catch (Exception ex)
		{
			CU.ZMessage(eMsgType.Error, string.Empty, ex.Message, 0);
		}

		LoadCityGrid(ePageIndex.Custom);
	}

	#endregion


	#region Pagging

	protected void lnkPrev_Click(object sender, EventArgs e)
	{
		LoadCityGrid(ePageIndex.Prev);
	}

	protected void lnkNext_Click(object sender, EventArgs e)
	{
		LoadCityGrid(ePageIndex.Next);
	}

	protected void lnkFirst_Click(object sender, EventArgs e)
	{
		LoadCityGrid(ePageIndex.First);
	}

	protected void lnkLast_Click(object sender, EventArgs e)
	{
		LoadCityGrid(ePageIndex.Last);
	}

	protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
	{
		if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
		{
			txtGotoPageNo.Text = "1";
			txtGotoPageNo.Focus();
		}
		LoadCityGrid(ePageIndex.Custom);
	}

	protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
	{
		txtGotoPageNo.Text = "1";
		LoadCityGrid(ePageIndex.Custom);
		Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
	}

	#endregion
}
