using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Validators;
using NHSOnline.Backend.Worker.Session;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    [Route("patient/prescriptions")]
    public class PrescriptionsController : Controller
    {
        private readonly ConfigurationSettings _settings;
        private readonly ISystemProviderFactory _systemProviderFactory;
        private readonly ILogger<PrescriptionsController> _logger;
        private readonly IPrescriptionRequestValidationService _prescriptionRequestValidationService;

        public PrescriptionsController(
            IOptions<ConfigurationSettings> settings,
            ILoggerFactory loggerFactory,
            ISystemProviderFactory systemProviderFactory,
            IPrescriptionRequestValidationService prescriptionRequestValidationService)
        {
            _settings = settings.Value;
            _logger = loggerFactory.CreateLogger<PrescriptionsController>();
            _systemProviderFactory = systemProviderFactory;
            _prescriptionRequestValidationService = prescriptionRequestValidationService;
        }
        
        [HttpGet, TimeoutExceptionFilter]
        public async Task<IActionResult> Get([FromQuery] DateTimeOffset? fromDate)
        {
            var defaultFromDate = GetDefaultFromDate();
            
            if (!_prescriptionRequestValidationService.IsValidFromDate(fromDate, defaultFromDate))
            {
                _logger.LogWarning($"Setting {nameof(fromDate)} to default {defaultFromDate:O} because value {fromDate:O} is earlier than allowed.");
                fromDate = defaultFromDate;
            }

            UserSession userSession = HttpContext.GetUserSession();
            
            var prescriptionService = _systemProviderFactory
                .CreateSystemProvider(userSession.Supplier)
                .GetPrescriptionService();

            var result = await prescriptionService.Get(userSession, fromDate, DateTimeOffset.Now);

            return result.Accept(new PrescriptionResultVisitor());
        }

        private DateTimeOffset GetDefaultFromDate()
        {
            return DateTimeOffset.Now.AddMonths(-_settings.PrescriptionsDefaultLastNumberMonthsToDisplay.Value);
        }
    }
}
