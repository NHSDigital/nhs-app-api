using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9658219004CassieBray : FakeUser
    {
        public FakeUser9658219004CassieBray(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9658219004";

        public override Guid Id => Guid.Parse("038f4c5c-5cfb-4c84-bb2a-9a6be9876fb8 ");

        public override string NhsNumber => NhsNumberInternal;

        public override string GivenName => "Cassie";

        public override string FamilyName => "Bray";

        public override DateTime DateOfBirth => new DateTime(1939,6,9);

        public override string Sex => "Female";
    }
}