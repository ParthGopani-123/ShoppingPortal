<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CCManageAddress.ascx.cs"
    Inherits="CCManageAddress" %>
<asp:UpdatePanel ID="updPanle1" runat="server">
    <ContentTemplate>
        <style type="text/css">
            .map_canvas
            {
                width: 100%;
                height: 300px;
            }
            .pac-logo
            {
                z-index: 999999 !important;
            }
        </style>
        <asp:Label ID="lblMapType" runat="server" Visible="false" Text=""></asp:Label>
        <div class="form-group">
            <div class="row">
                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                    Address Line1
                </label>
                <div class="col-lg-8 col-md-8 col-sm-7">
                    <asp:TextBox ID="txtAddress1" MaxLength="150" runat="server" class="form-control no-resize"
                        placeholder="Address Line 1" TextMode="MultiLine"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                    Line2
                </label>
                <div class="col-lg-8 col-md-8 col-sm-7">
                    <asp:TextBox ID="txtAddress2" MaxLength="150" runat="server" class="form-control no-resize"
                        placeholder="Line 2" TextMode="MultiLine"></asp:TextBox>
                </div>
            </div>
        </div>
        <div id="divMapData" runat="server" visible="false" class="mapdata">
            <div class="form-group">
                <div class="row">
                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                        Search Address
                    </label>
                    <div class="col-lg-8 col-md-8 col-sm-7">
                        <asp:TextBox ID="txtSearchAddress" runat="server" class="txtSearchAddress form-control"
                            MaxLength="50" placeholder="Search Address"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="row">
                    <label class="col-lg-1 col-md-1 col-sm-1 control-label">
                    </label>
                    <div class="col-lg-10 col-md-10 col-sm-9">
                        <div class="map_canvas">
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group hidden">
                <div class="row">
                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                        Latitude
                    </label>
                    <div class="col-lg-8 col-md-8 col-sm-7">
                        <input name="lat" class="lat form-control" type="text" placeholder="Latitude" maxlength="50"
                            value="">
                        <asp:TextBox ID="txtLatitude" runat="server" class="txtLatitude hidden form-control"
                            MaxLength="50" placeholder="Enter Latitude"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="form-group hidden">
                <div class="row">
                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                        Longitude
                    </label>
                    <div class="col-lg-8 col-md-8 col-sm-7">
                        <input name="lng" class="lng form-control" placeholder="Longitude" type="text" value="">
                        <asp:TextBox ID="txtLongitude" runat="server" class="txtLongitude hidden form-control"
                            MaxLength="50" placeholder="Enter Longitude"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="row">
                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                        Pincode
                    </label>
                    <div class="col-lg-8 col-md-8 col-sm-7">
                        <asp:TextBox ID="txtPincodeMap" runat="server" class="form-control"  MaxLength="12" placeholder="Pincode"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="row">
                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                        Area/City<span class="text-danger">*</span>
                    </label>
                    <div class="col-lg-8 col-md-8 col-sm-7">
                        <div class="input-group w100">
                            <input type="text" id="sublocality" name="sublocality" class="sublocality form-control"
                                zvalidation="e=blur|v=IsRequired|m=Area Name" placeholder="Area" maxlength="20"
                                type="text" value="">
                            <asp:TextBox ID="txtAreaMap" runat="server" class="txtAreaMap hidden"></asp:TextBox>
                            <span class="input-group-addon addonnoborder"></span>
                            <input type="text" id="locality" name="locality" class="locality form-control" maxlength="20"
                                zvalidation="e=blur|v=IsRequired|m=City Name" placeholder="City" type="text"
                                value="">
                            <asp:TextBox ID="txtCityMap" runat="server" class="txtCityMap hidden"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="row">
                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                        State/Country<span class="text-danger">*</span>
                    </label>
                    <div class="col-lg-8 col-md-8 col-sm-7">
                        <div class="input-group w100">
                            <input type="text" id="administrative_area_level_1" name="administrative_area_level_1"
                                class="administrative_area_level_1 form-control" maxlength="20" zvalidation="e=blur|v=IsRequired|m=State Name"
                                placeholder="State" type="text" value="">
                            <asp:TextBox ID="txtStateMap" runat="server" class="txtStateMap hidden"></asp:TextBox>
                            <span class="input-group-addon addonnoborder"></span>
                            <input type="text" id="country" name="country" class="country form-control" maxlength="20"
                                zvalidation="e=blur|v=IsRequired|m=Country Name" placeholder="Country" type="text"
                                value="">
                            <asp:TextBox ID="txtCountryMap" runat="server" class="txtCountryMap hidden"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div id="divAreaDetalNotMap" runat="server">
            <div class="form-group">
                <div class="row">
                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                        Pincode
                    </label>
                    <div class="col-lg-8 col-md-8 col-sm-7">
                        <asp:TextBox ID="txtPincode" runat="server" class="form-control" ZValidation="e=blur|v=IsNullPincode|m=Pincode"
                            MaxLength="6" placeholder="Pincode"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="form-group" id="divArea" runat="server">
                <div class="row">
                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                        Area
                    </label>
                    <div class="col-lg-8 col-md-8 col-sm-7">
                        <div class="input-group">
                            <asp:DropDownList ID="ddlArea" runat="server" onchange="addRegionLoader('address-loader')"
                                AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddlArea_OnSelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:LinkButton ID="lnkAreaAdd" runat="server" OnClick="lnkAreaAdd_OnClick" CssClass="tooltips input-group-addon clickloader"
                                data-toggle="tooltip" data-original-title="Add"><i class="fa fa-plus"></i>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group" id="divOtherArea" runat="server" visible="false">
                <div class="row">
                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                        Area<span class="text-danger">*</span>
                    </label>
                    <div class="col-lg-8 col-md-8 col-sm-7">
                        <div class="input-group">
                            <asp:TextBox ID="txtOtherArea" runat="server" MaxLength="20" ZValidation="e=blur|v=IsRequired|m=Area Name"
                                class="form-control" placeholder="Area Name"></asp:TextBox>
                            <asp:LinkButton ID="lnkAreaCancle" runat="server" OnClick="lnkAreaCancle_OnClick"
                                CssClass="tooltips input-group-addon clickloader" data-toggle="tooltip" data-original-title="Cancel"><i class="fa fa-close"></i>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group" id="divCity" runat="server">
                <div class="row">
                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                        City
                    </label>
                    <div class="col-lg-8 col-md-8 col-sm-7">
                        <div class="input-group">
                            <asp:DropDownList ID="ddlCity" AutoPostBack="true" onchange="addRegionLoader('address-loader')"
                                OnSelectedIndexChanged="ddlCity_OnSelectedIndexChanged" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                            <asp:LinkButton ID="lnkCityRefresh" runat="server" OnClick="lnkCityAdd_OnClick" CssClass="tooltips input-group-addon clickloader"
                                data-toggle="tooltip" data-original-title="Add">
                                                  <i class="fa fa-plus"></i>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group" id="divOtherCity" runat="server" visible="false">
                <div class="row">
                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                        City<span class="text-danger">*</span>
                    </label>
                    <div class="col-lg-8 col-md-8 col-sm-7">
                        <div class="input-group">
                            <asp:TextBox ID="txtOtherCity" runat="server" MaxLength="20" ZValidation="e=blur|v=IsRequired|m=City Name"
                                class="form-control" placeholder="City Name"></asp:TextBox>
                            <asp:LinkButton ID="lnkCityCancle" runat="server" OnClick="lnkCityCancle_OnClick"
                                CssClass="tooltips input-group-addon clickloader" data-toggle="tooltip" data-original-title="Cancel"><i class="fa fa-close"></i>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group" id="divState" runat="server">
                <div class="row">
                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                        State
                    </label>
                    <div class="col-lg-8 col-md-8 col-sm-7">
                        <div class="input-group">
                            <asp:DropDownList ID="ddlState" AutoPostBack="true" onchange="addRegionLoader('address-loader')"
                                OnSelectedIndexChanged="ddlState_OnSelectedIndexChanged" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                            <asp:LinkButton ID="lnkStateRefresh" runat="server" OnClick="lnkStateAdd_OnClick"
                                CssClass="tooltips input-group-addon clickloader" data-toggle="tooltip" data-original-title="Add"><i class="fa fa-plus"></i>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group" id="divOtherState" runat="server" visible="false">
                <div class="row">
                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                        State<span class="text-danger">*</span>
                    </label>
                    <div class="col-lg-8 col-md-8 col-sm-7">
                        <div class="input-group">
                            <asp:TextBox ID="txtOtherState" runat="server" MaxLength="20" ZValidation="e=blur|v=IsRequired|m=State Name"
                                class="form-control" placeholder="State Name"></asp:TextBox>
                            <asp:LinkButton ID="lnkStateCancle" runat="server" OnClick="lnkStateCancle_OnClick"
                                CssClass="tooltips input-group-addon clickloader" data-toggle="tooltip" data-original-title="Cancel"><i class="fa fa-close"></i>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group" id="divCountry" runat="server">
                <div class="row">
                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                        Country
                    </label>
                    <div class="col-lg-8 col-md-8 col-sm-7">
                        <div class="input-group">
                            <asp:DropDownList ID="ddlCountry" AutoPostBack="false" OnSelectedIndexChanged="ddlCountry_OnSelectedIndexChanged"
                                runat="server" CssClass="form-control">
                            </asp:DropDownList>
                            <asp:LinkButton ID="lnkCountryRefresh" runat="server" OnClick="lnkCountryAdd_OnClick"
                                CssClass="tooltips input-group-addon clickloader" data-toggle="tooltip" Text="Add"
                                data-original-title="Add"><i class="fa fa-plus"></i>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group" id="divOtherCountry" runat="server" visible="false">
                <div class="row">
                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                        Country<span class="text-danger">*</span>
                    </label>
                    <div class="col-lg-8 col-md-8 col-sm-7">
                        <div class="input-group">
                            <asp:TextBox ID="txtOtherCountry" runat="server" MaxLength="20" ZValidation="e=blur|v=IsRequired|m=Country Name"
                                class="form-control" placeholder="Country Name"></asp:TextBox>
                            <asp:LinkButton ID="lnkCountryCancle" runat="server" OnClick="lnkCountryCancle_OnClick"
                                CssClass="tooltips input-group-addon clickloader" data-toggle="tooltip" data-original-title="Cancel"><i class="fa fa-close"></i>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <asp:CheckBox ID="chkCountry" runat="server" Visible="false" Checked="false" />
        <asp:CheckBox ID="chkState" runat="server" Visible="false" Checked="false" />
        <asp:CheckBox ID="chkCity" runat="server" Visible="false" Checked="false" />
        <asp:CheckBox ID="chkArea" runat="server" Visible="false" Checked="false" />
    </ContentTemplate>
</asp:UpdatePanel>
