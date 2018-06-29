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

        public EmisPrescriptionService(ILoggerFactory loggerFactory,
            IOptions<ConfigurationSettings> settings,
            IEmisClient emisClient, IEmisPrescriptionMapper emisPrescriptionMapper)
        {
            _emisClient = emisClient;
            _settings = settings.Value;
            _emisPrescriptionMapper = emisPrescriptionMapper;
            _logger = loggerFactory.CreateLogger<EmisPrescriptionService>();
        }

        public async Task<PrescriptionResult> GetPrescriptions(UserSession userSession, DateTimeOffset? fromDate,
            DateTimeOffset? toDate)
        {
            var emisUserSession = (EmisUserSession) userSession;

            try
            {
                var prescriptionsResponse = await _emisClient.PrescriptionsGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, emisUserSession.EndUserSessionId, fromDate, toDate);

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
                        _logger.LogError(e, $"Something went wrong during building the response.");

                        return new PrescriptionResult.InternalServerError();
                    }
                }    
                
               return GetCorrectErrorResult(prescriptionsResponse);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving prescriptions");
                return new PrescriptionResult.SupplierSystemUnavailable();
            }                                     
        }

        private PrescriptionRequestsGetResponse GetPrescriptionsWithoutRepeatCourses(
            PrescriptionRequestsGetResponse prescriptionsResponse)
        {
            int totalCoursesRunningTotal = 0;
            var repeatCourses =
                prescriptionsResponse.MedicationCourses.Where(x => x.PrescriptionType == PrescriptionType.Repeat);
            var repeatCourseGuids = repeatCourses.Select(x => x.MedicationCourseGuid);

            var prescriptionsWithRepeatCourses = new List<PrescriptionRequest>();
            foreach (var prescription in prescriptionsResponse.PrescriptionRequests.OrderByDescending(x =>
                x.DateRequested))
            {
                if (totalCoursesRunningTotal >= _settings.PrescriptionsMaxCoursesSoftLimit.Value)
                {
                    break;
                }

                var repeatCoursesInPrescription =
                    prescription.RequestedMedicationCourses.Where(x =>
                        repeatCourseGuids.Contains(x.RequestedMedicationCourseGuid));

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
                var response = await _emisClient.PrescriptionsPost(
                    emisUserSession.SessionId, emisUserSession.EndUserSessionId, postRequest);

                if (response.HasSuccessStatusCode)
                {
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