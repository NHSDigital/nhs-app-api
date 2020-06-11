using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.PatientsKnowBest
{
    public class FakeUser9990551332TobyDay : FakeUser
    {
        public FakeUser9990551332TobyDay(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990551332";

        public override Guid Id => Guid.Parse("38c110c2-2579-4794-ac54-caa64d1cf82d");
        public override string EmailAddress => "onboarding.nhsapp+pkb.user9@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00001";
        public override string GivenName => "Toby";
        public override string FamilyName => "Day";
        public override DateTime DateOfBirth => new DateTime(1939, 6, 9);
        public override string Sex => "Male";
    }
}