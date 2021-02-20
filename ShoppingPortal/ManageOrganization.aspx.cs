using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;

public partial class ManageOrganization : CompressorPage
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
			CU.LoadDisplayPerPage(ref ddlRecordPerPage);

			LoadOrganizationGrid(ePageIndex.Custom);
			CheckVisibleButton();
		}

		Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
		Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
		Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

		try { grdOrganization.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}


	private DataTable GetOrganizationDt(ePageIndex ePageIndex)
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

		return objQuery.Select(eSP.qry_Organization);
	}

	private void LoadOrganizationGrid(ePageIndex ePageIndex)
	{
		DataTable dtOrganization = GetOrganizationDt(ePageIndex);

		if (dtOrganization.Rows.Count > 0)
			lblCount.Text = dtOrganization.Rows[0][CS.TotalRecord].ToString();
		else
			lblCount.Text = "0";

		divPaging.Visible = (dtOrganization.Rows.Count > 0);

		txtGotoPageNo.Text = PageIndex.ToString();

		ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

		grdOrganization.DataSource = dtOrganization;
		grdOrganization.DataBind();

		try { grdOrganization.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}

	private void CheckVisibleButton()
	{
		var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.Organization);

		lnkAdd.Visible = lnkEdit.Visible = objAuthority.IsAddEdit;
		lnkDelete.Visible = objAuthority.IsDelete;

		lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
		lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
	}


	protected void lnkAdd_OnClick(object sender, EventArgs e)
	{
		lblOrganizationId.Text = string.Empty;
		LoadOrganizationDetail();
		popupOrganization.Show();
	}

	protected void lnkEdit_OnClick(object sender, EventArgs e)
	{
		if (CU.GetAuthority(CU.GetUsersId(), eAuthority.Organization).IsAddEdit && (sender == null || grdOrganization.zIsValidSelection(lblOrganizationId, "chkSelect", CS.OrganizationId)))
		{
			LoadOrganizationDetail();
			popupOrganization.Show();
		}
	}

	protected void lnkEditOrganization_OnClick(object sender, EventArgs e)
	{
		lblOrganizationId.Text = ((LinkButton)sender).CommandArgument.ToString();
		lnkEdit_OnClick(null, null);
	}

	protected void lnkRefresh_OnClick(object sender, EventArgs e)
	{
		LoadOrganizationGrid(ePageIndex.Custom);
	}

	protected void lnkActive_OnClick(object sender, EventArgs e)
	{
		if (grdOrganization.zIsValidSelection(lblOrganizationId, "chkSelect", CS.OrganizationId))
		{
			if (new Organization()
			{
				OrganizationId = lblOrganizationId.zToInt(),
				eStatus = (int)eStatus.Active
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This Organization is already Active.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Active, "Active Organization", "Are You Sure To Active Organization?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDeactive_OnClick(object sender, EventArgs e)
	{
		if (grdOrganization.zIsValidSelection(lblOrganizationId, "chkSelect", CS.OrganizationId))
		{
			if (new Organization()
			{
				OrganizationId = lblOrganizationId.zToInt(),
				eStatus = (int)eStatus.Deactive
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This Organization is already Deactive.");
				return;
			}

			string Message = string.Empty;
			if (CU.IsOrganizationUsed(lblOrganizationId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Deactive.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive Organization", "Are You Sure To Deactive Organization?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDelete_OnClick(object sender, EventArgs e)
	{
		if (grdOrganization.zIsValidSelection(lblOrganizationId, "chkSelect", CS.OrganizationId))
		{
			string Message = string.Empty;
			if (CU.IsOrganizationUsed(lblOrganizationId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Delete.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Organization", "Are You Sure To Delete Organization?");
			popupConfirmation.Show();
		}
	}

	protected void Control_CheckedChanged(object sender, EventArgs e)
	{
		LoadOrganizationGrid(ePageIndex.Custom);
		CheckVisibleButton();
	}


	private void ManageOrganizationStatus(eStatus Status)
	{
		new Organization()
		{
			OrganizationId = lblOrganizationId.zToInt(),
			eStatus = (int)Status
		}.Update();
	}

	protected void btnActive_OnClick(object sender, EventArgs e)
	{
		ManageOrganizationStatus(eStatus.Active);
		CU.ZMessage(eMsgType.Success, string.Empty, "Organization Activated Successfully.");
		LoadOrganizationGrid(ePageIndex.Custom);
	}

	protected void btnDeactive_OnClick(object sender, EventArgs e)
	{
		ManageOrganizationStatus(eStatus.Deactive);
		CU.ZMessage(eMsgType.Success, string.Empty, "Organization Deactive Successfully.");
		LoadOrganizationGrid(ePageIndex.Custom);
	}

	protected void btnDelete_OnClick(object sender, EventArgs e)
	{
		ManageOrganizationStatus(eStatus.Delete);
		CU.ZMessage(eMsgType.Success, string.Empty, "Organization Delete Successfully.");
		LoadOrganizationGrid(ePageIndex.Custom);
	}


	protected void grdOrganization_OnRowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			if (!IsAddEdit.HasValue)
				IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.Organization).IsAddEdit;

			if (IsAddEdit.Value)
				e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdOrganization, "Select$" + e.Row.RowIndex);

			if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdOrganization, CS.eStatus)].Text) != (int)eStatus.Active)
				e.Row.Attributes["class"] = "GridDesableRow ";

			DataRowView dataItem = (DataRowView)e.Row.DataItem;
			var lnkEditOrganization = e.Row.FindControl("lnkEditOrganization") as LinkButton;
			var ltrOrganization = e.Row.FindControl("ltrOrganization") as Literal;

			lnkEditOrganization.Visible = IsAddEdit.Value;
			ltrOrganization.Visible = !IsAddEdit.Value;

			lnkEditOrganization.Text = ltrOrganization.Text = dataItem[CS.OrganizationName].ToString();
			lnkEditOrganization.CommandArgument = dataItem[CS.OrganizationId].ToString();
		}
	}

	protected void grdOrganization_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		lblOrganizationId.Text = grdOrganization.Rows[grdOrganization.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdOrganization, CS.OrganizationId)].Text;
		lnkEdit_OnClick(null, null);
	}


	private void LoadOrganizationDetail()
	{
		txtOrganizationName.Focus();

		if (IsEditMode())
		{
			lblPopupTitle.Text = "Edit Organization";
			var objOrganization = new Organization() { OrganizationId = lblOrganizationId.zToInt(), }.SelectList<Organization>()[0];

			txtOrganizationName.Text = objOrganization.OrganizationName;
			txtOrgUId.Text = objOrganization.OrgUId.ToString();
		}
		else
		{
			lblPopupTitle.Text = "New Organization";

			txtOrganizationName.Text = string.Empty;

			var drMaxOrgUId = new Query() { eStatusNot = (int)eStatus.Delete }.Select(eSP.qry_Max_OrgUId).Rows[0];
			int? OrgUId = drMaxOrgUId[CS.OrgUId].zToInt();
			txtOrgUId.Text = OrgUId.HasValue ? (OrgUId + 1).ToString() : "1";
		}
	}

	private bool IsEditMode()
	{
		return !lblOrganizationId.zIsNullOrEmpty();
	}

	private bool IsValidate()
	{
		if (txtOrganizationName.zIsNullOrEmpty())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Organization Name.");
			txtOrganizationName.Focus();
			return false;
		}

		var dtOrganization = new Query()
		{
			eStatusNot = (int)eStatus.Delete,
			OrganizationName = txtOrganizationName.Text.Trim(),
		}.Select(eSP.qry_Organization);

		if (dtOrganization.Rows.Count > 0 && dtOrganization.Rows[0][CS.OrganizationId].ToString() != lblOrganizationId.Text)
		{
			string Status = dtOrganization.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
			CU.ZMessage(eMsgType.Error, string.Empty, "This Organization is already exist" + Status + ".");
			txtOrganizationName.Focus();
			return false;
		}

		if (!txtOrgUId.zIsNumber())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Unique Id.");
			txtOrgUId.Focus();
			return false;
		}

		var dtOrganizationUId = new Query()
		{
			eStatusNot = (int)eStatus.Delete,
			OrgUId = txtOrgUId.zToInt(),
		}.Select(eSP.qry_Organization);

		if (dtOrganizationUId.Rows.Count > 0 && dtOrganizationUId.Rows[0][CS.OrganizationId].ToString() != lblOrganizationId.Text)
		{
			string Status = dtOrganizationUId.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
			CU.ZMessage(eMsgType.Error, string.Empty, "This Unique Id is already exist" + Status + ".");
			txtOrgUId.Focus();
			return false;
		}

		return true;
	}

	private bool SaveData()
	{
		if (!IsValidate())
			return false;

		string Message = string.Empty;

		var objOrganization = new Organization()
		{
			OrganizationName = txtOrganizationName.Text.Trim().zFirstCharToUpper(),
			OrgUId = txtOrgUId.zToInt()
		};

		if (IsEditMode())
		{
			objOrganization.OrganizationId = lblOrganizationId.zToInt();
			objOrganization.Update();

			Message = "Organization Detail Change Sucessfully.";
		}
		else
		{
			objOrganization.eStatus = (int)eStatus.Active;
			int OrganizationId = objOrganization.Insert();

			var dtStatusType = CU.GetEnumDt<eStatusType>(string.Empty);
			foreach (DataRow dr in dtStatusType.Rows)
			{
				new OrderStatus()
				{
					OrganizationId = OrganizationId,
					eStatusType = dr[CS.Id].zToInt(),
					SerialNo = dr[CS.Id].zToInt(),
					StatusName = dr[CS.Name].ToString(),
					eStatus = (int)eStatus.Active,
				}.Insert();
			}

			Message = "New Organization Added Sucessfully.";
		}

		CU.ZMessage(eMsgType.Success, string.Empty, Message);

		return true;
	}

	protected void btnSave_OnClick(object sender, EventArgs e)
	{
		if (SaveData())
		{
			LoadOrganizationGrid(ePageIndex.Custom);
		}
	}

	protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
	{
		if (SaveData())
		{
			LoadOrganizationGrid(ePageIndex.Custom);
			lnkAdd_OnClick(null, null);
		}
	}


	#region Pagging


	protected void lnkPrev_Click(object sender, EventArgs e)
	{
		LoadOrganizationGrid(ePageIndex.Prev);
	}

	protected void lnkNext_Click(object sender, EventArgs e)
	{
		LoadOrganizationGrid(ePageIndex.Next);
	}

	protected void lnkFirst_Click(object sender, EventArgs e)
	{
		LoadOrganizationGrid(ePageIndex.First);
	}

	protected void lnkLast_Click(object sender, EventArgs e)
	{
		LoadOrganizationGrid(ePageIndex.Last);
	}

	protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
	{
		if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
		{
			txtGotoPageNo.Text = "1";
			txtGotoPageNo.Focus();
		}
		LoadOrganizationGrid(ePageIndex.Custom);
	}

	protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
	{
		txtGotoPageNo.Text = "1";
		LoadOrganizationGrid(ePageIndex.Custom);
		Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
	}


	#endregion
}
