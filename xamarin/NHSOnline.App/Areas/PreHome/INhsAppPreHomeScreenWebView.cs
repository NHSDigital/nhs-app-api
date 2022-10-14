using System;
using System.Threading.Tasks;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.PreHome
{
    internal interface INhsAppPreHomeScreenWebView : INavigationView<INhsAppPreHomeScreenWebView.IEvents>, IAccessibleControl
    {
        internal interface IEvents
        {
            Func<Task>? Appearing { get; set; }
            Action<WebNavigatingEventArgs>? Navigating { get; set; }
            Func<Uri, Task>? NavigationFailed { get; set; }
            Action<WebViewPageNavigationEventArgs>? PageLoadComplete { get; set; }
            Func<Task>? GetNotificationsStatusRequested { get; set; }
            Func<string, Task>? FetchBiometricStatusRequested { get; set; }
            Func<string, Task>? UpdateBiometricRegistrationRequested { get; set; }
            Func<Task>? GoToLoggedInHomeRequested { get; set; }
            Func<Task>? LogoutRequested { get; set; }
            Func<Task>? ResetAndShowErrorRequested { get; set; }
            Func<string, Task>? GetPnsTokenRequested { get; set; }
            Func<string, Task>? NotificationsRegistrationRequested { get; set; }
            Func<SetNotificationsRegistrationRequest, Task>? SetNotificationsRegistrationRequested { get; set; }
            Func<Uri, Task>? DeeplinkRequested { get; set; }
            Func<Task>? SessionExpiredRequested { get; set; }
            Func<Task>? OnSessionExpiringRequested { get; set; }
        }

        Task<Uri?> GetCurrentWebViewUrl();
        void GoToUri(Uri uri);
        Task SendBiometricStatus(BiometricStatus biometricStatus);
        Task SendBiometricCompletion(BiometricCompletion completionDetails);
        Task SendNotificationsStatus(string status);
        Task SendNotificationAuthorised(NotificationAuthorisedResponse authorisedResponse);
        Task SendNotificationUnauthorised();
        Task SendNotificationsRegistration(NotificationsRegistration response);
        Task SendSessionExtend();
        Task Logout();
    }
}
