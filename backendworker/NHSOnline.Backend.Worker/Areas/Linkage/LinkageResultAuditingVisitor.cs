using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.Linkage
{
    internal class LinkageResultAuditingVisitor : ILinkageResultVisitor<IActionResult>
    {
        private readonly IAuditor _auditor;
        private readonly Supplier _supplier;
        private readonly string _nhsNumber;
        private readonly string _auditType;
        
        public LinkageResultAuditingVisitor(IAuditor auditor, Supplier supplier, string nhsNumber, string auditType)
        {
            _auditor = auditor;
            _supplier = supplier;
            _nhsNumber = nhsNumber;
            _auditType = auditType;
        }
        
        public IActionResult Visit(LinkageResult.SuccessfullyRetrieved result)
        {
            _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, "Linkage details successfully retrieved.");
            
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(LinkageResult.SuccessfullyRetrievedAlreadyExists result)
        {
            _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, "Linkage details successfully retrieved - already existed.");

            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(LinkageResult.SupplierSystemUnavailable result)
        {
            _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful due to supplier being unavailable.");
            
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(LinkageResult.InternalServerError result)
        {
            _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful due to internal server error.");
            
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        public IActionResult Visit(LinkageResult.ErrorCreatingPatientWhoAlreadyHasAnOnlineAccount result)
        {
            _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, "The patient already has an online account.");

            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        public IActionResult Visit(LinkageResult.SuccessfullyCreated result)
        {
            _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, "Linkage key successfully created.");
            
            var createdResult = new ObjectResult(result.Response)
            {
                StatusCode = StatusCodes.Status201Created
            };

            return createdResult;
        }

        public IActionResult Visit(LinkageResult.PracticeNotLive result)
        {
            _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - practice not live.");
            
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(LinkageResult.PatientMarkedAsArchived result)
        {
            _auditor.AuditWithExplicitNhsNumber(
                _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - patient marked as archived.");

            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(LinkageResult.PatientNonCompetentOrUnderMinimumAge result)
        {
            _auditor.AuditWithExplicitNhsNumber(
                _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - patient non competent or under 16.");

            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(LinkageResult.AccountStatusInvalid result)
        {
            _auditor.AuditWithExplicitNhsNumber(
                _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - invalid account status.");

            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(LinkageResult.PatientNotRegisteredAtPractice result)
        {
            _auditor.AuditWithExplicitNhsNumber(
                _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - patient not registered at practice.");

            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public IActionResult Visit(LinkageResult.NoRegisteredOnlineUserFound result)
        {
            _auditor.AuditWithExplicitNhsNumber(
                _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - no registered online user found.");

            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public IActionResult Visit(LinkageResult.NotFoundErrorRetrievingNhsUser result)
        {
            _auditor.AuditWithExplicitNhsNumber(
                _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - not found error retrieving nhs user - unknown reason.");

            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public IActionResult Visit(LinkageResult.NotFoundErrorCreatingNhsUser result)
        {
            _auditor.AuditWithExplicitNhsNumber(
                _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - not found error creating nhs user - unknown reason.");

            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public IActionResult Visit(LinkageResult.BadRequestErrorRetrievingNhsUser result)
        {
            _auditor.AuditWithExplicitNhsNumber(
                   _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - bad request error retrieving nhs user - unknown reason.");

            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(LinkageResult.BadRequestErrorCreatingNhsUser result)
        {
            _auditor.AuditWithExplicitNhsNumber(
                _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - bad request creating nhs user - unknown reason.");

            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
    }
}