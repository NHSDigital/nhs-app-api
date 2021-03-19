using Android.Webkit;
using Java.Interop;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using Object = Java.Lang.Object;

namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public class NhsAppPreHomeJavascriptBridge : Object
    {
        public const string JavascriptObjectName = "nativeApp";

        private readonly NhsAppPreHomeScreenWebview _nhsAppPreHomeScreenWebView;

        public NhsAppPreHomeJavascriptBridge(NhsAppPreHomeScreenWebview nhsAppPreHomeScreenWebView)
        {
            _nhsAppPreHomeScreenWebView = nhsAppPreHomeScreenWebView;
        }

        [JavascriptInterface]
        [Export("goToLoggedInHomeScreen")]
        public void GoToLoggedInHomeScreen()
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppPreHomeScreenWebView.GoToLoggedInHomeScreen());
        }

        [JavascriptInterface]
        [Export("getNotificationsStatus")]
        public void GetNotificationsStatus()
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppPreHomeScreenWebView.GetNotificationsStatus());
        }

        [JavascriptInterface]
        [Export("requestPnsToken")]
        public void RequestPnsToken(string trigger)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppPreHomeScreenWebView.RequestPnsToken(trigger));
        }
    }
}