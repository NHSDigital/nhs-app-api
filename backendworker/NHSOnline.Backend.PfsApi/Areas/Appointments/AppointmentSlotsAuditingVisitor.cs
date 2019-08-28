using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Appointments;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class AppointmentSlotsAuditingVisitor : IAppointmentSlotsResultVisitor<Task>
    {
        private readonly ILogger<AppointmentSlotsController> _logger;
        private readonly IAuditor _auditor;
        
        private const string AuditType = AuditingOperations.GetSlotsAuditTypeResponse;

        public AppointmentSlotsAuditingVisitor(IAuditor auditor, ILogger<AppointmentSlotsController> logger)
        {
            _auditor = auditor;
            _logger = logger;
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
