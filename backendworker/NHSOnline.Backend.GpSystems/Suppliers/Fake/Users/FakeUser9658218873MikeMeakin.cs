using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9658218873MikeMeakin : FakeUser
    {
        public FakeUser9658218873MikeMeakin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9658218873";

        public override Guid Id => Guid.Parse("322226b5-b41d-4c6a-9269-1b4e20fb3a25");

        public override string NhsNumber => NhsNumberInternal;

        public override string GivenName => "Mike";

        public override string FamilyName => "Meakin";

        public override DateTime DateOfBirth => new DateTime(1927, 6, 19);

        public override string Sex => "Male";
    }
}