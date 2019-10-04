using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserInfo
{
    public class GetInfoAuditingVisitor : IGetInfoResultVisitor<Task>
    {
        private readonly ILogger<InfoController> _logger;
        private readonly IAuditor _auditor;
        private readonly AccessToken _accessToken;

        private const Supplier Supplier = Support.Supplier.Microsoft;
        private const string AuditType = AuditingOperations.GetUserInfoAuditTypeResponse;

        public GetInfoAuditingVisitor(ILogger<InfoController> logger, IAuditor auditor, AccessToken accessToken)
        {
            _logger = logger;
            _auditor = auditor;
            _accessToken = accessToken;
        }

        public async Task Visit(GetInfoResult.Found result)
        {
            try
            {
                var auditMessage = "User Info retrieved";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetInfoResult.Found)}");
            }
        }

        public async Task Visit(GetInfoResult.FoundMultiple result)
        {
            try
            {
                var auditMessage = "User Info retrieved";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetInfoResult.FoundMultiple)}");
            }
        }

        public async Task Visit(GetInfoResult.NotFound result)
        {
            try
            {
                var auditMessage = "User Info not found";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetInfoResult.NotFound)}");
            }
        }

        public async Task Visit(GetInfoResult.BadGateway result)
        {
            try
            {
                var auditMessage = "User Info request unsuccessful due to BadGateway";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetInfoResult.BadGateway)}");
            }
        }

        public async Task Visit(GetInfoResult.InternalServerError result)
        {
            try
            {
                var auditMessage = "User Info request unsuccessful due to InternalServerError";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetInfoResult.InternalServerError)}");
            }
        }
    }
}