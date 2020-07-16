using System;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ICreateSessionErrorBadRequestView
    {
        public event EventHandler<EventArgs>? BackHomeRequested;
        public event EventHandler<EventArgs>? ContactUsRequested;

        public string ServiceDeskReference { get; set; }

        INavigation Navigation { get; }
    }
}