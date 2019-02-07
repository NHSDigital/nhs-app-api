using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public class EmisAppointmentsService : IAppointmentsService
    {
        private readonly EmisAppointmentsBookingService _booker;
        private readonly EmisAppointmentsCancellationService _canceller;
        private readonly EmisAppointmentsRetrievalService _getter;

        public EmisAppointmentsService(
            EmisAppointmentsRetrievalService getter,
            EmisAppointmentsBookingService booker,
            EmisAppointmentsCancellationService canceller)
        {
            _getter = getter;
            _booker = booker;
            _canceller = canceller;
        }

        public async Task<AppointmentBookResult> Book(GpUserSession gpUserSession, AppointmentBookRequest request)
        {
            return await _booker.Book((EmisUserSession) gpUserSession, request);
        }

        public async Task<AppointmentCancelResult> Cancel(GpUserSession gpUserSession, AppointmentCancelRequest request)
        {
            return await _canceller.Cancel((EmisUserSession) gpUserSession, request);
        }

        public async Task<AppointmentsResult> GetAppointments(GpUserSession gpUserSession)
        {
            return await _getter.GetAppointments((EmisUserSession) gpUserSession);
        }
    }
}
