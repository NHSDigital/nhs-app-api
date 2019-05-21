using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    public interface IAppointmentsResultBuilder
    {
        Option<AppointmentsResult> Build(
            Task<TppClient.TppApiObjectResponse<ViewAppointmentsReply>> viewPastAppointmentsTask,
            Task<TppClient.TppApiObjectResponse<ViewAppointmentsReply>> viewUpcomingAppointmentsTask);
    }
}