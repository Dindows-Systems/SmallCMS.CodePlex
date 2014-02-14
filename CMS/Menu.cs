using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Threading;

namespace CMS
{
	public class Menu : Control
	{
		public enum IndentMode
		{
			Indent = 0,
			List = 1
		}

		public bool RedirectDisabled { get; set; }
		public string SiteMapProviderName { get; set; }

		public string ItemSeparator { get; set; }

		public bool ExcludeStartingNode { get; set; }

		public bool EnableSecurityTrimming { get; set; }

		public string HashInMenuTemplate { get; set; }

		public bool RenderAsText { get; set; }

		public string ContainerCssClass { get; set; }

		public string RootCssClass { get; set; }

		public string AutoIdAttribute { get; set; }

		private int autoId = 1;

		public bool UsePageTitleWhenMissingMenuTitle { get; set; }

		public int? MaxDepth { get; set; }

		public string StartingNodeUrl { get; set; }

		public string SelectedCssClass { get; set; }

		public string DeselectedCssClass { get; set; }

		private SiteMapNode selectedNode;

		public IndentMode Indent { get; set; }

		protected override void CreateChildControls()
		{
			SiteMapProvider provider;
			if (string.IsNullOrEmpty(SiteMapProviderName))
			{
				// Default provider
				provider = SiteMap.Provider;
			}
			else
			{
				string name = string.Format(SiteMapProviderName, Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);
				provider = SiteMap.Providers[name];
				if (provider == null)
				{
					throw new ApplicationException(string.Format("SiteMapProvider '{0}' could not be found.", name));
				}
			}

			selectedNode = provider.CurrentNode;

			SiteMapNode root = provider.RootNode;
			if (!string.IsNullOrEmpty(StartingNodeUrl))
			{
				if (StartingNodeUrl == ".")
				{
					root = provider.CurrentNode;
				}
				else if (StartingNodeUrl == "..")
				{
					root = provider.CurrentNode.ParentNode;
				}
				else
				{
					root = root.Provider.FindSiteMapNodeFromKey(StartingNodeUrl);
				}
				if (root == null)
				{
					throw new ArgumentException("StartingNodeUrl is not valid.", StartingNodeUrl);
				}
			}
			if (ExcludeStartingNode)
			{
				CreateNodes(root.ChildNodes, this, ContainerCssClass, 1);
			}
			else
			{
				HtmlGenericControl ul = new HtmlGenericControl("ul");
				if (!string.IsNullOrEmpty(ContainerCssClass))
				{
					ul.Attributes["class"] = ContainerCssClass;
				}
				Controls.Add(ul);
				CreateNode(root, ul, RootCssClass, 1);
			}
		}

		private void CreateNode(SiteMapNode node, Control container, string rootCss, int depth)
		{
			//string authentication = (node["Authentication"] ?? "both").ToLower();

			//if (EnableSecurityTrimming
			//    && !(authentication == "both"
			//        || (!Thread.CurrentPrincipal.Identity.IsAuthenticated && authentication == "anonymous")
			//        || (Thread.CurrentPrincipal.Identity.IsAuthenticated && authentication == "authenticated")
			//        )
			//    )
			//{
			//    return;
			//}

			if (EnableSecurityTrimming 
				&& !Thread.CurrentPrincipal.IsInAnyRoleOrEmpty(node.Roles)
				|| (string.IsNullOrEmpty(node["menutitle"]) && !UsePageTitleWhenMissingMenuTitle))
			{
				return;
			}

			HtmlGenericControl li = new HtmlGenericControl("li");
			if (!string.IsNullOrEmpty(rootCss))
			{
				li.Attributes["class"] = rootCss;
			}
			if (!string.IsNullOrEmpty(AutoIdAttribute))
			{
				li.Attributes[AutoIdAttribute] = autoId++.ToString();
			}
			if (node == selectedNode)
			{
				if (!string.IsNullOrEmpty(SelectedCssClass))
				{
					li.Attributes.Add("class", SelectedCssClass);
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(DeselectedCssClass))
				{
					li.Attributes.Add("class", DeselectedCssClass);
				}
			}
			container.Controls.Add(li);

			string title = node["menutitle"];
			if (string.IsNullOrEmpty(title) && UsePageTitleWhenMissingMenuTitle)
			{
				title = node.Title;
			}

			string url = string.Empty;
			string redirect = string.Empty;
			if (!string.IsNullOrEmpty(node["redirecttofirstchild"]) && node["redirecttofirstchild"].Equals("true", StringComparison.InvariantCultureIgnoreCase) && node.HasChildNodes)
			{
				redirect = node.ChildNodes[0].Url;
			}
			else
			{
				redirect = node["redirect"];
			}
			if (!RedirectDisabled && !string.IsNullOrEmpty(redirect))
			{
				url = redirect;
			}
			else
			{
				string template = node["template"];
				url =
					 !string.IsNullOrEmpty(HashInMenuTemplate)
					 && HashInMenuTemplate.Equals(template, StringComparison.OrdinalIgnoreCase)
					 ? "#"
					 : node.Url;
			}
			if (RedirectDisabled && !string.IsNullOrEmpty(redirect))
			{
				url += (url.IndexOf('?') == -1 ? "?" : "&") + "redirect=disabled";
			}

			if (RenderAsText)
			{
				li.Controls.Add(new LiteralControl(string.Format("<span>{0}</span>", HttpUtility.HtmlEncode(title ?? node.Title))));
			}
			else
			{
				li.Controls.Add(new LiteralControl(string.Format("<a href='{0}'>{1}</a>", url, HttpUtility.HtmlEncode(title ?? node.Title))));
			}
			if (!string.IsNullOrEmpty(ItemSeparator) && node.NextSibling != null)
			{
				li.Controls.Add(new LiteralControl("<span>" + HttpUtility.HtmlEncode(ItemSeparator) + "</span>"));
			}

			if (node.HasChildNodes && (!MaxDepth.HasValue || depth < MaxDepth.Value))
			{
				CreateNodes(node.ChildNodes, li, null, depth + 1);
			}
		}

		private void CreateNodes(SiteMapNodeCollection nodes, Control container, string containerCss, int depth)
		{
			if (Indent == IndentMode.Indent || container.Controls.Count == 0)
			{
				HtmlGenericControl ul = new HtmlGenericControl("ul");
				if (!string.IsNullOrEmpty(containerCss))
				{
					ul.Attributes["class"] = containerCss;
				}
				container.Controls.Add(ul);
				foreach (SiteMapNode child in nodes)
				{
					CreateNode(child, ul, null, depth);
				}
			}
			else
			{
				foreach (SiteMapNode child in nodes)
				{
					CreateNode(child, container, null, depth);
				}
			}
		}
	}
}
