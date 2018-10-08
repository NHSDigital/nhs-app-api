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

        public VisionAppointmentsService(VisionAppointmentsRetrievalService getter)
        {
            _getter = getter;
        }
        
        public Task<AppointmentBookResult> Book(UserSession userSession, AppointmentBookRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<AppointmentCancelResult> Cancel(UserSession userSession, AppointmentCancelRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<AppointmentsResult> GetAppointments(UserSession userSession, 
            bool includePastAppointments,
            DateTimeOffset? pastAppointmentsFromDate)
        {
            return await _getter.GetAppointments((VisionUserSession) userSession);
        }
    }
}
