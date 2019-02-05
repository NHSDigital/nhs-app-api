using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    public class AppointmentSlotsAuditingVisitor : IAppointmentSlotsResultVisitor<Task>
    {
        private readonly ILogger<AppointmentSlotsController> _logger;
        private readonly IAuditor _auditor;
        private readonly UserSession _userSession;
        
        private const string AuditType = Constants.AuditingTitles.GetSlotsAuditTypeResponse;

        public AppointmentSlotsAuditingVisitor(IAuditor auditor, ILogger<AppointmentSlotsController> logger, UserSession userSession)
        {
            _auditor = auditor;
            _logger = logger;
            _userSession = userSession;
        }

        public async Task Visit(AppointmentSlotsResult.SuccessfullyRetrieved result)
        {
            var slotCount = result.Response?.Slots?.Count() ?? 0;

            try
            {
                await _auditor.Audit(AuditType, $"Available appointment slots successfully viewed - { slotCount } slots");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentSlotsResult.SuccessfullyRetrieved)}");
            }
            
            var kvp = new Dictionary<string, string>
            {
                { "Supplier", _userSession.GpUserSession.Supplier.ToString() },
                { "OdsCode", _userSession.GpUserSession.OdsCode },
                { "Count", slotCount.ToString(CultureInfo.InvariantCulture) }
            };

            _logger.LogInformationKeyValuePairs("Appointment Slot Count", kvp);   
        }

        public async Task Visit(AppointmentSlotsResult.SupplierSystemUnavailable result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Available appointment slots view unsuccessful due to supplier unavailable");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentSlotsResult.SupplierSystemUnavailable)}");
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

        public async Task Visit(AppointmentSlotsResult.CannotBookAppointments result)
        {
            try
            {
                await _auditor.Audit(AuditType,  "Available appointment slots view unsuccessful due to not having permissions " +
                                                 "to book appointments");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentSlotsResult.CannotBookAppointments)}");
            }
        }
    }
}
