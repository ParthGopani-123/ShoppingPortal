<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CCTextExtender.ascx.cs"
    Inherits="CCTextExtender" %>
<div class="input-group">
    <asp:TextBox ID="txtData" placeholder="Select Data" CssClass="form-control txtNotFocus txtData brnone"
        runat="server"></asp:TextBox>
    <cc1:AutoCompleteExtender ID="acaData" runat="server" ServicePath="TextboxExtender.asmx"
        MinimumPrefixLength="1" CompletionInterval="100" EnableCaching="true" TargetControlID="txtData"
        FirstRowSelected="false" OnClientPopulating="Data_OnClientPopulating" OnClientPopulated="Data_OnClientPopulated"
        CompletionListCssClass="ComListClass" CompletionListItemCssClass="ComListItemClass"
        CompletionListHighlightedItemCssClass="CompListHighlightedItemClass">
    </cc1:AutoCompleteExtender>
    <asp:TextBox ID="txtDataId" CssClass="txtDataId hidden" runat="server"></asp:TextBox>
    <label class="lblDataSelectedJSON hidden">
    </label>
    <asp:Label ID="lblDestinationAddon" CssClass="input-group-addon disabledspan bgwhite" runat="server"><i class="fa fa-search"></i></asp:Label>
</div>
