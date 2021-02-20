using System;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Utility;

public static class Validation
{
    public static bool zIsNullOrEmpty(this TextBox Value)
    {
        return Value.Text.zIsNullOrEmpty();
    }

    public static bool zIsNullOrEmpty(this Label Value)
    {
        return Value.Text.zIsNullOrEmpty();
    }


    public static bool zIsSelect(this DropDownList Value)
    {
        return Value.SelectedValue.zIsSelect();
    }

    public static bool zIsSelectOther(this DropDownList Value)
    {
        return Value.SelectedValue.zIsSelectOther();
    }


    public static bool zIsURL(this TextBox Value)
    {
        return Value.Text.zIsURL();
    }

    public static bool zIsNullURL(this TextBox Value)
    {
        return Value.Text.zIsNullURL();
    }


    public static bool zIsCharacter(this TextBox Value)
    {
        return Value.Text.zIsCharacter();
    }

    public static bool zIsNullCharacter(this TextBox Value)
    {
        return Value.Text.zIsNullCharacter();
    }


    public static bool zIsFloat(this TextBox Value, bool AllowNegative)
    {
        return Value.Text.zIsFloat(AllowNegative);
    }

    public static bool zIsFloat(this Label Value, bool AllowNegative)
    {
        return Value.Text.zIsFloat(AllowNegative);
    }


    public static bool zIsInteger(this TextBox Value, bool AllowNegative)
    {
        return Value.Text.zIsInteger(AllowNegative);
    }

    public static bool zIsInteger(this Label Value, bool AllowNegative)
    {
        return Value.Text.zIsInteger(AllowNegative);
    }


    public static bool zIsDecimal(this TextBox Value, bool AllowNegative)
    {
        return Value.Text.zIsDecimal(AllowNegative);
    }

    public static bool zIsDecimal(this Label Value, bool AllowNegative)
    {
        return Value.Text.zIsDecimal(AllowNegative);
    }


    public static bool zIsNumber(this TextBox Value)
    {
        return Value.Text.zIsNumber();
    }

    public static bool zIsNullNumber(this TextBox Value)
    {
        return Value.Text.zIsNullNumber();
    }


    public static bool zIsMobile(this TextBox Value)
    {
        return Value.Text.zIsMobile();
    }

    public static bool zIsNullMobile(this TextBox Value)
    {
        return Value.Text.zIsNullMobile();
    }


    public static bool zIsEmail(this TextBox Value)
    {
        return Value.Text.zIsEmail();
    }

    public static bool zIsNullEmail(this TextBox Value)
    {
        return Value.Text.zIsNullEmail();
    }


    public static bool zIsDate(this TextBox Value)
    {
        return Value.Text.zIsDate();
    }

    public static bool zIsNullDate(this TextBox Value)
    {
        return Value.Text.zIsNullDate();
    }

    public static bool zIsDate(this Label Value)
    {
        return Value.Text.zIsDate();
    }

    public static bool zIsNullDate(this Label Value)
    {
        return Value.Text.zIsNullDate();
    }


    public static bool zIsDateTime(this TextBox Value)
    {
        return Value.Text.zIsDateTime();
    }

    public static bool zIsNullDateTime(this TextBox Value)
    {
        return Value.Text.zIsNullDateTime();
    }

    public static bool zIsDateTime24(this TextBox Value)
    {
        return Value.Text.zIsDateTime24();
    }

    public static bool zIsNullDateTime24(this TextBox Value)
    {
        return Value.Text.zIsNullDateTime24();
    }


    public static bool zIsTime(this TextBox Value)
    {
        return Value.Text.zIsTime();
    }

    public static bool zIsNullTime(this TextBox Value)
    {
        return Value.Text.zIsNullTime();
    }

    public static bool zIsTime24(this TextBox Value)
    {
        return Value.Text.zIsTime24();
    }

    public static bool zIsNullTime24(this TextBox Value)
    {
        return Value.Text.zIsNullTime24();
    }



    public static bool zIsPastDate(this TextBox Value)
    {
        DateTime? Date = Value.zToDate();
        return (Date.HasValue && Date.Value <= IndianDateTime.Today);
    }

