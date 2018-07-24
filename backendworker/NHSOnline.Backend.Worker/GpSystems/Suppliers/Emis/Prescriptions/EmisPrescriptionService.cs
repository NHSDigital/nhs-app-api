using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.Worker.Settings;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Prescriptions
{
    public class EmisPrescriptionService : IPrescriptionService
    {
        private readonly ILogger _logger;
        private readonly ConfigurationSettings _settings;
        private readonly IEmisClient _emisClient;
        private readonly IEmisPrescriptionMapper _emisPrescriptionMapper;

        public EmisPrescriptionService(ILogger<EmisPrescriptionService> logger,
            IOptions<ConfigurationSettings> settings,
            IEmisClient emisClient, IEmisPrescriptionMapper emisPrescriptionMapper)
        {
            _logger = logger;
            _emisClient = emisClient;
            _settings = settings.Value;
            _emisPrescriptionMapper = emisPrescriptionMapper;
        }

        public async Task<PrescriptionResult> GetPrescriptions(UserSession userSession, DateTimeOffset? fromDate,
            DateTimeOffset? toDate)
        {
            var emisUserSession = (EmisUserSession) userSession;
            
            try
            {
                _logger.LogDebug("Beginning Fetch Prescriptions For User");
                
                var prescriptionsResponse = await _emisClient.PrescriptionsGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, emisUserSession.EndUserSessionId, fromDate, toDate);

                _logger.LogDebug("Fetch Prescriptions For User Complete");
                
                if (prescriptionsResponse.HasSuccessStatusCode)
                {
                    try
                    {
                        var prescriptionListResponseFiltered =
                            GetPrescriptionsWithoutRepeatCourses(prescriptionsResponse.Body);

                        _logger.LogDebug($"Mapping successful response from {nameof(PrescriptionRequestsGetResponse)} to {nameof(PrescriptionListResponse)}");
                        var mappedPrescriptionList = _emisPrescriptionMapper.Map(prescriptionListResponseFiltered);

                        if (mappedPrescriptionList.Prescriptions != null)
                        {
                            var allowedStatuses = new List<Status> { Status.Approved, Status.Rejected, Status.Requested };
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
                
               return GetCorrectErrorResult(prescriptionsResponse);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"Unsuccessful request retrieving prescriptions");
                return new PrescriptionResult.SupplierSystemUnavailable();
            }                                     
        }

        private PrescriptionRequestsGetResponse GetPrescriptionsWithoutRepeatCourses(
            PrescriptionRequestsGetResponse prescriptionsResponse)
        {
            int totalCoursesRunningTotal = 0;
            var repeatCourses =
                prescriptionsResponse.MedicationCourses.Where(x => x.PrescriptionType == PrescriptionType.Repeat).ToList();
            var repeatCourseGuids = repeatCourses.Select(x => x.MedicationCourseGuid);

            var prescriptionsWithRepeatCourses = new List<PrescriptionRequest>();
            foreach (var prescription in prescriptionsResponse.PrescriptionRequests.OrderByDescending(x =>
                x.DateRequested))
            {
                if (totalCoursesRunningTotal >= _settings.PrescriptionsMaxCoursesSoftLimit.Value)
                {
                    break;
                }

                var thisPrescription = prescription;
                var requestedMedicationCourses = thisPrescription.RequestedMedicationCourses.ToList();
                
                var repeatCoursesInPrescription =
                    requestedMedicationCourses.Where(x =>
                        repeatCourseGuids.Contains(x.RequestedMedicationCourseGuid)).ToList();

                if (repeatCoursesInPrescription.Any())
                {
                    prescription.RequestedMedicationCourses = repeatCoursesInPrescription;
                    prescriptionsWithRepeatCourses.Add(prescription);
                }

                totalCoursesRunningTotal += repeatCoursesInPrescription.Count();
            }

            var prescriptionListResponseFiltered = new PrescriptionRequestsGetResponse
            {
                PrescriptionRequests = prescriptionsWithRepeatCourses,
                MedicationCourses = repeatCourses,
            };

            return prescriptionListResponseFiltered;
        }

        public async Task<PrescriptionResult> OrderPrescription(UserSession userSession, RepeatPrescriptionRequest request)
        {
            var emisUserSession = (EmisUserSession) userSession;

            var postRequest = new PrescriptionRequestsPost()
            {
                MedicationCourseGuids = request.CourseIds,
                RequestComment = request.SpecialRequest,
                UserPatientLinkToken = emisUserSession.UserPatientLinkToken
            };

            try
            {
                _logger.LogInformation("Beginning Place Prescription Request");
                
                var response = await _emisClient.PrescriptionsPost(
                    emisUserSession.SessionId, emisUserSession.EndUserSessionId, postRequest);

                if (response.HasSuccessStatusCode)
                {
                    _logger.LogDebug($"Prescription order placed successfully");
                    return new PrescriptionResult.SuccessfulPost();
                }

                return GetCorrectErrorResult(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Repeat prescription order failed with message {e.Message}");
                return new PrescriptionResult.SupplierSystemUnavailable();
            }
        }

        private PrescriptionResult GetCorrectErrorResult(
            EmisClient.EmisApiResponse response)
        {
            if (HasAlreadyBeenOrderedLast30Days(response))
            {
                _logger.LogError("The prescription request is invalid as the prescription has already been ordered in the last 30 days");

                return new PrescriptionResult.CannotReorderPrescription();
            }

            if (response.HasForbiddenResponse())
            {
                _logger.LogError("The emis prescriptions service is not enabled");
                
                return new PrescriptionResult.SupplierNotEnabled();
            }

            if (IsBadRequest(response))
            {
                _logger.LogError($"The prescription request is invalid with message {JsonConvert.SerializeObject(response.ErrorResponseBadRequest)}");

                return new PrescriptionResult.BadRequest();
            }
            
            _logger.LogError("Emis system is currently unavailable");

            return new PrescriptionResult.SupplierSystemUnavailable();
        }

        private static bool IsBadRequest(EmisClient.EmisApiResponse response)
        {
            return response.StatusCode == HttpStatusCode.BadRequest;
        }

        private static bool HasAlreadyBeenOrderedLast30Days(EmisClient.EmisApiResponse response)
        {
            return response.HasExceptionWithMessageContaining(
                EmisApiErrorMessages.Prescriptions_AlreadyOrderedLast30Days);
        }
    }
}
