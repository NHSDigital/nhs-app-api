using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Bridges.Emis;
using NHSOnline.Backend.Worker.Session;

namespace NHSOnline.Backend.Worker.Router.Appointments
{
    public interface IAppointmentsService
    {
        Task<AppointmentBookResult> Book(UserSession userSession, AppointmentBookRequest request);
    }
}