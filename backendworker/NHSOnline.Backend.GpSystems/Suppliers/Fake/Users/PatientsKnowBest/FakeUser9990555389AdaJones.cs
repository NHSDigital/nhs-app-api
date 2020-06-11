using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.PatientsKnowBest
{
    public class FakeUser9990555389AdaJones : FakeUser
    {
        public FakeUser9990555389AdaJones(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9990555389";

        public override Guid Id => Guid.Parse("bd1b5427-0a96-42cd-92cd-8e428c38be22");
        public override string EmailAddress => "onboarding.nhsapp+pkb.user6@gmail.com";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "F00002";
        public override string GivenName => "Ada";
        public override string FamilyName => "Jones";
        public override DateTime DateOfBirth => new DateTime(1978, 7, 21);
        public override string Sex => "Female";

        public override IEnumerable<string> LinkedAccountsNhsNumbers =>
            new[]
            {
                FakeUser9990546762CharlieSmithson.NhsNumberInternal,
            };
    }
}