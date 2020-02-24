using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public class MicrotestUserSession : GpUserSession
    {
        public override Supplier Supplier => Supplier.Microtest;
        
        public override bool HasLinkedAccounts => false;
    }
}
