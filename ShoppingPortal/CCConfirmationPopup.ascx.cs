using System;
using BOL;

public partial class CCConfirmationPopup : System.Web.UI.UserControl
{
    public event EventHandler btnActivePopup_OnClick;
    public event EventHandler btnDeactivePopup_OnClick;
    public event EventHandler btnDeletePopup_OnClick;

    public void SetPopupType(ePopupType PopupType, string PopupTitle, string PopupMessage)
    {
        switch (PopupType)
        {
            case ePopupType.Active:
                btnActive.Focus();
                break;
            case ePopupType.Deactive:
                btnDeactive.Focus();
                break;
            case ePopupType.Delete:
                btnDelete.Focus();
                break;
        }

        btnActive.Visible = PopupType == ePopupType.Active;
        btnDeactive.Visible = PopupType == ePopupType.Deactive;
        btnDelete.Visible = PopupType == ePopupType.Delete;

        lblPopupTitle.Text = PopupTitle;
        lblPopupMessage.Text = PopupMessage;
    }


    protected void btnActive_OnClick(object sender, EventArgs e)
    {
        btnActivePopup_OnClick(sender, e);
    }

    protected void btnDeactive_OnClick(object sender, EventArgs e)
    {
        btnDeactivePopup_OnClick(sender, e);
    }

    protected void btnDelete_OnClick(object sender, EventArgs e)
    {
        btnDeletePopup_OnClick(sender, e);
    }
}
