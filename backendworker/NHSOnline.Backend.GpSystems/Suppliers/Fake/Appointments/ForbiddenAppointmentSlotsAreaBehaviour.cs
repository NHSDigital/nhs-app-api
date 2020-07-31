using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments
{
    [FakeGpAreaBehaviour(Behaviour.Forbidden)]
    public class ForbiddenAppointmentSlotsBehaviour : IAppointmentSlotsAreaBehaviour
    {
        public Task<AppointmentSlotsResult> GetSlots(
            GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentSlotsDateRange dateRange) =>
            Task.FromResult<AppointmentSlotsResult>(new AppointmentSlotsResult.Forbidden());
    }
}