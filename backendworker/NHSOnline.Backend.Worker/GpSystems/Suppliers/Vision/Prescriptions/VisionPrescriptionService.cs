using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Worker.Support.Logging;
using NHSOnline.Backend.Worker.Settings;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Prescriptions
{
    public class VisionPrescriptionService : IPrescriptionService
    {
        private readonly ILogger<VisionPrescriptionService> _logger;
        private readonly ConfigurationSettings _settings;
        private readonly IVisionClient _visionClient;
        private readonly IVisionPrescriptionMapper _visionPrescriptionMapper;

        public VisionPrescriptionService(ILogger<VisionPrescriptionService> logger,
            IOptions<ConfigurationSettings> settings,
            IVisionClient visionClient, IVisionPrescriptionMapper visionPrescriptionMapper)
        {
            _logger = logger;
            _visionClient = visionClient;
            _settings = settings.Value;
            _visionPrescriptionMapper = visionPrescriptionMapper;
        }

        public async Task<PrescriptionResult> GetPrescriptions(
            UserSession userSession,
            DateTimeOffset? fromDate,
            DateTimeOffset? toDate)
        {
            _logger.LogEnter();

            var visionUserSession = (VisionUserSession) userSession.GpUserSession;

            if (!visionUserSession.IsRepeatPrescriptionsEnabled)
            {
                _logger.LogError("The Vision repeat prescriptions service is not enabled");
                return new PrescriptionResult.SupplierNotEnabled();
            }

            try
            {
                _logger.LogDebug("Beginning fetch prescriptions for user from Vision");
                
                var request = new PrescriptionRequest
                {
                    PatientId = visionUserSession.PatientId,
                };
                
                var prescriptionsResponse = await _visionClient.GetHistoricPrescriptions(visionUserSession, request);

                _logger.LogDebug("Fetch Prescriptions from Vision complete");
                
                if (!prescriptionsResponse.HasErrorResponse)
                {
                    try
                    {
                        var prescriptionHistory = prescriptionsResponse.Body.PrescriptionHistory;
                        
                        FilterAndSortPrescriptionHistoryRepeats(prescriptionHistory, fromDate, toDate);
                        
                        _logger.LogDebug($"Mapping successful response from {nameof(PrescriptionHistory)} to {nameof(PrescriptionListResponse)}");
                        var mappedPrescriptionList = _visionPrescriptionMapper.Map(prescriptionHistory);
                        
                        if (mappedPrescriptionList != null)
                        {
                            var allowedStatuses = new List<Areas.Prescriptions.Models.Status>
                            {
                                Areas.Prescriptions.Models.Status.Approved,
                                Areas.Prescriptions.Models.Status.Rejected,
                                Areas.Prescriptions.Models.Status.Requested,
                            };

                            mappedPrescriptionList.Prescriptions = mappedPrescriptionList.Prescriptions
                                .Where(x => x.Status.HasValue && allowedStatuses.Contains(x.Status.Value));
                        }

                        return new PrescriptionResult.SuccessfulGet(mappedPrescriptionList);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Something went wrong building the Prescription History response");
                        return new PrescriptionResult.InternalServerError();
                    }
                }
                
                _logger.LogError($"Vision system encountered an error: { prescriptionsResponse.ErrorForLogging }");
                return new PrescriptionResult.SupplierSystemUnavailable();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"Unsuccessful request retrieving prescriptions");
                return new PrescriptionResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<PrescriptionResult> OrderPrescription(UserSession userSession, RepeatPrescriptionRequest repeatPrescriptionRequest)
        {
            var visionUserSession = (VisionUserSession) userSession.GpUserSession;

            if (!visionUserSession.IsRepeatPrescriptionsEnabled)
            {
                _logger.LogError("The Vision repeat prescriptions service is not enabled");
                return new PrescriptionResult.SupplierNotEnabled();
            }

            if (!visionUserSession.AllowFreeTextPrescriptions && repeatPrescriptionRequest.SpecialRequest != null)
            {
                _logger.LogError($"User tried to submit prescription with { nameof(repeatPrescriptionRequest.SpecialRequest) } not null. " +
                    $"Stopped because Vision configuration for { nameof(visionUserSession.AllowFreeTextPrescriptions) } currently set to { visionUserSession.AllowFreeTextPrescriptions }");
                return new PrescriptionResult.BadRequest();
            }

            var postRequest = new OrderNewPrescriptionRequest
            {
                PatientId = visionUserSession.PatientId,
                Repeats = repeatPrescriptionRequest.CourseIds.Select(x => new NewPrescriptionRepeat { Id = x }).ToList(),
                Message = repeatPrescriptionRequest.SpecialRequest,
            };

            try
            {
                _logger.LogInformation($"Vision prescription order start. Ordering { postRequest.Repeats.Count } repeats");

                var response = await _visionClient.OrderNewPrescription(visionUserSession, postRequest);

                if (response.HasErrorResponse)
                {
                    _logger.LogError($"Vision response does not indicate a successful order: { response.ErrorForLogging }");
                    return new PrescriptionResult.SupplierSystemUnavailable();
                }

                if (!string.Equals(
                    response.RawResponse.Body.VisionResponse.ServiceContent.Result?.Trim(),
                    OrderNewPrescriptionResponse.OkResponseText,
                    StringComparison.Ordinal))
                {
                    _logger.LogError(
                        $"Vision response service content, " +
                        $"expected: { OrderNewPrescriptionResponse.OkResponseText }, " +
                        $"actual: { response.RawResponse.Body.VisionResponse.ServiceContent.Result }");
                    return new PrescriptionResult.SupplierSystemUnavailable();
                }
                
                _logger.LogDebug("Vision prescription order placed successfully");
                return new PrescriptionResult.SuccessfulPost();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Repeat prescription order failed with message { e.Message }");
                return new PrescriptionResult.SupplierSystemUnavailable();
            }
        }

        private void FilterAndSortPrescriptionHistoryRepeats(
            PrescriptionHistory prescriptionHistory,
            DateTimeOffset? fromDate,
            DateTimeOffset? toDate)
        {
            int initialPrescriptionCount = prescriptionHistory.Requests.Count;

            // Filter by dates on our side as Vision doesn't have server side filtering.
            if (fromDate.HasValue)
            {
                _logger.LogInformation($"Filtering Vision prescription history - {nameof(fromDate)}: {fromDate}");
                prescriptionHistory.Requests = prescriptionHistory.Requests.Where(x => x.Date.Date >= fromDate.Value.Date).ToList();
                _logger.LogDebug($"Filtered Vision prescription history - { prescriptionHistory.Requests.Count } after {nameof(fromDate)} filter");
            }

            if (toDate.HasValue)
            {
                _logger.LogInformation($"Filtering Vision prescription history - {nameof(toDate)}: {toDate}");
                prescriptionHistory.Requests = prescriptionHistory.Requests.Where(x => x.Date.Date <= toDate.Value.Date).ToList();
                _logger.LogDebug($"Filtered Vision prescription history - { prescriptionHistory.Requests.Count } after {nameof(toDate)} filter");
            }

            // Order prescriptions
            prescriptionHistory.Requests = prescriptionHistory.Requests.OrderByDescending(x => x.Date).ToList();

            // Limit
            int totalCoursesRunningTotal = 0;
            int prescriptionsToTake = 0;
            
            foreach (var prescription in prescriptionHistory.Requests)
            {
                if (totalCoursesRunningTotal >= _settings.PrescriptionsMaxCoursesSoftLimit.Value)
                {
                    _logger.LogWarning("Total courses exceeded maximum, discarding remainder.");
                    break;
                }

                totalCoursesRunningTotal += prescription.Repeats.Count;
                prescriptionsToTake++;
            }

            prescriptionHistory.Requests = prescriptionHistory.Requests.Take(prescriptionsToTake).ToList();

            _logger.LogInformation($"{ initialPrescriptionCount } prescriptions received. { prescriptionHistory.Requests.Count } being returned.");
        }
    }
}
