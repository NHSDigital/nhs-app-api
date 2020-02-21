using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages
{
    public class MessagesAuditingVisitor : IMessagesResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly AccessToken _accessToken;
        private readonly ILogger<MessagesController> _logger;

        private const Supplier Supplier = Support.Supplier.Microsoft;
        private const string AuditType = AuditingOperations.GetUserMessagesAuditTypeResponse;

        public MessagesAuditingVisitor(ILogger<MessagesController> logger, IAuditor auditor, AccessToken accessToken)
        {
            _logger = logger;
            _auditor = auditor;
            _accessToken = accessToken;
        }

        public async Task Visit(MessagesResult.Some result)
        {
            try
            {
                var auditMessage = $"{result.Response.Sum(x => x.Messages.Count)} user messages retrieved";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(MessagesResult.Some)}");
            }
        }

        public async Task Visit(MessagesResult.None result)
        {
            try
            {
                var auditMessage = "No user messages retrieved";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(MessagesResult.None)}");
            }
        }

        public async Task Visit(MessagesResult.BadGateway result)
        {
            try
            {
                var auditMessage = "User messages request unsuccessful due to BadGateway";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(MessagesResult.BadGateway)}");
            }
        }

        public async Task Visit(MessagesResult.BadRequest result)
        {
            try
            {
                var auditMessage = "User messages request unsuccessful due to BadRequest";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(MessagesResult.BadRequest)}");
            }
        }

        public async Task Visit(MessagesResult.InternalServerError result)
        {
            try
            {
                var auditMessage = "User messages request unsuccessful due to InternalServerError";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(MessagesResult.InternalServerError)}");
            }
        }
    }
}