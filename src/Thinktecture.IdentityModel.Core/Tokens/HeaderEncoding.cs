using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Thinktecture.IdentityModel.Tokens
{
    public static class HeaderEncoding
    {
        public static bool IsBase64Encoded(string token)
        {
            token = token.Trim();
            return (token.Length % 4 == 0) &&
                   (Regex.IsMatch(token, @"^[a-zA-Z0-9+/]*={0,3}$", RegexOptions.None));
        }

        public static string EncodeBase64(string token)
        {
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string encodedToken = Convert.ToBase64String(encoding.GetBytes(token));
            return encodedToken;
        }

        public static string DecodeBase64(string token)
        {
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string decodedToken = encoding.GetString(Convert.FromBase64String(token));
            return decodedToken;
        }
    }
}
