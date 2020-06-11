using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.Communal
{
    public class FakeUser9686368906IainHughes : FakeUser
    {
        public FakeUser9686368906IainHughes(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9686368906";

        public override Guid Id => Guid.Parse("2fc256ae-4044-443f-a83a-5ad5bbc097df ");

        public override string EmailAddress => "testuserlive+1@demo.signin.nhs.uk";

        public override string NhsNumber => NhsNumberInternal;

        public override string OdsCode => "C83615";

        public override string GivenName => "Iain";

        public override string FamilyName => "Hughes";

        public override DateTime DateOfBirth => new DateTime(1942, 2, 1);

        public override string Sex => "Male";
    }
}