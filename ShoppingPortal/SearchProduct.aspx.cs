using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.HtmlControls;

public partial class SearchProduct : Page
{
	DataTable dtProductImage = null, dtPriceList = null;

	int PriceListId = 0;

	bool IsAdmin = false;

	string emperson = "_emperson_";
	protected void Page_Load(object sender, EventArgs e)
	{
		string url = Request.Url.ToString();
		if (url.Contains(emperson))
			Response.Redirect(url.Replace(emperson, "&"));
		LoginUtilities.CheckSession(false);

		Page.Title = Page.Title + " - Shopping Portal";
		lblErrorMessage.Text = string.Empty;

		divSearchResult.Visible = divNoDataFound.Visible = false;

		if (!IsPostBack)
		{
			int OrganizationId = 0;
			if (!Request.QueryString[CS.OrganizationId.Encrypt()].zIsNullOrEmpty())
			{
				try { OrganizationId = Request.QueryString[CS.OrganizationId.Encrypt()].ToString().Decrypt().zToInt().Value; }
				catch { }
			}

			lblOrganizationId.Text = OrganizationId.ToString();
			lbleOrganization.Text = CU.GeteOrganisationId(OrganizationId).zToInt().ToString();


			lblFirmId.Text = CU.GetFirmId().ToString();
			int UsersId = CU.GetUsersId();
			lblViewProductVendor.Text = ((int)eYesNo.No).ToString();
			if (UsersId > 0)
			{
				eDesignation Designation = CU.GeteDesignationId(UsersId);
				lbleDesignation.Text = ((int)Designation).ToString();
				lblViewProductVendor.Text = CU.GetAuthority(UsersId, eAuthority.ProductVendorView).IsView ? ((int)eYesNo.Yes).ToString() : ((int)eYesNo.No).ToString();
			}
			else
			{
				lbleDesignation.Text = "0";
			}

			IsAdmin = lbleDesignation.zToInt() == (int)eDesignation.SystemAdmin || lbleDesignation.zToInt() == (int)eDesignation.Admin;

			if (!IsAdmin && UsersId > 0)
			{
				var objUser = new Users() { UsersId = UsersId }.SelectList<Users>()[0];

				lblPriceListId.Text = objUser.PriceListId.HasValue ? objUser.PriceListId.ToString() : "0";
			}

			if (!Request.QueryString["pl"].zIsNullOrEmpty())
			{
				try { PriceListId = Request.QueryString["pl"].ToString().Decrypt().zToInt().Value; }
				catch { }
			}

			if (!Request.QueryString["cp"].zIsNullOrEmpty())
				try { chkPrice.Checked = Convert.ToBoolean(Request.QueryString["cp"].Replace(" ", "")); } catch { }
			if (!Request.QueryString["cg"].zIsNullOrEmpty())
				try { chkGlobal.Checked = Convert.ToBoolean(Request.QueryString["cg"].Replace(" ", "")); } catch { }
			if (!Request.QueryString["tp"].zIsNullOrEmpty())
			{
				txtProduct.Text = Request.QueryString["tp"].ToString();
			}

			SearchProducts();
		}
		else
		{
			IsAdmin = lbleDesignation.zToInt() == (int)eDesignation.SystemAdmin || lbleDesignation.zToInt() == (int)eDesignation.Admin;
		}

		aSearchService.HRef = "SearchService.aspx?" + CS.OrganizationId.Encrypt() + "=" + CU.GetOrganizationId().ToString().Encrypt();
		aWhatsAppLink.HRef = "https://wa.me/?text=" + Request.Url.ToString().Replace("&", emperson);

	}

	protected void btnSearchProduct_OnClick(object sender, EventArgs e)
	{
		string sp = Request.QueryString["sp"].zIsNullOrEmpty() ? "" : "&sp=" + Request.QueryString["sp"];
		string pl = Request.QueryString["pl"].zIsNullOrEmpty() ? "" : "&pl=" + Request.QueryString["pl"];
		Response.Redirect("SearchProduct.aspx?tp=" + txtProduct.Text + "&" + CS.OrganizationId.Encrypt() + "=" + lblOrganizationId.Text.Encrypt() + "&cp=" + chkPrice.Checked + sp + pl + "&cg=" + chkGlobal.Checked);
	}

