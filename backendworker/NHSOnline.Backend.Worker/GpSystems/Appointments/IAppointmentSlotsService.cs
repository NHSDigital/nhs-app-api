using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.GpSystems.Appointments
{
    public interface IAppointmentSlotsService
    {   
        Task<AppointmentSlotsResult> GetSlots(UserSession userSession, AppointmentSlotsDateRange dateRange);
    }
}
