using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.PatientsKnowBest
{
    public class FakeUser9990555346MillieGreen : FakeUser
    {
        public FakeUser9990555346MillieGreen(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990555346";

        public override Guid Id => Guid.Parse("2d29d98b-a283-407c-8449-73872e2bd764");
        public override string EmailAddress => "onboarding.nhsapp+pkb.user4@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00002";
        public override string GivenName => "Millie";
        public override string FamilyName => "Green";
        public override DateTime DateOfBirth => new DateTime(1943, 6, 19);
        public override string Sex => "Female";
    }
}