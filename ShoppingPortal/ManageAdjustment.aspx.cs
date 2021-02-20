using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;

public partial class ManageAdjustment : CompressorPage
{
    bool? IsAddEdit;
    bool IsSetFocus = false;

    DataTable dtProduct;

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
            lblOrganizationId.Text = CU.GetOrganizationId().ToString();
            CU.LoadDisplayPerPage(ref ddlRecordPerPage);

            txtFromDate.Text = IndianDateTime.Today.AddDays(-5).ToString(CS.ddMMyyyy);
            txtToDate.Text = IndianDateTime.Today.ToString(CS.ddMMyyyy);

            SetControl(eControl.ItemAdjustment);
        }

        Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

        try { grdItemAdjustment.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    #region Item Adjustment

    private DataTable GetItemAdjustmentDt(ePageIndex ePageIndex)
    {
        var objQuery = new Query()
        {
            MasterSearch = txtSearch.Text,
            OrganizationId = lblOrganizationId.zToInt(),
            FromDate = txtFromDate.zToDate().HasValue ? txtFromDate.zToDate() : (DateTime?)null,
            ToDate = txtToDate.zToDate().HasValue ? txtToDate.zToDate().Value.AddDays(1).AddSeconds(-1) : (DateTime?)null,
            eStatusNot = (int)eStatus.Delete,
        };

        #region Page Index

        int RecordPerPage = ddlRecordPerPage.zToInt().Value;
        int PageIndexTemp = PageIndex;

        CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref objQuery, ref txtGotoPageNo, ref lblCount);
        PageIndex = PageIndexTemp;

        #endregion Page Index

        return objQuery.Select(eSP.qry_ItemAdjustment);
    }

    private void LoadItemAdjustmentGrid(ePageIndex ePageIndex)
    {
        DataTable dtItemAdjustment = GetItemAdjustmentDt(ePageIndex);

        if (dtItemAdjustment.Rows.Count > 0)
            lblCount.Text = dtItemAdjustment.Rows[0][CS.TotalRecord].ToString();
        else
            lblCount.Text = "0";

        divPaging.Visible = (dtItemAdjustment.Rows.Count > 0);

        txtGotoPageNo.Text = PageIndex.ToString();

        ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

        grdItemAdjustment.DataSource = dtItemAdjustment;
        grdItemAdjustment.DataBind();

        try { grdItemAdjustment.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void CheckVisibleButton()
    {
        var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.Adjustment);

        lnkAdd.Visible = lnkEdit.Visible = objAuthority.IsAddEdit;
        lnkDelete.Visible = objAuthority.IsDelete;
    }


    protected void lnkAdd_OnClick(object sender, EventArgs e)
    {
        lblItemAdjustmentId.Text = string.Empty;
        LoadItemAdjustmentDetail();
        SetControl(eControl.ItemAdjustmentDetail);
    }

    protected void lnkEdit_OnClick(object sender, EventArgs e)
    {
        if (CU.GetAuthority(CU.GetUsersId(), eAuthority.Adjustment).IsAddEdit && (sender == null || grdItemAdjustment.zIsValidSelection(lblItemAdjustmentId, "chkSelect", CS.ItemAdjustmentId)))
        {
            if (new ItemAdjustment() { ItemAdjustmentId = lblItemAdjustmentId.zToInt(), OrdersId = 0 }.SelectCount() == 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Adjustment is Order Adjustment, Then It can not Edit.");
                return;
            }

            LoadItemAdjustmentDetail();
            SetControl(eControl.ItemAdjustmentDetail);
        }
    }

    protected void lnkEditItemAdjustment_OnClick(object sender, EventArgs e)
    {
        lblItemAdjustmentId.Text = ((LinkButton)sender).CommandArgument.ToString();
        lnkEdit_OnClick(null, null);
    }

    protected void lnkRefresh_OnClick(object sender, EventArgs e)
    {
        LoadItemAdjustmentGrid(ePageIndex.Custom);
    }

    protected void lnkDelete_OnClick(object sender, EventArgs e)
    {
        if (grdItemAdjustment.zIsValidSelection(lblItemAdjustmentId, "chkSelect", CS.ItemAdjustmentId))
        {
            if (new ItemAdjustment() { ItemAdjustmentId = lblItemAdjustmentId.zToInt(), OrdersId = 0 }.SelectCount() == 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Adjustment is Order Adjustment, Then It can not Delete.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Adjustment", "Are You Sure To Delete Adjustment?");
            popupConfirmation.Show();
        }
    }

    protected void Control_CheckedChanged(object sender, EventArgs e)
    {
        LoadItemAdjustmentGrid(ePageIndex.Custom);
        CheckVisibleButton();
    }

    protected void btnDelete_OnClick(object sender, EventArgs e)
    {
        var dtItemAdjustmentDetail = new ItemAdjustmentDetail() { ItemAdjustmentId = lblItemAdjustmentId.zToInt(), }.Select();
        foreach (DataRow drItemAdjustmentDetail in dtItemAdjustmentDetail.Rows)
            CU.AdjustStockDelete(drItemAdjustmentDetail[CS.ItemAdjustmentDetailId].zToInt().Value);

        new ItemAdjustment() { ItemAdjustmentId = lblItemAdjustmentId.zToInt(), }.Delete();
        CU.ZMessage(eMsgType.Success, string.Empty, "Adjustment Delete Successfully.");
        LoadItemAdjustmentGrid(ePageIndex.Custom);
    }


    protected void grdItemAdjustment_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!IsAddEdit.HasValue)
                IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.Adjustment).IsAddEdit;

            if (IsAddEdit.Value)
                e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdItemAdjustment, "Select$" + e.Row.RowIndex);

            DataRowView dataItem = (DataRowView)e.Row.DataItem;
            var lnkEditItemAdjustment = e.Row.FindControl("lnkEditItemAdjustment") as LinkButton;
            var ltrItemAdjustment = e.Row.FindControl("ltrItemAdjustment") as Literal;

            lnkEditItemAdjustment.Visible = IsAddEdit.Value;
            ltrItemAdjustment.Visible = !IsAddEdit.Value;

            lnkEditItemAdjustment.Text = ltrItemAdjustment.Text = dataItem[CS.ReferenceNo].ToString();
            lnkEditItemAdjustment.CommandArgument = dataItem[CS.ItemAdjustmentId].ToString();
        }
    }

    protected void grdItemAdjustment_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        lblItemAdjustmentId.Text = grdItemAdjustment.Rows[grdItemAdjustment.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdItemAdjustment, CS.ItemAdjustmentId)].Text;
        SetControl(eControl.ItemAdjustmentDetail);
    }


    #region Pagging


    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        LoadItemAdjustmentGrid(ePageIndex.Prev);
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        LoadItemAdjustmentGrid(ePageIndex.Next);
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        LoadItemAdjustmentGrid(ePageIndex.First);
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        LoadItemAdjustmentGrid(ePageIndex.Last);
    }

    protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
    {
        if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
        {
            txtGotoPageNo.Text = "1";
            txtGotoPageNo.Focus();
        }
        LoadItemAdjustmentGrid(ePageIndex.Custom);
    }

    protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
    {
        txtGotoPageNo.Text = "1";
        LoadItemAdjustmentGrid(ePageIndex.Custom);
        Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
    }


    #endregion

    #endregion

    #region ITem Adjustment Detail

    private void LoadItemAdjustmentDetail()
    {
        txtReferenceNo.Focus();

        if (IsEditMode())
        {
            var objItemAdjustment = new ItemAdjustment() { ItemAdjustmentId = lblItemAdjustmentId.zToInt(), }.SelectList<ItemAdjustment>()[0];
            txtReferenceNo.Text = objItemAdjustment.ReferenceNo;
            txtAdjustmentDate.Text = objItemAdjustment.AdjustmentDate.Value.ToString(CS.ddMMyyyy);
            txtNote.Text = objItemAdjustment.Note;
        }
        else
        {
            txtReferenceNo.Text = txtNote.Text = string.Empty;
            txtAdjustmentDate.Text = IndianDateTime.Today.ToString(CS.ddMMyyyy);
        }

        ManageAdjustmentDetail(null, eRepeaterOperation.Select);
    }


    private bool IsEditMode()
    {
        return !lblItemAdjustmentId.zIsNullOrEmpty();
    }

    private bool IsValidate()
    {
        if (txtReferenceNo.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter ReferenceNo.");
            txtReferenceNo.Focus();
            return false;
        }

        if (!txtAdjustmentDate.zIsDate())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Date.");
            txtAdjustmentDate.Focus();
            return false;
        }

        if (!ManageAdjustmentDetail(null, eRepeaterOperation.Validate))
            return false;

        return true;
    }

    private bool SaveData()
    {
        if (!IsValidate())
            return false;

        string Message = string.Empty;
        var objItemAdjustment = new ItemAdjustment()
        {
            OrganizationId = lblOrganizationId.zToInt(),
            OrdersId = 0,
            ReferenceNo = txtReferenceNo.Text,
            AdjustmentDate = txtAdjustmentDate.zToDate(),
            Note = txtNote.Text
        };

        if (IsEditMode())
        {
            objItemAdjustment.ItemAdjustmentId = lblItemAdjustmentId.zToInt();
            objItemAdjustment.Update();

            Message = "Adjustment Change Sucessfully.";
        }
        else
        {
            lblItemAdjustmentId.Text = objItemAdjustment.Insert().ToString();

            Message = "Adjustment Added Sucessfully.";
        }

        ManageAdjustmentDetail(null, eRepeaterOperation.Save);

        CU.ZMessage(eMsgType.Success, string.Empty, Message);

        return true;
    }

    protected void lnkSave_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadItemAdjustmentGrid(ePageIndex.Custom);
            SetControl(eControl.ItemAdjustment);
        }
    }

    protected void lnkSaveAndNew_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            lnkAdd_OnClick(null, null);
        }
    }

    protected void lnkCancel_OnClick(object sender, EventArgs e)
    {
        SetControl(eControl.ItemAdjustment);
    }


    private bool ManageAdjustmentDetail(int? PK, eRepeaterOperation RepeaterOperation)
    {
        #region Check Same Product

        var lstCheckProductId = new List<int>();
        if (RepeaterOperation == eRepeaterOperation.Validate)
        {
            foreach (RepeaterItem Item in rptAdjustmentDetail.Items)
            {
                var ddlProduct = Item.FindControl("ddlProduct") as DropDownList;
                if (ddlProduct.zIsSelect())
                {
                    if (lstCheckProductId.Contains(ddlProduct.zToInt().Value))
                    {
                        ddlProduct.Focus();
                        CU.ZMessage(eMsgType.Error, string.Empty, "This Product is Repeat in Same Adjustment Please Marge Same Product.");
                        return false;
                    }

                    lstCheckProductId.Add(ddlProduct.zToInt().Value);
                }
            }
        }

        #endregion

        var lstAdjProduct = new List<AdjProduct>();
        var dtItemAdjustmentDetail = new DataTable();

        int ItemAdjustmentId = lblItemAdjustmentId.zToInt().HasValue ? lblItemAdjustmentId.zToInt().Value : 0;

        if (RepeaterOperation == eRepeaterOperation.Save)
        {
            dtItemAdjustmentDetail = new ItemAdjustmentDetail() { ItemAdjustmentId = ItemAdjustmentId, }.Select();
        }

        if (RepeaterOperation == eRepeaterOperation.Select)
        {
            #region Get Data From DB

            dtItemAdjustmentDetail = new ItemAdjustmentDetail() { ItemAdjustmentId = ItemAdjustmentId, }.Select();
            var dtItemAdjustmentDetailProduct = dtItemAdjustmentDetail.DefaultView.ToTable(true, CS.ProductId);

            var lstProductId = new List<int>();
            foreach (DataRow drItemAdjustmentDetailProduct in dtItemAdjustmentDetailProduct.Rows)
                lstProductId.Add(drItemAdjustmentDetailProduct[CS.ProductId].zToInt().Value);

            var dtItem = new Query() { ProductIdIn = CU.GetParaIn(lstProductId, true) }.Select(eSP.qry_Item);

            foreach (DataRow drItemAdjustmentDetailProduct in dtItemAdjustmentDetailProduct.Rows)
            {
                var lstAdjProductItem = new List<AdjProductItem>();
                foreach (DataRow drItem in dtItem.Select(CS.ProductId + "=" + drItemAdjustmentDetailProduct[CS.ProductId].zToInt()))
                {
                    int? ItemAdjustmentDetailId = null;
                    decimal? Quantity = null, Rate = null;
                    var drItemAdjustmentDetailtem = dtItemAdjustmentDetail.Select(CS.ProductId + "=" + drItemAdjustmentDetailProduct[CS.ProductId].zToInt() + " AND " + CS.ItemId + "=" + drItem[CS.ItemId].zToInt());
                    if (drItemAdjustmentDetailtem.Length > 0)
                    {
                        ItemAdjustmentDetailId = drItemAdjustmentDetailtem[0][CS.ItemAdjustmentDetailId].zToInt();
                        Quantity = drItemAdjustmentDetailtem[0][CS.Quantity].zToDecimal();
                        Rate = drItemAdjustmentDetailtem[0][CS.Rate].zToDecimal();
                    }

                    lstAdjProductItem.Add(new AdjProductItem()
                    {
                        ItemAdjustmentDetailId = ItemAdjustmentDetailId,
                        ItemId = drItem[CS.ItemId].zToInt().Value,
                        ItemName = drItem[CS.ItemName].ToString(),
                        Quantity = Quantity,
                        Rate = Rate,
                    });
                }

                lstAdjProduct.Add(new AdjProduct()
                {
                    ProductId = drItemAdjustmentDetailProduct[CS.ProductId].zToInt().Value,
                    lstAdjProductItem = lstAdjProductItem,
                });
            }

            #endregion
        }
        else
        {
            #region Manage Data 

            foreach (RepeaterItem Item in rptAdjustmentDetail.Items)
            {
                var lblPK = Item.FindControl("lblPK") as Label;

                if (RepeaterOperation == eRepeaterOperation.Remove && PK == lblPK.zToInt())
                    continue;

                var ddlProduct = Item.FindControl("ddlProduct") as DropDownList;

                var rptAdjustmentItemDetail = Item.FindControl("rptAdjustmentItemDetail") as Repeater;

                if (RepeaterOperation == eRepeaterOperation.Add || RepeaterOperation == eRepeaterOperation.Refresh
                || RepeaterOperation == eRepeaterOperation.Remove)
                {
                    #region Get Old Data

                    #region Product Item

                    var lstAdjProductItem = new List<AdjProductItem>();
                    foreach (RepeaterItem ItemAdjustmentItemDetail in rptAdjustmentItemDetail.Items)
                    {
                        var lblItemId = ItemAdjustmentItemDetail.FindControl("lblItemId") as Label;
                        var lblItemName = ItemAdjustmentItemDetail.FindControl("lblItemName") as Label;
                        var txtQuantity = ItemAdjustmentItemDetail.FindControl("txtQuantity") as TextBox;
                        var txtRate = ItemAdjustmentItemDetail.FindControl("txtRate") as TextBox;

                        lstAdjProductItem.Add(new AdjProductItem()
                        {
                            ItemId = lblItemId.zToInt().Value,
                            ItemName = lblItemName.Text,
                            Quantity = txtQuantity.zToDecimal(),
                            Rate = txtRate.zToDecimal(),
                        });
                    }

                    #endregion

                    lstAdjProduct.Add(new AdjProduct()
                    {
                        ProductId = ddlProduct.zToInt().Value,
                        lstAdjProductItem = lstAdjProductItem
                    });

                    #endregion
                }
                else if (RepeaterOperation == eRepeaterOperation.Validate)
                {
                    #region Validate

                    //if (ddlProduct.zIsSelect())
                    //{
                    //    foreach (RepeaterItem ItemAdjustmentItemDetail in rptAdjustmentItemDetail.Items)
                    //    {
                    //        var txtQuantity = ItemAdjustmentItemDetail.FindControl("txtQuantity") as TextBox;
                    //        var txtRate = ItemAdjustmentItemDetail.FindControl("txtRate") as TextBox;

                    //        if (!txtQuantity.zIsDecimal(true))
                    //        {
                    //            txtQuantity.Focus();
                    //            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Quantity");
                    //            return false;
                    //        }
                    //    }
                    //}

                    #endregion
                }
                else if (RepeaterOperation == eRepeaterOperation.Save)
                {
                    #region Save Data

                    if (ddlProduct.zIsSelect())
                    {
                        foreach (RepeaterItem ItemAdjustmentItemDetail in rptAdjustmentItemDetail.Items)
                        {
                            var lblItemAdjustmentDetailId = ItemAdjustmentItemDetail.FindControl("lblItemAdjustmentDetailId") as Label;
                            var lblItemId = ItemAdjustmentItemDetail.FindControl("lblItemId") as Label;
                            var lblItemName = ItemAdjustmentItemDetail.FindControl("lblItemName") as Label;
                            var txtQuantity = ItemAdjustmentItemDetail.FindControl("txtQuantity") as TextBox;
                            var txtRate = ItemAdjustmentItemDetail.FindControl("txtRate") as TextBox;

                            if (txtQuantity.zIsDecimal(true))
                            {
                                var objItemAdjustmentDetail = new ItemAdjustmentDetail()
                                {
                                    ItemAdjustmentDetailId = lblItemAdjustmentDetailId.zToInt(),
                                    ItemAdjustmentId = lblItemAdjustmentId.zToInt(),
                                    ProductId = ddlProduct.zToInt(),
                                    ItemId = lblItemId.zToInt(),
                                    Quantity = txtQuantity.zToDecimal(),
                                    Rate = txtRate.zToDecimal(),
                                };

                                CU.AdjustStock(objItemAdjustmentDetail, true);
                                if (objItemAdjustmentDetail.ItemAdjustmentDetailId.HasValue && objItemAdjustmentDetail.ItemAdjustmentDetailId > 0)
                                {
                                    var drItemAdjustmentDetail = dtItemAdjustmentDetail.Select(CS.ItemAdjustmentDetailId + "=" + objItemAdjustmentDetail.ItemAdjustmentDetailId);
                                    if (drItemAdjustmentDetail.Length > 0)
                                    {
                                        dtItemAdjustmentDetail.Rows.Remove(drItemAdjustmentDetail[0]);
                                    }
                                }
                            }
                        }
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

            if (RepeaterOperation == eRepeaterOperation.Add || lstAdjProduct.Count == 0)
            {
                for (int i = 0; i < 5; i++)
                    lstAdjProduct.Add(new AdjProduct() { });
            }

            dtProduct = new Query() { eStatus = (int)eStatus.Active, OrganizationId = lblOrganizationId.zToInt() }.Select(eSP.qry_Product);
            var drProduct = dtProduct.NewRow();
            drProduct[CS.ProductId] = "0";
            drProduct[CS.ProductCode] = "-- Select Product --";
            dtProduct.Rows.InsertAt(drProduct, 0);

            rptAdjustmentDetail.DataSource = lstAdjProduct;
            rptAdjustmentDetail.DataBind();

            #endregion
        }
        else if (RepeaterOperation == eRepeaterOperation.Save)
        {
            #region Save Data

            foreach (DataRow drItemAdjustmentDetail in dtItemAdjustmentDetail.Rows)
            {
                CU.AdjustStockDelete(drItemAdjustmentDetail[CS.ItemAdjustmentDetailId].zToInt().Value);
            }

            #endregion
        }

        return true;
    }

    protected void rptAdjustmentDetail_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var lblPK = e.Item.FindControl("lblPK") as Label;
        var ddlProduct = e.Item.FindControl("ddlProduct") as DropDownList;
        var lnkDeleteProduct = e.Item.FindControl("lnkDeleteProduct") as LinkButton;

        var rptAdjustmentItemDetail = e.Item.FindControl("rptAdjustmentItemDetail") as Repeater;

        AdjProduct objAdjProduct = (AdjProduct)((RepeaterItem)e.Item).DataItem;

        lblPK.Text = e.Item.ItemIndex.ToString();
        lnkDeleteProduct.CommandArgument = lblPK.Text;

        ddlProduct.DataSource = dtProduct;
        ddlProduct.DataValueField = CS.ProductId;
        ddlProduct.DataTextField = CS.ProductCode;
        ddlProduct.DataBind();

        ddlProduct.SelectedValue = objAdjProduct.ProductId.ToString();

        rptAdjustmentItemDetail.DataSource = objAdjProduct.lstAdjProductItem;
        rptAdjustmentItemDetail.DataBind();

        if (IsSetFocus)
            ddlProduct.Focus();
    }

    protected void rptAdjustmentItemDetail_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var lblItemAdjustmentDetailId = e.Item.FindControl("lblItemAdjustmentDetailId") as Label;
        var lblItemId = e.Item.FindControl("lblItemId") as Label;
        var lblItemName = e.Item.FindControl("lblItemName") as Label;

        var txtQuantity = e.Item.FindControl("txtQuantity") as TextBox;
        var txtRate = e.Item.FindControl("txtRate") as TextBox;

        AdjProductItem objAdjProductItem = (AdjProductItem)((RepeaterItem)e.Item).DataItem;

        lblItemAdjustmentDetailId.Text = objAdjProductItem.ItemAdjustmentDetailId.HasValue ? objAdjProductItem.ItemAdjustmentDetailId.ToString() : string.Empty;
        lblItemId.Text = objAdjProductItem.ItemId.ToString();
        lblItemName.Text = objAdjProductItem.ItemName.ToString();

        txtQuantity.Text = objAdjProductItem.Quantity.HasValue ? objAdjProductItem.Quantity.ToString().Replace(".00", "") : string.Empty;
        txtRate.Text = objAdjProductItem.Rate.HasValue ? objAdjProductItem.Rate.ToString().Replace(".00", "") : string.Empty;
    }

    protected void ddlProduct_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        var ddl = ((DropDownList)sender);
        var rptAdjustmentItemDetail = ddl.Parent.FindControl("rptAdjustmentItemDetail") as Repeater;
        if (ddl.zIsSelect())
        {
            var lstAdjProductItem = new List<AdjProductItem>();
            var dtItem = new Query() { ProductId = ddl.zToInt() }.Select(eSP.qry_Item);
            foreach (DataRow drItem in dtItem.Rows)
            {
                lstAdjProductItem.Add(new AdjProductItem()
                {
                    ItemId = drItem[CS.ItemId].zToInt().Value,
                    ItemName = drItem[CS.ItemName].ToString(),
                });
            }

            rptAdjustmentItemDetail.DataSource = lstAdjProductItem;
            rptAdjustmentItemDetail.DataBind();
        }
        else
        {
            rptAdjustmentItemDetail.DataSource = null;
            rptAdjustmentItemDetail.DataBind();
        }

    }

    protected void lnkAddNewProduct_OnClick(object sender, EventArgs e)
    {
        IsSetFocus = true;
        ManageAdjustmentDetail(null, eRepeaterOperation.Add);
    }

    protected void lnkDeleteProduct_OnClick(object sender, EventArgs e)
    {
        var lnk = ((LinkButton)sender);
        var lblPK = lnk.Parent.FindControl("lblPK") as Label;

        ManageAdjustmentDetail(lblPK.zToInt(), eRepeaterOperation.Remove);
    }

    #endregion


    private void SetControl(eControl Control)
    {
        pnlItemAdjustment.Visible = (Control == eControl.ItemAdjustment);
        pnlItemAdjustmentDetail.Visible = (Control == eControl.ItemAdjustmentDetail);

        if (Control == eControl.ItemAdjustment)
        {
            LoadItemAdjustmentGrid(ePageIndex.Custom);
            CheckVisibleButton();
        }
        else if (Control == eControl.ItemAdjustmentDetail)
        {
            LoadItemAdjustmentDetail();
        }
    }

    public enum eControl
    {
        ItemAdjustment = 1,
        ItemAdjustmentDetail = 2,
    }


    private class AdjProduct
    {
        public AdjProduct()
        {
            lstAdjProductItem = new List<AdjProductItem>();
        }

        public int ProductId;
        public List<AdjProductItem> lstAdjProductItem;
    }

    private class AdjProductItem
    {
        public int? ItemAdjustmentDetailId;
        public int ItemId;
        public string ItemName;
        public decimal? Quantity;
        public decimal? Rate;
    }
}
