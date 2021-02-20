using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Utility;
using BOL;
using System.Data;

/// <summary>
/// Summary description for TextboxExtender
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
//To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class TextboxExtender : System.Web.Services.WebService
{
    [WebMethod]
    public string[] Country(string prefixText)
    {
        return GetData(eSP.qry_Country, CS.FullDestination, CS.CountryId, prefixText);
    }

    private string[] GetData(eSP SP, string TextField, string ValueField, string prefixText)
    {
        List<string> datalist = new List<string>();
        if (!prefixText.zIsNullOrEmpty() && prefixText.Length > 1)
        {
            var dtData = new Query() { MasterSearch = prefixText.ToLower(), eStatus = (int)eStatus.Active }.Select(SP);
            foreach (DataRow drData in dtData.Rows)
                datalist.Add(AjaxControlToolkit.AutoCompleteExtender.CreateAutoCompleteItem(drData[TextField].ToString(), drData[ValueField].ToString()));
        }

        if (datalist.Count == 0)
            datalist.Add(AjaxControlToolkit.AutoCompleteExtender.CreateAutoCompleteItem("No Data Found", "0"));

        return datalist.ToArray();
    }
}

