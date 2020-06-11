using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Communal
{
    public class FakeUser9690869094AdamSmith : FakeUser
    {
        public FakeUser9690869094AdamSmith(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9690869094";

        public override Guid Id => Guid.Parse("9fb87c98-a5b1-4e74-91fa-7bbdb5a26d9f");
        public override string EmailAddress => "testuserlive+18@demo.signin.nhs.uk";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "A20047";
        public override string GivenName => "Adam";
        public override string FamilyName => "Smith";
        public override DateTime DateOfBirth => new DateTime(1976,7,16);
        public override string Sex => "Male";
    }
}