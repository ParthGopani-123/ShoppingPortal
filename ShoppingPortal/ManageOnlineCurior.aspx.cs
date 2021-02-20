using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Web.Script.Serialization;

public partial class ManageOnlineCurior : CompressorPage
{
    string UserName = string.Empty, Password = string.Empty;

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

        UserName = "jatinlathiya0";
        Password = "6fa094198193ea212d51ae835bb01f5f";

        if (!IsPostBack)
        {
            CU.LoadDisplayPerPage(ref ddlRecordPerPage);
            LoadOrderStatus();
            SetControl(eControl.OnlineCourier);
        }

        Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
        Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
        Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

        try { grdOnlineCourier.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }

        try { grdOnlineCourierValue.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    #region Online Courier

    private DataTable GetOnlineCourierDt(ePageIndex ePageIndex)
    {
        int? Status = null;
        if (chkActive.Checked && !chkDeactive.Checked)
            Status = (int)eStatus.Active;
        else if (!chkActive.Checked && chkDeactive.Checked)
            Status = (int)eStatus.Deactive;

        var objQuery = new Query()
        {
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

        return objQuery.Select(eSP.qry_OnlineCourier);
    }

    private void LoadOnlineCourierGrid(ePageIndex ePageIndex)
    {
        DataTable dtOnlineCourier = GetOnlineCourierDt(ePageIndex);

        if (dtOnlineCourier.Rows.Count > 0)
            lblCount.Text = dtOnlineCourier.Rows[0][CS.TotalRecord].ToString();
        else
            lblCount.Text = "0";

        divPaging.Visible = (dtOnlineCourier.Rows.Count > 0);

        txtGotoPageNo.Text = PageIndex.ToString();

        ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

        grdOnlineCourier.DataSource = dtOnlineCourier;
        grdOnlineCourier.DataBind();

        try { grdOnlineCourier.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void CheckVisibleButton()
    {
        lnkActive.Visible = (((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
        lnkDeactive.Visible = (((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
    }


    protected void lnkAdd_OnClick(object sender, EventArgs e)
    {
        lblOnlineCourierId.Text = string.Empty;
        LoadOnlineCourierDetail();
        popupOnlineCourier.Show();
    }

    protected void lnkEdit_OnClick(object sender, EventArgs e)
    {
        if ((sender == null || grdOnlineCourier.zIsValidSelection(lblOnlineCourierId, "chkSelect", CS.OnlineCourierId)))
        {
            LoadOnlineCourierDetail();
            popupOnlineCourier.Show();
        }
    }

    protected void lnkEditOnlineCourier_OnClick(object sender, EventArgs e)
    {
        lblOnlineCourierId.Text = ((LinkButton)sender).CommandArgument.ToString();
        lnkEdit_OnClick(null, null);
    }

    protected void lnkRefresh_OnClick(object sender, EventArgs e)
    {
        LoadOnlineCourierGrid(ePageIndex.Custom);
    }

    protected void lnkActive_OnClick(object sender, EventArgs e)
    {
        if (grdOnlineCourier.zIsValidSelection(lblOnlineCourierId, "chkSelect", CS.OnlineCourierId))
        {
            if (new OnlineCourier()
            {
                OnlineCourierId = lblOnlineCourierId.zToInt(),
                eStatus = (int)eStatus.Active
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Sheet is already Active.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Active, "Active Sheet", "Are You Sure To Active Sheet?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDeactive_OnClick(object sender, EventArgs e)
    {
        if (grdOnlineCourier.zIsValidSelection(lblOnlineCourierId, "chkSelect", CS.OnlineCourierId))
        {
            if (new OnlineCourier()
            {
                OnlineCourierId = lblOnlineCourierId.zToInt(),
                eStatus = (int)eStatus.Deactive
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Sheet is already Deactive.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive Sheet", "Are You Sure To Deactive Sheet?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDelete_OnClick(object sender, EventArgs e)
    {
        if (grdOnlineCourier.zIsValidSelection(lblOnlineCourierId, "chkSelect", CS.OnlineCourierId))
        {
            Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Sheet", "Are You Sure To Delete Sheet?");
            popupConfirmation.Show();
        }
    }

    protected void Control_CheckedChanged(object sender, EventArgs e)
    {
        LoadOnlineCourierGrid(ePageIndex.Custom);
        CheckVisibleButton();
    }


    private void ManageOnlineCourierStatus(eStatus Status)
    {
        new OnlineCourier()
        {
            OnlineCourierId = lblOnlineCourierId.zToInt(),
            eStatus = (int)Status
        }.Update();
    }

    protected void btnActive_OnClick(object sender, EventArgs e)
    {
        ManageOnlineCourierStatus(eStatus.Active);
        CU.ZMessage(eMsgType.Success, string.Empty, "Sheet Activated Successfully.");
        LoadOnlineCourierGrid(ePageIndex.Custom);
    }

    protected void btnDeactive_OnClick(object sender, EventArgs e)
    {
        ManageOnlineCourierStatus(eStatus.Deactive);
        CU.ZMessage(eMsgType.Success, string.Empty, "Sheet Deactive Successfully.");
        LoadOnlineCourierGrid(ePageIndex.Custom);
    }

    protected void btnDelete_OnClick(object sender, EventArgs e)
    {
        ManageOnlineCourierStatus(eStatus.Delete);
        CU.ZMessage(eMsgType.Success, string.Empty, "Sheet Delete Successfully.");
        LoadOnlineCourierGrid(ePageIndex.Custom);
    }


    protected void grdOnlineCourier_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdOnlineCourier, "Select$" + e.Row.RowIndex);

            if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdOnlineCourier, CS.eStatus)].Text) != (int)eStatus.Active)
                e.Row.Attributes["class"] = "GridDesableRow ";

            DataRowView dataItem = (DataRowView)e.Row.DataItem;

            var lnkEditOnlineCourier = e.Row.FindControl("lnkEditOnlineCourier") as LinkButton;
            var lblOnlineCourierDate = e.Row.FindControl("lblOnlineCourierDate") as Label;

            var lblPaymentMade = e.Row.FindControl("lblPaymentMade") as Label;
            var lblPaymentRecived = e.Row.FindControl("lblPaymentRecived") as Label;

            var lblTotalOrder = e.Row.FindControl("lblTotalOrder") as Label;
            var lblTotalQuantity = e.Row.FindControl("lblTotalQuantity") as Label;
            var lblTotalAmount = e.Row.FindControl("lblTotalAmount") as Label;

            var lblTotalDelOrder = e.Row.FindControl("lblTotalDelOrder") as Label;
            var lblTotalDelQuantity = e.Row.FindControl("lblTotalDelQuantity") as Label;
            var lblTotalDelAmount = e.Row.FindControl("lblTotalDelAmount") as Label;

            lnkEditOnlineCourier.Text = dataItem[CS.Name].ToString();
            lnkEditOnlineCourier.CommandArgument = dataItem[CS.OnlineCourierId].ToString();

            lblOnlineCourierDate.Text = Convert.ToDateTime(dataItem[CS.Date]).ToString(CS.ddMMyyyy);

            lblPaymentMade.Text = dataItem[CS.PaymentMade].ToString();
            lblPaymentRecived.Text = dataItem[CS.PaymentRecived].ToString();

            lblTotalOrder.Text = "Order: " + dataItem[CS.TotalOrder].ToString();
            lblTotalQuantity.Text = "Quantity: " + dataItem[CS.TotalQuantity].ToString();
            lblTotalAmount.Text = "Amount: " + dataItem[CS.TotalAmount].ToString();

            lblTotalDelOrder.Text = "Order: " + dataItem["TotalDelOrder"].ToString();
            lblTotalDelQuantity.Text = "Quantity: " + dataItem["TotalDelQuantity"].ToString();
            lblTotalDelAmount.Text = "Amount: " + dataItem["TotalDelAmount"].ToString();


        }
    }

    protected void grdOnlineCourier_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        lblOnlineCourierId.Text = grdOnlineCourier.Rows[grdOnlineCourier.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdOnlineCourier, CS.OnlineCourierId)].Text;
        SetControl(eControl.OnlineCourierValue);
    }


    private void LoadOnlineCourierDetail()
    {
        txtOnlineCourierName.Focus();

        if (IsEditMode())
        {
            lblPopupTitle.Text = "Edit Sheet";
            var objOnlineCourier = new OnlineCourier() { OnlineCourierId = lblOnlineCourierId.zToInt(), }.SelectList<OnlineCourier>()[0];
            txtOnlineCourierName.Text = objOnlineCourier.Name;
            txtDate.Text = objOnlineCourier.Date.Value.ToString(CS.ddMMyyyy);
            txtPaymentMade.Text = objOnlineCourier.PaymentMade.HasValue ? objOnlineCourier.PaymentMade.ToString() : string.Empty;
            txtPaymentRecived.Text = objOnlineCourier.PaymentRecived.HasValue ? objOnlineCourier.PaymentRecived.ToString() : string.Empty;
            txtDescription.Text = objOnlineCourier.Description;
        }
        else
        {
            lblPopupTitle.Text = "New Sheet";
            txtOnlineCourierName.Text = string.Empty;
            txtDate.Text = IndianDateTime.Today.ToString(CS.ddMMyyyy);
            txtPaymentMade.Text = txtPaymentRecived.Text = txtDescription.Text = string.Empty;
        }
    }

    private bool IsEditMode()
    {
        return !lblOnlineCourierId.zIsNullOrEmpty();
    }

    private bool IsValidate()
    {
        if (txtOnlineCourierName.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Name.");
            txtOnlineCourierName.Focus();
            return false;
        }

        var dtOnlineCourier = new Query()
        {
            eStatusNot = (int)eStatus.Delete,
            Name = txtOnlineCourierName.Text.Trim(),
        }.Select(eSP.qry_OnlineCourier);

        if (dtOnlineCourier.Rows.Count > 0 && dtOnlineCourier.Rows[0][CS.OnlineCourierId].ToString() != lblOnlineCourierId.Text)
        {
            string Status = dtOnlineCourier.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
            CU.ZMessage(eMsgType.Error, string.Empty, "This Sheet is already exist" + Status + ".");
            txtOnlineCourierName.Focus();
            return false;
        }

        if (!txtDate.zIsDate())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Date.");
            txtDate.Focus();
            return false;
        }

        if (fuSheet.HasFile)
        {
            var dt = new DataTable();
            if (!CU.IsValidExcelFile(fuSheet, ref dt, null, string.Empty))
            {
                return false;
            }

            if (dt.Rows.Count > 0)
            {
                if (!dt.Columns.Contains("TrakingId") || !dt.Columns.Contains("CarrierId")
                    || !dt.Columns.Contains("Amount") || !dt.Columns.Contains("Quantity"))
                {
                    CU.ZMessage(eMsgType.Error, string.Empty, "Please Check Column TrakingId, CarrierId, Amount, Quantity.");
                    return false;
                }
            }

        }

        return true;
    }

    private bool SaveData()
    {
        if (!IsValidate())
        {
            popupOnlineCourier.Show();
            return false;
        }

        string Message = string.Empty;

        var objOnlineCourier = new OnlineCourier()
        {
            Name = txtOnlineCourierName.Text,
            Date = txtDate.zToDate(),
            PaymentMade = txtPaymentMade.zToInt(),
            PaymentRecived = txtPaymentRecived.zToInt(),
            Description = txtDescription.Text,
        };

        if (IsEditMode())
        {
            objOnlineCourier.OnlineCourierId = lblOnlineCourierId.zToInt();
            objOnlineCourier.Update();

            Message = "Sheet Detail Change Sucessfully.";
        }
        else
        {
            objOnlineCourier.eStatus = (int)eStatus.Active;
            objOnlineCourier.OnlineCourierId = objOnlineCourier.Insert();

            Message = "New Sheet Added Sucessfully.";
        }

        #region Update Value

        if (fuSheet.HasFile)
        {
            var dt = new DataTable();
            if (CU.IsValidExcelFile(fuSheet, ref dt, null, string.Empty))
            {
                var lstInsert = new List<OnlineCourierValue>();
                var lstUpdate = new List<OnlineCourierValue>();

                if (dt.Rows.Count > 0)
                {
                    var dtOnlineCourierValue = new OnlineCourierValue() { OnlineCourierId = objOnlineCourier.OnlineCourierId }.Select();
                    foreach (DataRow dr in dt.Rows)
                    {
                        string TrakingId = dr["TrakingId"].ToString();
                        string CarrierId = dr["CarrierId"].ToString();
                        string Amount = dr["Amount"].ToString();
                        string Quantity = dr["Quantity"].ToString();

                        if (TrakingId.zIsNullOrEmpty())
                            continue;
                        if (CarrierId.zIsNullOrEmpty())
                            continue;
                        if (Amount.zIsNullOrEmpty())
                            continue;
                        if (Quantity.zIsNullOrEmpty())
                            continue;

                        var drOnlineCourierValue = dtOnlineCourierValue.Select(CS.TrakingId + " = '" + TrakingId + "'");

                        var objOnlineCourierValue = new OnlineCourierValue()
                        {
                            OnlineCourierValueId = drOnlineCourierValue.Length > 0 ? drOnlineCourierValue[0][CS.OnlineCourierValueId].zToInt() : null,
                            OnlineCourierId = objOnlineCourier.OnlineCourierId,
                            TrakingId = TrakingId,
                            CarrierId = CarrierId.zToInt(),
                            Amount = Amount.zToDecimal(),
                            Quantity = Quantity.zToInt(),
                        };

                        if (objOnlineCourierValue.OnlineCourierValueId.HasValue && objOnlineCourierValue.OnlineCourierValueId > 0)
                        {
                            dtOnlineCourierValue.Rows.Remove(drOnlineCourierValue[0]);

                            lstUpdate.Add(objOnlineCourierValue);
                            if (lstUpdate.Count > 200)
                            {
                                lstUpdate.Update();
                                lstUpdate = new List<OnlineCourierValue>();
                            }
                        }
                        else
                        {
                            objOnlineCourierValue.OrderStatus = (int)eOrderStatus.No;
                            lstInsert.Add(objOnlineCourierValue);
                            if (lstInsert.Count > 200)
                            {
                                lstInsert.Insert();
                                lstInsert = new List<OnlineCourierValue>();
                            }
                        }

                        #region Upload Shipway

                        var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://shipway.in/api/pushOrderData");
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";

                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            string json = "{\r\n\"username\":\"" + UserName + "\",\r\n\"password\":\"" + Password + "\",\r\n\"carrier_id\":\"" + CarrierId + "\",\r\n\"awb\":\" " + TrakingId + "\",\r\n\"order_id\":\"" + TrakingId + "\",\r\n\"first_name\":\"N/A\",\r\n\"last_name\":\" N/A \",\r\n\"email\":\" N/A \",\r\n\"phone\":\" N/A \",\r\n\"products\":\" N/A \",\r\n\"company\":\" xxxxx \"\r\n }";
                            streamWriter.Write(json);
                        }

                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                        }

                        #endregion
                    }

                    //foreach (DataRow drOnlineCourierValue in dtOnlineCourierValue.Rows)
                    //{
                    //    new OnlineCourierValue() { OnlineCourierValueId = drOnlineCourierValue[CS.OnlineCourierValueId].zToInt() }.Delete();
                    //}

                }

                lstInsert.Insert();
                lstUpdate.Update();
            }
        }

        #endregion

        CU.ZMessage(eMsgType.Success, string.Empty, Message);

        return true;
    }

    protected void btnSave_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadOnlineCourierGrid(ePageIndex.Custom);
        }
    }

    protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadOnlineCourierGrid(ePageIndex.Custom);
            lnkAdd_OnClick(null, null);
        }
    }


    #region Pagging

    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        LoadOnlineCourierGrid(ePageIndex.Prev);
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        LoadOnlineCourierGrid(ePageIndex.Next);
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        LoadOnlineCourierGrid(ePageIndex.First);
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        LoadOnlineCourierGrid(ePageIndex.Last);
    }

    protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
    {
        if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
        {
            txtGotoPageNo.Text = "1";
            txtGotoPageNo.Focus();
        }
        LoadOnlineCourierGrid(ePageIndex.Custom);
    }

    protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
    {
        txtGotoPageNo.Text = "1";
        LoadOnlineCourierGrid(ePageIndex.Custom);
    }


    #endregion

    #endregion

    #region Online Courier Value

    private void LoadOrderStatus()
    {
        CU.FillEnumddl<eOrderStatus>(ref ddlOrderStatus, "-- Select Status --");
    }

    private void LoadOnlineCourierValueGrid()
    {
        DataTable dtOnlineCourierValue = new Query()
        {
            OnlineCourierId = lblOnlineCourierId.zToInt(),
            OrderStatus = ddlOrderStatus.zIsSelect() ? ddlOrderStatus.zToInt() : (int?)null,
            MasterSearch = txtSearchValue.Text,
        }.Select(eSP.qry_OnlineCourierValue);

        lblCountValue.Text = dtOnlineCourierValue.Rows.Count.ToString();

        grdOnlineCourierValue.DataSource = dtOnlineCourierValue;
        grdOnlineCourierValue.DataBind();

        try { grdOnlineCourierValue.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    protected void lnkRefreshValue_OnClick(object sender, EventArgs e)
    {
        var lstUpdate = new List<OnlineCourierValue>();

        foreach (GridViewRow row in grdOnlineCourierValue.Rows)
        {
            var lblOnlineCourierValueId = row.FindControl("lblOnlineCourierValueId") as Label;
            var lblTrakingId = row.FindControl("lblTrakingId") as Label;
            var lblTrakingInfo = row.FindControl("lblTrakingInfo") as Label;
            var lblLocation = row.FindControl("lblLocation") as Label;

            try
            {
                string TrakingId = lblTrakingId.Text;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://shipway.in/api/getOrderShipmentDetails");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\r\n\"username\":\"" + UserName + "\",\r\n\"password\":\"" + Password + "\",\r\n\"order_id\":\"" + TrakingId + "\" }";
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var objTraking = new JavaScriptSerializer().Deserialize<Traking>(result);
                    lblTrakingInfo.Text = objTraking.response.current_status;
                    lblLocation.Text = (objTraking.response.scan != null && objTraking.response.scan.Count > 0) ? objTraking.response.scan[0].location : string.Empty;

                    lstUpdate.Add(new OnlineCourierValue() { OnlineCourierValueId = lblOnlineCourierValueId.zToInt(), TrakingInfo = lblTrakingInfo.Text, Location = lblLocation.Text });
                    if (lstUpdate.Count > 500)
                    {
                        lstUpdate.Update();
                        lstUpdate = new List<OnlineCourierValue>();
                    }
                }
            }
            catch { }
        }

        lstUpdate.Update();

        CU.ZMessage(eMsgType.Success, string.Empty, "Traking Successfully Updated");
    }

    protected void ControlValue_CheckedChanged(object sender, EventArgs e)
    {
        LoadOnlineCourierValueGrid();
    }


    //private void SetButton(int OrderStatus, ref LinkButton lnkDelivered, ref LinkButton lnkRefused, ref LinkButton lnkNoData)
    //{
    //    lnkDelivered.Attributes.Add("class", "btn btn-raised btn-default clickloader");
    //    lnkRefused.Attributes.Add("class", "btn btn-raised btn-default clickloader");
    //    lnkNoData.Attributes.Add("class", "btn btn-raised btn-default clickloader");

    //    if (OrderStatus == (int)eOrderStatus.Delivered)
    //        lnkDelivered.Attributes.Add("class", "btn btn-raised btn-default clickloader lnkDelivered");
    //    else if (OrderStatus == (int)eOrderStatus.Refused)
    //        lnkRefused.Attributes.Add("class", "btn btn-raised btn-default clickloader lnkRefused");
    //    else
    //        lnkNoData.Attributes.Add("class", "btn btn-raised btn-default clickloader lnkNoData");
    //}

    //protected void lnkDelivered_OnClick(object sender, EventArgs e)
    //{
    //    var lnk = (LinkButton)sender;

    //    var lblOnlineCourierValueId = lnk.Parent.FindControl("lblOnlineCourierValueId") as Label;
    //    new OnlineCourierValue() { OnlineCourierValueId = lblOnlineCourierValueId.zToInt(), OrderStatus = (int)eOrderStatus.Delivered }.Update();

    //    var lnkRefused = lnk.Parent.FindControl("lnkRefused") as LinkButton;
    //    var lnkNoData = lnk.Parent.FindControl("lnkNoData") as LinkButton;
    //    SetButton((int)eOrderStatus.Delivered, ref lnk, ref lnkRefused, ref lnkNoData);
    //}

    //protected void lnkRefused_OnClick(object sender, EventArgs e)
    //{
    //    var lnk = (LinkButton)sender;

    //    var lblOnlineCourierValueId = lnk.Parent.FindControl("lblOnlineCourierValueId") as Label;
    //    new OnlineCourierValue() { OnlineCourierValueId = lblOnlineCourierValueId.zToInt(), OrderStatus = (int)eOrderStatus.Refused }.Update();

    //    var lnkDelivered = lnk.Parent.FindControl("lnkDelivered") as LinkButton;
    //    var lnkNoData = lnk.Parent.FindControl("lnkNoData") as LinkButton;
    //    SetButton((int)eOrderStatus.Refused, ref lnkDelivered, ref lnk, ref lnkNoData);
    //}

    //protected void lnkNoData_OnClick(object sender, EventArgs e)
    //{
    //    var lnk = (LinkButton)sender;

    //    var lblOnlineCourierValueId = lnk.Parent.FindControl("lblOnlineCourierValueId") as Label;
    //    new OnlineCourierValue() { OnlineCourierValueId = lblOnlineCourierValueId.zToInt(), OrderStatus = (int)eOrderStatus.No }.Update();

    //    var lnkDelivered = lnk.Parent.FindControl("lnkDelivered") as LinkButton;
    //    var lnkRefused = lnk.Parent.FindControl("lnkRefused") as LinkButton;
    //    SetButton((int)eOrderStatus.No, ref lnkDelivered, ref lnkRefused, ref lnk);
    //}


    protected void grdOnlineCourierValue_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dataItem = (DataRowView)e.Row.DataItem;

            var lblTrakingId = e.Row.FindControl("lblTrakingId") as Label;
            var lblOnlineCourierValueId = e.Row.FindControl("lblOnlineCourierValueId") as Label;

            var lblAmount = e.Row.FindControl("lblAmount") as Label;
            var lblQuantity = e.Row.FindControl("lblQuantity") as Label;

            var lblTrakingInfo = e.Row.FindControl("lblTrakingInfo") as Label;
            var lblLocation = e.Row.FindControl("lblLocation") as Label;

            var txtOrderStatus = e.Row.FindControl("txtOrderStatus") as TextBox;
            var lnkDelivered = e.Row.FindControl("lnkDelivered") as LinkButton;
            var lnkRefused = e.Row.FindControl("lnkRefused") as LinkButton;
            var lnkNoData = e.Row.FindControl("lnkNoData") as LinkButton;

            lblAmount.Text = dataItem[CS.Amount].ToString();
            lblQuantity.Text = dataItem[CS.Quantity].ToString();

            lblTrakingId.Text = dataItem[CS.TrakingId].ToString();
            lblOnlineCourierValueId.Text = dataItem[CS.OnlineCourierValueId].ToString();

            lblTrakingInfo.Text = dataItem[CS.TrakingInfo].ToString();
            lblLocation.Text = dataItem[CS.Location].ToString();

            txtOrderStatus.Text = dataItem[CS.OrderStatus].ToString();
            //SetButton(dataItem[CS.OrderStatus].zToInt().Value, ref lnkDelivered, ref lnkRefused, ref lnkNoData);

        }
    }

