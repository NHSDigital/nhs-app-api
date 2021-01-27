using System;
using System.Net;
using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.Home
{
    internal interface INhsAppWebView
    {
        Func<Task>? Appearing { get; set; }
        Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
        Func<WebNavigatedEventArgs, Task>? Navigated { get; set; }

        Func<Task>? SettingsRequested { get; set; }
        Func<Task>? HelpRequested { get; set; }
        Func<Task>? HomeRequested { get; set; }
        Func<Task>? SymptomsRequested { get; set; }
        Func<Task>? AppointmentsRequested { get; set; }
        Func<Task>? PrescriptionsRequested { get; set; }
        Func<Task>? RecordRequested { get; set; }
        Func<Task>? MoreRequested { get; set; }

        Func<Task>? ResetAndShowErrorRequested { get; set; }

        Func<OpenWebIntegrationRequest, Task>? OpenWebIntegrationRequested { get; set; }

        INavigation Navigation { get; }

        void GoToUri(Uri uri);
        Task NavigateWithinApp(string spaPath);
        Task AddCookie(Cookie cookie);
    }
}
