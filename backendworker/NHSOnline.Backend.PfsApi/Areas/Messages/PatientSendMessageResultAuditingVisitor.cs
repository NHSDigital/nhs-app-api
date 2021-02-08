using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Messages;

namespace NHSOnline.Backend.PfsApi.Areas.Messages
{
    public class PatientSendMessageResultAuditingVisitor : IPatientSendMessageResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<PatientMessagesController> _logger;
        private const string AuditType = AuditingOperations.CreatePatientPracticeMessageResponse;

        public PatientSendMessageResultAuditingVisitor(IAuditor auditor, ILogger<PatientMessagesController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }


        public async Task Visit(PostPatientMessageResult.Success result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Patient practice message successfully sent");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PostPatientMessageResult.Success)}");
            }
        }

        public async Task Visit(PostPatientMessageResult.BadRequest result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Error sending patient practice message: Bad Request");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PostPatientMessageResult.BadRequest)}");
            }
        }

        public async Task Visit(PostPatientMessageResult.Forbidden result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Error sending patient practice message: Forbidden");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PostPatientMessageResult.Forbidden)}");
            }
        }

        public async Task Visit(PostPatientMessageResult.BadGateway result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Error sending patient practice message: Bad Gateway");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PostPatientMessageResult.BadGateway)}");
            }
        }

        public async Task Visit(PostPatientMessageResult.InternalServerError result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Error sending patient practice message: Internal Server Error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PostPatientMessageResult.InternalServerError)}");
            }
        }
    }
}
