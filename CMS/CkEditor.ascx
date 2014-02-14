<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CkEditor.ascx.cs" Inherits="CMS.CkEditor" %>

<asp:Panel runat="server" ID="pnl" EnableViewState="false" CssClass="htmleditor">
	<asp:Literal runat="server" ID="lit" EnableViewState="false" />
</asp:Panel>
<asp:PlaceHolder runat="server" ID="phScript">
<script type="text/javascript">
//<![CDATA[
jQuery(document).ready(function() {
	var editor = CKEDITOR.replace('<%= pnl.ClientID %>', {
		<%if(!string.IsNullOrEmpty(EditorHeight)){%>height: '<%= EditorHeight %>',<%}%>
		customConfig: '/Javascript/ckeditor_config.js'
	});
	editor.on('pluginsLoaded', function(ev) {
		// Register the command used to save.
		editor.addCommand('save', {exec: 
			function(editor) {
				jQuery.post('/CMS/OnSaveHtmlEditor.aspx', {
					content: escapeHTML(editor.getData()),
					id: '<%= ContentKey %>'
				},
				function (data, textStatus) { alert(data); }, 'text');
			}
		});
		// Connect CKFinder
		CKFinder.SetupCKEditor(editor) ;
	});
});
//]]>
</script>
</asp:PlaceHolder>