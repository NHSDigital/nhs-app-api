using NHSOnline.Backend.Support;
using System.Threading.Tasks;

namespace NHSOnline.Backend.GpSystems.Appointments
{
    public interface IAppointmentSlotsService
    {   
        Task<AppointmentSlotsResult> GetSlots(GpUserSession gpUserSession, AppointmentSlotsDateRange dateRange);
    }
}
