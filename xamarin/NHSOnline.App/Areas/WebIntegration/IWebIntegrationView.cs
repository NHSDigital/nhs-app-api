using System;
using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Controls;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration
{
    internal interface IWebIntegrationView: INavigationView<IWebIntegrationView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? Appearing { get; set; }

            Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }

            Func<Task>? HelpRequested { get; set; }
            Func<Task>? HomeRequested { get; set; }
            Func<Task>? AdviceRequested { get; set; }
            Func<Task>? AppointmentsRequested { get; set; }
            Func<Task>? PrescriptionsRequested { get; set; }
            Func<Task>? YourHealthRequested { get; set; }
            Func<Task>? MoreRequested { get; set; }
            Func<Task>? MessagesRequested { get; set; }

            Func<string, Task>? RedirectToNhsAppPageRequested { get; set; }
            Func<Uri, Task>? DeepLinkRequested { get; set; }

            Func<AddEventToCalendarRequest, Task>? AddEventToCalendarRequested { get; set; }
            Func<DownloadRequest, Task>? StartDownloadRequested { get; set; }
        }

        void GoToUri(Uri uri);

        void SetNavigationFooterItem(NavigationFooterItem footerItem);
    }
}
