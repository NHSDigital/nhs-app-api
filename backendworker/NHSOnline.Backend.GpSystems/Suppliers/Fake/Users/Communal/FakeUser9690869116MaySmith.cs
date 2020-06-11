using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Communal
{
    public class FakeUser9690869116MaySmith : FakeUser
    {
        public FakeUser9690869116MaySmith(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9690869116";

        public override Guid Id => Guid.Parse("cf8daa08-359d-43b4-af62-9aebe2364f12");
        public override string EmailAddress => "testuserlive+20@demo.signin.nhs.uk";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "A20047";
        public override string GivenName => "May";
        public override string FamilyName => "Smith";
        public override DateTime DateOfBirth => new DateTime(1974,4,13);
        public override string Sex => "Female";
    }
}