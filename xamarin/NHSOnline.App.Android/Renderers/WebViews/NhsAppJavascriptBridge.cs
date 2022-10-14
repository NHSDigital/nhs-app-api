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
        public void OpenWebIntegration(string rawArgument)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.OpenWebIntegration(rawArgument));
        }

        [JavascriptInterface]
        [Export("openPostWebIntegration")]
        public void OpenPostWebIntegration(string rawArgument)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.OpenPostWebIntegration(rawArgument));
        }

        [JavascriptInterface]
        [Export("startNhsLoginUplift")]
        public void StartNhsLoginUplift(string rawArgument)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.StartNhsLoginUplift(rawArgument));
        }

        [JavascriptInterface]
        [Export("getNotificationsStatus")]
        public void GetNotificationsStatus()
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.GetNotificationsStatus());
        }

        [JavascriptInterface]
        [Export("requestPnsToken")]
        public void RequestPnsToken(string rawArgument)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.RequestPnsToken(rawArgument));
        }

        [JavascriptInterface]
        [Export("fetchBiometricStatus")]
        public void FetchBiometricStatus(string rawArgument)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.FetchBiometricStatus(rawArgument));
        }

        [JavascriptInterface]
        [Export("updateBiometricRegistrationWithToken")]
        public void UpdateBiometricRegistrationWithToken(string rawArgument)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.UpdateBiometricRegistration(rawArgument));
        }

        [JavascriptInterface]
        [Export("openBrowserOverlay")]
        public void OpenBrowserOverlay(string rawArgument)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.OpenBrowserOverlay(rawArgument));
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
        public void SetMenuBarItem(string rawArgument)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.SetMenuBarItem(rawArgument));
        }

        [JavascriptInterface]
        [Export("clearMenuBarItem")]
        public void ClearMenuBarItem()
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.ClearMenuBarItem());
        }

        [JavascriptInterface]
        [Export("addEventToCalendar")]
        public void AddEventToCalendar(string rawArgument)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.AddEventToCalendar(rawArgument));
        }

        [JavascriptInterface]
        [Export("startDownloadFromJson")]
        public void StartDownloadFromJson(string rawArgument)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.StartDownload(rawArgument));
        }

        [JavascriptInterface]
        [Export("displayPageLeaveWarning")]
        public void DisplayPageLeaveWarning()
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.DisplayPageLeaveWarning());
        }

        [JavascriptInterface]
        [Export("displayKeywordReplyPageLeaveWarning")]
        public void DisplayKeywordReplyPageLeaveWarning()
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.DisplayKeywordReplyPageLeaveWarning());
        }

        [JavascriptInterface]
        [Export("createOnDemandGpSession")]
        public void CreateOnDemandGpSession(string rawArgument)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.CreateOnDemandGpSession(rawArgument));
        }

        [JavascriptInterface]
        [Export("onSessionExpiring")]
        public void OnSessionExpiring()
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.OnSessionExpiring());
        }

        [JavascriptInterface]
        [Export("fetchNativeAppVersion")]
        public void FetchNativeAppVersion()
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.FetchNativeAppVersion());
        }

        [JavascriptInterface]
        [Export("requestNotificationsRegistration")]
        public void RequestNotificationsRegistration(string nhsLoginId)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.RequestNotificationsRegistration(nhsLoginId));
        }

        [JavascriptInterface]
        [Export("sendNotificationsRegistration")]
        public void SendNotificationsRegistration(string payload)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.SetNotificationsRegistration(payload));
        }
    }
}
