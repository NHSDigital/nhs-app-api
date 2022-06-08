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
using Rectangle = System.Drawing.Rectangle;

[assembly: Dependency(typeof(IosFileHandler))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public class IosFileHandler : IFileHandler
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(IosFileHandler));
        private const int InitialBoxSize = 20;

        public async Task<DownloadFileResult> DownloadFile(DownloadRequest downloadRequest, View webViewElement)
        {
            if (string.Equals(downloadRequest.MimeType, "application/vnd.apple.pkpass", StringComparison.Ordinal))
            {
                return HandlePassKitPassFile(downloadRequest);
            }

            return await HandleDefaultFileTypes(downloadRequest, webViewElement).PreserveThreadContext();
        }

        private static async Task<DownloadFileResult> HandleDefaultFileTypes(DownloadRequest downloadRequest,
            View webViewElement)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var tmp = Path.Combine(documents, "..", "tmp");

            try
            {
                await File.WriteAllBytesAsync(
                    Path.Combine(tmp, downloadRequest.FileName),
                    Convert.FromBase64String(downloadRequest.Base64Data)).PreserveThreadContext();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to write data to cache directory");
                return new DownloadFileResult.Failed();
            }

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = Regex.Replace(downloadRequest.FileName, @"\s+", ""),
                File = new ShareFile(Path.Combine(tmp, downloadRequest.FileName)),
                PresentationSourceBounds = GetViewBounds(webViewElement)
            }).PreserveThreadContext();

            return new DownloadFileResult.Success();
        }

        private static Rectangle GetViewBounds(View webViewElement)
        {
            var viewCenterX = webViewElement.Bounds.Center.X;
            var viewCenterY = webViewElement.Bounds.Center.Y;

            return DrawRectangle(viewCenterX, viewCenterY, InitialBoxSize, InitialBoxSize);
        }

        // Working out the boundaries is required for an iPad as it floats in the larger screen as opposed to iPhones where it is fixed to the bottom
        private static Rectangle DrawRectangle(double viewCenterX, double viewCenterY, double width, double height)
        {
            return DeviceInfo.Idiom == DeviceIdiom.Tablet
                ? new Rectangle((int)viewCenterX, (int)viewCenterY, (int)width, (int)height)
                : Rectangle.Empty;
        }

        private static DownloadFileResult HandlePassKitPassFile(DownloadRequest downloadRequest)
        {
            NSData data;
            PKPass passKitPass;
            NSError? error;

            try
            {
#pragma warning disable CA2000
                data = new NSData(downloadRequest.Base64Data, NSDataBase64DecodingOptions.IgnoreUnknownCharacters);
                passKitPass = new PKPass(data, out error);
#pragma warning restore CA2000
            }
            catch (Exception e)
            {
                Logger.LogError(e,
                    "Failed to construct a PKPass object as the data received is not in the expected format. DownloadRequest file length: {DownloadRequestFileLength}",
                    downloadRequest.Base64Data?.Length);
                return new DownloadFileResult.Failed();
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse - The generated PKPass is incorrect and the NSError can actually be null
            if (error != null)
            {
                Logger.LogError("Failed to create a pass kit pass, localised description is {Error}",
                    error.LocalizedDescription);
                return new DownloadFileResult.Failed();
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    using var passKitPassesViewController = new PKAddPassesViewController(passKitPass);
                    UIApplication.SharedApplication.KeyWindow?.RootViewController?.PresentModalViewController(
                        passKitPassesViewController, true);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Failed to display the pass kit file");
                }
                finally
                {
                    data.Dispose();
                    passKitPass.Dispose();
                }
            });

            return new DownloadFileResult.Success();
        }
    }
}