using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;

public partial class ManagePortal : CompressorPage
{
    private bool? IsAddEdit;

    private int PageIndex
    {
        get
        {
            if (ViewState["PageIndex"] != null)
                return Convert.ToInt32(ViewState["PageIndex"]);
            else
                return 0;
        }
        set
        {
            ViewState["PageIndex"] = value;
        }
    }


    protected void Page_Load(Object sender, EventArgs e)
    {
        LoginUtilities.CheckSession();

        if (!IsPostBack)
        {
            lblOrganizationId.Text = CU.GetOrganizationId().ToString();

            CU.LoadDisplayPerPage(ref ddlRecordPerPage);

            LoadPortal();
            LoadPortalGrid(ePageIndex.Custom);

            CheckVisibleButton();
        }

        Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
        Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
        Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

        try { grdPortal.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }


    private void LoadPortal()
    {
        CU.FillEnumddl<ePortal>(ref ddlSearchPortal, "-- All Portal --");
        CU.FillEnumddl<ePortal>(ref ddlPortal, "-- Select Portal --");
    }

    private DataTable GetPortalDt(ePageIndex ePageIndex)
    {
        int? status = null;
        if (chkActive.Checked && !chkDeactive.Checked)
            status = (int)eStatus.Active;
        else if (!chkActive.Checked && chkDeactive.Checked)
            status = (int)eStatus.Deactive;

        var ObjQuery = new Query()
        {
            OrganizationId = lblOrganizationId.zToInt(),
            MasterSearch = txtSearch.Text,
            ePortal = ddlSearchPortal.zIsSelect() ? ddlSearchPortal.zToInt() : (int?)null,
            eStatus = status,
            eStatusNot = (int)eStatus.Delete
        };

        int RecordPerPage = ddlRecordPerPage.zToInt().Value;
        int PageIndexTemp = PageIndex;
        CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref ObjQuery, ref txtGotoPageNo, ref lblCount);
        PageIndex = PageIndexTemp;

        return ObjQuery.Select(eSP.qry_Portal);
    }

    private void LoadPortalGrid(ePageIndex ePageIndex)
    {
        DataTable dtPortal = GetPortalDt(ePageIndex);

        if (dtPortal.Rows.Count > 0)
            lblCount.Text = dtPortal.Rows[0][CS.TotalRecord].ToString();
        else
            lblCount.Text = "0";

        divPaging.Visible = (dtPortal.Rows.Count > 0);
        txtGotoPageNo.Text = PageIndex.ToString();
        ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

        grdPortal.DataSource = dtPortal;
        grdPortal.DataBind();

        try { grdPortal.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void CheckVisibleButton()
    {
        var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManagePortal);

        lnkAdd.Visible = lnkEdit.Visible = objAuthority.IsAddEdit;
        lnkDelete.Visible = objAuthority.IsDelete;
        lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
        lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));

