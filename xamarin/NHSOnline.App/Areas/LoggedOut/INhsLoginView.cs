using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface INhsLoginView
    {
        event EventHandler<EventArgs> BackRequested;

        Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
        Func<Task>? NavigationFailed { get; set; }

        void LoadUrlAndNotifyOnRedirect(Uri uri, Func<Uri, bool> isRedirect, Action<Uri> redirected);

        INavigation Navigation { get; }
    }
}