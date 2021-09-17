using System;
using System.Globalization;

namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public class DownloadRequest
    {
        private const string DataScheme = "data:";
        public DownloadRequest(string base64Data, string fileName, string mimeType)
        {
            FileName = fileName;
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

        public string Base64Data { get; }
        public string FileName { get; }
        public string MimeType { get; }
    }
}