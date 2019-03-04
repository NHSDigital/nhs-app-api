using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.SharedModels;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Appointments
{
    public class VisionAppointmentsRetrievalService
    {
        private readonly IVisionClient _visionClient;
        private readonly IBookedAppointmentsResponseMapper _responseMapper;
        private readonly ILogger<VisionAppointmentsRetrievalService> _logger;
        private readonly ISessionCacheService _sessionCacheService;

        public VisionAppointmentsRetrievalService(
            ILogger<VisionAppointmentsRetrievalService> logger,
            IVisionClient visionClient,
            IBookedAppointmentsResponseMapper responseMapper,
            ISessionCacheService sessionCacheService
        )
        {
            _visionClient = visionClient;
            _responseMapper = responseMapper;
            _logger = logger;
            _sessionCacheService = sessionCacheService;
        }
        
        public async Task<AppointmentsResult> GetAppointments(GpUserSession gpUserSession)
        {
            try
            {
                _logger.LogEnter();
            
                var visionUserSession = (VisionUserSession)gpUserSession;

                if (!visionUserSession.IsAppointmentsEnabled)
                {
                    _logger.LogError("Appointments not enabled");
                    return new AppointmentsResult.CannotViewAppointments();
                }
                
                var response = await _visionClient.GetExistingAppointments(
                    visionUserSession
                    );
                return InterpretAppointmentsGetResponse(response, gpUserSession);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, $"Calling {nameof(_visionClient.GetExistingAppointments)} threw HttpRequestException.");
                return new AppointmentsResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private AppointmentsResult InterpretAppointmentsGetResponse(
            VisionPFSClient.VisionApiObjectResponse<BookedAppointmentsResponse> response,
            GpUserSession gpUserSession)
        {
            if (response.IsAccessDeniedError)
            {
                _logger.LogError("Vision appointments not enabled");
                _logger.LogVisionErrorResponse(response);
                return new AppointmentsResult.CannotViewAppointments();
            }

            if (response.UnparsableResultMessage != null)
            {
                return new AppointmentsResult.InternalServerError();
            }
            
            if (!response.HasSuccessResponse)
            {
                _logger.LogError($"Call to VISION ({nameof(VisionAppointmentsRetrievalService)}) returned an unanticipated error " +
                                 $"with status code: '{response.StatusCode}'. \n{response.ErrorForLogging}");
                _logger.LogVisionErrorResponse(response);
                return new AppointmentsResult.SupplierSystemUnavailable();
            }
            
            try
            {
                var visionUserSession = (VisionUserSession)gpUserSession;
                UpdateUserSessionBookingReasonNecessity(visionUserSession, response);

                var result = new AppointmentsResult.SuccessfullyRetrieved(_responseMapper.Map(response.Body), visionUserSession.AppointmentBookingReasonNecessity);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong during building the response.");
                return new AppointmentsResult.InternalServerError();
            }
        }

        private static void UpdateUserSessionBookingReasonNecessity(VisionUserSession visionUserSession,
            VisionPFSClient.VisionApiObjectResponse<BookedAppointmentsResponse> response)
        {
            visionUserSession.AppointmentBookingReasonNecessity = response.Body.Appointments.Settings.BookingReason.Add
                ? Necessity.Optional
                : Necessity.NotAllowed;
        }
    }
}
