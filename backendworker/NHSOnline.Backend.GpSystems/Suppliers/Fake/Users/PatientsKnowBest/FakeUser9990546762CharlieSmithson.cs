using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.PatientsKnowBest
{
    public class FakeUser9990546762CharlieSmithson : FakeUser
    {
        public FakeUser9990546762CharlieSmithson(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990546762";

        public override Guid Id => Guid.Parse("6fdef841-6f29-463b-9200-5ad0e7e9c70f");
        public override string EmailAddress => "onboarding.nhsapp+pkb.user7@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00001";
        public override string GivenName => "Charlie";
        public override string FamilyName => "Smithson";
        public override DateTime DateOfBirth => new DateTime(1954, 9, 19);
        public override string Sex => "Male";
    }
}