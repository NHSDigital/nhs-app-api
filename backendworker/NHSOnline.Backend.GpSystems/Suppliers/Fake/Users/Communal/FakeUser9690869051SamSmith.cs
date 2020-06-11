using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Communal
{
    public class FakeUser9690869051SamSmith : FakeUser
    {
        public FakeUser9690869051SamSmith(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9690869051";

        public override Guid Id => Guid.Parse("3415448d-e45a-4466-a176-9c4830717c25");
        public override string EmailAddress => "testuserlive+15@demo.signin.nhs.uk";
        public override string NhsNumber => NhsNumberInternal;
        public override string OdsCode => "A20047";
        public override string GivenName => "Sam";
        public override string FamilyName => "Smith";
        public override DateTime DateOfBirth => new DateTime(1989,9,24);
        public override string Sex => "Male";
    }
}