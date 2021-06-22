using Android.OS;
using Android.Webkit;
using NHSOnline.App.Droid.Handlers;
using NHSOnline.App.Threading;
using Xamarin.Essentials;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public class BaseChromeClient : FormsWebChromeClient
    {
        public override async void OnGeolocationPermissionsShowPrompt(string? origin, GeolocationPermissions.ICallback? callback)
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>().ResumeOnThreadPool();
            if (Build.VERSION.SdkInt < BuildVersionCodes.M || status == PermissionStatus.Granted)
            {
                callback?.Invoke(origin, true, false);
            }
            else
            {
                var results = await Permissions.RequestAsync<Permissions.LocationWhenInUse>().ResumeOnThreadPool();

                if (results == PermissionStatus.Granted)
                {
                    OnReceivedGeolocationPermissionsResultHandler.Instance.MGeolocationOrigin = origin;
                    OnReceivedGeolocationPermissionsResultHandler.Instance.MGeoLocationCallback = callback;
                }
                else
                {
                    callback?.Invoke(origin, false, false);
                }
            }
        }
    }
}