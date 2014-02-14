using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.IO;
using System.Text;

namespace CMS
{
	public partial class OnSaveHtmlEditor : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string id = Request["id"];
			try
			{
				string content = HttpUtility.HtmlDecode(Request["content"]);

				SiteMapManager.UpdateAttributes(Request.UrlReferrer.AbsolutePath, new Dictionary<string, string> { { id, content } }, false);

				Response.Write(string.Format("Saved '{0}'", id));
			}
			catch (Exception ex)
			{
				Response.Write(string.Format("Failed to save '{0}'\n\n{1}", id, ex.Message));

			}
		}

	}
}
