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
        private readonly VisionAppointmentsCancellationService _canceller;

        public VisionAppointmentsService(VisionAppointmentsRetrievalService getter, VisionAppointmentsCancellationService canceller)
        {
            _getter = getter;
            _canceller = canceller;
        }
        
        public Task<AppointmentBookResult> Book(UserSession userSession, AppointmentBookRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<AppointmentCancelResult> Cancel(UserSession userSession, AppointmentCancelRequest request)
        {
            return await _canceller.Cancel((VisionUserSession) userSession, request);
        }

        public async Task<AppointmentsResult> GetAppointments(UserSession userSession, 
            bool includePastAppointments,
            DateTimeOffset? pastAppointmentsFromDate)
        {
            return await _getter.GetAppointments((VisionUserSession) userSession);
        }
    }
}
