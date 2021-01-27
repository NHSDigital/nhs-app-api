using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration
{
    internal interface IWebIntegrationView
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

        INavigation Navigation { get; }

        void GoToUri(Uri uri);
    }
}
