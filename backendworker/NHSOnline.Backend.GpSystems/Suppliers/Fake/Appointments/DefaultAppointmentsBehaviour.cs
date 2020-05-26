using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments
{
    public class DefaultAppointmentsBehaviour : IAppointmentsBehaviour
    {
        public async Task<AppointmentBookResult> Book(GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentBookRequest request)
        {
            return await Task.FromResult<AppointmentBookResult>(new AppointmentBookResult.Forbidden());
        }

        public async Task<AppointmentCancelResult> Cancel(GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentCancelRequest request)
        {
            return await Task.FromResult<AppointmentCancelResult>(new AppointmentCancelResult.Forbidden());
        }

        public async Task<AppointmentsResult> GetAppointments(GpLinkedAccountModel gpLinkedAccountModel)
        {
            return await Task.FromResult<AppointmentsResult>(new AppointmentsResult.Forbidden());
        }
    }
}