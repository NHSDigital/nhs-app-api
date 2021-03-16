using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ICreateSessionErrorFallbackView: INavigationView<ICreateSessionErrorFallbackView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? OneOneOneRequested { get; set; }
            Func<Task>? ContactUsRequested { get; set; }
            Func<Task>? BackHomeRequested { get; set; }
        }
    }
}