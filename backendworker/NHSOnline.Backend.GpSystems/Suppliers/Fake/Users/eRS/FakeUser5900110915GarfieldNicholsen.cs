using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users.eRS
{
    public class FakeUser5900110915GarfieldNicholsen : FakeUser
    {
        public FakeUser5900110915GarfieldNicholsen(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "5900110915";

        public override Guid Id => Guid.Parse("be309d68-f401-4031-8aaf-f9c60d190bd9");
        public override string EmailAddress => "testpatientf8d66+garfield@gmail.com";

        public override string NhsNumber => NhsNumberInternal;

        public override string OdsCode => "A20047";

        public override string GivenName => "Garfield";

        public override string FamilyName => "Nicholsen";

        public override DateTime DateOfBirth => new DateTime(1995,12,31);

        public override string Sex => "Male";
    }
}