using System;
using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Controls;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration
{
    internal interface IWebIntegrationView: INavigationView<IWebIntegrationView.IEvents>, ITryAgainWebview
    {
        internal interface IEvents
        {
            Func<Task>? Appearing { get; set; }

            Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
            Func<Task>? ShowTryAgainNetworkErrorRequested { get; set; }
            Func<Task>? ShowBackToHomeNetworkErrorRequested { get; set; }

            Func<Task>? HelpRequested { get; set; }
            Func<Task>? HomeRequested { get; set; }
            Func<Task>? AdviceRequested { get; set; }
            Func<Task>? AppointmentsRequested { get; set; }
            Func<Task>? PrescriptionsRequested { get; set; }
            Func<Task>? YourHealthRequested { get; set; }
            Func<Task>? MoreRequested { get; set; }
            Func<Task>? MessagesRequested { get; set; }

            Func<string, Task>? GoToNhsAppPageRequested { get; set; }
            Func<Uri, Task>? DeepLinkRequested { get; set; }
            Func<Task>? ReloadInitialUrlRequested { get; set; }

            Func<AddEventToCalendarRequest, Task>? AddEventToCalendarRequested { get; set; }
            Func<DownloadRequest, Task>? StartDownloadRequested { get; set; }
        }

        void GoToUri(Uri uri);

        void SetNavigationFooterItem(NavigationFooterItem footerItem);
    }
}
