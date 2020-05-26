using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments
{
    public class DefaultAppointmentSlotsBehaviour : IAppointmentSlotsBehaviour
    {
        public async Task<AppointmentSlotsResult> GetSlots(GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentSlotsDateRange dateRange)
        {
            return await Task.FromResult<AppointmentSlotsResult>(new AppointmentSlotsResult.Forbidden());
        }
    }
}