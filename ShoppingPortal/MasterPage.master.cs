using System;
using BOL;
using Utility;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadMenu();
            lblStaticFilePath.Text = CU.StaticFilePath;

            lblMstClientBlanace.Text = CU.GeFormatedBalance(CU.GetUsersId());

        }

        if (Session["ZMessage"] != null)
        {
            var objZMessage = (ZMessage)Session["ZMessage"];
            CU.ZMessage(objZMessage.MsgType, objZMessage.Title, objZMessage.Message);
            Session["ZMessage"] = null;
        }

        Page.Title = Page.Title + " - Shopping Portal";
    }

    private void LoadMenu()
    {
        #region Load Manu Visible

        int UsersId = CU.GetUsersId();

        var drUser = new Users() { UsersId = UsersId }.Select(new Users() { DesignationId = 0, Name = string.Empty }).Rows[0];
        eDesignation Designation = CU.GetDesiToeDesi(drUser[CS.DesignationId].zToInt().Value);

        #region Load Detail

        lblClientName.Text = drUser[CS.Name].ToString();
        imgClientImage.ImageUrl = CU.GetFilePath(true, ePhotoSize.W50xH50, eFolder.UserProfile, UsersId.ToString(), true);
        imgLogo.ImageUrl = CU.GetSystemImage(eSystemImage.OCTFISLogo_EXT_png);

        #endregion

        if (Designation != eDesignation.SystemAdmin)
        {
            var dtUserAuthority = new UserAuthority()
            {
                UsersId = UsersId,
                IsAllowView = true,
            }.Select(new UserAuthority() { eAuthority = 0, IsAllowAddEdit = true, IsAllowView = true });

            #region Configuration

            liManageCountry.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageCountry).Length > 0;
            liManageState.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageState).Length > 0;
            liManageCity.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageCity).Length > 0;
            liManageArea.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageArea).Length > 0;
            liManageServiceAvailability.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageService).Length > 0;
            liManageBankAccount.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageBankAccount).Length > 0;
            liManageVariant.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageVariant).Length > 0;
            liManagePriceList.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManagePriceList).Length > 0;
            liManageCourier.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageCourier).Length > 0;
            liManageOrderStatus.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageOrderStatus).Length > 0;
            liManageOrderSource.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageOrderSource).Length > 0;
            liManageVendor.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageVendor).Length > 0;
            liConfiguration.Visible = Designation == eDesignation.SystemAdmin || Designation == eDesignation.Admin;

            if (!liManageCountry.Visible && !liManageState.Visible && !liManageCity.Visible && !liManageArea.Visible
                && !liManageBankAccount.Visible && !liManageVariant.Visible && !liManagePriceList.Visible && !liManageCourier.Visible
                && !liManageOrderStatus.Visible && !liManageOrderSource.Visible && !liConfiguration.Visible)
                ulConfiguration.Visible = false;

            #endregion

            #region System

            liManageDesignation.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.Designation).Length > 0;
            liManageOrganization.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.Organization).Length > 0;
            liManageFirm.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.Firm).Length > 0;
            liManageUser.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.User).Length > 0;

            if (!liManageDesignation.Visible && !liManageOrganization.Visible && !liManageFirm.Visible && !liManageUser.Visible
                && !liManageCustomer.Visible)
                ulSystem.Visible = false;

            #endregion

            #region Product 

            liManagePortal.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManagePortal).Length > 0;
            liManageProduct.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageProduct).Length > 0;
            liManageAdjustment.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.Adjustment).Length > 0;

            if (!liManagePortal.Visible && !liManageProduct.Visible && !liManageAdjustment.Visible)
                liProduct.Visible = false;

            #endregion

            #region Order

            liOrderView.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageOrder).Length > 0;
            liManageOrder.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageOrder + " AND " + CS.IsAllowAddEdit + " = " + true).Length > 0;
            liManageOrderPayment.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageOrderPayment).Length > 0;

            if (!liManageOrder.Visible && !liOrderView.Visible && !liManageOrderPayment.Visible)
                liManageOrderPage.Visible = false;

            #endregion

            liViewCart.Visible = false;

            #region Call Recording

            liManageCallType.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageCallType).Length > 0;
            liManageCallHistory.Visible = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageCallHistory).Length > 0;

            if (!liManageCallType.Visible && !liManageCallHistory.Visible)
                liCallRecording.Visible = false;

            #endregion

            string PageName = Request.Url.AbsolutePath.ToLower();
            PageName = PageName.Substring(PageName.LastIndexOf("/") + 1);
            bool IsValid = false;
            switch (PageName)
            {
                case "home.aspx":
                case "myprofile.aspx":
                case "managecustomer.aspx":
                case "manageonlinecurior.aspx":
                case "mywallet.aspx":
                    IsValid = true;
                    break;

                #region Configuration

                case "managecountry.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageCountry).Length > 0;
                    break;
                case "managestate.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageState).Length > 0;
                    break;
                case "managecity.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageCity).Length > 0;
                    break;
                case "managearea.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageArea).Length > 0;
                    break;
                case "manageserviceavailability.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageService).Length > 0;
                    break;
                case "managebankaccount.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageBankAccount).Length > 0;
                    break;
                case "managevariant.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageVariant).Length > 0;
                    break;
                case "managepricelist.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManagePriceList).Length > 0;
                    break;
                case "managecourier.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageCourier).Length > 0;
                    break;
                case "manageordersource.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageOrderSource).Length > 0;
                    break;
                case "manageorderstatus.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageOrderStatus).Length > 0;
                    break;
                case "managevendor.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageVendor).Length > 0;
                    break;
                #endregion

                #region System

                case "managedesignation.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.Designation).Length > 0;
                    break;
                case "manageorganization.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.Organization).Length > 0;
                    break;
                case "managefirm.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.Firm).Length > 0;
                    break;
                case "manageuser.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.User).Length > 0;
                    break;

                #endregion

                #region Product

                case "manageportal.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManagePortal).Length > 0;
                    break;
                case "manageproduct.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageProduct).Length > 0;
                    break;
                case "manageadjustment.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.Adjustment).Length > 0;
                    break;

                #endregion

                #region Order

                case "orderview.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageOrder).Length > 0;
                    break;
                case "manageorder.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageOrder + " AND " + CS.IsAllowAddEdit + " = " + true).Length > 0;
                    break;
                case "manageorderpayment.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageOrderPayment).Length > 0;
                    break;
                #endregion

                #region Call Recording

                case "managecalltype.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageCallType).Length > 0;
                    break;
                case "managecallhistory.aspx":
                    IsValid = dtUserAuthority.Select(CS.eAuthority + " = " + (int)eAuthority.ManageCallHistory).Length > 0;
                    break;

                #endregion

                case "configuration.aspx":
                    IsValid = (Designation == eDesignation.SystemAdmin || Designation == eDesignation.Admin);
                    break;
            }

            if (!IsValid)
                Response.Redirect("Home.aspx");
        }

        #endregion
    }
}
