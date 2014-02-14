using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Configuration;
using System.Web.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Security;
using System.Globalization;
using Website.Properties;

namespace CMS
{
	public class UrlRewriter : IHttpModule
	{
		private static Regex ignoreUrlRegex = new Regex(
			Settings.Default.UrlRewriter_IgnoreUrlRegex,
			RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

		public void Dispose()
		{
		}

		public void Init(HttpApplication context)
		{
			if (System.Web.SiteMap.Enabled)
			{
				context.PostAuthenticateRequest += new EventHandler(context_PostAuthenticateRequest);
			}
		}

		public string TemplatesRestrictedByRole { get; set; }

		private void ApplyCulture(HttpContext context)
		{
			// Get language from querystring, e.g. http://smallcms.net/xxx?language=en
			string language = context.Request.QueryString["language"];

			// Get Language from cookie
			if (string.IsNullOrEmpty(language))
			{
				language = CookieManager.GetCookieValue("Language");
			}

			// Get default language
			if (string.IsNullOrEmpty(language))
			{
				// Default language
				language = "en";
			}

			CultureInfo culture = System.Globalization.CultureInfo.CreateSpecificCulture(language);

			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;
		}

		void context_PostAuthenticateRequest(object sender, EventArgs e)
		{
			HttpContext context = ((System.Web.HttpApplication)sender).Context;

			if (ignoreUrlRegex.Match(context.Request.Path).Success)
			{
				return;
			}

			ApplyCulture(context);

			SiteMapNode node = SiteMapManager.FindNode(context.Request.Path);

			if (node != null && !string.IsNullOrEmpty(node.Key))
			{
				if (node.ParentNode == null)
				{
					string childUrl = node.ChildNodes[0].Url;
					if (childUrl != "/")
					{
						context.Response.Redirect(childUrl, true);
					}
				}

				// Redirect
				string redirect = string.Empty;
				if (!string.IsNullOrEmpty(node["redirecttofirstchild"]) && node["redirecttofirstchild"].Equals("true", StringComparison.InvariantCultureIgnoreCase) && node.HasChildNodes)
				{
					redirect = node.ChildNodes[0].Url;
				}
				else
				{
					redirect = node["redirect"];
				}
				if (!string.IsNullOrEmpty(redirect) && context.Request.QueryString["redirect"] != "disabled")
				{
					context.Response.Redirect(redirect);
				}

				// Authentication
				if (!System.Threading.Thread.CurrentPrincipal.IsInRole(Settings.Default.CMS_Role))
				{
					string authentication = (node["Authentication"] ?? "both").ToLower();
					if (!(authentication == "both"
							|| (!Thread.CurrentPrincipal.Identity.IsAuthenticated && authentication == "anonymous")
							|| (Thread.CurrentPrincipal.Identity.IsAuthenticated && authentication == "authenticated")
						)
						|| (!Thread.CurrentPrincipal.IsInAnyRoleOrEmpty(node.Roles))
						)
					{
						// Access denied
						context.Response.Redirect(FormsAuthentication.LoginUrl + "?aut=req&returnurl=" + context.Request.Url.PathAndQuery);
						context.Response.End();
					}
				}

				// Rewrite
				string template = node["template"];
				if (!string.IsNullOrEmpty(template) && template != node.Key)
				{
					// Rewrite
					string queryString = context.Request.QueryString.ToString();
					if (queryString.Length > 0)
					{
						template += (template.IndexOf('?') == -1 ? "?" : "&") + context.Request.QueryString.ToString();
					}
					//this.RaiseInfoEvent("Rewriting {0} to {1}", context.Request.Path, template);
					context.RewritePath(template, false);
					return;
				}
				return;
			}
			// Page not found
			context.Response.Clear();
			context.Response.Write("Page not found (404)");
			context.Response.StatusCode = 404;
			context.Response.End();
		}

	}
}

