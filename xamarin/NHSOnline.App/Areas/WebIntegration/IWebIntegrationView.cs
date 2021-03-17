using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration
{
    internal interface IWebIntegrationView: INavigationView<IWebIntegrationView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? Appearing { get; set; }

            Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }

            Func<Task>? SettingsRequested { get; set; }
            Func<Task>? HelpRequested { get; set; }
            Func<Task>? HomeRequested { get; set; }
            Func<Task>? SymptomsRequested { get; set; }
            Func<Task>? AppointmentsRequested { get; set; }
            Func<Task>? PrescriptionsRequested { get; set; }
            Func<Task>? RecordRequested { get; set; }
            Func<Task>? MoreRequested { get; set; }

            Func<string, Task>? RedirectToNhsAppPageRequested { get; set; }
        }

        void GoToUri(Uri uri);
    }
}
