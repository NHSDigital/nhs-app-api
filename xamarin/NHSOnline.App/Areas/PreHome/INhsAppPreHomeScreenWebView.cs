using System;
using System.Threading.Tasks;
using NHSOnline.App.Areas.Cookies;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.PreHome
{
    internal interface INhsAppPreHomeScreenWebView : ICookieView
    {
        INavigation Navigation { get; }
        Func<Task>? Appearing { get; set; }
        Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
        Func<WebNavigatedEventArgs, Task>? Navigated { get; set; }

        Func<Task>? GetNotificationsStatusRequested { get; set; }
        Func<Task>? GoToLoggedInHomeRequested { get; set; }
        Func<Task>? ResetAndShowErrorRequested { get; set; }

        void GoToUri(Uri uri);
        Task SendNotificationsStatus(string status);
    }
}
