using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Messages;

namespace NHSOnline.Backend.PfsApi.Areas.Messages
{
    public class PatientMessagesResultAuditingVisitor : IPatientMessagesResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<PatientMessagesController> _logger;
        private const string AuditType = AuditingOperations.ViewPatientPracticeMessagesResponse;

        public PatientMessagesResultAuditingVisitor(IAuditor auditor, ILogger<PatientMessagesController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }

        public async Task Visit(GetPatientMessagesResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Patient messages successfully retrieved");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientMessagesResult.Success)}");
            }
        }

        public async Task Visit(GetPatientMessagesResult.BadRequest result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving patient messages: Bad Request");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientMessagesResult.BadRequest)}");
            }
        }

        public async Task Visit(GetPatientMessagesResult.Forbidden result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving patient messages: Forbidden");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientMessagesResult.Forbidden)}");
            }
        }

        public async Task Visit(GetPatientMessagesResult.BadGateway result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving patient messages: Bad Gateway");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientMessagesResult.BadGateway)}");
            }
        }

        public async Task Visit(GetPatientMessagesResult.InternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving patient messages: Internal Server Error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPatientMessagesResult.InternalServerError)}");
            }
        }
    }
}