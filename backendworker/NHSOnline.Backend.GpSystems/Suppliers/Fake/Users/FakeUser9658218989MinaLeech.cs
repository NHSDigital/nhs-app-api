using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9658218989MinaLeech : FakeUser
    {
        public override string NhsNumber => "9658218989";
        public override string GivenName => "Mina";
        public override string FamilyName => "Leech";
        public override DateTime DateOfBirth => new DateTime(1918,9,19);
        public override string Sex => "Female";

        public FakeUser9658218989MinaLeech(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}