using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.Auditing
{
    internal class AuditContextGpUserSessionVisitor : IGpUserSessionVisitor<AuditUserContext>
    {
        private readonly P9UserSession _p9UserSession;
        private readonly bool _isProxying;
        private readonly string _linkedAccountNhsNumber;

        public AuditContextGpUserSessionVisitor(
            P9UserSession p9UserSession, bool isProxying, string linkedAccountNhsNumber)
        {
            _p9UserSession = p9UserSession;
            _isProxying = isProxying;
            _linkedAccountNhsNumber = linkedAccountNhsNumber;
        }

        public AuditUserContext Visit(NullGpSession nullGpSession)
        {
            return new AuditUserContext(
                _p9UserSession.CitizenIdUserSession.AccessToken,
                _p9UserSession.NhsNumber,
                Supplier.Unknown,
                _isProxying,
                _linkedAccountNhsNumber);
        }

        public AuditUserContext Visit(GpUserSession gpSession)
        {
            return new AuditUserContext(
                _p9UserSession.CitizenIdUserSession.AccessToken,
                _p9UserSession.GpUserSession.NhsNumber,
                _p9UserSession.GpUserSession.Supplier,
                _isProxying,
                _linkedAccountNhsNumber);
        }
    }
}