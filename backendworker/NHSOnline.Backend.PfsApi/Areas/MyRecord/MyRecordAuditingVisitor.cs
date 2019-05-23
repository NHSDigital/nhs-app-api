using System;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
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
        
        public async Task Visit(GetMyRecordResult.Success result)
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
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordResult.Success)}");
            }
        }

        public async Task Visit(GetMyRecordResult.BadGateway result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Unsuccessful");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordResult.BadGateway)}");
            }
        }

        public async Task Visit(GetMyRecordResult.InternalServerError result)
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