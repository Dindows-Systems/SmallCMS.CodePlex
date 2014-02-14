using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Text;
using System.Threading;

namespace CMS
{
	public partial class AdminBar : System.Web.UI.UserControl
	{
		// Get current node
		SiteMapNode CurrentNode = SiteMapManager.FindNode();

		public SiteMapManager CurrentSiteMapManager
		{
			get
			{
				string smp = CookieManager.GetCookieValue("CurrentSiteMapManager");
				if (string.IsNullOrEmpty(smp))
				{
					return (SiteMapManager)SiteMap.Provider;
				}
				return SiteMap.Providers[smp] as SiteMapManager; 
			}
			set
			{
				if (value == null)
				{
					CookieManager.SetCookieValue("CurrentSiteMapManager", null);
				}
				else
				{
					CookieManager.SetCookieValue("CurrentSiteMapManager", value.Name);
				}
			}
		}

		protected void ddlSiteMapProvider_OnSelectedIndexChanged(object sender, EventArgs e)
		{
			CookieManager.SetCookieValue("CurrentSiteMapManager", ddlSiteMapProvider.SelectedValue);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			ScriptManager.RegisterClientScriptInclude(this, GetType(), "simpletree", "/Javascript/jquery.simple.tree.js");
			//Page.RegisterCssInclude("/CMS/CMS.css");

			if (!IsPostBack)
			{
				foreach (SiteMapManager smm in SiteMap.Providers)
				{
					ddlSiteMapProvider.Items.Add(new ListItem(smm.Description, smm.Name));
				}
				if (CurrentSiteMapManager != null)
				{
					ddlSiteMapProvider.SelectedValue = CurrentSiteMapManager.Name;
				}
			}
			ShowProperties(false);
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			// Navigation tree
			if (pnlNavigation.Visible)
			{
				//SiteMapDataSource src = new SiteMapDataSource { Provider = CurrentSiteMapManager };
				//trv.DataSource = src;
				//trv.DataBind();
			}

			// EditMode
			chkEditMode.Checked = (CookieManager.GetCookieValue("EditMode") == "1");

			// Show properties when state = new
			if (CurrentNode != null && CurrentNode["state"] == "new")
			{
				pnlProperties.Visible = true;
				//pnlNavigation.Visible = false;
			}

			btnNavigation.Enabled = (CurrentNode != null && CurrentNode["state"] != "new");
			btnAddNew.Enabled = (CurrentNode != null && CurrentNode["state"] != "new");
			btnDelete.Enabled = (CurrentNode != null);
			btnDelete.OnClientClick = (btnDelete.Enabled ? "javascript:return confirm('Are you sure you want to delete this page ?')" : "");
		}

		protected void btnFindProvider_Click(object sender, EventArgs e)
		{
			if (CurrentNode != null)
			{
				CurrentSiteMapManager = CurrentNode.Provider as SiteMapManager;
				ddlSiteMapProvider.SelectedValue = CurrentSiteMapManager.Name;
			}
		}

		protected void btnNavigation_Click(object sender, EventArgs e)
		{
			pnlNavigation.Visible = !pnlNavigation.Visible;
			if (pnlNavigation.Visible)
			{
				mnuNavigation.SiteMapProviderName = CurrentSiteMapManager.Name; 
				pnlMove.Visible = false;
				pnlProperties.Visible = false;
			}
		}

		protected void btnMove_Click(object sender, EventArgs e)
		{
			pnlMove.Visible = !pnlMove.Visible;
			if (pnlMove.Visible)
			{
				mnuMove.SiteMapProviderName = CurrentSiteMapManager.Name;
				pnlNavigation.Visible = false;
				pnlProperties.Visible = false;
				CookieManager.SetCookieValue("CurrentSiteMapManager", CurrentSiteMapManager.Name);
			}
		}

		protected void btnProperties_Click(object sender, EventArgs e)
		{
			pnlProperties.Visible = !pnlProperties.Visible;
			if (pnlProperties.Visible)
			{
				pnlMove.Visible = false;
				pnlNavigation.Visible = false;
			}
		}

		protected void btnAddNew_Click(object sender, EventArgs e)
		{
			if (CurrentNode.Key.Equals("/default.aspx", StringComparison.OrdinalIgnoreCase) 
				|| CurrentNode.Key.Equals("/default", StringComparison.OrdinalIgnoreCase))
			{
				SiteMapManager.AddNew(SiteMap.Provider.FindSiteMapNodeFromKey("/"));
			}
			else
			{
				SiteMapManager.AddNew(CurrentNode);
			}
		}

		protected void btnDelete_Click(object sender, EventArgs e)
		{
			SiteMapManager.Delete(CurrentNode);
		}