        if (!objAuthority.IsAddEdit)
            grdPortal.Attributes.Add("class", grdPortal.Attributes["class"].ToString().Replace("rowloader", ""));
    }


    protected void lnkAdd_OnClick(object sender, EventArgs e)
    {
        lblPortalId.Text = string.Empty;
        LoadPortalDetail();
        popupPortal.Show();
    }

    protected void lnkEdit_OnClick(object sender, EventArgs e)
    {
        if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManagePortal).IsAddEdit && (sender == null || grdPortal.zIsValidSelection(lblPortalId, "chkSelect", CS.PortalId)))
        {
            LoadPortalDetail();
            popupPortal.Show();
        }
    }

    protected void lnkActive_OnClick(object sender, EventArgs e)
    {
        if (grdPortal.zIsValidSelection(lblPortalId, "chkSelect", CS.PortalId))
        {
            if (new Portal()
            {
                PortalId = lblPortalId.zToInt(),
                eStatus = (int)eStatus.Active
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Portal is Already Active.");
                return;
            }
            Confirmationpopup.SetPopupType(ePopupType.Active, "Active Portal", "Are You Sure To Active Portal?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDeactive_OnClick(object sender, EventArgs e)
    {
        if (grdPortal.zIsValidSelection(lblPortalId, "chkSelect", CS.PortalId))
        {
            if (new Portal()
            {
                PortalId = lblPortalId.zToInt(),
                eStatus = (int)eStatus.Deactive
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Portal is Already Deactive.");
                return;
            }

            //string Message = string.Empty;
            //if(CU.IsPortalUsed(lblPortalId.zToInt().Value,ref Message))
            //{
            //    CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It Cannot Delete.");
            //    return;
            //}

            Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive Portal", "Are You Sure To Deactive Portal?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDelete_OnClick(object sender, EventArgs e)
    {
        if (grdPortal.zIsValidSelection(lblPortalId, "chkSelect", CS.PortalId))
        {
            //string Message = string.Empty;
            //if (CU.IsPortalUsed(lblPortalId.zToInt().Value, ref Message))
            //{
            //    CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It Cannot Delete.");
            //    return;
            //}

            Confirmationpopup.SetPopupType(ePopupType.Delete, string.Empty, "Are You Sure To Delete Portal?");
            popupConfirmation.Show();
        }
    }

    protected void lnkRefresh_OnClick(object sender, EventArgs e)
    {
        LoadPortalGrid(ePageIndex.Custom);
    }


    private void ManagePortalStatus(eStatus Status)
    {
        new Portal
        {
            PortalId = lblPortalId.zToInt(),
            eStatus = (int)Status
        }.Update();
    }

    private void btnActive_OnClick(Object sender, EventArgs e)
    {
        ManagePortalStatus(eStatus.Active);
        CU.ZMessage(eMsgType.Success, string.Empty, "Portal Activated Successfully.");
        LoadPortalGrid(ePageIndex.Custom);
    }

    private void btnDeactive_OnClick(Object sender, EventArgs e)
    {
        ManagePortalStatus(eStatus.Deactive);
        CU.ZMessage(eMsgType.Success, string.Empty, "Portal Deactivated Successfully.");
        LoadPortalGrid(ePageIndex.Custom);
    }

    private void btnDelete_OnClick(Object sender, EventArgs e)
    {
        ManagePortalStatus(eStatus.Delete);
        CU.ZMessage(eMsgType.Success, string.Empty, "Portal Deleted Successfully.");
        LoadPortalGrid(ePageIndex.Custom);
    }


    private bool IsEditMode()
    {
        return !lblPortalId.zIsNullOrEmpty();
    }

    private bool IsValidate()
    {
        if (!ddlPortal.zIsSelect())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Portal.");
            ddlPortal.Focus();
            return false;
        }

        if (txtStoreName.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Store Name.");
            txtStoreName.Focus();
            return false;
        }
        var dtPortal = new Query()
        {
            OrganizationId = lblOrganizationId.zToInt(),
            ePortal = ddlPortal.zToInt(),
            StoreName = txtStoreName.Text,
            eStatusNot = (int)eStatus.Delete
        }.Select(eSP.qry_Portal);

        if (dtPortal.Rows.Count > 0 && dtPortal.Rows[0][CS.PortalId].ToString() != lblPortalId.Text)
        {
            string status = dtPortal.Rows[0][CS.eStatus].zToInt() == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
            CU.ZMessage(eMsgType.Error, string.Empty, "This Portal is already exists" + status + ".");
            txtStoreName.Focus();
            return false;
        }
        return true;
    }

    private void LoadPortalDetail()
    {
        if (IsEditMode())
        {
            lblPopupTitle.Text = "Edit Portal";
            var objPortal = new Portal() { PortalId = lblPortalId.zToInt(), }.SelectList<Portal>()[0];
            ddlPortal.SelectedValue = objPortal.ePortal.ToString();
            txtStoreName.Text = objPortal.StoreName.ToString();
        }
        else
        {
            lblPopupTitle.Text = "New Portal";
            ddlPortal.SelectedValue = "0";
            txtStoreName.Text = string.Empty;
        }
    }

    private bool SaveData()
    {
        if (!IsValidate())
        {
            popupPortal.Show();
            return false;
        }

        string Message = string.Empty;
        var objPortal = new Portal()
        {
            OrganizationId = lblOrganizationId.zToInt(),
            ePortal = ddlPortal.zToInt(),
            StoreName = txtStoreName.Text.Trim().zFirstCharToUpper(),
        };

        if (IsEditMode())
        {
            objPortal.PortalId = lblPortalId.zToInt();
            objPortal.Update();
            Message = "Portal Details Changed Successfully.";
        }
        else
        {
            objPortal.eStatus = (int)eStatus.Active;
            objPortal.Insert();
            Message = "New Portal Added Successfully";
        }

        CU.ZMessage(eMsgType.Success, string.Empty, Message);

        return true;
    }

    protected void btnSavePortal_Click(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadPortalGrid(ePageIndex.Custom);
            popupPortal.Hide();
        }
    }

    protected void btnSaveAndNewPortal_Click(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadPortalGrid(ePageIndex.Custom);
            lnkAdd_OnClick(null, null);
        }
    }


    protected void Control_CheckedChanged(object sender, EventArgs e)
    {
        LoadPortalGrid(ePageIndex.Custom);
        CheckVisibleButton();
    }


    protected void grdPortal_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!IsAddEdit.HasValue)
                IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManagePortal).IsAddEdit;

            DataRowView dataItem = (DataRowView)e.Row.DataItem;

            e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdPortal, "Select$" + e.Row.RowIndex);
            if (dataItem[CS.eStatus].zToInt() != (int)eStatus.Active)
                e.Row.Attributes["class"] = "GridDesableRow ";

            var lnkEditPortal = e.Row.FindControl("lnkEditPortal") as LinkButton;
            var ltrPortal = e.Row.FindControl("ltrPortal") as Literal;

            lnkEditPortal.Visible = IsAddEdit.Value;
            lnkEditPortal.CommandArgument = dataItem.Row[CS.PortalId].ToString();

            ltrPortal.Visible = !IsAddEdit.Value;
            ltrPortal.Text = lnkEditPortal.Text = dataItem.Row[CS.Portal].ToString();
        }
    }

    protected void grdPortal_OnSelectedIndexChanged1(object sender, EventArgs e)
    {
        lblPortalId.Text = grdPortal.Rows[grdPortal.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdPortal, CS.PortalId)].Text;
        lnkEdit_OnClick(null, null);
    }

    protected void lnkEditPortal_OnClick(object sender, EventArgs e)
    {
        lblPortalId.Text = ((LinkButton)sender).CommandArgument.ToString();
        lnkEdit_OnClick(null, null);
    }


    #region Paging

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        LoadPortalGrid(ePageIndex.First);
    }

    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        LoadPortalGrid(ePageIndex.Prev);
    }

    protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
    {
        if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() < 0)
        {
            txtGotoPageNo.Text = "1";
            txtGotoPageNo.Focus();
        }
        LoadPortalGrid(ePageIndex.Custom);
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        LoadPortalGrid(ePageIndex.Next);
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        LoadPortalGrid(ePageIndex.Last);
    }

    protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
    {
        txtGotoPageNo.Text = "1";
        LoadPortalGrid(ePageIndex.Custom);
        Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
    }

    #endregion
}
