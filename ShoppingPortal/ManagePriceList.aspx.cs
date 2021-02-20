using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;

public partial class ManagePriceList : CompressorPage
{
    bool? IsAddEdit;

    DataTable dtProductImage, dtPriceList, dtProductPriceList;

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
            SetControl(eControl.PriceList);

            int FirmId = 0, OrganizationId = 0;
            CU.GetFirmOrganizationId(ref FirmId, ref OrganizationId);
            lblFirmId.Text = FirmId.ToString();
            lblOrganizationId.Text = OrganizationId.ToString();

            CU.LoadDisplayPerPage(ref ddlRecordPerPage);

            LoadPriceListGrid(ePageIndex.Custom);
            CheckVisibleButton();
        }

        Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
        Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
        Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

        try { grdPriceList.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }


    private DataTable GetPriceListDt(ePageIndex ePageIndex)
    {
        int? Status = null;
        if (chkActive.Checked && !chkDeactive.Checked)
            Status = (int)eStatus.Active;
        else if (!chkActive.Checked && chkDeactive.Checked)
            Status = (int)eStatus.Deactive;

        var objQuery = new Query()
        {
            MasterSearch = txtSearch.Text,
            FirmId = lblFirmId.zToInt(),
            eStatus = Status,
            eStatusNot = (int)eStatus.Delete,
        };

        #region Page Index

        int RecordPerPage = ddlRecordPerPage.zToInt().Value;
        int PageIndexTemp = PageIndex;

        CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref objQuery, ref txtGotoPageNo, ref lblCount);
        PageIndex = PageIndexTemp;

        #endregion Page Index

        return objQuery.Select(eSP.qry_PriceList);
    }

    private void LoadPriceListGrid(ePageIndex ePageIndex)
    {
        DataTable dt = GetPriceListDt(ePageIndex);

        if (dt.Rows.Count > 0)
            lblCount.Text = dt.Rows[0][CS.TotalRecord].ToString();
        else
            lblCount.Text = "0";

        divPaging.Visible = (dt.Rows.Count > 0);

        txtGotoPageNo.Text = PageIndex.ToString();

        ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

        grdPriceList.DataSource = dt;
        grdPriceList.DataBind();

        try { grdPriceList.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void CheckVisibleButton()
    {
        var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManagePriceList);

        lnkAdd.Visible = lnkEdit.Visible = objAuthority.IsAddEdit;
        lnkDelete.Visible = objAuthority.IsDelete;

        lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
        lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
    }


    protected void lnkAdd_OnClick(object sender, EventArgs e)
    {
        lblPriceListId.Text = string.Empty;
        LoadPriceListDetail();
        popupPriceList.Show();
    }

    protected void lnkEdit_OnClick(object sender, EventArgs e)
    {
        if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManagePriceList).IsAddEdit && (sender == null || grdPriceList.zIsValidSelection(lblPriceListId, "chkSelect", CS.PriceListId)))
        {
            LoadPriceListDetail();
            popupPriceList.Show();
        }
    }

    protected void lnkEditPriceList_OnClick(object sender, EventArgs e)
    {
        lblPriceListId.Text = ((LinkButton)sender).CommandArgument.ToString();
        lnkEdit_OnClick(null, null);
    }

    protected void lnkRefresh_OnClick(object sender, EventArgs e)
    {
        LoadPriceListGrid(ePageIndex.Custom);
    }

    protected void lnkActive_OnClick(object sender, EventArgs e)
    {
        if (grdPriceList.zIsValidSelection(lblPriceListId, "chkSelect", CS.PriceListId))
        {
            if (new PriceList()
            {
                PriceListId = lblPriceListId.zToInt(),
                eStatus = (int)eStatus.Active
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This PriceList is already Active.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Active, "Active PriceList", "Are You Sure To Active PriceList?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDeactive_OnClick(object sender, EventArgs e)
    {
        if (grdPriceList.zIsValidSelection(lblPriceListId, "chkSelect", CS.PriceListId))
        {
            if (new PriceList()
            {
                PriceListId = lblPriceListId.zToInt(),
                eStatus = (int)eStatus.Deactive
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This PriceList is already Deactive.");
                return;
            }

            string Message = string.Empty;
            if (CU.IsPriceListUsed(lblPriceListId.zToInt().Value, ref Message))
            {
                CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Deactive.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive PriceList", "Are You Sure To Deactive PriceList?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDelete_OnClick(object sender, EventArgs e)
    {
        if (grdPriceList.zIsValidSelection(lblPriceListId, "chkSelect", CS.PriceListId))
        {
            string Message = string.Empty;
            if (CU.IsPriceListUsed(lblPriceListId.zToInt().Value, ref Message))
            {
                CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Delete.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete PriceList", "Are You Sure To Delete PriceList?");
            popupConfirmation.Show();
        }
    }

    protected void Control_CheckedChanged(object sender, EventArgs e)
    {
        LoadPriceListGrid(ePageIndex.Custom);
        CheckVisibleButton();
    }


    private void ManagePriceListStatus(eStatus Status)
    {
        new PriceList()
        {
            PriceListId = lblPriceListId.zToInt(),
            eStatus = (int)Status
        }.Update();
    }

    protected void btnActive_OnClick(object sender, EventArgs e)
    {
        ManagePriceListStatus(eStatus.Active);
        CU.ZMessage(eMsgType.Success, string.Empty, "PriceList Activated Successfully.");
        LoadPriceListGrid(ePageIndex.Custom);
    }

    protected void btnDeactive_OnClick(object sender, EventArgs e)
    {
        ManagePriceListStatus(eStatus.Deactive);
        CU.ZMessage(eMsgType.Success, string.Empty, "PriceList Deactive Successfully.");
        LoadPriceListGrid(ePageIndex.Custom);
    }

    protected void btnDelete_OnClick(object sender, EventArgs e)
    {
        ManagePriceListStatus(eStatus.Delete);
        CU.ZMessage(eMsgType.Success, string.Empty, "PriceList Delete Successfully.");
        LoadPriceListGrid(ePageIndex.Custom);
    }


    protected void grdPriceList_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!IsAddEdit.HasValue)
                IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManagePriceList).IsAddEdit;

            if (IsAddEdit.Value)
                e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdPriceList, "Select$" + e.Row.RowIndex);

            if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdPriceList, CS.eStatus)].Text) != (int)eStatus.Active)
                e.Row.Attributes["class"] = "GridDesableRow ";

            DataRowView dataItem = (DataRowView)e.Row.DataItem;
            var lnkEditPriceList = e.Row.FindControl("lnkEditPriceList") as LinkButton;
            var ltrPriceList = e.Row.FindControl("ltrPriceList") as Literal;
            var aPriceListId = e.Row.FindControl("aPriceListId") as HtmlAnchor;


            lnkEditPriceList.Visible = IsAddEdit.Value;
            ltrPriceList.Visible = !IsAddEdit.Value;

            lnkEditPriceList.Text = ltrPriceList.Text = dataItem[CS.PriceListName].ToString();
            lnkEditPriceList.CommandArgument = dataItem[CS.PriceListId].ToString();

            aPriceListId.InnerText = dataItem[CS.PriceListId].ToString().Encrypt();
            aPriceListId.HRef = "SearchProduct.aspx?pl=" + aPriceListId.InnerText + "&" + CS.OrganizationId.Encrypt() + "=" + CU.GetOrganizationId().ToString().Encrypt();

        }
    }

    protected void grdPriceList_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        lblPriceListId.Text = grdPriceList.Rows[grdPriceList.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdPriceList, CS.PriceListId)].Text;
        lnkEdit_OnClick(null, null);
    }


    private void LoadPriceListDetail()
    {
        txtPriceListName.Focus();

        if (IsEditMode())
        {
            lblPopupTitle.Text = "Edit PriceList";
            var objPriceList = new PriceList() { PriceListId = lblPriceListId.zToInt(), }.SelectList<PriceList>()[0];
            txtPriceListName.Text = objPriceList.PriceListName;
        }
        else
        {
            lblPopupTitle.Text = "New PriceList";
            txtPriceListName.Text = string.Empty;
        }
    }

    private bool IsEditMode()
    {
        return !lblPriceListId.zIsNullOrEmpty();
    }

    private bool IsValidate()
    {
        if (txtPriceListName.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter PriceList Name.");
            txtPriceListName.Focus();
            return false;
        }

        var dt = new Query()
        {
            eStatusNot = (int)eStatus.Delete,
            FirmId = lblFirmId.zToInt(),
            PriceListName = txtPriceListName.Text.Trim()
        }.Select(eSP.qry_PriceList);

        if (dt.Rows.Count > 0 && dt.Rows[0][CS.PriceListId].ToString() != lblPriceListId.Text)
        {
            string Status = dt.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
            CU.ZMessage(eMsgType.Error, string.Empty, "This PriceList is already exist" + Status + ".");
            txtPriceListName.Focus();
            return false;
        }

        return true;
    }

    private bool SaveData()
    {
        if (!IsValidate())
            return false;

        string Message = string.Empty;

        var objPriceList = new PriceList()
        {
            FirmId = lblFirmId.zToInt(),
            PriceListName = txtPriceListName.Text.Trim().zFirstCharToUpper(),
        };

        if (IsEditMode())
        {
            objPriceList.PriceListId = lblPriceListId.zToInt();
            objPriceList.Update();

            Message = "PriceList Detail Change Sucessfully.";
        }
        else
        {
            objPriceList.eStatus = (int)eStatus.Active;
            objPriceList.Insert();

            Message = "New PriceList Added Sucessfully.";
        }

        CU.ZMessage(eMsgType.Success, string.Empty, Message);

        return true;
    }

    protected void btnSave_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadPriceListGrid(ePageIndex.Custom);
        }
    }

    protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadPriceListGrid(ePageIndex.Custom);
            lnkAdd_OnClick(null, null);
        }
    }


    #region Pagging


    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        LoadPriceListGrid(ePageIndex.Prev);
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        LoadPriceListGrid(ePageIndex.Next);
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        LoadPriceListGrid(ePageIndex.First);
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        LoadPriceListGrid(ePageIndex.Last);
    }

    protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
    {
        if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
        {
            txtGotoPageNo.Text = "1";
            txtGotoPageNo.Focus();
        }
        LoadPriceListGrid(ePageIndex.Custom);
    }

    protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
    {
        txtGotoPageNo.Text = "1";
        LoadPriceListGrid(ePageIndex.Custom);
        Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
    }


    #endregion


    #region Price

    protected void chkShowAllProduct_OnCheckedChanged(object sender, EventArgs e)
    {
        LoadPriceListValue();
    }

    protected void lnkReload_OnClick(object sender, EventArgs e)
    {
        LoadPriceListValue();
    }


    protected void lnkPriceListValue_OnClick(object sender, EventArgs e)
    { LoadPriceListValue(); }

    protected void LoadPriceListValue()
    {
        dtProductImage = new Query() { OrganizationId = lblOrganizationId.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_ProductImage);
        dtPriceList = new Query() { FirmId = lblFirmId.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_PriceList);
        dtProductPriceList = new Query() { FirmId = lblFirmId.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_PriceListValue);

        var dtProduct = new Query()
        {
            OrganizationId = lblOrganizationId.zToInt(),
            eStatus = (int)eStatus.Active,
        }.Select(eSP.qry_Product);

        if (!chkShowAllProduct.Checked)
        {
            for (int i = 0; i < dtProduct.Rows.Count; i++)
            {
                if (dtProductPriceList.Select(CS.ProductId + "=" + dtProduct.Rows[i][CS.ProductId]).Length == dtPriceList.Rows.Count)
                {
                    dtProduct.Rows.RemoveAt(i);
                    i--;
                }
            }
        }

        if (dtProduct.Rows.Count > 0)
            dtProduct = dtProduct.Select("", CS.UploadDate + " DESC").CopyToDataTable();

        rptPriceListHead.DataSource = dtPriceList;
        rptPriceListHead.DataBind();

        rptProduct.DataSource = dtProduct;
        rptProduct.DataBind();

        SetControl(eControl.PriceListValue);
    }


    protected void rptProduct_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var lblProductId = e.Item.FindControl("lblProductId") as Label;
        var lblProductName = e.Item.FindControl("lblProductName") as Label;
        var imgProductImage = e.Item.FindControl("imgProductImage") as Image;

        var lblPurchasePrice = e.Item.FindControl("lblPurchasePrice") as Label;
        var rptPriceList = e.Item.FindControl("rptPriceList") as Repeater;

        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;

        lblProductId.Text = dataItem[CS.ProductId].ToString();
        lblProductName.Text = dataItem[CS.ProductCode].ToString();

        #region Set Image

        string ImagePath = string.Empty;

        var drProductImage = dtProductImage.Select(CS.ProductId + " = " + dataItem[CS.ProductId].ToString(), CS.eProductImageType + ", " + CS.SerialNo);
        foreach (DataRow dr in drProductImage)
        {
            string Path = CU.GetFilePath(true, ePhotoSize.P50, eFolder.ProductCImage, dr[CS.ProductImageId].ToString(), true);
            if (!Path.ToLower().Contains("systemimages"))
            {
                ImagePath = Path;
                break;
            }
        }

        imgProductImage.ImageUrl = (ImagePath.zIsNullOrEmpty() ? CU.GetSystemImage(eSystemImage.Noimage_EXT_jpg) : ImagePath);

        #endregion

        lblPurchasePrice.Text = dataItem[CS.PurchasePrice].zToInt() > 0 ? "Price : " + dataItem[CS.PurchasePrice].ToString() : string.Empty;

        rptPriceList.DataSource = dtPriceList;
        rptPriceList.DataBind();
    }

    protected void rptPriceListHead_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var ltrPriceListHead = e.Item.FindControl("ltrPriceListHead") as Literal;

        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;

        ltrPriceListHead.Text = dataItem[CS.PriceListName].ToString();
    }

    protected void rptPriceList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var lblPriceListId = e.Item.FindControl("lblPriceListId") as Label;
        var lblPriceListValueId = e.Item.FindControl("lblPriceListValueId") as Label;

        var txtProductPrice = e.Item.FindControl("txtProductPrice") as TextBox;

        var ItemParent = (RepeaterItem)e.Item.NamingContainer.Parent;
        var lblProductId = ItemParent.FindControl("lblProductId") as Label;

        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;

        lblPriceListId.Text = dataItem[CS.PriceListId].ToString();

        var dr = dtProductPriceList.Select(CS.PriceListId + " = " + dataItem[CS.PriceListId].zToInt() + " AND " + CS.ProductId + " = " + lblProductId.zToInt());
        txtProductPrice.Text = dr.Length > 0 ? dr[0][CS.Price].ToString() : string.Empty;
        lblPriceListValueId.Text = dr.Length > 0 ? dr[0][CS.PriceListValueId].ToString() : "0";
    }


    protected void lnkSavePriceListValue_OnClick(object sender, EventArgs e)
    {
        var lstInsertPriceList = new List<PriceListValue>();
        var lstUpdatePriceList = new List<PriceListValue>();
        var lstDeletePriceList = new List<PriceListValue>();

        foreach (RepeaterItem item in rptProduct.Items)
        {
            var lblProductId = item.FindControl("lblProductId") as Label;
            var rptPriceList = item.FindControl("rptPriceList") as Repeater;

            foreach (RepeaterItem PriceListItem in rptPriceList.Items)
            {
                var lblPriceListId = PriceListItem.FindControl("lblPriceListId") as Label;
                var lblPriceListValueId = PriceListItem.FindControl("lblPriceListValueId") as Label;
                var txtProductPrice = PriceListItem.FindControl("txtProductPrice") as TextBox;

                if (txtProductPrice.zToInt() > 0)
                {
                    #region Set Value

                    var objPriceListValue = new PriceListValue()
                    {
                        PriceListValueId = lblPriceListValueId.zToInt(),
                        ProductId = lblProductId.zToInt(),
                        PriceListId = lblPriceListId.zToInt(),
                        Price = txtProductPrice.zToInt()
                    };

                    if (objPriceListValue.PriceListValueId.HasValue && objPriceListValue.PriceListValueId > 0)
                    {
                        lstUpdatePriceList.Add(objPriceListValue);
                        if (lstUpdatePriceList.Count >= 500)
                        {
                            lstUpdatePriceList.Update();
                            lstUpdatePriceList = new List<PriceListValue>();
                        }
                    }
                    else
                    {
                        lstInsertPriceList.Add(objPriceListValue);
                        if (lstInsertPriceList.Count >= 500)
                        {
                            lstInsertPriceList.Insert();
                            lstInsertPriceList = new List<PriceListValue>();
                        }
                    }

                    #endregion
                }
                else if (lblPriceListValueId.zToInt() > 0)
                {
                    #region Delete

                    lstDeletePriceList.Add(new PriceListValue() { PriceListValueId = lblPriceListValueId.zToInt() });
                    if (lstDeletePriceList.Count >= 500)
                    {
                        lstDeletePriceList.Delete();
                        lstDeletePriceList = new List<PriceListValue>();
                    }

                    #endregion
                }
            }
        }

        lstUpdatePriceList.Update();
        lstInsertPriceList.Insert();
        lstDeletePriceList.Delete();

        LoadPriceListValue();
        CU.ZMessage(eMsgType.Success, string.Empty, "Price List Saved Successfully");
    }

    protected void lnkCancelPriceListValue_OnClick(object sender, EventArgs e)
    {
        SetControl(eControl.PriceList);
    }

    protected void lnkCopyCustomerPrice_OnClick(object sender, EventArgs e)
    {
        //var dtProduct = new Product() { OrganizationId = CU.GetOrganizationId() }.Select(new Product() { ProductId = 0 });
        //var lstProduct = new Product() { OrganizationId = CU.GetOrganizationId() }.SelectList<Product>();

        dtPriceList = new Query() { FirmId = lblFirmId.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_PriceList);
        dtProductPriceList = new Query() { FirmId = lblFirmId.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_PriceListValue);

        var dtProduct = new Query()
        {
            OrganizationId = lblOrganizationId.zToInt(),
            eStatus = (int)eStatus.Active,
        }.Select(eSP.qry_Product);
        //if (!chkShowAllProduct.Checked)
        //{
        //    for (int i = 0; i < dtProduct.Rows.Count; i++)
        //    {
        //        if (dtProductPriceList.Select(CS.ProductId + "=" + dtProduct.Rows[i][CS.ProductId]).Length == dtPriceList.Rows.Count)
        //        {
        //            dtProduct.Rows.RemoveAt(i);
        //            i--;
        //        }
        //    }
        //}

        if (dtProduct.Rows.Count > 0)
            dtProduct = dtProduct.Select("", CS.UploadDate + " DESC").CopyToDataTable();

        for (int i = 0; i <= dtProduct.Rows.Count; i++)
        {
            var objLocalPrice = new PriceListValue() { PriceListId = 1, ProductId = dtProduct.Rows[i]["ProductId"].zToInt().Value };
            var objGlobalPrice = new PriceListValue() { PriceListId = 2, ProductId = dtProduct.Rows[i]["ProductId"].zToInt().Value };
            var objCustomerPrice = new PriceListValue() { PriceListId = 3, ProductId = dtProduct.Rows[i]["ProductId"].zToInt().Value };

            var lstLocalPrice = objLocalPrice.SelectList<PriceListValue>();
            var lstGlobalPrice = objGlobalPrice.SelectList<PriceListValue>();

            var lstCustomerPrice = objCustomerPrice.SelectList<PriceListValue>();
            if (lstCustomerPrice.Count == 0)
                continue;
            objCustomerPrice = lstCustomerPrice[0];

            if (objCustomerPrice.Price.Value == 700 - 1)
            {
                objGlobalPrice.Price = 600 - 1;
                objLocalPrice.Price = 550 - 1;
            }
            else if (objCustomerPrice.Price.Value == 800 - 1)
            {
                objGlobalPrice.Price = 700 - 1;
                objLocalPrice.Price = 650 - 1;
            }
            else if (objCustomerPrice.Price.Value == 900 - 1)
            {
                objGlobalPrice.Price = 750 - 1;
                objLocalPrice.Price = 720 - 1;
            }
            else if (objCustomerPrice.Price.Value == 1000 - 1)
            {
                objGlobalPrice.Price = 850 - 1;
                objLocalPrice.Price = 800 - 1;
            }
            else if (objCustomerPrice.Price.Value == 1100 - 1)
            {
                objGlobalPrice.Price = 950 - 1;
                objLocalPrice.Price = 900 - 1;
            }
            else if (objCustomerPrice.Price.Value == 1200 - 1)
            {
                objGlobalPrice.Price = 1000 - 1;
                objLocalPrice.Price = 950 - 1;
            }
            else if (objCustomerPrice.Price.Value == 1300 - 1)
            {
                objGlobalPrice.Price = 1100 - 1;
                objLocalPrice.Price = 1050 - 1;
            }
            else if (objCustomerPrice.Price.Value == 1400 - 1)
            {
                objGlobalPrice.Price = 1200 - 1;
                objLocalPrice.Price = 1150 - 1;
            }
            else if (objCustomerPrice.Price.Value == 1500 - 1)
            {
                objGlobalPrice.Price = 1300 - 1;
                objLocalPrice.Price = 1250 - 1;
            }
            else if (objCustomerPrice.Price.Value == 1600 - 1)
            {
                objGlobalPrice.Price = 1400 - 1;
                objLocalPrice.Price = 1350 - 1;
            }
            else if (objCustomerPrice.Price.Value == 1700 - 1)
            {
                objGlobalPrice.Price = 1500 - 1;
                objLocalPrice.Price = 1450 - 1;
            }
            else if (objCustomerPrice.Price.Value == 1800 - 1)
            {
                objGlobalPrice.Price = 1600 - 1;
                objLocalPrice.Price = 1550 - 1;
            }
            else if (objCustomerPrice.Price.Value == 1900 - 1)
            {
                objGlobalPrice.Price = 1700 - 1;
                objLocalPrice.Price = 1650 - 1;
            }
            else if (objCustomerPrice.Price.Value == 2200 - 1)
            {
                objGlobalPrice.Price = 1950 - 1;
                objLocalPrice.Price = 1900 - 1;
            }
            else if (objCustomerPrice.Price.Value == 2600 - 1)
            {
                objGlobalPrice.Price = 2350 - 1;
                objLocalPrice.Price = 2300 - 1;
            }

            //if (objCustomerPrice.Price.Value >= 600 && objCustomerPrice.Price.Value < 699)
            //{
            //    objCustomerPrice.Price = 700 - 1;
            //    objGlobalPrice.Price = 550 - 1;
            //    objLocalPrice.Price = 600 - 1;
            //}
            //else if (objCustomerPrice.Price.Value >= 700 && objCustomerPrice.Price.Value < 799)
            //{
            //    objCustomerPrice.Price = 800 - 1;
            //    objGlobalPrice.Price = 650 - 1;
            //    objLocalPrice.Price = 650 - 1;
            //}
            //else if (objCustomerPrice.Price.Value >= 800 && objCustomerPrice.Price.Value < 899)
            //{
            //    objCustomerPrice.Price = 900 - 1;
            //    objGlobalPrice.Price = 720 - 1;
            //    objLocalPrice.Price = 750 - 1;
            //}
            //else if (objCustomerPrice.Price.Value >= 900 && objCustomerPrice.Price.Value < 999)
            //{
            //    objCustomerPrice.Price = 1000 - 1;
            //    objGlobalPrice.Price = 800 - 1;
            //    objLocalPrice.Price = 850 - 1;
            //}
            //else if (objCustomerPrice.Price.Value >= 1000 && objCustomerPrice.Price.Value < 1099)
            //{
            //    objCustomerPrice.Price = 1100 - 1;
            //    objGlobalPrice.Price = 900 - 1;
            //    objLocalPrice.Price = 950 - 1;
            //}
            //else if (objCustomerPrice.Price.Value >= 1100 && objCustomerPrice.Price.Value < 1199)
            //{
            //    objCustomerPrice.Price = 1200 - 1;
            //    objGlobalPrice.Price = 950 - 1;
            //    objLocalPrice.Price = 1000 - 1;
            //}
            //else if (objCustomerPrice.Price.Value >= 1200 && objCustomerPrice.Price.Value < 1299)
            //{
            //    objCustomerPrice.Price = 1300 - 1;
            //    objGlobalPrice.Price = 1050 - 1;
            //    objLocalPrice.Price = 1100 - 1;
            //}
            //else if (objCustomerPrice.Price.Value >= 1300 && objCustomerPrice.Price.Value < 1399)
            //{
            //    objCustomerPrice.Price = 1400 - 1;
            //    objGlobalPrice.Price = 1150 - 1;
            //    objLocalPrice.Price = 1200 - 1;
            //}
            //else if (objCustomerPrice.Price.Value >= 1400 && objCustomerPrice.Price.Value < 1499)
            //{
            //    objCustomerPrice.Price = 1500 - 1;
            //    objGlobalPrice.Price = 1250 - 1;
            //    objLocalPrice.Price = 1300 - 1;
            //}
            //else if (objCustomerPrice.Price.Value >= 1500 && objCustomerPrice.Price.Value < 1599)
            //{
            //    objCustomerPrice.Price = 1600 - 1;
            //    objGlobalPrice.Price = 1350 - 1;
            //    objLocalPrice.Price = 1400 - 1;
            //}
            //else if (objCustomerPrice.Price.Value >= 1600 && objCustomerPrice.Price.Value < 1699)
            //{
            //    objCustomerPrice.Price = 1700 - 1;
            //    objGlobalPrice.Price = 1450 - 1;
            //    objLocalPrice.Price = 1450 - 1;
            //}
            //else if (objCustomerPrice.Price.Value >= 1700 && objCustomerPrice.Price.Value < 1799)
            //{
            //    objCustomerPrice.Price = 1800 - 1;
            //    objGlobalPrice.Price = 1550 - 1;
            //    objLocalPrice.Price = 1550 - 1;
            //}
            //else if (objCustomerPrice.Price.Value >= 1800 && objCustomerPrice.Price.Value < 1899)
            //{
            //    objCustomerPrice.Price = 1900 - 1;
            //    objGlobalPrice.Price = 1650 - 1;
            //    objLocalPrice.Price = 1650 - 1;
            //}
            //else if (objCustomerPrice.Price.Value >= 2100 && objCustomerPrice.Price.Value < 2199)
            //{
            //    objCustomerPrice.Price = 2200 - 1;
            //    objGlobalPrice.Price = 1900 - 1;
            //    objLocalPrice.Price = 1950 - 1;
            //}
            //else if (objCustomerPrice.Price.Value >= 2500 && objCustomerPrice.Price.Value < 2599)
            //{
            //    objCustomerPrice.Price = 2600 - 1;
            //    objGlobalPrice.Price = 2300 - 1;
            //    objLocalPrice.Price = 2300 - 1;
            //}

            if (objGlobalPrice.Price.HasValue)
            {
                //objCustomerPrice.Update();
                if (lstGlobalPrice.Count == 0)
                {
                    objGlobalPrice.Insert();
                }
                else
                {
                    objGlobalPrice.PriceListValueId = lstGlobalPrice[0].PriceListValueId;
                    objGlobalPrice.Update();
                }
            }

            if (objLocalPrice.Price.HasValue)
            {
                if (lstLocalPrice.Count == 0)
                {
                    objLocalPrice.Insert();
                }
                else
                {
                    objLocalPrice.PriceListValueId = lstLocalPrice[0].PriceListValueId;
                    objLocalPrice.Update();
                }
            }
        }

        //var lstGopiPriceListValue = new PriceListValue() { PriceListId = 4 }.SelectList<PriceListValue>();
        //foreach (PriceListValue objGopiPriceListValue in lstGopiPriceListValue)
        //{
        //    var lstOctfisPriceList = new PriceListValue() { PriceListId = 3, ProductId = objGopiPriceListValue.ProductId }.SelectList<PriceListValue>();
        //    if (lstOctfisPriceList.Count == 0)
        //        new PriceListValue()
        //        {
        //            PriceListId = 3,
        //            Price = objGopiPriceListValue.Price,
        //            ProductId = objGopiPriceListValue.ProductId
        //        }.Insert();
        //    else
        //        new PriceListValue()
        //        {
        //            PriceListValueId = lstOctfisPriceList[0].PriceListValueId,
        //            PriceListId = 3,
        //            Price = objGopiPriceListValue.Price,
        //            ProductId = objGopiPriceListValue.ProductId
        //        }.Update();
        //}
    }



    #endregion

    private void SetControl(eControl Control)
    {
        pnlPriceList.Visible = Control == eControl.PriceList;
        pnlPriceListValue.Visible = Control == eControl.PriceListValue;
    }

    private enum eControl
    {
        PriceList = 1,
        PriceListValue = 2,
    }
}
