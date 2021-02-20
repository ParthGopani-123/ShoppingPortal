using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;

public partial class ManageServiceAvailability : CompressorPage
{
    string PincodeCol, CityCol, StateCol, PrepaidCol, CODCol, ReversePickupCol, PickupCol;
    int PincodeColNo = 0, CityColNo = 1, StateColNo = 2, PrepaidColNo = 3, CODColNo = 4, ReversePickupColNo = 5, PickupColNo = 6;

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
            lblOrganizationId.Text = CU.GetOrganizationId().ToString();

            LoadCourier();
            LoadSearchCountry();
            LoadServiceGrid(ePageIndex.Custom);
            CheckVisibleButton();
        }

        Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
        Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
        Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

        try { grdService.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void LoadCourier()
    {
        var dtCourier = new Courier() { OrganizationId = lblOrganizationId.zToInt(), eStatus = (int)eStatus.Active }.Select(new Courier() { CourierId = 0, CourierName = string.Empty });
        CU.FillDropdown(ref ddlSearchCourier, dtCourier, string.Empty, CS.CourierId, CS.CourierName);
        CU.FillDropdown(ref ddlCourier, dtCourier, "-- Select Courier --", CS.CourierId, CS.CourierName);
        CU.FillDropdown(ref ddlCourierExcel, dtCourier, string.Empty, CS.CourierId, CS.CourierName);

    }

    private void LoadSearchCountry()
    {
        int? CountryId = ddlSearchCountry.zToInt();

        CU.FillDropdown(ref ddlSearchCountry, new Country() { eStatus = (int)eStatus.Active }.Select(), "-- All Country --", CS.CountryId, CS.CountryName);

        try { ddlSearchCountry.SelectedValue = CountryId.ToString(); }
        catch { }

        LoadSearchState();
    }

    private void LoadSearchState()
    {
        int? StateId = ddlSearchState.zToInt();

        CU.FillDropdown(ref ddlSearchState, new State()
        {
            CountryId = ddlSearchCountry.zToInt(),
            eStatus = (int)eStatus.Active
        }.Select(), "-- All State --", CS.StateId, CS.StateName);

        try { ddlSearchState.SelectedValue = StateId.ToString(); }
        catch { }

        LoadSearchCity();
    }

    private void LoadSearchCity()
    {
        int? CityId = ddlSearchState.zToInt();

        CU.FillDropdown(ref ddlSearchCity, new City()
        {
            StateId = ddlSearchState.zToInt(),
            eStatus = (int)eStatus.Active
        }.Select(new City() { CityId = 0, CityName = "" }), "-- All City --", CS.CityId, CS.CityName);

        try { ddlSearchCity.SelectedValue = CityId.ToString(); }
        catch { }
    }


    private DataTable GetServiceDt(ePageIndex ePageIndex)
    {
        int? Status = null;
        if (chkActive.Checked && !chkDeactive.Checked)
            Status = (int)eStatus.Active;
        else if (!chkActive.Checked && chkDeactive.Checked)
            Status = (int)eStatus.Deactive;

        var objQuery = new Query()
        {
            CourierId = ddlSearchCourier.zToInt(),
            CountryId = ddlSearchCountry.zToInt() != 0 ? ddlSearchCountry.zToInt() : (int?)null,
            StateId = ddlSearchState.zToInt() != 0 ? ddlSearchState.zToInt() : (int?)null,
            CityId = ddlSearchCity.zToInt() != 0 ? ddlSearchCity.zToInt() : (int?)null,
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

        return objQuery.Select(eSP.qry_ServiceAvailability);
    }

    private void LoadServiceGrid(ePageIndex ePageIndex)
    {
        DataTable dtServiceAvailability = GetServiceDt(ePageIndex);

        if (dtServiceAvailability.Rows.Count > 0)
            lblCount.Text = dtServiceAvailability.Rows[0][CS.TotalRecord].ToString();
        else
            lblCount.Text = "0";

        divPaging.Visible = (dtServiceAvailability.Rows.Count > 0);

        txtGotoPageNo.Text = PageIndex.ToString();

        ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

        grdService.DataSource = dtServiceAvailability;
        grdService.DataBind();

        try { grdService.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void CheckVisibleButton()
    {
        var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCountry);

        lnkAdd.Visible = lnkEdit.Visible = lnkExcelImport.Visible = objAuthority.IsAddEdit;
        lnkDelete.Visible = objAuthority.IsDelete;

        lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
        lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));

        if (!objAuthority.IsAddEdit && !CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageState).IsView)
            grdService.Attributes.Add("class", grdService.Attributes["class"].ToString().Replace("rowloader", ""));
    }


    protected void lnkAdd_OnClick(object sender, EventArgs e)
    {
        lblServiceAvailabilityId.Text = string.Empty;
        LoadServiceDetail();
        popupService.Show();
    }

    protected void lnkEdit_OnClick(object sender, EventArgs e)
    {
        if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCountry).IsAddEdit && (sender == null || grdService.zIsValidSelection(lblServiceAvailabilityId, "chkSelect", CS.ServiceAvailabilityId)))
        {
            LoadServiceDetail();
            popupService.Show();
        }
    }

    protected void lnkEditService_OnClick(object sender, EventArgs e)
    {
        lblServiceAvailabilityId.Text = ((LinkButton)sender).CommandArgument.ToString();
        lnkEdit_OnClick(null, null);
    }

    protected void lnkRefresh_OnClick(object sender, EventArgs e)
    {
        LoadServiceGrid(ePageIndex.Custom);
    }

    protected void lnkActive_OnClick(object sender, EventArgs e)
    {
        if (grdService.zIsValidSelection(lblServiceAvailabilityId, "chkSelect", CS.ServiceAvailabilityId))
        {
            if (new ServiceAvailability()
            {
                ServiceAvailabilityId = lblServiceAvailabilityId.zToInt(),
                eStatus = (int)eStatus.Active
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Service is already Active.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Active, "Active Service", "Are You Sure To Active Service?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDeactive_OnClick(object sender, EventArgs e)
    {
        if (grdService.zIsValidSelection(lblServiceAvailabilityId, "chkSelect", CS.ServiceAvailabilityId))
        {
            if (new ServiceAvailability()
            {
                ServiceAvailabilityId = lblServiceAvailabilityId.zToInt(),
                eStatus = (int)eStatus.Deactive
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Service is already Deactive.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive Service", "Are You Sure To Deactive Service?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDelete_OnClick(object sender, EventArgs e)
    {
        if (grdService.zIsValidSelection(lblServiceAvailabilityId, "chkSelect", CS.ServiceAvailabilityId))
        {
            Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Service", "Are You Sure To Delete Service?");
            popupConfirmation.Show();
        }
    }


    private void ManageServiceStatus(eStatus Status)
    {
        new ServiceAvailability()
        {
            ServiceAvailabilityId = lblServiceAvailabilityId.zToInt(),
            eStatus = (int)Status
        }.Update();

        LoadServiceGrid(ePageIndex.Custom);
    }

    protected void btnActive_OnClick(object sender, EventArgs e)
    {
        ManageServiceStatus(eStatus.Active);
        CU.ZMessage(eMsgType.Success, string.Empty, "Service Activated Successfully.");
    }

    protected void btnDeactive_OnClick(object sender, EventArgs e)
    {
        ManageServiceStatus(eStatus.Deactive);
        CU.ZMessage(eMsgType.Success, string.Empty, "Service Deactive Successfully.");
    }

    protected void btnDelete_OnClick(object sender, EventArgs e)
    {
        ManageServiceStatus(eStatus.Delete);
        CU.ZMessage(eMsgType.Success, string.Empty, "Service Delete Successfully.");
    }


    protected void Control_CheckedChanged(object sender, EventArgs e)
    {
        LoadServiceGrid(ePageIndex.Custom);
        CheckVisibleButton();
    }

    protected void lnkRefreshCountry_OnClick(object sender, EventArgs e)
    {
        LoadSearchCountry();
    }

    protected void lnkRefreshState_OnClick(object sender, EventArgs e)
    {
        LoadSearchState();
    }

    protected void lnkRefreshCity_OnClick(object sender, EventArgs e)
    {
        LoadSearchCity();
    }


    protected void ddlSearchCountry_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        LoadSearchState();
    }

    protected void ddlSearchState_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        LoadSearchCity();
    }


    protected void grdService_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!IsAddEdit.HasValue)
                IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCountry).IsAddEdit;

            if (IsAddEdit.Value)
                e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdService, "Select$" + e.Row.RowIndex);

            if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdService, CS.eStatus)].Text) != (int)eStatus.Active)
                e.Row.Attributes["class"] = "GridDesableRow ";

            DataRowView dataItem = (DataRowView)e.Row.DataItem;
            var lnkEditService = e.Row.FindControl("lnkEditService") as LinkButton;
            var ltrService = e.Row.FindControl("ltrService") as Literal;
            var lblServiceDetail = e.Row.FindControl("lblServiceDetail") as Label;

            lnkEditService.Visible = IsAddEdit.Value;
            ltrService.Visible = !IsAddEdit.Value;

            lnkEditService.Text = ltrService.Text = dataItem[CS.Pincode].ToString();
            lnkEditService.CommandArgument = dataItem[CS.ServiceAvailabilityId].ToString();

            lblServiceDetail.Text = dataItem[CS.CityName].ToString() + ", " + dataItem[CS.StateName].ToString() + ", " + dataItem[CS.CountryName].ToString();
        }
    }

    protected void grdService_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        lblServiceAvailabilityId.Text = grdService.Rows[grdService.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdService, CS.ServiceAvailabilityId)].Text;
        lnkEdit_OnClick(null, null);
    }


    #region Add / Edit

    private void LoadCountry()
    {
        CU.FillDropdown(ref ddlCountry, new Query() { eStatus = (int)eStatus.Active }.Select(eSP.qry_Country), "-- Select Country --", CS.CountryId, CS.CountryName);
    }

    private void LoadState()
    {
        CU.FillDropdown(ref ddlState, new Query() { CountryId = ddlCountry.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_State), "-- Select State --", CS.StateId, CS.StateName);
    }

    private void LoadCity()
    {
        CU.FillDropdown(ref ddlCity, new Query() { StateId = ddlState.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_City), "-- Select City --", CS.CityId, CS.CityName);
    }


    private void LoadServiceDetail()
    {
        LoadCountry();

        ddlCourier.Focus();

        if (IsEditMode())
        {
            lblPopupTitle.Text = "Edit Service";

            var dtServiceAvailability = new Query() { ServiceAvailabilityId = lblServiceAvailabilityId.zToInt() }.Select(eSP.qry_ServiceAvailability);

            var objServiceAvailability = new ServiceAvailability() { }.SelectList<ServiceAvailability>(dtServiceAvailability.Select())[0];

            ddlCourier.SelectedValue = objServiceAvailability.CourierId.ToString();

            var dr = dtServiceAvailability.Rows[0];
            ddlCountry.SelectedValue = dr[CS.CountryId].ToString();

            LoadState();
            ddlState.SelectedValue = dr[CS.StateId].ToString();

            LoadCity();
            ddlCity.SelectedValue = dr[CS.CityId].ToString();

            txtPincode.Text = objServiceAvailability.Pincode;

            chkIsCODAvailable.Checked = objServiceAvailability.eCOD == (int)eYesNo.Yes;
            chkIsPrepaidAvailable.Checked = objServiceAvailability.ePrepaid == (int)eYesNo.Yes;
            chkIsReversePickupAvailable.Checked = objServiceAvailability.eReversePickup == (int)eYesNo.Yes;
            chkIsPickupAvailable.Checked = objServiceAvailability.ePickup == (int)eYesNo.Yes;
        }
        else
        {
            lblPopupTitle.Text = "New Service";

            LoadState();
            LoadCity();

            txtPincode.Text = string.Empty;
            chkIsCODAvailable.Checked = chkIsPrepaidAvailable.Checked = chkIsReversePickupAvailable.Checked = chkIsPickupAvailable.Checked = false;
        }
    }


    private bool IsEditMode()
    {
        return !lblServiceAvailabilityId.zIsNullOrEmpty();
    }

    private bool IsValidate()
    {
        if (!ddlCourier.zIsSelect())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Courier.");
            ddlCourier.Focus();
            return false;
        }

        if (!ddlCountry.zIsSelect())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Country.");
            ddlCountry.Focus();
            return false;
        }

        if (!ddlState.zIsSelect())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select State.");
            ddlState.Focus();
            return false;
        }

        if (!ddlCity.zIsSelect())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select City.");
            ddlCity.Focus();
            return false;
        }

        if (txtPincode.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Pincode.");
            txtPincode.Focus();
            return false;
        }

        var dtServiceAvailability = new Query()
        {
            CourierId = ddlCourier.zToInt(),
            eStatusNot = (int)eStatus.Delete,
            Pincode = txtPincode.Text,
        }.Select(eSP.qry_ServiceAvailability);

        if (dtServiceAvailability.Rows.Count > 0 && dtServiceAvailability.Rows[0][CS.ServiceAvailabilityId].ToString() != lblServiceAvailabilityId.Text)
        {
            string Status = dtServiceAvailability.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
            CU.ZMessage(eMsgType.Error, string.Empty, "This Pincode is already exist" + Status + ".");
            txtPincode.Focus();
            return false;
        }

        return true;
    }

    private bool SaveData()
    {
        if (!IsValidate())
        {
            popupService.Show();
            return false;
        }

        string Message = string.Empty;

        var objServiceAvailability = new ServiceAvailability()
        {
            CourierId = ddlCourier.zToInt(),
            CityId = ddlCity.zToInt(),
            Pincode = txtPincode.Text,
            eCOD = chkIsCODAvailable.Checked ? (int)eYesNo.Yes : (int)eYesNo.No,
            ePrepaid = chkIsPrepaidAvailable.Checked ? (int)eYesNo.Yes : (int)eYesNo.No,
            eReversePickup = chkIsReversePickupAvailable.Checked ? (int)eYesNo.Yes : (int)eYesNo.No,
            ePickup = chkIsPickupAvailable.Checked ? (int)eYesNo.Yes : (int)eYesNo.No,
        };

        if (IsEditMode())
        {
            objServiceAvailability.ServiceAvailabilityId = lblServiceAvailabilityId.zToInt();
            objServiceAvailability.Update();

            Message = "Service Detail Change Sucessfully.";
        }
        else
        {
            objServiceAvailability.eStatus = (int)eStatus.Active;
            objServiceAvailability.Insert();

            Message = "New Service Added Sucessfully.";
        }

        CU.ZMessage(eMsgType.Success, string.Empty, Message);

        return true;
    }


    protected void btnSave_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadServiceGrid(ePageIndex.Custom);
        }
    }

    protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadServiceGrid(ePageIndex.Custom);
            lnkAdd_OnClick(null, null);
        }
    }


    protected void ddlCountry_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        LoadState();
        LoadCity();
        popupService.Show();
    }

    protected void ddlState_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        LoadCity();
        popupService.Show();
    }

    #endregion


    #region Excel Import / Export

    protected void lnkExcelExport_OnClick(object sender, EventArgs e)
    {
        var dtService = GetServiceDt(ePageIndex.AllPage);
        var lstColumns = new System.Collections.Generic.List<string>();
        lstColumns.Add("Pincode");
        lstColumns.Add("CityName");
        lstColumns.Add("StateName");
        lstColumns.Add("CountryName");
        lstColumns.Add("COD");
        lstColumns.Add("Prepaid");
        lstColumns.Add("ReversePickup");
        lstColumns.Add("Pickup");

        var lstColumnsSelected = new System.Collections.Generic.List<string>();
        lstColumnsSelected.Add("Pincode");
        lstColumnsSelected.Add("CityName");
        lstColumnsSelected.Add("StateName");
        lstColumnsSelected.Add("COD");
        lstColumnsSelected.Add("Prepaid");
        lstColumnsSelected.Add("ReversePickup");
        lstColumnsSelected.Add("Pickup");

        ExcelExport.SetExportData(dtService, lstColumns, lstColumnsSelected, "Service");
        popupExcelExport.Show();
    }


    protected void lnkExcelImport_OnClick(object sender, EventArgs e)
    {
        chkReplace.Checked = false;
        popupExcelImport.Show();
    }

    protected void btnUpload_OnClick(object sender, EventArgs e)
    {
        if (!ddlCourierExcel.zIsSelect())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Courier.");
            ddlCourierExcel.Focus();
            popupExcelImport.Show();
            return;
        }

        if (fuImportExcel.HasFile)
        {
            var dt = new DataTable();
            if (!CU.IsValidExcelFile(fuImportExcel, ref dt, 7, "Service"))
            {
                popupExcelImport.Show();
                return;
            }

            if (CheckData(dt))
                InsertData(dt);
            else
                popupExcelImport.Show();
        }
        else
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Excel File to upload.");
            popupExcelImport.Show();
        }
    }

    private bool CheckData(DataTable dt)
    {
        int TotalCount = 0, SuccessCount = 0, FailCount = 0;
        string Message = string.Empty;

        try
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                TotalCount++;
                bool IsValid = true;

                string Connecter = " in Record-" + TotalCount.ToString() + ".<br />";

                #region Value Initialization

                PincodeCol = dt.Rows[i][PincodeColNo].ToString().Trim().TrimEnd(',');
                CityCol = dt.Rows[i][CityColNo].ToString().Trim().TrimEnd(',');
                StateCol = dt.Rows[i][StateColNo].ToString().Trim().TrimEnd(',');
                PrepaidCol = dt.Rows[i][PrepaidColNo].ToString().Trim().TrimEnd(',');
                CODCol = dt.Rows[i][CODColNo].ToString().Trim().TrimEnd(',');
                ReversePickupCol = dt.Rows[i][ReversePickupColNo].ToString().Trim().TrimEnd(',');
                PickupCol = dt.Rows[i][PickupColNo].ToString().Trim().TrimEnd(',');

                #endregion

                #region Check Pincode

                if (IsValid)
                {
                    if (PincodeCol.zIsNullOrEmpty())
                    {
                        Message += CS.Arrow + "Pincode Is Empty" + Connecter;
                        IsValid = false;
                    }
                }

                //if (IsValid)
                //{
                //	string RepeateColumn = string.Empty;
                //	if (CU.IsRepeateExcelRow(dt, i, PincodeCol, PincodeColNo, string.Empty, null, string.Empty, null, ref RepeateColumn))
                //	{
                //		Message += CS.Arrow + "Pincode " + PincodeCol + "  is Repeating in Record-" + RepeateColumn;
                //		IsValid = false;
                //	}
                //}

                if (IsValid)
                {
                    //DataTable dtService = new Query()
                    //{
                    //    CourierId = ddlCourierExcel.zToInt(),
                    //    Pincode = PincodeCol.zToInt(),
                    //    eStatusNot = (int)eStatus.Delete
                    //}.Select(eSP.qry_ServiceAvailability);

                    //if (dtService.Rows.Count > 0 && !chkReplace.Checked)
                    //{
                    //    string Status = dtService.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
                    //    Message += CS.Arrow + "This Pincode is already exist" + Status + "." + Connecter;
                    //    IsValid = false;
                    //}
                }

                #endregion

                #region check Available

                bool IsValidateAvailable = false;

                if (IsValid)
                {
                    GeteYesNo(PrepaidCol, ref IsValidateAvailable);

                    if (!IsValidateAvailable)
                    {
                        Message += CS.Arrow + "Prepaid Value Invalid" + Connecter;
                        IsValid = false;
                    }
                }

                if (IsValid)
                {
                    GeteYesNo(CODCol, ref IsValidateAvailable);

                    if (!IsValidateAvailable)
                    {
                        Message += CS.Arrow + "COD Value Invalid" + Connecter;
                        IsValid = false;
                    }
                }

                if (IsValid)
                {
                    GeteYesNo(ReversePickupCol, ref IsValidateAvailable);

                    if (!IsValidateAvailable)
                    {
                        Message += CS.Arrow + "Reverse Pickup Value Invalid" + Connecter;
                        IsValid = false;
                    }
                }

                if (IsValid)
                {
                    GeteYesNo(PickupCol, ref IsValidateAvailable);

                    if (!IsValidateAvailable)
                    {
                        Message += CS.Arrow + "Pickup Value Invalid" + Connecter;
                        IsValid = false;
                    }
                }

                #endregion

                if (IsValid)
                    SuccessCount++;
                else
                {
                    FailCount++;
                    if (FailCount >= 10)
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            CU.ZMessage(eMsgType.Error, string.Empty, ex.Message, 0);
            return false;
        }

        if (FailCount == 0)
            return true;
        else
        {
            CU.SetErrorExcelMessage(Message, SuccessCount, FailCount);
            return false;
        }
    }

    private void InsertData(DataTable dt)
    {
        int UpdateCount = 0, InsertCount = 0;

        try
        {
            var lstInsertSA = new List<ServiceAvailability>();
            var lstUpdateSA = new List<ServiceAvailability>();
            var lstDeleteSA = new List<ServiceAvailability>();

            int CourierId = ddlCourierExcel.zToInt().Value;

            int CountryId = CU.GetCountryId("India");
            bool IsValidate = false;

            DataTable dtServiceAvailibility = new Query()
            {
                CourierId = CourierId,
                eStatusNot = (int)eStatus.Delete
            }.Select(eSP.qry_ServiceAvailability);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                #region Value Initialization

                PincodeCol = dt.Rows[i][PincodeColNo].ToString().Trim().TrimEnd(',');
                CityCol = dt.Rows[i][CityColNo].ToString().Trim().TrimEnd(',');
                StateCol = dt.Rows[i][StateColNo].ToString().Trim().TrimEnd(',');
                PrepaidCol = dt.Rows[i][PrepaidColNo].ToString().Trim().TrimEnd(',').ToLower();
                CODCol = dt.Rows[i][CODColNo].ToString().Trim().TrimEnd(',').ToLower();
                ReversePickupCol = dt.Rows[i][ReversePickupColNo].ToString().Trim().TrimEnd(',').ToLower();
                PickupCol = dt.Rows[i][PickupColNo].ToString().Trim().TrimEnd(',').ToLower();

                #endregion

                int StateId = CU.GetStateId(CountryId, StateCol);
                int CityId = CU.GetCityId(StateId, CityCol);

                var drServiceAvailibility = dtServiceAvailibility.Select(CS.Pincode + " = '" + PincodeCol + "'");

                var objServiceAvailability = new ServiceAvailability()
                {
                    ServiceAvailabilityId = drServiceAvailibility.Length > 0 ? drServiceAvailibility[0][CS.ServiceAvailabilityId].zToInt() : (int?)null,
                    CourierId = CourierId,
                    Pincode = PincodeCol,
                    CityId = CityId,
                    ePrepaid = (int)GeteYesNo(PrepaidCol, ref IsValidate),
                    eCOD = (int)GeteYesNo(CODCol, ref IsValidate),
                    eReversePickup = (int)GeteYesNo(ReversePickupCol, ref IsValidate),
                    ePickup = (int)GeteYesNo(PickupCol, ref IsValidate),
                };

                if (objServiceAvailability.ServiceAvailabilityId.HasValue)
                {
                    lstUpdateSA.Add(objServiceAvailability);
                    if (lstUpdateSA.Count >= 250)
                    {
                        lstUpdateSA.Update();
                        lstUpdateSA = new List<ServiceAvailability>();
                    }

                    UpdateCount++;
                }
                else
                {
                    objServiceAvailability.eStatus = (int)eStatus.Active;
                    lstInsertSA.Add(objServiceAvailability);
                    if (lstInsertSA.Count >= 250)
                    {
                        lstInsertSA.Insert();
                        lstInsertSA = new List<ServiceAvailability>();
                    }

                    InsertCount++;
                }
            }

            foreach (DataRow dr in dtServiceAvailibility.Rows)
            {
                lstDeleteSA.Add(new ServiceAvailability() { ServiceAvailabilityId = dr[CS.ServiceAvailabilityId].zToInt() });
                if (lstDeleteSA.Count >= 250)
                {
                    lstDeleteSA.Delete();
                    lstDeleteSA = new List<ServiceAvailability>();
                }
            }

            lstUpdateSA.Update();
            lstInsertSA.Insert();
            lstDeleteSA.Delete();

            CU.SetSuccessExcelMessage(InsertCount, UpdateCount, "Service");
        }
        catch (Exception ex)
        {
            CU.ZMessage(eMsgType.Error, string.Empty, ex.Message, 0);
        }

        LoadServiceGrid(ePageIndex.Custom);
    }

    private eYesNo GeteYesNo(string Val, ref bool IsValidate)
    {
        IsValidate = true;

        Val = Val.Trim().ToLower();

        if (Val.zIsNullOrEmpty())
        {
            return eYesNo.No;
        }
        else
        {
            if (Val == "y" || Val == "yes")
                return eYesNo.Yes;
            else if (Val == "n" || Val == "no")
                return eYesNo.No;
            else
            {
                IsValidate = false;
                return eYesNo.No;
            }
        }
    }

    #endregion


    #region Pagging


    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        LoadServiceGrid(ePageIndex.Prev);
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        LoadServiceGrid(ePageIndex.Next);
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        LoadServiceGrid(ePageIndex.First);
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        LoadServiceGrid(ePageIndex.Last);
    }

    protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
    {
        if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
        {
            txtGotoPageNo.Text = "1";
            txtGotoPageNo.Focus();
        }
        LoadServiceGrid(ePageIndex.Custom);
    }

    protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
    {
        txtGotoPageNo.Text = "1";
        LoadServiceGrid(ePageIndex.Custom);
        Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
    }


    #endregion
}
