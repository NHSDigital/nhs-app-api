using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9658218881KevinLeach : FakeUser
    {
        public FakeUser9658218881KevinLeach(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9658218881";

        public override Guid Id => Guid.Parse("7854b567-5239-47d4-baeb-4773903a9ad4");

        public override string NhsNumber => NhsNumberInternal;

        public override string GivenName => "Kevin";

        public override string FamilyName => "Leach";

        public override DateTime DateOfBirth => new DateTime(1921,8,8);

        public override string Sex => "Male";
    }
}