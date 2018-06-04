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
using NHSOnline.Backend.Worker.Bridges.Emis.Mappers;
using NHSOnline.Backend.Worker.Bridges.Emis.Models.Prescriptions;
using NHSOnline.Backend.Worker.Router.Prescriptions;

namespace NHSOnline.Backend.Worker.Bridges.Emis
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

        public async Task<PrescriptionResult> Get(UserSession userSession, DateTimeOffset? fromDate,
            DateTimeOffset? toDate)
        {
            var emisUserSession = (EmisUserSession) userSession;

            try
            {
                var prescriptionsResponse = await _emisClient.PrescriptionsGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, emisUserSession.EndUserSessionId, fromDate, toDate);

                if (!prescriptionsResponse.HasSuccessStatusCode)
                {
                    _logger.LogError(
                        $"Unsuccessful request retrieving prescriptions for {nameof(fromDate)}={fromDate:O}, {nameof(toDate)}={toDate:O}. Status code: {(int) prescriptionsResponse.StatusCode}");
                    return new PrescriptionResult.SupplierSystemUnavailable();
                }

                var prescriptionListResponseFiltered = GetPrescriptionsWithoutRepeatCourses(prescriptionsResponse.Body);

                _logger.LogDebug(
                    $"Mapping response from {nameof(PrescriptionRequestsGetResponse)} to {nameof(PrescriptionListResponse)}");
                var result = _emisPrescriptionMapper.Map(prescriptionListResponseFiltered);

                return new PrescriptionResult.SuccessfullGet(result);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving prescriptions");
                return new PrescriptionResult.SupplierSystemUnavailable();
            }
            catch (NullReferenceException e)
            {
                _logger.LogError(e, "Prescription retrieval return null body");
                return new PrescriptionResult.UnexpectedError();
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

        public async Task<PrescriptionResult> Post(UserSession userSession, RepeatPrescriptionRequest request)
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
                    return new PrescriptionResult.SuccessfullPost();
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
                return new PrescriptionResult.CannotReorderPrescription();
            }

            if (HasInsufficientPermissions(response))
            {
                return new PrescriptionResult.InsufficientPermissions();
            }

            if (IsBadRequest(response))
            {
                _logger.LogError(
                    $"The prescription request is invalid with message {JsonConvert.SerializeObject(response.ErrorResponseBadRequest)}");

                return new PrescriptionResult.BadRequest();
            }

            return new PrescriptionResult.SupplierSystemUnavailable();
        }

        private static bool IsBadRequest(EmisClient.EmisApiResponse response)
        {
            return response.StatusCode == HttpStatusCode.BadRequest;
        }

        private static bool HasInsufficientPermissions(EmisClient.EmisApiResponse response)
        {
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return true;
            }

            return response.HasExceptionWithMessageContaining(
                EmisApiErrorMessages.EmisService_NotEnabledForUser);
        }

        private static bool HasAlreadyBeenOrderedLast30Days(EmisClient.EmisApiResponse response)
        {
            return response.HasExceptionWithMessageContaining(
                EmisApiErrorMessages.Prescriptions_AlreadyOrderedLast30Days);
        }
    }
}