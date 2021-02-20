using System;
using BOL;
using Utility;
using System.Web.UI;
using System.Data;

public partial class CCManageCustomer : System.Web.UI.UserControl
{
    public event EventHandler btnSaveCustomer_OnClick;
    public event EventHandler btnSaveAndNewCustomer_OnClick;

    public string SetCustomerId
    {
        get { return lblCustomerId.Text; }
        set { lblCustomerId.Text = value; }
    }


    public void LoadCustomerDetail(int? UserId)
    {
        txtName.Focus();
        if (IsEditModeCust())
        {
            lblpopupCustomerTitle.Text = "Edit Customer";
            var objCustomer = new Customer() { CustomerId = lblCustomerId.zToInt(), }.SelectList<Customer>()[0];

            lblUserId.Text = objCustomer.UsersId.ToString();
            txtName.Text = objCustomer.Name;
            txtWhatsappNo.Text = objCustomer.WhatsAppNo;
            txtMobileNo.Text = objCustomer.MobileNo;
            txtAddress.Text = objCustomer.Address;
            txtPincode.Text = objCustomer.Pincode;
            txtCity.Text = objCustomer.CityName;
            txtState.Text = objCustomer.StateName;
            txtCountry.Text = objCustomer.CountryName;
            txtCustomerNote.Text = objCustomer.CustomerNote;
        }
        else
        {
            lblpopupCustomerTitle.Text = "New Customer";
            lblUserId.Text = UserId.HasValue && UserId > 0 ? UserId.ToString() : CU.GetUsersId().ToString();
            txtName.Text = txtWhatsappNo.Text = txtMobileNo.Text = txtCustomerNote.Text = string.Empty;
            txtAddress.Text = txtPincode.Text = txtCity.Text = txtState.Text = string.Empty;
            txtCountry.Text = "India";
        }

        SetService(null);
    }


    private bool IsEditModeCust()
    {
        return !lblCustomerId.zIsNullOrEmpty();
    }

    private bool IsValidateCust()
    {
        if (txtName.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Name");
            txtName.Focus();
            return false;
        }

        if (!txtWhatsappNo.zIsNumber())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Whatsapp no.");
            txtWhatsappNo.Focus();
            return false;
        }

