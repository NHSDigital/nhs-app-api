using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9462767386SheriWrench : FakeUser
    {
        public override string NhsNumber => "9462767386";
        public override string GivenName => "Sheri";
        public override string FamilyName => "Wrench";
        public override DateTime DateOfBirth => new DateTime(1939,3,31);
        public override string Sex => "Female";

        public FakeUser9462767386SheriWrench(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}