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
using NHSOnline.App.Threading;
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

        public async Task<DownloadFileResult> DownloadFile(DownloadRequest downloadRequest)
        {
            if (string.Equals(downloadRequest.MimeType, "application/vnd.apple.pkpass", StringComparison.Ordinal))
            {
                return HandlePassKitPassFile(downloadRequest);
            }

            return await HandleDefaultFileTypes(downloadRequest).PreserveThreadContext();
        }

        private static async Task<DownloadFileResult> HandleDefaultFileTypes(DownloadRequest downloadRequest)
        {
            var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
            var tmp = Path.Combine (documents, "..", "tmp");

            try
            {
                await File.WriteAllBytesAsync(
                    Path.Combine(tmp,downloadRequest.FileName),
                    Convert.FromBase64String(downloadRequest.Base64Data)).PreserveThreadContext();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to write data to cache directory");
                return new DownloadFileResult.Failed();
            }

            ShareFile shareFile = new ShareFile(Path.Combine(tmp,downloadRequest.FileName));

            var requestShare = new ShareFileRequest(
                Regex.Replace(downloadRequest.FileName, @"\s+", ""),
                shareFile);

            await Share.RequestAsync(requestShare).PreserveThreadContext();

            return new DownloadFileResult.Success();
        }

        private static DownloadFileResult HandlePassKitPassFile(DownloadRequest downloadRequest)
        {

            #pragma warning disable CA2000
                var data = new NSData(downloadRequest.Base64Data, NSDataBase64DecodingOptions.IgnoreUnknownCharacters);
                var passKitPass = new PKPass(data, out NSError error);
            #pragma warning restore CA2000

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse - The generated PKPass is incorrect and the NSError can actually be null
            if (error != null)
            {
                Logger.LogError(
                    $"Failed to create a pass kit pass, localised description is {error.LocalizedDescription}");
                return new DownloadFileResult.Failed();
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    using var passKitPassesViewController = new PKAddPassesViewController(passKitPass);
                    UIApplication.SharedApplication.KeyWindow.RootViewController?.PresentModalViewController(passKitPassesViewController, true);
                    data.Dispose();
                    passKitPass.Dispose();
                    passKitPassesViewController.Dispose();
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Failed to display the pass kit file");
                }
            });

            return new DownloadFileResult.Success();
        }
    }
}