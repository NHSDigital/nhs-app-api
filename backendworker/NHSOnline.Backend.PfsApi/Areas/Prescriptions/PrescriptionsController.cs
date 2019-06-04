using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.Areas.Prescriptions
{
    [Route("patient/prescriptions")]
    public class PrescriptionsController : Controller
    {
        private readonly ConfigurationSettings _settings;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<PrescriptionsController> _logger;
        private readonly IAuditor _auditor;
        
        public PrescriptionsController(
            ConfigurationSettings settings,
            ILogger<PrescriptionsController> logger,
            IGpSystemFactory gpSystemFactory,
            IAuditor auditor)
        {
            _settings = settings;
            _logger = logger;
            _gpSystemFactory = gpSystemFactory;
            _auditor = auditor;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DateTimeOffset? fromDate)
        {
            var defaultFromDate = GetDefaultFromDate();

            UserSession userSession = HttpContext.GetUserSession();

            var gpSystem = _gpSystemFactory
                .CreateGpSystem(userSession.GpUserSession.Supplier);

            _logger.LogInformation($"Fetching prescriptions validator for supplier {userSession.GpUserSession.Supplier}");
            var prescriptionRequestValidationService = gpSystem.GetPrescriptionRequestValidationService();

            if (!prescriptionRequestValidationService.IsGetValid(fromDate, defaultFromDate))
            {
                _logger.LogWarning($"Setting {nameof(fromDate)} to default {defaultFromDate:O} because value {fromDate:O} is earlier than allowed.");
                fromDate = defaultFromDate;
            }
            
            _logger.LogInformation($"Fetching prescriptions service implementation for supplier {userSession.GpUserSession.Supplier}");
            var prescriptionService = gpSystem.GetPrescriptionService();

            await _auditor.Audit(Constants.AuditingTitles.RepeatPrescriptionsViewHistoryRequest, "Attempting to view prescriptions");

            _logger.LogInformation($"Calling prescription service to get prescriptions");
            var result = await prescriptionService.GetPrescriptions(userSession.GpUserSession, fromDate, DateTimeOffset.Now);
            
            await result.Accept(new GetPrescriptionsResultAuditingVisitor(_auditor, _logger));
            return result.Accept(new GetPrescriptionsResultVisitor());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RepeatPrescriptionRequest repeatPrescriptionRequest)
        {
            OrderPrescriptionResult result;
            UserSession userSession = HttpContext.GetUserSession();

            var gpSystem = _gpSystemFactory
                .CreateGpSystem(userSession.GpUserSession.Supplier);

            var courseIds = FormatCourseIds(repeatPrescriptionRequest.CourseIds);
            
            await _auditor.Audit(Constants.AuditingTitles.RepeatPrescriptionsOrderRepeatMedicationsRequest, "Attempting to create a prescription request with course ids: {0}", courseIds);
            
            _logger.LogInformation($"Fetching prescriptions validator for supplier {userSession.GpUserSession.Supplier}");
            var prescriptionRequestValidationService = gpSystem.GetPrescriptionRequestValidationService();

            if (!prescriptionRequestValidationService.IsPostValid(repeatPrescriptionRequest))
            {
                _logger.LogWarning($"Invalid model state for {nameof(repeatPrescriptionRequest)}");
                result = new OrderPrescriptionResult.BadRequest();
            }
            else
            {
                _logger.LogInformation($"Fetching prescriptions service implementation for supplier {userSession.GpUserSession.Supplier}");
                var prescriptionService = gpSystem.GetPrescriptionService();

                _logger.LogInformation($"Calling prescription service to order prescriptions");
                result = await prescriptionService.OrderPrescription(userSession.GpUserSession, repeatPrescriptionRequest);      
            }

            await result.Accept(new OrderPrescriptionResultAuditingVisitor(_auditor, _logger, courseIds));
            return result.Accept(new OrderPrescriptionResultVisitor());
        }

        private DateTimeOffset GetDefaultFromDate()
        {
            return DateTimeOffset.Now.AddMonths(-_settings.PrescriptionsDefaultLastNumberMonthsToDisplay.Value);
        }

        private static string FormatCourseIds(IEnumerable<string> courseIds)
        {
            var enumerable = courseIds.ToList();
            return !EnumerableExtensions.Any(enumerable) ? "No course ID's provided" : string.Join(",", enumerable);
        }
    }
}
