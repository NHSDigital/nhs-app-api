using System;
using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration
{
    internal interface INhsLoginUpliftView: INavigationView<INhsLoginUpliftView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? Appearing { get; set; }
            Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
            Func<Task>? BackRequested { get; set; }

            Func<ISelectMediaRequest, Task>? SelectMediaRequested { get; set; }
        }

        void GoToUri(Uri uri);

    }
}