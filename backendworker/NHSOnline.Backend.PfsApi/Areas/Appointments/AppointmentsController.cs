using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    [Route("patient/appointments")]
    public class AppointmentsController : Controller
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly IAuditor _auditor;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IServiceJourneyRulesService _serviceJourneyRulesService;

        public AppointmentsController(
            ILogger<AppointmentsController> logger,
            IGpSystemFactory gpSystemFactory,
            IAuditor auditor,
            ISessionCacheService sessionCacheService,
            IServiceJourneyRulesService serviceJourneyRulesService
            )
        {
            _logger = logger;
            _gpSystemFactory = gpSystemFactory;
            _auditor = auditor;
            _sessionCacheService = sessionCacheService;
            _serviceJourneyRulesService = serviceJourneyRulesService;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] AppointmentCancelRequest model)
        {
            try
            {
                _logger.LogEnter();
                

                var userSession = HttpContext.GetUserSession();

                var appointmentValidator = GetAppointmentsValidationService(userSession);

                if (!appointmentValidator.IsDeleteValid(model))
                {
                    _logger.LogError("Invalid request body supplied to delete request");
                    return BadRequest();
                }

                await _auditor.Audit(Constants.AuditingTitles.CancelAppointmentAuditTypeRequest, $"Attempting to cancel appointment with id: {model.AppointmentId}");

                var appointmentsService = GetAppointmentsService(userSession);
                var cancelResult = await appointmentsService.Cancel(userSession.GpUserSession, model);

                await cancelResult.Accept(new AppointmentCancelAuditingVisitor(_auditor, _logger, model.AppointmentId));
                return cancelResult.Accept(new AppointmentCancelResultVisitor());
                
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogEnter();
                
                await _auditor.Audit(Constants.AuditingTitles.ViewAppointmentAuditTypeRequest, "Attempting to view booked appointments");
                              
                var userSession = HttpContext.GetUserSession();
                
                _logger.LogInformation("Checking Service Journey Rules for Appointments");
                
                AppointmentsResult result;

                var isActive = await _serviceJourneyRulesService.IsJourneyEnabled(userSession.GpUserSession.OdsCode);

                if (!isActive)
                {
                    result = new AppointmentsResult.CannotViewAppointments();
                }
                else
                {
                    var appointmentsService = GetAppointmentsService(userSession);
                    result = await appointmentsService.GetAppointments(userSession.GpUserSession);                    
                }

                await result.Accept(new AppointmentsAuditingVisitor(_auditor, _logger, userSession));
                return await result.Accept(new AppointmentsResultVisitor(_sessionCacheService, userSession));
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AppointmentBookRequest model)
        {
            try
            {
                _logger.LogEnter();

                await _auditor.Audit(Constants.AuditingTitles.BookAppointmentAuditTypeRequest,
                    $"Attempting to book appointment with id: {model.SlotId} and startTime: {model.StartTime:O}");

                var userSession = HttpContext.GetUserSession();

                var appointentValidator = GetAppointmentsValidationService(userSession);
                if (!appointentValidator.IsPostValid(model))
                {
                    _logger.LogError("Invalid request body supplied to post request");
                    return BadRequest();
                }

                var appointmentsService = GetAppointmentsService(userSession);
                var bookResult = await appointmentsService.Book(userSession.GpUserSession, model);

                await bookResult.Accept(new AppointmentBookAuditingVisitor(_auditor, _logger, model.SlotId, model.StartTime));
                return bookResult.Accept(new AppointmentBookResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private IAppointmentsService GetAppointmentsService(UserSession userSession)
        {
            _logger.LogDebug($"Fetch Appointments Service for GP System: '{userSession.GpUserSession.Supplier}'.");

            return _gpSystemFactory
                .CreateGpSystem(userSession.GpUserSession.Supplier)
                .GetAppointmentsService();
        }

        private IAppointmentsValidationService GetAppointmentsValidationService(UserSession userSession)
        {
            _logger.LogDebug($"Fetch Appointments Service for GP System: '{userSession.GpUserSession.Supplier}'.");

            return _gpSystemFactory
                .CreateGpSystem(userSession.GpUserSession.Supplier)
                .GetAppointmentsValidationService();
        }
    }
}
