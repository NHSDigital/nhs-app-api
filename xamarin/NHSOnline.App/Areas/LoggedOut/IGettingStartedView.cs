using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface IGettingStartedView: INavigationView<IGettingStartedView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? LoginRequested { get; set; }
            Func<Task>? NhsUkCovidAppPageRequested { get; set; }
            Func<Task>? BackRequested { get; set; }
        }
    }
}