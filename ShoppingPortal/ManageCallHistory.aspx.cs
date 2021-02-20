using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI.HtmlControls;

public partial class ManageCallHistory : CompressorPage
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

            txtFromDate.Text = txtToDate.Text = IndianDateTime.Today.ToString(CS.ddMMyyyy);
            LoadSearchUser();
            LoadCallType();

            LoadCallHistoryGrid(ePageIndex.Custom);
            CheckVisibleButton();
        }

        Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
        Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
        Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

        try { grdCallHistory.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }


    private void LoadSearchUser()
    {
        var dtUser = new Query() { FirmId = lblFirmId.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_User);
        CU.FillDropdown(ref ddlSearchUser, dtUser, "-- All User --", CS.UsersId, CS.Name);

        try { ddlSearchUser.SelectedValue = CU.GetUsersId().ToString(); }
        catch { }
    }

    private void LoadCallType()
    {
        var dtCallType = new Query() { eStatus = (int)eStatus.Active, FirmId = lblFirmId.zToInt() }.Select(eSP.qry_CallType);
        var dtSearchCallType = dtCallType.Copy();

        CU.FillDropdown(ref ddlCallType, dtCallType, "-- Select CallType --", CS.CallTypeId, CS.CallTypeName);
        CU.FillDropdown(ref ddlSearchCallType, dtSearchCallType, "-- All CallType --", CS.CallTypeId, CS.CallTypeName);
    }

    private DataTable GetCallHistoryDt(ePageIndex ePageIndex)
    {
        int? Status = null;
        if (chkActive.Checked && !chkDeactive.Checked)
            Status = (int)eStatus.Active;
        else if (!chkActive.Checked && chkDeactive.Checked)
            Status = (int)eStatus.Deactive;

        int? CallDirection = null;
        if (chkSearchIncoming.Checked && !chkSearchOutgoing.Checked)
            CallDirection = (int)eCallDirection.Incoming;
        else if (!chkSearchIncoming.Checked && chkSearchOutgoing.Checked)
            CallDirection = (int)eCallDirection.Outgoing;


        var objQuery = new Query()
        {
            MasterSearch = txtSearch.Text,
            FirmId = lblFirmId.zToInt(),
            UsersId = ddlSearchUser.zIsSelect() ? ddlSearchUser.zToInt() : (int?)null,
            CallTypeId = ddlSearchCallType.zIsSelect() ? ddlSearchCallType.zToInt() : (int?)null,
            eCallDirection = CallDirection,
            FromDate = txtFromDate.zToDate().HasValue ? txtFromDate.zToDate() : (DateTime?)null,
            ToDate = txtToDate.zToDate().HasValue ? txtToDate.zToDate().Value.AddDays(1).AddSeconds(-1) : (DateTime?)null,
            eStatus = Status,
            eStatusNot = (int)eStatus.Delete,
        };

        #region Page Index

        int RecordPerPage = ddlRecordPerPage.zToInt().Value;
        int PageIndexTemp = PageIndex;

        CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref objQuery, ref txtGotoPageNo, ref lblCount);
        PageIndex = PageIndexTemp;

        #endregion Page Index

        return objQuery.Select(eSP.qry_CallHistory);
    }

    private void LoadCallHistoryGrid(ePageIndex ePageIndex)
    {
        DataTable dtCallHistory = GetCallHistoryDt(ePageIndex);

        if (dtCallHistory.Rows.Count > 0)
            lblCount.Text = dtCallHistory.Rows[0][CS.TotalRecord].ToString();
        else
            lblCount.Text = "0";

        divPaging.Visible = (dtCallHistory.Rows.Count > 0);

        txtGotoPageNo.Text = PageIndex.ToString();

        ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

        grdCallHistory.DataSource = dtCallHistory;
        grdCallHistory.DataBind();

        try { grdCallHistory.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void CheckVisibleButton()
    {
        var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCallHistory);

        lnkAdd.Visible = lnkEdit.Visible = objAuthority.IsAddEdit;
        lnkDelete.Visible = objAuthority.IsDelete;

        lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
        lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
    }


    protected void lnkAdd_OnClick(object sender, EventArgs e)
    {
        lblCallHistoryId.Text = string.Empty;
        LoadCallHistoryDetail();
        popupCallHistory.Show();
    }

    protected void lnkEdit_OnClick(object sender, EventArgs e)
    {
        if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCallHistory).IsAddEdit && (sender == null || grdCallHistory.zIsValidSelection(lblCallHistoryId, "chkSelect", CS.CallHistoryId)))
        {
            LoadCallHistoryDetail();
            popupCallHistory.Show();
        }
    }

    protected void lnkRefresh_OnClick(object sender, EventArgs e)
    {
        LoadCallHistoryGrid(ePageIndex.Custom);
    }

    protected void lnkActive_OnClick(object sender, EventArgs e)
    {
        if (grdCallHistory.zIsValidSelection(lblCallHistoryId, "chkSelect", CS.CallHistoryId))
        {
            if (new CallHistory()
            {
                CallHistoryId = lblCallHistoryId.zToInt(),
                eStatus = (int)eStatus.Active
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This CallHistory is already Active.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Active, "Active CallHistory", "Are You Sure To Active CallHistory?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDeactive_OnClick(object sender, EventArgs e)
    {
        if (grdCallHistory.zIsValidSelection(lblCallHistoryId, "chkSelect", CS.CallHistoryId))
        {
            if (new CallHistory()
            {
                CallHistoryId = lblCallHistoryId.zToInt(),
                eStatus = (int)eStatus.Deactive
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This CallHistory is already Deactive.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive CallHistory", "Are You Sure To Deactive CallHistory?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDelete_OnClick(object sender, EventArgs e)
    {
        if (grdCallHistory.zIsValidSelection(lblCallHistoryId, "chkSelect", CS.CallHistoryId))
        {
            Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete CallHistory", "Are You Sure To Delete CallHistory?");
            popupConfirmation.Show();
        }
    }

    protected void Control_CheckedChanged(object sender, EventArgs e)
    {
        LoadCallHistoryGrid(ePageIndex.Custom);
        CheckVisibleButton();
    }


    private void ManageCallHistoryStatus(eStatus Status)
    {
        new CallHistory()
        {
            CallHistoryId = lblCallHistoryId.zToInt(),
            eStatus = (int)Status
        }.Update();
    }

    protected void btnActive_OnClick(object sender, EventArgs e)
    {
        ManageCallHistoryStatus(eStatus.Active);
        CU.ZMessage(eMsgType.Success, string.Empty, "CallHistory Activated Successfully.");
        LoadCallHistoryGrid(ePageIndex.Custom);
    }

    protected void btnDeactive_OnClick(object sender, EventArgs e)
    {
        ManageCallHistoryStatus(eStatus.Deactive);
        CU.ZMessage(eMsgType.Success, string.Empty, "CallHistory Deactive Successfully.");
        LoadCallHistoryGrid(ePageIndex.Custom);
    }

    protected void btnDelete_OnClick(object sender, EventArgs e)
    {
        ManageCallHistoryStatus(eStatus.Delete);
        CU.ZMessage(eMsgType.Success, string.Empty, "CallHistory Delete Successfully.");
        LoadCallHistoryGrid(ePageIndex.Custom);
    }


    protected void grdCallHistory_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!IsAddEdit.HasValue)
                IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCallHistory).IsAddEdit;

            if (IsAddEdit.Value)
                e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdCallHistory, "Select$" + e.Row.RowIndex);

            if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdCallHistory, CS.eStatus)].Text) != (int)eStatus.Active)
                e.Row.Attributes["class"] = "GridDesableRow ";

            DataRowView dataItem = (DataRowView)e.Row.DataItem;

            string CallDirection = "<i title='Incoming' class='fa fa-arrow-circle-o-down iIncoming'></i>";
            if (dataItem[CS.eCallDirection].zToInt() == (int)eCallDirection.Outgoing)
                CallDirection = "<i title='Outgoing' class='fa fa-arrow-circle-o-up iOutgoing'></i>";

            var ltrMobileNo = e.Row.FindControl("ltrMobileNo") as Literal;
            var ltrTime = e.Row.FindControl("ltrTime") as Literal;

            var ltrAudioTag = e.Row.FindControl("ltrAudioTag") as Literal;
            var aDownloadRecording = e.Row.FindControl("aDownloadRecording") as HtmlAnchor;

            ltrMobileNo.Text = CallDirection + " " + dataItem[CS.MobileNo].ToString();
            ltrTime.Text = Convert.ToDateTime(dataItem[CS.Time]).ToString("dd MMM yy HH:mm") + " (" + new TimeSpan(0, 0, 0, dataItem[CS.Duration].zToInt().Value).ToString() + ")";

            string FilePath = CU.GetFilePath(false, ePhotoSize.Original, eFolder.CallRecording, dataItem[CS.CallHistoryId].ToString(), dataItem[CS.Extension].ToString()).Replace("~/", "");
            aDownloadRecording.HRef = FilePath;
            aDownloadRecording.Attributes.Add("download", dataItem[CS.MobileNo].ToString() + "-" + Convert.ToDateTime(dataItem[CS.Time]).ToString("ddMMMyyHHmm") + "-" + dataItem[CS.Duration] + "s" + dataItem[CS.Extension].ToString());

            ltrAudioTag.Text = "<audio controlsList='nodownload' controls><source src='" + FilePath + "' type='audio/" + dataItem[CS.Extension].ToString().Replace(".", "") + "'>Your browser does not support the audio element.</audio>";
        }
    }

    protected void grdCallHistory_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        lblCallHistoryId.Text = grdCallHistory.Rows[grdCallHistory.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdCallHistory, CS.CallHistoryId)].Text;
        lnkEdit_OnClick(null, null);
    }


    private void LoadCallHistoryDetail()
    {
        ddlCallType.Focus();

        if (IsEditMode())
        {
            lblPopupTitle.Text = "Edit Call History";
            var objCallHistory = new CallHistory() { CallHistoryId = lblCallHistoryId.zToInt(), }.SelectList<CallHistory>()[0];

            ddlCallType.SelectedValue = objCallHistory.CallTypeId.ToString();
            txtMobileNo.Text = objCallHistory.MobileNo;
            txtCallTime.Text = objCallHistory.Time.ToString();
            txtDuration.Text = objCallHistory.Duration.ToString();

            rdoIncoming.Checked = objCallHistory.eCallDirection == (int)eCallDirection.Incoming;
            rdoOutgoing.Checked = objCallHistory.eCallDirection == (int)eCallDirection.Outgoing;
        }
        else
        {
            lblPopupTitle.Text = "New Call History";

            ddlCallType.SelectedValue = "0";
            txtMobileNo.Text = string.Empty;
            txtCallTime.Text = IndianDateTime.Now.ToString(CS.ddMMyyyyHHmm);
            txtDuration.Text = string.Empty;

            rdoIncoming.Checked = true;
            rdoOutgoing.Checked = false;
        }
    }

    private bool IsEditMode()
    {
        return !lblCallHistoryId.zIsNullOrEmpty();
    }

    private bool IsValidate()
    {
        if (!ddlCallType.zIsSelect())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Call Type.");
            ddlCallType.Focus();
            return false;
        }

        if (!txtMobileNo.zIsMobile())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid MobileNo.");
            txtMobileNo.Focus();
            return false;
        }

        if (!txtCallTime.zIsDateTime24())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Call Time.");
            txtCallTime.Focus();
            return false;
        }

        if (!txtDuration.zIsNumber())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Call Duration.");
            txtDuration.Focus();
            return false;
        }

        return true;
    }

    private bool SaveData()
    {
        if (!IsValidate())
        {
            popupCallHistory.Show();
            return false;
        }
        string Message = string.Empty;
        var objCallHistory = new CallHistory()
        {
            UsersId = CU.GetUsersId(),
            CallTypeId = ddlCallType.zToInt(),
            MobileNo = txtMobileNo.Text,
            Time = txtCallTime.zToDateTime24(),
            Duration = txtDuration.zToInt(),
            eCallDirection = rdoIncoming.Checked ? (int)eCallDirection.Incoming : (int)eCallDirection.Outgoing
        };

        if (IsEditMode())
        {
            objCallHistory.CallHistoryId = lblCallHistoryId.zToInt();

            if (fuCallRecording.HasFile)
                objCallHistory.Extension = Path.GetExtension(fuCallRecording.FileName);

            objCallHistory.Update();

            Message = "CallHistory Detail Change Sucessfully.";
        }
        else
        {
            objCallHistory.eStatus = (int)eStatus.Active;
            if (fuCallRecording.HasFile)
                objCallHistory.Extension = Path.GetExtension(fuCallRecording.FileName);

            objCallHistory.CallHistoryId = objCallHistory.Insert();

            Message = "New CallHistory Added Sucessfully.";
        }

        if (fuCallRecording.HasFile)
            CU.UploadFile(fuCallRecording, new System.Collections.Generic.List<UploadPhoto>(), eFolder.CallRecording, objCallHistory.CallHistoryId.ToString(), true);

        CU.ZMessage(eMsgType.Success, string.Empty, Message);

        return true;
    }

    protected void btnSave_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadCallHistoryGrid(ePageIndex.Custom);
        }
    }

    protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadCallHistoryGrid(ePageIndex.Custom);
            lnkAdd_OnClick(null, null);
        }
    }

    #region Excel Export

    protected void lnkExcelExport_OnClick(object sender, EventArgs e)
    {
        var dtCallHistory = GetCallHistoryDt(ePageIndex.AllPage);
        dtCallHistory.Columns.Add(CS.CallRecording);

        dtCallHistory.Columns[CS.Name].ColumnName = "UserName";

        foreach (DataRow drCallHistory in dtCallHistory.Rows)
            drCallHistory[CS.CallRecording] = CU.GetFilePath(true, ePhotoSize.Original, eFolder.CallRecording, drCallHistory[CS.CallHistoryId].ToString(), drCallHistory[CS.Extension].ToString()).Replace("~/", "");


        var lstColumns = new System.Collections.Generic.List<string>();
        lstColumns.Add(CS.CallDirection);
        lstColumns.Add(CS.MobileNo);
        lstColumns.Add(CS.Time);
        lstColumns.Add(CS.Duration);
        lstColumns.Add(CS.CallTypeName);
        lstColumns.Add(CS.UserName);
        lstColumns.Add(CS.UsersMobileNo);
        lstColumns.Add(CS.CallRecording);


        var lstColumnsSelected = new System.Collections.Generic.List<string>();
        lstColumnsSelected.Add(CS.CallDirection);
        lstColumnsSelected.Add(CS.MobileNo);
        lstColumnsSelected.Add(CS.Time);
        lstColumnsSelected.Add(CS.Duration);
        lstColumnsSelected.Add(CS.CallTypeName);
        lstColumnsSelected.Add(CS.UserName);
        lstColumnsSelected.Add(CS.UsersMobileNo);
        lstColumnsSelected.Add(CS.CallRecording);

        ExcelExport.SetExportData(dtCallHistory, lstColumns, lstColumnsSelected, "Call History");
        popupExcelExport.Show();
    }


    #endregion

    #region Pagging


    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        LoadCallHistoryGrid(ePageIndex.Prev);
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        LoadCallHistoryGrid(ePageIndex.Next);
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        LoadCallHistoryGrid(ePageIndex.First);
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        LoadCallHistoryGrid(ePageIndex.Last);
    }

    protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
    {
        if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
        {
            txtGotoPageNo.Text = "1";
            txtGotoPageNo.Focus();
        }
        LoadCallHistoryGrid(ePageIndex.Custom);
    }

    protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
    {
        txtGotoPageNo.Text = "1";
        LoadCallHistoryGrid(ePageIndex.Custom);
        Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
    }


    #endregion
}
