using System;
using System.Net;
using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.KnownServices;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.Home
{
    internal interface INhsAppWebView
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

        event EventHandler<EventArgs> ResetAndShowErrorRequested;

        Func<OpenWebIntegrationRequest, Task>? OpenWebIntegrationRequested { get; set; }

        INavigation Navigation { get; }
        void GoToUri(Uri uri);
        void NavigateWithinApp(string spaPath);

        Task AddCookie(Cookie cookie);
    }
}
