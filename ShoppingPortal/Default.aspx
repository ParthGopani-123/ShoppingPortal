<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="_Default" Title="Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">
    <asp:Label ID="lbltabHeight1" runat="server" CssClass="lbltabHeight1 hidden" Text="126"></asp:Label>
    <div class="row">
        <div class="page-header clearfix">
            <ol class="breadcrumb mb-0">
                <li class="active"><a>Home</a></li>
            </ol>
        </div>
        <div class="page-contan tabHeight1">
            <div class="col-lg-12">
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="popup" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="script" runat="Server">
</asp:Content>
