using System;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;

public partial class Configuration : CompressorPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        LoginUtilities.CheckSession();

        if (!IsPostBack)
        {
            lblOrganizationId.Text = CU.GetOrganizationId().ToString();
            LoadData();
        }
    }


    private void LoadData()
    {
        var dtNameValue = new NameValue() { OrganizationId = lblOrganizationId.zToInt(), }.Select();

        var dr = dtNameValue.Select(CS.NameId + " = " + (int)eNameValue.EmailId);
        txtEmail.Text = dr.Length > 0 ? dr[0][CS.Value].ToString() : "";

        chkSameCustomerDifferantUser.Checked = CU.GetNameValue(eNameValue.SameCustomerDifferantUser).zToInt() == (int)eYesNo.Yes;
        chkCanUserSelectCourier.Checked = CU.GetNameValue(eNameValue.CanUserSelectCourier).zToInt() == (int)eYesNo.Yes;

        txtShipwayUsername.Text = CU.GetNameValue(eNameValue.ShipwayUsername);
        txtShipwayLicenceKey.Text = CU.GetNameValue(eNameValue.ShipwayLicenceKey);

    }

    protected void lnkSave_OnClick(object sender, EventArgs e)
    {
        CU.SetNameValue(eNameValue.EmailId, txtEmail.Text);

        if (!txtPassword.zIsNullOrEmpty())
            CU.SetNameValue(eNameValue.EmailPassword, txtPassword.Text);

        CU.SetNameValue(eNameValue.SameCustomerDifferantUser, chkSameCustomerDifferantUser.Checked ? ((int)eYesNo.Yes).ToString() : ((int)eYesNo.No).ToString());
        CU.SetNameValue(eNameValue.CanUserSelectCourier, chkCanUserSelectCourier.Checked ? ((int)eYesNo.Yes).ToString() : ((int)eYesNo.No).ToString());

        CU.SetNameValue(eNameValue.ShipwayUsername, txtShipwayUsername.Text);
        CU.SetNameValue(eNameValue.ShipwayLicenceKey, txtShipwayLicenceKey.Text);

        CU.ZMessage(eMsgType.Success, string.Empty, "Configuration Set Successfully.");
    }

    protected void lnkCancel_OnClick(object sender, EventArgs e)
    {
        LoadData();
    }
}
