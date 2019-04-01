using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public class MicrotestAppointmentsService : IAppointmentsService
    {
        private readonly MicrotestAppointmentsBookingService _booker;
        private readonly MicrotestAppointmentsRetrievalService _getter;
        
        public MicrotestAppointmentsService(
            MicrotestAppointmentsBookingService booker,
            MicrotestAppointmentsRetrievalService getter)
        {
            _getter = getter;
            _booker = booker;
        }
        
        public async Task<AppointmentBookResult> Book(GpUserSession gpUserSession, AppointmentBookRequest request)
        {
            return await _booker.Book((MicrotestUserSession) gpUserSession, request);
        }

        public async Task<AppointmentCancelResult> Cancel(GpUserSession gpUserSession, AppointmentCancelRequest request)
        {
            return await Task.FromResult(new AppointmentCancelResult.BadRequest());
        }

        public async Task<AppointmentsResult> GetAppointments(GpUserSession gpUserSession)
        {
            return await _getter.GetAppointments(gpUserSession);
        }
    }
}
