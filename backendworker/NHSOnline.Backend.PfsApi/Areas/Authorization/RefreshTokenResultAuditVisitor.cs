using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.CitizenId;

namespace NHSOnline.Backend.PfsApi.Areas.Authorization
{
    public class RefreshTokenResultAuditVisitor : IRefreshTokenResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger _logger;

        private readonly string AuditType = AuditingOperations.PostRefreshPatientAccessTokenResponse;

        public RefreshTokenResultAuditVisitor(IAuditor auditor, ILogger logger)
        {
            _auditor = auditor;
            _logger = logger;
        }

        public async Task Visit(RefreshAccessTokenResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Refresh of access token successful");
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception thrown auditing {AuditType} {nameof(RefreshAccessTokenResult.Success)}", e);
            }
        }

        public async Task Visit(RefreshAccessTokenResult.BadGateway result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Refresh of access token unsuccessful due to bad gateway");
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception thrown auditing {AuditType} {nameof(RefreshAccessTokenResult.BadGateway)}", e);
            }
        }

        public async Task Visit(RefreshAccessTokenResult.InternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Refresh of access token unsuccessful due to internal server error");
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception thrown auditing {AuditType} {nameof(RefreshAccessTokenResult.InternalServerError)}", e);
            }
        }
    }
}