    protected void grdOnlineCourierValue_OnSelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void lnkExcelExport_OnClick(object sender, EventArgs e)
    {
        DataTable dtOnlineCourierValue = new OnlineCourierValue()
        {
            OnlineCourierId = lblOnlineCourierId.zToInt(),
        }.Select();

        dtOnlineCourierValue.Columns.Add("OrderStatusView");


        foreach (DataRow drOnlineCourierValue in dtOnlineCourierValue.Rows)
        {
            drOnlineCourierValue["OrderStatusView"] = ((eOrderStatus)drOnlineCourierValue["OrderStatus"].zToInt()).ToString();
        }

        var lstColumns = new List<string>();
        lstColumns.Add("TrakingId");
        lstColumns.Add("CarrierId");
        lstColumns.Add("OrderStatusView");
        lstColumns.Add("Amount");
        lstColumns.Add("Quantity");
        lstColumns.Add("TrakingInfo");
        lstColumns.Add("Location");

        var lstColumnsSelected = new List<string>();
        lstColumnsSelected.Add("TrakingId");
        lstColumnsSelected.Add("CarrierId");
        lstColumnsSelected.Add("OrderStatusView");
        lstColumnsSelected.Add("Amount");
        lstColumnsSelected.Add("Quantity");
        lstColumnsSelected.Add("TrakingInfo");
        lstColumnsSelected.Add("Location");

        ExcelExport.SetExportData(dtOnlineCourierValue, lstColumns, lstColumnsSelected, "Traking");
        popupExcelExport.Show();
    }


