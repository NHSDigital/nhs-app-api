using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Demographics;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public class MicrotestAppointmentSlotsService : IAppointmentSlotsService
    {
        private readonly IMicrotestClient _microtestClient;
        private readonly ILogger<MicrotestAppointmentSlotsService> _logger;
        private readonly IAppointmentSlotsResponseMapper _appointmentSlotsResponseMapper;

        public MicrotestAppointmentSlotsService(
            IMicrotestClient microtestClient,
            ILogger<MicrotestAppointmentSlotsService> logger,
            IAppointmentSlotsResponseMapper appointmentSlotsResponseMapper)
        {
            _microtestClient = microtestClient;
            _logger = logger;
            _appointmentSlotsResponseMapper = appointmentSlotsResponseMapper;
        }

        public async Task<AppointmentSlotsResult> GetSlots(
            GpLinkedAccountModel gpLinkedAccountModel, AppointmentSlotsDateRange dateRange)
        {
            try
            {
                _logger.LogEnter();

                var microtestUserSession = (MicrotestUserSession) gpLinkedAccountModel.GpUserSession;

                _logger.LogInformation("Demographic data request starting");

                MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse> demographicsResponse = null;
                try
                {
                    demographicsResponse = await _microtestClient.DemographicsGet(microtestUserSession.OdsCode,
                        microtestUserSession.NhsNumber);
                    _logger.LogInformation("Demographic data request complete");

                }
                catch (Exception e)
                {
                    _logger.LogError(e,"Exception has been thrown retrieving Microtest " +
                                        "demographics data. Continuing...");
                }

                _logger.LogInformation("Appointment slots request starting");

                var appointmentSlotsResponse = await _microtestClient.AppointmentSlotsGet(microtestUserSession.OdsCode,
                    microtestUserSession.NhsNumber,
                    dateRange);

                _logger.LogInformation("Appointment slots request complete");

                return InterpretAppointmentsGetResponse(appointmentSlotsResponse, demographicsResponse);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "HttpRequestException has been thrown.");
                return new AppointmentSlotsResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        
        private AppointmentSlotsResult InterpretAppointmentsGetResponse(
            MicrotestClient.MicrotestApiObjectResponse<AppointmentSlotsGetResponse> appointmentSlotsResponse,
            MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse> demographicsResponse)
        {
            if (appointmentSlotsResponse.HasForbiddenResponse)
            {
                _logger.LogError("Call to Microtest returned a forbidden response");
                return new AppointmentSlotsResult.Forbidden();
            }

            if (!appointmentSlotsResponse.HasSuccessResponse)
            {
                _logger.LogError($"Call to Microtest ({nameof(MicrotestAppointmentSlotsService)}) returned an unanticipated " +
                                 $"error with status code: '{appointmentSlotsResponse.StatusCode}'.");
                return new AppointmentSlotsResult.BadGateway();
            }

            try
            {
                var result = _appointmentSlotsResponseMapper.Map(appointmentSlotsResponse.Body, 
                    demographicsResponse?.Body
                );
                return new AppointmentSlotsResult.Success(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error mapping appointment slots");
                return new AppointmentSlotsResult.InternalServerError();
            }
        }
    }
}