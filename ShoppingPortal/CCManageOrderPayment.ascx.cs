using System;
using BOL;
using Utility;

public partial class CCManageOrderPayment : System.Web.UI.UserControl
{
    public event EventHandler btnSave_OnClick;

    public string SetOrderPaymentId
    {
        get { return lblOrderPaymentId.Text; }
        set { lblOrderPaymentId.Text = value; }
    }

    private void LoadBankAccount()
    {
        var dtBankAccount = new BankAccount() { OrganizationId = CU.GetOrganizationId() }.Select();
        CU.FillDropdown(ref ddlBankAccount, dtBankAccount, "-- Select BankAccount --", CS.BankAccountId, CS.BankAccountName);
    }

    public void LoadOrderPaymentDetail(int OrdersId)
    {
        ddlBankAccount.Focus();

        LoadBankAccount();
        if (IsEditMode())
        {
            lblPopupTitle.Text = "Edit Payment";
            var objOrderPayment = new OrderPayment() { OrderPaymentId = lblOrderPaymentId.zToInt(), }.SelectList<OrderPayment>()[0];

            OrdersId = objOrderPayment.OrdersId.Value;
            ddlBankAccount.SelectedValue = objOrderPayment.BankAccountId.ToString();
            txtPaymentDate.Text = objOrderPayment.PaymentDate.Value.ToString(CS.ddMMyyyy);
            txtAmount.Text = objOrderPayment.Amount.ToString().Replace(".00", "");
            txtTransactionId.Text = objOrderPayment.TransactionId;
            txtPaymentNote.Text = objOrderPayment.Note;
        }
        else
        {
            lblPopupTitle.Text = "New Payment";

            ddlBankAccount.SelectedValue = "0";
            txtPaymentDate.Text = IndianDateTime.Today.ToString(CS.ddMMyyyy);

            var objOrders = new Orders() { OrdersId = OrdersId, }.SelectList<Orders>()[0];
            var dtOrderPayment = new OrderPayment() { OrdersId = OrdersId }.Select();
            int? PaidAmount = dtOrderPayment.Compute("Sum(Amount)", string.Empty).zToInt();
            PaidAmount = PaidAmount.HasValue ? PaidAmount : 0;
            txtAmount.Text = ((objOrders.SalePrice + objOrders.CustomerShipCharge) - PaidAmount).ToString().Replace(".00", "");

            txtPaymentNote.Text = string.Empty;
        }

        lblOrdersId.Text = OrdersId.ToString();

    }


    private bool IsEditMode()
    {
        return !lblOrderPaymentId.zIsNullOrEmpty();
    }

    private bool IsValidate()
    {
        if (!ddlBankAccount.zIsSelect())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Bank Account.");
            ddlBankAccount.Focus();
            return false;
        }

        if (!txtPaymentDate.zIsDate())
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Payment Date.");
            txtPaymentDate.Focus();
            return false;
        }

        if (!txtAmount.zIsDecimal(false))
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Amount.");
            txtAmount.Focus();
            return false;
        }

        if (!txtTransactionId.zIsNullOrEmpty())
        {
            var dtOrderPayment = new OrderPayment()
            {
                TransactionId = txtTransactionId.Text.Trim(),
            }.Select();

            if (dtOrderPayment.Rows.Count > 0 && dtOrderPayment.Rows[0][CS.OrderPaymentId].ToString() != lblOrderPaymentId.Text)
            {
                CU.ZMessage(eMsgType.Error, string.Empty, "This TransactionId is already exist.");
                txtTransactionId.Focus();
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

        var objOrderPayment = new OrderPayment()
        {
            OrdersId = lblOrdersId.zToInt(),
            BankAccountId = ddlBankAccount.zToInt(),
            PaymentDate = txtPaymentDate.zToDate(),
            Amount = txtAmount.zToDecimal(),
            TransactionId = txtTransactionId.Text,
            Note = txtPaymentNote.Text,
        };

        if (IsEditMode())
        {
            objOrderPayment.OrderPaymentId = lblOrderPaymentId.zToInt();
            objOrderPayment.Update();

            Message = "Payment Detail Change Sucessfully.";
        }
        else
        {
            objOrderPayment.UsersId = CU.GetUsersId();
            objOrderPayment.OrderPaymentId = objOrderPayment.Insert();

            Message = "OrderPayment Added Sucessfully.";
        }

        CU.ZMessage(eMsgType.Success, string.Empty, Message);

        return true;
    }


    protected void btnSaveOrderPayment_OnClick(object sender, EventArgs e)
    {
        if (SaveData())
        {
            try { btnSave_OnClick(null, null); }
            catch { }
        }
    }
}
