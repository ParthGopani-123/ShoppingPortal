using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;

public partial class MyWallet : CompressorPage
{
    bool? IsAddEdit;
    int LoginUsersId;

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
        LoginUsersId = CU.GetUsersId();

        if (!IsPostBack)
        {
            lblOrganizationId.Text = CU.GetOrganizationId().ToString();
            lblFirmId.Text = CU.GetFirmId().ToString();

            CU.LoadDisplayPerPage(ref ddlRecordPerPage);

            txtFromDate.Text = IndianDateTime.Today.AddDays(-50).ToString(CS.ddMMyyyy);
            txtToDate.Text = IndianDateTime.Today.ToString(CS.ddMMyyyy);

            LoadUser();

            LoadWalletGrid(ePageIndex.Custom);
            CheckVisibleButton();
        }

        Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
        Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
        Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

        try { grdWallet.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }


    private DataTable GetWalletDt(ePageIndex ePageIndex)
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
            UsersId = ddlSearchUser.zIsSelect() ? ddlSearchUser.zToInt() : (int?)null,
            FromDate = txtFromDate.zIsDate() ? txtFromDate.zToDate() : (DateTime?)null,
            ToDate = txtToDate.zIsDate() ? txtToDate.zToDate().Value.AddDays(1).AddSeconds(-1) : (DateTime?)null,
            eStatus = Status,
            eStatusNot = (int)eStatus.Delete,
        };

        #region Page Index

        int RecordPerPage = ddlRecordPerPage.zToInt().Value;
        int PageIndexTemp = PageIndex;

        CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref objQuery, ref txtGotoPageNo, ref lblCount);
        PageIndex = PageIndexTemp;

        #endregion Page Index

        return objQuery.Select(eSP.qry_Wallet);
    }

    private void LoadWalletGrid(ePageIndex ePageIndex)
    {
        DataTable dtWallet = GetWalletDt(ePageIndex);

        if (dtWallet.Rows.Count > 0)
            lblCount.Text = dtWallet.Rows[0][CS.TotalRecord].ToString();
        else
            lblCount.Text = "0";

        divPaging.Visible = (dtWallet.Rows.Count > 0);

        txtGotoPageNo.Text = PageIndex.ToString();

        ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

        grdWallet.DataSource = dtWallet;
        grdWallet.DataBind();

        try { grdWallet.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void CheckVisibleButton()
    {
        var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageWallet);

        lnkAdd.Visible = lnkEdit.Visible = objAuthority.IsAddEdit;
        lnkDelete.Visible = objAuthority.IsDelete;

        lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
        lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));

        divSearchUser.Visible = divSearchActive.Visible = divSearchDeactive.Visible = objAuthority.IsView;

        if (!objAuthority.IsAddEdit)
            grdWallet.Attributes.Add("class", grdWallet.Attributes["class"].ToString().Replace("rowloader", ""));
    }


    private void LoadUser()
    {
        var dtUser = new Query()
        {
            FirmId = lblFirmId.zToInt(),
            eStatus = (int)eStatus.Active
        }.Select(eSP.qry_User);

        var dtSearchUser = dtUser.Copy();

        CU.FillDropdown(ref ddlUser, dtUser, "-- Select User --", CS.UsersId, CS.Name);
        CU.FillDropdown(ref ddlSearchUser, dtSearchUser, "-- All User --", CS.UsersId, CS.Name);

        try { ddlSearchUser.SelectedValue = CU.GetUsersId().ToString(); }
        catch { }
    }

    private void LoadWalletDetail()
    {
        ddlUser.Focus();

        if (IsEditMode())
        {
            lblPopupTitle.Text = "Edit Transaction";
            var objWallet = new Wallet() { WalletId = lblWalletId.zToInt(), }.SelectList<Wallet>()[0];

            ddlUser.SelectedValue = objWallet.UsersId.ToString();
            txtTransactionTime.Text = objWallet.TransactionTime.Value.ToString(CS.ddMMyyyyHHmm);
            rdoCredit.Checked = objWallet.eDirection == (int)eTransactionDirection.Credit;
            rdoDebit.Checked = objWallet.eDirection == (int)eTransactionDirection.Debit;
            txtAmount.Text = objWallet.Amount.ToString();
            txtNarration.Text = objWallet.Narration;
        }
        else
        {
            lblPopupTitle.Text = "New Transaction";

            rdoCredit.Checked = true;
            rdoDebit.Checked = false;
            txtTransactionTime.Text = IndianDateTime.Now.ToString(CS.ddMMyyyyHHmm);
            txtAmount.Text = txtNarration.Text = string.Empty;
        }
    }

    private bool IsEditMode()
    {
        return !lblWalletId.zIsNullOrEmpty();
    }

    private bool IsValidate()
    {
        if (!ddlUser.zIsSelect())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select User.");
            ddlUser.Focus();
            return false;
        }

        if (!txtTransactionTime.zIsDateTime24() || txtTransactionTime.zToDateTime24() > IndianDateTime.Now)
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Transaction Date.");
            txtTransactionTime.Focus();
            return false;
        }

        if (!txtAmount.zIsFloat(false))
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Amount.");
            txtAmount.Focus();
            return false;
        }

        return true;
    }

    private bool SaveData()
    {
        if (!IsValidate())
        {
            popupWallet.Show();
            return false;
        }

        CU.SetTransaction(lblWalletId.zToInt(), ddlUser.zToInt().Value, txtAmount.zToDecimal().Value, rdoCredit.Checked ? eTransactionDirection.Credit : eTransactionDirection.Debit, eTransactionType.Manually, txtNarration.Text, txtTransactionTime.zToDateTime24(), null, null);

        CU.ZMessage(eMsgType.Success, string.Empty, IsEditMode() ? "Transaction Detail Change Sucessfully." : "New Transaction Added Sucessfully.");

        return true;
    }


    protected void btnSave_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadWalletGrid(ePageIndex.Custom);
            popupWallet.Hide();
        }
    }

    protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadWalletGrid(ePageIndex.Custom);
            lnkAdd_OnClick(null, null);
        }
    }


    protected void lnkAdd_OnClick(object sender, EventArgs e)
    {
        lblWalletId.Text = string.Empty;
        LoadWalletDetail();
        popupWallet.Show();
    }

    protected void lnkEdit_OnClick(object sender, EventArgs e)
    {
        if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageWallet).IsAddEdit && (sender == null || grdWallet.zIsValidSelection(lblWalletId, "chkSelect", CS.WalletId)))
        {
            LoadWalletDetail();
            popupWallet.Show();
        }
    }

    protected void lnkRefresh_OnClick(object sender, EventArgs e)
    {
        LoadWalletGrid(ePageIndex.Custom);
    }

    protected void lnkActive_OnClick(object sender, EventArgs e)
    {
        if (grdWallet.zIsValidSelection(lblWalletId, "chkSelect", CS.WalletId))
        {
            if (new Wallet()
            {
                WalletId = lblWalletId.zToInt(),
                eStatus = (int)eStatus.Active
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Transaction is already Active.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Active, "Active Transaction", "Are You Sure To Active Transaction?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDeactive_OnClick(object sender, EventArgs e)
    {
        if (grdWallet.zIsValidSelection(lblWalletId, "chkSelect", CS.WalletId))
        {
            if (new Wallet()
            {
                WalletId = lblWalletId.zToInt(),
                eStatus = (int)eStatus.Deactive
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Transaction is already Deactive.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive Transaction", "Are You Sure To Deactive Transaction?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDelete_OnClick(object sender, EventArgs e)
    {
        if (grdWallet.zIsValidSelection(lblWalletId, "chkSelect", CS.WalletId))
        {
            string Message = string.Empty;
            Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Transaction", "Are You Sure To Delete Transaction?");
            popupConfirmation.Show();
        }
    }


    private void ManageWalletStatus(eStatus Status)
    {
        var objOldWallent = new Wallet() { WalletId = lblWalletId.zToInt() }.SelectList<Wallet>()[0];

        new Wallet()
        {
            WalletId = lblWalletId.zToInt(),
            eStatus = (int)Status
        }.Update();

        CU.CountCurrentBalancee(objOldWallent);
    }

    protected void btnActive_OnClick(object sender, EventArgs e)
    {
        ManageWalletStatus(eStatus.Active);
        CU.ZMessage(eMsgType.Success, string.Empty, "Transaction Activated Successfully.");
        LoadWalletGrid(ePageIndex.Custom);
    }

    protected void btnDeactive_OnClick(object sender, EventArgs e)
    {
        ManageWalletStatus(eStatus.Deactive);
        CU.ZMessage(eMsgType.Success, string.Empty, "Transaction Deactive Successfully.");
        LoadWalletGrid(ePageIndex.Custom);
    }

    protected void btnDelete_OnClick(object sender, EventArgs e)
    {
        ManageWalletStatus(eStatus.Delete);
        CU.ZMessage(eMsgType.Success, string.Empty, "Transaction Delete Successfully.");
        LoadWalletGrid(ePageIndex.Custom);
    }


    protected void Control_CheckedChanged(object sender, EventArgs e)
    {
        LoadWalletGrid(ePageIndex.Custom);
        CheckVisibleButton();
    }

    protected void grdWallet_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!IsAddEdit.HasValue)
                IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageWallet).IsAddEdit;

            if (IsAddEdit.Value)
                e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdWallet, "Select$" + e.Row.RowIndex);

            if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdWallet, CS.eStatus)].Text) != (int)eStatus.Active)
                e.Row.Attributes["class"] = "GridDesableRow ";

            DataRowView dataItem = (DataRowView)e.Row.DataItem;

            var ltrTransactionId = e.Row.FindControl("ltrTransactionId") as Literal;
            var lblTransactionAmountDebit = e.Row.FindControl("lblTransactionAmountDebit") as Label;
            var lblTransactionAmountCredit = e.Row.FindControl("lblTransactionAmountCredit") as Label;
            var lblBalance = e.Row.FindControl("lblBalance") as Label;

            string Amount = String.Format("{0:n}", dataItem[CS.Amount].zToDecimal().Value).Replace(".00", "") + " Rs.";

            if ((eTransactionDirection)dataItem[CS.eDirection].zToInt() == eTransactionDirection.Debit)
                lblTransactionAmountDebit.Text = Amount;
            else
                lblTransactionAmountCredit.Text = Amount;

            lblBalance.Text = String.Format("{0:n}", dataItem[CS.CurrentBalance].zToDecimal().Value).Replace(".00", "") + " Rs.";
            ltrTransactionId.Text = dataItem[CS.CashTransactionId].ToString();
        }
    }

    protected void grdWallet_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        lblWalletId.Text = grdWallet.Rows[grdWallet.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdWallet, CS.WalletId)].Text;
        lnkEdit_OnClick(null, null);
    }


    #region Excel Export

    protected void lnkExcelExport_OnClick(object sender, EventArgs e)
    {
        var dtWallet = GetWalletDt(ePageIndex.AllPage);
        dtWallet.Columns[CS.CashTransactionId].ColumnName = CS.Id;
        dtWallet.Columns[CS.EntryFullName].ColumnName = CS.Name;
        dtWallet.Columns[CS.TransactionTime].ColumnName = CS.Date;

        var lstColumns = new System.Collections.Generic.List<string>();
        lstColumns.Add(CS.Id);
        lstColumns.Add(CS.TransactionType);
        lstColumns.Add(CS.Name);
        lstColumns.Add(CS.Date);
        lstColumns.Add(CS.DebitAmount);
        lstColumns.Add(CS.CreditAmount);
        lstColumns.Add(CS.Narration);

        string FileName = "Transaction";
        if (txtFromDate.zIsDate() && txtToDate.zIsDate())
            FileName += " For(" + txtFromDate.zToDate().Value.ToString(CS.ddMMMyy) + "-" + txtToDate.zToDate().Value.ToString(CS.ddMMMyy) + ")";

        ExcelExport.SetExportData(dtWallet, lstColumns, lstColumns, FileName);
        popupExcelExport.Show();
    }

    #endregion


    #region Pagging

    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        LoadWalletGrid(ePageIndex.Prev);
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        LoadWalletGrid(ePageIndex.Next);
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        LoadWalletGrid(ePageIndex.First);
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        LoadWalletGrid(ePageIndex.Last);
    }

    protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
    {
        if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
        {
            txtGotoPageNo.Text = "1";
            txtGotoPageNo.Focus();
        }
        LoadWalletGrid(ePageIndex.Custom);
    }

    protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
    {
        txtGotoPageNo.Text = "1";
        LoadWalletGrid(ePageIndex.Custom);
        Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
    }

    #endregion
}
