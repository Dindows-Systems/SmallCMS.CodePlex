using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CMS
{
	public partial class GoogleAnalytics : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ltlCode.Text = System.Configuration.ConfigurationManager.AppSettings["GoogleAnalyticsTrackingKey"];
			Visible = !string.IsNullOrEmpty(ltlCode.Text);
		}
	}
}