using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions
{
    public class TppPrescriptionService : IPrescriptionService
    {
        private readonly ILogger _logger;
        private readonly ConfigurationSettings _settings;
        private readonly ITppClient _tppClient;
        private readonly ITppPrescriptionMapper _tppPrescriptionMapper;

        public TppPrescriptionService(ILoggerFactory loggerFactory,
            IOptions<ConfigurationSettings> settings,
            ITppClient tppClient, ITppPrescriptionMapper tppPrescriptionMapper)
        {
            _tppClient = tppClient;
            _settings = settings.Value;
            _logger = loggerFactory.CreateLogger<TppPrescriptionService>();
            _tppPrescriptionMapper = tppPrescriptionMapper;
        }

        public async Task<PrescriptionResult> GetPrescriptions(UserSession userSession, DateTimeOffset? fromDate = null,
            DateTimeOffset? toDate = null)
        {
            var tppUserSession = (TppUserSession) userSession;

            try
            {
                var response = await _tppClient.ListRepeatMedicationPost(tppUserSession);

                if (response.HasSuccessResponse)
                {
                    try
                    {
                        var medicationListFiltered = GetMaxPrescriptions(response.Body.Medications.ToList());

                        _logger.LogDebug(
                            $"Mapping response from {nameof(ListRepeatMedicationReply)} to {nameof(PrescriptionListResponse)}");
                        var mapppedPrescriptionList = _tppPrescriptionMapper.Map(medicationListFiltered);

                        return new PrescriptionResult.SuccessfulGet(mapppedPrescriptionList);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(
                            $"Something went wrong during building the response. Exception message: {e.Message}");

                        return new PrescriptionResult.InternalServerError();
                    }
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving prescriptions");
                return new PrescriptionResult.SupplierSystemUnavailable();
            }

            return new PrescriptionResult.SupplierSystemUnavailable();
        }

        public async Task<PrescriptionResult> OrderPrescription(UserSession userSession, RepeatPrescriptionRequest request)
        {
            var tppUserSession = (TppUserSession)userSession;

            var postRequest = new RequestMedication
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                UnitId = tppUserSession.UnitId,
                Notes = request.SpecialRequest,
                Medications = request.CourseIds.Select(x => new MedicationRequest
                {
                    DrugId = x,
                    Type = TppApiConstants.MedicationType.Repeat,
                }).ToList(),
            };

            try
            {
                var response = await _tppClient.OrderPrescriptionsPost(tppUserSession, postRequest);

                if (response.HasSuccessResponse)
                {
                    return new PrescriptionResult.SuccessfulPost();
                }

                return new PrescriptionResult.SupplierSystemUnavailable();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Repeat prescription order failed with message {e.Message}");
                return new PrescriptionResult.SupplierSystemUnavailable();
            }
        }

        private List<Medication> GetMaxPrescriptions(List<Medication> medications)
        {
            return medications.Take(_settings.PrescriptionsMaxCoursesSoftLimit.Value).ToList();
        }
    }
}