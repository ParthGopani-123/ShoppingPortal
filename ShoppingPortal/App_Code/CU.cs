using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Net.Mail;
using Utility;
using BOL;
using System.Xml.Linq;
using System.Threading;

public static class CU
{
    public static bool? IsDevelopeMode = CheckDevelopeMode();

    //public static string UploadStaticFilePath;
    //public static string UploadFilePath;
    //public static string ImageReplacePath;


    public static string HTTP = CheckDevelopeMode() ? "http://" : "https://";

    public static string StaticFilePath
    {
        get
        {
            if (HttpContext.Current == null || HttpContext.Current.Request == null)
                return HTTP + "static.octfis.com/";
            else
                return IsDevelopeMode.Value ? HTTP + HttpContext.Current.Request.Url.Authority + "/" : HTTP + "static.octfis.com/";
        }
    }

    public static bool CheckDevelopeMode()
    {
        if (!IsDevelopeMode.HasValue)
            return (File.Exists(@"D:\work\Developer.txt"));
        else
            return IsDevelopeMode.Value;
    }

    public static void RepairDescription()
    {
        var dtProduct = new Query() { ProductSearch = "1359" }.Select(eSP.qry_ProductSearch);

        for (int i = 0; i < dtProduct.Rows.Count; i++)
        {
            string Description = CorrectDescription(dtProduct.Rows[i]["Descriptions"].ToString());
            new Product() { ProductId = dtProduct.Rows[i]["ProductId"].zToInt(), Description = Description }.Update();

            #region Learning
            //"👉🏿 👉 👉🏼 💵 🚛";
            //string d = "💵 Price : 1049/- + Shiping Charge🚛 COD Services Available👉🏿 Lengha: Banglori👉🏿 Choli: Banglori👉🏿 Dupatta: Net👉 Work: have Emrodary👉🏼 Type: semi - stich";

            //var sss = "😂😁👉🏿💵asdf";
            //var block = @"\uD83D[\uDC00-\uDFFF]|\uD83C[\uDC00-\uDFFF]|\uFFFD";
            //var pat = string.Format(@"{0}.*?\s*(?={0})", block);
            //string strrr = System.Text.RegularExpressions.Regex.Replace(sss, pat, "<br>");

            //Description = System.Text.RegularExpressions.Regex.Replace(Description, pat, "<br>");
            //Description = Description.Replace("<br><br>", "<br>");

            //Your code goes here
            //string serializeString = "😂😁👉🏿💵asdf";
            //serializeString = System.Text.RegularExpressions.Regex.Replace(serializeString, @"\uD83D[\uDC00-\uDFFF]|\uD83C[\uDC00-\uDFFF]|\uFFFD", "");

            //string str = serializeString;

            //var lstSymbol = new List<string>();
            //foreach (string Symbol in lstSymbol)
            //    Description = Description.Replace(Symbol, "<br>" + Symbol);

            //Description = Description.Replace("👉🏿", " < br/>👉🏿");
            //Description = Description;
            //string Desc = Description;

            //for (int s = 0; s < Description.Length; s++)
            //{
            //    if (Description[s].ToString().Length > 1)
            //    {
            //        if (!lstSymbol.Contains(Description[s].ToString()))
            //            lstSymbol.Add(Description[s].ToString());
            //        //Description = Description.Replace(Description[s].ToString(), "<br>" + Description[s].ToString());
            //    }
            //}

            //t =char.IsSymbol('👉🏿');
            //new Product() { ProductId = 3, Description = Description }.Update();
            #endregion Learning
        }
    }

    public static string CorrectDescription(string Description)
    {
        Description = Description.zRemoveHTML();
        Description = Description.Replace("👉🏿", "<br>👉🏿");
        Description = Description.Replace("👉", "<br>👉");
        Description = Description.Replace("👉🏼", "<br>👉🏼");
        Description = Description.Replace("💵", "<br>💵");
        Description = Description.Replace("🚛", "<br>🚛");
        Description = Description.Replace("✔", "<br>✔");
        Description = Description.Replace("#", "<br>#");
        Description = Description.Replace("<br><br>", "<br>");
        if (Description.StartsWith("<br>"))
            Description = Description.Substring(4);

        return Description;
    }

    public static void AssignValues()
    {
        IsDevelopeMode = File.Exists(@"D:\work\Developer.txt");

        //UploadStaticFilePath = "static.octfis.com/ShoppingPortal";

        //UploadFilePath = "shop.octfis.com";
        //ImageReplacePath = "shop.octfis.com";
    }


    public static string ServerTimeZoneName
    {
        get
        {
            return TimeZone.CurrentTimeZone.StandardName;
        }
    }

    public static string GetDateTimeName()
    {
        return IndianDateTime.Now.ToString("ddMMyyHHmmss");
    }

    public static int CreateDefaultAdmin()
    {
        int FirmId = 0;
        var dtFirm = new Query() { eStatus = (int)eStatus.Active }.Select(eSP.qry_Firm);
        if (dtFirm.Rows.Count == 0)
        {
            int OrganizationId = 0;
            var dtOrganization = new Query() { eStatus = (int)eStatus.Active }.Select(eSP.qry_Organization);
            if (dtOrganization.Rows.Count == 0)
            {
                OrganizationId = new Organization()
                {
                    OrganizationName = "System Organization",
                    OrgUId = 0,
                    eStatus = (int)eStatus.Active,
                }.Insert();
            }
            else
                OrganizationId = dtOrganization.Rows[0][CS.OrganizationId].zToInt().Value;

            FirmId = new Firm()
            {
                OrganizationId = OrganizationId,
                FirmName = "System Firm",
                AddressId = 0,
                eStatus = (int)eStatus.Active,
            }.Insert();
        }
        else
            FirmId = dtFirm.Rows[0][CS.FirmId].zToInt().Value;

        int UsersId = new Users()
        {
            FirmId = FirmId,
            Name = "System Admin",
            AddressId = 0,
            DesignationId = CU.GetDesignationId(eDesignation.SystemAdmin),
            eStatus = (int)eStatus.Active,
            MobileNo = "9999999999",
            ParentUsersId = 0,
            PriceListId = 0,
        }.Insert();

        LoginUtilities.CreateLogin(UsersId, "superadmin", "123456");

        return UsersId;
    }

    public static int GetUsersId()
    {
        try { return GetMasterPageLabel("lblMstUsersId").zToInt().Value; }
        catch { }

        try
        {
            int UsersId = Convert.ToInt32(HttpContext.Current.Session[CS.UsersId].ToString());

            try { GetMasterPageLabel("lblMstUsersId").Text = UsersId.ToString(); }
            catch { }

            return UsersId;
        }
        catch { return 0; }
    }

    public static int GetFirmId()
    {
        var dtUser = new Query() { UsersId = CU.GetUsersId() }.Select(eSP.qry_UserFirmOrganizationId);
        if (dtUser.Rows.Count > 0)
            return dtUser.Rows[0][CS.FirmId].zToInt().Value;
        else
            return 0;
    }

    public static int GetOrganizationId()
    {
        var dtUser = new Query() { UsersId = CU.GetUsersId() }.Select(eSP.qry_UserFirmOrganizationId);
        if (dtUser.Rows.Count > 0)
            return dtUser.Rows[0][CS.OrganizationId].zToInt().Value;
        else
            return 0;
    }

    public static void GetFirmOrganizationId(ref int FirmId, ref int OrganizationId)
    {
        var dtUser = new Query() { UsersId = CU.GetUsersId() }.Select(eSP.qry_UserFirmOrganizationId);
        if (dtUser.Rows.Count > 0)
        {
            FirmId = dtUser.Rows[0][CS.FirmId].zToInt().Value;
            OrganizationId = dtUser.Rows[0][CS.OrganizationId].zToInt().Value;
        }
        else
        {
            FirmId = 0;
            OrganizationId = 0;
        }
    }

    public static eOrganisation GeteOrganisationId(int OrganizationId)
    {
        try { return (eOrganisation)new Organization() { OrganizationId = OrganizationId }.Select(new Organization() { OrgUId = 0 }).Rows[0][CS.OrgUId].zToInt(); }
        catch { return 0; }
    }


    public static int GetDesignationId(eDesignation Designation, int OrganizationId)
    {
        var dtDesignation = new Designation() { eDesignation = (int)Designation, OrganizationId = OrganizationId }.Select(new Designation() { DesignationId = 0, eStatus = 0 });
        if (dtDesignation.Rows.Count > 0)
        {
            if (dtDesignation.Rows[0][CS.eStatus].zToInt() == (int)eStatus.Deactive
                || dtDesignation.Rows[0][CS.eStatus].zToInt() == (int)eStatus.Delete)
            {
                new Designation()
                {
                    DesignationId = dtDesignation.Rows[0][CS.DesignationId].zToInt(),
                    eStatus = (int)eStatus.Active
                }.UpdateAsync();
            }

            return dtDesignation.Rows[0][CS.DesignationId].zToInt().Value;
        }
        else
        {
            return new Designation()
            {
                OrganizationId = OrganizationId,
                DesignationName = CU.GetDescription(Designation),
                eDesignation = (int)Designation,
                SerialNo = (int)Designation,
                eStatus = (int)eStatus.Active,
            }.Insert();
        }
    }

    public static int GetDesignationId(eDesignation Designation)
    {
        return GetDesignationId(Designation, CU.GetOrganizationId());
    }


    public static eDesignation GeteDesignationId(int UsersId)
    {
        try { return (eDesignation)new Designation() { DesignationId = Convert.ToInt32(new Users() { UsersId = UsersId }.Select(new Users() { DesignationId = 0 }).Rows[0][CS.DesignationId]) }.Select(new Designation() { eDesignation = 0 }).Rows[0][CS.eDesignation].zToInt(); }
        catch { return 0; }
    }

    public static eDesignation GetDesiToeDesi(int DesignationId)
    {
        try { return (eDesignation)new Designation() { DesignationId = DesignationId }.Select(new Designation() { eDesignation = 0 }).Rows[0][CS.eDesignation].zToInt(); }
        catch { return 0; }
    }

    public static eStatusType GetOrderStatusToeOrderStatus(int OrderStatusId)
    {
        try { return (eStatusType)new OrderStatus() { OrderStatusId = OrderStatusId }.Select(new OrderStatus() { eStatusType = 0 }).Rows[0][CS.eStatusType].zToInt(); }
        catch { return 0; }
    }


    public static string GetCustomAttributeValue(this Label Control, string AttributeName)
    {
        var Attributes = Control.Attributes[AttributeName];
        return Attributes == null ? string.Empty : Attributes;
    }

    public static string GetCustomAttributeValue(this TextBox Control, string AttributeName)
    {
        var Attributes = Control.Attributes[AttributeName];
        return Attributes == null ? string.Empty : Attributes;
    }

    public static string GetCustomAttributeValue(this LinkButton Control, string AttributeName)
    {
        var Attributes = Control.Attributes[AttributeName];
        return Attributes == null ? string.Empty : Attributes;
    }

    public static string GetCustomAttributeValue(System.Web.UI.HtmlControls.HtmlControl Control, string AttributeName)
    {
        var Attributes = Control.Attributes[AttributeName];
        return Attributes == null ? string.Empty : Attributes;
    }


    public static void addClass(this System.Web.UI.HtmlControls.HtmlControl Control, string ClassName)
    {
        var OldClass = GetCustomAttributeValue(Control, "class");
        if (!OldClass.ToLower().Contains(ClassName.ToLower()))
            Control.Attributes.Add("class", OldClass + " " + ClassName);
    }

    public static void removeClass(this System.Web.UI.HtmlControls.HtmlControl Control, string ClassName)
    {
        Control.Attributes.Add("class", GetCustomAttributeValue(Control, "class").Replace(ClassName, string.Empty));

        if (GetCustomAttributeValue(Control, "class").ToLower().Contains(ClassName.ToLower()))
            Control.removeClass(ClassName);
    }


    public static void addClass(this Label Control, string ClassName)
    {
        var OldClass = GetCustomAttributeValue(Control, "class");
        if (!OldClass.ToLower().Contains(ClassName.ToLower()))
            Control.Attributes.Add("class", OldClass + " " + ClassName);
    }

    public static void removeClass(this Label Control, string ClassName)
    {
        Control.Attributes.Add("class", GetCustomAttributeValue(Control, "class").Replace(ClassName, string.Empty));

        if (GetCustomAttributeValue(Control, "class").ToLower().Contains(ClassName.ToLower()))
            Control.removeClass(ClassName);
    }


    public static void addClass(this TextBox Control, string ClassName)
    {
        var OldClass = GetCustomAttributeValue(Control, "class");
        if (!OldClass.ToLower().Contains(ClassName.ToLower()))
            Control.Attributes.Add("class", OldClass + " " + ClassName);
    }

    public static void removeClass(this TextBox Control, string ClassName)
    {
        Control.Attributes.Add("class", GetCustomAttributeValue(Control, "class").Replace(ClassName, string.Empty));

        if (GetCustomAttributeValue(Control, "class").ToLower().Contains(ClassName.ToLower()))
            Control.removeClass(ClassName);
    }


    public static void addClass(this LinkButton Control, string ClassName)
    {
        var OldClass = GetCustomAttributeValue(Control, "class");
        if (!OldClass.ToLower().Contains(ClassName.ToLower()))
            Control.Attributes.Add("class", OldClass + " " + ClassName);
    }

