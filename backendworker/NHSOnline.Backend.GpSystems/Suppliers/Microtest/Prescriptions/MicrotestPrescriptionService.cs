using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

        public MicrotestPrescriptionService(ILogger<MicrotestPrescriptionService> logger,
            MicrotestConfigurationSettings settings,
            IMicrotestClient emisClient,
            IMicrotestPrescriptionMapper microtestPrescriptionMapper)
        {
            _logger = logger;
            _microtestClient = emisClient;
            _settings = settings;
            _microtestPrescriptionMapper = microtestPrescriptionMapper;
        }

        public async Task<GetPrescriptionsResult> GetPrescriptions(GpLinkedAccountModel gpLinkedAccountModel, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null)
        {
            var microtestUserSession = (MicrotestUserSession) gpLinkedAccountModel.GpUserSession;

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
                        var (prescriptionListResponseFiltered, prescriptionsCount) = 
                            FilterPrescriptions(prescriptionsResponse.Body);

                        _logger.LogDebug($"Mapping successful response from {nameof(PrescriptionHistoryGetResponse)} to {nameof(PrescriptionListResponse)}");
                        var mappedPrescriptionList = _microtestPrescriptionMapper.Map(prescriptionListResponseFiltered);

                        if (mappedPrescriptionList.Prescriptions != null)
                        {
                            var allowedStatuses = new List<Status> { Status.Approved, Status.Rejected, Status.Requested };
                            mappedPrescriptionList.Prescriptions = mappedPrescriptionList.Prescriptions
                                .Where(x => x.Status.HasValue && allowedStatuses.Contains(x.Status.Value));
                        }

                        return new GetPrescriptionsResult.Success(mappedPrescriptionList, prescriptionsCount);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Something went wrong building the Prescription History response");
                        return new GetPrescriptionsResult.InternalServerError();
                    }
                }
                else if (prescriptionsResponse.HasForbiddenResponse)
                {
                    _logger.LogError("Microtest prescriptions are not enabled");
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

        public async Task<OrderPrescriptionResult> OrderPrescription(GpLinkedAccountModel gpLinkedAccountModel, RepeatPrescriptionRequest repeatPrescriptionRequest)
        {
            var microtestUserSession = (MicrotestUserSession) gpLinkedAccountModel.GpUserSession;

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
                
                if (response.IsPartialSuccess)
                {
                    _logger.LogDebug("Prescription order partially succeeded");

                    var mappedPrescriptionPartialSuccess = _microtestPrescriptionMapper.Map(response.Body);

                    _logger.LogInformation(
                        "Partial success ordering prescription. Attempted to order course ids: {0}, Successful course ids: {1}, Unsuccessful course ids: {2}",
                        string.Join(',', repeatPrescriptionRequest.CourseIds),
                        string.Join(',', mappedPrescriptionPartialSuccess.SuccessfulOrders.Select(x => x.CourseId)),
                        string.Join(',', mappedPrescriptionPartialSuccess.UnsuccessfulOrders.Select(x => x.CourseId))
                        );

                    return new OrderPrescriptionResult.PartialSuccess(mappedPrescriptionPartialSuccess);
                }
                else if (response.HasSuccessResponse)
                {
                    _logger.LogDebug("Prescription order placed successfully");
                    return new OrderPrescriptionResult.Success();
                }
                else if (response.HasForbiddenResponse)
                {
                    _logger.LogError("Microtest prescriptions is not enabled");
                    return new OrderPrescriptionResult.Forbidden();
                }
                else if (response.HasConflictResponse)
                {
                    _logger.LogError("The microtest prescription has already been ordered or is not available");
                    return new OrderPrescriptionResult.CannotReorderPrescription();
                }

                _logger.LogError(response.ErrorForLogging);
                return new OrderPrescriptionResult.BadGateway();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Repeat prescription order failed");
                return new OrderPrescriptionResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private (PrescriptionHistoryGetResponse newPrescriptionHistory, FilteringCounts prescriptionsCount)
            FilterPrescriptions(PrescriptionHistoryGetResponse prescriptionsResponse)
        {
            List<PrescriptionCourse> repeatCourses = prescriptionsResponse.Courses
                                                     .Where(x => string.Equals(x.Type, PrescriptionType.Repeat, 
                                                         StringComparison.OrdinalIgnoreCase))
                                                     .OrderByDescending(x => x.OrderDate)
                                                     .ToList();

            var repeatCoursesAfterLimit = _settings.PrescriptionsMaxCoursesSoftLimit.HasValue ?
                repeatCourses.Take(_settings.PrescriptionsMaxCoursesSoftLimit.Value) : repeatCourses;

            var prescriptionsReturnedToUserCount = repeatCoursesAfterLimit.Count();
            var numberOfPrescriptionsDiscarded = repeatCourses.Count - prescriptionsReturnedToUserCount;
            
            var prescriptionsCount = new FilteringCounts
            {
                ReceivedCount = prescriptionsResponse.Courses.Count(),
                FilteredRemainingRepeatsCount = repeatCourses.Count,
                FilteredMaxAllowanceDiscardedCount = numberOfPrescriptionsDiscarded,
                ReturnedCount = prescriptionsReturnedToUserCount
            };

            var newPrescriptionHistory = new PrescriptionHistoryGetResponse
            {
                Courses = repeatCoursesAfterLimit
            };

            return (newPrescriptionHistory, prescriptionsCount);
        }
    }
}
