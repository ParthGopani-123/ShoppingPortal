using System;
using BOL;
using System.Data;
using Utility;

public partial class GetNotificaionList : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		LoadNotification();
	}

	private void LoadNotification()
	{
		string Query = "UPDATE Notification set eReadStatus = " + (int)eMessageStatus.Unread +
								   " WHERE UsersId = " + CU.GetUsersId() +
								   " AND eReadStatus = " + (int)eMessageStatus.WithHold;

		DBHelper.ExecuteSqlNonQuery(Query, false, new System.Collections.Generic.List<System.Data.SqlClient.SqlParameter>());

		var dtNotification = new Query()
		{
			UsersId = CU.GetUsersId(),
			IsNotify = true
		}.Select(eSP.qry_Notification);

		lblNoificaionial.Text = "You have " + dtNotification.Rows.Count + " notifications";

		rptNotification.DataSource = dtNotification;
		rptNotification.DataBind();
	}

	protected void rptNotification_OnItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
	{
		var divNotification = e.Item.FindControl("divNotification") as System.Web.UI.HtmlControls.HtmlControl;
		var imgNotificationImage = e.Item.FindControl("imgNotificationImage") as System.Web.UI.WebControls.Image;
		var aNotificationLink = e.Item.FindControl("aNotificationLink") as System.Web.UI.HtmlControls.HtmlAnchor;

		var dataItem = (System.Data.DataRowView)((System.Web.UI.WebControls.RepeaterItem)e.Item).DataItem;

		eNotificationType NotificationType = (eNotificationType)dataItem[CS.eNotificationType].ToString().zToInt();
		imgNotificationImage.ImageUrl = CU.GetNotificationImage(NotificationType);

		string Class = "divNotificationList Notification" + dataItem[CS.NotificationId];
		if (Convert.ToInt32(dataItem[CS.eReadStatus]) != (int)eMessageStatus.Read)
			Class += " NotificationActive";

		aNotificationLink.Visible = false;
		divNotification.Attributes.Add("class", Class);
	}
}