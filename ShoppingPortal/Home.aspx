<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Home.aspx.cs" Inherits="Home" Title="Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .aLink {
            color: #fff !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="Server">
    <div class="row">
        <div class="page-header clearfix">
            <ol class="breadcrumb mb-0">
                <li class="active"><a>Home</a></li>
            </ol>
        </div>
    </div>

    <div class="row">
        <div class="col-sm-3">
            <a class="aLink" href="ManageOrder.aspx">
                <div class="widget no-border p-15 bg-success media">
                    <div class="media-left media-middle"><i class="media-object ti-plus fs-36"></i></div>
                    <div class="media-body">
                        <h4 class="m-0">New Order</h4>
                    </div>
                </div>
            </a>
        </div>
        <div class="col-sm-3">
            <a class="aLink" href="OrderView.aspx">
                <div class="widget no-border p-15 bg-success media">
                    <div class="media-left media-middle"><i class="media-object ti-shopping-cart fs-36"></i></div>
                    <div class="media-body">
                        <h4 class="m-0">My Orders</h4>
                    </div>
                </div>
            </a>
        </div>
        <div class="col-sm-3">
            <a id="aSearchService" runat="server" target="_blank" class="aLink">
                <div class="widget no-border p-15 bg-success media">
                    <div class="media-left media-middle"><i class="media-object ti-pin fs-36"></i></div>
                    <div class="media-body">
                        <h4 class="m-0">Search Pincode</h4>
                    </div>
                </div>
            </a>
        </div>
        <div class="col-sm-3">
            <a id="aSearchProduct" runat="server" target="_blank" class="aLink">
                <div class="widget no-border p-15 bg-success media">
                    <div class="media-left media-middle"><i class="media-object ti-search fs-36"></i></div>
                    <div class="media-body">
                        <h4 class="m-0">Search Product</h4>
                    </div>
                </div>
            </a>
        </div>
        <div class="col-sm-3">
            <a id="aDownloadApp" href="Download/Application/Android.apk">
                <div class="widget no-border p-15 bg-success media">
                    <div class="media-left media-middle"><i class="media-object ti-android fs-36"></i></div>
                    <div class="media-body">
                        <h4 class="m-0">Application</h4>
                    </div>
                </div>
            </a>
        </div>
        <div class="col-sm-3">
            <a id="aRecorder" href="Download/Application/CallRecorder.apk">
                <div class="widget no-border p-15 bg-success media">
                    <div class="media-left media-middle"><i class="media-object ti-microphone fs-36"></i></div>
                    <div class="media-body">
                        <h4 class="m-0">Recorder</h4>
                    </div>
                </div>
            </a>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="popup" runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="script" runat="Server">
</asp:Content>
