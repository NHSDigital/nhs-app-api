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
    public class VisionAppointmentsRetrievalService
    {
        private readonly IVisionClient _visionClient;
        private readonly IBookedAppointmentsResponseMapper _responseMapper;
        private readonly ILogger<VisionAppointmentsRetrievalService> _logger;
        
        public VisionAppointmentsRetrievalService(
            ILogger<VisionAppointmentsRetrievalService> logger,
            IVisionClient visionClient,
            IBookedAppointmentsResponseMapper responseMapper
        )
        {
            _visionClient = visionClient;
            _responseMapper = responseMapper;
            _logger = logger;
        }
        
        public async Task<AppointmentsResult> GetAppointments(UserSession userSession)
        {
            try
            {
                _logger.LogEnter(nameof(GetAppointments));
            
                var visionUserSession = (VisionUserSession) userSession;

                if (!visionUserSession.IsAppointmentsEnabled)
                {
                    _logger.LogError("Appointments not enabled");
                    return new AppointmentsResult.CannotViewAppointments();
                }
                
                var response = await _visionClient.GetExistingAppointments(
                    visionUserSession
                    );
                return InterpretAppointmentsGetResponse(response);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "Calling GetExistingAppointments threw HttpRequestException.");
                return new AppointmentsResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit(nameof(GetAppointments));
            }
        }

        private AppointmentsResult InterpretAppointmentsGetResponse(
            VisionClient.VisionApiObjectResponse<BookedAppointmentsResponse> response)
        {
            if (response.HasErrorResponse)
            {
                _logger.LogError($"Call to VISION (VisionAppointmentsRetrievalService) returned an unanticipated error with status code: '{response.StatusCode}'. \n{response.ErrorContent}");
                return new AppointmentsResult.SupplierSystemUnavailable();
            }
            
            try
            {
                return new AppointmentsResult.SuccessfullyRetrieved(_responseMapper.Map(response.Body));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong during building the response.");
                return new AppointmentsResult.InternalServerError();
            }
        }
    }
}