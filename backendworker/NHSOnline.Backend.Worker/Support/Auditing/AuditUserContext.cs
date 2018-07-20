namespace NHSOnline.Backend.Worker.Support.Auditing
{
    public class AuditUserContext
    {
        public string NhsNumber { get; }

        public SupplierEnum Supplier { get; }

        public AuditUserContext(string nhsNumber, SupplierEnum supplier)
        {
            NhsNumber = nhsNumber;
            Supplier = supplier;
        }
    }
}
