using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas;
using NHSOnline.App.Controls.WebViews;
using Plugin.Media;
using Plugin.Media.Abstractions;

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
            try
            {
                await CrossMedia.Current.Initialize().PreserveThreadContext();
                var file = selectMediaRequest switch
                {
                    {Type: SelectMediaType.Image, Mode: SelectMediaMode.Pick}
                        => await PickPhoto().PreserveThreadContext(),

                    {Type: SelectMediaType.Image, Mode: SelectMediaMode.Take}
                        => await TakePhoto().PreserveThreadContext(),

                    {Type: SelectMediaType.Video, Mode: SelectMediaMode.Pick}
                        => await TakeVideo().PreserveThreadContext(),

                    {Type: SelectMediaType.Video, Mode: SelectMediaMode.Take}
                        => await PickVideo().PreserveThreadContext(),

                    _ => throw new NotSupportedException(
                        $"Type: {selectMediaRequest.Type}; Mode: {selectMediaRequest.Mode}: Not Supported")
                };

                if (file is null)
                {
                    selectMediaRequest.NoMediaSelected();
                }
                else
                {
                    selectMediaRequest.MediaSelected(file.Path);
                }
            }
            catch (MediaPermissionException e)
            {
                selectMediaRequest.NoMediaSelected();
                _logger.LogInformation(e, $"User refused storage permissions");
            }
            catch (Exception e)
            {
                selectMediaRequest.NoMediaSelected();
                _logger.LogError(e, $"Exception occurred, selectMediaRequest failed");
            }
        }

        private static async Task<MediaFile?> TakePhoto()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                return null;
            }

            return await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Login"
            }).PreserveThreadContext();
        }

        private static async Task<MediaFile?> PickPhoto()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickPhotoSupported)
            {
                return null;
            }

            return await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions()).PreserveThreadContext();
        }

        private static async Task<MediaFile?> TakeVideo()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported)
            {
                return null;
            }

            return await CrossMedia.Current.TakeVideoAsync(new StoreVideoOptions
            {
                Directory = "Login"
            }).PreserveThreadContext();
        }

        private static async Task<MediaFile?> PickVideo()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickVideoSupported)
            {
                return null;
            }

            return await CrossMedia.Current.PickVideoAsync().PreserveThreadContext();
        }
    }
}