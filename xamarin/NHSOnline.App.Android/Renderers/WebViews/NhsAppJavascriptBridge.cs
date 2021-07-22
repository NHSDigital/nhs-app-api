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

        [JavascriptInterface]
        [Export("openAppSettings")]
        public void OpenAppSettings()
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.OpenSettings());
        }

        [JavascriptInterface]
        [Export("logout")]
        public void Logout()
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.Logout());
        }

        [JavascriptInterface]
        [Export("sessionExpired")]
        public void SessionExpired()
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.SessionExpired());
        }

        [JavascriptInterface]
        [Export("setMenuBarItem")]
        public void SetMenuBarItem(string argumentJson)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.SetMenuBarItem(argumentJson));
        }

        [JavascriptInterface]
        [Export("clearMenuBarItem")]
        public void ClearMenuBarItem()
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.ClearMenuBarItem());
        }

        [JavascriptInterface]
        [Export("addEventToCalendar")]
        public void AddEventToCalendar(string argumentJson)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.AddEventToCalendar(argumentJson));
        }

        [JavascriptInterface]
        [Export("startDownloadFromJson")]
        public void StartDownloadFromJson(string argumentJson)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.StartDownload(argumentJson));
        }

        [JavascriptInterface]
        [Export("displayPageLeaveWarning")]
        public void DisplayPageLeaveWarning()
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.DisplayPageLeaveWarning());
        }

        [JavascriptInterface]
        [Export("createOnDemandGpSession")]
        public void CreateOnDemandGpSession(string argumentJson)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.CreateOnDemandGpSession(argumentJson));
        }

        [JavascriptInterface]
        [Export("onSessionExpiring")]
        public void OnSessionExpiring()
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.OnSessionExpiring());
        }
    }
}
