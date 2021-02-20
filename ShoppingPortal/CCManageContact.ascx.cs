using System;
using BOL;
using Utility;
using System.Web.UI.WebControls;
using System.Threading.Tasks;

public partial class CCManageContact : System.Web.UI.UserControl
{
    bool isSerFocus = false;
    protected void rptContacts_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var lblPK = e.Item.FindControl("lblPK") as Label;
        var lblContactId = e.Item.FindControl("lblContactId") as Label;

        var txtContactName = e.Item.FindControl("txtContactName") as TextBox;
        var txtContactNo = e.Item.FindControl("txtContactNo") as TextBox;
        var lnkDeleteContact = e.Item.FindControl("lnkDeleteContact") as LinkButton;

        var dataItem = (System.Data.DataRowView)((RepeaterItem)e.Item).DataItem;

        lblPK.Text = dataItem["PK"].ToString();
        lblContactId.Text = dataItem[CS.ContactId].ToString();

        txtContactName.Text = dataItem[CS.ContactName].ToString();
        txtContactNo.Text = dataItem[CS.ContactText].ToString();

        lnkDeleteContact.CommandArgument = dataItem["PK"].ToString();

        if (isSerFocus && e.Item.ItemIndex == rptContacts.Items.Count)
            txtContactName.Focus();
    }

    protected void lnkAddNewContact_OnClick(object sender, EventArgs e)
    {
        ManageContacts(null, true, false);
    }

    protected void lnkDeleteContact_OnClick(object sender, EventArgs e)
    {
        ManageContacts(((LinkButton)sender).CommandArgument.ToString().zToInt(), false, true);
    }


    private void ManageContacts(int? PK, bool IsAdd, bool IsDelete)
    {
        var dtContacts = ((System.Data.DataTable)ViewState["dtContacts"]);
        dtContacts = dtContacts.Clone();

        foreach (RepeaterItem item in rptContacts.Items)
        {
            var lblPK = item.FindControl("lblPK") as Label;
            var lblContactId = item.FindControl("lblContactId") as Label;
            var txtContactName = item.FindControl("txtContactName") as TextBox;
            var txtContactNo = item.FindControl("txtContactNo") as TextBox;

            var drContacts = dtContacts.NewRow();
            drContacts["PK"] = lblPK.Text;
            drContacts[CS.ContactId] = lblContactId.Text;
            drContacts[CS.ContactName] = txtContactName.Text;
            drContacts[CS.ContactText] = txtContactNo.Text;

            dtContacts.Rows.Add(drContacts);
        }

        if (IsAdd)
        {
            var drContacts = dtContacts.NewRow();
            drContacts["PK"] = dtContacts.Rows.Count + 1;
            drContacts[CS.ContactId] = string.Empty;
            drContacts[CS.ContactName] = string.Empty;
            drContacts[CS.ContactText] = string.Empty;

            dtContacts.Rows.Add(drContacts);
        }
        else if (IsDelete)
        {
            dtContacts.Rows.Remove(dtContacts.Select("PK = " + PK)[0]);
        }


        isSerFocus = true;
        rptContacts.DataSource = dtContacts;
        rptContacts.DataBind();
        ViewState["dtContacts"] = dtContacts;
    }


    public bool IsValidateContactDetail()
    {
        foreach (RepeaterItem item in rptContacts.Items)
        {
            var txtContactName = item.FindControl("txtContactName") as TextBox;
            var txtContactNo = item.FindControl("txtContactNo") as TextBox;

            if (!txtContactName.zIsNullOrEmpty() || !txtContactNo.zIsNullOrEmpty())
            {
                if (txtContactName.zIsNullOrEmpty())
                {
                    CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Contact Name.");
                    txtContactName.Focus();
                    return false;
                }

                if (txtContactNo.zIsNullOrEmpty())
                {
                    CU.ZMessage(eMsgType.Error, string.Empty, "Please Enter Valid Contact No.");
                    txtContactNo.Focus();
                    return false;
                }
            }
        }
        return true;
    }

    public void SaveContactDetail(int ParentId, int eParentType)
    {
        var dtContactDetail = new Query()
        {
            ParentId = ParentId,
            eParentType = eParentType,
        }.Select(eSP.qry_Contacts);
        foreach (RepeaterItem item in rptContacts.Items)
        {
            var lblContactId = item.FindControl("lblContactId") as Label;
            var txtContactName = item.FindControl("txtContactName") as TextBox;
            var txtContactNo = item.FindControl("txtContactNo") as TextBox;

            var objContact = new Contacts()
            {
                ContactId = lblContactId.zToInt(),
                ContactName = txtContactName.Text,
                ContactText = txtContactNo.Text,
                eParentType = eParentType,
                ParentId = ParentId,
            };
            if (objContact.ContactId.HasValue)
            {
                dtContactDetail.Rows.Remove(dtContactDetail.Select(CS.ContactId + " = " + objContact.ContactId)[0]);
                objContact.UpdateAsync();
            }
            else
                objContact.InsertAsync();
        }

        foreach (System.Data.DataRow drContactDetail in dtContactDetail.Rows)
        {
            new Contacts() { ContactId = drContactDetail[CS.ContactId].ToString().zToInt() }.DeleteAsync();
        }
    }

    public void LoadContactDetail(int? ParentId, int eParentType)
    {
        bool IsDefault = true;
        var dtContacts = new System.Data.DataTable();
        dtContacts.Columns.Add("PK");
        dtContacts.Columns.Add(CS.ContactId);
        dtContacts.Columns.Add(CS.ContactName);
        dtContacts.Columns.Add(CS.ContactText);

        if (ParentId.HasValue)
        {
            var DBdtContacts = new Query()
            {
                ParentId = ParentId,
                eParentType = eParentType,
            }.Select(eSP.qry_Contacts);

            if (DBdtContacts.Rows.Count > 0)
            {
                IsDefault = false;

                int i = 0;
                foreach (System.Data.DataRow DBdrContacts in DBdtContacts.Rows)
                {
                    i++;
                    var drContacts = dtContacts.NewRow();
                    drContacts["PK"] = i;
                    drContacts[CS.ContactId] = DBdrContacts[CS.ContactId];
                    drContacts[CS.ContactName] = DBdrContacts[CS.ContactName];
                    drContacts[CS.ContactText] = DBdrContacts[CS.ContactText];

                    dtContacts.Rows.Add(drContacts);
                }
            }
        }

        if (IsDefault)
        {
            var drContacts = dtContacts.NewRow();
            drContacts["PK"] = 1;
            drContacts[CS.ContactId] = string.Empty;
            drContacts[CS.ContactName] = string.Empty;
            drContacts[CS.ContactText] = string.Empty;

            dtContacts.Rows.Add(drContacts);
        }

        rptContacts.DataSource = dtContacts;
        rptContacts.DataBind();

        ViewState["dtContacts"] = dtContacts;
    }

    public string GetContactName()
    {
        try { return (rptContacts.Items[0].FindControl("txtContactName") as TextBox).Text; }
        catch { return ""; }
    }
}
