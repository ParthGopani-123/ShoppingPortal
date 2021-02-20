<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="ViewNotification.aspx.cs" Inherits="ViewNotification" EnableEventValidation="false"
    Title="Notification" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .tblpointer tr
        {
            cursor: pointer;
        }
        .lblNotificationDate
        {
            display: block;
            font-size: 11px;
            color: #b5b5b5;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Label ID="lbltabHeight1" runat="server" CssClass="lbltabHeight1 hidden" Text="151"></asp:Label>
            <asp:Label ID="lbltabHeightSmall1" runat="server" CssClass="lbltabHeightSmall1 hidden"
                Text="190"></asp:Label>
            <div class="row">
                <div class="page-header clearfix">
                    <div class="col-lg-9 col-md-9 col-sm-9 col-xs-12 p0">
                        <div class="btn-group">
                            <asp:LinkButton ID="lnkRefresh" ToolTip="Refresh" runat="server" OnClick="lnkRefresh_OnClick"
                                CssClass="btn btngroup btn-refresh tooltips clickloader" data-toggle="tooltip">
                            <i class="fa fa-refresh"></i>
                            </asp:LinkButton>
                        </div>
                    </div>
                    <div class="col-lg-3 col-md-3 col-sm-3 col-xs-8 search div-master-search div-master-search-xs">
                        <asp:Label ID="lblCount" ToolTip="Count" runat="server" Text="10" CssClass="pull-left mr-5 btn btn-icon btn-total tooltips"
                            data-toggle="tooltip">
                        </asp:Label>
                        <div class="input-group">
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control master-search"
                                OnChange="addRegionLoader('divloader')" AutoPostBack="true" OnTextChanged="lnkRefresh_OnClick"
                                placeholder="Search..."></asp:TextBox>
                            <a note-colspan="note-colspan-class" class="tooltips input-group-addon aShowSearch btn btn-master-search ml5"
                                data-original-title="Search" data-toggle="tooltip"><i note-colspan="note-colspan-class"
                                    class="fa fa-chevron-down"></i></a>
                        </div>
                    </div>
                    <div note-colspan="note-colspan-class" class="search-tools divShowSearch note-colspan-class">
                        <asp:Panel ID="pnlSearch" runat="server" DefaultButton="lnkSearch" class="divsearchloader"
                            note-colspan="note-colspan-class">
                            <div class="padbm" note-colspan="note-colspan-class">
                                <asp:DropDownList runat="server" ID="ddlNotificationType" CssClass="form-control"
                                    note-colspan="note-colspan-class">
                                </asp:DropDownList>
                            </div>
                            <div class="padbm" note-colspan="note-colspan-class">
                                <div class="input-group">
                                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control datepickerpast" note-colspan="note-colspan-class"></asp:TextBox>
                                    <span class="input-group-addon" note-colspan="note-colspan-class">to</span>
                                    <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control datepickerpast" note-colspan="note-colspan-class"></asp:TextBox>
                                </div>
                            </div>
                            <div class="padbm text-right" note-colspan="note-colspan-class">
                                <asp:LinkButton ID="lnkSearch" note-colspan="note-colspan-class" OnClientClick="addRegionLoader('divloader')"
                                    OnClick="lnkRefresh_OnClick" class="btn btn-warning btnsearch" runat="server"><i
                                    class="fa fa-filter"></i> Filter</asp:LinkButton>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
                <div class="page-content col-md-12 divtable divloader">
                    <div class="table-responsive tabHeight1 fixheight1">
                        <asp:GridView ID="grdNotification" AllowPaging="false" runat="server" AutoGenerateColumns="False"
                            OnRowDataBound="grdNotification_OnRowDataBound" class="table table-bordered tblpointer table-hover nomargin fixheader">
                            <Columns>
                                <asp:TemplateField HeaderText="Notification">
                                    <ItemTemplate>
                                        <asp:Literal ID="ltrNotificationText" runat="server"></asp:Literal>
                                        <div id="divNotification" runat="server">
                                            <a id="aNotificationLink" runat="server" class="hide aNotificationLink"></a>
                                        </div>
                                        <asp:Label ID="lblNotificationDate" CssClass="lblNotificationDate" runat="server"
                                            Text=""></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="text-center text-danger">
                                    <br />
                                    <i class="fa fa-4x fa-smile-o"></i>
                                    <h3>
                                        Sorry, No Data Found.</h3>
                                </div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                    <div id="divPaging" runat="server" class="col-md-12 col-sm-12 col-xs-12 div-paging">
                        <asp:LinkButton ID="lnkFirst" OnClick="lnkFirst_Click" OnClientClick="addRegionLoader('divloader')"
                            runat="server" ToolTip="First Page" CssClass="fa fa-fast-backward btn-paging tooltips"></asp:LinkButton>
                        <asp:LinkButton ID="lnkPrev" OnClick="lnkPrev_Click" OnClientClick="addRegionLoader('divloader')"
                            runat="server" ToolTip="Previous Page" CssClass="fa fa-backward btn-paging tooltips"></asp:LinkButton>
                        <asp:TextBox ID="txtGotoPageNo" Text="1" runat="server" CssClass="txt-paging" OnTextChanged="txtGotoPageNo_OnTextChange"
                            OnChange="addRegionLoader('divloader')" AutoPostBack="true"></asp:TextBox>
                        <asp:LinkButton ID="lnkNext" OnClick="lnkNext_Click" OnClientClick="addRegionLoader('divloader')"
                            runat="server" ToolTip="Next Page" CssClass="fa fa-forward btn-paging tooltips"></asp:LinkButton>
                        <asp:LinkButton ID="lnkLast" OnClick="lnkLast_Click" OnClientClick="addRegionLoader('divloader')"
                            runat="server" ToolTip="Last Page" CssClass="fa fa-fast-forward btn-paging tooltips"></asp:LinkButton>
                        <asp:DropDownList ID="ddlRecordPerPage" runat="server" CssClass="ddlNotSearch ml-5 ddl-paging"
                            AutoPostBack="True" OnChange="addRegionLoader('divloader')" OnSelectedIndexChanged="ddlRecordPerPage_LoadMember">
                        </asp:DropDownList>
                        <span class="lbl-paging">Records / Page</span>
                        <label class="pull-right mt2 lbl-paging hidden-xs">
                            <asp:Literal ID="ltrTotalContent" runat="server"></asp:Literal>
                        </label>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="popup" runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="script" runat="Server">
</asp:Content>
