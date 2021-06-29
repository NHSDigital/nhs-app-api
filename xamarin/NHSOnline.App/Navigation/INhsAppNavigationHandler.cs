using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Navigation
{
    internal interface INhsAppNavigationHandler
    {
        Task HomeRequested();
        Task AdviceRequested();
        Task AppointmentsRequested();
        Task PrescriptionsRequested();
        Task YourHealthRequested();
        Task MoreRequested();
        Task MessagesRequested();

        Task GoToNhsAppPageRequested(string page);
        Task RedirectToDeepLinkRequested(Uri deeplinkUrl);
    }
}
