using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.UnitTests
{
    public class MockGpUserSession : GpUserSession
    {
        public override Supplier Supplier => SupplierValue;

        public Supplier SupplierValue { get; set; }

        public override bool HasLinkedAccounts => HasLinkedAccountsValue;

        public bool HasLinkedAccountsValue { get; set; }

        public override T Accept<T>(IGpUserSessionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}