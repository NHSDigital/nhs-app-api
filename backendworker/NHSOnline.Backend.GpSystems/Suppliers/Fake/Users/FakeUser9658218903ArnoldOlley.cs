using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9658218903ArnoldOlley : FakeUser
    {
        public override string NhsNumber => "9658218903";
        public override string GivenName => "Arnold";
        public override string FamilyName => "Olley";
        public override DateTime DateOfBirth => new DateTime(1939,7,21);
        public override string Sex => "Male";

        public override IAppointmentSlotsBehaviour AppointmentSlotsBehaviour
            => new UserAppointmentSlotsBehaviour();

        private class UserAppointmentSlotsBehaviour : IAppointmentSlotsBehaviour
        {
            public async Task<AppointmentSlotsResult> GetSlots(GpLinkedAccountModel gpLinkedAccountModel,
                AppointmentSlotsDateRange dateRange)
            {
                return await Task.FromResult<AppointmentSlotsResult>(new AppointmentSlotsResult.BadGateway());
            }
        }

        public FakeUser9658218903ArnoldOlley(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}