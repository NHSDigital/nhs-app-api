using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9462767386SheriWrench : FakeUser
    {
        public FakeUser9462767386SheriWrench(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9462767386";

        public override Guid Id => Guid.Parse("b6e52eb8-39ed-4d83-9bd2-92f926ea33b1");

        public override string NhsNumber => NhsNumberInternal;

        public override string GivenName => "Sheri";

        public override string FamilyName => "Wrench";

        public override DateTime DateOfBirth => new DateTime(1939,3,31);

        public override string Sex => "Female";

        public override IEnumerable<string> LinkedAccountsNhsNumbers =>
            new[] { FakeUser5900110915GarfieldNicholsen.NhsNumberInternal };
    }
}