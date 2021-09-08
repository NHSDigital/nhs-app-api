using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Web;

namespace NHSOnline.App
{
    public static class ReadOnlyDictionaryExtension
    {
        [SuppressMessage("Design", "CA1055: URI return values should not be strings",
            Justification = "There is no URI here we are just using the uri encoding")]
        public static string ToFormUrlEncodedString(this IReadOnlyDictionary<string, string> dictionary)
        {
            var builder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                if (builder.Length > 0)
                {
                    builder.Append('&');
                }

                builder.Append(Encode(pair.Key));
                builder.Append('=');
                builder.Append(Encode(pair.Value));
            }

            return builder.ToString();
        }

        public static byte[] ToFormUrlEncodedBytes(this IReadOnlyDictionary<string, string> dictionary)
        {
            return Encoding.ASCII.GetBytes(ToFormUrlEncodedString(dictionary));
        }

        private static string Encode(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
            return HttpUtility.UrlEncode(data);
        }
    }
}