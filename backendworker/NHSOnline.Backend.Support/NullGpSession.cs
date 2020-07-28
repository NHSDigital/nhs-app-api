namespace NHSOnline.Backend.Support
{
    public class NullGpSession : GpUserSession
    {
        public override Supplier Supplier => Supplier.Disconnected;
        public override bool HasLinkedAccounts => false;
        public Supplier SessionSupplier { get; set; }
        public string SessionCreateReferenceCode { get; set; }

        public NullGpSession(Supplier supplier, string serviceDeskReference)
        {
            SessionSupplier = supplier;
            SessionCreateReferenceCode = serviceDeskReference;
        }

        public override T Accept<T>(IGpUserSessionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}