using System;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ICreateSessionErrorUpstreamSystemTimeoutView
    {
        public event EventHandler<EventArgs>? OneOneOneRequested;
        public event EventHandler<EventArgs>? ContactUsRequested;
        public event EventHandler<EventArgs>? BackHomeRequested;

        public string ServiceDeskReference { get; set; }

        INavigation Navigation { get; }
    }
}