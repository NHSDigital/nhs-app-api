using System.Linq;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.Im1Connection
{
    public class Im1ConnectionRegisterAuditingVisitor : IIm1ConnectionRegisterResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        private readonly Supplier _supplier;

        public Im1ConnectionRegisterAuditingVisitor(IAuditor auditor, Supplier supplier)
        {
            _auditor = auditor;
            _supplier = supplier;
        }

        public object Visit(Im1ConnectionRegisterResult.SuccessfullyRegistered result)
        {
            if (!string.IsNullOrEmpty(result.Response.NhsNumbers?.FirstOrDefault()?.NhsNumber))
            {
                _auditor.AuditWithExplicitNhsNumber(
                  result.Response.NhsNumbers.First().NhsNumber, _supplier,
                  Constants.AuditingTitles.Im1ConnectionRegisterResponse, "IM1 connection successfully registered with GP system.");
            }
            return null;
        }

        public object Visit(Im1ConnectionRegisterResult.BadRequest result)
        {
            return null;
        }

        public object Visit(Im1ConnectionRegisterResult.InsufficientPermissions result)
        {
            return null;
        }

        public object Visit(Im1ConnectionRegisterResult.NotFound result)
        {
            return null;
        }

        public object Visit(Im1ConnectionRegisterResult.AccountAlreadyExists result)
        {
            return null;
        }

        public object Visit(Im1ConnectionRegisterResult.SupplierSystemUnavailable result)
        {
            return null;
        }
    }
}
