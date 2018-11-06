using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
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
                _logger.LogEnter(nameof(GetSlots));
            
                var visionUserSession = (VisionUserSession)userSession;
                
                if (!visionUserSession.IsAppointmentsEnabled)
                {
                    _logger.LogError("Appointments not enabled");
                    return new AppointmentSlotsResult.CannotBookAppointments();
                }

                var response = await _visionClient.GetAvailableAppointments(
                    visionUserSession,
                    dateRange
                    );

                return InterpretAppointmentsGetResponse(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Calling GetAvailableAppointments threw HttpRequestException.");
                return new AppointmentSlotsResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit(nameof(GetSlots));
            }
        }
        
        private AppointmentSlotsResult InterpretAppointmentsGetResponse(
            VisionClient.VisionApiObjectResponse<AvailableAppointmentsResponse> response)
        {
            if (response.IsAccessDeniedError)
            {
                return new AppointmentSlotsResult.CannotBookAppointments();
            }
            
            if (response.HasErrorResponse)
            {
                _logger.LogError($"Call to VISION (VisionAppointmentSlotsService) returned an unanticipated error with status code: '{response.StatusCode}'. \n{response.ErrorContent}");
                return new AppointmentSlotsResult.SupplierSystemUnavailable();
            }
            
            try
            {
                return new AppointmentSlotsResult.SuccessfullyRetrieved(_mapper.Map(response.Body));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong during building the response.");
                return new AppointmentSlotsResult.InternalServerError();
            }
        }
    }
}