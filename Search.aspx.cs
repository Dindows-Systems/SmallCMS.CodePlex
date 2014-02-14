using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Website
{
	public partial class Search : System.Web.UI.Page
	{
		public static string Url(string keywords)
		{
			return string.Format("/search.aspx?keywords={0}", keywords);
		}

		public int PageSize
		{
			set { ViewState["PageSize"] = value; }
			get { return (int)(ViewState["PageSize"] ?? 10); }
		}

		private int FirstPage
		{
			get { return (int)(ViewState["FirstPage"] ?? 0); }
			set { ViewState["FirstPage"] = value; }
		}

		private int LastPage
		{
			get { return (int)(ViewState["LastPage"] ?? -1); }
			set { ViewState["LastPage"] = value; }
		}

		private int CurrentPage
		{
			get { return (int)(ViewState["CurrentPage"] ?? -1); }
			set { ViewState["CurrentPage"] = value; }
		}

		private int NumberOfPages
		{
			get { return (int)(ViewState["NumberOfPages"] ?? 0); }
			set { ViewState["NumberOfPages"] = value; }
		}

		private void EnsureCurrentPage(int number)
		{
			if (number < 1)
			{
				CurrentPage = 1;
			}
			else if (number > NumberOfPages)
			{
				CurrentPage = NumberOfPages;
			}
			else
			{
				CurrentPage = number;
			}
			FirstPage = CurrentPage - 5;
			if (FirstPage < 1)
			{
				FirstPage = 1;
			}
			LastPage = FirstPage + 9;
			if (LastPage > NumberOfPages)
			{
				LastPage = NumberOfPages;
			}
		}

		public void DoSearch(string keywords)
		{
			Visible = true;

			txtKeywords.Text = keywords;

			FirstPage = 0;
			LastPage = -1;
			CurrentPage = 1;
			NumberOfPages = 0;
		}

		void lnkPrev_Click(object sender, EventArgs e)
		{
			EnsureCurrentPage(CurrentPage - 1);
		}

		void lnkNext_Click(object sender, EventArgs e)
		{
			EnsureCurrentPage(CurrentPage + 1);
		}

		protected void btn_Click(object sender, EventArgs e)
		{
			string text = ((LinkButton)(sender)).Text.Trim().ToLower();
			EnsureCurrentPage(int.Parse(text));
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			DoSearch(txtKeywords.Text);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			lnkNext.Click += new EventHandler(lnkNext_Click);
			lnkPrev.Click += new EventHandler(lnkPrev_Click);
			CreatePagerButtons();
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				DoSearch(Request.QueryString["Keywords"]);
			}

			int total;

			List<CMS.SiteMapManager.SearchResult> results = ((CMS.SiteMapManager)SiteMap.Provider).Search(
				txtKeywords.Text,
				PageSize,
				CurrentPage,
				out total);

			lvwResults.DataSource = results;
			lvwResults.DataBind();

			CreatePagerButtons();

			NumberOfPages = (total - 1) / PageSize + 1;
			EnsureCurrentPage(CurrentPage);

			lnkPrev.Enabled = (CurrentPage > 1);
			lnkPrev.CssClass = (CurrentPage > 1 ? "pager-enabled" : "pager-disabled");
			lnkNext.Enabled = (CurrentPage < LastPage);
			lnkNext.CssClass = (CurrentPage < LastPage ? "pager-enabled" : "pager-disabled");
			if (total == 1)
			{
				lblTotal.Text = "Found 1 page.";
			}
			else if (total > 1)
			{
				lblTotal.Text = string.Format("Found {0} pages.", total);
			}
			CreatePagerButtons();
		}

		private void CreatePagerButtons()
		{
			phPager.Controls.Clear();

			for (int page = FirstPage; page <= LastPage; page++)
			{
				LinkButton btn = new LinkButton();
				btn.ID = "P" + page.ToString();
				btn.Text = page.ToString();
				btn.Enabled = (page != CurrentPage);
				btn.CssClass = (page != CurrentPage ? "pager-enabled" : "pager-disabled");
				btn.Click += new EventHandler(btn_Click);
				phPager.Controls.Add(new LiteralControl("&nbsp;"));
				phPager.Controls.Add(btn);
				phPager.Controls.Add(new LiteralControl("&nbsp;"));
			}
		}

	}
}
