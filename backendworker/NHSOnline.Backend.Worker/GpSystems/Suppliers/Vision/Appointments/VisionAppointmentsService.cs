using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments
{
    public class VisionAppointmentsService : IAppointmentsService
    {
        private readonly VisionAppointmentsRetrievalService _getter;
        private readonly VisionAppointmentsBookingService _booker;
        
        private readonly VisionAppointmentsCancellationService _canceller;

        public VisionAppointmentsService(
            VisionAppointmentsRetrievalService getter,
            VisionAppointmentsBookingService booker, 
            VisionAppointmentsCancellationService canceller)
        {
            _getter = getter;
            _booker = booker;
            _canceller = canceller;
        }
        
        public async Task<AppointmentBookResult> Book(UserSession userSession, AppointmentBookRequest request)
        {
            return await _booker.Book((VisionUserSession) userSession.GpUserSession, request);
        }

        public async Task<AppointmentCancelResult> Cancel(UserSession userSession, AppointmentCancelRequest request)
        {
            return await _canceller.Cancel((VisionUserSession) userSession.GpUserSession, request);
        }

        public async Task<AppointmentsResult> GetAppointments(UserSession userSession, 
            bool includePastAppointments,
            DateTimeOffset? pastAppointmentsFromDate)
        {
            return await _getter.GetAppointments(userSession);
        }
    }
}
