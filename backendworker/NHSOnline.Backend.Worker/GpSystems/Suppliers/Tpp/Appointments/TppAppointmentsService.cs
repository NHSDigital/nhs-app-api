using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;

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

        public async Task<AppointmentBookResult> Book(UserSession userSession, AppointmentBookRequest request)
        {
            return await _booker.Book((TppUserSession)userSession, request);
        }

        public async Task<AppointmentCancelResult> Cancel(UserSession userSession, AppointmentCancelRequest request)
        {
            return await _canceller.Cancel((TppUserSession) userSession, request);
        }

        public async Task<AppointmentsResult> GetAppointments(UserSession userSession, bool includePastAppointments,
            DateTimeOffset? pastAppointmentsFromDate)
        {
            return await _getter.GetAppointments((TppUserSession) userSession);
        }
    }
}
