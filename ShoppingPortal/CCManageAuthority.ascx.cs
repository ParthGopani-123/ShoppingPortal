using System;
using System.Data;
using BOL;
using Utility;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class CCManageAuthority : System.Web.UI.UserControl
{
	DataTable dtAuthority;

	public event EventHandler btnCancel_OnClick;

	public string SetDesignationId
	{
		get { return lblDesignationId.Text; }
		set { lblDesignationId.Text = value; }
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		ConfirmPopupChangeAuthority.btnConfirm_OnClick += new EventHandler(btnConfirmChangeAuthority_OnClick);
		ConfirmPopupChangeAuthority.btnCancel_OnClick += new EventHandler(btnNotChangeAuthority_OnClick);
	}

	public void LoadAuthorityDetail(int DesignationId, int UsersId, string ParentPageName)
	{
		lnkParentPage.Text = ParentPageName;
		lblDesignationId.Text = DesignationId.ToString();
		lblUsersId.Text = UsersId.ToString();

		if (IsDesignation())
		{
			ltrParentPageName.Text = new Designation() { DesignationId = lblDesignationId.zToInt() }.Select().Rows[0][CS.DesignationName].ToString();
		}
		else if (IsUser())
		{
			var drUser = new Users() { UsersId = lblUsersId.zToInt() }.Select().Rows[0];
			ltrParentPageName.Text = drUser[CS.Name].ToString();
		}

		LoadAuthority();
	}


	private bool IsUser()
	{
		return (!lblUsersId.zIsNullOrEmpty() && lblUsersId.zToInt() != 0);
	}

	private bool IsDesignation()
	{
		return (!lblDesignationId.zIsNullOrEmpty() && lblDesignationId.zToInt() != 0);
	}

	private void LoadAuthority()
	{
		int UsersId = CU.GetUsersId();
		if (IsUser())
		{
			var drUser = new Users() { UsersId = lblUsersId.zToInt() }.Select().Rows[0];
		}

		var dt = CU.GetAuthorityData(UsersId);

		if (IsDesignation())
		{
			dtAuthority = new DesignationAuthority()
			{
				DesignationId = lblDesignationId.zToInt(),
			}.Select();
		}
		else
		{
			dtAuthority = new UserAuthority()
			{
				UsersId = lblUsersId.zToInt(),
			}.Select();
		}

		chkAllView.Checked = chkAllAddEdit.Checked = chkAllDelete.Checked = false;

		rptAuthority.DataSource = dt;
		rptAuthority.DataBind();

		divData.Visible = dt.Rows.Count > 0;
		divNoData.Visible = dt.Rows.Count == 0;
	}

	protected void rptAuthority_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var lblAuthorityId = e.Item.FindControl("lblAuthorityId") as Label;
		var lblAuthorityName = e.Item.FindControl("lblAuthorityName") as Label;

		var chkAllRow = e.Item.FindControl("chkAllRow") as CheckBox;

		var chkView = e.Item.FindControl("chkView") as CheckBox;
		var chkAddEdit = e.Item.FindControl("chkAddEdit") as CheckBox;
		var chkDelete = e.Item.FindControl("chkDelete") as CheckBox;

		var chkFackAddEdit = e.Item.FindControl("chkFackAddEdit") as CheckBox;
		var chkFackDelete = e.Item.FindControl("chkFackDelete") as CheckBox;

		DataRowView dataItem = (DataRowView)((RepeaterItem)e.Item).DataItem;

		lblAuthorityId.Text = dataItem[CS.AuthorityId].ToString();
		lblAuthorityName.Text = dataItem[CS.AuthorityName].ToString();

		int AuthorityId = lblAuthorityId.zToInt().Value;
		if (AuthorityId == 0)
		{
			var tdAuthority = e.Item.FindControl("tdAuthority") as HtmlControl;
			var tdView = e.Item.FindControl("tdView") as HtmlControl;
			var tdAddEdit = e.Item.FindControl("tdAddEdit") as HtmlControl;
			var tdDelete = e.Item.FindControl("tdDelete") as HtmlControl;

			tdAuthority.Attributes.Add("class", "brnone");
			tdView.Attributes.Add("class", "brnone blnone selecttd");
			tdAddEdit.Attributes.Add("class", "brnone blnone selecttd");
			tdDelete.Attributes.Add("class", "blnone selecttd");

			lblAuthorityName.Attributes.Add("class", "AuthorityHead");
			chkAllRow.Visible = chkView.Visible = chkAddEdit.Visible = chkDelete.Visible = false;
		}
		else
		{
			var dr = dtAuthority.Select(CS.eAuthority + " = " + AuthorityId);

			chkView.Checked = (dr.Length > 0 && Convert.ToBoolean(dr[0][CS.IsAllowView]));
			chkAddEdit.Checked = (dr.Length > 0 && Convert.ToBoolean(dr[0][CS.IsAllowAddEdit]));
			chkDelete.Checked = (dr.Length > 0 && Convert.ToBoolean(dr[0][CS.IsAllowDelete]));

			chkAllRow.Visible = chkView.Visible = chkAddEdit.Visible = chkDelete.Visible = true;

			chkAddEdit.Visible = Convert.ToBoolean(dataItem[CS.IsAllowAddEdit].ToString());
			chkFackAddEdit.Visible = (!Convert.ToBoolean(dataItem[CS.IsAllowAddEdit].ToString()));

			chkDelete.Visible = Convert.ToBoolean(dataItem[CS.IsAllowDelete].ToString());
			chkFackDelete.Visible = (!Convert.ToBoolean(dataItem[CS.IsAllowDelete].ToString()));
		}

		//if (AuthorityId == (int)eAuthority.PaymentDeadline || AuthorityId == (int)eAuthority.ConfirmInclusion)
		//{
		//	chkAddEdit.Checked = chkFackAddEdit.Checked = false;
		//	chkAddEdit.Visible = false;
		//	chkFackAddEdit.Visible = true;
		//}
	}


	private void SaveAuthority(bool ChangeUserAuthority)
	{
		if (IsDesignation())
		{
			dtAuthority = new DesignationAuthority()
			{
				DesignationId = lblDesignationId.zToInt(),
			}.Select();
		}

		var dtUser = new DataTable();
		if (ChangeUserAuthority || IsUser())
		{
			if (IsUser())
			{
				dtAuthority = new UserAuthority() { UsersId = lblUsersId.zToInt() }.Select();
			}

			dtUser = new Query()
			{
				UsersId = IsUser() ? lblUsersId.zToInt() : (int?)null,
				DesignationId = IsUser() ? (int?)null : lblDesignationId.zToInt(),
				eStatusNot = (int)eStatus.Delete,
			}.Select(eSP.qry_User);
		}

		foreach (RepeaterItem item in rptAuthority.Items)
		{
			var lblAuthorityId = item.FindControl("lblAuthorityId") as Label;

			var chkView = item.FindControl("chkView") as CheckBox;
			var chkAddEdit = item.FindControl("chkAddEdit") as CheckBox;
			var chkDelete = item.FindControl("chkDelete") as CheckBox;

			if (!lblAuthorityId.zIsNullOrEmpty() && lblAuthorityId.zToInt() != 0)
			{
				if (dtAuthority.Select(CS.eAuthority + " = " + lblAuthorityId.zToInt()
								  + " AND " + CS.IsAllowView + " = " + chkView.Checked
								  + " AND " + CS.IsAllowAddEdit + " = " + chkAddEdit.Checked
								  + " AND " + CS.IsAllowDelete + " = " + chkDelete.Checked).Length == 0)
				{
					if (IsDesignation())
					{
						#region Designation Authority

						var objAuthority = new DesignationAuthority()
						{
							eAuthority = lblAuthorityId.zToInt(),
							DesignationId = lblDesignationId.zToInt(),
							IsAllowView = chkView.Checked,
							IsAllowAddEdit = chkAddEdit.Checked,
							IsAllowDelete = chkDelete.Checked,
						};

						SetDesignationAuthority(objAuthority);

						#endregion
					}

					if (ChangeUserAuthority || IsUser())
					{
						var objUserAuthority = new UserAuthority()
						{
							eAuthority = lblAuthorityId.zToInt(),
							IsAllowView = chkView.Checked,
							IsAllowAddEdit = chkAddEdit.Checked,
							IsAllowDelete = chkDelete.Checked,
						};

						SetUserAuthority(dtUser, objUserAuthority);
					}
				}
			}
		}

		CU.ZMessage(eMsgType.Success, string.Empty, "Authority Updated Successfully.");
	}

	private void SetDesignationAuthority(DesignationAuthority objParentAuthority)
	{
		var obj = new DesignationAuthority()
		{
			eAuthority = objParentAuthority.eAuthority,
			DesignationId = objParentAuthority.DesignationId
		};
		var lstDesignationAuthority = obj.SelectList<DesignationAuthority>();

		obj.IsAllowView = objParentAuthority.IsAllowView;
		obj.IsAllowAddEdit = objParentAuthority.IsAllowAddEdit;
		obj.IsAllowDelete = objParentAuthority.IsAllowDelete;

		if (lstDesignationAuthority.Count > 0)
		{
			obj.DesignationAuthorityId = lstDesignationAuthority[0].DesignationAuthorityId;
			obj.Update();
		}
		else
			obj.Insert();
	}

	private void SetUserAuthority(DataTable dtUser, UserAuthority objParentAuthority)
	{
		foreach (DataRow drUser in dtUser.Rows)
		{
			int? UsersId = drUser[CS.UsersId].zToInt();
			var lstUserAutority = new UserAuthority()
			{
				eAuthority = objParentAuthority.eAuthority,
				UsersId = UsersId
			}.SelectList<UserAuthority>();

			var objUserAuthority = new UserAuthority()
			{
				UsersId = UsersId,
				eAuthority = objParentAuthority.eAuthority,
				IsAllowView = objParentAuthority.IsAllowView.Value,
				IsAllowAddEdit = objParentAuthority.IsAllowAddEdit.Value,
				IsAllowDelete = objParentAuthority.IsAllowDelete.Value,
			};

			if (lstUserAutority.Count > 0)
			{
				objUserAuthority.UserAuthorityId = lstUserAutority[0].UserAuthorityId;
				objUserAuthority.Update();
			}
			else
				objUserAuthority.Insert();
		}
	}


	protected void btnSaveAuthority_OnClick(object sender, EventArgs e)
	{
		if (IsDesignation())
		{
			ConfirmPopupChangeAuthority.SetPopupType("Confirm", "Do You Want to any Change in User Authority?", true);
			popupConfirmChangeAuthority.Show();
		}
		else
		{
			btnConfirmChangeAuthority_OnClick(null, null);
		}
	}

	protected void btnConfirmChangeAuthority_OnClick(object sender, EventArgs e)
	{
		SaveAuthority(true);
	}

	protected void btnNotChangeAuthority_OnClick(object sender, EventArgs e)
	{
		SaveAuthority(false);
	}


	protected void btnResetAuthority_OnClick(object sender, EventArgs e)
	{
		LoadAuthority();
	}

	protected void btnCancelAuthority_OnClick(object sender, EventArgs e)
	{
		btnCancel_OnClick(null, null);
	}

	protected void lnkParentPage_OnClick(object sender, EventArgs e)
	{
		try { btnCancel_OnClick(null, null); }
		catch { }
	}

}
