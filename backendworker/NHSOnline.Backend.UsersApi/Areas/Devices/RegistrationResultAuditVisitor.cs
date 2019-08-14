using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class RegistrationResultAuditVisitor : IRegistrationResultAuditVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<DevicesController> _logger;
        private const string AuditType = AuditingOperations.RegisterUsersDeviceAuditTypeResponse;

        public RegistrationResultAuditVisitor(IAuditor auditor, ILogger<DevicesController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }
        
        public async Task Visit(RegistrationResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Device successfully registered for push notifications");
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
                await _auditor.Audit(AuditType, "Device failed to register for push notifications");
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
                await _auditor.Audit(AuditType, "Device failed to register for push notifications");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(RegistrationResult.InternalServerError)}");
            }
        }
    }
}