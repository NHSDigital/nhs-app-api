using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    [Route("patient/prescriptions")]
    public class PrescriptionsController : Controller
    {
        private readonly ConfigurationSettings _settings;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<PrescriptionsController> _logger;
        private readonly IPrescriptionRequestValidationService _prescriptionRequestValidationService;

        public PrescriptionsController(
            IOptions<ConfigurationSettings> settings,
            ILoggerFactory loggerFactory,
            IGpSystemFactory gpSystemFactory,
            IPrescriptionRequestValidationService prescriptionRequestValidationService)
        {
            _settings = settings.Value;
            _logger = loggerFactory.CreateLogger<PrescriptionsController>();
            _gpSystemFactory = gpSystemFactory;
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

            var prescriptionService = _gpSystemFactory
                .CreateGpSystem(userSession.Supplier)
                .GetPrescriptionService();

            var result = await prescriptionService.Get(userSession, fromDate, DateTimeOffset.Now);
            return result.Accept(new PrescriptionResultVisitor());
        }

        [HttpPost, TimeoutExceptionFilter]
        public async Task<IActionResult> Post([FromBody] RepeatPrescriptionRequest repeatPrescriptionRequest)
        {
            PrescriptionResult result;

            if (!_prescriptionRequestValidationService.IsValidRepeatPrescriptionRequest(repeatPrescriptionRequest))
            {
                _logger.LogWarning($"Invalid model state for {nameof(repeatPrescriptionRequest)}");
                result = new PrescriptionResult.BadRequest();
            }
            else
            {
                UserSession userSession = HttpContext.GetUserSession();

                var prescriptionService = _gpSystemFactory
                    .CreateGpSystem(userSession.Supplier)
                    .GetPrescriptionService();

                result = await prescriptionService.Post(userSession, repeatPrescriptionRequest);
            }

            return result.Accept(new PrescriptionResultVisitor());
        }

        private DateTimeOffset GetDefaultFromDate()
        {
            return DateTimeOffset.Now.AddMonths(-_settings.PrescriptionsDefaultLastNumberMonthsToDisplay.Value);
        }
    }
}
