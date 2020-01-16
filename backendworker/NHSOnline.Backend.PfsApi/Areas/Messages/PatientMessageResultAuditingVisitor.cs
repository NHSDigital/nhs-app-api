using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Messages;

namespace NHSOnline.Backend.PfsApi.Areas.Messages
{
    public class PatientMessageResultAuditingVisitor : IPatientMessageResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<PatientMessagesController> _logger;
        private const string AuditType = AuditingOperations.GetPatientPracticeMessageDetailsResponse;

        public PatientMessageResultAuditingVisitor(IAuditor auditor, ILogger<PatientMessagesController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }
        
        public async Task Visit(GetPatientMessageResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Patient message details successfully retrieved");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientMessageResult.Success)}");
            }
        }

        public async Task Visit(GetPatientMessageResult.BadRequest result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving patient message details: Bad Request");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientMessageResult.BadRequest)}");
            }
        }

        public async Task Visit(GetPatientMessageResult.Forbidden result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving patient message details: Forbidden");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientMessageResult.Forbidden)}");
            }
        }

        public async Task Visit(GetPatientMessageResult.BadGateway result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving patient message details: Bad Gateway");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientMessageResult.BadGateway)}");
            }
        }

        public async Task Visit(GetPatientMessageResult.InternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving patient message details: Internal Server Error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientMessageResult.InternalServerError)}");
            }
        }
    }
}