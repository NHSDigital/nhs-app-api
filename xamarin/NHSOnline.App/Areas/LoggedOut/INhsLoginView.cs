using System;
using System.Threading.Tasks;
using NHSOnline.App.Controls;
using NHSOnline.App.Events.Models;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface INhsLoginView: INavigationView<INhsLoginView.IEvents>, IAccessibleControl
    {
        void LoadUrlAndNotifyOnRedirect(Uri uri, Func<Uri, bool> isRedirect, Action<Uri> redirected);

        internal interface IEvents
        {
            Action<WebNavigatingEventArgs>? Navigating { get; set; }
            Func<NavigationFailedArgs, Task>? NavigationFailed { get; set; }

            Func<Task>? BackRequested { get; set; }
            Func<Uri, Task>? DeeplinkRequested { get; set; }
        }

        void GoToUri(Uri uri);
    }
}