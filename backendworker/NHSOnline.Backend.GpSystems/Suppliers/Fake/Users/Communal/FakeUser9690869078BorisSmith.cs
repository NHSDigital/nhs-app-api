using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Communal
{
    public class FakeUser9690869078BorisSmith : FakeUser
    {
        public FakeUser9690869078BorisSmith(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9690869078";

        public override Guid Id => Guid.Parse("7d9c0f0b-6a5d-452e-a161-bbc61a84098e");
        public override string EmailAddress => "testuserlive+16@demo.signin.nhs.uk";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "A20047";
        public override string GivenName => "Boris";
        public override string FamilyName => "Smith";
        public override DateTime DateOfBirth => new DateTime(2009,2,13);
        public override string Sex => "Male";
    }
}