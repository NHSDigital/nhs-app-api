using System;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration
{
    internal interface IWebIntegrationView
    {
        event EventHandler Appearing;

        event EventHandler<EventArgs> SettingsRequested;
        event EventHandler<EventArgs> HelpRequested;
        event EventHandler<EventArgs> HomeRequested;
        event EventHandler<EventArgs> SymptomsRequested;
        event EventHandler<EventArgs> AppointmentsRequested;
        event EventHandler<EventArgs> PrescriptionsRequested;
        event EventHandler<EventArgs> RecordRequested;
        event EventHandler<EventArgs> MoreRequested;

        INavigation Navigation { get; }
        void GoToUri(Uri uri);
        void NavigateWithinApp(string spaPath);
    }
}