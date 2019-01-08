using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments
{
    public class VisionAppointmentSlotsService: IAppointmentSlotsService
    {
        private readonly IVisionClient _visionClient;
        private readonly ILogger<VisionAppointmentSlotsService> _logger;
        private readonly IAvailableAppointmentsResponseMapper _mapper;
        
        public VisionAppointmentSlotsService(
            IVisionClient visionClient, 
            ILogger<VisionAppointmentSlotsService> logger,
            IAvailableAppointmentsResponseMapper mapper)
        {
            _visionClient = visionClient;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<AppointmentSlotsResult> GetSlots(UserSession userSession, AppointmentSlotsDateRange dateRange)
        {          
            try
            {
                _logger.LogEnter();

                var visionUserSession = (VisionUserSession) userSession.GpUserSession;
                
                if (!visionUserSession.IsAppointmentsEnabled)
                {
                    _logger.LogError("Appointments not enabled");
                    return new AppointmentSlotsResult.CannotBookAppointments();
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
                return new AppointmentSlotsResult.SupplierSystemUnavailable();
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
                return new AppointmentSlotsResult.CannotBookAppointments();
            }
            
            if (slotsResponse.HasErrorResponse)
            {
                _logger.LogError($"Call to VISION ({nameof(VisionAppointmentSlotsService)}) returned an unanticipated " +
                                 $"error with status code: '{slotsResponse.StatusCode}'. \n{slotsResponse.ErrorForLogging}");
                return new AppointmentSlotsResult.SupplierSystemUnavailable();
            }
            
            try
            {
                return new AppointmentSlotsResult.SuccessfullyRetrieved(_mapper.Map(slotsResponse.Body,
                    configResponse?.Body,
                    userSession));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong during building the response.");
                return new AppointmentSlotsResult.InternalServerError();
            }
        }
    }
}