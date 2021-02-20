using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ErrorPages_Error : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string FaceSad = "<i class='ti-face-sad fs-100'></i>";
        try
        {
            var ex = Server.GetLastError();
            switch ((ex as HttpException).GetHttpCode())
            {
                case 400:
                    ltrlErrorCode.Text = "4" + FaceSad + FaceSad;
                    ltrlErrorMessage.Text = "Bad Request!";
                    ltrlErrorDescription.Text = "Your browser sent a request that this server could not understand.";
                    break;
                case 401:
                    ltrlErrorCode.Text = "4" + FaceSad + "1";
                    ltrlErrorMessage.Text = "Not Authorized!";
                    ltrlErrorDescription.Text = "The requested resource requires user authentication.";
                    break;
                case 403:
                    ltrlErrorCode.Text = "4" + FaceSad + "3";
                    ltrlErrorMessage.Text = "Access Denied!";
                    ltrlErrorDescription.Text = "You don't have permission to access on this server.";
                    break;
                case 404:
                    ltrlErrorCode.Text = "4" + FaceSad + "4";
                    ltrlErrorMessage.Text = "Page Not Found!";
                    ltrlErrorDescription.Text = "Sorry! the page you are looking for doesn't exist.";
                    break;
                case 503:
                    ltrlErrorCode.Text = "5" + FaceSad + "3";
                    ltrlErrorMessage.Text = "Service Unavailable!";
                    ltrlErrorDescription.Text = "The service is not available. Please try again later.";
                    break;
                default:
                    ltrlErrorCode.Text = "5" + FaceSad + FaceSad;
                    ltrlErrorMessage.Text = "Unexpected Error!";
                    ltrlErrorDescription.Text = "An error occurred so your request couldn't be completed";
                    break;
            }
            if (ex.InnerException != null)
                ltrlErrorDetail.Text += "<b>InnerException:</b> " + ex.InnerException.Message;
            ltrlErrorDetail.Text += "<br/><b>Message:</b> " + ex.Message;
            ltrlErrorDetail.Text += "<br/><b>Source:</b> " + ex.Source;
            ltrlErrorDetail.Text += "<br/><b>StackTrace:</b> " + ex.StackTrace;
        }
        catch { Response.Redirect("Default.aspx"); }
    }
}
