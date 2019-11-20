using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    internal class RegistrationRepositoryAuditingVisitor : IDeviceRegistrationResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<DevicesController> _logger;
        private readonly AccessToken _accessToken;

        private const string AuditType = AuditingOperations.UsersDevicePostAuditTypeResponse;
        private const Supplier Supplier = Support.Supplier.Microsoft;

        public RegistrationRepositoryAuditingVisitor(IAuditor auditor, ILogger<DevicesController> logger, AccessToken accessToken)
        {
            _auditor = auditor;
            _logger = logger;
            _accessToken = accessToken;
        }

        public async Task Visit(DeviceRegistrationResult.Created result)
        {
            try
            {
                var auditMessage = "User device successfully added to the repository for push notifications";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DeviceRegistrationResult.Created)}");
            }
        }

        public async Task Visit(DeviceRegistrationResult.BadGateway result)
        {
            try
            {
                var auditMessage = "User device failed to be added to the repository for push notifications due to BadGateway";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DeviceRegistrationResult.BadGateway)}");
            }
        }

        public async Task Visit(DeviceRegistrationResult.InternalServerError result)
        {
            try
            {
                var auditMessage = "User device failed to be added to the repository for push notifications due to InternalServerError";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DeviceRegistrationResult.InternalServerError)}");
            }
        }
    }
}