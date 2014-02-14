<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminBar.ascx.cs" Inherits="CMS.AdminBar" %>

<div class="CMS">
	<div style="float:left;margin:5px 2px 0 7px;font-size:75%">
		<asp:LinkButton runat="server" ID="btnFindProvider" Text="Sitemap?" OnClick="btnFindProvider_Click" ToolTip="Find the sitemap that contains the current page." />
	</div>
	<div style="float:left;margin:0px 7px 0 3px">
		<asp:DropDownList runat="server" ID="ddlSiteMapProvider" OnSelectedIndexChanged="ddlSiteMapProvider_OnSelectedIndexChanged" AutoPostBack="true" Font-Size="8" ToolTip="The sitemap that is used by the buttons next to this list." />
	</div>
	<div style="float:left;margin:3px 7px 0">
		<asp:LinkButton runat="server" ID="btnNavigation" Text="Navigate" OnClick="btnNavigation_Click" ToolTip="Navigate to a page"/>
	</div>
	<div style="float:left;margin:3px 7px 0">
		<asp:LinkButton runat="server" ID="btnProperties" Text="Properties" OnClick="btnProperties_Click" ToolTip="Properties of the current page" />
	</div>
	<div style="float:left;margin:3px 7px 0">
		<asp:LinkButton runat="server" ID="btnAddNew" Text="Add new" OnClick="btnAddNew_Click" ToolTip="Add a new page" />
	</div>
	<div style="float:left;margin:3px 7px 0">
		<asp:LinkButton runat="server" ID="btnDelete" Text="Delete" OnClick="btnDelete_Click" ToolTip="Delete the current page" />
	</div>
	<div style="float:left;margin:3px 7px 0">
		<asp:LinkButton runat="server" ID="btnMove" Text="Move" OnClick="btnMove_Click" ToolTip="Move a page" />
	</div>

	<div style="float:right;margin:0 7px">
		<asp:CheckBox runat="server" ID="chkEditMode" Text="EditMode" AutoPostBack="true" OnCheckedChanged="chkEditMode_CheckedChanged" EnableViewState="true" ToolTip="Switch to/from edit mode" />
	</div>

	<div>
		<asp:Panel runat="server" ID="pnlNavigation" Visible="false" CssClass="tree">
			<cms:Menu runat="server" ID="mnuNavigation" ContainerCssClass="navigationTree" 
				RedirectDisabled="true" ExcludeStartingNode="false" EnableSecurityTrimming="false" RenderAsText="false" UsePageTitleWhenMissingMenuTitle="true" />
		</asp:Panel>

		<asp:Panel runat="server" ID="pnlMove" Visible="false" CssClass="tree">
			<cms:Menu runat="server" ID="mnuMove" ContainerCssClass="simpleTree" RootCssClass="root" 
				ExcludeStartingNode="false" EnableSecurityTrimming="false" RenderAsText="true" UsePageTitleWhenMissingMenuTitle="true" 
				AutoIdAttribute="treeid" />
			<div class="buttons">
				<asp:Button runat="server" Text="Undo" />
				<asp:Button runat="server" Text="Save" OnClientClick="saveTree();return false;" />
			</div>
			<script type='text/javascript'>
				var simpleTreeCollection;
				jQuery(document).ready(function () {
					simpleTreeCollection = jQuery('.simpleTree').simpleTree({
						autoclose: true,
						afterClick: function (node) { }, //alert("text-"+$('span:first',node).text());},
						afterDblClick: function (node) { }, //alert("text-"+$('span:first',node).text());},
						afterMove: function (destination, source, pos) { }, //alert("destination-"+$('span:first',destination).text()+" source-"+$('span:first',source).text()+" pos-"+pos);},
						afterAjax: function () { }, //alert('Loaded');},
						animate: true,
						docToFolderConvert: true
					});
				});
			</script>
		</asp:Panel>
