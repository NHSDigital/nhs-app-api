using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Substrakt
{
    public class FakeUser9990546665AndyMurs : FakeUser
    {
        public FakeUser9990546665AndyMurs(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990546665";

        public override Guid Id => Guid.Parse("8da96e79-f991-4dbf-a205-0377556d0dd6");
        public override string EmailAddress => "onboarding.nhsapp+substrakt.user8@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00003";
        public override string GivenName => "Andy";
        public override string FamilyName => "Murs";
        public override DateTime DateOfBirth => new DateTime(1976,7,19);
        public override string Sex => "Male";

        public override IEnumerable<string> LinkedAccountsNhsNumbers =>
            new[]
            {
                FakeUser9990555443ShaunBlunt.NhsNumberInternal,
                FakeUser9990555486ClaireMarch.NhsNumberInternal,
            };
    }
}