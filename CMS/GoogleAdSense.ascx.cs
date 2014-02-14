using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;

namespace CMS
{
	public partial class GoogleAdSense : System.Web.UI.UserControl
	{
		public string FileName { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			string fileName = Server.MapPath(FileName);
			if (File.Exists(fileName))
			{
				ltl.Text = File.ReadAllText(fileName);

			}
		}
	}
}