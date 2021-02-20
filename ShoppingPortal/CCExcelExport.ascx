<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CCExcelExport.ascx.cs"
    Inherits="CCExcelExport" %>
<asp:UpdatePanel ID="UpdatePanel2" runat="server">
    <ContentTemplate>
        <div class="modal-dialog">
            <div class="modal-content darkmodel">
                <asp:Label ID="lblFileName" runat="server" Visible="false" Text=""></asp:Label>
                <div class="modal-header bg-black">
                    <button type="button" class="ClosePopup close">
                        ×</button>
                    <h4 class="modal-title">Select Detail To Export</h4>
                </div>
                <div class="modal-body divloaderexport">
                    <asp:Panel ID="pnlExport" runat="server" DefaultButton="btnExport">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="col-lg-12 col-md-12 col-sm-12">
                                    <asp:LinkButton ID="lnkSetDefaultSelect" OnClientClick="addRegionLoader('divloaderexport');"
                                        OnClick="lnkSetDefaultSelect_OnClick" runat="server">Select Default</asp:LinkButton>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-lg-12 col-md-12 col-sm-12">
                                    <asp:Repeater ID="rptExportColumn" runat="server" OnItemDataBound="rptExportColumn_OnItemDataBound">
                                        <ItemTemplate>
                                            <div class="checkbox-custom">
                                                <asp:CheckBox ID="chkExportColumn" runat="server" />
                                            </div>
                                            <br />
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </div>
                <div class="modal-footer">
                    <button type="button" class="ClosePopup btn btn-raised btn-default">
                        Cancel</button>
                    <asp:Button ID="btnExport" OnClick="btnExport_OnClick" runat="server" CssClass="btn btn-raised btn-black clickloader"
                        Text="Export" />
                </div>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