    protected void lnkSaveTraking_OnClick(object sender, EventArgs e)
    {
        var lstUpdate = new List<OnlineCourierValue>();
        foreach (GridViewRow row in grdOnlineCourierValue.Rows)
        {
            var lblOnlineCourierValueId = row.FindControl("lblOnlineCourierValueId") as Label;
            var txtOrderStatus = row.FindControl("txtOrderStatus") as TextBox;

            lstUpdate.Add(new OnlineCourierValue() { OnlineCourierValueId = lblOnlineCourierValueId.zToInt(), OrderStatus = txtOrderStatus.zToInt() });
            if (lstUpdate.Count > 500)
            {
                lstUpdate.Update();
                lstUpdate = new List<OnlineCourierValue>();
            }
        }

        lstUpdate.Update();

        CU.ZMessage(eMsgType.Success, string.Empty, "Data Save Successfully.");
    }

    #endregion

    protected void lnkOnlineCourier_OnClick(object sender, EventArgs e)
    {
        SetControl(eControl.OnlineCourier);
    }

    private void SetControl(eControl Control)
    {
        pnlOnlineCourier.Visible = (Control == eControl.OnlineCourier);
        pnlOnlineCourierValue.Visible = (Control == eControl.OnlineCourierValue);

        if (Control == eControl.OnlineCourier)
        {
            LoadOnlineCourierGrid(ePageIndex.Custom);
            CheckVisibleButton();
        }
        else if (Control == eControl.OnlineCourierValue)
        {
            LoadOnlineCourierValueGrid();
        }
    }

    public enum eControl
    {
        OnlineCourier = 1,
        OnlineCourierValue = 2,
    }

    public enum eOrderStatus
    {
        No = 1,
        Refused = 2,
        Delivered = 3,
    }
}
