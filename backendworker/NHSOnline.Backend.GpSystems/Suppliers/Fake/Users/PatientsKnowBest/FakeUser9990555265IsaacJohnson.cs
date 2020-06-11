using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.PatientsKnowBest
{
    public class FakeUser9990555265IsaacJohnson : FakeUser
    {
        public FakeUser9990555265IsaacJohnson(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990555265";

        public override Guid Id => Guid.Parse("6d5d0f04-f163-4c0b-9fa8-91950f9a608e");
        public override string EmailAddress => "onboarding.nhsapp+pkb.user2@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00002";
        public override string GivenName => "Isaac";
        public override string FamilyName => "Johnson";
        public override DateTime DateOfBirth => new DateTime(1955,2,1);
        public override string Sex => "Male";
    }
}