using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
{
    public class TppAppointmentsService : IAppointmentsService
    {
        private readonly TppAppointmentsRetrievalService _getter;
        private readonly TppAppointmentsBookingService _booker;
        private readonly TppAppointmentsCancellationService _canceller;

        public TppAppointmentsService(
            TppAppointmentsRetrievalService getter,
            TppAppointmentsBookingService booker,
            TppAppointmentsCancellationService canceller)
        {
            _getter = getter;
            _booker = booker;
            _canceller = canceller;
        }

        public async Task<AppointmentBookResult> Book(GpUserSession gpUserSession, AppointmentBookRequest request)
        {
            return await _booker.Book((TppUserSession) gpUserSession, request);
        }

        public async Task<AppointmentCancelResult> Cancel(GpUserSession gpUserSession, AppointmentCancelRequest request)
        {
            return await _canceller.Cancel((TppUserSession) gpUserSession, request);
        }

        public async Task<AppointmentsResult> GetAppointments(GpUserSession gpUserSession)
        {
            return await _getter.GetAppointments((TppUserSession) gpUserSession);
        }
    }
}
