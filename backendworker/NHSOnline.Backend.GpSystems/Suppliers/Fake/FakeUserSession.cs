using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    public class FakeUserSession : GpUserSession
    {
        public override Supplier Supplier => Supplier.Fake;
        public override bool HasLinkedAccounts => false;
    }
}