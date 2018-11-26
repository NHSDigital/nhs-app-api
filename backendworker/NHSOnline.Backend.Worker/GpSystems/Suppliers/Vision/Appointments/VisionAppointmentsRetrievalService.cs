using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.SharedModels;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Worker.Support.Logging;
using NHSOnline.Backend.Worker.Support.Session;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments
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
        
        public async Task<AppointmentsResult> GetAppointments(UserSession userSession)
        {
            try
            {
                _logger.LogEnter();
            
                var visionUserSession = (VisionUserSession) userSession;

                if (!visionUserSession.IsAppointmentsEnabled)
                {
                    _logger.LogError("Appointments not enabled");
                    return new AppointmentsResult.CannotViewAppointments();
                }
                
                var response = await _visionClient.GetExistingAppointments(
                    visionUserSession
                    );
                return await InterpretAppointmentsGetResponse(response, visionUserSession);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "Calling GetExistingAppointments threw HttpRequestException.");
                return new AppointmentsResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<AppointmentsResult> InterpretAppointmentsGetResponse(
            VisionClient.VisionApiObjectResponse<BookedAppointmentsResponse> response,
            VisionUserSession userSession)
        {
            if (response.IsAccessDeniedError)
            {
                return new AppointmentsResult.CannotViewAppointments();
            }
            
            if (!response.HasSuccessResponse)
            {
                _logger.LogError("Call to VISION (VisionAppointmentsRetrievalService) returned an unanticipated error " +
                                 $"with status code: '{response.StatusCode}'. \n{response.ErrorForLogging}");
                return new AppointmentsResult.SupplierSystemUnavailable();
            }
            
            try
            {
                var updateUserSessionTask = UpdateUserSessionBookingReasonNecessity(userSession, response);

                var result = new AppointmentsResult.SuccessfullyRetrieved(_responseMapper.Map(response.Body));

                await updateUserSessionTask;
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong during building the response.");
                return new AppointmentsResult.InternalServerError();
            }
        }

        private async Task UpdateUserSessionBookingReasonNecessity(VisionUserSession userSession,
            VisionClient.VisionApiObjectResponse<BookedAppointmentsResponse> response)
        {
            userSession.AppointmentBookingReasonNecessity = response.Body.Appointments.Settings.BookingReason.Add
                ? Necessity.Optional
                : Necessity.NotAllowed;
            await _sessionCacheService.UpdateUserSession(userSession);
        }
    }
}