<script type="text/javascript">
function saveTree() {
	jQuery.post(
	'/CMS/OnSaveMovedTree.aspx',
	{ tree: escapeHTML(simpleTreeCollection[0].innerHTML) },
	function(data, textStatus) { if (data != '') alert(data); else document.location = document.location; },
	'text');
}
</script>
		<asp:Panel runat="server" ID="pnlProperties" Visible="false">
			<div class="properties">
				<p>
					<asp:Label Text="Cross language code:" AssociatedControlID="CodeTextBox" runat="server" />
					<asp:TextBox ID="CodeTextBox" runat="server" CssClass="propertiestextbox" />
				</p>
				<p>
					<asp:Label Text="Title (required):" AssociatedControlID="PageTitleTextBox" runat="server" />
					<asp:TextBox ID="PageTitleTextBox" runat="server" CssClass="propertiestextbox" />
				</p>
				<p>
					<asp:Label Text="Menu title: (leave blank to hide from menu)" AssociatedControlID="PageMenuTitleTextBox" runat="server" />
					<asp:TextBox ID="PageMenuTitleTextBox" runat="server" CssClass="propertiestextbox" />
				</p>
				<p>
					<asp:Label Text="Url:" AssociatedControlID="PageUrlTextBox" runat="server" />
					<asp:TextBox ID="PageUrlTextBox" runat="server" CssClass="propertiestextbox" />
				</p>
				<p>
					<asp:Label Text="Description:" AssociatedControlID="PageDescriptionTextBox" runat="server" />
					<asp:TextBox ID="PageDescriptionTextBox" runat="server" TextMode="MultiLine" Height="100" CssClass="propertiestextbox" />
				</p>
				<p>
					<asp:Label Text="Keywords (separated by commas, max. 20):" AssociatedControlID="PageKeywordsTextBox" runat="server" />
					<asp:TextBox ID="PageKeywordsTextBox" runat="server" TextMode="MultiLine" Height="50" CssClass="propertiestextbox" />
				</p>
				<p>
					<asp:Label Text="Template:" AssociatedControlID="TemplateDropDownList" runat="server" />
					<asp:DropDownList runat="server" ID="TemplateDropDownList" Width="99%">
						<asp:ListItem Value="none" Text="None (same as url)" />
						<asp:ListItem Value="/HashInMenu.aspx" Text="No target page (#)" />
						<asp:ListItem Value="/ArticleTemplate.aspx" Text="Article" />
						<asp:ListItem Value="custom" Selected="true" Text="Custom" />
					</asp:DropDownList>
				</p>
				<p>
					<asp:Label Text="Template name (.aspx, readonly when not custom template):" AssociatedControlID="CustomTemplateTextBox" runat="server" />
					<asp:TextBox ID="CustomTemplateTextBox" runat="server" CssClass="propertiestextbox" />
				</p>
				<p>
					<asp:CheckBox ID="RedirectToFirstChildCheckBox" runat="server" CssClass="propertiestextbox" Text="Redirect to first child" />
				</p>
				<p>
					<asp:Label Text="Redirect:" AssociatedControlID="RedirectTextBox" runat="server" />
					<asp:TextBox ID="RedirectTextBox" runat="server" CssClass="propertiestextbox" />
				</p>
				<p>
					<asp:Label Text="Authentication:" AssociatedControlID="AuthenticationRadioButtonList" runat="server" />
					<asp:RadioButtonList ID="AuthenticationRadioButtonList" runat="server" RepeatDirection="Horizontal" Width="60%">
						<asp:ListItem Text="Anonymous" /> 
						<asp:ListItem Text="Authenticated" /> 
						<asp:ListItem Text="Both" /> 
					</asp:RadioButtonList>
				</p>
				<p>
					<asp:Label Text="Roles:" AssociatedControlID="RolesCheckBoxList" runat="server" />
					<asp:CheckBoxList ID="RolesCheckBoxList" runat="server" Width="97%" />
				</p>
				<div class="buttons">
					<asp:Button runat="server" ID="btnSave" OnClick="btnSave_Click" Text="Save" />
				</div>
			</div>
		</asp:Panel>
	</div>
		
</div>
