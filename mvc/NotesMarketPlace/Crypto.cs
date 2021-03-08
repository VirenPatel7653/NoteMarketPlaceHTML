using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NotesMarketPlace
{
    public static class Crypto
    {
           public static string Hash(string value)
           {
            return Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create()
                .ComputeHash(Encoding.UTF8.GetBytes(value))
                );

           }

		public static string EncryptBase64(string value)
		{
			string result = "";

			try
			{
				if (!string.IsNullOrEmpty(value))
				{
					byte[] bytes = System.Text.UnicodeEncoding.UTF8.GetBytes(value);
					result = Convert.ToBase64String(bytes);
				}
			}
			catch { }

			return result;
		}


		public static string DecryptBase64(string value)
		{
			string result = "";

			try
			{
				byte[] bytes = Convert.FromBase64String(value);

				result = System.Text.Encoding.UTF8.GetString(bytes);
			}
			catch { }

			return result;
		}
	}
}