using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.Linkage
{
    internal class CreateLinkageResultAuditorVisitor : ICreateLinkageResultVisitor<IActionResult>
    {        
        private readonly IAuditor _auditor;
        private readonly Supplier _supplier;
        private readonly string _nhsNumber;
        private const string AuditType = Constants.AuditingTitles.CreateLinkageKeyAuditTypeResponse;

        public CreateLinkageResultAuditorVisitor(IAuditor auditor, Supplier supplier, string nhsNumber)
        {
            _auditor = auditor;
            _supplier = supplier;
            _nhsNumber = nhsNumber;
        }

        public IActionResult Visit(CreateLinkageResult.SuccessfullyRetrieved result)
        {
            _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, AuditType, "Linkage key successfully created.");

            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(CreateLinkageResult.NhsNumberNotFound result)
        {
            _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, AuditType, "Linkage details not found.");

            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public IActionResult Visit(CreateLinkageResult.LinkageKeyAlreadyExists result)
        {
            _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, AuditType, "Linkage key creation unsuccessful. Linkage key already exists.");

            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        public IActionResult Visit(CreateLinkageResult.SupplierSystemUnavailable result)
        {   
            _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, AuditType, "Linkage key creation unsuccessful due to supplier being unavailable.");

            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(CreateLinkageResult.InternalServerError result)
        {
            _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, AuditType, "Linkage key creation unsuccessful due to internal erver error.");

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}