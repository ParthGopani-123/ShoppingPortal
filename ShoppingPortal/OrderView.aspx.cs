using System;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.Services;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI.HtmlControls;

public partial class OrderView : CompressorPage
{
    bool? IsAddEdit;

    DataTable dtOrderStatus, dtOrderProduct, dtUser, dtFirm, dtActionToBeTaken, dtActionTaken, dtCustomerReply, dtOrderComplain;

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
            lbleOrganization.Text = CU.GeteOrganisationId(OrganizationId).zToInt().ToString();

            lblUsersId.Text = CU.GetUsersId().ToString();

            CU.LoadDisplayPerPage(ref ddlRecordPerPage);

            DateTime DateToday = IndianDateTime.Today;

            try { txtFromDate.Text = CU.GetSearchCookie(txtFromDate.ID); }
            catch { }

            if (txtFromDate.zIsNullOrEmpty())
                txtFromDate.Text = DateToday.AddDays(-5).ToString(CS.ddMMyyyy);
            txtToDate.Text = DateToday.ToString(CS.ddMMyyyy);

            LoadFirm();
            try { ddlFirm.SelectedValue = CU.GetSearchCookie(ddlFirm.ID); }
            catch { }

            LoadUser();
            try { ddlUser.SelectedValue = CU.GetSearchCookie(ddlUser.ID); }
            catch { }

            //LoadCustomer();
            LoadCourier();
            LoadOrderSource();
            LoadStatusType();
            try
            {
                string OrderStatusType = CU.GetSearchCookie(lstStatusTypeIn.ID).Replace(" ", "") + ",";
                foreach (ListItem Item in lstStatusTypeIn.Items)
                    Item.Selected = (OrderStatusType.Contains(Item.Value + ","));
            }
            catch { }

            try
            {
                string OrderStatusTypeNot = CU.GetSearchCookie(lstStatusTypeNotIn.ID).Replace(" ", "") + ",";
                foreach (ListItem Item in lstStatusTypeNotIn.Items)
                    Item.Selected = (OrderStatusTypeNot.Contains(Item.Value + ","));
            }
            catch { }

