using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;

public partial class ManageBankAccount : CompressorPage
{
	bool? IsAddEdit;

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
			lblOrganizationId.Text = CU.GetOrganizationId().ToString();
			CU.LoadDisplayPerPage(ref ddlRecordPerPage);

			LoadBankAccountGrid(ePageIndex.Custom);
			CheckVisibleButton();
		}

		Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
		Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
		Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

		try { grdBankAccount.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}


	private DataTable GetBankAccountDt(ePageIndex ePageIndex)
	{
		int? Status = null;
		if (chkActive.Checked && !chkDeactive.Checked)
			Status = (int)eStatus.Active;
		else if (!chkActive.Checked && chkDeactive.Checked)
			Status = (int)eStatus.Deactive;

		var objQuery = new Query()
		{
			MasterSearch = txtSearch.Text,
			OrganizationId = lblOrganizationId.zToInt(),
			eStatus = Status,
			eStatusNot = (int)eStatus.Delete,
		};

		#region Page Index

		int RecordPerPage = ddlRecordPerPage.zToInt().Value;
		int PageIndexTemp = PageIndex;

		CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref objQuery, ref txtGotoPageNo, ref lblCount);
		PageIndex = PageIndexTemp;

		#endregion Page Index

		return objQuery.Select(eSP.qry_BankAccount);
	}

	private void LoadBankAccountGrid(ePageIndex ePageIndex)
	{
		DataTable dtBankAccount = GetBankAccountDt(ePageIndex);

		if (dtBankAccount.Rows.Count > 0)
			lblCount.Text = dtBankAccount.Rows[0][CS.TotalRecord].ToString();
		else
			lblCount.Text = "0";

		divPaging.Visible = (dtBankAccount.Rows.Count > 0);

		txtGotoPageNo.Text = PageIndex.ToString();

		ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

		grdBankAccount.DataSource = dtBankAccount;
		grdBankAccount.DataBind();

		try { grdBankAccount.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}

	private void CheckVisibleButton()
	{
		var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageBankAccount);

		lnkAdd.Visible = lnkEdit.Visible = objAuthority.IsAddEdit;
		lnkDelete.Visible = objAuthority.IsDelete;

		lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
		lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
	}


	protected void lnkAdd_OnClick(object sender, EventArgs e)
	{
		lblBankAccountId.Text = string.Empty;
		LoadBankAccountDetail();
		popupBankAccount.Show();
	}

	protected void lnkEdit_OnClick(object sender, EventArgs e)
	{
		if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageBankAccount).IsAddEdit && (sender == null || grdBankAccount.zIsValidSelection(lblBankAccountId, "chkSelect", CS.BankAccountId)))
		{
			LoadBankAccountDetail();
			popupBankAccount.Show();
		}
	}

	protected void lnkEditBankAccount_OnClick(object sender, EventArgs e)
	{
		lblBankAccountId.Text = ((LinkButton)sender).CommandArgument.ToString();
		lnkEdit_OnClick(null, null);
	}

	protected void lnkRefresh_OnClick(object sender, EventArgs e)
	{
		LoadBankAccountGrid(ePageIndex.Custom);
	}

	protected void lnkActive_OnClick(object sender, EventArgs e)
	{
		if (grdBankAccount.zIsValidSelection(lblBankAccountId, "chkSelect", CS.BankAccountId))
		{
			if (new BankAccount()
			{
				BankAccountId = lblBankAccountId.zToInt(),
				eStatus = (int)eStatus.Active
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This BankAccount is already Active.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Active, "Active BankAccount", "Are You Sure To Active BankAccount?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDeactive_OnClick(object sender, EventArgs e)
	{
		if (grdBankAccount.zIsValidSelection(lblBankAccountId, "chkSelect", CS.BankAccountId))
		{
			if (new BankAccount()
			{
				BankAccountId = lblBankAccountId.zToInt(),
				eStatus = (int)eStatus.Deactive
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This BankAccount is already Deactive.");
				return;
			}

			string Message = string.Empty;
			if (CU.IsBankAccountUsed(lblBankAccountId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Deactive.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive BankAccount", "Are You Sure To Deactive BankAccount?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDelete_OnClick(object sender, EventArgs e)
	{
		if (grdBankAccount.zIsValidSelection(lblBankAccountId, "chkSelect", CS.BankAccountId))
		{
			string Message = string.Empty;
			if (CU.IsBankAccountUsed(lblBankAccountId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Delete.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete BankAccount", "Are You Sure To Delete BankAccount?");
			popupConfirmation.Show();
		}
	}

	protected void Control_CheckedChanged(object sender, EventArgs e)
	{
		LoadBankAccountGrid(ePageIndex.Custom);
		CheckVisibleButton();
	}


	private void ManageBankAccountStatus(eStatus Status)
	{
		new BankAccount()
		{
			BankAccountId = lblBankAccountId.zToInt(),
			eStatus = (int)Status
		}.Update();
	}

	protected void btnActive_OnClick(object sender, EventArgs e)
	{
		ManageBankAccountStatus(eStatus.Active);
		CU.ZMessage(eMsgType.Success, string.Empty, "BankAccount Activated Successfully.");
		LoadBankAccountGrid(ePageIndex.Custom);
	}

	protected void btnDeactive_OnClick(object sender, EventArgs e)
	{
		ManageBankAccountStatus(eStatus.Deactive);
		CU.ZMessage(eMsgType.Success, string.Empty, "BankAccount Deactive Successfully.");
		LoadBankAccountGrid(ePageIndex.Custom);
	}

	protected void btnDelete_OnClick(object sender, EventArgs e)
	{
		ManageBankAccountStatus(eStatus.Delete);
		CU.ZMessage(eMsgType.Success, string.Empty, "BankAccount Delete Successfully.");
		LoadBankAccountGrid(ePageIndex.Custom);
	}


	protected void grdBankAccount_OnRowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			if (!IsAddEdit.HasValue)
				IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageBankAccount).IsAddEdit;

			if (IsAddEdit.Value)
				e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdBankAccount, "Select$" + e.Row.RowIndex);

			if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdBankAccount, CS.eStatus)].Text) != (int)eStatus.Active)
				e.Row.Attributes["class"] = "GridDesableRow ";

			DataRowView dataItem = (DataRowView)e.Row.DataItem;
			var lnkEditBankAccount = e.Row.FindControl("lnkEditBankAccount") as LinkButton;
			var ltrBankAccount = e.Row.FindControl("ltrBankAccount") as Literal;

			lnkEditBankAccount.Visible = IsAddEdit.Value;
			ltrBankAccount.Visible = !IsAddEdit.Value;

			lnkEditBankAccount.Text = ltrBankAccount.Text = dataItem[CS.BankAccountName].ToString();
			lnkEditBankAccount.CommandArgument = dataItem[CS.BankAccountId].ToString();
		}
	}

	protected void grdBankAccount_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		lblBankAccountId.Text = grdBankAccount.Rows[grdBankAccount.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdBankAccount, CS.BankAccountId)].Text;
		lnkEdit_OnClick(null, null);
	}


	private void LoadBankAccountDetail()
	{
		txtBankAccountName.Focus();

		if (IsEditMode())
		{
			lblPopupTitle.Text = "Edit BankAccount";
			var objBankAccount = new BankAccount() { BankAccountId = lblBankAccountId.zToInt(), }.SelectList<BankAccount>()[0];
			txtBankAccountName.Text = objBankAccount.BankAccountName;
		}
		else
		{
			lblPopupTitle.Text = "New BankAccount";
			txtBankAccountName.Text = string.Empty;
		}
	}

	private bool IsEditMode()
	{
		return !lblBankAccountId.zIsNullOrEmpty();
	}

	private bool IsValidate()
	{
		if (txtBankAccountName.zIsNullOrEmpty())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter BankAccount Name.");
			txtBankAccountName.Focus();
			return false;
		}

		var dtBankAccount = new Query()
		{
			eStatusNot = (int)eStatus.Delete,
			OrganizationId = lblOrganizationId.zToInt(),
			BankAccountName = txtBankAccountName.Text.Trim(),
		}.Select(eSP.qry_BankAccount);

		if (dtBankAccount.Rows.Count > 0 && dtBankAccount.Rows[0][CS.BankAccountId].ToString() != lblBankAccountId.Text)
		{
			string Status = dtBankAccount.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
			CU.ZMessage(eMsgType.Error, string.Empty, "This BankAccount is already exist" + Status + ".");
			txtBankAccountName.Focus();
			return false;
		}

		return true;
	}

	private bool SaveData()
	{
		if (!IsValidate())
			return false;

		string Message = string.Empty;

		var objBankAccount = new BankAccount()
		{
			OrganizationId = lblOrganizationId.zToInt(),
			BankAccountName = txtBankAccountName.Text.Trim().zFirstCharToUpper(),
		};

		if (IsEditMode())
		{
			objBankAccount.BankAccountId = lblBankAccountId.zToInt();
			objBankAccount.Update();

			Message = "BankAccount Detail Change Sucessfully.";
		}
		else
		{
			objBankAccount.eStatus = (int)eStatus.Active;
			objBankAccount.Insert();

			Message = "New BankAccount Added Sucessfully.";
		}

		CU.ZMessage(eMsgType.Success, string.Empty, Message);

		return true;
	}

	protected void btnSave_OnClick(object sender, EventArgs e)
	{
		if (SaveData())
		{
			LoadBankAccountGrid(ePageIndex.Custom);
		}
	}

	protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
	{
		if (SaveData())
		{
			LoadBankAccountGrid(ePageIndex.Custom);
			lnkAdd_OnClick(null, null);
		}
	}


	#region Pagging


	protected void lnkPrev_Click(object sender, EventArgs e)
	{
		LoadBankAccountGrid(ePageIndex.Prev);
	}

	protected void lnkNext_Click(object sender, EventArgs e)
	{
		LoadBankAccountGrid(ePageIndex.Next);
	}

	protected void lnkFirst_Click(object sender, EventArgs e)
	{
		LoadBankAccountGrid(ePageIndex.First);
	}

	protected void lnkLast_Click(object sender, EventArgs e)
	{
		LoadBankAccountGrid(ePageIndex.Last);
	}

	protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
	{
		if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
		{
			txtGotoPageNo.Text = "1";
			txtGotoPageNo.Focus();
		}
		LoadBankAccountGrid(ePageIndex.Custom);
	}

	protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
	{
		txtGotoPageNo.Text = "1";
		LoadBankAccountGrid(ePageIndex.Custom);
		Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
	}


	#endregion
}
