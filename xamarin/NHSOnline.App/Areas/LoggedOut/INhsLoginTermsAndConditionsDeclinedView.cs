using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal interface INhsLoginTermsAndConditionsDeclinedView: INavigationView<INhsLoginTermsAndConditionsDeclinedView.IEvents>
    {
        internal interface IEvents
        {
            Func<Task>? BackToHomeRequested { get; set; }
            Func<Task>? OneOneOneRequested { get; set; }
        }
    }
}