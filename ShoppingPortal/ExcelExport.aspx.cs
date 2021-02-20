using System;
using System.Data;
using Utility;

public partial class ExcelExport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString.Count > 0 && Request.QueryString["FileName".Encrypt()] != null
            && Session[Request.QueryString["FileName".Encrypt()].Decrypt()] != null)
        {
            ExportToExcel();
        }
    }

    protected void ExportToExcel()
    {
        DataTable dt = (DataTable)Session[Request.QueryString["FileName".Encrypt()].Decrypt()];

        Response.ClearContent();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", Request.QueryString["FileName".Encrypt()].Decrypt() + ".xls"));
        Response.ContentType = "application/ms-excel";
        string str = string.Empty;
        foreach (DataColumn dtcol in dt.Columns)
        {
            Response.Write(str + dtcol.ColumnName);
            str = "\t";
        }
        Response.Write("\n");
        foreach (DataRow dr in dt.Rows)
        {
            str = "";
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                Response.Write(str + dr[j].ToString().Replace("<br/>", " | ").Replace("\t", " _ ")
                                                     .Replace("\r\n", " | ").Replace("\n", " | ").Replace("\r", " | "));
                str = "\t";
            }
            Response.Write("\n");
        }

        Session[Request.QueryString["FileName".Encrypt()].Decrypt()] = null;
        Response.End();
    }
}