using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Substrakt
{
    public class FakeUser9990555168RudyKilby : FakeUser
    {
        public FakeUser9990555168RudyKilby(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990555168";

        public override Guid Id => Guid.Parse("e6fe3236-21dd-4f47-9d18-c9d5768b0c66");
        public override string EmailAddress => "onboarding.nhsapp+substrakt.user3@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00004";
        public override string GivenName => "Rudy";
        public override string FamilyName => "Kilby";
        public override DateTime DateOfBirth => new DateTime(1946,8,4);
        public override string Sex => "Male";

        public override IEnumerable<string> LinkedAccountsNhsNumbers =>
            new[]
            {
                FakeUser9990555176HarrietSlack.NhsNumberInternal,
                FakeUser9990555192MiaMcelroy.NhsNumberInternal,
            };
    }
}