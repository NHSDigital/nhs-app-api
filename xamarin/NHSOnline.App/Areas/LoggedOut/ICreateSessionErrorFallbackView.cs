using System;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ICreateSessionErrorFallbackView
    {
        event EventHandler<EventArgs>? OneOneOneRequested;
        event EventHandler<EventArgs>? ContactUsRequested;
        event EventHandler<EventArgs>? BackHomeRequested;

        INavigation Navigation { get; }
    }
}