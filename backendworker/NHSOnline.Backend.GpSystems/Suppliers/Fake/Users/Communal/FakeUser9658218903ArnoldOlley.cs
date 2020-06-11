using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Appointments;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Communal
{
    public class FakeUser9658218903ArnoldOlley : FakeUser
    {
        public FakeUser9658218903ArnoldOlley(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9658218903";

        public override Guid Id => Guid.Parse("11719623-7a50-4ec0-8a58-67ad08de8834");

        public override string EmailAddress => "testuserlive+5@demo.signin.nhs.uk";

        public override string NhsNumber => NhsNumberInternal;

        public override string OdsCode => "A20047";

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
    }
}