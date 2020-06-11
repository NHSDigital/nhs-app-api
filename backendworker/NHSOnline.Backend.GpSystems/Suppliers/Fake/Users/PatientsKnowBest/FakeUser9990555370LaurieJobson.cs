using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.PatientsKnowBest
{
    public class FakeUser9990555370LaurieJobson : FakeUser
    {
        public FakeUser9990555370LaurieJobson(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990555370";

        public override Guid Id => Guid.Parse("905c9df3-b9ce-4b36-b0cc-c2ee06ddfaa4");
        public override string EmailAddress => "onboarding.nhsapp+pkb.user5@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00002";
        public override string GivenName => "Laurie";
        public override string FamilyName => "Jobson";
        public override DateTime DateOfBirth => new DateTime(1989,8,8);
        public override string Sex => "Female";
    }
}