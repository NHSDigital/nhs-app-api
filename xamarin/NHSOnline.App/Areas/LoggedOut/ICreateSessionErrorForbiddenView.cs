using System;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ICreateSessionErrorForbiddenView
    {
        event EventHandler<EventArgs>? OneOneOneRequested;
        event EventHandler<EventArgs>? BackHomeRequested;
        event EventHandler<EventArgs>? ContactUsRequested;

        string ServiceDeskReference { get; set; }

        INavigation Navigation { get; }
    }
}