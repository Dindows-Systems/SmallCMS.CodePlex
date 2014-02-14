using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using Website.Properties;

namespace CMS
{
	public class SiteMapManager : XmlSiteMapProvider
	{
		public static XNamespace xn = "http://schemas.microsoft.com/AspNet/SiteMap-File-1.0";

		public string Language { get; private set; }

		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection attributes)
		{
			Language = attributes["language"];
			attributes.Remove("language");
			base.Initialize(name, attributes);
		}

		public override SiteMapNode CurrentNode
		{
			get
			{
				return FindNode();
			}
		}

		public string FileName
		{
			get
			{
				if (HttpContext.Current == null || string.IsNullOrEmpty(ResourceKey))
				{
					// For test only
					return @"D:\Solutions\SmallCMS\Web.sitemap";
				}
				return HttpContext.Current.Server.MapPath(ResourceKey);
			}
		}

		public XDocument SiteMapFile
		{
			get
			{
				return XDocument.Load(FileName);
			}
			set
			{
				string filename = FileName;
				Backup(filename);
				value.Save(filename);
			}
		}

		public static SiteMapNode FindNode()
		{
			return FindNode(HttpContext.Current.Request.RawUrl);
		}

		public static SiteMapNode FindNode(string url)
		{
			// HACK for the home page on IIS6
			if (url.Equals("/", StringComparison.InvariantCultureIgnoreCase))
			{
				url = "/default.aspx";
			}

			int index = url.IndexOf('?');
			if (index > 0)
			{
				url = url.Substring(0, index);
			}

			// Try all providers
			foreach (SiteMapManager smp in SiteMap.Providers)
			{
				if (string.IsNullOrEmpty(smp.Language) 
					|| smp.Language.Equals(Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
				{
					SiteMapNode node2 = smp.FindSiteMapNode(url);
					if (node2 != null)
					{
						return node2;
					}
				}
			}
			return null;
		}

		internal static SiteMapNode FindNodeByAttribute(string attribute, string value, string language)
		{
			// Try all providers
			foreach (SiteMapManager smp in SiteMap.Providers)
			{
				if (string.IsNullOrEmpty(smp.Language)
					|| smp.Language.Equals(language, StringComparison.OrdinalIgnoreCase))
				{
					foreach (SiteMapNode node in smp.RootNode.GetAllNodes())
					{
						string attributevalue = node[attribute];
						if (!string.IsNullOrEmpty(attributevalue) && attributevalue.Equals(value, StringComparison.OrdinalIgnoreCase))
						{
							return node;
						}
					}
				}
			}
			return null;
		}

		public static string FileNameFromNode(SiteMapNode node)
		{
			return HttpContext.Current.Server.MapPath("\\" + ((SiteMapManager)node.Provider).ResourceKey);
		}

		public static string FileNameFromProviderName(string provider)
		{
			return HttpContext.Current.Server.MapPath("\\" + SiteMap.Providers[provider].ResourceKey);
		}

		public static void UpdateMovedTree(string siteMapManagerName, XDocument newxdoc)
		{
			// Load
			string filename = FileNameFromProviderName(siteMapManagerName);
			XDocument xdoc = XDocument.Load(filename);

			foreach (XElement newelement in newxdoc.Descendants(xn + "siteMapNode"))
			{
				string id = newelement.Attribute("treeid").Value;
				XElement xel = ((XElement)xdoc.DescendantNodes().ElementAt(int.Parse(id)));
				newelement.Attribute("treeid").Remove();
				IEnumerable<XAttribute> attributes = xel.Attributes();
				newelement.Add(attributes);
			}

			// Save
			Backup(filename);
			newxdoc.Save(filename);
		}

		public static void UpdateAttributes(string url, Dictionary<string, string> attributes, bool redirect)
		{
			SiteMapNode node = FindNode(url);
			if (node == null)
			{
				throw new ApplicationException(string.Format("The url '{0}' was not found in any sitemap.", url));
			}
			UpdateAttributes(node, attributes, redirect);
		}

		public static void UpdateAttributes(SiteMapNode node, Dictionary<string, string> attributes, bool redirect)
		{
			// Load
			string filename = FileNameFromNode(node);
			XDocument xdoc = XDocument.Load(filename);

			// Update or create 
			string url = (node != null ? node.Url : HttpContext.Current.Request.UrlReferrer.AbsolutePath);
			XElement element = xdoc.Descendants(xn + "siteMapNode").FirstOrDefault(x => x.Attribute("url").Value == url);
			if (element == null)
			{
				throw new ArgumentException(string.Format("Element {0} is not in the sitemap.", url));
			}
			foreach (KeyValuePair<string, string> kv in attributes)
			{
				XAttribute attribute = element.Attribute(kv.Key);
				if (attribute == null)
				{
					if (kv.Value != null)
					{
						// Add attribute
						attribute = new XAttribute(kv.Key, kv.Value);
						element.Add(attribute);
					}
					else
					{
						// No action
					}
				}
				else
				{
					if (kv.Value != null)
					{
						// Update attribute
						attribute.Value = kv.Value;
					}
					else
					{
						// Remove attribute
						attribute.Remove();
					}
				}
				// Save
				Backup(filename);
				xdoc.Save(filename);
				
				// Redirect
				if (redirect)
				{
					if (attributes.ContainsKey("url"))
					{
						HttpContext.Current.Response.Redirect(attributes["url"]);
					}
					else
					{
						HttpContext.Current.Response.Redirect(url);
					}
				}
			}
		}

		internal static void AddNew(SiteMapNode node)
		{
			// Load
			string filename = FileNameFromNode(node);
			XDocument xdoc = XDocument.Load(filename);

			string url = node.Url;
			XNamespace xn = "http://schemas.microsoft.com/AspNet/SiteMap-File-1.0";
			XElement element = xdoc.Descendants(xn + "siteMapNode").FirstOrDefault(x => x.Attribute("url").Value == url);
			string newUrl = string.Format("/{0:yyyyMMddHHmmss}.aspx", DateTime.Now);
			if (element != null)
			{
				element.Parent.Add(
					new XElement(xn + "siteMapNode",
						new XAttribute("url", newUrl),
						new XAttribute("title", string.Format("New page {0:yyyy-MM-dd HH:mm:ss}", DateTime.Now)),
						new XAttribute("menutitle", "New page"),
						new XAttribute("template", "/ArticleTemplate.aspx"),
						new XAttribute("state", "new"),
						new XAttribute("roles", Settings.Default.CMS_Role)
					)
				);
			}

			// Save
			Backup(filename);
			xdoc.Save(filename);

			// Redirect
			HttpContext.Current.Response.Redirect(newUrl);
		}

		internal static void Delete(SiteMapNode node)
		{
			// Load
			string filename = FileNameFromNode(node);
			XDocument xdoc = XDocument.Load(filename);

			string url = node.Url;
			XNamespace xn = "http://schemas.microsoft.com/AspNet/SiteMap-File-1.0";
			XElement element = xdoc.Descendants(xn + "siteMapNode").FirstOrDefault(x => x.Attribute("url").Value == url);
			if (element != null)
			{
				element.Remove();
			}

			// Save
			Backup(filename);
			xdoc.Save(filename);

			// Redirect
			HttpContext.Current.Response.Redirect("/");
		}

		public class SearchResult
		{
			public double Rank { get; set; }
			public string Url { get; set; }
			public string Title { get; set; }
			public string Description { get; set; }
		}

		internal void TestSearch()
		{
			int total;
			var result = Search("lorem ipsum small cms", 3, 1, out total);
		}

		internal List<SearchResult> Search(string keywords, int pageSize, int currentPage, out int total)
		{
			// Load
			XDocument xdoc = SiteMapFile;
			XNamespace xn = "http://schemas.microsoft.com/AspNet/SiteMap-File-1.0";

			var qry = from element in xdoc.Descendants(xn + "siteMapNode")
					  from keyword in Regex.Split(keywords, @"\s+")
					  let rowrank = element.Attributes().Sum
					  (
						  delegate(XAttribute attr)
						  {
							  int sum = 0;
							  foreach (Match match in Regex.Matches(attr.Value, @"\b" + keyword, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline))
							  {
								  sum += match.Index;
							  }
							  return sum;
						  }
					  )
					  where rowrank > 0
					  group rowrank by element into grp
					  let rank = grp.Sum(x => 100.0 / (x + 100))
					  orderby rank descending
					  select new SearchResult
					  {
						  Rank = rank,
						  Url = grp.Key.Attribute("url").Value,
						  Title = grp.Key.Attribute("title").Value,
						  Description = grp.Key.Attribute("description").Value
					  };
		
			total = qry.Count();
			List<SearchResult> result = qry.Skip(pageSize * (currentPage - 1)).Take(pageSize).ToList();
			return result;
		}

		public static void Backup(string filename)
		{
			string backup = string.Format(@"{0}\App_Data\Backup\{1}.{2:yyyyMMdd.HHmmss}", 
				Path.GetDirectoryName(filename), 
				Path.GetFileName(filename),
				DateTime.Now);
			File.Copy(filename, backup);
		}

	}
}
