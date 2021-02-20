using System;
using System.Collections.Generic;
using BOL;
using System.Data;
using System.Web.UI.WebControls;
using Utility;

public partial class CCExcelExport : System.Web.UI.UserControl
{
    private void SelectDefaultCheckbox(List<string> lstSelectedColumn)
    {
        foreach (RepeaterItem item in rptExportColumn.Items)
        {
            var chkExportColumn = item.FindControl("chkExportColumn") as CheckBox;
            chkExportColumn.Checked = lstSelectedColumn.Contains(chkExportColumn.Text);
        }
    }

    public void SetExportData(System.Data.DataTable dtExport, List<string> lstColumn, List<string> lstSelectedColumn, string FileName)
    {
        dtExport = dtExport.DefaultView.ToTable(false, lstColumn.ToArray());

        Session["dtExport"] = dtExport;
        ViewState["lstSelectedColumn"] = lstSelectedColumn;
        lblFileName.Text = FileName;

        DataTable dtColumn = new DataTable();
        dtColumn.Columns.Add("Columns");
        foreach (string Column in lstColumn)
            dtColumn.Rows.Add(Column);

        rptExportColumn.DataSource = dtColumn;
        rptExportColumn.DataBind();

        SelectDefaultCheckbox(lstSelectedColumn);
    }

    protected void btnExport_OnClick(object sender, EventArgs e)
    {
        DataTable dtExport = ((DataTable)Session["dtExport"]);
        List<string> ColumnList = new List<string>();

        foreach (RepeaterItem item in rptExportColumn.Items)
        {
            var chkExportColumn = item.FindControl("chkExportColumn") as CheckBox;
            if (chkExportColumn.Checked)
                ColumnList.Add(chkExportColumn.Text);
        }

        if (ColumnList.Count == 0)
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Export Field.");
            return;
        }

        dtExport = dtExport.DefaultView.ToTable(false, ColumnList.ToArray());

		
        //string FileName = lblFileName.Text + "_" + CU.GetDateTimeName();
		//Session[FileName] = dtExport;
		//System.Web.UI.ScriptManager.RegisterStartupScript(Page, typeof(System.Web.UI.Page), "OpenWindow", "OpenNewWindow('ExcelExport.aspx?" + "FileName".Encrypt() + "=" + FileName.Encrypt() + "');", true);

        string FileName = lblFileName.Text + "_" + CU.GetDateTimeName()+ ".xls";
		CU.ExportToExcel(Server.MapPath(CU.GettempDownloadPath()) + "/" + FileName, dtExport);
        System.Web.UI.ScriptManager.RegisterStartupScript(Page, typeof(System.Web.UI.Page), "OpenWindow", "OpenNewWindow('" + CU.GettempDownloadPath() + "/" + FileName + "');", true);
    }

    protected void lnkSetDefaultSelect_OnClick(object sender, EventArgs e)
    {
        SelectDefaultCheckbox(((List<string>)ViewState["lstSelectedColumn"]));
    }


    protected void rptExportColumn_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var chkExportColumn = e.Item.FindControl("chkExportColumn") as CheckBox;

        DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;

        chkExportColumn.Text = dataItem["Columns"].ToString();
    }
}
