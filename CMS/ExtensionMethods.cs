using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace System
{
	#region IPrincipal

	public static partial class ExtensionMethods
	{
		public static bool IsInAnyRoleOrEmpty(this System.Security.Principal.IPrincipal principal, params object[] roles)
		{
			return principal.IsInAnyRoleOrEmpty((IList)roles);
		}

		public static bool IsInAnyRoleOrEmpty(this System.Security.Principal.IPrincipal principal, System.Collections.IList roles)
		{
			if (roles.Count == 0)
			{
				return true;
			}
			foreach (string role in roles)
			{
				if (principal.IsInRole(role) || role == "*")
				{
					return true;
				}
			}
			return false;
		}
	}

	#endregion
}

namespace System.Web
{
	public static partial class ExtensionMethods
	{
		public static string Title(this SiteMapNode node, string formatString)
		{
			if (string.IsNullOrEmpty(formatString))
			{
				formatString = "{0}";
			}
			while (node != null)
			{
				if (!string.IsNullOrEmpty(node.Title))
				{
					formatString = string.Format(formatString, node.Title.Replace("#", "{0}"));
				}
				node = node.ParentNode;
			}
			return formatString;
		}

		public static string Description(this SiteMapNode node, string formatString)
		{
			if (string.IsNullOrEmpty(formatString))
			{
				formatString = "{0}";
			}
			while (node != null)
			{
				if (!string.IsNullOrEmpty(node.Description))
				{
					formatString = string.Format(formatString, node.Description.Replace("#", "{0}"));
				}
				node = node.ParentNode;
			}
			return formatString;
		}

		public static string Keywords(this SiteMapNode node, string formatString)
		{
			if (string.IsNullOrEmpty(formatString))
			{
				formatString = "{0}";
			}
			while (node != null)
			{
				if (!string.IsNullOrEmpty(node["keywords"]))
				{
					formatString = string.Format(formatString, node["keywords"].Replace("#", "{0}"));
				}
				node = node.ParentNode;
			}
			return formatString;
		}
	}
}
