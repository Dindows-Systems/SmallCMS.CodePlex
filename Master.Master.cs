using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using CMS;

namespace Website
{
	public partial class Master : System.Web.UI.MasterPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Thread.CurrentPrincipal.IsInAnyRoleOrEmpty("Administrator", "Editor"))
			{
				CMS.AdminBar adminbar = LoadControl("~/CMS/AdminBar.ascx") as CMS.AdminBar;
				adminbar.ID = "adminbar";
				phAdminBar.Controls.Add(adminbar);
			}
		}

		protected void btnLanguage_Click(object sender, EventArgs e)
		{
			LinkButton btn = (LinkButton)sender;

			string url = "/";
			string code = SiteMap.CurrentNode["code"];
			if (!string.IsNullOrEmpty(code))
			{
				SiteMapNode node = SiteMapManager.FindNodeByAttribute("code", code, btn.CommandArgument);
				if (node != null)
				{
					// Redirect to the language equivalent page, if it exists
					url = node.Url;
				}
			}
			// Create url
			url = string.Format("{0}://{1}{2}{3}{4}",
				Request.Url.Scheme,
				Request.Url.Host + (Request.Url.Port != 80 ? ":" + Request.Url.Port : ""),
				url,
				(url.IndexOf('?') == -1 ? "?language=" : "&language="),
				btn.CommandArgument);

			// Change cookie
			CookieManager.SetCookieValue("Language", btn.CommandArgument);

			// Redirect
			Response.Redirect(url);
		}

		private void RenderTitleDescriptionAndKeywords()
		{
			SiteMapNode node = SiteMapManager.FindNode();
			if (node != null)
			{
				Page.Title = node.Title;
				litMetaDescription.Text = string.Format("\n\t<meta name='description' content='{0}' />", node.Description);
				litMetaKeywords.Text = string.Format("\n\t<meta name='keywords' content='{0}' />", node["keywords"]);
			}
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			RenderTitleDescriptionAndKeywords();

			if (Request.Browser.MSDomVersion.Major == 0)
			{
				// If it is Non IE Browser
				Response.Cache.SetNoStore();
			}

			litMetaContentLanguage.Text = string.Format("\n\t<meta http-equiv='content-language' content='{0}'/>",
				System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);

			btnEN.CssClass +=
				(System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals(btnEN.CommandArgument, StringComparison.InvariantCultureIgnoreCase)
				? " active"
				: " inactive");

			btnNL.CssClass +=
				(System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals(btnNL.CommandArgument, StringComparison.InvariantCultureIgnoreCase)
				? " active"
				: " inactive");
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			Response.Redirect(Search.Url(txtKeywords.Text));
		}

	}
}

#region Extension Methods

namespace System.Web.UI
{
	public static partial class ExtensionMethods
	{
		public static void RegisterCssInclude(this Page page, string cssFile)
		{
			if (page != null)
			{
				MasterPage master = page.Master;
				while (master != null && !(master is Website.Master))
				{
					master = master.Master;
				}
				if (master != null)
				{
					master.FindControl("head").Controls.Add(new LiteralControl("\n\t\t<link href='" + cssFile + "' type='text/css' rel='stylesheet' />"));
				}
			}
		}
	}
}

#endregion

