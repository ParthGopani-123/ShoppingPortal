using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;

public partial class ManageVendor : CompressorPage
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

            LoadVendor(ePageIndex.Custom);
            CheckVisibleButton();
        }

        Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
        Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
        Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

        try { grdVendor.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }


    private DataTable GetVendorDt(ePageIndex ePageIndex)
    {
        int? Status = null;
        if (chkActive.Checked && !chkDeactive.Checked)
            Status = (int)eStatus.Active;
        else if (!chkActive.Checked && chkDeactive.Checked)
            Status = (int)eStatus.Deactive;

        int OrganizationId = CU.GetOrganizationId();
        var objQuery = new Query()
        {
            OrganizationId = CU.GetOrganizationId(),
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

        return objQuery.Select(eSP.qry_Vendor);
    }

    private void LoadVendor(ePageIndex ePageIndex)
    {
        DataTable dtVendor = GetVendorDt(ePageIndex);

        if (dtVendor.Rows.Count > 0)
            lblCount.Text = dtVendor.Rows[0][CS.TotalRecord].ToString();
        else
            lblCount.Text = "0";

        divPaging.Visible = (dtVendor.Rows.Count > 0);

        txtGotoPageNo.Text = PageIndex.ToString();

        ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

        grdVendor.DataSource = dtVendor;
        grdVendor.DataBind();

        try { grdVendor.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void CheckVisibleButton()
    {
        var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageVendor);

        lnkAdd.Visible = lnkEdit.Visible = objAuthority.IsAddEdit;
        lnkDelete.Visible = objAuthority.IsDelete;

        lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
        lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
    }


    protected void lnkAdd_OnClick(object sender, EventArgs e)
    {
        lblVendorId.Text = string.Empty;
        LoadVendorDetail();
        popupVendor.Show();
    }

    protected void lnkEdit_OnClick(object sender, EventArgs e)
    {
        if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageVendor).IsAddEdit && (sender == null || grdVendor.zIsValidSelection(lblVendorId, "chkSelect", CS.VendorId)))
        {
            LoadVendorDetail();
            popupVendor.Show();
        }
    }

    protected void lnkEditVendor_Click(object sender, EventArgs e)
    {
        lblVendorId.Text = ((LinkButton)sender).CommandArgument;
        lnkEdit_OnClick(null, null);
    }

    protected void lnkRefresh_OnClick(object sender, EventArgs e)
    {
        LoadVendor(ePageIndex.Custom);
    }

    protected void lnkActive_OnClick(object sender, EventArgs e)
    {
        if (grdVendor.zIsValidSelection(lblVendorId, "chkSelect", CS.VendorId))
        {
            if (new Vendor()
            {
                VendorId = lblVendorId.zToInt(),
                eStatus = (int)eStatus.Active
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Vendor is already Active.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Active, "Active Vendor", "Are You Sure To Active Vendor?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDeactive_OnClick(object sender, EventArgs e)
    {
        if (grdVendor.zIsValidSelection(lblVendorId, "chkSelect", CS.VendorId))
        {
            if (new Vendor()
            {
                VendorId = lblVendorId.zToInt(),
                eStatus = (int)eStatus.Deactive
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Vendor is already Deactive.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive Vendor", "Are You Sure To Deactive Vendor?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDelete_OnClick(object sender, EventArgs e)
    {
        if (grdVendor.zIsValidSelection(lblVendorId, "chkSelect", CS.VendorId))
        {

            Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Vendor", "Are You Sure To Delete Vendor?");
            popupConfirmation.Show();
        }
    }

    protected void Control_CheckedChanged(object sender, EventArgs e)
    {
        LoadVendor(ePageIndex.Custom);
        CheckVisibleButton();
    }


    private void ManageVendorStatus(eStatus Status)
    {
        new Vendor()
        {
            VendorId = lblVendorId.zToInt(),
            eStatus = (int)Status
        }.Update();
    }

    protected void btnActive_OnClick(object sender, EventArgs e)
    {
        ManageVendorStatus(eStatus.Active);
        CU.ZMessage(eMsgType.Success, string.Empty, "Vendor Activated Successfully.");
        LoadVendor(ePageIndex.Custom);
    }

    protected void btnDeactive_OnClick(object sender, EventArgs e)
    {
        ManageVendorStatus(eStatus.Deactive);
        CU.ZMessage(eMsgType.Success, string.Empty, "Vendor Deactive Successfully.");
        LoadVendor(ePageIndex.Custom);
    }

    protected void btnDelete_OnClick(object sender, EventArgs e)
    {
        ManageVendorStatus(eStatus.Delete);
        CU.ZMessage(eMsgType.Success, string.Empty, "Vendor Delete Successfully.");
        LoadVendor(ePageIndex.Custom);
    }

    protected void grdVendor_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!IsAddEdit.HasValue)
                IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageVendor).IsAddEdit;

            if (IsAddEdit.Value)
                e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdVendor, "Select$" + e.Row.RowIndex);

            if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdVendor, CS.eStatus)].Text) != (int)eStatus.Active)
                e.Row.Attributes["class"] = "GridDesableRow ";

            DataRowView dataItem = (DataRowView)e.Row.DataItem;

            var lnkEditVendor = e.Row.FindControl("lnkEditVendor") as LinkButton;
            var ltrVendor = e.Row.FindControl("ltrVendor") as Literal;

            lnkEditVendor.Visible = IsAddEdit.Value;
            ltrVendor.Visible = !IsAddEdit.Value;

            lnkEditVendor.Text = ltrVendor.Text = dataItem[CS.VendorName].ToString();
            lnkEditVendor.CommandArgument = dataItem[CS.VendorId].ToString();
        }
    }

    protected void grdVendor_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblVendorId.Text = grdVendor.Rows[grdVendor.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdVendor, CS.VendorId)].Text;
        lnkEdit_OnClick(null, null);
    }


    private void LoadVendorDetail()
    {
        txtVendorName.Focus();
        if (IsEditMode())
        {
            lblPopupTitle.Text = "Edit Vendor";
            var objVendor = new Vendor() { VendorId = lblVendorId.zToInt() }.SelectList<Vendor>()[0];

            txtVendorName.Text = objVendor.VendorName;
            txtMobileNo.Text = objVendor.MobileNo;
            txtAddress.Text = objVendor.Address;
        }
        else
        {
            lblPopupTitle.Text = "New Vendor";
            txtVendorName.Text = txtMobileNo.Text = txtAddress.Text = string.Empty;
        }

    }

    private bool IsEditMode()
    {
        return !lblVendorId.zIsNullOrEmpty();
    }

    private bool IsValidate()
    {
        if (txtVendorName.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Vendor Name.");
            txtVendorName.Focus();
            return false;
        }

        if (txtMobileNo.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Mobile No.");
            txtMobileNo.Focus();
            return false;
        }

        var dtVendor = new Query()
        {
            OrganizationId = CU.GetOrganizationId(),
            VendorName = txtVendorName.Text,
            MobileNo = txtMobileNo.Text,
            eStatusNot = (int)eStatus.Delete,
        }.Select(eSP.qry_Vendor);

        if (dtVendor.Rows.Count > 0 && dtVendor.Rows[0][CS.VendorId].ToString() != lblVendorId.Text)
        {
            string Status = dtVendor.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
            CU.ZMessage(eMsgType.Error, string.Empty, "This Vendor is already exist" + Status + ".");
            txtVendorName.Focus();
            return false;
        }

        if (!txtMobileNo.zIsMobile())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Mobile No.");
            txtMobileNo.Focus();
            return false;
        }

        return true;
    }

    private bool SaveData()
    {
        if (!IsValidate())
        {
            popupVendor.Show();
            return false;
        }

        string Message = string.Empty;

        var OrganizationId = CU.GetOrganizationId();
        var objVendor = new Vendor()
        {
            OrganizationId = CU.GetOrganizationId(),
            VendorName = txtVendorName.Text.Trim().zFirstCharToUpper(),
            MobileNo = txtMobileNo.Text,
            Address = txtAddress.Text.Trim().zFirstCharToUpper(),
        };

        if (IsEditMode())
        {
            objVendor.VendorId = lblVendorId.zToInt();
            objVendor.Update();

            Message = "Vendor Detail Change Sucessfully.";
        }
        else
        {
            objVendor.eStatus = (int)eStatus.Active;
            objVendor.VendorId = objVendor.Insert();

            Message = "New Vendor Added Sucessfully.";
        }

        CU.ZMessage(eMsgType.Success, string.Empty, Message);

        return true;
    }

    protected void btnSave_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadVendor(ePageIndex.Custom);
        }
    }

    protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadVendor(ePageIndex.Custom);
            lnkAdd_OnClick(null, null);
        }
    }

    #region Pagging


    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        LoadVendor(ePageIndex.Prev);
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        LoadVendor(ePageIndex.Next);
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        LoadVendor(ePageIndex.First);
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        LoadVendor(ePageIndex.Last);
    }

    protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
    {
        if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
        {
            txtGotoPageNo.Text = "1";
            txtGotoPageNo.Focus();
        }
        LoadVendor(ePageIndex.Custom);
    }

    protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
    {
        txtGotoPageNo.Text = "1";
        LoadVendor(ePageIndex.Custom);
        Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
    }


    #endregion
}
