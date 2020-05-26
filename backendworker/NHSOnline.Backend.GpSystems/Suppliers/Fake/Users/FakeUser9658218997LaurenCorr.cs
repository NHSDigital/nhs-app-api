using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9658218997LaurenCorr : FakeUser
    {
        public override string NhsNumber => "9658218997";
        public override string GivenName => "Lauren";
        public override string FamilyName => "Corr";
        public override DateTime DateOfBirth => new DateTime(1944,12,28);
        public override string Sex => "Female";

        public FakeUser9658218997LaurenCorr(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}