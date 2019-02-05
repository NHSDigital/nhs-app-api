using System;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.Support.Auditing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    public class MyRecordAuditingVisitor : IMyRecordResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<MyRecordController> _logger;
        
        private const string AuditType = Constants.AuditingTitles.ViewPatientRecordAuditTypeResponse;

        public MyRecordAuditingVisitor(IAuditor auditor, ILogger<MyRecordController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }
        
        public async Task Visit(GetMyRecordResult.SuccessfullyRetrieved result)
        {
            try
            {
                var hasSummaryRecordAccess = result.Response.HasSummaryRecordAccess;
                var hasDetailedRecordAccess = result.Response.HasDetailedRecordAccess;
            
                await _auditor.Audit(AuditType, 
                    $"Patient record successfully retrieved. {nameof(hasSummaryRecordAccess)}={hasSummaryRecordAccess}," +
                    $" {nameof(hasDetailedRecordAccess)}={hasDetailedRecordAccess}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordResult.SuccessfullyRetrieved)}");
            }
        }

        public async Task Visit(GetMyRecordResult.SupplierBadData result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Supplier - bad data");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordResult.SupplierBadData)}");
            }
        }

        public async Task Visit(GetMyRecordResult.Unsuccessful result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Unsuccessful");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordResult.Unsuccessful)}");
            }
        }

        public async Task Visit(GetMyRecordResult.ErrorProcessingSecurityHeader errorProcessingSecurityHeader)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Error processing security header");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordResult.ErrorProcessingSecurityHeader)}");
            }
        }

        public async Task Visit(GetMyRecordResult.InvalidUserCredentials invalidUserCredentials)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Invalid user credentials");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordResult.InvalidUserCredentials)}");
            }
        }

        public async Task Visit(GetMyRecordResult.InvalidRequest invalidRequest)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Invalid request");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordResult.InvalidRequest)}");
            }
        }

        public async Task Visit(GetMyRecordResult.UnknownError unknownError)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Unknown error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordResult.UnknownError)}");
            }
        }

        public async Task Visit(GetMyRecordResult.InternalServerError internalServerError)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Internal server error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordResult.InternalServerError)}");
            }
        }
    }
}