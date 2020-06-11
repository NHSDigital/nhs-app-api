using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Communal
{
    public class FakeUser9658220150AmandaCurrie : FakeUser
    {
        public FakeUser9658220150AmandaCurrie(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9658220150";

        public override Guid Id => Guid.Parse("b632a0b7-8180-410c-a146-6daef5a74fad");
        public override string EmailAddress => "testuserlive+12@demo.signin.nhs.uk";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "A20047";
        public override string GivenName => "Amanda";
        public override string FamilyName => "Currie";
        public override DateTime DateOfBirth => new DateTime(1964,2,13);
        public override string Sex => "Female";
    }
}