using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;

public partial class ManageOrderSource : CompressorPage
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

            LoadOrderSource(ePageIndex.Custom);
            CheckVisibleButton();
        }

        Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
        Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
        Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

        try { grdOrderSource.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }


    private DataTable GetOrderSourceDt(ePageIndex ePageIndex)
    {
        int? Status = null;
        if (chkActive.Checked && !chkDeactive.Checked)
            Status = (int)eStatus.Active;
        else if (!chkActive.Checked && chkDeactive.Checked)
            Status = (int)eStatus.Deactive;

        var objQuery = new Query()
        {
            FirmId = CU.GetFirmId(),
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

        return objQuery.Select(eSP.qry_OrderSource);
    }

    private void LoadOrderSource(ePageIndex ePageIndex)
    {
        DataTable dtOrderSource = GetOrderSourceDt(ePageIndex);

        if (dtOrderSource.Rows.Count > 0)
            lblCount.Text = dtOrderSource.Rows[0][CS.TotalRecord].ToString();
        else
            lblCount.Text = "0";

        divPaging.Visible = (dtOrderSource.Rows.Count > 0);

        txtGotoPageNo.Text = PageIndex.ToString();

        ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

        grdOrderSource.DataSource = dtOrderSource;
        grdOrderSource.DataBind();

        try { grdOrderSource.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void CheckVisibleButton()
    {
        var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageOrderSource);

        lnkAdd.Visible = lnkEdit.Visible = objAuthority.IsAddEdit;
        lnkDelete.Visible = objAuthority.IsDelete;

        lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
        lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
    }


    protected void lnkAdd_OnClick(object sender, EventArgs e)
    {
        lblOrderSourceId.Text = string.Empty;
        LoadOrderSourceDetail();
        popupOrderSource.Show();
    }

    protected void lnkEdit_OnClick(object sender, EventArgs e)
    {
        if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageOrderSource).IsAddEdit && (sender == null || grdOrderSource.zIsValidSelection(lblOrderSourceId, "chkSelect", CS.OrderSourceId)))
        {
            LoadOrderSourceDetail();
            popupOrderSource.Show();
        }
    }

    protected void lnkEditOrderSource_Click(object sender, EventArgs e)
    {
        lblOrderSourceId.Text = ((LinkButton)sender).CommandArgument;
        lnkEdit_OnClick(null, null);
    }

    protected void lnkRefresh_OnClick(object sender, EventArgs e)
    {
        LoadOrderSource(ePageIndex.Custom);
    }

    protected void lnkActive_OnClick(object sender, EventArgs e)
    {
        if (grdOrderSource.zIsValidSelection(lblOrderSourceId, "chkSelect", CS.OrderSourceId))
        {
            if (new OrderSource()
            {
                OrderSourceId = lblOrderSourceId.zToInt(),
                eStatus = (int)eStatus.Active
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Order Source is already Active.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Active, "Active Order Source", "Are You Sure To Active Order Source?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDeactive_OnClick(object sender, EventArgs e)
    {
        if (grdOrderSource.zIsValidSelection(lblOrderSourceId, "chkSelect", CS.OrderSourceId))
        {
            if (new OrderSource()
            {
                OrderSourceId = lblOrderSourceId.zToInt(),
                eStatus = (int)eStatus.Deactive
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Order Source is already Deactive.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive Order Source", "Are You Sure To Deactive Order Source?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDelete_OnClick(object sender, EventArgs e)
    {
        if (grdOrderSource.zIsValidSelection(lblOrderSourceId, "chkSelect", CS.OrderSourceId))
        {

            Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Order Source", "Are You Sure To Delete Order Source?");
            popupConfirmation.Show();
        }
    }

    protected void Control_CheckedChanged(object sender, EventArgs e)
    {
        LoadOrderSource(ePageIndex.Custom);
        CheckVisibleButton();
    }


    private void ManageOrderSourceStatus(eStatus Status)
    {
        new OrderSource()
        {
            OrderSourceId = lblOrderSourceId.zToInt(),
            eStatus = (int)Status
        }.Update();
    }

    protected void btnActive_OnClick(object sender, EventArgs e)
    {
        ManageOrderSourceStatus(eStatus.Active);
        CU.ZMessage(eMsgType.Success, string.Empty, "Order Source Activated Successfully.");
        LoadOrderSource(ePageIndex.Custom);
    }

    protected void btnDeactive_OnClick(object sender, EventArgs e)
    {
        ManageOrderSourceStatus(eStatus.Deactive);
        CU.ZMessage(eMsgType.Success, string.Empty, "Order Source Deactive Successfully.");
        LoadOrderSource(ePageIndex.Custom);
    }

    protected void btnDelete_OnClick(object sender, EventArgs e)
    {
        ManageOrderSourceStatus(eStatus.Delete);
        CU.ZMessage(eMsgType.Success, string.Empty, "Order Source Delete Successfully.");
        LoadOrderSource(ePageIndex.Custom);
    }

    protected void grdOrderSource_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!IsAddEdit.HasValue)
                IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageOrderSource).IsAddEdit;

            if (IsAddEdit.Value)
                e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdOrderSource, "Select$" + e.Row.RowIndex);

            if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdOrderSource, CS.eStatus)].Text) != (int)eStatus.Active)
                e.Row.Attributes["class"] = "GridDesableRow ";

            DataRowView dataItem = (DataRowView)e.Row.DataItem;

            var lnkEditOrderSource = e.Row.FindControl("lnkEditOrderSource") as LinkButton;
            var ltrOrderSource = e.Row.FindControl("ltrOrderSource") as Literal;

            lnkEditOrderSource.Visible = IsAddEdit.Value;
            ltrOrderSource.Visible = !IsAddEdit.Value;

            lnkEditOrderSource.Text = ltrOrderSource.Text = dataItem[CS.OrderSourceName].ToString();
            lnkEditOrderSource.CommandArgument = dataItem[CS.OrderSourceId].ToString();
        }
    }

    protected void grdOrderSource_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblOrderSourceId.Text = grdOrderSource.Rows[grdOrderSource.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdOrderSource, CS.OrderSourceId)].Text;
        lnkEdit_OnClick(null, null);
    }


    private void LoadOrderSourceDetail()
    {
        txtOrderSourceName.Focus();
        if (IsEditMode())
        {
            lblPopupTitle.Text = "Edit Order Source";
            var objOrderSource = new OrderSource() { OrderSourceId = lblOrderSourceId.zToInt() }.SelectList<OrderSource>()[0];

            txtOrderSourceName.Text = objOrderSource.OrderSourceName;
            txtDescription.Text = objOrderSource.Description;
        }
        else
        {
            lblPopupTitle.Text = "New Order Source";
            txtOrderSourceName.Text =  txtDescription.Text = string.Empty;
        }

    }

    private bool IsEditMode()
    {
        return !lblOrderSourceId.zIsNullOrEmpty();
    }

    private bool IsValidate()
    {
        if (txtOrderSourceName.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Order Source Name.");
            txtOrderSourceName.Focus();
            return false;
        }

        var dtOrderSource = new Query()
        {
            FirmId = CU.GetFirmId(),
            OrderSourceName = txtOrderSourceName.Text.Trim(),
            eStatusNot = (int)eStatus.Delete,
        }.Select(eSP.qry_OrderSource);

        if (dtOrderSource.Rows.Count > 0 && dtOrderSource.Rows[0][CS.OrderSourceId].ToString() != lblOrderSourceId.Text)
        {
            string Status = dtOrderSource.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
            CU.ZMessage(eMsgType.Error, string.Empty, "This Order Source is already exist" + Status + ".");
            txtOrderSourceName.Focus();
            return false;
        }

        return true;
    }

    private bool SaveData()
    {
        if (!IsValidate())
        {
            popupOrderSource.Show();
            return false;
        }

        string Message = string.Empty;

        var objOrderSource = new OrderSource()
        {
            FirmId = CU.GetFirmId(),
            OrderSourceName = txtOrderSourceName.Text.Trim().zFirstCharToUpper(),
            Description = txtDescription.Text.Trim().zFirstCharToUpper()
        };

        if (IsEditMode())
        {
            objOrderSource.OrderSourceId = lblOrderSourceId.zToInt();
            objOrderSource.Update();

            Message = "Order Source Detail Change Sucessfully.";
        }
        else
        {
            objOrderSource.eStatus = (int)eStatus.Active;
            objOrderSource.OrderSourceId = objOrderSource.Insert();

            Message = "New Order Source Added Sucessfully.";
        }

        CU.ZMessage(eMsgType.Success, string.Empty, Message);

        return true;
    }

    protected void btnSave_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadOrderSource(ePageIndex.Custom);
        }
    }

    protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadOrderSource(ePageIndex.Custom);
            lnkAdd_OnClick(null, null);
        }
    }

    #region Pagging


    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        LoadOrderSource(ePageIndex.Prev);
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        LoadOrderSource(ePageIndex.Next);
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        LoadOrderSource(ePageIndex.First);
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        LoadOrderSource(ePageIndex.Last);
    }

    protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
    {
        if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
        {
            txtGotoPageNo.Text = "1";
            txtGotoPageNo.Focus();
        }
        LoadOrderSource(ePageIndex.Custom);
    }

    protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
    {
        txtGotoPageNo.Text = "1";
        LoadOrderSource(ePageIndex.Custom);
        Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
    }


    #endregion
}
