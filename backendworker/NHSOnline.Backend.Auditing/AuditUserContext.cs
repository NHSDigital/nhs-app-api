using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auditing
{
    internal sealed class AuditUserContext
    {
        public AuditUserContext(
            string accessToken,
            string nhsNumber,
            Supplier supplier,
            bool isProxying,
            string linkedAccountNhsNumber)
        {
            AccessToken = accessToken;
            NhsNumber = nhsNumber;
            Supplier = supplier;
            IsProxying = isProxying;
            LinkedAccountNhsNumber = linkedAccountNhsNumber;
        }

        public string AccessToken { get; }
        public string NhsNumber { get; }
        public Supplier Supplier { get; }
        public bool IsProxying { get; }
        public string LinkedAccountNhsNumber { get; }
    }
}