            LoadOrderGrid(ePageIndex.Custom);
            CheckVisibleButton();
        }

        popupManageOrderPayment.btnSave_OnClick += new EventHandler(btnSaveOrderPayment_OnClick);


        try { grdOrder.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }


    private void LoadFirm()
    {
        eDesignation Designation = CU.GeteDesignationId(CU.GetUsersId());
        divFirm.Visible = Designation == eDesignation.SystemAdmin || Designation == eDesignation.Admin;

        var dtFirm = new Query() { OrganizationId = lblOrganizationId.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_Firm);
        CU.FillDropdown(ref ddlFirm, dtFirm, "-- All Firm --", CS.FirmId, CS.FirmName);

        try { ddlFirm.SelectedValue = lblFirmId.Text; }
        catch { }
    }

    private void LoadUser()
    {
        int? UsersId = lblUsersId.zToInt(), ParentUsersId = null;

        var Designation = CU.GeteDesignationId(UsersId.Value);
        if (Designation == eDesignation.Admin || Designation == eDesignation.SystemAdmin)
        {
            UsersId = ParentUsersId = null;
        }
        else
        {
            if (CU.GetAuthority(UsersId.Value, eAuthority.OrderChildView).IsView)
            {
                ParentUsersId = UsersId;
                UsersId = null;
            }
            else
            {
                ParentUsersId = null;
            }
        }

        var dtUser = new Query()
        {
            OrganizationId = lblOrganizationId.zToInt(),
            FirmId = ddlFirm.zIsSelect() ? ddlFirm.zToInt() : null,
            ParentUsersId = ParentUsersId,
            UsersId = UsersId,
            eStatus = (int)eStatus.Active,
        }.Select(eSP.qry_User);

        CU.FillDropdown(ref ddlUser, dtUser, "-- All User --", CS.UsersId, CS.Name);

        try { ddlUser.SelectedValue = lblUsersId.Text; }
        catch { }
    }

    //private void LoadCustomer()
    //{
    //    int? CustomerId = ddlCustomer.zToInt();

    //    var dtCustomer = new Query()
    //    {
    //        OrganizationId = lblOrganizationId.zToInt(),
    //        FirmId = ddlFirm.zIsSelect() ? ddlFirm.zToInt() : null,
    //        eStatus = (int)eStatus.Active,
    //    }.Select(eSP.qry_Customer);

    //    CU.FillDropdown(ref ddlCustomer, dtCustomer, "-- All Customer --", CS.CustomerId, CS.Name);

    //    try { ddlCustomer.SelectedValue = CustomerId.ToString(); }
    //    catch { }
    //}

    private void LoadCourier()
    {
        var dtCourier = new Query()
        {
            OrganizationId = lblOrganizationId.zToInt(),
            eStatus = (int)eStatus.Active,
        }.Select(eSP.qry_Courier);

        CU.FillDropdown(ref ddlCourier, dtCourier, "-- All Courier --", CS.CourierId, CS.CourierName);

    }

    private void LoadOrderSource()
    {
        var dtOrderSource = new Query()
        {
            OrganizationId = lblOrganizationId.zToInt(),
            FirmId = ddlFirm.zIsSelect() ? ddlFirm.zToInt() : null,
            eStatus = (int)eStatus.Active,
        }.Select(eSP.qry_OrderSource);

        CU.FillDropdown(ref ddlOrderSource, dtOrderSource, "-- All Order Source --", CS.OrderSourceId, CS.OrderSourceName);
    }

    private void LoadStatusType()
    {
        CU.FillEnumlbx<eStatusType>(ref lstStatusTypeIn, string.Empty);
        CU.FillEnumlbx<eStatusType>(ref lstStatusTypeNotIn, string.Empty);
    }


    private DataTable GetOrderDt(ePageIndex ePageIndex, List<int> lstOrdersId)
    {
        var lstUsersIdIn = new List<int>();
        if (ddlUser.zIsSelect())
        {
            lstUsersIdIn.Add(ddlUser.zToInt().Value);
        }
        else
        {
            int UsersId = CU.GetUsersId();
            var Designation = CU.GeteDesignationId(UsersId);
            if (Designation == eDesignation.Admin || Designation == eDesignation.SystemAdmin)
            {

            }
            else
            {
                foreach (ListItem Item in ddlUser.Items)
                {
                    lstUsersIdIn.Add(Item.Value.zToInt().Value);
                }
            }
        }

        int? CourierType = null;
        if (chkCOD.Checked && !chkPrepaid.Checked)
            CourierType = (int)eCourierType.COD;
        else if (!chkCOD.Checked && chkPrepaid.Checked)
            CourierType = (int)eCourierType.Prepaid;

        var objQuery = new Query()
        {
            OrganizationId = lblOrganizationId.zToInt(),

            FirmId = ddlFirm.zToInt() > 0 ? ddlFirm.zToInt() : (int?)null,

            eStatusTypeIn = CU.GetParaIn(lstStatusTypeIn, true, false),
            eStatusTypeNotIn = CU.GetParaIn(lstStatusTypeNotIn, true, false),

            UsersIdIn = CU.GetParaIn(lstUsersIdIn, false),

            FromDate = txtFromDate.zToDate().HasValue ? txtFromDate.zToDate() : (DateTime?)null,
            ToDate = txtToDate.zToDate().HasValue ? txtToDate.zToDate().Value.AddDays(1).AddSeconds(-1) : (DateTime?)null,

            OrdersIdIn = CU.GetParaIn(lstOrdersId, false),

            //CustomerId = ddlCustomer.zIsSelect() ? ddlCustomer.zToInt() : null,
            eCourierType = CourierType,
            OrderSourceId = ddlOrderSource.zIsSelect() ? ddlOrderSource.zToInt() : null,
            CourierId = ddlCourier.zIsSelect() ? ddlCourier.zToInt() : null,
            MasterSearch = txtSearch.Text,
        };

        #region Page Index

        int RecordPerPage = ddlRecordPerPage.zToInt().Value;
        int PageIndexTemp = PageIndex;

        CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref objQuery, ref txtGotoPageNo, ref lblCount);
        PageIndex = PageIndexTemp;

        #endregion Page Index

        return objQuery.Select(eSP.qry_Orders);
    }


    private void LoadOrderGrid(ePageIndex ePageIndex)
    {
        DataTable dtOrder = GetOrderDt(ePageIndex, new List<int>());

        if (dtOrder.Rows.Count > 0)
            lblCount.Text = dtOrder.Rows[0][CS.TotalRecord].ToString();
        else
            lblCount.Text = "0";

        divPaging.Visible = (dtOrder.Rows.Count > 0);

        txtGotoPageNo.Text = PageIndex.ToString();

        ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

        dtOrderStatus = new Query() { eStatus = (int)eStatus.Active, OrganizationId = lblOrganizationId.zToInt() }.Select(eSP.qry_OrderStatus);

        grdOrder.DataSource = dtOrder;
        grdOrder.DataBind();

        try { grdOrder.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void CheckVisibleButton()
    {
        int UsersId = CU.GetUsersId();
        var objAuthority = CU.GetAuthority(UsersId, eAuthority.ManageOrder);
        lnkAdd.Visible = lnkEdit.Visible = objAuthority.IsAddEdit;

        lnkSetOrderStatus.Visible = lnkTrackCurior.Visible = lnkExcelChangeStatus.Visible = CU.GetAuthority(UsersId, eAuthority.OrderTracking).IsAddEdit;
        lnkPrintOrderSlip.Visible = CU.GetAuthority(UsersId, eAuthority.OrderSlipPrint).IsAddEdit;
        lnkPrintOrderProduct.Visible = CU.GetAuthority(UsersId, eAuthority.OrderProductPrint).IsAddEdit;
        lnkOrderComplain.Visible = CU.GetAuthority(UsersId, eAuthority.OrderComplain).IsAddEdit;
        lnkOrderPayment.Visible = lnkExcelPayment.Visible = CU.GetAuthority(UsersId, eAuthority.ManageOrderPayment).IsAddEdit;
    }


    protected void lnkEdit_OnClick(object sender, EventArgs e)
    {
        if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageOrder).IsAddEdit && (sender == null || grdOrder.zIsValidSelection(lblOrdersId, "chkSelect", CS.OrdersId)))
        {
            Response.Redirect("ManageOrder.aspx?" + CS.OrdersId.Encrypt() + "=" + lblOrdersId.Text.Encrypt());
        }
    }


    protected void lnkRefresh_OnClick(object sender, EventArgs e)
    {
        CU.SetSearchCookie(ddlFirm.ID, ddlFirm.SelectedValue);
        CU.SetSearchCookie(ddlUser.ID, ddlUser.SelectedValue);

        CU.SetSearchCookie(lstStatusTypeIn.ID, CU.GetParaIn(lstStatusTypeIn, true, false));
        CU.SetSearchCookie(lstStatusTypeNotIn.ID, CU.GetParaIn(lstStatusTypeNotIn, true, false));

        CU.SetSearchCookie(txtFromDate.ID, txtFromDate.Text);

        LoadOrderGrid(ePageIndex.Custom);
    }

    protected void ddlFirm_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        LoadUser();
        //LoadCustomer();
    }

    protected void lnkSetOrderStatus_OnClick(object sender, EventArgs e)
    {
        bool IsEditMode = false;
        if (lblOrderStatusMode.zToInt() == (int)eYesNo.Yes)
        {
            IsEditMode = false;
            lblOrderStatusMode.Text = ((int)eYesNo.No).ToString();
        }
        else
        {
            IsEditMode = true;
            lblOrderStatusMode.Text = ((int)eYesNo.Yes).ToString();
        }

        dtOrderStatus = new Query() { eStatus = (int)eStatus.Active, OrganizationId = lblOrganizationId.zToInt() }.Select(eSP.qry_OrderStatus);

        foreach (GridViewRow gvrow in grdOrder.Rows)
        {
            var ddlOrderStatus = gvrow.FindControl("ddlOrderStatus") as DropDownList;
            var lblOrderStatus = gvrow.FindControl("lblOrderStatus") as Label;
            var txtAWBNo = gvrow.FindControl("txtAWBNo") as TextBox;
            var lblAWBNo = gvrow.FindControl("lblAWBNo") as Label;

            var dr = dtOrderStatus.Select(CS.OrderStatusId + " = " + ddlOrderStatus.zToInt());

            lblOrderStatus.Text = ddlOrderStatus.SelectedItem.Text;
            lblOrderStatus.Attributes.Add("class", "lblGrid lblOrderStatus sts" + ((eStatusType)dr[0][CS.eStatusType].zToInt()).ToString());

            lblAWBNo.Text = txtAWBNo.Text;

            ddlOrderStatus.Visible = txtAWBNo.Visible = IsEditMode;
            lblOrderStatus.Visible = lblAWBNo.Visible = !IsEditMode;
        }
    }

    protected void lnkOrderPayment_OnClick(object sender, EventArgs e)
    {
        if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageOrderPayment).IsAddEdit && grdOrder.zIsValidSelection(lblOrdersId, "chkSelect", CS.OrdersId))
        {
            popupManageOrderPayment.SetOrderPaymentId = string.Empty;
            popupManageOrderPayment.LoadOrderPaymentDetail(lblOrdersId.zToInt().Value);
            popupOrderPayment.Show();
        }
    }

    protected void btnSaveOrderPayment_OnClick(object sender, EventArgs e)
    {
        LoadOrderGrid(ePageIndex.Custom);
        popupOrderPayment.Hide();
    }


    protected void grdOrder_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!IsAddEdit.HasValue)
                IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageOrder).IsAddEdit;

            DataRowView dataItem = (DataRowView)e.Row.DataItem;

            var lblRowOrderId = e.Row.FindControl("lblRowOrderId") as Label;
            var aEditOrder = e.Row.FindControl("aEditOrder") as HtmlAnchor;
            var ltrOrder = e.Row.FindControl("ltrOrder") as Literal;
            var lblTotalAmount = e.Row.FindControl("lblTotalAmount") as Label;
            var lblUserName = e.Row.FindControl("lblUserName") as Label;

            var ddlOrderStatus = e.Row.FindControl("ddlOrderStatus") as DropDownList;
            var lblOrderStatus = e.Row.FindControl("lblOrderStatus") as Label;
            var lblCourierType = e.Row.FindControl("lblCourierType") as Label;
            var txtAWBNo = e.Row.FindControl("txtAWBNo") as TextBox;
            var lblCarrierCode = e.Row.FindControl("lblCarrierCode") as Label;
            var lblAWBNo = e.Row.FindControl("lblAWBNo") as Label;

            #region OrderStatus

            ddlOrderStatus.DataSource = dtOrderStatus;
            ddlOrderStatus.DataValueField = CS.OrderStatusId;
            ddlOrderStatus.DataTextField = CS.StatusName;
            ddlOrderStatus.DataBind();

            ddlOrderStatus.SelectedValue = dataItem[CS.OrderStatusId].ToString();

            ddlOrderStatus.Visible = txtAWBNo.Visible = false;

            #endregion

            var lblCustomerName = e.Row.FindControl("lblCustomerName") as Label;
            var lblCustomerCity = e.Row.FindControl("lblCustomerCity") as Label;
            var lblCustomerMobile = e.Row.FindControl("lblCustomerMobile") as Label;

            var lblProductList = e.Row.FindControl("lblProductList") as Label;
            var lblTotalPaidAmount = e.Row.FindControl("lblTotalPaidAmount") as Label;
            var lblPaymentStatus = e.Row.FindControl("lblPaymentStatus") as Label;

            var lblgrdOrdersId = e.Row.FindControl("lblgrdOrdersId") as Label;
            var lblTraking = e.Row.FindControl("lblTraking") as Label;
            var aTrakingInfo = e.Row.FindControl("aTrakingInfo") as HtmlAnchor;
            var lblLastTrakingInfo = e.Row.FindControl("lblLastTrakingInfo") as Label;


            aEditOrder.Visible = IsAddEdit.Value;
            ltrOrder.Visible = !IsAddEdit.Value;

            lblRowOrderId.Text = dataItem[CS.OrdersId].ToString();

            aEditOrder.HRef = "ManageOrder.aspx?" + CS.OrdersId.Encrypt() + "=" + dataItem[CS.OrdersId].ToString().Encrypt();

            aEditOrder.InnerHtml = ltrOrder.Text = dataItem[CS.OrdersId].ToString() + " - " + Convert.ToDateTime(dataItem[CS.Date]).ToString(CS.ddMMM);
            lblTotalAmount.Text = "Rs." + dataItem[CS.TotalAmount].ToString().Replace(".00", "") + " (" + dataItem[CS.UserCommition].ToString().Replace(".00", "") + ")";
            lblUserName.Text = "by " + dataItem[CS.UserName].ToString();

            lblOrderStatus.Text = dataItem[CS.StatusName].ToString();
            lblOrderStatus.Attributes.Add("class", "lblGrid lblOrderStatus sts" + ((eStatusType)dataItem[CS.eStatusType].zToInt()).ToString());
            lblCourierType.Text = CU.GetDescription((eCourierType)dataItem[CS.eCourierType].zToInt()) + " - " + dataItem[CS.CourierName].ToString();

            if (dataItem[CS.eStatusType].zToInt() == (int)eStatusType.RPickup || dataItem[CS.eStatusType].zToInt() == (int)eStatusType.RPickupDelivered)
            {
                txtAWBNo.Text = lblAWBNo.Text = dataItem[CS.ReturnAWBNo].ToString();
            }
            else
            {
                txtAWBNo.Text = lblAWBNo.Text = dataItem[CS.AWBNo].ToString();
            }

            lblCustomerName.Text = dataItem[CS.Name].ToString();
            lblCustomerCity.Text = dataItem[CS.CityName].ToString();
            lblCustomerMobile.Text = dataItem[CS.WhatsAppNo].ToString() + ", " + dataItem[CS.MobileNo].ToString();

            lblProductList.Text = dataItem[CS.ProductList].ToString();
            lblTotalPaidAmount.Text = dataItem[CS.TotalPaidAmount].zToDecimal() > 0 ? ("Paid Amount: " + dataItem[CS.TotalPaidAmount].ToString()) : "";
            if (dataItem[CS.PaymentStatus].zToInt() == (int)ePaymentStatus.Paid)
            {
                lblPaymentStatus.Text = "Paid";
                lblPaymentStatus.addClass("stsPayPaid");
            }
            else if (dataItem[CS.PaymentStatus].zToInt() == (int)ePaymentStatus.PartiallyPaid)
            {
                lblPaymentStatus.Text = "Partially Paid";
                lblPaymentStatus.addClass("stsPayParPaid");
            }
            else if (dataItem[CS.PaymentStatus].zToInt() == (int)ePaymentStatus.NotPaid)
            {
                lblPaymentStatus.Text = "Not Paid";
                lblPaymentStatus.addClass("stsPayNotPaid");
            }

            lblgrdOrdersId.Text = dataItem[CS.OrdersId].ToString();
            lblCarrierCode.Text = dataItem[CS.CarrierCode].ToString();

            lblTraking.Text = dataItem[CS.TrakingInfo].ToString();

            var objTraking = new JavaScriptSerializer().Deserialize<Traking>(lblTraking.Text);

            if (objTraking != null)
            {
                aTrakingInfo.HRef = objTraking.response.tracking_url;
                aTrakingInfo.InnerHtml = objTraking.response.current_status;
                if (objTraking.response.scan != null && objTraking.response.scan.Count > 0)
                    lblLastTrakingInfo.Text = "Last Location: " + objTraking.response.scan[0].location + " - " + objTraking.response.scan[0].time;
            }
        }
    }

    [WebMethod]
    public static string SetOrderStatus(int OrdersId, int OrderStatusId)
    {
        string AWBNo = string.Empty;
        if (OrdersId > 0 && OrderStatusId > 0)
        {
            CU.UpdateOrderStatus(new Orders()
            {
                OrdersId = OrdersId,
                OrderStatusId = OrderStatusId,
            });

            var dtOrders = new Query() { OrdersId = OrdersId }.Select(eSP.qry_Orders);
            if (dtOrders.Rows.Count > 0)
            {
                int StatusType = dtOrders.Rows[0][CS.eStatusType].zToInt().Value;
                if (StatusType == (int)eStatusType.RPickup || StatusType == (int)eStatusType.RPickupDelivered)
                    AWBNo = dtOrders.Rows[0][CS.ReturnAWBNo].ToString();
                else
                    AWBNo = dtOrders.Rows[0][CS.AWBNo].ToString();

                CU.SetOrderTransaction(OrdersId);
            }
        }

        return AWBNo;
    }



    [WebMethod]
    public static string SetAWBNo(int OrdersId, string AWBNo)
    {
        if (OrdersId > 0)
        {
            var drOrders = new Query() { OrdersId = OrdersId }.Select(eSP.qry_Orders).Rows[0];
            int StatusType = drOrders[CS.eStatusType].zToInt().Value;

            if (StatusType == (int)eStatusType.RPickup || StatusType == (int)eStatusType.RPickupDelivered)
            {
                new Orders()
                {
                    OrdersId = OrdersId,
                    ReturnAWBNo = AWBNo,
                }.Update();
            }
            else
            {
                new Orders()
                {
                    OrdersId = OrdersId,
                    AWBNo = AWBNo,
                }.Update();
            }
        }

        return "";
    }



    #region Print Order Slip

    protected void lnkPrintOrderSlip_OnClick(object sender, EventArgs e)
    {
        if (sender != null)
            txtNoofPrint.Text = "2";

        var lstOrdersId = grdOrder.zIsValidSelection("chkSelect", CS.OrdersId);

        var dtOrder = GetOrderDt(ePageIndex.AllPage, lstOrdersId);
        if (dtOrder.Rows.Count > 0)
        {
            lstOrdersId = new List<int>();
            var lstUsersId = new List<int>();
            var lstFirmId = new List<int>();

            foreach (DataRow dr in dtOrder.Rows)
            {
                lstOrdersId.Add(dr[CS.OrdersId].zToInt().Value);
                if (dr[CS.eDesignation].zToInt() == (int)eDesignation.Admin || dr[CS.eDesignation].zToInt() == (int)eDesignation.SystemAdmin)
                    lstFirmId.Add(dr[CS.FirmId].zToInt().Value);
                else
                    lstUsersId.Add(dr[CS.UsersId].zToInt().Value);
            }

            dtUser = new Query() { UsersIdIn = CU.GetParaIn(lstUsersId, true) }.Select(eSP.qry_User);
            dtFirm = new Query() { FirmIdIn = CU.GetParaIn(lstFirmId, true) }.Select(eSP.qry_Firm);
            dtOrderProduct = new Query() { OrdersIdIn = CU.GetParaIn(lstOrdersId, true) }.Select(eSP.qry_OrderProduct);

            var dtNewOrder = dtOrder.Copy();

            if (txtNoofPrint.zToInt() > 0)
            {
                for (int i = 1; i < txtNoofPrint.zToInt(); i++)
                    dtNewOrder.Merge(dtOrder.Copy());
            }

            dtNewOrder = dtNewOrder.Select("", "Date DESC, OrdersId").CopyToDataTable();

            rptPrintOrderSlip.DataSource = dtNewOrder;
            rptPrintOrderSlip.DataBind();

            popupPrintOrderSlip.Show();
        }
        else
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "No Data Fount");
        }
    }
    int PrintIndex = 0;
    protected void rptPrintOrderSlip_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        PrintIndex++;
        var lblOrderDate = e.Item.FindControl("lblOrderDate") as Label;
        var lblOrderNo = e.Item.FindControl("lblOrderNo") as Label;
        var lblOrderAmountTitle = e.Item.FindControl("lblOrderAmountTitle") as Label;
        var lblOrderAmount = e.Item.FindControl("lblOrderAmount") as Label;
        var lblOrderCouriarType = e.Item.FindControl("lblOrderCouriarType") as Label;
        var lblOrderCouriarName = e.Item.FindControl("lblOrderCouriarName") as Label;
        var lblOrderFirm = e.Item.FindControl("lblOrderFirm") as Label;

        var lblCustomerName = e.Item.FindControl("lblCustomerName") as Label;
        var lblCustomerAddress = e.Item.FindControl("lblCustomerAddress") as Label;
        var lblCustomerMobile = e.Item.FindControl("lblCustomerMobile") as Label;
        var lblCustomerPincode = e.Item.FindControl("lblCustomerPincode") as Label;
        var lblCustomerCity = e.Item.FindControl("lblCustomerCity") as Label;

        var lblOrderFirmName = e.Item.FindControl("lblOrderFirmName") as Label;
        var lblOrderFirmAddress = e.Item.FindControl("lblOrderFirmAddress") as Label;
        var lblOrderFirmMobile = e.Item.FindControl("lblOrderFirmMobile") as Label;


        var lblOrderTotalQuantity = e.Item.FindControl("lblOrderTotalQuantity") as Label;
        var rptPrintOrderProduct = e.Item.FindControl("rptPrintOrderProduct") as Repeater;
        var divPageBreak = e.Item.FindControl("divPageBreak") as HtmlControl;


        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;

        if (lbleOrganization.zToInt() != (int)eOrganisation.Vinay)
            lblOrderDate.Text = "Date: " + Convert.ToDateTime(dataItem[CS.Date]).ToString(CS.ddMMyyyy);

        lblOrderNo.Text = "Order No: " + dataItem[CS.OrdersId].ToString();
        lblOrderAmountTitle.Text = dataItem[CS.eCourierType].zToInt() == (int)eCourierType.COD ? "Collectable Amount" : "Invoice Amount";
        lblOrderAmount.Text = (dataItem[CS.TotalAmount].zToInt() - (!dataItem[CS.TotalPaidAmount].zIsNullOrEmpty() ? dataItem[CS.TotalPaidAmount].zToInt() : 0)).ToString().Replace(".00", "0") + " Rs";
        lblOrderCouriarType.Text = dataItem[CS.IsPost].zToInt() == (int)eYesNo.Yes ? "POST" : CU.GetDescription((eCourierType)dataItem[CS.eCourierType].zToInt());
        lblOrderCouriarName.Text = dataItem[CS.CourierName].ToString();
        //lblOrderFirm.Text = "OFB";//???

        lblCustomerName.Text = dataItem[CS.Name].ToString();
        lblCustomerAddress.Text = dataItem[CS.FullAddress].ToString();
        lblCustomerMobile.Text = dataItem[CS.WhatsAppNo].ToString() + ", " + dataItem[CS.MobileNo].ToString();
        lblCustomerPincode.Text = dataItem[CS.Pincode].ToString();
        lblCustomerCity.Text = dataItem[CS.CityName].ToString() + " - " + dataItem[CS.StateName].ToString();
        lblCustomerPincode.Text = dataItem[CS.Pincode].ToString();

        if (dataItem[CS.eDesignation].zToInt() == (int)eDesignation.Admin || dataItem[CS.eDesignation].zToInt() == (int)eDesignation.SystemAdmin)
        {
            var drFirm = dtFirm.Select(CS.FirmId + " = " + dataItem[CS.FirmId].zToInt());
            lblOrderFirmName.Text = drFirm[0][CS.FirmName].ToString();
            lblOrderFirmAddress.Text = drFirm[0][CS.FullAddress].ToString();
        }
        else
        {
            var drUser = dtUser.Select(CS.UsersId + " = " + dataItem[CS.UsersId].zToInt());
            lblOrderFirmName.Text = drUser[0][CS.Name].ToString();
            lblOrderFirmAddress.Text = drUser[0][CS.FullAddress].ToString();
        }
        lblOrderFirmMobile.Text = "+91 " + dataItem[CS.UserMobileNo].ToString();


        lblOrderTotalQuantity.Text = "Total Qty: " + dataItem[CS.TotalQuantity].ToString();

        var dtOP = new DataTable();
        var drOP = dtOrderProduct.Select(CS.OrdersId + " = " + dataItem[CS.OrdersId].zToInt());
        if (drOP.Length > 0)
            dtOP = drOP.CopyToDataTable();

        divPageBreak.Visible = (PrintIndex > 0 && PrintIndex % 2 == 0);

        rptPrintOrderProduct.DataSource = dtOP;
        rptPrintOrderProduct.DataBind();
    }

    protected void rptPrintOrderProduct_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var lblItemName = e.Item.FindControl("lblItemName") as Label;

        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;

        lblItemName.Text = dataItem[CS.ItemName].ToString() + "-" + dataItem[CS.SerialNo].ToString() + " * " + dataItem[CS.Quantity].ToString() + " " + dataItem[CS.Description].ToString();
    }


    protected void lnkNoofPrint_OnClick(object sender, EventArgs e)
    {
        lnkPrintOrderSlip_OnClick(null, null);
    }

    #endregion

    #region Print Order Product

    protected void lnkPrintOrderProduct_OnClick(object sender, EventArgs e)
    {
        var lstOrdersId = grdOrder.zIsValidSelection("chkSelect", CS.OrdersId);

        var dtOrder = GetOrderDt(ePageIndex.AllPage, lstOrdersId);
        if (dtOrder.Rows.Count > 0)
        {
            lstOrdersId = new List<int>();

            foreach (DataRow dr in dtOrder.Rows)
                lstOrdersId.Add(dr[CS.OrdersId].zToInt().Value);

            dtOrderProduct = new Query() { OrdersIdIn = CU.GetParaIn(lstOrdersId, true) }.Select(eSP.qry_OrderProduct);

            var dtOrderProductVendor = dtOrderProduct.DefaultView.ToTable(true, CS.VendorName);

            rptPrintOrderVendor.DataSource = dtOrderProductVendor;
            rptPrintOrderVendor.DataBind();

            popupPrintOrderProduct.Show();
        }
        else
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "No Data Fount");
        }

    }

    protected void rptPrintOrderVendor_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var ltrOrderVendorName = e.Item.FindControl("ltrOrderVendorName") as Literal;
        var rptOrderProduct = e.Item.FindControl("rptOrderProduct") as Repeater;

        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;

        ltrOrderVendorName.Text = dataItem[CS.VendorName].ToString().Trim().zIsNullOrEmpty() ? "Other" : dataItem[CS.VendorName].ToString();

        var dtProduct = new DataTable();
        string Query = CS.VendorName + " = '" + dataItem[CS.VendorName].ToString() + "'";
        if (dataItem[CS.VendorName].ToString().Trim().zIsNullOrEmpty())
            Query = CS.VendorName + " IS NULL OR " + CS.VendorName + " = ''";

        dtProduct = dtOrderProduct.Select(Query).CopyToDataTable();

        dtProduct = dtProduct.DefaultView.ToTable(true, CS.ItemName, CS.ProductImageId, CS.ProductId, CS.PurchasePrice);

        rptOrderProduct.DataSource = dtProduct;
        rptOrderProduct.DataBind();
    }

    protected void rptOrderProduct_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var imgOrderProductImage = e.Item.FindControl("imgOrderProductImage") as Image;
        var lblOrderProductName = e.Item.FindControl("lblOrderProductName") as Label;
        var lblOrderProductPrice = e.Item.FindControl("lblOrderProductPrice") as Label;
        var ltrOrderRequiredQty = e.Item.FindControl("ltrOrderRequiredQty") as Literal;

        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;

        lblOrderProductName.Text = dataItem[CS.ItemName].ToString();
        lblOrderProductPrice.Text = "Price: " + dataItem[CS.PurchasePrice].ToString();
        imgOrderProductImage.Visible = dataItem[CS.ProductImageId].zToInt() > 0;
        imgOrderProductImage.ImageUrl = CU.GetFilePath(true, ePhotoSize.P50, eFolder.ProductCImage, dataItem[CS.ProductImageId].ToString(), true);
        ltrOrderRequiredQty.Text = dtOrderProduct.Compute("SUM(" + CS.Quantity + ")", CS.ProductImageId + " = " + dataItem[CS.ProductImageId].ToString() + " AND " + CS.ItemName + " = '" + dataItem[CS.ItemName].ToString() + "'").ToString();
    }

    #endregion

    #region Order Complain

    protected void lnkOrderComplain_OnClick(object sender, EventArgs e)
    {
        txtOrderComplainDate.Text = IndianDateTime.Today.ToString(CS.ddMMyyyy);
        lblOrderComplainEmailList.Text = "jatin111lathiya@gmail.com, jatin@octfis.com, vishal@octfis.com";//???
        txtOrderComplainEmail.Text = string.Empty;
        txtOrderComplainSubject.Text = "Re Deliver Parcel";

        LoadOrderComplain();
    }


    private void LoadOrderComplain()
    {
        var lstOrdersId = grdOrder.zIsValidSelection("chkSelect", CS.OrdersId);
        var dtOrder = GetOrderDt(ePageIndex.AllPage, lstOrdersId);

        if (dtOrder.Rows.Count > 0)
        {
            dtOrderComplain = new Query() { OrdersIdIn = CU.GetParaIn(lstOrdersId, false) }.Select(eSP.qry_OrderComplain);

            dtActionToBeTaken = new Query() { }.Select(eSP.qry_ActionToBeTakenOnly);
            dtActionTaken = new Query() { }.Select(eSP.qry_ActionTakenOnly);
            dtCustomerReply = new Query() { }.Select(eSP.qry_CustomerReplyOnly);

            rptOrderComplain.DataSource = dtOrder;
            rptOrderComplain.DataBind();

            popupOrderComplain.Show();
        }
        else
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "No Data Fount");
        }
    }

    protected void rptOrderComplain_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var lblAWBNo = e.Item.FindControl("lblAWBNo") as Label;
        var lblComplainOrderId = e.Item.FindControl("lblComplainOrderId") as Label;
        var lblOrderNo = e.Item.FindControl("lblOrderNo") as Label;
        var lblOrderStatus = e.Item.FindControl("lblOrderStatus") as Label;
        var lblOrderDate = e.Item.FindControl("lblOrderDate") as Label;
        var lblCustomerName = e.Item.FindControl("lblCustomerName") as Label;
        var lblCustomerMobile = e.Item.FindControl("lblCustomerMobile") as Label;
        var lblCustomerAddress = e.Item.FindControl("lblCustomerAddress") as Label;
        var lblComplainCount = e.Item.FindControl("lblComplainCount") as Label;

        var txtActionToBeTaken = e.Item.FindControl("txtActionToBeTaken") as TextBox;
        var txtActionTaken = e.Item.FindControl("txtActionTaken") as TextBox;
        var txtCustomerReply = e.Item.FindControl("txtCustomerReply") as TextBox;

        var rptActionToBeTaken = e.Item.FindControl("rptActionToBeTaken") as Repeater;
        var rptActionTaken = e.Item.FindControl("rptActionTaken") as Repeater;
        var rptCustomerReply = e.Item.FindControl("rptCustomerReply") as Repeater;

        txtActionToBeTaken.Text = txtActionTaken.Text = txtCustomerReply.Text = string.Empty;


        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;


        lblComplainOrderId.Text = dataItem[CS.OrdersId].ToString();
        lblOrderDate.Text = Convert.ToDateTime(dataItem[CS.Date]).ToString(CS.ddMMyyyy);
        lblComplainCount.Text = dtOrderComplain.Select(CS.OrdersId + "='" + lblComplainOrderId.zToInt() + "'").Length.ToString();

        var drOrderComplain = dtOrderComplain.Select(CS.OrdersId + "='" + lblComplainOrderId.zToInt() + "'" + " AND " + CS.Date + "='" + txtOrderComplainDate.zToDate() + "'", "Date DESC");
        if (drOrderComplain.Length > 0)
        {
            txtActionToBeTaken.Text = drOrderComplain[0][CS.ActionToBeTaken].ToString();
            txtActionTaken.Text = drOrderComplain[0][CS.ActionTaken].ToString();
            txtCustomerReply.Text = drOrderComplain[0][CS.CustomerReply].ToString();
        }

        lblAWBNo.Text = dataItem[CS.AWBNo].ToString();
        lblOrderNo.Text = "O No." + dataItem[CS.OrdersId].ToString();
        lblOrderStatus.Text = dataItem[CS.StatusName].ToString();
        lblCustomerName.Text = dataItem[CS.Name].ToString();
        lblCustomerMobile.Text = dataItem[CS.WhatsAppNo].ToString() + ", " + dataItem[CS.MobileNo].ToString();
        lblCustomerAddress.Text = dataItem[CS.CityName].ToString() + " - " + dataItem[CS.Pincode].ToString();

        rptActionToBeTaken.DataSource = dtActionToBeTaken;
        rptActionToBeTaken.DataBind();

        rptActionTaken.DataSource = dtActionTaken;
        rptActionTaken.DataBind();

        rptCustomerReply.DataSource = dtCustomerReply;
        rptCustomerReply.DataBind();

    }

    protected void rptAutoFill_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var ltrActionToBeTaken = e.Item.FindControl("ltrActionToBeTaken") as Literal;
        var ltrActionTaken = e.Item.FindControl("ltrActionTaken") as Literal;
        var ltrCustomerReply = e.Item.FindControl("ltrCustomerReply") as Literal;

        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;

        if (ltrActionToBeTaken != null)
        {
            ltrActionToBeTaken.Text = "<option value='" + dataItem[CS.ActionToBeTaken].ToString() + "'>";
        }
        else if (ltrActionTaken != null)
        {
            ltrActionTaken.Text = "<option value='" + dataItem[CS.ActionTaken].ToString() + "'>";
        }
        else if (ltrCustomerReply != null)
        {
            ltrCustomerReply.Text = "<option value='" + dataItem[CS.CustomerReply].ToString() + "'>";
        }
    }

    protected void txtOrderComplainDate_OnTextChanged(object sender, EventArgs e)
    {
        LoadOrderComplain();
    }


    private bool IsValidOrderComplain()
    {
        if (txtOrderComplainEmail.Text.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Email.");
            txtOrderComplainEmail.Focus();
            return false;
        }

        if (txtOrderComplainSubject.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Subject.");
            txtOrderComplainSubject.Focus();
            return false;
        }

        return true;
    }

    private string GetOrderComplainBody(ref List<OrderComplain> lstOrderComplain)
    {
        var dtOrderComplain = new OrderComplain() { UsersId = CU.GetUsersId(), Date = txtOrderComplainDate.zToDate() }.Select();

        lstOrderComplain = new List<OrderComplain>();

        string thStyle = "border: 1px solid #000;padding:5px;font-size: 15px;";
        string tdStyle = "border: 1px solid #000;padding:5px;font-size: 14px;";

        var Body = @"
			<html>
			<head>
				<title>Order Complain</title>
			</head>
			<body>
				<table style='width: 100%;border-collapse: collapse;'>
					<thead>
						<tr>
							<th style='" + thStyle + @"'>AWB No</th>
							<th style='" + thStyle + @"'>Status</th>
							<th style='" + thStyle + @"'>Customer</th>
                            <th style='" + thStyle + @"'>Action To Be Taken</th >
							<th style='" + thStyle + @"'>City</th>
							<th style='" + thStyle + @"'>Date</th>
						</tr>
					</thead>
					<tbody>";


        #region Set Data

        foreach (RepeaterItem Item in rptOrderComplain.Items)
        {
            var lblAWBNo = Item.FindControl("lblAWBNo") as Label;
            var lblComplainOrderId = Item.FindControl("lblComplainOrderId") as Label;
            var lblOrderStatus = Item.FindControl("lblOrderStatus") as Label;
            var lblOrderDate = Item.FindControl("lblOrderDate") as Label;
            var lblCustomerName = Item.FindControl("lblCustomerName") as Label;
            var lblCustomerMobile = Item.FindControl("lblCustomerMobile") as Label;
            var lblCustomerAddress = Item.FindControl("lblCustomerAddress") as Label;

            var txtActionToBeTaken = Item.FindControl("txtActionToBeTaken") as TextBox;
            var txtActionTaken = Item.FindControl("txtActionTaken") as TextBox;
            var txtCustomerReply = Item.FindControl("txtCustomerReply") as TextBox;

            if (!txtActionToBeTaken.zIsNullOrEmpty() || !txtActionTaken.zIsNullOrEmpty() || !txtCustomerReply.zIsNullOrEmpty())
            {
                var drOrderComplain = dtOrderComplain.Select(CS.OrdersId + "=" + lblComplainOrderId.zToInt() + " AND " + CS.ActionTaken + "='" + txtActionTaken.Text + "'" +
                " AND " + CS.CustomerReply + "='" + txtCustomerReply.Text + "'" +
                " AND " + CS.ActionToBeTaken + "='" + txtActionToBeTaken.Text + "'");

                if (drOrderComplain.Length == 0)
                {
                    Body += @"<tr>
							<td style='" + tdStyle + @"text-align: center;width: 100px;'>" + lblAWBNo.Text + @"</td>
							<td style='" + tdStyle + @"text-align: center;width: 100px;'>" + lblOrderStatus.Text + @"</td>
							<td style='" + tdStyle + @"'>" + lblCustomerName.Text + @"<br />" + lblCustomerMobile.Text + @"</td>
                            <td style='" + tdStyle + @"'>" + txtActionToBeTaken.Text + @"</td >

							<td style='" + tdStyle + @"width: 150px;'>" + lblCustomerAddress.Text + @"</td>
							<td style='" + tdStyle + @"text-align: center;width: 75px;'>" + lblOrderDate.Text + @"</td>
						</tr>";

                    lstOrderComplain.Add(new OrderComplain()
                    {
                        OrdersId = lblComplainOrderId.zToInt(),
                        Date = IndianDateTime.Today,
                        UsersId = CU.GetUsersId(),
                        ActionToBeTaken = txtActionToBeTaken.Text,
                        ActionTaken = txtActionTaken.Text,
                        CustomerReply = txtCustomerReply.Text
                    });
                }
            }
        }

        #endregion

        Body += @"	</tbody>
				</table>
			</body>
			</html>";

        return Body;
    }

    protected void btnSendOrderComplain_OnClick(object sender, EventArgs e)
    {
        if (!IsValidOrderComplain())
        {
            popupOrderComplain.Show();
            return;
        }

        List<string> lstToEmailId = new List<string>();
        foreach (string Email in txtOrderComplainEmail.Text.Split(','))
            lstToEmailId.Add(Email.Trim());

        List<string> lstCCEmailId = new List<string>();
        foreach (string Email in txtOrderComplainCCEmail.Text.Split(','))
            lstCCEmailId.Add(Email.Trim());

        string ErrorMsg = string.Empty;
        List<OrderComplain> lstOrderComplain = new List<OrderComplain>();
        string HTMLBody = GetOrderComplainBody(ref lstOrderComplain);

        if (lstOrderComplain.Count > 0)
        {
            CU.SendMail(lstToEmailId, lstCCEmailId, txtOrderComplainSubject.Text, HTMLBody, true, string.Empty, ref ErrorMsg);
            if (ErrorMsg.zIsNullOrEmpty())
            {
                List<OrderComplain> lstOrderComplainInsert = new List<OrderComplain>();
                foreach (OrderComplain objOrderComplain in lstOrderComplain)
                {
                    lstOrderComplainInsert.Add(objOrderComplain);
                    if (lstOrderComplainInsert.Count >= 100)
                    {
                        lstOrderComplainInsert.Insert();
                        lstOrderComplainInsert = new List<OrderComplain>();
                    }
                }

                lstOrderComplainInsert.Insert();
                CU.ZMessage(eMsgType.Success, string.Empty, "Mail Send Successfully.");
            }
            else
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "Erro" + ErrorMsg.Replace("'", " "));
                popupOrderComplain.Show();
            }
        }
        else
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "No Data Found.");
            popupOrderComplain.Show();
        }
    }

    #endregion

    #region Change Status

    protected void lnkExcelChangeStatus_OnClick(object sender, EventArgs e)
    {
        popupExcelChangeStatus.Show();
    }

    protected void btnChangeStatus_OnClick(object sender, EventArgs e)
    {
        if (fuImportChangeStatus.HasFile)
        {
            var dt = new DataTable();
            if (!CU.IsValidCSVFile(fuImportChangeStatus, ref dt, 0, "Change Status"))
            {
                popupExcelChangeStatus.Show();
                return;
            }

            InsertData(dt);
        }
        else
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Excel File to upload.");
            popupExcelChangeStatus.Show();
        }
    }

    private void InsertData(DataTable dt)
    {
        int UnDefinedCount = 0;

        try
        {
            int CourierAwbNoColumn = 1, CourierStatusColumn = 7, PaymentModeColumn = 13;
            int ColumnIndex = 0;
            foreach (DataColumn dc in dt.Columns)
            {
                string ColumnName = Regex.Replace(dc.ColumnName, @"[\W_]$", "").Replace(" ", "").ToLower();

                if (ColumnName == "docket_no")
                    CourierAwbNoColumn = ColumnIndex;

                if (ColumnName == "status")
                    CourierStatusColumn = ColumnIndex;

                if (ColumnName == "payment_mode")
                    PaymentModeColumn = ColumnIndex;

                ColumnIndex++;
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string CourierAwbNo = dt.Rows[i][CourierAwbNoColumn].ToString().Trim().TrimEnd(',');

                if (!CourierAwbNo.zIsNullOrEmpty())
                {
                    var dtOrder = new Query()
                    {
                        AWBNo = CourierAwbNo
                    }.Select(eSP.qry_Orders);

                    if (dtOrder.Rows.Count > 0)
                    {
                        string CourierStatus = dt.Rows[i][CourierStatusColumn].ToString().Trim().TrimEnd(',');
                        string PaymentMode = string.Empty;

                        try
                        {
                            PaymentMode = dt.Rows[i][PaymentModeColumn].ToString().Trim().TrimEnd(',');
                        }
                        catch { }
                        int geteStatusType = 0;
                        CU.UpdateOrderStatus(new Orders()
                        {
                            OrdersId = dtOrder.Rows[0][CS.OrdersId].zToInt(),
                            OrderStatusId = CU.GetOrderStatusId(ref geteStatusType, 0, CourierStatus, PaymentMode),
                        });

                        CU.SetOrderTransaction(dtOrder.Rows[0][CS.OrdersId].zToInt().Value);

                        if (geteStatusType == (int)eStatusType.UnDefined)
                            UnDefinedCount++;
                    }
                }
            }


            CU.ZMessage(eMsgType.Success, string.Empty, "Status Change Successfully");
        }
        catch (Exception ex)
        {
            CU.ZMessage(eMsgType.Error, string.Empty, ex.Message, 0);
        }

        if (UnDefinedCount > 0)
        {
            foreach (ListItem Item in lstStatusTypeIn.Items)
                Item.Selected = (Item.Value.zToInt() == (int)eStatusType.UnDefined);
        }

        LoadOrderGrid(ePageIndex.Custom);
    }

    #endregion

    #region Order Traking

    protected void lnkTrackCurior_OnClick(object sender, EventArgs e)
    {
        var lstUpdate = new List<Orders>();
        string ShipwayUsername = CU.GetNameValue(eNameValue.ShipwayUsername);
        string ShipwayLicenceKey = CU.GetNameValue(eNameValue.ShipwayLicenceKey);

        foreach (GridViewRow Item in grdOrder.Rows)
        {
            var lblgrdOrdersId = Item.FindControl("lblgrdOrdersId") as Label;
            var lblCarrierCode = Item.FindControl("lblCarrierCode") as Label;
            var txtAWBNo = Item.FindControl("txtAWBNo") as TextBox;

            var lblTraking = Item.FindControl("lblTraking") as Label;
            var aTrakingInfo = Item.FindControl("aTrakingInfo") as HtmlAnchor;
            var lblLastTrakingInfo = Item.FindControl("lblLastTrakingInfo") as Label;


            if (txtAWBNo.zIsNullOrEmpty())
                continue;

            if (lblTraking.Text.zIsNullOrEmpty() && !lblCarrierCode.zIsNullOrEmpty())
            {
                try
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://shipway.in/api/pushOrderData");
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = "{\r\n\"username\":\"" + ShipwayUsername + "\",\r\n\"password\":\"" + ShipwayLicenceKey + "\",\r\n\"carrier_id\":\"" + lblCarrierCode.Text + "\",\r\n\"awb\":\" " + txtAWBNo.Text + "\",\r\n\"order_id\":\"" + lblgrdOrdersId.Text + "\",\r\n\"first_name\":\"N/A\",\r\n\"last_name\":\" N/A \",\r\n\"email\":\" N/A \",\r\n\"phone\":\" N/A \",\r\n\"products\":\" N/A \",\r\n\"company\":\" xxxxx \"\r\n }";
                        streamWriter.Write(json);
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                    }
                }
                catch { }
            }

            try
            {
                string TrakingId = txtAWBNo.Text;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://shipway.in/api/getOrderShipmentDetails");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\r\n\"username\":\"" + ShipwayUsername + "\",\r\n\"password\":\"" + ShipwayLicenceKey + "\",\r\n\"order_id\":\"" + lblgrdOrdersId.Text + "\" }";
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    lblTraking.Text = result;

                    var objTraking = new JavaScriptSerializer().Deserialize<Traking>(result);

                    aTrakingInfo.HRef = objTraking.response.tracking_url;
                    aTrakingInfo.InnerHtml = objTraking.response.current_status;
                    if (objTraking.response.scan != null && objTraking.response.scan.Count > 0)
                        lblLastTrakingInfo.Text = "Last Location: " + objTraking.response.scan[0].location + " - " + objTraking.response.scan[0].time;

                    lstUpdate.Add(new Orders() { OrdersId = lblgrdOrdersId.zToInt(), TrakingInfo = result });
                    if (lstUpdate.Count > 500)
                    {
                        lstUpdate.Update();
                        lstUpdate = new List<Orders>();
                    }
                }
            }
            catch { }
        }

        lstUpdate.Update();
        CU.ZMessage(eMsgType.Success, string.Empty, "Traking Successfully Updated");
    }

    #endregion

    #region payment

    protected void lnkExcelPayment_OnClick(object sender, EventArgs e)
    {
        popupExcelPayment.Show();
    }

    protected void btnPay_OnClick(object sender, EventArgs e)
    {
        if (fuImportPayment.HasFile)
        {
            var dt = new DataTable();
            if (!CU.IsValidCSVFile(fuImportPayment, ref dt, 0, "Payment"))
            {
                popupExcelPayment.Show();
                return;
            }

            InsertPaymentData(dt);
        }
        else
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Excel File to upload.");
            popupExcelPayment.Show();
        }
    }

    private void InsertPaymentData(DataTable dt)
    {
        try
        {
            int DocketNoColumn = 0, NoteColumn = 1, PaymentDateColumn = 7, TransactionIdColumn = 6, AmountColumn = 5, BankColumn = 8;
            int ColumnIndex = 0;
            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.ColumnName == "Docket_No")
                    DocketNoColumn = ColumnIndex;

                if (dc.ColumnName == "Payment_Date")
                    PaymentDateColumn = ColumnIndex;

                if (dc.ColumnName == "Amount")
                    AmountColumn = ColumnIndex;

                if (dc.ColumnName == "Transaction_ID_No")
                    TransactionIdColumn = ColumnIndex;

                if (dc.ColumnName == "Order_No")
                    NoteColumn = ColumnIndex;

                if (dc.ColumnName == "Bank")
                    BankColumn = ColumnIndex;

                ColumnIndex++;
            }
            var dtBankAccount = new Query() { OrganizationId = CU.GetOrganizationId(), eStatusNot = (int)eStatus.Delete }.Select(eSP.qry_BankAccount);
            var lstOrderPayment = new List<OrderPayment>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string DocketNo = dt.Rows[i][DocketNoColumn].ToString().Trim().TrimEnd(',');
                string TransactionId = dt.Rows[i][TransactionIdColumn].ToString().Trim().TrimEnd(',');
                string Note = dt.Rows[i][NoteColumn].ToString().Trim().TrimEnd(',');
                decimal? Amount = dt.Rows[i][AmountColumn].zToDecimal();
                DateTime? PaymentDate = dt.Rows[i][PaymentDateColumn].zToDate();
                string BankName = dt.Rows[i][BankColumn].ToString();

                var drBankAccount = dtBankAccount.Select(CS.BankAccountName + "='" + BankName + "'");

                if (DocketNo.zIsNullOrEmpty() || TransactionId.zIsNullOrEmpty()
                    || Amount.zIsNullOrEmpty() || !Amount.zIsDecimal(false)
                    || BankName.zIsNullOrEmpty() || !PaymentDate.zIsDate()
                    || drBankAccount.Length == 0)
                    continue;

                var dtOrder = new Query()
                {
                    AWBNo = DocketNo
                }.Select(eSP.qry_Orders);

                if (dtOrder.Rows.Count != 0 || new OrderPayment() { TransactionId = TransactionId }.SelectCount() != 0)
                    continue;

                lstOrderPayment.Add(new OrderPayment()
                {
                    OrdersId = dtOrder.Rows[0][CS.OrdersId].zToInt(),
                    BankAccountId = drBankAccount[0][CS.BankAccountId].zToInt(),
                    PaymentDate = PaymentDate.HasValue ? PaymentDate : null,
                    Amount = Amount.HasValue ? Amount.zToDecimal() : 0,
                    TransactionId = TransactionId,
                    Note = Note,
                });
            }

            lstOrderPayment.Insert();
            CU.ZMessage(lstOrderPayment.Count != 0 ? eMsgType.Success : eMsgType.Error, string.Empty, lstOrderPayment.Count + " Payment Done.");
        }
        catch (Exception ex)
        {
            CU.ZMessage(eMsgType.Error, string.Empty, ex.Message, 0);
        }

        LoadOrderGrid(ePageIndex.Custom);
    }


    #endregion

    #region export

    protected void lnkExcelExport_Click(object sender, EventArgs e)
    {
        var dtOrder = GetOrderDt(ePageIndex.AllPage, new List<int>());

        dtOrder.Columns.Remove(dtOrder.Columns[CS.Weight]);

        dtOrder.Columns[CS.RowIndex].ColumnName = CS.Order_No;
        dtOrder.Columns[CS.ProductList].ColumnName = CS.Item_Name;
        dtOrder.Columns[CS.AWBNo].ColumnName = CS.AWB_No;
        dtOrder.Columns[CS.Name].ColumnName = CS.Customer_Name;
        dtOrder.Columns[CS.Address].ColumnName = CS.Shipping_Add1;
        dtOrder.Columns[CS.CityName].ColumnName = CS.Shipping_City;
        dtOrder.Columns[CS.StateName].ColumnName = CS.Shipping_State;
        dtOrder.Columns[CS.Pincode].ColumnName = CS.Shipping_Zip;
        dtOrder.Columns[CS.WhatsAppNo].ColumnName = CS.Shipping_MobileNo;
        dtOrder.Columns[CS.MobileNo].ColumnName = CS.Shipping_Add2;
        dtOrder.Columns[CS.TotalAmount].ColumnName = CS.Invoice_Value;
        dtOrder.Columns[CS.CollectableAmount].ColumnName = CS.Collectable_amount;

        dtOrder.Columns.Add(CS.N0_of_Pieces, typeof(string), "'1'");
        dtOrder.Columns.Add(CS.Mode, typeof(string), "'C'");
        dtOrder.Columns.Add(CS.Weight, typeof(string), "'0.3'");
        dtOrder.Columns.Add(CS.Type_of_Service, typeof(string), "'Express'");

        dtOrder.Columns.Add(CS.IsPUDO, typeof(string), "'N'");
        dtOrder.Columns.Add(CS.Type_of_Delivery, typeof(string), "'Home Delivery'");
        dtOrder.Columns.Add(CS.PUDO_Code, typeof(string), "'123'");
        dtOrder.Columns.Add(CS.Length, typeof(string), "''");
        dtOrder.Columns.Add(CS.Breadth, typeof(string), "''");
        dtOrder.Columns.Add(CS.Height, typeof(string), "''");
        dtOrder.Columns.Add(CS.ReturnName, typeof(string));
        dtOrder.Columns.Add(CS.ReturnAddress, typeof(string));
        dtOrder.Columns.Add(CS.ReturnPincode, typeof(string));
        dtOrder.Columns.Add(CS.ReturnTelephoneNumber, typeof(string));

        if (lblOrganizationId.zToInt() == (int)eOrganisation.Vinay)
        {
            dtOrder.Columns.Add(CS.VendorName, typeof(string), "'insta: @royalselection_india, @amazon_villa'");
            dtOrder.Columns.Add(CS.VendorAddress1, typeof(string), "'Madhav Park Society'");
            dtOrder.Columns.Add(CS.VendorAddress2, typeof(string), "'Singanpor Char Rasta, Vedroad'");
            dtOrder.Columns.Add(CS.VendorPincode, typeof(string), "'395004'");
            dtOrder.Columns.Add(CS.VendorTeleNo, typeof(string), "'8200430927'");

            dtOrder.Columns.Add(CS.Agent_code, typeof(string), "'SP000103359'");

            foreach (DataRow drOrder in dtOrder.Rows)
            {
                drOrder[CS.Customer_Name] = drOrder[CS.Customer_Name].ToString() + " " + drOrder[CS.Item_Name].ToString();
            }
        }
        else
        {
            dtOrder.Columns.Add(CS.VendorName, typeof(string), "'gopi fashion'");
            dtOrder.Columns.Add(CS.VendorAddress1, typeof(string), "'ifm ,3006 near sita nager chokadi , puna kunbhaniya rode'");
            dtOrder.Columns.Add(CS.VendorAddress2, typeof(string));
            dtOrder.Columns.Add(CS.VendorPincode, typeof(string), "'395010'");
            dtOrder.Columns.Add(CS.VendorTeleNo, typeof(string), "'7202032894'");

            dtOrder.Columns.Add(CS.Agent_code, typeof(string), "'SP000101331'");
        }

        var lstColumns = new List<string>();
        lstColumns.Add(CS.Order_No);
        lstColumns.Add(CS.Item_Name);
        lstColumns.Add(CS.AWB_No);
        lstColumns.Add(CS.N0_of_Pieces);
        lstColumns.Add(CS.Customer_Name);
        lstColumns.Add(CS.Shipping_Add1);
        lstColumns.Add(CS.Shipping_Add2);
        lstColumns.Add(CS.Shipping_City);
        lstColumns.Add(CS.Shipping_State);
        lstColumns.Add(CS.Shipping_Zip);
        lstColumns.Add(CS.Shipping_MobileNo);
        lstColumns.Add(CS.Invoice_Value);
        lstColumns.Add(CS.Mode);
        lstColumns.Add(CS.Collectable_amount);
        lstColumns.Add(CS.Weight);
        lstColumns.Add(CS.Type_of_Service);
        lstColumns.Add(CS.VendorName);
        lstColumns.Add(CS.VendorAddress1);
        lstColumns.Add(CS.VendorAddress2);
        lstColumns.Add(CS.VendorPincode);
        lstColumns.Add(CS.VendorTeleNo);
        lstColumns.Add(CS.IsPUDO);
        lstColumns.Add(CS.Type_of_Delivery);
        lstColumns.Add(CS.PUDO_Code);
        lstColumns.Add(CS.Length);
        lstColumns.Add(CS.Breadth);
        lstColumns.Add(CS.Height);
        lstColumns.Add(CS.Agent_code);
        lstColumns.Add(CS.ReturnName);
        lstColumns.Add(CS.ReturnAddress);
        lstColumns.Add(CS.ReturnPincode);
        lstColumns.Add(CS.ReturnTelephoneNumber);

        var lstColumnsSelected = new List<string>();
        lstColumnsSelected.Add(CS.Order_No);
        lstColumnsSelected.Add(CS.Item_Name);
        lstColumnsSelected.Add(CS.AWB_No);
        lstColumnsSelected.Add(CS.N0_of_Pieces);
        lstColumnsSelected.Add(CS.Customer_Name);
        lstColumnsSelected.Add(CS.Shipping_Add1);
        lstColumnsSelected.Add(CS.Shipping_Add2);
        lstColumnsSelected.Add(CS.Shipping_City);
        lstColumnsSelected.Add(CS.Shipping_State);
        lstColumnsSelected.Add(CS.Shipping_Zip);
        lstColumnsSelected.Add(CS.Shipping_MobileNo);
        lstColumnsSelected.Add(CS.Invoice_Value);
        lstColumnsSelected.Add(CS.Mode);
        lstColumnsSelected.Add(CS.Collectable_amount);
        lstColumnsSelected.Add(CS.Weight);
        lstColumnsSelected.Add(CS.Type_of_Service);
        lstColumnsSelected.Add(CS.VendorName);
        lstColumnsSelected.Add(CS.VendorAddress1);
        lstColumnsSelected.Add(CS.VendorAddress2);
        lstColumnsSelected.Add(CS.VendorPincode);
        lstColumnsSelected.Add(CS.VendorTeleNo);
        lstColumnsSelected.Add(CS.IsPUDO);
        lstColumnsSelected.Add(CS.Type_of_Delivery);
        lstColumnsSelected.Add(CS.PUDO_Code);
        lstColumnsSelected.Add(CS.Length);
        lstColumnsSelected.Add(CS.Breadth);
        lstColumnsSelected.Add(CS.Height);
        lstColumnsSelected.Add(CS.Agent_code);
        lstColumnsSelected.Add(CS.ReturnName);
        lstColumnsSelected.Add(CS.ReturnAddress);
        lstColumnsSelected.Add(CS.ReturnPincode);
        lstColumnsSelected.Add(CS.ReturnTelephoneNumber);

        ExcelExport.SetExportData(dtOrder, lstColumns, lstColumnsSelected, "Order");
        popupExcelExport.Show();
    }

    #endregion



    #region Pagging


    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        LoadOrderGrid(ePageIndex.Prev);
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        LoadOrderGrid(ePageIndex.Next);
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        LoadOrderGrid(ePageIndex.First);
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        LoadOrderGrid(ePageIndex.Last);
    }

    protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
    {
        if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
        {
            txtGotoPageNo.Text = "1";
            txtGotoPageNo.Focus();
        }
        LoadOrderGrid(ePageIndex.Custom);
    }

    protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
    {
        txtGotoPageNo.Text = "1";
        LoadOrderGrid(ePageIndex.Custom);
        Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
    }


    #endregion
}
