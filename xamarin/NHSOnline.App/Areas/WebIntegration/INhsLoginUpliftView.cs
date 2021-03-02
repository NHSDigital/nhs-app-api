using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration
{
    internal interface INhsLoginUpliftView
    {
        Func<Task>? Appearing { get; set; }
        Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
        Func<Task>? BackRequested { get; set; }

        INavigation Navigation { get; }

        void GoToUri(Uri uri);
    }
}