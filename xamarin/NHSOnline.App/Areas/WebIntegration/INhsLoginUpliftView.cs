using System;
using System.Threading.Tasks;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration
{
    internal interface INhsLoginUpliftView: INavigationView<INhsLoginUpliftView.IEvents>, IAccessibleControl
    {
        internal interface IEvents
        {
            Action<WebNavigatingEventArgs>? Navigating { get; set; }
            Func<Task>? NavigationFailed { get; set; }

            Func<Task>? BackRequested { get; set; }
            Func<ISelectMediaRequest, Task>? SelectMediaRequested { get; set; }
        }

        void GoToUri(Uri uri);
        void LoadUrlAndNotifyOnRedirect(Uri uri, Func<Uri, bool> isRedirect, Action<Uri> redirected);

    }
}