		protected void chkEditMode_CheckedChanged(object sender, EventArgs e)
		{
			CookieManager.SetCookieValue("EditMode", ((CheckBox)sender).Checked ? "1" : "0");
			Response.Redirect(HttpContext.Current.Request.RawUrl);
		}

		private void ShowProperties(bool isNew)
		{
			// Properties
			CodeTextBox.Text = (isNew || CurrentNode == null ? string.Empty : CurrentNode["Code"]);
			PageTitleTextBox.Text = (isNew || CurrentNode == null ? string.Empty : CurrentNode.Title);
			PageMenuTitleTextBox.Text = (isNew || CurrentNode == null ? string.Empty : CurrentNode["MenuTitle"]);
			PageUrlTextBox.Text = (isNew || CurrentNode == null ? string.Empty : CurrentNode.Url);
			PageDescriptionTextBox.Text = (isNew || CurrentNode == null ? string.Empty : CurrentNode.Description);
			PageKeywordsTextBox.Text = (isNew || CurrentNode == null ? string.Empty : CurrentNode["keywords"]);
			RedirectToFirstChildCheckBox.Checked = (isNew || (CurrentNode == null || CurrentNode["redirecttofirstchild"] == null) ? false : CurrentNode["redirecttofirstchild"].Equals("true", StringComparison.InvariantCultureIgnoreCase));
			RedirectTextBox.Text = (isNew || CurrentNode == null ? string.Empty : CurrentNode["redirect"]);

			// Template 
			if (CurrentNode == null)
			{
				TemplateDropDownList.SelectedIndex = -1;
			}
			else
			{
				if (TemplateDropDownList.Items.FindByValue(CurrentNode["Template"]) != null)
				{
					TemplateDropDownList.SelectedValue = CurrentNode["Template"];
				}
				else if (CurrentNode["Template"] == null || CurrentNode["Template"].Replace(".aspx", "").Equals(CurrentNode.Url.Replace(".aspx", ""), StringComparison.OrdinalIgnoreCase))
				{
					TemplateDropDownList.SelectedValue = "none"; // None (same as url)
				}
				CustomTemplateTextBox.Text = CurrentNode["Template"];
			}

			// Authentication
			if (CurrentNode != null)
			{
				AuthenticationRadioButtonList.SelectedValue = CurrentNode["Authentication"] ?? "Both";
			}

			// Roles
			if (RolesCheckBoxList.Items.Count == 0)
			{
				RolesCheckBoxList.DataSource = Roles.GetAllRoles();
				RolesCheckBoxList.DataBind();
			}
			foreach (ListItem item in RolesCheckBoxList.Items)
			{
				item.Selected = (CurrentNode != null && CurrentNode.Roles.Contains(item.Value));
			}
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			// Url should start with a slash
			if (!PageUrlTextBox.Text.StartsWith("/"))
			{
				PageUrlTextBox.Text = "/" + PageUrlTextBox.Text;
			}

			// Properties
			string code = CodeTextBox.Text;
			string pageTitle = PageTitleTextBox.Text;
			string pageMenuTitle = PageMenuTitleTextBox.Text;
			string pageUrl = PageUrlTextBox.Text;
			string pageDescription = PageDescriptionTextBox.Text;
			string pageKeywords = PageKeywordsTextBox.Text;
			string redirectToFirstChild = RedirectToFirstChildCheckBox.Checked.ToString();
			string redirect = RedirectTextBox.Text;

			// Template
			string template = TemplateDropDownList.SelectedValue;
			if (TemplateDropDownList.SelectedValue == "none")
			{
				template = pageUrl.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase) ? pageUrl : pageUrl + ".aspx";
			}
			else if (TemplateDropDownList.SelectedValue == "custom")
			{
				template = CustomTemplateTextBox.Text;
			}

			// Authentication
			string authentication = AuthenticationRadioButtonList.SelectedValue;

			// Roles
			StringBuilder roles = new StringBuilder();
			foreach (ListItem item in RolesCheckBoxList.Items)
			{
				if (item.Selected)
				{
					roles.Append(item.Value + ',');
				}
			}

			SiteMapManager.UpdateAttributes(CurrentNode, new Dictionary<string, string> 
			{
				{"code", code},
				{"title", pageTitle},
				{"menutitle", pageMenuTitle},
				{"url", pageUrl},
				{"lastmod", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm")},
				{"description", pageDescription},
				{"keywords", pageKeywords},
				{"state", null},
				{"template", template},
				{"redirecttofirstchild", redirectToFirstChild},
				{"redirect", redirect},
				{"authentication", authentication},
				{"roles", roles.Length == 0 ? "" : roles.ToString(0, roles.Length - 1)}
			},
			true);
		}

	}
}