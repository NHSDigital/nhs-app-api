using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9658218989MinaLeech : FakeUser
    {
        public FakeUser9658218989MinaLeech(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9658218989";

        public override Guid Id => Guid.Parse("d27fd924-f86f-4692-acac-c8b08678eec8");

        public override string NhsNumber => NhsNumberInternal;

        public override string GivenName => "Mina";

        public override string FamilyName => "Leech";

        public override DateTime DateOfBirth => new DateTime(1918,9,19);

        public override string Sex => "Female";

        public override IEnumerable<string> LinkedAccountsNhsNumbers =>
            new[] { FakeUser9658218997LaurenCorr.NhsNumberInternal };
    }
}