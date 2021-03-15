using System;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface INhsLoginTermsAndConditionsDeclinedView
    {
        public event EventHandler<EventArgs>? BackToHomeRequested;
        public event EventHandler<EventArgs>? OneOneOneRequested;

        INavigation Navigation { get; }
    }
}