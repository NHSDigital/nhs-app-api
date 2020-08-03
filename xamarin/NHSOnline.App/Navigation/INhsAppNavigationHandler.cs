using System.Threading.Tasks;

namespace NHSOnline.App.Navigation
{
    internal interface INhsAppNavigationHandler
    {
        Task SettingsRequested();
        Task HomeRequested();
        Task SymptomsRequested();
        Task AppointmentsRequested();
        Task PrescriptionsRequested();
        Task RecordRequested();
        Task MoreRequested();
    }
}