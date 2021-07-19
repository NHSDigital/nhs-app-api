using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Provider;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices;
using NHSOnline.App.Logging;
using Xamarin.Essentials;
using Xamarin.Forms;
using Environment = Android.OS.Environment;
using File = System.IO.File;
using Uri = Android.Net.Uri;

[assembly: Dependency(typeof(AndroidFileHandler))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public class AndroidFileHandler: IFileHandler
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AndroidFileHandler));
        internal static MainActivity? MainActivity { set; get; }

        public async Task StoreFileInDownloads(DownloadRequest downloadRequest)
        {
            try
            {
                var resolver = CreateContentResolver(downloadRequest, out var fileUri);

                var convertedData = Convert.FromBase64String(downloadRequest.Base64Data);

                var outputStream = resolver.OpenOutputStream(fileUri!);
                await outputStream!.WriteAsync(convertedData).ConfigureAwait(true);
                outputStream.Close();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to store file in downloads");
            }
        }

        private static ContentResolver CreateContentResolver(DownloadRequest downloadRequest, out Uri? fileUri)
        {
            var resolver = MainActivity?.ContentResolver;

            using var contentValues = new ContentValues();
            contentValues.Put(MediaStore.MediaColumns.DisplayName, downloadRequest.FileName);
            contentValues.Put(MediaStore.MediaColumns.MimeType, downloadRequest.MimeType);
            contentValues.Put(MediaStore.MediaColumns.RelativePath, Environment.DirectoryDownloads + "/NhsApp");

            fileUri = resolver!.Insert(MediaStore.Downloads.ExternalContentUri, contentValues);
            return resolver;
        }

        public async Task HandleFile(DownloadRequest downloadRequest)
        {
            try
            {
                var convertedData = Convert.FromBase64String(downloadRequest.Base64Data);
                await File.WriteAllBytesAsync(downloadRequest.FileCachePath, convertedData).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to write data to cache directory");
            }

            var target = new ReadOnlyFile(downloadRequest.FileCachePath);

            var request = new OpenFileRequest
            {
                File = target
            };

            await Launcher.OpenAsync(request).ConfigureAwait(true);
        }
    }
}