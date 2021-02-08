using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.PatientRecord;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    public class PatientDocumentAuditingVisitor : IPatientDocumentResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<PatientDocumentController> _logger;
        private const string AuditType = AuditingOperations.ViewDocumentAuditTypeResponse;

        public PatientDocumentAuditingVisitor(IAuditor auditor, ILogger<PatientDocumentController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }

        public async Task Visit(GetPatientDocumentResult.Success result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Successfully retrieved patient document for viewing");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientDocumentResult.Success)}");
            }
        }

        public async Task Visit(GetPatientDocumentResult.BadGateway result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Error retrieving patient document for viewing: Bad Gateway");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientDocumentResult.BadGateway)}");
            }
        }
    }
}
