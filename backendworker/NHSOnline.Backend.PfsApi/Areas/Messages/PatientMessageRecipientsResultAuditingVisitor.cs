using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Messages;

namespace NHSOnline.Backend.PfsApi.Areas.Messages
{
    public class PatientMessageRecipientsResultAuditingVisitor : IPatientMessageRecipientsResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<PatientMessagesController> _logger;
        private const string AuditType = AuditingOperations.GetPatientPracticeMessageRecipientsResponse;

        public PatientMessageRecipientsResultAuditingVisitor(IAuditor auditor,
            ILogger<PatientMessagesController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }


        public async Task Visit(GetPatientMessageRecipientsResult.Success result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Patient message recipients successfully retrieved");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientMessageRecipientsResult.Success)}");
            }
        }

        public async Task Visit(GetPatientMessageRecipientsResult.BadGateway result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Error retrieving patient message recipients: Bad Gateway");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientMessageRecipientsResult.BadRequest)}");
            }
        }

        public async Task Visit(GetPatientMessageRecipientsResult.Forbidden result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Error retrieving patient message recipients: Forbidden");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientMessageRecipientsResult.Forbidden)}");
            }
        }

        public async Task Visit(GetPatientMessageRecipientsResult.BadRequest result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Error retrieving patient message recipients: Bad Request");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientMessageRecipientsResult.BadGateway)}");
            }
        }

        public async Task Visit(GetPatientMessageRecipientsResult.InternalServerError result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Error retrieving patient message recipients: Internal Server Error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientMessageRecipientsResult.InternalServerError)}");
            }
        }
    }
}
