using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ICreateSessionView: INavigationView<ICreateSessionView.IEvents>
    {
        internal interface IEvents
        {
            Func<Uri, Task>? DeeplinkRequested { get; set; }
        }
    }
}