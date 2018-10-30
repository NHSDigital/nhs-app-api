using System;
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
            _logger.LogEnter(nameof(GetSlots));
            
            try
            {
                _logger.LogEnter(nameof(GetSlots));
            
                var visionUserSession = (VisionUserSession)userSession;
                var visionConnectionToken = new VisionConnectionToken
                {
                    RosuAccountId = visionUserSession.RosuAccountId,
                    ApiKey = visionUserSession.ApiKey
                };

                var response = await _visionClient.GetAvailableAppointments(
                    visionConnectionToken,
                    visionUserSession.OdsCode,
                    visionUserSession.PatientId,
                    dateRange
                    );

                return InterpretAppointmentsGetResponse(response);

            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Calling GetAvailableAppointments threw HttpRequestException.");
                return new AppointmentSlotsResult.SupplierSystemUnavailable();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong during retrieving the response.");
                return new AppointmentSlotsResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit(nameof(GetSlots));
            }
        }
        
        private AppointmentSlotsResult InterpretAppointmentsGetResponse(
            VisionClient.VisionApiObjectResponse<AvailableAppointmentsResponse> response)
        {
            if (response.HasErrorResponse)
            {
                _logger.LogError($"Call to VISION returned an unanticipated error with status code: '{response.StatusCode}'.");
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