using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Logging;

namespace NHSOnline.App.Controls.WebViews.Payloads
{
    public class DownloadRequest
    {
        private const string DataScheme = "data:";
        private static ILogger Logger => NhsAppLogging.CreateLogger<DownloadRequest>();
        public DownloadRequest(string base64Data, string fileName, string mimeType)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                // Builds a string out of valid chars and an _ for invalid ones
                fileName = new string(fileName.Select(ch => Path.GetInvalidFileNameChars().Contains(ch) ? '_' : ch).ToArray());
                FileName = fileName.Replace('/', '_');
            }
            else
            {
                FileName = fileName;
                Logger.LogError( "Download Request: Filename is null or empty");
            }

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