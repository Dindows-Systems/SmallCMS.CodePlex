<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GoogleAnalytics.ascx.cs" Inherits="CMS.GoogleAnalytics" EnableViewState="false" %>
<%@OutputCache Duration="86400" VaryByParam="none" %> <%-- duration in seconds, 86400 seconds = 24 hours --%>

<script type="text/javascript">
var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
</script>
<script type="text/javascript">
try {
	var pageTracker = _gat._getTracker("<asp:Literal runat="server" ID="ltlCode" />");
pageTracker._trackPageview();
} catch(err) {}</script>
