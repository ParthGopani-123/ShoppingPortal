using System;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;

public partial class ManageState : CompressorPage
{
	string CountryName, StateName, Description;
	int CountryNameColumn = 0, StateNameColumn = 1, DescriptionColumn = 2;

	bool? IsAddEdit, IsAddEditCountry, IsViewDestination;

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

			if (!string.IsNullOrEmpty(Request.QueryString[CS.CountryId.Encrypt()]))
			{
				try { ddlSearchCountry.SelectedValue = Request.QueryString[CS.CountryId.Encrypt()].ToString().Decrypt(); }
				catch { }
			}

			LoadStateGrid(ePageIndex.Custom);
			CheckVisibleButton();
		}

		Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
		Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
		Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

		poupManageState.btnSave_OnClick += new EventHandler(btnSave_OnClick);
		poupManageState.btnSaveAndNew_OnClick += new EventHandler(btnSaveAndNew_OnClick);

		popupManageCountry.btnSave_OnClick += new EventHandler(btnSaveCountry_OnClick);

		try { grdState.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }

	}

	private void LoadSearchCountry()
	{
		int? CountryId = ddlSearchCountry.zToInt();

		CU.FillDropdown(ref ddlSearchCountry, new Country() { eStatus = (int)eStatus.Active }.Select(), "-- All Country --", CS.CountryId, CS.CountryName);

		try { ddlSearchCountry.SelectedValue = CountryId.ToString(); }
		catch { }
	}

	private DataTable GetStateDt(ePageIndex ePageIndex)
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
			eStatus = Status,
			eStatusNot = (int)eStatus.Delete,
		};

		#region Page Index

		int RecordPerPage = ddlRecordPerPage.zToInt().Value;
		int PageIndexTemp = PageIndex;

		CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref objQuery, ref txtGotoPageNo, ref lblCount);
		PageIndex = PageIndexTemp;

		#endregion Page Index

		return objQuery.Select(eSP.qry_State);
	}

	private void LoadStateGrid(ePageIndex ePageIndex)
	{
		DataTable dtState = GetStateDt(ePageIndex);

		if (dtState.Rows.Count > 0)
			lblCount.Text = dtState.Rows[0][CS.TotalRecord].ToString();
		else
			lblCount.Text = "0";

		divPaging.Visible = (dtState.Rows.Count > 0);

		txtGotoPageNo.Text = PageIndex.ToString();

		ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

		grdState.DataSource = dtState;
		grdState.DataBind();

		try { grdState.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}

	private void CheckVisibleButton()
	{
		var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageState);

		lnkAdd.Visible = lnkEdit.Visible = lnkExcelImport.Visible = objAuthority.IsAddEdit;
		lnkDelete.Visible = objAuthority.IsDelete;

		lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
		lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));

		if (!objAuthority.IsAddEdit && !CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCity).IsView)
			grdState.Attributes.Add("class", grdState.Attributes["class"].ToString().Replace("rowloader", ""));
	}


	protected void btnSave_OnClick(object sender, EventArgs e)
	{
		LoadStateGrid(ePageIndex.Custom);
		popupState.Hide();
	}

	protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
	{
		LoadStateGrid(ePageIndex.Custom);
		lnkAdd_OnClick(null, null);
	}


	protected void btnSaveCountry_OnClick(object sender, EventArgs e)
	{
		LoadStateGrid(ePageIndex.Custom);
		popupCountry.Hide();
	}


	protected void lnkAdd_OnClick(object sender, EventArgs e)
	{
		lblStateId.Text = string.Empty;
		poupManageState.SetStateId = lblStateId.Text;
		poupManageState.LoadStateDetail(false);
		popupState.Show();
	}

	protected void lnkEdit_OnClick(object sender, EventArgs e)
	{
		if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageState).IsAddEdit && (sender == null || grdState.zIsValidSelection(lblStateId, "chkSelect", CS.StateId)))
		{
			poupManageState.SetStateId = lblStateId.Text;
			poupManageState.LoadStateDetail(false);
			popupState.Show();
		}
	}

	protected void lnkEditState_OnClick(object sender, EventArgs e)
	{
		lblStateId.Text = ((LinkButton)sender).CommandArgument.ToString();
		lnkEdit_OnClick(null, null);
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
		LoadStateGrid(ePageIndex.Custom);
	}

	protected void lnkActive_OnClick(object sender, EventArgs e)
	{
		if (grdState.zIsValidSelection(lblStateId, "chkSelect", CS.StateId))
		{
			if (new State()
			{
				StateId = lblStateId.zToInt(),
				eStatus = (int)eStatus.Active
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This State is already Active.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Active, "Active State", "Are You Sure To Active State?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDeactive_OnClick(object sender, EventArgs e)
	{
		if (grdState.zIsValidSelection(lblStateId, "chkSelect", CS.StateId))
		{
			if (new State()
			{
				StateId = lblStateId.zToInt(),
				eStatus = (int)eStatus.Deactive
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This State is already Deactive.");
				return;
			}

			string Message = string.Empty;
			if (CU.IsStateUsed(lblStateId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Deactive.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive State", "Are You Sure To Deactive State?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDelete_OnClick(object sender, EventArgs e)
	{
		if (grdState.zIsValidSelection(lblStateId, "chkSelect", CS.StateId))
		{
			string Message = string.Empty;
			if (CU.IsStateUsed(lblStateId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Delete.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete State", "Are You Sure To Delete State?");
			popupConfirmation.Show();
		}
	}


	private void ManageStateStatus(eStatus Status)
	{
		new State()
		{
			StateId = lblStateId.zToInt(),
			eStatus = (int)Status
		}.Update();

		LoadStateGrid(ePageIndex.Custom);
	}

	protected void btnActive_OnClick(object sender, EventArgs e)
	{
		ManageStateStatus(eStatus.Active);
		CU.ZMessage(eMsgType.Success, string.Empty, "State Activated Successfully.");
	}

	protected void btnDeactive_OnClick(object sender, EventArgs e)
	{
		ManageStateStatus(eStatus.Deactive);
		CU.ZMessage(eMsgType.Success, string.Empty, "State Deactive Successfully.");
	}

	protected void btnDelete_OnClick(object sender, EventArgs e)
	{
		ManageStateStatus(eStatus.Delete);
		CU.ZMessage(eMsgType.Success, string.Empty, "State Delete Successfully.");
	}



	protected void Control_CheckedChanged(object sender, EventArgs e)
	{
		LoadStateGrid(ePageIndex.Custom);
		CheckVisibleButton();
	}

	protected void lnkRefreshCountry_OnClick(object sender, EventArgs e)
	{
		LoadSearchCountry();
	}

	protected void grdState_OnRowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			if (!IsAddEdit.HasValue)
				IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageState).IsAddEdit;

			if (!IsViewDestination.HasValue)
				IsViewDestination = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCity).IsView;

			if (!IsAddEditCountry.HasValue)
				IsAddEditCountry = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCountry).IsAddEdit;

			if (IsAddEdit.Value || IsViewDestination.Value)
				e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdState, "Select$" + e.Row.RowIndex);

			if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdState, CS.eStatus)].Text) != (int)eStatus.Active)
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

			lnkEditState.Visible = IsAddEdit.Value;
			ltrState.Visible = !IsAddEdit.Value;

			lnkEditState.Text = ltrState.Text = dataItem[CS.StateName].ToString();
			lnkEditState.CommandArgument = dataItem[CS.StateId].ToString();

			#endregion
		}
	}

	protected void grdState_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		lblStateId.Text = grdState.Rows[grdState.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdState, CS.StateId)].Text;
		if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCity).IsView)
			Response.Redirect("ManageCity.aspx?" + CS.StateId.Encrypt() + "=" + lblStateId.Text.Encrypt());
		else
			lnkEdit_OnClick(null, null);
	}


	#region Excel Import / Export

	protected void lnkExcelExport_OnClick(object sender, EventArgs e)
	{
		var dtState = GetStateDt(ePageIndex.AllPage);
		var lstColumns = new System.Collections.Generic.List<string>();
		lstColumns.Add("CountryName");
		lstColumns.Add("StateName");
		lstColumns.Add("Description");

		ExcelExport.SetExportData(dtState, lstColumns, lstColumns, "State");
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
		if (!CU.IsValidExcelFile(fuImportExcel, ref dt, 3, "State"))
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
				int CountryId = 0;

				string Connecter = " in Record-" + TotalCount.ToString() + ".<br />";

				#region Value Initialization

				CountryName = dt.Rows[i][CountryNameColumn].ToString().Trim();
				StateName = dt.Rows[i][StateNameColumn].ToString().Trim();
				Description = dt.Rows[i][DescriptionColumn].ToString().Trim();

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
					var lstCountry = new Country() { CountryName = CountryName.ToLower(), eStatus = (int)eStatus.Active }.SelectList<Country>();
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
					string RepeateColumn = string.Empty;
					if (CU.IsRepeateExcelRow(dt, i, StateName, StateNameColumn, CountryName, CountryNameColumn, string.Empty, null, ref RepeateColumn))
					{
						Message += CS.Arrow + "State " + StateName + " is Repeating in Record-" + RepeateColumn;
						IsValid = false;
					}
				}

				if (IsValid)
				{
					DataTable dtState = new Query()
					{
						CountryId = CountryId,
						StateName = StateName,
						eStatusNot = (int)eStatus.Delete
					}.Select(eSP.qry_State);

					if (dtState.Rows.Count > 0 && !chkReplace.Checked)
					{
						string Status = dtState.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
						Message += CS.Arrow + "This State is already exist" + Status + "." + Connecter;
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
				Description = dt.Rows[i][DescriptionColumn].ToString().Trim();

				#endregion

				var objCountry = new Country() { CountryName = CountryName.ToLower(), eStatus = (int)eStatus.Active }.SelectList<Country>()[0];

				DataTable dtState = new Query()
				{
					CountryId = objCountry.CountryId,
					StateName = StateName,
					eStatusNot = (int)eStatus.Delete
				}.Select(eSP.qry_State);

				var objState = new State()
				{
					CountryId = objCountry.CountryId,
					StateId = dtState.Rows.Count > 0 ? dtState.Rows[0][CS.StateId].zToInt() : (int?)null,
					StateName = StateName.zFirstCharToUpper(),
					Description = Description,
				};

				if (objState.StateId.HasValue)
				{
					objState.Update();
					UpdateCount++;
				}
				else
				{
					objState.eStatus = (int)eStatus.Active;
					objState.Insert();
					InsertCount++;
				}
			}

			CU.SetSuccessExcelMessage(InsertCount, UpdateCount, "State");
		}
		catch (Exception ex)
		{
			CU.ZMessage(eMsgType.Error, string.Empty, ex.Message, 0);
		}

		LoadStateGrid(ePageIndex.Custom);
	}

	#endregion


	#region Pagging


	protected void lnkPrev_Click(object sender, EventArgs e)
	{
		LoadStateGrid(ePageIndex.Prev);
	}

	protected void lnkNext_Click(object sender, EventArgs e)
	{
		LoadStateGrid(ePageIndex.Next);
	}

	protected void lnkFirst_Click(object sender, EventArgs e)
	{
		LoadStateGrid(ePageIndex.First);
	}

	protected void lnkLast_Click(object sender, EventArgs e)
	{
		LoadStateGrid(ePageIndex.Last);
	}

	protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
	{
		if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
		{
			txtGotoPageNo.Text = "1";
			txtGotoPageNo.Focus();
		}
		LoadStateGrid(ePageIndex.Custom);
	}

	protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
	{
		txtGotoPageNo.Text = "1";
		LoadStateGrid(ePageIndex.Custom);
		Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
	}


	#endregion
}
