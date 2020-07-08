using System;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ILoggedOutHomeScreenView
    {
        event EventHandler<EventArgs> LoginRequested;
        event EventHandler<EventArgs> NhsUkCovidConditionsServicePageRequested;
        event EventHandler<EventArgs> NhsUkLoginHelpServicePageRequested;

        INavigation Navigation { get; }
    }
}