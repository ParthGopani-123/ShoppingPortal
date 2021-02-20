using System;
using System.Web.UI;
using BOL;
using Utility;
using System.Data;
using System.Web.UI.WebControls;

public partial class ViewCart : CompressorPage
{
    private int PageIndex
    {
        get
        {
            if (ViewState["PageIndex"] != null)
                return Convert.ToInt32(ViewState["PageIndex"]);
            else
                return 0;
        }
        set { ViewState["PageIndex"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        LoginUtilities.CheckSession();
        if (!IsPostBack)
        {
            CU.LoadDisplayPerPage(ref ddlRecordPerPage);

            LoadCartGrid(ePageIndex.Custom);
        }

        try { grdCart.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private DataTable GetCartDt(ePageIndex ePageIndex)
    {
        var objQuery = new Query()
        {
            MasterSearch = txtSearch.Text,
            OrganizationId = CU.GetOrganizationId(),
            eStatusNot = (int)eStatus.Delete,
        };

        #region Page Index

        int RecordPerPage = ddlRecordPerPage.zToInt().Value;
        int PageIndexTemp = PageIndex;

        CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref objQuery, ref txtGotoPageNo, ref lblCount);
        PageIndex = PageIndexTemp;

        #endregion Page Index

        return objQuery.Select(eSP.qry_Cart);
    }

    private void LoadCartGrid(ePageIndex ePageIndex)
    {
        DataTable dtCart = GetCartDt(ePageIndex);

        if (dtCart.Rows.Count > 0)
            lblCount.Text = dtCart.Rows[0][CS.TotalRecord].ToString();
        else
            lblCount.Text = "0";

        divPaging.Visible = (dtCart.Rows.Count > 0);

        txtGotoPageNo.Text = PageIndex.ToString();

        ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

        grdCart.DataSource = dtCart;
        grdCart.DataBind();

        try { grdCart.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }


    protected void lnkRefresh_OnClick(object sender, EventArgs e)
    {
        LoadCartGrid(ePageIndex.Custom);
    }

    protected void Control_CheckedChanged(object sender, EventArgs e)
    {
        LoadCartGrid(ePageIndex.Custom);
    }


    protected void grdCart_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
    }

    protected void grdCart_OnSelectedIndexChanged(object sender, EventArgs e)
    {
    }

    #region Pagging


    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        LoadCartGrid(ePageIndex.Prev);
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        LoadCartGrid(ePageIndex.Next);
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        LoadCartGrid(ePageIndex.First);
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        LoadCartGrid(ePageIndex.Last);
    }

    protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
    {
        if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
        {
            txtGotoPageNo.Text = "1";
            txtGotoPageNo.Focus();
        }
        LoadCartGrid(ePageIndex.Custom);
    }

    protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
    {
        txtGotoPageNo.Text = "1";
        LoadCartGrid(ePageIndex.Custom);
        Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
    }


    #endregion
}
