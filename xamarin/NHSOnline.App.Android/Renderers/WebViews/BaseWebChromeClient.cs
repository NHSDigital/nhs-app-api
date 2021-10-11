using System.Collections.ObjectModel;
using Android.OS;
using Android.Webkit;
using NHSOnline.App.Droid.Handlers;
using NHSOnline.App.Droid.Renderers.WebViews.Extensions;
using NHSOnline.App.Threading;
using Xamarin.Essentials;
using Xamarin.Forms.Platform.Android;

namespace NHSOnline.App.Droid.Renderers.WebViews
{
    internal class BaseWebChromeClient : FormsWebChromeClient
    {
        private readonly ReadOnlyCollection<WebViewRendererExtension> _extensions;

        public BaseWebChromeClient(ReadOnlyCollection<WebViewRendererExtension> extensions)
        {
            _extensions = extensions;
        }

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