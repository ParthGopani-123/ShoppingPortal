using System;
using BOL;
using Utility;

public partial class CCManageArea : System.Web.UI.UserControl
{
    public event EventHandler btnSave_OnClick;
    public event EventHandler btnSaveAndNew_OnClick;

    public string SetAreaId
    {
        get { return lblAreaId.Text; }
        set { lblAreaId.Text = value; }
    }


    private void LoadCountry()
    {
        int? CountryId = ddlCountry.zToInt();

        CU.FillDropdown(ref ddlCountry, new Country() { eStatus = (int)eStatus.Active }.Select(new Country() { CountryId = 0, CountryName = "" }), "-- Select Country --", CS.CountryId, CS.CountryName);

        try { ddlCountry.SelectedValue = CountryId.ToString(); }
        catch { }
    }

    private void LoadState()
    {
        int? SateId = ddlState.zToInt();

        CU.FillDropdown(ref ddlState, new State()
        {
            CountryId = ddlCountry.zToInt(),
            eStatus = (int)eStatus.Active
        }.Select(new State() { StateId = 0, StateName = "" }), "-- Select State --", CS.StateId, CS.StateName);

        try { ddlState.SelectedValue = SateId.ToString(); }
        catch { }
    }

    private void LoadCity()
    {
        int? CityId = ddlCity.zToInt();

        CU.FillDropdown(ref ddlCity, new City()
        {
            StateId = ddlState.zToInt(),
            eStatus = (int)eStatus.Active
        }.Select(new City() { CityId = 0, CityName = "" }), "-- Select City --", CS.CityId, CS.CityName);

        try { ddlCity.SelectedValue = CityId.ToString(); }
        catch { }
    }

    public void LoadAreaDetail(bool IsOnlyEdit)
    {
        btnSaveAndNewArea.Visible = !IsOnlyEdit;
        LoadCountry();
        ddlCountry.Focus();

        if (IsEditMode())
        {
            lblPopupTitle.Text = "Edit Area";
            var dtArea = new Query() { AreaId = lblAreaId.zToInt() }.Select(eSP.qry_Area);

            var objArea = new Area() { }.SelectList<Area>(dtArea.Select())[0];

            ddlCountry.SelectedValue = dtArea.Rows[0][CS.CountryId].ToString();
            LoadState();
            ddlState.SelectedValue = dtArea.Rows[0][CS.StateId].ToString();
            LoadCity();
            ddlCity.SelectedValue = objArea.CityId.ToString();

            txtAreaName.Text = objArea.AreaName;
            txtPincode.Text = objArea.Pincode;
        }
        else
        {
            lblPopupTitle.Text = "New Area";
            LoadState();
            LoadCity();
            txtAreaName.Text = txtPincode.Text = string.Empty;
        }
    }

    private bool IsEditMode()
    {
        return !lblAreaId.zIsNullOrEmpty();
    }

    private bool IsValidate()
    {
        if (!ddlCountry.zIsSelect())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Country.");
            ddlCountry.Focus();
            return false;
        }

        if (!ddlState.zIsSelect())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select State.");
            ddlState.Focus();
            return false;
        }

        if (!ddlCity.zIsSelect())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select City.");
            ddlCity.Focus();
            return false;
        }

        if (txtAreaName.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Area Name.");
            txtAreaName.Focus();
            return false;
        }

        var dtArea = new Query()
        {
            eStatusNot = (int)eStatus.Delete,
            CountryId = ddlCountry.zToInt(),
            StateId = ddlState.zToInt(),
            CityId = ddlCity.zToInt(),
            AreaName = txtAreaName.Text.Trim(),
        }.Select(eSP.qry_Area);

        if (dtArea.Rows.Count > 0 && dtArea.Rows[0][CS.AreaId].ToString() != lblAreaId.Text)
        {
            string Status = dtArea.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
            CU.ZMessage(eMsgType.Error, string.Empty, "This Area is already exist" + Status + ".");
            txtAreaName.Focus();
            return false;
        }

        if (!txtPincode.zIsNullOrEmpty())
        {
            var dtAreaPincode = new Query()
            {
                eStatusNot = (int)eStatus.Delete,
                CountryId = ddlCountry.zToInt(),
                Pincode = txtPincode.Text,
            }.Select(eSP.qry_Area);

            if (dtAreaPincode.Rows.Count > 0 && dtAreaPincode.Rows[0][CS.AreaId].ToString() != lblAreaId.Text)
            {
                string Status = dtAreaPincode.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
                CU.ZMessage(eMsgType.Error, string.Empty, "This Pincode is already exist" + Status + ".");
                txtAreaName.Focus();
                return false;
            }
        }

        return true;
    }

    private bool SaveData()
    {
        if (!IsValidate())
            return false;

        string Message = string.Empty;

        var objArea = new Area()
        {
            CityId = ddlCity.zToInt(),
            AreaName = txtAreaName.Text.Trim().zFirstCharToUpper(),
            Pincode = txtPincode.Text,
        };

        if (IsEditMode())
        {
            objArea.AreaId = lblAreaId.zToInt();
            objArea.Update();

            Message = "Area Detail Change Sucessfully.";
        }
        else
        {
            objArea.eStatus = (int)eStatus.Active;
            objArea.Insert();

            Message = "New Area Added Sucessfully.";
        }

        CU.ZMessage(eMsgType.Success, string.Empty, Message);

        return true;
    }

    protected void btnSaveArea_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            try { btnSave_OnClick(null, null); }
            catch { }
        }
    }

    protected void btnSaveAndNewArea_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            try { btnSaveAndNew_OnClick(null, null); }
            catch { }
        }
    }


    protected void ddlCountry_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        LoadState();
        LoadCity();
        ddlState.Focus();
    }

    protected void ddlState_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        LoadCity();
        ddlCity.Focus();
    }


    protected void lnkCountry_OnClick(object sender, EventArgs e)
    {
        LoadCountry();
        ddlCountry.Focus();
    }

    protected void lnkState_OnClick(object sender, EventArgs e)
    {
        LoadState();
        ddlState.Focus();
    }

    protected void lnkCity_OnClick(object sender, EventArgs e)
    {
        LoadCity();
        ddlCity.Focus();
    }
}
