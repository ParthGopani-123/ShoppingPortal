using System;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class ManageUser : CompressorPage
{
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

	DataTable dtContact = new DataTable();

	protected void Page_Load(object sender, EventArgs e)
	{
		LoginUtilities.CheckSession();

		if (!IsPostBack)
		{
			lblFirmId.Text = CU.GetFirmId().ToString();
			CU.LoadDisplayPerPage(ref ddlRecordPerPage);

            var DesignationId = CU.GeteDesignationId(CU.GetUsersId());
            divorganization.Visible = divFirm.Visible = DesignationId == eDesignation.SystemAdmin;

            LoadOrganization();
            LoadFirm();

            SetControl(eControl.User);
		}

		Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
		Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
		Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

		ManageUserCC.btnSaveUser_OnClick += new EventHandler(btnSaveUser_OnClick);
		ManageUserCC.btnSaveAndNewUser_OnClick += new EventHandler(btnSaveAndNewUser_OnClick);

		ManageAuthority.btnCancel_OnClick += new EventHandler(lnkCancelAuthority_OnClick);

		try { grdUser.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}


    private void LoadOrganization()
    {
        var dtOrganization = new Query() { eStatus = (int)eStatus.Active }.Select(eSP.qry_Organization);
        CU.FillDropdown(ref ddlOrganization, dtOrganization, "-- Select Organization --", CS.OrganizationId, CS.OrganizationName);

        try
        {
            ddlOrganization.SelectedValue = CU.GetOrganizationId().ToString();
        }
        catch { }
    }

    private void LoadFirm()
    {
        var dtFirm = new Query() { OrganizationId = ddlOrganization.zIsSelect() ? ddlOrganization.zToInt() : null, eStatus = (int)eStatus.Active }.Select(eSP.qry_Firm);
        CU.FillDropdown(ref ddlFirm, dtFirm, "-- Select Firm --", CS.FirmId, CS.FirmName);
        try
        {
            ddlFirm.SelectedValue = CU.GetFirmId().ToString();
        }
        catch { }
    }

    private void LoadUserGrid(ePageIndex ePageIndex)
	{
		eDesignation Designation = CU.GeteDesignationId(CU.GetUsersId());
		int? Status = null;
		if (chkActive.Checked && !chkDeactive.Checked)
			Status = (int)eStatus.Active;
		else if (!chkActive.Checked && chkDeactive.Checked)
			Status = (int)eStatus.Deactive;

		var objQuery = new Query()
		{
            OrganizationId = ddlOrganization.zIsSelect() ? ddlOrganization.zToInt() : null,
            FirmId = ddlFirm.zIsSelect() ? ddlFirm.zToInt() : (Designation == eDesignation.SystemAdmin ? (int?)null : lblFirmId.zToInt()),
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

		DataTable dtUser = objQuery.Select(eSP.qry_User);

		#region Count Total

		lblCount.Text = dtUser.Rows.Count.ToString();

		divPaging.Visible = (dtUser.Rows.Count > 0);

		txtGotoPageNo.Text = PageIndex.ToString();

		ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

		#endregion

		dtContact = new Query()
		{
			eParentType = (int)eParentType.User,
		}.Select(eSP.qry_Contacts);

		grdUser.DataSource = dtUser;
		grdUser.DataBind();

		try { grdUser.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}

	private void CheckVisibleButton()
	{
		lnkActive.Visible = ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked));
		lnkDeactive.Visible = ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked));
	}


	protected void btnSaveUser_OnClick(object sender, EventArgs e)
	{
		LoadUserGrid(ePageIndex.Custom);
		popupUser.Hide();
	}

	protected void btnSaveAndNewUser_OnClick(object sender, EventArgs e)
	{
		LoadUserGrid(ePageIndex.Custom);
		lnkAdd_OnClick(null, null);
	}

	protected void lnkAdd_OnClick(object sender, EventArgs e)
	{
		ManageUserCC.SetUsersId = string.Empty;
		ManageUserCC.LoadUserDetail();
		popupUser.Show();
	}

	protected void lnkEditUser_OnClick(object sender, EventArgs e)
	{
		txtUsersId.Text = ((LinkButton)sender).CommandArgument.ToString();
		lnkEdit_OnClick(null, null);
	}

	protected void lnkEdit_OnClick(object sender, EventArgs e)
	{
		if (sender == null || grdUser.zIsValidSelection(txtUsersId, "chkSelect", CS.UsersId))
		{
			ManageUserCC.SetUsersId = txtUsersId.Text;
			ManageUserCC.LoadUserDetail();
			popupUser.Show();
		}
	}

	protected void lnkRefresh_OnClick(object sender, EventArgs e)
	{
		LoadUserGrid(ePageIndex.Custom);
	}

	protected void lnkActive_OnClick(object sender, EventArgs e)
	{
		if (grdUser.zIsValidSelection(txtUsersId, "chkSelect", CS.UsersId))
		{
			if (new Users()
			{
				UsersId = txtUsersId.zToInt(),
				eStatus = (int)eStatus.Active
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This User is already Active.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Active, "Active User", "Are You Sure To Active this User?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDeactive_OnClick(object sender, EventArgs e)
	{
		if (grdUser.zIsValidSelection(txtUsersId, "chkSelect", CS.UsersId))
		{
			if (new Users()
			{
				UsersId = txtUsersId.zToInt(),
				eStatus = (int)eStatus.Deactive
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This User is already Deactive.");
				return;
			}

			string Message = string.Empty;
			if (CU.IsUserUsed(txtUsersId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", So You can not Deactive It.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive User", "Are You Sure To Deactive this User?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDelete_OnClick(object sender, EventArgs e)
	{
		if (grdUser.zIsValidSelection(txtUsersId, "chkSelect", CS.UsersId))
		{
			string Message = string.Empty;
			if (CU.IsUserUsed(txtUsersId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Delete.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete User", "Are You Sure To Delete this User?");
			popupConfirmation.Show();
		}
	}


	protected void btnActive_OnClick(object sender, EventArgs e)
	{
		new Users() { UsersId = txtUsersId.zToInt(), eStatus = (int)eStatus.Active }.Update();
		CU.ZMessage(eMsgType.Success, string.Empty, "User Activet Successfully");
		LoadUserGrid(ePageIndex.Custom);
	}

	protected void btnDeactive_OnClick(object sender, EventArgs e)
	{
		new Users() { UsersId = txtUsersId.zToInt(), eStatus = (int)eStatus.Deactive }.Update();
		CU.ZMessage(eMsgType.Success, string.Empty, "User Deactivet Successfully");
		LoadUserGrid(ePageIndex.Custom);
	}

	protected void btnDelete_OnClick(object sender, EventArgs e)
	{
		new Users() { UsersId = txtUsersId.zToInt(), eStatus = (int)eStatus.Delete }.Update();
		CU.ZMessage(eMsgType.Success, string.Empty, "User Deleted Successfully");
		LoadUserGrid(ePageIndex.Custom);
	}


	protected void Control_CheckedChanged(object sender, EventArgs e)
	{
		LoadUserGrid(ePageIndex.Custom);
		CheckVisibleButton();
	}

    protected void ddlOrganization_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadFirm();
    }


    protected void grdUser_OnRowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdUser, "Select$" + e.Row.RowIndex);
			if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdUser, CS.eStatus)].Text) != (int)eStatus.Active)
				e.Row.Attributes["class"] = "GridDesableRow ";

			DataRowView dataItem = (DataRowView)e.Row.DataItem;
			var lnkMobile = e.Row.FindControl("lnkMobile") as LinkButton;
			lnkMobile.Text = "(" + dataItem[CS.MobileNo] + ")";
			lnkMobile.PostBackUrl = "tel:" + dataItem[CS.MobileNo];

			string Contacts = string.Empty;
			var drUserContact = dtContact.Select(CS.ParentId + " = " + dataItem[CS.UsersId].ToString());
			foreach (var drContact in drUserContact)
			{
				if (!drContact[CS.ContactName].zIsNullOrEmpty() || !drContact[CS.ContactText].zIsNullOrEmpty())
					Contacts += drContact[CS.ContactName] + "(<a class='fontblue' href='tel:" + drContact[CS.ContactText] + "'>" + drContact[CS.ContactText] + "</a>)</br>";
			}

			e.Row.Cells[CU.GetColumnIndexByName(grdUser, "Contact")].Text = Contacts;

			var lnkUser = e.Row.FindControl("lnkUser") as LinkButton;
			lnkUser.Text = dataItem[CS.Name].ToString();
			lnkUser.CommandArgument = dataItem[CS.UsersId].ToString();
			lnkUser.Attributes.Add("controlid", dataItem[CS.UsersId].ToString());

			if (dataItem[CS.DesignationName].ToString().ToLower().Contains("admin"))
				lnkUser.Text += "*";
		}
	}

	protected void grdUser_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		txtUsersId.Text = grdUser.Rows[grdUser.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdUser, CS.UsersId)].Text;
		lnkEdit_OnClick(null, null);
	}

	#region Authority

	protected void lnkSetAuthority_OnClick(object sender, EventArgs e)
	{
		if (grdUser.zIsValidSelection(txtUsersId, "chkSelect", CS.UsersId))
		{
			SetControl(eControl.Authority);
		}
	}

	protected void lnkCancelAuthority_OnClick(object sender, EventArgs e)
	{
		SetControl(eControl.User);
	}

	#endregion

	#region Pagging

	protected void lnkPrev_Click(object sender, EventArgs e)
	{
		LoadUserGrid(ePageIndex.Prev);
	}

	protected void lnkNext_Click(object sender, EventArgs e)
	{
		LoadUserGrid(ePageIndex.Next);
	}

	protected void lnkFirst_Click(object sender, EventArgs e)
	{
		LoadUserGrid(ePageIndex.First);
	}

	protected void lnkLast_Click(object sender, EventArgs e)
	{
		LoadUserGrid(ePageIndex.Last);
	}

	protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
	{
		if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
		{
			txtGotoPageNo.Text = "1";
			txtGotoPageNo.Focus();
		}
		LoadUserGrid(ePageIndex.Custom);
	}

	protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
	{
		txtGotoPageNo.Text = "1";
		LoadUserGrid(ePageIndex.Custom);
		Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
	}

	#endregion

	private void SetControl(eControl Control)
	{
		divUser.Visible = false;
		pnlAuthority.Visible = false;

		PageIndex = 0;
		switch (Control)
		{
			case eControl.User:
				LoadUserGrid(ePageIndex.Custom);
				CheckVisibleButton();
				divUser.Visible = true;
				break;
			case eControl.Authority:
				ManageAuthority.LoadAuthorityDetail(0, txtUsersId.zToInt().Value, "User");
				pnlAuthority.Visible = true;
				break;
		}
	}

	private enum eControl
	{
		User = 1,
		Authority = 2,
	}
}
