using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class RegistrationExistsAuditingVisitor : IRegistrationExistsResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly AccessToken _accessToken;
        private readonly ILogger<DevicesController> _logger;

        private const Supplier Supplier = Support.Supplier.Microsoft;
        private const string AuditType = AuditingOperations.UsersDeviceGetAuditTypeResponse;

        public RegistrationExistsAuditingVisitor(ILogger<DevicesController> logger, IAuditor auditor, AccessToken accessToken)
        {
            _logger = logger;
            _auditor = auditor;
            _accessToken = accessToken;
        }

        public async Task Visit(RegistrationExistsResult.Found result)
        {
            try
            {
                var auditMessage = "User device registration found";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(RegistrationExistsResult.Found)}");
            }
        }

        public async Task Visit(RegistrationExistsResult.NotFound result)
        {
            try
            {
                var auditMessage = "User device registration not found";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(RegistrationExistsResult.NotFound)}");
            }
        }

        public async Task Visit(RegistrationExistsResult.BadGateway result)
        {
            try
            {
                var auditMessage = "User device registration search unsuccessful due to BadGateway";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(RegistrationExistsResult.BadGateway)}");
            }
        }

        public async Task Visit(RegistrationExistsResult.InternalServerError result)
        {
            try
            {
                var auditMessage = "User device registration search unsuccessful due to InternalServerError";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(RegistrationExistsResult.InternalServerError)}");
            }
        }
    }
}