using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ICreateSessionErrorOdsCodeNotSupportedView: INavigationView<ICreateSessionErrorOdsCodeNotSupportedView.IEvents>
    {
        string ServiceDeskReference { get; set; }

        internal interface IEvents
        {
            Func<Task>? MyHealthOnlineRequested { get; set; }
            Func<Task>? OneOneOneWalesRequested { get; set; }
            Func<Task>? OneOneOneRequested { get; set; }
            Func<Task>? ContactUsRequested { get; set; }
            Func<Task>? CovidPassRequested { get; set; }
            Func<Task>? GpOutOfHoursServiceRequested { get; set; }
        }
    }
}