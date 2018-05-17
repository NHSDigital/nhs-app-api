using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.Mappers;
using NHSOnline.Backend.Worker.Bridges.Emis.Models.Prescriptions;
using NHSOnline.Backend.Worker.Router.Prescriptions;
using NHSOnline.Backend.Worker.Session;

namespace NHSOnline.Backend.Worker.Bridges.Emis
{
    public class EmisPrescriptionService : IPrescriptionService
    {
        public const int MaxCoursesSoftLimit = 100;

        private readonly IEmisClient _emisClient;
        private readonly IEmisPrescriptionMapper _emisPrescriptionMapper;
        private readonly ILogger _logger;

        public EmisPrescriptionService(ILoggerFactory loggerFactory, IEmisClient emisClient, IEmisPrescriptionMapper emisPrescriptionMapper)
        {
            _emisClient = emisClient;
            _emisPrescriptionMapper = emisPrescriptionMapper;
            _logger = loggerFactory.CreateLogger<EmisPrescriptionService>();
        }

        public async Task<GetPrescriptionsResult> Get(UserSession userSession, DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            var emisUserSession = (EmisUserSession) userSession;

            try
            {
                var prescriptionsResponse = await _emisClient.PrescriptionsGet(emisUserSession.UserPatientLinkToken,
                    emisUserSession.SessionId, emisUserSession.EndUserSessionId, fromDate, toDate);

                if (!prescriptionsResponse.HasSuccessStatusCode)
                {
                    _logger.LogError($"Unsuccessful request retrieving prescriptions for {nameof(fromDate)}={fromDate:O}, {nameof(toDate)}={toDate:O}. Status code: {(int)prescriptionsResponse.StatusCode}");
                    return new GetPrescriptionsResult.Unsuccessful();
                }

                var prescriptionListResponseFiltered = GetPrescriptionsWithoutRepeatCourses(prescriptionsResponse.Body);

                _logger.LogDebug($"Mapping response from {nameof(PrescriptionRequestsGetResponse)} to {nameof(PrescriptionListResponse)}");
                var result = _emisPrescriptionMapper.Map(prescriptionListResponseFiltered);

                return new GetPrescriptionsResult.SuccessfullyRetrieved(result);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving prescriptions");
                return new GetPrescriptionsResult.Unsuccessful();
            }
        }

        private PrescriptionRequestsGetResponse GetPrescriptionsWithoutRepeatCourses(PrescriptionRequestsGetResponse prescriptionsResponse)
        {
            int totalCoursesRunningTotal = 0;
            var repeatCourses = prescriptionsResponse.MedicationCourses.Where(x => x.PrescriptionType == PrescriptionType.Repeat);
            var repeatCourseGuids = repeatCourses.Select(x => x.MedicationCourseGuid);
            
            var prescriptionsWithRepeatCourses = new List<PrescriptionRequest>();
            foreach (var prescription in prescriptionsResponse.PrescriptionRequests.OrderByDescending(x => x.DateRequested))
            {
                if (totalCoursesRunningTotal >= MaxCoursesSoftLimit)
                {
                    break;
                }

                var repeatCoursesInPrescription = prescription.RequestedMedicationCourses.Where(x => repeatCourseGuids.Contains(x.RequestedMedicationCourseGuid));

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
    }
}
