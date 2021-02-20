using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BOL;
using Utility;
using System.Data;

public partial class CCTextExtender : System.Web.UI.UserControl
{
    public string _AutoCompleteExtender = "";
    public string _SelectedItemText = "";
    public bool _HighlightResults = false;
    public string _WaterMarkText = "";

    public string placeholder
    {
        set { txtData.Attributes.Add("placeholder", value); }
    }

    public string Data
    {
        set { acaData.ServiceMethod = value; }
    }

    public string OId
    {
        get { return txtDataId.Text; }
        set { txtDataId.Text = value; }
    }

    public string OText
    {
        get { return txtData.Text; }
        set { txtData.Text = value; }
    }

    public bool OEnabled
    {
        set { txtData.Enabled = lblDestinationAddon.Enabled = value; }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string Script = @"
        function CallPostbackAutoCompliteData() {
            $('.txtData').change(function() {
                SetSearchtxtValue('txtData', 'txtDataId', 'lblDataSelectedJSON');
            });
        }
        function Data_OnClientPopulating(sender, e) {
            ClientPopulating('txtData', 'lblDataSelectedJSON');
        }

        function Data_OnClientPopulated(sender, e) {
            var Datalist = sender.get_completionList();
            ClientPopulated('txtData', 'lblDataSelectedJSON', Datalist);
        }";

        acaData.OnClientShowing = "CallPostbackAutoCompliteData";
        Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "CallPostbackAutoCompliteData", Script, true);
    }

    public void OFocus()
    {
        txtData.Focus();
    }


}
