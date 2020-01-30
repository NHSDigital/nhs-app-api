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
        private const string AuditType = AuditingOperations.CreatePracticePatientMessageResponse;

        public PatientSendMessageResultAuditingVisitor(IAuditor auditor, ILogger<PatientMessagesController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }


        public async Task Visit(PostSendMessageResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Patient messages successfully retrieved");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PostSendMessageResult.Success)}");
            }
        }

        public async Task Visit(PostSendMessageResult.BadRequest result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving patient messages: Bad Request");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PostSendMessageResult.BadRequest)}");
            }
        }

        public async Task Visit(PostSendMessageResult.Forbidden result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving patient messages: Forbidden");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PostSendMessageResult.Forbidden)}");
            }
        }

        public async Task Visit(PostSendMessageResult.BadGateway result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving patient messages: Bad Gateway");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PostSendMessageResult.BadGateway)}");
            }
        }

        public async Task Visit(PostSendMessageResult.InternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving patient messages: Internal Server Error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PostSendMessageResult.InternalServerError)}");
            }
        }
    }
}