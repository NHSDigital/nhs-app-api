using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Substrakt
{
    public class FakeUser9990555486ClaireMarch : FakeUser
    {
        public FakeUser9990555486ClaireMarch(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990555486";

        public override Guid Id => Guid.Parse("c24d32e4-290c-4916-a856-8b9426c7bfb0");
        public override string EmailAddress => "onboarding.nhsapp+substrakt.user10@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00004";
        public override string GivenName => "Claire";
        public override string FamilyName => "March";
        public override DateTime DateOfBirth => new DateTime(1974,4,19);
        public override string Sex => "Female";
    }
}