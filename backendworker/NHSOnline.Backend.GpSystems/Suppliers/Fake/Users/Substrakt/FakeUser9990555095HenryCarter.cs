using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Substrakt
{
    public class FakeUser9990555095HenryCarter : FakeUser
    {
        public FakeUser9990555095HenryCarter(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990555095";

        public override Guid Id => Guid.Parse("e43cd1fe-7d42-4d5e-a092-224a9ae5a9fc");
        public override string EmailAddress => "onboarding.nhsapp+substrakt.user1@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00003";
        public override string GivenName => "Henry";
        public override string FamilyName => "Carter";
        public override DateTime DateOfBirth => new DateTime(1999,5,7);
        public override string Sex => "Male";

        public override IEnumerable<string> LinkedAccountsNhsNumbers =>
            new[]
            {
                FakeUser9990555141OtisSwift.NhsNumberInternal,
            };
    }
}