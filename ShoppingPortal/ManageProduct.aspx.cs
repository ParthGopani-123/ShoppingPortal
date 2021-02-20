using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;

public partial class ManageProduct : CompressorPage
{
    string VendorCode, ProductPrice, ProductCode, Description;
    int VendorCodeColumn = 0, ProductPriceColumn = 1, ProductCodeColumn = 2, DescriptionColumn = 3;

    bool? IsAddEdit;

    DataTable dtProductImage, dtProductItemVariant, dtProductPriceList, dtPriceList, dtNameTagGlobal;

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

            if (!Request.QueryString["tp"].zIsNullOrEmpty())
                txtSearch.Text = Request.QueryString["tp"];

            CU.LoadDisplayPerPage(ref ddlRecordPerPage);
            FillStockDDL();
            LoadProductGrid(ePageIndex.Custom);
            CheckVisibleButton();
        }

        Confirmationpopup.btnActivePopup_OnClick += new EventHandler(btnActive_OnClick);
        Confirmationpopup.btnDeactivePopup_OnClick += new EventHandler(btnDeactive_OnClick);
        Confirmationpopup.btnDeletePopup_OnClick += new EventHandler(btnDelete_OnClick);

        try { grdProduct.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    protected void FillStockDDL()
    {
        DataTable dtStock = CU.GetEnumDt<eStockStatus>("");

        ddlStock.DataSource = dtStock;
        ddlStock.DataValueField = "Id";
        ddlStock.DataTextField = "Name";
        ddlStock.DataBind();

        var drStock = dtStock.NewRow();
        drStock[CS.Id] = "0";
        drStock[CS.Name] = "--All Stock--";
        dtStock.Rows.InsertAt(drStock, 0);

        ddlSearchStock.DataSource = dtStock;
        ddlSearchStock.DataValueField = "Id";
        ddlSearchStock.DataTextField = "Name";
        ddlSearchStock.DataBind();
    }


    private DataTable GetProductDt(ePageIndex ePageIndex)
    {
        int? Status = null;
        if (chkActive.Checked && !chkDeactive.Checked)
            Status = (int)eStatus.Active;
        else if (!chkActive.Checked && chkDeactive.Checked)
            Status = (int)eStatus.Deactive;

        var objQuery = new Query()
        {
            OrganizationId = lblOrganizationId.zToInt(),
            MasterSearch = txtSearch.Text,
            eStatus = Status,
            eStatusNot = (int)eStatus.Delete,
            FromPrice = txtPriceFrom.zToInt(),
            ToPrice = txtPriceTo.zToInt(),
            ProductCode = txtSearchCode.Text,
            VendorName = txtSearchVendorName.Text,
            ProductDescription = txtSearchDescription.Text,
            ProductNameTag = txtSearchNameTag.Text,
            eStockStatus = ddlSearchStock.zIsSelect() ? ddlSearchStock.zToInt() : null,
        };

        #region Page Index

        int RecordPerPage = ddlRecordPerPage.zToInt().Value;
        int PageIndexTemp = PageIndex;

        CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref objQuery, ref txtGotoPageNo, ref lblCount);
        PageIndex = PageIndexTemp;

        #endregion Page Index

        return objQuery.Select(eSP.qry_Product);
    }

    private void LoadProductGrid(ePageIndex ePageIndex)
    {
        DataTable dtProduct = GetProductDt(ePageIndex);

        if (dtProduct.Rows.Count > 0)
            lblCount.Text = dtProduct.Rows[0][CS.TotalRecord].ToString();
        else
            lblCount.Text = "0";

        divPaging.Visible = (dtProduct.Rows.Count > 0);

        txtGotoPageNo.Text = PageIndex.ToString();

        ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

        if (dtProduct.Rows.Count > 0)
        {
            var lstProductId = new List<int>();
            foreach (DataRow drProduct in dtProduct.Rows)
                lstProductId.Add(drProduct[CS.ProductId].zToInt().Value);

            string ProductIdIn = CU.GetParaIn(lstProductId, true);

            dtProductImage = new Query() { ProductIdIn = ProductIdIn, eStatus = (int)eStatus.Active }.Select(eSP.qry_ProductImage);
            dtPriceList = new Query() { ProductIdIn = ProductIdIn, FirmId = lblFirmId.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_PriceListValue);
            dtNameTagGlobal = new Query() { ProductIdIn = ProductIdIn, }.Select(eSP.qry_NameTag);
        }

        grdProduct.DataSource = dtProduct;
        grdProduct.DataBind();

        try { grdProduct.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void CheckVisibleButton()
    {
        var objAuthority = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageProduct);

        lnkAdd.Visible = lnkEdit.Visible = lnkExcelImport.Visible = objAuthority.IsAddEdit;
        lnkDelete.Visible = objAuthority.IsDelete;

        lnkActive.Visible = (objAuthority.IsAddEdit && ((!chkActive.Checked && chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
        lnkDeactive.Visible = (objAuthority.IsDelete && ((chkActive.Checked && !chkDeactive.Checked) || (chkActive.Checked && chkDeactive.Checked) || (!chkActive.Checked && !chkDeactive.Checked)));
    }

    private string GetProductCode(string VendorCode, int Price)
    {
        string ProductCode = string.Empty;

        string ProductPrice = Price.ToString();
        VendorCode = Regex.Replace(VendorCode, "[^0-9]+", string.Empty);

        while (VendorCode.Length > ProductPrice.Length)
            ProductPrice = ("0" + ProductPrice);

        while (VendorCode.Length < ProductPrice.Length)
            VendorCode = ("0" + VendorCode);

        var arrVendorCode = VendorCode.ToCharArray();
        var arrProductPrice = ProductPrice.ToCharArray();
        for (int i = 0; i < arrProductPrice.Length; i++)
            ProductCode = (ProductCode + arrVendorCode[i].ToString() + arrProductPrice[i].ToString());

        return ProductCode;
    }

    private void SetProductImage(string TextOnImage)
    {
        var lstProductImage = new ProductImage() { ProductId = lblProductId.zToInt() }.SelectList<ProductImage>();
        foreach (ProductImage objProductImage in lstProductImage)
        {
            var OrignelPic = Server.MapPath(CU.GetFilePath(false, ePhotoSize.Original, eFolder.ProductOImage, objProductImage.ProductImageId.ToString(), false));
            if (File.Exists(OrignelPic))
            {
                if (lbleOrganization.zToInt() == (int)eOrganisation.Vinay && false)//square Image but size proplame than not use now
                {
                    string TempFileSquere = Server.MapPath(CU.GettempDownloadPath()) + "\\" + objProductImage.ProductImageId + CU.GetDateTimeName() + ".jpg";
                    if (File.Exists(TempFileSquere))
                        File.Delete(TempFileSquere);

                    File.Copy(OrignelPic, TempFileSquere);

                    System.Drawing.Image originalImage = (System.Drawing.Image)Bitmap.FromFile(TempFileSquere); // set image 

                    int largestDimension = Math.Max(originalImage.Height, originalImage.Width);
                    Size squareSize = new Size(largestDimension, largestDimension);
                    Bitmap squareImage = new Bitmap(squareSize.Width, squareSize.Height);
                    using (Graphics graphics = Graphics.FromImage(squareImage))
                    {
                        graphics.FillRectangle(Brushes.White, 0, 0, squareSize.Width, squareSize.Height);
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                        graphics.DrawImage(originalImage, (squareSize.Width / 2) - (originalImage.Width / 2), (squareSize.Height / 2) - (originalImage.Height / 2), originalImage.Width, originalImage.Height);
                        graphics.Dispose();
                    }

                    File.Delete(OrignelPic);

                    squareImage.Save(OrignelPic);
                    squareImage.Dispose();
                    originalImage.Dispose();

                    File.Delete(TempFileSquere);

                    OrignelPic = Server.MapPath(CU.GetFilePath(false, ePhotoSize.Original, eFolder.ProductOImage, objProductImage.ProductImageId.ToString(), false));
                }

                string TempFile = Server.MapPath(CU.GettempDownloadPath()) + "\\" + objProductImage.ProductImageId + CU.GetDateTimeName() + ".jpg";
                if (File.Exists(TempFile))
                    File.Delete(TempFile);

                File.Copy(OrignelPic, TempFile);

                System.Drawing.Image bitmap = (System.Drawing.Image)Bitmap.FromFile(TempFile); // set image 

                Graphics graphicsImage = Graphics.FromImage(bitmap);
                graphicsImage.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphicsImage.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                StringFormat stringformat = new StringFormat();
                stringformat.Alignment = StringAlignment.Far;
                stringformat.LineAlignment = StringAlignment.Far;

                int Ratio = bitmap.Height / 30;
                string SetImageName = TextOnImage + "-" + objProductImage.SerialNo.ToString();

                if (lbleOrganization.zToInt() == (int)eOrganisation.Vinay)
                {
                    graphicsImage.DrawString(SetImageName,
                       new Font("Arial", Ratio, FontStyle.Bold),
                       new SolidBrush(Color.White),
                       new RectangleF(0, 0, bitmap.Width, bitmap.Height),
                       stringformat);

                    graphicsImage.DrawString(SetImageName,
                        new Font("Arial", Ratio, FontStyle.Regular),
                        new SolidBrush(Color.Black),
                        new RectangleF(0, 0, bitmap.Width, bitmap.Height),
                        stringformat);
                }
                else
                {
                    graphicsImage.DrawString(SetImageName,
                        new Font(FontFamily.GenericSerif, Ratio, FontStyle.Bold),
                        new SolidBrush(Color.White),
                        new RectangleF(0, 0, bitmap.Width, bitmap.Height),
                        stringformat);

                    graphicsImage.DrawString(SetImageName,
                        new Font(FontFamily.GenericSerif, Ratio, FontStyle.Regular),
                        new SolidBrush(Color.Black),
                        new RectangleF(0, 0, bitmap.Width, bitmap.Height),
                        stringformat);
                }

                string UploadFilePath = CU.GettempDownloadPath() + "\\" + objProductImage.ProductImageId + "_" + CU.GetDateTimeName() + ".jpg";
                bitmap.Save(Server.MapPath(UploadFilePath));
                bitmap.Dispose();
                graphicsImage.Dispose();

                File.Delete(TempFile);

                CU.DeleteImage(eFolder.ProductCImage, objProductImage.ProductImageId.ToString());

                var lstUploadPhoto = new List<UploadPhoto>();
                lstUploadPhoto.Add(new UploadPhoto()
                {
                    ControlId = objProductImage.ProductImageId.ToString(),
                    ImagePath = UploadFilePath,
                    IsRemoveOrignalFile = true,
                });

                CU.UploadFile(new FileUpload(), lstUploadPhoto, eFolder.ProductCImage, string.Empty, false);
            }
        }
    }


    protected void lnkAdd_OnClick(object sender, EventArgs e)
    {
        lblProductId.Text = string.Empty;
        LoadProductDetail();
        popupProduct.Show();
    }

    protected void lnkEdit_OnClick(object sender, EventArgs e)
    {
        if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageProduct).IsAddEdit && (sender == null || grdProduct.zIsValidSelection(lblProductId, "chkSelect", CS.ProductId)))
        {
            LoadProductDetail();
            popupProduct.Show();
        }
    }

    protected void lnkEditProduct_OnClick(object sender, EventArgs e)
    {
        lblProductId.Text = ((LinkButton)sender).CommandArgument.ToString();
        lnkEdit_OnClick(null, null);
    }

    protected void lnkClone_Click(object sender, EventArgs e)
    {
        if (CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageProduct).IsAddEdit && (sender == null || grdProduct.zIsValidSelection(lblProductId, "chkSelect", CS.ProductId)))
        {
            LoadProductDetail(true);
            popupProduct.Show();
        }
    }

    protected void lnkActive_OnClick(object sender, EventArgs e)
    {
        if (grdProduct.zIsValidSelection(lblProductId, "chkSelect", CS.ProductId))
        {
            if (new Product()
            {
                ProductId = lblProductId.zToInt(),
                eStatus = (int)eStatus.Active
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Product is already Active.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Active, "Active Product", "Are You Sure To Active Product?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDeactive_OnClick(object sender, EventArgs e)
    {
        if (grdProduct.zIsValidSelection(lblProductId, "chkSelect", CS.ProductId))
        {
            if (new Product()
            {
                ProductId = lblProductId.zToInt(),
                eStatus = (int)eStatus.Deactive
            }.SelectCount() > 0)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This Product is already Deactive.");
                return;
            }

            string Message = string.Empty;
            if (CU.IsProductUsed(lblProductId.zToInt().Value, ref Message))
            {
                CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Deactive.");
                return;
            }

            Confirmationpopup.SetPopupType(ePopupType.Deactive, "Deactive Product", "Are You Sure To Deactive Product?");
            popupConfirmation.Show();
        }
    }

    protected void lnkDelete_OnClick(object sender, EventArgs e)
    {
        var lstProductId = new List<int>();
        foreach (GridViewRow gvrow in grdProduct.Rows)
        {
            var chkSelect = gvrow.FindControl("chkSelect") as CheckBox;
            if (chkSelect != null && chkSelect.Checked)
            {
                string ProductId = gvrow.Cells[CU.GetColumnIndexByName(grdProduct, CS.ProductId)].Text;
                if (ProductId.zIsNumber())
                    lstProductId.Add(ProductId.zToInt().Value);
            }
        }

        if (lstProductId.Count == 0)
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Product.");
            return;
        }

        //for (int i = 0; i < lstProductId.Count; i++)
        //{
        //    string Message = string.Empty;
        //    if (CU.IsProductUsed(lstProductId[i], ref Message))
        //    {
        //        CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Delete.");
        //        return;
        //    }
        //}

        string Msg = "Are You Sure To Delete ";
        if (lstProductId.Count > 1)
            Msg += lstProductId.Count.ToString() + " Products?";
        else
            Msg += "Product?";

        Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Product", Msg);
        popupConfirmation.Show();

        //if (grdProduct.zIsValidSelection(lblProductId, "chkSelect", CS.ProductId))
        //{
        //    string Message = string.Empty;
        //    if (CU.IsProductUsed(lblProductId.zToInt().Value, ref Message))
        //    {
        //        CU.ZMessage(eMsgType.Error, string.Empty, Message + ", Then It can not Delete.");
        //        return;
        //    }

        //    Confirmationpopup.SetPopupType(ePopupType.Delete, "Delete Product", "Are You Sure To Delete Product?");
        //    popupConfirmation.Show();
        //}
    }

    protected void lnkRefresh_OnClick(object sender, EventArgs e)
    {
        LoadProductGrid(ePageIndex.Custom);
    }


    private void ManageProductStatus(eStatus Status)
    {
        new Product()
        {
            ProductId = lblProductId.zToInt(),
            eStatus = (int)Status
        }.Update();
    }

    protected void btnActive_OnClick(object sender, EventArgs e)
    {
        ManageProductStatus(eStatus.Active);
        CU.ZMessage(eMsgType.Success, string.Empty, "Product Activated Successfully.");
        LoadProductGrid(ePageIndex.Custom);
    }

    protected void btnDeactive_OnClick(object sender, EventArgs e)
    {
        ManageProductStatus(eStatus.Deactive);
        CU.ZMessage(eMsgType.Success, string.Empty, "Product Deactive Successfully.");
        LoadProductGrid(ePageIndex.Custom);
    }

    protected void btnDelete_OnClick(object sender, EventArgs e)
    {
        var lstProductId = new List<int>();
        foreach (GridViewRow gvrow in grdProduct.Rows)
        {
            var chkSelect = gvrow.FindControl("chkSelect") as CheckBox;
            if (chkSelect != null && chkSelect.Checked)
            {
                string ProductId = gvrow.Cells[CU.GetColumnIndexByName(grdProduct, CS.ProductId)].Text;
                if (ProductId.zIsNumber())
                    lstProductId.Add(ProductId.zToInt().Value);
            }
        }

        for (int i = 0; i < lstProductId.Count; i++)
        {
            var dtProductImage = new ProductImage() { ProductId = lstProductId[i], }.Select();
            for (int x = 0; x < dtProductImage.Rows.Count; x++)
            {
                int? ProductImageId = dtProductImage.Rows[x][CS.ProductImageId].zToInt();
                new ProductImage() { ProductImageId = ProductImageId }.DeleteAsync();
                CU.DeleteImage(eFolder.ProductOImage, ProductImageId.ToString());
                CU.DeleteImage(eFolder.ProductCImage, ProductImageId.ToString());
            }

            //new Product()
            //{
            //    ProductId = lstProductId[i],
            //    eStatus = (int)eStatus.Delete,
            //}.Update();
        }

        string Msg = "";
        if (lstProductId.Count > 1)
            Msg += lstProductId.Count.ToString() + " Products";
        else
            Msg += "Product";
        Msg += " Deleted Successfully.";

        CU.ZMessage(eMsgType.Success, string.Empty, Msg);
        //LoadProductGrid(ePageIndex.Custom);
    }


    protected void Control_CheckedChanged(object sender, EventArgs e)
    {
        LoadProductGrid(ePageIndex.Custom);
        CheckVisibleButton();
    }


    protected void grdProduct_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!IsAddEdit.HasValue)
                IsAddEdit = CU.GetAuthority(CU.GetUsersId(), eAuthority.ManageProduct).IsAddEdit;

            if (IsAddEdit.Value)
                e.Row.Attributes["ondblclick"] = Page.ClientScript.GetPostBackClientHyperlink(grdProduct, "Select$" + e.Row.RowIndex);

            if (Convert.ToInt32(e.Row.Cells[CU.GetColumnIndexByName(grdProduct, CS.eStatus)].Text) != (int)eStatus.Active)
                e.Row.Attributes["class"] = "GridDesableRow ";

            DataRowView dataItem = (DataRowView)e.Row.DataItem;

            var lnkEditProduct = e.Row.FindControl("lnkEditProduct") as LinkButton;
            var ltrProduct = e.Row.FindControl("ltrProduct") as Literal;
            var imgProductImage = e.Row.FindControl("imgProductImage") as System.Web.UI.WebControls.Image;

            var lblVendorName = e.Row.FindControl("lblVendorName") as Label;
            var lblUploadDate = e.Row.FindControl("lblUploadDate") as Label;
            var lblNameTag = e.Row.FindControl("lblNameTag") as Label;


            var ltrProductStock = e.Row.FindControl("ltrProductStock") as Literal;

            var aSearchProduct = e.Row.FindControl("aSearchProduct") as System.Web.UI.HtmlControls.HtmlAnchor;

            lnkEditProduct.Visible = IsAddEdit.Value;
            ltrProduct.Visible = !IsAddEdit.Value;

            lnkEditProduct.Text = ltrProduct.Text = dataItem[CS.ProductCode].ToString();
            lnkEditProduct.CommandArgument = dataItem[CS.ProductId].ToString();
            lblVendorName.Text = dataItem[CS.VendorName].ToString();
            lblUploadDate.Text = "Upload Date: " + Convert.ToDateTime(dataItem[CS.UploadDate]).ToString(CS.ddMMyyyy);


            var drNameTag = dtNameTagGlobal.Select(CS.ProductId + " = " + dataItem[CS.ProductId].zToInt());
            string nameTag = string.Empty;
            if (drNameTag.Length > 0)
            {
                nameTag = "<b>Name Tag : </b><br />";
                foreach (DataRow drTag in drNameTag)
                {
                    nameTag += drTag[CS.Name] + "<br />";
                }
            }

            lblNameTag.Text = nameTag;
            aSearchProduct.HRef = "SearchProduct.aspx?tp=" + dataItem[CS.ProductCode].ToString() + "&" + CS.OrganizationId.Encrypt() + "=" + dataItem[CS.OrganizationId].ToString().Encrypt();

            if (dataItem[CS.eStockStatus].zToInt() == (int)eStockStatus.OutOfStock)
                e.Row.BackColor = Color.FromArgb(250, 195, 195);//Out
            else if (dataItem[CS.eStockStatus].zToInt() == (int)eStockStatus.WaitingForStock)
                e.Row.BackColor = Color.FromArgb(195, 205, 250);//Wait

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
            if (ImagePath.zIsNullOrEmpty())
                imgProductImage.Attributes.Add("class", "h50p");

            #endregion

            #region Product Price

            var lblPrice = e.Row.FindControl("lblPrice") as Label;

            lblPrice.Text = "Vendor: " + dataItem[CS.PurchasePrice].ToString();
            var drPriceList = dtPriceList.Select(CS.ProductId + " = " + dataItem[CS.ProductId].ToString());
            if (drPriceList.Length > 0)
            {
                var rptProductPrice = e.Row.FindControl("rptProductPrice") as Repeater;

                rptProductPrice.DataSource = drPriceList.CopyToDataTable();
                rptProductPrice.DataBind();
            }

            #endregion

            #region Stock

            ltrProductStock.Text = dataItem[CS.ProductStock].ToString().Replace(",", "</br>");

            #endregion
        }
    }

    protected void grdProduct_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        lblProductId.Text = grdProduct.Rows[grdProduct.SelectedRow.RowIndex].Cells[CU.GetColumnIndexByName(grdProduct, CS.ProductId)].Text;
        lnkEdit_OnClick(null, null);
    }

    protected void rptProductPrice_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var lblPrice = e.Item.FindControl("lblPrice") as Label;

        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;

        lblPrice.Text = dataItem[CS.PriceListName].ToString().Replace(" Price", "") + ": " + dataItem[CS.Price];
    }

    #region Product

    private void LoadVendor()
    {
        CU.FillDropdown(ref ddlVendor, new Query() { OrganizationId = CU.GetOrganizationId(), eStatusNot = (int)eStatus.Delete }.Select(eSP.qry_Vendor), "-- Select Vendor --", CS.VendorId, CS.VendorName);
    }

    private void LoadProductDetail(bool IsClone = false)
    {
        LoadVendor();

        txtVendorCode.Focus();
        SetControl(eControl.ProductDetail);

        var dtNameTag = new Query() { OrganizationId = CU.GetOrganizationId(), }.Select(eSP.qry_NameTag);

        if (IsEditMode())
        {
            lblPopupTitle.Text = IsClone ? "Clone Product" : "Edit Product";
            var objProduct = new Product() { ProductId = lblProductId.zToInt(), }.SelectList<Product>()[0];

            txtStockNote.Text = objProduct.StockNote;
            ddlStock.SelectedValue = objProduct.eStockStatus.HasValue ? objProduct.eStockStatus.ToString() : ((int)eStockStatus.InStock).ToString();
            txtVendorCode.Text = objProduct.VendorCode;
            txtProductCode.Text = objProduct.ProductCode.ToString();
            txtPrice.Text = objProduct.ProductPrice.ToString();
            txtRecelerPrice.Text = objProduct.RecelerPrice.HasValue ? objProduct.RecelerPrice.ToString() : "";
            txtPurchasePrice.Text = objProduct.PurchasePrice.HasValue ? objProduct.PurchasePrice.ToString() : "";
            txtWeight.Text = objProduct.Weight.HasValue ? objProduct.Weight.ToString() : "0";
            ddlVendor.SelectedValue = objProduct.VendorId.HasValue ? objProduct.VendorId.ToString() : null;

            string SelectedNameTag = string.Empty;
            var drNameTag = dtNameTag.Select(CS.ProductId + "='" + lblProductId.zToInt() + "'");
            foreach (DataRow drItemNameTag in drNameTag)
                SelectedNameTag += !SelectedNameTag.zIsNullOrEmpty() ? "," + drItemNameTag[CS.Name] : drItemNameTag[CS.Name];

            txtNameTag.Text = SelectedNameTag;

            txtDescription.Text = objProduct.Description;
            txtStockNote.Text = objProduct.StockNote;
        }
        else
        {
            lblPopupTitle.Text = "New Product";

            ddlStock.SelectedValue = ((int)eStockStatus.InStock).ToString();

            txtStockNote.Text = txtVendorCode.Text = txtProductCode.Text = txtRecelerPrice.Text = txtPurchasePrice.Text = txtWeight.Text = txtNameTag.Text = txtDescription.Text = string.Empty;
            txtPrice.Text = "0";
        }

        string NameTag = string.Empty;
        foreach (DataRow drNameTag in dtNameTag.Rows)
        {
            NameTag += drNameTag[CS.Name] + ",";
        }
        lblNameTagList.Text = NameTag;

        ManageProductImage(null, (IsEditMode() && !IsClone) ? eRepeaterOperation.Select : eRepeaterOperation.Reset, null, 0);
        ManageProductVariant(eRepeaterOperation.Select);
        ManageProductPriceList(eRepeaterOperation.Select);
        ManagePortalProduct(null, eRepeaterOperation.Select);

        lblVendorCodeError.Text = lblProductCodeError.Text = string.Empty;

        if (IsClone)
            lblProductId.Text = string.Empty;
    }

    private bool IsEditMode()
    {
        return !lblProductId.zIsNullOrEmpty();
    }

    private bool CheckVendorCode()
    {
        if (!txtVendorCode.zIsNullOrEmpty())
        {
            var dtProduct = new Query()
            {
                OrganizationId = lblOrganizationId.zToInt(),
                eStatusNot = (int)eStatus.Delete,
                VendorCode = txtVendorCode.Text.Trim(),
            }.Select(eSP.qry_Product);

            if (dtProduct.Rows.Count > 0 && dtProduct.Rows[0][CS.ProductId].ToString() != lblProductId.Text)
            {
                string Status = dtProduct.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
                lblVendorCodeError.Text = "This Vendor Code is already exist" + Status + ".";
                CU.ZMessage(eMsgType.Error, string.Empty, lblVendorCodeError.Text);
                txtVendorCode.Focus();
                SetControl(eControl.ProductDetail);
                return false;
            }
        }

        lblVendorCodeError.Text = string.Empty;
        return true;
    }

    private bool CheckProductCode()
    {
        if (!txtProductCode.zIsNullOrEmpty())
        {
            var dtProductCode = new Query()
            {
                OrganizationId = lblOrganizationId.zToInt(),
                eStatusNot = (int)eStatus.Delete,
                ProductCode = txtProductCode.Text.Trim(),
            }.Select(eSP.qry_Product);

            if (dtProductCode.Rows.Count > 0 && dtProductCode.Rows[0][CS.ProductId].ToString() != lblProductId.Text)
            {
                string Status = dtProductCode.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
                lblProductCodeError.Text = "This Product Code is already exist" + Status + ".";
                CU.ZMessage(eMsgType.Error, string.Empty, lblProductCodeError.Text);
                txtVendorCode.Focus();
                SetControl(eControl.ProductDetail);
                return false;
            }
        }

        lblProductCodeError.Text = string.Empty;
        return true;
    }

    private bool IsValidate()
    {
        if (txtVendorCode.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Vendor Code.");
            txtVendorCode.Focus();
            SetControl(eControl.ProductDetail);
            return false;
        }

        if (!CheckVendorCode())
        {
            return false;
        }

        if (txtProductCode.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Product Code.");
            txtProductCode.Focus();
            SetControl(eControl.ProductDetail);
            return false;
        }

        if (!CheckProductCode())
        {
            return false;
        }

        if (!txtPrice.zIsInteger(false))
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Price.");
            txtPrice.Focus();
            SetControl(eControl.ProductDetail);
            return false;
        }

        if (!ManagePortalProduct(null, eRepeaterOperation.Validate))
        {
            return false;
        }

        return true;
    }

    private bool SaveData()
    {
        if (!IsValidate())
        {
            popupProduct.Show();
            return false;
        }

        string Message = string.Empty;

        var objProduct = new Product()
        {
            OrganizationId = lblOrganizationId.zToInt(),
            eStockStatus = ddlStock.zToInt(),
            VendorCode = txtVendorCode.Text.Trim(),
            ProductCode = txtProductCode.Text.Trim(),
            ProductPrice = txtPrice.zToInt(),
            RecelerPrice = txtRecelerPrice.zToInt(),
            PurchasePrice = txtPurchasePrice.zToInt(),
            Weight = txtWeight.zIsDecimal(false) ? txtWeight.zToDecimal() : 0,
            VendorId = ddlVendor.zToInt(),
            Description = (lbleOrganization.zToInt() == (int)eOrganisation.OCTFIS) ? CU.CorrectDescription(txtDescription.Text) : txtDescription.Text,
            StockNote = txtStockNote.Text
        };

        bool EditMode = IsEditMode();
        if (EditMode)
        {
            objProduct.ProductId = lblProductId.zToInt();
            objProduct.Update();

            Message = "Product Detail Change Sucessfully.";
        }
        else
        {
            objProduct.eStatus = (int)eStatus.Active;
            objProduct.UploadDate = IndianDateTime.Now;
            objProduct.ProductId = objProduct.Insert();
            lblProductId.Text = objProduct.ProductId.ToString();

            Message = "New Product Added Sucessfully.";
        }

        var dtNameTag = new NameTag() { ProductId = lblProductId.zToInt() }.Select();
        var lstNameTag = new List<NameTag>();
        foreach (string strNameTag in txtNameTag.Text.Split(','))
        {
            var drNameTag = dtNameTag.Select(CS.Name + "='" + strNameTag + "'");
            if (drNameTag.Length > 0)
                dtNameTag.Rows.Remove(drNameTag[0]);
            else
                lstNameTag.Add(new NameTag() { Name = strNameTag.Trim(), ProductId = lblProductId.zToInt() });
        }

        lstNameTag.Insert();
        foreach (DataRow drNameTag in dtNameTag.Rows)
        {
            new NameTag() { NameTagId = drNameTag[CS.NameTagId].zToInt() }.Delete();
        }

        ManageProductImage(null, eRepeaterOperation.Save, null, 0);
        SetProductImage(objProduct.ProductCode);
        ManageProductVariant(eRepeaterOperation.Save);
        ManageProductPriceList(eRepeaterOperation.Save);
        ManagePortalProduct(null, eRepeaterOperation.Save);

        #region Set Firm Pricelist Value

        if (!EditMode)
        {
            int Price = 0, CuruntFirmPriceListId = 0, ProductId = lblProductId.zToInt().Value;

            var dtFirm = new Firm() { OrganizationId = lblOrganizationId.zToInt() }.Select(new Firm() { FirmId = 0, PriceListId = 0 });
            CuruntFirmPriceListId = dtFirm.Select(CS.FirmId + " = " + lblFirmId.zToInt())[0][CS.PriceListId].zToInt().Value;

            var dtPriceListValue = new PriceListValue() { PriceListId = CuruntFirmPriceListId, ProductId = ProductId }.Select(new PriceListValue() { Price = 0 });
            if (dtPriceListValue.Rows.Count > 0)
                Price = dtPriceListValue.Rows[0][CS.Price].zToInt().Value;

            var lstPriceListValueInsert = new List<PriceListValue>();
            foreach (DataRow dr in dtFirm.Rows)
            {
                int PriceListId = dr[CS.PriceListId].zToInt().Value;
                if (PriceListId != CuruntFirmPriceListId)
                {
                    lstPriceListValueInsert.Add(new PriceListValue()
                    {
                        PriceListId = PriceListId,
                        ProductId = ProductId,
                        Price = Price
                    });
                }
            }

            lstPriceListValueInsert.Insert();
        }

        #endregion

        CU.ZMessage(eMsgType.Success, string.Empty, Message);

        return true;
    }

    protected void btnSaveProduct_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadProductGrid(ePageIndex.Custom);
        }
    }

    protected void btnSaveAndNewProduct_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadProductGrid(ePageIndex.Custom);
            lnkAdd_OnClick(null, null);
        }
    }

    protected void btnSaveAndCloneProduct_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            LoadProductGrid(ePageIndex.Custom);
            lnkClone_Click(null, null);
        }
    }


    protected void lnkGetProductCode_OnClick(object sender, EventArgs e)
    {
        bool IsValidate = true;
        if (txtVendorCode.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Vendor Code.");
            txtVendorCode.Focus();
            SetControl(eControl.ProductDetail);
            IsValidate = false;
        }

        if (!txtPrice.zIsInteger(false))
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Price.");
            txtPrice.Focus();
            SetControl(eControl.ProductDetail);
            IsValidate = false;
        }

        if (IsValidate)
        {
            txtProductCode.Text = GetProductCode(txtVendorCode.Text, txtPrice.zToInt().Value);
        }

        popupProduct.Show();
    }

    protected void txtVendorCode_OnTextChanged(object sender, EventArgs e)
    {
        CheckVendorCode();
        txtProductCode.Focus();
        popupProduct.Show();
    }

    protected void txtProductCode_OnTextChanged(object sender, EventArgs e)
    {
        CheckProductCode();
        popupProduct.Show();
    }


    #region Image

    private void UploadProductImage(eProductImageType ProductImageType)
    {
        var txtImagePath = txtProductImage;
        if (ProductImageType == eProductImageType.Original)
            txtImagePath = txtProductImageOriginal;
        else if (ProductImageType == eProductImageType.Customer)
            txtImagePath = txtProductImageCustomer;

        var lstProductImage = new List<string>();
        foreach (string ImagePath in txtImagePath.Text.Split('~'))
        {
            if (CU.IsImage(ImagePath))
                lstProductImage.Add(ImagePath);
        }

        if (lstProductImage.Count == 0)
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Valid Image");
        }
        else
            ManageProductImage(null, eRepeaterOperation.Add, lstProductImage, (int)ProductImageType);

        popupProduct.Show();
        SetControl(eControl.ProductImage);
    }

    protected void lnkProductImageUpload_OnClick(object sender, EventArgs e)
    {
        UploadProductImage(eProductImageType.Modeling);
    }

    protected void lnkProductImageOriginalUpload_OnClick(object sender, EventArgs e)
    {
        UploadProductImage(eProductImageType.Original);
    }

    protected void lnkProductImageCustomerUpload_OnClick(object sender, EventArgs e)
    {
        UploadProductImage(eProductImageType.Customer);
    }


    private bool ManageProductImage(int? PK, eRepeaterOperation RepeaterOperation, List<string> lstProductImage, int ProductImageType)
    {
        var lstUploadPhoto = new List<UploadPhoto>();
        var dtProductImage = new DataTable();

        if (RepeaterOperation == eRepeaterOperation.Save)
        {
            dtProductImage = new ProductImage()
            {
                ProductId = lblProductId.zToInt().HasValue ? lblProductId.zToInt() : 0,
            }.Select();

            if (dtProductImage.Rows.Count > 0)
                dtProductImage = dtProductImage.Select("", CS.SerialNo).CopyToDataTable();
        }
        else if (RepeaterOperation != eRepeaterOperation.Validate)
        {
            dtProductImage.Columns.Add(CS.PK, typeof(int));
            dtProductImage.Columns.Add(CS.ProductImageId);
            dtProductImage.Columns.Add(CS.eProductImageType, typeof(int));
            dtProductImage.Columns.Add(CS.ImagePath);
            dtProductImage.Columns.Add(CS.SerialNo, typeof(int));
            dtProductImage.Columns.Add(CS.eStatus);
        }

        if (RepeaterOperation == eRepeaterOperation.Select)
        {
            #region Get Data From DB

            var dt = new ProductImage()
            {
                ProductId = lblProductId.zToInt().HasValue ? lblProductId.zToInt() : 0,
            }.Select();

            if (dt.Rows.Count > 0)
                dt = dt.Select("", CS.SerialNo).CopyToDataTable();

            foreach (DataRow dr in dt.Rows)
            {
                string ImagePath = CU.GetFilePath(true, ePhotoSize.Original, eFolder.ProductOImage, dr[CS.ProductImageId].ToString(), true);
                if (!ImagePath.ToLower().Contains("systemimages"))
                {
                    var drProductImage = dtProductImage.NewRow();

                    drProductImage[CS.PK] = dtProductImage.Rows.Count + 1;
                    drProductImage[CS.ProductImageId] = dr[CS.ProductImageId].ToString();
                    drProductImage[CS.eProductImageType] = dr[CS.eProductImageType].ToString();
                    drProductImage[CS.ImagePath] = ImagePath;
                    drProductImage[CS.SerialNo] = dr[CS.SerialNo].ToString();
                    drProductImage[CS.eStatus] = (int)eStatus.Active;

                    dtProductImage.Rows.Add(drProductImage);
                }
            }

            #endregion
        }
        else
        {
            foreach (Enum ENum in Enum.GetValues(typeof(eProductImageType)))
            {
                if (ProductImageType == 0 || ENum.zToInt() == ProductImageType)
                {
                    var rptImage = rptProductImage;
                    if (ENum.zToInt() == (int)eProductImageType.Original)
                        rptImage = rptProductImageOriginal;
                    else if (ENum.zToInt() == (int)eProductImageType.Customer)
                        rptImage = rptProductImageCustomer;

                    foreach (RepeaterItem Item in rptImage.Items)
                    {
                        var lblPK = Item.FindControl("lblPK") as Label;
                        var lblProductImageId = Item.FindControl("lblProductImageId") as Label;
                        var lblImagePath = Item.FindControl("lblImagePath") as Label;
                        var txtImageSerialNo = Item.FindControl("txtImageSerialNo") as TextBox;
                        var txteStatus = Item.FindControl("txteStatus") as TextBox;

                        if (RepeaterOperation == eRepeaterOperation.Add || RepeaterOperation == eRepeaterOperation.Refresh
                            || RepeaterOperation == eRepeaterOperation.Remove || RepeaterOperation == eRepeaterOperation.Select)
                        {
                            #region Get Old Data

                            if (RepeaterOperation != eRepeaterOperation.Remove
                                || (RepeaterOperation == eRepeaterOperation.Remove && PK != lblPK.zToInt()))
                            {
                                if (txteStatus.zToInt() == (int)eStatus.Active)
                                {
                                    var drProductImage = dtProductImage.NewRow();

                                    drProductImage[CS.PK] = lblPK.zToInt();
                                    drProductImage[CS.ProductImageId] = lblProductImageId.Text;
                                    drProductImage[CS.eProductImageType] = ENum.zToInt();
                                    drProductImage[CS.ImagePath] = lblImagePath.Text;
                                    drProductImage[CS.SerialNo] = txtImageSerialNo.Text;
                                    drProductImage[CS.eStatus] = (int)eStatus.Active;

                                    dtProductImage.Rows.Add(drProductImage);
                                }
                            }

                            #endregion
                        }
                        else if (RepeaterOperation == eRepeaterOperation.Validate)
                        {
                            #region Validate


                            #endregion
                        }
                        else if (RepeaterOperation == eRepeaterOperation.Save)
                        {
                            #region Save Data

                            if (txteStatus.zToInt() == (int)eStatus.Active)
                            {
                                var objProductImage = new ProductImage()
                                {
                                    ProductImageId = lblProductImageId.zToInt(),
                                    ProductId = lblProductId.zToInt(),
                                    SerialNo = txtImageSerialNo.zIsInteger(false) ? txtImageSerialNo.zToInt() : (Item.ItemIndex + 1),
                                    eProductImageType = ENum.zToInt(),
                                };

                                if (objProductImage.ProductImageId.HasValue && objProductImage.ProductImageId != 0)
                                {
                                    dtProductImage.Rows.Remove(dtProductImage.Select(CS.ProductImageId + " = " + objProductImage.ProductImageId)[0]);
                                    objProductImage.UpdateAsync();
                                }
                                else
                                {
                                    int ProductImageId = objProductImage.Insert();

                                    lstUploadPhoto.Add(new UploadPhoto()
                                    {
                                        ControlId = ProductImageId.ToString(),
                                        ImagePath = lblImagePath.Text,
                                        IsRemoveOrignalFile = true,
                                    });
                                }
                            }

                            #endregion
                        }
                    }
                }
            }
        }

        if (RepeaterOperation == eRepeaterOperation.Add || RepeaterOperation == eRepeaterOperation.Refresh || RepeaterOperation == eRepeaterOperation.Reset
           || RepeaterOperation == eRepeaterOperation.Remove || RepeaterOperation == eRepeaterOperation.Select)
        {
            #region Manage And Bind Data

            if (RepeaterOperation == eRepeaterOperation.Add)
            {
                foreach (string ImageURL in lstProductImage)
                {
                    if (!ImageURL.zIsNullOrEmpty())
                    {
                        var drProductImage = dtProductImage.NewRow();

                        drProductImage[CS.PK] = dtProductImage.Rows.Count + 1;
                        drProductImage[CS.ImagePath] = ImageURL;
                        drProductImage[CS.eProductImageType] = ProductImageType;
                        drProductImage[CS.SerialNo] = dtProductImage.Rows.Count + 1;
                        drProductImage[CS.eStatus] = (int)eStatus.Active;

                        dtProductImage.Rows.Add(drProductImage);
                    }
                }
            }

            for (int i = 0; i < dtProductImage.Rows.Count; i++)
                dtProductImage.Rows[i][CS.PK] = i + 1;

            if (dtProductImage.Rows.Count > 0)
                dtProductImage = dtProductImage.Select("", CS.SerialNo).CopyToDataTable();

            if (ProductImageType == 0 || ProductImageType == (int)eProductImageType.Modeling)
            {
                var dtProductImageMo = dtProductImage.Clone();
                var dr = dtProductImage.Select(CS.eProductImageType + " = " + (int)eProductImageType.Modeling);
                if (dr.Length > 0)
                    dtProductImageMo = dr.CopyToDataTable();

                rptProductImage.DataSource = dtProductImageMo;
                rptProductImage.DataBind();
            }

            if (ProductImageType == 0 || ProductImageType == (int)eProductImageType.Original)
            {
                var dtProductImageOr = dtProductImage.Clone();
                var dr = dtProductImage.Select(CS.eProductImageType + " = " + (int)eProductImageType.Original);
                if (dr.Length > 0)
                    dtProductImageOr = dr.CopyToDataTable();

                rptProductImageOriginal.DataSource = dtProductImageOr;
                rptProductImageOriginal.DataBind();
            }

            if (ProductImageType == 0 || ProductImageType == (int)eProductImageType.Customer)
            {
                var dtProductImageCu = dtProductImage.Clone();
                var dr = dtProductImage.Select(CS.eProductImageType + " = " + (int)eProductImageType.Customer);
                if (dr.Length > 0)
                    dtProductImageCu = dr.CopyToDataTable();

                rptProductImageCustomer.DataSource = dtProductImageCu;
                rptProductImageCustomer.DataBind();
            }

            #endregion
        }
        else if (RepeaterOperation == eRepeaterOperation.Save)
        {
            #region Upload Immage

            CU.UploadFile(new FileUpload(), lstUploadPhoto, eFolder.ProductOImage, string.Empty, false);

            #endregion

            #region Save Data

            foreach (DataRow drProductImage in dtProductImage.Rows)
            {
                new ProductImage() { ProductImageId = drProductImage[CS.ProductImageId].zToInt() }.DeleteAsync();
                CU.DeleteImage(eFolder.ProductCImage, drProductImage[CS.ProductImageId].ToString());
                CU.DeleteImage(eFolder.ProductOImage, drProductImage[CS.ProductImageId].ToString());
            }

            #endregion
        }

        return true;
    }

    protected void rptProductImage_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var lblPK = e.Item.FindControl("lblPK") as Label;
        var lblProductImageId = e.Item.FindControl("lblProductImageId") as Label;
        var lblImagePath = e.Item.FindControl("lblImagePath") as Label;
        var txteStatus = e.Item.FindControl("txteStatus") as TextBox;
        var txtImageSerialNo = e.Item.FindControl("txtImageSerialNo") as TextBox;
        var imgProduct = e.Item.FindControl("imgProduct") as System.Web.UI.WebControls.Image;

        var divProductImage = e.Item.FindControl("divProductImage") as System.Web.UI.HtmlControls.HtmlControl;

        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;

        lblPK.Text = dataItem[CS.PK].ToString();
        lblProductImageId.Text = dataItem[CS.ProductImageId].ToString();
        lblImagePath.Text = dataItem[CS.ImagePath].ToString();
        txteStatus.Text = dataItem[CS.eStatus].ToString();
        txtImageSerialNo.Text = dataItem[CS.SerialNo].ToString();
        imgProduct.ImageUrl = dataItem[CS.ImagePath].ToString();

        divProductImage.Visible = txteStatus.zToInt() == (int)eStatus.Active;
    }

    #endregion

    #region Variant

    private void ManageProductVariant(eRepeaterOperation RepeaterOperation)
    {
        dtProductItemVariant = new Query() { ProductId = IsEditMode() ? lblProductId.zToInt() : 0 }.Select(eSP.qry_ProductItemVarient);

        if (RepeaterOperation == eRepeaterOperation.Select)
        {
            #region Get Data From DB

            var dtVariant = new Query() { OrganizationId = lblOrganizationId.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_Variant);

            rptProductVariant.DataSource = dtVariant;
            rptProductVariant.DataBind();

            #endregion
        }
        else if (RepeaterOperation == eRepeaterOperation.Save)
        {
            #region Save Date

            int ProductId = lblProductId.zToInt().Value;

            var dtAllVariant = new DataTable();
            dtAllVariant.Columns.Add(CS.VariantId, typeof(int));
            dtAllVariant.Columns.Add(CS.VariantValueId, typeof(int));
            dtAllVariant.Columns.Add(CS.Value);
            var lstVariantList = new List<List<ProductVariant>>();

            foreach (RepeaterItem item in rptProductVariant.Items)
            {
                var lstVariant = new List<ProductVariant>();
                var lbxVariantValue = item.FindControl("lbxVariantValue") as ListBox;
                var lblVariantId = item.FindControl("lblVariantId") as Label;
                foreach (ListItem itemVariantValue in lbxVariantValue.Items)
                {
                    if (itemVariantValue.Selected)
                    {
                        var objProductVariant = new ProductVariant()
                        {
                            ProductId = ProductId,
                            VariantId = lblVariantId.zToInt(),
                            VariantValueId = itemVariantValue.Value.zToInt(),
                        };
                        lstVariant.Add(objProductVariant);

                        var drVariant = dtAllVariant.NewRow();
                        drVariant[CS.VariantId] = lblVariantId.zToInt();
                        drVariant[CS.VariantValueId] = itemVariantValue.Value.zToInt();
                        drVariant[CS.Value] = itemVariantValue.Text;
                        dtAllVariant.Rows.Add(drVariant);
                    }
                }

                if (lstVariant.Count > 0)
                    lstVariantList.Add(lstVariant);
            }

            var dtAllVariantCopy = dtAllVariant.Copy();
            var dtItem = new DataTable();
            foreach (List<ProductVariant> lstVariant in lstVariantList)
            {
                dtItem.Columns.Add("V" + lstVariant[0].VariantId);
                int RowIndex = 0;
                foreach (ProductVariant objProductVariant in lstVariant)
                {
                    if (RowIndex > 0 && dtItem.Rows.Count >= RowIndex)
                        break;
                    int RemVariant = 1;
                    var drRemainingVariant = dtAllVariant.Select(CS.VariantId + "<>" + objProductVariant.VariantId);
                    if (drRemainingVariant.Length > 0)
                    {
                        dtAllVariant = drRemainingVariant.CopyToDataTable();
                        var dtVariantId = dtAllVariant.DefaultView.ToTable(true, CS.VariantId);
                        foreach (DataRow drVariantId in dtVariantId.Rows)
                        {
                            RemVariant = RemVariant * dtAllVariant.Select(CS.VariantId + "=" + drVariantId[CS.VariantId].zToInt()).Length;
                        }
                    }
                    if (dtItem.Columns.Count == 1)
                    {
                        for (int i = 0; i < RemVariant; i++)
                        {
                            var drNew = dtItem.NewRow();
                            drNew["V" + objProductVariant.VariantId] = objProductVariant.VariantValueId;
                            dtItem.Rows.Add(drNew);
                        }
                    }
                    else
                    {
                        int index = 0;
                        for (int i = 0; i < dtItem.Rows.Count / RemVariant; i++)
                        {
                            for (int j = 0; j < RemVariant; j++)
                            {
                                dtItem.Rows[RowIndex]["V" + objProductVariant.VariantId] = lstVariant[index].VariantValueId;
                                RowIndex++;
                            }
                            index++;
                            if (lstVariant.Count == index)
                                index = 0;
                        }
                    }
                }
            }
            dtAllVariant = dtAllVariantCopy.Copy();

            dtItem.Columns.Add("ItemName");
            for (int i = 0; i < dtItem.Rows.Count; i++)
            {
                string ItemName = "";
                for (int j = 0; j < dtItem.Columns.Count - 1; j++)
                {
                    ItemName += (ItemName.zIsNullOrEmpty() ? "" : "-") + dtAllVariant.Select(CS.VariantValueId + "=" + dtItem.Rows[i][j])[0][CS.Value];
                }
                dtItem.Rows[i]["ItemName"] = ItemName;
            }

            var objProduct = new Product() { ProductId = ProductId }.SelectList<Product>()[0];

            #region Save Data 

            var dtDBItemVariant = new Query() { ProductId = ProductId }.Select(eSP.qry_ItemVarient);
            var dtDBItem = dtDBItemVariant.DefaultView.ToTable(true, CS.ItemId);

            for (int i = 0; i < dtItem.Rows.Count; i++)
            {
                #region Item

                int? ItemId = null;

                if (dtDBItemVariant.Rows.Count > 0)
                {
                    var lstVariantValueId = new List<int>();
                    for (int j = 0; j < dtItem.Columns.Count - 1; j++)
                        lstVariantValueId.Add(dtItem.Rows[i][j].zToInt().Value);

                    foreach (DataRow drDBItem in dtDBItem.Rows)
                    {
                        bool IsFindItemId = true;
                        var drDBItemVariant = dtDBItemVariant.Select(CS.ItemId + "=" + drDBItem[CS.ItemId].zToInt());
                        if (lstVariantValueId.Count != drDBItemVariant.Length)
                            break;

                        foreach (DataRow dr in drDBItemVariant)
                        {
                            if (!lstVariantValueId.Contains(dr[CS.VariantValueId].zToInt().Value))
                            {
                                IsFindItemId = false;
                                break;
                            }
                        }

                        if (IsFindItemId)
                            ItemId = drDBItem[CS.ItemId].zToInt();
                    }
                }

                #endregion

                #region Item Variant

                var objItem = new Item()
                {
                    ItemId = ItemId,
                    ProductId = ProductId,
                    ItemName = objProduct.ProductCode + " " + dtItem.Rows[i][CS.ItemName].ToString(),
                };

                if (objItem.ItemId.HasValue && objItem.ItemId > 0)
                {
                    objItem.Update();
                }
                else
                {
                    objItem.Stock = 0;
                    objItem.ItemId = objItem.Insert();
                }

                var lstItemVariant = new List<ItemVariant>();
                for (int j = 0; j < dtItem.Columns.Count - 1; j++)
                {
                    var objItemVariant = new ItemVariant()
                    {
                        ItemId = objItem.ItemId,
                        VariantValueId = dtItem.Rows[i][j].zToInt(),
                    };
                    var drDBItemVariant = dtDBItemVariant.Select(CS.ItemId + "=" + objItemVariant.ItemId + " AND " + CS.VariantValueId + "=" + objItemVariant.VariantValueId);
                    if (drDBItemVariant.Length > 0)
                        objItemVariant.ItemVariantId = drDBItemVariant[0][CS.ItemVariantId].zToInt();

                    if (objItemVariant.ItemVariantId.HasValue && objItemVariant.ItemVariantId > 0)
                    {
                        objItemVariant.Update();
                    }
                    else
                    {
                        objItemVariant.Insert();
                    }
                }

                #endregion
            }

            if (dtItem.Rows.Count == 0)
            {
                var drDBItemVariant = dtDBItemVariant.Select(CS.VariantValueId + "= 0");
                var objItem = new Item()
                {
                    ItemId = drDBItemVariant.Length > 0 ? drDBItemVariant[0][CS.ItemId].zToInt() : null,
                    ProductId = ProductId,
                    ItemName = objProduct.ProductCode,
                    Stock = 0,
                };

                if (objItem.ItemId.HasValue && objItem.ItemId > 0)
                    objItem.Update();
                else
                    objItem.ItemId = objItem.Insert();

                var objItemVariant = new ItemVariant()
                {
                    ItemVariantId = drDBItemVariant.Length > 0 ? drDBItemVariant[0][CS.ItemVariantId].zToInt() : null,
                    VariantValueId = 0,
                    ItemId = objItem.ItemId
                };

                if (objItemVariant.ItemVariantId.HasValue && objItemVariant.ItemVariantId > 0)
                    objItemVariant.Update();
                else
                    objItemVariant.Insert();
            }

            #endregion

            #endregion
        }
    }

    protected void rptProductVariant_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var lblVariantId = e.Item.FindControl("lblVariantId") as Label;
        var ltrVariantName = e.Item.FindControl("ltrVariantName") as Literal;
        var lbxVariantValue = e.Item.FindControl("lbxVariantValue") as ListBox;

        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;

        lblVariantId.Text = dataItem[CS.VariantId].ToString();
        ltrVariantName.Text = dataItem[CS.VariantName].ToString();


        var dtVariantValue = new VariantValue() { VariantId = dataItem[CS.VariantId].zToInt() }.Select();
        lbxVariantValue.DataSource = dtVariantValue;
        lbxVariantValue.DataTextField = CS.Value;
        lbxVariantValue.DataValueField = CS.VariantValueId;
        lbxVariantValue.DataBind();

        string VariantValue = string.Empty;
        var drProductVariant = dtProductItemVariant.Select(CS.VariantId + " = " + dataItem[CS.VariantId].zToInt());
        if (drProductVariant.Length > 0)
        {
            var dtProductVariantThis = drProductVariant.CopyToDataTable();
            foreach (ListItem Item in lbxVariantValue.Items)
                Item.Selected = dtProductVariantThis.Select("VariantValueId=" + Item.Value.zToInt()).Length > 0;
        }
    }

    #endregion

    #region PriceList

    private void ManageProductPriceList(eRepeaterOperation RepeaterOperation)
    {
        dtProductPriceList = new Query() { ProductId = IsEditMode() ? lblProductId.zToInt() : 0, FirmId = lblFirmId.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_PriceListValue);

        if (RepeaterOperation == eRepeaterOperation.Select)
        {
            #region Get Data From DB

            var dtPriceList = new Query() { FirmId = lblFirmId.zToInt(), eStatus = (int)eStatus.Active }.Select(eSP.qry_PriceList);

            rptPriceList.DataSource = dtPriceList;
            rptPriceList.DataBind();

            #endregion
        }
        else if (RepeaterOperation == eRepeaterOperation.Save)
        {
            #region Save Date

            int ProductId = lblProductId.zToInt().Value;

            var lstInsertPriceList = new List<PriceListValue>();
            var lstUpdatePriceList = new List<PriceListValue>();
            var lstDeletePriceList = new List<PriceListValue>();

            foreach (RepeaterItem item in rptPriceList.Items)
            {
                var txtProductPriceListPrice = item.FindControl("txtProductPriceListPrice") as TextBox;
                if (txtProductPriceListPrice.zIsInteger(false))
                {
                    var lblPriceListId = item.FindControl("lblPriceListId") as Label;

                    var objPriceListValue = new PriceListValue()
                    {
                        PriceListValueId = dtProductPriceList.Rows.Count > 0 ? dtProductPriceList.Rows[0][CS.PriceListValueId].zToInt() : (int?)null,
                        ProductId = ProductId,
                        PriceListId = lblPriceListId.zToInt(),
                        Price = txtProductPriceListPrice.zToInt(),
                    };

                    if (objPriceListValue.PriceListValueId.HasValue)
                    {
                        dtProductPriceList.Rows.RemoveAt(0);
                        lstUpdatePriceList.Add(objPriceListValue);
                    }
                    else
                        lstInsertPriceList.Add(objPriceListValue);
                }
            }

            #region Delete 

            foreach (DataRow dr in dtProductPriceList.Rows)
                lstDeletePriceList.Add(new PriceListValue() { PriceListValueId = dr[CS.PriceListValueId].zToInt() });

            #endregion

            lstInsertPriceList.Insert();
            lstUpdatePriceList.Update();
            lstDeletePriceList.Delete();

            #endregion
        }
    }

    protected void rptPriceList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var lblPriceListName = e.Item.FindControl("lblPriceListName") as Label;
        var lblPriceListId = e.Item.FindControl("lblPriceListId") as Label;
        var txtProductPriceListPrice = e.Item.FindControl("txtProductPriceListPrice") as TextBox;

        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;


        lblPriceListName.Text = dataItem[CS.PriceListName].ToString();
        lblPriceListId.Text = dataItem[CS.PriceListId].ToString();

        var dr = dtProductPriceList.Select(CS.PriceListId + " = " + dataItem[CS.PriceListId].zToInt());
        txtProductPriceListPrice.Text = dr.Length > 0 ? dr[0][CS.Price].ToString() : string.Empty;
    }

    #endregion

    #region Portal

    private bool ManagePortalProduct(int? PK, eRepeaterOperation RepeaterOperation)
    {
        var lstPortalProduct = new List<manPortalProduct>();
        var dtPortalProduct = new DataTable();

        int ProductId = lblProductId.zToInt().HasValue ? lblProductId.zToInt().Value : 0;

        if (RepeaterOperation == eRepeaterOperation.Save || RepeaterOperation == eRepeaterOperation.Select)
        {
            dtPortalProduct = new PortalProduct() { ProductId = ProductId, }.Select();
        }

        if (RepeaterOperation == eRepeaterOperation.Select)
        {
            #region Get Data From DB

            foreach (DataRow drPortalProduct in dtPortalProduct.Rows)
            {
                lstPortalProduct.Add(new manPortalProduct()
                {
                    PortalProductId = drPortalProduct[CS.PortalProductId].zToInt(),
                    PortalId = drPortalProduct[CS.PortalId].zToInt().Value,
                    PortalProductName = drPortalProduct[CS.PortalProductName].ToString()
                });
            }
            #endregion
        }
        else
        {
            foreach (RepeaterItem Item in rptPortalProduct.Items)
            {
                var lblPK = Item.FindControl("lblPK") as Label;

                if (RepeaterOperation == eRepeaterOperation.Remove && PK == lblPK.zToInt())
                    continue;

                var lblPortalProductId = Item.FindControl("lblPortalProductId") as Label;
                var ddlPortal = Item.FindControl("ddlPortal") as DropDownList;
                var txtPortalProductName = Item.FindControl("txtPortalProductName") as TextBox;

                if (RepeaterOperation == eRepeaterOperation.Add || RepeaterOperation == eRepeaterOperation.Refresh
                || RepeaterOperation == eRepeaterOperation.Remove)
                {
                    #region Get Old Data

                    lstPortalProduct.Add(new manPortalProduct()
                    {
                        PortalProductId = lblPortalProductId.zToInt(),
                        PortalId = ddlPortal.zToInt().Value,
                        PortalProductName = txtPortalProductName.Text
                    });

                    #endregion
                }
                else if (RepeaterOperation == eRepeaterOperation.Validate)
                {
                    #region Validate

                    //if (!txtQuantity.zIsDecimal(true))
                    //{
                    //    txtQuantity.Focus();
                    //    CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Quantity");
                    //    return false;
                    //}

                    #endregion
                }
                else if (RepeaterOperation == eRepeaterOperation.Save)
                {
                    #region Save Data

                    if (ddlPortal.zIsSelect() && !txtPortalProductName.zIsNullOrEmpty())
                    {
                        var objPortalProduct = new PortalProduct()
                        {
                            PortalProductId = lblPortalProductId.zToInt(),
                            ProductId = ProductId,
                            PortalId = ddlPortal.zToInt(),
                            PortalProductName = txtPortalProductName.Text,
                        };

                        if (objPortalProduct.PortalProductId.HasValue && objPortalProduct.PortalProductId > 0)
                        {
                            var drPortalProduct = dtPortalProduct.Select(CS.PortalProductId + "=" + objPortalProduct.PortalProductId);
                            if (drPortalProduct.Length > 0)
                                dtPortalProduct.Rows.Remove(drPortalProduct[0]);

                            objPortalProduct.Update();
                        }
                        else
                        {
                            objPortalProduct.Insert();
                        }
                    }

                    #endregion
                }
            }
        }

        if (RepeaterOperation == eRepeaterOperation.Add || RepeaterOperation == eRepeaterOperation.Refresh
            || RepeaterOperation == eRepeaterOperation.Remove || RepeaterOperation == eRepeaterOperation.Select)
        {
            #region Manage And Bind Data

            if (RepeaterOperation == eRepeaterOperation.Add || lstPortalProduct.Count == 0)
                lstPortalProduct.Add(new manPortalProduct() { PortalId = 0 });

            rptPortalProduct.DataSource = lstPortalProduct;
            rptPortalProduct.DataBind();

            #endregion
        }
        else if (RepeaterOperation == eRepeaterOperation.Save)
        {
            #region Save Data

            foreach (DataRow drPortalProduct in dtPortalProduct.Rows)
                new PortalProduct() { PortalProductId = drPortalProduct[CS.PortalProductId].zToInt() }.Delete();

            #endregion
        }

        return true;
    }

    protected void rptPortalProduct_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var lblPK = e.Item.FindControl("lblPK") as Label;
        var lblPortalProductId = e.Item.FindControl("lblPortalProductId") as Label;
        var ddlPortal = e.Item.FindControl("ddlPortal") as DropDownList;
        var txtPortalProductName = e.Item.FindControl("txtPortalProductName") as TextBox;

        manPortalProduct objPortalProduct = (manPortalProduct)((RepeaterItem)e.Item).DataItem;

        lblPK.Text = e.Item.ItemIndex.ToString();
        lblPortalProductId.Text = objPortalProduct.PortalProductId.HasValue ? objPortalProduct.PortalProductId.ToString() : string.Empty;

        CU.FillEnumddl<ePortal>(ref ddlPortal, "-- Select Portal --");
        ddlPortal.SelectedValue = objPortalProduct.PortalId.ToString();

        txtPortalProductName.Text = objPortalProduct.PortalProductName;
    }

    protected void lnkAddNewPortalProduct_Click(object sender, EventArgs e)
    {
        ManagePortalProduct(null, eRepeaterOperation.Add);
        popupProduct.Show();
    }

    protected void lnkDeletePortalProduct_OnClick(object sender, EventArgs e)
    {
        var lnk = ((LinkButton)sender);
        var lblPK = lnk.Parent.FindControl("lblPK") as Label;
        ManagePortalProduct(lblPK.zToInt(), eRepeaterOperation.Remove);
        popupProduct.Show();
    }

    #endregion

    #endregion


    #region Excel Import / Export

    protected void lnkExcelExport_OnClick(object sender, EventArgs e)
    {
        var dtProduct = GetProductDt(ePageIndex.AllPage);
        var lstColumns = new List<string>();
        lstColumns.Add(CS.VendorCode);
        lstColumns.Add(CS.ProductPrice);
        lstColumns.Add(CS.ProductCode);
        lstColumns.Add(CS.Description);
        lstColumns.Add(CS.UploadDate);
        lstColumns.Add(CS.PurchasePrice);

        var lstColumnsSelected = new System.Collections.Generic.List<string>();
        lstColumnsSelected.Add(CS.VendorCode);
        lstColumnsSelected.Add(CS.ProductPrice);
        lstColumnsSelected.Add(CS.ProductCode);
        lstColumnsSelected.Add(CS.Description);
        lstColumnsSelected.Add(CS.PurchasePrice);

        ExcelExport.SetExportData(dtProduct, lstColumns, lstColumnsSelected, "Product");
        popupExcelExport.Show();
    }


    protected void lnkExcelImport_OnClick(object sender, EventArgs e)
    {
        chkReplace.Checked = false;
        popupExcelImport.Show();
    }

    protected void btnUpload_OnClick(object sender, EventArgs e)
    {
        if (fuImportExcel.HasFile)
        {
            var dt = new DataTable();
            if (!CU.IsValidExcelFile(fuImportExcel, ref dt, 4, "Product"))
            {
                popupExcelImport.Show();
                return;
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                VendorCode = dt.Rows[i][VendorCodeColumn].ToString().Trim().TrimEnd(',');
                ProductPrice = dt.Rows[i][ProductPriceColumn].ToString().Trim().TrimEnd(',');

                ProductCode = dt.Rows[i][ProductCodeColumn].ToString().Trim().TrimEnd(',');

                if (ProductCode.zIsNullOrEmpty())
                {
                    if (!VendorCode.zIsNullOrEmpty() && ProductPrice.zIsInteger(false))
                        dt.Rows[i][ProductCodeColumn] = GetProductCode(VendorCode, ProductPrice.zToInt().Value);
                }
            }

            dt.AcceptChanges();

            if (CheckData(dt))
                InsertData(dt);
            else
                popupExcelImport.Show();
        }
        else
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Excel File to upload.");
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

                VendorCode = dt.Rows[i][VendorCodeColumn].ToString().Trim().TrimEnd(',');
                ProductPrice = dt.Rows[i][ProductPriceColumn].ToString().Trim().TrimEnd(',');
                ProductCode = dt.Rows[i][ProductCodeColumn].ToString().Trim().TrimEnd(',');
                Description = dt.Rows[i][DescriptionColumn].ToString().Trim().TrimEnd(',');

                #endregion

                #region Check Vendor Code

                if (IsValid)
                {
                    if (VendorCode.zIsNullOrEmpty())
                    {
                        Message += CS.Arrow + "Vendor Code Is Empty" + Connecter;
                        IsValid = false;
                    }
                }

                if (IsValid)
                {
                    string RepeateColumn = string.Empty;
                    if (CU.IsRepeateExcelRow(dt, i, VendorCode, VendorCodeColumn, string.Empty, null, string.Empty, null, ref RepeateColumn))
                    {
                        Message += CS.Arrow + "Vendor Code " + VendorCode + " is Repeating in Record-" + RepeateColumn;
                        IsValid = false;
                    }
                }

                if (IsValid)
                {
                    DataTable dtProduct = new Query()
                    {
                        OrganizationId = lblOrganizationId.zToInt(),
                        VendorCode = VendorCode,
                        eStatusNot = (int)eStatus.Delete
                    }.Select(eSP.qry_Product);

                    if (dtProduct.Rows.Count > 0 && !chkReplace.Checked)
                    {
                        string Status = dtProduct.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
                        Message += CS.Arrow + "This Vendor Code is already exist" + Status + "." + Connecter;
                        IsValid = false;
                    }
                }

                #endregion

                #region Product Price 

                if (IsValid)
                {
                    if (ProductPrice.zIsInteger(false))
                    {
                        Message += CS.Arrow + "Price Is Invalid" + Connecter;
                        IsValid = false;
                    }
                }

                #endregion

                #region Check Product Code

                if (IsValid)
                {
                    if (ProductCode.zIsNullOrEmpty())
                    {
                        Message += CS.Arrow + "Product Code Is Empty" + Connecter;
                        IsValid = false;
                    }
                }

                if (IsValid)
                {
                    if (ProductCode.zIsInteger(false))
                    {
                        Message += CS.Arrow + "Product Code Is Invalid" + Connecter;
                        IsValid = false;
                    }
                }

                if (IsValid)
                {
                    string RepeateColumn = string.Empty;
                    if (CU.IsRepeateExcelRow(dt, i, ProductCode, ProductCodeColumn, string.Empty, null, string.Empty, null, ref RepeateColumn))
                    {
                        Message += CS.Arrow + "Product Code " + ProductCode + " is Repeating in Record-" + RepeateColumn;
                        IsValid = false;
                    }
                }

                if (IsValid)
                {
                    DataTable dtProduct = new Query()
                    {
                        OrganizationId = lblOrganizationId.zToInt(),
                        ProductCode = ProductCode,
                        eStatusNot = (int)eStatus.Delete
                    }.Select(eSP.qry_Product);

                    if (dtProduct.Rows.Count > 0 && !chkReplace.Checked)
                    {
                        string Status = dtProduct.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
                        Message += CS.Arrow + "This Product Code is already exist" + Status + "." + Connecter;
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
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                #region Value Initialization

                VendorCode = dt.Rows[i][VendorCodeColumn].ToString().Trim().TrimEnd(',');
                ProductPrice = dt.Rows[i][ProductPriceColumn].ToString().Trim().TrimEnd(',');
                ProductCode = dt.Rows[i][ProductCodeColumn].ToString().Trim().TrimEnd(',');
                Description = dt.Rows[i][DescriptionColumn].ToString().Trim().TrimEnd(',');

                #endregion

                DataTable dtProduct = new Query()
                {
                    OrganizationId = lblOrganizationId.zToInt(),
                    VendorCode = VendorCode,
                    eStatusNot = (int)eStatus.Delete
                }.Select(eSP.qry_Product);

                var objProduct = new Product()
                {
                    ProductId = dtProduct.Rows.Count > 0 ? dtProduct.Rows[0][CS.ProductId].zToInt() : (int?)null,
                    VendorCode = VendorCode,
                    ProductPrice = ProductPrice.zToInt(),
                    ProductCode = ProductCode,
                    Description = Description,
                };

                if (objProduct.ProductId.HasValue)
                {
                    objProduct.Update();
                    UpdateCount++;
                }
                else
                {
                    objProduct.OrganizationId = lblOrganizationId.zToInt();
                    objProduct.UploadDate = IndianDateTime.Now;
                    objProduct.eStatus = (int)eStatus.Active;
                    objProduct.Insert();
                    InsertCount++;
                }
            }

            CU.SetSuccessExcelMessage(InsertCount, UpdateCount, "Product");
        }
        catch (Exception ex)
        {
            CU.ZMessage(eMsgType.Error, string.Empty, ex.Message, 0);
        }

        LoadProductGrid(ePageIndex.Custom);
    }

    #endregion


    #region Pagging


    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        LoadProductGrid(ePageIndex.Prev);
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        LoadProductGrid(ePageIndex.Next);
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        LoadProductGrid(ePageIndex.First);
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        LoadProductGrid(ePageIndex.Last);
    }

    protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
    {
        if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
        {
            txtGotoPageNo.Text = "1";
            txtGotoPageNo.Focus();
        }
        LoadProductGrid(ePageIndex.Custom);
    }

    protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
    {
        txtGotoPageNo.Text = "1";
        LoadProductGrid(ePageIndex.Custom);
        Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
    }


    #endregion

    private void SetControl(eControl Control)
    {
        txtActiveTabProduct.Text = ((int)Control).ToString();
    }

    private enum eControl
    {
        ProductDetail = 0,
        ProductImage = 1,
        ProductVariant = 2,
        ProductPriceList = 3,
        PortalProduct = 4,
    }

    public class ProductVariant
    {
        public int? VariantId { get; set; }
        public int? ProductId { get; set; }
        public string VariantValue { get; set; }
        public int? VariantValueId { get; set; }
    }

    private class manPortalProduct
    {
        public int? PortalProductId;
        public int PortalId;
        public string PortalProductName;
    }
}
