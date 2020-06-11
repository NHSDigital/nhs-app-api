using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.PatientsKnowBest
{
    public class FakeUser9990555230FreyaStephenson : FakeUser
    {
        public FakeUser9990555230FreyaStephenson(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990555230";

        public override Guid Id => Guid.Parse("3621c3e4-74d5-4a5f-a881-99373b070971");
        public override string EmailAddress => "onboarding.nhsapp+pkb.user1@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00001";
        public override string GivenName => "Freya";
        public override string FamilyName => "Stephenson";
        public override DateTime DateOfBirth => new DateTime(1945, 2, 12);
        public override string Sex => "Female";

        public override IEnumerable<string> LinkedAccountsNhsNumbers =>
            new[]
            {
                FakeUser9990555265IsaacJohnson.NhsNumberInternal
            };
    }
}