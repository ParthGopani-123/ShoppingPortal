<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CCManageAuthority.ascx.cs"
	Inherits="CCManageAuthority" %>
<%@ Register Src="~/CCConfirmPopup.ascx" TagName="ConfirmPopup" TagPrefix="CPP" %>
<asp:Label ID="lblDesignationId" runat="server" Visible="false" Text=""></asp:Label>
<asp:Label ID="lblUsersId" runat="server" Visible="false" Text=""></asp:Label>
<asp:Label ID="lbltabHeight5" runat="server" CssClass="lbltabHeight5 hidden" Text="178"></asp:Label>
<style type="text/css">
	.AuthorityHead {
		color: #1abe9e;
	}

	.chklabel label {
		color: transparent;
		padding-left: 0px !important;
	}

	.selecttd {
		text-align: center;
		width: 150px;
	}
</style>
<asp:Panel ID="pnlDesignationAuthority" runat="server">
	<div class="page-header clearfix">
		<ol class="breadcrumb mb-0">
			<li><a href="Default.aspx">Home</a></li>
			<li id="liDesignation" runat="server">
				<asp:LinkButton ID="lnkParentPage" OnClientClick="addRegionLoader('divloaderauthority')"
					OnClick="lnkParentPage_OnClick" runat="server">Designation</asp:LinkButton></li>
			<li id="liDesignationName" runat="server" class="active">
				<asp:Literal ID="ltrParentPageName" runat="server"></asp:Literal></li>
		</ol>
	</div>
	<div class="page-content col-md-12 p0 divloaderauthority">
		<div id="divData" runat="server">
			<div class="table-responsive divtable divdesignationauthority tabHeight5">
				<table class="table table-bordered table-hover nomargin fixheader">
					<thead>
						<tr>
							<th>Authority
							</th>
							<th class="selecttd">
								<div class="checkbox-custom">
									<asp:CheckBox ID="chkAllView" CssClass="chkAllView" onchange="SelectAll('chkAllView', 'chkView')"
										Text="View" runat="server" />
								</div>
							</th>
							<th class="selecttd">
								<div class="checkbox-custom">
									<asp:CheckBox ID="chkAllAddEdit" CssClass="chkAllAddEdit" onchange="SelectAll('chkAllAddEdit', 'chkAddEdit')"
										Text="Add / Edit" runat="server" />
								</div>
							</th>
							<th class="selecttd">
								<div class="checkbox-custom">
									<asp:CheckBox ID="chkAllDelete" CssClass="chkAllDelete" onchange="SelectAll('chkAllDelete', 'chkDelete')"
										Text="Delete" runat="server" />
								</div>
							</th>
						</tr>
					</thead>
					<tbody>
						<asp:Repeater ID="rptAuthority" runat="server" OnItemDataBound="rptAuthority_OnItemDataBound">
							<ItemTemplate>
								<tr>
									<td id="tdAuthority" runat="server">
										<asp:Label ID="lblAuthorityId" runat="server" Visible="false" Text="0"></asp:Label>
										<asp:Label ID="lblAuthorityName" runat="server" CssClass="" Text="Authority Name"></asp:Label>
										<div class="checkbox-custom chklabel pull-right mr-25">
											<asp:CheckBox ID="chkAllRow" AuthorityId='<%# Eval("AuthorityId") %>' CssClass='<%# "chkAllRow chkAllRow" + Eval("AuthorityId") %>'
												Text="A" runat="server" />
										</div>
									</td>
									<td id="tdView" runat="server" class="selecttd">
										<div class="checkbox-custom chklabel">
											<asp:CheckBox ID="chkView" AuthorityId='<%# Eval("AuthorityId") %>' CssClass='<%# "chkView chkView" + Eval("AuthorityId") %>'
												onchange="SelectAllCheck('chkAllView', 'chkView')" Text="V" runat="server" />
										</div>
									</td>
									<td id="tdAddEdit" runat="server" class="selecttd">
										<div class="checkbox-custom chklabel">
											<asp:CheckBox ID="chkAddEdit" AuthorityId='<%# Eval("AuthorityId") %>' CssClass='<%# "chkAddEdit chkAddEdit" + Eval("AuthorityId") %>'
												onchange="SelectAllCheck('chkAllAddEdit', 'chkAddEdit')" Text="A" runat="server" />
											<asp:CheckBox ID="chkFackAddEdit" Visible="false" Enabled="false" Text="A" runat="server" />
										</div>
									</td>
									<td id="tdDelete" runat="server" class="selecttd">
										<div class="checkbox-custom chklabel">
											<asp:CheckBox ID="chkDelete" AuthorityId='<%# Eval("AuthorityId") %>' CssClass='<%# "chkDelete chkDelete" + Eval("AuthorityId") %>'
												onchange="SelectAllCheck('chkAllDelete', 'chkDelete')" Text="D" runat="server" />
											<asp:CheckBox ID="chkFackDelete" Visible="false" Enabled="false" Text="D" runat="server" />
										</div>
									</td>
								</tr>
							</ItemTemplate>
						</asp:Repeater>
					</tbody>
				</table>
			</div>
			<div class="panel-footer text-right">
				<asp:Button ID="btnCancelAuthority" OnClick="btnCancelAuthority_OnClick" runat="server"
					CssClass="btn btn-raised btn-default clickloader" Text="Cancel" />
				<asp:Button ID="btnResetAuthority" OnClick="btnResetAuthority_OnClick" runat="server"
					CssClass="btn btn-raised btn-default clickloader" Text="Reset" />
				<asp:Button ID="btnSaveAuthority" OnClick="btnSaveAuthority_OnClick" runat="server"
					CssClass="btn btn-raised btn-black clickloader" Text="Save" />
			</div>
		</div>
		<div id="divNoData" runat="server" class="text-center text-danger">
			<br />
			<i class="fa fa-4x fa-smile-o"></i>
			<h3>Sorry, No Data Found.</h3>
		</div>
	</div>
</asp:Panel>
<cc1:ModalPopupExtender ID="popupConfirmChangeAuthority" runat="server" DropShadow="false"
	PopupControlID="pnlConfirmChangeAuthority" BehaviorID="PopupBehaviorID2" TargetControlID="lnkFackConfirmChangeAuthority"
	BackgroundCssClass="modalBackground">
</cc1:ModalPopupExtender>
<asp:LinkButton ID="lnkFackConfirmChangeAuthority" runat="server"></asp:LinkButton>
<asp:Panel ID="pnlConfirmChangeAuthority" CssClass="modal-content zoomIn modal-confirmation col-xs-12 col-sm-12 col-md-12 p0"
	Style="display: none" runat="server">
	<CPP:ConfirmPopup ID="ConfirmPopupChangeAuthority" runat="server" />
</asp:Panel>
