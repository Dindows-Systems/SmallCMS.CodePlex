using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace CMS
{
	public partial class OnSaveMovedTree : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				string tree = HttpUtility.HtmlDecode(Request["tree"]).ToLower();

				MatchCollection matches = Regex.Matches(tree, @"<(?<1>li)\s | treeid=['""](?<1>\d+)['""] | <(?<1>/li)>", 
					RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.CultureInvariant);

				XNamespace xn = "http://schemas.microsoft.com/AspNet/SiteMap-File-1.0";
				XElement root = new XElement(xn + "siteMap");
				XDocument xdoc = new XDocument(root);
				XElement element = null;
				StringBuilder sb = new StringBuilder();
				int indent = 0;
				foreach (Match match in matches)
				{
					string val = match.Groups[1].Value;
					switch (val)
					{
						case "li":
							indent++;
							if (element != null && element.HasElements)
							{
								element = element.Elements().Last();
							}
							break;
						case "/li":
							indent--;
							if (element.Parent != null)
							{
								element = element.Parent;
							}
							break;
						default:
							sb.Append("".PadRight(indent, '\t') + val + "\n");
							XElement lmnt = new XElement(xn + "siteMapNode", new XAttribute("treeid", val));
							if (element == null)
							{
								element = lmnt;
								root.Add(lmnt);
							}
							else
							{
								element.Add(lmnt);
							}
							break;
					}
				}

				SiteMapManager.UpdateMovedTree(CookieManager.GetCookieValue("CurrentSiteMapManager"), xdoc);
			}
			catch (Exception ex)
			{
				Response.Write(string.Format("Failed to save tree\n\n{0}", ex.Message));

			}
		}

	}
}
