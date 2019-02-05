using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Logging;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    public class AppointmentsAuditingVisitor : IAppointmentsResultVisitor<Task>
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

        public async Task Visit(AppointmentsResult.SuccessfullyRetrieved result)
        {
            try
            {
                var appointmentCount = result.Response?.UpcomingAppointments?.Count() ?? 0;
            
                await _auditor.Audit(AuditType, $"Booked appointments successfully viewed - { appointmentCount } appointments");

                var kvp = new Dictionary<string, string>
                {
                    { "Supplier", _userSession.GpUserSession.Supplier.ToString() },
                    { "OdsCode", _userSession.GpUserSession.OdsCode },
                    { "Count", appointmentCount.ToString(CultureInfo.InvariantCulture) }
                };

                _logger.LogInformationKeyValuePairs("Appointment Count", kvp);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentsResult.SuccessfullyRetrieved)}");
            }
        }

        public async Task Visit(AppointmentsResult.BadRequest result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Booked appointments view unsuccessful due to bad request");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentsResult.BadRequest)}");
            }
        }

        public async Task Visit(AppointmentsResult.SupplierSystemUnavailable result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Booked appointments view unsuccessful due to supplier being unavailable");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentsResult.SupplierSystemUnavailable)}");
            }
        }

        public async Task Visit(AppointmentsResult.InternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Booked appointments view unsuccessful due to internal server error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentsResult.InternalServerError)}");
            }
        }

        public async Task Visit(AppointmentsResult.CannotViewAppointments result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Booked appointments view unsuccessful");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentsResult.CannotViewAppointments)}");
            }
        }
    }
}
