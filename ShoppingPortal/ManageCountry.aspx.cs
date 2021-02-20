using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;

public partial class ManageCountry : CompressorPage
{
	string CountryName, Description;
	int CountryNameColumn = 0, DescriptionColumn = 1;

	bool? IsAddEdit, IsViewState;

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

			LoadCountryGrid(ePageIndex.Custom);
			CheckVisibleButton();
		}

		Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
		Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
		Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

		popupManageCountry.btnSave_OnClick += new EventHandler(btnSave_OnClick);
		popupManageCountry.btnSaveAndNew_OnClick += new EventHandler(btnSaveAndNew_OnClick);

		try { grdCountry.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}

	private DataTable GetCountryDt(ePageIndex ePageIndex)
	{
		int? Status = null;
		if (chkActive.Checked && !chkDeactive.Checked)
			Status = (int)eStatus.Active;
		else if (!chkActive.Checked && chkDeactive.Checked)
			Status = (int)eStatus.Deactive;

		var objQuery = new Query()
		{
			MasterSearch = txtSearch.Text,
			eStatus = Status,
			eStatusNot = (int)eStatus.Delete,
		};

		#region Page Index

		int RecordPerPage = ddlRecordPerPage.zToInt().Value;
		int PageIndexTemp = PageIndex;

		CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref objQuery, ref txtGotoPageNo, ref lblCount);
		PageIndex = PageIndexTemp;

		#endregion Page Index

		return objQuery.Select(eSP.qry_Country);
	}

	private void LoadCountryGrid(ePageIndex ePageIndex)
	{
		DataTable dtCountry = GetCountryDt(ePageIndex);

		if (dtCountry.Rows.Count > 0)
			lblCount.Text = dtCountry.Rows[0][CS.TotalRecord].ToString();
		else
			lblCount.Text = "0";

		divPaging.Visible = (dtCountry.Rows.Count > 0);

		txtGotoPageNo.Text = PageIndex.ToString();

		ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

		grdCountry.DataSource = dtCountry;
		grdCountry.DataBind();

		try { grdCountry.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}

	private void CheckVisibleButton()
	{
		var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCountry);

		lnkAdd.Visible = lnkEdit.Visible = lnkExcelImport.Visible = objAuthority.IsAddEdit;
		lnkDelete.Visible = objAuthority.IsDelete;

		lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
		lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));

		if (!objAuthority.IsAddEdit && !CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageState).IsView)
			grdCountry.Attributes.Add("class", grdCountry.Attributes["class"].ToString().Replace("rowloader", ""));
	}


	protected void btnSave_OnClick(object sender, EventArgs e)
	{
		LoadCountryGrid(ePageIndex.Custom);
		popupCountry.Hide();
	}

	protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
	{
		LoadCountryGrid(ePageIndex.Custom);
		lnkAdd_OnClick(null, null);
	}


	protected void lnkAdd_OnClick(object sender, EventArgs e)
	{
		lblCountryId.Text = string.Empty;
		popupManageCountry.SetCountryId = lblCountryId.Text;
		popupManageCountry.LoadCountryDetail(false);
		popupCountry.Show();
	}

	protected void lnkEdit_OnClick(object sender, EventArgs e)
	{
		if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCountry).IsAddEdit && (sender == null || grdCountry.zIsValidSelection(lblCountryId, "chkSelect", CS.CountryId)))
		{
			popupManageCountry.SetCountryId = lblCountryId.Text;
			popupManageCountry.LoadCountryDetail(false);
			popupCountry.Show();
		}
	}

	protected void lnkEditCountry_OnClick(object sender, EventArgs e)
	{
		lblCountryId.Text = ((LinkButton)sender).CommandArgument.ToString();
		lnkEdit_OnClick(null, null);
	}

	protected void lnkRefresh_OnClick(object sender, EventArgs e)
	{
		LoadCountryGrid(ePageIndex.Custom);
	}

	protected void lnkActive_OnClick(object sender, EventArgs e)
	{
		if (grdCountry.zIsValidSelection(lblCountryId, "chkSelect", CS.CountryId))
		{
			if (new Country()
			{
				CountryId = lblCountryId.zToInt(),
				eStatus = (int)eStatus.Active
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This Country is already Active.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Active, "Active Country", "Are You Sure To Active Country?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDeactive_OnClick(object sender, EventArgs e)
	{
		if (grdCountry.zIsValidSelection(lblCountryId, "chkSelect", CS.CountryId))
		{
			if (new Country()
			{
				CountryId = lblCountryId.zToInt(),
				eStatus = (int)eStatus.Deactive
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This Country is already Deactive.");
				return;
			}

			string Message = string.Empty;
			if (CU.IsCountryUsed(lblCountryId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Deactive.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive Country", "Are You Sure To Deactive Country?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDelete_OnClick(object sender, EventArgs e)
	{
		if (grdCountry.zIsValidSelection(lblCountryId, "chkSelect", CS.CountryId))
		{
			string Message = string.Empty;
			if (CU.IsCountryUsed(lblCountryId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Delete.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Country", "Are You Sure To Delete Country?");
			popupConfirmation.Show();
		}
	}


	private void ManageCountryStatus(eStatus Status)
	{
		new Country()
		{
			CountryId = lblCountryId.zToInt(),
			eStatus = (int)Status
		}.Update();
	}

	protected void btnActive_OnClick(object sender, EventArgs e)
	{
		ManageCountryStatus(eStatus.Active);
		CU.ZMessage(eMsgType.Success, string.Empty, "Country Activated Successfully.");
		LoadCountryGrid(ePageIndex.Custom);
	}

	protected void btnDeactive_OnClick(object sender, EventArgs e)
	{
		ManageCountryStatus(eStatus.Deactive);
		CU.ZMessage(eMsgType.Success, string.Empty, "Country Deactive Successfully.");
		LoadCountryGrid(ePageIndex.Custom);
	}

	protected void btnDelete_OnClick(object sender, EventArgs e)
	{
		ManageCountryStatus(eStatus.Delete);
		CU.ZMessage(eMsgType.Success, string.Empty, "Country Delete Successfully.");
		LoadCountryGrid(ePageIndex.Custom);
	}


	protected void Control_CheckedChanged(object sender, EventArgs e)
	{
		LoadCountryGrid(ePageIndex.Custom);
		CheckVisibleButton();
	}


	protected void grdCountry_OnRowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			if (!IsViewState.HasValue)
				IsViewState = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageState).IsView;

			if (!IsAddEdit.HasValue)
				IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCountry).IsAddEdit;

			if (IsAddEdit.Value || IsViewState.Value)
				e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdCountry, "Select$" + e.Row.RowIndex);

			if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdCountry, CS.eStatus)].Text) != (int)eStatus.Active)
				e.Row.Attributes["class"] = "GridDesableRow ";

			DataRowView dataItem = (DataRowView)e.Row.DataItem;
			var lnkEditCountry = e.Row.FindControl("lnkEditCountry") as LinkButton;
			var ltrCountry = e.Row.FindControl("ltrCountry") as Literal;

			lnkEditCountry.Visible = IsAddEdit.Value;
			ltrCountry.Visible = !IsAddEdit.Value;

			lnkEditCountry.Text = ltrCountry.Text = dataItem[CS.CountryName].ToString();
			lnkEditCountry.CommandArgument = dataItem[CS.CountryId].ToString();
		}
	}

	protected void grdCountry_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		lblCountryId.Text = grdCountry.Rows[grdCountry.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdCountry, CS.CountryId)].Text;
		if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageState).IsView)
			Response.Redirect("ManageState.aspx?" + CS.CountryId.Encrypt() + "=" + lblCountryId.Text.Encrypt(), true);
		lnkEdit_OnClick(null, null);
	}


	#region Excel Import / Export

	protected void lnkExcelExport_OnClick(object sender, EventArgs e)
	{
		var dtCountry = GetCountryDt(ePageIndex.AllPage);
		var lstColumns = new System.Collections.Generic.List<string>();
		lstColumns.Add("CountryName");
		lstColumns.Add("Description");

		var lstColumnsSelected = new System.Collections.Generic.List<string>();
		lstColumnsSelected.Add("CountryName");
		lstColumnsSelected.Add("Description");

		ExcelExport.SetExportData(dtCountry, lstColumns, lstColumnsSelected, "Country");
		popupExcelExport.Show();
	}


	protected void lnkExcelImport_OnClick(object sender, EventArgs e)
	{
		chkReplace.Checked = false;
		popupExcelImport.Show();
	}

	protected void btnUpload_OnClick(object sender, EventArgs e)
	{
		if (fuImportExcel.HasFile)
		{
			var dt = new DataTable();
			if (!CU.IsValidExcelFile(fuImportExcel, ref dt, 2, "Country"))
			{
				popupExcelImport.Show();
				return;
			}

			if (CheckData(dt))
				InsertData(dt);
			else
				popupExcelImport.Show();
		}
		else
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Excel File to upload.");
		}
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

				string Connecter = " in Record-" + TotalCount.ToString() + ".<br />";

				#region Value Initialization

				CountryName = dt.Rows[i][CountryNameColumn].ToString().Trim().TrimEnd(',');
				Description = dt.Rows[i][DescriptionColumn].ToString().Trim().TrimEnd(',');

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
					string RepeateColumn = string.Empty;
					if (CU.IsRepeateExcelRow(dt, i, CountryName, CountryNameColumn, string.Empty, null, string.Empty, null, ref RepeateColumn))
					{
						Message += CS.Arrow + "Country  " + CountryName + "  is Repeating in Record-" + RepeateColumn;
						IsValid = false;
					}
				}

				if (IsValid)
				{
					DataTable dtCountry = new Query()
					{
						CountryName = CountryName,
						eStatusNot = (int)eStatus.Delete
					}.Select(eSP.qry_Country);

					if (dtCountry.Rows.Count > 0 && !chkReplace.Checked)
					{
						string Status = dtCountry.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
						Message += CS.Arrow + "This Country is already exist" + Status + "." + Connecter;
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

				CountryName = dt.Rows[i][CountryNameColumn].ToString().Trim().TrimEnd(',');
					Description = dt.Rows[i][DescriptionColumn].ToString().Trim().TrimEnd(',');
				
				#endregion

				DataTable dtCountry = new Query()
				{
					CountryName = CountryName,
					eStatusNot = (int)eStatus.Delete
				}.Select(eSP.qry_Country);

				var objCountry = new Country()
				{
					CountryId = dtCountry.Rows.Count > 0 ? dtCountry.Rows[0][CS.CountryId].zToInt() : (int?)null,
					CountryName = CountryName.zFirstCharToUpper(),
					Description = Description,
				};

				if (objCountry.CountryId.HasValue)
				{
					objCountry.Update();
					UpdateCount++;
				}
				else
				{
					objCountry.eStatus = (int)eStatus.Active;
					objCountry.Insert();
					InsertCount++;
				}
			}

			CU.SetSuccessExcelMessage(InsertCount, UpdateCount, "Country");
		}
		catch (Exception ex)
		{
			CU.ZMessage(eMsgType.Error, string.Empty, ex.Message, 0);
		}

		LoadCountryGrid(ePageIndex.Custom);
	}

	#endregion


	#region Pagging


	protected void lnkPrev_Click(object sender, EventArgs e)
	{
		LoadCountryGrid(ePageIndex.Prev);
	}

	protected void lnkNext_Click(object sender, EventArgs e)
	{
		LoadCountryGrid(ePageIndex.Next);
	}

	protected void lnkFirst_Click(object sender, EventArgs e)
	{
		LoadCountryGrid(ePageIndex.First);
	}

	protected void lnkLast_Click(object sender, EventArgs e)
	{
		LoadCountryGrid(ePageIndex.Last);
	}

	protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
	{
		if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
		{
			txtGotoPageNo.Text = "1";
			txtGotoPageNo.Focus();
		}
		LoadCountryGrid(ePageIndex.Custom);
	}

	protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
	{
		txtGotoPageNo.Text = "1";
		LoadCountryGrid(ePageIndex.Custom);
		Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
	}


	#endregion
}
