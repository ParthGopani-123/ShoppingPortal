using System;

public partial class VersonUpdate_Default : CompressorPage
{


    protected void Page_Load(object sender, EventArgs e)
    {
        divError.Visible = divSucess.Visible = false;
        LoadDetail();
    }

    private void LoadDetail()
    {
        string CurrntVersion = VU.GetDBVersion();

        lblLatestVersion.Text = VU.LatestVersion;
        lblCurentVerson.Text = CurrntVersion;
        lblLastVersionUpdateDate.Text = "Last Update: " + VU.LatestUpdate;

        if (CurrntVersion == VU.LatestVersion)
        {
            divUptodate.Visible = true;
            divUpdateNow.Visible = false;
        }
        else
        {
            divUpdateNow.Visible = true;
            divUptodate.Visible = false;
        }
    }

    private void ZError(string Msg)
    {
        divError.Visible = true;
        lblError.Text = Msg;
    }

    private void ZSuccess(string Msg)
    {
        divSucess.Visible = true;
        lblSucess.Text = Msg;
    }


    protected void btnUpdateVerson_Click(object sender, EventArgs e)
    {
        string Message = "";
        if (VU.UpdateVersion(ref Message))
            ZSuccess(Message);
        else
            ZError(Message);
        LoadDetail();
    }

}
