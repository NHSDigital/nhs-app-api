using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
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
