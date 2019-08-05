using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class AppointmentSlotsAuditingVisitor : IAppointmentSlotsResultVisitor<Task>
    {
        private readonly ILogger<AppointmentSlotsController> _logger;
        private readonly IAuditor _auditor;
        private readonly UserSession _userSession;
        
        private const string AuditType = AuditingOperations.GetSlotsAuditTypeResponse;

        public AppointmentSlotsAuditingVisitor(IAuditor auditor, ILogger<AppointmentSlotsController> logger, UserSession userSession)
        {
            _auditor = auditor;
            _logger = logger;
            _userSession = userSession;
        }

        public async Task Visit(AppointmentSlotsResult.Success result)
        {
            var slotCount = result.Response?.Slots?.Count() ?? 0;

            try
            {
                await _auditor.Audit(AuditType, $"Available appointment slots successfully viewed - { slotCount } slots");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentSlotsResult.Success)}");
            }
            
            var kvp = new Dictionary<string, string>
            {
                { "Supplier", _userSession.GpUserSession.Supplier.ToString() },
                { "OdsCode", _userSession.GpUserSession.OdsCode },
                { "Count", slotCount.ToString(CultureInfo.InvariantCulture) }
            };

            _logger.LogInformationKeyValuePairs("Appointment Slot Count", kvp);   
        }

        public async Task Visit(AppointmentSlotsResult.BadGateway result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Available appointment slots view unsuccessful due to supplier unavailable");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentSlotsResult.BadGateway)}");
            }
        }

        public async Task Visit(AppointmentSlotsResult.InternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Available appointment slots view unsuccessful due to internal server error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentSlotsResult.InternalServerError)}");
            }
        }

        public async Task Visit(AppointmentSlotsResult.Forbidden result)
        {
            try
            {
                await _auditor.Audit(AuditType,  "Available appointment slots view unsuccessful due to not having permissions " +
                                                 "to book appointments");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentSlotsResult.Forbidden)}");
            }
        }
    }
}
