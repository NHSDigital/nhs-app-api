using System;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface IGettingStartedView
    {
        event EventHandler<EventArgs> LoginRequested;
        event EventHandler<EventArgs> NhsUkCovidServicePageRequested;
        event EventHandler<EventArgs> BackRequested;

        INavigation Navigation { get; }
    }
}