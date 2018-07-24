using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.Settings;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions
{
    public class TppPrescriptionService : IPrescriptionService
    {
        private readonly ILogger _logger;
        private readonly ConfigurationSettings _settings;
        private readonly ITppClient _tppClient;
        private readonly ITppPrescriptionMapper _tppPrescriptionMapper;

        public TppPrescriptionService(
            ILogger<TppPrescriptionService> logger,
            IOptions<ConfigurationSettings> settings,
            ITppClient tppClient, ITppPrescriptionMapper tppPrescriptionMapper)
        {
            _tppClient = tppClient;
            _settings = settings.Value;
            _logger = logger;
            _tppPrescriptionMapper = tppPrescriptionMapper;
        }

        public async Task<PrescriptionResult> GetPrescriptions(UserSession userSession, DateTimeOffset? fromDate = null,
            DateTimeOffset? toDate = null)
        {
            var tppUserSession = (TppUserSession) userSession;

            try
            {
                _logger.LogDebug("Beginning Fetch Prescriptions For User");
                var response = await _tppClient.ListRepeatMedicationPost(tppUserSession);
                _logger.LogDebug("Fetch Prescriptions For User Complete");

                if (!response.HasSuccessResponse) return GetCorrectErrorResult(response);
                
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
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving prescriptions");
                return new PrescriptionResult.SupplierSystemUnavailable();
            }
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
                _logger.LogInformation("Beginning Place Prescription Request");
                var response = await _tppClient.OrderPrescriptionsPost(tppUserSession, postRequest);

                if (!response.HasSuccessResponse) return GetCorrectErrorResult(response);
                
                _logger.LogDebug($"Prescription order placed successfully");
                return new PrescriptionResult.SuccessfulPost();

            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Repeat prescription order failed with message {e.Message}");
                return new PrescriptionResult.SupplierSystemUnavailable();
            }
        }

        private List<Medication> GetMaxPrescriptions(List<Medication> medications)
        {
            if (_settings.PrescriptionsMaxCoursesSoftLimit != null)
            {
                medications = medications.Take(_settings.PrescriptionsMaxCoursesSoftLimit.Value).ToList();
            }

            return medications;
        }
        
        private PrescriptionResult GetCorrectErrorResult(
            TppClient.TppApiResponse response)
        {

            if (response.HasForbiddenResponse)
            {
                _logger.LogError("The tpp prescriptions service is not enabled");
                
                return new PrescriptionResult.SupplierNotEnabled();
            }

            if (PrescriptionHasAlreadyBeenOrderedOrIsUnavailable(response))
            {
                _logger.LogError("The tpp prescription has already been ordered or is not available");
                
                return new PrescriptionResult.CannotReorderPrescription();
            }
            
            if (InvalidCourseId(response) || RequestNoteTooLarge(response) || MustViewMedications(response) )
            {
                _logger.LogError($"The tpp prescription request is invalid with message {JsonConvert.SerializeObject(response.ErrorResponse.TechnicalMessage)}");

                return new PrescriptionResult.BadRequest();
            }     
            
            _logger.LogError("Tpp system is currently unavailable");

            return new PrescriptionResult.SupplierSystemUnavailable();
        }
        
        private static bool PrescriptionHasAlreadyBeenOrderedOrIsUnavailable(TppClient.TppApiResponse response)
        {
            return response.HasErrorMessageContaining(
                TppApiErrorMessages.Prescriptions_CourseAlreadyOrdered_IsUnavailable);
        }
        
        private static bool InvalidCourseId(TppClient.TppApiResponse response)
        {
            return response.HasErrorMessageContaining(
                TppApiErrorMessages.Prescriptions_InvalidCourseIds);
        }
        
        private static bool RequestNoteTooLarge(TppClient.TppApiResponse response)
        {
            return response.HasErrorMessageContaining(
                TppApiErrorMessages.Prescriptions_RequestNoteTooLarge);
        }
        
        private static bool MustViewMedications(TppClient.TppApiResponse response)
        {
            return response.HasErrorMessageContaining(
                TppApiErrorMessages.Prescriptions_MustViewMedicationsListFirst);
        }
    }
}