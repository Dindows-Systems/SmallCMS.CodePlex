using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Website.Properties;

namespace Website
{
	public partial class HashInMenu : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!System.Threading.Thread.CurrentPrincipal.IsInRole(Settings.Default.CMS_Role))
			{
				// Page not found
				Response.Clear();
				Response.Write("Page Not Found (404)");
				Response.StatusCode = 404;
				Response.End();
			}
		}
	}
}
