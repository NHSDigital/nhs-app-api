using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9658218865GarthWright : FakeUser
    {
        public override string NhsNumber => "9658218865";
        public override string GivenName => "Garth";
        public override string FamilyName => "Wright";
        public override DateTime DateOfBirth => new DateTime(1933, 11, 14);
        public override string Sex => "Male";

        public FakeUser9658218865GarthWright(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}