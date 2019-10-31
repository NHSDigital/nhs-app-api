using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public class MessagePatchAuditingVisitor: IMessagePatchResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly AccessToken _accessToken;
        private readonly ILogger<MessagesController> _logger;
        
        private const Supplier Supplier = Support.Supplier.Microsoft;
        private const string AuditType = AuditingOperations.PatchUserMessageAuditTypeResponse;

        public MessagePatchAuditingVisitor(ILogger<MessagesController> logger, IAuditor auditor,
            AccessToken accessToken)
        {
            _logger = logger;
            _auditor = auditor;
            _accessToken = accessToken;
        }

        public async Task Visit(MessagePatchResult.BadRequest result)
        {
            try
            {
                var auditMessage = "User message patch request unsuccessful due to a BadRequest";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier,AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(MessagePatchResult.BadRequest)}");
            }
        }

        public async Task Visit(MessagePatchResult.Updated result)
        {
            try
            {
                var auditMessage = "User message updated";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier,AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(MessagePatchResult.Updated)}");
            }
        }

        public async Task Visit(MessagePatchResult.NotFound result)
        {
            try
            {
                var auditMessage = "User messages patch request unsuccessful due to message NotFound";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier,AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(MessagePatchResult.NotFound)}");
            }
        }

        public async Task Visit(MessagePatchResult.BadGateway result)
        {
            try
            {
                var auditMessage = "User messages patch request unsuccessful due to a BadGateway";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier,AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(MessagePatchResult.BadGateway)}");
            }
        }

        public async Task Visit(MessagePatchResult.InternalServerError result)
        {
            try
            {
                var auditMessage = "User messages patch request unsuccessful due to an InternalServerError";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier,AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(MessagePatchResult.InternalServerError)}");
            }
        }
    }
}