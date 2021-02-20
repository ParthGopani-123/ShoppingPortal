using System;
using Utility;
using BOL;

public partial class GetAjaxData : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		GetData();
	}

	private void GetData()
	{
		int UsersId = CU.GetUsersId();
		if (UsersId == 0)
		{
			Response.Write(0);
			return;
		}

		int ajaxDataType = Convert.ToInt32(Request.QueryString["ajaxDataType"]);

		switch (ajaxDataType)
		{
			case (int)eAjaxDataType.GetNotification:
				#region CheckNotification
				{
					int NotificationCount = 0;
					string Notification = "0";

					#region GetNotificationCount

					NotificationCount = new Notification()
					{
						UsersId = UsersId,
						eReadStatus = (int)eMessageStatus.WithHold
					}.SelectCount();

					#endregion UpdateViewNotificationStatus

					#region Get Notification

					if (new Notification() { UsersId = UsersId, eViewStatus = (int)eViewStatus.UnSeen }.SelectCount() > 0)
					{
						var dtNotification = new Query()
						{
							UsersId = UsersId,
							eViewStatus = (int)eViewStatus.UnSeen
						}.Select(eSP.qry_Notification);

						Notification = string.Empty;

						foreach (System.Data.DataRow drNotification in dtNotification.Rows)
						{
							new Notification()
							{
								NotificationId = drNotification[CS.NotificationId].zToInt(),
								eViewStatus = (int)eViewStatus.Seen,
							}.UpdateAsync();

							string ImageUrl = CU.GetNotificationImage((eNotificationType)drNotification[CS.eNotificationType].zToInt());
							Notification += (drNotification[CS.NotificationText].ToString().zRemoveHTML() + "#~#" + Convert.ToDateTime(drNotification[CS.InsertTime]).ToString(CS.ddMMyyyyhhmmtt) + "#~#" + ImageUrl.Replace("~/", "") + "##~##");
						}
					}

					#endregion

					Response.Write(NotificationCount.ToString() + "#OCTFIS#" + Notification);
				}
				break;
			#endregion CheckNotification

			case (int)eAjaxDataType.UpdateViewNotificationStatus:
				#region UpdateViewNotificationStatus
				{
					string Query = "UPDATE Notification set eReadStatus=" + (int)eMessageStatus.Unread +
								   " WHERE UsersId = " + UsersId +
								   " AND eReadStatus = " + (int)eMessageStatus.WithHold;

					DBHelper.ExecuteSqlNonQuery(Query, false, new System.Collections.Generic.List<System.Data.SqlClient.SqlParameter>());
				}
				break;
			#endregion UpdateViewNotificationStatus

			case (int)eAjaxDataType.UpdateNotificationRead:
				#region UpdateNotificationRead
				try
				{
					if (new Notification()
					{
						NotificationId = Convert.ToInt32(Request.QueryString[CS.NotificationId])
					}.Select(new Notification() { eReadStatus = 0 }).Rows[0][CS.eReadStatus].zToInt() != (int)eMessageStatus.Read)
						new Notification()
						{
							NotificationId = Convert.ToInt32(Request.QueryString[CS.NotificationId]),
							eReadStatus = (int)eMessageStatus.Read,
							ReadTime = IndianDateTime.Now,
						}.UpdateAsync();
				}
				catch { }
				break;
			#endregion UpdateNotificationRead
		}
	}
}
