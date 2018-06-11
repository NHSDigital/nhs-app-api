using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Appointments
{
    public interface IAppointmentsService
    {
        Task<AppointmentBookResult> Book(UserSession userSession, AppointmentBookRequest request);

        Task<MyAppointmentsResult> GetMyAppointments(UserSession userSession, bool includePastAppointments,
            DateTimeOffset? pastAppointmentsFromDate);
    }
}