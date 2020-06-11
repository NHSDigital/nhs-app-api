using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Communal
{
    public class FakeUser9690869035MichaelSeniorSmith : FakeUser
    {
        public FakeUser9690869035MichaelSeniorSmith(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9690869035";

        public override Guid Id => Guid.Parse("0fac7884-2c44-48bf-a9d4-4c3530af0826");
        public override string EmailAddress => "testuserlive+13@demo.signin.nhs.uk";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "A20047";
        public override string GivenName => "Michael Senior";
        public override string FamilyName => "Smith";
        public override DateTime DateOfBirth => new DateTime(1946,7,4);
        public override string Sex => "Male";
    }
}