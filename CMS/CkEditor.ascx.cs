using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CMS
{
	public partial class CkEditor : UserControl
	{
		public string EditorHeight { get; set; }

		public string ContentKey { get; set; }

		public string Style { get; set; }

		public string SiteMapProvider { get; set; }

		protected void Page_PreRender(object sender, EventArgs e)
		{
			SiteMapNode node = SiteMapManager.FindNode();
			if (node != null)
			{
				if (Tools.IsInEditMode)
				{
					ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "cke", "/ckeditor/ckeditor.js");
					ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "ckf", "/ckfinder/ckfinder.js");
				}
				lit.Text = node[ContentKey];
				lit.Visible = true;
			}
			phScript.Visible = CMS.Tools.IsInEditMode;
		}
	}
}