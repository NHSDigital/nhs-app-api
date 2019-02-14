using System;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    public class MyRecordSectionAuditingVisitor : IMyRecordSectionResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<MyRecordSectionController> _logger;
        
        private const string AuditType = Constants.AuditingTitles.ViewPatientRecordSectionAuditTypeResponse;

        public MyRecordSectionAuditingVisitor(IAuditor auditor, ILogger<MyRecordSectionController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }

        public async Task Visit(GetMyRecordSectionResult.SuccessfullyRetrieved result)
        {
            try
            {
                var section = result.Response.SectionName;

                await _auditor.Audit(AuditType,
                    $"Patient record {section} successfully retrieved.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordSectionResult.SuccessfullyRetrieved)}");
            }
        }

        public async Task Visit(GetMyRecordSectionResult.SupplierBadData result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Supplier - bad data");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordSectionResult.SupplierBadData)}");
            }
        }

        public async Task Visit(GetMyRecordSectionResult.Unsuccessful result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Unsuccessful");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordSectionResult.Unsuccessful)}");
            }
        }

        public async Task Visit(GetMyRecordSectionResult.ErrorProcessingSecurityHeader errorProcessingSecurityHeader)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Error processing security header");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordSectionResult.ErrorProcessingSecurityHeader)}");
            }
        }

        public async Task Visit(GetMyRecordSectionResult.InvalidUserCredentials invalidUserCredentials)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Invalid user credentials");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordSectionResult.InvalidUserCredentials)}");
            }
        }

        public async Task Visit(GetMyRecordSectionResult.InvalidRequest invalidRequest)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Invalid request");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordSectionResult.InvalidRequest)}");
            }
        }

        public async Task Visit(GetMyRecordSectionResult.UnknownError unknownError)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Unknown error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordSectionResult.UnknownError)}");
            }
        }

        public async Task Visit(GetMyRecordSectionResult.InternalServerError internalServerError)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Internal server error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordSectionResult.InternalServerError)}");
            }
        }
    }
}
