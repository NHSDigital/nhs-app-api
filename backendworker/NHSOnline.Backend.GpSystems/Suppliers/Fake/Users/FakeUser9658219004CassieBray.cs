using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9658219004CassieBray : FakeUser
    {
        public override string NhsNumber => "9658219004";
        public override string GivenName => "Cassie";
        public override string FamilyName => "Bray";
        public override DateTime DateOfBirth => new DateTime(1939,6,9);
        public override string Sex => "Female";

        public FakeUser9658219004CassieBray(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}