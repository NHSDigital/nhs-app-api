using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    public class AppointmentsAuditingVisitor : IAppointmentsResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<AppointmentsController> _logger;
        private readonly UserSession _userSession;
        
        private const string AuditType = Constants.AuditingTitles.ViewAppointmentAuditTypeResponse;

        public AppointmentsAuditingVisitor(IAuditor auditor, ILogger<AppointmentsController> logger, UserSession userSession)
        {
            _auditor = auditor;
            _logger = logger;
            _userSession = userSession;
        }

        public object Visit(AppointmentsResult.SuccessfullyRetrieved result)
        {
            var appointmentCount = result.Response?.Appointments?.Count() ?? 0;
            
            _auditor.Audit(AuditType, $"Booked appointments successfully viewed - { appointmentCount } appointments");

            var kvp = new Dictionary<string, string>
            {
                { "Supplier", _userSession.GpUserSession.Supplier.ToString() },
                { "OdsCode", _userSession.GpUserSession.OdsCode },
                { "Count", appointmentCount.ToString(CultureInfo.InvariantCulture) }
            };

            _logger.LogInformationKeyValuePairs("Appointment Count", kvp);
            
            return null;
        }

        public object Visit(AppointmentsResult.BadRequest result)
        {
            _auditor.Audit(AuditType, "Booked appointments view unsuccessful due to bad request");

            return null;
        }

        public object Visit(AppointmentsResult.SupplierSystemUnavailable result)
        {
            _auditor.Audit(AuditType, "Booked appointments view unsuccessful due to supplier being unavailable");

            return null;
        }

        public object Visit(AppointmentsResult.InternalServerError result)
        {
            _auditor.Audit(AuditType, "Booked appointments view unsuccessful due to internal server error");

            return null;
        }

        public object Visit(AppointmentsResult.CannotViewAppointments result)
        {
            _auditor.Audit(AuditType, "Booked appointments view unsuccessful");

            return null;
        }
    }
}
