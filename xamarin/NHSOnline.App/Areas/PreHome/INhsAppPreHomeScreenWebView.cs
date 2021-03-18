using System;
using System.Threading.Tasks;
using NHSOnline.App.Areas.Cookies;
using NHSOnline.App.DependencyServices.Notifications;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.PreHome
{
    internal interface INhsAppPreHomeScreenWebView : INavigationView<INhsAppPreHomeScreenWebView.IEvents>, ICookieView
    {
        Func<Task>? Appearing { get; set; }
        Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
        Func<WebNavigatedEventArgs, Task>? Navigated { get; set; }

        Func<Task>? GetNotificationsStatusRequested { get; set; }
        Func<Task>? GoToLoggedInHomeRequested { get; set; }
        Func<Task>? ResetAndShowErrorRequested { get; set; }
        Func<string, Task>? GetPnsTokenRequested { get; set; }

        void GoToUri(Uri uri);
        Task SendNotificationsStatus(string status);
        Task SendNotificationAuthorised(NotificationAuthorisedResponse authorisedResponse);
        Task SendNotificationUnauthorised();

        internal interface IEvents
        {
        }
    }
}
