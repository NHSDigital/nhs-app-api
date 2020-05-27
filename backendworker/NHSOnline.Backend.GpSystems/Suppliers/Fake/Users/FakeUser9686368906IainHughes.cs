using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9686368906IainHughes : FakeUser
    {
        public override string NhsNumber => "9686368906";
        public override string GivenName => "Iain";
        public override string FamilyName => "Hughes";
        public override DateTime DateOfBirth => new DateTime(1942, 2, 1);
        public override string Sex => "Male";

        public FakeUser9686368906IainHughes(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}