using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Communal
{
    public class FakeUser9690869108EveSmith : FakeUser
    {
        public FakeUser9690869108EveSmith(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9690869108";

        public override Guid Id => Guid.Parse("86b905c2-415d-4820-9f08-e84e145a4d43");
        public override string EmailAddress => "testuserlive+19@demo.signin.nhs.uk";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "A20047";
        public override string GivenName => "Eve";
        public override string FamilyName => "Smith";
        public override DateTime DateOfBirth => new DateTime(1977,5,30);
        public override string Sex => "Female";
    }
}