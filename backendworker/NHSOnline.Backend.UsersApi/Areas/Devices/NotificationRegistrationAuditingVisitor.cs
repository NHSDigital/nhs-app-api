using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class NotificationRegistrationAuditingVisitor : IRegistrationResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<DevicesController> _logger;
        private readonly AccessToken _accessToken;

        private const string AuditType = AuditingOperations.UsersDevicePostAuditTypeResponse;
        private const Supplier Supplier = Support.Supplier.Microsoft;

        public NotificationRegistrationAuditingVisitor(IAuditor auditor, ILogger<DevicesController> logger, AccessToken accessToken)
        {
            _auditor = auditor;
            _logger = logger;
            _accessToken = accessToken;
        }
        
        public async Task Visit(RegistrationResult.Success result)
        {
            try
            {
                var auditMessage= "User device successfully registered for push notifications";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(RegistrationResult.Success)}");
            }
        }

        public async Task Visit(RegistrationResult.BadGateway result)
        {
            try
            {
                var auditMessage = "User device failed to register for push notifications due to BadGateway";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(RegistrationResult.BadGateway)}");
            }
        }

        public async Task Visit(RegistrationResult.InternalServerError result)
        {
            try
            {
                var auditMessage = "User device failed to register for push notifications due to InternalServerError";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(RegistrationResult.InternalServerError)}");
            }
        }
    }
}