using System;
using System.Threading.Tasks;
using NHSOnline.App.Controls;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration
{
    internal interface INhsLoginOnDemandGpSessionView: INavigationView<INhsLoginOnDemandGpSessionView.IEvents>, IAccessibleControl
    {
        internal interface IEvents
        {
            Action<WebNavigatingEventArgs>? Navigating { get; set; }
            Func<Task>? NavigationFailed { get; set; }

            Func<Task>? BackRequested { get; set; }
            Func<Uri, Task>? DeeplinkRequested { get; set; }

            Func<Task>? HomeRequested { get; set; }
            Func<Task>? HelpRequested { get; set; }
            Func<Task>? MoreRequested { get; set; }
            Func<Task>? AdviceRequested { get; set; }
            Func<Task>? AppointmentsRequested { get; set; }
            Func<Task>? PrescriptionsRequested { get; set; }
            Func<Task>? YourHealthRequested { get; set; }
            Func<Task>? MessagesRequested { get; set; }
        }

        void LoadUrlAndNotifyOnRedirect(Uri uri, Func<Uri, bool> isRedirect, Action<Uri> redirected);
        void SetNavigationFooterItem(NavigationFooterItem footerItem);
    }
}