	protected void SearchProducts()
	{
		var objQuery = new Query()
		{
			OrganizationId = lblOrganizationId.zToInt(),
			eStatus = (int)eStatus.Active
		};

		if (txtProduct.Text.Contains("-")
			&& txtProduct.Text.Replace("-", "").zIsNumber())
		{
			objQuery.FromPrice = txtProduct.Text.Split(new char[] { '-' })[0].zToInt();
			objQuery.ToPrice = txtProduct.Text.Split(new char[] { '-' })[1].zToInt();
		}
		else if (chkPrice.Checked && txtProduct.Text.Replace(" ", "").Replace("-", "").zIsNumber())
		{
			txtProduct.Text = txtProduct.Text.Replace(" ", "");
			if (txtProduct.Text.Contains("-"))
			{
				objQuery.FromPrice = txtProduct.Text.Split(new char[] { '-' })[0].zToInt();
				objQuery.ToPrice = txtProduct.Text.Split(new char[] { '-' })[1].zToInt();
			}
			else
				objQuery.FromPrice = objQuery.ToPrice = txtProduct.Text.zToInt();
		}
		else if (chkGlobal.Checked)
			objQuery.MasterSearch = txtProduct.Text.Trim();
		else
			objQuery.ProductSearch = txtProduct.Text.Trim();

		var dtProduct = new DataTable();
		if (!objQuery.MasterSearch.zIsNullOrEmpty() && objQuery.MasterSearch.Contains(","))
		{
			var strMasterSearch = objQuery.MasterSearch.Split(',');
			foreach (string MasterSearch in strMasterSearch)
			{
				if (!MasterSearch.zIsNullOrEmpty())
				{
					objQuery.MasterSearch = MasterSearch.Trim();
					dtProduct.Merge(objQuery.Select(eSP.qry_ProductSearch));
				}
			}
		}
		else if (!objQuery.ProductSearch.zIsNullOrEmpty() && objQuery.ProductSearch.Contains(","))
		{
			var strProductSearch = objQuery.ProductSearch.Split(',');
			foreach (string ProductSearch in strProductSearch)
			{
				if (!ProductSearch.zIsNullOrEmpty())
				{
					objQuery.ProductSearch = ProductSearch.Trim();
					dtProduct.Merge(objQuery.Select(eSP.qry_ProductSearch));
				}
			}
		}
		else
		{
			if (txtProduct.zIsNullOrEmpty())
			{
				objQuery.FromRow = 1;
				objQuery.ToRow = 5;
				objQuery.ProductSearch = null;
				objQuery.MasterSearch = null;
			}

			dtProduct = objQuery.Select(eSP.qry_ProductSearch);
		}


		divSearchResult.Visible = dtProduct.Rows.Count > 0;
		divNoDataFound.Visible = dtProduct.Rows.Count == 0;

		if (dtProduct.Rows.Count > 0)
		{
			dtProduct = dtProduct.Select("", "ProductPrice, ProductId DESC").CopyToDataTable();

			var lstProductId = new List<int>();
			foreach (DataRow drProduct in dtProduct.Rows)
				lstProductId.Add(drProduct[CS.ProductId].zToInt().Value);

			string ProductIdIn = CU.GetParaIn(lstProductId, true);
			dtProductImage = new Query()
			{
				ProductIdIn = ProductIdIn,
				eProductImageTypeNot = (lbleDesignation.zToInt() == (int)eDesignation.User && !CU.GetAuthority(CU.GetUsersId(), eAuthority.HappyCustomer).IsView) ? (int)eProductImageType.Customer : (int?)null
			}.Select(eSP.qry_ProductImage);
			dtPriceList = new Query()
			{
				ProductIdIn = ProductIdIn,
				FirmId = lblFirmId.zToInt() > 0 ? lblFirmId.zToInt() : (PriceListId > 0 ? (int?)null : 0),
				PriceListId = lblFirmId.zToInt() > 0 ? (int?)null : (PriceListId > 0 ? PriceListId : (int?)null),
			}.Select(eSP.qry_PriceListValue);

			//txtProduct.Text = string.Empty;

			rptProduct.DataSource = dtProduct;
			rptProduct.DataBind();
		}
	}

	protected void rptProduct_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var lblProductCode = e.Item.FindControl("lblProductCode") as Label;
		var lblStock = e.Item.FindControl("lblStock") as Label;
		var lblStockNote = e.Item.FindControl("lblStockNote") as Label;
		var lblPrice = e.Item.FindControl("lblPrice") as Label;
		var lblRecelerPrice = e.Item.FindControl("lblRecelerPrice") as Label;
		var lblVendorPrice = e.Item.FindControl("lblVendorPrice") as Label;
		var lblVendorName = e.Item.FindControl("lblVendorName") as Label;

		var lblDescription = e.Item.FindControl("lblDescription") as Label;
		var txtDescription = e.Item.FindControl("txtDescription") as TextBox;

		var trDescription = e.Item.FindControl("trDescription") as System.Web.UI.HtmlControls.HtmlControl;
		var trImage = e.Item.FindControl("trImage") as System.Web.UI.HtmlControls.HtmlControl;

