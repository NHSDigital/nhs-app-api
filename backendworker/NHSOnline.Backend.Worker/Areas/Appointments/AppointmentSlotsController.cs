using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Conventions;
using NHSOnline.Backend.Worker.Support.Logging;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    [Route("patient/appointment-slots"),PfsSecurityMode]
    public class AppointmentSlotsController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly ILogger<AppointmentSlotsController> _logger;
        private readonly IAuditor _auditor;

        public AppointmentSlotsController(
            IGpSystemFactory gpSystemFactory,
            IDateTimeOffsetProvider dateTimeOffsetProvider,
            ILogger<AppointmentSlotsController> logger,
            IAuditor auditor
        )
        {
            _gpSystemFactory = gpSystemFactory;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
            _logger = logger;
            _auditor = auditor;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogEnter(nameof(Get));
                
                await _auditor.Audit(Constants.AuditingTitles.GetSlotsAuditTypeRequest, "Attempting to get available appointments");

                var userSession = HttpContext.GetUserSession();
                _logger.LogDebug($"Fetch Appointment Slots Service for GP System: '{userSession.Supplier}'.");
                var appointmentService = _gpSystemFactory.CreateGpSystem(userSession.Supplier)
                    .GetAppointmentSlotsService();

                var dateRange = new AppointmentSlotsDateRange(_dateTimeOffsetProvider);
            
                var result = await appointmentService.GetSlots(userSession, dateRange);

                result.Accept(new AppointmentSlotsAuditingVisitor(_auditor));
                return result.Accept(new AppointmentSlotsResultVisitor());
            }
            finally
            {
                _logger.LogExit(nameof(Get));
            }
        }
    }
}
