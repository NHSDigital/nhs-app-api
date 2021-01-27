using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface ILoggedOutHomeScreenView
    {
        event EventHandler<EventArgs> LoginRequested;
        event EventHandler<EventArgs> NhsUkCovidConditionsServicePageRequested;
        event EventHandler<EventArgs> NhsUkLoginHelpServicePageRequested;

        Func<Task>? ResetAndShowErrorRequested { get; set; }

        INavigation Navigation { get; }
    }
}
