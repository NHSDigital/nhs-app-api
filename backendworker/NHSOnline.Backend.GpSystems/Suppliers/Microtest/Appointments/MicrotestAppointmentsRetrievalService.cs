using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public class MicrotestAppointmentsRetrievalService
    {
        private readonly IMicrotestClient _microtestClient;
        private readonly ILogger<MicrotestAppointmentsRetrievalService> _logger;
        private readonly IAppointmentsResponseMapper _responseMapper;

        public MicrotestAppointmentsRetrievalService(
            ILogger<MicrotestAppointmentsRetrievalService> logger,
            IMicrotestClient microtestClient,
            IAppointmentsResponseMapper responseMapper)
        {
            _logger = logger;
            _microtestClient = microtestClient;
            _responseMapper = responseMapper;
        }

        public async Task<AppointmentsResult> GetAppointments(GpUserSession gpUserSession)
        {
            try
            {
                _logger.LogEnter();

                var microtestUserSession = (MicrotestUserSession)gpUserSession;

                _logger.LogInformation("Appointments GET request starting");

                var response = await _microtestClient.AppointmentsGet(microtestUserSession.OdsCode, microtestUserSession.NhsNumber);

                _logger.LogInformation("Appointments GET request complete");

                try
                {
                    if (!response.StatusCode.IsSuccessStatusCode())
                    {
                        return new AppointmentsResult.SupplierSystemUnavailable();
                    }

                    return new AppointmentsResult.SuccessfullyRetrieved(_responseMapper.Map(response.Body));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error mapping retrieved appointments");
                    return new AppointmentsResult.InternalServerError();
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Appointments retrieval failed.");
                return new AppointmentsResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}