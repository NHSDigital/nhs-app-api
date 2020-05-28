using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUser9658218997LaurenCorr : FakeUser
    {
        public FakeUser9658218997LaurenCorr(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal const string NhsNumberInternal = "9658218997";

        public override Guid Id => Guid.Parse("a69413e0-a66d-4840-babc-f137b6ceae72");

        public override string NhsNumber => NhsNumberInternal;

        public override string GivenName => "Lauren";

        public override string FamilyName => "Corr";

        public override DateTime DateOfBirth => new DateTime(1944,12,28);

        public override string Sex => "Female";
    }
}