using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;

public partial class ManageOrderStatus : CompressorPage
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

			LoadOrderStatusGrid(ePageIndex.Custom);
			CheckVisibleButton();
		}

		Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
		Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
		Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

		try { grdOrderStatus.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}


	private DataTable GetOrderStatusDt(ePageIndex ePageIndex)
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

		return objQuery.Select(eSP.qry_OrderStatus);
	}

	private void LoadOrderStatusGrid(ePageIndex ePageIndex)
	{
		DataTable dtOrderStatus = GetOrderStatusDt(ePageIndex);

		if (dtOrderStatus.Rows.Count > 0)
			lblCount.Text = dtOrderStatus.Rows[0][CS.TotalRecord].ToString();
		else
			lblCount.Text = "0";

		divPaging.Visible = (dtOrderStatus.Rows.Count > 0);

		txtGotoPageNo.Text = PageIndex.ToString();

		ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

		grdOrderStatus.DataSource = dtOrderStatus;
		grdOrderStatus.DataBind();

		try { grdOrderStatus.HeaderRow.TableSection = TableRowSection.TableHeader; }
		catch { }
	}

	private void CheckVisibleButton()
	{
		var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageOrderStatus);

		lnkAdd.Visible = lnkEdit.Visible = objAuthority.IsAddEdit;
		lnkDelete.Visible = objAuthority.IsDelete;

		lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
		lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
	}


	protected void lnkAdd_OnClick(object sender, EventArgs e)
	{
		lblOrderStatusId.Text = string.Empty;
		LoadOrderStatusDetail();
		popupOrderStatus.Show();
	}

	protected void lnkEdit_OnClick(object sender, EventArgs e)
	{
		if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageOrderStatus).IsAddEdit && (sender == null || grdOrderStatus.zIsValidSelection(lblOrderStatusId, "chkSelect", CS.OrderStatusId)))
		{
			LoadOrderStatusDetail();
			popupOrderStatus.Show();
		}
	}

	protected void lnkEditOrderStatus_OnClick(object sender, EventArgs e)
	{
		lblOrderStatusId.Text = ((LinkButton)sender).CommandArgument.ToString();
		lnkEdit_OnClick(null, null);
	}

	protected void lnkRefresh_OnClick(object sender, EventArgs e)
	{
		LoadOrderStatusGrid(ePageIndex.Custom);
	}

	protected void lnkActive_OnClick(object sender, EventArgs e)
	{
		if (grdOrderStatus.zIsValidSelection(lblOrderStatusId, "chkSelect", CS.OrderStatusId))
		{
			if (new OrderStatus()
			{
				OrderStatusId = lblOrderStatusId.zToInt(),
				eStatus = (int)eStatus.Active
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This Status is already Active.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Active, "Active Status", "Are You Sure To Active Status?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDeactive_OnClick(object sender, EventArgs e)
	{
		if (grdOrderStatus.zIsValidSelection(lblOrderStatusId, "chkSelect", CS.OrderStatusId))
		{
			if (new OrderStatus()
			{
				OrderStatusId = lblOrderStatusId.zToInt(),
				eStatus = (int)eStatus.Deactive
			}.SelectCount() > 0)
			{
				CU.ZMessage(eMsgType.Error, string.Empty, "This Status is already Deactive.");
				return;
			}

			string Message = string.Empty;
			if (CU.IsOrderStatusUsed(lblOrderStatusId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Deactive.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive Status", "Are You Sure To Deactive Status?");
			popupConfirmation.Show();
		}
	}

	protected void lnkDelete_OnClick(object sender, EventArgs e)
	{
		if (grdOrderStatus.zIsValidSelection(lblOrderStatusId, "chkSelect", CS.OrderStatusId))
		{
			string Message = string.Empty;
			if (CU.IsOrderStatusUsed(lblOrderStatusId.zToInt().Value, ref Message))
			{
				CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Delete.");
				return;
			}

			Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Status", "Are You Sure To Delete Status?");
			popupConfirmation.Show();
		}
	}

	protected void Control_CheckedChanged(object sender, EventArgs e)
	{
		LoadOrderStatusGrid(ePageIndex.Custom);
		CheckVisibleButton();
	}


	private void ManageOrderStatusStatus(eStatus Status)
	{
		new OrderStatus()
		{
			OrderStatusId = lblOrderStatusId.zToInt(),
			eStatus = (int)Status
		}.Update();
	}

	protected void btnActive_OnClick(object sender, EventArgs e)
	{
		ManageOrderStatusStatus(eStatus.Active);
		CU.ZMessage(eMsgType.Success, string.Empty, "Status Activated Successfully.");
		LoadOrderStatusGrid(ePageIndex.Custom);
	}

	protected void btnDeactive_OnClick(object sender, EventArgs e)
	{
		ManageOrderStatusStatus(eStatus.Deactive);
		CU.ZMessage(eMsgType.Success, string.Empty, "Status Deactive Successfully.");
		LoadOrderStatusGrid(ePageIndex.Custom);
	}

	protected void btnDelete_OnClick(object sender, EventArgs e)
	{
		ManageOrderStatusStatus(eStatus.Delete);
		CU.ZMessage(eMsgType.Success, string.Empty, "Status Delete Successfully.");
		LoadOrderStatusGrid(ePageIndex.Custom);
	}


	protected void grdOrderStatus_OnRowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			if (!IsAddEdit.HasValue)
				IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageOrderStatus).IsAddEdit;

			if (IsAddEdit.Value)
				e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdOrderStatus, "Select$" + e.Row.RowIndex);

			if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdOrderStatus, CS.eStatus)].Text) != (int)eStatus.Active)
				e.Row.Attributes["class"] = "GridDesableRow ";

			DataRowView dataItem = (DataRowView)e.Row.DataItem;
			var lnkEditOrderStatus = e.Row.FindControl("lnkEditOrderStatus") as LinkButton;
			var ltrOrderStatus = e.Row.FindControl("ltrOrderStatus") as Literal;
			var ltrStatusType = e.Row.FindControl("ltrStatusType") as Literal;

			lnkEditOrderStatus.Visible = IsAddEdit.Value;
			ltrOrderStatus.Visible = !IsAddEdit.Value;

			lnkEditOrderStatus.Text = ltrOrderStatus.Text = dataItem[CS.StatusName].ToString();
			lnkEditOrderStatus.CommandArgument = dataItem[CS.OrderStatusId].ToString();

			ltrStatusType.Text = CU.GetDescription((eStatusType)dataItem[CS.eStatusType].zToInt());
		}
	}

	protected void grdOrderStatus_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		lblOrderStatusId.Text = grdOrderStatus.Rows[grdOrderStatus.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdOrderStatus, CS.OrderStatusId)].Text;
		lnkEdit_OnClick(null, null);
	}


	private void LoadeStatusType()
	{
		CU.FillEnumddl<eStatusType>(ref ddleStatusType, "-- Select Type --");
	}

	private void LoadOrderStatusDetail()
	{
		LoadeStatusType();
		txtStatusName.Focus();

		if (IsEditMode())
		{
			lblPopupTitle.Text = "Edit Status";
			var objOrderStatus = new OrderStatus() { OrderStatusId = lblOrderStatusId.zToInt(), }.SelectList<OrderStatus>()[0];
			txtStatusName.Text = objOrderStatus.StatusName;
			txtSerialNo.Text = objOrderStatus.SerialNo.ToString();
			ddleStatusType.SelectedValue = objOrderStatus.eStatusType.ToString();
		}
		else
		{
			lblPopupTitle.Text = "New OrderStatus";
			txtStatusName.Text = string.Empty;

			var drMaxSerialNo = new Query() { eStatusNot = (int)eStatus.Delete }.Select(eSP.qry_Max_OrderStatusSerialNo).Rows[0];
			int? SerialNo = drMaxSerialNo[CS.SerialNo].zToInt();
			txtSerialNo.Text = SerialNo.HasValue ? (SerialNo + 1).ToString() : "1";

			ddleStatusType.SelectedValue = "0";
		}
	}

	private bool IsEditMode()
	{
		return !lblOrderStatusId.zIsNullOrEmpty();
	}

	private bool IsValidate()
	{
		if (txtStatusName.zIsNullOrEmpty())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Status Name.");
			txtStatusName.Focus();
			return false;
		}

		var dtOrderStatus = new Query()
		{
			eStatusNot = (int)eStatus.Delete,
			OrganizationId = lblOrganizationId.zToInt(),
			StatusName = txtStatusName.Text.Trim(),
		}.Select(eSP.qry_OrderStatus);

		if (dtOrderStatus.Rows.Count > 0 && dtOrderStatus.Rows[0][CS.OrderStatusId].ToString() != lblOrderStatusId.Text)
		{
			string Status = dtOrderStatus.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
			CU.ZMessage(eMsgType.Error, string.Empty, "This Status is already exist" + Status + ".");
			txtStatusName.Focus();
			return false;
		}

		if (txtSerialNo.zIsNullOrEmpty())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter SerialNo.");
			txtSerialNo.Focus();
			return false;
		}

		if (!txtSerialNo.zIsInteger(false))
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid SerialNo.");
			txtSerialNo.Focus();
			return false;
		}

		if (!ddleStatusType.zIsSelect())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Type.");
			ddleStatusType.Focus();
			return false;
		}

		return true;
	}

	private bool SaveData()
	{
		if (!IsValidate())
			return false;

		string Message = string.Empty;

		var objOrderStatus = new OrderStatus()
		{
			OrganizationId = lblOrganizationId.zToInt(),
			StatusName = txtStatusName.Text.Trim().zFirstCharToUpper(),
			SerialNo = txtSerialNo.zToInt(),
			eStatusType = ddleStatusType.zToInt(),
		};

		if (IsEditMode())
		{
			objOrderStatus.OrderStatusId = lblOrderStatusId.zToInt();
			objOrderStatus.Update();

			Message = "Status Detail Change Sucessfully.";
		}
		else
		{
			objOrderStatus.eStatus = (int)eStatus.Active;
			objOrderStatus.Insert();

			Message = "New Status Added Sucessfully.";
		}

		CU.ZMessage(eMsgType.Success, string.Empty, Message);

		return true;
	}

	protected void btnSave_OnClick(object sender, EventArgs e)
	{
		if (SaveData())
		{
			LoadOrderStatusGrid(ePageIndex.Custom);
		}
	}

	protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
	{
		if (SaveData())
		{
			LoadOrderStatusGrid(ePageIndex.Custom);
			lnkAdd_OnClick(null, null);
		}
	}


	#region Pagging


	protected void lnkPrev_Click(object sender, EventArgs e)
	{
		LoadOrderStatusGrid(ePageIndex.Prev);
	}

	protected void lnkNext_Click(object sender, EventArgs e)
	{
		LoadOrderStatusGrid(ePageIndex.Next);
	}

	protected void lnkFirst_Click(object sender, EventArgs e)
	{
		LoadOrderStatusGrid(ePageIndex.First);
	}

	protected void lnkLast_Click(object sender, EventArgs e)
	{
		LoadOrderStatusGrid(ePageIndex.Last);
	}

	protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
	{
		if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
		{
			txtGotoPageNo.Text = "1";
			txtGotoPageNo.Focus();
		}
		LoadOrderStatusGrid(ePageIndex.Custom);
	}

	protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
	{
		txtGotoPageNo.Text = "1";
		LoadOrderStatusGrid(ePageIndex.Custom);
		Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
	}


	#endregion
}
