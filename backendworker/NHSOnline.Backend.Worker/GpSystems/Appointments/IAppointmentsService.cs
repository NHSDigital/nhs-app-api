using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Appointments
{
    public interface IAppointmentsService
    {
        Task<AppointmentBookResult> Book(GpUserSession gpUserSession, AppointmentBookRequest request);

        Task<AppointmentCancelResult> Cancel(GpUserSession gpUserSession, AppointmentCancelRequest request);

        Task<AppointmentsResult> GetAppointments(GpUserSession gpUserSession);
    }
}