    //public static bool zIsNullPastDate(this object Value)
    //{
    //    return Value.zIsNullOrEmpty() || Value.zIsPastDate();
    //}

    //public static bool zIsNullPastDate(this TextBox Value)
    //{
    //    return Value.Text.zIsPastDate();
    //}


    public static bool zIsFutureDate(this TextBox Value)
    {
        DateTime? Date = Value.zToDate();
        return (Date.HasValue && Date.Value >= IndianDateTime.Today);
    }

    //public static bool zIsNullFutureDate(this TextBox Value)
    //{
    //    return Value.Text.zIsNullFutureDate();
    //}


    public static bool zIsPhone(this TextBox Value)
    {
        return Value.Text.zIsPhone();
    }

    public static bool zIsNullPhone(this TextBox Value)
    {
        return Value.Text.zIsNullPhone();
    }


    public static bool zIsFileName(this TextBox Value)
    {
        return Value.Text.zIsFileName();
    }

    public static bool zIsNullFileName(this TextBox Value)
    {
        return Value.Text.zIsNullFileName();
    }


    public static int? zToInt(this TextBox Value)
    {
        return Value.Text.zToInt();
    }

    public static int? zToInt(this Label Value)
    {
        return Value.Text.zToInt();
    }

    public static int? zToInt(this DropDownList Value)
    {
        return Value.Text.zToInt();
    }


    public static float? zToFloat(this TextBox Value)
    {
        return Value.Text.zToFloat();
    }

    public static float? zToFloat(this Label Value)
    {
        return Value.Text.zToFloat();
    }


    public static decimal? zToDecimal(this TextBox Value)
    {
        return Value.Text.zToDecimal();
    }

    public static decimal? zToDecimal(this Label Value)
    {
        return Value.Text.zToDecimal();
    }


    public static DateTime? zToDate(this TextBox Value)
    {
        return Value.Text.zToDate();
    }

    public static DateTime? zToDate(this Label Value)
    {
        return Value.Text.zToDate();
    }


    public static DateTime? zToDateTime(this TextBox Value)
    {
        return Value.Text.zToDateTime();
    }

    public static DateTime? zToDateTime(this Label Value)
    {
        return Value.Text.zToDateTime();
    }

    public static DateTime? zToDateTime24(this TextBox Value)
    {
        return Value.Text.zToDateTime24();
    }

    public static DateTime? zToDateTime24(this Label Value)
    {
        return Value.Text.zToDateTime24();
    }

    public static DateTime? zToTime(this TextBox Value)
    {
        return Value.Text.zToTime();
    }

    public static DateTime? zToTime(this Label Value)
    {
        return Value.Text.zToTime();
    }

    public static DateTime? zToTime24(this TextBox Value)
    {
        return Value.Text.zToTime24();
    }

    public static DateTime? zToTime24(this Label Value)
    {
        return Value.Text.zToTime24();
    }


    public static bool zIsValidSelection(this GridView grdValidSelect, TextBox txtSetId, string CheckBox, string ColumnName)
    {
        Label lblSetId = new Label();
        lblSetId.Text = string.Empty;
        bool Retunval = grdValidSelect.zIsValidSelection(lblSetId, CheckBox, ColumnName);
        txtSetId.Text = lblSetId.Text;
        return Retunval;
    }

    public static bool zIsValidSelection(this GridView grdValidSelect, Label lblSetId, string CheckBox, string ColumnName)
    {
        int SelectedCount = 0;
        int? SetId = null;
        foreach (GridViewRow gvrow in grdValidSelect.Rows)
        {
            CheckBox chk = (CheckBox)gvrow.FindControl(CheckBox);
            if (chk.Checked)
            {
                SetId = Convert.ToInt32(gvrow.Cells[CU.GetColumnIndexByName(grdValidSelect, ColumnName)].Text);
                SelectedCount++;
                if (SelectedCount >= 2)
                    break;
            }
        }
        if (SelectedCount == 1)
            lblSetId.Text = SetId.ToString();
        else if (SelectedCount == 0)
        {
            CU.ZMessage(BOL.eMsgType.Error, string.Empty, "Please Select One Record.");
            return false;
        }
        else
        {
            CU.ZMessage(BOL.eMsgType.Error, string.Empty, "You Can Select Only One Record At A Time.");
            return false;
        }
        return true;
    }

