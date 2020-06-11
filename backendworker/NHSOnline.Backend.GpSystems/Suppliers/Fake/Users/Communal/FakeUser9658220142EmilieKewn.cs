using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Communal
{
    public class FakeUser9658220142EmilieKewn : FakeUser
    {
        public FakeUser9658220142EmilieKewn(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9658220142";

        public override Guid Id => Guid.Parse("99598613-c04a-436e-bad4-8c6c042505dc");
        public override string EmailAddress => "testuserlive+11@demo.signin.nhs.uk";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "A20047";
        public override string GivenName => "Emilie";
        public override string FamilyName => "Kewn";
        public override DateTime DateOfBirth => new DateTime(1999,10,3);
        public override string Sex => "Female";
    }
}