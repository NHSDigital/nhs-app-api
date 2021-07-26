using System;
using System.Threading.Tasks;
using NHSOnline.App.Controls;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration
{
    internal interface INhsLoginOnDemandGpSessionView: INavigationView<INhsLoginOnDemandGpSessionView.IEvents>
    {
        internal interface IEvents
        {
            Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
            Func<Task>? NavigationFailed { get; set; }
            Func<Task>? BackRequested { get; set; }
            Func<Uri, Task>? DeeplinkRequested { get; set; }
        }
        void LoadUrlAndNotifyOnRedirect(Uri uri, Func<Uri, bool> isRedirect, Action<Uri> redirected);

        void SetNavigationFooterItem(NavigationFooterItem footerItem);
    }
}