        if (!txtMobileNo.zIsNumber())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Mobile no.");
            txtMobileNo.Focus();
            return false;
        }

        var dtCustomerWN = new Query()
        {
            eStatusNot = (int)eStatus.Delete,
            WhatsAppNo = txtWhatsappNo.Text.Trim(),
        }.Select(eSP.qry_Customer);

        if (dtCustomerWN.Rows.Count > 0 && dtCustomerWN.Rows[0][CS.CustomerId].ToString() != lblCustomerId.Text)
        {
            string Status = dtCustomerWN.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
            CU.ZMessage(eMsgType.Error, string.Empty, "This Whatsapp no is already exist" + Status + ".");
            txtWhatsappNo.Focus();
            return false;
        }

        var dtCustomer = new Query()
        {
            eStatusNot = (int)eStatus.Delete,
            MobileNo = txtMobileNo.Text.Trim(),
        }.Select(eSP.qry_Customer);

        if (dtCustomer.Rows.Count > 0 && dtCustomer.Rows[0][CS.CustomerId].ToString() != lblCustomerId.Text)
        {
            string Status = dtCustomer.Rows[0][CS.eStatus].zToInt().Value == (int)eStatus.Deactive ? "(Deactive)" : string.Empty;
            CU.ZMessage(eMsgType.Error, string.Empty, "This mobile no is already exist" + Status + ".");
            txtMobileNo.Focus();
            return false;
        }

        if (txtAddress.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Address");
            txtAddress.Focus();
            return false;
        }

        if (txtPincode.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Pincode");
            txtPincode.Focus();
            return false;
        }

        if (txtCity.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter City");
            txtCity.Focus();
            return false;
        }

        if (txtState.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter State");
            txtState.Focus();
            return false;
        }

        if (txtCountry.zIsNullOrEmpty())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Country");
            txtCountry.Focus();
            return false;
        }

        return true;
    }

    private bool SaveDataCust()
    {
        if (!IsValidateCust())
            return false;

        string Message = string.Empty;

        var objCustomer = new Customer()
        {
            Name = txtName.Text.zFirstCharToUpper(),
            MobileNo = txtMobileNo.Text.Trim(),
            WhatsAppNo = txtWhatsappNo.Text.Trim(),
            Address = txtAddress.Text,
            Pincode = txtPincode.Text,
            CityName = txtCity.Text.zFirstCharToUpper(),
            StateName = txtState.Text.zFirstCharToUpper(),
            CountryName = txtCountry.Text.zFirstCharToUpper(),
            CustomerNote = txtCustomerNote.Text
        };

        if (IsEditModeCust())
        {
            objCustomer.CustomerId = lblCustomerId.zToInt();
            objCustomer.Update();

            Message = "Customer Detail Change Sucessfully";
        }
        else
        {
            objCustomer.eStatus = (int)eStatus.Active;
            objCustomer.UsersId = lblUserId.zToInt();
            lblCustomerId.Text = objCustomer.Insert().ToString();

            Message = "Customer Added Sucessfully";
        }

        CU.ZMessage(eMsgType.Success, string.Empty, Message);
        return true;
    }


    protected void btnSave_OnClick(object sender, EventArgs e)
    {
        if (SaveDataCust())
            btnSaveCustomer_OnClick(null, null);
    }

    protected void btnSaveAndNew_OnClick(object sender, EventArgs e)
    {
        if (SaveDataCust())
        {
            btnSaveAndNewCustomer_OnClick(null, null);
        }
    }


    protected void txtPincode_OnTextChanged(object sender, EventArgs e)
    {
        DataTable dtSA = null;
        if (txtPincode.zIsNumber())
        {
            dtSA = new Query() { OrganizationId = CU.GetOrganizationId(), Pincode = txtPincode.Text, eStatus = (int)eStatus.Active }.Select(eSP.qry_ServiceAvailability);
            if (dtSA.Rows.Count > 0)
            {
                try
                {
                    txtCity.Text = dtSA.Rows[0][CS.CityName].ToString();
                    txtState.Text = dtSA.Rows[0][CS.StateName].ToString();
                    txtCountry.Text = dtSA.Rows[0][CS.CountryName].ToString();
                }
                catch { }

            }
            else
            {
                txtCity.Text = txtState.Text = string.Empty;
                txtCountry.Text = "India";
            }
        }

        SetService(dtSA);
    }

    private void SetService(DataTable dtService)
    {
        bool IsCOD = false, IsPrepaid = false, IsReversePickup = false, IsPickup = false;

        if (dtService == null && txtPincode.zIsNumber())
            dtService = new Query() { OrganizationId = CU.GetOrganizationId(), Pincode = txtPincode.Text, eStatus = (int)eStatus.Active }.Select(eSP.qry_ServiceAvailability);

        if (dtService != null && dtService.Rows.Count > 0)
        {
            IsCOD = CheckService(dtService, CS.eCOD);
            IsPrepaid = CheckService(dtService, CS.ePrepaid);
            IsReversePickup = CheckService(dtService, CS.eReversePickup);
            IsPickup = CheckService(dtService, CS.ePickup);

            divCOD.addClass(IsCOD ? "bg-success" : "bg-red");
            divPrepaid.addClass(IsPrepaid ? "bg-success" : "bg-purple");
            divReversePickup.addClass(IsReversePickup ? "bg-success" : "bg-red");
            divPickup.addClass(IsPickup ? "bg-success" : "bg-red");

            divCOD.removeClass(!IsCOD ? "bg-success" : "bg-red");
            divPrepaid.removeClass(!IsPrepaid ? "bg-success" : "bg-purple");
            divReversePickup.removeClass(!IsReversePickup ? "bg-success" : "bg-red");
            divPickup.removeClass(!IsPickup ? "bg-success" : "bg-red");

            lblCODStatus.Text = IsCOD ? "Available" : "Unvailable";
            lblPrepaidStatus.Text = IsPrepaid ? "Available" : "Available";
            lblReversePickupStatus.Text = IsReversePickup ? "Available" : "Unvailable";
            lblPickupStatus.Text = IsPickup ? "Available" : "Unvailable";
        }

        pnlService.Visible = dtService != null && dtService.Rows.Count > 0;
    }

    private bool CheckService(DataTable dtService, string ColumnName)
    {
        return dtService.Select(ColumnName + " = " + (int)eYesNo.Yes).Length > 0;
    }
}