		var divProduct = e.Item.FindControl("divProduct") as System.Web.UI.HtmlControls.HtmlControl;
		var thVendorPrice = e.Item.FindControl("thVendorPrice") as System.Web.UI.HtmlControls.HtmlControl;
		var tdVendorPrice = e.Item.FindControl("tdVendorPrice") as System.Web.UI.HtmlControls.HtmlControl;
		var thVendorName = e.Item.FindControl("thVendorName") as System.Web.UI.HtmlControls.HtmlControl;
		var tdVendorName = e.Item.FindControl("tdVendorName") as System.Web.UI.HtmlControls.HtmlControl;
		var thViewProduct = e.Item.FindControl("thViewProduct") as System.Web.UI.HtmlControls.HtmlControl;


		var thPrice = e.Item.FindControl("thPrice") as System.Web.UI.HtmlControls.HtmlControl;
		var thResalerPrice = e.Item.FindControl("thResalerPrice") as System.Web.UI.HtmlControls.HtmlControl;
		var tdPrice = e.Item.FindControl("tdPrice") as System.Web.UI.HtmlControls.HtmlControl;
		var tdResalerPrice = e.Item.FindControl("tdResalerPrice") as System.Web.UI.HtmlControls.HtmlControl;
		var tdViewProduct = e.Item.FindControl("tdViewProduct") as System.Web.UI.HtmlControls.HtmlControl;

		var aSearchProduct = e.Item.FindControl("aSearchProduct") as System.Web.UI.HtmlControls.HtmlAnchor;
		var aWhatsAppProduct = e.Item.FindControl("aWhatsAppProduct") as System.Web.UI.HtmlControls.HtmlAnchor;

		var rptPriceListHead = e.Item.FindControl("rptPriceListHead") as Repeater;
		var rptPriceList = e.Item.FindControl("rptPriceList") as Repeater;

		var dataItem = (DataRowView)(e.Item).DataItem;

		lblProductCode.Text = dataItem[CS.ProductCode].ToString();
		lblPrice.Text = dataItem[CS.ProductPrice].ToString();
		lblRecelerPrice.Text = dataItem[CS.RecelerPrice].ToString();
		lblVendorPrice.Text = dataItem[CS.PurchasePrice].ToString();
		lblVendorName.Text = dataItem[CS.VendorName].ToString();

		if (lbleOrganization.zToInt() == (int)eOrganisation.OCTFIS)
		{
			if (IsAdmin)
				lblDescription.Text = dataItem[CS.Description].ToString();
			else
			{
				string Desc = dataItem[CS.Description].ToString();

				try
				{
					string PrePrice = Desc.Substring(0, Desc.IndexOf("Price"));
					string PostPrice = Desc.Substring(Desc.IndexOf("Price"));
					lblDescription.Text = PrePrice + PostPrice.Replace(PostPrice.Substring(PostPrice.IndexOf("Price"), PostPrice.IndexOf("<br>") - PostPrice.IndexOf("Price")), "");
					lblDescription.Text = lblDescription.Text.Replace("👉🏿 <br>", "").Replace("👉🏿<br>", "");
				}
				catch
				{
					lblDescription.Text = Desc;
				}
			}
		}
		else
		{
			lblDescription.Text = dataItem[CS.Description].ToString();
		}

		txtDescription.Text = lblDescription.Text.Replace("<br>", " \n ");

		trDescription.Visible = !lblDescription.Text.zIsNullOrEmpty();
		lblStockNote.Text = dataItem[CS.StockNote].ToString();
		if (dataItem[CS.eStockStatus].zToInt() == (int)eStockStatus.OutOfStock)
		{
			divProduct.addClass("outOfStock");
			lblStock.Text = "Out Of Stock";
			lblStock.ForeColor = System.Drawing.Color.Red;
		}
		else if (dataItem[CS.eStockStatus].zToInt() == (int)eStockStatus.WaitingForStock)
		{
			divProduct.addClass("waitingStock");
			lblStock.Text = "Will Arrive Soon";
			lblStock.ForeColor = System.Drawing.Color.Blue;
		}
		else if (dataItem[CS.eStockStatus].zToInt() == (int)eStockStatus.ReadyToDispatch)
		{
			lblStock.Text = "Ready To Dispatch";
			lblStock.ForeColor = System.Drawing.Color.LawnGreen;
		}
		else
		{
			lblStock.Text = "Available";
			lblStock.ForeColor = System.Drawing.Color.Green;
		}

		thVendorName.Visible = tdVendorName.Visible = thVendorPrice.Visible = tdVendorPrice.Visible = lblViewProductVendor.zToInt() == (int)eYesNo.Yes;
		thPrice.Visible = tdPrice.Visible = IsAdmin;
		thResalerPrice.Visible = tdResalerPrice.Visible = IsAdmin;
		thViewProduct.Visible = tdViewProduct.Visible = lblFirmId.zToInt() > 0;

