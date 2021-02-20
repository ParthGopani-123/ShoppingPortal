using System;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;

public partial class ManageCustomer : CompressorPage
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

	protected void Page_Load(object sender, EventArgs e)
	{
		LoginUtilities.CheckSession();

		if (!IsPostBack)
		{
			lblUsersId.Text = CU.GetUsersId().ToString();
			CU.LoadDisplayPerPage(ref ddlRecordPerPage);

			LoadCustomerGrid(ePageIndex.Custom);
			CheckVisibleButton();
		}

		Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
		Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
		Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

		ManageCustomerCC.btnSaveCustomer_OnClick += new EventHandler(btnSaveCustomer_OnClick);
		ManageCustomerCC.btnSaveAndNewCustomer_OnClick += new EventHandler(btnSaveAndNewCustomer_OnClick);

		try { grdCustomer.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}


	private void LoadCustomerGrid(ePageIndex ePageIndex)
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
			UsersId = lblUsersId.zToInt(),
		};

		#region Page Index

		int RecordPerPage = ddlRecordPerPage.zToInt().Value;
		int PageIndexTemp = PageIndex;

		CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref objQuery, ref txtGotoPageNo, ref lblCount);
		PageIndex = PageIndexTemp;

		#endregion Page Index

		DataTable dtCustomer = objQuery.Select(eSP.qry_Customer);

		#region Count Total

		lblCount.Text = dtCustomer.Rows.Count.ToString();

		divPaging.Visible = (dtCustomer.Rows.Count > 0);

		txtGotoPageNo.Text = PageIndex.ToString();

		ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

		#endregion

		grdCustomer.DataSource = dtCustomer;
		grdCustomer.DataBind();

		try { grdCustomer.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}

	private void CheckVisibleButton()
	{
		lnkActive.Visible = ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked));
		lnkDeactive.Visible = ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked));
	}


	protected void btnSaveCustomer_OnClick(object sender, EventArgs e)
	{
		LoadCustomerGrid(ePageIndex.Custom);
		popupCustomer.Hide();
	}

	protected void btnSaveAndNewCustomer_OnClick(object sender, EventArgs e)
	{
		LoadCustomerGrid(ePageIndex.Custom);
		lnkAdd_OnClick(null, null);
	}


	protected void lnkAdd_OnClick(object sender, EventArgs e)
	{
		ManageCustomerCC.SetCustomerId = string.Empty;
		ManageCustomerCC.LoadCustomerDetail(null);
		popupCustomer.Show();
	}

	protected void lnkEditCustomer_OnClick(object sender, EventArgs e)
	{
		txtCustomerId.Text = ((LinkButton)sender).CommandArgument.ToString();
		lnkEdit_OnClick(null, null);
	}

	protected void lnkEdit_OnClick(object sender, EventArgs e)
	{
		if (sender == null || grdCustomer.zIsValidSelection(txtCustomerId, "chkSelect", CS.CustomerId))
		{
			ManageCustomerCC.SetCustomerId = txtCustomerId.Text;
			ManageCustomerCC.LoadCustomerDetail(null);
			popupCustomer.Show();
		}
	}

	protected void lnkRefresh_OnClick(object sender, EventArgs e)
	{
		LoadCustomerGrid(ePageIndex.Custom);
	}

	protected void lnkActive_OnClick(object sender, EventArgs e)
	{
		if (grdCustomer.zIsValidSelection(txtCustomerId, "chkSelect", CS.CustomerId))
		{
			if (new Customer()
			{
				CustomerId = txtCustomerId.zToInt(),
				eStatus = (int)eStatus.Active
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This Customer is already Active.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Active, "Active Customer", "Are You Sure To Active this Customer?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDeactive_OnClick(object sender, EventArgs e)
	{
		if (grdCustomer.zIsValidSelection(txtCustomerId, "chkSelect", CS.CustomerId))
		{
			if (new Customer()
			{
				CustomerId = txtCustomerId.zToInt(),
				eStatus = (int)eStatus.Deactive
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This Customer is already Deactive.");
				return;
			}

			string Message = string.Empty;
			if (CU.IsCustomerUsed(txtCustomerId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", So You can not Deactive It.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive Customer", "Are You Sure To Deactive this Customer?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDelete_OnClick(object sender, EventArgs e)
	{
		if (grdCustomer.zIsValidSelection(txtCustomerId, "chkSelect", CS.CustomerId))
		{
			string Message = string.Empty;
			if (CU.IsCustomerUsed(txtCustomerId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Delete.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Customer", "Are You Sure To Delete this Customer?");
			popupConfirmation.Show();
		}
	}


	protected void btnActive_OnClick(object sender, EventArgs e)
	{
		new Customer() { CustomerId = txtCustomerId.zToInt(), eStatus = (int)eStatus.Active }.Update();
		CU.ZMessage(eMsgType.Success, string.Empty, "Customer Activet Successfully");
		LoadCustomerGrid(ePageIndex.Custom);
	}

	protected void btnDeactive_OnClick(object sender, EventArgs e)
	{
		new Customer() { CustomerId = txtCustomerId.zToInt(), eStatus = (int)eStatus.Deactive }.Update();
		CU.ZMessage(eMsgType.Success, string.Empty, "Customer Deactivet Successfully");
		LoadCustomerGrid(ePageIndex.Custom);
	}

	protected void btnDelete_OnClick(object sender, EventArgs e)
	{
		new Customer() { CustomerId = txtCustomerId.zToInt(), eStatus = (int)eStatus.Delete }.Update();
		CU.ZMessage(eMsgType.Success, string.Empty, "Customer Deleted Successfully");
		LoadCustomerGrid(ePageIndex.Custom);
	}


	protected void Control_CheckedChanged(object sender, EventArgs e)
	{
		LoadCustomerGrid(ePageIndex.Custom);
		CheckVisibleButton();
	}


	protected void grdCustomer_OnRowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdCustomer, "Select$" + e.Row.RowIndex);
			if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdCustomer, CS.eStatus)].Text) != (int)eStatus.Active)
				e.Row.Attributes["class"] = "GridDesableRow ";

			DataRowView dataItem = (DataRowView)e.Row.DataItem;
			var lnkCustomer = e.Row.FindControl("lnkCustomer") as LinkButton;
			var lnkWhatsAppNo = e.Row.FindControl("lnkWhatsAppNo") as LinkButton;
			var lnkMobile = e.Row.FindControl("lnkMobile") as LinkButton;

			lnkWhatsAppNo.Text = dataItem[CS.WhatsAppNo].ToString();
			lnkWhatsAppNo.PostBackUrl = "tel:" + dataItem[CS.WhatsAppNo].ToString();

			lnkMobile.Text = dataItem[CS.MobileNo].ToString();
			lnkMobile.PostBackUrl = "tel:" + dataItem[CS.MobileNo].ToString();

			lnkCustomer.Text = dataItem[CS.Name].ToString();
			lnkCustomer.CommandArgument = dataItem[CS.CustomerId].ToString();
			lnkCustomer.Attributes.Add("controlid", dataItem[CS.CustomerId].ToString());
		}
	}

	protected void grdCustomer_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		txtCustomerId.Text = grdCustomer.Rows[grdCustomer.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdCustomer, CS.CustomerId)].Text;
		lnkEdit_OnClick(null, null);
	}

	#region Pagging

	protected void lnkPrev_Click(object sender, EventArgs e)
	{
		LoadCustomerGrid(ePageIndex.Prev);
	}

	protected void lnkNext_Click(object sender, EventArgs e)
	{
		LoadCustomerGrid(ePageIndex.Next);
	}

	protected void lnkFirst_Click(object sender, EventArgs e)
	{
		LoadCustomerGrid(ePageIndex.First);
	}

	protected void lnkLast_Click(object sender, EventArgs e)
	{
		LoadCustomerGrid(ePageIndex.Last);
	}

	protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
	{
		if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
		{
			txtGotoPageNo.Text = "1";
			txtGotoPageNo.Focus();
		}
		LoadCustomerGrid(ePageIndex.Custom);
	}

	protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
	{
		txtGotoPageNo.Text = "1";
		LoadCustomerGrid(ePageIndex.Custom);
		Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
	}

	#endregion
}
