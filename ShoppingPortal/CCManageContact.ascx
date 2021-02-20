<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CCManageContact.ascx.cs"
    Inherits="CCManageContact" %>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:Repeater ID="rptContacts" runat="server" OnItemDataBound="rptContacts_OnItemDataBound">
            <ItemTemplate>
                <div class="form-group mb-5">
                    <div class="row">
                        <label class="col-lg-3 col-md-3 col-sm-4 control-label">
                            Contact<span class="text-danger">*</span>
                        </label>
                        <div class="col-lg-8 col-md-8 col-sm-7">
                            <div class="input-group">
                                <asp:Label ID="lblPK" runat="server" Text="" Visible="false"></asp:Label>
                                <asp:Label ID="lblContactId" runat="server" Text="" Visible="false"></asp:Label>
                                <span class="input-group-addon"><i class="fa fa-user"></i></span>
                                <asp:TextBox ID="txtContactName" CssClass="form-control" runat="server" MaxLength="500"
                                    placeholder="Name"></asp:TextBox>
                                <span class="input-group-addon"><i class="fa fa-mobile"></i></span>
                                <asp:TextBox ID="txtContactNo" CssClass="form-control intnumber" runat="server" MaxLength="15"
                                    placeholder="Contact No"></asp:TextBox>
                                <asp:LinkButton ID="lnkDeleteContact" OnClick="lnkDeleteContact_OnClick" CssClass="input-group-addon"
                                    runat="server"><i class="fa fa-trash-o"></i></asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <div class="form-group">
            <div class="row">
                <div class="col-lg-11 col-md-11 col-sm-11 col-xs-11 text-right">
                    <asp:LinkButton ID="lnkAddNewContact" OnClick="lnkAddNewContact_OnClick" runat="server"><i class="fa fa-plus"></i> New Contact</asp:LinkButton>
                </div>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
