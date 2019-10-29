using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Prescriptions
{
    public class TppPrescriptionService : IPrescriptionService
    {
        private readonly ILogger<TppPrescriptionService> _logger;
        private readonly TppConfigurationSettings _settings;
        private readonly ITppClient _tppClient;
        private readonly ITppPrescriptionMapper _tppPrescriptionMapper;

        public TppPrescriptionService(
            ILogger<TppPrescriptionService> logger,
            TppConfigurationSettings settings,
            ITppClient tppClient, ITppPrescriptionMapper tppPrescriptionMapper)
        {
            _tppClient = tppClient;
            _settings = settings;
            _logger = logger;
            _tppPrescriptionMapper = tppPrescriptionMapper;

            _settings.Validate();
        }

        public async Task<GetPrescriptionsResult> GetPrescriptions(GpLinkedAccountModel gpLinkedAccountModel, DateTimeOffset? fromDate = null,
            DateTimeOffset? toDate = null)
        {
            var tppUserSession = (TppUserSession) gpLinkedAccountModel.GpUserSession;

            try
            {
                _logger.LogEnter();
                _logger.LogDebug("Beginning Fetch Prescriptions For User");
                var response = await _tppClient.ListRepeatMedicationPost(tppUserSession);
                _logger.LogDebug("Fetch Prescriptions For User Complete");

                if (!response.HasSuccessResponse)
                {
                    return InterpretGetPrescriptionsError(response);
                }
                try
                {
                    var medicationListFiltered = GetMaxPrescriptions(response.Body.Medications.ToList());

                    _logger.LogDebug(
                        $"Mapping response from {nameof(ListRepeatMedicationReply)} to {nameof(PrescriptionListResponse)}");
                    var mapppedPrescriptionList = _tppPrescriptionMapper.Map(medicationListFiltered);
                    
                    return new GetPrescriptionsResult.Success(mapppedPrescriptionList);
                }
                catch (Exception e)
                {
                    _logger.LogError(
                        $"Something went wrong during building the response. Exception message: {e.Message}");
                    return new GetPrescriptionsResult.InternalServerError();
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request retrieving prescriptions");
                return new GetPrescriptionsResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }

        }

        public async Task<OrderPrescriptionResult> OrderPrescription(GpLinkedAccountModel gpLinkedAccountModel, RepeatPrescriptionRequest repeatPrescriptionRequest)
        {
            _logger.LogEnter();

            var tppUserSession = (TppUserSession) gpLinkedAccountModel.GpUserSession;

            var postRequest = new RequestMedication
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                UnitId = tppUserSession.OdsCode,
                Notes = repeatPrescriptionRequest.SpecialRequest,
                Medications = repeatPrescriptionRequest.CourseIds.Select(x => new MedicationRequest
                {
                    DrugId = x,
                    Type = TppApiConstants.MedicationType.Repeat,
                }).ToList(),
            };

            try
            {
                _logger.LogInformation("Beginning Place Prescription Request");
                var response = await _tppClient.OrderPrescriptionsPost(tppUserSession, postRequest);

                if (!response.HasSuccessResponse) return InterpretOrderPrescriptionError(response);
                
                _logger.LogDebug($"Prescription order placed successfully");
                return new OrderPrescriptionResult.Success();

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

        private List<Medication> GetMaxPrescriptions(List<Medication> medications)
        {
            if(_settings.PrescriptionsMaxCoursesSoftLimit.HasValue) 
            {
                medications = medications.Take(_settings.PrescriptionsMaxCoursesSoftLimit.Value).ToList();
            }
            

            return medications;
        }
        
        private GetPrescriptionsResult InterpretGetPrescriptionsError(
            TppClient.TppApiResponse response)
        {

            if (response.HasForbiddenResponse)
            {
                _logger.LogError("The tpp prescriptions service is not enabled");
                
                return new GetPrescriptionsResult.Forbidden();
            }

            if (PrescriptionHasAlreadyBeenOrderedOrIsUnavailable(response))
            {
                _logger.LogError("The tpp prescription has already been ordered or is not available");
                
                return new GetPrescriptionsResult.CannotReorderPrescription();
            }
            
            if (InvalidCourseId(response) || RequestNoteTooLarge(response) || MustViewMedications(response) )
            {
                _logger.LogError($"The tpp prescription request is invalid with message {JsonConvert.SerializeObject(response.ErrorResponse.TechnicalMessage)}");

                return new GetPrescriptionsResult.BadRequest();
            }     
            
            _logger.LogError("Tpp system is currently unavailable");

            return new GetPrescriptionsResult.BadGateway();
        }
        
        private OrderPrescriptionResult InterpretOrderPrescriptionError(
            TppClient.TppApiResponse response)
        {

            if (response.HasForbiddenResponse)
            {
                _logger.LogError("The tpp prescriptions service is not enabled");
                
                return new OrderPrescriptionResult.Forbidden();
            }

            if (PrescriptionHasAlreadyBeenOrderedOrIsUnavailable(response))
            {
                _logger.LogError("The tpp prescription has already been ordered or is not available");
                
                return new OrderPrescriptionResult.CannotReorderPrescription();
            }
            
            if (InvalidCourseId(response) || RequestNoteTooLarge(response) || MustViewMedications(response) )
            {
                _logger.LogError($"The tpp prescription request is invalid with message {JsonConvert.SerializeObject(response.ErrorResponse.TechnicalMessage)}");

                return new OrderPrescriptionResult.BadRequest();
            }     
            
            _logger.LogError("Tpp system is currently unavailable");

            return new OrderPrescriptionResult.BadGateway();
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