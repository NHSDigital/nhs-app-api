namespace NHSOnline.Backend.Support.Auditing
{
    public class AuditUserContext
    {
        public string NhsNumber { get; }

        public Supplier Supplier { get; }

        public AuditUserContext(string nhsNumber, Supplier supplier)
        {
            NhsNumber = nhsNumber;
            Supplier = supplier;
        }
    }
}
