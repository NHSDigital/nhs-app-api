using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Appointments
{
    public class VisionAppointmentSlotsService : IAppointmentSlotsService
    {
        private readonly IVisionClient _visionClient;
        private readonly ILogger<VisionAppointmentSlotsService> _logger;
        private readonly IAvailableAppointmentsResponseMapper _mapper;
        private readonly VisionConfigurationSettings _settings;

        public VisionAppointmentSlotsService(
            IVisionClient visionClient,
            ILogger<VisionAppointmentSlotsService> logger,
            IAvailableAppointmentsResponseMapper mapper,
            VisionConfigurationSettings settings)
        {
            _visionClient = visionClient;
            _logger = logger;
            _mapper = mapper;
            _settings = settings;

            _settings.Validate();
        }

        public async Task<AppointmentSlotsResult> GetSlots(
            GpLinkedAccountModel gpLinkedAccountModel, AppointmentSlotsDateRange dateRange)
        {
            try
            {
                _logger.LogEnter();

                var visionUserSession = (VisionUserSession)gpLinkedAccountModel.GpUserSession;

                if (!visionUserSession.IsAppointmentsEnabled)
                {
                    _logger.LogError("Appointments not enabled");
                    return new AppointmentSlotsResult.Forbidden();
                }

                var slotsTask = _visionClient.GetAvailableAppointments(
                    visionUserSession,
                    dateRange
                );

                var configTask = _visionClient.GetConfiguration(visionUserSession);

                VisionPFSClient.VisionApiObjectResponse<PatientConfigurationResponse> configResponse = null;

                try
                {
                    configResponse = await configTask;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception has been thrown retrieving Vision booking guidance.");
                }

                var slotsResponse = await slotsTask;

                return InterpretAppointmentsGetResponse(slotsResponse, configResponse, visionUserSession);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"Calling {nameof(_visionClient.GetAvailableAppointments)} threw HttpRequestException.");
                return new AppointmentSlotsResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private AppointmentSlotsResult InterpretAppointmentsGetResponse(
            VisionPFSClient.VisionApiObjectResponse<AvailableAppointmentsResponse> slotsResponse,
            VisionPFSClient.VisionApiObjectResponse<PatientConfigurationResponse> configResponse,
            VisionUserSession userSession)
        {
            if (slotsResponse.IsAccessDeniedError)
            {
                _logger.LogError("Vision appointments disabled");
                _logger.LogVisionErrorResponse(slotsResponse);
                return new AppointmentSlotsResult.Forbidden();
            }

            if (slotsResponse.HasErrorResponse)
            {
                _logger.LogError($"Call to VISION ({nameof(VisionAppointmentSlotsService)}) returned an unanticipated " +
                                 $"error with status code: '{slotsResponse.StatusCode}'. \n{slotsResponse.ErrorForLogging}");
                _logger.LogVisionErrorResponse(slotsResponse);
                return new AppointmentSlotsResult.BadGateway();
            }

            try
            {
                var response = _mapper.Map(slotsResponse.Body, configResponse?.Body, userSession);

                if (response.Slots.Count == _settings.VisionAppointmentSlotsRequestCount)
                {
                    _logger.LogWarning($"Appointment slots retrieved for Vision patient is equal to the maximum requested ({_settings.VisionAppointmentSlotsRequestCount}).");
                }

                return new AppointmentSlotsResult.Success(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong during building the response.");
                return new AppointmentSlotsResult.InternalServerError();
            }
        }
    }
}
