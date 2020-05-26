using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9658218873MikeMeakin : FakeUser
    {
        public override string NhsNumber => "9658218873";
        public override string GivenName => "Mike";
        public override string FamilyName => "Meakin";
        public override DateTime DateOfBirth => new DateTime(1927, 6, 19);
        public override string Sex => "Male";

        public FakeUser9658218873MikeMeakin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}