using System;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;

public partial class ManageOrder : CompressorPage
{
    bool IsSetFocus = false;

    DataTable dtProduct, dtProductItem, dtProductImage;

    protected void Page_Load(object sender, EventArgs e)
    {
        LoginUtilities.CheckSession();

        if (!IsPostBack)
        {

            lblOrganizationId.Text = CU.GetOrganizationId().ToString();
            lblFirmId.Text = CU.GetFirmId().ToString();

            #region Set OrdersId

            string OrdersId = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString[CS.OrdersId.Encrypt()]))
            {
                OrdersId = Request.QueryString[CS.OrdersId.Encrypt()].ToString().Decrypt();
                if (!OrdersId.zIsInteger(false) || new Orders() { OrdersId = OrdersId.zToInt() }.SelectCount() == 0)
                    OrdersId = string.Empty;
            }

            lblOrdersId.Text = OrdersId;

            #endregion

            LoadOrderSourceddl();
            LoadZoneddl();
            LoadUserddl();

            LoadOrderDetail();
        }

        popupManageOrderPayment.btnSave_OnClick += new EventHandler(btnSaveOrderPayment_OnClick);
    }


    protected void ddlUser_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        //LoadCustomerddl();
        LoadCustomerDetail(0);

        foreach (RepeaterItem Item in rptOrderProduct.Items)
            SetProductPrice(Item);

        SetShipCharge();
    }

    protected void ddlCustomer_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        if (sender == null)
            SetZone();
        else
            LoadCustomerDetail(ddlCustomer.zToInt().Value);

        LoadCourierddl();
        SetShipCharge();
    }

    protected void ddlCourier_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        lblCourierIdMenual.Text = ddlCourier.SelectedValue;
        SetShipCharge();
    }

    protected void CourierType_OnCheckedChanged(object sender, EventArgs e)
    {
        SetShipCharge();
    }

    protected void lnkRefreshCustomer_Click(object sender, EventArgs e)
    {
        LoadCustomerddl();
    }


    private void LoadUserddl()
    {
        var dtUser = new Query()
        {
            FirmId = lblFirmId.zToInt(),
            eStatus = (int)eStatus.Active
        }.Select(eSP.qry_User);

        CU.FillDropdown(ref ddlUser, dtUser, string.Empty, CS.UsersId, CS.Name);
        ddlUser.SelectedValue = CU.GetUsersId().ToString();
    }

    private void LoadCustomerddl()
    {
        var dtCustomer = new Query()
        {
            UsersId = ddlUser.zToInt(),
            eStatus = (int)eStatus.Active
        }.Select(eSP.qry_Customer);

        CU.FillDropdown(ref ddlCustomer, dtCustomer, "-- New Customer --", CS.CustomerId, CS.NameWithNumber);
    }

    private void LoadOrderSourceddl()
    {
        var dtOrderSource = new OrderSource()
        {
            FirmId = lblFirmId.zToInt(),
            eStatus = (int)eStatus.Active
        }.Select();

        CU.FillDropdown(ref ddlOrderSource, dtOrderSource, "-- Select Order Source --", CS.OrderSourceId, CS.OrderSourceName);
    }

    private void LoadCourierddl()
    {
        var dtCourier = new DataTable();
        dtCourier.Columns.Add(CS.CourierId, typeof(int));
        dtCourier.Columns.Add(CS.CourierName, typeof(string));
        dtCourier.Columns.Add(CS.SerialNo, typeof(int));

        var drCourier = dtCourier.NewRow();
        drCourier[CS.CourierId] = 0;
        drCourier[CS.CourierName] = "-- Select Courier --";
        drCourier[CS.SerialNo] = 0;
        dtCourier.Rows.InsertAt(drCourier, 0);

        bool IsCOD = true;
        if (!txtPincode.zIsNullOrEmpty())
        {
            var dtServiceAvailability = new Query()
            {
                OrganizationId = lblOrganizationId.zToInt(),
                Pincode = txtPincode.Text,
                eStatus = (int)eStatus.Active
            }.Select(eSP.qry_ServiceAvailability);

            bool IsSetPost = false;
            IsCOD = false;
            foreach (DataRow drServiceAvailability in dtServiceAvailability.Rows)
            {
                if (!IsSetPost)
                    IsSetPost = drServiceAvailability[CS.IsPost].zToInt() == (int)eYesNo.Yes;
                if (!IsCOD)
                    IsCOD = drServiceAvailability[CS.eCOD].zToInt() == (int)eYesNo.Yes;

                drCourier = dtCourier.NewRow();
                drCourier[CS.CourierId] = drServiceAvailability[CS.CourierId].zToInt();
                drCourier[CS.CourierName] = drServiceAvailability[CS.CourierName].ToString();
                drCourier[CS.SerialNo] = drServiceAvailability[CS.SerialNo].zToInt(); ;
                dtCourier.Rows.Add(drCourier);
            }

            if (!IsSetPost)
            {
                var lstCurior = new Courier()
                {
                    OrganizationId = lblOrganizationId.zToInt(),
                    IsPost = (int)eYesNo.Yes,
                    eStatus = (int)eStatus.Active
                }.SelectList<Courier>();

                if (lstCurior.Count > 0)
                {
                    drCourier = dtCourier.NewRow();
                    drCourier[CS.CourierId] = lstCurior[0].CourierId;
                    drCourier[CS.CourierName] = lstCurior[0].CourierName;
                    drCourier[CS.SerialNo] = lstCurior[0].SerialNo.zToInt();
                    dtCourier.Rows.Add(drCourier);
                }
            }
        }

        rdoCOD.Enabled = IsCOD;

        if (!IsCOD)
            rdoCOD.Checked = false;

        dtCourier = dtCourier.Select("", CS.SerialNo).CopyToDataTable();

        ddlCourier.DataSource = dtCourier;
        ddlCourier.DataValueField = CS.CourierId;
        ddlCourier.DataTextField = CS.CourierName;
        ddlCourier.DataBind();
        try { ddlCourier.SelectedValue = lblCourierIdMenual.Text; } catch { }
    }

    private void LoadZoneddl()
    {
        CU.FillEnumddl<eZone>(ref ddlZone, "-- Select Zone --");
    }


    private void LoadOrderDetail()
    {
        int CourierId = 0;
        bool EditMode = IsEditMode();
        var eDesignationId = CU.GeteDesignationId(CU.GetUsersId());

        var objOrderPayment = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageOrderPayment);

        if (EditMode)
        {
            var objOrder = new Orders() { OrdersId = lblOrdersId.zToInt(), }.SelectList<Orders>()[0];

            ddlUser.SelectedValue = objOrder.UsersId.ToString();
            //LoadCustomerddl();
            try { ddlCustomer.SelectedValue = objOrder.CustomerId.ToString(); } catch { }
            txtDate.Text = objOrder.Date.Value.ToString(CS.ddMMyyyy);
            CourierId = objOrder.CourierId.Value;
            rdoPrepaid.Checked = objOrder.eCourierType == (int)eCourierType.Prepaid;
            rdoCOD.Checked = objOrder.eCourierType == (int)eCourierType.COD;
            lblWeight.Text = objOrder.Weight.ToString();
            txtAWBNo.Text = objOrder.AWBNo;
            txtReturnAWBNo.Text = objOrder.ReturnAWBNo;
            lblShipCharge.Text = objOrder.ShipCharge.ToString();
            txtFirmShipCharge.Text = objOrder.FirmShipCharge.ToString();
            txtCustomerShipCharge.Text = objOrder.CustomerShipCharge.ToString();
            txtAdjustment.Text = objOrder.Adjustment.HasValue ? objOrder.Adjustment.ToString() : "0";
            txtCommissionAdjustment.Text = objOrder.CommissionAdjustment.HasValue ? objOrder.CommissionAdjustment.ToString() : "0";
            lblSalePrice.Text = (objOrder.SalePrice + objOrder.CustomerShipCharge).ToString();

            try { ddlOrderSource.SelectedValue = objOrder.OrderSourceId.ToString(); }
            catch { }

            txtDescription.Text = objOrder.Description;

            eStatusType StatusType = CU.GetOrderStatusToeOrderStatus(objOrder.OrderStatusId.Value);

            //lnkSave.Visible = lnkSaveAndNew.Visible = (StatusType != eStatusType.Cancel && StatusType != eStatusType.RTODelivered);
            //lnkSaveAndPay.Visible = (objOrderPayment.IsAddEdit && StatusType != eStatusType.Cancel && StatusType != eStatusType.RTODelivered);

            lnkSave.Visible = lnkSaveAndNew.Visible = (eDesignationId == eDesignation.SystemAdmin || eDesignationId == eDesignation.Admin || StatusType == eStatusType.Draft);
            lnkSaveAndPay.Visible = (objOrderPayment.IsAddEdit && (StatusType == eStatusType.Draft || eDesignationId == eDesignation.SystemAdmin || eDesignationId == eDesignation.Admin));
            LoadCustomerDetail(objOrder.CustomerId.Value);
        }
        else
        {
            //LoadCustomerddl();

            ddlCustomer.SelectedValue = "0";
            if (IndianDateTime.Now.TimeOfDay < new TimeSpan(15, 0, 0))
                txtDate.Text = IndianDateTime.Today.ToString(CS.ddMMyyyy);
            else
                txtDate.Text = IndianDateTime.Today.AddDays(1).ToString(CS.ddMMyyyy);

            rdoPrepaid.Checked = false;
            rdoCOD.Checked = false;
            lblWeight.Text = "0";
            txtAWBNo.Text = string.Empty;
            txtReturnAWBNo.Text = string.Empty;
            lblShipCharge.Text = "0";
            txtFirmShipCharge.Text = "0";
            txtCustomerShipCharge.Text = "0";
            txtAdjustment.Text = "0";
            txtCommissionAdjustment.Text = "0";
            lblSalePrice.Text = "0";
            lblCommission.Text = "0";
            txtDescription.Text = string.Empty;

            lnkSave.Visible = lnkSaveAndNew.Visible = true;
            lnkSaveAndPay.Visible = objOrderPayment.IsAddEdit;
        }


        divUser.Visible = eDesignationId == eDesignation.SystemAdmin || eDesignationId == eDesignation.Admin;
        divCourier.Visible = (eDesignationId == eDesignation.SystemAdmin || eDesignationId == eDesignation.Admin || CU.GetNameValue(eNameValue.CanUserSelectCourier).zToInt() == (int)eYesNo.Yes);
        divZone.Visible = false;
        ddlCourier.SelectedValue = CourierId.ToString();

        pnlService.Visible = false;

        ManageProduct(null, eRepeaterOperation.Select);

        //divAWBNo.Visible = divReturnAWBNo.Visible = EditMode;
    }

    private bool IsEditMode()
    {
        return !lblOrdersId.zIsNullOrEmpty();
    }

    private bool IsValidate()
    {
        if (!rdoPrepaid.Checked && !rdoCOD.Checked)
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Shipping Type.");
            return false;
        }

        if (!txtDate.zIsDate())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Date.");
            txtDate.Focus();
            return false;
        }

        if (!ddlCourier.zIsSelect())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Courier.");
            ddlCourier.Focus();
            return false;
        }

        if (!ddlZone.zIsSelect())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Zone.");
            ddlZone.Focus();
            return false;
        }

        if (!ddlOrderSource.zIsSelect())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Order Source.");
            ddlOrderSource.Focus();
            return false;
        }

        if (!txtCustomerShipCharge.zIsInteger(true))
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Ship Charge.");
            txtCustomerShipCharge.Focus();
            return false;
        }

        if (!ManageProduct(null, eRepeaterOperation.Validate))
            return false;

        return true;
    }

    private bool SaveData()
    {
        if (!IsValidate())
            return false;

        if (!SaveDataCust())
            return false;

        string Message = string.Empty;

        decimal PurchasePrice = 0, UserPrice = 0, SalePrice = 0;

        #region Get Product Amount

        foreach (RepeaterItem Item in rptOrderProduct.Items)
        {
            var ddlProduct = Item.FindControl("ddlProduct") as DropDownList;

            if (!ddlProduct.zIsSelect())
                continue;

            var txtQuantity = Item.FindControl("txtQuantity") as TextBox;

            var lblProductPurchasePrice = Item.FindControl("lblProductPurchasePrice") as Label;
            var lblProductUserPrice = Item.FindControl("lblProductUserPrice") as Label;
            var txtProductSalePrice = Item.FindControl("txtProductSalePrice") as TextBox;

            int Qty = txtQuantity.zToInt().Value;

            if (lblProductPurchasePrice.zIsDecimal(false))
                PurchasePrice += lblProductPurchasePrice.zToDecimal().Value * Qty;

            if (lblProductUserPrice.zIsDecimal(false))
                UserPrice += lblProductUserPrice.zToDecimal().Value * Qty;

            if (txtProductSalePrice.zIsDecimal(false))
                SalePrice += txtProductSalePrice.zToDecimal().Value * Qty;
        }

        #endregion

        var objOrder = new Orders()
        {
            CustomerId = lblCustomerId.zToInt(),
            Date = txtDate.zToDate(),
            CourierId = ddlCourier.zToInt(),
            eCourierType = rdoPrepaid.Checked ? (int)eCourierType.Prepaid : (int)eCourierType.COD,
            eZone = ddlZone.zToInt(),
            Weight = lblWeight.zToDecimal(),
            AWBNo = txtAWBNo.Text,
            ReturnAWBNo = txtReturnAWBNo.Text,
            ShipCharge = lblShipCharge.zToDecimal(),
            FirmShipCharge = !txtFirmShipCharge.zIsNullOrEmpty() ? txtFirmShipCharge.zToDecimal() : 0,
            CustomerShipCharge = txtCustomerShipCharge.zToDecimal(),
            PurchasePrice = PurchasePrice,
            UserPrice = UserPrice,
            SalePrice = SalePrice,
            Adjustment = !txtAdjustment.zIsNullOrEmpty() ? txtAdjustment.zToDecimal() : 0,
            CommissionAdjustment = !txtCommissionAdjustment.zIsNullOrEmpty() ? txtCommissionAdjustment.zToDecimal() : 0,
            OrderSourceId = ddlOrderSource.zToInt(),
            Description = txtDescription.Text
        };

        if (IsEditMode())
        {
            objOrder.OrdersId = lblOrdersId.zToInt();
            objOrder.Update();

            Message = "Order Detail Change Sucessfully.";
        }
        else
        {
            objOrder.UsersId = ddlUser.zToInt();
            objOrder.OrderStatusId = CU.GetOrderStatusId((int)eStatusType.Draft, string.Empty);
            lblOrdersId.Text = objOrder.Insert().ToString();

            Message = "New Order Added Sucessfully.";
        }

        ManageProduct(null, eRepeaterOperation.Save);

        CU.SetOrderTransaction(lblOrdersId.zToInt().Value);

        CU.ZMessage(eMsgType.Success, string.Empty, Message);

        return true;
    }

    #region Product

    private bool ManageProduct(int? PK, eRepeaterOperation RepeaterOperation)
    {
        #region Check Same Item

        //if (RepeaterOperation == eRepeaterOperation.Validate)
        //{
        //    var lstCheckItemId = new List<int>();
        //    foreach (RepeaterItem Item in rptOrderProduct.Items)
        //    {
        //        var ddlProductItem = Item.FindControl("ddlProductItem") as DropDownList;
        //        if (ddlProductItem.zIsSelect())
        //        {
        //            if (lstCheckItemId.Contains(ddlProductItem.zToInt().Value))
        //            {
        //                ddlProductItem.Focus();
        //                CU.ZMessage(eMsgType.Error, string.Empty, "This Item is Repeat in Same Order Please Marge Same Product.");
        //                return false;
        //            }

        //            lstCheckItemId.Add(ddlProductItem.zToInt().Value);
        //        }
        //    }
        //}

        #endregion

        var lstOrderProduct = new List<disOrderProduct>();
        var dtOrderProduct = new DataTable();

        int OrdersId = lblOrdersId.zToInt().HasValue ? lblOrdersId.zToInt().Value : 0;

        if (RepeaterOperation == eRepeaterOperation.Save)
        {
            dtOrderProduct = new OrderProduct() { OrdersId = OrdersId, }.Select(new OrderProduct() { OrderProductId = 0 });
        }

        if (RepeaterOperation == eRepeaterOperation.Select)
        {
            #region Get Data From DB

            var dt = new Query()
            {
                OrdersId = OrdersId,
            }.Select(eSP.qry_OrderProduct);

            foreach (DataRow dr in dt.Rows)
            {
                lstOrderProduct.Add(new disOrderProduct()
                {
                    ProductId = dr[CS.ProductId].zToInt().Value,
                    ItemId = dr[CS.ItemId].zToInt().HasValue ? dr[CS.ItemId].zToInt().Value : 0,
                    Quantity = dr[CS.Quantity].zToInt().HasValue ? dr[CS.Quantity].zToInt().Value : 0,
                    PurchasePrice = dr[CS.PurchasePrice].zToDecimal().HasValue ? dr[CS.PurchasePrice].zToDecimal().Value : 0,
                    UserPrice = dr[CS.UserPrice].zToDecimal().HasValue ? dr[CS.UserPrice].zToDecimal().Value : 0,
                    SalePrice = dr[CS.SalePrice].zToDecimal().HasValue ? dr[CS.SalePrice].zToDecimal().Value : 0,
                    Weight = dr[CS.Weight].zToDecimal().HasValue ? dr[CS.Weight].zToDecimal().Value : 0,
                    ProductImageId = dr[CS.ProductImageId].zToInt().HasValue ? dr[CS.ProductImageId].zToInt().Value : 0,
                    Description = dr[CS.Description].ToString(),
                    eStockStatus = dr[CS.eStockStatus].zToInt(),
                    StockNote = dr[CS.StockNote].ToString(),
                });
            }

            #endregion
        }
        else
        {
            #region Manage Data 

            foreach (RepeaterItem Item in rptOrderProduct.Items)
            {
                var lblPK = Item.FindControl("lblPK") as Label;

                if (RepeaterOperation == eRepeaterOperation.Remove && PK == lblPK.zToInt())
                    continue;

                var ddlProduct = Item.FindControl("ddlProduct") as DropDownList;
                var ddlProductItem = Item.FindControl("ddlProductItem") as DropDownList;
                var txtProductDescription = Item.FindControl("txtProductDescription") as TextBox;
                var txtQuantity = Item.FindControl("txtQuantity") as TextBox;
                var lblProductPurchasePrice = Item.FindControl("lblProductPurchasePrice") as Label;
                var lblProductUserPrice = Item.FindControl("lblProductUserPrice") as Label;
                var txtProductSalePrice = Item.FindControl("txtProductSalePrice") as TextBox;
                var lblProductWeight = Item.FindControl("lblProductWeight") as Label;
                var lbleStockStatus = Item.FindControl("lbleStockStatus") as Label;
                var lblStockNote = Item.FindControl("lblStockNote") as Label;

                var rptProductImage = Item.FindControl("rptProductImage") as Repeater;


                if (RepeaterOperation == eRepeaterOperation.Add || RepeaterOperation == eRepeaterOperation.Refresh
                || RepeaterOperation == eRepeaterOperation.Remove)
                {
                    #region Get Old Data

                    #region Product Image

                    int ProductImageId = 0;
                    foreach (RepeaterItem item in rptProductImage.Items)
                    {
                        var rdoProductImage = item.FindControl("rdoProductImage") as RadioButton;
                        if (rdoProductImage.Checked)
                        {
                            var lblProductImageId = item.FindControl("lblProductImageId") as Label;
                            ProductImageId = lblProductImageId.zToInt().Value;
                        }
                    }

                    #endregion

                    lstOrderProduct.Add(new disOrderProduct()
                    {
                        ProductId = ddlProduct.zToInt().Value,
                        ItemId = ddlProductItem.zToInt().Value,
                        Quantity = txtQuantity.zToInt().HasValue ? txtQuantity.zToInt().Value : 0,
                        PurchasePrice = lblProductPurchasePrice.zToDecimal().HasValue ? lblProductPurchasePrice.zToDecimal().Value : 0,
                        UserPrice = lblProductUserPrice.zToDecimal().HasValue ? lblProductUserPrice.zToDecimal().Value : 0,
                        SalePrice = txtProductSalePrice.zToDecimal().HasValue ? txtProductSalePrice.zToDecimal().Value : 0,
                        Weight = lblProductWeight.zToDecimal().HasValue ? lblProductWeight.zToDecimal().Value : 0,
                        Description = txtProductDescription.Text,
                        eStockStatus = lbleStockStatus.zToInt(),
                        StockNote = lblStockNote.Text,
                        ProductImageId = ProductImageId,
                    });

                    #endregion
                }
                else if (RepeaterOperation == eRepeaterOperation.Validate)
                {
                    #region Validate

                    if (ddlProduct.zIsSelect())
                    {
                        if (!txtQuantity.zIsInteger(false))
                        {
                            txtQuantity.Focus();
                            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Quantity");
                            return false;
                        }

                        if (!txtProductSalePrice.zIsDecimal(false))
                        {
                            txtProductSalePrice.Focus();
                            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Price");
                            return false;
                        }

                        if (!ddlProductItem.zIsSelect())
                        {
                            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Item");
                            ddlProductItem.Focus();
                            return false;
                        }

                        #region Product Image

                        if (rptProductImage.Items.Count > 0)
                        {
                            bool IsSelected = false;
                            foreach (RepeaterItem item in rptProductImage.Items)
                            {
                                var rdoProductImage = item.FindControl("rdoProductImage") as RadioButton;
                                if (rdoProductImage.Checked)
                                {
                                    IsSelected = true;
                                    break;
                                }
                            }

                            if (!IsSelected)
                            {
                                CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Product Image");
                                ddlProduct.Focus();
                                return false;
                            }
                        }

                        #endregion
                    }

                    #endregion
                }
                else if (RepeaterOperation == eRepeaterOperation.Save)
                {
                    #region Save Data

                    if (ddlProduct.zIsSelect())
                    {
                        #region Product Image

                        int ProductImageId = 0;
                        foreach (RepeaterItem item in rptProductImage.Items)
                        {
                            var rdoProductImage = item.FindControl("rdoProductImage") as RadioButton;
                            if (rdoProductImage.Checked)
                            {
                                var lblProductImageId = item.FindControl("lblProductImageId") as Label;
                                ProductImageId = lblProductImageId.zToInt().Value;
                            }
                        }

                        #endregion

                        var objOrderProduct = new OrderProduct()
                        {
                            OrderProductId = dtOrderProduct.Rows.Count > 0 ? dtOrderProduct.Rows[0][CS.OrderProductId].zToInt() : (int?)null,
                            OrdersId = OrdersId,
                            ProductId = ddlProduct.zToInt(),
                            ItemId = ddlProductItem.zToInt(),
                            Quantity = txtQuantity.zToInt(),
                            PurchasePrice = lblProductPurchasePrice.zToDecimal(),
                            UserPrice = lblProductUserPrice.zToDecimal(),
                            SalePrice = txtProductSalePrice.zToDecimal(),
                            Weight = lblProductWeight.zToDecimal(),
                            ProductImageId = ProductImageId,
                            Description = txtProductDescription.Text,
                        };

                        if (objOrderProduct.OrderProductId.HasValue)
                            dtOrderProduct.Rows.RemoveAt(0);

                        objOrderProduct.OrderProductId = CU.AdjustStockOrderProduct(objOrderProduct);
                    }

                    #endregion
                }
            }

            #endregion
        }

        if (RepeaterOperation == eRepeaterOperation.Add || RepeaterOperation == eRepeaterOperation.Refresh
            || RepeaterOperation == eRepeaterOperation.Remove || RepeaterOperation == eRepeaterOperation.Select)
        {
            #region Manage And Bind Data

            if (RepeaterOperation == eRepeaterOperation.Add || lstOrderProduct.Count == 0)
                lstOrderProduct.Add(new disOrderProduct() { Quantity = 1 });

            //dtProduct = new Query() { eStatus = (int)eStatus.Active, OrganizationId = lblOrganizationId.zToInt() }.Select(eSP.qry_Product);
            dtProduct = new Product() { eStatus = (int)eStatus.Active, OrganizationId = lblOrganizationId.zToInt() }.Select(new Product() { ProductId = 0, ProductCode = "" });
            var drProduct = dtProduct.NewRow();
            drProduct[CS.ProductId] = "0";
            drProduct[CS.ProductCode] = "-- Select Product --";
            dtProduct.Rows.InsertAt(drProduct, 0);

            var lstProductId = new List<int>();
            foreach (disOrderProduct objOrderProduct in lstOrderProduct)
                lstProductId.Add(objOrderProduct.ProductId);

            dtProductItem = new Query()
            {
                eStatus = (int)eStatus.Active,
                ProductIdIn = CU.GetParaIn(lstProductId, true)
            }.Select(eSP.qry_Item);

            var lstProductImageType = new List<int>();
            lstProductImageType.Add((int)eProductImageType.Modeling);
            lstProductImageType.Add((int)eProductImageType.Original);

            dtProductImage = new Query()
            {
                eProductImageTypeIn = CU.GetParaIn(lstProductImageType, false),
                ProductIdIn = CU.GetParaIn(lstProductId, true),
                eStatus = (int)eStatus.Active
            }.Select(eSP.qry_ProductImage);

            rptOrderProduct.DataSource = lstOrderProduct;
            rptOrderProduct.DataBind();

            #endregion
        }
        else if (RepeaterOperation == eRepeaterOperation.Save)
        {
            #region Save Data

            foreach (DataRow dr in dtOrderProduct.Rows)
                CU.OrderProductDelete(dr[CS.OrderProductId].zToInt().Value);

            #endregion
        }

        return true;
    }

    protected void rptOrderProduct_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var lblPK = e.Item.FindControl("lblPK") as Label;
        var ddlProductItem = e.Item.FindControl("ddlProductItem") as DropDownList;
        var ddlProduct = e.Item.FindControl("ddlProduct") as DropDownList;
        var txtProductDescription = e.Item.FindControl("txtProductDescription") as TextBox;
        var txtQuantity = e.Item.FindControl("txtQuantity") as TextBox;
        var lblProductWeight = e.Item.FindControl("lblProductWeight") as Label;
        var lblProductPurchasePrice = e.Item.FindControl("lblProductPurchasePrice") as Label;
        var lblProductUserPrice = e.Item.FindControl("lblProductUserPrice") as Label;
        var lblProductViewUserPrice = e.Item.FindControl("lblProductViewUserPrice") as Label;
        var txtProductSalePrice = e.Item.FindControl("txtProductSalePrice") as TextBox;
        var lblProductAmount = e.Item.FindControl("lblProductAmount") as Label;
        var lbleStockStatus = e.Item.FindControl("lbleStockStatus") as Label;
        var lblStockNote = e.Item.FindControl("lblStockNote") as Label;
        var lblOrderStatus = e.Item.FindControl("lblOrderStatus") as Label;

        var rptProductImage = e.Item.FindControl("rptProductImage") as Repeater;


        disOrderProduct objOrderProduct = (disOrderProduct)((RepeaterItem)e.Item).DataItem;

        lblPK.Text = e.Item.ItemIndex.ToString();

        ddlProduct.DataSource = dtProduct;
        ddlProduct.DataValueField = CS.ProductId;
        ddlProduct.DataTextField = CS.ProductCode;
        ddlProduct.DataBind();

        ddlProduct.SelectedValue = objOrderProduct.ProductId.ToString();
        txtProductDescription.Text = objOrderProduct.Description;
        txtQuantity.Text = objOrderProduct.Quantity.ToString();
        lblProductWeight.Text = "Weight: " + objOrderProduct.Weight.ToString().Replace(".00", "") + "gm";
        lblProductPurchasePrice.Text = objOrderProduct.PurchasePrice.ToString();
        lblProductUserPrice.Text = objOrderProduct.UserPrice.ToString();
        lblProductViewUserPrice.Text = "P.Price: " + objOrderProduct.UserPrice.ToString().Replace(".00", "");

        txtProductSalePrice.Text = objOrderProduct.SalePrice.ToString().Replace(".00", "");

        lblProductAmount.Text = (objOrderProduct.Quantity * objOrderProduct.SalePrice).ToString();

        lbleStockStatus.Text = objOrderProduct.eStockStatus.HasValue ? objOrderProduct.eStockStatus.ToString() : string.Empty;
        lblStockNote.Text = objOrderProduct.StockNote;
        lblOrderStatus.Visible = false;

        if (objOrderProduct.eStockStatus.HasValue && objOrderProduct.eStockStatus > 0 && objOrderProduct.eStockStatus != (int)eStockStatus.InStock)
        {
            lblOrderStatus.Text = CU.GetDescription((eStockStatus)objOrderProduct.eStockStatus);
            if (!objOrderProduct.StockNote.zIsNullOrEmpty())
                lblOrderStatus.Text += " - " + objOrderProduct.StockNote;

            lblOrderStatus.Visible = true;
        }

        #region Product Item

        var dtItem = dtProductItem.Clone();
        var dr = dtProductItem.Select(CS.ProductId + "=" + ddlProduct.zToInt().Value);
        if (dr.Length > 0)
            dtItem = dr.CopyToDataTable();

        CU.FillDropdown(ref ddlProductItem, dtItem, "-- Select Item --", CS.ItemId, CS.ItemName);
        ddlProductItem.SelectedValue = objOrderProduct.ItemId.ToString();

        #endregion

        #region Product Image

        LoadProductImage(ddlProduct.zToInt().Value, dtProductImage, rptProductImage);

        if (objOrderProduct.ProductImageId > 0)
        {
            foreach (RepeaterItem item in rptProductImage.Items)
            {
                var lblProductImageId = item.FindControl("lblProductImageId") as Label;
                if (lblProductImageId.zToInt() == objOrderProduct.ProductImageId)
                {
                    var rdoProductImage = item.FindControl("rdoProductImage") as RadioButton;
                    rdoProductImage.Checked = true;
                }
            }
        }

        #endregion

        if (IsSetFocus)
            ddlProduct.Focus();
    }


    protected void lnkAddNewProduct_OnClick(object sender, EventArgs e)
    {
        IsSetFocus = true;
        ManageProduct(null, eRepeaterOperation.Add);
    }

    protected void lnkDeleteProduct_OnClick(object sender, EventArgs e)
    {
        var lnk = ((LinkButton)sender);
        var lblPK = lnk.Parent.FindControl("lblPK") as Label;

        ManageProduct(lblPK.zToInt(), eRepeaterOperation.Remove);
    }


    private void SetShipCharge()
    {
        decimal Weight = 0;
        int Quantity = 0;

        foreach (RepeaterItem Item in rptOrderProduct.Items)
        {
            var ddlProduct = Item.FindControl("ddlProduct") as DropDownList;
            var txtQuantity = Item.FindControl("txtQuantity") as TextBox;
            var lblProductWeight = Item.FindControl("lblProductWeight") as Label;
            if (ddlProduct.zIsSelect() && txtQuantity.zIsInteger(false))
            {
                if (lblProductWeight.zIsDecimal(false))
                    Weight += lblProductWeight.zToDecimal().Value * txtQuantity.zToInt().Value;
                else
                    Weight += (decimal)0.5;

                Quantity += txtQuantity.zToInt().Value;
            }
        }

        lblWeight.Text = Weight.ToString();

        decimal ShipCharge = 0, FirmShipCharge = 0;

        if (Weight > 0)
        {
            var lstCourierId = new List<int>();
            foreach (ListItem ltItem in ddlCourier.Items)
                lstCourierId.Add(ltItem.Value.zToInt().Value);

            var dtShipCharge = new Query()
            {
                CourierIdIn = CU.GetParaIn(lstCourierId, false),
                FirmId = lblFirmId.zToInt(),
                eCourierType = (!rdoCOD.Checked && !rdoPrepaid.Checked) ? 0 : (rdoCOD.Checked ? (int)eCourierType.COD : (int)eCourierType.Prepaid),
                eZone = ddlZone.zToInt(),
            }.Select(eSP.qry_ShippingCharge);

            bool SCQuantityWise = false, SCWeightWise = false; //lblOrganizationId.zToInt() == (int)eOrganisation.OCTFIS;
            if (SCQuantityWise)
            {
                #region Quantity Wise

                int CourierId = 0;
                decimal CheckCharge = 0;

                foreach (int Id in lstCourierId)
                {
                    var dr = dtShipCharge.Select(CS.CourierId + " = " + Id + " AND " + CS.ShipCharge + " > 0 AND " + CS.FirmShipCharge + " > 0", CS.Weight + ", " + CS.FirmShipCharge);
                    if (dr.Length >= Quantity)
                    {
                        int ItemIndex = Quantity - 1;

                        if (CheckCharge == 0 || CheckCharge > dr[ItemIndex][CS.ShipCharge].zToDecimal().Value)
                        {
                            CourierId = dr[ItemIndex][CS.CourierId].zToInt().Value;
                            CheckCharge = dr[ItemIndex][CS.ShipCharge].zToDecimal().Value;
                        }
                    }
                }

                if (CourierId == 0)
                {
                    CheckCharge = 0;
                    foreach (int Id in lstCourierId)
                    {
                        var dr = dtShipCharge.Select(CS.CourierId + " = " + Id + " AND " + CS.ShipCharge + " > 0 AND " + CS.FirmShipCharge + " > 0", CS.Weight + ", " + CS.FirmShipCharge);
                        if (dr.Length > 0)
                        {
                            if (CheckCharge == 0 || CheckCharge > dr[0][CS.ShipCharge].zToDecimal().Value)
                            {
                                CourierId = dr[0][CS.CourierId].zToInt().Value;
                                CheckCharge = dr[0][CS.ShipCharge].zToDecimal().Value;
                            }
                        }
                    }
                }
                try { ddlCourier.SelectedValue = lblCourierIdMenual.Text; } catch { }
                if (!ddlCourier.zIsSelect() || ddlCourier.zToInt() != lblCourierIdMenual.zToInt())
                {
                    var drShipCharge = dtShipCharge.Select(CS.CourierId + " = " + CourierId + " AND " + CS.ShipCharge + " > 0 AND " + CS.FirmShipCharge + " > 0", CS.Weight);
                    if (drShipCharge.Length > 0)
                    {
                        int ItemIndex = Quantity - 1;
                        if (drShipCharge.Length < Quantity)
                            ItemIndex = drShipCharge.Length - 1;

                        ddlCourier.SelectedValue = drShipCharge[ItemIndex][CS.CourierId].ToString();

                        ShipCharge = drShipCharge[ItemIndex][CS.ShipCharge].zToDecimal().Value;
                        FirmShipCharge = drShipCharge[ItemIndex][CS.FirmShipCharge].zToDecimal().Value;
                    }
                }
                #endregion
            }
            else if (SCWeightWise)
            {
                #region Weight Wise

                try { ddlCourier.SelectedValue = lblCourierIdMenual.Text; } catch { }
                if (!ddlCourier.zIsSelect() || ddlCourier.zToInt() != lblCourierIdMenual.zToInt())
                {
                    var drShipCharge = dtShipCharge.Select(CS.Weight + " >= " + Weight + " AND " + CS.ShipCharge + " > 0 AND " + CS.FirmShipCharge + " > 0", CS.Weight + ", " + CS.FirmShipCharge);
                    if (drShipCharge.Length > 0)
                    {
                        ddlCourier.SelectedValue = drShipCharge[0][CS.CourierId].ToString();

                        ShipCharge = drShipCharge[0][CS.ShipCharge].zToDecimal().Value;
                        FirmShipCharge = drShipCharge[0][CS.FirmShipCharge].zToDecimal().Value;
                    }
                }

                #endregion
            }
            else
            {
                #region Serial No Wise

                try { ddlCourier.SelectedValue = lblCourierIdMenual.Text; } catch { }
                if (!ddlCourier.zIsSelect() || ddlCourier.zToInt() != lblCourierIdMenual.zToInt())
                {
                    var drShipCharge = dtShipCharge.Select(CS.ShipCharge + " > 0 AND " + CS.FirmShipCharge + " > 0", CS.SerialNo);
                    if (drShipCharge.Length > 0)
                    {
                        ddlCourier.SelectedValue = drShipCharge[0][CS.CourierId].ToString();

                        ShipCharge = drShipCharge[0][CS.ShipCharge].zToDecimal().Value;
                        FirmShipCharge = drShipCharge[0][CS.FirmShipCharge].zToDecimal().Value;
                    }
                }

                #endregion
            }
        }

        lblShipCharge.Text = ShipCharge.ToString().Replace(".00", "");
        txtFirmShipCharge.Text = txtCustomerShipCharge.Text = FirmShipCharge.ToString().Replace(".00", "");
    }

    private void SetZone()
    {
        if (txtState.Text.ToLower() == "gujarat")
            ddlZone.SelectedValue = ((int)eZone.InState).ToString();
        else
            ddlZone.SelectedValue = ((int)eZone.OutOfState).ToString();
    }

    private void SetProductPrice(RepeaterItem Item)
    {
        var ddlProduct = Item.FindControl("ddlProduct") as DropDownList;
        var txtQuantity = Item.FindControl("txtQuantity") as TextBox;
        var lblProductWeight = Item.FindControl("lblProductWeight") as Label;
        var lblProductPurchasePrice = Item.FindControl("lblProductPurchasePrice") as Label;
        var lblProductUserPrice = Item.FindControl("lblProductUserPrice") as Label;
        var lblProductViewUserPrice = Item.FindControl("lblProductViewUserPrice") as Label;
        var txtProductSalePrice = Item.FindControl("txtProductSalePrice") as TextBox;
        var lblProductAmount = Item.FindControl("lblProductAmount") as Label;
        var lbleStockStatus = Item.FindControl("lbleStockStatus") as Label;
        var lblStockNote = Item.FindControl("lblStockNote") as Label;
        var lblOrderStatus = Item.FindControl("lblOrderStatus") as Label;

        lbleStockStatus.Text = string.Empty;
        lblStockNote.Text = string.Empty;
        lblOrderStatus.Visible = false;

        int ProductId = ddlProduct.zToInt().Value;
        if (ProductId == 0)
        {
            lblProductWeight.Text = "0";
            lblProductPurchasePrice.Text = "0";
            lblProductUserPrice.Text = "0";
            lblProductViewUserPrice.Text = "P.Price: 0";
            txtProductSalePrice.Text = "0";
            lblProductAmount.Text = "0";
        }
        else
        {
            var objProduct = new Product() { ProductId = ProductId }.SelectList<Product>()[0];

            lblProductWeight.Text = "Weight: " + (objProduct.Weight.HasValue ? objProduct.Weight.ToString().Replace(".00", "") : "0") + "gm";
            lblProductPurchasePrice.Text = objProduct.PurchasePrice.HasValue ? objProduct.PurchasePrice.ToString() : "0";

            decimal ProductPrice = CU.GetProductPrice(lblFirmId.zToInt().Value, ProductId);
            decimal UserPrice = GetUserProductPrice(objProduct, ddlUser.zToInt().Value);
            if (UserPrice == 0)
                UserPrice = ProductPrice;

            lblProductUserPrice.Text = UserPrice.ToString();
            lblProductViewUserPrice.Text = "P.Price: " + UserPrice.ToString().Replace(".00", "");

            txtProductSalePrice.Text = ProductPrice.ToString();

            if (txtQuantity.zToInt() > 0 && txtProductSalePrice.zToDecimal() > 0)
                lblProductAmount.Text = (txtQuantity.zToInt().Value * txtProductSalePrice.zToDecimal().Value).ToString();
            else
                lblProductAmount.Text = "0";

            if (objProduct.eStockStatus.HasValue && objProduct.eStockStatus > 0)
            {
                lbleStockStatus.Text = objProduct.eStockStatus.ToString();
                lblStockNote.Text = objProduct.StockNote;
                if (objProduct.eStockStatus != (int)eStockStatus.InStock)
                {
                    lblOrderStatus.Text = CU.GetDescription((eStockStatus)objProduct.eStockStatus);
                    if (!objProduct.StockNote.zIsNullOrEmpty())
                        lblOrderStatus.Text += " - " + objProduct.StockNote;

                    lblOrderStatus.Visible = true;
                }
            }
        }
    }

    private decimal GetUserProductPrice(Product objProduct, int UsersId)
    {
        var objUser = new Users() { UsersId = UsersId }.SelectList<Users>()[0];

        decimal UserPrice = 0;
        if (objUser.PriceListId > 0)
        {
            var lstPriceListValue = new PriceListValue() { ProductId = objProduct.ProductId, PriceListId = objUser.PriceListId, }.SelectList<PriceListValue>();
            if (lstPriceListValue.Count > 0 && lstPriceListValue[0].Price.HasValue)
                UserPrice = lstPriceListValue[0].Price.Value;
        }

        if (UserPrice == 0)
        {
            eDesignation Designation = CU.GetDesiToeDesi(objUser.DesignationId.Value);
            if (Designation == eDesignation.SystemAdmin || Designation == eDesignation.Admin)
                UserPrice = objProduct.PurchasePrice.HasValue ? objProduct.PurchasePrice.Value : 0;
        }

        return UserPrice;
    }

    protected void ddlProduct_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        var ddl = ((DropDownList)sender);

        var Item = (RepeaterItem)ddl.Parent;

        SetProductPrice(Item);

        var txtQuantity = Item.FindControl("txtQuantity") as TextBox;
        var ddlProductItem = Item.FindControl("ddlProductItem") as DropDownList;
        var rptProductImage = Item.FindControl("rptProductImage") as Repeater;


        var dtPV = new DataTable();

        int ProductId = ddl.zToInt().Value;
        if (ProductId > 0)
        {
            dtPV = new Query()
            {
                ProductId = ProductId
            }.Select(eSP.qry_Item);
        }
        else
        {
            dtPV.Columns.Add(CS.ItemId, typeof(int));
            dtPV.Columns.Add(CS.ItemName, typeof(string));
        }

        CU.FillDropdown(ref ddlProductItem, dtPV, "-- Select Item --", CS.ItemId, CS.ItemName);

        txtQuantity.Focus();

        SetShipCharge();


        var lstProductImageType = new List<int>();
        lstProductImageType.Add((int)eProductImageType.Modeling);
        lstProductImageType.Add((int)eProductImageType.Original);

        var dtProductImage = new Query()
        {
            ProductId = ProductId,
            eStatus = (int)eStatus.Active,
            eProductImageTypeIn = CU.GetParaIn(lstProductImageType, false)
        }.Select(eSP.qry_ProductImage);
        LoadProductImage(ProductId, dtProductImage, rptProductImage);
    }

    protected void txtQuantity_OnTextChanged(object sender, EventArgs e)
    {
        var txt = ((TextBox)sender);
        var Item = (RepeaterItem)txt.Parent;

        var txtProductSalePrice = Item.FindControl("txtProductSalePrice") as TextBox;

        txtProductSalePrice.Focus();
        SetShipCharge();
    }

    #region Product Image

    private void LoadProductImage(int ProductId, DataTable dt, Repeater rptProductImage)
    {
        bool IsSetNullData = true;
        if (ProductId > 0 && dt.Rows.Count > 0)
        {
            var dr = dt.Select(CS.ProductId + " = " + ProductId);
            if (dr.Length > 0)
            {
                rptProductImage.DataSource = dr.CopyToDataTable();
                rptProductImage.DataBind();

                IsSetNullData = false;
            }
        }

        if (IsSetNullData)
        {
            rptProductImage.DataSource = null;
            rptProductImage.DataBind();
        }
    }

    protected void rptProductImage_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var lblProductImageId = e.Item.FindControl("lblProductImageId") as Label;
        var imgProductImage = e.Item.FindControl("imgProductImage") as Image;
        var rdoProductImage = e.Item.FindControl("rdoProductImage") as RadioButton;

        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;

        lblProductImageId.Text = dataItem[CS.ProductImageId].ToString();
        imgProductImage.ImageUrl = CU.GetFilePath(true, ePhotoSize.P50, eFolder.ProductCImage, dataItem[CS.ProductImageId].ToString(), true);

        string ProductImageType = ((eProductImageType)dataItem[CS.eProductImageType].zToInt()).ToString()[0].ToString();
        rdoProductImage.Text = ProductImageType + "-" + dataItem[CS.SerialNo].ToString();//if not add space then nopt display radiobutton
    }

    #endregion

    #endregion

    #region Customer

    private void LoadCustomerDetail(int CustomerId)
    {
        if (IsEditModeCust())
        {
            var objCustomer = new Customer() { CustomerId = CustomerId, }.SelectList<Customer>()[0];

            txtName.Text = objCustomer.Name;
            txtWhatsappNo.Text = objCustomer.WhatsAppNo;
            txtMobileNo.Text = objCustomer.MobileNo;
            txtAddress.Text = objCustomer.Address;
            txtPincode.Text = objCustomer.Pincode;
            txtCity.Text = objCustomer.CityName;
            txtState.Text = objCustomer.StateName;
            txtCountry.Text = objCustomer.CountryName;
        }
        else
        {
            txtName.Text = txtWhatsappNo.Text = txtMobileNo.Text = string.Empty;
            txtAddress.Text = txtPincode.Text = txtCity.Text = txtState.Text = string.Empty;
            txtCountry.Text = "India";
        }

        LoadCourierddl();
        SetZone();
        SetService(null);
    }

    private bool IsEditModeCust()
    {
        return ddlCustomer.zIsSelect();
    }

    private bool IsValidateCust()
    {
        if (!txtWhatsappNo.zIsMobile())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Whatsapp no.");
            txtWhatsappNo.Focus();
            return false;
        }

        if (!txtMobileNo.zIsMobile())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Mobile no.");
            txtMobileNo.Focus();
            return false;
        }

        if (txtName.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Name");
            txtName.Focus();
            return false;
        }

        var dtCustomerWN = new Query()
        {
            eStatusNot = (int)eStatus.Delete,
            UsersId = ddlUser.zToInt(),
            WhatsAppNo = txtWhatsappNo.Text.Trim(),
        }.Select(eSP.qry_Customer);

        if (dtCustomerWN.Rows.Count > 0 && dtCustomerWN.Rows[0][CS.CustomerId].zToInt() != ddlCustomer.zToInt())
        {
            string Status = dtCustomerWN.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
            CU.ZMessage(eMsgType.Error, string.Empty, "This Whatsapp no is already exist" + Status + ".");
            txtWhatsappNo.Focus();
            return false;
        }

        var dtCustomer = new Query()
        {
            eStatusNot = (int)eStatus.Delete,
            UsersId = ddlUser.zToInt(),
            MobileNo = txtMobileNo.Text.Trim(),
        }.Select(eSP.qry_Customer);

        if (dtCustomer.Rows.Count > 0 && dtCustomer.Rows[0][CS.CustomerId].zToInt() != ddlCustomer.zToInt())
        {
            string Status = dtCustomer.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
            CU.ZMessage(eMsgType.Error, string.Empty, "This mobile no is already exist" + Status + ".");
            txtMobileNo.Focus();
            return false;
        }

        string Message = string.Empty;
        if (CheckSameCustinDefferantUser(txtWhatsappNo.Text, ref Message))
        {
            CU.ZMessage(eMsgType.Error, string.Empty, Message);
            return false;
        }

        if (txtAddress.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Address");
            txtAddress.Focus();
            return false;
        }

        if (txtPincode.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Pincode");
            txtPincode.Focus();
            return false;
        }

        if (txtCity.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter City");
            txtCity.Focus();
            return false;
        }

        if (txtState.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter State");
            txtState.Focus();
            return false;
        }

        if (txtCountry.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Country");
            txtCountry.Focus();
            return false;
        }

        return true;
    }

    private bool SaveDataCust()
    {
        if (!IsValidateCust())
            return false;

        string Message = string.Empty;

        var objCustomer = new Customer()
        {
            Name = txtName.Text.zFirstCharToUpper(),
            MobileNo = txtMobileNo.Text.Trim(),
            WhatsAppNo = txtWhatsappNo.Text.Trim(),
            Address = txtAddress.Text,
            Pincode = txtPincode.Text,
            CityName = txtCity.Text.zFirstCharToUpper(),
            StateName = txtState.Text.zFirstCharToUpper(),
            CountryName = txtCountry.Text.zFirstCharToUpper(),
        };

        var lstCustomer = new Customer()
        {
            WhatsAppNo = objCustomer.WhatsAppNo,
            UsersId = ddlUser.zToInt(),
            eStatus = (int)eStatus.Active,
        }.SelectList<Customer>();

        int CustomerId = lstCustomer.Count > 0 ? lstCustomer[0].CustomerId.Value : 0;
        if (CustomerId > 0)
        {
            objCustomer.CustomerId = CustomerId;
            objCustomer.Update();
        }
        else
        {
            objCustomer.eStatus = (int)eStatus.Active;
            objCustomer.UsersId = ddlUser.zToInt();
            CustomerId = objCustomer.Insert();
        }

        lblCustomerId.Text = CustomerId.ToString();

        return true;
    }

    private bool CheckSameCustinDefferantUser(string MobileNo, ref string Message)
    {
        Message = string.Empty;
        if (CU.GetNameValue(eNameValue.SameCustomerDifferantUser).zToInt() == (int)eYesNo.No)
        {
            var dtCustomer = new Query()
            {
                WhatsAppNo = MobileNo.Trim(),
                UsersIdNot = ddlUser.zToInt(),
                eStatus = (int)eStatus.Active,
            }.Select(eSP.qry_Customer);

            if (dtCustomer.Rows.Count > 0)
            {
                Message = "This Customer already Exist in user " + dtCustomer.Rows[0][CS.UserName].ToString();
                return true;
            }
        }

        return false;
    }

    protected void txtWhatsappNo_OnTextChanged(object sender, EventArgs e)
    {
        if (txtWhatsappNo.zIsMobile())
        {
            var dtCustomer = new Customer()
            {
                WhatsAppNo = txtWhatsappNo.Text.Trim(),
                UsersId = ddlUser.zToInt(),
                eStatus = (int)eStatus.Active,
            }.Select(new Customer() { CustomerId = 0 });

            if (dtCustomer.Rows.Count > 0)
            {
                try { ddlCustomer.SelectedValue = dtCustomer.Rows[0][CS.CustomerId].ToString(); } catch { }
                LoadCustomerDetail(dtCustomer.Rows[0][CS.CustomerId].zToInt().Value);
            }

            string Message = string.Empty;
            if (CheckSameCustinDefferantUser(txtWhatsappNo.Text, ref Message))
            {
                CU.ZMessage(eMsgType.Info, string.Empty, Message);
            }
        }

        txtWhatsappNo.Focus();
    }

    protected void txtPincode_OnTextChanged(object sender, EventArgs e)
    {
        DataTable dtSA = null;
        if (!txtPincode.zIsNullOrEmpty())
        {
            dtSA = new Query() { OrganizationId = lblOrganizationId.zToInt(), Pincode = txtPincode.Text, eStatus = (int)eStatus.Active }.Select(eSP.qry_ServiceAvailability);
            if (dtSA.Rows.Count > 0)
            {
                try
                {
                    txtCity.Text = dtSA.Rows[0][CS.CityName].ToString();
                    txtState.Text = dtSA.Rows[0][CS.StateName].ToString();
                    txtCountry.Text = dtSA.Rows[0][CS.CountryName].ToString();
                }
                catch { }

            }
            else
            {
                txtCity.Text = txtState.Text = string.Empty;
                txtCountry.Text = "India";
            }
        }

        ddlCustomer_OnSelectedIndexChanged(null, null);

        SetService(dtSA);

        txtPincode.Focus();
    }

    protected void txtState_OnTextChanged(object sender, EventArgs e)
    {
        SetZone();
        SetShipCharge();
        txtState.Focus();
    }

    private void SetService(DataTable dtService)
    {
        bool IsCOD = false, IsPrepaid = false, IsReversePickup = false, IsPickup = false;

        if (dtService == null && !txtPincode.zIsNullOrEmpty())
            dtService = new Query() { OrganizationId = lblOrganizationId.zToInt(), Pincode = txtPincode.Text, eStatus = (int)eStatus.Active }.Select(eSP.qry_ServiceAvailability);

        if (dtService != null && dtService.Rows.Count > 0)
        {
            var drService = dtService.Rows[0];
            IsCOD = drService[CS.eCOD].zToInt() == (int)eYesNo.Yes;
            IsPrepaid = drService[CS.ePrepaid].zToInt() == (int)eYesNo.Yes;
            IsReversePickup = drService[CS.eReversePickup].zToInt() == (int)eYesNo.Yes;
            IsPickup = drService[CS.ePickup].zToInt() == (int)eYesNo.Yes;

            divCOD.addClass(IsCOD ? "bg-success" : "bg-red");
            divPrepaid.addClass(IsPrepaid ? "bg-success" : "bg-purple");
            divReversePickup.addClass(IsReversePickup ? "bg-success" : "bg-red");
            divPickup.addClass(IsPickup ? "bg-success" : "bg-red");

            divCOD.removeClass(!IsCOD ? "bg-success" : "bg-red");
            divPrepaid.removeClass(!IsPrepaid ? "bg-success" : "bg-purple");
            divReversePickup.removeClass(!IsReversePickup ? "bg-success" : "bg-red");
            divPickup.removeClass(!IsPickup ? "bg-success" : "bg-red");

            lblCODStatus.Text = IsCOD ? "Available" : "Unvailable";
            lblPrepaidStatus.Text = IsPrepaid ? "Available" : "Available";
            lblReversePickupStatus.Text = IsReversePickup ? "Available" : "Unvailable";
            lblPickupStatus.Text = IsPickup ? "Available" : "Unvailable";
        }

        pnlService.Visible = dtService != null && dtService.Rows.Count > 0;
    }

    #endregion


    protected void lnkSave_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            Response.Redirect("OrderView.aspx");
        }
    }

    protected void lnkSaveAndNew_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            lblOrdersId.Text = string.Empty;
            LoadOrderDetail();
        }
    }

    protected void lnkSaveAndPay_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            popupManageOrderPayment.SetOrderPaymentId = string.Empty;
            popupManageOrderPayment.LoadOrderPaymentDetail(lblOrdersId.zToInt().Value);
            popupOrderPayment.Show();
        }
    }

    protected void btnSaveOrderPayment_OnClick(object sender, EventArgs e)
    {
        Response.Redirect("OrderView.aspx");
    }

    protected void lnkCancel_OnClick(object sender, EventArgs e)
    {
        Response.Redirect("OrderView.aspx");
    }


    private class disOrderProduct
    {
        public disOrderProduct()
        {
        }

        public int ProductId;
        public int ItemId;
        public int Quantity;
        public decimal PurchasePrice;
        public decimal UserPrice;
        public decimal SalePrice;
        public decimal Weight;
        public string Description;
        public int ProductImageId;
        public int? eStockStatus;
        public string StockNote;
    }

}
