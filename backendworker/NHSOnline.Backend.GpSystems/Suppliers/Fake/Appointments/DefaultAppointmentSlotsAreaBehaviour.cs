using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments
{
    [FakeGpAreaBehaviour(Behaviour.Default)]
    public class DefaultAppointmentSlotsAreaBehaviour : IAppointmentSlotsAreaBehaviour
    {
        public async Task<AppointmentSlotsResult> GetSlots(GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentSlotsDateRange dateRange)
        {
            return await Task.FromResult<AppointmentSlotsResult>(new AppointmentSlotsResult.Forbidden());
        }
    }
}