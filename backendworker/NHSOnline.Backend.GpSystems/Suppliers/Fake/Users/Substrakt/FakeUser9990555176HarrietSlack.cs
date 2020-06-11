using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Substrakt
{
    public class FakeUser9990555176HarrietSlack : FakeUser
    {
        public FakeUser9990555176HarrietSlack(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990555176";

        public override Guid Id => Guid.Parse("08b05eb1-c5d7-4336-a8bc-914a24118ecc");
        public override string EmailAddress => "onboarding.nhsapp+substrakt.user4@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00004";
        public override string GivenName => "Harriet";
        public override string FamilyName => "Slack";
        public override DateTime DateOfBirth => new DateTime(1948,4, 18);
        public override string Sex => "Female";
    }
}