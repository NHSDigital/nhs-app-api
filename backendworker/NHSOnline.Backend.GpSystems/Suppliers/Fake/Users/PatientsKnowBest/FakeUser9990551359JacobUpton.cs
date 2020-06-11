using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.PatientsKnowBest
{
    public class FakeUser9990551359JacobUpton : FakeUser
    {
        public FakeUser9990551359JacobUpton(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990551359";

        public override Guid Id => Guid.Parse("fbac45f2-b377-496c-8395-6c9625fe5df8");
        public override string EmailAddress => "onboarding.nhsapp+pkb.user10@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00001";
        public override string GivenName => "Jacob";
        public override string FamilyName => "Upton";
        public override DateTime DateOfBirth => new DateTime(1925, 11, 2);
        public override string Sex => "Male";
    }
}