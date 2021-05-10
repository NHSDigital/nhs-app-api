using Android.Webkit;
using Java.Interop;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using Object = Java.Lang.Object;

namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public class NhsAppJavascriptBridge : Object
    {
        public const string JavascriptObjectName = "nativeApp";

        private readonly NhsAppWebView _nhsAppWebView;

        public NhsAppJavascriptBridge(NhsAppWebView nhsAppWebView)
        {
            _nhsAppWebView = nhsAppWebView;
        }

        [JavascriptInterface]
        [Export("openWebIntegration")]
        public void OpenWebIntegration(string argumentJson)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.OpenWebIntegration(argumentJson));
        }

        [JavascriptInterface]
        [Export("startNhsLoginUplift")]
        public void StartNhsLoginUplift(string argumentJson)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.StartNhsLoginUplift(argumentJson));
        }

        [JavascriptInterface]
        [Export("getNotificationsStatus")]
        public void GetNotificationsStatus()
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.GetNotificationsStatus());
        }

        [JavascriptInterface]
        [Export("requestPnsToken")]
        public void RequestPnsToken(string argumentJson)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.RequestPnsToken(argumentJson));
        }

        [JavascriptInterface]
        [Export("fetchBiometricStatus")]
        public void FetchBiometricStatus(string argumentJson)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.FetchBiometricStatus(argumentJson));
        }

        [JavascriptInterface]
        [Export("updateBiometricRegistrationWithToken")]
        public void UpdateBiometricRegistrationWithToken(string argumentJson)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.UpdateBiometricRegistration(argumentJson));
        }
    }
}
