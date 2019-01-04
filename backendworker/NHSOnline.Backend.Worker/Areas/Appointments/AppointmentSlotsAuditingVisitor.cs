using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    public class AppointmentSlotsAuditingVisitor : IAppointmentSlotsResultVisitor<object>
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

        public object Visit(AppointmentSlotsResult.SuccessfullyRetrieved result)
        {
            var slotCount = result.Response?.Slots?.Count() ?? 0;
            
            _auditor.Audit(AuditType, $"Available appointment slots successfully viewed - { slotCount } slots");

            var kvp = new Dictionary<string, string>
            {
                { "Supplier", _userSession.GpUserSession.Supplier.ToString() },
                { "OdsCode", _userSession.GpUserSession.OdsCode },
                { "Count", slotCount.ToString(CultureInfo.InvariantCulture) }
            };

            _logger.LogInformationKeyValuePairs("Appointment Slot Count", kvp);
            
            return null;
        }

        public object Visit(AppointmentSlotsResult.SupplierSystemUnavailable result)
        {
            _auditor.Audit(AuditType, "Available appointment slots view unsuccessful due to supplier unavailable");

            return null;
        }

        public object Visit(AppointmentSlotsResult.InternalServerError result)
        {
            _auditor.Audit(AuditType, "Available appointment slots view unsuccessful due to internal server error");

            return null;
        }

        public object Visit(AppointmentSlotsResult.CannotBookAppointments result)
        {
            _auditor.Audit(AuditType,  "Available appointment slots view unsuccessful due to not having permissions " +
                                       "to book appointments");

            return null;
        }
    }
}
