using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.AppointmentSlots;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;
using NHSOnline.Backend.Worker.Router.Appointments;

namespace NHSOnline.Backend.Worker.Bridges.Emis
{
    public class EmisAppointmentSlotsService: IAppointmentSlotsService
    {
        private readonly IEmisClient _emisClient;
        private readonly IAppointmentSlotsResponseMapper _appointmentSlotsResponseMapper;
        private readonly ILogger<EmisAppointmentSlotsService> _logger;
        
        public EmisAppointmentSlotsService(IEmisClient emisClient,
            ILoggerFactory loggerFactory,
            IAppointmentSlotsResponseMapper appointmentSlotsResponseMapper)
        {
            _emisClient = emisClient;
            _logger = loggerFactory.CreateLogger<EmisAppointmentSlotsService>();
            _appointmentSlotsResponseMapper = appointmentSlotsResponseMapper;
        }
        
        public async Task<AppointmentSlotsResult> Get(UserSession userSession, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            var emisSserSession = (EmisUserSession) userSession;
            AppointmentSlotsMetadataGetResponse metaBody;
            AppointmentsSlotsGetResponse slotBody;
            
            try
            {
                var metaParams = new SlotsMetadataGetQueryParameters()
                {
                    SessionStartDate = fromDate,
                    SessionEndDate = toDate,
                    UserPatientLinkToken = emisSserSession.UserPatientLinkToken
                };

                var slotsParams = new SlotsGetQueryParameters()
                {
                    FromDateTime = fromDate,
                    ToDateTime = toDate,
                    UserPatientLinkToken = emisSserSession.UserPatientLinkToken
                };
                var headerParams = new EmisHeaderParameters()
                {
                    EndUserSessionId = emisSserSession.EndUserSessionId,
                    SessionId = emisSserSession.SessionId
                };

                var metaTask = _emisClient.AppointmentsSlotsMetadataGet(headerParams, metaParams);
                var slotTask = _emisClient.AppointmentsSlotsGet(headerParams, slotsParams);

                await Task.WhenAll(metaTask, slotTask);

                if (!metaTask.IsCompletedSuccessfully)
                {
                    _logger.LogError("Retrieving appointment slots metadata task completed unsuccessfully");
                    return new AppointmentSlotsResult.SupplierSystemUnavailable();
                }

                var metaResponse = metaTask.Result;

                if (!metaResponse.HasSuccessStatusCode)
                {
                    if (metaResponse.HasExceptionWithMessageContaining(EmisApiErrorMessages.Appointments_NotEnabledOnEmisForUser))
                    {
                        return new AppointmentSlotsResult.SuccessfullyRetrieved(new AppointmentSlotsResponse());
                    }
                    
                    _logger.LogError($"Retrieving appointment slots metadata failed with status code {metaResponse.StatusCode}");
                    return new AppointmentSlotsResult.SupplierSystemUnavailable();
                }

                if (!slotTask.IsCompletedSuccessfully)
                {
                    _logger.LogError("Retrieving appointment slots task completed unsuccessfully");
                    return new AppointmentSlotsResult.SupplierSystemUnavailable();
                }

                var slotResponse = slotTask.Result;

                if (!slotResponse.HasSuccessStatusCode)
                {
                    if (slotResponse.HasExceptionWithMessageContaining(EmisApiErrorMessages.Appointments_NotEnabledOnEmisForUser))
                    {
                        return new AppointmentSlotsResult.SuccessfullyRetrieved(new AppointmentSlotsResponse());
                    }
                    
                    _logger.LogError($"Retrieving appointment slots metadata failed with status code {slotResponse.StatusCode}");
                    return new AppointmentSlotsResult.SupplierSystemUnavailable();
                }

                metaBody = metaResponse.Body;
                slotBody = slotResponse.Body;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"HttpRequestException has been thrown with message: {e.Message}");
                return new AppointmentSlotsResult.SupplierSystemUnavailable();
            }

            try
            {
                return new AppointmentSlotsResult.SuccessfullyRetrieved(_appointmentSlotsResponseMapper.Map(slotBody, metaBody));
            }
            catch (Exception e)
            {
                _logger.LogError($"Something went wrong during buildin the response. Exception message: {e.Message}");
                return new AppointmentSlotsResult.InternalServerError();
            }
        }
    }
}
