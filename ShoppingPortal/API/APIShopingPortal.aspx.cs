using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using BOL;
using Utility;

public partial class APIShopingPortal : System.Web.UI.Page
{
    int DefOrganizationId = 0, DefFirmId = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        var drFirm = new Firm() { eStatus = (int)eStatus.Active }.Select(new Firm() { OrganizationId = 0, FirmId = 0 }).Rows[0];
        DefOrganizationId = drFirm[CS.OrganizationId].zToInt().Value;
        DefFirmId = drFirm[CS.FirmId].zToInt().Value;

        Response.Write(GiveResponse());
    }


    private string GiveResponse()
    {
        var objAppComm = new AppComm();

        try
        {
            eAPI API = (eAPI)Convert.ToInt32(Request.QueryString["API"]);

            #region 1.Login

            if (API == eAPI.Login)
            {
                //UserName, Password
                if (Request.QueryString[CS.UserName] == null || Request.QueryString[CS.UserName].zIsNullOrEmpty()
                    || Request.QueryString[CS.Password] == null || Request.QueryString[CS.Password].zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter UserName & Password.";
                }
                else
                {
                    string UserName = Request.QueryString[CS.UserName].ToString();
                    string Password = Request.QueryString[CS.Password].ToString();

                    int UsersId = 0;
                    if (LoginUtilities.IsValidLogin(UserName, Password, ref UsersId))
                    {
                        var drUser = new Query() { UsersId = UsersId }.Select(eSP.qry_User).Rows[0];

                        objAppComm.Success = "success";
                        objAppComm.Message = "Login Successfull";
                        objAppComm.OrganizationId = drUser[CS.OrganizationId].zToInt().Value;
                        objAppComm.FirmId = drUser[CS.FirmId].zToInt().Value;
                        objAppComm.UsersId = UsersId;
                        objAppComm.Name = drUser[CS.Name].ToString();
                    }
                    else
                    {
                        objAppComm.Success = "fail";
                        objAppComm.Message = "Invalid Username or Password";
                    }
                }
            }

            #endregion

            #region 2.User Register

            if (API == eAPI.UserRegister)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=2
                //Name, MobileNo, Email, Address, Pincode, Area, City, State, Country, Password;

                string Area = Request.Form["Area"].ToString();
                string Name = Request.Form[CS.Name].ToString();
                string MobileNo = Request.Form[CS.MobileNo].ToString();
                string Email = Request.Form[CS.Email].ToString();
                string Address = Request.Form[CS.Address].ToString();
                string Pincode = Request.Form[CS.Pincode].ToString();
                string City = Request.Form[CS.City].ToString();
                string State = Request.Form[CS.State].ToString();
                string Country = Request.Form[CS.Country].ToString();
                string Password = Request.Form[CS.Password].ToString();

                bool IsValidate = true;
                if (IsValidate && Name.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter Name.";
                    IsValidate = false;
                }

                if (IsValidate && !MobileNo.zIsMobile())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter Valid Mobile No.";
                    IsValidate = false;
                }

                var dtUser = new Query()
                {
                    eStatusNot = (int)eStatus.Delete,
                    MobileNo = MobileNo.Trim(),
                }.Select(eSP.qry_User);

                if (dtUser.Rows.Count > 0)
                {
                    string Status = dtUser.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
                    objAppComm.Success = "fail";
                    objAppComm.Message = "This mobile no is already exist" + Status + ".";
                    IsValidate = false;
                }

                if (IsValidate && !Email.zIsEmail())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter Valid Email.";
                    IsValidate = false;
                }

                if (IsValidate && Address.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter Address.";
                    IsValidate = false;
                }

                if (IsValidate && (Pincode.zIsNullOrEmpty() || !Pincode.zIsNumber() || Pincode.Length != 6))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter Valid Pincode.";
                    IsValidate = false;
                }

                if (IsValidate && Area.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter Area.";
                    IsValidate = false;
                }

                if (IsValidate && City.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter City.";
                    IsValidate = false;
                }

                if (IsValidate && State.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter State.";
                    IsValidate = false;
                }

                if (IsValidate && Country.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter Country.";
                    IsValidate = false;
                }

                if (IsValidate && Password.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter Password.";
                    IsValidate = false;
                }


                if (IsValidate)
                {
                    int CountryId = CU.GetCountryId(Country);
                    int StateId = CU.GetStateId(CountryId, State);
                    int CityId = CU.GetCityId(StateId, City);
                    int AreaId = CU.GetAreaId(CityId, Area, Pincode);

                    int AddressId = new Address()
                    {
                        Address1 = Address,
                        Pincode = Pincode,
                        AreaId = AreaId,
                        CityId = CityId,
                        StateId = StateId,
                        CountryId = CountryId
                    }.Insert();

                    var objUser = new Users()
                    {
                        FirmId = DefFirmId,
                        Name = Name,
                        AddressId = AddressId,
                        DesignationId = CU.GetDesignationId(eDesignation.User, DefOrganizationId),
                        MobileNo = MobileNo,
                        Email = Email,
                        ParentUsersId = 0,
                        PriceListId = 0,
                        eStatus = (int)eStatus.Active,
                    };

                    int UsersId = objUser.Insert();

                    LoginUtilities.CreateLogin(UsersId, MobileNo.Trim().ToLower(), Password);
                    CU.SetDefaultAuthority(UsersId, objUser.DesignationId.Value);

                    objAppComm.Success = "success";
                    objAppComm.Message = "Register Successful";
                    objAppComm.OrganizationId = DefOrganizationId;
                    objAppComm.FirmId = DefFirmId;
                    objAppComm.UsersId = UsersId;
                    objAppComm.Name = Name;
                }
            }

            #endregion

            #region 5.Product

            if (API == eAPI.Product)
            {
                //UsersId Login User Id, ProductId=null, SortBy=null
                if (Request.QueryString[CS.UsersId] == null || !Request.QueryString[CS.UsersId].ToString().zIsInteger(false))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass UserId.";
                }
                else
                {
                    bool IsDefaultUser = false;
                    int UsersId = Request.QueryString[CS.UsersId].ToString().zToInt().Value;

                    int? ProductId = null;
                    try { ProductId = Request.QueryString[CS.ProductId].ToString().zToInt(); }
                    catch { }

                    int? SortBy = null;
                    try { SortBy = Request.QueryString[CS.SortBy].ToString().zToInt(); }
                    catch { }

                    if (UsersId == 0)
                    {
                        UsersId = new Users() { DesignationId = CU.GetDesignationId(eDesignation.SystemAdmin), eStatus = (int)eStatus.Active }.SelectList<Users>()[0].UsersId.Value;
                        IsDefaultUser = true;
                    }

                    var dtUser = new Query() { UsersId = UsersId }.Select(eSP.qry_User).Rows[0];
                    int OrganizationId = dtUser[CS.OrganizationId].zToInt().Value;
                    int PriceListId = dtUser[CS.PriceListId].zToInt().Value;

                    var dtProduct = new Query()
                    {
                        OrganizationId = OrganizationId,
                        ProductId = ProductId,
                        eStatus = (int)eStatus.Active,
                    }.Select(eSP.qry_Product);

                    var lstProductId = new List<int>();
                    foreach (DataRow drProduct in dtProduct.Rows)
                        lstProductId.Add(drProduct[CS.ProductId].zToInt().Value);

                    string ProductIdIn = CU.GetParaIn(lstProductId, true);
                    var dtProductImage = new Query() { ProductIdIn = ProductIdIn }.Select(eSP.qry_ProductImage);

                    var dtPriceList = new Query()
                    {
                        ProductIdIn = ProductIdIn,
                        OrganizationId = OrganizationId,
                    }.Select(eSP.qry_PriceListValue);

                    var dtVariant = new Query() { OrganizationId = OrganizationId, eStatus = (int)eStatus.Active }.Select(eSP.qry_Variant);
                    //var dtProductVariant = new Query() { ProductIdIn = ProductIdIn }.Select(eSP.qry_ProductVariant);

                    //var drCategory = dtProductVariant.Select(CS.ShowInOrder + " = " + (int)eYesNo.No);
                    //if (drCategory.Length > 0)
                    //{
                    //    var dtCategory = drCategory.CopyToDataTable();
                    //    dtCategory = dtCategory.DefaultView.ToTable(true, CS.VariantValue);

                    //    var lstCategory = new List<APIProductCategory>();
                    //    foreach (DataRow drCategoryVal in dtCategory.Rows)
                    //    {
                    //        string CategoryLogo = string.Empty;
                    //        var drCategoryProduct = dtProductVariant.Select(CS.VariantValue + " = '" + drCategoryVal[CS.VariantValue].ToString() + "'");
                    //        foreach (DataRow drProduct in drCategoryProduct)
                    //        {
                    //            var drProductImage = dtProductImage.Select(CS.ProductId + " = " + drProduct[CS.ProductId].zToInt() + " AND " + CS.eProductImageType + " = " + (int)eProductImageType.Modeling);
                    //            foreach (DataRow drImage in drProductImage)
                    //            {
                    //                CategoryLogo = CU.GetFilePath(true, ePhotoSize.Original, eFolder.ProductCImage, drImage[CS.ProductImageId].ToString(), true);
                    //                break;
                    //            }

                    //            if (!CategoryLogo.zIsNullOrEmpty())
                    //                break;
                    //        }

                    //        lstCategory.Add(new APIProductCategory()
                    //        {
                    //            CategoryName = drCategoryVal[CS.VariantValue].ToString(),
                    //            CategoryLogo = CategoryLogo
                    //        });
                    //    }

                    //    objAppComm.lstProductCategory = lstCategory;
                    //}

                    var lstProductVariant = new List<APIProductVariant>();
                    foreach (DataRow drVariant in dtVariant.Rows)
                    {
                        lstProductVariant.Add(new APIProductVariant()
                        {
                            VariantId = drVariant[CS.VariantId].zToInt(),
                            VariantName = drVariant[CS.VariantName].ToString(),
                            ShowInOrder = drVariant[CS.ShowInOrder].zToInt()
                        });
                    }

                    foreach (DataRow drProduct in dtProduct.Rows)
                    {
                        decimal PurchasePrice = 0, UserPrice = 0, SalePrice = 0;
                        var drPriceList = dtPriceList.Select(CS.ProductId + " = " + drProduct[CS.ProductId].zToInt());
                        if (drPriceList.Length > 0)
                        {
                            var dtProductPrice = drPriceList.CopyToDataTable();
                            try { PurchasePrice = dtProductPrice.Compute("min([" + CS.Price + "])", string.Empty).zToDecimal().Value; }
                            catch { }
                            try { SalePrice = dtProductPrice.Compute("min([" + CS.Price + "])", string.Empty).zToDecimal().Value; }
                            catch { }
                            UserPrice = SalePrice;

                            if (!IsDefaultUser && PriceListId > 0)
                            {
                                var drProductPrice = dtProductPrice.Select(CS.PriceListId + " = " + PriceListId);
                                if (drProductPrice.Length > 0 && drProductPrice[0][CS.Price].zIsDecimal(false))
                                    UserPrice = drProductPrice[0][CS.Price].zToDecimal().Value;
                            }
                        }

                        var objProduct = new APIProduct()
                        {
                            ProductId = drProduct[CS.ProductId].zToInt().Value,
                            Weight = drProduct[CS.Weight].zToDecimal(),
                            VendorName = drProduct[CS.VendorName].ToString(),
                            NameTag = drProduct[CS.NameTag].ToString(),
                            Description = drProduct[CS.Description].ToString(),
                            ProductCode = drProduct[CS.ProductCode].ToString(),
                            VendorCode = drProduct[CS.VendorCode].ToString(),
                            eStockStatus = drProduct[CS.eStockStatus].zToInt(),
                            StockNote = drProduct[CS.StockNote].ToString(),

                            PurchasePrice = PurchasePrice,
                            UserPrice = UserPrice,
                            SalePrice = SalePrice,
                        };

                        #region Product Image

                        foreach (DataRow drProductImage in dtProductImage.Select(CS.ProductId + " = " + objProduct.ProductId))
                        {
                            string ImageURL = CU.GetFilePath(true, ePhotoSize.Original, eFolder.ProductCImage, drProductImage[CS.ProductImageId].ToString(), true);
                            objProduct.lstProductImage.Add(new APIProductImage()
                            {
                                ProductImageId = drProductImage[CS.ProductImageId].zToInt(),
                                SerialNo = drProductImage[CS.SerialNo].zToInt(),
                                eProductImageType = drProductImage[CS.eProductImageType].zToInt(),
                                ImageURL = ImageURL,
                            });
                        }

                        #endregion

                        #region Product Variant

                        //foreach (APIProductVariant objProductVariant in lstProductVariant)
                        //{
                        //    var lstProductVariantValue = new List<APIProductVariantValue>();
                        //    foreach (DataRow drProductVariant in dtProductVariant.Select(CS.ProductId + " = " + objProduct.ProductId + " AND " + CS.VariantId + " = " + objProductVariant.VariantId))
                        //    {
                        //        lstProductVariantValue.Add(new APIProductVariantValue()
                        //        {
                        //            VariantId = objProductVariant.VariantId,
                        //            ProductVariantId = drProductVariant[CS.ProductVariantId].zToInt(),
                        //            VariantValue = drProductVariant[CS.VariantValue].ToString()
                        //        });
                        //    }

                        //    //If Null Value then Create Default Array
                        //    if (lstProductVariantValue.Count == 0)
                        //    {
                        //        lstProductVariantValue.Add(new APIProductVariantValue()
                        //        {
                        //            VariantId = null,
                        //            ProductVariantId = null,
                        //            VariantValue = string.Empty,
                        //        });
                        //    }

                        //    objProduct.lstProductVariant.Add(new APIProductVariant()
                        //    {
                        //        VariantId = objProductVariant.VariantId,
                        //        OrderVariantValue = objProductVariant.OrderVariantValue,
                        //        ShowInOrder = objProductVariant.ShowInOrder,
                        //        VariantName = objProductVariant.VariantName,
                        //        lstProductVariantValue = lstProductVariantValue
                        //    });
                        //}

                        #endregion

                        #region Review/Rating

                        var dtReviewRating = new Query() { ProductId = objProduct.ProductId }.Select(eSP.qry_ReviewRating);
                        foreach (DataRow drdtReviewRating in dtReviewRating.Rows)
                        {
                            objProduct.lstReviewRating.Add(new APIReviewRating()
                            {
                                ReviewRatingId = drdtReviewRating[CS.ReviewRatingId].zToInt(),
                                UsersId = drdtReviewRating[CS.UsersId].zToInt(),
                                UsersName = drdtReviewRating[CS.UserName].ToString(),
                                Rating = drdtReviewRating[CS.Rating].zToInt(),
                                Review = drdtReviewRating[CS.Review].ToString(),
                            });
                        }

                        #endregion

                        #region Wishlitst

                        var WishlistCount = new Wishlist() { ProductId = objProduct.ProductId, UsersId = UsersId }.SelectCount();
                        objProduct.IsWishlitst = WishlistCount > 0 ? (int)eYesNo.Yes : (int)eYesNo.No;

                        #endregion

                        objAppComm.lstProduct.Add(objProduct);
                    }

                    #region Short

                    if (SortBy.HasValue && objAppComm.lstProduct.Count > 0)
                    {
                        if (SortBy == (int)eSortBy.Ascending)
                            objAppComm.lstProduct = objAppComm.lstProduct.OrderBy(o => o.SalePrice).ToList();
                        else if (SortBy == (int)eSortBy.Descending)
                            objAppComm.lstProduct = objAppComm.lstProduct.OrderByDescending(o => o.SalePrice).ToList();
                    }

                    #endregion

                    objAppComm.Success = "success";
                    objAppComm.Message = "Product Listing Successfully.";
                }
            }

            #endregion

            #region 6.ChangePasowrd

            if (API == eAPI.ChangePasowrd)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=6
                //UsersId, OldPassword, NewPassword;

                string UsersId = Request.Form["UsersId"].ToString();
                string OldPassword = Request.Form["OldPassword"].ToString();
                string NewPassword = Request.Form["NewPassword"].ToString();

                bool IsValidate = true;
                if (IsValidate && (UsersId.zIsNullOrEmpty() || UsersId.zToInt() <= 0))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter UserId.";
                    IsValidate = false;
                }

                if (IsValidate && OldPassword.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter Old Password.";
                    IsValidate = false;
                }

                if (IsValidate && NewPassword.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter New Password.";
                    IsValidate = false;
                }

                if (IsValidate && !LoginUtilities.IsValidPassword(UsersId.zToInt().Value, OldPassword))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Old Password is Incorrect.";
                    IsValidate = false;
                }

                if (IsValidate)
                {
                    LoginUtilities.ChangePassword(UsersId.zToInt().Value, NewPassword);

                    objAppComm.Success = "success";
                    objAppComm.Message = "Password Change Successfully.";
                }
            }

            #endregion

            #region 7.UpdateProfile

            if (API == eAPI.UpdateProfile)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=7
                //UsersId, Name, Email, Address, Pincode, Area, City, State, Country

                string UsersId = Request.Form[CS.UsersId].ToString();
                string Name = Request.Form[CS.Name].ToString();
                string Email = Request.Form[CS.Email].ToString();
                string Address = Request.Form[CS.Address].ToString();
                string Pincode = Request.Form[CS.Pincode].ToString();
                string Area = Request.Form["Area"].ToString();
                string City = Request.Form[CS.City].ToString();
                string State = Request.Form[CS.State].ToString();
                string Country = Request.Form[CS.Country].ToString();

                bool IsValidate = true;
                if (IsValidate && (UsersId.zIsNullOrEmpty() || UsersId.zToInt() <= 0))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass UsersId.";
                    IsValidate = false;
                }

                var lstUser = new Users() { UsersId = UsersId.zToInt() }.SelectList<Users>();
                if (IsValidate && lstUser.Count == 0)
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Users Not Found.";
                    IsValidate = false;
                }

                if (IsValidate && Name.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter Name.";
                    IsValidate = false;
                }

                if (IsValidate && !Email.zIsEmail())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter Valid Email.";
                    IsValidate = false;
                }

                if (IsValidate && Address.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter Address.";
                    IsValidate = false;
                }

                if (IsValidate && (Pincode.zIsNullOrEmpty() || !Pincode.zIsNumber() || Pincode.Length != 6))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter Valid Pincode.";
                    IsValidate = false;
                }

                if (IsValidate && Area.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter Area.";
                    IsValidate = false;
                }

                if (IsValidate && City.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter City.";
                    IsValidate = false;
                }

                if (IsValidate && State.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter State.";
                    IsValidate = false;
                }

                if (IsValidate && Country.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Enter Country.";
                    IsValidate = false;
                }


                if (IsValidate)
                {
                    var objUsers = lstUser[0];
                    int CountryId = CU.GetCountryId(Country);
                    int StateId = CU.GetStateId(CountryId, State);
                    int CityId = CU.GetCityId(StateId, City);
                    int AreaId = CU.GetAreaId(CityId, Area, Pincode);

                    new Address()
                    {
                        AddressId = objUsers.AddressId,
                        Address1 = Address,
                        Pincode = Pincode,
                        AreaId = AreaId,
                        CityId = CityId,
                        StateId = StateId,
                        CountryId = CountryId
                    }.Update();

                    new Users()
                    {
                        UsersId = objUsers.UsersId,
                        Name = Name,
                        Email = Email,
                    }.Update();

                    objAppComm.Success = "success";
                    objAppComm.Message = "Profile Change Successful";
                    objAppComm.UsersId = UsersId.zToInt().Value;
                }
            }

            #endregion

            #region 8.UserDetail

            if (API == eAPI.UserDetail)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=8&UsersId=1

                if (Request.QueryString[CS.UsersId] == null || Request.QueryString[CS.UsersId].zToInt() <= 0)
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass UsersId.";
                }
                else
                {
                    int UsersId = Request.QueryString[CS.UsersId].ToString().zToInt().Value;
                    var drUser = new Query() { UsersId = UsersId }.Select(eSP.qry_User).Rows[0];

                    var objUsers = new APIUsers()
                    {
                        UsersId = drUser[CS.UsersId].zToInt(),
                        Name = drUser[CS.Name].ToString(),
                        MobileNo = drUser[CS.MobileNo].ToString(),
                        Email = drUser[CS.Email].ToString(),
                        Description = drUser[CS.Description].ToString(),
                        PriceListId = drUser[CS.PriceListId].zToInt(),

                        Address = drUser[CS.Address].ToString(),
                        AreaName = drUser[CS.AreaName].ToString(),
                        CityName = drUser[CS.CityName].ToString(),
                        StateName = drUser[CS.StateName].ToString(),
                        CountryName = drUser[CS.CountryName].ToString(),
                        AreaPincode = drUser[CS.AreaPincode].ToString(),

                        FirmId = drUser[CS.FirmId].zToInt(),
                        FirmName = drUser[CS.FirmName].ToString(),

                        OrganizationId = drUser[CS.OrganizationId].zToInt(),
                        OrganizationName = drUser[CS.OrganizationName].ToString(),
                    };

                    objAppComm.objUsers = objUsers;

                    objAppComm.Success = "success";
                    objAppComm.Message = "User Detail Get Successful";
                }
            }

            #endregion


            #region 9.View Cart

            if (API == eAPI.ViewCart)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=9&UsersId=1
                if (Request.QueryString[CS.UsersId] == null || !Request.QueryString[CS.UsersId].ToString().zIsInteger(false))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass UserId.";
                }
                else
                {
                    int UsersId = Request.QueryString[CS.UsersId].ToString().zToInt().Value;

                    var lstAPICart = new List<APICart>();

                    var lstCart = new Cart() { UsersId = UsersId }.SelectList<Cart>();
                    var lstProductId = new List<int>();
                    foreach (Cart objCart in lstCart)
                        lstProductId.Add(objCart.ProductId.Value);

                    var lstProduct = GetAPIProduct(UsersId, CU.GetParaIn(lstProductId, true));

                    foreach (Cart objCart in lstCart)
                    {
                        var lstAPIProduct = lstProduct.Where(o => o.ProductId == objCart.ProductId).ToList();

                        lstAPICart.Add(new APICart()
                        {
                            CartId = objCart.CartId,
                            UsersId = objCart.UsersId,
                            Quantity = objCart.Quantity,
                            objProduct = lstAPIProduct.Count > 0 ? lstAPIProduct[0] : null,
                        });
                    }

                    objAppComm.lstCart = lstAPICart;
                    objAppComm.Success = "success";
                    objAppComm.Message = "Cart Listing Successfully.";
                }
            }

            #endregion

            #region 10.Add Cart

            if (API == eAPI.AddCart)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=10&ProductId=1&UsersId=1&Quantity=1

                if (Request.QueryString[CS.ProductId] == null || Request.QueryString[CS.UsersId] == null || Request.QueryString[CS.Quantity] == null)
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass ProductId, UsersId, Quantity.";
                }
                else
                {

                    bool IsValidate = true;
                    string ProductId = Request.QueryString[CS.ProductId].ToString();
                    string UsersId = Request.QueryString[CS.UsersId].ToString();
                    string Quantity = Request.QueryString[CS.Quantity].ToString();

                    if (IsValidate && (ProductId.zIsNullOrEmpty() || !ProductId.zIsInteger(false) || ProductId.zToInt() <= 0))
                    {
                        objAppComm.Success = "fail";
                        objAppComm.Message = "Please Pass ProductId.";
                        IsValidate = false;
                    }

                    if (IsValidate && (UsersId.zIsNullOrEmpty() || !UsersId.zIsInteger(false) || UsersId.zToInt() <= 0))
                    {
                        objAppComm.Success = "fail";
                        objAppComm.Message = "Please Pass UsersId.";
                        IsValidate = false;
                    }

                    if (IsValidate && (Quantity.zIsNullOrEmpty() || !Quantity.zIsInteger(false) || Quantity.zToInt() <= 0))
                    {
                        objAppComm.Success = "fail";
                        objAppComm.Message = "Please Pass Quantity.";
                        IsValidate = false;
                    }

                    if (IsValidate)
                    {
                        new Cart()
                        {
                            ProductId = ProductId.zToInt(),
                            UsersId = UsersId.zToInt(),
                            Quantity = Quantity.zToInt(),
                        }.Insert();

                        objAppComm.Success = "success";
                        objAppComm.Message = "Cart Add Successfully.";
                    }
                }
            }

            #endregion

            #region 11.Remove Cart

            if (API == eAPI.RemoveCart)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=11&CartId=1
                if (Request.QueryString[CS.CartId] == null || !Request.QueryString[CS.CartId].ToString().zIsInteger(false))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass CartId.";
                }
                else
                {
                    int CartId = Request.QueryString[CS.CartId].ToString().zToInt().Value;
                    new Cart() { CartId = CartId }.Delete();

                    objAppComm.Success = "success";
                    objAppComm.Message = "Cart Remove Successfully.";
                }
            }

            #endregion

            #region 12.Update Cart

            if (API == eAPI.UpdateCart)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=12&CartId=1&Quantity=2
                if (Request.QueryString[CS.CartId] == null || !Request.QueryString[CS.CartId].ToString().zIsInteger(false)
                    || Request.QueryString[CS.Quantity] == null || !Request.QueryString[CS.Quantity].ToString().zIsInteger(false))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass CartId or Quantity.";
                }
                else
                {
                    int CartId = Request.QueryString[CS.CartId].ToString().zToInt().Value;
                    int Quantity = Request.QueryString[CS.Quantity].ToString().zToInt().Value;
                    new Cart()
                    {
                        CartId = CartId,
                        Quantity = Quantity
                    }.Update();

                    objAppComm.Success = "success";
                    objAppComm.Message = "Cart Updated Successfully.";
                }
            }

            #endregion


            #region 13.Add Review/Rating

            if (API == eAPI.AddReviewRating)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=13
                //ProductId, UsersId, Rating, Review

                bool IsValidate = true;
                string ProductId = Request.Form[CS.ProductId].ToString();
                string UsersId = Request.Form[CS.UsersId].ToString();
                string Rating = Request.Form[CS.Rating].ToString();
                string Review = Request.Form[CS.Review].ToString();

                if (IsValidate && (ProductId.zIsNullOrEmpty() || !ProductId.zIsInteger(false) || ProductId.zToInt() <= 0))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass ProductId.";
                    IsValidate = false;
                }

                if (IsValidate && (UsersId.zIsNullOrEmpty() || !UsersId.zIsInteger(false) || UsersId.zToInt() <= 0))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass UsersId.";
                    IsValidate = false;
                }

                if (IsValidate && (Rating.zIsNullOrEmpty() || !Rating.zIsInteger(false) || Rating.zToInt() <= 0))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass Rating.";
                    IsValidate = false;
                }

                if (IsValidate && Review.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass Review.";
                    IsValidate = false;
                }

                if (IsValidate)
                {
                    new ReviewRating()
                    {
                        ProductId = ProductId.zToInt(),
                        UsersId = UsersId.zToInt(),
                        Rating = Rating.zToInt(),
                        Review = Review,
                    }.Insert();

                    objAppComm.Success = "success";
                    objAppComm.Message = "Review / Rating Add Successfully.";
                }

            }

            #endregion

            #region 14.Remove Review/Rating

            if (API == eAPI.RemoveReviewRating)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=14&ReviewRatingId=1
                if (Request.QueryString[CS.ReviewRatingId] == null || !Request.QueryString[CS.ReviewRatingId].ToString().zIsInteger(false))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass ReviewRatingId.";
                }
                else
                {
                    int ReviewRatingId = Request.QueryString[CS.ReviewRatingId].ToString().zToInt().Value;
                    new ReviewRating() { ReviewRatingId = ReviewRatingId }.Delete();

                    objAppComm.Success = "success";
                    objAppComm.Message = "Review / Rating Remove Successfully.";
                }
            }

            #endregion


            #region 15.View Wishlist

            if (API == eAPI.ViewWishlist)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=15&UsersId=1
                if (Request.QueryString[CS.UsersId] == null || !Request.QueryString[CS.UsersId].ToString().zIsInteger(false))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass UserId.";
                }
                else
                {
                    int UsersId = Request.QueryString[CS.UsersId].ToString().zToInt().Value;

                    var lstAPIWishlist = new List<APIWishlist>();

                    var lstWishlist = new Wishlist() { UsersId = UsersId }.SelectList<Wishlist>();
                    var lstProductId = new List<int>();
                    foreach (Wishlist objWishlist in lstWishlist)
                        lstProductId.Add(objWishlist.ProductId.Value);

                    var lstProduct = GetAPIProduct(UsersId, CU.GetParaIn(lstProductId, true));

                    foreach (Wishlist objWishlist in lstWishlist)
                    {
                        var lstAPIProduct = lstProduct.Where(o => o.ProductId == objWishlist.ProductId).ToList();

                        lstAPIWishlist.Add(new APIWishlist()
                        {
                            WishlistId = objWishlist.WishlistId,
                            UsersId = objWishlist.UsersId,
                            objProduct = lstAPIProduct.Count > 0 ? lstAPIProduct[0] : null,
                        });
                    }

                    objAppComm.lstWishlist = lstAPIWishlist;
                    objAppComm.Success = "success";
                    objAppComm.Message = "Wishlist Listing Successfully.";
                }
            }

            #endregion

            #region 16.Add Wishlist

            if (API == eAPI.AddWishlist)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=16&ProductId=1&UsersId=1

                if (Request.QueryString[CS.ProductId] == null || Request.QueryString[CS.UsersId] == null)
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass ProductId, UsersId.";
                }
                else
                {

                    bool IsValidate = true;
                    string ProductId = Request.QueryString[CS.ProductId].ToString();
                    string UsersId = Request.QueryString[CS.UsersId].ToString();

                    if (IsValidate && (ProductId.zIsNullOrEmpty() || !ProductId.zIsInteger(false) || ProductId.zToInt() <= 0))
                    {
                        objAppComm.Success = "fail";
                        objAppComm.Message = "Please Pass ProductId.";
                        IsValidate = false;
                    }

                    if (IsValidate && (UsersId.zIsNullOrEmpty() || !UsersId.zIsInteger(false) || UsersId.zToInt() <= 0))
                    {
                        objAppComm.Success = "fail";
                        objAppComm.Message = "Please Pass UsersId.";
                        IsValidate = false;
                    }

                    if (IsValidate)
                    {
                        new Wishlist()
                        {
                            ProductId = ProductId.zToInt(),
                            UsersId = UsersId.zToInt(),
                        }.Insert();

                        objAppComm.Success = "success";
                        objAppComm.Message = "Wishlist Add Successfully.";
                    }
                }
            }

            #endregion

            #region 17.Remove Wishlist

            if (API == eAPI.RemoveWishlist)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=17&WishlistId=1
                if (Request.QueryString[CS.WishlistId] == null || !Request.QueryString[CS.WishlistId].ToString().zIsInteger(false))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass WishlistId.";
                }
                else
                {
                    int WishlistId = Request.QueryString[CS.WishlistId].ToString().zToInt().Value;
                    new Wishlist() { WishlistId = WishlistId }.Delete();

                    objAppComm.Success = "success";
                    objAppComm.Message = "Wishlist Remove Successfully.";
                }
            }

            #endregion


            #region 18.View Order

            if (API == eAPI.ViewOrder)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=18&UsersId=1
                if (Request.QueryString[CS.UsersId] == null || !Request.QueryString[CS.UsersId].ToString().zIsInteger(false))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass UserId.";
                }
                else
                {
                    int UsersId = Request.QueryString[CS.UsersId].ToString().zToInt().Value;

                    var dtOrder = new Query() { UsersId = UsersId }.Select(eSP.qry_Orders);

                    var lstOrderId = new List<int>();
                    foreach (DataRow drOrder in dtOrder.Rows)
                        lstOrderId.Add(drOrder[CS.OrdersId].zToInt().Value);

                    var dtOrderProduct = new Query() { OrdersIdIn = CU.GetParaIn(lstOrderId, true) }.Select(eSP.qry_OrderProduct);


                    var lstAPIOrder = new List<APIOrder>();
                    foreach (DataRow drOrder in dtOrder.Rows)
                    {
                        var lstProduct = new List<APIProduct>();
                        foreach (DataRow drOrderProduct in dtOrderProduct.Select(CS.OrdersId + " = " + drOrder[CS.OrdersId].zToInt()))
                        {
                            #region Image

                            var lstProductImage = new List<APIProductImage>();
                            lstProductImage.Add(new APIProductImage()
                            {
                                ProductImageId = drOrderProduct[CS.ProductImageId].zToInt(),
                                ImageURL = CU.GetFilePath(true, ePhotoSize.Original, eFolder.ProductCImage, drOrderProduct[CS.ProductImageId].ToString(), true),
                            });

                            #endregion

                            #region Variant

                            //var lstProductVariant = new List<APIProductVariant>();
                            //foreach (DataRow drOrderProductVariant in dtOrderProductVariant.Select(CS.OrderProductId + " = " + drOrderProduct[CS.OrderProductId].zToInt()))
                            //{
                            //    lstProductVariant.Add(new APIProductVariant()
                            //    {
                            //        VariantId = drOrderProductVariant[CS.VariantId].zToInt(),
                            //        VariantName = drOrderProductVariant[CS.VariantName].ToString(),
                            //        OrderVariantValue = drOrderProductVariant[CS.VariantValue].ToString()
                            //    });
                            //}

                            #endregion

                            lstProduct.Add(new APIProduct()
                            {
                                ProductId = drOrderProduct[CS.ProductId].zToInt(),
                                ProductCode = drOrderProduct[CS.ProductCode].ToString(),

                                OrderProductId = drOrderProduct[CS.OrderProductId].zToInt(),
                                OrderQuantity = drOrderProduct[CS.Quantity].zToInt(),
                                OrderPurchasePrice = drOrderProduct[CS.PurchasePrice].zToDecimal(),
                                OrderUserPrice = drOrderProduct[CS.UserPrice].zToDecimal(),
                                OrderSalePrice = drOrderProduct[CS.SalePrice].zToDecimal(),
                                OrderWeight = drOrderProduct[CS.Weight].zToDecimal(),
                                OrderDescription = drOrderProduct[CS.Description].ToString(),

                                lstProductImage = lstProductImage,
                                //lstProductVariant = lstProductVariant,
                            });
                        }

                        lstAPIOrder.Add(new APIOrder()
                        {
                            OrdersId = drOrder[CS.OrdersId].zToInt(),
                            Date = Convert.ToDateTime(drOrder[CS.Date]).ToString(CS.ddMMyyyy),
                            Weight = drOrder[CS.Weight].zToDecimal(),
                            AWBNo = drOrder[CS.AWBNo].ToString(),
                            ReturnAWBNo = drOrder[CS.ReturnAWBNo].ToString(),
                            ShipCharge = drOrder[CS.ShipCharge].zToDecimal(),
                            FirmShipCharge = drOrder[CS.FirmShipCharge].zToDecimal(),
                            CustomerShipCharge = drOrder[CS.CustomerShipCharge].zToDecimal(),
                            PurchasePrice = drOrder[CS.PurchasePrice].zToDecimal(),
                            UserPrice = drOrder[CS.UserPrice].zToDecimal(),
                            SalePrice = drOrder[CS.SalePrice].zToDecimal(),
                            Description = drOrder[CS.Description].ToString(),

                            TotalAmount = drOrder[CS.TotalAmount].zToDecimal(),
                            UserCommition = drOrder[CS.UserCommition].zToDecimal(),
                            FirmCommition = drOrder[CS.FirmCommition].zToDecimal(),

                            CourierName = drOrder[CS.CourierName].ToString(),
                            StatusName = drOrder[CS.StatusName].ToString(),
                            eStatusType = drOrder[CS.eStatusType].zToInt(),

                            CustomerName = drOrder[CS.Name].ToString(),
                            WhatsAppNo = drOrder[CS.WhatsAppNo].ToString(),
                            MobileNo = drOrder[CS.MobileNo].ToString(),
                            FullAddress = drOrder[CS.FullAddress].ToString(),

                            lstProduct = lstProduct,
                            //objProduct = lstAPIProduct.Count > 0 ? lstAPIProduct[0] : null,
                        });
                    }

                    objAppComm.lstOrder = lstAPIOrder;
                    objAppComm.Success = "success";
                    objAppComm.Message = "Order Listing Successfully.";
                }
            }

            #endregion

            #region 19.Add Order

            if (API == eAPI.AddOrder)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=19
                //{"OrdersId":null,"UsersId":null,"eCourierType":null,"CustomerShipCharge":null,"SalePrice":null,"Description":null,"CustomerName":null,"WhatsAppNo":null,"MobileNo":null,"Address":null,"Pincode":null,"CityName":null,"StateName":null,"CountryName":null,"lstOrderProduct":[{"ProductId":null,"Quantity":null,"SalePrice":null,"ProductImageId":null,"Description":null,"lstOrderProductVariant":[{"ProductVariantId":null}]}]}

                #region Fack Create Object

                //var objAddOrder = new APIAddOrder()
                //{
                //    UsersId = 1,
                //    eCourierType = (int)eCourierType.COD,
                //    CustomerShipCharge = 120,
                //    Description = "test",

                //    CustomerName = "Jatin Lathiya",
                //    WhatsAppNo = "7878129872",
                //    MobileNo = "7878129872",
                //    Address = "54, Balwant nager, singanporecharrasta, katargam",
                //    Pincode = 395004,
                //    CityName = "Surat",
                //    StateName = "Gujrat",
                //    CountryName = "India",
                //};

                //var lstOrderProductVariant = new List<APIOrderProductVariant>();
                //lstOrderProductVariant.Add(new APIOrderProductVariant()
                //{
                //    ProductVariantId = 9
                //});

                //var lstAPIOrderProduct = new List<APIOrderProduct>();
                //lstAPIOrderProduct.Add(new APIOrderProduct()
                //{
                //    ProductId = 2,
                //    Quantity = 2,
                //    SalePrice = 500,
                //    ProductImageId = 5,
                //    Description = "Test P Description",

                //    lstOrderProductVariant = lstOrderProductVariant
                //});

                //objAddOrder.lstOrderProduct = lstAPIOrderProduct;


                //string JSonsAPIAddOrder = objAddOrder.zSerializeJSON();

                #endregion

                string JSonsAPIAddOrder = Request.Form["Order"].ToString();
                var objAPIAddOrder = JSonsAPIAddOrder.zDeserializeJSON<APIAddOrder>();

                #region Validate

                bool IsValidate = true;
                if (IsValidate && (!objAPIAddOrder.UsersId.HasValue || objAPIAddOrder.UsersId <= 0))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass UsersId.";
                    IsValidate = false;
                }

                if (IsValidate && (!objAPIAddOrder.eCourierType.HasValue || objAPIAddOrder.eCourierType <= 0))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass eCourierType.";
                    IsValidate = false;
                }

                if (IsValidate && !objAPIAddOrder.CustomerShipCharge.HasValue)
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass Customer ShipCharge.";
                    IsValidate = false;
                }

                if (IsValidate && objAPIAddOrder.CustomerName.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass Customer Name.";
                    IsValidate = false;
                }

                if (IsValidate && objAPIAddOrder.WhatsAppNo.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass WhatsAppNo.";
                    IsValidate = false;
                }

                if (IsValidate && objAPIAddOrder.MobileNo.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass MobileNo.";
                    IsValidate = false;
                }

                if (IsValidate && objAPIAddOrder.Address.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass Address.";
                    IsValidate = false;
                }

                if (IsValidate && objAPIAddOrder.Pincode.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass Pincode.";
                    IsValidate = false;
                }

                if (IsValidate && objAPIAddOrder.CityName.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass CityName.";
                    IsValidate = false;
                }

                if (IsValidate && objAPIAddOrder.StateName.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass StateName.";
                    IsValidate = false;
                }

                if (IsValidate && objAPIAddOrder.CountryName.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass CountryName.";
                    IsValidate = false;
                }

                if (IsValidate && objAPIAddOrder.lstOrderProduct.Count == 0)
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass Order Product.";
                    IsValidate = false;
                }

                if (IsValidate)
                {
                    #region Order Product

                    foreach (APIOrderProduct objOrderProduct in objAPIAddOrder.lstOrderProduct)
                    {
                        if (IsValidate && (!objOrderProduct.ProductId.HasValue || objOrderProduct.ProductId <= 0))
                        {
                            objAppComm.Success = "fail";
                            objAppComm.Message = "Please Pass ProductId.";
                            IsValidate = false;
                        }

                        if (IsValidate && (!objOrderProduct.Quantity.HasValue || objOrderProduct.Quantity <= 0))
                        {
                            objAppComm.Success = "fail";
                            objAppComm.Message = "Please Pass Quantity.";
                            IsValidate = false;
                        }

                        if (IsValidate && (!objOrderProduct.SalePrice.HasValue || objOrderProduct.SalePrice <= 0))
                        {
                            objAppComm.Success = "fail";
                            objAppComm.Message = "Please Pass Sale Price.";
                            IsValidate = false;
                        }
                    }

                    #endregion
                }

                #endregion Validate

                #region Insert

                if (IsValidate)
                {
                    var objUser = new Users() { UsersId = objAPIAddOrder.UsersId }.SelectList<Users>()[0];

                    #region Customer

                    var objCustomer = new Customer()
                    {
                        Name = objAPIAddOrder.CustomerName,
                        WhatsAppNo = objAPIAddOrder.WhatsAppNo,
                        MobileNo = objAPIAddOrder.MobileNo,

                        Address = objAPIAddOrder.Address,
                        Pincode = objAPIAddOrder.Pincode,
                        CityName = objAPIAddOrder.CityName,
                        StateName = objAPIAddOrder.StateName,
                        CountryName = objAPIAddOrder.CountryName,
                        UsersId = objAPIAddOrder.UsersId,
                        eStatus = (int)eStatus.Active,
                    };

                    var lstCustomer = new Customer() { UsersId = objAPIAddOrder.UsersId, MobileNo = objAPIAddOrder.MobileNo }.SelectList<Customer>();
                    if (lstCustomer.Count > 0)
                        objCustomer.CustomerId = lstCustomer[0].CustomerId;

                    if (objCustomer.CustomerId.HasValue && objCustomer.CustomerId > 0)
                        objCustomer.Update();
                    else
                        objCustomer.CustomerId = objCustomer.Insert();

                    #endregion

                    #region Order

                    var objOrder = new Orders()
                    {
                        UsersId = objAPIAddOrder.UsersId,
                        CustomerId = objCustomer.CustomerId,
                        OrderStatusId = CU.GetOrderStatusId((int)eStatusType.Draft, string.Empty),
                        Date = IndianDateTime.Today,
                        eCourierType = objAPIAddOrder.eCourierType,
                        eZone = objCustomer.StateName.ToLower() == "gujarat" ? (int)eZone.InState : (int)eZone.OutOfState,
                        AWBNo = "",
                        ReturnAWBNo = "",
                        CustomerShipCharge = objAPIAddOrder.CustomerShipCharge,
                        OrderSourceId = 0,
                        Description = objAPIAddOrder.Description,
                    };

                    objOrder.OrdersId = objOrder.Insert();

                    #endregion

                    #region Order Product

                    var lstProductId = new List<int>();
                    foreach (APIOrderProduct objAPIOrderProduct in objAPIAddOrder.lstOrderProduct)
                        lstProductId.Add(objAPIOrderProduct.ProductId.Value);

                    var dtProduct = new Query() { ProductIdIn = CU.GetParaIn(lstProductId, true) }.Select(eSP.qry_Product);

                    decimal TotalWeight = 0, TotalPurchasePrice = 0, TotalUserPrice = 0, TotalSalePrice = 0;
                    int TotalQuantity = 0;

                    foreach (APIOrderProduct objAPIOrderProduct in objAPIAddOrder.lstOrderProduct)
                    {
                        var drProduct = dtProduct.Select(CS.ProductId + " = " + objAPIOrderProduct.ProductId);

                        decimal UserPrice = 0;
                        if (objUser.PriceListId > 0)
                        {
                            var lstPriceListValue = new PriceListValue() { ProductId = objAPIOrderProduct.ProductId, PriceListId = objUser.PriceListId, }.SelectList<PriceListValue>();
                            if (lstPriceListValue.Count > 0 && lstPriceListValue[0].Price.HasValue)
                                UserPrice = lstPriceListValue[0].Price.Value;
                        }

                        if (UserPrice == 0)
                        {
                            eDesignation Designation = CU.GetDesiToeDesi(objUser.DesignationId.Value);
                            if (Designation == eDesignation.SystemAdmin || Designation == eDesignation.Admin)
                                UserPrice = drProduct[0][CS.PurchasePrice].zToDecimal().HasValue ? drProduct[0][CS.PurchasePrice].zToDecimal().Value : 0;
                            else
                                UserPrice = CU.GetProductPrice(objUser.FirmId.Value, objAPIOrderProduct.ProductId.Value);
                        }

                        var objOrderProduct = new OrderProduct()
                        {
                            OrdersId = objOrder.OrdersId,
                            ProductId = objAPIOrderProduct.ProductId,
                            Quantity = objAPIOrderProduct.Quantity,
                            PurchasePrice = drProduct[0][CS.PurchasePrice].zToDecimal().HasValue ? drProduct[0][CS.PurchasePrice].zToDecimal() : 0,
                            UserPrice = UserPrice,
                            SalePrice = objAPIOrderProduct.SalePrice,
                            Weight = drProduct[0][CS.Weight].zToDecimal().HasValue ? (drProduct[0][CS.Weight].zToDecimal() * objAPIOrderProduct.Quantity) : ((decimal)0.5 * objAPIOrderProduct.Quantity),
                            ProductImageId = objAPIOrderProduct.ProductImageId,
                            Description = objAPIOrderProduct.Description,
                        };

                        TotalWeight += objOrderProduct.Weight.zToDecimal().Value * objOrderProduct.Quantity.Value;
                        TotalQuantity += objOrderProduct.Quantity.zToInt().Value;
                        TotalPurchasePrice += objOrderProduct.PurchasePrice.zToDecimal().Value * objOrderProduct.Quantity.Value;
                        TotalUserPrice += objOrderProduct.UserPrice.zToDecimal().Value * objOrderProduct.Quantity.Value;
                        TotalSalePrice += objOrderProduct.SalePrice.zToDecimal().Value * objOrderProduct.Quantity.Value;

                        objOrderProduct.OrderProductId = objOrderProduct.Insert();

                        #region Order Product Variant

                        foreach (APIOrderProductVariant objAPIOrderProductVariant in objAPIOrderProduct.lstOrderProductVariant)
                        {
                            //var objOrderProductVariant = new OrderProductVariant()
                            //{
                            //    OrderProductId = objOrderProduct.OrderProductId,
                            //    ProductVariantId = objAPIOrderProductVariant.ProductVariantId,
                            //};

                            //objOrderProductVariant.Insert();
                        }

                        #endregion
                    }

                    #endregion

                    #region Update Order Detail

                    #region Set Courier & Shipping Charge

                    int CourierId = 0;
                    decimal ShipCharge = 0, FirmShipCharge = 0;
                    if (TotalWeight > 0)
                    {
                        var lstCourierId = new List<int>();

                        bool IsCOD = true;
                        var dtServiceAvailability = new Query()
                        {
                            //FirmId = lblFirmId.zToInt(),
                            Pincode = objCustomer.Pincode,
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

                            lstCourierId.Add(drServiceAvailability[CS.CourierId].zToInt().Value);
                        }

                        if (!IsSetPost)
                        {
                            var lstCurior = new Courier()
                            {
                                //FirmId = objUser.FirmId,
                                IsPost = (int)eYesNo.Yes,
                                eStatus = (int)eStatus.Active
                            }.SelectList<Courier>();

                            if (lstCurior.Count > 0)
                            {
                                lstCourierId.Add(lstCurior[0].CourierId.Value);
                            }
                        }

                        var dtShipCharge = new Query()
                        {
                            CourierIdIn = CU.GetParaIn(lstCourierId, false),
                            eCourierType = objAPIAddOrder.eCourierType,
                            eZone = objOrder.eZone,
                        }.Select(eSP.qry_ShippingCharge);

                        bool SCWeightWise = false;
                        if (SCWeightWise)
                        {
                            #region Weight Wise

                            var drShipCharge = dtShipCharge.Select(CS.Weight + " >= " + TotalWeight + " AND " + CS.ShipCharge + " > 0 AND " + CS.FirmShipCharge + " > 0", CS.Weight + ", " + CS.FirmShipCharge);
                            if (drShipCharge.Length > 0)
                            {
                                CourierId = drShipCharge[0][CS.CourierId].zToInt().Value;

                                ShipCharge = drShipCharge[0][CS.ShipCharge].zToDecimal().Value;
                                FirmShipCharge = drShipCharge[0][CS.FirmShipCharge].zToDecimal().Value;
                            }

                            #endregion
                        }
                        else
                        {
                            #region Quantity Wise

                            decimal CheckCharge = 0;

                            foreach (int Id in lstCourierId)
                            {
                                var dr = dtShipCharge.Select(CS.CourierId + " = " + Id + " AND " + CS.ShipCharge + " > 0 AND " + CS.FirmShipCharge + " > 0", CS.Weight + ", " + CS.FirmShipCharge);
                                if (dr.Length >= TotalQuantity)
                                {
                                    int ItemIndex = TotalQuantity - 1;

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

                            var drShipCharge = dtShipCharge.Select(CS.CourierId + " = " + CourierId + " AND " + CS.ShipCharge + " > 0 AND " + CS.FirmShipCharge + " > 0", CS.Weight);
                            if (drShipCharge.Length > 0)
                            {
                                int ItemIndex = TotalQuantity - 1;
                                if (drShipCharge.Length < TotalQuantity)
                                    ItemIndex = drShipCharge.Length - 1;

                                CourierId = drShipCharge[ItemIndex][CS.CourierId].zToInt().Value;

                                ShipCharge = drShipCharge[ItemIndex][CS.ShipCharge].zToDecimal().Value;
                                FirmShipCharge = drShipCharge[ItemIndex][CS.FirmShipCharge].zToDecimal().Value;
                            }

                            #endregion
                        }
                    }

                    #endregion

                    new Orders()
                    {
                        OrdersId = objOrder.OrdersId,
                        PurchasePrice = TotalPurchasePrice,
                        UserPrice = TotalUserPrice,
                        SalePrice = TotalSalePrice,
                        Weight = TotalWeight,
                        CourierId = CourierId,
                        ShipCharge = ShipCharge,
                        FirmShipCharge = FirmShipCharge,
                    }.Update();

                    #endregion

                    objAppComm.Success = "success";
                    objAppComm.Message = "Order Add Successfully.";
                }

                #endregion
            }

            #endregion

            #region 20.Banner

            if (API == eAPI.Banner)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=20
                objAppComm.lstBanner.Add(CU.GetFilePath(true, ePhotoSize.Original, eFolder.Banner, "1", false));
                objAppComm.lstBanner.Add(CU.GetFilePath(true, ePhotoSize.Original, eFolder.Banner, "2", false));
                objAppComm.lstBanner.Add(CU.GetFilePath(true, ePhotoSize.Original, eFolder.Banner, "3", false));
                objAppComm.lstBanner.Add(CU.GetFilePath(true, ePhotoSize.Original, eFolder.Banner, "4", false));
                objAppComm.lstBanner.Add(CU.GetFilePath(true, ePhotoSize.Original, eFolder.Banner, "5", false));

                objAppComm.Success = "success";
                objAppComm.Message = "Order Add Successfully.";
            }

            #endregion

            #region 21.Call Type

            if (API == eAPI.CallType)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=21&UsersId=1
                if (Request.QueryString[CS.UsersId] == null || !Request.QueryString[CS.UsersId].ToString().zIsInteger(false))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass UserId.";
                }
                else
                {
                    int UsersId = Request.QueryString[CS.UsersId].ToString().zToInt().Value;

                    var objUser = new Users() { UsersId = UsersId }.SelectList<Users>()[0];

                    var lstAPICallType = new List<APICallType>();
                    var lstCallType = new CallType() { FirmId = objUser.FirmId, eStatus = (int)eStatus.Active }.SelectList<CallType>();
                    foreach (CallType objCallType in lstCallType)
                    {
                        lstAPICallType.Add(new APICallType()
                        {
                            CallTypeId = objCallType.CallTypeId,
                            CallTypeName = objCallType.CallTypeName,
                            SMSText = objCallType.SMSText,
                            IsSendSMS = objCallType.IsSendSMS,
                        });
                    }

                    objAppComm.lstAPICallType = lstAPICallType;
                    objAppComm.Success = "success";
                    objAppComm.Message = "Call Type Listing Successfully.";
                }
            }

            #endregion

            #region 22.Call History

            if (API == eAPI.CallHistory)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=22&UsersId=1
                if (Request.QueryString[CS.UsersId] == null || !Request.QueryString[CS.UsersId].ToString().zIsInteger(false))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass UserId.";
                }
                else
                {
                    #region Set Search Value

                    int? SearchUsersId = null, SearchCallTypeId = null, SearchCallDirection = null;
                    DateTime? SearchFromDate = null, SearchToDate = null;

                    try { SearchUsersId = Request.QueryString["SearchUsersId"].zToInt(); }
                    catch { }

                    try { SearchCallTypeId = Request.QueryString["SearchCallTypeId"].zToInt(); }
                    catch { }

                    try { SearchCallDirection = Request.QueryString["SearchCallDirection"].zToInt(); }
                    catch { }

                    try { SearchFromDate = Request.QueryString["SearchFromDate"].zToDate(); }
                    catch { }

                    try { SearchToDate = Request.QueryString["SearchToDate"].zToDate(); }
                    catch { }

                    if (SearchUsersId.zToInt() <= 0)
                        SearchUsersId = null;

                    if (SearchCallTypeId.zToInt() <= 0)
                        SearchCallTypeId = null;

                    if (SearchCallDirection.zToInt() <= 0)
                        SearchCallDirection = null;

                    #endregion

                    int UsersId = Request.QueryString[CS.UsersId].ToString().zToInt().Value;
                

                    var objUser = new Users() { UsersId = UsersId }.SelectList<Users>()[0];

                    var lstAPICallHistory = new List<APICallHistory>();
                    var dtCallHistory = new Query()
                    {
                        FirmId = objUser.FirmId,
                        eStatus = (int)eStatus.Active,

                        UsersId = SearchUsersId,
                        CallTypeId = SearchCallTypeId,
                        eCallDirection = SearchCallDirection,
                        FromDate = SearchFromDate,
                        ToDate = SearchToDate.HasValue ? SearchToDate.Value.AddDays(1).AddSeconds(-1) : (DateTime?)null
                    }.Select(eSP.qry_CallHistory);

                    foreach (DataRow drCallHistory in dtCallHistory.Rows)
                    {
                        lstAPICallHistory.Add(new APICallHistory()
                        {
                            CallHistoryId = drCallHistory[CS.CallHistoryId].zToInt(),
                            UsersId = drCallHistory[CS.UsersId].zToInt(),
                            UserProfile = CU.GetFilePath(true, ePhotoSize.Original, eFolder.UserProfile, drCallHistory[CS.UsersId].ToString(), true),
                            UsersName = drCallHistory[CS.Name].ToString(),
                            CallTypeId = drCallHistory[CS.CallTypeId].zToInt(),
                            CallTypeName = drCallHistory[CS.CallTypeName].ToString(),
                            MobileNo = drCallHistory[CS.MobileNo].ToString(),
                            Time = Convert.ToDateTime(drCallHistory[CS.Time]).ToString("dd MMM yy HH:mm"),
                            Duration = drCallHistory[CS.Duration].zToInt(),
                            eCallDirection = drCallHistory[CS.eCallDirection].zToInt(),
                            FilePath = CU.GetFilePath(true, ePhotoSize.Original, eFolder.CallRecording, drCallHistory[CS.CallHistoryId].ToString(), drCallHistory[CS.Extension].ToString())
                        });
                    }

                    objAppComm.lstAPICallHistory = lstAPICallHistory;
                    objAppComm.Success = "success";
                    objAppComm.Message = "Call History Listing Successfully.";
                }
            }

            #endregion

            #region 23.Add Call History

            if (API == eAPI.AddCallHistory)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=23
                //set Post in CallHistory Variable
                //{"CallHistoryId":null,"UsersId":1,"UsersName":null,"CallTypeId":1,"CallTypeName":null,"MobileNo":"7878129873","Time":"26-04-2019 12:21","Duration":540,"eCallDirection":2}

                #region Fack Create Object

                //var objAddCallHistory = new APICallHistory()
                //{
                //    UsersId = 1,
                //    CallTypeId = 1,
                //    MobileNo = "7984300747",
                //    Time = "07-05-2019 10:21",
                //    Duration = 21,
                //    eCallDirection = 2,
                //    RecordData = @"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\u003d\n".Replace("\n"," ")
                //};

                //string JSonsAPIAddCallHistory = objAddCallHistory.zSerializeJSON();

                #endregion

                string JSonsAPIAddCallHistory = Request.Form["CallHistory"].ToString();
                var objAPIAddCallHistory = JSonsAPIAddCallHistory.zDeserializeJSON<APICallHistory>();

                #region Validate

                bool IsValidate = true;
                if (IsValidate && (!objAPIAddCallHistory.UsersId.HasValue || objAPIAddCallHistory.UsersId <= 0))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass UsersId.";
                    IsValidate = false;
                }

                if (IsValidate && (!objAPIAddCallHistory.CallTypeId.HasValue || objAPIAddCallHistory.CallTypeId <= 0))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass CallTypeId.";
                    IsValidate = false;
                }

                if (IsValidate && objAPIAddCallHistory.MobileNo.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass MobileNo.";
                    IsValidate = false;
                }

                if (IsValidate && objAPIAddCallHistory.Time.zIsNullOrEmpty())
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass Time.";
                    IsValidate = false;
                }

                if (IsValidate && !objAPIAddCallHistory.Time.zToDateTime24().HasValue)
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass Valid Time.";
                    IsValidate = false;
                }

                if (IsValidate && !objAPIAddCallHistory.Duration.HasValue)
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass Duration.";
                    IsValidate = false;
                }

                if (IsValidate && (!objAPIAddCallHistory.eCallDirection.HasValue || objAPIAddCallHistory.eCallDirection <= 0))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass eCallDirection.";
                    IsValidate = false;
                }

                #endregion Validate

                #region Insert

                if (IsValidate)
                {
                    #region Add CallHistory

                    int CallHistoryId = new CallHistory()
                    {
                        UsersId = objAPIAddCallHistory.UsersId,
                        CallTypeId = objAPIAddCallHistory.CallTypeId,
                        MobileNo = objAPIAddCallHistory.MobileNo,
                        Time = objAPIAddCallHistory.Time.zToDateTime24(),
                        Duration = objAPIAddCallHistory.Duration,
                        eCallDirection = objAPIAddCallHistory.eCallDirection,
                        Extension = ".aac",
                        eStatus = (int)eStatus.Active
                    }.Insert();

                    try
                    {
                        Byte[] bytes = Convert.FromBase64String(objAPIAddCallHistory.RecordData);
                        File.WriteAllBytes(Server.MapPath(CU.GetFolderPath(false, ePhotoSize.Original, eFolder.CallRecording)) + "\\CallRecording_" + CallHistoryId.ToString() + ".aac", bytes);
                    }
                    catch (Exception e)
                    {
                        FileWriteW.Error("CallRecording_Audio_Upload_" + CallHistoryId.ToString(), true, true, "", e.Message, e.Source, e.StackTrace);
                    }


                    var objCallType = new CallType() { CallTypeId = objAPIAddCallHistory.CallTypeId }.SelectList<CallType>()[0];
                    if (objCallType.IsSendSMS == (int)eYesNo.Yes)
                        CU.SendSMS(objAPIAddCallHistory.UsersId.Value, objCallType.SMSText, objAPIAddCallHistory.MobileNo, false, CallHistoryId, (int)eComponentType.CallHistory);

                    #endregion

                    objAppComm.Success = "success";
                    objAppComm.Message = "CallHistory Add Successfully.";
                }

                #endregion
            }

            #endregion

            #region 24.Users

            if (API == eAPI.lstUsers)
            {
                //http://shop.octfis.com/API/APIShopingPortal.aspx?API=24&FirmId=1
                if (Request.QueryString[CS.FirmId] == null || !Request.QueryString[CS.FirmId].ToString().zIsInteger(false))
                {
                    objAppComm.Success = "fail";
                    objAppComm.Message = "Please Pass FirmId.";
                }
                else
                {
                    int FirmId = Request.QueryString[CS.FirmId].ToString().zToInt().Value;

                    var lstAPIUsers = new List<APIUsers>();

                    var lstUsers = new Users() { FirmId = FirmId }.SelectList<Users>();
                    foreach (Users objUsers in lstUsers)
                    {
                        lstAPIUsers.Add(new APIUsers()
                        {
                            UsersId = objUsers.UsersId,
                            Name = objUsers.Name,
                        });
                    }

                    objAppComm.lstAPIUsers = lstAPIUsers;
                    objAppComm.Success = "success";
                    objAppComm.Message = "User Listing Successfully.";
                }
            }

            #endregion
        }
        catch (Exception ex)
        {
            objAppComm.Success += "fail";
            objAppComm.Message += ex.Message;
        }

        string JSon = objAppComm.zSerializeJSON();
        return JSon.Replace(" 12:00:00 AM", "");
    }

    private List<APIProduct> GetAPIProduct(int UsersId, string ProductIdIn)
    {
        var lstProduct = new List<APIProduct>();

        #region Product

        var dtUser = new Query() { UsersId = UsersId }.Select(eSP.qry_User).Rows[0];
        int OrganizationId = dtUser[CS.OrganizationId].zToInt().Value;
        int PriceListId = dtUser[CS.PriceListId].zToInt().Value;

        var dtProduct = new Query()
        {
            OrganizationId = OrganizationId,
            ProductIdIn = ProductIdIn,
            eStatus = (int)eStatus.Active
        }.Select(eSP.qry_Product);

        var dtProductImage = new Query() { ProductIdIn = ProductIdIn }.Select(eSP.qry_ProductImage);

        var dtPriceList = new Query()
        {
            ProductIdIn = ProductIdIn,
            OrganizationId = OrganizationId,
        }.Select(eSP.qry_PriceListValue);

        var dtVariant = new Query() { OrganizationId = OrganizationId, eStatus = (int)eStatus.Active }.Select(eSP.qry_Variant);
        //var dtProductVariant = new Query() { ProductIdIn = ProductIdIn }.Select(eSP.qry_ProductVariant);

        var lstProductVariant = new List<APIProductVariant>();
        foreach (DataRow drVariant in dtVariant.Rows)
        {
            lstProductVariant.Add(new APIProductVariant()
            {
                VariantId = drVariant[CS.VariantId].zToInt(),
                VariantName = drVariant[CS.VariantName].ToString(),
                ShowInOrder = drVariant[CS.ShowInOrder].zToInt()
            });
        }

        foreach (DataRow drProduct in dtProduct.Rows)
        {

            decimal PurchasePrice = 0, UserPrice = 0, SalePrice = 0;
            var drPriceList = dtPriceList.Select(CS.ProductId + " = " + drProduct[CS.ProductId].zToInt());
            if (drPriceList.Length > 0)
            {
                var dtProductPrice = drPriceList.CopyToDataTable();
                try { PurchasePrice = dtProductPrice.Compute("min([" + CS.Price + "])", string.Empty).zToDecimal().Value; }
                catch { }
                try { SalePrice = dtProductPrice.Compute("min([" + CS.Price + "])", string.Empty).zToDecimal().Value; }
                catch { }
                UserPrice = SalePrice;

                if (PriceListId > 0)
                {
                    var drProductPrice = dtProductPrice.Select(CS.PriceListId + " = " + PriceListId);
                    if (drProductPrice.Length > 0 && drProductPrice[0][CS.Price].zIsDecimal(false))
                        UserPrice = drProductPrice[0][CS.Price].zToDecimal().Value;
                }
            }

            var objProduct = new APIProduct()
            {
                ProductId = drProduct[CS.ProductId].zToInt().Value,
                Weight = drProduct[CS.Weight].zToDecimal(),
                VendorName = drProduct[CS.VendorName].ToString(),
                NameTag = drProduct[CS.NameTag].ToString(),
                Description = drProduct[CS.Description].ToString(),
                ProductCode = drProduct[CS.ProductCode].ToString(),
                VendorCode = drProduct[CS.VendorCode].ToString(),
                eStockStatus = drProduct[CS.eStockStatus].zToInt(),
                StockNote = drProduct[CS.StockNote].ToString(),

                PurchasePrice = PurchasePrice,
                UserPrice = UserPrice,
                SalePrice = SalePrice,
            };

            #region Product Image

            foreach (DataRow drProductImage in dtProductImage.Select(CS.ProductId + " = " + drProduct[CS.ProductId].zToInt()))
            {
                string ImageURL = CU.GetFilePath(true, ePhotoSize.Original, eFolder.ProductCImage, drProductImage[CS.ProductImageId].ToString(), true);
                objProduct.lstProductImage.Add(new APIProductImage()
                {
                    ProductImageId = drProductImage[CS.ProductImageId].zToInt(),
                    SerialNo = drProductImage[CS.SerialNo].zToInt(),
                    eProductImageType = drProductImage[CS.eProductImageType].zToInt(),
                    ImageURL = ImageURL,
                });
            }

            #endregion

            #region Product Variant

            foreach (APIProductVariant objProductVariant in lstProductVariant)
            {
                var lstProductVariantValue = new List<APIProductVariantValue>();
                //foreach (DataRow drProductVariant in dtProductVariant.Select(CS.ProductId + " = " + objProduct.ProductId + " AND " + CS.VariantId + " = " + objProductVariant.VariantId))
                //{
                //    lstProductVariantValue.Add(new APIProductVariantValue()
                //    {
                //        VariantId = objProductVariant.VariantId,
                //        ProductVariantId = drProductVariant[CS.ProductVariantId].zToInt(),
                //        VariantValue = drProductVariant[CS.VariantValue].ToString()
                //    });
                //}

                //If Null Value then Create Default Array
                if (lstProductVariantValue.Count == 0)
                {
                    lstProductVariantValue.Add(new APIProductVariantValue()
                    {
                        VariantId = null,
                        ProductVariantId = null,
                        VariantValue = string.Empty,
                    });
                }

                objProduct.lstProductVariant.Add(new APIProductVariant()
                {
                    VariantId = objProductVariant.VariantId,
                    OrderVariantValue = objProductVariant.OrderVariantValue,
                    ShowInOrder = objProductVariant.ShowInOrder,
                    VariantName = objProductVariant.VariantName,
                    lstProductVariantValue = lstProductVariantValue
                });
            }

            #endregion

            lstProduct.Add(objProduct);
        }

        #endregion

        return lstProduct;
    }


    private string GetFailStatus(eAPI API, string Tag)
    {
        return Tag + " - " + API.ToString() + "(" + (int)API + ")";
    }

    public enum eAPI
    {
        Login = 1,
        UserRegister = 2,
        CustRegister = 3, //???
        Category = 4, //???
        Product = 5,
        ChangePasowrd = 6,
        UpdateProfile = 7,
        UserDetail = 8,

        ViewCart = 9,
        AddCart = 10,
        RemoveCart = 11,
        UpdateCart = 12,

        AddReviewRating = 13,
        RemoveReviewRating = 14,

        ViewWishlist = 15,
        AddWishlist = 16,
        RemoveWishlist = 17,

        ViewOrder = 18,
        AddOrder = 19,

        Banner = 20,
        CallType = 21,
        CallHistory = 22,
        AddCallHistory = 23,
        lstUsers = 24,
    }
}