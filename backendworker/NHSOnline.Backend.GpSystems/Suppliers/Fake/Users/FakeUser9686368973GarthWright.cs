using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9686368973GarthWright : FakeUser
    {
        public override string NhsNumber => "9686368973";
        public override string GivenName => "Garth";
        public override string FamilyName => "Wright";
        public override DateTime DateOfBirth => new DateTime(1933, 11, 14);
        public override string Sex => "Male";

        public FakeUser9686368973GarthWright(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}