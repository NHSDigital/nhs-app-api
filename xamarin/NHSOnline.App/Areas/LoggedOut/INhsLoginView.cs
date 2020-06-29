using System;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface INhsLoginView
    {
        event EventHandler<EventArgs> NavigationFailed;

        void LoadUrlAndNotifyOnRedirect(Uri uri, Func<Uri, bool> isRedirect, Action<Uri> redirected);

        INavigation Navigation { get; }
    }
}