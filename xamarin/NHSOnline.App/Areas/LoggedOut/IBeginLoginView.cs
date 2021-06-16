using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface IBeginLoginView: INavigationView<IBeginLoginView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? Appearing { get; set; }
            Func<Uri, Task>? DeeplinkRequested { get; set; }
        }
    }
}