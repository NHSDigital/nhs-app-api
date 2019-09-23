using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.PatientRecord;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    public class PatientDocumentAuditingVisitor : IPatientDocumentResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<PatientDocumentController> _logger;
        private const string AuditType = AuditingOperations.GetDocumentAuditTypeResponse;
        
        public PatientDocumentAuditingVisitor(IAuditor auditor, ILogger<PatientDocumentController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }

        public async Task Visit(GetPatientDocumentResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, AuditingMessages.DocumentSuccessfullyViewed);
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
                await _auditor.Audit(AuditType, AuditingMessages.DocumentBadGatewayResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientDocumentResult.BadGateway)}");
            }
        }
    }
}