using System;
using Utility;

public partial class logout : CompressorPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LoginUtilities.CheckSession();

        string SessionId = "0";
        try { SessionId = Session[LoginUtilities.SessionId].ToString(); } catch { }

        LoginUtilities.RemoveSession();
        LoginUtilities.RemoveCookies();
        if (Request.QueryString.Count > 0 && Request.QueryString["loginurl"] != null)
        {
            Response.Redirect("Login?" + "CheckUsersId".Encrypt() + "=" + SessionId.Encrypt() + "&" + BOL.CS.rurl.Encrypt() + "=" + Request.QueryString["loginurl"].ToString().Encrypt());
        }
        else
        {
            if (Request.QueryString[BOL.CS.rurl.Encrypt()] == null)
                Response.Redirect("Login");
            else
                Response.Redirect("Login?" + BOL.CS.rurl.Encrypt() + "=" + Request.QueryString[BOL.CS.rurl.Encrypt()].ToString());
        }
    }
}
