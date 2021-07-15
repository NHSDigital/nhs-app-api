using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas;
using NHSOnline.App.Controls.WebViews.Payloads;
using Xamarin.Essentials;

namespace NHSOnline.App.Services.Media
{
    public class SelectMediaService : ISelectMediaService
    {
        private readonly ILogger<SelectMediaService> _logger;
        public SelectMediaService(ILogger<SelectMediaService> logger)
        {
            _logger = logger;
        }

        public async Task SelectMedia(ISelectMediaRequest selectMediaRequest)
        {
            var file = selectMediaRequest switch
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

            if (file is null)
            {
                selectMediaRequest.NoMediaSelected();
            }
            else
            {
                selectMediaRequest.MediaSelected(file.FullPath);
            }
        }

        private async Task<FileResult?> TakePhoto()
        {
            try
            {
                return await MediaPicker.CapturePhotoAsync().PreserveThreadContext();
            }
            catch (FeatureNotSupportedException featureNotSupportedException)
            {
                _logger.LogInformation(featureNotSupportedException, "Capture photo feature not supported");
                return null;
            }
            catch (PermissionException permissionException)
            {
                _logger.LogInformation(permissionException, "User refused permissions");
                return null;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Unexpected exception from {nameof(MediaPicker.CapturePhotoAsync)}");
                return null;
            }
        }

        private async Task<FileResult?> PickPhoto()
        {
            try
            {
               return await MediaPicker.PickPhotoAsync().PreserveThreadContext();
            }
            catch (FeatureNotSupportedException featureNotSupportedException)
            {
                _logger.LogInformation(featureNotSupportedException, "Pick photo feature not supported");
                return null;
            }
            catch (PermissionException permissionException)
            {
                _logger.LogInformation(permissionException, "User refused permissions");
                return null;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Unexpected exception from {nameof(MediaPicker.PickPhotoAsync)}");
                return null;
            }
        }

        private async Task<FileResult?> TakeVideo()
        {
            try
            {
                return await MediaPicker.CaptureVideoAsync().PreserveThreadContext();
            }
            catch (FeatureNotSupportedException featureNotSupportedException)
            {
                _logger.LogInformation(featureNotSupportedException, "Capture video feature not supported");
                return null;
            }
            catch (PermissionException permissionException)
            {
                _logger.LogInformation(permissionException, "User refused permissions");
                return null;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Unexpected exception from {nameof(MediaPicker.CaptureVideoAsync)}");
                return null;
            }
        }

        private async Task<FileResult?> PickVideo()
        {
            try
            {
                return await MediaPicker.PickVideoAsync().PreserveThreadContext();
            }
            catch (FeatureNotSupportedException featureNotSupportedException)
            {
                _logger.LogInformation(featureNotSupportedException, "Pick video feature not supported");
                return null;
            }
            catch (PermissionException permissionException)
            {
                _logger.LogInformation(permissionException, "User refused permissions");
                return null;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Unexpected exception from {nameof(MediaPicker.PickVideoAsync)}");
                return null;
            }
        }
    }
}