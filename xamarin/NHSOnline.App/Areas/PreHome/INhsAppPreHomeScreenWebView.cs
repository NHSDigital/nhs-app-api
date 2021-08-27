using System;
using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.PreHome
{
    internal interface INhsAppPreHomeScreenWebView : INavigationView<INhsAppPreHomeScreenWebView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? Appearing { get; set; }
            Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
            Func<Uri, Task>? NavigationFailed { get; set; }

            Func<Task>? GetNotificationsStatusRequested { get; set; }
            Func<Task>? GoToLoggedInHomeRequested { get; set; }
            Func<Task>? LogoutRequested { get; set; }
            Func<Task>? ResetAndShowErrorRequested { get; set; }
            Func<string, Task>? GetPnsTokenRequested { get; set; }
            Func<Uri, Task>? DeeplinkRequested { get; set; }
            Func<Task>? SessionExpiredRequested { get; set; }
            Func<Task>? OnSessionExpiringRequested { get; set; }
        }

        void GoToUri(Uri uri);
        Task SendNotificationsStatus(string status);
        Task SendNotificationAuthorised(NotificationAuthorisedResponse authorisedResponse);
        Task SendNotificationUnauthorised();
        Task SendSessionExtend();
        Task Logout();
    }
}
