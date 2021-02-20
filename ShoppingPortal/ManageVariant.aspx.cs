using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;

public partial class ManageVariant : CompressorPage
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

            SetControl(eControl.Variant);
        }

        Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
        Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
        Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

        ConfirmationpopupValue.btnDeletePopup_OnClick += new EventHandler(btnDeleteValue_OnClick);

        try { grdVariant.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }

        try { grdVariantValue.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    #region Variant

    private DataTable GetVariantDt(ePageIndex ePageIndex)
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

        return objQuery.Select(eSP.qry_Variant);
    }

    private void LoadVariantGrid(ePageIndex ePageIndex)
    {
        DataTable dtVariant = GetVariantDt(ePageIndex);

        if (dtVariant.Rows.Count > 0)
            lblCount.Text = dtVariant.Rows[0][CS.TotalRecord].ToString();
        else
            lblCount.Text = "0";

        divPaging.Visible = (dtVariant.Rows.Count > 0);

        txtGotoPageNo.Text = PageIndex.ToString();

        ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

        grdVariant.DataSource = dtVariant;
        grdVariant.DataBind();

        try { grdVariant.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void CheckVisibleButton()
    {
        var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageVariant);

        lnkAdd.Visible = lnkEdit.Visible = objAuthority.IsAddEdit;
        lnkDelete.Visible = objAuthority.IsDelete;

        lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
        lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
    }


    protected void lnkAdd_OnClick(object sender, EventArgs e)
    {
        lblVariantId.Text = string.Empty;
        LoadVariantDetail();
        popupVariant.Show();
    }

    protected void lnkEdit_OnClick(object sender, EventArgs e)
    {
        if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageVariant).IsAddEdit && (sender == null || grdVariant.zIsValidSelection(lblVariantId, "chkSelect", CS.VariantId)))
        {
            LoadVariantDetail();
            popupVariant.Show();
        }
    }

    protected void lnkEditVariant_OnClick(object sender, EventArgs e)
    {
        lblVariantId.Text = ((LinkButton)sender).CommandArgument.ToString();
        lnkEdit_OnClick(null, null);
    }

    protected void lnkRefresh_OnClick(object sender, EventArgs e)
    {
        LoadVariantGrid(ePageIndex.Custom);
    }

    protected void lnkActive_OnClick(object sender, EventArgs e)
    {
        if (grdVariant.zIsValidSelection(lblVariantId, "chkSelect", CS.VariantId))
        {
            if (new Variant()
            {
                VariantId = lblVariantId.zToInt(),
                eStatus = (int)eStatus.Active
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Variant is already Active.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Active, "Active Variant", "Are You Sure To Active Variant?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDeactive_OnClick(object sender, EventArgs e)
    {
        if (grdVariant.zIsValidSelection(lblVariantId, "chkSelect", CS.VariantId))
        {
            if (new Variant()
            {
                VariantId = lblVariantId.zToInt(),
                eStatus = (int)eStatus.Deactive
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Variant is already Deactive.");
                return;
            }

            string Message = string.Empty;
            if (CU.IsVariantUsed(lblVariantId.zToInt().Value, ref Message))
            {
                CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Deactive.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive Variant", "Are You Sure To Deactive Variant?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDelete_OnClick(object sender, EventArgs e)
    {
        if (grdVariant.zIsValidSelection(lblVariantId, "chkSelect", CS.VariantId))
        {
            string Message = string.Empty;
            if (CU.IsVariantUsed(lblVariantId.zToInt().Value, ref Message))
            {
                CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Delete.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Variant", "Are You Sure To Delete Variant?");
            popupConfirmation.Show();
        }
    }

    protected void Control_CheckedChanged(object sender, EventArgs e)
    {
        LoadVariantGrid(ePageIndex.Custom);
        CheckVisibleButton();
    }


    private void ManageVariantStatus(eStatus Status)
    {
        new Variant()
        {
            VariantId = lblVariantId.zToInt(),
            eStatus = (int)Status
        }.Update();
    }

    protected void btnActive_OnClick(object sender, EventArgs e)
    {
        ManageVariantStatus(eStatus.Active);
        CU.ZMessage(eMsgType.Success, string.Empty, "Variant Activated Successfully.");
        LoadVariantGrid(ePageIndex.Custom);
    }

    protected void btnDeactive_OnClick(object sender, EventArgs e)
    {
        ManageVariantStatus(eStatus.Deactive);
        CU.ZMessage(eMsgType.Success, string.Empty, "Variant Deactive Successfully.");
        LoadVariantGrid(ePageIndex.Custom);
    }

    protected void btnDelete_OnClick(object sender, EventArgs e)
    {
        ManageVariantStatus(eStatus.Delete);
        CU.ZMessage(eMsgType.Success, string.Empty, "Variant Delete Successfully.");
        LoadVariantGrid(ePageIndex.Custom);
    }


    protected void grdVariant_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!IsAddEdit.HasValue)
                IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageVariant).IsAddEdit;

            if (IsAddEdit.Value)
                e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdVariant, "Select$" + e.Row.RowIndex);

            if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdVariant, CS.eStatus)].Text) != (int)eStatus.Active)
                e.Row.Attributes["class"] = "GridDesableRow ";

            DataRowView dataItem = (DataRowView)e.Row.DataItem;
            var lnkEditVariant = e.Row.FindControl("lnkEditVariant") as LinkButton;
            var ltrVariant = e.Row.FindControl("ltrVariant") as Literal;

            lnkEditVariant.Visible = IsAddEdit.Value;
            ltrVariant.Visible = !IsAddEdit.Value;

            lnkEditVariant.Text = ltrVariant.Text = dataItem[CS.VariantName].ToString();
            lnkEditVariant.CommandArgument = dataItem[CS.VariantId].ToString();
        }
    }

    protected void grdVariant_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        lblVariantId.Text = grdVariant.Rows[grdVariant.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdVariant, CS.VariantId)].Text;
        SetControl(eControl.VariantValue);
    }


    private void LoadVariantDetail()
    {
        txtVariantName.Focus();

        if (IsEditMode())
        {
            lblPopupTitle.Text = "Edit Variant";
            var objVariant = new Variant() { VariantId = lblVariantId.zToInt(), }.SelectList<Variant>()[0];
            txtVariantName.Text = objVariant.VariantName;
            rbtnVarient.Checked = objVariant.ShowInOrder == (int)eYesNo.Yes;
        }
        else
        {
            lblPopupTitle.Text = "New Variant";
            txtVariantName.Text = string.Empty;
            rbtnVarient.Checked = true;
        }
    }

    private bool IsEditMode()
    {
        return !lblVariantId.zIsNullOrEmpty();
    }

    private bool IsValidate()
    {
        if (txtVariantName.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Variant Name.");
            txtVariantName.Focus();
            return false;
        }

        var dtVariant = new Query()
        {
            eStatusNot = (int)eStatus.Delete,
            OrganizationId = lblOrganizationId.zToInt(),
            VariantName = txtVariantName.Text.Trim(),
        }.Select(eSP.qry_Variant);

        if (dtVariant.Rows.Count > 0 && dtVariant.Rows[0][CS.VariantId].ToString() != lblVariantId.Text)
        {
            string Status = dtVariant.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
            CU.ZMessage(eMsgType.Error, string.Empty, "This Variant is already exist" + Status + ".");
            txtVariantName.Focus();
            return false;
        }

        return true;
    }

    private bool SaveData()
    {
        if (!IsValidate())
        {
            popupVariant.Show();
            return false;
        }

        string Message = string.Empty;

        var objVariant = new Variant()
        {
            OrganizationId = lblOrganizationId.zToInt(),
            VariantName = txtVariantName.Text.Trim().zFirstCharToUpper(),
            ShowInOrder = rbtnVarient.Checked ? (int)eYesNo.Yes : (int)eYesNo.No,
        };

        if (IsEditMode())
        {
            objVariant.VariantId = lblVariantId.zToInt();
            objVariant.Update();

            Message = "Variant Detail Change Sucessfully.";
        }
        else
        {
            objVariant.eStatus = (int)eStatus.Active;
            objVariant.Insert();

            Message = "New Variant Added Sucessfully.";
        }

        CU.ZMessage(eMsgType.Success, string.Empty, Message);

        return true;
    }

    protected void btnSave_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadVariantGrid(ePageIndex.Custom);
        }
    }

    protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadVariantGrid(ePageIndex.Custom);
            lnkAdd_OnClick(null, null);
        }
    }


    #region Pagging


    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        LoadVariantGrid(ePageIndex.Prev);
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        LoadVariantGrid(ePageIndex.Next);
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        LoadVariantGrid(ePageIndex.First);
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        LoadVariantGrid(ePageIndex.Last);
    }

    protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
    {
        if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
        {
            txtGotoPageNo.Text = "1";
            txtGotoPageNo.Focus();
        }
        LoadVariantGrid(ePageIndex.Custom);
    }

    protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
    {
        txtGotoPageNo.Text = "1";
        LoadVariantGrid(ePageIndex.Custom);
        Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
    }


    #endregion

    #endregion

    #region Variant Value

    private void LoadVariantValueGrid()
    {
        DataTable dtVariantValue = new VariantValue()
        {
            Value = txtSearch.zIsNullOrEmpty() ? null : txtSearch.Text,
            VariantId = lblVariantId.zToInt(),
        }.Select();

        lblCountValue.Text = dtVariantValue.Rows.Count.ToString();

        grdVariantValue.DataSource = dtVariantValue;
        grdVariantValue.DataBind();

        try { grdVariantValue.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void CheckVisibleButtonValue()
    {
        var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageVariant);

        lnkAddValue.Visible = lnkEditValue.Visible = objAuthority.IsAddEdit;
        lnkDeleteValue.Visible = objAuthority.IsDelete;
    }


    protected void lnkAddValue_OnClick(object sender, EventArgs e)
    {
        lblVariantValueId.Text = string.Empty;
        LoadVariantValueDetail();
        popupVariantValue.Show();
    }

    protected void lnkEditValue_OnClick(object sender, EventArgs e)
    {
        if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageVariant).IsAddEdit && (sender == null || grdVariantValue.zIsValidSelection(lblVariantValueId, "chkSelect", CS.VariantValueId)))
        {
            LoadVariantValueDetail();
            popupVariantValue.Show();
        }
    }

    protected void lnkEditVariantValue_OnClick(object sender, EventArgs e)
    {
        lblVariantValueId.Text = ((LinkButton)sender).CommandArgument.ToString();
        lnkEditValue_OnClick(null, null);
    }

    protected void lnkRefreshValue_OnClick(object sender, EventArgs e)
    {
        LoadVariantValueGrid();
    }

    protected void lnkDeleteValue_OnClick(object sender, EventArgs e)
    {
        if (grdVariantValue.zIsValidSelection(lblVariantValueId, "chkSelect", CS.VariantValueId))
        {
            string Message = string.Empty;
            if (CU.IsVariantValueUsed(lblVariantValueId.zToInt().Value, ref Message))
            {
                CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Delete.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Value", "Are You Sure To Delete Value?");
            popupConfirmation.Show();
        }
    }


    protected void btnDeleteValue_OnClick(object sender, EventArgs e)
    {
        new VariantValue()
        {
            VariantValueId = lblVariantValueId.zToInt(),
        }.Delete();

        CU.ZMessage(eMsgType.Success, string.Empty, "Variant Value Delete Successfully.");
        LoadVariantValueGrid();
    }


    protected void grdVariantValue_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!IsAddEdit.HasValue)
                IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageVariant).IsAddEdit;

            if (IsAddEdit.Value)
                e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdVariantValue, "Select$" + e.Row.RowIndex);

            DataRowView dataItem = (DataRowView)e.Row.DataItem;
            var lnkEditVariantValue = e.Row.FindControl("lnkEditVariantValue") as LinkButton;
            var ltrVariantValue = e.Row.FindControl("ltrVariantValue") as Literal;

            lnkEditVariantValue.Visible = IsAddEdit.Value;
            ltrVariantValue.Visible = !IsAddEdit.Value;

            lnkEditVariantValue.Text = ltrVariantValue.Text = dataItem[CS.Value].ToString();
            lnkEditVariantValue.CommandArgument = dataItem[CS.VariantValueId].ToString();
        }
    }

    protected void grdVariantValue_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        lblVariantValueId.Text = grdVariantValue.Rows[grdVariantValue.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdVariantValue, CS.VariantValueId)].Text;
        lnkEditValue_OnClick(null, null);
    }


    private void LoadVariantValueDetail()
    {
        txtVariantValue.Focus();

        if (IsEditModeValue())
        {
            var objVariantValue = new VariantValue() { VariantValueId = lblVariantValueId.zToInt(), }.SelectList<VariantValue>()[0];
            txtVariantValue.Text = objVariantValue.Value;
            lblVariantValueNote.Visible = false;
        }
        else
        {
            txtVariantValue.Text = string.Empty;
            lblVariantValueNote.Visible = true;
        }
    }

    private bool IsEditModeValue()
    {
        return !lblVariantValueId.zIsNullOrEmpty();
    }

    private bool IsValidateValue()
    {
        if (txtVariantValue.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Variant Value.");
            txtVariantValue.Focus();
            return false;
        }

        if (IsEditModeValue())
        {
            var dtVariantValue = new VariantValue()
            {
                VariantId = lblVariantId.zToInt(),
                Value = txtVariantValue.Text.Trim(),
            }.Select();

            if (dtVariantValue.Rows.Count > 0 && dtVariantValue.Rows[0][CS.VariantValueId].ToString() != lblVariantValueId.Text)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Variant Value is already exist.");
                txtVariantValue.Focus();
                return false;
            }
        }

        return true;
    }

    private bool SaveDataValue()
    {
        if (!IsValidateValue())
        {
            popupVariantValue.Show();
            return false;
        }

        string Message = string.Empty;

        if (IsEditModeValue())
        {
            var objVariantValue = new VariantValue()
            {
                VariantValueId = lblVariantValueId.zToInt(),
                VariantId = lblVariantId.zToInt(),
                Value = txtVariantValue.Text.Trim().zFirstCharToUpper(),
            }.Update();

            Message = "Variant Value Change Sucessfully.";
        }
        else
        {
            var strValue = txtVariantValue.Text.Split(',');
            foreach (string Value in strValue)
            {
                if (!Value.Trim().zIsNullOrEmpty())
                {
                    if (new VariantValue() { VariantId = lblVariantId.zToInt(), Value = Value.Trim().zFirstCharToUpper() }.SelectCount() == 0)
                    {
                        var objVariantValue = new VariantValue()
                        {
                            VariantId = lblVariantId.zToInt(),
                            Value = Value.Trim().zFirstCharToUpper(),
                        }.Insert();
                    }
                }
            }

            Message = "New Variant Value Added Sucessfully.";
        }

        CU.ZMessage(eMsgType.Success, string.Empty, Message);

        return true;
    }

    protected void btnSaveValue_OnClick(object sender, EventArgs e)
    {
        if (SaveDataValue())
        {
            LoadVariantValueGrid();
        }
    }

    protected void btnSaveAndNewValue_OnClick(object sender, EventArgs e)
    {
        if (SaveDataValue())
        {
            LoadVariantValueGrid();
            lnkAddValue_OnClick(null, null);
        }
    }

    #endregion

    protected void lnkVariant_OnClick(object sender, EventArgs e)
    {
        SetControl(eControl.Variant);
    }

    private void SetControl(eControl Control)
    {
        pnlVariant.Visible = (Control == eControl.Variant);
        pnlVariantValue.Visible = (Control == eControl.VariantValue);

        if (Control == eControl.Variant)
        {
            LoadVariantGrid(ePageIndex.Custom);
            CheckVisibleButton();
        }
        else if (Control == eControl.VariantValue)
        {
            LoadVariantValueGrid();
            CheckVisibleButtonValue();
        }
    }

    public enum eControl
    {
        Variant = 1,
        VariantValue = 2,
    }
}
