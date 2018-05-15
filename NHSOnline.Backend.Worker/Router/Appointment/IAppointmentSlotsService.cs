using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Session;

namespace NHSOnline.Backend.Worker.Router.Appointment
{
    public interface IAppointmentSlotsService
    {   
        Task<AppointmentSlotsResult> Get(DateTimeOffset fromDate, DateTimeOffset toDate);
    }
}
