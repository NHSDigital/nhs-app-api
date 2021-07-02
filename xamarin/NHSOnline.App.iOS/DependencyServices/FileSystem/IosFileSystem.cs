using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.iOS.DependencyServices.FileSystem;
using NHSOnline.App.Logging;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosFileSystem))]
namespace NHSOnline.App.iOS.DependencyServices.FileSystem
{
    public class IosFileSystem: IFileSystemService
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(IosFileSystem));

        public void StoreFileInDownloads(DownloadRequest downloadRequest)
        {
            Logger.LogInformation("We currently do not store files directly to the download folder");
        }

        public async void PresentFileActionController(DownloadRequest downloadRequest)
        {
            try
            {
                await File.WriteAllBytesAsync(
                    downloadRequest.FileCachePath,
                    Convert.FromBase64String(downloadRequest.Base64Data)).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to write data to cache directory", e);
                return;
            }

            ShareFile shareFile = new ShareFile(downloadRequest.FileCachePath);

            var requestShare = new ShareFileRequest(
                Regex.Replace(downloadRequest.FileName, @"\s+", ""),
                shareFile);

            await Share.RequestAsync(requestShare).ConfigureAwait(true);
        }
    }
}