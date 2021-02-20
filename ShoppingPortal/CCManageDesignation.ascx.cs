using System;
using BOL;
using Utility;
using System.Data;

public partial class CCManageDesignation : System.Web.UI.UserControl
{
	public event EventHandler btnSave_OnClick;
	public event EventHandler btnSaveAndNew_OnClick;

	public string SetDesignationId
	{
		get { return lblDesignationId.Text; }
		set { lblDesignationId.Text = value; }
	}

	private void LoadDesignation()
	{
		var dtDesignation = CU.GetEnumDt<eDesignation>(string.Empty);
		var lstDesignation = CU.GetlstAuthoDesignation();

		var dt = dtDesignation.Copy();
		foreach (DataRow drDesignation in dtDesignation.Rows)
		{
			if (!lstDesignation.Contains(drDesignation[CS.Id].zToInt().Value))
				dt.Rows.Remove(dt.Select(CS.Id + " = " + drDesignation[CS.Id].zToInt())[0]);
		}

		var dr = dt.NewRow();
		dr[CS.Id] = 0;
		dr[CS.Name] = "-- Select Designation --";
		dt.Rows.InsertAt(dr, 0);

		ddlDesignation.DataSource = dt;
		ddlDesignation.DataValueField = CS.Id;
		ddlDesignation.DataTextField = CS.Name;
		ddlDesignation.DataBind();
	}

	public void LoadDesignationDetail(bool IsOnlyEdit)
	{
		lblOrganizationId.Text = CU.GetOrganizationId().ToString();

		btnSaveAndNewDesignation.Visible = !IsOnlyEdit;

		LoadDesignation();
		ddlDesignation.Focus();

		if (IsEditMode())
		{
			lblPopupTitle.Text = "Edit Designation";
			var objDesignation = new Designation() { DesignationId = lblDesignationId.zToInt(), }.SelectList<Designation>()[0];

			txtDesignationName.Text = objDesignation.DesignationName;
			txtSerialNo.Text = objDesignation.SerialNo.ToString();
			ddlDesignation.SelectedValue = objDesignation.eDesignation.ToString();
		}
		else
		{
			lblPopupTitle.Text = "New Designation";
			txtDesignationName.Text = txtSerialNo.Text = string.Empty;

			var drMaxSerialNo = new Query() { eStatusNot = (int)eStatus.Delete, OrganizationId = lblOrganizationId.zToInt() }.Select(eSP.qry_Max_DesignationSerialNo).Rows[0];
			int? SerialNo = drMaxSerialNo[CS.SerialNo].zToInt();
			txtSerialNo.Text = SerialNo.HasValue ? (SerialNo + 1).ToString() : "1";
		}
	}

	private bool IsEditMode()
	{
		return !lblDesignationId.zIsNullOrEmpty();
	}

	private bool IsValidate()
	{
		if (!ddlDesignation.zIsSelect())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Designaation.");
			ddlDesignation.Focus();
			return false;
		}

		var dteDesignation = new Query()
		{
			eStatusNot = (int)eStatus.Delete,
			eDesignation = ddlDesignation.zToInt(),
			OrganizationId = lblOrganizationId.zToInt()
		}.Select(eSP.qry_Designation);

		if (dteDesignation.Rows.Count > 0 && dteDesignation.Rows[0][CS.DesignationId].ToString() != lblDesignationId.Text)
		{
			string Status = dteDesignation.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
			CU.ZMessage(eMsgType.Error, string.Empty, "This Designation is already exist" + Status + ".");
			ddlDesignation.Focus();
			return false;
		}


		if (txtDesignationName.zIsNullOrEmpty())
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Designation Name.");
			txtDesignationName.Focus();
			return false;
		}

		var dtDesignation = new Query()
		{
			eStatusNot = (int)eStatus.Delete,
			DesignationName = txtDesignationName.Text.Trim(),
			OrganizationId = lblOrganizationId.zToInt()
		}.Select(eSP.qry_Designation);

		if (dtDesignation.Rows.Count > 0 && dtDesignation.Rows[0][CS.DesignationId].ToString() != lblDesignationId.Text)
		{
			string Status = dtDesignation.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
			CU.ZMessage(eMsgType.Error, string.Empty, "This Designation Name is already exist" + Status + ".");
			txtDesignationName.Focus();
			return false;
		}

		if (!txtSerialNo.zIsInteger(false))
		{
			CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Serial No.");
			txtSerialNo.Focus();
			return false;
		}

		return true;
	}

	private bool SaveData()
	{
		if (!IsValidate())
			return false;

		string Message = string.Empty;

		var objDesignation = new Designation()
		{
			OrganizationId = lblOrganizationId.zToInt(),
			DesignationName = txtDesignationName.Text.Trim().zFirstCharToUpper(),
			SerialNo = txtSerialNo.zToInt(),
			eDesignation = ddlDesignation.zToInt(),
		};

		if (IsEditMode())
		{
			objDesignation.DesignationId = lblDesignationId.zToInt();
			objDesignation.Update();

			Message = "Designation Detail Change Sucessfully.";
		}
		else
		{
			objDesignation.eStatus = (int)eStatus.Active;
			objDesignation.Insert();

			Message = "New Designation Added Sucessfully.";
		}

		CU.ZMessage(eMsgType.Success, string.Empty, Message);

		return true;
	}

	protected void btnSaveDesignation_OnClick(object sender, EventArgs e)
	{
		if (SaveData())
		{
			try { btnSave_OnClick(null, null); }
			catch { }
		}
	}

	protected void btnSaveAndNewDesignation_OnClick(object sender, EventArgs e)
	{
		if (SaveData())
		{
			try { btnSaveAndNew_OnClick(null, null); }
			catch { }
		}
	}
}
