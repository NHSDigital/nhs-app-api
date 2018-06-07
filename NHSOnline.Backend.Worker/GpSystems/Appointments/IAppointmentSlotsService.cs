using System;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.GpSystems.Appointments
{
    public interface IAppointmentSlotsService
    {   
        Task<AppointmentSlotsResult> Get(UserSession userSession, DateTimeOffset fromDate, DateTimeOffset toDate);
    }
}
