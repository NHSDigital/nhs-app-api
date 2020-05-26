using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9462767270KieranLogan : FakeUser
    {
        public override string NhsNumber => "9462767270";
        public override string GivenName => "Kieran";
        public override string FamilyName => "Logan";
        public override DateTime DateOfBirth => new DateTime(1937,5,7);
        public override string Sex => "Male";

        public FakeUser9462767270KieranLogan(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}