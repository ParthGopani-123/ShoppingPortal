using System;

public partial class CCConfirmPopup : System.Web.UI.UserControl
{
    public event EventHandler btnCancel_OnClick;
    public event EventHandler btnConfirm_OnClick;


    public void SetPopupType(string PopupTitle, string PopupMessage, bool IsYesNo, bool IsOnlyOk)
    {
        SetPopupType(PopupTitle, PopupMessage, IsYesNo);
        if (IsOnlyOk)
        {
            btnConfirmPopup.Text = "Oky";
            btnCancelPopup.Visible = aClosePopup.Visible = false;
        }
    }

    public void SetPopupType(string PopupTitle, string PopupMessage, bool IsYesNo)
    {
        lblPopupTitle.Text = PopupTitle;
        lblPopupMessage.Text = PopupMessage;
        btnCancelPopup.Visible = aClosePopup.Visible = true;

        if (IsYesNo)
        {
            btnConfirmPopup.Text = "Yes";
            btnCancelPopup.Text = "No";
        }
        else
        {
            btnConfirmPopup.Text = "Confirm";
            btnCancelPopup.Text = "Cancel";
        }
    }

    protected void btnCancelPopup_OnClick(object sender, EventArgs e)
    {
        try { btnCancel_OnClick(sender, e); }
        catch { }
    }

    protected void btnConfirmPopup_OnClick(object sender, EventArgs e)
    {
        try { btnConfirm_OnClick(sender, e); }
        catch { }
    }
}
