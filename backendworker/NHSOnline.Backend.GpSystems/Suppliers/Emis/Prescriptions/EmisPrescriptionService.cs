using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Prescriptions
{
    public class EmisPrescriptionService : IPrescriptionService
    {
        private readonly ILogger<EmisPrescriptionService> _logger;
        private readonly EmisConfigurationSettings _settings;
        private readonly IEmisClient _emisClient;
        private readonly IEmisPrescriptionMapper _emisPrescriptionMapper;

        public EmisPrescriptionService(
            ILogger<EmisPrescriptionService> logger,
            EmisConfigurationSettings settings,
            IEmisClient emisClient, 
            IEmisPrescriptionMapper emisPrescriptionMapper)
        {
            _logger = logger;
            _emisClient = emisClient;
            _settings = settings;
            _emisPrescriptionMapper = emisPrescriptionMapper;

            _settings.Validate();
        }

        public async Task<GetPrescriptionsResult> GetPrescriptions(GpLinkedAccountModel gpLinkedAccountModel, DateTimeOffset? fromDate,
            DateTimeOffset? toDate)
        {
            try
            {
                _logger.LogEnter();
                _logger.LogDebug("Beginning Fetch Prescriptions For User");

                EmisRequestParameters emisRequestParameters = gpLinkedAccountModel.BuildEmisRequestParameters(_logger);

                var prescriptionsResponse = await _emisClient.PrescriptionsGet(emisRequestParameters, fromDate, toDate);

                _logger.LogDebug("Fetch Prescriptions For User Complete");

                if (prescriptionsResponse.HasSuccessResponse)
                {
                    try
                    {
                        var (prescriptionListResponseFiltered, prescriptionsCount) = 
                            GetPrescriptionsWithoutRepeatCourses(prescriptionsResponse.Body);


                        _logger.LogDebug($"Mapping successful response from {nameof(PrescriptionRequestsGetResponse)} to {nameof(PrescriptionListResponse)}");
                        var mappedPrescriptionList = _emisPrescriptionMapper.Map(prescriptionListResponseFiltered);

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

        private (PrescriptionRequestsGetResponse prescriptionListResponseFiltered, FilteringCounts prescriptionsCount) 
            GetPrescriptionsWithoutRepeatCourses(PrescriptionRequestsGetResponse prescriptionsResponse)
        {
            var prescriptionsReceivedCount = prescriptionsResponse.PrescriptionRequests.Count();
            var totalCoursesRunningTotal = 0;
            var numberOfPrescriptionsDiscarded = 0;

            var repeatCourses = prescriptionsResponse.MedicationCourses
                                                     .Where(x => x.PrescriptionType == PrescriptionType.Repeat)
                                                     .ToList();

            var repeatCourseIdLookup = repeatCourses.Select(x => x.MedicationCourseGuid)
                                                    .Distinct()
                                                    .ToDictionary(x => x);

            var filteredPrescriptionsCount = 0;

            var prescriptionsWithRepeatCourses = new List<PrescriptionRequest>();

            foreach (var prescription in prescriptionsResponse.PrescriptionRequests.OrderByDescending(x =>
                x.DateRequested))
            {
                if (totalCoursesRunningTotal >= _settings.PrescriptionsMaxCoursesSoftLimit)
                {
                    numberOfPrescriptionsDiscarded = prescriptionsReceivedCount - totalCoursesRunningTotal;
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

                totalCoursesRunningTotal += repeatCoursesInPrescription.Count;
            }
            
            var prescriptionsWithRepeatableCourses = prescriptionsReceivedCount - filteredPrescriptionsCount;
            var prescriptionsReturnedToUserCount = prescriptionsWithRepeatCourses.Count;

            var prescriptionsCount = new FilteringCounts
            {
                ReceivedCount = prescriptionsReceivedCount,
                FilteredRemainingRepeatsCount = prescriptionsWithRepeatableCourses,
                FilteredMaxAllowanceDiscardedCount = numberOfPrescriptionsDiscarded,
                ReturnedCount = prescriptionsReturnedToUserCount
            };

            var prescriptionListResponseFiltered = new PrescriptionRequestsGetResponse
            {
                PrescriptionRequests = prescriptionsWithRepeatCourses,
                MedicationCourses = repeatCourses,
            };

            return (prescriptionListResponseFiltered, prescriptionsCount);
        }

        public async Task<OrderPrescriptionResult> OrderPrescription(GpLinkedAccountModel gpLinkedAccountModel, RepeatPrescriptionRequest repeatPrescriptionRequest)
        {
            EmisRequestParameters emisRequestParameters = gpLinkedAccountModel.BuildEmisRequestParameters(_logger);

            var postRequest = new PrescriptionRequestsPost
            {
                MedicationCourseGuids = repeatPrescriptionRequest.CourseIds,
                RequestComment = repeatPrescriptionRequest.SpecialRequest,
                UserPatientLinkToken = emisRequestParameters.UserPatientLinkToken
            };

            var prescriptionsAttemptingToOrderCount = repeatPrescriptionRequest.CourseIds.Count();
            _logger.LogInformation($"Attempting to order {prescriptionsAttemptingToOrderCount} prescriptions");

            try
            {
                _logger.LogEnter();
                _logger.LogInformation("Beginning Place Prescription Request");

                var response = await _emisClient.PrescriptionsPost(
                    emisRequestParameters.SessionId, emisRequestParameters.EndUserSessionId, postRequest);

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
