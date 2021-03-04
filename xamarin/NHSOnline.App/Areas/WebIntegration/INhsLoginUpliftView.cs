using System;
using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration
{
    internal interface INhsLoginUpliftView
    {
        Func<Task>? Appearing { get; set; }
        Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
        Func<Task>? BackRequested { get; set; }

        Func<ISelectMediaRequest, Task>? SelectMediaRequested { get; set; }

        INavigation Navigation { get; }

        void GoToUri(Uri uri);
    }
}