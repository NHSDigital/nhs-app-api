using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class SearchDeviceAuditingVisitor : ISearchDeviceResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly AccessToken _accessToken;
        private readonly string _auditType;
        private readonly ILogger<DevicesController> _logger;

        private const Supplier Supplier = Support.Supplier.Microsoft;

        public SearchDeviceAuditingVisitor(ILogger<DevicesController> logger, 
            IAuditor auditor,
            AccessToken accessToken, 
            string auditType)
        {
            _logger = logger;
            _auditor = auditor;
            _accessToken = accessToken;
            _auditType = auditType;
        }

        public async Task Visit(SearchDeviceResult.Found result)
        {
            try
            {
                var auditMessage = "User device registration for notifications found";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, _auditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(SearchDeviceResult.Found)}");
            }
        }

        public async Task Visit(SearchDeviceResult.NotFound result)
        {
            try
            {
                var auditMessage = "No user device registrations for notifications found";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, _auditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(SearchDeviceResult.NotFound)}");
            }
        }

        public async Task Visit(SearchDeviceResult.BadGateway result)
        {
            try
            {
                var auditMessage = "User devices registrations for notifications search unsuccessful due to BadGateway";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, _auditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(SearchDeviceResult.BadGateway)}");
            }
        }

        public async Task Visit(SearchDeviceResult.InternalServerError result)
        {
            try
            {
                var auditMessage = "User devices registrations for notifications search unsuccessful due to InternalServerError";
                await _auditor.AuditSecureTokenEvent(_accessToken, Supplier, _auditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(SearchDeviceResult.InternalServerError)}");
            }
        }
    }
}