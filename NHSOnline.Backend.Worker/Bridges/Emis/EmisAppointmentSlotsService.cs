using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.Appointments;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;
using NHSOnline.Backend.Worker.Date;
using NHSOnline.Backend.Worker.Router.Appointment;
using NHSOnline.Backend.Worker.Session;

namespace NHSOnline.Backend.Worker.Bridges.Emis
{
    public class EmisAppointmentSlotsService: IAppointmentSlotsService
    {
        private readonly EmisUserSession _userSession;
        private readonly IEmisClient _emisClient;
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly ILogger<EmisAppointmentSlotsService> _logger;
        
        public EmisAppointmentSlotsService(UserSession userSession, IEmisClient emisClient,
            ILoggerFactory loggerFactory,
            IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _userSession = (EmisUserSession) userSession;
            _emisClient = emisClient;
            _logger = loggerFactory.CreateLogger<EmisAppointmentSlotsService>();
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }
        
        public async Task<AppointmentSlotsResult> Get(DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            AppointmentSlotsMetadataGetResponse metaBody;
            AppointmentsSlotsGetResponse slotBody;
            
            try
            {
                var metaParams = new SlotsMetadataGetQueryParameters()
                {
                    SessionStartDate = fromDate,
                    SessionEndDate = toDate,
                    UserPatientLinkToken = _userSession.UserPatientLinkToken
                };

                var slotsParams = new SlotsGetQueryParameters()
                {
                    FromDateTime = fromDate,
                    ToDateTime = toDate,
                    UserPatientLinkToken = _userSession.UserPatientLinkToken
                };
                var headerParams = new EmisHeaderParameters()
                {
                    EndUserSessionId = _userSession.EndUserSessionId,
                    SessionId = _userSession.SessionId
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
            catch (Exception)
            {
                return new AppointmentSlotsResult.BadRequest();
            }

            try
            {
                return new AppointmentSlotsResult.SuccessfullyRetrieved(BuildResponse(slotBody, metaBody));
            }
            catch (Exception e)
            {
                _logger.LogError($"Something went wrong during buildin the response. Exception message: {e.Message}");
                return new AppointmentSlotsResult.InternalServerError();
            }
        }

        private AppointmentSlotsResponse BuildResponse(AppointmentsSlotsGetResponse slotsResponse, AppointmentSlotsMetadataGetResponse slotsMetadataResponse)
        {
            var response = new AppointmentSlotsResponse
            {
                Slots = new AppointmentSlotMapper(_dateTimeOffsetProvider).Map(slotsResponse, slotsMetadataResponse),
                Locations = new AppointmentLocationMapper().Map(slotsMetadataResponse),
                Clinicians = new AppointmentClinicianMapper().Map(slotsMetadataResponse),
                AppointmentSessions = new AppointmentSessionMapper().Map(slotsMetadataResponse),
            };

            return response;
        }

    }
}
