using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using NHSOnline.Backend.Support.Logging;
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
                var upcomingAppointmentsCount = result.Response?.UpcomingAppointments?.Count() ?? 0;
                var pastAppointmentsCount = result.Response?.PastAppointments?.Count() ?? 0;

                const string messageFormat = "Booked appointments successfully viewed - {0} upcoming appointments" + 
                                             " and {1} historical appointments";

                var auditMessage = string.Format(CultureInfo.InvariantCulture, messageFormat, 
                    upcomingAppointmentsCount, pastAppointmentsCount);
                
                await _auditor.Audit(AuditType, auditMessage);

                var kvp = new Dictionary<string, string>
                {
                    { "Supplier", _userSession.GpUserSession.Supplier.ToString() },
                    { "OdsCode", _userSession.GpUserSession.OdsCode },
                    { "Count", (upcomingAppointmentsCount+pastAppointmentsCount).ToString(CultureInfo.InvariantCulture) },
                    { "UpcomingCount", upcomingAppointmentsCount.ToString(CultureInfo.InvariantCulture) },
                    { "HistoricalCount", pastAppointmentsCount.ToString(CultureInfo.InvariantCulture)}
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
