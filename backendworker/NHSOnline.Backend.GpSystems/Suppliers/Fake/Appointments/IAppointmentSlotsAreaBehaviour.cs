using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments
{
    [FakeGpArea("AppointmentSlots")]
    public interface IAppointmentSlotsAreaBehaviour
    {
        Task<AppointmentSlotsResult> GetSlots(GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentSlotsDateRange dateRange);
    }
}