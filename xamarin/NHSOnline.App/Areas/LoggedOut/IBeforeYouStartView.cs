using System;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface IBeforeYouStartView
    {
        event EventHandler<EventArgs> LoginRequested;
        event EventHandler<EventArgs> NhsUkCovidServicePageRequested;
        event EventHandler<EventArgs> NhsUkConditionsServicePageRequested;
        event EventHandler<EventArgs> NhsUkOneOneOneServicePageRequested;

        INavigation Navigation { get; }
    }
}