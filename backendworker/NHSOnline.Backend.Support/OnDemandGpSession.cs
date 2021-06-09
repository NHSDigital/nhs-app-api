namespace NHSOnline.Backend.Support
{
    public class OnDemandGpSession : GpUserSession
    {
        public override Supplier Supplier => Supplier.Disconnected;
        public override bool HasLinkedAccounts => false;
        public Supplier SessionSupplier { get; set; }

        public OnDemandGpSession(Supplier supplier)
        {
            SessionSupplier = supplier;
            Im1MessagingEnabled = null;
        }

        public override T Accept<T>(IGpUserSessionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
