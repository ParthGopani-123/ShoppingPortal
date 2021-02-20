using System;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.Services;
using System.Linq;
using System.Text.RegularExpressions;

public partial class ManageOrderPayment : CompressorPage
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
            int FirmId = 0, OrganizationId = 0;
            CU.GetFirmOrganizationId(ref FirmId, ref OrganizationId);
            lblFirmId.Text = FirmId.ToString();
            lblOrganizationId.Text = OrganizationId.ToString();
            lblUsersId.Text = CU.GetUsersId().ToString();

            CU.LoadDisplayPerPage(ref ddlRecordPerPage);

            DateTime DateToday = IndianDateTime.Today;

            try { txtFromDate.Text = CU.GetSearchCookie("OrderFromDate"); }
            catch { }

            if (txtFromDate.zIsNullOrEmpty())
                txtFromDate.Text = DateToday.AddDays(-5).ToString(CS.ddMMyyyy);
            txtToDate.Text = DateToday.ToString(CS.ddMMyyyy);

            LoadFirm();
            try { ddlFirm.SelectedValue = CU.GetSearchCookie("OrderFirm"); }
            catch { }

            LoadUser();
            try { ddlUser.SelectedValue = CU.GetSearchCookie("OrderUser"); }
            catch { }


            LoadOrderPaymentGrid(ePageIndex.Custom);
            CheckVisibleButton();
        }

        popupManageOrderPayment.btnSave_OnClick += new EventHandler(btnSaveOrderPayment_OnClick);

        Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

        try { grdOrderPayment.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }


    private void LoadFirm()
    {
        var dtFirm = new Query() { OrganizationId = lblOrganizationId.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_Firm);
        CU.FillDropdown(ref ddlFirm, dtFirm, "-- All Firm --", CS.FirmId, CS.FirmName);

        try { ddlFirm.SelectedValue = lblFirmId.Text; }
        catch { }
    }

    private void LoadUser()
    {
        var dtUser = new Query() { FirmId = ddlFirm.zIsSelect() ? ddlFirm.zToInt() : (int?)null, eStatus = (int)eStatus.Active }.Select(eSP.qry_User);
        CU.FillDropdown(ref ddlUser, dtUser, "-- All User --", CS.UsersId, CS.Name);

        try { ddlUser.SelectedValue = lblUsersId.Text; }
        catch { }
    }

    private DataTable GetOrderPaymentDt(ePageIndex ePageIndex)
    {
        var objQuery = new Query()
        {
            OrganizationId = lblOrganizationId.zToInt(),

            FirmId = ddlFirm.zToInt() > 0 ? ddlFirm.zToInt() : (int?)null,
            UsersId = ddlUser.zToInt() > 0 ? ddlUser.zToInt() : (int?)null,
            OrdersId = txtSearchOrderId.zToInt(),
            FromDate = txtFromDate.zToDate().HasValue ? txtFromDate.zToDate() : (DateTime?)null,
            ToDate = txtToDate.zToDate().HasValue ? txtToDate.zToDate().Value.AddDays(1).AddSeconds(-1) : (DateTime?)null,

            MasterSearch = txtSearch.Text,
        };

        #region Page Index

        int RecordPerPage = ddlRecordPerPage.zToInt().Value;
        int PageIndexTemp = PageIndex;

        CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref objQuery, ref txtGotoPageNo, ref lblCount);
        PageIndex = PageIndexTemp;

        #endregion Page Index

        return objQuery.Select(eSP.qry_OrderPayment);
    }

    private void LoadOrderPaymentGrid(ePageIndex ePageIndex)
    {
        DataTable dtOrderPayment = GetOrderPaymentDt(ePageIndex);

        if (dtOrderPayment.Rows.Count > 0)
            lblCount.Text = dtOrderPayment.Rows[0][CS.TotalRecord].ToString();
        else
            lblCount.Text = "0";

        divPaging.Visible = (dtOrderPayment.Rows.Count > 0);

        txtGotoPageNo.Text = PageIndex.ToString();

        ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

        grdOrderPayment.DataSource = dtOrderPayment;
        grdOrderPayment.DataBind();

        try { grdOrderPayment.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void CheckVisibleButton()
    {
        var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageOrderPayment);
        lnkEdit.Visible = objAuthority.IsAddEdit;
    }


    protected void lnkEdit_OnClick(object sender, EventArgs e)
    {
        if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageOrderPayment).IsAddEdit && (grdOrderPayment.zIsValidSelection(lblOrderPaymentId, "chkSelect", CS.OrderPaymentId)))
        {
            popupManageOrderPayment.SetOrderPaymentId = lblOrderPaymentId.Text;
            popupManageOrderPayment.LoadOrderPaymentDetail(0);
            popupOrderPayment.Show();
        }
    }

    protected void btnSaveOrderPayment_OnClick(object sender, EventArgs e)
    {
        LoadOrderPaymentGrid(ePageIndex.Custom);
        popupOrderPayment.Hide();
    }

    protected void lnkDelete_OnClick(object sender, EventArgs e)
    {
        if (grdOrderPayment.zIsValidSelection(lblOrderPaymentId, "chkSelect", CS.OrderPaymentId))
        {
            Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Payment", "Are You Sure To Delete Payment?");
            popupConfirmation.Show();
        }
    }

    protected void btnDelete_OnClick(object sender, EventArgs e)
    {
        new OrderPayment() { OrderPaymentId = lblOrderPaymentId.zToInt() }.Delete();
        CU.ZMessage(eMsgType.Success, string.Empty, "Payment Delete Successfully.");
        LoadOrderPaymentGrid(ePageIndex.Custom);
    }


    protected void lnkRefresh_OnClick(object sender, EventArgs e)
    {
        CU.SetSearchCookie("OrderFirm", ddlFirm.SelectedValue);
        CU.SetSearchCookie("OrderUser", ddlUser.SelectedValue);

        CU.SetSearchCookie("OrderFromDate", txtFromDate.Text);

        LoadOrderPaymentGrid(ePageIndex.Custom);
    }

    protected void ddlFirm_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        LoadUser();
    }

    protected void grdOrderPayment_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

        }
    }

    #region Pagging


    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        LoadOrderPaymentGrid(ePageIndex.Prev);
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        LoadOrderPaymentGrid(ePageIndex.Next);
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        LoadOrderPaymentGrid(ePageIndex.First);
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        LoadOrderPaymentGrid(ePageIndex.Last);
    }

    protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
    {
        if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
        {
            txtGotoPageNo.Text = "1";
            txtGotoPageNo.Focus();
        }
        LoadOrderPaymentGrid(ePageIndex.Custom);
    }

    protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
    {
        txtGotoPageNo.Text = "1";
        LoadOrderPaymentGrid(ePageIndex.Custom);
        Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
    }


    #endregion
}
