using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Appointments
{
    public interface IAppointmentsService
    {
        Task<AppointmentBookResult> Book(GpLinkedAccountModel gpLinkedAccountModel, AppointmentBookRequest request);

        Task<AppointmentCancelResult> Cancel(GpLinkedAccountModel gpLinkedAccountModel, AppointmentCancelRequest request);

        Task<AppointmentsResult> GetAppointments(GpLinkedAccountModel gpLinkedAccountModel);
    }
}