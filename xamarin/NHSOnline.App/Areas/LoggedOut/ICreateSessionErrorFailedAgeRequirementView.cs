using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ICreateSessionErrorFailedAgeRequirementView: INavigationView<ICreateSessionErrorFailedAgeRequirementView.IEvents>
    {
        string ServiceDeskReference { get; set; }

        internal interface IEvents
        {
            Func<Task>? OneOneOneRequested { get; set; }
        }
    }
}