using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Prescriptions;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Prescriptions
{
    public class MicrotestPrescriptionService : IPrescriptionService
    {
        private readonly ILogger<MicrotestPrescriptionService> _logger;
        private readonly MicrotestConfigurationSettings _settings;
        private readonly IMicrotestClient _microtestClient;
        private readonly IMicrotestPrescriptionMapper _microtestPrescriptionMapper;
        private readonly IAuditor _auditor;

        public MicrotestPrescriptionService(ILogger<MicrotestPrescriptionService> logger,
            MicrotestConfigurationSettings settings,
            IMicrotestClient emisClient,
            IMicrotestPrescriptionMapper microtestPrescriptionMapper,
            IAuditor auditor)
        {
            _logger = logger;
            _microtestClient = emisClient;
            _settings = settings;
            _microtestPrescriptionMapper = microtestPrescriptionMapper;
            _auditor = auditor;
        }

        public async Task<GetPrescriptionsResult> GetPrescriptions(GpUserSession gpUserSession, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null)
        {
            var microtestUserSession = (MicrotestUserSession) gpUserSession;

            try
            {
                _logger.LogEnter();
                _logger.LogDebug("Beginning Fetch Prescriptions For User");

                var prescriptionsResponse = await _microtestClient.PrescriptionHistoryGet(
                    microtestUserSession.OdsCode,
                    microtestUserSession.NhsNumber,
                    fromDate);

                _logger.LogDebug("Fetch Prescriptions For User Complete");

                if (prescriptionsResponse.HasSuccessResponse)
                {
                    try
                    {
                        var prescriptionListResponseFiltered = await FilterPrescriptions(prescriptionsResponse.Body);

                        _logger.LogDebug($"Mapping successful response from {nameof(PrescriptionHistoryGetResponse)} to {nameof(PrescriptionListResponse)}");
                        var mappedPrescriptionList = _microtestPrescriptionMapper.Map(prescriptionListResponseFiltered);

                        if (mappedPrescriptionList.Prescriptions != null)
                        {
                            var allowedStatuses = new List<Status> { Status.Approved, Status.Rejected, Status.Requested };
                            mappedPrescriptionList.Prescriptions = mappedPrescriptionList.Prescriptions
                                .Where(x => x.Status.HasValue && allowedStatuses.Contains(x.Status.Value));
                        }

                        return new GetPrescriptionsResult.Success(mappedPrescriptionList);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Something went wrong building the Prescription History response");
                        return new GetPrescriptionsResult.InternalServerError();
                    }
                }
                else if (prescriptionsResponse.HasForbiddenResponse)
                {
                    _logger.LogError("Microtest prescriptions is not enabled");
                    return new GetPrescriptionsResult.Forbidden();
                }

                _logger.LogError(prescriptionsResponse.ErrorForLogging);
                return new GetPrescriptionsResult.BadGateway();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"Unsuccessful request retrieving prescriptions");
                return new GetPrescriptionsResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<OrderPrescriptionResult> OrderPrescription(GpUserSession gpUserSession, RepeatPrescriptionRequest repeatPrescriptionRequest)
        {
            var microtestUserSession = (MicrotestUserSession) gpUserSession;

            var postRequest = new PrescriptionRequestsPost
            {
                CourseIds = repeatPrescriptionRequest.CourseIds,
                SpecialRequestMessage = repeatPrescriptionRequest.SpecialRequest,
            };

            try
            {
                _logger.LogEnter();
                _logger.LogInformation("Beginning order prescription");

                var response = await _microtestClient.PrescriptionsPost(
                   microtestUserSession.OdsCode, microtestUserSession.NhsNumber, postRequest);

                if (response.HasSuccessResponse)
                {
                    _logger.LogDebug($"Prescription order placed successfully");
                    return new OrderPrescriptionResult.Success();
                }
                else if (response.HasForbiddenResponse)
                {
                    _logger.LogError("Microtest prescriptions is not enabled");
                    return new OrderPrescriptionResult.Forbidden();
                }

                _logger.LogError(response.ErrorForLogging);
                return new OrderPrescriptionResult.BadGateway();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"Repeat prescription order failed");
                return new OrderPrescriptionResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<PrescriptionHistoryGetResponse> FilterPrescriptions(PrescriptionHistoryGetResponse prescriptionsResponse)
        {
            List<PrescriptionCourse> repeatCourses = prescriptionsResponse.Courses
                                                     .Where(x => string.Equals(x.Type, PrescriptionType.Repeat, StringComparison.OrdinalIgnoreCase))
                                                     .OrderByDescending(x => x.OrderDate)
                                                     .ToList();

            var repeatCoursesAfterLimit = _settings.PrescriptionsMaxCoursesSoftLimit.HasValue ?
                repeatCourses.Take(_settings.PrescriptionsMaxCoursesSoftLimit.Value) : repeatCourses;

            var kvp = new Dictionary<string, string>
            {
                { "Prescriptions Received",  prescriptionsResponse.Courses.Count().ToString(CultureInfo.InvariantCulture) },
                { "Prescriptions remaining after filtering out non-repeats", repeatCourses.Count.ToString(CultureInfo.InvariantCulture) },
                { "Returned to user",  repeatCoursesAfterLimit.Count().ToString(CultureInfo.InvariantCulture) }
            };

            await _auditor.Audit(AuditingOperations.RepeatPrescriptionsViewHistoryResponse,
                    "Prescriptions received: {0}, Prescriptions remaining after filtering out non-repeats: {1}, Returned to user: {2}",
                    prescriptionsResponse.Courses.Count().ToString(CultureInfo.InvariantCulture),
                    repeatCourses.Count.ToString(CultureInfo.InvariantCulture),
                    repeatCoursesAfterLimit.Count().ToString(CultureInfo.InvariantCulture));

            _logger.LogInformationKeyValuePairs("Prescription Count", kvp);

            var newPrescriptionHistory = new PrescriptionHistoryGetResponse
            {
                Courses = repeatCoursesAfterLimit,
            };

            return newPrescriptionHistory;
        }
    }
}
