using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class EmisAppointmentSlotsService : IAppointmentSlotsService
    {
        private readonly IEmisClient _emisClient;
        private readonly ILogger<EmisAppointmentSlotsService> _logger;
        private readonly IAppointmentSlotsResponseMapper _appointmentSlotsResponseMapper;

        public EmisAppointmentSlotsService(
            IEmisClient emisClient,
            ILogger<EmisAppointmentSlotsService> logger,
            IAppointmentSlotsResponseMapper appointmentSlotsResponseMapper)
        {
            _emisClient = emisClient;
            _logger = logger;
            _appointmentSlotsResponseMapper = appointmentSlotsResponseMapper;
        }

        public async Task<AppointmentSlotsResult> Get(
            UserSession userSession, 
            DateTimeOffset fromDate,
            DateTimeOffset toDate)
        {
            var emisUserSession = (EmisUserSession) userSession;

            try
            {
                var patientLinkToken = emisUserSession.UserPatientLinkToken;
                var metaParams = new SlotsMetadataGetQueryParameters(fromDate, toDate, patientLinkToken);
                var slotsParams = new SlotsGetQueryParameters(fromDate, toDate, patientLinkToken);
                var headerParams = new EmisHeaderParameters(emisUserSession);

                var metaTask = _emisClient.AppointmentSlotsMetadataGet(headerParams, metaParams);
                var slotTask = _emisClient.AppointmentSlotsGet(headerParams, slotsParams);

                await Task.WhenAll(metaTask, slotTask);

                var result =
                    new EmisAppointmentSlotsResultBuilder(_logger, _appointmentSlotsResponseMapper, metaTask, slotTask)
                        .Build();

                return result.ValueOrFailure();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "HttpRequestException has been thrown.");
                return new AppointmentSlotsResult.SupplierSystemUnavailable();
            }
        }
    }
}
