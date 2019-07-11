namespace NHSOnline.Backend.Support.Auditing
{
    public class AuditUserContext
    {
        public AuditUserContext(
            string accessToken,
            string nhsNumber,
            Supplier supplier
        )
        {
            AccessToken = accessToken;
            NhsNumber = nhsNumber;
            Supplier = supplier;
        }

        public string AccessToken { get; }
        public string NhsNumber { get; }
        public Supplier Supplier { get; }
    }
}