using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
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

        public async Task<AppointmentBookResult> Book(UserSession userSession, AppointmentBookRequest request)
        {
            return await _booker.Book((EmisUserSession) userSession, request);
        }

        public async Task<AppointmentCancelResult> Cancel(UserSession userSession, AppointmentCancelRequest request)
        {
            return await _canceller.Cancel((EmisUserSession) userSession, request);
        }

        public async Task<AppointmentsResult> GetAppointments(UserSession userSession, 
            bool includePastAppointments,
            DateTimeOffset? pastAppointmentsFromDate)
        {
            return await _getter.GetAppointments((EmisUserSession) userSession,
                includePastAppointments,
                pastAppointmentsFromDate);
        }
    }
}
