using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace CMS
{
	/// <summary>
	/// Utilities for the entire solution to use.
	/// </summary>
	public static class Tools
	{
		public static bool IsInEditMode
		{
			get
			{
				if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
				{
					return false;
				}
				if (Thread.CurrentPrincipal.IsInAnyRoleOrEmpty("Editor", "Administrator"))
				{
					return (CMS.CookieManager.GetCookieValue("EditMode") == "1");
				}
				return false;
			}
		}

		/// <summary>
		/// Encrypts a string using the SHA256 algorithm.
		/// </summary>
		public static string HashPassword(string plainMessage)
		{
			byte[] data = Encoding.UTF8.GetBytes(plainMessage);
			using (HashAlgorithm sha = new SHA256Managed())
			{
				byte[] encryptedBytes = sha.TransformFinalBlock(data, 0, data.Length);
				return Convert.ToBase64String(sha.Hash);
			}
		}

	}
}
