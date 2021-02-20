using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;

public partial class ManageDesignation : CompressorPage
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
		lblOrganizationId.Text = CU.GetOrganizationId().ToString();

		if (!IsPostBack)
		{
			CU.LoadDisplayPerPage(ref ddlRecordPerPage);
			SetControl(eControl.Designation);
		}

		Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
		Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
		Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

		ManageAuthority.btnCancel_OnClick += new EventHandler(lnkDesignation_OnClick);

		popupManageDesignation.btnSave_OnClick += new EventHandler(btnSave_OnClick);
		popupManageDesignation.btnSaveAndNew_OnClick += new EventHandler(btnSaveAndNew_OnClick);

		try { grdDesignation.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}

	private DataTable GetDesignationDt(ePageIndex ePageIndex)
	{
		int? Status = null;
		if (chkActive.Checked && !chkDeactive.Checked)
			Status = (int)eStatus.Active;
		else if (!chkActive.Checked && chkDeactive.Checked)
			Status = (int)eStatus.Deactive;

		var lstDesignationId = CU.GetlstAuthoDesignation();
		if (lstDesignationId.Count == 0)
			lstDesignationId.Add(0);

		var objQuery = new Query()
		{
			MasterSearch = txtSearch.Text,
			eStatus = Status,
			eStatusNot = (int)eStatus.Delete,
			OrganizationId = lblOrganizationId.zToInt(),
			eDesignationIn = CU.GetParaIn(lstDesignationId, false),
		};

		#region Page Index

		int RecordPerPage = ddlRecordPerPage.zToInt().Value;
		int PageIndexTemp = PageIndex;

		CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref objQuery, ref txtGotoPageNo, ref lblCount);
		PageIndex = PageIndexTemp;

		#endregion Page Index

		return objQuery.Select(eSP.qry_Designation);
	}

	private void LoadDesignationGrid(ePageIndex ePageIndex)
	{
		DataTable dtDesignation = GetDesignationDt(ePageIndex);

		if (dtDesignation.Rows.Count > 0)
			lblCount.Text = dtDesignation.Rows[0][CS.TotalRecord].ToString();
		else
			lblCount.Text = "0";

		divPaging.Visible = (dtDesignation.Rows.Count > 0);

		txtGotoPageNo.Text = PageIndex.ToString();

		ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

		grdDesignation.DataSource = dtDesignation;
		grdDesignation.DataBind();

		try { grdDesignation.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}

	private void CheckVisibleButton()
	{
		var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.Designation);

		lnkAdd.Visible = lnkEdit.Visible = objAuthority.IsAddEdit;
		lnkDelete.Visible = objAuthority.IsDelete;

		lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
		lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));

		if (!objAuthority.IsAddEdit)
			grdDesignation.Attributes.Add("class", grdDesignation.Attributes["class"].ToString().Replace("rowloader", ""));
	}


	protected void btnSave_OnClick(object sender, EventArgs e)
	{
		LoadDesignationGrid(ePageIndex.Custom);
		popupDesignation.Hide();
	}

	protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
	{
		LoadDesignationGrid(ePageIndex.Custom);
		lnkAdd_OnClick(null, null);
	}


	protected void lnkAdd_OnClick(object sender, EventArgs e)
	{
		lblDesignationId.Text = string.Empty;
		popupManageDesignation.SetDesignationId = lblDesignationId.Text;
		popupManageDesignation.LoadDesignationDetail(false);
		popupDesignation.Show();
	}

	protected void lnkEdit_OnClick(object sender, EventArgs e)
	{
		if (CU.GetAuthority(CU.GetUsersId(), eAuthority.Designation).IsAddEdit && (sender == null || grdDesignation.zIsValidSelection(lblDesignationId, "chkSelect", CS.DesignationId)))
		{
			popupManageDesignation.SetDesignationId = lblDesignationId.Text;
			popupManageDesignation.LoadDesignationDetail(false);
			popupDesignation.Show();
		}
	}

	protected void lnkEditDesignation_OnClick(object sender, EventArgs e)
	{
		lblDesignationId.Text = ((LinkButton)sender).CommandArgument.ToString();
		lnkEdit_OnClick(null, null);
	}

	protected void lnkRefresh_OnClick(object sender, EventArgs e)
	{
		LoadDesignationGrid(ePageIndex.Custom);
	}

	protected void lnkActive_OnClick(object sender, EventArgs e)
	{
		if (grdDesignation.zIsValidSelection(lblDesignationId, "chkSelect", CS.DesignationId))
		{
			if (new Designation()
			{
				DesignationId = lblDesignationId.zToInt(),
				eStatus = (int)eStatus.Active
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This Designation is already Active.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Active, "Active Designation", "Are You Sure To Active Designation?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDeactive_OnClick(object sender, EventArgs e)
	{
		if (grdDesignation.zIsValidSelection(lblDesignationId, "chkSelect", CS.DesignationId))
		{
			if (new Designation()
			{
				DesignationId = lblDesignationId.zToInt(),
				eStatus = (int)eStatus.Deactive
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This Designation is already Deactive.");
				return;
			}

			string Message = string.Empty;
			if (CU.IsDesignationUsed(lblDesignationId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Deactive.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive Designation", "Are You Sure To Deactive Designation?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDelete_OnClick(object sender, EventArgs e)
	{
		if (grdDesignation.zIsValidSelection(lblDesignationId, "chkSelect", CS.DesignationId))
		{
			string Message = string.Empty;
			if (CU.IsDesignationUsed(lblDesignationId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Delete.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Designation", "Are You Sure To Delete Designation?");
			popupConfirmation.Show();
		}
	}


	protected void lnkSetAuthority_OnClick(object sender, EventArgs e)
	{
		if (grdDesignation.zIsValidSelection(lblDesignationId, "chkSelect", CS.DesignationId))
		{
			SetControl(eControl.DesignationAuthority);
		}
	}


	private void ManageDesignationStatus(eStatus Status)
	{
		new Designation()
		{
			DesignationId = lblDesignationId.zToInt(),
			eStatus = (int)Status
		}.Update();
	}

	protected void btnActive_OnClick(object sender, EventArgs e)
	{
		ManageDesignationStatus(eStatus.Active);
		CU.ZMessage(eMsgType.Success, string.Empty, "Designation Activated Successfully.");
		LoadDesignationGrid(ePageIndex.Custom);
	}

	protected void btnDeactive_OnClick(object sender, EventArgs e)
	{
		ManageDesignationStatus(eStatus.Deactive);
		CU.ZMessage(eMsgType.Success, string.Empty, "Designation Deactive Successfully.");
		LoadDesignationGrid(ePageIndex.Custom);
	}

	protected void btnDelete_OnClick(object sender, EventArgs e)
	{
		ManageDesignationStatus(eStatus.Delete);
		CU.ZMessage(eMsgType.Success, string.Empty, "Designation Deleted Successfully.");
		LoadDesignationGrid(ePageIndex.Custom);
	}


	protected void Control_CheckedChanged(object sender, EventArgs e)
	{
		LoadDesignationGrid(ePageIndex.Custom);
		CheckVisibleButton();
	}


	protected void grdDesignation_OnRowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			if (!IsAddEdit.HasValue)
				IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.Designation).IsAddEdit;
			if (IsAddEdit.Value)
				e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdDesignation, "Select$" + e.Row.RowIndex);

			if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdDesignation, CS.eStatus)].Text) != (int)eStatus.Active)
				e.Row.Attributes["class"] = "GridDesableRow ";

			DataRowView dataItem = (DataRowView)e.Row.DataItem;
			var lnkEditDesignation = e.Row.FindControl("lnkEditDesignation") as LinkButton;
			var ltrDesignation = e.Row.FindControl("ltrDesignation") as Literal;

			lnkEditDesignation.Visible = IsAddEdit.Value;
			ltrDesignation.Visible = !IsAddEdit.Value;

			lnkEditDesignation.Text = ltrDesignation.Text = dataItem[CS.DesignationName].ToString();
			lnkEditDesignation.CommandArgument = dataItem[CS.DesignationId].ToString();
		}
	}

	protected void grdDesignation_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		lblDesignationId.Text = grdDesignation.Rows[grdDesignation.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdDesignation, CS.DesignationId)].Text;
		//Response.Redirect("ManageState.aspx?" + CS.DesignationId.Encrypt() + "=" + lblDesignationId.Text.Encrypt());
		lnkEdit_OnClick(null, null);
	}


	#region Pagging


	protected void lnkPrev_Click(object sender, EventArgs e)
	{
		LoadDesignationGrid(ePageIndex.Prev);
	}

	protected void lnkNext_Click(object sender, EventArgs e)
	{
		LoadDesignationGrid(ePageIndex.Next);
	}

	protected void lnkFirst_Click(object sender, EventArgs e)
	{
		LoadDesignationGrid(ePageIndex.First);
	}

	protected void lnkLast_Click(object sender, EventArgs e)
	{
		LoadDesignationGrid(ePageIndex.Last);
	}

	protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
	{
		if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
		{
			txtGotoPageNo.Text = "1";
			txtGotoPageNo.Focus();
		}
		LoadDesignationGrid(ePageIndex.Custom);
	}

	protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
	{
		txtGotoPageNo.Text = "1";
		LoadDesignationGrid(ePageIndex.Custom);
		Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
	}


	#endregion


	protected void lnkDesignation_OnClick(object sender, EventArgs e)
	{
		SetControl(eControl.Designation);
	}

	private void SetControl(eControl Control)
	{
		pnlDesignation.Visible = false;
		pnlDesignationAuthority.Visible = false;

		switch (Control)
		{
			case eControl.Designation:
				pnlDesignation.Visible = true;
				LoadDesignationGrid(ePageIndex.Custom);
				CheckVisibleButton();
				break;
			case eControl.DesignationAuthority:
				pnlDesignationAuthority.Visible = true;
				ManageAuthority.LoadAuthorityDetail(lblDesignationId.zToInt().Value, 0, "Designation");
				break;
			default:
				break;
		}
	}

	private enum eControl
	{
		Designation = 1,
		DesignationAuthority = 2,
	}
}
