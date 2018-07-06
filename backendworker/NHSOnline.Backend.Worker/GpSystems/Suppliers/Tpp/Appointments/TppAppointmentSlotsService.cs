using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Appointments;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
{
    public class TppAppointmentSlotsService : IAppointmentSlotsService
    {
        public async Task<AppointmentSlotsResult> Get(UserSession userSession, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            throw new NotImplementedException();
        }
    }
}
