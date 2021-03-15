using System;
using System.Threading.Tasks;
using NHSOnline.App.Areas.Cookies;
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

        void GoToUri(Uri uri);
        Task SendNotificationsStatus(string status);

        internal interface IEvents
        {
        }
    }
}
