using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9462767270KieranLogan : FakeUser
    {
        public FakeUser9462767270KieranLogan(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9462767270";

        public override Guid Id => Guid.Parse("291531e6-92f5-49be-852c-365f229d17e0");

        public override string NhsNumber => NhsNumberInternal;

        public override string GivenName => "Kieran";

        public override string FamilyName => "Logan";

        public override DateTime DateOfBirth => new DateTime(1937,5,7);

        public override string Sex => "Male";
    }
}