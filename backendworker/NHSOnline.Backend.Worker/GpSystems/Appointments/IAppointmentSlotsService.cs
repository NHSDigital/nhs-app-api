using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.GpSystems.Appointments
{
    public interface IAppointmentSlotsService
    {   
        Task<AppointmentSlotsResult> GetSlots(GpUserSession gpUserSession, AppointmentSlotsDateRange dateRange);
    }
}
