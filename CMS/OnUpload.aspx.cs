using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace CMS
{
	public partial class OnUpload : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (Request.Files.Count == 1)
				{
					HttpPostedFile post = Request.Files[0];
					string clientfilename = System.IO.Path.GetFileName(post.FileName);
					string serverfilename = Server.MapPath("/files/" + clientfilename);
					post.SaveAs(serverfilename);
					Response.Write(string.Format("Uploaded /files/{0}", clientfilename));
				}
			}
			catch (Exception ex)
			{
				Response.Write(string.Format("Failed to upload.\n\n{0}", ex.Message));
			}
		}

	}
}
