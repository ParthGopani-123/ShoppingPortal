using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;

public partial class ManageCourier : CompressorPage
{
    bool? IsAddEdit;

    bool IsSetFocus;

    DataTable dtCourierType, dtZone, dtShippingChargeData;

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
            SetControl(eControl.Courier);

            lblOrganizationId.Text = CU.GetOrganizationId().ToString();
            lblFirmId.Text = CU.GetFirmId().ToString();
            CU.LoadDisplayPerPage(ref ddlRecordPerPage);

            LoadCourierGrid(ePageIndex.Custom);
            CheckVisibleButton();
        }

        Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
        Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
        Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

        try { grdCourier.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }


    private DataTable GetCourierDt(ePageIndex ePageIndex)
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

        return objQuery.Select(eSP.qry_Courier);
    }

    private void LoadCourierGrid(ePageIndex ePageIndex)
    {
        DataTable dtCourier = GetCourierDt(ePageIndex);

        if (dtCourier.Rows.Count > 0)
            lblCount.Text = dtCourier.Rows[0][CS.TotalRecord].ToString();
        else
            lblCount.Text = "0";

        divPaging.Visible = (dtCourier.Rows.Count > 0);

        txtGotoPageNo.Text = PageIndex.ToString();

        ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

        grdCourier.DataSource = dtCourier;
        grdCourier.DataBind();

        try { grdCourier.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void CheckVisibleButton()
    {
        var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCourier);

        lnkAdd.Visible = lnkEdit.Visible = objAuthority.IsAddEdit;
        lnkDelete.Visible = objAuthority.IsDelete;

        lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
        lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
    }


    protected void lnkAdd_OnClick(object sender, EventArgs e)
    {
        lblCourierId.Text = string.Empty;
        LoadCourierDetail();
        popupCourier.Show();
    }

    protected void lnkEdit_OnClick(object sender, EventArgs e)
    {
        if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCourier).IsAddEdit && (sender == null || grdCourier.zIsValidSelection(lblCourierId, "chkSelect", CS.CourierId)))
        {
            LoadCourierDetail();
            popupCourier.Show();
        }
    }

    protected void lnkEditCourier_OnClick(object sender, EventArgs e)
    {
        lblCourierId.Text = ((LinkButton)sender).CommandArgument.ToString();
        lnkEdit_OnClick(null, null);
    }

    protected void lnkRefresh_OnClick(object sender, EventArgs e)
    {
        LoadCourierGrid(ePageIndex.Custom);
    }

    protected void lnkActive_OnClick(object sender, EventArgs e)
    {
        if (grdCourier.zIsValidSelection(lblCourierId, "chkSelect", CS.CourierId))
        {
            if (new Courier()
            {
                CourierId = lblCourierId.zToInt(),
                eStatus = (int)eStatus.Active
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Courier is already Active.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Active, "Active Courier", "Are You Sure To Active Courier?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDeactive_OnClick(object sender, EventArgs e)
    {
        if (grdCourier.zIsValidSelection(lblCourierId, "chkSelect", CS.CourierId))
        {
            if (new Courier()
            {
                CourierId = lblCourierId.zToInt(),
                eStatus = (int)eStatus.Deactive
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Courier is already Deactive.");
                return;
            }

            string Message = string.Empty;
            if (CU.IsCourierUsed(lblCourierId.zToInt().Value, ref Message))
            {
                CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Deactive.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive Courier", "Are You Sure To Deactive Courier?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDelete_OnClick(object sender, EventArgs e)
    {
        if (grdCourier.zIsValidSelection(lblCourierId, "chkSelect", CS.CourierId))
        {
            string Message = string.Empty;
            if (CU.IsCourierUsed(lblCourierId.zToInt().Value, ref Message))
            {
                CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Delete.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Courier", "Are You Sure To Delete Courier?");
            popupConfirmation.Show();
        }
    }

    protected void Control_CheckedChanged(object sender, EventArgs e)
    {
        LoadCourierGrid(ePageIndex.Custom);
        CheckVisibleButton();
    }


    private void ManageCourierStatus(eStatus Status)
    {
        new Courier()
        {
            CourierId = lblCourierId.zToInt(),
            eStatus = (int)Status
        }.Update();
    }

    protected void btnActive_OnClick(object sender, EventArgs e)
    {
        ManageCourierStatus(eStatus.Active);
        CU.ZMessage(eMsgType.Success, string.Empty, "Courier Activated Successfully.");
        LoadCourierGrid(ePageIndex.Custom);
    }

    protected void btnDeactive_OnClick(object sender, EventArgs e)
    {
        ManageCourierStatus(eStatus.Deactive);
        CU.ZMessage(eMsgType.Success, string.Empty, "Courier Deactive Successfully.");
        LoadCourierGrid(ePageIndex.Custom);
    }

    protected void btnDelete_OnClick(object sender, EventArgs e)
    {
        ManageCourierStatus(eStatus.Delete);
        CU.ZMessage(eMsgType.Success, string.Empty, "Courier Delete Successfully.");
        LoadCourierGrid(ePageIndex.Custom);
    }


    protected void grdCourier_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!IsAddEdit.HasValue)
                IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageCourier).IsAddEdit;

            if (IsAddEdit.Value)
                e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdCourier, "Select$" + e.Row.RowIndex);

            if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdCourier, CS.eStatus)].Text) != (int)eStatus.Active)
                e.Row.Attributes["class"] = "GridDesableRow ";

            DataRowView dataItem = (DataRowView)e.Row.DataItem;
            var lnkEditCourier = e.Row.FindControl("lnkEditCourier") as LinkButton;
            var ltrCourier = e.Row.FindControl("ltrCourier") as Literal;

            lnkEditCourier.Visible = IsAddEdit.Value;
            ltrCourier.Visible = !IsAddEdit.Value;

            lnkEditCourier.Text = ltrCourier.Text = dataItem[CS.CourierName].ToString() + (dataItem[CS.IsPost].zToInt() == (int)eYesNo.Yes ? "*" : string.Empty);
            lnkEditCourier.CommandArgument = dataItem[CS.CourierId].ToString();
        }
    }

    protected void grdCourier_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        lblCourierId.Text = grdCourier.Rows[grdCourier.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdCourier, CS.CourierId)].Text;
        lnkShippingCharge_OnClick(null, null);
    }


    private void LoadCourierDetail()
    {
        txtCourierName.Focus();


        if (IsEditMode())
        {
            lblPopupTitle.Text = "Edit Courier";
            var objCourier = new Courier() { CourierId = lblCourierId.zToInt(), }.SelectList<Courier>()[0];
            txtCourierName.Text = objCourier.CourierName;
            txtCODTrackingURL.Text = objCourier.CODTrackingURL;
            txtPrepaidTrackingURL.Text = objCourier.PrepaidTrackingURL;
            chkIsPost.Checked = objCourier.IsPost == (int)eYesNo.Yes;
            txtSerialNo.Text = objCourier.SerialNo.ToString();
            txtCarrierCode.Text = objCourier.CarrierCode;
        }
        else
        {
            lblPopupTitle.Text = "New Courier";
            txtCourierName.Text = txtCODTrackingURL.Text = txtPrepaidTrackingURL.Text = txtCarrierCode.Text = string.Empty;
            chkIsPost.Checked = false;
            var drMaxSerialNo = new Query() { eStatusNot = (int)eStatus.Delete, OrganizationId = lblOrganizationId.zToInt() }.Select(eSP.qry_Max_CourierSerialNo).Rows[0];
            int? SerialNo = drMaxSerialNo[CS.SerialNo].zToInt();
            txtSerialNo.Text = SerialNo.HasValue ? (SerialNo + 1).ToString() : "1";
        }
    }

    private bool IsEditMode()
    {
        return !lblCourierId.zIsNullOrEmpty();
    }

    private bool IsValidate()
    {
        if (txtCourierName.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Courier Name.");
            txtCourierName.Focus();
            return false;
        }

        var dtCourier = new Query()
        {
            OrganizationId = lblOrganizationId.zToInt(),
            eStatusNot = (int)eStatus.Delete,
            CourierName = txtCourierName.Text.Trim(),
        }.Select(eSP.qry_Courier);

        if (dtCourier.Rows.Count > 0 && dtCourier.Rows[0][CS.CourierId].ToString() != lblCourierId.Text)
        {
            string Status = dtCourier.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
            CU.ZMessage(eMsgType.Error, string.Empty, "This Courier is already exist" + Status + ".");
            txtCourierName.Focus();
            return false;
        }

        if (!txtSerialNo.zIsInteger(false))
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Serial No.");
            txtSerialNo.Focus();
            return false;
        }

        return true;
    }

    private bool SaveData()
    {
        if (!IsValidate())
            return false;

        string Message = string.Empty;

        #region Check Post Curiar

        if (chkIsPost.Checked)
        {
            var dtCourier = new Courier()
            {
                OrganizationId = lblOrganizationId.zToInt(),
                IsPost = (int)eYesNo.Yes,
            }.Select();

            if (dtCourier.Rows.Count > 0)
            {
                var lstUpdateCourier = new List<Courier>();
                foreach (DataRow drCourier in dtCourier.Rows)
                {
                    lstUpdateCourier.Add(new Courier()
                    {
                        CourierId = drCourier[CS.CourierId].zToInt(),
                        IsPost = (int)eYesNo.No
                    });
                }

                lstUpdateCourier.Update();
            }
        }

        #endregion

        var objCourier = new Courier()
        {
            OrganizationId = lblOrganizationId.zToInt(),
            SerialNo = txtSerialNo.zToInt(),
            CarrierCode = txtCarrierCode.Text,
            CourierName = txtCourierName.Text.Trim().zFirstCharToUpper(),
            CODTrackingURL = txtCODTrackingURL.Text,
            PrepaidTrackingURL = txtPrepaidTrackingURL.Text,
            IsPost = chkIsPost.Checked ? (int)eYesNo.Yes : (int)eYesNo.No,
        };

        if (IsEditMode())
        {
            objCourier.CourierId = lblCourierId.zToInt();
            objCourier.Update();

            Message = "Courier Detail Change Sucessfully.";
        }
        else
        {
            objCourier.eStatus = (int)eStatus.Active;
            objCourier.Insert();

            Message = "New Courier Added Sucessfully.";
        }

        CU.ZMessage(eMsgType.Success, string.Empty, Message);

        return true;
    }

    protected void btnSave_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadCourierGrid(ePageIndex.Custom);
        }
    }

    protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadCourierGrid(ePageIndex.Custom);
            lnkAdd_OnClick(null, null);
        }
    }


    #region Pagging


    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        LoadCourierGrid(ePageIndex.Prev);
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        LoadCourierGrid(ePageIndex.Next);
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        LoadCourierGrid(ePageIndex.First);
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        LoadCourierGrid(ePageIndex.Last);
    }

    protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
    {
        if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
        {
            txtGotoPageNo.Text = "1";
            txtGotoPageNo.Focus();
        }
        LoadCourierGrid(ePageIndex.Custom);
    }

    protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
    {
        txtGotoPageNo.Text = "1";
        LoadCourierGrid(ePageIndex.Custom);
        Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
    }


    #endregion

    #region Shipping Charge

    protected void lnkShippingCharge_OnClick(object sender, EventArgs e)
    {
        if (sender != null && !grdCourier.zIsValidSelection(lblCourierId, "chkSelect", CS.CourierId))
            return;

        ManageShippingCharge(null, eRepeaterOperation.Select);

        rptZoneHead.DataSource = dtZone;
        rptZoneHead.DataBind();

        rptZoneHead2.DataSource = dtZone;
        rptZoneHead2.DataBind();

        rptZoneHead3.DataSource = dtZone;
        rptZoneHead3.DataBind();

        SetControl(eControl.ShippingCharge);
    }


    protected void rptZoneHead_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var ltrZone = e.Item.FindControl("ltrZone") as Literal;
        var tdZone = e.Item.FindControl("tdZone") as HtmlControl;

        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;

        ltrZone.Text = dataItem[CS.Name].ToString();
        tdZone.Attributes.Add("colspan", (dtCourierType.Rows.Count * 2).ToString());
    }

    protected void rptZoneHead2_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var rptCourierTypeHead = e.Item.FindControl("rptCourierTypeHead") as Repeater;

        rptCourierTypeHead.DataSource = dtCourierType;
        rptCourierTypeHead.DataBind();
    }

    protected void rptCourierTypeHead_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var ltrCourierTypeHead = e.Item.FindControl("ltrCourierTypeHead") as Literal;

        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;

        ltrCourierTypeHead.Text = dataItem[CS.Name].ToString();
    }


    private bool ManageShippingCharge(int? PK, eRepeaterOperation RepeaterOperation)
    {
        var dtShippingCharge = new DataTable();

        var lstShippingChargeInsert = new List<ShippingCharge>();
        var lstShippingChargeUpdate = new List<ShippingCharge>();

        int CourierId = lblCourierId.zToInt().Value;

        if (RepeaterOperation == eRepeaterOperation.Save)
        {
            dtShippingCharge = new Query()
            {
                FirmId = lblFirmId.zToInt(),
                CourierId = CourierId,
            }.Select(eSP.qry_ShippingCharge);
        }
        else if (RepeaterOperation != eRepeaterOperation.Validate)
        {
            dtShippingCharge = new DataTable();
            dtShippingCharge.Columns.Add(CS.eCourierType);
            dtShippingCharge.Columns.Add(CS.eZone);
            dtShippingCharge.Columns.Add(CS.Weight);
            dtShippingCharge.Columns.Add(CS.ShipCharge);
            dtShippingCharge.Columns.Add(CS.FirmShipCharge);
        }

        if (RepeaterOperation == eRepeaterOperation.Select)
        {
            #region Get Data From DB

            var dt = new Query()
            {
                FirmId = lblFirmId.zToInt(),
                CourierId = CourierId,
            }.Select(eSP.qry_ShippingCharge);

            foreach (DataRow dr in dt.Rows)
            {
                var drShippingCharge = dtShippingCharge.NewRow();

                drShippingCharge[CS.eCourierType] = dr[CS.eCourierType].ToString();
                drShippingCharge[CS.eZone] = dr[CS.eZone].ToString();
                drShippingCharge[CS.Weight] = dr[CS.Weight].ToString();
                drShippingCharge[CS.ShipCharge] = dr[CS.ShipCharge].ToString();
                drShippingCharge[CS.FirmShipCharge] = dr[CS.FirmShipCharge].ToString();

                dtShippingCharge.Rows.Add(drShippingCharge);
            }

            #endregion
        }
        else
        {
            #region Manage Data 

            foreach (RepeaterItem Item in rptShippingCharge.Items)
            {
                var lblPK = Item.FindControl("lblPK") as Label;
                var txtWeight = Item.FindControl("txtWeight") as TextBox;

                if (RepeaterOperation == eRepeaterOperation.Remove && PK == lblPK.zToInt())
                    continue;

                if (RepeaterOperation == eRepeaterOperation.Save && (!txtWeight.zIsDecimal(false) || txtWeight.zToDecimal() <= 0))
                    continue;

                var rptZone = Item.FindControl("rptZone") as Repeater;
                foreach (RepeaterItem ItemZone in rptZone.Items)
                {
                    var rptCourierType = ItemZone.FindControl("rptCourierType") as Repeater;
                    foreach (RepeaterItem ItemCourierType in rptCourierType.Items)
                    {
                        var lblCourierTypeId = ItemCourierType.FindControl("lblCourierTypeId") as Label;
                        var lblZoneId = ItemCourierType.FindControl("lblZoneId") as Label;
                        var txtShipCharge = ItemCourierType.FindControl("txtShipCharge") as TextBox;
                        var txtFirmShipCharge = ItemCourierType.FindControl("txtFirmShipCharge") as TextBox;

                        if (RepeaterOperation == eRepeaterOperation.Add || RepeaterOperation == eRepeaterOperation.Refresh
                        || RepeaterOperation == eRepeaterOperation.Remove)
                        {
                            #region Get Old Data

                            var drShippingCharge = dtShippingCharge.NewRow();

                            drShippingCharge[CS.eCourierType] = lblCourierTypeId.Text;
                            drShippingCharge[CS.eZone] = lblZoneId.Text;
                            drShippingCharge[CS.Weight] = txtWeight.Text;
                            drShippingCharge[CS.ShipCharge] = txtShipCharge.Text;
                            drShippingCharge[CS.FirmShipCharge] = txtFirmShipCharge.Text;

                            dtShippingCharge.Rows.Add(drShippingCharge);

                            #endregion
                        }
                        else if (RepeaterOperation == eRepeaterOperation.Validate)
                        {
                            #region Validate

                            if (txtShipCharge.zToInt() > 0 || txtFirmShipCharge.zToInt() > 0)
                            {
                                if (!txtWeight.zIsDecimal(false) || txtWeight.zToDecimal() <= 0)
                                {
                                    txtWeight.Focus();
                                    CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Weight");
                                    return false;
                                }

                                if (!txtShipCharge.zIsDecimal(false) || txtShipCharge.zToDecimal() <= 0)
                                {
                                    txtShipCharge.Focus();
                                    CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Courier Charge");
                                    return false;
                                }

                                if (!txtFirmShipCharge.zIsDecimal(false) || txtFirmShipCharge.zToDecimal() <= 0)
                                {
                                    txtFirmShipCharge.Focus();
                                    CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Firm Courier Charge");
                                    return false;
                                }
                            }

                            #endregion
                        }
                        else if (RepeaterOperation == eRepeaterOperation.Save)
                        {
                            #region Save Data

                            if (txtShipCharge.zToInt() > 0 || txtFirmShipCharge.zToInt() > 0)
                            {
                                var objShippingCharge = new ShippingCharge()
                                {
                                    ShippingChargeId = dtShippingCharge.Rows.Count > 0 ? dtShippingCharge.Rows[0][CS.ShippingChargeId].zToInt() : (int?)null,
                                    FirmId = lblFirmId.zToInt(),
                                    CourierId = CourierId,
                                    Weight = txtWeight.zToDecimal(),
                                    eCourierType = lblCourierTypeId.zToInt(),
                                    eZone = lblZoneId.zToInt(),
                                    ShipCharge = txtShipCharge.zToInt(),
                                    FirmShipCharge = txtFirmShipCharge.zToInt()
                                };

                                if (objShippingCharge.ShippingChargeId.HasValue)
                                {
                                    dtShippingCharge.Rows.RemoveAt(0);
                                    lstShippingChargeUpdate.Add(objShippingCharge);
                                }
                                else
                                    lstShippingChargeInsert.Add(objShippingCharge);
                            }

                            #endregion
                        }
                    }
                }
            }

            #endregion
        }

        if (RepeaterOperation == eRepeaterOperation.Add || RepeaterOperation == eRepeaterOperation.Refresh
            || RepeaterOperation == eRepeaterOperation.Remove || RepeaterOperation == eRepeaterOperation.Select)
        {
            #region Manage And Bind Data

            if (RepeaterOperation == eRepeaterOperation.Add || dtShippingCharge.Rows.Count == 0)
            {
                var drShippingCharge = dtShippingCharge.NewRow();
                dtShippingCharge.Rows.Add(drShippingCharge);
            }

            dtCourierType = CU.GetEnumDt<eCourierType>(string.Empty);
            dtZone = CU.GetEnumDt<eZone>(string.Empty);
            dtShippingChargeData = dtShippingCharge;


            dtShippingCharge = dtShippingCharge.DefaultView.ToTable(true, CS.Weight);
            rptShippingCharge.DataSource = dtShippingCharge;
            rptShippingCharge.DataBind();

            #endregion
        }
        else if (RepeaterOperation == eRepeaterOperation.Save)
        {
            #region Save Data

            var lstShippingChargeDelete = new List<ShippingCharge>();

            foreach (DataRow dr in dtShippingCharge.Rows)
                lstShippingChargeDelete.Add(new ShippingCharge() { ShippingChargeId = dr[CS.ShippingChargeId].zToInt() });

            lstShippingChargeInsert.Insert();
            lstShippingChargeUpdate.Update();
            lstShippingChargeDelete.Delete();

            #endregion
        }

        return true;
    }

    protected void rptShippingCharge_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var lblPK = e.Item.FindControl("lblPK") as Label;
        var txtWeight = e.Item.FindControl("txtWeight") as TextBox;
        var rptZone = e.Item.FindControl("rptZone") as Repeater;

        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;

        lblPK.Text = (e.Item.ItemIndex + 1).ToString();
        txtWeight.Text = dataItem[CS.Weight].ToString().Replace(".00", "");

        rptZone.DataSource = dtZone;
        rptZone.DataBind();

        if (IsSetFocus)
            txtWeight.Focus();
    }

    protected void rptZone_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var rptCourierType = e.Item.FindControl("rptCourierType") as Repeater;

        rptCourierType.DataSource = dtCourierType;
        rptCourierType.DataBind();
    }

    protected void rptCourierType_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var lblCourierTypeId = e.Item.FindControl("lblCourierTypeId") as Label;
        var lblZoneId = e.Item.FindControl("lblZoneId") as Label;
        var txtShipCharge = e.Item.FindControl("txtShipCharge") as TextBox;
        var txtFirmShipCharge = e.Item.FindControl("txtFirmShipCharge") as TextBox;

        var ItemParent = (RepeaterItem)e.Item.NamingContainer.Parent;

        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;
        DataRowView dataItemZone = (DataRowView)((RepeaterItem)ItemParent).DataItem;

        var ItemParentCourierType = ((RepeaterItem)e.Item.NamingContainer.Parent).NamingContainer.Parent;
        DataRowView dataItemShippingCharge = (DataRowView)((RepeaterItem)ItemParentCourierType).DataItem;


        lblZoneId.Text = dataItemZone[CS.Id].ToString();
        lblCourierTypeId.Text = dataItem[CS.Id].ToString();

        decimal? Weight = dataItemShippingCharge[CS.Weight].zToDecimal();
        if (Weight.HasValue)
        {
            var dr = dtShippingChargeData.Select(CS.Weight + " = '" + Weight + "' AND " + CS.eCourierType + " = '" + lblCourierTypeId.zToInt() + "' AND " + CS.eZone + " = '" + lblZoneId.zToInt() + "'");
            if (dr.Length > 0)
            {
                txtShipCharge.Text = dr[0][CS.ShipCharge].ToString();
                txtFirmShipCharge.Text = dr[0][CS.FirmShipCharge].ToString();
            }
        }
    }


    protected void lnkAddNewShippingCharge_OnClick(object sender, EventArgs e)
    {
        IsSetFocus = true;
        ManageShippingCharge(null, eRepeaterOperation.Add);
    }

    protected void lnkDeleteShippingCharge_OnClick(object sender, EventArgs e)
    {
        var lnk = ((LinkButton)sender);
        var lblPK = lnk.Parent.FindControl("lblPK") as Label;

        ManageShippingCharge(lblPK.zToInt(), eRepeaterOperation.Remove);
    }


    protected void lnkCancelShippingCharge_OnClick(object sender, EventArgs e)
    {
        SetControl(eControl.Courier);
    }

    protected void lnkSaveShippingCharge_OnClick(object sender, EventArgs e)
    {
        if (!ManageShippingCharge(null, eRepeaterOperation.Validate))
            return;

        ManageShippingCharge(null, eRepeaterOperation.Save);
        CU.ZMessage(eMsgType.Success, string.Empty, "Shipping Charge Detail Save Successfully");
    }

    #endregion


    private void SetControl(eControl Control)
    {
        pnlCourier.Visible = Control == eControl.Courier;
        pnlShippingCharge.Visible = Control == eControl.ShippingCharge;
    }

    private enum eControl
    {
        Courier = 1,
        ShippingCharge = 2,
    }
}
