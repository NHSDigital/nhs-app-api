using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Substrakt
{
    public class FakeUser9990555443ShaunBlunt : FakeUser
    {
        public FakeUser9990555443ShaunBlunt(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990555443";

        public override Guid Id => Guid.Parse("3fa39087-1823-49f0-b47c-2b626f61d8bb");
        public override string EmailAddress => "onboarding.nhsapp+substrakt.user9@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00004";
        public override string GivenName => "Shaun";
        public override string FamilyName => "Blunt";
        public override DateTime DateOfBirth => new DateTime(1977,5,4);
        public override string Sex => "Male";
    }
}