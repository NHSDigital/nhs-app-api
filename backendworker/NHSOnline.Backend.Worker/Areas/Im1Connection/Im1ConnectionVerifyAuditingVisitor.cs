using System.Linq;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.Im1Connection
{
    public class Im1ConnectionVerifyAuditingVisitor : IIm1ConnectionVerifyResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        private readonly SupplierEnum _supplier;

        public Im1ConnectionVerifyAuditingVisitor(IAuditor auditor, SupplierEnum supplier)
        {
            _auditor = auditor;
            _supplier = supplier;
        }

        public object Visit(Im1ConnectionVerifyResult.SuccessfullyVerified result)
        {
            _auditor.AuditWithExplicitNhsNumber(
                result.Response.NhsNumbers.First().NhsNumber, _supplier, 
                Constants.AuditingTitles.Im1ConnectionVerifyResponse, "IM1 connection successfully verified with GP system.");

            return null;
        }

        public object Visit(Im1ConnectionVerifyResult.InsufficientPermissions result)
        {
            return null;
        }

        public object Visit(Im1ConnectionVerifyResult.NotFound result)
        {
            return null;
        }

        public object Visit(Im1ConnectionVerifyResult.SupplierSystemUnavailable result)
        {
            return null;
        }

        public object Visit(Im1ConnectionVerifyResult.ErrorProcessingSecurityHeader errorProcessingSecurityHeader)
        {
            return null;
        }

        public object Visit(Im1ConnectionVerifyResult.InvalidUserCredentials invalidUserCredentials)
        {
            return null;
        }

        public object Visit(Im1ConnectionVerifyResult.InvalidRequest invalidRequest)
        {
            return null;
        }

        public object Visit(Im1ConnectionVerifyResult.UnknownError unknownError)
        {
            return null;
        }
    }
}
