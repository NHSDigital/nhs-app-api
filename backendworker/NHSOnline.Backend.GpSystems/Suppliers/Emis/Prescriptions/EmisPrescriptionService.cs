using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NHSOnline.Backend.Support.Auditing;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Prescriptions
{
    public class EmisPrescriptionService : IPrescriptionService
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<EmisPrescriptionService> _logger;
        private readonly EmisConfigurationSettings _settings;
        private readonly IEmisClient _emisClient;
        private readonly IEmisPrescriptionMapper _emisPrescriptionMapper;

        public EmisPrescriptionService(IAuditor auditor, ILogger<EmisPrescriptionService> logger,
            EmisConfigurationSettings settings,
            IEmisClient emisClient, IEmisPrescriptionMapper emisPrescriptionMapper)
        {
            _auditor = auditor;
            _logger = logger;
            _emisClient = emisClient;
            _settings = settings;
            _emisPrescriptionMapper = emisPrescriptionMapper;

            _settings.Validate();
        }

        public async Task<GetPrescriptionsResult> GetPrescriptions(GpUserSession gpUserSession, DateTimeOffset? fromDate,
            DateTimeOffset? toDate)
        {
            var emisUserSession = (EmisUserSession) gpUserSession;
            try
            {
                _logger.LogEnter();
                _logger.LogDebug("Beginning Fetch Prescriptions For User");

                var prescriptionsResponse = await _emisClient.PrescriptionsGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, emisUserSession.EndUserSessionId, fromDate, toDate);

                _logger.LogDebug("Fetch Prescriptions For User Complete");

                if (prescriptionsResponse.HasSuccessResponse)
                {
                    try
                    {
                        var prescriptionListResponseFiltered =
                            await GetPrescriptionsWithoutRepeatCourses(prescriptionsResponse.Body);


                        _logger.LogDebug($"Mapping successful response from {nameof(PrescriptionRequestsGetResponse)} to {nameof(PrescriptionListResponse)}");
                        var mappedPrescriptionList = _emisPrescriptionMapper.Map(prescriptionListResponseFiltered);

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
                        _logger.LogError(e, $"Something went wrong building the Prescription History response");
                        return new GetPrescriptionsResult.InternalServerError();
                    }
                }

                return InterpretGetPrescriptionsError(prescriptionsResponse);
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

        private async Task<PrescriptionRequestsGetResponse> GetPrescriptionsWithoutRepeatCourses(
            PrescriptionRequestsGetResponse prescriptionsResponse)
        {
            const string auditType = Constants.AuditingTitles.RepeatPrescriptionsViewHistoryResponse;

            int totalCoursesRunningTotal = 0;
            var repeatCourses = prescriptionsResponse.MedicationCourses
                                                     .Where(x => x.PrescriptionType == PrescriptionType.Repeat)
                                                     .ToList();

            var repeatCourseIdLookup = repeatCourses.Select(x => x.MedicationCourseGuid)
                                                    .Distinct()
                                                    .ToDictionary(x => x);

            var filteredPrescriptionsCount = 0;

            var prescriptionsWithRepeatCourses = new List<PrescriptionRequest>();

            var coursesBeforeFiltering = prescriptionsResponse.PrescriptionRequests
                .Sum(x => x.RequestedMedicationCourses.Count())
                .ToString(CultureInfo.InvariantCulture);

            await _auditor.Audit(auditType, "Total courses before filtering: {0}", coursesBeforeFiltering);

            foreach (var prescription in prescriptionsResponse.PrescriptionRequests.OrderByDescending(x =>
                x.DateRequested))
            {
                if (totalCoursesRunningTotal >= _settings.PrescriptionsMaxCoursesSoftLimit)
                {
                    _logger.LogInformation($"The number of courses has reached {totalCoursesRunningTotal} which has exceeded the maximum, discarding the remainder.");
                    break;
                }

                var requestedMedicationCourses = prescription.RequestedMedicationCourses.ToList();

                var repeatCoursesInPrescription = 
                    requestedMedicationCourses
                    .Where(x => repeatCourseIdLookup.ContainsKey(x.RequestedMedicationCourseGuid))
                    .ToList();

                if (repeatCoursesInPrescription.Count != 0)
                {
                    prescription.RequestedMedicationCourses = repeatCoursesInPrescription;
                    prescriptionsWithRepeatCourses.Add(prescription);
                }
                else
                {
                    filteredPrescriptionsCount += 1;
                }

                _logger.LogInformation($"{repeatCoursesInPrescription.Count} repeat courses in this prescription");

                totalCoursesRunningTotal += repeatCoursesInPrescription.Count;
            }

            var prescriptionsReturnedToUserCount = prescriptionsWithRepeatCourses.Count;
            var prescriptionsReceivedCount = prescriptionsResponse.PrescriptionRequests.Count();
            var prescriptionsWithNoRepeatableCoursesCount = filteredPrescriptionsCount;

            var kvp = new Dictionary<string, string>
            {
                { "Prescriptions Received",  prescriptionsReceivedCount.ToString(CultureInfo.InvariantCulture)},
                { "Prescriptions with no repeatable courses",  prescriptionsWithNoRepeatableCoursesCount.ToString(CultureInfo.InvariantCulture)},
                { "Returned to user",  prescriptionsReturnedToUserCount.ToString(CultureInfo.InvariantCulture) }
            };

            await _auditor.Audit(auditType, 
                    "Prescriptions Received: {0}, Number of prescriptions without repeatable courses: {1}, Number of prescriptions returned to user: {2}", 
                    prescriptionsReceivedCount.ToString(CultureInfo.InvariantCulture), 
                    prescriptionsWithNoRepeatableCoursesCount.ToString(CultureInfo.InvariantCulture), 
                    prescriptionsReturnedToUserCount.ToString(CultureInfo.InvariantCulture));

            _logger.LogInformationKeyValuePairs("Prescription Count", kvp);

            var prescriptionListResponseFiltered = new PrescriptionRequestsGetResponse
            {
                PrescriptionRequests = prescriptionsWithRepeatCourses,
                MedicationCourses = repeatCourses,
            };

            return prescriptionListResponseFiltered;
        }

        public async Task<OrderPrescriptionResult> OrderPrescription(GpUserSession gpUserSession, RepeatPrescriptionRequest repeatPrescriptionRequest)
        {
            const string auditType = Constants.AuditingTitles.RepeatPrescriptionsOrderRepeatMedicationsResponse;

            var emisUserSession = (EmisUserSession) gpUserSession;

            var postRequest = new PrescriptionRequestsPost
            {
                MedicationCourseGuids = repeatPrescriptionRequest.CourseIds,
                RequestComment = repeatPrescriptionRequest.SpecialRequest,
                UserPatientLinkToken = emisUserSession.UserPatientLinkToken
            };

            var prescriptionsAttemptingToOrderCount = repeatPrescriptionRequest.CourseIds.Count();
            _logger.LogInformation($"Attempting to order {prescriptionsAttemptingToOrderCount} prescriptions");
            await _auditor.Audit(auditType, "Attempting to order {0} prescriptions", prescriptionsAttemptingToOrderCount);


            try
            {
                _logger.LogEnter();
                _logger.LogInformation("Beginning Place Prescription Request");

                var response = await _emisClient.PrescriptionsPost(
                    emisUserSession.SessionId, emisUserSession.EndUserSessionId, postRequest);

                if (response.HasSuccessResponse)
                {
                    _logger.LogDebug($"Prescription order placed successfully. {repeatPrescriptionRequest.CourseIds.Count()} ordered.");
                    return new OrderPrescriptionResult.Success();
                }

                return InterpretOrderPrescriptionError(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Repeat prescription order failed with message {e.Message}");
                return new OrderPrescriptionResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private GetPrescriptionsResult InterpretGetPrescriptionsError(
            EmisClient.EmisApiResponse response)
        {
            if (HasAlreadyBeenOrderedLast30Days(response))
            {
                _logger.LogWarning("The prescription request is invalid as the prescription has already been ordered in the last 30 days");
                _logger.LogEmisWarningResponse(response);
                return new GetPrescriptionsResult.MedicationAlreadyOrderedWithinLast30Days();
            }

            if (response.HasForbiddenResponse())
            {
                _logger.LogError("The emis prescriptions service is not enabled");
                _logger.LogEmisErrorResponse(response);
                return new GetPrescriptionsResult.Forbidden();
            }

            if (IsBadRequest(response))
            {
                _logger.LogError($"The prescription request is invalid with message {JsonConvert.SerializeObject(response.ErrorResponseBadRequest)}");
                _logger.LogEmisErrorResponse(response);
                return new GetPrescriptionsResult.BadRequest();
            }

            _logger.LogError("Emis system is currently unavailable");
            _logger.LogEmisErrorResponse(response);
            return new GetPrescriptionsResult.BadGateway();
        }
        
        private OrderPrescriptionResult InterpretOrderPrescriptionError(
            EmisClient.EmisApiResponse response)
        {
            if (HasAlreadyBeenOrderedLast30Days(response))
            {
                _logger.LogWarning("The prescription request is invalid as the prescription has already been ordered in the last 30 days");
                _logger.LogEmisWarningResponse(response);
                return new OrderPrescriptionResult.MedicationAlreadyOrderedWithinLast30Days();
            }

            if (response.HasForbiddenResponse())
            {
                _logger.LogError("The emis prescriptions service is not enabled");
                _logger.LogEmisErrorResponse(response);
                return new OrderPrescriptionResult.Forbidden();
            }

            if (IsBadRequest(response))
            {
                _logger.LogError($"The prescription request is invalid with message {JsonConvert.SerializeObject(response.ErrorResponseBadRequest)}");
                _logger.LogEmisErrorResponse(response);
                return new OrderPrescriptionResult.BadRequest();
            }

            _logger.LogError("Emis system is currently unavailable");
            _logger.LogEmisErrorResponse(response);
            return new OrderPrescriptionResult.BadGateway();
        }

        private static bool IsBadRequest(EmisClient.EmisApiResponse response)
        {
            return response.StatusCode == HttpStatusCode.BadRequest;
        }

        private static bool HasAlreadyBeenOrderedLast30Days(EmisClient.EmisApiResponse response)
        {
            return (response.StatusCode == HttpStatusCode.Conflict) || response.HasExceptionWithMessageContaining(
                EmisApiErrorMessages.Prescriptions_AlreadyOrderedLast30Days);
        }
    }
}
