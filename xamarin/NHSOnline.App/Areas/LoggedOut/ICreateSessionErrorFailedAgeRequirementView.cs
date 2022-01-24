using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ICreateSessionErrorFailedAgeRequirementView: INavigationView<ICreateSessionErrorFailedAgeRequirementView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? OneOneOneRequested { get; set; }
            Func<Task>? DigitalCovidPassRequested { get; set; }
            Func<Task>? PaperCovidPassRequested { get; set; }
        }
    }
}
