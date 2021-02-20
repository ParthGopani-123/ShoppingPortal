using BOL;
using System;
using Utility;

public partial class Home : CompressorPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LoginUtilities.CheckSession();

        aSearchService.HRef = "SearchService.aspx?" + CS.OrganizationId.Encrypt() + "=" + CU.GetOrganizationId().ToString().Encrypt();
        aSearchProduct.HRef = "SearchProduct.aspx?" + CS.OrganizationId.Encrypt() + "=" + CU.GetOrganizationId().ToString().Encrypt();
    }
}
