using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;

namespace NHSOnline.Backend.PfsApi.Areas.Messages
{
    public class PatientMessageDeleteResultAuditingVisitor : IPatientMessageDeleteResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<PatientMessagesController> _logger;
        private const string AuditType = AuditingOperations.DeletePracticePatientMessageResponse;

        public PatientMessageDeleteResultAuditingVisitor(IAuditor auditor, ILogger<PatientMessagesController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }


        public async Task Visit(DeletePatientMessageResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Patient message successfully deleted");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DeletePatientMessageResult.Success)}");
            }
        }

        public async Task Visit(DeletePatientMessageResult.BadRequest result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error deleting patient message: Bad Request");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DeletePatientMessageResult.BadRequest)}");
            }
        }

        public async Task Visit(DeletePatientMessageResult.Forbidden result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error deleting patient message: Forbidden");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DeletePatientMessageResult.Forbidden)}");
            }
        }

        public async Task Visit(DeletePatientMessageResult.BadGateway result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error deleting patient message: Bad Gateway");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DeletePatientMessageResult.BadGateway)}");
            }
        }

        public async Task Visit(DeletePatientMessageResult.InternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error deleting patient message: Internal Server Error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DeletePatientMessageResult.InternalServerError)}");
            }
        }
    }
}