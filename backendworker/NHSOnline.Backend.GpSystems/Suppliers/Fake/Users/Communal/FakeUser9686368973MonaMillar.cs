using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Communal
{
    public class FakeUser9686368973MonaMillar : FakeUser
    {
        public FakeUser9686368973MonaMillar(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9686368973";

        public override Guid Id => Guid.Parse("9e39ad97-ec0d-457b-9947-34ec863fa100");

        public override string EmailAddress => "testuserlive@demo.signin.nhs.uk";

        public override string NhsNumber => NhsNumberInternal;

        public override string OdsCode => "C83615";

        public override string GivenName => "Mona";

        public override string FamilyName => "Millar";

        public override DateTime DateOfBirth => new DateTime(1968, 2, 12);

        public override string Sex => "Female";

        public override IEnumerable<string> LinkedAccountsNhsNumbers =>
            new[]{ FakeUser9686368906IainHughes.NhsNumberInternal };
    }
}