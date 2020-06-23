using System;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface IBeforeYouStartView
    {
        event EventHandler<EventArgs> LoginRequested;

        INavigation Navigation { get; }
    }
}