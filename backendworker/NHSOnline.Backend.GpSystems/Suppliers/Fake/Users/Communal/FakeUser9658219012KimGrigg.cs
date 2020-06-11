using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Communal
{
    public class FakeUser9658219012KimGrigg : FakeUser
    {
        public FakeUser9658219012KimGrigg(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9658219012";

        public override Guid Id => Guid.Parse("a3d2f4c6-19cf-4442-8ac4-6248fcece2b2");
        public override string EmailAddress => "testuserlive+9@demo.signin.nhs.uk";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "A20047";
        public override string GivenName => "Kim";
        public override string FamilyName => "Grigg";
        public override DateTime DateOfBirth => new DateTime(1925, 10, 5);
        public override string Sex => "Male";
    }
}