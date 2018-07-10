using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
{
    public class TppAppointmentsService : IAppointmentsService
    {
        public Task<AppointmentBookResult> Book(UserSession userSession, AppointmentBookRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<AppointmentCancelResult> Cancel(UserSession userSession, AppointmentCancelRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<AppointmentsResult> GetAppointments(UserSession userSession, bool includePastAppointments, DateTimeOffset? pastAppointmentsFromDate)
        {
            throw new NotImplementedException();
        }
    }
}
