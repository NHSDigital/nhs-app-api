using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public class MicrotestAppointmentsService : IAppointmentsService
    {
        public async Task<AppointmentBookResult> Book(GpUserSession gpUserSession, AppointmentBookRequest request)
        {
            return await Task.FromResult(new AppointmentBookResult.BadRequest());
        }

        public async Task<AppointmentCancelResult> Cancel(GpUserSession gpUserSession, AppointmentCancelRequest request)
        {
            return await Task.FromResult(new AppointmentCancelResult.BadRequest());
        }

        public async Task<AppointmentsResult> GetAppointments(GpUserSession gpUserSession)
        {
            var emptyResponse = new AppointmentsResponse();
            return await Task.FromResult(new AppointmentsResult.SuccessfullyRetrieved(emptyResponse));
        }
    }
}
