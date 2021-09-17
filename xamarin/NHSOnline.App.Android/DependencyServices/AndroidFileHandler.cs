using System;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Provider;
using Java.IO;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using Xamarin.Forms;
using Environment = Android.OS.Environment;
using FileProvider = AndroidX.Core.Content.FileProvider;
using Uri = Android.Net.Uri;

[assembly: Dependency(typeof(AndroidFileHandler))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public class AndroidFileHandler: IFileHandler
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AndroidFileHandler));
        internal static MainActivity? MainActivity { set; get; }

        private static readonly string FilePath = Path.Combine(Environment.DirectoryDownloads + "/NhsApp");

        public async Task<DownloadFileResult> DownloadFile(DownloadRequest downloadRequest)
        {
            Uri? uri;

            try
            {
                var convertedData = Convert.FromBase64String(downloadRequest.Base64Data);
                uri = await StoreFileInDownloads(convertedData, downloadRequest, downloadRequest.FileName).PreserveThreadContext();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to write save file to device");
                return new DownloadFileResult.Failed();
            }

            try
            {
                using var viewFileIntent = new Intent(Intent.ActionView, uri);
                viewFileIntent.AddFlags(ActivityFlags.GrantPersistableUriPermission | ActivityFlags.NewTask);
                MainActivity?.StartActivity(viewFileIntent);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to share the downloaded file");
                return new DownloadFileResult.Failed();
            }

            return new DownloadFileResult.Success();
        }

        private static async Task<Uri?> StoreFileInDownloads(byte[] convertedData, DownloadRequest downloadRequest, string fileName)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            {
                return await CreateFileFromResolver(convertedData, downloadRequest).PreserveThreadContext();
            }

            return await CreateFileUsingDeprecated(convertedData, fileName).PreserveThreadContext();

        }

        private static async Task<Uri?> CreateFileUsingDeprecated(byte[] convertedData, string fileName)
        {
            #pragma warning disable 618
                var directory = Path.Combine(Environment.ExternalStorageDirectory?.AbsolutePath!, FilePath);
            #pragma warning restore 618

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var file = new Java.IO.File(Path.Combine(directory, fileName));
            using var outputStream = new FileOutputStream(file);
            await outputStream.WriteAsync(convertedData).PreserveThreadContext();

            return FileProvider.GetUriForFile(
                MainActivity,
                Android.App.Application.Context.PackageName + ".fileprovider",
                file);
        }

        private static async Task<Uri?> CreateFileFromResolver(byte[] convertedData, DownloadRequest downloadRequest)
        {
            var resolver = CreateContentResolver(downloadRequest, out var fileUri);
            await using var outputStream = resolver.OpenOutputStream(fileUri)!;
            await outputStream.WriteAsync(convertedData).PreserveThreadContext();
            return fileUri;
        }

        private static ContentResolver CreateContentResolver(DownloadRequest downloadRequest, out Uri fileUri)
        {
            var resolver = MainActivity?.ContentResolver!;

            using var contentValues = new ContentValues();
            contentValues.Put(MediaStore.IMediaColumns.DisplayName, downloadRequest.FileName);
            contentValues.Put(MediaStore.IMediaColumns.MimeType, downloadRequest.MimeType);
            contentValues.Put(MediaStore.IMediaColumns.RelativePath, FilePath);

            fileUri = resolver.Insert(MediaStore.Downloads.ExternalContentUri, contentValues)!;
            return resolver;
        }
    }
}