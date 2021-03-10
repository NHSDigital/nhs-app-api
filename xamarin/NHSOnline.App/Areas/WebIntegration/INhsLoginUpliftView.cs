using System;
using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Controls.WebViews.Payloads.Paycasso;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration
{
    internal interface INhsLoginUpliftView
    {
        Func<Task>? Appearing { get; set; }
        Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
        Func<Task>? BackRequested { get; set; }

        Func<ISelectMediaRequest, Task>? SelectMediaRequested { get; set; }
        Func<LaunchPaycassoRequest, Task>? LaunchPaycassoRequested { get; set; }

        INavigation Navigation { get; }

        void GoToUri(Uri uri);

        Task PaycassoOnSuccess(string response);
        Task PaycassoOnFailure(string error);
    }
}