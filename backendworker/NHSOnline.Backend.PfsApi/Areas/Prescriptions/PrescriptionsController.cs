using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Settings;
using static NHSOnline.Backend.Support.Constants.HttpHeaders;

namespace NHSOnline.Backend.PfsApi.Areas.Prescriptions
{
    [ApiVersionRoute("patient/prescriptions")]
    public class PrescriptionsController : Controller
    {
        private readonly ConfigurationSettings _settings;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<PrescriptionsController> _logger;
        private readonly IAuditor _auditor;
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;
        
        public PrescriptionsController(
            ConfigurationSettings settings,
            ILogger<PrescriptionsController> logger,
            IGpSystemFactory gpSystemFactory,
            IAuditor auditor,
            IErrorReferenceGenerator errorReferenceGenerator)
        {
            _settings = settings;
            _logger = logger;
            _gpSystemFactory = gpSystemFactory;
            _auditor = auditor;
            _errorReferenceGenerator = errorReferenceGenerator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DateTimeOffset? fromDate, [FromHeader(Name=PatientId)] Guid patientId)
        {
            try
            {
                _logger.LogEnter();
                
                var userSession = HttpContext.GetUserSession();

                await _auditor.Audit(AuditingOperations.RepeatPrescriptionsViewHistoryRequest, "Attempting to view prescriptions");

                var result = await GetPrescriptions(fromDate, userSession, patientId);
                
                var prescriptionCount = new FilteringCounts();
                if (result is GetPrescriptionsResult.Success successResult)
                {
                    prescriptionCount = successResult.FilteringCounts;
                    LogPrescriptionInformation(prescriptionCount);
                }
                
                await result.Accept(new GetPrescriptionsResultAuditingVisitor(_auditor, _logger, prescriptionCount));
                return result.Accept(new GetPrescriptionsResultVisitor(_errorReferenceGenerator, userSession));
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RepeatPrescriptionRequest repeatPrescriptionRequest, [FromHeader(Name=PatientId)] Guid patientId)
        {
            try
            {
                _logger.LogEnter();
                
                var userSession = HttpContext.GetUserSession();

                var courseIds = FormatCourseIds(repeatPrescriptionRequest?.CourseIds ?? new List<string>());
            
                await _auditor.Audit(AuditingOperations.RepeatPrescriptionsOrderRepeatMedicationsRequest, "Attempting to create a prescription request with course ids: {0}", courseIds);

                var result = await OrderPrescription(repeatPrescriptionRequest, userSession, patientId);

                await result.Accept(new OrderPrescriptionResultAuditingVisitor(_auditor, _logger, courseIds));
                return result.Accept(new OrderPrescriptionResultVisitor(_errorReferenceGenerator, userSession));
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<GetPrescriptionsResult> GetPrescriptions(DateTimeOffset? fromDate, UserSession userSession, Guid patientId)
        {
            var gpSystem = _gpSystemFactory
                .CreateGpSystem(userSession.GpUserSession.Supplier);

            _logger.LogInformation($"Fetching prescriptions validator for supplier {userSession.GpUserSession.Supplier}");
            var prescriptionRequestValidationService = gpSystem.GetPrescriptionValidationService();

            var defaultFromDate = GetDefaultFromDate();
            
            if (!ModelState.IsValid)
            {
                _logger.LogModelStateValidationFailure(ModelState);
                return new GetPrescriptionsResult.BadRequest();
            }
            
            if (!prescriptionRequestValidationService.IsGetValid(fromDate, defaultFromDate))
            {
                _logger.LogWarning($"Setting {nameof(fromDate)} to default {defaultFromDate:O} because value {fromDate:O} is earlier than allowed.");
                fromDate = defaultFromDate;
            }
            
            _logger.LogInformation($"Fetching prescriptions service implementation for supplier {userSession.GpUserSession.Supplier}");
            var prescriptionService = gpSystem.GetPrescriptionService();

            _logger.LogInformation($"Calling prescription service to get prescriptions");

            var gpLinkedAccountModel = new GpLinkedAccountModel(
                userSession.GpUserSession, patientId
            );
            return await prescriptionService.GetPrescriptions(gpLinkedAccountModel, fromDate, DateTimeOffset.Now);
        }

        private async Task<OrderPrescriptionResult> OrderPrescription(
            RepeatPrescriptionRequest repeatPrescriptionRequest, UserSession userSession, Guid patientId)
        {
            var gpSystem = _gpSystemFactory
                .CreateGpSystem(userSession.GpUserSession.Supplier);
            
            _logger.LogInformation($"Fetching prescriptions validator for supplier {userSession.GpUserSession.Supplier}");
            var prescriptionRequestValidationService = gpSystem.GetPrescriptionValidationService();

            if (!ModelState.IsValid)
            {
                _logger.LogModelStateValidationFailure(ModelState);
                return new OrderPrescriptionResult.BadRequest();
            }
            
            if (!prescriptionRequestValidationService.IsPostValid(repeatPrescriptionRequest))
            {
                _logger.LogWarning($"Invalid model state for {nameof(repeatPrescriptionRequest)}");
                return new OrderPrescriptionResult.BadRequest();
            }

            _logger.LogInformation($"Fetching prescriptions service implementation for supplier {userSession.GpUserSession.Supplier}");
            var prescriptionService = gpSystem.GetPrescriptionService();
            
            var gpLinkedAccountModel = new GpLinkedAccountModel(
                userSession.GpUserSession, patientId
            );

            _logger.LogInformation($"Calling prescription service to order prescriptions");
            return await prescriptionService.OrderPrescription(gpLinkedAccountModel, repeatPrescriptionRequest);
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
        
        private void LogPrescriptionInformation(FilteringCounts result) 
        {
            try
            {
                var kvp = new Dictionary<string, string>
                {
                    { "Prescriptions Received", 
                        result.ReceivedCount.ToString(CultureInfo.InvariantCulture) },
                    { "Prescriptions remaining after filtering out non-repeats", 
                        result.FilteredRemainingRepeatsCount.ToString(CultureInfo.InvariantCulture) },
                    { "Prescriptions filtered out for exceeding maximum allowance", 
                        result.FilteredMaxAllowanceDiscardedCount.ToString(CultureInfo.InvariantCulture) },
                    { "Prescriptions Returned to user", 
                        result.ReturnedCount.ToString(CultureInfo.InvariantCulture) }
                };
                
                _logger.LogInformationKeyValuePairs("Prescription Count", kvp);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to log prescription filtering details. " +
                                    "Catching exception to prevent inability to get prescriptions");
            }
        }
    }
}
