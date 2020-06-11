using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.PatientsKnowBest
{
    public class FakeUser9990546770FloranceBennett : FakeUser
    {
        public FakeUser9990546770FloranceBennett(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990546770";

        public override Guid Id => Guid.Parse("a7ae6d5c-3920-4681-8e11-cd17b0aeb455");
        public override string EmailAddress => "onboarding.nhsapp+pkb.user8@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00001";
        public override string GivenName => "Florance";
        public override string FamilyName => "Bennett";
        public override DateTime DateOfBirth => new DateTime(1944, 12, 28);
        public override string Sex => "Female";

        public override IEnumerable<string> LinkedAccountsNhsNumbers =>
            new[]
            {
                FakeUser9990551332TobyDay.NhsNumberInternal,
                FakeUser9990551359JacobUpton.NhsNumberInternal
            };
    }
}