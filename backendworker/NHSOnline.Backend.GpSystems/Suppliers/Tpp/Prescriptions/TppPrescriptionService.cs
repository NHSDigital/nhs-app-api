using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Prescriptions
{
    internal class TppPrescriptionService : IPrescriptionService
    {
        private readonly ILogger<TppPrescriptionService> _logger;
        private readonly TppConfigurationSettings _settings;
        private readonly ITppClientRequest<(TppRequestParameters, RepeatPrescriptionRequest), RequestMedicationReply> _orderPrescriptions;
        private readonly ITppClientRequest<TppRequestParameters, ListRepeatMedicationReply> _listRepeatMedication;
        private readonly ITppClientRequest<TppRequestParameters, RequestSystmOnlineMessagesReply> _requestSystmOnlineMessages;
        private readonly ITppPrescriptionMapper _tppPrescriptionMapper;

        public TppPrescriptionService(
            ITppClientRequest<(TppRequestParameters, RepeatPrescriptionRequest), RequestMedicationReply> orderPrescriptions,
            ITppClientRequest<TppRequestParameters, ListRepeatMedicationReply> listRepeatMedication,
            ITppClientRequest<TppRequestParameters, RequestSystmOnlineMessagesReply> requestSystmOnlineMessages,
            ILogger<TppPrescriptionService> logger,
            TppConfigurationSettings settings,
            ITppPrescriptionMapper tppPrescriptionMapper)
        {
            _orderPrescriptions = orderPrescriptions;
            _listRepeatMedication = listRepeatMedication;
            _requestSystmOnlineMessages = requestSystmOnlineMessages;
            _settings = settings;
            _logger = logger;
            _tppPrescriptionMapper = tppPrescriptionMapper;

            _settings.Validate();
        }

        public async Task<GetPrescriptionsResult> GetPrescriptions(
            GpLinkedAccountModel gpLinkedAccountModel,
            DateTimeOffset? fromDate = null,
            DateTimeOffset? toDate = null)
        {
            var tppRequestParameters = gpLinkedAccountModel.BuildTppRequestParameters(_logger);

            try
            {
                _logger.LogEnter();
                _logger.LogDebug("Beginning Fetch Prescriptions For User");
                var response = await _listRepeatMedication.Post(tppRequestParameters);
                _logger.LogDebug("Fetch Prescriptions For User Complete");

                if (!response.HasSuccessResponse)
                {
                    return InterpretGetPrescriptionsError(response);
                }
                try
                {
                    var medicationsToBeFiltered = response.Body.Medications.ToList();
                    var medicationListFiltered = GetMaxPrescriptions(medicationsToBeFiltered);
                    var numberOfPrescriptionsDiscarded = medicationsToBeFiltered.Count - medicationListFiltered.Count;

                    var prescriptionsCount = new FilteringCounts
                    {
                        ReceivedCount = medicationsToBeFiltered.Count,
                        FilteredRemainingRepeatsCount = medicationsToBeFiltered.Count,
                        FilteredMaxAllowanceDiscardedCount = numberOfPrescriptionsDiscarded,
                        ReturnedCount = medicationListFiltered.Count
                    };

                    _logger.LogDebug(
                        $"Mapping successful response from {nameof(ListRepeatMedicationReply)} to {nameof(PrescriptionListResponse)}");
                    var mappedPrescriptionList = _tppPrescriptionMapper.Map(medicationListFiltered);

                    await LogPrescriptionMessaging(gpLinkedAccountModel);
                    return new GetPrescriptionsResult.Success(mappedPrescriptionList, prescriptionsCount);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Something went wrong building the Prescription History response");
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
            
            TppRequestParameters tppRequestParameters = gpLinkedAccountModel.BuildTppRequestParameters(_logger);
            
            try
            {
                _logger.LogInformation("Beginning Place Prescription Request");
                var response = await _orderPrescriptions.Post((tppRequestParameters, repeatPrescriptionRequest));

                if (!response.HasSuccessResponse)
                {
                    return InterpretOrderPrescriptionError(response);
                }

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

        private async Task LogPrescriptionMessaging(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var tppRequestParameters = gpLinkedAccountModel.BuildTppRequestParameters(_logger);

            try
            {
                var messages = await _requestSystmOnlineMessages.Post(tppRequestParameters);

                const string intro = "Prescription Messaging from practice:";
                var confirmation = messages?.Body?.RequestMedicationConfirmation?.Trim() ?? string.Empty;
                var medication = messages?.Body?.Medication?.Trim() ?? string.Empty;
                _logger.LogInformation($"{intro} RequestMedicationConfirmation={confirmation}, Medication={medication}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception has been thrown calling TPP.");
            }
        }

        private List<Medication> GetMaxPrescriptions(List<Medication> medications)
        {
            if (_settings.PrescriptionsMaxCoursesSoftLimit.HasValue)
            {
                medications = medications.Take(_settings.PrescriptionsMaxCoursesSoftLimit.Value).ToList();
            }

            return medications;
        }

        private GetPrescriptionsResult InterpretGetPrescriptionsError(
            TppApiResponse response)
        {

            if (response.HasForbiddenResponse)
            {
                _logger.LogError("The tpp prescriptions service is not enabled");

                return new GetPrescriptionsResult.Forbidden();
            }

            if (InvalidCourseId(response) || RequestNoteTooLarge(response) || MustViewMedications(response))
            {
                _logger.LogError($"The tpp prescription request is invalid with message {JsonConvert.SerializeObject(response.ErrorResponse.TechnicalMessage)}");

                return new GetPrescriptionsResult.BadRequest();
            }

            _logger.LogError("Tpp system is currently unavailable");

            return new GetPrescriptionsResult.BadGateway();
        }

        private OrderPrescriptionResult InterpretOrderPrescriptionError(
            TppApiResponse response)
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

            if (InvalidCourseId(response) || RequestNoteTooLarge(response) || MustViewMedications(response))
            {
                _logger.LogError($"The tpp prescription request is invalid with message {JsonConvert.SerializeObject(response.ErrorResponse.TechnicalMessage)}");

                return new OrderPrescriptionResult.BadRequest();
            }

            _logger.LogError("Tpp system is currently unavailable");

            return new OrderPrescriptionResult.BadGateway();
        }

        private static bool PrescriptionHasAlreadyBeenOrderedOrIsUnavailable(TppApiResponse response)
        {
            return response.HasErrorMessageContaining(
                TppApiErrorMessages.Prescriptions_CourseAlreadyOrdered_IsUnavailable);
        }

        private static bool InvalidCourseId(TppApiResponse response)
        {
            return response.HasErrorMessageContaining(
                TppApiErrorMessages.Prescriptions_InvalidCourseIds);
        }

        private static bool RequestNoteTooLarge(TppApiResponse response)
        {
            return response.HasErrorMessageContaining(
                TppApiErrorMessages.Prescriptions_RequestNoteTooLarge);
        }

        private static bool MustViewMedications(TppApiResponse response)
        {
            return response.HasErrorMessageContaining(
                TppApiErrorMessages.Prescriptions_MustViewMedicationsListFirst);
        }
    }
}