<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GetNotificaionList.aspx.cs"
    Inherits="GetNotificaionList" EnableEventValidation="false" %>

<form id="form1" runat="server">
    #OCTFIS#
    <h5 class="dropdown-header">
        <asp:label runat="server" id="lblNoificaionial" text="You have 0 notifications"></asp:label>
    </h5>
    <ul data-mcs-theme="minimal-dark" class="media-list mCustomScrollbar">
        <asp:repeater id="rptNotification" onitemdatabound="rptNotification_OnItemDataBound" runat="server">
        <ItemTemplate>
            <li class="media divViewNotificationText" onclick="ViewNotificationText('<%# Eval("NotificationId") %>', '<%# Eval("NotificationText").ToString() %>');">
                <div id="divNotification" runat="server">
                    <div class="media-left avatar valignsub">
                        <asp:Image ID="imgNotificationImage" runat="server" alt="" CssClass="media-object img-circlee"></asp:Image>
                    </div>
                    <div class="media-body">
                        <h6 class="media-heading">
                            <%# Eval("NotificationText")%></h6>
                        <p class="text-muted mb-0 pull-left">
                            <%# Eval("InsertTime", "{0:dd-MM-yyyy hh:mm tt}")%></p>
                            <a id="aNotificationLink" runat="server" class="pull-right aNotificationLink"></a>
                    </div>
                </div>
            </li>
        </ItemTemplate>    
    </asp:repeater>
    </ul>
    <div class="dropdown-footer text-center p-10">
        <a href="ViewNotification.aspx" class="fw-500 text-muted">See all notifications</a>
    </div>
    #OCTFIS#
</form>
