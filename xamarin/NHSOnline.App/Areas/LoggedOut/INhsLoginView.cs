using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface INhsLoginView: INavigationView<INhsLoginView>
    {

        Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
        Func<Task>? NavigationFailed { get; set; }
        Func<Task>? BackRequested { get; set; }

        void LoadUrlAndNotifyOnRedirect(Uri uri, Func<Uri, bool> isRedirect, Action<Uri> redirected);
    }
}