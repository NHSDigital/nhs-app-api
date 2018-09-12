using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support.Logging;
using NHSOnline.Backend.Worker.GpSystems.Appointments;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
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

        public async Task<AppointmentSlotsResult> GetSlots(
            UserSession userSession, 
            AppointmentSlotsDateRange dateRange)
        {
            try
            {
                _logger.LogEnter(nameof(GetSlots));
            
                var emisUserSession = (EmisUserSession) userSession;
                var patientLinkToken = emisUserSession.UserPatientLinkToken;
                var metaParams = new SlotsMetadataGetQueryParameters(dateRange.FromDate, dateRange.ToDate,
                    patientLinkToken);
                var slotsParams = new SlotsGetQueryParameters(dateRange.FromDate, dateRange.ToDate, patientLinkToken);
                var headerParams = new EmisHeaderParameters(emisUserSession);

                _logger.LogInformation("Creating appointment slots requests");
                var metaTask = _emisClient.AppointmentSlotsMetadataGet(headerParams, metaParams);
                var slotTask = _emisClient.AppointmentSlotsGet(headerParams, slotsParams);
                var practiceTask = _emisClient.PracticeSettingsGet(headerParams, emisUserSession.OdsCode);

                await Task.WhenAll(metaTask, slotTask);
                _logger.LogInformation("Appointment slot requests completed");

                // Wait for practice task to complete, but unlike the other tasks supress any errors such as timeout.
                try
                {
                    await Task.WhenAll(practiceTask);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception has been thrown calling emis practice details.");
                }

                var result =
                    new EmisAppointmentSlotsResultBuilder(_logger, _appointmentSlotsResponseMapper, metaTask, slotTask, practiceTask)
                        .Build();

                return result.ValueOrFailure();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "HttpRequestException has been thrown.");
                return new AppointmentSlotsResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit(nameof(GetSlots));
            }
        }
    }
}
