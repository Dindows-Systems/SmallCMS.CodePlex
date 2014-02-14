using System.Web;
using System.Text.RegularExpressions;

namespace CMS
{
	public static class CookieManager
	{
		/// <summary>
		/// Gets the cookie value.
		/// </summary>
		/// <param name="key">Name of the cookie.</param>
		/// <returns></returns>
		public static string GetCookieValue(string key)
		{
			HttpCookie cookie = HttpContext.Current.Request.Cookies[key];
			if (cookie != null)
			{
				return cookie.Value;
			}
			return null;
		}

		/// <summary>
		/// Sets the cookie value.
		/// </summary>
		/// <param name="key">Name of the cookie.</param>
		/// <param name="value">The cookie value.</param>
		public static void SetCookieValue(string key, string value)
		{
			HttpContext.Current.Response.Cookies[key].Value = value;
			HttpContext.Current.Response.Cookies[key].Expires = System.DateTime.Now.AddYears(1);
			HttpContext.Current.Request.Cookies[key].Value = value;
		}

	}
}
