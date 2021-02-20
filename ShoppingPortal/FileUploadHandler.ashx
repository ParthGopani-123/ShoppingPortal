<%@ WebHandler Language="C#" Class="FileUploadHandler" %>
 
using System;
using System.Web;
using System.IO;

public class FileUploadHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        if (context.Request.Files.Count > 0)
        {
            string Filename = string.Empty;
            HttpFileCollection files = context.Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFile file = files[i];
                string SaveFileName = CU.GetDateTimeName() + "_" + file.FileName;
                string fname = context.Server.MapPath(CU.GettempDownloadPath()) + "/" + SaveFileName;
                file.SaveAs(fname);
                Filename += "~" + CU.GettempDownloadPath() + "/" + SaveFileName;
            }

            Filename = Filename.TrimStart('~');
            context.Response.ContentType = "text/plain";
            context.Response.Write(Filename);
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
 
}