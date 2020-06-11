using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Substrakt
{
    public class FakeUser9990555192MiaMcelroy : FakeUser
    {
        public FakeUser9990555192MiaMcelroy(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990555192";

        public override Guid Id => Guid.Parse("f43115c4-5218-4f9c-a97a-f307f2a421e5");
        public override string EmailAddress => "onboarding.nhsapp+substrakt.user5@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00004";
        public override string GivenName => "Mia";
        public override string FamilyName => "Mcelroy";
        public override DateTime DateOfBirth => new DateTime(2001,9,24);
        public override string Sex => "Female";
    }
}