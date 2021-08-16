using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Foundation;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.iOS.DependencyServices;
using NHSOnline.App.Logging;
using PassKit;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosFileHandler))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public class IosFileHandler: IFileHandler
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(IosFileHandler));

        public Task StoreFileInDownloads(DownloadRequest downloadRequest)
        {
            Logger.LogInformation("We currently do not store files directly to the download folder");
            return Task.CompletedTask;
        }

        public async Task HandleFile(DownloadRequest downloadRequest)
        {
            if (string.Equals(downloadRequest.MimeType, "application/vnd.apple.pkpass", StringComparison.Ordinal))
            {
                HandlePassKitPassFile(downloadRequest);
            }

            await HandleDefaultFileTypes(downloadRequest).ConfigureAwait(true);
        }

        private static async Task HandleDefaultFileTypes(DownloadRequest downloadRequest)
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

        private static void HandlePassKitPassFile(DownloadRequest downloadRequest)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    using var data = new NSData(downloadRequest.Base64Data, NSDataBase64DecodingOptions.IgnoreUnknownCharacters);
                    using var passKitPass = new PKPass(data, out NSError error);

                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse - The generated PKPass is incorrect and the NSError can actually be null
                    if (error != null)
                    {
                        Logger.LogError(
                            $"Failed to create a pass kit pass, localised description is {error.LocalizedDescription}");
                        return;
                    }

                    using var passKitPassesViewController = new PKAddPassesViewController(passKitPass);

                    UIApplication.SharedApplication.KeyWindow.RootViewController?.PresentModalViewController(passKitPassesViewController, true);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Failed to handle the pass kit file");
                }
            });
        }
    }
}