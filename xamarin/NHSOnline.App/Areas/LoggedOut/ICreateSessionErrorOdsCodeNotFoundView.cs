using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ICreateSessionErrorOdsCodeNotFoundView: INavigationView<ICreateSessionErrorOdsCodeNotFoundView.IEvents>
    {
        string ServiceDeskReference { get; set; }

        internal interface IEvents
        {
            Func<Task>? OneOneOneRequested { get; set; }
            Func<Task>? ContactUsRequested { get; set; }
            Func<Task>? CovidStatusServiceRequested { get; set; }
            Func<Task>? CovidPassRequested { get; set; }
        }
    }
}