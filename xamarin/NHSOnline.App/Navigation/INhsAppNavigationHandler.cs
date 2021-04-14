using System.Threading.Tasks;

namespace NHSOnline.App.Navigation
{
    internal interface INhsAppNavigationHandler
    {
        Task HomeRequested();
        Task SymptomsRequested();
        Task AppointmentsRequested();
        Task PrescriptionsRequested();
        Task RecordRequested();
        Task MoreRequested();
        Task MessagesRequested();

        Task RedirectToNhsAppPageRequested(string page);
    }
}
