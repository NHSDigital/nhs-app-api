using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9686368973MonaMillar : FakeUser
    {
        public override string NhsNumber => "9686368973";
        public override string GivenName => "Mona";
        public override string FamilyName => "Millar";
        public override DateTime DateOfBirth => new DateTime(1968, 2, 12);
        public override string Sex => "Female";

        public FakeUser9686368973MonaMillar(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}