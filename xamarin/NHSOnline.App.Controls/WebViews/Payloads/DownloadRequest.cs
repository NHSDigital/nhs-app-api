using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public class DownloadRequest
    {
        private const string DataScheme = "data:";
        public DownloadRequest(string base64Data, string fileName, string mimeType)
        {
            FileName = fileName;
            FileCachePath = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, Regex.Replace(fileName, @"\s+", ""));
            MimeType = mimeType;

            if (base64Data.StartsWith(DataScheme, true, CultureInfo.InvariantCulture))
            {
                var withoutScheme = base64Data.Replace(DataScheme, "", StringComparison.Ordinal);
                var splitOnComma = withoutScheme.Split(",");
                Base64Data = splitOnComma[1];
            }
            else
            {
                Base64Data = base64Data;
            }
        }

        public string Base64Data { get; set; }
        public string FileName { get; set; }
        public string FileCachePath { get; set; }
        public string MimeType { get; set; }
    }
}