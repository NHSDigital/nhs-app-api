using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser5900110915GarfieldNicholsen : FakeUser
    {
        public override string NhsNumber => "5900110915";
        public override string GivenName => "Garfield";
        public override string FamilyName => "Nicholsen";
        public override DateTime DateOfBirth => new DateTime(1995,12,31);
        public override string Sex => "Male";

        public FakeUser5900110915GarfieldNicholsen(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}