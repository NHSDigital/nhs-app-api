using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.PatientRecord;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    public class PatientDocumentDownloadAuditingVisitor : IPatientDocumentDownloadResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<PatientDocumentController> _logger;
        private const string AuditType = AuditingOperations.DownloadDocumentAuditTypeResponse;
        
        public PatientDocumentDownloadAuditingVisitor(IAuditor auditor, ILogger<PatientDocumentController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }

        public async Task Visit(GetPatientDocumentDownloadResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Successfully retrieved patient document for downloading");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientDocumentResult.Success)}");
            }
        }

        public async Task Visit(GetPatientDocumentDownloadResult.BadGateway result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving patient document for downloading: Bad Gateway");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientDocumentResult.BadGateway)}");
            }
        }
    }
}