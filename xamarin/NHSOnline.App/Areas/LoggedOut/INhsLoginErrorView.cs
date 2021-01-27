using System;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface INhsLoginErrorView
    {
        public event EventHandler<EventArgs>? BackHomeRequested;
        public event EventHandler<EventArgs>? ContactUsRequested;

        INavigation Navigation { get; }

        public string ServiceDeskReference { get; set; }
    }
}