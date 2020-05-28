using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9658218865GarthWright : FakeUser
    {
        public FakeUser9658218865GarthWright(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9658218865";

        public override Guid Id => Guid.Parse("b9fd14bd-0a86-4349-909a-dfd904859e1d");

        public override string NhsNumber => NhsNumberInternal;

        public override string GivenName => "Garth";

        public override string FamilyName => "Wright";

        public override DateTime DateOfBirth => new DateTime(1933, 11, 14);

        public override string Sex => "Male";

        public override IEnumerable<string> LinkedAccountsNhsNumbers =>
            new[]
            {
                FakeUser9658218873MikeMeakin.NhsNumberInternal,
                FakeUser9658218881KevinLeach.NhsNumberInternal,
                FakeUser9658218903ArnoldOlley.NhsNumberInternal
            };
    }
}