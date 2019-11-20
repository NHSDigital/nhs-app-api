using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class DeleteRegistrationAuditingVisitor : IDeleteRegistrationResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly AccessToken _accessToken;
        private readonly ILogger<DevicesController> _logger;

        private const Supplier Supplier = Support.Supplier.Microsoft;
        private const string AuditType = AuditingOperations.UsersDeviceDeleteAuditTypeResponse;

        public DeleteRegistrationAuditingVisitor(ILogger<DevicesController> logger, IAuditor auditor, AccessToken accessToken)
        {
            _logger = logger;
            _auditor = auditor;
            _accessToken = accessToken;
        }

        public async Task Visit(DeleteRegistrationResult.Success result)
        {
            try
            {
                var auditMessage = "User device notification registration successfully deleted";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DeleteRegistrationResult.Success)}");
            }
        }

        public async Task Visit(DeleteRegistrationResult.BadGateway result)
        {
            try
            {
                var auditMessage = "User device notification registration deletion unsuccessful due to BadGateway";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DeleteRegistrationResult.BadGateway)}");
            }
        }

        public async Task Visit(DeleteRegistrationResult.InternalServerError result)
        {
            try
            {
                var auditMessage = "User device notification registration deletion unsuccessful due to InternalServerError";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(DeleteRegistrationResult.InternalServerError)}");
            }
        }
    }
}