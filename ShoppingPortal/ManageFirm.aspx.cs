using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;

public partial class ManageFirm : CompressorPage
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

            LoadFirmGrid(ePageIndex.Custom);
            CheckVisibleButton();
        }

        Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
        Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
        Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

        ManageAddress.btnPagePostback += new EventHandler(AddressPostback);

        try { grdFirm.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }


    private DataTable GetFirmDt(ePageIndex ePageIndex)
    {
        int? Status = null;
        if (chkActive.Checked && !chkDeactive.Checked)
            Status = (int)eStatus.Active;
        else if (!chkActive.Checked && chkDeactive.Checked)
            Status = (int)eStatus.Deactive;

        var objQuery = new Query()
        {
            MasterSearch = txtSearch.Text,
            OrganizationId = CU.GetOrganizationId(),
            eStatus = Status,
            eStatusNot = (int)eStatus.Delete,
        };

        #region Page Index

        int RecordPerPage = ddlRecordPerPage.zToInt().Value;
        int PageIndexTemp = PageIndex;

        CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref objQuery, ref txtGotoPageNo, ref lblCount);
        PageIndex = PageIndexTemp;

        #endregion Page Index

        return objQuery.Select(eSP.qry_Firm);
    }

    private void LoadFirmGrid(ePageIndex ePageIndex)
    {
        DataTable dtFirm = GetFirmDt(ePageIndex);

        if (dtFirm.Rows.Count > 0)
            lblCount.Text = dtFirm.Rows[0][CS.TotalRecord].ToString();
        else
            lblCount.Text = "0";

        divPaging.Visible = (dtFirm.Rows.Count > 0);

        txtGotoPageNo.Text = PageIndex.ToString();

        ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

        grdFirm.DataSource = dtFirm;
        grdFirm.DataBind();

        try { grdFirm.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void CheckVisibleButton()
    {
        var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.Firm);

        lnkAdd.Visible = lnkEdit.Visible = objAuthority.IsAddEdit;
        lnkDelete.Visible = objAuthority.IsDelete;

        lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
        lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
    }


    protected void lnkAdd_OnClick(object sender, EventArgs e)
    {
        lblFirmId.Text = string.Empty;
        LoadFirmDetail();
        popupFirm.Show();
    }

    protected void lnkEdit_OnClick(object sender, EventArgs e)
    {
        if (CU.GetAuthority(CU.GetUsersId(), eAuthority.Firm).IsAddEdit && (sender == null || grdFirm.zIsValidSelection(lblFirmId, "chkSelect", CS.FirmId)))
        {
            LoadFirmDetail();
            popupFirm.Show();
        }
    }

    protected void lnkEditFirm_OnClick(object sender, EventArgs e)
    {
        lblFirmId.Text = ((LinkButton)sender).CommandArgument.ToString();
        lnkEdit_OnClick(null, null);
    }

    protected void lnkRefresh_OnClick(object sender, EventArgs e)
    {
        LoadFirmGrid(ePageIndex.Custom);
    }

    protected void lnkActive_OnClick(object sender, EventArgs e)
    {
        if (grdFirm.zIsValidSelection(lblFirmId, "chkSelect", CS.FirmId))
        {
            if (new Firm()
            {
                FirmId = lblFirmId.zToInt(),
                eStatus = (int)eStatus.Active
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Firm is already Active.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Active, "Active Firm", "Are You Sure To Active Firm?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDeactive_OnClick(object sender, EventArgs e)
    {
        if (grdFirm.zIsValidSelection(lblFirmId, "chkSelect", CS.FirmId))
        {
            if (new Firm()
            {
                FirmId = lblFirmId.zToInt(),
                eStatus = (int)eStatus.Deactive
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Firm is already Deactive.");
                return;
            }

            string Message = string.Empty;
            if (CU.IsFirmUsed(lblFirmId.zToInt().Value, ref Message))
            {
                CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Deactive.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive Firm", "Are You Sure To Deactive Firm?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDelete_OnClick(object sender, EventArgs e)
    {
        if (grdFirm.zIsValidSelection(lblFirmId, "chkSelect", CS.FirmId))
        {
            string Message = string.Empty;
            if (CU.IsFirmUsed(lblFirmId.zToInt().Value, ref Message))
            {
                CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Delete.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Firm", "Are You Sure To Delete Firm?");
            popupConfirmation.Show();
        }
    }

    protected void Control_CheckedChanged(object sender, EventArgs e)
    {
        LoadFirmGrid(ePageIndex.Custom);
        CheckVisibleButton();
    }


    private void ManageFirmStatus(eStatus Status)
    {
        new Firm()
        {
            FirmId = lblFirmId.zToInt(),
            eStatus = (int)Status
        }.Update();
    }

    protected void btnActive_OnClick(object sender, EventArgs e)
    {
        ManageFirmStatus(eStatus.Active);
        CU.ZMessage(eMsgType.Success, string.Empty, "Firm Activated Successfully.");
        LoadFirmGrid(ePageIndex.Custom);
    }

    protected void btnDeactive_OnClick(object sender, EventArgs e)
    {
        ManageFirmStatus(eStatus.Deactive);
        CU.ZMessage(eMsgType.Success, string.Empty, "Firm Deactive Successfully.");
        LoadFirmGrid(ePageIndex.Custom);
    }

    protected void btnDelete_OnClick(object sender, EventArgs e)
    {
        ManageFirmStatus(eStatus.Delete);
        CU.ZMessage(eMsgType.Success, string.Empty, "Firm Delete Successfully.");
        LoadFirmGrid(ePageIndex.Custom);
    }


    protected void grdFirm_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!IsAddEdit.HasValue)
                IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.Firm).IsAddEdit;

            if (IsAddEdit.Value)
                e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdFirm, "Select$" + e.Row.RowIndex);

            if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdFirm, CS.eStatus)].Text) != (int)eStatus.Active)
                e.Row.Attributes["class"] = "GridDesableRow ";

            DataRowView dataItem = (DataRowView)e.Row.DataItem;
            var lnkEditFirm = e.Row.FindControl("lnkEditFirm") as LinkButton;
            var ltrFirm = e.Row.FindControl("ltrFirm") as Literal;

            lnkEditFirm.Visible = IsAddEdit.Value;
            ltrFirm.Visible = !IsAddEdit.Value;

            lnkEditFirm.Text = ltrFirm.Text = dataItem[CS.FirmName].ToString();
            lnkEditFirm.CommandArgument = dataItem[CS.FirmId].ToString();
        }
    }

    protected void grdFirm_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        lblFirmId.Text = grdFirm.Rows[grdFirm.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdFirm, CS.FirmId)].Text;
        lnkEdit_OnClick(null, null);
    }


    private void LoadOrganization()
    {
        var dtOrganization = new Query() { eStatus = (int)eStatus.Active }.Select(eSP.qry_Organization);
        CU.FillDropdown(ref ddlOrganization, dtOrganization, "-- Select Organization --", CS.OrganizationId, CS.OrganizationName);
    }

    private void LoadFirmDetail()
    {
        ddlOrganization.Focus();
        LoadOrganization();

        if (IsEditMode())
        {
            lblPopupTitle.Text = "Edit Firm";
            var objFirm = new Firm() { FirmId = lblFirmId.zToInt() }.SelectList<Firm>()[0];

            ddlOrganization.SelectedValue = objFirm.OrganizationId.ToString();
            txtFirmName.Text = objFirm.FirmName;
            lblAddressId.Text = objFirm.AddressId.HasValue ? objFirm.AddressId.ToString() : string.Empty;
        }
        else
        {
            lblPopupTitle.Text = "New Firm";
            ddlOrganization.SelectedValue = "0";
            txtFirmName.Text = lblAddressId.Text = string.Empty;
        }

        ManageAddress.LoadAddreessDetail(lblAddressId.zToInt(), false);

    }

    private bool IsEditMode()
    {
        return !lblFirmId.zIsNullOrEmpty();
    }

    private bool IsValidate()
    {
        if (!ddlOrganization.zIsSelect())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Organization.");
            ddlOrganization.Focus();
            return false;
        }

        if (txtFirmName.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Firm Name.");
            txtFirmName.Focus();
            return false;
        }

        var dtFirm = new Query()
        {
            eStatusNot = (int)eStatus.Delete,
            OrganizationId = ddlOrganization.zToInt(),
            FirmName = txtFirmName.Text.Trim()
        }.Select(eSP.qry_Firm);

        if (dtFirm.Rows.Count > 0 && dtFirm.Rows[0][CS.FirmId].ToString() != lblFirmId.Text)
        {
            string Status = dtFirm.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
            CU.ZMessage(eMsgType.Error, string.Empty, "This Firm is already exist" + Status + ".");
            txtFirmName.Focus();
            return false;
        }

        if (!ManageAddress.IsValidateAddress())
            return false;

        return true;
    }

    private bool SaveData()
    {
        if (!IsValidate())
            return false;

        string Message = string.Empty;

        lblAddressId.Text = ManageAddress.SaveAddress(lblAddressId.zToInt()).ToString();

        var objFirm = new Firm()
        {
            OrganizationId = ddlOrganization.zToInt(),
            FirmName = txtFirmName.Text.Trim().zFirstCharToUpper(),
            AddressId = lblAddressId.zToInt().HasValue ? lblAddressId.zToInt() : NullValue.intNull,
        };

        if (IsEditMode())
        {
            objFirm.FirmId = lblFirmId.zToInt();
            objFirm.Update();

            Message = "Firm Detail Change Sucessfully.";
        }
        else
        {
            objFirm.eStatus = (int)eStatus.Active;
            objFirm.FirmId = objFirm.Insert();

            objFirm.PriceListId = new PriceList()
            {
                FirmId = objFirm.FirmId,
                PriceListName = "Price",
                eStatus = (int)eStatus.Active,
            }.Insert();

            objFirm.Update();

            Message = "New Firm Added Sucessfully.";
        }

        CU.ZMessage(eMsgType.Success, string.Empty, Message);

        return true;
    }

    protected void btnSave_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadFirmGrid(ePageIndex.Custom);
        }
    }

    protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadFirmGrid(ePageIndex.Custom);
            lnkAdd_OnClick(null, null);
        }
    }

    protected void AddressPostback(object sender, EventArgs e)
    {
        popupFirm.Show();
    }


    #region Pagging


    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        LoadFirmGrid(ePageIndex.Prev);
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        LoadFirmGrid(ePageIndex.Next);
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        LoadFirmGrid(ePageIndex.First);
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        LoadFirmGrid(ePageIndex.Last);
    }

    protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
    {
        if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
        {
            txtGotoPageNo.Text = "1";
            txtGotoPageNo.Focus();
        }
        LoadFirmGrid(ePageIndex.Custom);
    }

    protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
    {
        txtGotoPageNo.Text = "1";
        LoadFirmGrid(ePageIndex.Custom);
        Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
    }


    #endregion
}
