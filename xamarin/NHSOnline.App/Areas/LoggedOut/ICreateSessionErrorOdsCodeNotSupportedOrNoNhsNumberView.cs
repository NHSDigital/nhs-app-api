using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ICreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberView: INavigationView<ICreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberView.IEvents>
    {
        string ServiceDeskReference { get; set; }

        internal interface IEvents
        {
            Func<Task>? MyHealthOnlineRequested { get; set; }
            Func<Task>? OneOneOneWalesRequested { get; set; }
            Func<Task>? OneOneOneRequested { get; set; }
            Func<Task>? ContactUsRequested { get; set; }
        }
    }
}