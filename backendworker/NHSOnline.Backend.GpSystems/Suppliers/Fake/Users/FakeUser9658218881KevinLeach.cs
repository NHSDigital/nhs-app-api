using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9658218881KevinLeach : FakeUser
    {
        public override string NhsNumber => "9658218881";
        public override string GivenName => "Kevin";
        public override string FamilyName => "Leach";
        public override DateTime DateOfBirth => new DateTime(1921,8,8);
        public override string Sex => "Male";

        public FakeUser9658218881KevinLeach(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}