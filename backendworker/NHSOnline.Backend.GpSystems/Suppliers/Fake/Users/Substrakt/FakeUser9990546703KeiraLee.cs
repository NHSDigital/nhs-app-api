using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Substrakt
{
    public class FakeUser9990546703KeiraLee : FakeUser
    {
        public FakeUser9990546703KeiraLee(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990546703";

        public override Guid Id => Guid.Parse("80c95faa-79a6-4f0f-aad6-7dca4d89a10e");
        public override string EmailAddress => "onboarding.nhsapp+substrakt.user6@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00003";
        public override string GivenName => "Keira";
        public override string FamilyName => "Lee";
        public override DateTime DateOfBirth => new DateTime(2000,2,13);
        public override string Sex => "Female";

        public override IEnumerable<string> LinkedAccountsNhsNumbers =>
            new[]
            {
                FakeUser9990546711JamesTaylor.NhsNumberInternal,
            };
    }
}