    public static void removeClass(this LinkButton Control, string ClassName)
    {
        Control.Attributes.Add("class", GetCustomAttributeValue(Control, "class").Replace(ClassName, string.Empty));

        if (GetCustomAttributeValue(Control, "class").ToLower().Contains(ClassName.ToLower()))
            Control.removeClass(ClassName);
    }


    public static void CallJavascriptFunction(string Function)
    {
        Page page = HttpContext.Current.Handler as Page;
        ScriptManager.RegisterStartupScript(page, typeof(Page), "FunctionCall", Function, true);
    }

    public static string GetParaIn(List<int> lstPera, bool SetDefaultValue)
    {
        string strParaIn = string.Empty;
        foreach (int Item in lstPera)
            strParaIn += strParaIn.zIsNullOrEmpty() ? Item.ToString() : ", " + Item.ToString();

        if (SetDefaultValue && strParaIn.zIsNullOrEmpty())
            strParaIn = "0";

        return strParaIn;
    }

    public static string GetParaIn(ListBox lstPera, bool GetSelectedValue, bool SetDefaultValue)
    {
        string strParaIn = string.Empty;
        var lstPeraIn = new List<int>();
        foreach (ListItem Item in lstPera.Items)
        {
            if ((GetSelectedValue && Item.Selected) || (!GetSelectedValue && !Item.Selected))
                lstPeraIn.Add(Item.Value.zToInt().Value);
        }
        return GetParaIn(lstPeraIn, SetDefaultValue);
    }


    public static List<int> GetchklstSelectedValue(CheckBoxList chklst)
    {
        var lstSelectedValue = new List<int>();
        foreach (ListItem Item in chklst.Items)
        {
            if (Item.Selected)
                lstSelectedValue.Add(Item.Value.zToInt().Value);
        }

        return lstSelectedValue;
    }

    public static void setchklstSelectedValue(CheckBoxList chklst, List<int> lstSelectedValue)
    {
        foreach (ListItem Item in chklst.Items)
            Item.Selected = lstSelectedValue.Contains(Item.Value.zToInt().Value);
    }


    public static Label GetMasterPageLabel(string LabelName)
    {
        try
        {
            var pageHandler = HttpContext.Current.CurrentHandler;
            if (pageHandler is System.Web.UI.Page)
                return (((System.Web.UI.Page)pageHandler).Master.FindControl(LabelName) as Label);
        }
        catch { }

        return new Label();
    }

    #region JSON

    public static T zDeserializeJSON<T>(this string data) where T : new()
    {
        return new System.Web.Script.Serialization.JavaScriptSerializer() { MaxJsonLength = 2147483647 }.Deserialize<T>(data);
    }

    public static string zSerializeJSON(this object obj)
    {
        return new System.Web.Script.Serialization.JavaScriptSerializer() { MaxJsonLength = 2147483647 }.Serialize(obj);
    }

    #endregion

    #region Address

    public static int GetCountryId(string CountryName)
    {
        CountryName = CountryName.Trim();

        var dtCountry = new Country() { CountryName = CountryName }.Select(new Country() { CountryId = 0, eStatus = 0 });
        if (dtCountry.Rows.Count > 0)
        {
            int CountryId = Convert.ToInt32(dtCountry.Rows[0][CS.CountryId]);
            if (dtCountry.Rows[0][CS.eStatus].zToInt() != (int)eStatus.Active)
            {
                new Country()
                {
                    CountryId = CountryId,
                    eStatus = (int)eStatus.Active,
                }.UpdateAsync();
            }
            return CountryId;
        }
        else
        {
            return new Country()
            {
                CountryName = CountryName,
                eStatus = (int)eStatus.Active,
            }.Insert();
        }

    }

    public static int GetStateId(int? CountryId, string StateName)
    {
        var dtState = new State()
        {
            CountryId = CountryId,
            StateName = StateName,
        }.Select(new State() { StateId = 0, eStatus = 0 });

        if (dtState.Rows.Count > 0)
        {
            int StateId = Convert.ToInt32(dtState.Rows[0][CS.StateId]);
            if (dtState.Rows[0][CS.eStatus].zToInt() != (int)eStatus.Active)
            {
                new State()
                {
                    StateId = StateId,
                    eStatus = (int)eStatus.Active,
                }.UpdateAsync();
            }
            return StateId;
        }
        else
        {
            return new State()
            {
                StateName = StateName,
                CountryId = CountryId,
                eStatus = (int)eStatus.Active
            }.Insert();
        }
    }

    public static int GetCityId(int? ddlState, string txtOtherCity)
    {
        var dtCity = new City()
        {
            StateId = ddlState,
            CityName = txtOtherCity,
        }.Select(new City() { CityId = 0, eStatus = 0 });

        if (dtCity.Rows.Count > 0)
        {
            int CityId = Convert.ToInt32(dtCity.Rows[0][CS.CityId]);
            if (dtCity.Rows[0][CS.eStatus].zToInt() != (int)eStatus.Active)
            {
                new City()
                {
                    CityId = CityId,
                    eStatus = (int)eStatus.Active,
                }.UpdateAsync();
            }
            return CityId;
        }
        else
        {
            return new City()
            {
                CityName = txtOtherCity,
                StateId = ddlState,
                eStatus = (int)eStatus.Active
            }.Insert();
        }
    }

    public static int GetAreaId(int? ddlCity, string txtOtherArea, string txtPincode)
    {
        var dtArea = new Area()
        {
            CityId = ddlCity,
            AreaName = txtOtherArea,
        }.Select(new Area() { AreaId = 0, eStatus = 0 });
        if (dtArea.Rows.Count > 0)
        {
            int AreaId = Convert.ToInt32(dtArea.Rows[0][CS.AreaId]);
            if (dtArea.Rows[0][CS.eStatus].zToInt() != (int)eStatus.Active)
            {
                new Area()
                {
                    AreaId = AreaId,
                    eStatus = (int)eStatus.Active,
                }.UpdateAsync();
            }

            return AreaId;
        }
        else
        {
            return new Area()
            {
                AreaName = txtOtherArea,
                CityId = ddlCity,
                Pincode = txtPincode,
                eStatus = (int)eStatus.Active,
            }.Insert();

        }
    }

    #endregion

    #region enum

    public static T ToEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static int? ToEnumInt<T>(this string value)
    {
        try { return Convert.ToInt32(value.ToEnum<T>()); }
        catch { return null; }
    }


    public static void FillEnumchklst<T>(ref CheckBoxList chklst, string Caption) where T : struct
    {
        var dt = GetEnumDt<T>(Caption);
        chklst.DataSource = dt;
        chklst.DataTextField = CS.Name;
        chklst.DataValueField = CS.Id;
        chklst.DataBind();
    }

    public static void FillEnumlbx<T>(ref ListBox lbx, string Caption) where T : struct
    {
        var dt = GetEnumDt<T>(Caption);
        lbx.DataSource = dt;
        lbx.DataTextField = CS.Name;
        lbx.DataValueField = CS.Id;
        lbx.DataBind();
    }

    public static void FillEnumddl<T>(ref DropDownList ddl, string Caption) where T : struct
    {
        var dt = GetEnumDt<T>(Caption);
        ddl.DataSource = dt;
        ddl.DataTextField = CS.Name;
        ddl.DataValueField = CS.Id;
        ddl.DataBind();
    }

    public static DataTable GetEnumDt<T>(string Caption) where T : struct
    {
        Type enumType = typeof(T);
        var dtEnum = new DataTable();
        dtEnum.Columns.Add("Id", typeof(int));
        dtEnum.Columns.Add("Name");
        if (!Caption.zIsNullOrEmpty())
            dtEnum.Rows.Add(0, Caption);

        if (enumType.BaseType != typeof(Enum))
            return dtEnum;

        foreach (Enum ENum in Enum.GetValues(enumType))
            dtEnum.Rows.Add(Convert.ToInt32(ENum), GetDescription(ENum));

        if (dtEnum.Rows.Count > 0)
            dtEnum = dtEnum.Select("", "Id").CopyToDataTable();
        return dtEnum;
    }


    public static string GetDescription(Enum eInput)
    {
        Type type = eInput.GetType();

        System.Reflection.MemberInfo[] memInfo = type.GetMember(eInput.ToString());

        if (memInfo != null && memInfo.Length > 0)
        {
            object[] attrs = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (attrs != null && attrs.Length > 0)
                return ((System.ComponentModel.DescriptionAttribute)attrs[0]).Description;
        }
        return eInput.ToString();
    }

    #endregion

    #region Get Grid View Value

    public static string GetCellValueByName(GridViewRow Row, string ColumnName)
    {
        return Row.Cells[GetColumnIndexByName(Row, ColumnName)].Text;
    }

    public static int GetColumnIndexByName(GridViewRow Row, string ColumnName)
    {
        int columnIndex = 0;
        foreach (DataControlFieldCell cell in Row.Cells)
        {
            if (cell.ContainingField is BoundField)
                if (((BoundField)cell.ContainingField).DataField.Equals(ColumnName))
                    break;
            columnIndex++; // keep adding 1 while we don't have the correct name
        }
        return columnIndex;
    }

    public static int GetColumnIndexByName(GridView grdInput, string ColumnName)
    {
        int columnIndex = 0;
        for (int i = 0; i < grdInput.Columns.Count; i++)
        {
            if (grdInput.Columns[i].HeaderText == ColumnName)
                break;
            columnIndex++;
        }
        return columnIndex;
    }

    #endregion

    #region Message

    public static string ZMessage(string Message, eMsgType MsgType)
    {
        string Msg = string.Empty, Class = "success";
        if (MsgType == eMsgType.Error)
            Class = "danger";
        else if (MsgType == eMsgType.Warning)
            Class = "warning";

        Msg += "<div class='alert alert-" + Class + "'>";
        Msg += "    <button type='button' class='close' data-dismiss='alert' aria-hidden='true'>×</button>";
        Msg += Message;
        Msg += "    </div>";
        return Msg;
    }

    public static void ZMessage(eMsgType MsgType, string Title, string Message)
    {
        ZMessage(MsgType, Title, Message, 3000);
    }

    public static void ZMessage(eMsgType MsgType, string Title, string Message, int DisplaySecond)
    {
        if (DisplaySecond == 0)
            DisplaySecond = 100000000;
        string Class = "with-icon check-circle success";
        if (MsgType == eMsgType.Error)
            Class = "with-icon times-circle danger";
        else if (MsgType == eMsgType.Info)
            Class = "with-icon question-circle primary";
        else if (MsgType == eMsgType.Warning)
            Class = "with-icon exclamation-circle warning";

        Message = Message.Replace("\n", "</br>").Replace("'", " ");

        Title = string.IsNullOrEmpty(Title) ? MsgType.ToString() : Title;
        Page page = HttpContext.Current.Handler as Page;
        ScriptManager.RegisterStartupScript(page, typeof(Page), "OpenWindow", "AlertMsg('" + Title + "'," + DisplaySecond + ",'" + Message + "','" + Class + "');", true);
    }

    public static void ZMessage(eMsgType MsgType, string Title, string Message, bool IsRedirecting)
    {
        if (IsRedirecting)
        {
            HttpContext.Current.Session["ZMessage"] = new ZMessage()
            {
                MsgType = MsgType,
                Title = Title,
                Message = Message,
            };
        }
        else
            ZMessage(MsgType, Title, Message);
    }

    #endregion

    #region Name Value

    public static string GetNameValue(eNameValue NameValue)
    {
        int OrganizationId = CU.GetOrganizationId();
        var dtNameValue = new NameValue() { NameId = (int)NameValue, OrganizationId = OrganizationId }.Select(new NameValue() { Value = string.Empty });
        if (dtNameValue.Rows.Count > 0)
        {
            return dtNameValue.Rows[0]["Value"].ToString();
        }
        else
        {
            string Value = GetDefaultValue(NameValue);
            new NameValue()
            {
                NameId = (int)NameValue,
                Value = Value,
                Name = NameValue.ToString(),
                OrganizationId = OrganizationId
            }.InsertAsync();

            return Value;
        }
    }

    public static bool SetNameValue(eNameValue NameValue, string Value)
    {
        try
        {
            int OrganizationId = CU.GetOrganizationId();
            var dtNameValue = new NameValue() { NameId = (int)NameValue, OrganizationId = OrganizationId }.Select(new NameValue() { NameValueId = 0 });
            var objNameValue = new NameValue()
            {
                NameId = (int)NameValue,
                Name = NameValue.ToString(),
                Value = Value,
            };
            if (dtNameValue.Rows.Count > 0)
            {
                objNameValue.NameValueId = dtNameValue.Rows[0][CS.NameValueId].zToInt();
                objNameValue.UpdateAsync();
            }
            else
            {
                objNameValue.OrganizationId = OrganizationId;
                objNameValue.InsertAsync();
            }

            return true;
        }
        catch { return false; }
    }

