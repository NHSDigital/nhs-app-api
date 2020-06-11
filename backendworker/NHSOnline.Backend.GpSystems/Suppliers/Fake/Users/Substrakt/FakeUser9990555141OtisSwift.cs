using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Substrakt
{
    public class FakeUser9990555141OtisSwift : FakeUser
    {
        public FakeUser9990555141OtisSwift(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990555141";

        public override Guid Id => Guid.Parse("9a1b6e74-81f6-492a-8106-903b6c06cf8c");
        public override string EmailAddress => "onboarding.nhsapp+substrakt.user2@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00003";
        public override string GivenName => "Otis";
        public override string FamilyName => "Swift";
        public override DateTime DateOfBirth => new DateTime(1964,2,13);
        public override string Sex => "Male";
    }
}