    public static bool zIsValidSelection(this Repeater rptValidSelect, Label lblSetId, string CheckBox, string lblControlId)
    {
        int SelectedCount = 0;
        int? SetId = null;
        foreach (RepeaterItem item in rptValidSelect.Items)
        {
            var chk = item.FindControl(CheckBox) as CheckBox;
            var ControlId = item.FindControl(lblControlId) as Label;
            if (chk.Checked)
            {
                SetId = ControlId.zToInt();
                SelectedCount++;
                if (SelectedCount >= 2)
                    break;
            }
        }
        if (SelectedCount == 1)
            lblSetId.Text = SetId.ToString();
        else if (SelectedCount == 0)
        {
            CU.ZMessage(BOL.eMsgType.Error, string.Empty, "Please Select One Record.");
            return false;
        }
        else
        {
            CU.ZMessage(BOL.eMsgType.Error, string.Empty, "You Can Select Only One Record At A Time.");
            return false;
        }
        return true;
    }

    public static bool zIsValidSelection(this GridView grdValidSelect, Label lblSetId, string CheckBox)
    {
        int SelectedCount = 0;
        int? SetId = null;
        foreach (GridViewRow gvrow in grdValidSelect.Rows)
        {
            //CheckBox chk = new CheckBox();
            //foreach (System.Web.UI.Control RowControl in gvrow.Controls)
            //{
            //    if (RowControl.ID == CheckBox)
            //    {
            //        chk = (CheckBox)RowControl;
            //        break;
            //    }
            //}
            CheckBox chk = (CheckBox)gvrow.FindControl(CheckBox);
            if (chk.Checked)
            {
                SetId = Convert.ToInt32(chk.Text);
                SelectedCount++;
                if (SelectedCount >= 2)
                    break;
            }
        }
        if (SelectedCount == 1)
            lblSetId.Text = SetId.ToString();
        else if (SelectedCount == 0)
        {
            CU.ZMessage(BOL.eMsgType.Error, string.Empty, "Please Select Record.");
            return false;
        }
        else
        {
            CU.ZMessage(BOL.eMsgType.Error, string.Empty, "You Can Select Only One Record At A Time.");
            return false;
        }
        return true;
    }

    public static System.Collections.Generic.List<int> zIsValidSelection(this GridView grdValidSelect, string CheckBox, string ColumnName)
    {
        var lstSelectRow = new System.Collections.Generic.List<int>();
        foreach (GridViewRow gvrow in grdValidSelect.Rows)
        {
            CheckBox chk = (CheckBox)gvrow.FindControl(CheckBox);
            if (chk.Checked)
                lstSelectRow.Add(Convert.ToInt32(gvrow.Cells[CU.GetColumnIndexByName(grdValidSelect, ColumnName)].Text));
        }

        return lstSelectRow;
    }

    public static System.Collections.Generic.List<int> zIsValidSelection(this Repeater rptValidSelect, string CheckBox, string lblControlId)
    {
        var lstSelectRow = new System.Collections.Generic.List<int>();
        foreach (RepeaterItem item in rptValidSelect.Items)
        {
            var chk = item.FindControl(CheckBox) as CheckBox;
            var ControlId = item.FindControl(lblControlId) as Label;
            if (chk.Checked)
                lstSelectRow.Add(ControlId.zToInt().Value);
        }
        return lstSelectRow;
    }


    public static string zFirstCharToUpper(this TextBox txtValue)
    {
        return txtValue.Text.zFirstCharToUpper();
    }

    public static string zFirstCharToUpper(this string Value)
    {
        if (Value != null)
        {
            Value = Value.Trim();
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Value.ToLower());
        }
        else
            return string.Empty;
    }


    public static string zRemoveHTML(this string value)
    {
        var step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;", "").Trim();
        return Regex.Replace(step1, @"\s{2,}", " ");
    }

    public static string zToTimeSpan(this object strTimeSpan, string TimeFormate)
    {
        try { return IndianDateTime.Today.Add(TimeSpan.Parse(strTimeSpan.ToString())).ToString(TimeFormate); }
        catch { return string.Empty; }
    }
}