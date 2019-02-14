using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Appointments
{
    public interface IAppointmentsService
    {
        Task<AppointmentBookResult> Book(GpUserSession gpUserSession, AppointmentBookRequest request);

        Task<AppointmentCancelResult> Cancel(GpUserSession gpUserSession, AppointmentCancelRequest request);

        Task<AppointmentsResult> GetAppointments(GpUserSession gpUserSession);
    }
}