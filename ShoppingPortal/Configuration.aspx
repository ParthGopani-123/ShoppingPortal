<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Configuration.aspx.cs" Inherits="Configuration" Title="Configuration" EnableEventValidation="false" %>

<%@ Register Src="~/CCManageCustomer.ascx" TagName="ManageCustomerCC" TagPrefix="MC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <p>
        &nbsp;</p>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Label ID="lbltabHeight1" runat="server" CssClass="lbltabHeight1 hidden" Text="139"></asp:Label>
            <asp:Label ID="lblOrganizationId" runat="server" Visible="false"></asp:Label>
            <div class="row">
                <asp:Panel ID="pnlOrder" runat="server" DefaultButton="lnkSave" class="page-content col-md-12 divtable divloader">
                    <div class="tabHeight1 checkvalidOrderDetail mt-10">
                        <div class="form-horizontal">
                            <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                <div class="form-group">
                                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                        Email
                                    </label>
                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                        Password
                                    </label>
                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                        Shipway Username
                                    </label>
                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                        <asp:TextBox ID="txtShipwayUsername" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                                        Shipway Licence Key
                                    </label>
                                    <div class="col-lg-8 col-md-8 col-sm-7">
                                        <asp:TextBox ID="txtShipwayLicenceKey" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                <div class="form-group">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <div class="checkbox-custom">
                                            <asp:CheckBox ID="chkSameCustomerDifferantUser" runat="server" CssClass="chk" Text="Allow same customer's order to different users" />
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <div class="checkbox-custom">
                                            <asp:CheckBox ID="chkCanUserSelectCourier" runat="server" CssClass="chk" Text="Users Can Select Courier" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer footer">
                        <asp:LinkButton ID="lnkCancel" OnClick="lnkCancel_OnClick" runat="server" CssClass="btn btn-raised btn-default clickloader">Cancel</asp:LinkButton>
                        <asp:LinkButton ID="lnkSave" OnClick="lnkSave_OnClick" runat="server" CssClass="lnkSave btn btn-raised btn-black clickloader">Save</asp:LinkButton>
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="popup" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="script" runat="Server">

    <script type="text/javascript">

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Checkpostback);
        jQuery(function () {
            Checkpostback();
        });
        function Checkpostback() {


        }

    </script>

</asp:Content>
