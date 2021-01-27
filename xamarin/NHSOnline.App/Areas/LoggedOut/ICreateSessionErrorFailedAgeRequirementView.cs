using System;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ICreateSessionErrorFailedAgeRequirementView
    {
        public event EventHandler<EventArgs>? OneOneOneRequested;

        public string ServiceDeskReference { get; set; }

        INavigation Navigation { get; }
    }
}