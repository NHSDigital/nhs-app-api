using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Dialogs;
using NHSOnline.App.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace NHSOnline.App.Services
{
    internal sealed class FileDownloadService
    {
        private readonly ILogger<FileDownloadService> _logger;

        private const string FirstStorageWriteSharedPrefKey = "FirstStorageWriteRationaleShown";

        public FileDownloadService(ILogger<FileDownloadService> logger)
        {
            _logger = logger;
        }

        public async Task StartDownloadRequested(
            IDialogPresenter dialogPresenter,
            DownloadRequest downloadRequest,
            IFileHandler fileHandler,
            View webViewElement,
            Action downloadFailed)
        {
            if (dialogPresenter.ShouldShowProminentDialog(
                    FirstStorageWriteSharedPrefKey,
                    Permissions.ShouldShowRationale<Permissions.StorageWrite>()))
            {
                await dialogPresenter.DisplayAlertDialog(
                    new FileStoragePermissionRationale(async () =>
                    {
                        _logger.LogInformation("User has accepted the info dialog for the file write permission");
                        if (!Preferences.ContainsKey(FirstStorageWriteSharedPrefKey))
                        {
                            Preferences.Set(FirstStorageWriteSharedPrefKey, "true");
                        }

                        await RequestPermissionAndAttemptFileDownload(downloadRequest, fileHandler, webViewElement, downloadFailed).PreserveThreadContext();
                    }, () =>
                    {
                        _logger.LogInformation("User has cancelled the info dialog for the file write permission");
                        return Task.CompletedTask;
                    })).PreserveThreadContext();
            }
            else
            {
                await RequestPermissionAndAttemptFileDownload(downloadRequest, fileHandler, webViewElement, downloadFailed).PreserveThreadContext();
            }
        }

        private static async Task RequestPermissionAndAttemptFileDownload(
            DownloadRequest downloadRequest,
            IFileHandler fileHandler,
            View webViewElement,
            Action downloadFailed)
        {
            var storagePermissionRequest = await Permissions.RequestAsync<Permissions.StorageWrite>().PreserveThreadContext();

            if (storagePermissionRequest == PermissionStatus.Granted)
            {
                var handleFileResult = await fileHandler.DownloadFile(downloadRequest, webViewElement).PreserveThreadContext();
                if (handleFileResult is DownloadFileResult.Failed)
                {
                    downloadFailed.Invoke();
                }
            }
        }
    }
}