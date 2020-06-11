using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Communal
{
    public class FakeUser9690869043MichaelJuniorSmith : FakeUser
    {
        public FakeUser9690869043MichaelJuniorSmith(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9690869043";

        public override Guid Id => Guid.Parse("e40dbd60-93fb-4171-bb8c-3ca8baf9e2c2");
        public override string EmailAddress => "testuserlive+14@demo.signin.nhs.uk";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "A20047";
        public override string GivenName => "Michael Junior";
        public override string FamilyName => "Smith";
        public override DateTime DateOfBirth => new DateTime(1948, 4, 18);
        public override string Sex => "Male";
    }
}