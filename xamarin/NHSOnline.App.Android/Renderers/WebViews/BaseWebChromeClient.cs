using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Android.OS;
using Android.Webkit;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Dialogs;
using NHSOnline.App.Droid.DependencyServices.AlertDialog;
using NHSOnline.App.Droid.Handlers;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using Xamarin.Essentials;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews
{
    internal class BaseWebChromeClient : FormsWebChromeClient
    {
        private readonly ReadOnlyCollection<WebViewRendererExtension> _extensions;

        private const string FirstGeoLocationSharedPrefKey = "FirstGeoLocationRationaleShown";

        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(BaseWebChromeClient));

        public BaseWebChromeClient(ReadOnlyCollection<WebViewRendererExtension> extensions)
        {
            _extensions = extensions;
        }

        public override async void OnGeolocationPermissionsShowPrompt(string? origin, GeolocationPermissions.ICallback? callback)
        {
            if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>() || !Preferences.ContainsKey(FirstGeoLocationSharedPrefKey))
            {
                await AndroidDialogPresenter.CreateAndShowAlertDialog(
                    new LocationPermissionRationale(async () =>
                    {
                        Logger.LogError("User has accepted the info dialog for the location permission");
                        if (!Preferences.ContainsKey(FirstGeoLocationSharedPrefKey))
                        {
                            Preferences.Set(FirstGeoLocationSharedPrefKey, "true");
                        }

                        await ShowGeoLocationPrompt(origin, callback).ResumeOnThreadPool();
                    }, () =>
                    {
                        Logger.LogError("User has cancelled the info dialog for the location permission");
                        callback?.Invoke(origin, false, false);
                        return Task.CompletedTask;
                    })).PreserveThreadContext();
            }
            else
            {
                Logger.LogError("User has already seen the info dialog for the location permission");
                await ShowGeoLocationPrompt(origin, callback).ResumeOnThreadPool();
            }
        }

        private static async Task ShowGeoLocationPrompt(string? origin, GeolocationPermissions.ICallback? callback)
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
                    OnReceivedGeolocationPermissionsResultHandler.Instance.GeolocationOrigin = origin;
                    OnReceivedGeolocationPermissionsResultHandler.Instance.GeolocationCallback = callback;
                }
                else
                {
                    callback?.Invoke(origin, false, false);
                }
            }
        }

        public override bool OnShowFileChooser(WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
        {
            foreach (var extension in _extensions)
            {
                if (extension.OnShowFileChooser(webView, filePathCallback, fileChooserParams) == ShowFileChooserExtensionDecision.Handled)
                {
                    return true;
                }
            }

            return base.OnShowFileChooser(webView, filePathCallback, fileChooserParams);
        }
    }
}