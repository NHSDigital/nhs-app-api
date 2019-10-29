using NHSOnline.Backend.Support;
using System.Threading.Tasks;

namespace NHSOnline.Backend.GpSystems.Appointments
{
    public interface IAppointmentSlotsService
    {   
        Task<AppointmentSlotsResult> GetSlots(GpLinkedAccountModel gpLinkedAccountModel, AppointmentSlotsDateRange dateRange);
    }
}
