using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments
{
    public interface IAppointmentsBehaviour
    {
        Task<AppointmentBookResult> Book(GpLinkedAccountModel gpLinkedAccountModel, AppointmentBookRequest request);

        Task<AppointmentCancelResult> Cancel(GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentCancelRequest request);

        Task<AppointmentsResult> GetAppointments(GpLinkedAccountModel gpLinkedAccountModel);
    }
}