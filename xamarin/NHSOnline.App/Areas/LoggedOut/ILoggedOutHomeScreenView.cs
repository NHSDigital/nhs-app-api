using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ILoggedOutHomeScreenView: INavigationView<ILoggedOutHomeScreenView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? LoginRequested { get; set; }
            Func<Task>? NhsUkCovidConditionsServicePageRequested { get; set; }
            Func<Task>? NhsUkLoginHelpServicePageRequested { get; set; }
            Func<Task>? BackRequested { get; set; }
            Func<Task>? ResetAndShowErrorRequested { get; set; }
        }
    }
}
