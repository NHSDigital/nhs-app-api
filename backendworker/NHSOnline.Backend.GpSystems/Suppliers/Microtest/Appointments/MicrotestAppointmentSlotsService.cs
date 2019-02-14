using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public class MicrotestAppointmentSlotsService : IAppointmentSlotsService
    {
        private readonly IMicrotestClient _microtestClient;
        private readonly ILogger<MicrotestAppointmentSlotsService> _logger;
        private readonly IAppointmentSlotsResponseMapper _appointmentSlotsResponseMapper;

        public MicrotestAppointmentSlotsService(
            IMicrotestClient microtestClient,
            ILogger<MicrotestAppointmentSlotsService> logger,
            IAppointmentSlotsResponseMapper appointmentSlotsResponseMapper)
        {
            _microtestClient = microtestClient;
            _logger = logger;
            _appointmentSlotsResponseMapper = appointmentSlotsResponseMapper;
        }

        public async Task<AppointmentSlotsResult> GetSlots(
            GpUserSession gpUserSession, 
            AppointmentSlotsDateRange dateRange)
        {
            try
            {
                _logger.LogEnter();

                var microtestUserSession = (MicrotestUserSession) gpUserSession;
                
                _logger.LogInformation("Appointment slots request starting");

                var appointmentSlotsResponse = await _microtestClient.AppointmentSlotsGet(microtestUserSession.OdsCode, microtestUserSession.NhsNumber, dateRange);

                _logger.LogInformation("Appointment slots request complete");

                try
                {
                    var result = _appointmentSlotsResponseMapper.Map(appointmentSlotsResponse.Body);
                    if (!appointmentSlotsResponse.StatusCode.IsSuccessStatusCode())
                    {
                        return new AppointmentSlotsResult.SupplierSystemUnavailable();
                    }

                    return new AppointmentSlotsResult.SuccessfullyRetrieved(result);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error mapping appointment slots");
                    return new AppointmentSlotsResult.InternalServerError();
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "HttpRequestException has been thrown.");
                return new AppointmentSlotsResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
