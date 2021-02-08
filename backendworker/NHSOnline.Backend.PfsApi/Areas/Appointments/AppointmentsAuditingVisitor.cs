using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Appointments;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class AppointmentsAuditingVisitor : IAppointmentsResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<AppointmentsController> _logger;

        private const string AuditType = AuditingOperations.ViewAppointmentAuditTypeResponse;

        public AppointmentsAuditingVisitor(IAuditor auditor, ILogger<AppointmentsController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }

        public async Task Visit(AppointmentsResult.Success result)
        {
            try
            {
                var upcomingAppointmentsCount = result.Response?.UpcomingAppointments?.Count() ?? 0;
                var pastAppointmentsCount = result.Response?.PastAppointments?.Count() ?? 0;

                const string messageFormat = "Booked appointments successfully viewed - {0} upcoming appointments" +
                                             " and {1} historical appointments";

                var auditMessage = string.Format(CultureInfo.InvariantCulture, messageFormat,
                    upcomingAppointmentsCount, pastAppointmentsCount);

                await _auditor.PostOperationAudit(AuditType, auditMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentsResult.Success)}");
            }
        }

        public async Task Visit(AppointmentsResult.BadRequest result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Booked appointments view unsuccessful due to bad request");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentsResult.BadRequest)}");
            }
        }

        public async Task Visit(AppointmentsResult.BadGateway result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Booked appointments view unsuccessful due to supplier being unavailable");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentsResult.BadGateway)}");
            }
        }

        public async Task Visit(AppointmentsResult.InternalServerError result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Booked appointments view unsuccessful due to internal server error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentsResult.InternalServerError)}");
            }
        }

        public async Task Visit(AppointmentsResult.Forbidden result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Booked appointments view unsuccessful due to insufficient permissions");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentsResult.Forbidden)}");
            }
        }
    }
}
