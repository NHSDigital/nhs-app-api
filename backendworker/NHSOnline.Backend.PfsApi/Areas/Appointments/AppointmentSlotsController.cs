using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support.Auditing;
using NHSOnline.Backend.Support.Temporal;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.ApiSupport;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    [Route("patient/appointment-slots"),PfsSecurityMode]
    public class AppointmentSlotsController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly ILogger<AppointmentSlotsController> _logger;
        private readonly IAuditor _auditor;
        private readonly IAppointmentSlotTypeScraper _appointmentSlotTypeScraper;

        public AppointmentSlotsController(
            IGpSystemFactory gpSystemFactory,
            IDateTimeOffsetProvider dateTimeOffsetProvider,
            ILogger<AppointmentSlotsController> logger,
            IAuditor auditor,
            IAppointmentSlotTypeScraper appointmentSlotTypeScraper
        )
        {
            _gpSystemFactory = gpSystemFactory;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
            _logger = logger;
            _auditor = auditor;
            _appointmentSlotTypeScraper = appointmentSlotTypeScraper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogEnter();
                
                await _auditor.Audit(Constants.AuditingTitles.GetSlotsAuditTypeRequest, "Attempting to get available appointments");

                var userSession = HttpContext.GetUserSession();
                _logger.LogDebug($"Fetch Appointment Slots Service for GP System: '{userSession.GpUserSession.Supplier}'.");
                var appointmentService = _gpSystemFactory.CreateGpSystem(userSession.GpUserSession.Supplier)
                    .GetAppointmentSlotsService();

                var dateRange = new AppointmentSlotsDateRange(_dateTimeOffsetProvider);
            
                var result = await appointmentService.GetSlots(userSession.GpUserSession, dateRange);

                try
                {
                    _appointmentSlotTypeScraper.CaptureAppointmentSlotTypes(userSession, result);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unable to log appointment slot type details. " +
                                        "Catching exception to prevent inability to create appointment");
                }

                await result.Accept(new AppointmentSlotsAuditingVisitor(_auditor, _logger, userSession));
                return result.Accept(new AppointmentSlotsResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
