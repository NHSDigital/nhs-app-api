using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.Settings;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    [Route("patient/prescriptions")]
    public class PrescriptionsController : Controller
    {
        private readonly ConfigurationSettings _settings;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<PrescriptionsController> _logger;

        public PrescriptionsController(
            IOptions<ConfigurationSettings> settings,
            ILoggerFactory loggerFactory,
            IGpSystemFactory gpSystemFactory)
        {
            _settings = settings.Value;
            _logger = loggerFactory.CreateLogger<PrescriptionsController>();
            _gpSystemFactory = gpSystemFactory;
        }

        [HttpGet, TimeoutExceptionFilter]
        public async Task<IActionResult> Get([FromQuery] DateTimeOffset? fromDate)
        {
            var defaultFromDate = GetDefaultFromDate();

            UserSession userSession = HttpContext.GetUserSession();

            var gpSystem = _gpSystemFactory
                .CreateGpSystem(userSession.Supplier);

            _logger.LogInformation($"Fetching prescriptions validator for supplier {userSession.Supplier}");
            var prescriptionRequestValidationService = gpSystem.GetPrescriptionRequestValidationService();

            if (!prescriptionRequestValidationService.IsValidFromDate(fromDate, defaultFromDate))
            {
                _logger.LogWarning($"Setting {nameof(fromDate)} to default {defaultFromDate:O} because value {fromDate:O} is earlier than allowed.");
                fromDate = defaultFromDate;
            }
            
            _logger.LogInformation($"Fetching prescriptions service implementation for supplier {userSession.Supplier}");
            var prescriptionService = gpSystem.GetPrescriptionService();

            _logger.LogInformation($"Calling prescription service to get prescriptions");
            var result = await prescriptionService.GetPrescriptions(userSession, fromDate, DateTimeOffset.Now);

            return result.Accept(new PrescriptionResultVisitor());
        }

        [HttpPost, TimeoutExceptionFilter]
        public async Task<IActionResult> Post([FromBody] RepeatPrescriptionRequest repeatPrescriptionRequest)
        {
            PrescriptionResult result;
            UserSession userSession = HttpContext.GetUserSession();

            var gpSystem = _gpSystemFactory
                .CreateGpSystem(userSession.Supplier);
            
            _logger.LogInformation($"Fetching prescriptions validator for supplier {userSession.Supplier}");
            var prescriptionRequestValidationService = gpSystem.GetPrescriptionRequestValidationService();

            if (!prescriptionRequestValidationService.IsValidRepeatPrescriptionRequest(repeatPrescriptionRequest))
            {
                _logger.LogWarning($"Invalid model state for {nameof(repeatPrescriptionRequest)}");
                result = new PrescriptionResult.BadRequest();
            }
            else
            {
                _logger.LogInformation($"Fetching prescriptions service implementation for supplier {userSession.Supplier}");
                var prescriptionService = gpSystem.GetPrescriptionService();

                _logger.LogInformation($"Calling prescription service to order prescriptions");
                result = await prescriptionService.OrderPrescription(userSession, repeatPrescriptionRequest);      
            }

            return result.Accept(new PrescriptionResultVisitor());
        }

        private DateTimeOffset GetDefaultFromDate()
        {
            return DateTimeOffset.Now.AddMonths(-_settings.PrescriptionsDefaultLastNumberMonthsToDisplay.Value);
        }
    }
}
