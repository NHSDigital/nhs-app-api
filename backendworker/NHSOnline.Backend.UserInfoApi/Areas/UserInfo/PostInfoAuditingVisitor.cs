using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserInfo
{
    public class PostInfoAuditingVisitor : IInfoResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly AccessToken _accessToken;
        private readonly ILogger<InfoController> _logger;

        private const Supplier Supplier = Support.Supplier.Microsoft;
        private const string AuditType = AuditingOperations.GetUserMessagesAuditTypeResponse;

        public PostInfoAuditingVisitor(ILogger<InfoController> logger, IAuditor auditor, AccessToken accessToken)
        {
            _logger = logger;
            _auditor = auditor;
            _accessToken = accessToken;
        }
        
        public async Task Visit(PostInfoResult.Created result)
        {
            try
            {
                var auditMessage = "User info created";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PostInfoResult.Created)}");
            }
        }
        
        public async Task Visit(PostInfoResult.BadGateway result)
        {
            try
            {
                var auditMessage = "User info creation unsuccessful due to BadGateway";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PostInfoResult.BadGateway)}");
            }
        }
        
        public async Task Visit(PostInfoResult.InternalServerError result)
        {
            try
            {
                var auditMessage = "User info creation unsuccessful due to InternalServerError";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PostInfoResult.InternalServerError)}");
            }
        }
    }
}