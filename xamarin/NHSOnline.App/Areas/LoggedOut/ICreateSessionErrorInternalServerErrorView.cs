using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ICreateSessionErrorInternalServerErrorView: INavigationView<ICreateSessionErrorInternalServerErrorView.IEvents>
    {
        string ServiceDeskReference { get; set; }

        internal interface IEvents
        {
            Func<Task>? OneOneOneRequested { get; set; }
            Func<Task>? BackHomeRequested { get; set; }
            Func<Task>? DigitalCovidCertRequested { get; set; }
        }
    }
}