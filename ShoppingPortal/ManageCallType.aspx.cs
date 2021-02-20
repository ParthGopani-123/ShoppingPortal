using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;

public partial class ManageCallType : CompressorPage
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
            lblFirmId.Text = CU.GetFirmId().ToString();
            CU.LoadDisplayPerPage(ref ddlRecordPerPage);

            LoadCallTypeGrid(ePageIndex.Custom);
            CheckVisibleButton();
        }

        Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
        Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
        Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

        try { grdCallType.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }


    private DataTable GetCallTypeDt(ePageIndex ePageIndex)
    {
        int? Status = null;
        if (chkActive.Checked && !chkDeactive.Checked)
            Status = (int)eStatus.Active;
        else if (!chkActive.Checked && chkDeactive.Checked)
            Status = (int)eStatus.Deactive;

        var objQuery = new Query()
        {
            MasterSearch = txtSearch.Text,
            FirmId = lblFirmId.zToInt(),
            eStatus = Status,
            eStatusNot = (int)eStatus.Delete,
        };

        #region Page Index

        int RecordPerPage = ddlRecordPerPage.zToInt().Value;
        int PageIndexTemp = PageIndex;

        CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref objQuery, ref txtGotoPageNo, ref lblCount);
        PageIndex = PageIndexTemp;

        #endregion Page Index

        return objQuery.Select(eSP.qry_CallType);
    }

    private void LoadCallTypeGrid(ePageIndex ePageIndex)
    {
        DataTable dtCallType = GetCallTypeDt(ePageIndex);

        if (dtCallType.Rows.Count > 0)
            lblCount.Text = dtCallType.Rows[0][CS.TotalRecord].ToString();
        else
            lblCount.Text = "0";

        divPaging.Visible = (dtCallType.Rows.Count > 0);

        txtGotoPageNo.Text = PageIndex.ToString();

        ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

        grdCallType.DataSource = dtCallType;
        grdCallType.DataBind();

        try { grdCallType.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void CheckVisibleButton()
    {
        var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCallType);

        lnkAdd.Visible = lnkEdit.Visible = objAuthority.IsAddEdit;
        lnkDelete.Visible = objAuthority.IsDelete;

        lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
        lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
    }


    protected void lnkAdd_OnClick(object sender, EventArgs e)
    {
        lblCallTypeId.Text = string.Empty;
        LoadCallTypeDetail();
        popupCallType.Show();
    }

    protected void lnkEdit_OnClick(object sender, EventArgs e)
    {
        if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCallType).IsAddEdit && (sender == null || grdCallType.zIsValidSelection(lblCallTypeId, "chkSelect", CS.CallTypeId)))
        {
            LoadCallTypeDetail();
            popupCallType.Show();
        }
    }

    protected void lnkEditCallType_OnClick(object sender, EventArgs e)
    {
        lblCallTypeId.Text = ((LinkButton)sender).CommandArgument.ToString();
        lnkEdit_OnClick(null, null);
    }

    protected void lnkRefresh_OnClick(object sender, EventArgs e)
    {
        LoadCallTypeGrid(ePageIndex.Custom);
    }

    protected void lnkActive_OnClick(object sender, EventArgs e)
    {
        if (grdCallType.zIsValidSelection(lblCallTypeId, "chkSelect", CS.CallTypeId))
        {
            if (new CallType()
            {
                CallTypeId = lblCallTypeId.zToInt(),
                eStatus = (int)eStatus.Active
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This CallType is already Active.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Active, "Active CallType", "Are You Sure To Active CallType?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDeactive_OnClick(object sender, EventArgs e)
    {
        if (grdCallType.zIsValidSelection(lblCallTypeId, "chkSelect", CS.CallTypeId))
        {
            if (new CallType()
            {
                CallTypeId = lblCallTypeId.zToInt(),
                eStatus = (int)eStatus.Deactive
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This CallType is already Deactive.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive CallType", "Are You Sure To Deactive CallType?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDelete_OnClick(object sender, EventArgs e)
    {
        if (grdCallType.zIsValidSelection(lblCallTypeId, "chkSelect", CS.CallTypeId))
        {
            Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete CallType", "Are You Sure To Delete CallType?");
            popupConfirmation.Show();
        }
    }

    protected void Control_CheckedChanged(object sender, EventArgs e)
    {
        LoadCallTypeGrid(ePageIndex.Custom);
        CheckVisibleButton();
    }


    private void ManageCallTypeStatus(eStatus Status)
    {
        new CallType()
        {
            CallTypeId = lblCallTypeId.zToInt(),
            eStatus = (int)Status
        }.Update();
    }

    protected void btnActive_OnClick(object sender, EventArgs e)
    {
        ManageCallTypeStatus(eStatus.Active);
        CU.ZMessage(eMsgType.Success, string.Empty, "CallType Activated Successfully.");
        LoadCallTypeGrid(ePageIndex.Custom);
    }

    protected void btnDeactive_OnClick(object sender, EventArgs e)
    {
        ManageCallTypeStatus(eStatus.Deactive);
        CU.ZMessage(eMsgType.Success, string.Empty, "CallType Deactive Successfully.");
        LoadCallTypeGrid(ePageIndex.Custom);
    }

    protected void btnDelete_OnClick(object sender, EventArgs e)
    {
        ManageCallTypeStatus(eStatus.Delete);
        CU.ZMessage(eMsgType.Success, string.Empty, "CallType Delete Successfully.");
        LoadCallTypeGrid(ePageIndex.Custom);
    }


    protected void grdCallType_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!IsAddEdit.HasValue)
                IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCallType).IsAddEdit;

            if (IsAddEdit.Value)
                e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdCallType, "Select$" + e.Row.RowIndex);

            if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdCallType, CS.eStatus)].Text) != (int)eStatus.Active)
                e.Row.Attributes["class"] = "GridDesableRow ";

            DataRowView dataItem = (DataRowView)e.Row.DataItem;
            var lnkEditCallType = e.Row.FindControl("lnkEditCallType") as LinkButton;
            var ltrCallType = e.Row.FindControl("ltrCallType") as Literal;

            lnkEditCallType.Visible = IsAddEdit.Value;
            ltrCallType.Visible = !IsAddEdit.Value;

            lnkEditCallType.Text = ltrCallType.Text = dataItem[CS.CallTypeName].ToString();
            lnkEditCallType.CommandArgument = dataItem[CS.CallTypeId].ToString();
        }
    }

    protected void grdCallType_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        lblCallTypeId.Text = grdCallType.Rows[grdCallType.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdCallType, CS.CallTypeId)].Text;
        lnkEdit_OnClick(null, null);
    }


    private void LoadCallTypeDetail()
    {
        txtCallTypeName.Focus();

        if (IsEditMode())
        {
            lblPopupTitle.Text = "Edit CallType";
            var objCallType = new CallType() { CallTypeId = lblCallTypeId.zToInt(), }.SelectList<CallType>()[0];

            txtCallTypeName.Text = objCallType.CallTypeName;
            txtSMSText.Text = objCallType.SMSText;
            chkSendSMS.Checked = objCallType.IsSendSMS == (int)eYesNo.Yes;
        }
        else
        {
            lblPopupTitle.Text = "New CallType";
            txtCallTypeName.Text = txtSMSText.Text = string.Empty;
            chkSendSMS.Checked = true;
        }
    }

    private bool IsEditMode()
    {
        return !lblCallTypeId.zIsNullOrEmpty();
    }

    private bool IsValidate()
    {
        if (txtCallTypeName.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter CallType Name.");
            txtCallTypeName.Focus();
            return false;
        }

        var dtCallType = new Query()
        {
            eStatusNot = (int)eStatus.Delete,
            FirmId = lblFirmId.zToInt(),
            CallTypeName = txtCallTypeName.Text.Trim(),
        }.Select(eSP.qry_CallType);

        if (dtCallType.Rows.Count > 0 && dtCallType.Rows[0][CS.CallTypeId].ToString() != lblCallTypeId.Text)
        {
            string Status = dtCallType.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
            CU.ZMessage(eMsgType.Error, string.Empty, "This CallType is already exist" + Status + ".");
            txtCallTypeName.Focus();
            return false;
        }

        return true;
    }

    private bool SaveData()
    {
        if (!IsValidate())
        {
            popupCallType.Show();
            return false;
        }

        string Message = string.Empty;

        var objCallType = new CallType()
        {
            FirmId = lblFirmId.zToInt(),
            CallTypeName = txtCallTypeName.Text.Trim().zFirstCharToUpper(),
            SMSText = txtSMSText.Text,
            IsSendSMS = chkSendSMS.Checked ? (int)eYesNo.Yes : (int)eYesNo.No
        };

        if (IsEditMode())
        {
            objCallType.CallTypeId = lblCallTypeId.zToInt();
            objCallType.Update();

            Message = "CallType Detail Change Sucessfully.";
        }
        else
        {
            objCallType.eStatus = (int)eStatus.Active;
            objCallType.Insert();

            Message = "New CallType Added Sucessfully.";
        }

        CU.ZMessage(eMsgType.Success, string.Empty, Message);

        return true;
    }

    protected void btnSave_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadCallTypeGrid(ePageIndex.Custom);
        }
    }

    protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadCallTypeGrid(ePageIndex.Custom);
            lnkAdd_OnClick(null, null);
        }
    }


    #region Pagging


    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        LoadCallTypeGrid(ePageIndex.Prev);
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        LoadCallTypeGrid(ePageIndex.Next);
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        LoadCallTypeGrid(ePageIndex.First);
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        LoadCallTypeGrid(ePageIndex.Last);
    }

    protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
    {
        if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
        {
            txtGotoPageNo.Text = "1";
            txtGotoPageNo.Focus();
        }
        LoadCallTypeGrid(ePageIndex.Custom);
    }

    protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
    {
        txtGotoPageNo.Text = "1";
        LoadCallTypeGrid(ePageIndex.Custom);
        Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
    }


    #endregion
}
