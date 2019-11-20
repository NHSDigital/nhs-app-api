using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class DeleteDeviceAuditingVisitor : IDeleteDeviceResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly AccessToken _accessToken;
        private readonly string _auditType;
        private readonly ILogger<DevicesController> _logger;

        private const Supplier Supplier = Support.Supplier.Microsoft;

        public DeleteDeviceAuditingVisitor(
            ILogger<DevicesController> logger,
            IAuditor auditor,
            AccessToken accessToken, 
            string auditType)
        {
            _logger = logger;
            _auditor = auditor;
            _accessToken = accessToken;
            _auditType = auditType;
        }

        public async Task Visit(DeleteDeviceResult.Success result)
        {
            try
            {
                var auditMessage = "User device notification registration successfully deleted";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, _auditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(DeleteDeviceResult.Success)}");
            }
        }

        public async Task Visit(DeleteDeviceResult.BadGateway result)
        {
            try
            {
                var auditMessage = "User device notification registration deletion unsuccessful due to BadGateway";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, _auditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(DeleteDeviceResult.BadGateway)}");
            }
        }

        public async Task Visit(DeleteDeviceResult.InternalServerError result)
        {
            try
            {
                var auditMessage = "User device notification registration deletion unsuccessful due to InternalServerError";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, _auditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(DeleteDeviceResult.InternalServerError)}");
            }
        }
    }
}