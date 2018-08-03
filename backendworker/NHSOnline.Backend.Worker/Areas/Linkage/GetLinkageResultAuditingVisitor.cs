using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.Linkage
{
    internal class GetLinkageResultAuditingVisitor : IGetLinkageResultVisitor<IActionResult>
    {
        private readonly IAuditor _auditor;
        private readonly Supplier _supplier;
        private readonly string _nhsNumber;
        private const string AuditType = Constants.AuditingTitles.GetLinkageDetailsAuditTypeResponse;
        
        public GetLinkageResultAuditingVisitor(IAuditor auditor, Supplier supplier, string nhsNumber)
        {
            _auditor = auditor;
            _supplier = supplier;
            _nhsNumber = nhsNumber;
        }
        
        public IActionResult Visit(GetLinkageResult.SuccessfullyRetrieved result)
        {
            _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, AuditType, "Linkage details successfully retrieved.");
            
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(GetLinkageResult.NhsNumberNotFound result)
        {
            _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, AuditType, "Linkage details not found.");
            
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public IActionResult Visit(GetLinkageResult.SupplierSystemUnavailable result)
        {
            _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, AuditType, "Fetching linkage details unsuccessful due to supplier being unavailable.");
            
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(GetLinkageResult.InternalServerError result)
        {
            _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, AuditType, "Fetching linkage details unsuccessful due to internal server error.");
            
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        public IActionResult Visit(GetLinkageResult.LinkageKeyRevoked result)
        {
            _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, AuditType, "Fetching linkage details unsuccessful due to linkage key revoked.");
            
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
    }
}