<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CCManageCustomer.ascx.cs"
    Inherits="CCManageCustomer" %>
<asp:UpdatePanel ID="upnl" runat="server">
    <ContentTemplate>
        <style type="text/css">
            .divwidget {
                width: 20%;
                color: #fff;
                display: inline-block;
                padding: 10px;
                font-size: 15px;
                margin-top: 5px;
            }

            .bg-success {
                background-color: green;
            }

            .bg-purple {
                background-color: purple;
            }

            .bg-red {
                background-color: #d60505;
            }

            .Status {
                font-size: 12px;
                display: block;
                color: #dadada;
            }
        </style>
        <asp:Label ID="lblUserId" runat="server" Visible="false"></asp:Label>
        <asp:Label ID="lblCustomerId" runat="server" Visible="false"></asp:Label>
        <asp:Panel ID="pnlCustomer" runat="server" DefaultButton="btnSave">
            <div class="modal-dialog">
                <div class="modal-content darkmodel">
                    <div class="modal-header bg-black">
                        <a class="close ClosePopup">×</a>
                        <h4 class="modal-title">
                            <asp:Label ID="lblpopupCustomerTitle" runat="server">New Customer</asp:Label></h4>
                    </div>
                    <div class="modal-body div-loader-Customer div-check-validation-Customer">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="col-lg-12 col-md-12 col-sm-12">
                                    Name<span class="text-danger">*</span>
                                    <asp:TextBox ID="txtName" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=Name"
                                        runat="server" MaxLength="100" placeholder="Enter Name"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                    Whatsapp No<span class="text-danger">*</span>
                                    <asp:TextBox ID="txtWhatsappNo" CssClass="form-control intnumber" ZValidation="e=blur|v=IsNumber|m=Whatsapp No"
                                        runat="server" MaxLength="15" placeholder="Enter Whatsapp No"></asp:TextBox>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                    Mobile No<span class="text-danger">*</span>
                                    <asp:TextBox ID="txtMobileNo" CssClass="form-control intnumber" ZValidation="e=blur|v=IsNumber|m=Mobile No"
                                        runat="server" MaxLength="15" placeholder="Enter Mobile No"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-lg-12 col-md-12 col-sm-12">
                                    Address<span class="text-danger">*</span>
                                    <asp:TextBox ID="txtAddress" TextMode="MultiLine" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=Address"
                                        runat="server" placeholder="Enter Address"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                    Pincode<span class="text-danger">*</span>
                                    <asp:TextBox ID="txtPincode" AutoPostBack="true" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=Pincode"
                                        OnTextChanged="txtPincode_OnTextChanged" onchange="addRegionLoader('div-loader-Customer')" runat="server" MaxLength="10" placeholder="Enter Pincode"></asp:TextBox>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                    City<span class="text-danger">*</span>
                                    <asp:TextBox ID="txtCity" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=City"
                                        runat="server" MaxLength="100" placeholder="Enter City"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                    State<span class="text-danger">*</span>
                                    <asp:TextBox ID="txtState" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=State"
                                        runat="server" MaxLength="100" placeholder="Enter State"></asp:TextBox>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                    Country<span class="text-danger">*</span>
                                    <asp:TextBox ID="txtCountry" CssClass="form-control" ZValidation="e=blur|v=IsRequired|m=Country"
                                        runat="server" MaxLength="100" placeholder="Enter Country"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-lg-12 col-md-12 col-sm-12">
                                    Customer Note
									<asp:TextBox ID="txtCustomerNote" CssClass="form-control" runat="server" TextMode="MultiLine" placeholder="Enter Customer Note"></asp:TextBox>
                                </div>
                            </div>
                            <asp:Panel ID="pnlService" runat="server" class="form-group text-center">
                                <div id="divPrepaid" runat="server" class="divwidget">
                                    <span class="divTitle">Prepaid</span>
                                    <asp:Label ID="lblPrepaidStatus" runat="server" CssClass="Status" Text="Unvailable"></asp:Label>
                                </div>
                                <div id="divCOD" runat="server" class="divwidget">
                                    <span class="divTitle">COD</span>
                                    <asp:Label ID="lblCODStatus" runat="server" CssClass="Status" Text="Unvailable"></asp:Label>
                                </div>
                                <div id="divPickup" runat="server" class="divwidget">
                                    <span class="divTitle">Pickup</span>
                                    <asp:Label ID="lblPickupStatus" runat="server" CssClass="Status" Text="Unvailable"></asp:Label>
                                </div>
                                <div id="divReversePickup" runat="server" class="divwidget">
                                    <span class="divTitle">R Pickup</span>
                                    <asp:Label ID="lblReversePickupStatus" runat="server" CssClass="Status" Text="Unvailable"></asp:Label>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-raised btn-default ClosePopup"
                            Text="Cancel" />
                        <asp:Button ID="btnSave" OnClick="btnSave_OnClick" runat="server" CssClass="btnSaveCustomer btn btn-raised btn-black"
                            Text="Save" />
                        <asp:Button ID="btnSaveAndNew" OnClick="btnSaveAndNew_OnClick" runat="server" CssClass="btnSaveCustomerAndNew btn btn-raised btn-black"
                            Text="Save & New" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckpostbackCustomer);

    function CheckpostbackCustomer() {
        $(".btnSaveCustomer").click(function () {
            if (CheckValidation("div-check-validation-Customer")) {
                addLoader('btnSaveCustomer');
                return true;
            }
            else {
                return false;
            }
        });

        $(".btnSaveCustomerAndNew").click(function () {
            if (CheckValidation("div-check-validation-Customer")) {
                addLoader('btnSaveCustomerAndNew');
                return true;
            }
            else {
                return false;
            }
        });
    }
</script>

