using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Dialogs;
using NHSOnline.App.Threading;
using Xamarin.Essentials;

namespace NHSOnline.App.Services.Media
{
    public class SelectMediaService : ISelectMediaService
    {
        private readonly ILogger<SelectMediaService> _logger;
        private readonly IDialogPresenter _dialogPresenter;

        private const string AcceptedCameraDialogLogText =
            "User has accepted the info dialog for the camera permission";

        private const string CancelledCameraDialogLogText =
            "User has cancelled the info dialog for the camera permission";

        private const string AcceptedStorageDialogLogText =
            "User has accepted the info dialog for the file storage permission";

        private const string CancelledStorageDialogLogText =
            "User has cancelled the info dialog for the file storage permission";

        private const string FirstCameraSharedPrefKey = "FirstCameraRationaleShown";

        private const string FirstStorageReadSharedPrefKey = "FirstStorageReadRationaleShown";

        public SelectMediaService(ILogger<SelectMediaService> logger, IDialogPresenter dialogPresenter)
        {
            _logger = logger;
            _dialogPresenter = dialogPresenter;
        }

        public async Task SelectMedia(ISelectMediaRequest selectMediaRequest)
        {
            var prefKey =
                selectMediaRequest.Mode == SelectMediaMode.Take
                    ? FirstCameraSharedPrefKey
                    : FirstStorageReadSharedPrefKey;

            var permissionRationale =
                selectMediaRequest.Mode == SelectMediaMode.Take
                    ? Permissions.ShouldShowRationale<Permissions.Camera>()
                    : Permissions.ShouldShowRationale<Permissions.StorageRead>();

            if (_dialogPresenter.ShouldShowProminentDialog(prefKey, permissionRationale))
            {
                if (selectMediaRequest.Mode == SelectMediaMode.Take)
                {
                    await _dialogPresenter.DisplayAlertDialog(
                        new CameraPermissionRationale(async () =>
                        {
                            _logger.LogError(AcceptedCameraDialogLogText);

                            if (!Preferences.ContainsKey(prefKey))
                            {
                                Preferences.Set(prefKey, "true");
                            }

                            var file = await RetrieveFile(selectMediaRequest).PreserveThreadContext();

                            if (file is null)
                            {
                                selectMediaRequest.NoMediaSelected();
                            }
                            else
                            {
                                selectMediaRequest.MediaSelected(file.FullPath);
                            }
                        }, () =>
                        {
                            _logger.LogError(CancelledCameraDialogLogText);

                            selectMediaRequest.NoMediaSelected();

                            return Task.CompletedTask;
                        })).PreserveThreadContext();
                }
                else
                {
                    await _dialogPresenter.DisplayAlertDialog(
                        new FileStoragePermissionRationale(async () =>
                        {
                            _logger.LogError(AcceptedStorageDialogLogText);

                            if (!Preferences.ContainsKey(prefKey))
                            {
                                Preferences.Set(prefKey, "true");
                            }

                            var file = await RetrieveFile(selectMediaRequest).PreserveThreadContext();

                            if (file is null)
                            {
                                selectMediaRequest.NoMediaSelected();
                            }
                            else
                            {
                                selectMediaRequest.MediaSelected(file.FullPath);
                            }
                        }, () =>
                        {
                            _logger.LogError(CancelledStorageDialogLogText);

                            selectMediaRequest.NoMediaSelected();

                            return Task.CompletedTask;
                        })).PreserveThreadContext();
                }

            }
            else
            {
                var file = await RetrieveFile(selectMediaRequest).PreserveThreadContext();

                if (file is null)
                {
                    selectMediaRequest.NoMediaSelected();
                }
                else
                {
                    selectMediaRequest.MediaSelected(file.FullPath);
                }
            }
        }

        private async Task<FileResult?> RetrieveFile(ISelectMediaRequest selectMediaRequest)
        {
            return selectMediaRequest switch
            {
                {Type: SelectMediaType.Image, Mode: SelectMediaMode.Pick}
                    => await PickPhoto().PreserveThreadContext(),

                {Type: SelectMediaType.Image, Mode: SelectMediaMode.Take}
                    => await TakePhoto().PreserveThreadContext(),

                {Type: SelectMediaType.Video, Mode: SelectMediaMode.Pick}
                    => await PickVideo().PreserveThreadContext(),

                {Type: SelectMediaType.Video, Mode: SelectMediaMode.Take}
                    => await TakeVideo().PreserveThreadContext(),

                _ => throw new NotSupportedException(
                    $"Type: {selectMediaRequest.Type}; Mode: {selectMediaRequest.Mode}: Not Supported")
            };
        }

        private async Task<FileResult?> TakePhoto()
        {
            try
            {
                return await MediaPicker.CapturePhotoAsync().PreserveThreadContext();
            }
            catch (FeatureNotSupportedException e)
            {
                _logger.LogInformation(e, "Capture photo feature not supported");
                return null;
            }
            catch (PermissionException e)
            {
                _logger.LogInformation(e, "User refused permissions");
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected exception from {Method}", nameof(MediaPicker.CapturePhotoAsync));
                return null;
            }
        }

        private async Task<FileResult?> PickPhoto()
        {
            try
            {
               return await MediaPicker.PickPhotoAsync().PreserveThreadContext();
            }
            catch (FeatureNotSupportedException e)
            {
                _logger.LogInformation(e, "Pick photo feature not supported");
                return null;
            }
            catch (PermissionException e)
            {
                _logger.LogInformation(e, "User refused permissions");
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected exception from {Method}", nameof(MediaPicker.PickPhotoAsync));
                return null;
            }
        }

        private async Task<FileResult?> TakeVideo()
        {
            try
            {
                return await MediaPicker.CaptureVideoAsync().PreserveThreadContext();
            }
            catch (FeatureNotSupportedException e)
            {
                _logger.LogInformation(e, "Capture video feature not supported");
                return null;
            }
            catch (PermissionException e)
            {
                _logger.LogInformation(e, "User refused permissions");
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected exception from {Method}",nameof(MediaPicker.CaptureVideoAsync));
                return null;
            }
        }

        private async Task<FileResult?> PickVideo()
        {
            try
            {
                return await MediaPicker.PickVideoAsync().PreserveThreadContext();
            }
            catch (FeatureNotSupportedException e)
            {
                _logger.LogInformation(e, "Pick video feature not supported");
                return null;
            }
            catch (PermissionException e)
            {
                _logger.LogInformation(e, "User refused permissions");
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected exception from {Method}",  nameof(MediaPicker.PickVideoAsync));
                return null;
            }
        }
    }
}