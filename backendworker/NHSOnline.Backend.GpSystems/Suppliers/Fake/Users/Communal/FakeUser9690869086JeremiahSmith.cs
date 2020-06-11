using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Communal
{
    public class FakeUser9690869086JeremiahSmith : FakeUser
    {
        public FakeUser9690869086JeremiahSmith(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9690869086";

        public override Guid Id => Guid.Parse("a4b91b6e-4793-4660-8d2a-745e1e888f8b");
        public override string EmailAddress => "testuserlive+17@demo.signin.nhs.uk";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "A20047";
        public override string GivenName => "Jeremiah";
        public override string FamilyName => "Smith";
        public override DateTime DateOfBirth => new DateTime(2017,9,4);
        public override string Sex => "Male";
    }
}