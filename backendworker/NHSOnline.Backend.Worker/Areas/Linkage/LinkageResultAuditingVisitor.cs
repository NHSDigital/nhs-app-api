using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Areas.Linkage
{
    internal class LinkageResultAuditingVisitor : ILinkageResultVisitor<Task<IActionResult>>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<LinkageController> _logger;
        private readonly Supplier _supplier;
        private readonly string _nhsNumber;
        private readonly string _auditType;
        
        public LinkageResultAuditingVisitor(IAuditor auditor, ILogger<LinkageController> logger, Supplier supplier, string nhsNumber, string auditType)
        {
            _auditor = auditor;
            _logger = logger;
            _supplier = supplier;
            _nhsNumber = nhsNumber;
            _auditType = auditType;
        }
        
        public async Task<IActionResult> Visit(LinkageResult.SuccessfullyRetrieved result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType,
                    "Linkage details successfully retrieved.");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {_auditType} {nameof(LinkageResult.SuccessfullyRetrieved)}");
            }
            
            return new OkObjectResult(result.Response);
        }

        public async Task<IActionResult> Visit(LinkageResult.SuccessfullyRetrievedAlreadyExists result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, "Linkage details successfully retrieved - already existed.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.SuccessfullyRetrievedAlreadyExists)}");
            }
            
            return new OkObjectResult(result.Response);
        }

        public async Task<IActionResult> Visit(LinkageResult.SupplierSystemUnavailable result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful due to supplier being unavailable.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.SupplierSystemUnavailable)}");
            }
            
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public async Task<IActionResult> Visit(LinkageResult.InternalServerError result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful due to internal server error.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.InternalServerError)}");
            }
            
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        public async Task<IActionResult> Visit(LinkageResult.ErrorCreatingPatientWhoAlreadyHasAnOnlineAccount result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, "The patient already has an online account.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.ErrorCreatingPatientWhoAlreadyHasAnOnlineAccount)}");
            }
            
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        public async Task<IActionResult> Visit(LinkageResult.SuccessfullyCreated result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, "Linkage key successfully created.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.SuccessfullyCreated)}");
            }
            
            var createdResult = new ObjectResult(result.Response)
            {
                StatusCode = StatusCodes.Status201Created
            };

            return createdResult;
        }

        public async Task<IActionResult> Visit(LinkageResult.PracticeNotLive result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - practice not live.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.PracticeNotLive)}");
            }
            
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public async Task<IActionResult> Visit(LinkageResult.PatientMarkedAsArchived result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(
                    _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - patient marked as archived.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.PatientMarkedAsArchived)}");
            }
            
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public async Task<IActionResult> Visit(LinkageResult.PatientNonCompetentOrUnderMinimumAge result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(
                    _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - patient non competent or under 16.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.PatientNonCompetentOrUnderMinimumAge)}");
            }
            
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public async Task<IActionResult> Visit(LinkageResult.AccountStatusInvalid result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(
                    _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - invalid account status.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.AccountStatusInvalid)}");
            }
            
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public async Task<IActionResult> Visit(LinkageResult.PatientNotRegisteredAtPractice result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(
                    _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - patient not registered at practice.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.PatientNotRegisteredAtPractice)}");
            }
            
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public async Task<IActionResult> Visit(LinkageResult.NoRegisteredOnlineUserFound result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(
                    _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - no registered online user found.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.NoRegisteredOnlineUserFound)}");
            }
            
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public async Task<IActionResult> Visit(LinkageResult.NotFoundErrorRetrievingNhsUser result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(
                    _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - not found error retrieving nhs user - unknown reason.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.NotFoundErrorRetrievingNhsUser)}");
            }
            
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public async Task<IActionResult> Visit(LinkageResult.NotFoundErrorCreatingNhsUser result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(
                    _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - not found error creating nhs user - unknown reason.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.NotFoundErrorCreatingNhsUser)}");
            }
            
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public async Task<IActionResult> Visit(LinkageResult.BadRequestErrorRetrievingNhsUser result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(
                    _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - bad request error retrieving nhs user - unknown reason.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.BadRequestErrorRetrievingNhsUser)}");
            }
            
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public async Task<IActionResult> Visit(LinkageResult.BadRequestErrorCreatingNhsUser result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(
                    _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - bad request creating nhs user - unknown reason.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.BadRequestErrorCreatingNhsUser)}");
            }
            
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public async Task<IActionResult> Visit(LinkageResult.ForbiddenErrorRetrievingNhsUser result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(
                    _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - forbidden retrieving nhs user - unknown reason.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.ForbiddenErrorRetrievingNhsUser)}");
            }
            
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public async Task<IActionResult> Visit(LinkageResult.LinkageKeyRevoked result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(
                    _nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful - linkage key revoked.");                
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.LinkageKeyRevoked)}");
            }
            
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
    }
}