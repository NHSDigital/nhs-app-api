using System;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ICreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberView
    {
        public event EventHandler<EventArgs>? MyHealthOnlineRequested;
        public event EventHandler<EventArgs>? OneOneOneWalesRequested;
        public event EventHandler<EventArgs>? OneOneOneRequested;
        public event EventHandler<EventArgs>? ContactUsRequested;

        public string ServiceDeskReference { get; set; }

        INavigation Navigation { get; }
    }
}