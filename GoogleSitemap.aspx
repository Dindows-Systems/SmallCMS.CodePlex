<%@ Page Language="C#" Inherits="System.Web.UI.Page" %>

<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Xml.Linq" %>
<%@ Import Namespace="System.Data" %>
<%@ Assembly Name="System.Data.Linq, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" %>
<%@ Import Namespace="Website" %>

<script runat="server" type="text/C#">

	protected void Page_Load(object sender, EventArgs e)
	{
		Response.ContentType = "application/xml";

		XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
		XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

		XElement root =
			new XElement(xmlns + "urlset",
				new XAttribute(XNamespace.Xmlns + "xsi", xsi),
				new XAttribute(xsi + "schemaLocation", xmlns),
			// Sitemaps
				from CMS.SiteMapManager provider in SiteMap.Providers
				from SiteMapNode node in provider.RootNode.GetAllNodes()
				where !string.IsNullOrEmpty(node["menutitle"])
				&& (string.IsNullOrEmpty(node["authentication"])
					|| node["authentication"].Equals("both", StringComparison.OrdinalIgnoreCase)
					|| node["authentication"].Equals("anonymous", StringComparison.OrdinalIgnoreCase)
					)
				select
				new XElement(xmlns + "url",
					new XElement(xmlns + "loc", FormatUrl(provider.Language, node.Url)),
					new XElement(xmlns + "lastmod", (node["lastmod"] ?? DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm")) + "+00:00")
				)
			);

		root.Save(new System.IO.StreamWriter(Response.OutputStream, System.Text.UTF8Encoding.UTF8));
	}

	private string FormatUrl(string language, object url)
	{
		return string.Format("{0}://{1}{2}{3}",
			Request.Url.Scheme,
			Request.Url.Host + (Request.Url.Port != 80 ? ":" + Request.Url.Port : ""),
			url,
			(string.IsNullOrEmpty(language) ? "" : "?language=" + language));
	}

</script>

