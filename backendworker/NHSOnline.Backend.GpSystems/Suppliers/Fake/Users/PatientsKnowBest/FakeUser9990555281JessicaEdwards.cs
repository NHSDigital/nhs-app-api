using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.PatientsKnowBest
{
    public class FakeUser9990555281JessicaEdwards : FakeUser
    {
        public FakeUser9990555281JessicaEdwards(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990555281";

        public override Guid Id => Guid.Parse("1477395f-bd65-46e9-b602-eb86046e1e93");
        public override string EmailAddress => "onboarding.nhsapp+pkb.user3@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00002";
        public override string GivenName => "Jessica";
        public override string FamilyName => "Edwards";
        public override DateTime DateOfBirth => new DateTime(1967,11,14);
        public override string Sex => "Female";

        public override IEnumerable<string> LinkedAccountsNhsNumbers =>
            new[]
            {
                FakeUser9990555346MillieGreen.NhsNumberInternal,
                FakeUser9990555370LaurieJobson.NhsNumberInternal
            };
    }
}