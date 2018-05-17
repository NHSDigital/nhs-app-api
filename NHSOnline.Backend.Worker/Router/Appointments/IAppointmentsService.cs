using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;

namespace NHSOnline.Backend.Worker.Router.Appointments
{
    public interface IAppointmentsService
    {
        Task<AppointmentBookResult> Book(UserSession userSession, AppointmentBookRequest request);
    }
}