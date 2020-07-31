using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments
{
    [FakeGpAreaBehaviour(Behaviour.Unauthorised)]
    public class UnauthorisedAppointmentSlotsBehaviour : IAppointmentSlotsAreaBehaviour
    {
        public Task<AppointmentSlotsResult> GetSlots(
                GpLinkedAccountModel gpLinkedAccountModel,
                AppointmentSlotsDateRange dateRange) =>
            throw new UnauthorisedGpSystemHttpRequestException();
    }
}