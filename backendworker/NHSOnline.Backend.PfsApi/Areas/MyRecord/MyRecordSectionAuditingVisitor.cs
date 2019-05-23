using System;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
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

        public async Task Visit(GetMyRecordSectionResult.Success result)
        {
            try
            {
                var section = result.Response.SectionName;

                await _auditor.Audit(AuditType,
                    $"Patient record {section} successfully retrieved.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordSectionResult.Success)}");
            }
        }

        public async Task Visit(GetMyRecordSectionResult.BadGateway result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Unsuccessful");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetMyRecordSectionResult.BadGateway)}");
            }
        }

        public async Task Visit(GetMyRecordSectionResult.BadRequest result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error: Invalid request");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(GetMyRecordSectionResult.BadRequest)}");
            }
        }

        public async Task Visit(GetMyRecordSectionResult.InternalServerError result)
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
