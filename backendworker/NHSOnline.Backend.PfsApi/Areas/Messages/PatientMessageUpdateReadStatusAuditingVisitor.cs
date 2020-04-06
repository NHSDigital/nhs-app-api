using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;

namespace NHSOnline.Backend.PfsApi.Areas.Messages
{
    public class PatientMessageUpdateReadStatusAuditingVisitor : IPatientMessageUpdateReadStatusResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<PatientMessagesController> _logger;
        private const string AuditType = AuditingOperations.UpdatePatientPracticeMessageUnreadStatusResponse;
        private readonly UpdateMessageReadStatusRequestBody _request;

        public PatientMessageUpdateReadStatusAuditingVisitor(IAuditor auditor,
            ILogger<PatientMessagesController> logger,
            UpdateMessageReadStatusRequestBody request)
        {
            _auditor = auditor;
            _logger = logger;
            _request = request;
        }

        public async Task Visit(PutPatientMessageReadStatusResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Patient message read status successfully updated");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PutPatientMessageReadStatusResult.Success)}");
            }
        }

        public async Task Visit(PutPatientMessageReadStatusResult.BadRequest result)
        {
            try
            {
                await _auditor.Audit(AuditType, $"Error updating unread status for message with id {_request.MessageId}: Bad Request");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PutPatientMessageReadStatusResult.BadRequest)}");
            }
        }

        public async Task Visit(PutPatientMessageReadStatusResult.Forbidden result)
        {
            try
            {
                await _auditor.Audit(AuditType, $"Error updating unread status for message with id {_request.MessageId}: Forbidden");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PutPatientMessageReadStatusResult.Forbidden)}");
            }
        }

        public async Task Visit(PutPatientMessageReadStatusResult.BadGateway result)
        {
            try
            {
                await _auditor.Audit(AuditType, $"Error updating unread status for message with id {_request.MessageId}: Bad Gateway");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PutPatientMessageReadStatusResult.BadGateway)}");
            }
        }

        public async Task Visit(PutPatientMessageReadStatusResult.InternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, $"Error updating unread status for message with id {_request.MessageId}: Internal Server Error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PutPatientMessageReadStatusResult.InternalServerError)}");
            }
        }
    }
}