using System;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.Router.Appointments
{
    public interface IAppointmentSlotsService
    {   
        Task<AppointmentSlotsResult> Get(DateTimeOffset fromDate, DateTimeOffset toDate);
    }
}
