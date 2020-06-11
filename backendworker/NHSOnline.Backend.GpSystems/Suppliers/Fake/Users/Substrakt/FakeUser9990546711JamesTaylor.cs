using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Substrakt
{
    public class FakeUser9990546711JamesTaylor : FakeUser
    {
        public FakeUser9990546711JamesTaylor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990546711";

        public override Guid Id => Guid.Parse("a9f452af-c94f-4abb-b4fa-d912a05906b7");
        public override string EmailAddress => "onboarding.nhsapp+substrakt.user7@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode  => "F00003";
        public override string GivenName => "James";
        public override string FamilyName => "Taylor";
        public override DateTime DateOfBirth => new DateTime(2017,6,4);
        public override string Sex => "Male";
    }
}