    public static string GetDefaultValue(eNameValue NameValue)
    {
        string Value = string.Empty;
        switch (NameValue)
        {
            case eNameValue.DBVersion:
                Value = "1.0.0.0";
                break;
            case eNameValue.OTPValidMinute:
                Value = "10";
                break;
            case eNameValue.ForgotPasswordSMSText:
                Value = "OTP to Reset Password on Your Account is #OTP#";
                break;
            case eNameValue.SMSUserName:
                Value = "OCTFIS";
                break;
            case eNameValue.SMSPassword:
                Value = "123456";
                break;
            case eNameValue.SMSSenderId:
                Value = "OCTFIS";
                break;
            case eNameValue.SameCustomerDifferantUser:
                Value = ((int)eYesNo.Yes).ToString();
                break;
            case eNameValue.CanUserSelectCourier:
                Value = ((int)eYesNo.No).ToString(); ;
                break;
            default:
                Value = string.Empty;
                break;
        }

        return Value;
    }

    #endregion

    #region File Path

    public static bool IsImage(string FileName)
    {
        return FileName.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase)
            || FileName.EndsWith(".jpeg", StringComparison.CurrentCultureIgnoreCase)
            || FileName.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool IsGIFImage(string FileName)
    {
        return FileName.ToLower().EndsWith(".gif", StringComparison.CurrentCultureIgnoreCase)
            || FileName.ToLower().EndsWith(".gifv", StringComparison.CurrentCultureIgnoreCase);
    }


    public static bool IsVideo(string FileName)
    {
        return FileName.EndsWith(".mp4", StringComparison.CurrentCultureIgnoreCase)
            || FileName.EndsWith(".3gp", StringComparison.CurrentCultureIgnoreCase)
            || FileName.EndsWith(".wmv", StringComparison.CurrentCultureIgnoreCase);
    }

    public static string getVideoString(string URL, bool IsAutoPlay)
    {
        string AutoPlay = "";
        if (IsAutoPlay)
            AutoPlay = "autoplay";


        return @"<video id='video' src='" + URL + "' type='video/mp4' controls " + AutoPlay + " class='videotag'></video>";
    }

    public static string getAudioString(string URL, bool IsAutoPlay)
    {
        string AutoPlay = "";
        if (IsAutoPlay)
            AutoPlay = "autoplay";

        return @"<audio controls " + AutoPlay + " class='audiotag'><source src='" + URL + "'>Your browser does not support the audio element.</audio>";
    }



