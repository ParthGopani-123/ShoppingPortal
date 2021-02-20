using System;
using BOL;
using System.Web.UI.WebControls;
using System.Data;
using Utility;

public partial class ViewNotification : System.Web.UI.Page
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
            int UsersId = CU.GetUsersId();
            new System.Threading.Thread(() => ReadAllNotification(UsersId)).Start();

            txtFromDate.Text = IndianDateTime.Today.AddDays(-1).ToString(CS.ddMMyyyy);
            txtToDate.Text = IndianDateTime.Today.ToString(CS.ddMMyyyy);

            CU.LoadDisplayPerPage(ref ddlRecordPerPage);

            LoaNotificationType();
            LoadNotificationGrid(ePageIndex.Custom);
        }

        try { grdNotification.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }

    private void ReadAllNotification(int UsersId)
    {
        var lstReadStatus = new System.Collections.Generic.List<int>();
        lstReadStatus.Add((int)eMessageStatus.Unread);
        lstReadStatus.Add((int)eMessageStatus.WithHold);
        var dtNotification = new Query()
        {
			UsersId = UsersId,
            eReadStatusIn = CU.GetParaIn(lstReadStatus, true)
        }.Select(eSP.qry_Notification);

        foreach (DataRow dr in dtNotification.Rows)
            new Notification() { NotificationId = Convert.ToInt32(dr[CS.NotificationId]), eReadStatus = (int)eMessageStatus.Read, eViewStatus = (int)eViewStatus.Seen }.UpdateAsync();

        dtNotification = new Notification()
        {
			UsersId = CU.GetUsersId(),
            eViewStatus = (int)eViewStatus.UnSeen,
        }.Select();
        foreach (DataRow dr in dtNotification.Rows)
            new Notification() { NotificationId = Convert.ToInt32(dr[CS.NotificationId]), eReadStatus = (int)eMessageStatus.Read, eViewStatus = (int)eViewStatus.Seen }.UpdateAsync();
    }


    protected void LoaNotificationType()
    {
        CU.FillEnumddl<eNotificationType>(ref ddlNotificationType, "-- All Type --");
    }

    private void LoadNotificationGrid(ePageIndex ePageIndex)
    {
        var objQuery = new Query()
        {
            FromDate = txtFromDate.zToDate().HasValue ? txtFromDate.zToDate() : (DateTime?)null,
            ToDate = txtToDate.zToDate().HasValue ? txtToDate.zToDate().Value.AddDays(1).AddSeconds(-1) : (DateTime?)null,
            eNotificationType = ddlNotificationType.zIsSelect() ? ddlNotificationType.zToInt() : (int?)null,
			UsersId = CU.GetUsersId(),
            MasterSearch = txtSearch.Text,
        };

        #region Page Index

        int RecordPerPage = ddlRecordPerPage.zToInt().Value;
        int PageIndexTemp = PageIndex;

        CU.GetPageIndex(ePageIndex, RecordPerPage, ref PageIndexTemp, ref objQuery, ref txtGotoPageNo, ref lblCount);
        PageIndex = PageIndexTemp;

        #endregion Page Index

        DataTable dtNotification = objQuery.Select(eSP.qry_Notification);

        #region Count Total

        if (dtNotification.Rows.Count > 0)
            lblCount.Text = dtNotification.Rows[0][CS.TotalRecord].ToString();
        else
            lblCount.Text = "0";

        divPaging.Visible = (dtNotification.Rows.Count > 0);

        txtGotoPageNo.Text = PageIndex.ToString();

        ltrTotalContent.Text = CU.PageRecordString(lblCount.zToInt().Value, ddlRecordPerPage.zToInt().Value, lnkFirst, lnkPrev, txtGotoPageNo, lnkNext, lnkLast);

        #endregion

        grdNotification.DataSource = dtNotification;
        grdNotification.DataBind();

        try { grdNotification.HeaderRow.TableSection = TableRowSection.TableHeader; }
        catch { }
    }


    protected void lnkRefresh_OnClick(object sender, EventArgs e)
    {
        LoadNotificationGrid(ePageIndex.Custom);
    }

    protected void grdNotification_OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dataItem = (DataRowView)e.Row.DataItem;
            var divNotification = e.Row.FindControl("divNotification") as System.Web.UI.HtmlControls.HtmlControl;
            var ltrNotificationText = e.Row.FindControl("ltrNotificationText") as Literal;
            var lblNotificationDate = e.Row.FindControl("lblNotificationDate") as Label;

            ltrNotificationText.Text = dataItem[CS.NotificationText].ToString();
            e.Row.Attributes["onclick"] = "ViewNotificationText('" + dataItem[CS.NotificationId] + "','" + dataItem[CS.NotificationText].ToString() + "')";

            lblNotificationDate.Text = Convert.ToDateTime(dataItem[CS.InsertTime]).ToString(CS.ddMMyyyyHHmm);

            divNotification.addClass("Notification" + dataItem[CS.NotificationId].ToString());

            divNotification.Visible = false;
        }
    }

    #region Pagging

    protected void lnkPrev_Click(object sender, EventArgs e)
    {
        LoadNotificationGrid(ePageIndex.Prev);
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        LoadNotificationGrid(ePageIndex.Next);
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        LoadNotificationGrid(ePageIndex.First);
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        LoadNotificationGrid(ePageIndex.Last);
    }

    protected void txtGotoPageNo_OnTextChange(object sender, EventArgs e)
    {
        if (!txtGotoPageNo.zIsInteger(false) || txtGotoPageNo.zToInt() <= 0)
        {
            txtGotoPageNo.Text = "1";
            txtGotoPageNo.Focus();
        }
        LoadNotificationGrid(ePageIndex.Custom);
    }

    protected void ddlRecordPerPage_LoadMember(object sender, EventArgs e)
    {
        txtGotoPageNo.Text = "1";
        LoadNotificationGrid(ePageIndex.Custom);
        Response.Cookies["RecordPerPage"].Value = ddlRecordPerPage.SelectedValue;
    }

    #endregion
}
