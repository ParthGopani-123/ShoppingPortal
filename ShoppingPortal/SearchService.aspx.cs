using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class SearchService : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int OrganizationId = 0;
        if (!Request.QueryString[CS.OrganizationId.Encrypt()].zIsNullOrEmpty())
        {
            try { OrganizationId = Request.QueryString[CS.OrganizationId.Encrypt()].ToString().Decrypt().zToInt().Value; }
            catch { }
        }

        lblOrganizationId.Text = OrganizationId.ToString();


        Page.Title = Page.Title + " - Shopping Portal";
        lblErrorMessage.Text = string.Empty;

        lblPincodeDetail.Text = lblPincode.Text = string.Empty;
        aSearchProduct.HRef = "SearchProduct.aspx?" + CS.OrganizationId.Encrypt() + "=" + CU.GetOrganizationId().ToString().Encrypt();

    }

    protected void btnSearchPincode_OnClick(object sender, EventArgs e)
    {
        var dtService = new Query()
        {
            Pincode = txtPincode.Text,
            OrganizationId = lblOrganizationId.zToInt(),
            eStatus = (int)eStatus.Active
        }.Select(eSP.qry_ServiceAvailability);

        lblPincode.Text = txtPincode.Text;
        if (dtService.Rows.Count > 0)
        {
            var drService = dtService.Rows[0];
            lblPincodeDetail.Text = drService[CS.CityName].ToString() + ", " + drService[CS.StateName].ToString();// + ", " + drService[CS.CountryName].ToString();
        }
        else
        {
            lblPincodeDetail.Text = "Only Prepaid Available";
            var dtApproxCity = new DataTable();
            string PrePincode = txtPincode.Text;
            while (PrePincode.Length > 4 && dtApproxCity.Rows.Count == 0)
            {
                PrePincode = PrePincode.Substring(0, PrePincode.Length - 1);
                dtApproxCity = new Query()
                {
                    MasterSearch = PrePincode,
                    OrganizationId = lblOrganizationId.zToInt(),
                    eStatus = (int)eStatus.Active
                }.Select(eSP.qry_ServiceAvailability);

                if (dtApproxCity.Rows.Count > 0)
                {
                    dtApproxCity.Columns.Add("ClosestPincode", typeof(int));
                    foreach (DataRow dr in dtApproxCity.Rows)
                        dr["ClosestPincode"] = dr["Pincode"].zToInt() - PrePincode.zToInt();

                    dtApproxCity = dtApproxCity.Select("", "ClosestPincode").CopyToDataTable();

                    var drService = dtApproxCity.Rows[0];
                    lblPincodeDetail.Text = drService[CS.CityName].ToString() + "***, " + drService[CS.StateName].ToString();// + ", " + drService[CS.CountryName].ToString();
                }
            }
        }

        var dtTempService = dtService.Clone();
        if (dtService.Select(CS.eCOD + "=" + '1').Length > 0)
            dtTempService.ImportRow(dtService.Select(CS.eCOD + "=" + '1')[0]);
        else if (dtService.Select(CS.ePrepaid + "=" + '1').Length > 0)
            dtTempService.ImportRow(dtService.Select(CS.ePrepaid + "=" + '1')[0]);

        rptServiceAvailability.DataSource = dtTempService.Rows.Count > 0 ? dtTempService : dtService;
        rptServiceAvailability.DataBind();

        txtPincode.Text = "";
        btnSearchPincode.Focus();
    }

    private bool CheckService(DataTable dtService, string ColumnName)
    {
        return dtService.Select(ColumnName + " = " + (int)eYesNo.Yes).Length > 0;
    }

    protected void rptServiceAvailability_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var divCOD = e.Item.FindControl("divCOD") as HtmlControl;
        var divPrepaid = e.Item.FindControl("divPrepaid") as HtmlControl;
        var divReversePickup = e.Item.FindControl("divReversePickup") as HtmlControl;
        var divPickup = e.Item.FindControl("divPickup") as HtmlControl;

        var lblCourierName = e.Item.FindControl("lblCourierName") as Label;

        var lblCODStatus = e.Item.FindControl("lblCODStatus") as Label;
        var lblPrepaidStatus = e.Item.FindControl("lblPrepaidStatus") as Label;
        var lblReversePickupStatus = e.Item.FindControl("lblReversePickupStatus") as Label;
        var lblPickupStatus = e.Item.FindControl("lblPickupStatus") as Label;

        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;

        lblCourierName.Text = dataItem[CS.CourierName].ToString();

        bool IsCOD = dataItem[CS.eCOD].zToInt() == (int)eYesNo.Yes;
        bool IsPrepaid = dataItem[CS.ePrepaid].zToInt() == (int)eYesNo.Yes;
        bool IsReversePickup = dataItem[CS.eReversePickup].zToInt() == (int)eYesNo.Yes;
        bool IsPickup = dataItem[CS.ePickup].zToInt() == (int)eYesNo.Yes;

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
}