    public static string GettempDownloadPath()
    {
        string Path = "tempDownload";
        if (!Directory.Exists(HttpContext.Current.Server.MapPath(Path)))
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(Path));
        return Path;
    }

    public static string GetSystemImage(eSystemImage SystemImage)
    {
        return CU.StaticFilePath + "SystemImages/" + SystemImage.ToString().Replace("_EXT_", ".");
    }


    public static string GetRootURI()
    {
        return HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath;
    }

    public static string GetFileURI(string ImagePath)
    {
        return (GetRootURI() + ImagePath.Replace("//", "##")).Replace("..", "").Replace("~", "");
    }

    public static string GetFileURI(bool FullPath, ePhotoSize PhotoSize, eFolder Folder, string Id, string Extention)
    {
        return GetFileURI(GetFilePath(false, PhotoSize, Folder, Id, Extention, false, true));
    }


    public static string GetFolderPath(bool FullPath, ePhotoSize PhotoSize, eFolder Folder)
    {
        if (!File.Exists(HttpContext.Current.Server.MapPath("~/Upload/" + Folder.ToString() + "/" + PhotoSize.ToString())))
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Upload/" + Folder.ToString() + "/" + PhotoSize.ToString()));

        if (FullPath)
            return ("~/Upload/" + Folder.ToString() + "/" + PhotoSize.ToString());
        else
            return ("../Upload/" + Folder.ToString() + "/" + PhotoSize.ToString());
    }

    public static string GetFilePath(bool FullPath, ePhotoSize PhotoSize, eFolder Folder, string Id, string Extention)
    {
        return GetFilePath(FullPath, PhotoSize, Folder, Id, Extention, false, true);
    }

    public static string GetFilePath(bool FullPath, ePhotoSize PhotoSize, eFolder Folder, string Id, bool IsGetDefaultPhoto)
    {
        return GetFilePath(FullPath, PhotoSize, Folder, Id, ".jpg", IsGetDefaultPhoto, true);
    }

    public static string GetFilePath(bool FullPath, ePhotoSize PhotoSize, eFolder Folder, string Id, string Extention, bool IsGetDefaultPhoto, bool IsStaticPath)
    {
        bool GIFImage = IsGIFImage(Extention);
        if (GIFImage)
            PhotoSize = ePhotoSize.Original;

        string ImageName = Folder.ToString() + "_" + Id + Extention;
        string ImagePath = CU.GetFolderPath(true, PhotoSize, Folder) + "/" + ImageName;
        string ImagePathFull = HttpContext.Current.Server.MapPath(GetFolderPath(true, PhotoSize, Folder) + "/" + ImageName);

        if (!GIFImage && !File.Exists(ImagePathFull))
            CU.CreateThumbnailImage(Folder, Id, Extention, (int)PhotoSize);

        //if (ImagePathFull.Contains(CU.UploadFilePath))
        //{
        //    string StaticImageFullPath = IsStaticPath ? ImagePathFull.Replace(CU.UploadFilePath, CU.UploadStaticFilePath) : ImagePathFull;
        //    if (File.Exists(ImagePathFull) && !File.Exists(StaticImageFullPath))
        //        UploadFileStatic(ImagePathFull);
        //}

        if (FullPath)
        {
            ImagePath = GetFileURI(ImagePath);
            //if (IsStaticPath)
            //    ImagePath = ImagePath.Replace(CU.ImageReplacePath, CU.UploadStaticFilePath);
        }
        if (IsGetDefaultPhoto && !File.Exists(ImagePathFull))
            ImagePath = CU.StaticFilePath + "SystemImages/" + Folder.ToString() + ".jpg";

        if (FullPath)
            return ImagePath.Contains(HTTP) ? ImagePath : HTTP + ImagePath;
        else
            return ImagePath;
    }


    public static bool CheckFileExist(bool FullPath, ePhotoSize PhotoSize, eFolder Folder, string Id)
    {
        return CheckFileExist(FullPath, PhotoSize, Folder, Id, ".jpg");
    }

    public static bool CheckFileExist(bool FullPath, ePhotoSize PhotoSize, eFolder Folder, string Id, string Extention)
    {
        return File.Exists(HttpContext.Current.Server.MapPath(CU.GetFolderPath(FullPath, PhotoSize, Folder) + "/" + Folder.ToString() + "_" + Id + Extention));
    }


    public static string UploadFile(FileUpload FileUpload, List<UploadPhoto> lstUploadPhoto, eFolder Folder, string FolderPkId, bool IsSetExtention)
    {
        string NewPhotoPath = HttpContext.Current.Server.MapPath(CU.GetFolderPath(true, ePhotoSize.Original, Folder));
        if (!Directory.Exists(NewPhotoPath))
            Directory.CreateDirectory(NewPhotoPath);

        if (FileUpload != null && FileUpload.HasFile)
        {
            string Extention = (IsSetExtention) ? Path.GetExtension(FileUpload.FileName) : ".jpg";
            string ImageName = Folder.ToString() + "_" + FolderPkId + Extention;

            string OldPhotoPath = NewPhotoPath + "\\" + ImageName;
            if (File.Exists(OldPhotoPath))
                CU.MoveDeletedFile(OldPhotoPath, Folder);

            FileUpload.SaveAs(NewPhotoPath + "/" + ImageName);
            //UploadFileStatic(NewPhotoPath + "/" + ImageName);

            //if (!IsGIFImage(Extention) && IsImage(ImageName))
            //	CreateThumbnailImage(Folder, FolderPkId, Extention);

            return Extention;
        }
        else if (lstUploadPhoto.Count > 0)
        {
            foreach (UploadPhoto objUploadPhoto in lstUploadPhoto)
            {
                if (objUploadPhoto.Extention.zIsNullOrEmpty())
                    objUploadPhoto.Extention = ".jpg";

                string ImageName = Folder.ToString() + "_" + objUploadPhoto.ControlId + objUploadPhoto.Extention;

                string OldPhotoPath = NewPhotoPath + "\\" + ImageName;
                if (File.Exists(OldPhotoPath))
                    CU.MoveDeletedFile(OldPhotoPath, Folder);

                string OldFile = HttpContext.Current.Server.MapPath(objUploadPhoto.ImagePath);
                if (objUploadPhoto.IsRemoveOrignalFile.HasValue && objUploadPhoto.IsRemoveOrignalFile.Value)
                    File.Move(OldFile, NewPhotoPath + "/" + ImageName);
                else
                    File.Copy(OldFile, NewPhotoPath + "/" + ImageName);

                //UploadFileStatic(NewPhotoPath + "/" + ImageName);

                //if (!IsGIFImage(objUploadPhoto.Extention) && IsImage(ImageName))
                //	CreateThumbnailImage(Folder, objUploadPhoto.ControlId, objUploadPhoto.Extention);
            }
        }

        return string.Empty;
    }

    //public static bool CreateThumbnailImage(eFolder Folder, string FolderPkId)
    //{
    //	return CreateThumbnailImage(Folder, FolderPkId, ".jpg", null);
    //}

    public static bool CreateThumbnailImage(eFolder Folder, string FolderPkId, string Extention, int? PhotoSizeId)
    {
        string PhotoPath = HttpContext.Current.Server.MapPath(CU.GetFolderPath(true, ePhotoSize.Original, Folder));
        string ImageName = Folder.ToString() + "_" + FolderPkId + Extention;
        string SourcePath = PhotoPath + "\\" + ImageName;

        if (!File.Exists(SourcePath))
            return false;

        System.Drawing.Image img = System.Drawing.Image.FromFile(SourcePath);

        try
        {
            List<PhotoSize> lstPhotoSize = GetelstPhotoSize();
            foreach (PhotoSize objPhotoSize in lstPhotoSize)
            {
                if (PhotoSizeId.HasValue && PhotoSizeId != 0 && PhotoSizeId != (int)objPhotoSize.ePhotoSize)
                    continue;

                string SaveFolder = HttpContext.Current.Server.MapPath(CU.GetFolderPath(true, objPhotoSize.ePhotoSize, Folder));
                if (!Directory.Exists(SaveFolder))
                    Directory.CreateDirectory(SaveFolder);

                string ThumbImageName = SaveFolder + "\\" + ImageName;

                int Height = 0, Width = 0;
                if (objPhotoSize.Width > 0 && objPhotoSize.Height > 0)
                {
                    Height = (int)objPhotoSize.Height;
                    Width = (int)objPhotoSize.Width;
                }
                else if (objPhotoSize.Percentage > 0)
                {
                    Height = (int)(img.Height * objPhotoSize.Percentage / 100);
                    Width = (int)(img.Width * objPhotoSize.Percentage / 100);
                }

                System.Drawing.Image newImg = resizeImage(img, Width, Height);
                //System.Drawing.Image newImg = img.GetThumbnailImage((int)objPhotoSize.Weight, (int)objPhotoSize.Height, null, new System.IntPtr());
                try
                {
                    if (File.Exists(ThumbImageName))
                        File.Delete(ThumbImageName);
                    newImg.Save(ThumbImageName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    //UploadFileStatic(ThumbImageName);
                }
                catch
                { }
                finally
                {
                    newImg.Dispose();
                }
            }
        }
        catch
        {
            return false;
        }
        finally
        {
            img.Dispose();
        }

        return true;
    }

    private static System.Drawing.Image resizeImage(System.Drawing.Image imgToResize, int Width, int Height)
    {
        int sourceWidth = imgToResize.Width;
        int sourceHeight = imgToResize.Height;

        float nPercent = 0;
        float nPercentW = 0;
        float nPercentH = 0;

        nPercentW = ((float)Width / (float)sourceWidth);
        nPercentH = ((float)Height / (float)sourceHeight);

        if (nPercentH < nPercentW)
            nPercent = nPercentH;
        else
            nPercent = nPercentW;

        //int destWidth = (int)(sourceWidth * nPercent);
        //int destHeight = (int)(sourceHeight * nPercent);

        int destWidth = Width;
        int destHeight = Height;

        System.Drawing.Bitmap b = new System.Drawing.Bitmap(destWidth, destHeight);

        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage((System.Drawing.Image)b);
        //b.imgQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

        g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
        g.Dispose();

        return (System.Drawing.Image)b;
    }

    public static List<PhotoSize> GetelstPhotoSize()
    {
        List<PhotoSize> lstPhotoSize = new List<PhotoSize>();

        foreach (ePhotoSize PhotoSize in Enum.GetValues(typeof(ePhotoSize)))
        {
            float Height = 0, Weight = 0, Percentage = 0;
            GetDimention(PhotoSize, ref Weight, ref Height, ref Percentage);
            if (Height != 0 || Weight != 0 || Percentage != 0)
            {
                lstPhotoSize.Add(new PhotoSize()
                {
                    ePhotoSize = PhotoSize,
                    Height = Height,
                    Width = Weight,
                    Percentage = Percentage,
                });
            }
        }

        return lstPhotoSize;
    }

    private static void GetDimention(ePhotoSize PSize, ref float Weight, ref float Height, ref float Percentage)
    {
        Weight = 0; Height = 0;
        try
        {
            string PhotoSize = PSize.ToString().Replace("W", "").Replace("H", "").Replace("P", "");

            if ((int)PSize <= 100)
            {
                var lstPhotoSize = PhotoSize.Split('x');
                Weight = float.Parse(lstPhotoSize[0]);
                Height = float.Parse(lstPhotoSize[1]);
            }
            else if ((int)PSize > 100)
            {
                Percentage = float.Parse(PhotoSize);
            }
        }
        catch { }
    }


    private static void MoveDeletedFile(string FilePath, eFolder Folder)
    {
        string FolderPath = HttpContext.Current.Server.MapPath(CU.GetFolderPath(true, ePhotoSize.Original, Folder)) + "\\Deleted";

        if (!Directory.Exists(FolderPath))
            Directory.CreateDirectory(FolderPath);

        string FileName = FilePath;
        int fileExtPos = FileName.LastIndexOf(".");

        string Extention = FileName.Substring(fileExtPos).ToLower();
        if (fileExtPos >= 0)
            FileName = FileName.Substring(0, fileExtPos);

        string NewFileName = FileName.Substring(FileName.LastIndexOf("\\")).Replace("\\", "").ToString() + "_" + GetDateTimeName() + Extention;

        try
        {
            File.Move(FilePath, FolderPath + "\\" + NewFileName);
        }
        catch { }

        try { new Thread(() => PermenantDeleteOldFile(FolderPath)).Start(); } catch { }


        //DeletedFileStatic(FilePath);
    }

    private static void PermenantDeleteOldFile(string FolderPath)
    {
        foreach (string strFile in Directory.GetFiles(FolderPath))
        {
            if (File.GetLastAccessTime(strFile).AddDays(7) < DateTime.Today)
                File.Delete(strFile);
        }
    }

    public static void DeleteImage(eFolder Folder, string FolderPkId)
    {
        DeleteImage(Folder, FolderPkId, ".jpg");
    }

    public static void DeleteImage(eFolder Folder, string FolderPkId, string Extention)
    {
        foreach (ePhotoSize PhotoSize in Enum.GetValues(typeof(ePhotoSize)))
        {
            string ImagePath = HttpContext.Current.Server.MapPath(CU.GetFolderPath(true, PhotoSize, Folder)) + "\\" + Folder + "_" + FolderPkId + Extention;
            if (File.Exists(ImagePath))
            {
                if (PhotoSize == ePhotoSize.Original)
                    CU.MoveDeletedFile(ImagePath, Folder);
                else
                {
                    File.Delete(ImagePath);
                    //DeletedFileStatic(ImagePath);
                }
            }
        }
    }


    public static void UploadFileStatic(string FilePath)
    {
        //if (FilePath.Contains(CU.UploadFilePath))
        //{
        //    try
        //    {
        //        string UploadPath = FilePath.Replace(CU.UploadFilePath, CU.UploadStaticFilePath);
        //        string FileDirectory = Path.GetDirectoryName(UploadPath);
        //        if (!Directory.Exists(FileDirectory))
        //            Directory.CreateDirectory(FileDirectory);

        //        if (File.Exists(UploadPath))
        //            File.Delete(UploadPath);

        //        File.Copy(FilePath, UploadPath);
        //    }
        //    catch { }
        //}
    }

    public static void DeletedFileStatic(string FilePath)
    {
        //if (FilePath.Contains(CU.UploadFilePath))
        //{
        //    try
        //    {
        //        FilePath = FilePath.Replace(CU.UploadFilePath, CU.UploadStaticFilePath);

        //        if (File.Exists(FilePath))
        //            File.Delete(FilePath);
        //    }
        //    catch { }
        //}
    }

    #endregion

    #region Is Used

    public static bool IsCountryUsed(int CountryId, ref string Message)
    {
        if (new Query() { CountryId = CountryId, eStatusNot = (int)eStatus.Delete }.Select(eSP.qry_State).Rows.Count > 0)
        {
            Message = "This Country is Contain State";
            return true;
        }

        return false;
    }

    public static bool IsStateUsed(int StateId, ref string Message)
    {
        return false;
    }

    public static bool IsCityUsed(int CityId, ref string Message)
    {
        //if (CU.GetDestinationDt(new Query() { StateId = StateId, eStatusNot = (int)eStatus.Delete }).Rows.Count > 0)
        //{
        //    Message = "This State is Contain Destination";
        //    return true;
        //}

        return false;
    } //????

    public static bool IsAreaUsed(int AreaId, ref string Message)
    {
        //if (CU.GetDestinationDt(new Query() { StateId = StateId, eStatusNot = (int)eStatus.Delete }).Rows.Count > 0)
        //{
        //    Message = "This State is Contain Destination";
        //    return true;
        //}

        return false;
    } //????

    public static bool IsDesignationUsed(int DesignationId, ref string Message) //????
    {

        //if (CU.GetDestinationDt(new Query() { StateId = StateId, eStatusNot = (int)eStatus.Delete }).Rows.Count > 0)
        //{
        //    Message = "This State is Contain Destination";
        //    return true;
        //}

        return false;
    }

    public static bool IsUserUsed(int UsersId, ref string Message) //????
    {
        return false;
    }

    public static bool IsProductUsed(int UsersId, ref string Message) //????
    {
        return false;
    }

    public static bool IsOrganizationUsed(int UsersId, ref string Message) //????
    {
        return false;
    }

    public static bool IsFirmUsed(int UsersId, ref string Message) //????
    {
        return false;
    }

    public static bool IsBankAccountUsed(int UsersId, ref string Message) //????
    {
        return false;
    }

    public static bool IsVariantUsed(int UsersId, ref string Message) //????
    {
        return false;
    }

    public static bool IsPriceListUsed(int UsersId, ref string Message) //????
    {
        return false;
    }

    public static bool IsCourierUsed(int UsersId, ref string Message) //????
    {
        return false;
    }

    public static bool IsOrderStatusUsed(int UsersId, ref string Message) //????
    {
        return false;
    }

    public static bool IsCustomerUsed(int UsersId, ref string Message) //????
    {
        return false;
    }

    public static bool IsVariantValueUsed(int VariantValueId, ref string Message)
    {
        if (new ItemVariant() { VariantValueId = VariantValueId }.SelectCount() > 0)
        {
            Message = "This Variant Value is Used in Product";
            return true;
        }

        return false;
    }


    #endregion

    #region Fill Dropdown

    public static void FillDropdown(ref DropDownList ddl, DataTable dt, string Caption, string DataValueField, string DataTextField)
    {
        bool IsSingleRow = dt.Rows.Count == 1;
        string SelectedValue = "";
        if (IsSingleRow)
            SelectedValue = dt.Rows[0][DataValueField].ToString();
        if (!Caption.zIsNullOrEmpty())
        {
            var dr = dt.NewRow();
            dr[DataValueField] = 0;
            dr[DataTextField] = Caption;
            dt.Rows.InsertAt(dr, 0);
        }

        ddl.DataSource = dt;
        ddl.DataValueField = DataValueField;
        ddl.DataTextField = DataTextField;
        ddl.DataBind();

        if (IsSingleRow)
            ddl.SelectedValue = SelectedValue;
    }

    #endregion

    #region Excel Export

    public static bool ExportToExcel(string FileName, string SavePath, DataTable objDataReader)
    {
        // Dim i As Integer
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        try
        {
            int intColumn = 0;
            int intColumnValue = 0;
            DataRow row = null;
            for (intColumn = 0; intColumn <= objDataReader.Columns.Count - 1; intColumn++)
            {
                sb.Append(objDataReader.Columns[intColumn].ColumnName);
                if (intColumnValue != objDataReader.Columns.Count - 1)
                {
                    sb.Append("\t");
                }
            }
            sb.Append("\n");
            foreach (DataRow row_loopVariable in objDataReader.Rows)
            {
                row = row_loopVariable;
                for (intColumnValue = 0; intColumnValue <= objDataReader.Columns.Count - 1; intColumnValue++)
                {
                    string a = (String.IsNullOrEmpty(row[intColumnValue].ToString()) ? "" : row[intColumnValue].ToString());
                    a = a.Replace('\r', ' ');
                    a = a.Replace('\n', ' ');
                    a = a.Replace('{', ' ');
                    a = a.Replace('}', ' ');
                    a = a.Replace('$', ' ');
                    a = a.Replace('<', ' ');
                    a = a.Replace('>', ' ');
                    a = a.Replace('?', ' ');
                    a = a.Replace('/', '-');
                    a = a.Replace('\\', '-');
                    a = a.Replace('*', ' ');
                    a = a.Replace('|', '-');
                    sb.AppendFormat(a);
                    if (intColumnValue != objDataReader.Columns.Count - 1)
                    {
                        sb.Append("\t");
                    }
                }
                sb.Append("\n");
            }
            ExportToExcel(SavePath + "\\" + FileName + ".xls", sb);
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            objDataReader = null;
            sb = null;
        }
    }

    public static bool ExportToExcel(string FullPath, DataTable dt)
    {
        // Dim i As Integer
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        try
        {
            for (int j = 0; j <= dt.Columns.Count - 1; j++)
            {
                sb.Append(dt.Columns[j].ColumnName);
                if (j != dt.Columns.Count - 1)
                {
                    sb.Append("\t");
                }
            }
            sb.Append("\n");
            foreach (DataRow dr in dt.Rows)
            {
                for (int j = 0; j <= dt.Columns.Count - 1; j++)
                {
                    string a = (String.IsNullOrEmpty(dr[j].ToString()) ? "" : dr[j].ToString());
                    a = a.Replace('\r', ' ');
                    a = a.Replace('\n', ' ');
                    a = a.Replace('{', ' ');
                    a = a.Replace('}', ' ');
                    a = a.Replace('$', ' ');
                    a = a.Replace('<', ' ');
                    a = a.Replace('>', ' ');
                    a = a.Replace('?', ' ');
                    //a = a.Replace('/', '-');
                    //a = a.Replace('\\', '-');
                    a = a.Replace('*', ' ');
                    a = a.Replace('|', '-');
                    sb.Append(a);
                    if (j != dt.Columns.Count - 1)
                    {
                        sb.Append("\t");
                    }
                }
                sb.Append("\n");
            }

            return ExportToExcel(FullPath, sb);
        }
        catch
        {
            return false;
        }
        finally
        {
            dt = null;
            sb = null;
        }
    }

    private static bool ExportToExcel(string fpath, System.Text.StringBuilder sb)
    {
        FileStream fsFile = new FileStream(fpath, FileMode.Create, FileAccess.Write);
        StreamWriter strWriter = new StreamWriter(fsFile);
        try
        {
            strWriter.BaseStream.Seek(0, SeekOrigin.End);
            strWriter.WriteLine(sb);
            strWriter.Close();
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            sb = null;
            strWriter = null;
            fsFile = null;
        }
    }


    public static void SetSuccessExcelMessage(int InsertCount, int UpdateCount, string TableName)
    {
        string SuccessMsg = string.Empty;
        if (InsertCount > 0)
            SuccessMsg = InsertCount + " " + TableName + " Added Successfully.";
        if (UpdateCount > 0)
            SuccessMsg += (SuccessMsg.zIsNullOrEmpty() ? string.Empty : "<br />") + UpdateCount + " " + TableName + " Updated Successfully.";

        CU.ZMessage(eMsgType.Success, string.Empty, SuccessMsg);
    }

    public static void SetErrorExcelMessage(string Message, int SuccessCount, int FailCount)
    {
        Message += "<br />" + SuccessCount.ToString() + ", Record(s) are Correct. " + FailCount.ToString() + " Record(s) are Wrong.";
        Message += "<br /> Correct The Wrong Information and Try Again.";
        CU.ZMessage(eMsgType.Error, string.Empty, Message, 0);

    }


    public static bool GetDataTableFromCsv(string strFilePath, ref DataTable dt, ref string Error)
    {
        Error = string.Empty;
        try
        {
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(new char[] { '|', ',' });
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }

                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(new char[] { '|', ',' });
                    DataRow dr = dt.NewRow();
                    bool IsValid = true;
                    for (int i = 0; i < headers.Length; i++)
                    {
                        try
                        {
                            dr[i] = rows[i];
                        }
                        catch (Exception e)
                        {
                            string str = e.Message;
                            IsValid = false;
                            break;
                        }
                    }

                    if (IsValid)
                        dt.Rows.Add(dr);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            return false;
        }

    }

    public static bool IsValidExcelFile(FileUpload fu, ref DataTable dt, int? TotalValidColumn, string FileName)
    {
        if (!fu.HasFile)
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Excel File to upload.");
            return false;
        }

        //if (!fu.FileName.ToLower().EndsWith(".xls", StringComparison.CurrentCultureIgnoreCase))
        //{
        //    CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Valid .xls File to upload.");
        //    return false;
        //}

        string ext = System.IO.Path.GetExtension(fu.FileName);
        if (!CU.IsValidExcel(ext))
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Select Only Excel File.");
            return false;
        }

        string UploadFilePath = HttpContext.Current.Server.MapPath(CU.GettempDownloadPath()) + "/" + FileName + "_" + CU.GetDateTimeName() + ext;
        fu.SaveAs(UploadFilePath);

        string ExcelError = string.Empty;
        if (!CU.OpenExcelFileFirstExcelSheet(UploadFilePath, ref dt, ref ExcelError))
        {
            CU.ZMessage(eMsgType.Error, string.Empty, ExcelError);
            return false;
        }

        if (dt.Rows.Count == 0)
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "No Data Found.");
            return false;
        }
        //if (!dt.Columns.Contains("Pincode") || !dt.Columns.Contains("City") || !dt.Columns.Contains("State") ||
        //	!dt.Columns.Contains("PRAPAID") || !dt.Columns.Contains("COD") || !dt.Columns.Contains("REVERSEPICKUP") || !dt.Columns.Contains("Pickup"))
        if (TotalValidColumn.HasValue && dt.Columns.Count != TotalValidColumn)
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Selected Excel File Is Not Valid Excel File.<br>Selected File Has Old Format, Contact Service Provider For New File Format.");
            return false;
        }

        return true;
    }

    public static bool IsRepeateExcelRow(DataTable dt, int CurentRow, string ColumnValue, int ColumnIndex, string ColumnValue2, int? ColumnIndex2, string ColumnValue3, int? ColumnIndex3, ref string RepeateItem)
    {
        string Query = ("[" + dt.Columns[ColumnIndex].ColumnName + "]") + " = '" + ColumnValue.Replace("'", "''") + "'";
        Query += (ColumnIndex2.HasValue && !ColumnValue2.zIsNullOrEmpty()) ? (" AND [" + dt.Columns[ColumnIndex2.Value].ColumnName + "] = '" + ColumnValue2.Replace("'", "''") + "'") : string.Empty;
        Query += (ColumnIndex3.HasValue && !ColumnValue3.zIsNullOrEmpty()) ? (" AND [" + dt.Columns[ColumnIndex3.Value].ColumnName + "] = '" + ColumnValue3.Replace("'", "''") + "'") : string.Empty;

        var dr = dt.Select(Query, "");
        if (dr.Length >= 2)
        {
            RepeateItem = string.Empty;
            foreach (DataRow Item in dr)
                RepeateItem += (RepeateItem.zIsNullOrEmpty() ? string.Empty : " ,") + (dt.Rows.IndexOf(Item) + 2).ToString();

            RepeateItem += ".<br />";
            return true;
        }

        return false;
    }

    public static bool IsValidCSVFile(FileUpload fu, ref DataTable dt, int TotalValidColumn, string FileName)
    {
        if (!fu.HasFile)
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select CSV File to upload.");
            return false;
        }

        if (!fu.FileName.ToLower().EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Please Select Valid .csv File to upload.");
            return false;
        }

        string UploadFilePath = HttpContext.Current.Server.MapPath(CU.GettempDownloadPath()) + "/" + FileName + "_" + CU.GetDateTimeName() + ".csv";
        fu.SaveAs(UploadFilePath);

        string Error = string.Empty;
        if (!CU.GetDataTableFromCsv(UploadFilePath, ref dt, ref Error))
        {
            CU.ZMessage(eMsgType.Error, string.Empty, Error);
            return false;
        }

        if (TotalValidColumn != 0 && dt.Columns.Count != TotalValidColumn)
        {
            CU.ZMessage(eMsgType.Error, string.Empty, "Selected Excel File Is Not Valid Excel File.<br>Selected File Has Old Format, Contact Service Provider For New File Format.");
            return false;
        }


        return true;
    }

    #endregion

    #region ExcelImport

    public static bool OpenTSV(string strFilePath, ref DataTable dt, ref string Error)
    { return OpenOctfisExcel(strFilePath, "\t", ref dt, ref Error); }

    public static bool OpenPSV(string strFilePath, ref DataTable dt, ref string Error)
    { return OpenOctfisExcel(strFilePath, "|", ref dt, ref Error); }

    public static bool OpenCSV(string strFilePath, ref DataTable dt, ref string Error)
    { return OpenOctfisExcel(strFilePath, ",", ref dt, ref Error); }

    public static bool OpenOctfisExcel(string strFilePath, string Saparator, ref DataTable dt, ref string Error)
    {
        Error = string.Empty;
        try
        {
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split('\t');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }

                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split('\t');
                    //if (rows.Length == headers.Length)
                    {
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            try
                            {
                                dr[i] = rows[i];
                            }
                            catch { }
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            RemoveEmptyRowColumn(ref dt);
            return true;
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            return false;
        }

    }


    public static bool OpenExcelFile(string strFilePath, string[] SheetName, ref DataTable dt, ref string error)
    {
        OleDbConnection olecon = new OleDbConnection(GetConnectionString(strFilePath, true));

        try { olecon.Open(); }
        catch (Exception ex)
        {
            if (Path.GetExtension(strFilePath) == ".xlsx")
            {
                error = "Selected File is not Valid EXCEL File. \nSave It as Excel 2003 Format(.xls)\nError : " + ex.Message;
                return false;
            }

            return OpenTSV(strFilePath, ref dt, ref error);
        }

        OleDbCommand cmd = olecon.CreateCommand();
        OleDbDataReader dr;
        for (int i = 0; i < SheetName.Length; i++)
        {
            try
            {
                cmd.CommandText = "SELECT * FROM [" + SheetName[i] + "$]";
                dr = cmd.ExecuteReader();

                dt.Load(dr);
                dr.Close();
                olecon.Close();
                olecon.Dispose();

                return true;
            }
            catch { }
        }

        if (olecon.State == ConnectionState.Open)
            olecon.Close();
        olecon.Dispose();

        string msg = "";
        msg += "Invalid Excel File. (Check Sheet name as ";
        for (int i = 0; i < SheetName.Length; i++)
            msg += "'" + SheetName[i] + "' or ";
        msg += ")";
        error = msg;

        return false;
    }

    private static string GetConnectionString(string strFilePath, bool IsStringFormat)
    {
        //if (Path.GetExtension(strFilePath) == ".xlsx")
        return @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + strFilePath + ";Extended Properties = 'Excel 12.0;HDR=YES;IMEX=1;'; ";
        //else
        //    return @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source = " + strFilePath + ";Extended Properties =\"Excel 8.0;HDR=Yes;IMEX=2\"";


        //string strConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFilePath + ";Extended Properties='Excel 8.0;";

        //if (strFilePath.ToLower().EndsWith(".xlsx"))
        //    strConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + strFilePath + ";Extended Properties='Excel 8.0;";

        //if (IsStringFormat)
        //    strConnectionString += "HDR=Yes;IMEX=1';";
        //else
        //    strConnectionString += "'";


        //string strConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + strFilePath + "';Extended Properties='Excel 8.0';";
        //string strConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + strFilePath + "';Extended Properties='Excel 8.0';IMEX=1;";
        //string strConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + strFilePath + "';Extended Properties='Excel 8.0';HDR=Yes;IMEX=1";
        //string strConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + strFilePath + "';Extended Properties='Excel 8.0;HDR=Yes;IMEX=1'";

        //return strConnectionString;
    }

    public static bool OpenExcelFileFirstExcelSheet(string strFilePath, ref DataTable dt, ref string error)
    {
        OleDbConnection olecon = new OleDbConnection(GetConnectionString(strFilePath, true));

        try { olecon.Open(); }
        catch (Exception ex)
        {
            if (Path.GetExtension(strFilePath) == ".xlsx")
            {
                error = "Selected File is not Valid EXCEL File. \nSave It as Excel 2003 Format(.xls)\nError : " + ex.Message;
                return false;
            }
            else if (Path.GetExtension(strFilePath) == ".csv")
                return OpenCSV(strFilePath, ref dt, ref error);
            else
                return OpenTSV(strFilePath, ref dt, ref error);
        }

        OleDbCommand cmd = olecon.CreateCommand();
        OleDbDataReader dr;

        try
        {
            cmd.CommandText = "SELECT * FROM [" + GetFirstExcelSheetName(olecon)[0] + "$]"; ;
            dr = cmd.ExecuteReader();

            dt.Load(dr);
            dr.Close();
            olecon.Close();
            olecon.Dispose();

            //dt = dt.AsEnumerable().Where(r => r.ItemArray.All(v => !string.IsNullOrEmpty(v.ToString()))).CopyToDataTable();
            RemoveEmptyRowColumn(ref dt);

            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
        }

        if (olecon.State == ConnectionState.Open)
            olecon.Close();
        olecon.Dispose();

        return false;
    }

    private static void RemoveEmptyRowColumn(ref DataTable dt)
    {
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            bool IsBlankRow = true;
            foreach (DataColumn dc in dt.Columns)
            {
                if (!string.IsNullOrEmpty(dt.Rows[i][dc].ToString()))
                {
                    IsBlankRow = false;
                    break;
                }
            }
            if (IsBlankRow)
            {
                dt.Rows.RemoveAt(i);
                i--;
            }
        }
    }

    public static string[] GetFirstExcelSheetName(OleDbConnection connToExcel)
    {
        try
        {
            DataTable dtSheetName = connToExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            List<String> lstExcelSheet = new List<string>(dtSheetName.Rows.Count);

            foreach (DataRow row in dtSheetName.Rows)
                if (row["TABLE_NAME"].ToString().EndsWith("$"))//checks whether row contains '_xlnm#_FilterDatabase' or sheet name(i.e. sheet name always ends with $ sign)
                {
                    lstExcelSheet.Add(row["TABLE_NAME"].ToString().Replace("$", "").Replace("'", ""));
                }

            return lstExcelSheet.ToArray();
        }
        catch
        {
            //MessageBox.Show("Selected File is not Valid EXCEL File. \nSave It as Excel 2003 Format(.xls) \nError : " + ex.Message, "Corrupted File", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        return new String[0];
    }

    public static bool IsValidExcel(string ext)
    {
        return ext.ToLower() == ".xls" || ext.ToLower() == ".xlsx"
            || ext.ToLower() == ".tsv" || ext.ToLower() == ".csv"
            || ext.ToLower() == ".txt";
    }

    #endregion

    #region Authority

    public static Authority GetAuthority(int UsersId, eAuthority Authority)
    {
        eDesignation Designation = GeteDesignationId(UsersId);
        if (Designation == eDesignation.SystemAdmin)
        {
            return new Authority()
            {
                IsView = true,
                IsAddEdit = true,
                IsDelete = true,
            };
        }
        else
        {
            var dtClientAuthority = new UserAuthority() { UsersId = UsersId, eAuthority = (int)Authority }.Select(new UserAuthority() { IsAllowView = false, IsAllowAddEdit = false, IsAllowDelete = false });
            if (dtClientAuthority.Rows.Count > 0)
            {
                return new Authority()
                {
                    IsView = Convert.ToBoolean(dtClientAuthority.Rows[0][CS.IsAllowView]),
                    IsAddEdit = Convert.ToBoolean(dtClientAuthority.Rows[0][CS.IsAllowAddEdit]),
                    IsDelete = Convert.ToBoolean(dtClientAuthority.Rows[0][CS.IsAllowDelete])
                };
            }
            else
            {
                return new Authority()
                {
                    IsView = false,
                    IsAddEdit = false,
                    IsDelete = false,
                };
            }
        }
    }

    public static DataTable GetAuthorityData(int UsersId)
    {
        eDesignation Designation = GeteDesignationId(UsersId);
        var dtUserAuthority = new DataTable();

        if (Designation != eDesignation.SystemAdmin)
        {
            dtUserAuthority = new UserAuthority()
            {
                UsersId = UsersId,
                IsAllowView = true,
            }.Select();
        }

        DataTable dtAuthority = new DataTable();

        dtAuthority.Columns.Add(CS.AuthorityId, typeof(int));
        dtAuthority.Columns.Add(CS.AuthorityName, typeof(string));
        dtAuthority.Columns.Add(CS.IsAllowView, typeof(bool));
        dtAuthority.Columns.Add(CS.IsAllowAddEdit, typeof(bool));
        dtAuthority.Columns.Add(CS.IsAllowDelete, typeof(bool));

        DataTable dt = dtAuthority.Clone();

        #region Configuration

        dt = dtAuthority.Clone();

        dt.Rows.Add(0, "Configuration");
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManageCountry);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManageState);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManageCity);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManageArea);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManageService);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManageBankAccount);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManageVariant);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManagePriceList);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManageCourier);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManageOrderSource);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManageOrderStatus);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManageVendor);

        if (dt.Rows.Count > 1)
            dtAuthority.Merge(dt);

        #endregion

        #region System

        dt = dtAuthority.Clone();

        dt.Rows.Add(0, "System");
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.Designation);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.Organization);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.Firm);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.User);

        if (dt.Rows.Count > 1)
            dtAuthority.Merge(dt);

        #endregion

        #region Product

        dt = dtAuthority.Clone();

        dt.Rows.Add(0, "Product");
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManagePortal);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManageProduct);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.Adjustment);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.HappyCustomer);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ProductVendorView);

        if (dt.Rows.Count > 1)
            dtAuthority.Merge(dt);

        #endregion

        #region Order

        dt = dtAuthority.Clone();

        dt.Rows.Add(0, "Order");
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManageOrder);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.OrderChildView);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManageOrderPayment);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.OrderTracking);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.OrderSlipPrint);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.OrderProductPrint);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.OrderComplain);

        if (dt.Rows.Count > 1)
            dtAuthority.Merge(dt);

        #endregion

        #region Call Recording

        dt = dtAuthority.Clone();

        dt.Rows.Add(0, "Call Recording");
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManageCallType);
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManageCallHistory);

        if (dt.Rows.Count > 1)
            dtAuthority.Merge(dt);

        #endregion

        #region Other

        dt = dtAuthority.Clone();

        dt.Rows.Add(0, "Other");
        SetApplyAuthority(dtUserAuthority, Designation, ref dt, eAuthority.ManageWallet);

        if (dt.Rows.Count > 1)
            dtAuthority.Merge(dt);

        #endregion

        return dtAuthority;
    }

    private static void SetApplyAuthority(DataTable dtUserAuthority, eDesignation Designation, ref DataTable dtAuthority, eAuthority Authority)
    {
        bool IsAllowView = true, IsAllowAddEdit = true, IsAllowDelete = true;
        if (Designation != eDesignation.SystemAdmin)
        {
            var drClientAuthority = dtUserAuthority.Select(CS.eAuthority + " = " + (int)Authority);
            IsAllowView = (drClientAuthority.Length > 0 && Convert.ToBoolean(drClientAuthority[0][CS.IsAllowView]));
            IsAllowAddEdit = (drClientAuthority.Length > 0 && Convert.ToBoolean(drClientAuthority[0][CS.IsAllowAddEdit]));
            IsAllowDelete = (drClientAuthority.Length > 0 && Convert.ToBoolean(drClientAuthority[0][CS.IsAllowDelete]));
        }

        if (IsAllowView)
            dtAuthority.Rows.Add((int)Authority, GetDescription(Authority), IsAllowView, IsAllowAddEdit, IsAllowDelete);
    }

    public static void SetDefaultAuthority(int UsersId, int DesignationId)
    {
        var dtDesignationAuthority = new DesignationAuthority()
        {
            DesignationId = DesignationId,
        }.Select(new DesignationAuthority() { eAuthority = 0, IsAllowView = false, IsAllowAddEdit = false, IsAllowDelete = false });

        var dtClientAuthority = new UserAuthority()
        {
            UsersId = UsersId,
        }.Select(new UserAuthority() { UserAuthorityId = 0, eAuthority = 0 });

        foreach (DataRow drDesignationAuthority in dtDesignationAuthority.Rows)
        {
            var objUserAuthority = new UserAuthority()
            {
                UsersId = UsersId,
                eAuthority = drDesignationAuthority[CS.eAuthority].zToInt(),
                IsAllowView = Convert.ToBoolean(drDesignationAuthority[CS.IsAllowView]),
                IsAllowAddEdit = Convert.ToBoolean(drDesignationAuthority[CS.IsAllowAddEdit]),
                IsAllowDelete = Convert.ToBoolean(drDesignationAuthority[CS.IsAllowDelete]),
            };

            var drUserAuthority = dtClientAuthority.Select(CS.eAuthority + " = " + objUserAuthority.eAuthority);
            if (drUserAuthority.Length > 0)
            {
                objUserAuthority.UserAuthorityId = drUserAuthority[0][CS.UserAuthorityId].zToInt();
                objUserAuthority.Update();
            }
            else
            {
                objUserAuthority.Insert();
            }
        }
    }

    public static List<int> GetlstAuthoDesignation()
    {
        var lsteDesignation = new List<int>();
        lsteDesignation.Add((int)eDesignation.Admin);
        lsteDesignation.Add((int)eDesignation.User);

        return lsteDesignation;
    }

    #endregion

    #region Paging

    public static void LoadDisplayPerPageSmall(ref DropDownList ddlRecordPerPage)
    {
        var dtRecordPerPage = new DataTable();
        dtRecordPerPage.Columns.Add("RecordPerPage", typeof(int));
        dtRecordPerPage.Columns.Add("RecordPerPageName", typeof(string));

        //dtRecordPerPage.Rows.Add("3", "3");
        //dtRecordPerPage.Rows.Add("10", "10");
        dtRecordPerPage.Rows.Add("20", "20");
        dtRecordPerPage.Rows.Add("25", "25");
        dtRecordPerPage.Rows.Add("30", "30");
        dtRecordPerPage.Rows.Add("0", "All");

        ddlRecordPerPage.DataSource = dtRecordPerPage;
        ddlRecordPerPage.DataValueField = "RecordPerPage";
        ddlRecordPerPage.DataTextField = "RecordPerPageName";
        ddlRecordPerPage.DataBind();
    }

    public static void LoadDisplayPerPage(ref DropDownList ddlRecordPerPage)
    {
        var dtRecordPerPage = new DataTable();
        dtRecordPerPage.Columns.Add("RecordPerPage", typeof(int));
        dtRecordPerPage.Columns.Add("RecordPerPageName", typeof(string));

        dtRecordPerPage.Rows.Add("30", "30");
        dtRecordPerPage.Rows.Add("50", "50");
        dtRecordPerPage.Rows.Add("100", "100");
        dtRecordPerPage.Rows.Add("150", "150");
        dtRecordPerPage.Rows.Add("200", "200");
        dtRecordPerPage.Rows.Add("0", "All");

        ddlRecordPerPage.DataSource = dtRecordPerPage;
        ddlRecordPerPage.DataValueField = "RecordPerPage";
        ddlRecordPerPage.DataTextField = "RecordPerPageName";
        ddlRecordPerPage.DataBind();

        if (HttpContext.Current.Request.Cookies[CS.RecordPerPage] == null)
            HttpContext.Current.Response.Cookies[CS.RecordPerPage].Value = "50";

        try { ddlRecordPerPage.SelectedValue = HttpContext.Current.Request.Cookies[CS.RecordPerPage].Value.ToString(); }
        catch { }
    }

    public static void GetPageIndex(ePageIndex ePageIndex, int RecordPerPage, ref int PageIndex, ref Query objQuery, ref TextBox txtCurrent, ref Label lblTotalRecord)
    {
        if (RecordPerPage != 0 && ePageIndex != ePageIndex.AllPage)
        {
            try
            {
                int LastPage = (int)(Math.Ceiling(lblTotalRecord.zToInt().Value / (RecordPerPage * 1.0)));
                if (LastPage == 0)
                    LastPage = 1;

                if (ePageIndex == ePageIndex.Last)
                    PageIndex = LastPage;
                else if (ePageIndex == ePageIndex.Next)
                {
                    if (PageIndex < LastPage)
                        ++PageIndex;
                }
                else if (ePageIndex == ePageIndex.Prev && PageIndex > 1)
                    --PageIndex;
                else if (ePageIndex == ePageIndex.Custom && txtCurrent.zIsNumber())
                {
                    if (txtCurrent.zToInt().Value <= LastPage && txtCurrent.zToInt().Value >= 1)
                        PageIndex = txtCurrent.zToInt().Value;
                }
                else
                    PageIndex = 1;
            }
            catch { };
            objQuery.FromRow = ((PageIndex - 1) * RecordPerPage) + 1;
            objQuery.ToRow = PageIndex * RecordPerPage;
        }
        else
        {
            PageIndex = 1;
        }
    }

    public static string PageRecordString(int TotalRecord, int TotalDisplay, LinkButton lnkFirst, LinkButton lnkPrev, TextBox txtGotoPageNo, LinkButton lnkNext, LinkButton lnkLast)
    {
        int TotalPage = (int)(TotalDisplay == 0 ? 1 : (Math.Ceiling((float)TotalRecord / (float)TotalDisplay)));
        lnkFirst.Enabled = lnkPrev.Enabled = txtGotoPageNo.Enabled = lnkNext.Enabled = lnkLast.Enabled = (TotalPage > 1);

        if (TotalRecord > 0)
            return "<b>" + TotalRecord + "</b>" + " Records in <b>" + TotalPage + "</b> Pages";
        else
            return "No Record Found";
    }

    #endregion

    #region Send SMS

    public static int SendSMS(int UsersId, string SMSText, string MobileNo, bool IsUnicode, int ComponentId, int ComponentType)
    {
        string SMSUserName = GetNameValue(eNameValue.SMSUserName);
        string SMSPassword = GetNameValue(eNameValue.SMSPassword);
        string SMSSenderName = GetNameValue(eNameValue.SMSSenderId);

        SMSText = SMSText.Replace("&", "%26");
        SMSText = SMSText.Replace("#", "%23");
        SMSText = SMSText.Replace("+", "%2B");

        string SMSDomainName = "admin.octfis.com";
        string Responce = APICall("http://" + SMSDomainName + "/SMSSend.aspx?UserName=" + SMSUserName + "&Password=" + SMSPassword + "&MobileNo=" + MobileNo + "&SenderId=" + SMSSenderName + "&Message=" + SMSText + "&Unicode=" + (IsUnicode ? "yes" : "no"));

        string MessageId = string.Empty;
        var lstMessageId = Responce.Split(':');
        if (lstMessageId.Length > 1 && !lstMessageId[1].zIsNullOrEmpty())
            MessageId = lstMessageId[1].Trim();

        var objSMSLog = new SMSLog()
        {
            UsersId = UsersId,
            ComponentId = ComponentId,
            eComponentType = ComponentType,
            MessageId = MessageId,
            MobileNo = MobileNo,
            SMSSendDate = IndianDateTime.Now,
            SMSText = SMSText,
            eSMSStatus = Responce.Contains("OPERATION SUCCESSFUL") ? (int)eSMSStatus.Sent : (int)eSMSStatus.Fail,
            Responce = Responce,
        };

        objSMSLog.Insert();

        return objSMSLog.eSMSStatus.Value;
    }

    #endregion

    #region Send Mail

    public static bool SendMail(List<string> lstToEmailId, List<string> lstCCEmailId, string Subject, string MailText, bool IsBodyHtml, string Attachment, ref string ErrorMsg)
    {
        string SendFromEmailId = string.Empty, SendFromEmailPassword = string.Empty;

        SendFromEmailId = GetNameValue(eNameValue.EmailId).Trim().ToLower();
        SendFromEmailPassword = GetNameValue(eNameValue.EmailPassword);

        string Host = "smtp.gmail.com";
        //int Port = 587;

        //string Host = "smtp.zoho.com";
        int Port = 587;
        bool UseSSL = true;

        if (string.IsNullOrEmpty(SendFromEmailId) || string.IsNullOrEmpty(SendFromEmailPassword))
            return false;

        var dtOrganization = new Organization() { OrganizationId = CU.GetOrganizationId() }.Select(new Organization() { OrganizationName = string.Empty });

        string HeaderName = dtOrganization.Rows.Count > 0 ? dtOrganization.Rows[0][CS.OrganizationName].ToString() : SendFromEmailId.Trim().ToLower();
        MailAddress FromAddress = new MailAddress(SendFromEmailId, HeaderName);

        var smtp = new SmtpClient
        {
            Host = Host,
            Port = Port,
            EnableSsl = UseSSL,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Timeout = 99999999,
            Credentials = new NetworkCredential(SendFromEmailId, SendFromEmailPassword),
        };

        smtp.ServicePoint.MaxIdleTime = 1;
        var message = new MailMessage()
        {
            From = FromAddress,
            Subject = Subject,
            Body = MailText,
            IsBodyHtml = IsBodyHtml,
        };

        //if (!FromEmailId.Trim().ToLower().zIsNullOrEmpty())
        //	message.ReplyToList.Add(new MailAddress(FromEmailId.Trim().ToLower()));

        if (!IsDevelopeMode.Value)
        {
            message.Bcc.Add(new MailAddress("jatinlathiya0@gmail.com"));
        }

        foreach (string ToEmailId in lstToEmailId)
            message.To.Add(new MailAddress(ToEmailId.Trim().ToLower()));

        foreach (string CCEmailId in lstCCEmailId)
        {
            if (!CCEmailId.Trim().zIsNullOrEmpty())
                message.CC.Add(new MailAddress(CCEmailId.Trim().ToLower()));
        }

        if (!string.IsNullOrEmpty(Attachment))
            message.Attachments.Add(new Attachment(Attachment));

        //https://myaccount.google.com/intro/security For Google authority
        try { smtp.Send(message); }
        catch (SmtpException e)
        {
            ErrorMsg = "Mail Can Not Be Sent Due To Illigle Attachment\r\n" + e.Message;
            return false;
        }
        catch (Exception e)
        {
            ErrorMsg = "Following Error Occured :\r\n" + e.Message;
            return false;
        }

        ErrorMsg = string.Empty;
        return true;
    }

    public static bool SendMail(string EmailId, string CCEmailId, string MailText, bool IsBodyHtml, string Subject, ref string ErrorMsg)
    {
        List<string> lstToEmailId = new List<string>();
        lstToEmailId.Add(EmailId);

        List<string> lstCCEmailId = new List<string>();
        lstCCEmailId.Add(CCEmailId);

        return SendMail(lstToEmailId, lstCCEmailId, Subject, MailText, IsBodyHtml, string.Empty, ref ErrorMsg);
    }

    #endregion

    #region Set Search Value

    public static void SetSearchCookie(string CookieName, string CookieValue)
    {
        HttpCookie mycookie = new HttpCookie("SearchInfo" + CookieName);
        mycookie.Value = CookieValue;
        mycookie.Expires = DateTime.Now.AddDays(30);
        HttpContext.Current.Response.Cookies.Add(mycookie);
    }

    public static string GetSearchCookie(string CookieName)
    {
        try { return HttpContext.Current.Request.Cookies["SearchInfo" + CookieName].Value; }
        catch { return string.Empty; }
    }

    #endregion

    #region Notification

    public static void CrateNotification(int UsersId, int ComponentId, eComponentType ComponentType, eNotificationType NotificationType)
    {
        string NotificationText = string.Empty;
        GetDefaultNotification(NotificationType, ref NotificationText);

        CrateNotification(UsersId, ComponentId, ComponentType, NotificationType, NotificationText);
    }

    public static void CrateNotification(int UsersId, int ComponentId, eComponentType ComponentType, eNotificationType NotificationType, string NotificationText)
    {
        SetNotificationText(UsersId, ComponentId, ComponentType, NotificationType, ref NotificationText);

        var lstUsersId = new List<int>();
        lstUsersId.Add(UsersId);

        if (!NotificationText.zIsNullOrEmpty())
        {
            #region Send SMS

            bool IsSendSMS = false;

            if (UsersId > 0 && IsSendSMS)
            {
                string MobileNo = new Users() { UsersId = UsersId }.Select(new Users() { MobileNo = string.Empty }).Rows[0][CS.MobileNo].ToString();
                SendSMS(UsersId, NotificationText, MobileNo, false, ComponentId, (int)ComponentType);
            }

            #endregion

            foreach (int id in lstUsersId)
            {
                if (id != 0)
                {
                    new Notification()
                    {
                        UsersId = id,
                        ComponentId = ComponentId,
                        eComponentType = (int)ComponentType,
                        eNotificationType = (int)NotificationType,
                        InsertTime = IndianDateTime.Now,
                        eReadStatus = (int)eMessageStatus.WithHold,
                        eViewStatus = (int)eViewStatus.UnSeen,
                        NotificationText = NotificationText,
                    }.Insert();
                }
            }
        }
    }


    public static void GetDefaultNotification(eNotificationType NotificationType, ref string NotificationText)
    {
        NotificationText = string.Empty;

        switch (NotificationType)
        {
            //case eNotificationType.Transaction:
            //    NotificationText = "#PaymentDeadlineDate# is Payment Deadline for Package #PackageName# (#ContactName#).";
            //    break;
        }
    }

    public static void SetNotificationText(int UsersId, int ComponentId, eComponentType ComponentType, eNotificationType NotificationType, ref string NotificationText)
    {
        if (NotificationType == eNotificationType.Transaction)
        {
            var drWallet = new Query() { WalletId = ComponentId }.Select(eSP.qry_Wallet).Select();
            var objWallet = new Wallet() { }.SelectList<Wallet>(drWallet)[0];
            if (objWallet.eTransactionType == (int)eTransactionType.Manually)
            {
                NotificationText = "Your Wallet has a " + (objWallet.eDirection.IsCredit() ? "Credit" : "Debit") + " by " + drWallet[0][CS.EntryUsersName].ToString() + " of Rs " + (objWallet.Amount.Value.AmountFormat()) + " on " + objWallet.TransactionTime.Value.ToString(CS.ddMMyyyyHHmm) + " Avl Bal Rs " + GeFormatedBalance(objWallet.UsersId.Value);
            }
        }
    }


    public static string GetNotificationImage(eNotificationType NotificationType)
    {
        string Image = "";
        switch (NotificationType)
        {
            case eNotificationType.Transaction:
                Image = "SystemImages/NotificationIcon/Transaction.png";
                break;
        }

        return CU.StaticFilePath + Image;
    }

    #endregion

    #region Wallet

    public static bool IsCredit(this int? Direction)
    {
        return (Direction == (int)eTransactionDirection.Credit);
    }

    public static bool IsDebit(this int? Direction)
    {
        return (Direction == (int)eTransactionDirection.Debit);
    }


    public static void SetTransaction(int? WalletId, int UsersId, decimal Amount, eTransactionDirection Direction, eTransactionType TransactionType, string Narration, DateTime? TransactionTime, int? ComponentId, int? ComponentType)
    {
        var objWallet = new Wallet()
        {
            WalletId = WalletId,
            UsersId = UsersId,
            TransactionTime = TransactionTime.HasValue ? TransactionTime : IndianDateTime.Now,
            eDirection = (int)Direction,
            Amount = Amount,
            Narration = Narration,
        };

        if (WalletId.HasValue && WalletId > 0)
        {
            var objOldWallent = new Wallet() { WalletId = objWallet.WalletId }.SelectList<Wallet>()[0];

            objWallet.Update();

            CU.CountCurrentBalancee(objOldWallent);
        }
        else
        {
            objWallet.EntryUsersId = CU.GetUsersId();
            objWallet.ComponentId = ComponentId.HasValue ? ComponentId : 0;
            objWallet.eComponentType = ComponentType.HasValue ? ComponentType : 0;
            objWallet.eTransactionType = (int)TransactionType;
            objWallet.EntryTime = IndianDateTime.Now;
            objWallet.eStatus = (int)eStatus.Active;

            objWallet.WalletId = objWallet.Insert();

            CU.CountCurrentBalancee(objWallet);

            decimal CurrentBalance = CU.GetBalance(objWallet.UsersId.Value);
            SetMasterBalance(objWallet.UsersId.Value, CurrentBalance);

            CrateNotification(objWallet.UsersId.Value, objWallet.WalletId.Value, eComponentType.Wallet, eNotificationType.Transaction);
        }
    }

    public static void SetMasterBalance(int UsersId, decimal CurrentBalance)
    {
        var pageHandler = HttpContext.Current.CurrentHandler;
        if (pageHandler is System.Web.UI.Page)
        {
            if (CU.GetUsersId() == UsersId)
            {
                try
                {
                    var lblMstClientBlanace = ((System.Web.UI.Page)pageHandler).Master.FindControl("lblMstClientBlanace") as Label;
                    lblMstClientBlanace.Text = CurrentBalance.AmountFormat();
                }
                catch { }
            }
        }
    }

    public static void CountCurrentBalancee(Wallet objOldWallet)
    {
        DateTime? FromDate = null;
        var objWallet = new Wallet() { WalletId = objOldWallet.WalletId }.SelectList<Wallet>()[0];

        #region Count Amount

        //decimal Amount = 0;

        //if (objWallet.eStatus != objOldWallet.eStatus)
        //{
        //	#region Change Transaction Status

        //	Amount = objWallet.Amount.Value;
        //	if (objWallet.eStatus == (int)eStatus.Active)
        //	{
        //		if (objWallet.eDirection.IsDebit())
        //			Amount = Amount * -1;
        //	}
        //	else
        //	{
        //		if (objWallet.eDirection.IsCredit())
        //			Amount = Amount * -1;
        //	}

        //	#endregion
        //}
        //else if (objWallet.eDirection != objOldWallet.eDirection)
        //{
        //	#region Change Direction

        //	Amount = objOldWallet.Amount.Value + objWallet.Amount.Value;
        //	if (objWallet.eDirection.IsDebit())
        //		Amount = Amount * -1;

        //	#endregion
        //}
        //else if (objWallet.Amount != objOldWallet.Amount)
        //{
        //	#region Change Amount

        //	if (objWallet.eDirection.IsDebit())
        //		Amount = objOldWallet.Amount.Value - objWallet.Amount.Value;
        //	else
        //		Amount = objWallet.Amount.Value - objOldWallet.Amount.Value;

        //	#endregion
        //}

        #endregion

        #region Get From Date

        if (objWallet.TransactionTime == objOldWallet.TransactionTime
            || objWallet.TransactionTime < objOldWallet.TransactionTime)
            FromDate = objWallet.TransactionTime;
        else
            FromDate = objOldWallet.TransactionTime;

        #endregion

        if (FromDate.HasValue)
        {
            var dtWallet = new Query()
            {
                UsersId = objWallet.UsersId,
                FromDate = FromDate,
            }.ExeNonQuery(eSP.qry_CountCurrentBalance);

            SetMasterBalance(objWallet.UsersId.Value, GetBalance(objWallet.UsersId.Value));
        }
    }

    public static decimal GetBalance(int UsersId)
    {
        decimal Balance = 0;
        try { Balance = new Query() { UsersId = UsersId }.Select(eSP.qry_GetCurrentBalance).Rows[0][CS.CurrentBalance].zToDecimal().Value; }
        catch { }

        return Balance;
    }

    public static string GeFormatedBalance(int UsersId)
    {
        return GetBalance(UsersId).AmountFormat();
    }


    public static void SetOrderTransaction(int OrdersId)
    {
        var dtOrders = new Query() { OrdersId = OrdersId }.Select(eSP.qry_Orders);
        if (dtOrders.Rows.Count > 0)
        {
            int StatusType = dtOrders.Rows[0][CS.eStatusType].zToInt().Value;

            var dtWallet = new Query() { eComponentType = (int)eComponentType.Order, ComponentId = OrdersId, eStatusNot = (int)eStatus.Delete }.Select(eSP.qry_Wallet);
            if (StatusType == (int)eStatusType.Cancel || StatusType == (int)eStatusType.RTO || StatusType == (int)eStatusType.RTODelivered
                    || StatusType == (int)eStatusType.RPickup || StatusType == (int)eStatusType.RPickupDelivered
                    || StatusType == (int)eStatusType.Draft)
            {
                if (dtWallet.Rows.Count > 0)
                {
                    CU.SetTransaction(dtWallet.Rows[0][CS.WalletId].zToInt(), dtOrders.Rows[0][CS.UsersId].zToInt().Value, 0,
                        eTransactionDirection.Debit, eTransactionType.Order, "Order " + CU.GetDescription((eStatusType)StatusType) + " (Order Id: " + OrdersId.ToString() + ")",
                        dtOrders.Rows[0][CS.Date].zToDate(), OrdersId, (int)eComponentType.Order);
                }
            }
            else
            {
                CU.SetTransaction(dtWallet.Rows.Count > 0 ? dtWallet.Rows[0][CS.WalletId].zToInt() : null,
                    dtOrders.Rows[0][CS.UsersId].zToInt().Value, dtOrders.Rows[0][CS.UserCommition].zToDecimal().Value, eTransactionDirection.Credit,
                    eTransactionType.Order, "Order Confirm (Order Id: " + OrdersId.ToString() + ")", dtOrders.Rows[0][CS.Date].zToDate(), OrdersId,
                    (int)eComponentType.Order);
            }
        }
    }

    #endregion

    #region Contact Detail

    public static void SaveContactDetail(int ParentId, eParentType ParentType, List<Contacts> lstContacts)
    {
        var dtContactDetail = new Contacts()
        {
            ParentId = ParentId,
            eParentType = (int)ParentType,
        }.Select(new Contacts() { ContactId = 0 });

        foreach (Contacts obj in lstContacts)
        {
            var objContact = new Contacts()
            {
                ContactId = dtContactDetail.Rows.Count > 0 ? dtContactDetail.Rows[0][CS.ContactId].zToInt() : null,
                ContactName = obj.ContactName,
                ContactText = obj.ContactText,
                eParentType = (int)ParentType,
                ParentId = ParentId,
                eContactType = (obj.eContactType.HasValue && obj.eContactType != 0) ? obj.eContactType : (int)eContactType.ContactNo,
            };

            if (objContact.ContactId.HasValue && objContact.ContactId != 0)
            {
                try { dtContactDetail.Rows.Remove(dtContactDetail.Select(CS.ContactId + " = " + objContact.ContactId)[0]); }
                catch { }

                objContact.UpdateAsync();
            }
            else
                objContact.InsertAsync();
        }

        foreach (DataRow drContactDetail in dtContactDetail.Rows)
            new Contacts() { ContactId = drContactDetail[CS.ContactId].ToString().zToInt() }.DeleteAsync();
    }

    #endregion

    #region API

    public static string APICall(string url)
    {
        bool IsBeforeAPICall = true;
        HttpWebRequest httpreq = (HttpWebRequest)WebRequest.Create(url);
        httpreq.Timeout = (10 * 60 * 1000);
        try
        {
            HttpWebResponse httpres = (HttpWebResponse)httpreq.GetResponse();
            IsBeforeAPICall = false;
            StreamReader sr = new StreamReader(httpres.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();
            return results;
        }
        catch (Exception ex)
        {
            FileWriteW.Error("APICall", true, true, "", "BeforeAPICall : " + IsBeforeAPICall.ToString() + Environment.NewLine + ex.Message, ex.Source, ex.StackTrace);
            return "0";
        }
    }

    #endregion

    #region Order


    public static int GetOrderStatusId(int StatusType, string StatusName)
    {
        int geteStatusType = 0;
        return GetOrderStatusId(ref geteStatusType, StatusType, StatusName, string.Empty);
    }

    public static int GetOrderStatusId(ref int geteStatusType, int StatusType, string StatusName, string PaymentMode)
    {
        #region Set Satus Type & Name

        var SetStatusType = StatusType == 0 ? eStatusType.UnDefined : (eStatusType)StatusType;
        string SetStatusName = StatusName;

        StatusName = StatusName.ToLower().Trim().Replace(" ", "");

        PaymentMode = PaymentMode.ToLower().Trim().Replace(" ", "");

        if (PaymentMode.ToLower() == "reverse")
        {
            if (StatusName.Contains("returnorder") || StatusName.Contains("intansit"))
                SetStatusType = eStatusType.RPickup;
            else if (StatusName.Contains("delivered"))
                SetStatusType = eStatusType.RTODelivered;
        }
        else
        {
            if (StatusName.Contains("rtointransit") || StatusName.Contains("rtointansit"))
                SetStatusType = eStatusType.RTO;
            else if (StatusName.Contains("pickedupandbookingprocessed"))
                SetStatusType = eStatusType.Dispatch;
            else if (StatusName.Contains("undelivered"))
            {
                if (SetStatusName.Contains("-"))
                {
                    var lstStatusName = SetStatusName.Split('-');
                    SetStatusName = lstStatusName.Length >= 1 ? lstStatusName[1] : SetStatusName;
                }

                SetStatusType = eStatusType.UnDelivered;
            }
            else if (StatusName.Contains("availableforrto"))
                SetStatusType = eStatusType.AvailabaleRTO;
            else if (StatusName.Contains("rtoarrived"))
                SetStatusType = eStatusType.RTO;
            else if (StatusName.Contains("delivered"))
                SetStatusType = eStatusType.Delivered;
            else if (StatusName.Contains("intransit") || StatusName.Contains("intansit") || StatusName.Contains("arrived")
                || StatusName.Contains("outfordelivery") || StatusName.Contains("connectedto")
                || StatusName.Contains("processing"))
                SetStatusType = eStatusType.InTranst;
        }

        #endregion

        #region OrderStatusId

        int OrganizationId = CU.GetOrganizationId();

        var dtOrderStatus = new Query()
        {
            OrganizationId = OrganizationId,
            StatusName = SetStatusName,
            eStatusType = StatusType == 0 ? (int?)null : StatusType,
            eStatusNot = (int)eStatus.Delete
        }.Select(eSP.qry_OrderStatus);

        if (dtOrderStatus.Rows.Count > 0)
        {
            var dr = dtOrderStatus.Select(CS.eStatus + " = " + (int)eStatus.Active);
            if (dr.Length > 0)
            {
                geteStatusType = dr[0][CS.eStatusType].zToInt().Value;
                return dr[0][CS.OrderStatusId].zToInt().Value;

            }
            else
            {
                var drDeactive = dtOrderStatus.Select(CS.eStatus + " = " + (int)eStatus.Deactive);

                new OrderStatus()
                {
                    OrderStatusId = drDeactive[0][CS.OrderStatusId].zToInt(),
                    eStatus = (int)eStatus.Active,
                }.Update();

                geteStatusType = dr[0][CS.eStatusType].zToInt().Value;
                return drDeactive[0][CS.OrderStatusId].zToInt().Value;
            }
        }
        else
        {
            geteStatusType = (int)SetStatusType;

            return new OrderStatus()
            {
                OrganizationId = OrganizationId,
                SerialNo = 30,
                StatusName = SetStatusName.zIsNullOrEmpty() ? CU.GetDescription(SetStatusType) : SetStatusName,
                eStatusType = (int)SetStatusType,
                eStatus = (int)eStatus.Active,
            }.Insert();
        }

        #endregion
    }

    #endregion

    #region Product

    public static decimal GetProductPrice(int FirmId, int ProductId)
    {
        decimal ProductPrice = 0;

        var dtFirm = new Firm() { FirmId = FirmId }.Select(new Firm() { PriceListId = 0 });
        if (dtFirm.Rows.Count > 0)
        {
            int? PriceListId = dtFirm.Rows[0][CS.PriceListId].zToInt();
            if (PriceListId.HasValue && PriceListId > 0)
            {
                var dtPriceListValue = new PriceListValue() { PriceListId = PriceListId, ProductId = ProductId }.Select(new PriceListValue() { Price = 0 });
                if (dtPriceListValue.Rows.Count > 0 && dtPriceListValue.Rows[0][CS.Price].zToInt() > 0)
                    ProductPrice = dtPriceListValue.Rows[0][CS.Price].zToInt().Value;
            }
        }

        return ProductPrice;
    }

    #endregion

    #region Adjustment

    public static void AdjustStockDelete(int ItemAdjustmentDetailId)
    {
        var objOldAdjustment = new ItemAdjustmentDetail() { ItemAdjustmentDetailId = ItemAdjustmentDetailId }.SelectList<ItemAdjustmentDetail>()[0];
        decimal NewQuantity = objOldAdjustment.Quantity.Value * -1;

        var objNewItem = new Item() { ItemId = objOldAdjustment.ItemId };
        var lstNewItem = objNewItem.SelectList<Item>();
        objNewItem.Stock = lstNewItem[0].Stock.HasValue ? lstNewItem[0].Stock + NewQuantity : NewQuantity;
        objNewItem.Update();

        new ItemAdjustmentDetail() { ItemAdjustmentDetailId = ItemAdjustmentDetailId }.Delete();
    }

    public static void AdjustStock(ItemAdjustmentDetail objNewAdjustment, bool IsInStock)
    {
        var objOldAdjustment = new ItemAdjustmentDetail();
        if (objNewAdjustment.ItemAdjustmentDetailId == null)
            objOldAdjustment = new ItemAdjustmentDetail() { Quantity = 0, ItemId = objNewAdjustment.ItemId };
        else
            objOldAdjustment = new ItemAdjustmentDetail() { ItemAdjustmentDetailId = objNewAdjustment.ItemAdjustmentDetailId }.SelectList<ItemAdjustmentDetail>()[0];

        decimal NewQuantity = 0;
        if (objNewAdjustment.ItemId == objOldAdjustment.ItemId)
        {
            if (IsInStock)
                NewQuantity = objNewAdjustment.Quantity.Value - objOldAdjustment.Quantity.Value;
            else
                NewQuantity = (objOldAdjustment.Quantity.Value + objNewAdjustment.Quantity.Value) * -1;
        }
        else
        {
            NewQuantity = IsInStock ? objNewAdjustment.Quantity.Value : (objNewAdjustment.Quantity.Value * -1);

            var OldQuantity = !IsInStock ? objNewAdjustment.Quantity.Value : (objNewAdjustment.Quantity.Value * -1);
            var objOldItem = new Item() { ItemId = objOldAdjustment.ItemId };
            var lstOldItem = objOldItem.SelectList<Item>();
            objOldItem.Stock = lstOldItem[0].Stock.HasValue ? lstOldItem[0].Stock + OldQuantity : OldQuantity;
            objOldItem.Update();
        }

        var objNewItem = new Item() { ItemId = objNewAdjustment.ItemId };
        var lstNewItem = objNewItem.SelectList<Item>();
        objNewItem.Stock = lstNewItem[0].Stock.HasValue ? lstNewItem[0].Stock + NewQuantity : NewQuantity;
        objNewItem.Update();

        if (!IsInStock)
            objNewAdjustment.Quantity = objNewAdjustment.Quantity * -1;
        if (objNewAdjustment.ItemAdjustmentDetailId.HasValue)
            objNewAdjustment.Update();
        else
            objNewAdjustment.Insert();
    }


    public static void OrderProductDelete(int OrderProductId)
    {
        var objOrderProduct = new OrderProduct()
        {
            OrderProductId = OrderProductId
        }.SelectList<OrderProduct>()[0];

        var lstOldAdjustment = new ItemAdjustment() { OrdersId = objOrderProduct.OrdersId }.SelectList<ItemAdjustment>();
        if (lstOldAdjustment.Count > 0)
        {
            var objOldAdjustment = lstOldAdjustment[0];
            var objAdjustmentDetail = new ItemAdjustmentDetail()
            {
                ItemAdjustmentId = objOldAdjustment.ItemAdjustmentId,
                ItemId = objOrderProduct.ItemId
            }.SelectList<ItemAdjustmentDetail>()[0];

            AdjustStockDelete(objAdjustmentDetail.ItemAdjustmentDetailId.Value);
        }

        new OrderProduct()
        {
            OrderProductId = OrderProductId
        }.Delete();
    }

    public static int AdjustStockOrderProduct(OrderProduct objNewOrderProduct)
    {
        var objOldOrderProduct = new OrderProduct();
        if (objNewOrderProduct.OrderProductId.HasValue)
        {
            objOldOrderProduct = new OrderProduct()
            {
                OrderProductId = objNewOrderProduct.OrderProductId
            }.SelectList<OrderProduct>()[0];

            var lstOldAdjustment = new ItemAdjustment() { OrdersId = objNewOrderProduct.OrdersId }.SelectList<ItemAdjustment>();
            if (lstOldAdjustment.Count > 0)
            {
                var objOldAdjustment = lstOldAdjustment[0];
                var objOldAdjustmentDetail = new ItemAdjustmentDetail()
                {
                    ItemAdjustmentId = objOldAdjustment.ItemAdjustmentId,
                    ItemId = objOldOrderProduct.ItemId
                }.SelectList<ItemAdjustmentDetail>()[0];

                var objNewAdjustment = new ItemAdjustmentDetail()
                {
                    ItemAdjustmentDetailId = objOldAdjustmentDetail.ItemAdjustmentDetailId,
                    ItemId = objNewOrderProduct.ItemId,
                    ProductId = objNewOrderProduct.ProductId,
                    ItemAdjustmentId = objOldAdjustmentDetail.ItemAdjustmentId,
                    Quantity = objNewOrderProduct.Quantity,
                    Rate = objNewOrderProduct.SalePrice
                };
                AdjustStock(objNewAdjustment, false);
                objNewOrderProduct.Update();
            }
        }
        else
        {
            var lstAdjustment = new ItemAdjustment() { OrdersId = objNewOrderProduct.OrdersId }.SelectList<ItemAdjustment>();
            int ItemAdjustmentId = 0;
            if (lstAdjustment.Count == 0)
            {
                ItemAdjustmentId = new ItemAdjustment()
                {
                    AdjustmentDate = IndianDateTime.Today,
                    OrdersId = objNewOrderProduct.OrdersId,
                    ReferenceNo = objNewOrderProduct.OrdersId.ToString(),
                    OrganizationId = CU.GetOrganizationId(),
                    Note = "Ordor No: " + objNewOrderProduct.OrdersId,
                }.Insert();
            }
            else
            {
                ItemAdjustmentId = lstAdjustment[0].ItemAdjustmentId.Value;
            }

            var objNewAdjustment = new ItemAdjustmentDetail()
            {
                ItemAdjustmentId = ItemAdjustmentId,
                ItemId = objNewOrderProduct.ItemId,
                ProductId = objNewOrderProduct.ProductId,
                Quantity = objNewOrderProduct.Quantity,
                Rate = objNewOrderProduct.SalePrice
            };
            AdjustStock(objNewAdjustment, false);

            objNewOrderProduct.OrderProductId = objNewOrderProduct.Insert();
        }
        return objNewOrderProduct.OrderProductId.Value;
    }

    public static void UpdateOrderStatus(Orders objNewOrders)
    {
        var objOldOrder = new Orders() { OrdersId = objNewOrders.OrdersId }.SelectList<Orders>()[0];
        eStatusType OldStatusType = GetOrderStatusToeOrderStatus(objOldOrder.OrderStatusId.Value);
        eStatusType NewStatusType = GetOrderStatusToeOrderStatus(objNewOrders.OrderStatusId.Value);

        bool? IsInStock = null;
        if ((OldStatusType == eStatusType.Cancel || OldStatusType == eStatusType.RTODelivered)
            && (NewStatusType != eStatusType.Cancel && NewStatusType != eStatusType.RTODelivered))
        {
            IsInStock = false;
        }
        else if ((OldStatusType != eStatusType.Cancel && OldStatusType != eStatusType.RTODelivered)
            && (NewStatusType == eStatusType.Cancel || NewStatusType == eStatusType.RTODelivered))
        {
            IsInStock = true;
        }

        if (IsInStock.HasValue)
        {
            var lstOrderProduct = new OrderProduct() { OrdersId = objOldOrder.OrdersId }.SelectList<OrderProduct>();
            foreach (OrderProduct objOrderProduct in lstOrderProduct)
            {
                var lstOldAdjustment = new ItemAdjustment() { OrdersId = objOrderProduct.OrdersId }.SelectList<ItemAdjustment>();
                if (lstOldAdjustment.Count > 0)
                {
                    var objOldAdjustment = lstOldAdjustment[0];
                    var objOldAdjustmentDetail = new ItemAdjustmentDetail()
                    {
                        ItemAdjustmentId = objOldAdjustment.ItemAdjustmentId,
                        ItemId = objOrderProduct.ItemId
                    }.SelectList<ItemAdjustmentDetail>()[0];

                    var objNewAdjustment = new ItemAdjustmentDetail()
                    {
                        ItemAdjustmentDetailId = objOldAdjustmentDetail.ItemAdjustmentDetailId,
                        ItemId = objOrderProduct.ItemId,
                        ProductId = objOrderProduct.ProductId,
                        ItemAdjustmentId = objOldAdjustmentDetail.ItemAdjustmentId,
                        Quantity = IsInStock.Value ? 0 : objOrderProduct.Quantity,
                        Rate = objOrderProduct.SalePrice
                    };
                    AdjustStock(objNewAdjustment, IsInStock.Value);
                }
            }
        }

        objNewOrders.Update();
    }

    #endregion
}