		aSearchProduct.HRef = "ManageProduct.aspx?tp=" + dataItem[CS.ProductCode].ToString();
		aWhatsAppProduct.HRef = "https://wa.me/?text=" + Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "/") +
				"SearchProduct.aspx?tp=" + dataItem[CS.ProductCode].ToString() + emperson + CS.OrganizationId.Encrypt() + "=" + lblOrganizationId.Text.Encrypt();


		var drImage = dtProductImage.Select(CS.ProductId + " = " + dataItem[CS.ProductId].zToInt());
		trImage.Visible = drImage.Length > 0;
		if (drImage.Length > 0)
		{
			var rptProductImage = e.Item.FindControl("rptProductImage") as Repeater;

			rptProductImage.DataSource = drImage.CopyToDataTable();
			rptProductImage.DataBind();
		}

		if (dtPriceList != null && dtPriceList.Rows.Count > 0)
		{
			var drPriceList = dtPriceList.Select(CS.ProductId + " = " + dataItem[CS.ProductId].zToInt());
			if (drPriceList.Length > 0)
			{
				var dtPL = drPriceList.CopyToDataTable();

				rptPriceListHead.DataSource = dtPL;
				rptPriceListHead.DataBind();

				rptPriceList.DataSource = dtPL;
				rptPriceList.DataBind();
			}
		}
	}

	protected void rptPriceListHead_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var ltrPriceListName = e.Item.FindControl("ltrPriceListName") as Literal;
		var thPriceListHead = e.Item.FindControl("thPriceListHead") as HtmlControl;

		var dataItem = (DataRowView)(e.Item).DataItem;

		ltrPriceListName.Text = dataItem[CS.PriceListName].ToString();

		thPriceListHead.Visible = (dataItem[CS.PriceListName].ToString().ToLower() == "customer price" || IsAdmin || lblPriceListId.zToInt() == dataItem[CS.PriceListId].zToInt() || PriceListId == dataItem[CS.PriceListId].zToInt());
	}

	protected void rptPriceList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var ltrPriceListPrice = e.Item.FindControl("ltrPriceListPrice") as Literal;
		var tdPriceList = e.Item.FindControl("tdPriceList") as HtmlControl;

		var dataItem = (DataRowView)(e.Item).DataItem;

		ltrPriceListPrice.Text = dataItem[CS.Price].ToString();

		tdPriceList.Visible = (dataItem[CS.PriceListName].ToString().ToLower() == "customer price" || IsAdmin || lblPriceListId.zToInt() == dataItem[CS.PriceListId].zToInt() || PriceListId == dataItem[CS.PriceListId].zToInt());
	}

	protected void rptProductImage_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var divImage = e.Item.FindControl("divImage") as System.Web.UI.HtmlControls.HtmlControl;
		var aProductImage = e.Item.FindControl("aProductImage") as System.Web.UI.HtmlControls.HtmlAnchor;
		var aShareWhatsApp = e.Item.FindControl("aShareWhatsApp") as System.Web.UI.HtmlControls.HtmlAnchor;
		var imgProductImage = e.Item.FindControl("imgProductImage") as Image;
		//var btnDownloadProduct = e.Item.FindControl("btnDownloadProduct") as Button;

		var dataItem = (DataRowView)(e.Item).DataItem;
		if (File.Exists(Server.MapPath(CU.GetFilePath(false, ePhotoSize.Original, eFolder.ProductCImage, dataItem[CS.ProductImageId].ToString(), false))))
		{
			//btnDownloadProduct.ResolveUrl(CU.GetFilePath(true, ePhotoSize.Original, eFolder.ProductCImage, dataItem[CS.ProductImageId].ToString(), true));
			aProductImage.HRef = CU.GetFilePath(true, ePhotoSize.Original, eFolder.ProductCImage, dataItem[CS.ProductImageId].ToString(), ".jpg", true, false).Replace("https", "http");
			aShareWhatsApp.HRef = CU.GetFilePath(true, ePhotoSize.Original, eFolder.ProductCImage, dataItem[CS.ProductImageId].ToString(), ".jpg", true, false) + "#Share";
			imgProductImage.ImageUrl = CU.GetFilePath(true, ePhotoSize.P50, eFolder.ProductCImage, dataItem[CS.ProductImageId].ToString(), ".jpg", true, false);
			divImage.Visible = true;
		}
		else
			divImage.Visible = false;

		//{
		//    using (WebClient client = new WebClient())
		//    {
		//        client.DownloadFile(new Uri(url), @"c:\temp\image35.png");

		//        //OR 

		//        client.DownloadFileAsync(new Uri(url), @"c:\temp\image35.png");
		//    }
		//}
	}

	protected void btnDownloadProduct_OnClick(object sender, EventArgs e)
	{

	}
}